using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.AppUserDtos;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;
using System.Text;

namespace Payment.WebUI.Controllers
{

    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var errorMessage = string.Empty;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                errorMessage = "Kullanıcı oturumu açık değil, lütfen tekrar giriş yapın.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            ViewBag.UserID = user.Id;
            var logins = await _userManager.GetLoginsAsync(user);
            var externalLogin = logins.FirstOrDefault(x => x.LoginProvider == "Google" || x.LoginProvider == "Facebook");

            var userRoles = await _userManager.GetRolesAsync(user);
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

            var responseMessage = await client.GetAsync("https://localhost:7066/api/User/GetUserID");
            if (responseMessage.IsSuccessStatusCode)
            {
                var userID = await responseMessage.Content.ReadFromJsonAsync<int>();
                ViewBag.UserID = userID;
            }
            else
            {
                errorMessage = "User ID alınamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var responseMessage1 = await client.GetAsync("https://localhost:7066/api/User");
            if (responseMessage1.IsSuccessStatusCode)
            {
                var profile = await responseMessage1.Content.ReadFromJsonAsync<ResultAppUserDto>();
                return View(profile);
            }

            errorMessage = "Profil bilgileri alınamadı.";
            return RedirectToAction("Index", "Login", new { message = errorMessage });

        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı oturumu açık değil, lütfen tekrar giriş yapın.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            ViewBag.UserID = user.Id;
            var logins = await _userManager.GetLoginsAsync(user);
            var externalLogin = logins.FirstOrDefault(x => x.LoginProvider == "Google" || x.LoginProvider == "Facebook");

            var userRoles = await _userManager.GetRolesAsync(user);
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var responseMessage = await client.GetAsync("https://localhost:7066/api/User/GetUserID");
            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorMessage = "Kullanıcı ID'si alınamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });

            }
            var userID = await responseMessage.Content.ReadFromJsonAsync<int>();
            ViewBag.UserID = userID;


            var responseMessage1 = await client.GetAsync("https://localhost:7066/api/User");
            if (!responseMessage1.IsSuccessStatusCode)
            {
                var errorMessage = "Profil bilgileri alınamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var profile = await responseMessage1.Content.ReadFromJsonAsync<ResultAppUserDto>();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUserDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı oturumu açık değil, lütfen tekrar giriş yapın.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            ViewBag.UserID = user.Id;
            var logins = await _userManager.GetLoginsAsync(user);
            var externalLogin = logins.FirstOrDefault(x => x.LoginProvider == "Google" || x.LoginProvider == "Facebook");

            var userRoles = await _userManager.GetRolesAsync(user);
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }


            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var jsonData = JsonConvert.SerializeObject(updateUserDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("https://localhost:7066/api/User", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Profil güncellenemedi.";
            return View(updateUserDto);
        }


    }
}