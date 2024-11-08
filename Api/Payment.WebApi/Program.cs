using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Payment.BusinessLayer.Abstract;
using Payment.BusinessLayer.Concrete;
using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.Concrete;
using Payment.DataAccessLayer.EntityFramework;
using Payment.DtoLayer.Dtos.TokenDtos;
using Payment.EntityLayer.Concrete;
using Payment.WebApi.Mapping;
using System.Globalization;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
        fv.DisableDataAnnotationsValidation = true;
        fv.ValidatorOptions.LanguageManager.Culture = new CultureInfo("tr");
    });

builder.Services.AddDbContext<Context>();
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<Context>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = JwtTokenDefaults.ValidIssuer;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtTokenDefaults.ValidIssuer,
        ValidAudience = JwtTokenDefaults.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key)),
        ClockSkew = TimeSpan.Zero 
    };

    // JWT token doðrulama sýrasýnda olaylar
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var claims = context.Principal?.Claims;
            var issuer = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Issuer;
            if (issuer == "https://accounts.google.com")
            {
                // Google kullanýcýsý için ek iþlemler
                // Örneðin, Google kullanýcýlarýný veritabanýnda kontrol etme
            }

            await Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(x =>
{
    x.ClientId = "268001054971-83d3k9qufn9f2gqle99s58e08op9h9nf.apps.googleusercontent.com";
    x.ClientSecret = "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";

})
.AddFacebook(x =>
{
    x.ClientId = "1504086693556868";
    x.ClientSecret = "fa3e08f66e488f5426fd4af40aac5a7f";
});


var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperConfig()));
builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddScoped<IProductDal, EfProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddScoped<IAddressDal, EfAddressDal>();
builder.Services.AddScoped<IAddressService, AddressManager>();

builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<IUserService, UserManager>();

builder.Services.AddScoped<IProductDetailDal, EfProductDetailDal>();
builder.Services.AddScoped<IProductDetailService, ProductDetailManager>();

builder.Services.AddScoped<IContactDal, EfContactDal>();
builder.Services.AddScoped<IContactService, ContactManager>();

builder.Services.AddScoped<ISubjectDal, EfSubjectDal>();
builder.Services.AddScoped<ISubjectService, SubjectManager>();

builder.Services.AddScoped<IProductSaleDal, EfProductSaleDal>();
builder.Services.AddScoped<IProductSaleService, ProductSaleManager>();

builder.Services.AddScoped<IOrderDal, EfOrderDal>();
builder.Services.AddScoped<IOrderService, OrderManager>();


builder.Services.AddCors(opt =>
{
    opt.AddPolicy("PaymentApiCors", opts =>
    {
        opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("PaymentApiCors");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
