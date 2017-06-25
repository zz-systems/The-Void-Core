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

namespace Assets.Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomClipBehavior : MonoBehaviour {

        public AudioClip[] Sounds = new AudioClip[0];

        private AudioSource _source;
        public void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void PlayRandom()
        {
            if (_source.isPlaying || Sounds.Length == 0)
                return;
            
            _source.clip = Sounds[Random.Range(0, Sounds.Length)];            
            _source.Play();
        }
    }
}
