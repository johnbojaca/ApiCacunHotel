using CancunHotel.Api.Controllers;
using CancunHotel.Entities.Rooms;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CancunHotel.Test
{
    [TestClass]
    public class ApiRoomTest
    {
        private HttpClient _httpClient;

        public ApiRoomTest()
        {
            var webAppFactory = new WebApplicationFactory<RoomsController>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [TestMethod]
        public async Task GetAllRoomsTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms");
            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();

            Assert.AreEqual(4, rooms?.Count);
        }

        [TestMethod]
        public async Task GetRoom1001Test()
        {
            var response = await _httpClient.GetAsync("api/Rooms?RoomNumber=1001");
            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();

            Assert.AreEqual(1, rooms?.Count);
        }

        [TestMethod]
        public async Task GetRoomMMMMTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms?RoomNumber=MMMM");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetRoomMMTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms?IsActive=MM");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetRoom3001Test()
        {
            var response = await _httpClient.GetAsync("api/Rooms?RoomNumber=3001");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task GetRoom1001AndInactiveTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms?RoomNumber=1001&IsActive=false");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task GetRoomInactiveTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms?IsActive=false");
            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();

            Assert.AreEqual(1, rooms?.Count);
        }

        [TestMethod]
        public async Task GetRoomActiveTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms?IsActive=true");
            var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();

            Assert.AreEqual(3, rooms?.Count);
        }

        [TestMethod]
        public async Task GetAvailableDays1001RoomActiveTest()
        {
            var response = await _httpClient.GetAsync("api/Rooms/AvailableDays?RoomNumber=1001");
            var dates = await response.Content.ReadFromJsonAsync<List<DateTime>>();

            Assert.AreNotEqual(0, dates?.Count);
        }

        [TestMethod]
        public async Task GetQuote1001RoomTest()
        {
            var startDate = DateTime.Now.AddDays(1).ToString("s");
            var finalDate = DateTime.Now.AddDays(2).ToString("s");
            var response = await _httpClient.GetAsync($"api/Rooms/Quote?RoomNumber=1001&StarteDate={startDate}&FinalDate={finalDate}&Adults=1&Childs=0");
            var quote = await response.Content.ReadAsStringAsync();

            decimal quoteDecimal = 0;
            decimal.TryParse(quote, out quoteDecimal);

            Assert.AreNotEqual(0, quoteDecimal);
        }

        [TestMethod]
        public async Task GetQuoteMMMMRoomTest()
        {
            var startDate = DateTime.Now.AddDays(1).ToString("s");
            var finalDate = DateTime.Now.AddDays(2).ToString("s");
            var response = await _httpClient.GetAsync($"api/Rooms/Quote?RoomNumber=MMMM&StarteDate={startDate}&FinalDate={finalDate}&Adults=1&Childs=0");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


    }
}