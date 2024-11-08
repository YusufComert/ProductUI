using Payment.DataAccessLayer.Concrete;
using Payment.EntityLayer.Concrete;
using Payment.WebUI.Models.Mail;
using Payment.BusinessLayer.Abstract;
using Payment.BusinessLayer.Concrete;
using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.EntityFramework;
using FluentValidation.AspNetCore;
using System.Globalization;
using Payment.WebUI.Services;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();


builder.Services.AddScoped<Payment.WebUI.Models.Mail.IEmailSender, SmtpEmailSender>(i =>
    new SmtpEmailSender(
        builder.Configuration["EmailSender:Host"],
        builder.Configuration.GetValue<int>("EmailSender:Port"),
        builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
        builder.Configuration["EmailSender:Username"],
        builder.Configuration["EmailSender:Password"])
);


builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddScoped<IProductDal, EfProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddScoped<ICategoryServiceUI, CategoryServiceUI>();

builder.Services.AddScoped<IContactDal, EfContactDal>();
builder.Services.AddScoped<IContactService, ContactManager>();



builder.Services.AddDbContext<Context>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication()
.AddGoogle(x =>
{
    x.ClientId = "268001054971-83d3k9qufn9f2gqle99s58e08op9h9nf.apps.googleusercontent.com";
    x.ClientSecret = "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";

    x.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };

})
.AddFacebook(x =>
{
    x.ClientId = "1504086693556868";
    x.ClientSecret = "fa3e08f66e488f5426fd4af40aac5a7f";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    });

    options.AddPolicy("Manager", policy =>
    {
        policy.RequireRole("Admin", "Manager"); 
    });

    options.AddPolicy("Visitor", policy =>
    {
        policy.RequireRole("Admin", "Manager", "Visitor"); 
    });
});

builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
        fv.DisableDataAnnotationsValidation = true;
        fv.ValidatorOptions.LanguageManager.Culture = new CultureInfo("tr");
    });

builder.Services.AddControllers().AddXmlSerializerFormatters();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
