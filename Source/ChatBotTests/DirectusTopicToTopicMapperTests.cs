﻿using ChatBot.Mappers;
using Infrastructure.Directus.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;

namespace ChatBotTests
{
    [TestFixture()]
    public class DirectusTopicToTopicMapperTests
    {
        [Test()]
        [TestCase("Russisch")]
        [TestCase("Ukrainisch")]
        public void When_DirecutsTopicIsMapped_Then_ReturnsSomeResults(string language)
        {
            //arrange
            var exampleResponse = File.ReadAllText("exampleResponse.json");
            var directusTopics = JsonConvert.DeserializeObject<DirectusTopicWrapper>(exampleResponse).Data;

            var sut = new DirectusTopicToTopicMapper(language);

            //act
            var result = sut.Map(directusTopics);

            //assert
            Assert.Greater(result.Count, 0);
        }
    }
}