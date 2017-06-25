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
using ZzSystems.TheVoid.Behavior.Shared;

namespace ZzSystems.TheVoid.Behavior.Star
{
    [RequireComponent(typeof(RandomColorOnActivationBehavior))]
    public class StarBehavior : MonoBehaviour {
        public  Rigidbody2D         Player;
	
        private Light 				_ownLight;
        private LensFlare 			_ownFlare;
        private SpriteRenderer      _ownRenderer;
        private ParticleSystem      _materiaParticles;

        private RandomColorOnActivationBehavior _randomColor;

        public bool                 WasHit          { get; set; }

        public bool                 IsKinematic = false;
        void Awake ()
        {
            _materiaParticles   = GetComponentInChildren<ParticleSystem>();

            _ownLight 		    = GetComponentInChildren<Light> ();
            _ownFlare 		    = GetComponentInChildren<LensFlare> ();
            _ownRenderer        = GetComponentInChildren<SpriteRenderer>();
            _randomColor = GetComponentInChildren<RandomColorOnActivationBehavior>();

            GetComponentInChildren<Rigidbody2D>().isKinematic = IsKinematic;
        }
        void OnEnable()
        {
            if (_ownLight != null)
                //_materiaParticles.startColor = 
                    _ownRenderer.color = _ownLight.color =
                    _ownFlare.color = _randomColor.CurrentColor;
        }
    }
}
