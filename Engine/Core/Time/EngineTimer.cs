﻿using System.Diagnostics;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Win32.Native;
using TheraEngine.Rendering;

namespace TheraEngine.Timers
{
    public class EngineTimer
    {
        const float MaxFrequency = 500.0f; // Frequency cap for Update/RenderFrame events
        float _targetUpdatePeriod, _targetRenderPeriod;
        float _updateTimestamp; // timestamp of last UpdateFrame event
        float _renderTimestamp; // timestamp of last RenderFrame event

        float _updateEpsilon = 0.0f; // quantization error for UpdateFrame events

        bool _isRunningSlowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        FrameEventArgs _updateArgs = new FrameEventArgs();
        FrameEventArgs _renderArgs = new FrameEventArgs();

        readonly Stopwatch _watch = new Stopwatch();

        ManualResetEvent _commandsReady;
        ManualResetEvent _commandsSwappedForRender;
        ManualResetEvent _renderDone;

        public event EventHandler<FrameEventArgs> RenderFrame = delegate { };
        public event EventHandler<FrameEventArgs> UpdateFrame = delegate { };
        
        /// <summary>
        /// Runs the timer until Stop() is called.
        /// </summary>
        public void Run(bool singleThreaded = false)
        {
            if (IsRunning)
                return;

            Engine.PrintLine("Started game loop.");
            IsRunning = true;
            _watch.Start();
            if (singleThreaded)
                Application.Idle += Application_Idle_SingleThread;
            else
            {
                _commandsReady = new ManualResetEvent(false);
                _commandsSwappedForRender = new ManualResetEvent(false);
                _renderDone = new ManualResetEvent(true);
                
                Task.Factory.StartNew(RunUpdateMultiThreadInternal, TaskCreationOptions.LongRunning);

                Application.Idle += Application_Idle_MultiThread;
            }
        }

        private bool IsApplicationIdle() => NativeMethods.PeekMessage(out NativeMessage result, IntPtr.Zero, 0, 0, 0) == 0;

        private RenderQuery _renderTimeQuery = new RenderQuery();
        private void Application_Idle_SingleThread(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                //_renderTimeQuery.BeginQuery(EQueryTarget.TimeElapsed);
                {
                    DispatchUpdate();
                    SwapBuffers?.Invoke();
                    DispatchRender();
                }
                //_renderTimeQuery.EndQuery(EQueryTarget.TimeElapsed);
                //double time = _renderTimeQuery.GetQueryObjectLong(EGetQueryObject.QueryResult) * 1e-6;
                //Engine.PrintLine(time.ToString("0.00 ms"));
            }
        }
        private void Application_Idle_MultiThread(object sender, EventArgs e)
        {
            while (IsRunning && IsApplicationIdle())
            {
                //_renderTimeQuery.BeginQuery(EQueryTarget.TimeElapsed);
                {
                    _commandsReady.WaitOne(); //Wait for the update thread to finish
                    _commandsReady.Reset();
                    
                    //Swap command buffers
                    //Update commands will be consumed by the render pass
                    //And new update commands will be issued for the next render
                    SwapBuffers?.Invoke();
                    _commandsSwappedForRender.Set();
                    
                    DispatchRender();
                    _renderDone.Set(); //Signal the update thread that rendering is done
                }
                //_renderTimeQuery.EndQuery(EQueryTarget.TimeElapsed);
                //double time = _renderTimeQuery.GetQueryObjectLong(EGetQueryObject.QueryResult) * 1e-6;
                //Engine.PrintLine(time.ToString("0.00 ms"));
            }
        }
        private void RunUpdateMultiThreadInternal()
        {
            while (IsRunning)
            {
                //Updating populates the command buffer while render consumes the previous buffer
                DispatchUpdate();

                //Wait for the previous frame render to complete
                _renderDone.WaitOne();
                _renderDone.Reset();

                //Signal the render thread that the update is done
                _commandsReady.Set();

                //Wait until the render thread has swapped and is now rendering
                _commandsSwappedForRender.WaitOne();
                _commandsSwappedForRender.Reset();
            }

            _commandsSwappedForRender = null;
            _commandsReady = null;
            _renderDone = null;
        }
        public void Stop()
        {
            IsRunning = false;

            if (_commandsReady != null)
                Application.Idle -= Application_Idle_MultiThread;
            else
                Application.Idle -= Application_Idle_SingleThread;
            
            _watch.Stop();
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

                if (TargetUpdatePeriod <= Single.Epsilon)
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
            timestamp = (float)_watch.Elapsed.TotalSeconds * TimeDilation;
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

        private void OnRenderFrameInternal(FrameEventArgs e) { OnRenderFrame(e); }
        private void OnUpdateFrameInternal(FrameEventArgs e) { OnUpdateFrame(e); }
        private void OnRenderFrame(FrameEventArgs e) => RenderFrame?.Invoke(this, e);
        private void OnUpdateFrame(FrameEventArgs e) => UpdateFrame?.Invoke(this, e);
        public bool IsRunning { get; private set; } = false;

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
        /// Gets a float representing the period of RenderFrame events, in seconds.
        /// </summary>
        public float RenderPeriod { get; private set; }

        /// <summary>
        /// Gets a float representing the time spent in the RenderFrame function, in seconds.
        /// </summary>
        public float RenderTime { get; private set; }

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
                    Debug.Print("Target update frequency set to unrestricted.");
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

        /// <summary>
        /// Gets a float representing the period of UpdateFrame events, in seconds (seconds per update).
        /// </summary>
        public float UpdatePeriod { get; private set; }

        /// <summary>
        /// Gets a float representing the time spent in the UpdateFrame function, in seconds.
        /// </summary>
        public float UpdateTime { get; private set; }
        public float TimeDilation { get; set; } = 1.0f;

        public event Action SwapBuffers;
    }

    public class FrameEventArgs : EventArgs
    {
        float elapsed;

        public FrameEventArgs() { }

        /// <param name="elapsed">The amount of time that has elapsed since the previous event, in seconds.</param>
        public FrameEventArgs(float elapsed)
        {
            Time = elapsed;
        }

        /// <summary>
        /// Gets a <see cref="System.Single"/> that indicates how many seconds of time elapsed since the previous event.
        /// </summary>
        public float Time
        {
            get { return elapsed; }
            set { elapsed = value; }
        }
    }
}
