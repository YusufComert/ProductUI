using Microsoft.AspNetCore.Mvc;

namespace Payment.WebUI.ViewComponents.Layout
{
    public class _LayoutMiniCartPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
