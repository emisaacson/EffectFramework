using System.Linq;
using Microsoft.AspNet.Mvc;

namespace HRMS.Core.Attributes
{
    public class ValidateOnlyFieldsWithPermission : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            var keysWithNoIncomingValue = modelState.Keys.Where(x => x == "wtf");
            foreach (var key in keysWithNoIncomingValue)
            {
                modelState[key].Errors.Clear();
            }
        }
    }
}