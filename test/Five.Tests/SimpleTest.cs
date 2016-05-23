using Five.Models;
using System;
using Xunit;

namespace Five.Tests
{
    public class SimpleTest
    {
        public SimpleTest()
        {
        }


        [Fact]
        public void JustABlankTest() {
            Assert.True(true);
        }

        [Fact]
        public void JustARefToFive()
        {
            Blog blog = new Blog() {
                Name = "Whatever"
            };

            Assert.Equal("Whatever", blog.Name);
        }
    }
}
