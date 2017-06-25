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
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.Unity.Shared.Resources;

namespace ZzSystems.TheVoid.Behavior.Gui
{
    public class UIToggle : MonoBehaviour
    {
        public Sprite NormalState;
        public Sprite ToggledState;

        public string Key;

        public Toggle Toggle;

        private Text _ownText;
        private Image _ownImage;
        //private Toggle _ownToggle;

        private bool? _toggled; 
      
        // Use this for initialization
        void Awake()
        {
            Toggle = GetComponent<Toggle>() ?? GetComponentInChildren<Toggle>();
            _ownImage = GetComponent<Image>() ?? GetComponentInChildren<Image>();

#if UNITY_EDITOR
            Toggle.OnValueChangedAsObservable().Subscribe(val => Debug.Log("toggle " + val));
#endif
            Toggle.OnValueChangedAsObservable().Subscribe(SwapSprites, GetComponent<RandomClipBehavior>().PlayRandom);

            //Toggle.AddListener(_ => GetComponent<RandomClipBehavior>().PlayRandom());
            //Toggle.AddListener(SwapSprites);

            //_ownToggle.onValueChanged = Toggle;
            _ownText = GetComponentInChildren<Text>();
            
            if(_ownText != null)
                _ownText.text = FindObjectOfType<Localization>().Get<string>(Key);

            if(_toggled != null)
                ToggleReceiver(_toggled.Value);
        }

        private void SwapSprites(bool isToggled)
        {
            if(_ownImage != null)
                _ownImage.sprite = isToggled ? ToggledState : NormalState;            
        }

        public void ToggleReceiver(bool value)
        {
            if (Toggle != null)
            {
                Toggle.isOn = value;
                SwapSprites(value);
            }
            else
            {
                _toggled = value;
            }
        }
    }
}
