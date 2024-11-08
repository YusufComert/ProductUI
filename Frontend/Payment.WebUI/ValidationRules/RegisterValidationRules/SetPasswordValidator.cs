using FluentValidation;
using Payment.WebUI.DTOs.RegisterDtos;

namespace Payment.WebUI.ValidationRules.RegisterValidationRules
{
    public class SetPasswordValidator:AbstractValidator<SetPasswordDto>
    {
        public SetPasswordValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre Alanı Gereklidir")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre Tekrar Alanı Gereklidir")
                .Equal(x => x.Password).WithMessage("Şifreler Uyuşmuyor");
        }
    }
}
