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
    public class OffScreenIndicator : MonoBehaviour {
        public Transform 	Target;

        private Camera 		_mainCamera;
        private Transform 	_ownTransform;
        private Renderer 	_ownRenderer;

        void Awake()
        {
            _mainCamera 		= Camera.main;
            _ownRenderer 	= GetComponent<Renderer> ();
            _ownTransform 	= GetComponent<Transform> ();
        }

        void Update () {
            Vector3 v3Screen = _mainCamera.WorldToViewportPoint(Target.position);

            if(v3Screen.y >= 1.01f)
            {
                _ownRenderer.enabled = true;

                v3Screen.x = Mathf.Clamp (v3Screen.x, 0.01f, 0.99f);
                v3Screen.y = Mathf.Clamp (v3Screen.y, 0.01f, 0.99f);

                _ownTransform.position = _mainCamera.ViewportToWorldPoint (v3Screen);

                // TODO
                //transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            }		
            else
            {
                _ownRenderer.enabled = false;
            }
        }
    }
}
