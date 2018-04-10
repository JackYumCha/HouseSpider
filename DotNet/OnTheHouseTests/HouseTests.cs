using System;
using System.Diagnostics;   
using Xunit;
using OnTheHouse;

namespace OnTheHouseTests
{
    public class UnitTest1
    {
        [Theory(DisplayName = "Test Domain.")]
        [InlineData("21-baroda-street-coopers-plains-qld-4108")]
        [InlineData("221-baroda-street-coopers-plains-qld-4108")]
        public void TestDomain(string path)
        {
            var result = Domain.SearchDomain(path);

            Debugger.Break();

        }

        [Theory(DisplayName = "Test RealEstate.")]
        [InlineData("21-baroda-st-coopers-plains-qld-4108")]
        [InlineData("221-baroda-st-coopers-plains-qld-4108")]
        public void TestRealEstate(string path)
        {
            var result = RealEstate.SearchRealEstate(path);
            Debugger.Break();
        }

        [Theory(DisplayName = "Test Pair Search.")]
        [InlineData("{2424{42}vsass} {24}")]
        public void TestPairSearch(string value)
        {
            var results = value.PairMatchSearch("{", "}");
            Debugger.Break();
        }
    }
}
