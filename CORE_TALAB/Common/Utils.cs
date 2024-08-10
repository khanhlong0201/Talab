using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CORE_TALAB.Common
{
    /// <summary>
    /// Class provide common APIs that are relate to orders. 
    /// </summary>
    public static class Utils
    {
        public static string GetFileFormatFromBase64(string base64String)
        {
            if (base64String.StartsWith("/9j") || base64String.StartsWith("iVB"))
            {
                return ".jpeg";
            }
            else if (base64String.StartsWith("R0lGOD"))
            {
                return ".gif";
            }
            else if (base64String.StartsWith("UklGR"))
            {
                return ".png";
            }
            else
            {
                return ".jpeg";
            }
        }
        public static string GetDynamicUrlPart(string fullUrl)
        {
            // Sử dụng regex để lấy phần cần thiết của URL
            Match match = Regex.Match(fullUrl, @"/Image/SOSMobile/.+");
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "Không tìm thấy URL cần thiết";
            }
        }
        public static string GenerateRandomString(Random random, int length)
        {
            const string chars = "0123456789";

            // Sử dụng StringBuilder để hiệu quả hơn khi nối chuỗi
            StringBuilder stringBuilder = new StringBuilder(length);

            // Tạo ngẫu nhiên ký tự và thêm vào chuỗi
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }
         public static string GetImagePath(string image)
        {
            if (!string.IsNullOrWhiteSpace(image))
                return string.Format("/Image/{0}", image);

            return "Image Default";
        }

        /// <summary>
        /// Gets the path of next image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        public static string GetPathOfNextImage(string path, string imageName)
        {
            try
            {
                var sliptPath = path.Split('/');
                if (sliptPath.Count() > 3)
                {
                    string oldName = sliptPath[3];
                    var sliptName = oldName.Split('.');
                    if (sliptName.Count() > 0)
                    {
                        return path.Replace(sliptName[0], imageName);
                    }
                }
            }
            catch
            {
                // e = AssignParam(e,
                //                 new Param("path", path),
                //                 new Param("imageName", imageName));
                // ExceptionHandler.HandleException(e, "WebServiceExceptionPolicy");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the redundant character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="redundantCharacter">The redundant character.</param>
        /// <returns></returns>
        public static string RemoveRedundantCharacter(string value, string redundantCharacter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i + 1 < value.Length && value[i].Equals(value[i + 1]))
                    {
                        value = value.Remove(i, 1);
                        i = i - 1;
                    }
                }
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Strings to date time.
        /// </summary>
        /// <param name="stringDate">The string date.</param>
        /// <returns></returns>
        static public DateTime StringToDateTime(string stringDate)
        {
            DateTime retVal;

            try
            {
                retVal = new DateTime(
                        int.Parse(stringDate.Substring(0, 4)),
                        int.Parse(stringDate.Substring(4, 2)),
                        int.Parse(stringDate.Substring(6, 2)));
            }
            catch
            {
                retVal = new DateTime();
                // LogHandler.Log(
                //     "stringDate: was incorrect format (yyyyMMdd), stringDate = " + stringDate,
                //     "StringToDateTime()",
                //     TraceEventType.Warning);
            }

            return retVal;
        }

        /// <summary>
        /// Strings to time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public static TimeSpan? StringToTimeSpan(string timeSpan)
        {
            TimeSpan time;
            if (TimeSpan.TryParse(timeSpan, out time))
                return time;
            return null;
        }
        public static CORE_TALAB.EventEnums.TypeOfSignal GetEventType(string code)
        {
            if (code == "FIRE")
                return CORE_TALAB.EventEnums.TypeOfSignal.FIRE;
            if (code == "FIRE_SOS")
                return CORE_TALAB.EventEnums.TypeOfSignal.FIRE_SOS;
            if (code == "ERROR_CABINET")
                return CORE_TALAB.EventEnums.TypeOfSignal.ERROR_CABINET;
            if (code == "ERROR_CHANNEL1")
                return CORE_TALAB.EventEnums.TypeOfSignal.ERROR_CHANNEL1;
            if (code == "ERROR_CHANNEL2")
                return CORE_TALAB.EventEnums.TypeOfSignal.ERROR_CHANNEL2;
            if (code == "ERROR_CHANNEL3")
                return CORE_TALAB.EventEnums.TypeOfSignal.ERROR_CHANNEL3;
            if (code == "CONNECT_WORKING")
                return CORE_TALAB.EventEnums.TypeOfSignal.CONNECT_WORKING;
            if (code == "CONNECT_LOST")
                return CORE_TALAB.EventEnums.TypeOfSignal.CONNECT_LOST;
             if (code == "FIRE_PRESS_BUTTON")
                return CORE_TALAB.EventEnums.TypeOfSignal.FIRE_PRESS_BUTTON;

            else
                return CORE_TALAB.EventEnums.TypeOfSignal.FIRE;
        }
        public static string GetString(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        public static string GetDefaultString(string str)
        {
            if (str == null) return string.Empty;
            return str;
        }

        public static string GetDefaultString(DateTime? dateTime, string format = "dd/MM/yyyy HH:mm")
        {
            if (dateTime.HasValue) return dateTime.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// Determines whether [is valid date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidDate(string date)
        {
            DateTime dateTime;
            return string.IsNullOrEmpty(date) || DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
        /// <summary>
        /// Determines whether [is valid date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidDate(string date, out DateTime dateTime)
        {
            dateTime = new DateTime();
            return string.IsNullOrEmpty(date) || DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
        /// <summary>
        /// Determines whether [is valid date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidDate(string date, out DateTime? dateTime)
        {
            DateTime dt;
            var result = DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            if (result)
            {
                dateTime = dt;
                return true;
            }
            dateTime = null;
            return false;
        }

        public static bool IsValidDate(string date, string formatDate, out DateTime? dateTime)
        {
            DateTime dt;
            var result = DateTime.TryParseExact(date, formatDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            if (result)
            {
                dateTime = dt;
                return true;
            }
            dateTime = null;
            return false;
        }

        /// <summary>
        /// Converts the day of week.
        /// </summary>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns></returns>
        public static int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                case DayOfWeek.Sunday:
                    return 8;
            }

            return 0;
        }


       

        /// <summary>
        /// Gets the culture info.
        /// </summary>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
        // public static CultureInfo GetCultureInfo(string languageId)
        // {
        //     if (languageId.ToUpper().Equals(Constants.VIETNAMESE_ID.ToUpper()) || languageId.ToUpper().Equals(Constants.VIETNAMESE_LANGUAGE_ID.ToUpper()))
        //         return new CultureInfo(Constants.VIETNAMESE_ID);
        //     return new CultureInfo(Constants.ENGLISH_ID);
        // }

        /// <summary>
        /// Removes the last character.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        public static string RemoveLastCharacter(string str, char character)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str[str.Length - 1].Equals(character))
                    return str.Remove(str.Length - 1, 1);
                return str;
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines whether [is unicode string] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is unicode string] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUnicodeString(string value)
        {
            string characterUnicode = "áảàạãăắẳằặẵâấẩầậẫóỏòọõôốổồộỗơớởờợỡéèẻẹẽêếềểệễúùủụũưứừửựữíìỉịĩýỳỷỵỹđ";
            for (int i = 0; i < value.Length; i++)
            {
                if (characterUnicode.IndexOf(value[i].ToString()) > -1)
                    return true;
            }
            return false;
        }
      

        /// <summary>
        /// Compare 2 Vietnamese name
        /// </summary>
        /// <param name="a"> First name.</param>
        /// <param name="b">The Second name</param>
        /// <returns> </returns>
        public static bool CompareVietNameseName(string a, string b)
        {
            //remove all any char is not letter or digit
            a = new string(a.Where(c => (char.IsLetterOrDigit(c))).ToArray()).ToLower();
            b = new string(b.Where(c => (char.IsLetterOrDigit(c))).ToArray()).ToLower();
            if (a == b)
                return true;
            return false;
        }

        private readonly static string[] unicodeToHop = new string[] {
            "a","â","ă","e","ê","i","o","ô","ơ","u","ư","y","A","Â","Ă","E","Ê","I","O","Ô","Ơ","U","Ư","Y","á","ấ","ắ","é","ế","í","ó","ố","ớ","ú","ứ","ý","Á","Ấ","Ắ","É","Ế","Í","Ó","Ố","Ớ","Ú","Ứ","Ý","à","ầ","ằ","è","ề","ì","ò","ồ","ờ","ù","ừ","ỳ","À","Ầ","Ằ","È","Ề","Ì","Ò","Ồ","Ờ","Ù","Ừ","Ỳ","ạ","ậ","ặ","ẹ","ệ","ị","ọ","ộ","ợ","ụ","ự","ỵ","Ạ","Ậ","Ặ","Ẹ","Ệ","Ị","Ọ","Ộ","Ợ","Ụ","Ự","Ỵ","ả","ẩ","ẳ","ẻ","ể","ỉ","ỏ","ổ","ở","ủ","ử","ỷ","Ả","Ẩ","Ẳ","Ẻ","Ể","Ỉ","Ỏ","Ổ","Ở","Ủ","Ử","Ỷ","ã","ẫ","ẵ","ẽ","ễ","ĩ","õ","ỗ","ỡ","ũ","ữ","ỹ","Ã","Ẫ","Ẵ","Ẽ","Ễ","Ĩ","Õ","Ỗ","Ỡ","Ũ","Ữ","Ỹ","d","đ","D","Đ"
        };

        private readonly static string[] unicode = new string[] {
            "a","â","ă","e","ê","i","o","ô","ơ","u","ư","y","A","Â","Ă","E","Ê","I","O","Ô","Ơ","U","Ư","Y","á","ấ","ắ","é","ế","í","ó","ố","ớ","ú","ứ","ý","Á","Ấ","Ắ","É","Ế","Í","Ó","Ố","Ớ","Ú","Ứ","Ý","à","ầ","ằ","è","ề","ì","ò","ồ","ờ","ù","ừ","ỳ","À","Ầ","Ằ","È","Ề","Ì","Ò","Ồ","Ờ","Ù","Ừ","Ỳ","ạ","ậ","ặ","ẹ","ệ","ị","ọ","ộ","ợ","ụ","ự","ỵ","Ạ","Ậ","Ặ","Ẹ","Ệ","Ị","Ọ","Ộ","Ợ","Ụ","Ự","Ỵ","ả","ẩ","ẳ","ẻ","ể","ỉ","ỏ","ổ","ở","ủ","ử","ỷ","Ả","Ẩ","Ẳ","Ẻ","Ể","Ỉ","Ỏ","Ổ","Ở","Ủ","Ử","Ỷ","ã","ẫ","ẵ","ẽ","ễ","ĩ","õ","ỗ","ỡ","ũ","ữ","ỹ","Ã","Ẫ","Ẵ","Ẽ","Ễ","Ĩ","Õ","Ỗ","Ỡ","Ũ","Ữ","Ỹ","d","đ","D","Đ"
        };
        public static string convertVietNameseText(string input)
        {
            // dấu unicode tổ hợp regex 
            if (!Regex.Match(input, @"[̣́̀̉̃]").Success)
                return input;
            for (int i = 0; i < unicodeToHop.Length; i++)
                input = Regex.Replace(input, unicodeToHop[i], unicode[i]);
            return input;
        }




        public static string RemoveSignAndLowerCaseVietnameseString(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                    return "";

                string[] VietnameseSigns = new string[]
                {

                    "aAeEoOuUiIdDyY",
                    "áàạảãâấầậẩẫăắằặẳẵ",
                    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                    "éèẹẻẽêếềệểễ",
                    "ÉÈẸẺẼÊẾỀỆỂỄ",
                    "óòọỏõôốồộổỗơớờợởỡ",
                    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                    "úùụủũưứừựửữ",
                    "ÚÙỤỦŨƯỨỪỰỬỮ",
                    "íìịỉĩ",
                    "ÍÌỊỈĨ",
                    "đ",
                    "Đ",
                    "ýỳỵỷỹ",
                    "ÝỲỴỶỸ"
                };
                str = str.Trim().ToLower();
                for (int i = 1; i < VietnameseSigns.Length; i++)
                    for (int j = 0; j < VietnameseSigns[i].Length; j++)
                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                return str;
            }
            catch (Exception)
            {
                return str;
                throw;
            }
        }

        public static bool IsEmail(string strEmail)
        {
            Regex rgxEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                       @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return rgxEmail.IsMatch(strEmail);
        }
        public static string GetListPhone(params object[] args)
        {
            string result = string.Empty;
            try
            {
                if (args.Count() <= 0)
                    return string.Empty;
                foreach (var value in args)
                {
                    if (value != null && !string.IsNullOrEmpty(value.ToString()) && !result.Contains(value.ToString()))
                        result += ";" + value;
                }
                if (!string.IsNullOrEmpty(result))
                    result = result.Remove(0, 1);
                return result;
            }
            catch
            {
                //ExceptionHandler.HandleException(e, "WebServiceExceptionPolicy");
            }
            return string.Empty;
        }

        public static string RemoveSpecialPhone(string input)
        {
            // Loại bỏ ký tự đặc biệt và khoảng trắng từ chuỗi sử dụng Regex
            return Regex.Replace(input, @"[^0-9]", "");
        }

        public static TimeSpan GetTimeSpan(string time)
        {
            var timeSpan = new TimeSpan();
            try
            {
                var splitTime = time.Split(':');
                if (splitTime.Count() >= 3)
                {
                    timeSpan = new TimeSpan(int.Parse(splitTime[0]), int.Parse(splitTime[1]), int.Parse(splitTime[2]));
                }
            }
            catch
            {
                // e = AssignParam(e,new Param("time", time));
                // ExceptionHandler.HandleException(e, "WebServiceExceptionPolicy");
            }
            return timeSpan;
        }

      

        /// <summary>
        /// Convert Unicode to NonUnicode
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns></returns>
        public static string NonUnicode(this string text)
        {
            string[] arr1 = new string[] {
                "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ", "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ","í","ì","ỉ","ĩ","ị","ó","ò","ỏ","õ",
                "ọ", "ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ","ú","ù","ủ","ũ","ụ","ư","ứ","ừ",
                "ử", "ữ","ự","ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] {
                "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d", "e","e","e","e","e","e","e","e","e","e","e","i","i","i","i","i","o","o","o","o",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","u","u","u","u","u","u","u","u",
                "u","u","u","y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }


        //----------------------------------------------------//
        public enum EncodeType
        {
            SHA_256
        }
        public static string secretKey = "auth_api_3sdf43rc11239hsdcnsc0esdcsd!asd0023";//line 1
        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        private static byte[] saltBytes = new byte[] { 5, 6, 1, 7, 4, 2, 8, 9 };

        public static string FortmatPhoneTo0(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9})*$");
            var fortmat = phoneRegex.Replace(phone, m => string.Format("0{0}", m.Groups[2].Value));
            return fortmat;
        }
        public static bool CheckIsMatch(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9})*$");
            Match m = phoneRegex.Match(phone);
            return m.Success;
        }

        public static string EncodePassword(string pass, string encodeType)
        {
            if (string.IsNullOrEmpty(encodeType) || encodeType.ToLower() == "sha256")
                return EncodePassword(pass, EncodeType.SHA_256);

            return "encode type " + encodeType + " not support";
        }

        public static string DecodePassword(string pass, string encodeType)
        {
            if (string.IsNullOrEmpty(encodeType) || encodeType.ToLower() == "sha256")
                return DecodePassword(pass, EncodeType.SHA_256);

            return "encode type " + encodeType + " not support";
        }

        public static string EncodePassword(string pass, EncodeType encodeType)
        {
            switch (encodeType)
            {
                case EncodeType.SHA_256:
                    return EncryptPassSHA256(pass);
                default:
                    return "encode type " + encodeType.ToString() + " not support";
            }
        }

        public static string DecodePassword(string encodePass, EncodeType encodeType)
        {
            switch (encodeType)
            {
                case EncodeType.SHA_256:
                    return DecryptPassSHA256(encodePass);
                default:
                    return "encode type " + encodeType.ToString() + " not support";
            }
        }

        public static string EncryptPassSHA256(string pass)
        {
            try
            {
                //string secretKey = Util.secretKey;

                var bytesToBeEncrypted = Encoding.UTF8.GetBytes(pass);
                var passwordBytes = Encoding.UTF8.GetBytes(secretKey);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                var bytesEncrypted = EncryptSHA256(bytesToBeEncrypted, passwordBytes);
                return Convert.ToBase64String(bytesEncrypted);
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        public static string DecryptPassSHA256(string encryptedText)
        {
            try
            {
                //string secretKey = Util.secretKey;
                // Get the bytes of the string
                var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
                var passwordBytes = Encoding.UTF8.GetBytes(secretKey);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                var bytesDecrypted = DecryptSHA256(bytesToBeDecrypted, passwordBytes);

                return Encoding.UTF8.GetString(bytesDecrypted);
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        private static byte[] EncryptSHA256(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] DecryptSHA256(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        //----------------------------------------------------//

        public static string RemoveSpecialCharacters(string str)
        {
            try
            {
                return Regex.Replace(str, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);
            }
            catch (Exception)
            {
                return str??""; 
            }
        }
        public static bool IsCharacterString(string value)
        {
            Regex mChact = new Regex(@"^[\p{L}\p{Mn}\p{Nd}\s,'-]*$");
            Match m = mChact.Match(value);
            return m.Success;
        }

        #region get children recursive
        public static List<Item> GetChildren(List<Item> items, int parentID)
        {
            return items.Where(item => item.ParentId == parentID).ToList();
        }

        public static List<int> GetChildrenRecursive(List<Item> items, List<int> result, int parentID)
        {
            var childrens = GetChildren(items, parentID);
            foreach (var child in childrens)
            {
                if (!result.Contains(child.Id))
                {
                    result.Add(child.Id);
                }
                GetChildrenRecursive(items, result, child.Id);
            }
            return result;
        }
        #endregion 

        public static Expression<Func<T, bool>> CombinePredicates<T>(IList<Expression<Func<T, bool>>> predicateExpressions, Func<Expression, Expression, BinaryExpression> logicalFunction)
        {
            Expression<Func<T, bool>> filter = null;

            if (predicateExpressions.Count > 0)
            {
                var firstPredicate = predicateExpressions[0];
                Expression body = firstPredicate.Body;
                for (int i = 1; i < predicateExpressions.Count; i++)
                {
                    body = logicalFunction(body, Expression.Invoke(predicateExpressions[i], firstPredicate.Parameters));
                }
                filter = Expression.Lambda<Func<T, bool>>(body, firstPredicate.Parameters);
            }

            return filter;
        }

        public static string GetDescriptionByName<T>(string name) 
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";  // Return a default value if name is null or empty
            }
            var type = typeof(T);
            var memberInfo = type.GetMember(name);

            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            return name; // Return the name if no description is found
        }
    }

    public class TreeView
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public List<TreeView> items { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
    }
    public static class FtpFunction
    {

        public static bool GetDirectoryExits(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool CreateDirectory(string path, string userNameFTP, string passwordFTP)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                request.Credentials = new NetworkCredential(userNameFTP, passwordFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                var resp = (FtpWebResponse)request.GetResponse();
                if (resp.StatusCode == FtpStatusCode.PathnameCreated)
                {
                    resp.Close();
                    return true;
                }
                resp.Close();
                return false;

            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
