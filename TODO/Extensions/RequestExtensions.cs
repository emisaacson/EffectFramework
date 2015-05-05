using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace HRMS.Core.Extensions
{
    public static class RequestExtensions
    {
        public static HttpResponseMessage CreateJsonResponse<T>(this HttpRequestMessage request, T value)
        {
            return request.CreateResponse(HttpStatusCode.OK, value, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("text/html"));
        }

        /// <param name="files">This parameter solves an issue with IE9/IE10, in which the iframe-json converter is not called when the response status is bad-request. Thus, we must respond with ok status when there are files, and validate client-side.</param>
        public static HttpResponseMessage CreateJsonResponse(this HttpRequestMessage request, Local.Models.Model model, bool files)
        {
            return request.CreateResponse(files ? HttpStatusCode.OK : HttpStatusCode.BadRequest, model, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("text/html"));
        }
    }
}