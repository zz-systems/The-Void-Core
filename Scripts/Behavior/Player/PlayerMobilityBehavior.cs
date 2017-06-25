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

using Assets.Scripts;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using ZzSystems.TheVoid.Behavior.Star;
using ZzSystems.TheVoid.GameState;

namespace ZzSystems.TheVoid.Behavior.Player
{
    public class PlayerMobilityBehavior : MonoBehaviour {

        public float Speed;
        public float Tilt 	= 30.0f;
        public float Smooth = 2.0f;

        public GameStateController  GameState;
        public GameSettings         GameSettings;

        private float _speed;

        private Vector3 _force;
        private Vector3         _accelCalibration;

        private int _playerFingerId = -1;

        void Start()
        {
            _accelCalibration   = Input.acceleration;
        }

        void FixedUpdate()
        {
            if (GameState.IsDead)
                return;

            _speed = Mathf.Lerp(_speed, 0.3f, Time.fixedDeltaTime);

            if (Application.isMobilePlatform) 		
            {
                if (Input.touchCount > 0) // Touch
                {
                    CalculateForceToTarget();
                }
                else // tilt
                {
                    CalculateTiltForce();
                }
            }
            else
            {
                CalculateDeltaForce();
            }

            _force = Vector3.Lerp(GetComponent<Rigidbody2D>().velocity, _force, 10);

            GetComponent<Rigidbody2D>().AddForce(_force);

            var tilt = Mathf.Abs(_force.x) < 0.01f ? 0 : -Mathf.Sign(_force.x) * Tilt;

            //_effects.SetThrusterDirection(new Vector3(-tilt, 0));

            var target = Quaternion.Euler(0, -tilt, tilt);
            // Dampen towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, target,
                Time.fixedDeltaTime * Smooth);

            //_effects.SetThrusterDirection(transform.rotation.eulerAngles);
        }

        void CalculateForceToTarget()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);

                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        continue;

                    if (touch.phase == TouchPhase.Began && _playerFingerId != touch.fingerId)
                    {
                        _playerFingerId = touch.fingerId;
                    }

                    if (touch.fingerId == _playerFingerId && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
                    //if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        var pos = touch.position;
                        var source = transform.position; //new Vector2(transform.position.x, 0f);
                        var target = UnityEngine.Camera.main.ScreenToWorldPoint(pos) + new Vector3(0, 1f);
                        //new Vector2(Camera.main.ScreenToWorldPoint(pos).x, 0f);

                        _force = Vector3.ClampMagnitude((target - source), 5f) * Speed * 2 * Time.fixedDeltaTime;
                    }
                    
                    if (touch.phase == TouchPhase.Ended && touch.fingerId == _playerFingerId)
                    {
                        _playerFingerId = -1;
                    }
                }
            }
            else
            {
                _force = Vector3.zero;
            }
        }

        void CalculateDeltaForce()
        {
            var target = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); //new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0f);

            _force = Vector3.ClampMagnitude(target, 1f) * Speed * 2 * Time.fixedDeltaTime;
        }

        private Vector3 _previousAcceleration = Vector3.zero;

        void CalculateTiltForce()
        {
            const float accelSmooth = 0.1f;

            // Use calibration
            var target = Input.acceleration - _accelCalibration; 

            // Apply filter
            //target          = (target * accelLowPass) + (_previousAcceleration * (1.0f - accelLowPass));
            var smoothed    = Vector3.Lerp(target, _previousAcceleration, accelSmooth * Time.deltaTime);

            _previousAcceleration = smoothed;
            _force          = smoothed  * Speed * Time.fixedDeltaTime;
        }
        
        public void Calibrate()
        {
            _accelCalibration = Input.acceleration;
        }

        void LateUpdate()
        {
            if (GameState.IsDead)
                return;

            var tp = GetComponent<Transform>().position;
            var main = UnityEngine.Camera.main;

            var left 	= main.ViewportToWorldPoint (new Vector3 (0.1f, 0)).x;
            var right 	= main.ViewportToWorldPoint (new Vector3 (0.9f, 0)).x;

            var top     = main.ViewportToWorldPoint(new Vector3(0, 0.2f)).y;
            var bottom  = main.ViewportToWorldPoint(new Vector3(0, 0.9f)).y;

            GetComponent<Transform>().position =
                new Vector3(Mathf.Clamp(tp.x + 0 * Speed * Time.fixedDeltaTime, left, right), Mathf.Clamp(tp.y + 0 * Speed * Time.fixedDeltaTime, top, bottom), tp.z);

            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
