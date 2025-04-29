using System;
using System.Security.Cryptography;

namespace ZenlessZoneZeroWiki.Models 
{
    public static class ApiKeyUtil
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();


        public static (string plain, string hashed) NewKey()
        {
            Span<byte> bytes = stackalloc byte[32];     
            _rng.GetBytes(bytes);

            string plain = Convert.ToBase64String(bytes);
            string hashed = Convert.ToHexString(
                                SHA256.HashData(bytes));

            return (plain, hashed);
        }
    }
}
