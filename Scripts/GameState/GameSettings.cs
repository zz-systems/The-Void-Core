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
using UnityEngine;
using UnityEngine.Audio;
using ZzSystems.TheVoid.Behavior.Gui;
using ZzSystems.TheVoid.Model;
using ZzSystems.Unity.Shared.Util;

namespace ZzSystems.TheVoid.GameState
{
    public class GameSettings : StandardSingleton<GameSettings>
    {
        public AudioMixer MasterMixer;

        // FOr settings toggles
        public Settings CurrentSettings { get; private set; }

        public GameObject   Player;
        public GameObject[] Entities;
        public long         Score;

        public UIToggle[] VolumeToggles;
        public UIToggle[] VibrationToggles;
        public UIToggle[] TouchToggles;

        void Awake()
        {
            CurrentSettings = new Settings();

            CurrentSettings.MusicVolume .SubscribeOnMainThread().Subscribe(val => MasterMixer.SetFloat("MusicVolume",   val));
            CurrentSettings.SfxVolume   .SubscribeOnMainThread().Subscribe(val => MasterMixer.SetFloat("SfxVolume",     val));

            CurrentSettings.MusicEnabled.SubscribeOnMainThread().Subscribe(val => CurrentSettings.MusicVolume.Value = val ? 0f : -80f);
            CurrentSettings.SfxEnabled  .SubscribeOnMainThread().Subscribe(val => CurrentSettings.SfxVolume.Value   = val ? 0f : -80f);

            foreach (var vol in VolumeToggles)
            {
                CurrentSettings.MusicEnabled.Subscribe(vol.ToggleReceiver);
                CurrentSettings.SfxEnabled.Subscribe(vol.ToggleReceiver);

                vol.Toggle.OnValueChangedAsObservable().Subscribe(val => CurrentSettings.MusicEnabled.Value = CurrentSettings.SfxEnabled.Value = val);
            }

            foreach (var vib in VibrationToggles)
            {
                CurrentSettings.VibrationEnabled.Subscribe(vib.ToggleReceiver);
                vib.Toggle.OnValueChangedAsObservable().Subscribe(val => CurrentSettings.VibrationEnabled.Value = val);
            }
        }
    }
}
