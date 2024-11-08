using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.AddressDto;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;
using System.Text;

namespace Payment.WebUI.Controllers
{
    public class AddressController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AddressController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> UserAddresses(int id, string message = null)
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
            var responseMessage = await client.GetAsync("https://localhost:7066/api/User/GetUserAddresses");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = await responseMessage.Content.ReadAsStringAsync();
                var userAddresses = JsonConvert.DeserializeObject<List<ResultAddressDto>>(result);

                ViewBag.AddressesId = userAddresses.Select(x => x.AddressID).ToList();

                if (message!=null)
                {
                    ViewBag.ErrorMessage = message;
                }

                return View(userAddresses);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateAddress()
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
            var responseMessage = await client.GetAsync("https://localhost:7066/api/User/GetUserID");
            if (responseMessage.IsSuccessStatusCode)
            {
                var userID = await responseMessage.Content.ReadFromJsonAsync<int>();
                ViewBag.UserID = userID;
                TempData["UserID"] = userID;
            }

            var filePath = Path.Combine(_env.WebRootPath, "Json", "il-ilce.json");
            var jsonData = System.IO.File.ReadAllText(filePath);

            ViewBag.JsonData = jsonData;
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

                var userID = TempData["UserID"];
            ViewBag.UserID = userID;
            createAddressDto.AppUserId = (int)userID;
            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var jsonData = JsonConvert.SerializeObject(createAddressDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7066/api/Address", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("UserAddresses");
            }
            var errorMessages = "Adres eklenemedi.";
            return RedirectToAction("UserAddresses", new { message = errorMessages });
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

                var filePath = Path.Combine(_env.WebRootPath, "Json", "il-ilce.json");
                var jsonDataCity = System.IO.File.ReadAllText(filePath);

                ViewBag.JsonData = jsonDataCity;
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
                return RedirectToAction("UserAddresses");
            }
            return View();
        }

        public async Task<IActionResult> DeleteAddress(int id)
        {
            ViewBag.AddressID = id;
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

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.DeleteAsync($"https://localhost:7066/api/Address/{id}");
            if (!responseMessage.IsSuccessStatusCode)
            {
                return View();
            }
            return RedirectToAction("UserAddresses");

        }
    }
}