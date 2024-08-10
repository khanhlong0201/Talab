using System.Text.RegularExpressions; 

namespace CORE_TALAB.Common
{
    public class CommonCulture
    {
        public bool CheckIsMatch(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9})*$");
            Match m = phoneRegex.Match(phone);
            return m.Success;
        }

        public string FortmatPhoneTo0(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9})*$");
            var fortmat = phoneRegex.Replace(phone, m => string.Format("0{0}", m.Groups[2].Value));

            return fortmat;
        }

        public string FortmatPhoneTo84(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9})*$");
            var fortmat = phoneRegex.Replace(phone, m => string.Format("84{0}", m.Groups[2].Value));

            return fortmat;
        }

        public static string CheckTypeFile(string url)
        {
            try
            { 
                var result = "";
                if (url != "") {
                    url = url.Substring(url.LastIndexOf('.') + 1); 
                }
                switch (url.ToLower())
                {
                    case "jpg":
                        result = "IMAGE"; 
                        break;
                    case "docx":
                        result = "WORD";
                        break;
                    case "xlsx":
                        result = "EXCEL";
                        break;
                    case "pdf":
                        result = "PDF";
                        break;
                    default: result = "";
                        break;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
    }
}
