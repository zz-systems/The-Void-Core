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
using ZzSystems.Unity.Shared.Util;

namespace Assets.Scripts.Util
{
    using Spawners;

    /// <summary>
    /// 
    /// </summary>
    public class Recycler : MonoBehaviour
    {
        public bool DetachChildren = false;
        private bool _recycling;

        void Awake()
        {
            _recycling = false;

            StopCoroutine(CheckRecycleCriteria());
            StartCoroutine(CheckRecycleCriteria());
        }

        void OnEnable()
        {
            _recycling = false;

            StopCoroutine(CheckRecycleCriteria());
            StartCoroutine(CheckRecycleCriteria());
        }

        IEnumerator CheckRecycleCriteria()
        {
            while (true)
            {
                if (!_recycling && transform.position.y < -5)
                {
                    _recycling = true;
                    
                    break;
                }

                yield return CommonWaits.Wait0_5S;
            }

            yield return CommonWaits.Wait0_5S;

            if(DetachChildren)
                transform.DetachChildren();

            this.Recycle();
        }
    }
}
