using System.Diagnostics.CodeAnalysis;
using Sandbox.Game.World;

namespace Shared.Tools
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class MySessionExtensions
    {
        public static bool IsUpdateAllowed(this MySession self)
        {
            return self.m_updateAllowed;
        }

        public static void SetUpdateAllowed(this MySession self, bool value)
        {
            self.m_updateAllowed = value;
        }
    }
}
