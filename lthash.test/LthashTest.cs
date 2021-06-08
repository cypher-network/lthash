using FluentAssertions;
using Lthash;
using Xunit;

namespace lthash.test
{
    public class LthashTest
    {
        [Fact]
        public void Test1()
        {
            var ltHash = new Lthash32();
            
            ltHash.Add(System.Text.Encoding.UTF8.GetBytes("apple"), System.Text.Encoding.UTF8.GetBytes("orange"));
            var checksum = ltHash.GetChecksum();
            
            ltHash.Remove(System.Text.Encoding.UTF8.GetBytes("apple"));
            var isEqual = ltHash.ChecksumEquals(checksum);

            isEqual.Should().Be(false);
            
            ltHash.Update(System.Text.Encoding.UTF8.GetBytes("orange"), System.Text.Encoding.UTF8.GetBytes("apple"));
            isEqual = ltHash.ChecksumEquals(checksum);
            
            isEqual.Should().Be(false);
            
            ltHash.Add(System.Text.Encoding.UTF8.GetBytes("orange"));
            isEqual = ltHash.ChecksumEquals(checksum);
            
            isEqual.Should().Be(true);
        }
    }
}