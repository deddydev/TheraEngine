using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace System
{
    public class GlobalTimer
    {
        const double MaxFrequency = 500.0; // Frequency cap for Update/RenderFrame events

        double update_period, render_period;
        double target_update_period, target_render_period;

        double update_time; // length of last UpdateFrame event
        double render_time; // length of last RenderFrame event

        double update_timestamp; // timestamp of last UpdateFrame event
        double render_timestamp; // timestamp of last RenderFrame event

        double update_epsilon; // quantization error for UpdateFrame events

        bool is_running_slowly; // true, when UpdatePeriod cannot reach TargetUpdatePeriod

        FrameEventArgs update_args = new FrameEventArgs();
        FrameEventArgs render_args = new FrameEventArgs();

        readonly Stopwatch watch = new Stopwatch();

        public event EventHandler<FrameEventArgs> RenderFrame = delegate { };
        public event EventHandler<FrameEventArgs> UpdateFrame = delegate { };

        /// <summary>
        /// Runs the timer until Stop() is called.
        /// Do note that the function that calls this will be suspended until the timer is stopped.
        /// Code located after where you call this will then be executed after.
        /// </summary>
        /// <param name="updatesPerSec">FPS of update events.</param>
        /// <param name="framesPerSec">FPS of render events.</param>
        public void Run(double updates_per_second, double frames_per_second)
        {
            _running = true;

            if (updates_per_second < 0.0 || updates_per_second > 200.0)
                throw new ArgumentOutOfRangeException("updates_per_second", updates_per_second,
                                                        "Parameter should be inside the range [0.0, 200.0]");
            if (frames_per_second < 0.0 || frames_per_second > 200.0)
                throw new ArgumentOutOfRangeException("frames_per_second", frames_per_second,
                                                        "Parameter should be inside the range [0.0, 200.0]");

            if (updates_per_second != 0)
                TargetUpdateFrequency = updates_per_second;
            if (frames_per_second != 0)
                TargetRenderFrequency = frames_per_second;
                
            watch.Start();
            while (_running)
            {
                ProcessEvents();
                DispatchUpdateAndRenderFrame(this, EventArgs.Empty);
            }
            
        }
        void DispatchUpdateAndRenderFrame(object sender, EventArgs e)
        {
            int is_running_slowly_retries = 4;
            double timestamp = watch.Elapsed.TotalSeconds;
            double elapsed = 0;

            elapsed = (timestamp - update_timestamp).Clamp(0.0f, 1.0f);
            while (elapsed > 0 && elapsed + update_epsilon >= TargetUpdatePeriod)
            {
                RaiseUpdateFrame(elapsed, ref timestamp);

                // Calculate difference (positive or negative) between
                // actual elapsed time and target elapsed time. We must
                // compensate for this difference.
                update_epsilon += elapsed - TargetUpdatePeriod;

                // Prepare for next loop
                elapsed = (timestamp - update_timestamp).Clamp(0.0f, 1.0f);

                if (TargetUpdatePeriod <= Double.Epsilon)
                {
                    // According to the TargetUpdatePeriod documentation,
                    // a TargetUpdatePeriod of zero means we will raise
                    // UpdateFrame events as fast as possible (one event
                    // per ProcessEvents() call)
                    break;
                }

                is_running_slowly = update_epsilon >= TargetUpdatePeriod;
                if (is_running_slowly && --is_running_slowly_retries == 0)
                {
                    // If UpdateFrame consistently takes longer than TargetUpdateFrame
                    // stop raising events to avoid hanging inside the UpdateFrame loop.
                    break;
                }
            }

            elapsed = (timestamp - render_timestamp).Clamp(0.0f, 1.0f);
            if (elapsed > 0 && elapsed >= TargetRenderPeriod)
            {
                RaiseRenderFrame(elapsed, ref timestamp);
            }
        }
        void RaiseUpdateFrame(double elapsed, ref double timestamp)
        {
            // Raise UpdateFrame event
            update_args.Time = elapsed;
            OnUpdateFrameInternal(update_args);

            // Update UpdatePeriod/UpdateFrequency properties
            update_period = elapsed;

            // Update UpdateTime property
            update_timestamp = timestamp;
            timestamp = watch.Elapsed.TotalSeconds;
            update_time = timestamp - update_timestamp;
        }
        void RaiseRenderFrame(double elapsed, ref double timestamp)
        {
            // Raise RenderFrame event
            render_args.Time = elapsed;
            OnRenderFrameInternal(render_args);

            // Update RenderPeriod/UpdateFrequency properties
            render_period = elapsed;

            // Update RenderTime property
            render_timestamp = timestamp;
            timestamp = watch.Elapsed.TotalSeconds;
            render_time = timestamp - render_timestamp;
        }

        void OnRenderFrameInternal(FrameEventArgs e) { if (_running) OnRenderFrame(e); }
        void OnUpdateFrameInternal(FrameEventArgs e) { if (_running) OnUpdateFrame(e); }
        void OnRenderFrame(FrameEventArgs e) { RenderFrame?.Invoke(this, e); }
        void OnUpdateFrame(FrameEventArgs e) { UpdateFrame?.Invoke(this, e); }

        bool _running = false;
        public bool IsRunning { get { return _running; } }
        public void Stop() { _running = false; }

        void ProcessEvents()
        {
            Application.DoEvents();
            Thread.Sleep(0);
        }

        #region --- GameWindow Timing ---

        #region RenderFrequency

        /// <summary>
        /// Gets a double representing the actual frequency of RenderFrame events, in hertz (i.e. fps or frames per second).
        /// </summary>
        public double RenderFrequency
        {
            get
            {
                if (render_period == 0.0)
                    return 1.0;
                return 1.0 / render_period;
            }
        }

        #endregion

        #region RenderPeriod

        /// <summary>
        /// Gets a double representing the period of RenderFrame events, in seconds.
        /// </summary>
        public double RenderPeriod
        {
            get
            {
                return render_period;
            }
        }

        #endregion

        #region RenderTime

        /// <summary>
        /// Gets a double representing the time spent in the RenderFrame function, in seconds.
        /// </summary>
        public double RenderTime
        {
            get
            {
                return render_time;
            }
            protected set
            {
                render_time = value;
            }
        }

        #endregion

        #region TargetRenderFrequency

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
                if (TargetRenderPeriod == 0.0)
                    return 0.0;
                return 1.0 / TargetRenderPeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    TargetRenderPeriod = 0.0;
                }
                else if (value <= MaxFrequency)
                {
                    TargetRenderPeriod = 1.0 / value;
                }
                else Debug.Print("Target render frequency clamped to {0}Hz.", MaxFrequency);
            }
        }

        #endregion

        #region TargetRenderPeriod

        /// <summary>
        /// Gets or sets a double representing the target render period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that RenderFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public double TargetRenderPeriod
        {
            get
            {
                return target_render_period;
            }
            set
            {
                if (value <= 1 / MaxFrequency)
                {
                    target_render_period = 0.0;
                }
                else if (value <= 1.0)
                {
                    target_render_period = value;
                }
                else Debug.Print("Target render period clamped to 1.0 seconds.");
            }
        }

        #endregion

        #region TargetUpdateFrequency

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
                if (TargetUpdatePeriod == 0.0)
                    return 0.0;
                return 1.0 / TargetUpdatePeriod;
            }
            set
            {
                if (value < 1.0)
                {
                    TargetUpdatePeriod = 0.0;
                }
                else if (value <= MaxFrequency)
                {
                    TargetUpdatePeriod = 1.0 / value;
                }
                else Debug.Print("Target render frequency clamped to {0}Hz.", MaxFrequency);
            }
        }

        #endregion

        #region TargetUpdatePeriod

        /// <summary>
        /// Gets or sets a double representing the target update period, in seconds.
        /// </summary>
        /// <remarks>
        /// <para>A value of 0.0 indicates that UpdateFrame events are generated at the maximum possible frequency (i.e. only limited by the hardware's capabilities).</para>
        /// <para>Values lower than 0.002 seconds (500Hz) are clamped to 0.0. Values higher than 1.0 seconds (1Hz) are clamped to 1.0.</para>
        /// </remarks>
        public double TargetUpdatePeriod
        {
            get
            {
                return target_update_period;
            }
            set
            {
                if (value <= 1 / MaxFrequency)
                {
                    target_update_period = 0.0;
                }
                else if (value <= 1.0)
                {
                    target_update_period = value;
                }
                else Debug.Print("Target update period clamped to 1.0 seconds.");
            }
        }

        #endregion

        #region UpdateFrequency

        /// <summary>
        /// Gets a double representing the frequency of UpdateFrame events, in hertz.
        /// </summary>
        public double UpdateFrequency
        {
            get
            {
                if (update_period == 0.0)
                    return 1.0;
                return 1.0 / update_period;
            }
        }

        #endregion

        #region UpdatePeriod

        /// <summary>
        /// Gets a double representing the period of UpdateFrame events, in seconds.
        /// </summary>
        public double UpdatePeriod
        {
            get
            {
                return update_period;
            }
        }

        #endregion

        #region UpdateTime

        /// <summary>
        /// Gets a double representing the time spent in the UpdateFrame function, in seconds.
        /// </summary>
        public double UpdateTime
        {
            get
            {
                return update_time;
            }
        }

        #endregion

        #endregion
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
