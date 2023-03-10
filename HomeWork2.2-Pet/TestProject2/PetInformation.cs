using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HomeWork2._2_Pet
{
    [TestClass]
    public class PetInformation
    {
        private static RestClient petRestClient;

        private static readonly string petBaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string petEndpoint = "pet";
        private static string GetURL(string enpoint) => $"{petBaseURL}{enpoint}";
        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<petModels> petCleanUp = new List<petModels>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            petRestClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in petCleanUp)
            { 
                var restRequest = new RestRequest(GetURI($"{petEndpoint}/{data.Id}"));
                var restResponse = await petRestClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            //adding petCategory data
            Category petCategory = new Category(1, "House Pets");

            //adding PhotoURLS data
            List<string> PhotoUrls = new List<string>();
            PhotoUrls.Add("testPetUrls");
            PhotoUrls.Add("testPetUrls2");

            //adding tags data
            List<CategoryT> tags = new List<CategoryT>();
            tags.Add(new CategoryT(1, "chihuahua brown"));
            tags.Add(new CategoryT(2, "chihuahua white"));

            //object creation
            petModels petObjects = new petModels()
            {
                Id = 5001,
                Category = petCategory,
                Name = "Harvey",
                PhotoUrls = PhotoUrls,
                Tags = tags,
                Status = "available"
            };

            //Send Post Request
            var postRestRequest = new RestRequest(GetURI(petEndpoint)).AddJsonBody(petObjects);
            var postRestResponse = await petRestClient.ExecutePostAsync(postRestRequest);

            //AssertResponseCode
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status Code is not equal to 200");

            //Get Request
            var restRequest = new RestRequest(GetURI($"{petEndpoint}/{petObjects.Id}"), Method.Get);
            var restResponse = await petRestClient.ExecuteGetAsync<petModels>(restRequest);

            //Assert Fields
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status Code is not equal to 200");
            Assert.AreEqual(petObjects.Name, restResponse.Data.Name , "Pet Name doesn't match");
            Assert.AreEqual(petObjects.Category.Id, restResponse.Data.Category.Id, "Category ID doesn't match");
            Assert.AreEqual(petObjects.Category.Name, restResponse.Data.Category.Name, "Category Name doesn't match");
            Assert.AreEqual(petObjects.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoURLS doesn't match");
            Assert.AreEqual(petObjects.Tags[0].Id, restResponse.Data.Tags[0].Id, "Tags ID doesn't match");
            Assert.AreEqual(petObjects.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tag Name doesn't match");
            Assert.AreEqual(petObjects.Status, restResponse.Data.Status, "Status doesn't match");

            //Test cleanup
            petCleanUp.Add(petObjects);

        }

    }
}