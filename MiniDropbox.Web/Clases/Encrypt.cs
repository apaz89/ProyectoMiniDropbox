using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace EncryptString
{
    class Encrypt
    {
        #region variables

        private string passPhrase = "Pas5prse";        // puede ser cualquier string
        private string saltValue = "s1tValue";        // puede ser cualquier string
        private string hashAlgorithm = "SHA1";             // puede ser tambien "MD5"
        private int passwordIterations = 2;                  // puede ser cualquier numero
        private string initVector = "01B2c3D4e5F6g7H8"; // debe ser de 16 bytes
        private int keySize = 256;                // puede ser tambien 192 or 128

        #endregion

        #region functions

        public void restoreSettings()
        {
            passPhrase = "Pas5prse";        // can be any string
            saltValue = "s1tValue";        // can be any string
            hashAlgorithm = "SHA1";             // can be "MD5"
            passwordIterations = 2;                  // can be any number
            initVector = "01B2c3D4e5F6g7H8"; // must be 16 bytes
            keySize = 256;                // can be 192 or 128

        }

        /// <summary>
        /// Asigna el valor a la variable "passPhrase" que se usa para generar la llave de encriptación.
        /// </summary>
        /// <param name="pPassPhrase">Puede ser cualquier cadena de caracteres.</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setPassPhrase(string pPassPhrase)
        {
            int intResult = -1;
            if (pPassPhrase.Length > 0)
            {
                this.passPhrase = pPassPhrase;
                intResult = 0;
            }
            return intResult;
        }


        /// <summary>
        /// Asigna el valor a la variable "saltValue" que se usa para generar la llave de encriptación
        /// </summary>
        /// <param name="pSaltValue">Puede ser cualquier cadena de caracteres</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setSaltValue(string pSaltValue)
        {
            int intResult = -1;
            if (pSaltValue.Length > 0)
            {
                this.saltValue = pSaltValue;
                intResult = 0;
            }
            return intResult;
        }


        /// <summary>
        /// Establece el tipo de algoritmo hash que se usará
        /// </summary>
        /// <param name="pHashAlgorithm">1: SHA1; 2:MD5</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setHashAlgorithm(int pHashAlgorithm)
        {
            int intResult = -1;

            switch (pHashAlgorithm)
            {
                case 1:
                    this.hashAlgorithm = "SHA1";
                    intResult = 0;
                    break;

                case 2:
                    this.hashAlgorithm = "MD5";
                    intResult = 0;
                    break;
            }

            return intResult;
        }


        /// <summary>
        /// Establece la cantidad de iteraciones que se realizarán para generar la clave
        /// </summary>
        /// <param name="intIterations">Número de iteraciones mayor a 0(cero)</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setPasswordIterations(int intIterations)
        {
            int intResult = 0;

            try
            {
                if (intIterations > 0)
                {
                    this.passwordIterations = intIterations;
                }
                else
                {
                    intResult = -1;
                }
            }
            catch
            {
                intResult = -1;
            }

            return intResult;
        }

        /// <summary>
        /// Establece el valor del IV (Initial Vector)
        /// </summary>
        /// <param name="pInitVector">Initial Vector, debe ser de 16 caracteres</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setInitVector(string pInitVector)
        {
            int intResult = -1;

            try
            {
                if (pInitVector.Length == 16)
                {
                    this.initVector = pInitVector;
                    intResult = 0;
                }

            }
            catch
            {
                intResult = -1;
            }

            return intResult;
        }


        /// <summary>
        /// Establece el tamaño de la llave de encriptación en bits 
        /// </summary>
        /// <param name="pKeySize">1:128; 2:192; 3:256</param>
        /// <returns>0:Exito, -1:No se completó</returns>
        public int setKeySize(int pKeySize)
        {
            int intResult = -1;

            switch (pKeySize)
            {
                case 1:
                    this.keySize = 128;
                    intResult = 0;
                    break;
                case 2:
                    this.keySize = 192;
                    intResult = 0;
                    break;
                case 3:
                    this.keySize = 256;
                    intResult = 0;
                    break;

            }


            return intResult;
        }

        /// <summary>
        /// Encripta una palabra o frase.
        /// </summary>
        /// <param name="strInput">Palabra o frase a encriptar.</param>
        /// <returns>Palabra o frase encriptada.</returns>
        public string encryptMe(string strInput)
        {
            string strOutput = "";
            try
            {
                strOutput = fEncrypt(strInput, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
            }
            catch
            {
                strOutput = "";
            }

            return strOutput;
        }

        /// <summary>
        /// Decripta una palabra o frase encriptada.
        /// </summary>
        /// <param name="strInput">Palabra o frase a decriptar.</param>
        /// <returns>Palabra o frase original(decriptada)</returns>
        public string decryptMe(string strInput)
        {
            string strOutput = "";
            try
            {
                strOutput = fDecrypt(strInput, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
            }
            catch
            {
                strOutput = "";
            }

            return strOutput;
        }



        /// <summary>
        /// Encrypts specified plaintext using Rijndael symmetric key algorithm
        /// and returns a base64-encoded result.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be encrypted.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be 
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Encrypted value formatted as a base64-encoded string.
        /// </returns>
        private static string fEncrypt(string plainText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }



        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>
        private static string fDecrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        #endregion

    }
}

