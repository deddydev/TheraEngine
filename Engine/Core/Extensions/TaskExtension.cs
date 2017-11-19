using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TaskExtension
    {
        public static async Task<TBase> Generalized<TBase, TDerived>(this Task<TDerived> task) where TDerived : TBase => await task;
    }
}
