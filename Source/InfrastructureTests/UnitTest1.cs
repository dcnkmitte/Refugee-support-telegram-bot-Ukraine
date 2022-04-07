using Infrastructure.Extensions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace InfrastructureTests;
public class Tests
{
    private const string Pattern = "([0-9A-F]{2}-?){16}";

    [Test]
    public void Test1()
    {
        //arrange
        //act
        var someText = "some value";
        var hash = someText.GetMd5Hash();
        var result = Regex.IsMatch(hash, Pattern);

        //assert
        Assert.IsTrue(result);
    }
}