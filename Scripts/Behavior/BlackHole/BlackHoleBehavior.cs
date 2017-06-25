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

namespace ZzSystems.TheVoid.Behavior.BlackHole
{
    public class BlackHoleBehavior : MonoBehaviour {

        SpriteRenderer      _renderer;
        private Light       _ownLight;
        private LensFlare   _ownFlare;

        public Color CurrentColor { get; private set; }

        // Use this for initialization
        void Awake () {
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _ownLight = GetComponentInChildren<Light>();
            _ownFlare = GetComponentInChildren<LensFlare>();

            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().AddTorque(Random.Range(-15f, -5f), ForceMode2D.Impulse);
        }

        //// Update is called once per frame
        //void Update () {
        //    transform.Rotate(0, 0, -500 * Time.deltaTime);
        //}    

        void OnEnable()
        {
            if (_ownLight != null)
                CurrentColor = _renderer.color = _ownLight.color =
                    _ownFlare.color = Random.ColorHSV(0, 1, 0.7f, 07f, 0.7f, 0.7f);

            
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            GetComponent<Rigidbody2D>().AddTorque(Random.Range(-15f, -5f), ForceMode2D.Impulse);
        }
    }
}
