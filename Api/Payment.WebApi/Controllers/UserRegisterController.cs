using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment.DtoLayer.Dtos.RegisterDtos;

namespace Payment.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUserByEmail != null)
            {
                return BadRequest("Bu email adresi ile kayıtlı bir kullanıcı zaten mevcut.");
            }

            var existingUserByPhone = await _userManager.Users
                .AnyAsync(x => x.PhoneNumber == registerDto.PhoneNumber);
            if (existingUserByPhone)
            {
                return BadRequest("Bu telefon numarası ile kayıtlı bir kullanıcı zaten mevcut.");
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUserByUsername != null)
            {
                return BadRequest("Bu kullanıcı adı ile kayıtlı bir kullanıcı zaten mevcut.");
            }

            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                PhoneNumber = registerDto.PhoneNumber,
                Gender = registerDto.Gender,
                CreateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString()),
                UpdateTime = DateTime.Parse(DateTime.UtcNow.ToShortDateString()),
                CreateUser = registerDto.Username,
                UpdateUser = registerDto.Username
            };

            if (registerDto.Password != registerDto.ConfirmPassword)
                return BadRequest("Şifre Eşleşmiyor");

            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (result.Succeeded)
                return Ok("User created successfully");

            return BadRequest("User creation failed");
        }
    }
}