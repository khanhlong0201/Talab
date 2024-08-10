using System;
using System.Text.RegularExpressions;

namespace CORE_TALAB.Services
{
    public static class Validate
    {
        public static string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Mật khẩu không được nhập dưới 8 ký tự!";

            if (password.Length > 64)
                return "Mật khẩu không được nhập quá 64 ký tự!";

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasUpperChar.IsMatch(password))
                return "Mật khẩu hải chứa ít nhất 1 chữ viết hoa!";
            else if (!hasLowerChar.IsMatch(password))
                return "Mật khẩu phải chứa ít nhất 1 chữ viết thường!";
            else if (!hasNumber.IsMatch(password))
                return "Mật khẩu phải chứa ít nhất 1 chữ số!";
            else if (!hasSymbols.IsMatch(password))
                return "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt!";
            return null;
        }
    }
}
