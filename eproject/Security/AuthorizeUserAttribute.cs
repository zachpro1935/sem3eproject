using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eproject.Models;
namespace eproject.Security
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom Authorize
        public string role { get; set; }
        context db = new context();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (HttpContext.Current.Session["userId"] == null)
            {
                return false;
            }
            var id = HttpContext.Current.Session["userId"].ToString();
            string userRole = db.user.Find(Guid.Parse(id)).role;// Call another method to get rights of the user from DB
            if (userRole == "ROLE_ADMIN")
            {
                return true;
            }
            return userRole.Contains(this.role);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                                  new RouteValueDictionary
                                  {
                                       { "action", "index" },
                                       { "controller", "home" }
                                  });
        }

      
    }
}