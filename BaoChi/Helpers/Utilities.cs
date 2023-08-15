using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaoChi.Helpers
{
    public static class Utilities
    {
        //3 cái xài chính
        public static int PAGE_SIZE = 10;
        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            url = Regex.Replace(url, @"[éèẹẻẽêếềệểễ]", "e");
            url = Regex.Replace(url, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            url = Regex.Replace(url, @"[íìịỉĩ]", "i");
            url = Regex.Replace(url, @"[ýỳỵỉỹ]", "y");
            url = Regex.Replace(url, @"[úùụủũưứừựửữ]", "u");
            url = Regex.Replace(url, @"[đ]", "d");

            //2. Chỉ cho phép nhận:[0-9a-z-\s]
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            //xử lý nhiều hơn 1 khoảng trắng --> 1 kt
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            //thay khoảng trắng bằng -
            url = Regex.Replace(url, @"\s", "-");
            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower())) // Khác các file định nghĩa
                {
                    return null;
                }
                else
                {
                    string fullPath = path + "//" + newname;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch
            {
                return null;
            }
        }

        public static int GetFileSize(string urlfile)
        {
            int sizeFile = 0;
            try
            {
                Uri uriPath = new Uri(urlfile);
                var webRequest = HttpWebRequest.Create(uriPath);
                webRequest.Method = "HEAD";
                using(var webResponse = webRequest.GetResponse())
                {
                    var fileSize = webResponse.Headers.Get("Content-Length");
                    var fileSizeInMegaByte = Math.Round(Convert.ToDouble(fileSize) / 1024.0 / 1024.0, 2);
                    sizeFile = Convert.ToInt32(fileSizeInMegaByte);
                }
            }
            catch
            {
                return sizeFile;
            }
            return sizeFile;
        }
        public static string ToTitleCase (string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for(int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if(s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static bool KiemTraHoVaTen(string input)
        {
            bool results = false;
            try
            {
                Match match = Regex.Match(input, @"(\d+)");
                if (match.Success)
                {
                    var numer = int.Parse(match.Groups[1].Value);
                    return true;
                }
            }
            catch
            {
                return true;
            }
            return results;
        }
        public static bool IsInteger (string str)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            try
            {
                if (String.IsNullOrEmpty(str))
                {
                    return false;
                }
                if (!regex.IsMatch(str))
                {
                    return false;
                }
                return true;
            }
            catch
            {

            }
            return false;
        }
        public static bool IsNumber(this string aNumber)
        {
            BigInteger temp_big_int;
            var is_number = BigInteger.TryParse(aNumber, out temp_big_int);
            return is_number;
        }
        public static string GetDomain(string url)
        {
            string host = "";
            try
            {
                if(!string.IsNullOrEmpty(url))
                {
                    Uri myUri = new Uri(url.Trim().ToLower());
                    host = myUri.Host.ToLower();
                }
            }
            catch
            {
                host = "";
            }
            return host;
        }
        public static string RemoveLinks(string html)
        {
            try
            {
                if (!string.IsNullOrEmpty(html))
                {
                    Regex r = new Regex(@"\<a href=.*?\>");
                    html = r.Replace(html, "");
                    r = new Regex(@"\</a\>");
                    html = r.Replace(html, "");
                }
                return html;
            }
            catch
            {
                return html;
            }
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input,"<.*?>",String.Empty);
        }
        public static string Right(string value,int length)
        {
            return value.Substring(value.Length - length);
        }

        public static string GetRandomKey(int length = 5)
        {
            //chuỗi mẫu (pattern)
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static void Great_folder(string link)
        {
            string path = link;
            if(!Directory.Exists(@path))
            {
                Directory.CreateDirectory(@path);
            }
        }
        public static string RandTime()
        {
            Random r = new Random();
            string rand = DateTime.Now.ToString().Replace("/", ":").Replace(":", "-").Replace(" ", "-").ToLower();
            rand = rand + r.Next(100, 999);
            return rand;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> ExtractLink (string html)
        {
            List<string> list = new List<string>();
            Regex regex = new Regex("(?:href|src)[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if(regex.IsMatch(html))
            {
                foreach(Match match in regex.Matches(html)) 
                {
                    list.Add(match.Groups[1].Value); 
                }
            }
            return list;
        }
    }
}
