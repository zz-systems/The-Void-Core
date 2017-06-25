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
    public class PauseGame : MonoBehaviour {
        public Canvas 		    Menu;
        //public SpaceSpawner 	System;

        private static bool _isPaused = false;

        public static bool IsPaused { get { return _isPaused; }}

        void Awake()
        {
            //menu.alpha = 0;

            Menu.GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
            Menu.GetComponent<GUITexture>().color = Color.black;
        }

        public void Pause()
        {
            /*
        menu.GetComponent<Animator>().SetInteger("mode", 1);
        menu.GetComponent<GUITexture>().enabled = true;

		isPaused 		= true;
		system.Pause ();
		Camera.main.BroadcastMessage ("OnGamePaused");
		bodies = GameObject.FindObjectsOfType<Rigidbody2D> ().ToDictionary(i => i, i => i.velocity);	

		foreach (var body in bodies.Keys)
			body.isKinematic = true;        
		//menu.alpha = 1;
         * */
        }

        public void Resume()
        {/*
        menu.GetComponent<Animator>().SetInteger("mode", 0);
        menu.GetComponent<GUITexture>().enabled = false;

		isPaused 		= false;
		system.Resume ();
		Camera.main.BroadcastMessage ("OnGameResumed");

		foreach (var kv in bodies)
		{
			kv.Key.isKinematic = false;
			kv.Key.velocity = kv.Value;
		}  */      
        }

        public void SwitchGameState()
        {
            if (_isPaused)
                Resume();
            else
                Pause ();
        }
    }
}
