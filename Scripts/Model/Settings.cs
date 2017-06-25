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

using UniRx;
using ZzSystems.TheVoid.Infrastructure.Configuration;
using ZzSystems.Unity.Shared.Settings;

namespace ZzSystems.TheVoid.Model
{
    public class Settings
    {
        private static readonly ISettingsProvider _settings = SettingsProviderFactory.Resolve();

        public FloatReactiveProperty MusicVolume        = new FloatReactiveProperty(_settings.Get<float>("MusicVolume"));
        public FloatReactiveProperty SfxVolume          = new FloatReactiveProperty(_settings.Get<float>("SfxVolume"));

        public BoolReactiveProperty MusicEnabled        = new BoolReactiveProperty(_settings.Get<bool>("MusicEnabled"));
        public BoolReactiveProperty SfxEnabled          = new BoolReactiveProperty(_settings.Get<bool>("SfxEnabled"));

        public BoolReactiveProperty VibrationEnabled    = new BoolReactiveProperty(_settings.Get<bool>("VibrationEnabled"));

        public Settings()
        {
            MusicVolume.Subscribe       (val => { _settings.Set("MusicVolume", val).Save(); });
            SfxVolume.Subscribe         (val => { _settings.Set("SfxVolume",   val).Save(); });

            MusicEnabled.Subscribe      (val => { _settings.Set("MusicEnabled", val).Save(); });
            SfxEnabled.Subscribe        (val => { _settings.Set("SfxEnabled", val).Save(); });

            VibrationEnabled.Subscribe  (val => { _settings.Set("VibrationEnabled", val).Save(); });
        }
    }
}