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

namespace ZzSystems.TheVoid.Visual
{
    public class Parallax : MonoBehaviour
    {
        public Vector2 UVAnimRate;
        private Material _material;

        private float _offset;
        void Awake()
        {
            _material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        void Update()
        {
            _offset += UVAnimRate.y * Time.deltaTime;
            _offset %= 1;
            //if (_renderer.enabled)
            {
                //_material.mainTextureOffset += UVAnimRate * Time.deltaTime;


                _material.mainTextureOffset = new Vector2(0, _offset);

                //.SetTextureOffset("_MainTex", _uvOffset);
            }
        }
    }
}
