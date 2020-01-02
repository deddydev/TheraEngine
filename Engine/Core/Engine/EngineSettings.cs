using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEngine.Core.Files;

namespace TheraEngine
{
    /// <summary>
    /// Base class for a class that only defines primitive properties; no classes or structs.
    /// Allows reading and writing all defined properties
    /// </summary>
    [TFileExt("set", "txt")]
    public abstract class TSettings : TFileObject
    {
        //public override void ManualRead3rdParty(string filePath)
        //{
        //    //string ext = Path.GetExtension(filePath).ToLowerInvariant().Substring(1);
        //    //if (ext == "txt")
        //    //{
        //    //    List<VarInfo> props = SerializationCommon.CollectSerializedMembers(GetType());
        //    //    string[] lines = File.ReadAllLines(filePath);
        //    //    string modLine;
        //    //    foreach (string line in lines)
        //    //    {
        //    //        modLine = line.Trim();
        //    //        if (modLine.StartsWith("[") && modLine.EndsWith("]"))
        //    //        {
        //    //            //Category; ignore
        //    //        }
        //    //        else
        //    //        {
        //    //            string[] parts = modLine.Split('=');
        //    //            if (parts.Length == 2)
        //    //            {
        //    //                string name = parts[0].TrimEnd();
        //    //                string value = parts[1].TrimStart();
        //    //                int propIndex = props.FindIndex(x => string.Equals(name, x.Name));
        //    //                if (props.IndexInRange(propIndex))
        //    //                {
        //    //                    VarInfo prop = props[propIndex];
        //    //                    if (SerializationCommon.CanParseAsString(prop.VariableType))
        //    //                    {
        //    //                        prop.SetValue(this, SerializationCommon.ParseString(value, prop.VariableType));
        //    //                        props.RemoveAt(propIndex);
        //    //                    }
        //    //                    else
        //    //                        Engine.LogWarning("Problem reading line: " + line);
        //    //                }
        //    //                else
        //    //                    Engine.LogWarning("Problem reading line: " + line);
        //    //            }
        //    //            else
        //    //                Engine.LogWarning("Problem reading line: " + line);
        //    //        }
        //    //    }
        //    //}
        //}
        //public override void ManualWrite3rdParty(string filePath)
        //{
        //    //string ext = Path.GetExtension(filePath).ToLowerInvariant().Substring(1);
        //    //if (ext == "txt")
        //    //{
        //    //    PropertyInfo[] properties = GetType().
        //    //      GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).
        //    //      Where(x => x.GetCustomAttributes<TSerialize>() != null).
        //    //      OrderBy(x => x.GetCustomAttribute<TSerialize>().Order).
        //    //      ToArray();
        //    //}
        //}
    }
    [Serializable]
    [TFileDef("Engine Settings")]
    public class EngineSettings : TSettings
    {
        private bool _singleThreaded = false;
        private bool _capUPS = false;
        private bool _capFPS = false;
        private float _targetFPS;
        private float _targetUPS;
        private bool _skinOnGPU;
        private bool _useIntegerWeightingIds;
        private bool _allowShaderPipelines;
        private bool _enableDeferredPass;
        private EOutputVerbosity _outputVerbosity;
        private double _allowedOutputRecencySeconds;
        private string texturesFolder;
        private bool _printAppDomainInOutput;
        private string fontsFolder;
        private string engineDataFolder;
        private string modelsFolder;
        private string shadersFolder;
        private string worldsFolder;
        private string scriptsFolder;
        private float doubleClickInputDelay;
        private float holdInputDelay;

        [Category("Performance")]
        [TSerialize]
        public bool SkinOnGPU
        {
            get => _skinOnGPU;
            set => Set(ref _skinOnGPU, value);
        }
        [Category("Performance")]
        [TSerialize]
        public bool UseIntegerWeightingIds
        {
            get => _useIntegerWeightingIds;
            set => Set(ref _useIntegerWeightingIds, value);
        }
        [Category("Performance")]
        [TSerialize]
        public bool AllowShaderPipelines
        {
            get => _allowShaderPipelines;
            set => Set(ref _allowShaderPipelines, value);
        }
        [Category("Performance")]
        [TSerialize]
        public bool EnableDeferredPass
        {
            get => _enableDeferredPass;
            set => Set(ref _enableDeferredPass, value);
        }
        [Category("Performance")]
        [TSerialize]
        public bool SingleThreaded
        {
            get => _singleThreaded;
            set => Set(ref _singleThreaded, value);
        }

        /// <summary>
        /// Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).
        /// </summary>
        [Description("Determines if the render rate should be capped at a specific frequency. If not, will run as fast as possible (though there is no point going any faster than the monitor can update).")]
        [Category("Frames Per Second")]
        [DisplayName("Capped")]
        [TSerialize("Capped", OverrideCategory = "FramesPerSecond", UseCategory = true, NodeType = ENodeType.Attribute)]
        public bool CapFPS
        {
            get => _capFPS;
            set => Set(ref _capFPS, value);
        }
        /// <summary>
        /// How many frames are expected to be rendered per second.
        /// </summary>
        [Description("How many frames are expected to be rendered per second.")]
        [Category("Frames Per Second")]
        [DisplayName("Target")]
        [TSerialize("Target", OverrideCategory = "FramesPerSecond", UseCategory = true, NodeType = ENodeType.Attribute, Condition = "CapFPS")]
        public float TargetFPS
        {
            get => _targetFPS;
            set => Set(ref _targetFPS, value);
        }

        /// <summary>
        /// Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.
        /// </summary>
        [Description("Determines if the update rate should be capped at a specific frequency. If not, will run as fast as possible.")]
        [Category("Updates Per Second")]
        [DisplayName("Capped")]
        [TSerialize("Capped", OverrideCategory = "UpdatesPerSecond", UseCategory = true, NodeType = ENodeType.Attribute)]
        public bool CapUPS
        {
            get => _capUPS;
            set => Set(ref _capUPS, value);
        }
        /// <summary>
        /// How many internal engine tick update calls are expected to be made per second. This is not the same as the render frequency.
        /// </summary>
        [Description("How many internal engine tick update calls are made per second. This is not the same as the render frequency.")]
        [Category("Updates Per Second")]
        [DisplayName("Target")]
        [TSerialize("Target", OverrideCategory = "UpdatesPerSecond", UseCategory = true, NodeType = ENodeType.Attribute, Condition = "CapUPS")]
        public float TargetUPS
        {
            get => _targetUPS;
            set => Set(ref _targetUPS, value);
        }

        /// <summary>
        /// How many seconds the user has to hold a button for it to register as a hold event.
        /// </summary>
        [Description("How many seconds the user has to hold a button for it to register as a hold event.")]
        [Category("Input")]
        [TSerialize]
        public float HoldInputDelay
        {
            get => holdInputDelay;
            set => Set(ref holdInputDelay, value);
        }
        /// <summary>
        /// How many seconds the user has between pressing the same button twice for it to register as a double click event.
        /// </summary>
        [Description("How many seconds the user has between pressing the same button twice for it to register as a double click event.")]
        [Category("Input")]
        [TSerialize]
        public float DoubleClickInputDelay
        {
            get => doubleClickInputDelay;
            set => Set(ref doubleClickInputDelay, value);
        }

        /// <summary>
        /// The path to the folder containing premade engine shaders.
        /// </summary>
        [Description("The path to the folder containing premade engine scripts.")]
        [Category("Paths")]
        [TSerialize]
        public string ScriptsFolder
        {
            get => scriptsFolder;
            set => Set(ref scriptsFolder, value);
        }

        /// <summary>
        /// The path to the folder containing premade engine worlds.
        /// </summary>
        [Description("The path to the folder containing premade engine worlds.")]
        [Category("Paths")]
        [TSerialize]
        public string WorldsFolder
        {
            get => worldsFolder;
            set => Set(ref worldsFolder, value);
        }

        /// <summary>
        /// The path to the folder containing premade engine shaders.
        /// </summary>
        [Description("The path to the folder containing premade engine shaders.")]
        [Category("Paths")]
        [TSerialize]
        public string ShadersFolder
        {
            get => shadersFolder;
            set => Set(ref shadersFolder, value);
        }

        /// <summary>
        /// The path to the folder containing premade engine models.
        /// </summary>
        [Description("The path to the folder containing premade engine models.")]
        [Category("Paths")]
        [TSerialize]
        public string ModelsFolder
        {
            get => modelsFolder;
            set => Set(ref modelsFolder, value);
        }

        /// <summary>
        /// The path to the folder containing default engine files.
        /// </summary>
        [Description("The path to the folder containing default engine files.")]
        [Category("Paths")]
        [TSerialize]
        public string EngineDataFolder
        {
            get => engineDataFolder;
            set => Set(ref engineDataFolder, value);
        }

        /// <summary>
        /// The path to the folder containing custom engine fonts.
        /// </summary>
        [Description("The path to the folder containing custom engine fonts.")]
        [Category("Paths")]
        [TSerialize]
        public string FontsFolder
        {
            get => fontsFolder;
            set => Set(ref fontsFolder, value);
        }

        /// <summary>
        /// The path to the folder containing premade engine textures.
        /// </summary>
        [Description("The path to the folder containing premade engine textures.")]
        [Category("Paths")]
        [TSerialize]
        public string TexturesFolder
        {
            get => texturesFolder;
            set => Set(ref texturesFolder, value);
        }

        /// <summary>
        /// If true, prepends the currently executing AppDomain to console output.
        /// </summary>
        [Description("If true, prepends the currently executing AppDomain to console output.")]
        [Category("Debug")]
        [TSerialize]
        public bool PrintAppDomainInOutput
        {
            get => _printAppDomainInOutput;
            set => Set(ref _printAppDomainInOutput, value);
        }
        /// <summary>
        /// How often the same message is allowed to be printed to output. Reduces output spam.
        /// </summary>
        [Description("How often the same message is allowed to be printed to output. Reduces output spam.")]
        [Category("Debug")]
        [TSerialize]
        public double AllowedOutputRecencySeconds
        {
            get => _allowedOutputRecencySeconds;
            set => Set(ref _allowedOutputRecencySeconds, value);
        }
        /// <summary>
        /// How specific the debug output will be.
        /// </summary>
        [Description("How specific the debug output will be.")]
        [Category("Debug")]
        [TSerialize]
        public EOutputVerbosity OutputVerbosity
        {
            get => _outputVerbosity;
            set => Set(ref _outputVerbosity, value);
        }

        public EngineSettings()
        {
            EnableDeferredPass = true;
            SkinOnGPU = true;
            UseIntegerWeightingIds = true;
            AllowShaderPipelines = true;
            PrintAppDomainInOutput = true;
            OutputVerbosity = EOutputVerbosity.Normal;
            AllowedOutputRecencySeconds = 2.0;

            CapFPS = false;
            TargetFPS = 60.0f;
            CapUPS = false;
            TargetUPS = 30.0f;

            string startupPath = Application.StartupPath;
            if (Engine.DesignMode)
            {
                //var dte = (EnvDTE80.DTE2)GetService(typeof(EnvDTE.DTE));
                //if (dte != null)
                //{
                //    var solution = dte.Solution;
                //    if (solution != null)
                //    {
                //        string baseDir = Path.GetDirectoryName(solution.FullName);
                //        MessageBox.Show(baseDir);
                //    }
                //}
                //string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                //startupPath = Path.GetDirectoryName(filePath);
                //if (string.Equals(Path.GetDirectoryName(startupPath), "IDE", System.StringComparison.InvariantCultureIgnoreCase))
                EngineDataFolder = string.Format("C:{0}Users{0}dnedd{0}source{0}repos{0}TheraEngine{0}Build{0}Engine{0}", Path.DirectorySeparatorChar);
            }
            else
            {
#if DEBUG
                EngineDataFolder = startupPath + string.Format("{0}..{0}..{0}..{0}Engine{0}", Path.DirectorySeparatorChar);
#else
                EngineDataFolder = startupPath + string.Format("{0}Engine{0}", Path.DirectorySeparatorChar);
#endif
            }

            ShadersFolder = EngineDataFolder + "Shaders" + Path.DirectorySeparatorChar;
            TexturesFolder = EngineDataFolder + "Textures" + Path.DirectorySeparatorChar;
            ScriptsFolder = EngineDataFolder + "Scripts" + Path.DirectorySeparatorChar;
            WorldsFolder = EngineDataFolder + "Worlds" + Path.DirectorySeparatorChar;
            FontsFolder = EngineDataFolder + "Fonts" + Path.DirectorySeparatorChar;
        }
    }
}
