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

using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.Unity.Shared.Resources;

namespace ZzSystems.TheVoid.Behavior.Gui
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class UIButton : MonoBehaviour
    {
        public Sprite Image;
        public string Key;

        public Button.ButtonClickedEvent Click;

        private Text _ownText;

        void Start ()
        {
            Click.AddListener(() => GetComponent<RandomClipBehavior>().PlayRandom());

            var image = GetComponentInChildren<Button>().image;
            if(image != null)
                image.sprite   = Image;

            GetComponentInChildren<Button>().onClick        = Click;
            _ownText = GetComponentInChildren<Text>();
            if(_ownText != null)
                _ownText.text             = FindObjectOfType<Localization>().Get<string>(Key);
        }
    }
}
