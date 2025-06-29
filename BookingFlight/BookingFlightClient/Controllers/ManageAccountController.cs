using BookingFlightClient.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class ManageAccountController : Controller
    {
        private readonly HttpClient httpClient;

        public ManageAccountController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            var apiUrl = "http://localhost:5077/api/ManageAccount/get-all-account";
            var accountDTOs = await httpClient.GetFromJsonAsync<List<ResponseAccountDTO>>(apiUrl);
            return View(accountDTOs);
        }
    }
}
