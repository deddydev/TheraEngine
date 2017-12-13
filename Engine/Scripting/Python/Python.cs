using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;

namespace TheraEngine.Scripting
{
    public static class PythonRuntime
    {
        public static void Initialize()
        {
            var ipy = Python.CreateRuntime();
            dynamic test = ipy.UseFile(Path.Combine(Engine.Settings.ScriptsFolder, "Test.py"));
            test.Test();
        }
    }
}
