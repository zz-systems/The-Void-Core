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

using Assets.Scripts;
using System;
using UnityEngine;
using ZzSystems.Unity.Shared.GameServices;
using ZzSystems.Unity.Shared.Settings;

//http://wiki.unity3d.com/index.php/ArrayPrefs2

namespace ZzSystems.TheVoid.Infrastructure.Configuration
{
    /// <summary>
    /// Provides an unsecured high-level access to Unity.PlayerPrefs.
    /// Accepts limited types
    /// </summary>
    public class SettingsProviderV093 : ISettingsProvider
    {
        /// <summary>
        /// Gets a value from PlayerPrefs
        /// </summary>
        /// <typeparam name="T">Desired type</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value in case of failed lookup / conversion</param>
        /// <returns>Found value or default value</returns>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)PlayerPrefsX.GetBool(key, Convert.ToBoolean(defaultValue));
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(object)PlayerPrefsX.GetFloat(key);
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(object)PlayerPrefsX.GetLong(key);
            }

            if (typeof(T) == typeof(int[]))
            {
                return (T)(object)PlayerPrefsX.GetIntArray(key);
            }

            if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)DateTime.Parse(PlayerPrefs.GetString(key, Convert.ToDateTime(defaultValue).ToString()));
            }

            if (typeof(T).IsAssignableFrom(typeof(Achievement)))
            {
                return (T)(object)new Achievement
                {
                    IsUnlocked = Get<bool>(key + "_unlocked"),
                    Value = Get<long>(key),
                    RawValue = Get<long>(key + "_raw")
                };
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
            if (typeof(T) == typeof(bool))
            {
                PlayerPrefsX.SetBool(key, Convert.ToBoolean(value));
            }

            if (typeof(T) == typeof(float))
            {
                PlayerPrefsX.SetFloat(key, Convert.ToSingle(value));
            }

            if (typeof(T) == typeof(int[]))
            {
                PlayerPrefsX.SetIntArray(key, (int[])(object)value);
            }

            if (typeof(T).IsAssignableFrom(typeof(Achievement)))
            {
                var model = (Achievement) (object) value;

                Set(key, model.Value);
                Set(key + "_unlocked", model.IsUnlocked);
                Set(key + "_raw", model.RawValue);
            }

            return this;
        }

        /// <summary>
        /// Checks if a particular key exists
        /// </summary>
        /// <param name="key">Key to lookup</param>
        /// <returns>true if key exists</returns>
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        /// <summary>
        /// Deletes a particular key
        /// </summary>
        /// <param name="key">Key to delete</param>
        /// <returns>self, fluent interface</returns>

        public ISettingsProvider DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);

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
    }
}