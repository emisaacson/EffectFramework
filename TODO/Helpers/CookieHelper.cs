using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Helpers
{
    public class CookieHelper
    {
        public const string KEY_COOKIE_NAME = "_hrmsCookie";
        public const string IMPERSONATE_COOKIE_NAME = "_impersionateCookie";

        /// <summary>
        /// returns encription key if it is right one, else returns empty string
        /// </summary>
        /// <returns></returns>
        public static string GetEncriptionKeyFromCookie()
        {
#if (!STAGING && !DEBUG)
            return "HRMS";
#endif
            var cookie = HttpContext.Current.Request.Cookies[KEY_COOKIE_NAME];
            if (cookie != null)
            {
                var base64Key = HttpUtility.UrlDecode(cookie.Value);
                string hash = Crypto.GetHashFromBase64String(base64Key);
                if (Crypto.HASHED_KEY == hash)
                {
                    byte[] data = Convert.FromBase64String(HttpUtility.UrlDecode(base64Key));
                    return System.Text.Encoding.UTF8.GetString(data);
                }
            }
            return "";
        }

        public static bool IsUserEnteredKey()
        {
#if (!STAGING && !DEBUG)
            return true;
#endif
            return !String.IsNullOrEmpty(GetEncriptionKeyFromCookie());
        }

        public static bool IsImpersonating
        {
            get
            {
                return HttpContext.Current.Request.Cookies.AllKeys.Any(x => x == IMPERSONATE_COOKIE_NAME);
            }
        }

        public static string ImpersonationUser
        {
            get
            {
                return HttpContext.Current.Request.Cookies[IMPERSONATE_COOKIE_NAME] != null ? HttpContext.Current.Request.Cookies[IMPERSONATE_COOKIE_NAME].Value : null;
            }
        }
    }
}
