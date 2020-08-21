using System;
using System.Linq;
using CaseFile.Api.Core.Services;

namespace CaseFile.Api.Business.Utils
{
    public class RandomNumberGenerator
    {
        public static string Generate(int digits)
        {
            Random random = new Random();
            string number = "";
            for (int i = 1; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }

        public static string GenerateWithPadding(int digits, string prefix)
        {
            Random random = new Random();
            string number = prefix;
            for (int i = 1 + prefix.Length; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }

    }

    public static class AccountHelper
    {
        public static string GetRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789%$#@!*^.-=";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}