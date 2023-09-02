using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RanStoreOracle.Controllers
{
    public class SessionAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            int? userId = context.HttpContext.Session.GetInt32("CustomerId");
            int? adminId = context.HttpContext.Session.GetInt32("AdminId");

            if (adminId.HasValue || userId.HasValue)
            {
            }

        else
            {
                context.Result = new RedirectToActionResult("Login", "LoginAndRegister", null);

            }
        }
    }
}