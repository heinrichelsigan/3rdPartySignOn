using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using ThirdPartySignOn.MSIdentity.Data;

namespace ThirdPartySignOn.MSIdentity.Data
{
    public static class CryptExtensions
    {

        /// <summary>
        /// Extension method for sting - encrypts plain with 1x AES, then 3 x 3-DES
        /// </summary>
        /// <param name="plain"><see cref="string">plain text</see></param>
        /// <returns><see cref="string">encrypted string</see></returns>
        public static string EnCrypt(this string plain) => SSO3rd.Library.Crypt.Encrypt(plain);


        /// <summary>
        /// Extension method for sting - decrypts encrypted text with 3 x 3-DES, then 1 x AES
        /// </summary>
        /// <param name="encrypted"><see cref="string">encrypted text</see></param>
        /// <returns><see cref="string">decrypted plain text</see></returns>
        public static string DeCrypt(this string encrypted) => SSO3rd.Library.Crypt.Decrypt(encrypted);


        /// <summary>
        /// <see cref="T:byte[]"/>.TarBytes extension method: tars 
        /// </summary>
        /// <param name="baseBytes">base byte array</param>
        /// <param name="bytesToAdd">more byte arrays</param>
        /// <returns>large tared byte array</returns>
        public static byte[] TarBytes(this byte[] baseBytes, params byte[][] bytesToAdd) => SSO3rd.Library.Crypt.TarBytes(baseBytes, bytesToAdd);

        /// <summary>
        /// Encode ToHex converts a binary byte array to hex string
        /// </summary>
        /// <param name="inBytes">this byte array</param>
        /// <returns>hex string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToHex(this byte[] inBytes) => SSO3rd.Library.Crypt.ToHex(inBytes);

        /// <summary>
        /// HexToBytes ransforms a hex string to binary byte array
        /// </summary>
        /// <param name="hexStr">this hex string</param>
        /// <returns>binary byte array</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] HexToBytes(this string hexStr) => SSO3rd.Library.Crypt.HexToBytes(hexStr);

    }

}
