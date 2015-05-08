using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HRMS.Core.Extensions;
using HRMS.Core.Security;
using HRMS.Core.Helpers;

namespace HRMS.Core.Attributes
{
    /*
     * This is a key attribute that should be set on EVERY
     * MVC method and or controller class.
     * 
     * The Attribute checks that the current user has the specified
     * permission level on the specified security object, and redirects
     * to a forbidden page with a 403 response if not. If the optional
     * ShouldRedirect parameter is specified, the default
     * Authorize attribute unauthorized response is generated.
     * 
     * Example:
     * [HRAuthorize(Permission._EMP_BASIC, PermissionLevel.READ)]
     * 
     */
    public class HRAuthorize : AuthorizeAttribute
    {
        private string _Permission;
        private PermissionLevel _PermissionLevel;
        private bool _ShouldRedirect;
        private bool _AllowSelfService;
        public HRAuthorize(string Permission, string PermissionLevel = "READ", bool ShouldRedirect = false, bool AllowSelfService = false)
        {
            _Permission = Permission;
            _PermissionLevel = (PermissionLevel)(typeof(PermissionLevel).GetField(PermissionLevel, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null));
            _ShouldRedirect = ShouldRedirect;
            _AllowSelfService = AllowSelfService;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }
            if (DBHelper.IsSelfService() && !_AllowSelfService)
            {
                return false;
            }

            bool IsAuthorized = false;
            var currentUser = Helpers.DBHelper.GetCurrentUserContext();
            if (currentUser != null)
                IsAuthorized = currentUser.HasPermissionTo(_PermissionLevel, _Permission);

            return IsAuthorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (_ShouldRedirect)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary 
                                {
                                    { "action", "Forbidden" },
                                    { "controller", "Error" }
                                });
            }
            else
            {
                filterContext.Result = new Http403Result();
            }
        }

        internal class Http403Result : ActionResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                // Set the response code to 403.
                context.HttpContext.Response.StatusCode = 403;
            }
        }
    }
}