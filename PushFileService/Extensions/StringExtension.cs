using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PushFileService.Extensions
{
    public static class StringExtension
    {
        public static string ToSlug(this string str)
        {
            // Chuyển hết sang chữ thường
            str = str.ToLower();

            // xóa dấu
            str = Regex.Replace(str, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            str = Regex.Replace(str, "[èéẹẻẽêềếệểễ]", "e");
            str = Regex.Replace(str, "[ìíịỉĩ]", "i");
            str = Regex.Replace(str, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            str = Regex.Replace(str, "[ùúụủũưừứựửữ]", "u");
            str = Regex.Replace(str, "[ỳýỵỷỹ]", "y");
            str = Regex.Replace(str, "[đ]", "d");

            // Xóa ký tự đặc biệt
            str = Regex.Replace(str, "[^0-9a-z\\s-]", "");

            // Xóa khoảng trắng thay bằng ký tự -
            str = Regex.Replace(str, "\\s+", "-");

            // Xóa ký tự - liên tiếp
            str = Regex.Replace(str, "-+", "-");

            // xóa phần dư - ở đầu
            str = Regex.Replace(str, "^-+", "");

            // xóa phần dư - ở cuối
            str = Regex.Replace(str, "-+$", "");

            // return
            return str;
        } 
    }
}
