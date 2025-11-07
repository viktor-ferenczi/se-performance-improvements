using System.Collections.Generic;

namespace ClientPlugin.Shared.Tools
{
    public static class Workarounds
    {
        // See #81
        public static TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var value) ? value : default;
        }        
    }
}