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
using JetBrains.Annotations;
using UnityEngine;
using ZzSystems.TheVoid.Behavior.Particles;
using ZzSystems.TheVoid.Behavior.ShapeContainer;
using ZzSystems.TheVoid.Behavior.Shared;
using ZzSystems.TheVoid.Resources;
using ZzSystems.Unity.Shared.Gui;
using ZzSystems.Unity.Shared.Resources;
using ZzSystems.Unity.Shared.Util;

namespace ZzSystems.TheVoid.GameState
{
    public class SpaceSpawner : MonoBehaviour
    {
        public GameObject StarFactory;
        public GameObject HoleFactory;
        public GameObject ShapeFactory;

        public IResourceProvider ResourceProvider;

        public GameObject Player;

        public MenuItem BlackHoleWarning;

        public int MaxStars = 2;
        public int MaxHoles = 2;
        public int ShapeMaxHoles    = 1;
        public int ShapeMinChildren = 3;
        public int ShapeMaxChildren = 6;

        public float BlackHoleProbability = 0.9f;
        public float ShapeProbability = 0.4f;

        public float SpawnWait = 0.5f;
        public float StartWait = 1;

        private readonly float[]    _rotations = { -75, -50, -25, 0, 25, 50, 75 };
        private SpawnModes          _spawnMode;
        private Vector3             _delta;
        private readonly Vector3    _center = new Vector3(0, 20, 10);

        private bool _burst;
        private ParticleSystem _playerSystem;

        private enum SpawnModes
        {
            SinWave         = 0,
            CosWave         = 1,
            SquareWave      = 2,
            TriangleWave    = 3,
            SawToothWave    = 4
        }

        [UsedImplicitly]
        private void Awake()
        {
            ResourceProvider = new DefaultResourceProvider();

            StarFactory = (GameObject)ResourceProvider.StarPrefab;
            HoleFactory = (GameObject)ResourceProvider.BlackHolePrefab;

            StarFactory.CreatePool(20);
            HoleFactory.CreatePool(10);
            //ShapeFactory.CreatePool(10);

            var left    = Camera.main.ViewportToWorldPoint(new Vector3(0.15f, 0)).x;
            var right   = Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0)).x;

            _delta = Vector3.right*(left - right)/2;


            _playerSystem = GameObject.FindGameObjectWithTag("PlayerMateria").GetComponent<ParticleSystem>();

            StartCoroutine(Spawner());

            StartCoroutine(ChangeSpawnMode());

            StartCoroutine(ChangeBurstMode());

            //HideBlackHoleWarning();
        }

        private IEnumerator Spawner()
        {
            yield return new WaitForSeconds(StartWait);
            float safeTime = 1.0f;

            while (true)
            {
                if (Random.value >= 1 - ShapeProbability)
                {
                    yield return new WaitForSeconds(safeTime/3);

                    SpawnShape(_center, _delta.x);

                    yield return new WaitForSeconds(safeTime/3);
                }
                else
                {
                    var location = _center + _delta * Deviation;

                    SpawnStar(location);

                    while (_burst)
                    {
                        yield return new WaitForSeconds(safeTime * 0.5f);

                        location = _center + _delta * Deviation;

                        SpawnStar(location);
                    } 
                }

                safeTime = Mathf.Max(safeTime - 0.1f, 0.3f);

                yield return new WaitForSeconds(safeTime);
            }
        }

        private IEnumerator ChangeBurstMode()
        {
            while (true)
            {
                _burst = Random.value > 0.7;

                if (_burst)
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
                else
                    yield return new WaitForSeconds(1f);
            }
        }
        private IEnumerator ChangeSpawnMode()
        {
            while (true)
            {
                _spawnMode = (SpawnModes) Random.Range((int) SpawnModes.SinWave, (int) SpawnModes.SawToothWave);

                yield return new WaitForSeconds(Random.Range(2f, 7f));
            }
        }

        private float Deviation
        {
            get
            {
                switch (_spawnMode)
                {
                    case SpawnModes.SinWave:
                        return Mathf.Sin(Time.timeSinceLevelLoad);
                    case SpawnModes.CosWave:
                        return Mathf.Cos(Time.timeSinceLevelLoad);
                    case SpawnModes.SquareWave:
                        return Mathf.Sign(Mathf.Cos(Time.timeSinceLevelLoad));
                    case SpawnModes.TriangleWave:
                        return Mathf.Abs(Time.timeSinceLevelLoad%4 - 2) - 1;
                    case SpawnModes.SawToothWave:
                        return Time.timeSinceLevelLoad - Mathf.Floor(Time.timeSinceLevelLoad);
                    default:
                        return Random.Range(-1f, 1f);
                }
            }
        }

        private GameObject SpawnStar(Vector3 location = default(Vector3), Transform parent = null)
        {
            return parent
                ? StarFactory.Spawn(parent, location)
                : StarFactory.Spawn(location);
        }

        private GameObject SpawnBlackHole(Vector3 location = default(Vector3), Transform parent = null)
        {
            //ShowBlackHoleWarning();
            //Invoke("HideBlackHoleWarning", 2);

            var hole = parent 
                ? HoleFactory.Spawn(parent, location)
                : HoleFactory.Spawn(location);

            hole.GetComponentInChildren<ParticleAttractorBehavior>().ParticleSystem = _playerSystem;
            hole.GetComponentInChildren<ParticleSystem>().Play();

            return hole;
        }

        private void SpawnShape(Vector3 center, float radius)
        {
            var spawnedHoles    = 0;

            var count = Random.Range(ShapeMinChildren, ShapeMaxChildren);

            var shape = SpawnShapeContainer(center).GetComponent<ShapeContainerBehavior>();
            shape.Radius = radius;
            shape.Components.Clear();
                        
            for(int i = 0; i < count; i++)
            {
                if (spawnedHoles < ShapeMaxHoles && Random.value >= (1 - BlackHoleProbability))
                {
                    var spawnedHole = SpawnBlackHole(parent: shape.transform);

                    shape.Components.Add(spawnedHole);

                    spawnedHoles++;
                }
                else
                {
                    var spawnedStar = SpawnStar(parent: shape.transform);
                    shape.Components.Add(spawnedStar);
                    //if(spawnedHole != null)
                    //    spawnedStar.GetComponentInChildren<ParticleAttractorBehavior>()
                }
            }

            shape.InitComponents();
        }

        private GameObject SpawnShapeContainer(Vector3 location)
        {
            var shapeContainer = ShapeFactory.Spawn(location);
            shapeContainer.GetComponent<RotateAroundPivotBehavior>().Speed = _rotations.RandomElement();

            return shapeContainer;
        }

        //private void ShowBlackHoleWarning()
        //{
        //    if (!BlackHoleWarning.IsVisible)
        //        BlackHoleWarning.Show();
        //}

        //private void HideBlackHoleWarning()
        //{
        //    if (BlackHoleWarning.IsVisible)
        //        BlackHoleWarning.Hide();
        //}
    }
}