using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Payment.WebUI.Controllers
{
    public class AdminFileController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var stream = new MemoryStream();//Akışı oluşturduk
            await file.CopyToAsync(stream);//dosyayı kopyaladık
            var bytes = stream.ToArray();//akıştaki dosyayı byte olarak tuttuk

            ByteArrayContent byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(byteArrayContent, "file", file.FileName);
            var httpclient = new HttpClient();
            await httpclient.PostAsync("https://localhost:7066/api/FileProcess", multipartFormDataContent);

            return View();
        }
    }
}
