using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Payment.WebUI.DTOs.ProductDtos;

namespace Payment.WebUI.ViewComponents.Dashboard
{
    public class _DashboardProductList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
