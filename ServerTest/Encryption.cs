using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ServerTest
{
    class Encryption
    {
        public static byte[] encryptStream(byte[] plain/*, byte[] Key, byte[] IV*/)
        {
            byte[] encrypted; ;
            byte[] Key = Encoding.ASCII.GetBytes("RDBKRbDR5PugyHi6kaiSppCmGgzBIcez");
            byte[] IV = Encoding.ASCII.GetBytes("zG7wwL0BksmUIlNG");

            using (MemoryStream mstream = new MemoryStream())
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.BlockSize = 128;
                    aesProvider.KeySize = 256;
                    aesProvider.Key = Key;
                    aesProvider.IV = IV;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;


                    using (CryptoStream cryptoStream = new CryptoStream(mstream,
                        aesProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plain, 0, plain.Length);
                    }
                    encrypted = mstream.ToArray();
                }
            }
            return encrypted;
        }

        public static byte[] mydec(byte[] encrypted/*, byte[] Key, byte[] IV*/)
        {
            byte[] plain;
            byte[] Key = Encoding.ASCII.GetBytes("RDBKRbDR5PugyHi6kaiSppCmGgzBIcez");
            byte[] IV = Encoding.ASCII.GetBytes("zG7wwL0BksmUIlNG");

            using (MemoryStream mStream = new MemoryStream(encrypted)) //add encrypted
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.BlockSize = 128;
                    aesProvider.KeySize = 256;
                    aesProvider.Key = Key;
                    aesProvider.IV = IV;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;

                    using (CryptoStream cryptoStream = new CryptoStream(mStream,
                        aesProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                    {
                        //cryptoStream.Read(encrypted, 0, encrypted.Length);
                        using (StreamReader stream = new StreamReader(cryptoStream))
                        {
                            string sf = stream.ReadToEnd();
                            plain = System.Text.Encoding.Default.GetBytes(sf);
                        }
                    }
                }
            }
            return plain;
        }
    }
}
