using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper
{
    public class Decrypt
    {
        public string DecryptString(string cipherText)
        {
            return DecryptBytes(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a byte array containing cipher text value and generates a
        /// string result.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        private string DecryptBytes(byte[] cipherTextBytes)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-encoded cipher text string to be decrypted.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        private byte[] DecryptToBytes(string cipherText)
        {
            return DecryptToBytes(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        private byte[] DecryptToBytes(byte[] cipherTextBytes)
        {
            byte[] decryptedBytes = null;
            byte[] plainTextBytes = null;
            int decryptedByteCount = 0;
            int saltLen = 0;

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Since we do not know how big decrypted value will be, use the same
            // size as cipher text. Cipher text is always longer than plain text
            // (in block cipher encryption), so we will just use the number of
            // decrypted data byte after we know how big it is.
            decryptedBytes = new byte[cipherTextBytes.Length];

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform decryption, we must use the Read mode.
                CryptoStream cryptoStream = new CryptoStream(
                                                   memoryStream,
                                                   CryptoTransform.decryptor,
                                                   CryptoStreamMode.Read);

                // Decrypting data and get the count of plain text bytes.
                decryptedByteCount = cryptoStream.Read(decryptedBytes,
                                                        0,
                                                        decryptedBytes.Length);
                // Release memory.
                memoryStream.Close();
                cryptoStream.Close();
            }

            // If we are using salt, get its length from the first 4 bytes of plain
            // text data.
            if (CryptoTransform.maxSaltLen > 0 && CryptoTransform.maxSaltLen >= CryptoTransform.minSaltLen)
            {
                saltLen = (decryptedBytes[0] & 0x03) |
                            (decryptedBytes[1] & 0x0c) |
                            (decryptedBytes[2] & 0x30) |
                            (decryptedBytes[3] & 0xc0);
            }

            // Allocate the byte array to hold the original plain text (without salt).
            plainTextBytes = new byte[decryptedByteCount - saltLen];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLen, plainTextBytes,
                        0, decryptedByteCount - saltLen);

            // Return original plain text value.
            return plainTextBytes;
        }
    }
}
