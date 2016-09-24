using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public delegate void MultiFireAction(float totalElapsed, int fireNumber);
    public class Timer : ObjectBase
    {
        //Set on start
        private MultiFireAction _multiMethod;
        private Action _singleMethod;
        private double _secondsPerFire;
        private double _startSeconds;
        private int _fireCount;

        //State
        private int _fireNumber;
        private bool _isRunning;
        private double _totalElapsed;
        private double _elapsedSinceLastFire;

        public void Reset()
        {
            _fireNumber = 0;
            _fireCount = 1;
            _totalElapsed = 0;
            _elapsedSinceLastFire = 0;
            _secondsPerFire = 0;
            _startSeconds = 0;
            _multiMethod = null;
            _singleMethod = null;
        }
        public void Stop()
        {
            Reset();
            _isRunning = false;
            if (Form._activeTimers.Contains(this))
                Form._activeTimers.Remove(this);
        }
        public void RunSingleFire(Action method, float seconds)
        {
            Reset();

            _singleMethod = method;
            _startSeconds = seconds;
        }
        public void RunMultiFire(MultiFireAction method, float secondsPerFire, int fireCount = 1, float startSeconds = 0.0f)
        {
            Reset();

            _multiMethod = method;
            _fireCount = fireCount;

            _startSeconds = startSeconds;
            _isRunning = true;
            if (!Form._activeTimers.Contains(this))
                Form._activeTimers.Add(this);
        }
        public void UpdateTick(double deltaTime)
        {
            _totalElapsed += deltaTime;
            _elapsedSinceLastFire += deltaTime;
            if ((_fireNumber == 0 && _elapsedSinceLastFire > _startSeconds) && _elapsedSinceLastFire > _secondsPerFire)
                if (_multiMethod != null)
                {
                    _multiMethod((float)_totalElapsed, _fireNumber++);
                    _elapsedSinceLastFire = 0;
                }
                else if (_singleMethod != null)
                {
                    _singleMethod();
                    Stop();
                }
        }
    }
}