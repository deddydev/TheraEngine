using System.Diagnostics;
using System.Threading;
using System;

namespace TheraEngine.Timers
{
    public class EngineTimer
    {
        const double MaxFrequency = 500.0; // Frequency cap for Update/RenderFrame events

        double _updatePeriod, _renderPeriod;
        double _targetUpdatePeriod, _targetRenderPeriod;

        double _updateTime; // length of last UpdateFrame event
        double _renderTime; // length of last RenderFrame event
        double _timeDilation = 1.0;

        double _updateTimestamp; // timestamp of last UpdateFrame event
        double _renderTimestamp; // timestamp of last RenderFrame event

        double _updateEpsilon = 0.0; // quantization error for UpdateFrame events

        bool _isRunningSlowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        FrameEventArgs _updateArgs = new FrameEventArgs();
        FrameEventArgs _renderArgs = new FrameEventArgs();

        readonly Stopwatch _watch = new Stopwatch();

        Thread _updateThread;

        public event EventHandler<FrameEventArgs> RenderFrame = delegate { };
        public event EventHandler<FrameEventArgs> UpdateFrame = delegate { };
        
        /// <summary>
        /// Runs the timer until Stop() is called.
        /// </summary>
        public void Run()
        {
            if (_running)
                return;
            _updateThread = new Thread(RunUpdateInternal)
            {
                Name = "Game Loop",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            _updateThread.Start();
        }
        private void RunUpdateInternal()
        {
            Debug.WriteLine("Started game loop on thread " + Thread.CurrentThread.ManagedThreadId);
            //RenderContext.Current.CreateContextForThread(Thread.CurrentThread);
            _running = true;
            _watch.Start();
            while (_running)
                DispatchUpdateAndRenderFrame();
            Debug.WriteLine("Game loop ended.");
        }
        public void Stop()
        {
            _running = false;
            _watch.Stop();
        }
        private void DispatchUpdateAndRenderFrame()
        {
            int runningSlowlyRetries = 4;
            double timestamp = _watch.Elapsed.TotalSeconds;
            double elapsed = (timestamp - _updateTimestamp).Clamp(0.0f, 1.0f);
            while (elapsed > 0 && elapsed + _updateEpsilon >= TargetUpdatePeriod)
            {
                RaiseUpdateFrame(elapsed, ref timestamp);

                // Calculate difference (positive or negative) between
                // actual elapsed time and target elapsed time. We must
                // compensate for this difference.
                _updateEpsilon += elapsed - TargetUpdatePeriod;

                // Prepare for next loop
                elapsed = (timestamp - _updateTimestamp).Clamp(0.0f, 1.0f);

                if (TargetUpdatePeriod <= Double.Epsilon)
                {
                    // According to the TargetUpdatePeriod documentation,
                    // a TargetUpdatePeriod of zero means we will raise
                    // UpdateFrame events as fast as possible (one event
                    // per ProcessEvents() call)
                    break;
                }

                _isRunningSlowly = _updateEpsilon >= TargetUpdatePeriod;
                if (_isRunningSlowly && --runningSlowlyRetries == 0)
                {
                    // If UpdateFrame consistently takes longer than TargetUpdateFrame
                    // stop raising events to avoid hanging inside the UpdateFrame loop.
                    break;
                }
            }

            timestamp = _watch.Elapsed.TotalSeconds;
            elapsed = (timestamp - _renderTimestamp).Clamp(0.0f, 1.0f);
            if (elapsed > 0 && elapsed >= TargetRenderPeriod)
                RaiseRenderFrame(elapsed, ref timestamp);
        }
        private void RaiseUpdateFrame(double elapsed, ref double timestamp)
        {
            // Raise UpdateFrame event
            _updateArgs.Time = elapsed;
            OnUpdateFrameInternal(_updateArgs);

            // Update UpdatePeriod/UpdateFrequency properties
            _updatePeriod = elapsed;

            // Update UpdateTime property
            _updateTimestamp = timestamp;
            timestamp = _watch.Elapsed.TotalSeconds * _timeDilation;
            _updateTime = timestamp - _updateTimestamp;
        }
        void RaiseRenderFrame(double elapsed, ref double timestamp)
        {
            // Raise RenderFrame event
            _renderArgs.Time = elapsed;
            OnRenderFrameInternal(_renderArgs);

            // Update RenderPeriod/UpdateFrequency properties
            _renderPeriod = elapsed;

            // Update RenderTime property
            _renderTimestamp = timestamp;
            timestamp = _watch.Elapsed.TotalSeconds;
            _renderTime = timestamp - _renderTimestamp;
        }

        private void OnRenderFrameInternal(FrameEventArgs e) { if (_running) OnRenderFrame(e); }
        private void OnUpdateFrameInternal(FrameEventArgs e) { if (_running) OnUpdateFrame(e); }
        private void OnRenderFrame(FrameEventArgs e) => RenderFrame?.Invoke(this, e);
        private void OnUpdateFrame(FrameEventArgs e) => UpdateFrame?.Invoke(this, e);

        private bool _running = false;
        public bool IsRunning => _running;
        
        /// <summary>
        /// Gets a double representing the actual frequency of RenderFrame events, in hertz (i.e. fps or frames per second).
        /// </summary>
        public double RenderFrequency
        {
            get
            {
                if (_renderPeriod == 0.0)
                    return 1.0;
                return 1.0 / _renderPeriod;
            }
        }
        
        /// <summary>
        /// Gets a double representing the period of RenderFrame events, in seconds.
        /// </summary>
        public double RenderPeriod => _renderPeriod;

        /// <summary>
        /// Gets a double representing the time spent in the RenderFrame function, in seconds.
        /// </summary>
        public double RenderTime => _renderTime;
        
        /// <summary>
        /// Gets or sets a double representing the target render frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 200.0Hz.</para>
        /// </remarks>
        public double TargetRenderFrequency
        {
            get
            {
                if (_targetRenderPeriod == 0.0)
                    return 0.0;
                return 1.0 / _targetRenderPeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    _targetRenderPeriod = 0.0;
                    Debug.Print("Target render frequency set to unrestricted speed.");
                }
                else if (value < MaxFrequency)
                {
                    _targetRenderPeriod = 1.0 / value;
                    Debug.Print("Target render frequency set to {0}Hz.", value);
                }
                else
                {
                    _targetRenderPeriod = 1.0 / MaxFrequency;
                    Debug.Print("Target render frequency clamped to {0}Hz.", MaxFrequency);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a double representing the target render period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public double TargetRenderPeriod
        {
            get => _targetRenderPeriod;
            set
            {
                if (value < 1.0 / MaxFrequency)
                {
                    _targetRenderPeriod = 0.0;
                    Debug.Print("Target render frequency set to unrestricted speed.");
                }
                else if (value < 1.0)
                {
                    _targetRenderPeriod = value;
                    Debug.Print("Target render frequency set to {0}Hz.", TargetRenderFrequency);
                }
                else
                {
                    _targetRenderPeriod = 1.0;
                    Debug.Print("Target render frequency clamped to 1Hz.");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a double representing the target update frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 500.0Hz.</para>
        /// </remarks>
        public double TargetUpdateFrequency
        {
            get
            {
                if (_targetUpdatePeriod == 0.0)
                    return 0.0;
                return 1.0 / _targetUpdatePeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    _targetUpdatePeriod = 0.0;
                    Debug.Print("Target update frequency set to unrestricted speed.");
                }
                else if (value < MaxFrequency)
                {
                    _targetUpdatePeriod = 1.0 / value;
                    Debug.Print("Target update frequency set to {0}Hz.", value);
                }
                else
                {
                    _targetUpdatePeriod = 1.0 / MaxFrequency;
                    Debug.Print("Target update frequency clamped to {0}Hz.", MaxFrequency);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a double representing the target update period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public double TargetUpdatePeriod
        {
            get => _targetUpdatePeriod;
            set
            {
                if (value < 1.0 / MaxFrequency)
                {
                    _targetUpdatePeriod = 0.0;
                    Debug.Print("Target update frequency set to unrestricted speed.");
                }
                else if (value < 1.0)
                {
                    _targetUpdatePeriod = value;
                    Debug.Print("Target update frequency set to {0}Hz.", TargetUpdateFrequency);
                }
                else
                {
                    _targetUpdatePeriod = 1.0;
                    Debug.Print("Target update frequency clamped to 1Hz.");
                }
            }
        }
        
        /// <summary>
        /// Gets a double representing the frequency of UpdateFrame events, in hertz (updates per second).
        /// </summary>
        public double UpdateFrequency
        {
            get
            {
                if (_updatePeriod == 0.0)
                    return 1.0;
                return 1.0 / _updatePeriod;
            }
        }
        
        /// <summary>
        /// Gets a double representing the period of UpdateFrame events, in seconds (seconds per update).
        /// </summary>
        public double UpdatePeriod => _updatePeriod;
        
        /// <summary>
        /// Gets a double representing the time spent in the UpdateFrame function, in seconds.
        /// </summary>
        public double UpdateTime => _updateTime;
        public double TimeDilation
        {
            get => _timeDilation;
            set => _timeDilation = value;
        }
    }

    public class FrameEventArgs : EventArgs
    {
        double elapsed;

        public FrameEventArgs() { }

        /// <param name="elapsed">The amount of time that has elapsed since the previous event, in seconds.</param>
        public FrameEventArgs(double elapsed)
        {
            Time = elapsed;
        }

        /// <summary>
        /// Gets a <see cref="System.Double"/> that indicates how many seconds of time elapsed since the previous event.
        /// </summary>
        public double Time
        {
            get { return elapsed; }
            set { elapsed = value; }
        }
    }
}
