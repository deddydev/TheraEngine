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

        public event EventHandler<FrameEventArgs> RenderFrame;
        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event Action SwapBuffers;

        private bool _isSingleThreaded = false;
        private float _targetUpdatePeriod, _targetRenderPeriod;
        private float _updateTimestamp; // timestamp of last UpdateFrame event
        private float _renderTimestamp; // timestamp of last RenderFrame event

        private float _updateEpsilon = 0.0f; // quantization error for UpdateFrame events
        private bool _isRunningSlowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        private readonly FrameEventArgs _updateArgs = new FrameEventArgs();
        private readonly FrameEventArgs _renderArgs = new FrameEventArgs();
        private readonly Stopwatch _watch = new Stopwatch();

        private ManualResetEventSlim _commandsReady;
        private ManualResetEventSlim _commandsSwappedForRender;
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
        public void Run(bool singleThreaded)
        {
            if (IsRunning)
                return;

            Engine.PrintLine($"Started {(singleThreaded ? "single" : "multi")}-threaded game loop.");

            IsSingleThreaded = singleThreaded;
            IsRunning = true;
            _watch.Start();

            InitiateLoop(singleThreaded);
        }
        private void MakeManualResetEvents()
        {
            _commandsReady = new ManualResetEventSlim(false);
            _commandsSwappedForRender = new ManualResetEventSlim(false);
            _renderDone = new ManualResetEventSlim(true);
        }
        private Task UpdateTask = null;
        private Task RenderTask = null;
        private Task SingleTask = null;
        private void InitiateLoop(bool singleThreaded)
        {
            if (AppDomainHelper.IsPrimaryDomain)
            {
                if (singleThreaded)
                    Application.Idle += Application_Idle_SingleThread;
                else
                {
                    MakeManualResetEvents();

                    UpdateTask = Task.Run(RunUpdateMultiThreadInternal);

                    Application.Idle += Application_Idle_MultiThread;
                }
            }
            else
            {
                if (singleThreaded)
                {
                    SingleTask = Task.Run(GameDomainSingleThreadLoop);
                }
                else
                {
                    MakeManualResetEvents();

                    UpdateTask = Task.Run(RunUpdateMultiThreadInternal);
                    RenderTask = Task.Run(RunRenderMultiThreadInternal);
                }
            }
        }

        #region Game Domain Loop
        private void GameDomainSingleThreadLoop()
        {
            while (IsSingleThreaded && IsRunning)
            {
                DispatchUpdate();
                SwapBuffers?.Invoke();
                DispatchRender();
            }
        }
        private void RunRenderMultiThreadInternal()
        {
            while (IsRunning && !IsSingleThreaded)
            {
                _commandsReady.Wait(); //Wait for the update thread to finish
                _commandsReady.Reset();

                //Swap command buffers
                //Update commands will be consumed by the render pass
                //And new update commands will be issued for the next render
                SwapBuffers?.Invoke();
                _commandsSwappedForRender.Set();

                DispatchRender();
                _renderDone.Set(); //Signal the update thread that rendering is done
            }
        }
        #endregion

        #region UI Domain Loop
        private bool IsApplicationIdle() => NativeMethods.PeekMessage(out _, IntPtr.Zero, 0, 0, 0) == 0;
        private void Application_Idle_SingleThread(object sender, EventArgs e)
        {
            while (IsApplicationIdle() && IsSingleThreaded)
            {
                DispatchUpdate();
                SwapBuffers?.Invoke();
                DispatchRender();
            }
        }
        private void Application_Idle_MultiThread(object sender, EventArgs e)
        {
            while (IsRunning && IsApplicationIdle())
            {
                _commandsReady.Wait(); //Wait for the update thread to finish
                _commandsReady.Reset();
                    
                //Swap command buffers
                //Update commands will be consumed by the render pass
                //And new update commands will be issued for the next render
                SwapBuffers?.Invoke();
                _commandsSwappedForRender.Set();
                    
                DispatchRender();
                _renderDone.Set(); //Signal the update thread that rendering is done
            }
        }
        #endregion

        //This method is commonly used by both loop types
        private void RunUpdateMultiThreadInternal()
        {
            while (IsRunning && !IsSingleThreaded)
            {
                //Updating populates the command buffer while render consumes the previous buffer
                DispatchUpdate();

                //Wait for the previous frame render to complete
                _renderDone.Wait();
                _renderDone.Reset();

                //Signal the render thread that the update is done
                _commandsReady.Set();

                //Wait until the render thread has swapped and is now rendering
                _commandsSwappedForRender.Wait();
                _commandsSwappedForRender.Reset();
            }
        }

        public void Stop()
        {
            IsRunning = false;

            _renderDone?.Set();
            _commandsSwappedForRender?.Set();
            _commandsReady?.Set();

            UpdateTask?.Wait();
            UpdateTask = null;

            RenderTask?.Wait();
            RenderTask = null;

            SingleTask?.Wait();
            SingleTask = null;

            if (AppDomainHelper.IsPrimaryDomain)
            {
                if (_commandsReady != null)
                    Application.Idle -= Application_Idle_MultiThread;
                else
                    Application.Idle -= Application_Idle_SingleThread;
            }

            _watch.Stop();

            _commandsSwappedForRender = null;
            _commandsReady = null;
            _renderDone = null;

            Engine.PrintLine("Game loop ended.");
        }
        private void DispatchRender()
        {
            float timestamp = (float)_watch.Elapsed.TotalSeconds;
            float elapsed = (timestamp - _renderTimestamp).Clamp(0.0f, 1.0f);
            if (elapsed > 0 && elapsed >= TargetRenderPeriod)
                RaiseRenderFrame(elapsed, ref timestamp);
        }
        private void DispatchUpdate()
        {
            int runningSlowlyRetries = 4;
            float timestamp = (float)_watch.Elapsed.TotalSeconds;
            float elapsed = (timestamp - _updateTimestamp).Clamp(0.0f, 1.0f);
            while (elapsed > 0 && elapsed + _updateEpsilon >= TargetUpdatePeriod)
            {
                RaiseUpdateFrame(elapsed, ref timestamp);

                // Calculate difference (positive or negative) between
                // actual elapsed time and target elapsed time. We must
                // compensate for this difference.
                _updateEpsilon += elapsed - TargetUpdatePeriod;

                // Prepare for next loop
                elapsed = (timestamp - _updateTimestamp).Clamp(0.0f, 1.0f);

                if (TargetUpdatePeriod <= float.Epsilon)
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
        }
        private void RaiseUpdateFrame(float elapsed, ref float timestamp)
        {
            // Raise UpdateFrame event
            _updateArgs.Time = elapsed;
            OnUpdateFrameInternal(_updateArgs);

            // Update UpdatePeriod/UpdateFrequency properties
            UpdatePeriod = elapsed;

            // Update UpdateTime property
            _updateTimestamp = timestamp;
            timestamp = (float)_watch.Elapsed.TotalSeconds/* * TimeDilation*/;
            UpdateTime = timestamp - _updateTimestamp;
        }
        void RaiseRenderFrame(float elapsed, ref float timestamp)
        {
            // Raise RenderFrame event
            _renderArgs.Time = elapsed;
            OnRenderFrameInternal(_renderArgs);

            // Update RenderPeriod/UpdateFrequency properties
            RenderPeriod = elapsed;

            // Update RenderTime property
            _renderTimestamp = timestamp;
            timestamp = (float)_watch.Elapsed.TotalSeconds;
            RenderTime = timestamp - _renderTimestamp;
        }

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
                    Debug.Print("Target update frequency set to {0}Hz.", value);
                }
                else
                {
                    _targetUpdatePeriod = 1.0f / MaxFrequency;
                    Debug.Print("Target update frequency clamped to {0}Hz.", MaxFrequency);
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
                    Debug.Print("Target update frequency set to unrestricted.");
                }
                else if (value < 1.0)
                {
                    _targetUpdatePeriod = value;
                    Debug.Print("Target update frequency set to {0}Hz.", TargetUpdateFrequency);
                }
                else
                {
                    _targetUpdatePeriod = 1.0f;
                    Debug.Print("Target update frequency clamped to 1Hz.");
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

                if (IsRunning)
                {
                    if (_commandsReady != null)
                        Application.Idle -= Application_Idle_MultiThread;
                    else
                        Application.Idle -= Application_Idle_SingleThread;
                }

                _isSingleThreaded = value;

                if (IsRunning)
                {
                    if (_isSingleThreaded)
                        Application.Idle += Application_Idle_SingleThread;
                    else
                    {
                        MakeManualResetEvents();

                        Task.Factory.StartNew(RunUpdateMultiThreadInternal, TaskCreationOptions.LongRunning);

                        Application.Idle += Application_Idle_MultiThread;
                    }
                }
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
