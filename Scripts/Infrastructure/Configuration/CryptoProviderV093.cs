#region copyright
/***************************************************************************
 * The Void
 * Copyright (C) 2015-2017  Sergej Zuyev
 * sergej.zuyev - at - zz-systems.net
 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **************************************************************************/
#endregion

using System;
using System.Security.Cryptography;
using System.Text;
using ZzSystems.Unity.Shared.Security;

namespace ZzSystems.TheVoid.Infrastructure.Configuration
{
    /// <summary>
    /// Provides a hashing, encryption and decryption abstraction layer for v0.9.3 settings
    /// </summary>
    public class CryptoProviderV093 : ICryptoProvider
    {
        private readonly ICryptoKeyProvider _keyProvider;

        /// <summary>
        /// Creates an instance of CryptoProvider with a ICryptoKeyProvider instance 
        /// </summary>
        /// <param name="keyProvider">ICryptoKeyProvider used for encryption settings</param>
        public CryptoProviderV093(ICryptoKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        /// <summary>
        /// Encrypt provided plain text using DES
        /// </summary>
        /// <param name="plainText">string to encrypt</param>
        /// <returns>encrypted string</returns>
        public string Encrypt(string plainText)
        {
            var iv = _keyProvider.InitialVector;
            var key = _keyProvider.GetKey(null);

            byte[] inputbuffer = Encoding.Unicode.GetBytes(plainText);
            byte[] outputBuffer = DES.Create().CreateEncryptor(key, iv).TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        /// <summary>
        /// Decrypt provided encrypted text using DES
        /// </summary>
        /// <param name="encryptedText">string to decrypt</param>
        /// <returns>decrypted string</returns>
        public string Decrypt(string encryptedText)
        {
            var iv      = _keyProvider.InitialVector;
            var key     = _keyProvider.GetKey(null);

            byte[] inputbuffer = Convert.FromBase64String(encryptedText);
            byte[] outputBuffer = DES.Create().CreateDecryptor(key, iv).TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        
        /// <summary>
        /// Generate fingerprint for provided string using MD5
        /// </summary>
        /// <param name="text">string to hash</param>
        /// <returns>fingerprint</returns>
        public string Hash(string text)
        {
            var secretKey = _keyProvider.Secret;

            byte[] hashBytes = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(text + secretKey));
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            return hashString.PadLeft(32, '0');
        }
    }
}
