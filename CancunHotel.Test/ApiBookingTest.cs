using CancunHotel.Api.Controllers;
using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Rooms;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CancunHotel.Test
{
    [TestClass]
    public class ApiBookingTest
    {
        private HttpClient _httpClient;

        public ApiBookingTest()
        {
            var webAppFactory = new WebApplicationFactory<BookingsController>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [TestMethod]
        public async Task GetBookingByIdBooking10Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByIdBooking?IdBooking=10");
            var booking = await response.Content.ReadFromJsonAsync<Booking>();

            Assert.AreNotEqual(null, booking);
        }

        [TestMethod]
        public async Task GetBookingByIdBookingMinus1Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByIdBooking?IdBooking=-1");
            var status = response.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, status);
        }

        [TestMethod]
        public async Task GetBookingByIdBookingMMMTest()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByIdBooking?IdBooking=MMM");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookingByIdBooking98765Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByIdBooking?IdBooking=98765");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookingByGuestIdentification81720127Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByGuestIdentification?IdType=1&Identification=81720127");
            var bookings = await response.Content.ReadFromJsonAsync<List<Booking>>();

            Assert.AreNotEqual(0, bookings?.Count);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookingByGuestIdentification99999999Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByGuestIdentification?IdType=1&Identification=99999999");
            var status = response.StatusCode;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookingByGuestIdentificationMinus1Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByGuestIdentification?IdType=1&Identification=-1");
            var status = response.StatusCode;

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task GetBookingByGuestIdentificationMMMTest()
        {
            var response = await _httpClient.GetAsync("api/Bookings/ByGuestIdentification?IdType=MMM&Identification=99999999");
            var status = response.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetLogBooking10Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/Logs?IdBooking=10");
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingLog>>();

            Assert.AreNotEqual(0, bookings?.Count);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetLogBookingMMMTest()
        {
            var response = await _httpClient.GetAsync("api/Bookings/Logs?IdBooking=MM");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetLogBooking98765Test()
        {
            var response = await _httpClient.GetAsync("api/Bookings/Logs?IdBooking=98765");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}