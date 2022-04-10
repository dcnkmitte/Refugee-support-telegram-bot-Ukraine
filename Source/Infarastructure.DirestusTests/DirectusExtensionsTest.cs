using Infrastructure.Directus.Extensions;
using Infrastructure.Directus.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace Infarastructure.DirestusTests;
public class Tests
{


    [Test]
    [TestCase("Russisch",0)]
    [TestCase("Russisch",1)]
    [TestCase("Ukrainisch",0)]
    [TestCase("Ukrainisch",1)]
    public void When_ContentInPrefferedLanguageIsNotValid_Then_ReturnValidContentInOtherLanguage(string language, int index)
    {
        //arrange
        var exampleResponse = File.ReadAllText("exampleResponse.json");
        var directusTopics = JsonConvert.DeserializeObject<DirectusTopicWrapper>(exampleResponse).Data;
        var directusTopicContentArea = directusTopics[index].TopicContentArea;

        //act
        var result = directusTopicContentArea.GetTopicContentIdeallyInPreferredLanguage(language);

        //arrange
        StringAssert.AreEqualIgnoringCase("contentukr",result);
    }

    [Test]
    [TestCase(0, "2022-03-15T19:21:08")]
    [TestCase(1, "2022-04-05T15:40:05")]
    public void When_DateUpdatedIsMissing_Then_ReturnDateCreated(int index, string expectedDate)
    {
        //arrange
        var exampleResponse = File.ReadAllText("exampleResponse.json");
        var directusTopics = JsonConvert.DeserializeObject<DirectusTopicWrapper>(exampleResponse).Data;
        var directusTopic= directusTopics[index];

        //act
        var result = directusTopic.GetLastModifiedUtc();

        //arrange
        Assert.IsTrue(result == DateTime.Parse(expectedDate));
    }

    [Test]
    [TestCase("Russisch", 0,"contentukr")]
    [TestCase("Russisch", 1,"contentru")]
    [TestCase("Ukrainisch", 0,"contentukr")]
    [TestCase("Ukrainisch", 1,"contentukr")]
    public void When_AreaInPrefferedLanguageIsNotValid_Then_ReturnValidAreaInOtherLanguage(
        string language, int index, string expected)
    {
        //arrange
        var exampleResponse = File.ReadAllText("exampleResponse.json");
        var directusTopics = JsonConvert.DeserializeObject<DirectusTopicWrapper>(exampleResponse).Data;
        var directusTopicNameArea = directusTopics[index].TopicNameArea;

        //act
        var result = directusTopicNameArea.GetTopicNameIdeallyInPreferredLanguage(language);

        //arrange
        StringAssert.AreEqualIgnoringCase(expected, result);
    }

}