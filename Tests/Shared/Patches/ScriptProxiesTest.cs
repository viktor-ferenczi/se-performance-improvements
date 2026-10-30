using System;
using System.Collections.Generic;
using Fake.Game;
using Fake.Ingame;
using Shared.Patches;
using Xunit;

namespace Fake.Ingame
{
    // Stand-ins for the game's ingame interfaces
    public interface IBlock
    {
        string Name { get; set; }
        IGrid Grid { get; }
        bool TryGetNeighbor(out IBlock neighbor);
    }

    public interface ICounter : IBlock
    {
        int Count { get; }
    }

    public interface IProperty<T>
    {
        // Like ITerminalProperty<T>.GetValue: the game casts the block to its own class
        T GetValue(IBlock block);
    }

    public interface IGrid
    {
        void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null)
            where T : class;

        IBlock Find(string name);
        IProperty<int> CountProperty { get; }
    }
}

namespace Fake.Game
{
    // Stand-ins for game classes; their own interface is not a script interface
    public interface IInternal
    {
        void Secret();
    }

    public class Block : ICounter, IInternal
    {
        public string Name { get; set; }
        public IGrid Grid { get; set; }
        public int Count { get; set; }
        public Block Neighbor;

        public bool TryGetNeighbor(out IBlock neighbor)
        {
            neighbor = Neighbor;
            return neighbor != null;
        }

        public void Secret() { }

        public override string ToString() => $"Block {Name}";
    }

    public class CountProperty : IProperty<int>
    {
        public int GetValue(IBlock block) => ((Block)block).Count;
    }

    public class Grid : IGrid
    {
        public readonly List<Block> Blocks = new();

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null)
            where T : class
        {
            blocks.Clear();
            foreach (var block in Blocks)
                if (block is T t && (collect == null || collect(t)))
                    blocks.Add(t);
        }

        public IBlock Find(string name) => Blocks.Find(b => b.Name == name);

        public IProperty<int> CountProperty { get; } = new CountProperty();
    }
}

namespace Tests.Shared.Patches
{
    public class ScriptProxiesTest
    {
        private readonly Grid grid = new();
        private readonly Block a,
            b;

        public ScriptProxiesTest()
        {
            a = new Block
            {
                Name = "a",
                Count = 1,
                Grid = grid,
            };
            b = new Block
            {
                Name = "b",
                Count = 0,
                Grid = grid,
                Neighbor = a,
            };
            grid.Blocks.Add(a);
            grid.Blocks.Add(b);
        }

        [Fact]
        public void ProxyLooksLikeTheObject()
        {
            var proxy = ScriptProxies.Wrap(a);
            Assert.NotSame(a, proxy);
            Assert.IsAssignableFrom<ICounter>(proxy);
            Assert.False(proxy is IInternal);
            Assert.Equal("Fake.Game.Block", proxy.GetType().FullName);
            Assert.Equal("Block", proxy.GetType().Name);
            Assert.Equal("Block a", proxy.ToString());
            Assert.Same(proxy, ScriptProxies.Wrap(a));
            Assert.Same(a, ScriptProxies.Unwrap(proxy));
        }

        [Fact]
        public void CallsForwardAndConvert()
        {
            var block = (ICounter)ScriptProxies.Wrap(b);
            block.Name = "renamed";
            Assert.Equal("renamed", b.Name);

            var g = block.Grid;
            Assert.Same(ScriptProxies.Wrap(grid), g);
            Assert.Same(ScriptProxies.Wrap(a), g.Find("a"));

            Assert.True(block.TryGetNeighbor(out var neighbor));
            Assert.Same(ScriptProxies.Wrap(a), neighbor);

            // The game's property casts the block it gets to Block: it must get the object
            Assert.Equal(1, g.CountProperty.GetValue(g.Find("a")));
        }

        [Fact]
        public void GenericListAndCollect()
        {
            var g = (IGrid)ScriptProxies.Wrap(grid);
            var seen = new List<ICounter>();
            var blocks = new List<ICounter>();
            g.GetBlocksOfType(
                blocks,
                x =>
                {
                    seen.Add(x);
                    return x.Count > 0;
                }
            );
            Assert.Equal(
                new object[] { ScriptProxies.Wrap(a), ScriptProxies.Wrap(b) },
                seen.ToArray()
            );
            Assert.Equal(new object[] { ScriptProxies.Wrap(a) }, blocks.ToArray());

            // Keys and == work as with the objects
            var counts = new Dictionary<IBlock, int> { [blocks[0]] = 5 };
            Assert.Equal(5, counts[g.Find("a")]);
            Assert.True(blocks[0] == g.Find("a"));
        }
    }
}
