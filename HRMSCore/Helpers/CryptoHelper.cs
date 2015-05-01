using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Local.Classes.Helpers
{
    public class CryptoHelper
    {

        public static string Decode(string text)
        {
            if (text == null) return null;
            if (string.IsNullOrEmpty(text)) return "";
            return Encoding.Unicode.GetString(Convert.FromBase64String(text));
        }

        public static string Encode(string text)
        {
            if (text == null) return null;
            if (string.IsNullOrEmpty(text)) return "";
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));
        }

        public static string Decrypt(string text, string CypherSecret)
        {
            if (text == null) return null;
            if (string.IsNullOrEmpty(text)) return "";
            try
            {
                return Crypto.DecryptStringAES(text, CypherSecret);
            }
            // temporary fallback
            catch
            {
                try
                {
                    return Decode(text);
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string Encrypt(string text, string CypherSecret)
        {
            if (text == null) return null;
            if (string.IsNullOrEmpty(text)) return "";

            return Crypto.EncryptStringAES(text, CypherSecret);
        }
    }
}