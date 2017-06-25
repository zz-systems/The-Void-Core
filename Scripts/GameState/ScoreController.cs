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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Assets.Scripts.Spawners;
using ZzSystems.TheVoid.Visual.UI;
using UnityEngine;
using UnityEngine.Events;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using ZzSystems.TheVoid.GameServices;
using ZzSystems.TheVoid.Infrastructure.Configuration;
using ZzSystems.Unity.Shared.GameServices;
using ZzSystems.Unity.Shared.Settings;
using ZzSystems.Unity.Shared.Util;
using Random = UnityEngine.Random;

namespace ZzSystems.TheVoid.GameState
{
    public class ScoreController : MonoBehaviour
    {
        private GameServicesProvider<Keys> _gameServicesProvider;

        public Time LongestSurvivalTime;
        
        public Model.Score CurrentScore { get; set; } 
        
        //public int Score = 0;
        

        public List<IScore> Scores = new List<IScore>();

        public UnityEvent OnSpreeReached;

        public int      MaxMultiplier  = 10;
        public float    SpreeFactor    = 15f;
        public float    SpreeCooldown  = 1f;

        public bool InGame = false;

        public GameOverScore GameOverScore;
        private readonly List<int>                  _possibleScores             = new List<int>();

        private ISettingsProvider _settings;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Keys
        {
            ACH_SHORTLIVED,
            ACH_SURVIVOR_I,
            ACH_SURVIVOR_II,
            ACH_SURVIVOR_III,
            ACH_SURVIVOR_IV,

            ACH_HARVESTER_I,
            ACH_HARVESTER_II,
            ACH_HARVESTER_III,
            ACH_HARVESTER_IV,
            ACH_HARVESTER_V,

            ACH_NOOB_I,
            ACH_NOOB_II,
            ACH_NOOB_III,
            ACH_NOOB_IV,
            ACH_NOOBUS_MAXIMUS,
            LEAD_HIGHSCORE,

            LOCAL_HIGHSCORE,
            LOCAL_DAILY_HIGHSCORE,
            LOCAL_WEEKLY_HIGHSCORE,
            LOCAL_DEATHS,
            LOCAL_HARVESTED,
            LOCAL_SHORTLIVED
        }

        private List<int> _localHighscoresCache;
        public List<int> LocalHighscores
        {
            get
            {
                if(_localHighscoresCache == null)
                {
                    var highscores = _settings.Get<int[]>("local_highscores");
                    if (highscores == null || highscores.Length == 0)
                        highscores = Enumerable.Repeat(0, 10).ToArray();

                    _localHighscoresCache = new List<int>(highscores.OrderByDescending(v => v));
                }

                return _localHighscoresCache;
            }
            set
            {
                _localHighscoresCache = value.OrderByDescending(v => v).Take(10).ToList();
                _settings.Set("local_highscores", _localHighscoresCache.ToArray());
            }
        }

        private float _spreePercentage  = 100;
        private float _lastHitTime;
        private float _survivalTime;

        public string GameOverText;

        public int              RandomScore                  { get { return _possibleScores.RandomElement(); } }

        private readonly Dictionary<int, string> _scoreMultiplierStringCache = new Dictionary<int, string>();
        

        public bool UseSocialPlatform
        {
            get { return _settings.Get("use_social_platform", true); }
            set { _settings.Set("use_social_platform", value); }
        }

        public DateTime LastHighscoreDate
        {
            get { return _settings.Get("highscore_date", DateTime.Today); }
            set { _settings.Set("highscore_date", value); }
        }

        public int GamesPlayed
        {
            get { return _settings.Get("games_played", 0); }
            set { _settings.Set("games_played", value); }
        }

        private float _maxSpreePercentage;


        public ScoreController()
        {
            
        }

    
        public long LocalHighscore
        {
            get { return _gameServicesProvider[Keys.LOCAL_HIGHSCORE].Value; }
        }

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            _settings = SettingsProviderFactory.Resolve();
            _gameServicesProvider = new GameServicesProvider<Keys>(new GameServicesMetadataProvider(), SettingsProviderFactory.Resolve(true));
           

            CurrentScore = new Model.Score();

            ResetLocalScores();
            
            if (UseSocialPlatform)
            {
                #if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
                PlayGamesPlatform.Activate();
                #endif

                LogIn();
                StartCoroutine(_gameServicesProvider.SubmitOfflineData());
            }

            CurrentScore.CurrentScore.Value = 0;

            _maxSpreePercentage = MaxMultiplier * SpreeFactor * 10;

            InitRandomScores();

            //StartCoroutine(InitScorePopupCaches());

            if (InGame)
            {
                GamesPlayed++;

                StartCoroutine(HandleSurvivalTimeAchievements());

                StartCoroutine(PropagateSurvivalTime());
            }
        }

        private static GameObject SpawnPopup(GameObject factory)
        {
            var sourceTransform = factory.GetComponent<RectTransform>();
            var failure = factory.Spawn(sourceTransform.parent, sourceTransform.localPosition);

            var rectTransform = failure.GetComponent<RectTransform>();

            rectTransform.anchorMin     = sourceTransform.anchorMin;
            rectTransform.anchorMax     = sourceTransform.anchorMax;

            rectTransform.offsetMin     = sourceTransform.offsetMin;
            rectTransform.offsetMax     = sourceTransform.offsetMax;

            rectTransform.localScale    = sourceTransform.localScale;

            return failure;
        }

        private void ResetLocalScores()
        {
            if ((DateTime.Today - LastHighscoreDate.Date).TotalDays > 1)
            {
                _gameServicesProvider.SubmitScore(Keys.LOCAL_DAILY_HIGHSCORE, 0);
            }

            if ((DateTime.Today - LastHighscoreDate.Date).TotalDays > 7)
            {
                _gameServicesProvider.SubmitScore(Keys.LOCAL_WEEKLY_HIGHSCORE, 0);
            }

            PlayerPrefs.Save();
        }

        private void InitRandomScores()
        {
            for (int multiplier = 0; multiplier <= MaxMultiplier; multiplier++)
            {
                _possibleScores.Add(Random.Range(500, 3000));
            }
        }

        public void LogIn()
        {
            //PlayGamesPlatform.DebugLogEnabled = true;
            Social.Active.localUser.Authenticate(success =>
            {
                UseSocialPlatform = success;
            });
        }

        public void LogOut()
        {
            #if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
            PlayGamesPlatform.Instance.SignOut();
            #endif
        }

        private void AuthCallback(bool success)
        {
            print(success ? "authentification succeeded" : "authentification failed");
        }

        // ReSharper disable once UnusedMember.Local
        void OnDestroy()
        {
            CancelInvoke();

            if(_gameServicesProvider != null)
                _gameServicesProvider.SaveLocal();
        }

        public void ResetScoreOnDeath()
        {
            CurrentScore.CurrentScore.Value = 0;
        }

        public string ChangeScore()
        {
            int delta       = RandomScore;
            int multiplied  = delta * Mathf.Max(1, CurrentScore.Multiplier.Value);

            CurrentScore.CurrentScore.Value += multiplied;

            _gameServicesProvider.IncrementAchievement(Keys.LOCAL_HARVESTED);

            StartCoroutine(HandleHarvestingAchievements());

            string value;
            if (_scoreMultiplierStringCache.TryGetValue(multiplied, out value)) return value;

            value = CurrentScore.Multiplier.Value > 1 
                ? string.Format("{0} X{1}", delta, CurrentScore.Multiplier.Value) 
                : delta.ToString();

            _scoreMultiplierStringCache.Add(multiplied, value);

            return value;
        }

        public bool IsInSpree { get { return CurrentScore.Multiplier.Value > 1; } }

        public void ChangeSpree(bool isBlackHole)
        {
            if (!isBlackHole)
            {
                _spreePercentage    = Mathf.Min(_maxSpreePercentage, _spreePercentage + SpreeFactor);
                _lastHitTime        = Time.timeSinceLevelLoad;
            }
            else
            {
                _spreePercentage = Mathf.Max(100, _spreePercentage - 300f);
            }
            
            var newValue = Mathf.FloorToInt(_spreePercentage / 100);

            if (CurrentScore.Multiplier.Value != newValue && OnSpreeReached != null)
                OnSpreeReached.Invoke();

            CurrentScore.Multiplier.Value = newValue;
        }

        IEnumerator PropagateSurvivalTime()
        {
            while (true)
            {
                CurrentScore.SurvivedTime.Value = _survivalTime;

                yield return CommonWaits.Wait0_5S;
            }
        }
        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if(Mathf.Abs(Time.timeSinceLevelLoad - _lastHitTime) > SpreeCooldown)
                _spreePercentage = Mathf.Lerp(_spreePercentage, 100, 1 * Time.deltaTime);

            _survivalTime += Time.deltaTime;
        }

        private GameObject _currentPopup;
        public List<string> Failures;
        public List<string> Successes;

        private string _lastText = string.Empty;
        private void ShowGameOverPopup()
        {
            //var newScore = Score.Value;
            //var oldScore = LocalHighscore;

            
            //GameOverScore.HighscoreTitle.ChangeKey(newScore >= oldScore 
            //    ? "GameOver.BestNew"
            //    : "GameOver.Best");

            //GameOverScore.Score.text = newScore.ToString();
            //GameOverScore.Highscore.text = newScore >= oldScore 
            //    ? newScore.ToString() 
            //    : oldScore.ToString();
        }
        
        private IEnumerator HandleSurvivalTimeAchievements()
        {
            while (true)
            {
                var survivalTime = _survivalTime;

                if (survivalTime > 60f) // 1 minute
                {
                    _gameServicesProvider.UnlockAchievement(Keys.ACH_SURVIVOR_I);
                    yield return null;
                }

                if (survivalTime > 120f) // 2 minutes
                {
                    _gameServicesProvider.UnlockAchievement(Keys.ACH_SURVIVOR_II);
                    yield return null;
                }

                if (survivalTime > 180f) // 3 minutes
                {
                    _gameServicesProvider.UnlockAchievement(Keys.ACH_SURVIVOR_III);

                    yield return null;
                }

                if (survivalTime > 300f) // 5 minutes
                {
                    _gameServicesProvider.UnlockAchievement(Keys.ACH_SURVIVOR_IV);

                    yield break;
                }

                yield return CommonWaits.Wait0_5S;
            }
        }

        public IEnumerator HandleHarvestingAchievements()
        {
            var harvested = _gameServicesProvider[Keys.LOCAL_HARVESTED].Value;

            if (harvested <= 200)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_HARVESTER_I);
                yield return CommonWaits.Wait0_1S;
            }

            if (harvested <= 1000)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_HARVESTER_II);
                yield return CommonWaits.Wait0_1S;
            }

            if (harvested <= 5000)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_HARVESTER_III);
                yield return CommonWaits.Wait0_1S;
            }

            if (harvested <= 50000 && harvested%5 == 0) // 50k to 10k mapping => step = 5
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_HARVESTER_IV);
                yield return CommonWaits.Wait0_1S;
            }

            if (harvested <= 1000000 && harvested%100 == 0) // 1m to 10k mapping => step = 100
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_HARVESTER_V);
            }
        }

        public IEnumerator HandleDeathAchievements()
        {
            var died = _gameServicesProvider[Keys.LOCAL_DEATHS].Value;

            if (died <= 50)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_NOOB_I);
                yield return null;
            }

            if (died <= 200)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_NOOB_II);
                yield return null;
            }

            if (died <= 1000)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_NOOB_III);
                yield return null;
            }

            if (died <= 3000)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_NOOB_IV);
                yield return null;
            }

            if (died <= 10000)
            {
                _gameServicesProvider.IncrementAchievement(Keys.ACH_NOOBUS_MAXIMUS);
            }
        }
        
        private void ShowHighscorePopup()
        {
            //GameObject popup;

            //do
            //{
            //    popup = _highscorePopups.RandomElement();
            //} while (_currentPopup == popup);

            //_currentPopup = popup;

            //var view    = popup.GetComponent<HighscorePopup>();
            //view.ScoreText.text = Score.Value.ToString();

            string text;
            do
            {
                text = Successes.RandomElement();
            } while (_lastText == text);

            //GameOverText = text + "\n • \n " + Score.Value;

            GameOverText = string.Format("<color=green>-{0}</color>", CurrentScore.CurrentScore.Value);

            _lastText = text;
            //_scoreFsm.GoTo(view);
            //_scoreFsm.Invoke("Previous", 1f);
        }
        
        public void Save()
        {
            _gameServicesProvider.SubmitScore(Keys.LOCAL_HIGHSCORE, CurrentScore.CurrentScore.Value);
            _gameServicesProvider.SubmitScore(Keys.LEAD_HIGHSCORE, CurrentScore.CurrentScore.Value);

            int index = LocalHighscores.FindIndex(val => val < CurrentScore.CurrentScore.Value);
            if(index > -1)
                LocalHighscores.Insert(index, CurrentScore.CurrentScore.Value);

            LocalHighscores = LocalHighscores;

            LastHighscoreDate = DateTime.Today;

            //if (    Get(Keys.LOCAL_HIGHSCORE)       .SubmitScore(Score.Value)
            //    |   Get(Keys.LOCAL_DAILY_HIGHSCORE) .SubmitScore(Score.Value)
            //    |   Get(Keys.LOCAL_WEEKLY_HIGHSCORE).SubmitScore(Score.Value))
            //{
            //    ShowHighscorePopup();
            //}
            //else
            //{
            //    ShowGameOverPopup();
            //}



            _gameServicesProvider.SubmitScore(Keys.LOCAL_DAILY_HIGHSCORE, CurrentScore.CurrentScore.Value);
            _gameServicesProvider.SubmitScore(Keys.LOCAL_WEEKLY_HIGHSCORE, CurrentScore.CurrentScore.Value);

            _gameServicesProvider.IncrementAchievement(Keys.LOCAL_DEATHS);
            
            StartCoroutine(HandleDeathAchievements());
            
            if (_gameServicesProvider[Keys.LOCAL_SHORTLIVED].Value == 0 && _survivalTime < 15f) // Short - Lived
            {
                _gameServicesProvider.UnlockAchievement(Keys.ACH_SHORTLIVED);
                _gameServicesProvider.UnlockAchievement(Keys.LOCAL_SHORTLIVED);
            }


            _survivalTime = 0;

            //Chartboost.showInterstitial(CBLocation.Default);

            //PlayerPrefs.Save();
        }
        public void ShowLeaderboardUi()
        {
            if (!Social.Active.localUser.authenticated)
            {
                Social.Active.localUser.Authenticate(success =>
                {
                    if (success)
                    {
                        StartCoroutine(_gameServicesProvider.SubmitOfflineData());

                        _gameServicesProvider.ShowLeaderboardUi(Keys.LEAD_HIGHSCORE);
                    }
                    //((PlayGamesPlatform) Social.Active).ShowLeaderboardUI(Get(Keys.LEAD_HIGHSCORE).ID);
                });
            }
            else
            {
                _gameServicesProvider.ShowLeaderboardUi(Keys.LEAD_HIGHSCORE);
            }
        }

        public void ShowAchievementsUi()
        {
            if (!Social.Active.localUser.authenticated)
            {
                Social.Active.localUser.Authenticate(success =>
                {
                    if (success)
                    {
                        StartCoroutine(_gameServicesProvider.SubmitOfflineData());
                        Social.Active.ShowAchievementsUI();
                    }
                    //((PlayGamesPlatform) Social.Active).ShowAchievementsUI();
                });
            }
            else
            {
                Social.Active.ShowAchievementsUI();
                //((PlayGamesPlatform) Social.Active).ShowAchievementsUI();
            }
        }
    }
}