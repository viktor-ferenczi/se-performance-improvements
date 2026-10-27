using Shared.Tools;
using Xunit;

namespace Tests.Shared.Tools
{
    public class ScriptCacheKeyTest
    {
        private static string Key(params string[] parts) => ScriptCacheKey.Compute(parts);

        [Fact]
        public void DuplicatePartsDoNotCancelOut()
        {
            // A mod with files {A, A, B} must not get the key of {B}
            Assert.NotEqual(Key("a.cs", "A", "a.cs", "A", "b.cs", "B"), Key("b.cs", "B"));
        }

        [Fact]
        public void OrderCounts()
        {
            Assert.NotEqual(Key("a.cs", "A", "b.cs", "B"), Key("b.cs", "B", "a.cs", "A"));
        }

        [Fact]
        public void NamesCount()
        {
            Assert.NotEqual(Key("a.cs", "A"), Key("b.cs", "A"));
        }

        [Fact]
        public void PartBoundariesCount()
        {
            Assert.NotEqual(Key("ab", "c"), Key("a", "bc"));
        }

        [Fact]
        public void SameInputSameKey()
        {
            Assert.Equal(Key("a.cs", "A", null), Key("a.cs", "A", ""));
            Assert.Equal(40, Key("a.cs", "A").Length);
        }
    }
}
