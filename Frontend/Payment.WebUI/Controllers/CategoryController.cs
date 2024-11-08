using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.CategoryDtos; 
using Payment.WebUI.Helpers;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;
using System.Text;

namespace Payment.WebUI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;

        public CategoryController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
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
            var responseMessage = await client.GetAsync("https://localhost:7066/api/Category");
            if (responseMessage.IsSuccessStatusCode)
            {
                var values = await responseMessage.Content.ReadFromJsonAsync<List<ResultCategoryDto>>();
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
            return RedirectToAction("Index", "Login", new {message=errorMessage});
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createCategoryDto);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            createCategoryDto.CreateTime = DateTime.Parse(DateTime.Now.ToShortDateString());
            createCategoryDto.UpdateTime = DateTime.Parse(DateTime.Now.ToShortDateString());
            createCategoryDto.CreateUser = user.Name;
            createCategoryDto.UpdateUser = user.Name;

            if (createCategoryDto.File != null)
            {
                createCategoryDto.ImagePath = ImageHelper.SaveImage(createCategoryDto.File);
            }

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createCategoryDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.PostAsync("https://localhost:7066/api/Category", content);
            if (!responseMessage.IsSuccessStatusCode)
            {
                return View(createCategoryDto);
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync($"https://localhost:7066/api/Category/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var values = await responseMessage.Content.ReadFromJsonAsync<UpdateCategoryDto>();
                return View(values);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCategoryDto);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            updateCategoryDto.UpdateTime = DateTime.Parse(DateTime.Now.ToShortDateString());
            updateCategoryDto.UpdateUser = user.Name;
            updateCategoryDto.CreateUser = user.Name;

            if (updateCategoryDto.File != null && updateCategoryDto.File.Length > 0)
            {
                ImageHelper.DeleteImage(updateCategoryDto.ImagePath);
                updateCategoryDto.ImagePath = ImageHelper.SaveImage(updateCategoryDto.File);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var jsonData = JsonConvert.SerializeObject(updateCategoryDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("https://localhost:7066/api/Category", stringContent);
            if (!responseMessage.IsSuccessStatusCode)
            {
                var contentData = await responseMessage.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = contentData;
                return View(updateCategoryDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.DeleteAsync($"https://localhost:7066/api/Category/{id}");
            if (!responseMessage.IsSuccessStatusCode)
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IsActiveApproved(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token Oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync($"https://localhost:7066/api/Category/IsActiveAproved?id={id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> IsActiveApprovedCancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı giriş bilgileri alınamadı.";
                return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Token Oluşturulamadı.";
                return RedirectToAction("Index", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var responseMessage = await client.GetAsync($"https://localhost:7066/api/Category/IsActiveAprovedCancel?id={id}");
            if (!responseMessage.IsSuccessStatusCode)
            {
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}
