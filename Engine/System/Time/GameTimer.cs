using CustomEngine;

namespace System
{
    public delegate void MultiFireAction(float totalElapsed, int fireNumber);
    public class GameTimer : ObjectBase
    {
        public bool IsRunning { get { return _isRunning; } }

        //Set on start
        private MultiFireAction _multiMethod;
        private Action _singleMethod;
        private float _secondsPerFire;
        private float _startSeconds;
        private int _fireCount;

        //State
        private int _fireNumber;
        private bool _isRunning;
        private float _totalElapsed;
        private float _elapsedSinceLastFire;

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

            UnregisterTick();
        }
        public void RunSingleFire(Action method, float seconds)
        {
            Reset();

            _isRunning = true;
            _singleMethod = method;
            _startSeconds = seconds;

            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Timers);
        }
        public void RunMultiFire(MultiFireAction method, float secondsPerFire, int fireCount = 1, float startSeconds = 0.0f)
        {
            Reset();

            _multiMethod = method;
            _fireCount = fireCount;

            _startSeconds = startSeconds;
            _isRunning = true;

            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Timers);
        }
        internal override void Tick(float delta)
        {
            _totalElapsed += delta;
            _elapsedSinceLastFire += delta;
            if ((_fireNumber == 0 && _elapsedSinceLastFire > _startSeconds) && _elapsedSinceLastFire > _secondsPerFire)
                if (_multiMethod != null)
                {
                    _multiMethod(_totalElapsed, _fireNumber++);
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