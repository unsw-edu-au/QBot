using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper
{
    public class Encrypt
    {
        public string EncryptString(string plainText)
        {
            return EncryptBytes(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a base64-encoded string.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a base64-encoded string.
        /// </returns>
        private string EncryptBytes(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
        }

        /// <summary>
        /// Encrypts a string value generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainText">
        /// Plain text string to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        private byte[] EncryptToBytes(string plainText)
        {
            return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        private byte[] EncryptToBytes(byte[] plainTextBytes)
        {
            // Add salt at the beginning of the plain text bytes (if needed).
            byte[] plainTextBytesWithSalt = Helper.AddSalt(plainTextBytes);

            // Encryption will be performed using memory stream.
            MemoryStream memoryStream = new MemoryStream();

            // Let's make cryptographic operations thread-safe.
            //lock (this)
            //{
            // To perform encryption, we must use the Write mode.
            CryptoStream cryptoStream = new CryptoStream(
                                               memoryStream,
                                               CryptoTransform.encryptor,
                                               CryptoStreamMode.Write);

            if (cryptoStream != null)
            {
                // Start encrypting data.
                cryptoStream.Write(plainTextBytesWithSalt,
                                    0,
                                   plainTextBytesWithSalt.Length);
            }
            // Finish the encryption operation.
            cryptoStream.FlushFinalBlock();

            // Move encrypted data from memory into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close memory streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Return encrypted data.
            return cipherTextBytes;
            //}
        }
    }
}
