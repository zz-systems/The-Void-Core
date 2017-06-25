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
using Assets.Scripts.Spawners;
using Assets.Scripts.Util;
using DG.Tweening;
using UnityEngine;
using ZzSystems.TheVoid.Behavior.Shared;
using ZzSystems.TheVoid.Behavior.Star;
using ZzSystems.TheVoid.GameState;

namespace ZzSystems.TheVoid.Behavior.Player
{
    public class PlayerCollisionBehavior : MonoBehaviour
    {
        public GameStateController  GameState;
        public ScoreController      GameScore;

        public GameObject           DynamicScoreFactory;
        public GameObject           PlayerThoughtsFactory;
        
        private GameObject          _player;
        //private RandomSkyBox        _skybox;

        private const string Glyphs = "0123456789X ";

        void Awake()
        {
            _player             = transform.parent.gameObject;

            DynamicScoreFactory.CreatePool(10);
            PlayerThoughtsFactory.CreatePool(5);

            //_skybox = FindObjectOfType<RandomSkyBox>();

            var scoreText = DynamicScoreFactory.GetComponent<TextMesh>();

            StartCoroutine(PrecacheDynamicScoreFont(scoreText.font, scoreText.fontSize, scoreText.fontStyle, Glyphs));
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Star"))//.gameObject.CompareTag("Star"))
            {
                var star        = other/*Parent*/.GetComponent<RandomColorOnActivationBehavior>();
                var body        = other.attachedRigidbody;
                GameScore.ChangeSpree(false);
                var str = GameScore.ChangeScore();

                DisplayDynamicText(DynamicScoreFactory, -body.velocity, star.CurrentColor, str);
            }
            else if(other.gameObject.CompareTag("BlackHole"))
            {
                //var otherParent = other.transform.parent.gameObject;
                //var otherBody = otherParent.GetComponent<Rigidbody2D>();

                //_player.transform.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f), 20);
                _player.transform.DOShakeRotation(0.9f, new Vector3(0, 0, 360f));
                //_player.transform.DOMoveY(5, 0.5f);//new Vector3(0, 10), 0.1f);
                _player.transform.DOShakePosition(0.9f, new Vector3(0, 5f), 1);

                GameState.Invoke("GameOver", 0.5f);
                //DisplayDynamicText(PlayerThoughtsFactory, -otherBody.velocity, Color.white, GameScore.GameOverText);

                //_skybox.ChangeSkyBox();
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
                if (GameSettings.Instance.CurrentSettings.VibrationEnabled.Value)
                    Handheld.Vibrate();
#endif
            }
        }

        private void DisplayDynamicText(GameObject factory, Vector2 velocity, Color color, string text)
        {
            var dynamicScore = factory.Spawn(transform.parent.position);

            var impulse = new Vector2(Random.value - 2 * _player.transform.position.x,
                velocity.y);

            dynamicScore.GetComponent<Rigidbody2D>().AddRelativeForce(impulse, ForceMode2D.Impulse);

            dynamicScore.GetComponent<TextMesh>().color = color; ////ColorGenerator.Random();
            dynamicScore.GetComponent<TextMesh>().text = text;
        }

        private IEnumerator PrecacheDynamicScoreFont(Font font, int fontSize, FontStyle style, string glyphs)
        {
            for (int index = 0; (index < glyphs.Length); ++index)
            {
                font.RequestCharactersInTexture(
                    glyphs[index].ToString(),
                    fontSize, style);
                yield return null;
            }
        }
    }
}
