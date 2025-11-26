using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VRage.Game;

namespace ClientPlugin.Shared.Tools
{
    public static class Workarounds
    {
        // See #81
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static MyDefinitionBase GetValueOrDefault(this Dictionary<MyDefinitionId, MyDefinitionBase> dict, MyDefinitionId key)
        {
            return dict.TryGetValue(key, out var value) ? value : null;
        }        
    }
}