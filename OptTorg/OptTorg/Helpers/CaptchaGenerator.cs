using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptTorg.Helpers
{
    internal class CaptchaGenerator
    {
        private static readonly Random _random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string Generate(int length = 8)
        {
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Chars[_random.Next(Chars.Length)];
            }
            return new string(result);
        }
    }
}
