using System.IO;
using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace WebAPI_Receta_Core.Utils
{
    public class ManageAES
    {
        public static void EncryptAesManaged(string raw)
        {
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var AES_key = Encoding.UTF8.GetBytes(config["AES_key"]);
            var AES_IV = Encoding.UTF8.GetBytes(config["AES_IV"]);

            //var AES_key = Encoding.UTF8.GetBytes("e7G9sR4pK2v8xH5tD1lA6oP0wQ3cZuYq");
            //var AES_IV = Encoding.UTF8.GetBytes("J9rP5m2s8A6o0W3c");
            try
            {
                // Create Aes that generates a new key and initialization vector (IV).
                // Same key must be used in encryption and decryption
                using (AesManaged aes = new AesManaged())
                {
                    // Encrypt string
                    //byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
                    byte[] encrypted = Encrypt(raw, AES_key, AES_IV);
                    // Print encrypted string
                    Console.WriteLine($"Encrypted data: {System.Text.Encoding.UTF8.GetString(encrypted)}");
                    //decrypt the bytes to a string.
                    //string decrypted = Decrypt(encrypted, aes.Key, aes.IV);
                    string decrypted = Decrypt(encrypted, AES_key, AES_IV);
                    // Print decrypted string. It should be same as raw data
                    Console.WriteLine($"Decrypted data: {decrypted}");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            Console.ReadKey();
        }
        public static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.
            using (var aes = new AesManaged())
            {
                // Create encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream
                    // to encrypt
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data
            return encrypted;
        }
        public static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
