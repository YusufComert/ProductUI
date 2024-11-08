using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;

namespace Payment.WebUI.ViewComponents.Dashboard
{
    public class _DashboardWidgetPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        public _DashboardWidgetPartial(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var logins = await _userManager.GetLoginsAsync(user);
            var externalLogin = logins.FirstOrDefault(x => x.LoginProvider == "Google" || x.LoginProvider == "Facebook");

            string provider = externalLogin != null ? externalLogin.LoginProvider : "Local";

            var model = new GetCheckAppUserViewModel
            {
                ID = user.Id.ToString(),
                Email = user.Email,
                Provider = provider,
                Role = userRoles.Count > 0 ? userRoles[0] : string.Empty,
                IsExist = true
            };

            var token = JwtTokenGenerator.GenerateToken(model);
            if (string.IsNullOrEmpty(token.Token))
            {
                ViewBag.ErrorMessage = "Token oluşturulamadı.";
                return View();
            }

            var client = _httpClientFactory.CreateClient("YourClientName");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync("https://localhost:7066/api/AdminDashboardWidgets/UserCount");

            if (!responseMessage.IsSuccessStatusCode)
            {
                return View();
            }

            var userCount = Convert.ToInt16(await responseMessage.Content.ReadAsStringAsync());
            ViewBag.UserCount = userCount;

            var responseMessage2 = await client.GetAsync("https://localhost:7066/api/AdminDashboardWidgets/ProductCount");
            if (!responseMessage2.IsSuccessStatusCode)
            {
                return View();
            }
            var productCount = Convert.ToInt16(await responseMessage2.Content.ReadAsStringAsync());
            ViewBag.ProductCount = productCount;

            var responseMessage3 = await client.GetAsync("https://localhost:7066/api/AdminDashboardWidgets/TotalProductValue");
            if (!responseMessage3.IsSuccessStatusCode)
            {
                return View();
            }
            var totalProductValue = await responseMessage3.Content.ReadAsStringAsync();
            var total=JsonConvert.DeserializeObject<decimal>(totalProductValue);
            ViewBag.TotalProductValue = total;

            return View();

        }
    }
}
