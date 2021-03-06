﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    partial class Helper
    {
        private static StringBuilder sb = new StringBuilder();
        private static JsonSerializer json = JsonSerializer.Create();
        public static string[] ScheduleColors = { "#59e09e", "#f48fb1", "#ce93d8", "#ff8a65", "#9fa8da", "#42a5f5", "#80deea", "#c6de7c" };
        public static string DataBaseDir;
        public static string SegoeMDL2;
#if __UWP__
        public static Windows.UI.Color[] ScheduleColors2 = {
            Windows.UI.Color.FromArgb(0xff, 0x59, 0xe0, 0x9e),
            Windows.UI.Color.FromArgb(0xff, 0xf4, 0x8f, 0xb1),
            Windows.UI.Color.FromArgb(0xff, 0xce, 0x93, 0xd8),
            Windows.UI.Color.FromArgb(0xff, 0xff, 0x8a, 0x65),
            Windows.UI.Color.FromArgb(0xff, 0x9f, 0xa8, 0xda),
            Windows.UI.Color.FromArgb(0xff, 0x42, 0xa5, 0xf5),
            Windows.UI.Color.FromArgb(0xff, 0x80, 0xde, 0xea),
            Windows.UI.Color.FromArgb(0xff, 0xc6, 0xde, 0x7c)
        };
#endif

        public static int GetDeviceSpecified(string name)
        {
            return 0;
        }

        public static string ReadConfFile(string name)
        {
            string fn = Path.Combine(DataBaseDir, name);
            if (File.Exists(fn))
                return File.ReadAllText(Path.Combine(DataBaseDir, name));
            else
                return "";
        }

        public static void WriteConfFile(string name, string value)
        {
            File.WriteAllText(Path.Combine(DataBaseDir, name), value);
        }

        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash;
            using (MD5 MD5p = new MD5CryptoServiceProvider())
            {
                bytHash = MD5p.ComputeHash(source);
            }
            return bytHash;
        }
        
        public static string MD5(string source, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return HexDigest(MD5(encoding.GetBytes(source)), true);
        }

        public static T JSON<T>(string jsonString)
        {
            if (jsonString == "") throw new JsonReaderException();
            return json.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
        }

        public static string Serialize(object value)
        {
            json.Serialize(new JsonTextWriter(new StringWriter(sb)), value);
            var ret = sb.ToString();
            sb.Clear();
            return ret;
        }

        public static string HexDigest(byte[] source, bool lower = false)
        {
            char[] chars = (lower ? "0123456789abcdef" : "0123456789ABCDEF").ToCharArray();
            int bit;
            for (int i = 0; i < source.Length; i++)
            {
                bit = (source[i] & 0x0f0) >> 4;
                sb.Append(chars[bit]);
                bit = source[i] & 0x0f;
                sb.Append(chars[bit]);
            }
            string ret = sb.ToString();
            sb.Clear();
            return ret;
        }

        public static string HttpBuildQuery(Dictionary<string, string> dict, string startupDelimiter = "")
        {
            string result = string.Empty;
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(result))
                    result += startupDelimiter;
                else
                    result += "&";
                result += string.Format("{0}={1}", item.Key, item.Value);
            }
            return result;
        }
    }
}
