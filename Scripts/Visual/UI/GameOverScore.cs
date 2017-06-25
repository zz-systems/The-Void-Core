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
using UnityEngine.UI;
using ZzSystems.TheVoid.GameState;
using ZzSystems.Unity.Shared.Gui;

namespace ZzSystems.TheVoid.Visual.UI
{
    public class GameOverScore : MonoBehaviour
    {
        public ScoreController ScoreController;

        public Text Score;
        public Text Highscore;
        public LocalizedText HighscoreTitle;

        void OnEnable()
        {
            var newScore = ScoreController.CurrentScore.CurrentScore.Value;
            var oldScore = ScoreController.LocalHighscores[0];

            HighscoreTitle.Key = (newScore >= oldScore 
                ? "GameOver.BestNew"
                : "GameOver.Best");

            Score.text      = newScore.ToString();
            Highscore.text = newScore >= oldScore 
                ? newScore.ToString() 
                : oldScore.ToString();
        }
    }
}
