using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.AddressDto;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;
using System.Text;

namespace Payment.WebUI.Controllers
{
    public class AdminAddressController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        public AdminAddressController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                var errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }


            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync("https://localhost:7066/api/Address/AddressListWithUserName");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();
                var Addresss = JsonConvert.DeserializeObject<List<AddressWithUsernameDto>>(result);
                return View(Addresss);
            }
            return RedirectToAction("Index","Login");
        }

        [HttpGet]
        public IActionResult CreateAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressDto createAddressDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                var errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var client = _httpClientFactory.CreateClient("");
            var content = new StringContent(JsonConvert.SerializeObject(createAddressDto), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.PostAsync("https://localhost:7066/api/Address", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(createAddressDto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                var errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync($"https://localhost:7066/api/Address/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateAddressDto>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(UpdateAddressDto updateAddressDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                var errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var jsonData = JsonConvert.SerializeObject(updateAddressDto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("https://localhost:7066/api/Address", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> DeleteAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var errorMessage = "Kullanıcı bulunamadı";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                var errorMessage = "Kullanıcı giriş bilgileri alınamadı.";
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
                var errorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login", new { message = errorMessage });
            }


            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.DeleteAsync($"https://localhost:7066/api/Address/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

