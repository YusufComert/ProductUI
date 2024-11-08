using Microsoft.AspNetCore.Mvc;

namespace Payment.WebUI.ViewComponents.Dashboard
{
    public class _DashboardScriptPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
