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

using UnityEngine;
using ZzSystems.TheVoid.Security;
using ZzSystems.Unity.Shared.Security;
using ZzSystems.Unity.Shared.Settings;

namespace ZzSystems.TheVoid.Infrastructure.Configuration
{
    /// <summary>
    /// Resolves a fitting ISettingsProvider instance
    /// </summary>
    public static class SettingsProviderFactory
    {

        private static readonly ISettingsProvider Current          = new SettingsProvider(new CryptoProvider(new CryptoKeyProvider()));
        private static readonly ISettingsProvider EncryptedV093    = new EncryptedSettingsProviderV093(new CryptoProviderV093(new CryptoKeyProviderV093()));
        private static readonly ISettingsProvider PlainV093        = new SettingsProviderV093();

        /// <summary>
        /// Returns a fitting instance of ISettingsProvider
        /// </summary>
        /// <param name="useEncryption">Important for legacy settings providers with partial encryption</param>
        /// <returns>Resolved ISettingsProvider instance</returns>
        public static ISettingsProvider Resolve(bool useEncryption = false)
        {
            var version = new Version(PlayerPrefs.GetString("Version", "0.9.3"));

            if (version <= new Version(0, 9, 3))
            {
                if (useEncryption)
                    return new MigratingSettingsProvider(EncryptedV093, Current);

                return new MigratingSettingsProvider(PlainV093, Current);
            }

            return Current;
        }
    }
}
