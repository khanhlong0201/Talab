namespace CORE_TALAB.Util
{
    using System;
    using System.Text.RegularExpressions;

    public abstract class Validate
    {
        public string removeSpecialCharater(string value) // chưa test
        {
            return value.Replace(@"/[^\x00-\x7F]/g", "").Trim();
        }
        public static bool checkFloat(string svalue)
        {
            return Regex.IsMatch(svalue, @"^(\d*)([.]{0,1})(\d{0,5})$");
        }

        public static bool checkString(string svalue)
        {
            if (Regex.IsMatch(svalue, "^([0-9]{1,})(.*)([0-9]{1,})$"))
            {
                if (Regex.IsMatch(svalue, @"(([0-9]{1,})([\+\-\*\/]{2,})([0-9]{1,}))|(([0-9]{1,})([^\+\-\*\/]{1,})([0-9]{1,}))"))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool isEmail(string str)
        {
            return Regex.IsMatch(str, @"^([\w]*)([@]?)([\w]*)([.]?)([\w]{1,3})$");
        }

        public static bool isInt(string str)
        {
            return Regex.IsMatch(str, @"^([+-]?)\d*$");
        }

        public static bool isNumeric(string str)
        {
            return Regex.IsMatch(str, @"^([+-]?)\d*[.]?\d*$");
        }

        public static bool isUnInt(string str)
        {
            return Regex.IsMatch(str, @"^\d*$");
        }

        public static bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static bool ValidateAccount(string str)
        {
            return Regex.IsMatch(str, "[0-9a-z_A-Z]{" + str.Length + "}");
        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static bool ValidateIPPort(string str)
        {
            return Regex.IsMatch(str, ":[1-9]{1,}$");
        }

        public static bool ValidateNumber(string strIn)
        {
            if (strIn.Length > 0)
            {
                string str = "0123456789.-+";
                for (int i = 0; i < strIn.Length; i++)
                {
                    if (str.IndexOf(strIn[i]) == -1)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool ValidatePassword(string str)
        {
            return Regex.IsMatch(str, "[0-9a-zA-Z]{" + str.Length + "}");
        }

        public static bool ValidatePhone(string strIn)
        {
            if (strIn.Length > 0)
            {
                string str = "0123456789+";
                for (int i = 0; i < strIn.Length; i++)
                {
                    if (str.IndexOf(strIn[i]) == -1)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool ValidateSQLString(string str)
        {
            return !Regex.IsMatch(str, "'+");
        }

        public static bool ValidateVehicleID(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            return Regex.IsMatch(str, "[0-9]{" + str.Length + "}");
        }


        /// Hoang
        public static bool ValidateCharacterNumber0_256(string str)
        {
            //chữ, số, "-", "/",space
            return Regex.IsMatch(str, @"^[a-zA-Z0-9\-/\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]{1,256}$");
        }
        public static bool ValidateCharacterNumber0(int number=0,string str="")
        {
            //chữ, số, "-", "/",space
            string tmp = "+";
            if(number>0) tmp ="{1,"+number+"}";
            return Regex.IsMatch(str, @"^[a-zA-Z0-9\-/\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateCharacterNumber1(int number = 0, string str = "")
        {
            //chữ, số, "-", space
            string tmp = "+";
            if (number > 0) tmp = "{1," + number + "}";
            return Regex.IsMatch(str, @"^[a-zA-Z0-9\-\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateCharacterNumber2(int number = 0, string str = "")
        {
            //chữ, số, ",", space
            string tmp = "+";
            if (number > 0) tmp = "{1," + number + "}";
            return Regex.IsMatch(str, @"^[a-zA-Z0-9,\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateCharacterNumber4(int number = 0, string str = "")
        {
            //chữ, số, "-" ,"." 
            string tmp = "+";
            if (number > 0) tmp = "{1," + number + "}";
            return Regex.IsMatch(str, @"^[a-z0-9A-Z\-.ÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateOnlyCharacterAndNumber(int number = 0, string str = "")
        { //chữ, số, "-" ,"." 
            string tmp = "+";
            if (number > 0) tmp = "{1," + number + "}";
            return Regex.IsMatch(str, @"^[a-z0-9A-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateOnlyCharacterAndSpace(string str = "")
        { //chữ, số, space
            string tmp = "+";
            return Regex.IsMatch(str, @"^[a-zA-Z\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        } 
        public static bool ValidateOnlyCharacterAndLength(string str = "", int minNumber = 3, int maxNumber = 100)
        { //chữ, số, "-" ,"." 
            string tmp = "+";
            tmp = "{"+ minNumber + "," + maxNumber + "}";
            return Regex.IsMatch(str, @"^[a-zA-Z\sÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]" + tmp + "$");
        }
        public static bool ValidateVehicleNumber( string str = "")
        {
            return Regex.IsMatch(str, @"^[0-9A-Z\-.]{4,12}$");
        }
        public static bool ValidatePhoneNumber(string strIn)
        {
            return Regex.IsMatch(strIn, @"(^[\+84|84|0]+(2|3|5|7|8|9))+([0-9]{8,9}$)\b");
        }
        public static bool ValidateChuHoa(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[A-Z].*");
        }
        public static bool ValidateSo(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[0-9].*");
        }
        public static bool ValidateChuThuong(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[a-z].*");
        }
        public static bool ValidateKyTuDacBiet(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[`!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~].*");
        }
        public static bool ValidateCMND(string str)
        {

            return Regex.IsMatch(str, @"^[0-9]{9,12}$");
        }
        public static bool ValidateDate(string str)
        {
            return Regex.IsMatch(str, @"^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$");
        }
        public static bool ValidateTax(string str)
        {

            return Regex.IsMatch(str, @"^[0-9]{10,13}$");
        }

        public static string ValidPhoneNumber(string phoneNumber)
        {
            phoneNumber = Utils.ReplaceWhitespace(phoneNumber?.Trim(), "");
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return "Vui lòng nhập số điện thoại!";
            }
            if (!ValidateSo(phoneNumber))
            {
                return "Số điện thoại chỉ được phép nhập số!";
            }
            if (phoneNumber.StartsWith("+84"))
            {
                phoneNumber = phoneNumber.Replace("+84", "0");
            }
            if (phoneNumber.StartsWith("84"))
            {
                phoneNumber = "0" + phoneNumber.Substring(2);
            }
            if (phoneNumber.Length < 10)
                return "Số điện thoại đang <10 số!";
            if (phoneNumber.Length > 12)
                return "Số điện thoại đang >11 số!";
            if (!Validate.ValidatePhoneNumber(phoneNumber))
                return "Số điện thoại không đúng định dạng!";
            return "true";
        }
    }
}

