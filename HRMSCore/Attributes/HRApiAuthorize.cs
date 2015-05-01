using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Local.Classes.Extensions;
using Local.Classes.Security;
using System.Web.Http.Controllers;
using System.Net.Http;
using Local.Classes.Helpers;

namespace Local.Classes.Attributes
{
    /*
     * This is a key attribute that should be set on EVERY
     * WebAPI method and or controller class.
     * 
     * The Attribute checks that the current user has the specified
     * permission level on the specified security object, and returns
     * a 403 Forbidden response if not
     * 
     * Example:
     * [HRApiAuthorize(Permission._EMP_BASIC, PermissionLevel.READ)]
     * 
     */
    public class HRApiAuthorize : AuthorizeAttribute
    {
        private string _Permission;
        private Type _ModelType;
        private PermissionLevel _PermissionLevel;
        private string _FieldName;
        private bool _AllowSelfService;

        public HRApiAuthorize(string Permission, string PermissionLevel = "READ", bool AllowSelfService = false)
        {
            _Permission = Permission;
            _PermissionLevel = (PermissionLevel)(typeof(PermissionLevel).GetField(PermissionLevel, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null));
            _AllowSelfService = AllowSelfService;
        }

        public HRApiAuthorize(Type ModelType, string PermissionLevel = "READ", bool AllowSelfService = false)
        {
            _ModelType = ModelType;
            _PermissionLevel = (PermissionLevel)(typeof(PermissionLevel).GetField(PermissionLevel, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null));
            _AllowSelfService = AllowSelfService;
        }

        public HRApiAuthorize(Type ModelType, string FieldName, string PermissionLevel = "READ", bool AllowSelfService = false)
        {
            _ModelType = ModelType;
            _FieldName = FieldName;
            _PermissionLevel = (PermissionLevel)(typeof(PermissionLevel).GetField(PermissionLevel, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null));
            _AllowSelfService = AllowSelfService;
        }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (!base.IsAuthorized(httpContext))
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
            {
                if (_ModelType != null)
                {
                    if (_FieldName != null)
                    {
                        IsAuthorized = currentUser.HasPermissionTo(_ModelType, _FieldName, _PermissionLevel);
                    }
                    else
                    {
                        IsAuthorized = currentUser.HasPermissionTo(_ModelType, _PermissionLevel);
                    }
                }
                else
                {
                    IsAuthorized = currentUser.HasPermissionTo(_PermissionLevel, _Permission);
                }
            }
            return IsAuthorized;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Forbidden, "Forbidden");
            actionContext.Response = response;
        }
    }
}