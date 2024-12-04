using System;
using System.Text;

namespace Gamm.Agent
{
    public static class ExtensionMethods
    {
        public static string GetUtfString(string str)
        {
            if (String.IsNullOrEmpty(str)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            var resultStr = Encoding.UTF8.GetString(bytes);
            return resultStr;
        }
    }
}