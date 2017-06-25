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

namespace ZzSystems.TheVoid.Behavior.Particles
{
    [ExecuteInEditMode]
    public class ParticleAttractorBehavior : MonoBehaviour {
        public  ParticleSystem              ParticleSystem;
        public  Transform                   TargetSystem;
        private ParticleSystem.Particle[]   _particles;

        private Vector3 _targetPos      = Vector3.zero;
        private Vector3 _dir            = Vector3.zero;
        private Vector3 _particlePos    = Vector3.zero;
        private Vector3 _particleVel    = Vector3.zero;
        private float   _dirInvMagnitude   = 0f;

        // Update is called once per frame
        void Update ()
        {
            //return;
            if (ParticleSystem == null)
                return;

            if(_particles == null)
                _particles = new ParticleSystem.Particle[ParticleSystem.main.maxParticles];

            ParticleSystem.GetParticles(_particles);

            _targetPos = TargetSystem.position;

            for (int i = 0; i < _particles.Length; i++)
            {
                _particlePos = _particles[i].position;
                _particleVel = _particles[i].velocity;

                _dir.x = _targetPos.x - _particlePos.x;
                _dir.y = _targetPos.y - _particlePos.y;

                _dirInvMagnitude = 1f / Mathf.Max(_dir.sqrMagnitude, 1f);

                _particles[i].velocity = new Vector3(
                    _particleVel.x + _dir.x * _dirInvMagnitude,
                    _particleVel.y + _dir.y * _dirInvMagnitude,
                    _particleVel.z);
            }

             ParticleSystem.SetParticles(_particles, _particles.Length);
        }

        void OnEnable()
        {
            TargetSystem        = transform;
        }
    }
}
