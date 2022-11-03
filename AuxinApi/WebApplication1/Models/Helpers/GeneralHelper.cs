using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication1;

namespace KMDCL.Model.Helpers
{
    public static class GeneralHelper
    {
        private static DbEntities db = new DbEntities();

        public static string DateFormate = "dd-MMM-yyyy";
        public static string BASE_URL = @"http://148.72.213.34/KMDCLMAPI/ProjectFiles/";
        //public static string BASE_URL = @"http://localhost:60851/ProjectFiles/";
        private readonly static string SecretKey = "4512631236589784";

        //Generate RandomNo
        public static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public static class RegularExpressions
        {
            public static readonly string MobileNumber = @"^[6-9]\d{9}$";
        }

        internal static string onGetShortSchemeName(string shortCode)
        {
            var shortName = string.Empty;
            switch (shortCode)
            {
                case "ARVUELS": shortName = "Arivu Education Loan Scheme"; break;
                case "GNGKLNS": shortName = "Ganga Kalyan Scheme"; break;
                case "MICROLS": shortName = "Micro Loan Scheme"; break;
                case "MLSCG-C19": shortName = "Micro-Charity Group Scheme"; break;
                case "SRMSHLS": shortName = "Shramashakthi Scheme"; break;
                case "SUBSTGA": shortName = "Auto/Taxi/Goods Subsidy"; break;
                default: break;
            }
            return shortName;
        }

        internal static int YearOrder(string key)
        {
            return Int32.Parse(key.Split('-')[0]);
        }
        internal static bool IsValidRole(List<String> userRoles, string role)
        {
            return userRoles.Contains(role);
        }

        public static bool IsValidMobile(string mobile)
        {
            return Regex.Match(mobile, RegularExpressions.MobileNumber).Success;
        }

        public static string GetClientIp(HttpRequestMessage request = null)
        {

            IPHostEntry host = default(IPHostEntry);
            string hostname = null;
            hostname = System.Environment.MachineName;
            host = Dns.GetHostEntry(hostname);
            string ipAddress = "127.0.0.1";
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = Convert.ToString(ip);
                }
            }
            return ipAddress;
        }
        public static void SendOtp(string number, string otp)
        {

            string message = "Please use OTP " + otp + " to login.";
            string url = string.Format("https://api-alerts.kaleyra.com/v4/?api_key=A088baef081265e84761270ea16e7e2fc&method=sms&message={0}&to={1}&sender=DEODKS", message, number);
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                sr.ReadToEnd();
            }
            response.Close();


        }

        internal static int onGetCasteOrder(string caste)
        {
            var order = 0;
            switch (caste)
            {
                case "MUSLIM":
                    order = 1;
                    break;
                case "CHRISTIAN":
                    order = 2;
                    break;
                case "BUDDHIST": order = 3; break;
                case "JAIN":
                    order = 4;
                    break;
                case "SIKH":
                    order = 5;
                    break;
                case "PARSI":
                    order = 6;
                    break;
                default: order = 999; break;
            }
            return order;
        }

        public static string DecryptString(string cipherTextStr)
        {
            byte[] cipherText = Convert.FromBase64String(cipherTextStr);
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var iv = Encoding.UTF8.GetBytes(SecretKey);
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }

            string plaintext = null;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        internal static int? onGetDistrictId(string dist)
        {
            var districts = db.Districts.ToList().Where(oo => oo.District.IndexOf(dist) != -1).ToList();
            if (!districts.Any()) return null;

            var district = districts.First();
            return district.Id;
        }

        public static string EncryptString(string strPlainText)
        {
            string cipherText = string.Empty;
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var iv = Encoding.UTF8.GetBytes(SecretKey);
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.BlockSize = 128;
                rijAlg.KeySize = 128;
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;
                try
                {
                    ICryptoTransform transform = rijAlg.CreateEncryptor();
                    var cipher = transform.TransformFinalBlock(strText, 0, strText.Length);
                    if (cipher == null || cipher.Length <= 0)
                    {
                        throw new ArgumentNullException("Encrypt Error");
                    }
                    else
                    {
                        cipherText = Convert.ToBase64String(cipher);
                    }
                }
                catch
                {
                    cipherText = "keyError";
                }
            }

            return cipherText;
        }

        internal static object GetRenewalmessage(DateTime? dueDate)
        {
            if (!dueDate.HasValue)
                return "";
            var NoOfDays = 0;
            string Duration = string.Empty;
            string message = string.Empty;

            if (dueDate.Value.Date <= DateTime.Today.Date)
            {
                message = "Expired!!!, Please Renew.";
            }
            else
            if (dueDate.Value.Date > DateTime.Today.Date)
            {
                NoOfDays = GetNoOfDays(dueDate.Value.Date, DateTime.Today.AddDays(1).Date);
                Duration = DateDuration(dueDate.Value.Date, DateTime.Today.AddDays(1).Date);
                message = "Expires in " + Duration + (NoOfDays < 30 ? ", Please Renew." : ".");
            }

            return message;
        }

        public static int GetNoOfDays(DateTime? StartData, DateTime? EndDate)
        {
            if (!StartData.HasValue || !EndDate.HasValue)
                return -1;

            return (int)(StartData.Value.Date - EndDate.Value.Date).TotalDays;

        }
        public static string DateDuration(DateTime? StartDate, DateTime? EndDate)
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
                return "";

            if (StartDate.Value >= DateTime.Today)
                return "";

            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            DateTime fromDate = StartDate.Value;
            DateTime toDate = EndDate.Value;
            int year = 0;
            int month = 0;
            int day = 0;
            int increment = 0;
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
            {
                day = toDate.Day - fromDate.Day;
            }
            if ((fromDate.Month + increment) > toDate.Month)
            {
                month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }
            year = toDate.Year - (fromDate.Year + increment);

            return (year > 0 ? year + " year(s), " : "") + (month > 0 ? month + " month(s), " : "") + day + " day(s)";

        }

        internal static object OnGetApplicationVerifyLevel(string applicationStatus)
        {
            var level = 0;
            var status = string.Empty;
            var roleName = string.Empty;
            var arpstatus = string.Empty;
            var rjtstatus = string.Empty;
            var revstatus = string.Empty;
            switch (applicationStatus.ToLower().Trim())
            {
                case "draft": level = 1;
                    arpstatus = "Application Submitted (Online)";
                    status = "DRAFT";  roleName = "APPLICANT";  break;

                case "application submitted (online)": level = 1;
                    arpstatus = "Verified By District Case Worker";
                    rjtstatus = "Rejected By District Case Worker";
                    revstatus = "DRAFT";
                    status = "Verification by District Case Worker";  roleName = "DCW";  break; 

                case "verified by district case worker": level = 2;
                    arpstatus = "Verified By District Manager";
                    rjtstatus = "Rejected By District Manager";
                    revstatus = "Re-Verification By District Case Worker";
                    status = "Verification by District Manager";  roleName = "DM";  break;
                case "re-verification by district case worker": level = 1;
                    arpstatus = "Verified By District Case Worker";
                    rjtstatus = "Rejected By District Case Worker";
                    revstatus = "DRAFT";
                    status = "Re-Verification by District Case Worker";  roleName = "DCW";  break;
                case "rejected by district case worker":
                    level = 1;
                    arpstatus = "Rejected By District Case Worker";
                    rjtstatus = "Rejected By District Case Worker";
                    revstatus = "Rejected By District Case Worker";
                    status = "Rejected By District Case Worker";  roleName = "DCW";  break;

                case "verified by district manager": level = 3;
                    arpstatus = "Selection Committee Shortlist";
                    rjtstatus = "Rejected By Selection Committee";
                    revstatus = "Re-Verification By District Manager";
                    status = "Selection Committee Shortlist";  roleName = "DM";  break;
                case "re-verification by district manager": level = 2;
                    arpstatus = "Verified By District Manager";
                    rjtstatus = "Rejected By District Manager";
                    revstatus = "Re-Verification By District Case Worker";
                    status = "Re-Verification by District Manager";  roleName = "DM";  break;
                case "rejected by district manager":
                    level = 1;
                    arpstatus = "Rejected By District Manager";
                    rjtstatus = "Rejected By District Manager";
                    revstatus = "Rejected By District Manager";
                    status = "Rejected By District Manager";  roleName = "DM";  break;

                case "selection committee shortlist": level = 4;
                    arpstatus = "Verified By H.O. Case Worker";
                    rjtstatus = "Rejected By H.O. Case Worker";
                    revstatus = "Re-Verification By District Manager";
                    status = "Verification by H.O. Case Worker";  roleName = "HOCW";  break;
                //case "re-approval by bank": level = 3;
                //    arpstatus = "Approved By Bank";
                //    rjtstatus = "Rejected By Bank";
                //    revstatus = "Re-Verification By District Manager";
                //    status = "Re-Approval By Bank";  roleName = "BANK_ADMIN";  break;
                case "rejected by selection committee":
                    level = 1;
                    arpstatus = "Rejected By Selection Committee";
                    rjtstatus = "Rejected By Selection Committee";
                    revstatus = "Rejected By Selection Committee";
                    status = "Rejected By Selection Committee";  roleName = "DM";  break;

                case "verified by h.o. case worker": level = 5;
                    arpstatus = "Verified By General Manager";
                    rjtstatus = "Rejected By General Manager";
                    revstatus = "Re-Verification By H.O. Case Worker";
                    status = "Verification by General Manager";  roleName = "GM";  break;
                case "re-verification by h.o. case worker": level = 4;
                    arpstatus = "Verified By H.O. Case Worker";
                    rjtstatus = "Rejected By H.O. Case Worker";
                    revstatus = "Re-Approval By District Manager";
                    status = "Verification by H.O. Case Worker";  roleName = "HOCW";  break;
                case "rejected by h.o. case worker":
                    level = 1;
                    arpstatus = "Rejected By H.O. Case Worker";
                    rjtstatus = "Rejected By H.O. Case Worker";
                    revstatus = "Rejected By H.O. Case Worker";
                    status = "Rejected By H.O. Case Worker";  roleName = "HOCW";  break;

                case "verified by general manager": level = 6;
                    arpstatus = "Verified By Managing Director";
                    rjtstatus = "Rejected By Managing Director";
                    revstatus = "Re-Verification By General Manager";
                    status = "Verification by Managing Director";  roleName = "MD";  break;
                case "re-verification by general manager": level = 5;
                    arpstatus = "Verified By General Manager";
                    rjtstatus = "Rejected By General Manager";
                    revstatus = "Re-Verification By H.O. Case Worker";
                    status = "Re-Verification By General Manager";  roleName = "GM";  break;
                case "rejected by general manager":
                    level = 1;
                    arpstatus = "Rejected By General Manager";
                    rjtstatus = "Rejected By General Manager";
                    revstatus = "Rejected By General Manager";
                    status = "Rejected By General Manager";  roleName = "GM";  break;

                case "verified by managing director": level = 7;
                    arpstatus = "Final Cheque Disbursed/NEFT By MD";
                    rjtstatus = "Rejected By Managing Director";
                    revstatus = "Re-Verification By Managing Director";
                    status = "Final Cheque Disbursment/NEFT by MD";  roleName = "MD";  break;
                case "re-verification by managing director": level = 6;
                    arpstatus = "Verified By Managing Director";
                    rjtstatus = "Rejected By Managing Director";
                    revstatus = "Re-Verification By General Manager";
                    status = "Re-Verification by Managing Director";  roleName = "MD";  break;
                case "rejected by managing director":
                    level = 1;
                    arpstatus = "Rejected By Managing Director";
                    rjtstatus = "Rejected By Managing Director";
                    revstatus = "Rejected By Managing Director";
                    status = "Rejected By Managing Director";  roleName = "MD";  break;

                case "final cheque disbursed/neft by md": level = 8;
                    arpstatus = "Final Cheque Disbursed/NEFT";
                    rjtstatus = "Final Cheque Disbursed/NEFT";
                    revstatus = "Final Cheque Disbursed/NEFT";
                    status = "Final Cheque Disbursed/NEFT";  roleName = "MD";  break;
                default: level = 999;   break;
            }
            return new
            {
                Level = level,
                RoleName = roleName,
                NextStatus = status,
                AprStatus = arpstatus,
                RjtStatus = rjtstatus,
                RevStatus = revstatus
            };
        }

        public static object TimeDuration(DateTime? StartDate, DateTime? EndDate)
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
                return "0 min";

            if (StartDate.Value >= DateTime.Today)
                return "0 min";

            DateTime fromDate = StartDate.Value;
            DateTime toDate = EndDate.Value;
            int hour = 0;
            int min = 0;
            int sec = 0;

            var totalSeconds = Convert.ToInt32((toDate - fromDate).TotalSeconds);
            var remainingSeconds = 0;

            hour = Convert.ToInt32(totalSeconds/3600);
            remainingSeconds = totalSeconds - (hour * 3600);

            min = Convert.ToInt32(remainingSeconds/60);
            remainingSeconds = remainingSeconds - (min * 60);

            sec = remainingSeconds;

            return new
            {
                Hours = (hour > 0 ? hour + " hour(s) " : ""),
                Minutes = (min > 0 ? min + " min(s) " : ""),
                Seconds = (sec > 0 ? sec : 0) + " sec(s) "
            };
        }
        /// <summary>
        /// To demonstrate extraction of file extension from base64 string.
        /// </summary>
        /// <param name="base64String">base64 string.</param>
        /// <returns>Henceforth file extension from string.</returns>
        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";
                case "AAAAF":
                    return ".mp4";
                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "MQOWM":
                case "77U/M":
                    return ".srt";
                default:
                    return string.Empty;
            }
        }
    }
}