using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.LoginDtos;
using Payment.WebUI.DTOs.RegisterDtos;
using Payment.WebUI.Tools;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Payment.WebUI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.ErrorMessage = message;

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(loginDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7066/api/UserLogin", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Contains("Admin"))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "AdminProduct");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            var errorMessage = await responseMessage.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = errorMessage;
            return View(loginDto);
        }

        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Login", new { ReturnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            properties.Items["scope"] = "openid profile email";
            //properties.Items["prompt"] = "consent";
            return new ChallengeResult("Google", properties);
        }
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")

        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ViewBag.ErrorMessage = "Google hesabı ile giriş başarısız oldu.";
                return RedirectToAction("Index");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded || (await _userManager.GetLoginsAsync(user)).Any(l => l.LoginProvider == info.LoginProvider))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var resultPassword = await _userManager.HasPasswordAsync(user);
                    if (!resultPassword)
                    {
                        return RedirectToAction("SetPassword", new { userId = user.Id });
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Bu Google hesabı mevcut kullanıcıya eklenemedi.";
                    return RedirectToAction("Index");
                }
            }
            user = new AppUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname),
                Gender = "Belirtmek İstemiyorum",
                PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                CreateUser = "Google",
                CreateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString()),
                UpdateUser = "Google",
                UpdateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString())
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ViewBag.ErrorMessage = "Kullanıcı oluşturulamadı";
                return RedirectToAction("Index");
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("SetPassword", new { userId = user.Id });
        }

        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("FacebookResponse", "Login", new { ReturnUrl = ReturnUrl });
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }

        public async Task<IActionResult> FacebookResponse(string ReturnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Index");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var userLogins = await _userManager.GetLoginsAsync(user);
                var isAlreadyLinked = userLogins.Any(x => x.LoginProvider == info.LoginProvider);

                if (!isAlreadyLinked)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ViewBag.ErrorMessage = "Kullanıcıya giriş sağlayıcısı eklenemedi.";
                        return RedirectToAction("Index");
                    }

                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (!await _userManager.HasPasswordAsync(user))
                {
                    return RedirectToAction("SetPassword", new { userId = user.Id });
                }

                return RedirectToAction("Index", "Home");
            }
            user = new AppUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.Name),
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname),
                Gender = "Belirtmek İstemiyorum",
                PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                CreateUser = "Facebook",
                CreateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString()),
                UpdateUser = "Facebook",
                UpdateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString())
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ViewBag.ErrorMessage = "Kullanıcı oluşturulamadı";
                return RedirectToAction("Index");
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("SetPassword", new { userId = user.Id });
        }


        [HttpGet]
        public IActionResult SetPassword(string userId)
        {
            var model = new SetPasswordDto();
            ViewBag.UserId = userId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(string userId, SetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.AddPasswordAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessages = result.Errors.Select(x => x.Description).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.PostAsync("https://localhost:7066/api/UserLogin/Logout", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                return RedirectToAction("Index", "Home");
            }
            return BadRequest("Failed to logout");

        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}