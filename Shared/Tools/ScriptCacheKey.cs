using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Tools
{
    // Cache key of a compiled script assembly: one SHA1 over an ordered sequence of
    // length-prefixed strings, so duplicate parts don't cancel out and order counts.
    public static class ScriptCacheKey
    {
        // Bump when the key layout or the cache file format changes, so old cache files never match
        public const string Format = "ScriptCache/2";

        public static string Compute(IEnumerable<string> parts)
        {
            using (var buffer = new MemoryStream())
            {
                using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
                {
                    writer.Write(Format);
                    foreach (var part in parts)
                        writer.Write(part ?? "");
                }

                byte[] hash;
                using (var sha1 = SHA1.Create())
                    hash = sha1.ComputeHash(buffer.GetBuffer(), 0, (int)buffer.Length);

                var hex = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                    hex.Append(b.ToString("x2"));
                return hex.ToString();
            }
        }
    }
}
