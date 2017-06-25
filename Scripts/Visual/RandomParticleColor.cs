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

using System.Collections;
using UnityEngine;

namespace ZzSystems.TheVoid.Visual
{
    [RequireComponent(typeof(ParticleSystem))]
    public class RandomParticleColor : MonoBehaviour {

        private ParticleSystem _ownParticles;
        private Color _newColor;
        // Use this for initialization
        void Start () {
            _ownParticles = GetComponent<ParticleSystem>();
            StartCoroutine(ChangeColor());
        }

        IEnumerator ChangeColor()
        {
            while(true)
            {
                _newColor = Random.ColorHSV();

                yield return new WaitForSeconds(Random.Range(1, 3));
            }
        }


        void Update()
        {
            var main = _ownParticles.main;

            main.startColor = Color.Lerp(main.startColor.color, _newColor, 0.2f * Time.deltaTime);	
        }
    }
}
