namespace AppDomainToolkit
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// This is a thin wrapper around the .NET AppDomain class that enables safe disposal. Use
    /// these objects where you would normally grab an AppDomain object. Note that if the 
    /// current application domain is passed to this class, a call to Dispose will do nothing. We will
    /// never unload the current application domain.
    /// </summary>
    internal sealed class DisposableAppDomain : AppDomainToolkit.IDisposable
    {
        #region Fields & Constants

        private AppDomain domain;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the DisposableAppDomain class.
        /// </summary>
        /// <param name="domain">
        /// The domain to wrap.
        /// </param>
        public DisposableAppDomain(AppDomain domain)
        {
            if (domain is null)
            {
                throw new ArgumentNullException("domain");
            }

            this.domain = domain;
            this.IsDisposed = false;
        }

        /// <summary>
        /// Finalizes an instance of the DisposableAppDomain class.
        /// </summary>
        ~DisposableAppDomain()
        {
            this.OnDispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the wrapped application domain.
        /// </summary>
        public AppDomain Domain
        { 
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException("The AppDomain has been unloaded or disposed!");
                }

                return this.domain;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            this.OnDispose(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Should be called when the object is being disposed.
        /// </summary>
        /// <param name="disposing">
        /// Was Dispose() called or did we get here from the finalizer?
        /// </param>
        private void OnDispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.IsDisposed)
                {
                    // Do *not* unload the current app domain.
                    if (!(this.Domain is null || this.Domain.Equals(AppDomain.CurrentDomain)))
                    {
                        bool success = false;
                        while (!success)
                        {
                            try
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                AppDomain.Unload(Domain);
                                success = true;
                            }
                            catch (CannotUnloadAppDomainException)
                            {
                                Trace.WriteLine("Couldn't unload AppDomain. Retrying in 1 sec");
                                Thread.Sleep(1);
                            }
                        }
                    }

                    this.domain = null;
                }
            }

            this.IsDisposed = true;
        }

        #endregion
    }
}
