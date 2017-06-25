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

namespace ZzSystems.TheVoid.Effects
{
    public class RandomColorOverLifetimeBehavior : MonoBehaviour
    {
        private ParticleSystem      _particleSystem;
        private Renderer            _renderer;

        private ParticleSystem.Particle[] _particles;
        private Gradient _newGradient;
        private Color _newColor;
        private Color _currentColor;

        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _renderer       = _particleSystem.GetComponent<Renderer>();

            StartCoroutine(ChangeColor());
        }

        // Update is called once per frame
        void LateUpdate()
        {
            //if (_particleSystem == null)
            //    return;

            //if (_particles == null)
            //    _particles = new ParticleSystem.Particle[_particleSystem.maxParticles];

            //_particleSystem.GetParticles(_particles);


            //for (int i = 0; i < _particles.Length; i++)
            //{
            //    _particles[i].color = _newGradient.Evaluate(_particles[i].lifetime / _particles[i].startLifetime);
            //    _particleSystem.GetComponent<Renderer>().sharedMaterial.SetColor("_EmisColor", _particles[i].color);
            //}


            //_particleSystem.SetParticles(_particles, _particles.Length);

            _currentColor = Color.Lerp(_currentColor, _newColor, Time.deltaTime);

            _renderer.material.SetColor("_EmisColor", _currentColor);
        }
        IEnumerator ChangeColor()
        {
            while (true)
            {
                //_newGradient = GradientGenerator.RandomGradient(5, 70, 70);
                _newColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);// GradientGenerator.RandomColor(90, 90);
                yield return new WaitForSeconds(Random.Range(1, 5));
            }
        }
    }
}
