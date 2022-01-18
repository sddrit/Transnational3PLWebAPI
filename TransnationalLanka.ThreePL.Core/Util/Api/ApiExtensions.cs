using System;
using System.Security.Cryptography;
using System.Text;

namespace TransnationalLanka.ThreePL.Core.Util.Api
{
    public class ApiExtensions
    {
        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GenerateClientId(long supplierId)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(supplierId.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string GenerateSecret()
        {
            return GetUniqueKey(25);
        }

        private static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            var result = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

    }
}
