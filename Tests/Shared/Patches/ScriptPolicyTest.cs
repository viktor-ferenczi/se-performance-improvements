using System;
using System.IO;
using System.Linq;
using Shared.Patches;
using Xunit;

namespace Policy.Ingame
{
    public interface ILamp
    {
        bool Enabled { get; set; }
        float Radius { get; set; }
        string Name { get; }
        void Blink(int times);
        bool TryRepair();
        int Reset();
        ILamp Neighbor { get; set; }
        string Describe(int detail);
        int Color { get; }
    }

    public class Lamp : ILamp
    {
        public bool Enabled { get; set; }
        public float Radius { get; set; }
        public string Name => "lamp";
        public int Blinks;
        public ILamp Neighbor { get; set; }

        public void Blink(int times) => Blinks += times;

        public int Repairs;

        public bool TryRepair() => ++Repairs > 0;

        public int Reset() => 0;

        public string Describe(int detail) => "game";

        public int Color => 1;
    }
}

namespace Tests.Shared.Patches
{
    using Policy.Ingame;

    public class ScriptPolicyTest
    {
        private const string Table =
            "Policy.Ingame.ILamp.Enabled\tpass\tqueue\tnote\n"
            + "Policy.Ingame.ILamp.Radius\tpass\tmain\n"
            + "Policy.Ingame.ILamp.Name\tpass\t-\n"
            + "Policy.Ingame.ILamp.Blink\tqueue\t-\n"
            + "Policy.Ingame.ILamp.TryRepair\tqueue\t-\treturns true until replayed\n"
            + "Policy.Ingame.ILamp.Reset\tqueue\t-\treturns a value, so main\n"
            + "Policy.Ingame.ILamp.Neighbor\tpass\tqueue\n"
            + "Policy.Ingame.ILamp.Describe\tstandin@Sprites\t-\n"
            + "Policy.Ingame.ILamp.Color\tpass@Terminal\t-\n";

        // Off the main thread the proxy calls this instead of Lamp.Describe
        [StandInFor("Policy.Ingame.ILamp.Describe")]
        public static string Describe(ILamp lamp, int detail) => $"stand-in {detail} {lamp.Name}";

        static ScriptPolicyTest()
        {
            ScriptPolicy.Load(new StringReader(Table));
        }

        private static (Lamp, ILamp) Make()
        {
            var lamp = new Lamp();
            return (lamp, ScriptProxies.WrapAs<ILamp>(lamp));
        }

        [Fact]
        public void PoliciesComeFromTheTable()
        {
            var type = typeof(ILamp);
            Assert.Equal(
                MemberPolicy.Queue,
                ScriptPolicy.Of(type.GetProperty("Enabled").SetMethod)
            );
            Assert.Equal(MemberPolicy.Pass, ScriptPolicy.Of(type.GetProperty("Enabled").GetMethod));
            Assert.Equal(MemberPolicy.Main, ScriptPolicy.Of(type.GetProperty("Radius").SetMethod));
            Assert.Equal(MemberPolicy.Queue, ScriptPolicy.Of(type.GetMethod("Blink")));
            Assert.Equal(
                MemberPolicy.Main,
                ScriptPolicy.Of(typeof(IDisposable).GetMethod("Dispose"))
            );
        }

        [Fact]
        public void PoliciesNameTheMechanismTheyNeed()
        {
            var type = typeof(ILamp);
            Assert.Equal(MemberPolicy.Pass, ScriptPolicy.Of(type.GetProperty("Color").GetMethod));
            Assert.Equal(
                StandIn.Terminal,
                ScriptPolicy.NeedsOf(type.GetProperty("Color").GetMethod)
            );
            Assert.Equal(MemberPolicy.StandIn, ScriptProxies.PolicyOf(type.GetMethod("Describe")));
            Assert.Equal(StandIn.Sprites, ScriptPolicy.NeedsOf(type.GetMethod("Describe")));
            Assert.Equal(StandIn.None, ScriptPolicy.NeedsOf(type.GetMethod("Blink")));
        }

        [Fact]
        public void StandInsReplaceTheMemberOffMain()
        {
            var (_, proxy) = Make();
            Assert.Equal("game", proxy.Describe(2));
            var log = new ScriptWrites();
            log.Begin();
            try
            {
                Assert.Equal("stand-in 2 lamp", proxy.Describe(2));
            }
            finally
            {
                ScriptWrites.End();
            }
        }

        [Fact]
        public void OnTheMainThreadEverythingCallsTheGame()
        {
            var (lamp, proxy) = Make();
            proxy.Enabled = true;
            proxy.Radius = 2;
            proxy.Blink(3);

            Assert.True(lamp.Enabled);
            Assert.Equal(2, lamp.Radius);
            Assert.Equal(3, lamp.Blinks);
            Assert.True(proxy.TryRepair());
        }

        [Fact]
        public void WritesWaitForReplayAndReadBack()
        {
            var (lamp, proxy) = Make();
            var (other, otherProxy) = Make();
            var log = new ScriptWrites();
            log.Begin();
            try
            {
                proxy.Enabled = true;
                proxy.Blink(2);
                proxy.Blink(1);
                proxy.Neighbor = otherProxy;

                // Not applied yet, but the script reads back what it set
                Assert.False(lamp.Enabled);
                Assert.Equal(0, lamp.Blinks);
                Assert.True(proxy.Enabled);
                Assert.Same(otherProxy, proxy.Neighbor);
                Assert.Equal("lamp", proxy.Name);
                Assert.Equal(4, log.Count);
            }
            finally
            {
                ScriptWrites.End();
            }

            Assert.False(proxy.Enabled);
            log.Replay(e => throw e);
            Assert.True(lamp.Enabled);
            Assert.Equal(3, lamp.Blinks);
            // The game object, not the proxy, reaches the game
            Assert.Same(other, lamp.Neighbor);
            Assert.Equal(0, log.Count);
        }

        [Fact]
        public void QueuedBoolMethodsReturnTrue()
        {
            var (lamp, proxy) = Make();
            var log = new ScriptWrites();
            log.Begin();
            try
            {
                Assert.True(proxy.TryRepair());
            }
            finally
            {
                ScriptWrites.End();
            }

            Assert.Equal(0, lamp.Repairs);
            log.Replay(e => throw e);
            Assert.Equal(1, lamp.Repairs);
        }

        [Fact]
        public void TheGameTableIsWellFormed()
        {
            var lines = ScriptPolicyTable.Text.Split('\n').Where(l => l.Length != 0).ToList();
            Assert.True(lines.Count > 400);
            var policies = new[] { "pass", "queue", "standin", "main", "-" };
            var standIns = Enum.GetNames(typeof(StandIn));
            foreach (var line in lines)
            {
                var columns = line.Split('\t');
                Assert.True(columns.Length == 4, line);
                foreach (var column in columns.Skip(1).Take(2))
                {
                    var parts = column.Split('@');
                    Assert.Contains(parts[0], policies);
                    if (parts.Length > 1)
                        Assert.Contains(parts[1], standIns);
                    Assert.True(parts[0] != "standin" || parts.Length > 1, line);
                }
            }
            Assert.Single(
                lines
                    .Select(l => l.Split('\t')[0])
                    .Where(m => m == "Sandbox.ModAPI.Ingame.IMyTerminalBlock.CustomData")
            );
            Assert.Equal(lines.Count, lines.Select(l => l.Split('\t')[0]).Distinct().Count());
        }

        // Every member of the game's PB whitelist that the eligibility scan looks up has a row,
        // so the table decides it instead of the missing-means-main default
        [Fact]
        public void EveryWhitelistedMemberHasARow()
        {
            var rows = ScriptPolicyTable.Text.Split('\n').Select(l => l.Split('\t')[0]).ToHashSet();
            var missing = File.ReadAllLines(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Shared",
                        "Patches",
                        "ScriptWhitelist.txt"
                    )
                )
                .Where(m => m.Length != 0 && !rows.Contains(m))
                .ToList();
            Assert.True(
                missing.Count == 0,
                $"{missing.Count} members without a row: {string.Join(", ", missing.Take(20))}"
            );
        }

        [Fact]
        public void MainThreadMembersAreRefusedOffMain()
        {
            var (_, proxy) = Make();
            var log = new ScriptWrites();
            log.Begin();
            try
            {
                Assert.Throws<InvalidOperationException>(() => proxy.Radius = 1);
                Assert.Throws<InvalidOperationException>(() => proxy.Reset());
            }
            finally
            {
                ScriptWrites.End();
            }
        }

        private static void UsesTheLamp(ILamp lamp)
        {
            lamp.Enabled = !lamp.Enabled;
            lamp.Blink(1);
        }

        [Fact]
        public void TheScanFindsTheCalls()
        {
            var calls = ScriptEligibility
                .Calls(
                    typeof(ScriptPolicyTest).GetMethod(
                        nameof(UsesTheLamp),
                        System.Reflection.BindingFlags.NonPublic
                            | System.Reflection.BindingFlags.Static
                    )
                )
                .Select(m => m.Name)
                .ToArray();
            Assert.Equal(new[] { "get_Enabled", "set_Enabled", "Blink" }, calls);
        }
    }
}
