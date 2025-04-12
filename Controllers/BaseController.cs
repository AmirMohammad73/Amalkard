using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeePerformanceSystem.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            
            // اگر کاربر به صفحه‌ای غیر از لاگین می‌رود و لاگین نکرده است
            if (!(context.Controller is LoginController) && 
                string.IsNullOrEmpty(session.GetString("Fullname")))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }
            
            // اگر کاربر لاگین کرده است و می‌خواهد به صفحه لاگین برود
            if (context.Controller is LoginController && 
                !string.IsNullOrEmpty(session.GetString("Fullname")))
            {
                context.Result = new RedirectToActionResult("Index", "Record", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}