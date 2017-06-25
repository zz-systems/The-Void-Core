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

namespace ZzSystems.TheVoid.Visual.UI
{
    [RequireComponent(typeof(GUITexture))]
    public class SceneFade : MonoBehaviour {
        public float FadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
	
        private bool _sceneStarting = true;      // Whether or not the scene is still fading in.
	
	
        void Awake ()
        {
            // Set the texture so that it is the the size of the screen and covers it.
            GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        }
	
	
        void Update ()
        {
            // If the scene is starting...
            if(_sceneStarting)
                // ... call the StartScene function.
                StartScene();
        }


        void FadeToClear()
        {
            // Lerp the colour of the texture between itself and transparent.
            GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, FadeSpeed * Time.deltaTime);
        }


        void FadeToBlack()
        {
            // Lerp the colour of the texture between itself and black.
            GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, FadeSpeed * Time.deltaTime);
        }
	
	
        void StartScene ()
        {
            // Fade the texture to clear.
            FadeToClear();
		
            // If the texture is almost clear...
            if(GetComponent<GUITexture>().color.a <= 0.05f)
            {
                // ... set the colour to clear and disable the GUITexture.
                GetComponent<GUITexture>().color = Color.clear;
                GetComponent<GUITexture>().enabled = false;
			
                // The scene is no longer starting.
                _sceneStarting = false;
            }
        }
	
	
        public void EndScene ()
        {
            // Make sure the texture is enabled.
            GetComponent<GUITexture>().enabled = true;
		
            // Start fading towards black.
            FadeToBlack();
		
            // If the screen is almost black...
            //if(guiTexture.color.a >= 0.95f)
            // ... reload the level.
            //Application.LoadLevel(0);
        }
    }
}