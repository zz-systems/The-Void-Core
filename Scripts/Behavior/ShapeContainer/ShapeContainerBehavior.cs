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

using System.Collections.Generic;
using UnityEngine;

namespace ZzSystems.TheVoid.Behavior.ShapeContainer
{
    public class ShapeContainerBehavior : MonoBehaviour {

        public List<GameObject> Components = new List<GameObject>();
        public float Radius = 1f;
       
        public void InitComponents()
        {
            if (Components == null || Components.Count == 0)
                return;

            var deviation       = Vector3.up * Radius;
            float angleDelta    = 360f / Components.Count;

            var body = GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.isKinematic = false;
            }

            for (int i = 0; i < Components.Count; i++)
            {
                var target = Quaternion.Euler(0, 0, i*angleDelta)*deviation;

                //body = Components[i].GetComponent<Rigidbody2D>();

                //if (body != null)
                //    body.isKinematic = false;

                Components[i].transform.localPosition = target;
            }
        }
    }
}
