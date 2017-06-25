﻿#region copyright
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZzSystems.TheVoid.Resources;

namespace ZzSystems.TheVoid.Util
{
    class MainMenuStar : MonoBehaviour
    {
        void Awake()
        {
            var star = (GameObject)Instantiate(new DefaultResourceProvider().StarPrefab, transform);

            star.GetComponent<Rigidbody2D>().isKinematic = true;
            star.GetComponent<Light>().enabled = false;
        }
    }
}
