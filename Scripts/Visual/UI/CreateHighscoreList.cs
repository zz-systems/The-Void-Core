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
using ZzSystems.TheVoid.Behavior.Star;
using UnityEngine;
using UnityEngine.UI;
using ZzSystems.TheVoid.GameState;

namespace ZzSystems.TheVoid.Visual.UI
{
    [Serializable]
    public class HighscoreItem
    {
        public int Score;
    }

    public class CreateHighscoreList : MonoBehaviour
    {

        public ScoreController Score;
        public GameObject EntryTemplate;
        //public List<HighscoreItem> itemList;

        public Transform ContentPanel;
        
        void Awake()
        {
            PopulateList();
        }

        void PopulateList()
        {
            
            for (int i = 0; i < Score.LocalHighscores.Count; i++)
            {
                var item = Score.LocalHighscores[i];
                var entryObject = Instantiate(EntryTemplate);
                
                
                entryObject.transform.SetParent(ContentPanel);
                entryObject.transform.localScale = Vector3.one;

                var entry = entryObject.GetComponent<HighscoreEntry>();

                if (item > 0)
                    entry.Text.text = item.ToString();
                
            }
        }
    }
}