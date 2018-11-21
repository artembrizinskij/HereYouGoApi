using System;
using System.Linq;
using DAl.Sql.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Security.Providers;


namespace Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class UserIdentityAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var id = context.HttpContext?.User.Claims?.First(x => x.Type == "Id")?.Value;
                var contextProvider = context.HttpContext.RequestServices.GetService<IContextProvider>();
                var db = context.HttpContext.RequestServices.GetService<ICommonDbService>();
                contextProvider.Inizialize(id, db);
            }
        }
    }
}
