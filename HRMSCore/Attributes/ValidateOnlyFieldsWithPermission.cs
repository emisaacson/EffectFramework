using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Local.Classes.Attributes
{
    public class ValidateOnlyFieldsWithPermission : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var modelState = filterContext.ModelState;

            var keysWithNoIncomingValue = modelState.Keys.Where(x => x == "wtf");
            foreach (var key in keysWithNoIncomingValue)
            {
                modelState[key].Errors.Clear();
            }
        }
    }
}