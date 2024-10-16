//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;


//namespace BdJobsCorporate_Corporate_Insert.AggregateRoot.Validation
//{
//    public class Validation
//    {
//        public static bool IsNullOrEmpty(string dataString)
//        {
//            return string.IsNullOrEmpty(dataString);
//        }

//        public static bool IsDataLengthValid(string dataString, int maxLength)
//        {
//            return dataString.Length <= maxLength;
//        }

//        public static bool TestCaptcha(string sessionValue, string captchaValue, ISession session)
//        {
//            string sessionCaptcha = session.GetString(sessionValue);
//            session.Remove(sessionValue);

//            if (string.IsNullOrEmpty(sessionCaptcha))
//            {
//                return false;
//            }

//            captchaValue = captchaValue.Replace("i", "I");
//            return string.Equals(sessionCaptcha, captchaValue, StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
