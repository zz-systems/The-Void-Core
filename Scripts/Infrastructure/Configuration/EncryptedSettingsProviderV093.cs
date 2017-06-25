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

using UnityEngine;
using ZzSystems.Unity.Shared.Security;
using ZzSystems.Unity.Shared.Settings;

namespace ZzSystems.TheVoid.Infrastructure.Configuration
{
    /// <summary>
    /// Provides a secured high-level access to Unity.PlayerPrefs.
    /// Accepts limited types
    /// </summary>
    class EncryptedSettingsProviderV093 : ISettingsProvider
    {
        /// <summary>
        /// CryptoProvider instance
        /// </summary>
        private readonly ICryptoProvider _cryptoProvider;

        /// <summary>
        /// Creates an instance of SettingsProvider with a ICryptoProvider instance 
        /// </summary>
        /// <param name="cryptoProvider">CryptoProvider used for encryption and decryption</param>
        public EncryptedSettingsProviderV093(ICryptoProvider cryptoProvider)
        {
            _cryptoProvider = cryptoProvider;
        }

        /// <summary>
        /// Gets a value from PlayerPrefs
        /// </summary>
        /// <typeparam name="T">Desired type</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value in case of failed lookup / conversion</param>
        /// <returns>Found value or default value</returns>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (!HasKey(key))
                return defaultValue;

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)bool.Parse(DecryptString(key));
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(object)float.Parse(DecryptString(key));
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(DecryptString(key));
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(object)long.Parse(DecryptString(key));
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets a value in PlayerPrefs
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="value">Value to set</param>
        /// <returns>self, fluent interface</returns>
        public ISettingsProvider Set<T>(string key, T value)
        {
            PlayerPrefs.SetString(_cryptoProvider.Hash(key), _cryptoProvider.Encrypt(value.ToString()));

            return this;
        }

        /// <summary>
        /// Checks if a particular key exists
        /// </summary>
        /// <param name="key">Key to lookup</param>
        /// <returns>true if key exists</returns>
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(_cryptoProvider.Hash(key));
        }

        /// <summary>
        /// Deletes a particular key
        /// </summary>
        /// <param name="key">Key to delete</param>
        /// <returns>self, fluent interface</returns>
        public ISettingsProvider DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(_cryptoProvider.Hash(key));

            return this;
        }

        /// <summary>
        /// Deletes all keys
        /// </summary>
        /// <returns>self, fluent interface</returns>
        public ISettingsProvider DeleteAll()
        {
            PlayerPrefs.DeleteAll();

            return this;
        }

        /// <summary>
        /// Saves changes
        /// </summary>
        /// <returns>self, fluent interface</returns>
        public ISettingsProvider Save()
        {
            PlayerPrefs.Save();

            return this;
        }

        /// <summary>
        /// Decrypt a setting
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <returns>Decrypted value from PlayerPrefs</returns>
        private string DecryptString(string key)
        {
            var hash = _cryptoProvider.Hash(key);            
            var raw = PlayerPrefs.GetString(hash);

            return _cryptoProvider.Decrypt(raw);
        }
    }
}
