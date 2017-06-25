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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZzSystems.TheVoid.Visual.UI
{
    public class ChangeScene : MonoBehaviour
    {

        public Image Progressbar;

        public void ChangeToGame()
        {
            StartCoroutine(ChangeToScene("Game"));
        }

        public void ChangeToGameOver()
        {
            StartCoroutine(ChangeToScene("GameOver"));
        }

        public void ChangeToMainMenu()
        {
            StartCoroutine(ChangeToScene("Menu"));
        }

        public IEnumerator ChangeToScene(string sceneName)
        {
            FindObjectOfType<SceneFade>().EndScene();
            Time.timeScale = 1;
            var async = SceneManager.LoadSceneAsync(sceneName);

            if (Progressbar != null)
                while (async.progress < 1)
                {
                    Progressbar.fillAmount = async.progress;

                    yield return null;
                }

            yield return async;
        }
    }
}
