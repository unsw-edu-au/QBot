using System;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper
{
    public class CryptoTransform
    {
        #region Private members
        // If hashing algorithm is not specified, use SHA-1.
        private static string DEFAULT_HASH_ALGORITHM = "SHA256";

        // If key size is not specified, use the longest 256-bit key.
        private static int DEFAULT_KEY_SIZE = 256;

        // Do not allow salt to be longer than 255 bytes, because we have only
        // 1 byte to store its length. 
        private static int MAX_ALLOWED_SALT_LEN = 255;

        // Do not allow salt to be smaller than 4 bytes, because we use the first
        // 4 bytes of salt to store its length. 
        private static int MIN_ALLOWED_SALT_LEN = 4;

        // Random salt value will be between 4 and 8 bytes long.
        private static int DEFAULT_MIN_SALT_LEN = MIN_ALLOWED_SALT_LEN;
        private static int DEFAULT_MAX_SALT_LEN = 8;

        public static int minSaltLen
        {
            get; set;
        }
        public static int maxSaltLen
        {
            get; set;
        }

        public static ICryptoTransform encryptor
        {
            get; set;
        }
        public static ICryptoTransform decryptor
        {
            get; set;
        }
        // These members will be used to perform encryption and decryption.
        #endregion

        #region Constructors
        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with 256-bit key, derived using 1 password iteration,
        /// hashing without salt, no initialization vector, electronic codebook
        /// (ECB) mode, SHA-1 hashing algorithm, and 4-to-8 byte long salt.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <remarks>
        /// This constructor is not recommended because it does not use
        /// initialization vector and uses the ECB cipher mode, which is less
        /// secure than the CBC mode.
        /// </remarks>
        public CryptoTransform(string passPhrase) :
        this(passPhrase, null)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with 256-bit key, derived using 1 password iteration,
        /// hashing without salt, cipher block chaining (CBC) mode, SHA-1
        /// hashing algorithm, and 4-to-8 byte long salt.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector) :
        this(passPhrase, initVector, -1)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with 256-bit key, derived using 1 password iteration,
        /// hashing without salt, cipher block chaining (CBC) mode, SHA-1 
        /// hashing algorithm, and 0-to-8 byte long salt.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen) :
        this(passPhrase, initVector, minSaltLen, -1)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with 256-bit key, derived using 1 password iteration,
        /// hashing without salt, cipher block chaining (CBC) mode, SHA-1
        /// hashing algorithm. Use the minSaltLen and maxSaltLen parameters to
        /// specify the size of randomly generated salt.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen) :
        this(passPhrase, initVector, minSaltLen, maxSaltLen, -1)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption using the key derived from 1 password iteration,
        /// hashing without salt, cipher block chaining (CBC) mode, and
        /// SHA-1 hashing algorithm.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be 
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize) :
        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, null)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption using the key derived from 1 password iteration, hashing 
        /// without salt, and cipher block chaining (CBC) mode.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm) :
        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize,
             hashAlgorithm, null)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption using the key derived from 1 password iteration, and
        /// cipher block chaining (CBC) mode.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used for password hashing during key generation. This is
        /// not the same as the salt we will use during encryption. This parameter
        /// can be any string.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm,
                                string saltValue) :
        this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize,
             hashAlgorithm, saltValue, 1)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with the key derived from the explicitly specified
        /// parameters.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used for password hashing during key generation. This is
        /// not the same as the salt we will use during encryption. This parameter
        /// can be any string.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to hash password. More iterations are
        /// considered more secure but may take longer.
        /// </param>
        public CryptoTransform(string passPhrase,
                                string initVector,
                                int minSaltLen,
                                int maxSaltLen,
                                int keySize,
                                string hashAlgorithm,
                                string saltValue,
                                int passwordIterations)
        {
            // Save min salt length; set it to default if invalid value is passed.
            if (minSaltLen < MIN_ALLOWED_SALT_LEN)
                CryptoTransform.minSaltLen = DEFAULT_MIN_SALT_LEN;
            else
                CryptoTransform.minSaltLen = minSaltLen;

            // Save max salt length; set it to default if invalid value is passed.
            if (maxSaltLen < 0 || maxSaltLen > MAX_ALLOWED_SALT_LEN)
                CryptoTransform.maxSaltLen = DEFAULT_MAX_SALT_LEN;
            else
                CryptoTransform.maxSaltLen = maxSaltLen;

            // Set the size of cryptographic key.
            if (keySize <= 0)
                keySize = DEFAULT_KEY_SIZE;

            // Set the name of algorithm. Make sure it is in UPPER CASE and does
            // not use dashes, e.g. change "sha-1" to "SHA1".
            if (hashAlgorithm == null)
                hashAlgorithm = DEFAULT_HASH_ALGORITHM;
            else
                hashAlgorithm = hashAlgorithm.ToUpper().Replace("-", "");

            // Initialization vector converted to a byte array.
            byte[] initVectorBytes = null;

            // Salt used for password hashing (to generate the key, not during
            // encryption) converted to a byte array.
            byte[] saltValueBytes = null;

            // Get bytes of initialization vector.
            if (initVector == null)
                initVectorBytes = new byte[0];
            else
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            // Get bytes of salt (used in hashing).
            if (saltValue == null)
                saltValueBytes = new byte[0];
            else
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Generate password, which will be used to derive the key.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                       passPhrase,
                                                       saltValueBytes,
                                                       hashAlgorithm,
                                                       passwordIterations);

            // Convert key to a byte array adjusting the size from bits to bytes.
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Initialize Rijndael key object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // If we do not have initialization vector, we cannot use the CBC mode.
            // The only alternative is the ECB mode (which is not as good).
            if (initVectorBytes.Length == 0)
                symmetricKey.Mode = CipherMode.ECB;
            else
                symmetricKey.Mode = CipherMode.CBC;

            // Create encryptor and decryptor, which we will use for cryptographic
            // operations.
            CryptoTransform.encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            CryptoTransform.decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        }

        public string Encrypt(string textToEncrypt)
        {
            try
            {

                Encrypt e = new Encrypt();
                return e.EncryptString(textToEncrypt);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public string Decrypt(string textToDecrypt)
        {
            try
            {
                Decrypt d = new Decrypt();
                return d.DecryptString(textToDecrypt);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
