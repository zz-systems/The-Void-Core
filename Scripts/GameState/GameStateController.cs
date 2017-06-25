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

using Assets.Scripts.Spawners;
using ZzSystems.TheVoid.Visual.UI;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.TheVoid.Behavior.Player;
using ZzSystems.Unity.Shared.Gui;

namespace ZzSystems.TheVoid.GameState
{
    public class GameStateController : MonoBehaviour
    {
        public UIStateMachine   UiState;
        public ScoreController  Score;
        public MenuItem     IngameMenu;
        public MenuItem     GameOverMenu;
        public SpaceSpawner     SpawnSystem;
        public PlayerMobilityBehavior   PlayerMobilityBehavior;

        public Text             Version;
        public bool GodMode = false;

        public bool IsDead { get; private set; }

        void Awake()
        {
            if (IngameMenu != null)
            {
                IngameMenu.gameObject.SetActive(false);
            }

            Time.timeScale = 1;
            IsDead = false;

            if (Version != null)
                Version.text = "V" + Application.version;
            
            //Chartboost.didFailToLoadInterstitial    += (location, error)    => ChangeToGame();
            //Chartboost.didDismissInterstitial       += location             => ChangeToGame();
            //Chartboost.didCloseInterstitial         += location             => ChangeToGame();
        }
    
        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void StartGame()
        {
//#if UNITY_EDITOR
            ChangeToGame();
//#else
//            Chartboost.showInterstitial(CBLocation.Default);
//#endif
        }

        private void ChangeToGame()
        {
            GetComponent<ChangeScene>().ChangeToGame();
        }

        public void Resume()
        {
            PlayerMobilityBehavior.Calibrate();

            Time.timeScale = 1;

            IngameMenu.Invoke("Hide", 0.1f);

            Invoke("DisableIngameMenu", 1f);
        }

       

        private void DisableIngameMenu()
        {
            UiState.Close(IngameMenu);
        }

        public void Restart()
        {
            //Chartboost.showInterstitial(CBLocation.GameScreen);
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
            if (Score.GamesPlayed > 0 && Score.GamesPlayed % 3 == 0)
                AdBuddizBinding.ShowAd();
#endif
            ChangeToGame();
            //GetComponent<ChangeScene>().ChangeToGame();
        }

        public void Lose()
        {
            if (GodMode)
                return;

            IsDead = true;
            Score.Save();

            Invoke("GameOver", 0.1f);
        }


        public void GameOver()
        {
            if (GodMode)
                return;
            //yield return new WaitForSeconds(0.5f);
            IsDead = true;

            UiState.Open(GameOverMenu);

            Time.timeScale = 0;

            Score.ChangeSpree(true);
            Score.Save();


            //Score.Invoke("ResetScoreOnDeath", 1f);

            

            //yield return new WaitForSeconds(1.5f);

            //var endScoreText = GameObject.FindGameObjectWithTag("GameOverScore").GetComponent<Text>();
            //endScoreText.text = Score.Score.Value.ToString();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && !IsDead)
            {
                IngameMenu.Show();
                Time.timeScale = 0;
            }
        }

        public void Quit()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
