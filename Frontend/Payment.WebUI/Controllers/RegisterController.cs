﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.RegisterDtos;
using Payment.WebUI.Models.Mail;
using System.Text;

namespace Payment.WebUI.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterController(IHttpClientFactory httpClientFactory, IEmailSender emailSender, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(registerDto);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7066/api/UserRegister", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                var user = await _userManager.FindByEmailAsync(registerDto.Email);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Register", new { user.Id, token });
                string confirmationLink = _configuration["Site_Url"] + url;

                await _emailSender.SendEmailAsync(user.Email, "Hesap Onayı", $"Hesabınızı onaylamak için <a href=\"{confirmationLink}\">linke</a> tıklayınız.");
                TempData["message"] = ("Hesap onayı için mail adresinizi kontrol ediniz.");
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var errorMessage = await responseMessage.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                TempData["message"] = "Geçersiz Token Bilgisi";
                return RedirectToAction("Error", "Home");
            }

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData["message"] = "Mail Onaylandı";
                    return View();

                    
                }
                else
                {
                    TempData["message"] = "Mail Onaylanmadı";
                    return RedirectToAction("Error", "Home");
                }
            }
            return View();
        }
    }
}