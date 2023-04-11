using CancunHotel.Api.Controllers;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;
using CancunHotel.Entities.Rooms;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace CancunHotel.Test
{
    [TestClass]
    public class ApiParameterTest
    {
        private HttpClient _httpClient;

        public ApiParameterTest()
        {
            var webAppFactory = new WebApplicationFactory<ParametersController>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [TestMethod]
        public async Task GetAllGendersTest()
        {
            var response = await _httpClient.GetAsync("api/Parameters/Genders");
            var genders = await response.Content.ReadFromJsonAsync<List<Gender>>();

            Assert.AreEqual(2, genders?.Count);
        }

        [TestMethod]
        public async Task GetAllIdTypesTest()
        {
            var response = await _httpClient.GetAsync("api/Parameters/IdTypes");
            var genders = await response.Content.ReadFromJsonAsync<List<IdType>>();

            Assert.AreEqual(2, genders?.Count);
        }

        [TestMethod]
        public async Task GetAllBookingStatusTest()
        {
            var response = await _httpClient.GetAsync("api/Parameters/BookingStates");
            var genders = await response.Content.ReadFromJsonAsync<List<BookingStatus>>();

            Assert.AreEqual(6, genders?.Count);
        }


    }
}