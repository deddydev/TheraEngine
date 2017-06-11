﻿using TheraEngine;

namespace System
{
    public delegate void MultiFireAction(float totalElapsed, int fireNumber);
    public class GameTimer : ObjectBase
    {
        public bool IsRunning => _isRunning;

        //Set on start
        private MultiFireAction _multiMethod;
        private Action _singleMethod;
        private float _secondsBetweenFires;
        private float _startSeconds;
        private int _fireMax;

        //State
        private int _fireNumber;
        private bool _isRunning;
        private float _totalElapsed;
        private float _elapsedSinceLastFire;

        private void Reset()
        {
            _fireNumber = 0;
            _fireMax = -1;
            _totalElapsed = 0;
            _elapsedSinceLastFire = 0;
            _secondsBetweenFires = 0;
            _startSeconds = 0;
            _multiMethod = null;
            _singleMethod = null;
        }
        public void Stop()
        {
            if (!_isRunning)
                return;

            Reset();
            _isRunning = false;

            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Timers, Tick);
        }
        /// <summary>
        /// Executes a method once after the given time period.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        /// <param name="seconds">How much time should pass before executing the method.</param>
        public void StartSingleFire(Action method, float seconds)
        {
            if (_isRunning)
                Stop();
            else
                Reset();

            if (seconds <= 0)
                method();
            else
            {
                _isRunning = true;
                _singleMethod = method;
                _startSeconds = seconds;

                RegisterTick(ETickGroup.PrePhysics, ETickOrder.Timers, Tick);
            }
        }
        /// <summary>
        /// Executes a single method multiple times with a given interval of time between each execution.
        /// </summary>
        /// <param name="method">The method to execute per fire.</param>
        /// <param name="secondsBetweenFires">How many seconds should pass before running the method again.</param>
        /// <param name="maxFires">The maximum number of times the method should execute before the timer stops completely. Pass a number less than 0 for infinite.</param>
        /// <param name="startSeconds">How many seconds should pass before running the method for the first time.</param>
        public void StartMultiFire(MultiFireAction method, float secondsBetweenFires, int maxFires = -1, float startSeconds = 0.0f)
        {
            if (_isRunning)
                Stop();
            else
                Reset();

            if (maxFires == 0 || method == null)
                return;

            _multiMethod = method;
            _fireMax = maxFires;

            _startSeconds = startSeconds;
            _isRunning = true;

            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Timers, Tick);
        }
        private void Tick(float delta)
        {
            _totalElapsed += delta;
            _elapsedSinceLastFire += delta;
            if ((_fireNumber == 0 && _elapsedSinceLastFire > _startSeconds) && _elapsedSinceLastFire > _secondsBetweenFires)
                if (_multiMethod != null)
                {
                    _multiMethod(_totalElapsed, _fireNumber++);
                    _elapsedSinceLastFire = 0;
                    if (_fireNumber >= _fireMax)
                        Stop();
                }
                else if (_singleMethod != null)
                {
                    _singleMethod();
                    Stop();
                }
        }
    }
}