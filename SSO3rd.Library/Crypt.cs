#define  STATIC_KEYS

using SSO3rd.Library;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SSO3rd.Library
{

    /// <summary>
    /// Basic static crypt class
    /// </summary>
    public static class Crypt
    {

        /// <summary>
        /// Encrypt encrypts a string with  1x AES then 3x TripleDES encryption
        /// </summary>
        /// <param name="plain">plain text to encrypt</param>
        /// <returns>hex string of encrypted bytes</returns>
        public static string Encrypt(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return string.Empty;

            // 1x AES encryption
            AES aes = new AES();
            byte[] am = aes.EncryptBytes(System.Text.Encoding.UTF8.GetBytes(plain));

            // 3x Triple DES encryption                       
            DES3 des3 = new DES3();
            byte[] dam = des3.EncryptBytes(am);
            des3 = new DES3();
            byte[] des = des3.EncryptBytes(dam);
            des3 = new DES3();
            byte[] cryptBytes = des3.EncryptBytes(des);

            string cryptText = Crypt.ToHex(cryptBytes);

            return cryptText;
        }

        /// <summary>
        /// Decrypt decrypts an previous encrypted hexstring to plain text
        /// with 3x TripleDES => 1x AES
        /// </summary>
        /// <param name="crypt">crypted text</param>
        /// <returns>plain text</returns>
        public static string Decrypt(string crypt)
        {
            if (string.IsNullOrEmpty(crypt)) return string.Empty;

            // 3x Triple DES decryption
            byte[] cryptBytes = Crypt.HexToBytes(crypt);
            DES3 des3 = new DES3();
            byte[] am = des3.DecryptBytes(cryptBytes);
            des3 = new DES3();
            byte[] dam = des3.DecryptBytes(am);
            des3 = new DES3();
            byte[] des = des3.DecryptBytes(dam);

            // 1x AES decryption
            AES aes = new AES();
            byte[] plainBytes = aes.DecryptBytes(des);

            string plainText = System.Text.Encoding.UTF8.GetString(plainBytes);

            return plainText.TrimEnd("\0".ToCharArray());
        }

        /// <summary>
        /// TarBytes tars a lot of byte[]
        /// </summary>
        /// <param name="baseBytes">base byte array</param>
        /// <param name="bytesToAdd">more byte arrays</param>
        /// <returns>large tared byte array</returns>
        public static byte[] TarBytes(byte[] baseBytes, params byte[][] bytesToAdd)
        {
            List<byte> largeBytesList = [.. baseBytes];
            foreach (byte[] bs in bytesToAdd)
            {
                largeBytesList.AddRange(bs);
            }
            return largeBytesList.ToArray(); // [.. largeBytesList]
        }

        /// <summary>
        /// Encode ToHex converts a binary byte array to hex string
        /// </summary>
        /// <param name="inBytes">byte array</param>
        /// <returns>hex string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToHex(byte[] inBytes)
        {
            if (inBytes == null || inBytes.Length == 0)
                throw new ArgumentNullException("inBytes", "public static string ToHex(byte[] inBytes == NULL)");

            string hexString = string.Empty;
            for (int wc = 0; wc < inBytes.Length; wc++)
            {
                hexString += string.Format("{0:x2}", inBytes[wc]);
            }

            // string strUtf8 = System.Text.Encoding.UTF8.GetString(inBytes);
            return hexString;
        }

        /// <summary>
        /// HexToBytes ransforms a hex string to binary byte array
        /// </summary>
        /// <param name="hexStr">hex string</param>
        /// <returns>binary byte array</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] HexToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
                throw new ArgumentNullException("hexStr", "public static byte[] FromHex(string hexStr), hexStr == NULL || hexStr == \"\"");

            List<byte> bytes = new List<byte>();

            for (int wb = 0; wb < hexStr.Length; wb += 2)
            {
                char msb, lsb;
                if (wb == hexStr.Length - 1)
                {
                    msb = '0';
                    lsb = hexStr[wb];
                }
                else
                {
                    msb = hexStr[wb];
                    lsb = hexStr[wb + 1];
                }
                string sb = msb.ToString() + lsb.ToString();
                byte b = Convert.ToByte(sb, 16);
                bytes.Add(b);
            }

            return bytes.ToArray(); // [.. bytes]

        }

    }

    #region En-/DeCrypt extension methods
    public static class CryptExtensions
    {

        /// <summary>
        /// Extension method for sting - encrypts plain with 1x AES, then 3 x 3-DES
        /// </summary>
        /// <param name="plain"><see cref="string">plain text</see></param>
        /// <returns><see cref="string">encrypted string</see></returns>
        public static string EnCrypt(this string plain) => Crypt.Encrypt(plain);


        /// <summary>
        /// Extension method for sting - decrypts encrypted text with 3 x 3-DES, then 1 x AES
        /// </summary>
        /// <param name="encrypted"><see cref="string">encrypted text</see></param>
        /// <returns><see cref="string">decrypted plain text</see></returns>
        public static string DeCrypt(this string encrypted) => Crypt.Decrypt(encrypted);

    }
    #endregion En-/DeCrypt extension methods

    #region AES 3-DES implementation

    /// <summary>
    /// 3 x 3 Des native, for 3 x Triple DES encryption/decryption
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.tripledes.-ctor?view=net-8.0" />
    /// <seealso cref="https://www.c-sharpcorner.com/article/tripledes-encryption-and-decryption-in-c-sharp/"/>
    /// </summary>
    public class DES3
    {
        static string SecretKey =>
#if STATIC_KEYS
            "Truter-Puter; Katze, Finger, Panda Raslaggs Faz";
#else
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                (!string.IsNullOrEmpty(SettingsKeyReader.HostDomainName) ?
                    SettingsKeyReader.HostDomainName : Environment.MachineName))).Substring(0, 16);
#endif

        #region properties

        private static readonly int DesKeyLen = 16;
        private static readonly string DES3_KEY = SecretKey;
        private static readonly string DES3_IV = "SecretIV";

        protected internal byte[] DesKey { get; private set; } = Array.Empty<byte>(); // []
        protected internal byte[] DesIv { get; private set; } = Array.Empty<byte>(); // []

        protected internal TripleDES Des3;

        protected internal static ICryptoTransform? CryptTrans = null;

        #endregion properties

        #region ctor helpers

        internal void Gen3DesKey(ref byte[] keyBytes)
        {
            List<byte> span = new List<byte>(keyBytes); // [.. keyBytes]
            while (span.Count < DesKeyLen)
                span.AddRange(keyBytes);

            DesKey = new byte[DesKeyLen];
            Array.Copy(span.ToArray(), 0, DesKey, 0, DesKeyLen);

            keyBytes = new byte[DesKeyLen];
            Array.Copy(span.ToArray(), 0, keyBytes, 0, DesKeyLen);

            return;
        }

        internal void Gen3DesIv(byte[] keyBytes, ref byte[] ivBytes)
        {
            int iVLenght = 0;
            using (TripleDES tripleDes = TripleDES.Create())
            {
                tripleDes.Key = keyBytes;
                tripleDes.GenerateIV();
                iVLenght = tripleDes.IV.Length;
                DesIv = new byte[iVLenght];
                if (iVLenght > DesKeyLen)
                {
                    while (ivBytes.Length < iVLenght)
                        ivBytes = Crypt.TarBytes(ivBytes, ivBytes);
                }

                Array.Copy(ivBytes, 0, DesIv, 0, iVLenght);

                ivBytes = new byte[iVLenght];
                Array.Copy(DesIv, 0, ivBytes, 0, iVLenght);

                tripleDes.Clear();
            }

            return;
        }

        #endregion ctor helpers

        #region ctor

        public DES3() : this(DES3_KEY, DES3_IV) { }

        public DES3(string desKey, string hash)
        {
            if (string.IsNullOrEmpty(desKey))
                desKey = DES3_KEY;
            if (string.IsNullOrEmpty(hash))
                hash = DES3_IV;

            byte[] key3Des = Encoding.UTF8.GetBytes(desKey);
            byte[] iv3Des = Encoding.UTF8.GetBytes(hash);
            Gen3DesKey(ref key3Des);
            Gen3DesIv(DesKey, ref iv3Des);

            // MD5 md5 = new MD5CryptoServiceProvider();
            // DesKey = md5.ComputeHash(desKey);
            if (Des3 == null)
            {
                Des3 = TripleDES.Create();
                Des3.Key = DesKey;
                Des3.IV = DesIv;
                Des3.Mode = CipherMode.CFB;
                Des3.Padding = PaddingMode.Zeros;
            }
        }

        public DES3(byte[] desKey, byte[] desIv)
        {
            if (desKey == null || desKey.Length == 0)
            {
                desKey = Encoding.UTF8.GetBytes(DES3_KEY);
                desIv = Encoding.UTF8.GetBytes(DES3_IV);
            }

            // MD5 md5 = new MD5CryptoServiceProvider(); // DesKey = md5.ComputeHash(desKey);
            Gen3DesKey(ref desKey);
            Gen3DesIv(DesKey, ref desIv);
            if (Des3 == null)
            {
                Des3 = TripleDES.Create();
                Des3.Key = DesKey;
                Des3.IV = DesIv;
                Des3.Mode = CipherMode.CFB;
                Des3.Padding = PaddingMode.Zeros;
            }
        }

        #endregion ctor

        #region En-/DeCrypt

        /// <summary>
        /// 3Des encrypt bytes
        /// </summary>
        /// <param name="inBytes">Hex bytes</param>
        /// <returns>byte[] encrypted bytes</returns>
        internal byte[] EncryptBytes(byte[] inBytes)
        {
            if (inBytes == null || inBytes.Length == 0)
                throw new ArgumentNullException("inBytes");

            if (Des3 == null)
            {
                Des3 = TripleDES.Create();
                Des3.Key = DesKey;
                Des3.IV = DesIv;
                Des3.Mode = CipherMode.CFB;
                Des3.Padding = PaddingMode.Zeros;
            }
            CryptTrans = Des3.CreateEncryptor();

            byte[] cryptedBytes = CryptTrans.TransformFinalBlock(inBytes, 0, inBytes.Length);
            Des3.Clear();

            return cryptedBytes;
        }

        /// <summary>
        /// 3Des decrypt bytes
        /// </summary>
        /// <param name="cipherBytes">Hex bytes encrypted</param>
        /// <returns>byte[] decrypted bytes</returns>
        internal byte[] DecryptBytes(byte[] cipherBytes)
        {
            // Check arguments. 
            if (cipherBytes == null || cipherBytes.Length <= 0)
                throw new ArgumentNullException("cipherBytes");

            if (Des3 == null)
            {
                Des3 = TripleDES.Create();
                Des3.Key = DesKey;
                Des3.IV = DesIv;
                Des3.Mode = CipherMode.CFB;
                Des3.Padding = PaddingMode.Zeros;
            }
            CryptTrans = Des3.CreateDecryptor();

            byte[] decryptedBytes = CryptTrans.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            Des3.Clear();

            // return decrypted byte[]
            return decryptedBytes;
        }


        #endregion En-/DeCrypt

        #region EnDeCryptString

        /// <summary>
        /// 3Des encrypt string
        /// </summary>
        /// <param name="inString">string in plain text</param>
        /// <returns>Base64 encoded encrypted byte array</returns>
        public string EncryptString(string inString)
        {
            byte[] inBytes = Encoding.UTF8.GetBytes(inString);
            byte[] cipherBytes = EncryptBytes(inBytes);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// 3Des decrypts string
        /// </summary>
        /// <param name="cipherText">Base64 encoded encrypted byte[]</param>
        /// <returns>plain text string</returns>
        public string DecryptString(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] plainBytes = DecryptBytes(cipherBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }

        #endregion EnDeCryptString       

    }

    /// <summary>
    /// AES native .Net Aes RijndaelManaged without bouncy castle
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0" />
    /// </summary>
    public class AES
    {

        static string SecretKey => Convert.ToBase64String(Encoding.UTF8.GetBytes(
#if STATIC_KEYS
            "Truter-Puter; Katze, Finger, Panda Raslaggs Faz"
#else
            !string.IsNullOrEmpty(SettingsKeyReader.HostDomainName) ?
                    SettingsKeyReader.HostDomainName : Environment.MachineName
#endif
        ));

        #region properties

        static readonly string AES_KEY = SecretKey;
        static readonly string AES_IV = Convert.ToBase64String(Encoding.UTF8.GetBytes("Secret Key"));

        protected internal static readonly int AesKeyLen = 32;
        protected internal static byte[] AesKey { get; private set; } = new byte[AesKeyLen];

        protected internal static byte[] AesIv { get; private set; } = Array.Empty<byte>(); // []

        protected internal static Aes AesAlgo { get; private set; }

        #endregion properties

        #region ctor helpers

        internal void GenAesKey(ref byte[] keyBytes)
        {
            List<byte> span = new List<byte>(keyBytes); //  [.. keyBytes]
            while (span.Count < AesKeyLen)
                span.AddRange(keyBytes);

            AesKey = new byte[AesKeyLen];
            Array.Copy(span.ToArray(), 0, AesKey, 0, AesKeyLen);
            keyBytes = new byte[AesKeyLen];
            Array.Copy(span.ToArray(), 0, keyBytes, 0, AesKeyLen);

        }

        internal void GenAesIv(byte[] keyBytes, ref byte[] ivBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV();
                int iVLenght = aesAlg.IV.Length;
                AesIv = new byte[iVLenght];
                if (iVLenght > AesKeyLen)
                {
                    while (ivBytes.Length < iVLenght)
                        ivBytes = Crypt.TarBytes(ivBytes, ivBytes);
                    Array.Copy(ivBytes, 0, AesIv, 0, iVLenght);
                }
                else
                    Array.Copy(ivBytes, 0, AesIv, 0, iVLenght);

                ivBytes = new byte[iVLenght];
                Array.Copy(AesIv, 0, ivBytes, 0, iVLenght);
            }
        }

        protected internal Aes GetAesAlgo()
        {
            if (AesAlgo == null)
            {
                AesAlgo = Aes.Create();
                AesAlgo.Key = AesKey;
                AesAlgo.IV = AesIv;
                // AesAlgo.KeySize = AesKeyLen;
                AesAlgo.Mode = CipherMode.CFB;
                AesAlgo.Padding = PaddingMode.Zeros;
            }
            return AesAlgo;
        }

        #endregion ctor helpers

        #region ctor

        public AES() : this(AES_KEY, AES_IV)
        {
            GetAesAlgo();
        }

        public AES(string key, string hash)
        {
            if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(hash))
            {
                key = AES_KEY;
                hash = AES_IV;
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] hashBytes = Encoding.UTF8.GetBytes(hash);

            try
            {
                GenAesKey(ref keyBytes);
                GenAesIv(AesKey, ref hashBytes);
            }
            catch (Exception e)
            {
                SSOLog.LogOriginMsgEx(typeof(AES).GetCallerInfo(1),
                    $"ctor AES(string key = {key}, string hash = {hash}) throwed {e.GetType().Name}", e);
                AesKey = Encoding.UTF8.GetBytes(AES_KEY);
                AesIv = Encoding.UTF8.GetBytes(AES_IV);
            }

            GetAesAlgo();
        }

        public AES(byte[] aesKey, byte[] aesIv)
        {
            if (aesKey == null || aesKey.Length == 0)
                aesKey = Encoding.UTF8.GetBytes(AES_KEY);
            if (aesIv == null || aesIv.Length == 0)
                aesIv = Encoding.UTF8.GetBytes(AES_IV);

            GenAesKey(ref aesKey);
            GenAesIv(aesKey, ref aesIv);

            GetAesAlgo();
        }

        #endregion ctor

        #region en-/decrypt

        /// <summary>
        /// AES Encrypt by using RijndaelManaged
        /// </summary>
        /// <param name="plainData">Array of plain data byte</param>
        /// <returns>Array of encrypted data byte</returns>
        /// <exception cref="ArgumentNullException">is thrown when input enrypted <see cref="T:byte[]"/> is null or zero length</exception>
        public byte[] EncryptBytes(byte[] plainData)
        {
            // Check arguments. 
            if (plainData == null || plainData.Length <= 0)
                throw new ArgumentNullException("plainData is null or length = 0 in static byte[] EncryptBytes(byte[] plainData)...");

            // create a decryptor by AesAlgo.CreateEncrypto(AesAlgo.Key, AesAlgo.IV);
            ICryptoTransform encryptor = AesAlgo.CreateEncryptor(AesAlgo.Key, AesAlgo.IV);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);

            // return the encrypted bytes
            return encryptedBytes;

        }

        /// <summary>
        /// AES Decrypt by using RijndaelManaged
        /// </summary>
        /// <param name="encryptedBytes">Array of encrypted data byte</param>
        /// <returns>Array of plain data byte</returns>
        /// <exception cref="ArgumentNullException">is thrown when input enrypted <see cref="T:byte[]"/> is null or zero length</exception>
        public byte[] DecryptBytes(byte[] encryptedBytes)
        {
            // Check arguments. 
            if (encryptedBytes == null || encryptedBytes.Length <= 0)
                throw new ArgumentNullException("ArgumentNullException encryptedBytes = null or Lenght 0 in static string DecryptBytes(byte[] encryptedBytes)...");

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = AesAlgo.CreateDecryptor(AesAlgo.Key, AesAlgo.IV);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return decryptedBytes;
        }

        #endregion en-/decrypt

        #region EnDecryptString

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="inPlainString">plain text string</param>
        /// <returns>Base64 encoded encrypted byte[]</returns>
        public string EncryptString(string inPlainString)
        {
            byte[] plainTextData = Encoding.UTF8.GetBytes(inPlainString);
            byte[] encryptedData = EncryptBytes(plainTextData);
            return Convert.ToBase64String(encryptedData, Base64FormattingOptions.None);
        }

        /// <summary>
        /// Decrypts a string, that is truely a base64 encoded encrypted byte[]
        /// </summary>
        /// <param name="inCryptString">base64 encoded string from encrypted byte[]</param>
        /// <returns>plain text string (decrypted)</returns>
        public string DecryptString(string inCryptString)
        {
            byte[] cryptData = Convert.FromBase64String(inCryptString);
            byte[] plainTextData = DecryptBytes(cryptData);
            return Encoding.UTF8.GetString(plainTextData);
        }

        #endregion EnDecryptString

    }

    #endregion AES 3-DES implementation


}

