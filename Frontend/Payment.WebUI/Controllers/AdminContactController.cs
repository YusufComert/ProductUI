using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.BusinessLayer.Abstract;
using Payment.DataAccessLayer.Concrete;
using Payment.WebUI.DTOs.CategoryDtos;
using Payment.WebUI.DTOs.ContactDtos;
using Payment.WebUI.DTOs.ProductDtos;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;

namespace Payment.WebUI.Controllers
{
    public class AdminContactController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly IContactService _contactService;

        public AdminContactController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager, IContactService contactService)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index()
        {
            var errorMessage = string.Empty;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }
            else
                TempData["Username"] = user.UserName;
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var externalLogin = logins.FirstOrDefault(x => x.LoginProvider == "Google" || x.LoginProvider == "Facebook");

            var model = new GetCheckAppUserViewModel
            {
                ID = user.Id.ToString(),
                Email = user.Email,
                Provider = externalLogin == null ? "Local" : externalLogin.LoginProvider,
                Role = userRoles.Count > 0 ? userRoles[0] : string.Empty,
                IsExist = true
            };
            var token = JwtTokenGenerator.GenerateToken(model);
            if (string.IsNullOrEmpty(token.Token))
            {
                errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync("https://localhost:7066/api/Contact");
            if (responseMessage.IsSuccessStatusCode)
            {
                var values = await responseMessage.Content.ReadFromJsonAsync<List<ResultContactDto>>();
                return View(values);
            }
            else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                errorMessage = "Yetkiniz yok. Lütfen yetkili bir kullanıcıyla giriş yapın.";
            }
            else
            {
                errorMessage = "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.";
            }
            return RedirectToAction("Index", "Login", new { message = errorMessage });
        }
    }
}