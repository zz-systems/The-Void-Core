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
using ZzSystems.TheVoid.Behavior.Star;
using Assets.Scripts.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.TheVoid.GameState;

namespace ZzSystems.TheVoid.Visual.UI
{
    [RequireComponent(typeof(Text))]
    public class ScoreText : MonoBehaviour
    {
        public GameStateController GameState;
        public ScoreController Score;
        private Text _ownText;
        private readonly IntReactiveProperty _currentScore = new IntReactiveProperty(0);

        void Start()
        {
            _ownText    = GetComponent<Text>();
            GameState = FindObjectOfType<GameStateController>();

            //Score.CurrentScore.CurrentScore.SubscribeToText(_ownText);
        }

        void Update()
        {
            if (Score.CurrentScore.CurrentScore.Value != 0)
                _currentScore.Value = Mathf.FloorToInt(Mathf.Lerp(_currentScore.Value, Score.CurrentScore.CurrentScore.Value, 10 * Time.deltaTime));

            if (GameState.IsDead)
                _currentScore.Value = Score.CurrentScore.CurrentScore.Value;

            _ownText.text = _currentScore.ToString();
        }
    }
}