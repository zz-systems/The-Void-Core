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

using System.Linq;
using UnityEngine;

namespace ZzSystems.TheVoid.Behavior.Particles
{
    [ExecuteInEditMode]
    public class PlayerParticlesAttractorBehavior : MonoBehaviour {
        public  ParticleSystem[]            ParticleSystems;
        public  Transform                   TargetSystem;

        private Vector3 _targetPos      = Vector3.zero;
        private Vector3 _dir            = Vector3.zero;
        private Vector3 _particlePos    = Vector3.zero;
        private Vector3 _particleVel    = Vector3.zero;
        private float   _dirInvMagnitude   = 0f;

        private ParticleSystem.Particle[][] _particles;
        private ParticleSystem[] _systems;

        void Awake()
        {
            TargetSystem = transform;

            var player = GameObject.FindGameObjectsWithTag("PlayerMateria");
            _systems = player.SelectMany(go => go.GetComponentsInChildren<ParticleSystem>()).ToArray();

            _particles = new ParticleSystem.Particle[_systems.Length][];
            for (int i = 0; i < _systems.Length; i++)
            {
                _particles[i] = new ParticleSystem.Particle[_systems[i].main.maxParticles];
            }
        }

        // Update is called once per frame
        void Update ()
        {
            for (int s = 0; s < _systems.Length; s++)
            {
                var ps = _systems[s];
                
                ps.GetParticles(_particles[s]);

                _targetPos = TargetSystem.position;

                for (int i = 0; i < _particles[s].Length; i++)
                {
                    _particlePos = _particles[s][i].position;
                    _particleVel = _particles[s][i].velocity;

                    _dir.x = _targetPos.x - _particlePos.x;
                    _dir.y = _targetPos.y - _particlePos.y;

                    _dirInvMagnitude = 1f / Mathf.Max(_dir.sqrMagnitude, 1f);

                    _particles[s][i].velocity = new Vector3(
                        _particleVel.x + _dir.x * _dirInvMagnitude,
                        _particleVel.y + _dir.y * _dirInvMagnitude,
                        _particleVel.z);
                }

                ps.SetParticles(_particles[s], _particles[s].Length);
            }
        }

        void OnEnable()
        {
            TargetSystem        = transform;
        }
    }
}
