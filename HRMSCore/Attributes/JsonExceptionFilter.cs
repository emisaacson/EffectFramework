using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Local.Classes.Attributes
{
    /*
     * This attribute is applied to controllers and action methods
     * to return exception messages in json format.
    */
    public class JsonExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("text/html"));
        }
    }
}
