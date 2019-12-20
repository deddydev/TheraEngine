using Extensions;
using System;
using System.Diagnostics;
using TheraEngine.Core;
using TheraEngine.Core.Reflection;

namespace TheraEngine
{
    public interface IObjectSlim : ISponsorableMarshalByRefObject
    {
        TypeProxy GetTypeProxy();
    }
    /// <summary>
    /// Use this class for objects that just need to be marshalled between domains and need no extra functionality.
    /// </summary>
    public abstract class TObjectSlim : SponsorableMarshalByRefObject, IObjectSlim
    {
        TypeProxy IObjectSlim.GetTypeProxy() => GetType();

        public bool ExistsInOtherDomain(AppDomain thisDomain)
            => thisDomain != Domain;

        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.PrintLine().
        /// </summary>
        protected static void PrintLine(string message, params object[] args) => Debug.Print(message, args);
        /// <summary>
        /// Prints a line to output.
        /// Identical to Engine.PrintLine().
        /// </summary>
        protected static void PrintLine(string message) => Debug.Print(message);

        public override string ToString() => GetType().GetFriendlyName();
    }
}
