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
using ZzSystems.TheVoid.Infrastructure.Configuration;
using ZzSystems.TheVoid.Visual.Views;
using ZzSystems.Unity.Shared.Gui;
using ZzSystems.Unity.Shared.Settings;

namespace ZzSystems.TheVoid.GameState
{
    public class RatingController : MonoBehaviour
    {
        private ISettingsProvider _settings;

        public bool Rated
        {
            get { return _settings.Get("rating_has_rated", false); }
            set { _settings.Set("rating_has_rated", value); }
        }

        public bool NeverAskAgain
        {
            get { return _settings.Get("rating_never_ask_again", false); }
            set { _settings.Set("rating_never_ask_again", value); }
        }

        public int AskAfterNGames      = 5;

        private ScoreController _scoreController;
        public UIStateMachine Fsm;

        public MenuItem RatingPopup;

        // Use this for initialization
        void Start()
        {
            _settings           = SettingsProviderFactory.Resolve();
            _scoreController    = GetComponent<ScoreController>();

            if (Fsm != null && !_scoreController.InGame && !Rated && !NeverAskAgain && _scoreController.GamesPlayed > 0 && _scoreController.GamesPlayed % AskAfterNGames == 0)
            {
                Fsm.GoTo(RatingPopup);
            }
        }

        public void RedirectToRatingPage()
        {
            Fsm.Previous();

            Rated = true;
            PlayerPrefs.Save();
            Application.OpenURL("market://details?id=com.sphericalbrain.thevoid");
        }

        public void Cancel()
        {
            Fsm.Previous();
        }

        public void DoNotAskagain()
        {
            Fsm.Previous();

            Rated           = false;
            NeverAskAgain   = true;

            PlayerPrefs.Save();
        }
    }
}
