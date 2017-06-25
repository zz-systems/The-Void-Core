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
using ZzSystems.TheVoid.Visual.Views;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.Unity.Shared.Gui;

namespace ZzSystems.TheVoid.Behavior.ShapeContainer
{
    [ExecuteInEditMode]
    public class MenuShapeContainerBehavior : MenuItem
    {
        public List<GameObject> Components = new List<GameObject>();
        public float Radius;
        public float AnimationDuration = 0.3f;

        public Image Background;
        public bool ShowOnAwake = false;

        public Vector3 Offset   = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;

        void Awake()
        {
            if (ShowOnAwake)
                Show();
        }

        public override void Show()
        {
            base.Show();

            if (Components == null || Components.Count == 0)
                return;

            var deviation       = Vector3.up * Radius;
            float angleDelta    = 360f / Components.Count;

            //transform.localRotation = Quaternion.Euler(0, 0, -90f);
            //transform.DORotate(Vector3.zero, AnimationDuration).SetUpdate(UpdateType.Late, true); 

            transform.localEulerAngles = Rotation;

            if(Background != null)
                Background.CrossFadeAlpha(1f, AnimationDuration, true);

            for (int i = 0; i < Components.Count; i++)
            {
                var target = Offset + Quaternion.Euler(0, 0, i * angleDelta) * deviation;

                Components[i].transform.localPosition           = Vector3.zero;
                Components[i].transform.localScale              = Vector3.zero;
                Components[i].transform.localEulerAngles        = -Rotation;

                Components[i].transform.DOScale(Vector3.one, AnimationDuration).SetUpdate(UpdateType.Late, true);
                Components[i].transform.DOLocalMove(target, AnimationDuration).SetUpdate(UpdateType.Late, true);
                
            }
        }

        public override void Hide()
        {
            base.Hide();

            if (Components == null || Components.Count == 0)
                return;

            if(Background != null)
                Background.CrossFadeAlpha(0f, AnimationDuration, true);

            //transform.DORotate(new Vector3(0, 0, -90f), AnimationDuration).SetUpdate(UpdateType.Late, true);


            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].transform.DOScale(Vector3.zero, AnimationDuration).SetUpdate(UpdateType.Late, true);
                Components[i].transform.DOLocalMove(Vector3.zero, AnimationDuration).SetUpdate(UpdateType.Late, true);
            }
        }
    }
}
