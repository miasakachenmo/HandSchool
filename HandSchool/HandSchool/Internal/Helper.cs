﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HandSchool.Internal
{
    class Helper
    {
        private static MD5 MD5p = new MD5CryptoServiceProvider();
        private static StringBuilder sb = new StringBuilder();
        private static JsonSerializer json = JsonSerializer.Create();

        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash;
            try
            {
                bytHash = MD5p.ComputeHash(source);
            }
            catch (ObjectDisposedException e)
            {
                MD5p = new MD5CryptoServiceProvider();
                bytHash = MD5p.ComputeHash(source);
            }
            MD5p.Clear();
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
            return json.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
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