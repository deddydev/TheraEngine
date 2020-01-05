using Extensions;
using WindowsNativeInterop;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Reflection;
using System.ComponentModel;

namespace TheraEngine.Timers
{
    public class EngineTimer : TObjectSlim
    {
        const float MaxFrequency = 500.0f; // Frequency cap for Update/RenderFrame events

        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event EventHandler<FrameEventArgs> PreRenderFrame;
        public event EventHandler<FrameEventArgs> RenderFrame;

        public event Action SwapRenderBuffers;

        private bool _isSingleThreaded = false;
        private float _targetUpdatePeriod, _targetRenderPeriod;
        private float _lastUpdateTimestamp; // timestamp of last UpdateFrame event
        private float _lastRenderTimestamp; // timestamp of last RenderFrame event
        private float _lastPreRenderTimestamp;
        
        private float _updateEpsilon = 0.0f; // quantization error for UpdateFrame events
        private bool _isRunningSlowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        private readonly FrameEventArgs _updateArgs = new FrameEventArgs();
        private readonly FrameEventArgs _renderArgs = new FrameEventArgs();
        private readonly FrameEventArgs _preRenderArgs = new FrameEventArgs();
        private readonly Stopwatch _watch = new Stopwatch();

        private ManualResetEventSlim _renderStarted;
        private ManualResetEventSlim _preRenderDone;
        private ManualResetEventSlim _renderDone;

        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Gets a float representing the period of RenderFrame events, in seconds.
        /// </summary>
        public float RenderPeriod { get; private set; }

        /// <summary>
        /// Gets a float representing the time spent in the RenderFrame function, in seconds.
        /// </summary>
        public float RenderTime { get; private set; }

        /// <summary>
        /// Gets a float representing the period of UpdateFrame events, in seconds (seconds per update).
        /// </summary>
        public float UpdatePeriod { get; private set; }

        /// <summary>
        /// Gets a float representing the time spent in the UpdateFrame function, in seconds.
        /// </summary>
        public float UpdateTime { get; private set; }
        public float TimeDilation { get; set; } = 1.0f;

        /// <summary>
        /// Runs the timer until Stop() is called.
        /// </summary>
        public void Run()
        {
            if (IsRunning)
                return;

            Engine.PrintLine($"Started {(IsSingleThreaded ? "single" : "multi")}-threaded game loop.");

            InitiateLoop();
        }
        private void MakeManualResetEvents()
        {
            _preRenderDone = new ManualResetEventSlim(false);
            _renderStarted = new ManualResetEventSlim(false);
            _renderDone = new ManualResetEventSlim(true);
            //_preSimTickDone = new ManualResetEventSlim(false);
        }

        private Task UpdateTask = null;
        private Task PreRenderTask = null;
        private Task RenderTask = null;
        private Task SingleTask = null;

        private void InitiateLoop()
        {
            IsRunning = true;
            _watch.Start();

            if (AppDomainHelper.IsPrimaryDomain)
            {
                if (IsSingleThreaded)
                    Application.Idle += UIDomainSingleThreadLoop;
                else
                {
                    MakeManualResetEvents();

                    UpdateTask = Task.Run(UpdateThread);
                    PreRenderTask = Task.Run(PreRenderThread);
                    Application.Idle += UIDomainRenderLoop;
                }
            }
            else
            {
                if (IsSingleThreaded)
                    SingleTask = Task.Run(GameDomainSingleThreadLoop);
                else
                {
                    MakeManualResetEvents();

                    UpdateTask = Task.Run(UpdateThread);
                    PreRenderTask = Task.Run(PreRenderThread);
                    RenderTask = Task.Run(RenderThread);
                }
            }
        }

        private bool IsApplicationIdle() => NativeMethods.PeekMessage(out _, IntPtr.Zero, 0, 0, 0) == 0;
        private void UIDomainSingleThreadLoop(object sender, EventArgs e)
        {
            while (IsApplicationIdle() && IsSingleThreadActive)
                SingleThreadLoop();
        }
        private void UIDomainRenderLoop(object sender, EventArgs e)
        {
            while (IsApplicationIdle() && IsMultiThreadActive)
                MultiThreadRenderLoop();
        }

        private void GameDomainSingleThreadLoop()
        {
            while (IsSingleThreadActive)
                SingleThreadLoop();
        }

        //private ManualResetEventSlim _preSimTickDone;

        public void PreSimulationTick(float delta)
        {
            Engine.TickGroup(ETickGroup.PrePhysics, delta);
            //_preSimTickDone.Set();
        }
        public void PostSimulationTick(float delta)
        {
            Engine.TickGroup(ETickGroup.PostPhysics, delta);
        }

        private void UpdateThread()
        {
            while (IsMultiThreadActive)
                DispatchUpdate();
        }
        private void PreRenderThread()
        {
            while (IsMultiThreadActive)
                MultiThreadPreRenderLoop();
        }
        private void RenderThread()
        {
            while (IsMultiThreadActive)
                MultiThreadRenderLoop();
        }

        private void SingleThreadLoop()
        {
            DispatchUpdate();
            DispatchPreRender();
            OnSwapBuffers();
            DispatchRender();
        }
        private void MultiThreadPreRenderLoop()
        {
            DispatchPreRender();
            WaitRenderDone();
            OnSwapBuffers();
            SetPreRenderDone();
            WaitRenderStarted();
        }
        private void MultiThreadRenderLoop()
        {
            SetRenderDone();
            WaitPreRenderDone();
            SetRenderStarted();
            while (!DispatchRender()) ;
        }
        private void MultiThreadRenderLoopSingle()
        {
            DispatchPreRender();
            OnSwapBuffers();
            while (!DispatchRender()) ;
        }

        private void OnSwapBuffers()
            => SwapRenderBuffers?.Invoke();

        private void SetRenderStarted()
        {
            _renderStarted.Set();
        }
        private void SetPreRenderDone()
        {
            _preRenderDone.Set();
        }
        private void SetRenderDone()
        {
            _renderDone.Set();
        }
        private void WaitRenderStarted()
        {
            _renderStarted.Wait();
            _renderStarted.Reset();
        }
        private void WaitPreRenderDone()
        {
            _preRenderDone.Wait();
            _preRenderDone.Reset();
        }
        private void WaitRenderDone()
        {
            _renderDone.Wait();
            _renderDone.Reset();
        }

        public bool IsSingleThreadActive => IsRunning && IsSingleThreaded;
        public bool IsMultiThreadActive => IsRunning && !IsSingleThreaded;

        public void Stop()
        {
            IsRunning = false;

            _renderDone?.Set();
            _preRenderDone?.Set();
            _renderStarted?.Set();
            //_preSimTickDone?.Set();

            UpdateTask?.Wait();
            UpdateTask = null;

            PreRenderTask?.Wait();
            PreRenderTask = null;

            RenderTask?.Wait();
            RenderTask = null;

            SingleTask?.Wait();
            SingleTask = null;

            if (AppDomainHelper.IsPrimaryDomain)
            {
                if (_renderStarted != null)
                    Application.Idle -= UIDomainRenderLoop;
                else
                    Application.Idle -= UIDomainSingleThreadLoop;
            }

            _watch.Stop();

            _preRenderDone = null;
            _renderStarted = null;
            _renderDone = null;

            Engine.PrintLine("Game loop ended.");
        }
        private bool DispatchRender()
        {
            float timestamp = (float)_watch.Elapsed.TotalSeconds;
            float elapsed = (timestamp - _lastRenderTimestamp).Clamp(0.0f, 1.0f);
            bool dispatch = elapsed > 0 && elapsed >= TargetRenderPeriod;
            if (dispatch)
                RaiseRenderFrame(elapsed, ref timestamp);
            return dispatch;
        }
        private void DispatchPreRender()
        {
            float timestamp = (float)_watch.Elapsed.TotalSeconds;
            float elapsed = (timestamp - _lastPreRenderTimestamp).Clamp(0.0f, 1.0f);
            RaisePreRenderFrame(elapsed, ref timestamp);
        }
        private void DispatchUpdate()
        {
            //int runningSlowlyRetries = 4;

            float timestamp = (float)_watch.Elapsed.TotalSeconds;
            float elapsed = (timestamp - _lastUpdateTimestamp).Clamp(0.0f, 1.0f);

            if (elapsed > 0 && elapsed >= TargetUpdatePeriod)
            //    RaiseUpdateFrame(elapsed, ref timestamp);

            //while (IsRunning && elapsed > 0 && elapsed + _updateEpsilon >= TargetUpdatePeriod)
            {
                //RaiseUpdateFrame(elapsed, ref timestamp);

                if (Engine.IsPaused)
                    Engine.TickGroup(ETickGroup.PrePhysics, elapsed);

                Engine.TickGroup(ETickGroup.DuringPhysics, elapsed);

                if (Engine.IsPaused)
                    Engine.TickGroup(ETickGroup.PostPhysics, elapsed);

                // Calculate difference (positive or negative) between
                // actual elapsed time and target elapsed time. We must
                // compensate for this difference.
                //_updateEpsilon += elapsed - TargetUpdatePeriod;

                //// Prepare for next loop
                //elapsed = (timestamp - _lastUpdateTimestamp).Clamp(0.0f, 1.0f);

                //if (TargetUpdatePeriod <= float.Epsilon)
                //{
                //    // According to the TargetUpdatePeriod documentation,
                //    // a TargetUpdatePeriod of zero means we will raise
                //    // UpdateFrame events as fast as possible (one event
                //    // per ProcessEvents() call)
                //    break;
                //}

                //_isRunningSlowly = _updateEpsilon >= TargetUpdatePeriod;
                //if (_isRunningSlowly && --runningSlowlyRetries == 0)
                //{
                //    // If UpdateFrame consistently takes longer than TargetUpdateFrame
                //    // stop raising events to avoid hanging inside the UpdateFrame loop.
                //    break;
                //}
            }
        }
        private void RaiseUpdateFrame(float elapsed, ref float timestamp)
        {
            // Raise UpdateFrame event
            _updateArgs.Time = elapsed * TimeDilation;
            OnUpdateFrameInternal(_updateArgs);

            // Update UpdatePeriod/UpdateFrequency properties
            UpdatePeriod = elapsed;

            // Update UpdateTime property
            _lastUpdateTimestamp = timestamp;
            timestamp = (float)_watch.Elapsed.TotalSeconds/* * TimeDilation*/;
            UpdateTime = timestamp - _lastUpdateTimestamp;
        }
        void RaiseRenderFrame(float elapsed, ref float timestamp)
        {
            // Raise RenderFrame event
            _renderArgs.Time = elapsed * TimeDilation;
            OnRenderFrameInternal(_renderArgs);

            // Update RenderPeriod/UpdateFrequency properties
            RenderPeriod = elapsed;

            // Update RenderTime property
            _lastRenderTimestamp = timestamp;
            timestamp = (float)_watch.Elapsed.TotalSeconds;
            RenderTime = timestamp - _lastRenderTimestamp;
        }
        void RaisePreRenderFrame(float elapsed, ref float timestamp)
        {
            // Raise RenderFrame event
            _preRenderArgs.Time = elapsed * TimeDilation;
            OnPreRenderFrameInternal(_preRenderArgs);
            _lastPreRenderTimestamp = timestamp;
        }

        private void OnPreRenderFrameInternal(FrameEventArgs e) => PreRenderFrame?.Invoke(this, e);
        private void OnRenderFrameInternal(FrameEventArgs e) => RenderFrame?.Invoke(this, e);
        private void OnUpdateFrameInternal(FrameEventArgs e) => UpdateFrame?.Invoke(this, e);
        
        /// <summary>
        /// Gets a float representing the actual frequency of RenderFrame events, in hertz (i.e. fps or frames per second).
        /// </summary>
        public float RenderFrequency
        {
            get
            {
                if (RenderPeriod == 0.0f)
                    return 1.0f;
                return 1.0f / RenderPeriod;
            }
        }

        /// <summary>
        /// Gets or sets a float representing the target render frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 200.0Hz.</para>
        /// </remarks>
        public float TargetRenderFrequency
        {
            get
            {
                if (_targetRenderPeriod == 0.0f)
                    return 0.0f;
                return 1.0f / _targetRenderPeriod;
            }
            set
            {
                if (value < 1.0f)
                {
                    _targetRenderPeriod = 0.0f;
                    Engine.PrintLine("Target render frequency set to unrestricted.");
                }
                else if (value < MaxFrequency)
                {
                    _targetRenderPeriod = 1.0f / value;
                    Engine.PrintLine("Target render frequency set to {0}Hz.", value.ToString());
                }
                else
                {
                    _targetRenderPeriod = 1.0f / MaxFrequency;
                    Engine.PrintLine("Target render frequency clamped to {0}Hz.", MaxFrequency.ToString());
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a float representing the target render period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public float TargetRenderPeriod
        {
            get => _targetRenderPeriod;
            set
            {
                if (value < 1.0f / MaxFrequency)
                {
                    _targetRenderPeriod = 0.0f;
                    Engine.PrintLine("Target render frequency set to unrestricted.");
                }
                else if (value < 1.0f)
                {
                    _targetRenderPeriod = value;
                    Engine.PrintLine("Target render frequency set to {0}Hz.", TargetRenderFrequency.ToString());
                }
                else
                {
                    _targetRenderPeriod = 1.0f;
                    Engine.PrintLine("Target render frequency clamped to 1Hz.");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a float representing the target update frequency, in hertz.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 500.0Hz.</para>
        /// </remarks>
        public float TargetUpdateFrequency
        {
            get
            {
                if (_targetUpdatePeriod == 0.0f)
                    return 0.0f;
                return 1.0f / _targetUpdatePeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    _targetUpdatePeriod = 0.0f;
                    Engine.PrintLine("Target update frequency set to unrestricted.");
                }
                else if (value < MaxFrequency)
                {
                    _targetUpdatePeriod = 1.0f / value;
                    Engine.PrintLine("Target update frequency set to {0}Hz.", value);
                }
                else
                {
                    _targetUpdatePeriod = 1.0f / MaxFrequency;
                    Engine.PrintLine("Target update frequency clamped to {0}Hz.", MaxFrequency);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a float representing the target update period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public float TargetUpdatePeriod
        {
            get => _targetUpdatePeriod;
            set
            {
                if (value < 1.0f / MaxFrequency)
                {
                    _targetUpdatePeriod = 0.0f;
                    Engine.PrintLine("Target update frequency set to unrestricted.");
                }
                else if (value < 1.0)
                {
                    _targetUpdatePeriod = value;
                    Engine.PrintLine("Target update frequency set to {0}Hz.", TargetUpdateFrequency);
                }
                else
                {
                    _targetUpdatePeriod = 1.0f;
                    Engine.PrintLine("Target update frequency clamped to 1Hz.");
                }
            }
        }
        
        /// <summary>
        /// Gets a float representing the frequency of UpdateFrame events, in hertz (updates per second).
        /// </summary>
        public float UpdateFrequency
        {
            get
            {
                if (UpdatePeriod == 0.0f)
                    return 1.0f;
                return 1.0f / UpdatePeriod;
            }
        }

        public bool IsSingleThreaded
        {
            get => _isSingleThreaded;
            set
            {
                if (_isSingleThreaded == value)
                    return;

                Stop();

                _isSingleThreaded = value;

                Run();
            }
        }
    }

    [Serializable]
    public class FrameEventArgs : EventArgs
    {
        public FrameEventArgs() { }

        /// <param name="elapsed">The amount of time that has elapsed since the previous event, in seconds.</param>
        public FrameEventArgs(float elapsed) => Time = elapsed;

        /// <summary>
        /// Gets a <see cref="float"/> that indicates how many seconds of time elapsed since the previous event.
        /// </summary>
        public float Time { get; set; }
    }
}
