using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Files;
using Extensions;
using System.Linq;
using TheraEngine.Core.Files.XML;
using System.Threading.Tasks;

namespace TheraEngine.ThirdParty
{
    [TFile3rdPartyExt("sln")]
    public class VisualStudioSolution : TFileObject
    {
        public Guid SolutionGuid { get; set; }
        public string VisualStudioVersion { get; set; }
        public string MinimumVisualStudioVersion { get; set; }
        public string[] Platforms { get; set; }
        public string[] Configurations { get; set; }
        public Project[] Projects { get; set; }
        public bool HideSolutionNode { get; set; }
        
        public class Project : TObjectSlim
        {
            public Guid TypeGuid { get; set; }
            public Guid Guid { get; set; }
            public string Name { get; set; }
            public string RelativePath { get; set; }
            public Guid[] Dependencies { get; set; }
            public Dictionary<string, (string config, string platform)> ActiveCfg { get; set; } = new Dictionary<string, (string config, string platform)>();
            public Dictionary<string, (string config, string platform)> Build0 { get; set; } = new Dictionary<string, (string config, string platform)>();

            public string GetLine()
                => $"Project(\"{TypeGuid.ToString("B")}\") = \"{Name}\", \"{RelativePath}\", \"{Guid.ToString("B")}\"";

            public void GetConfigPlatform(
                string globalConfiguration, string globalPlatform,
                out string localConfiguration, out string localPlatform)
            {
                string query = $"{globalConfiguration}|{globalPlatform}";
                if (ActiveCfg.ContainsKey(query))
                {
                    (localConfiguration, localPlatform) = ActiveCfg[query];
                }
                else
                {
                    localConfiguration = globalConfiguration;
                    localPlatform = globalPlatform;
                }
            }
        }

        public string PathForProject(Project project)
            => Path.Combine(DirectoryPath, project.RelativePath);

        public async Task<MSBuild.Project> ReadProjectAsync(Project project)
            => await XMLSchemaDefinition<MSBuild.Project>.ImportAsync(PathForProject(project), 0);
        
        public override void ManualRead3rdParty(string filePath)
        {
            List<Project> projects = new List<Project>();
            HashSet<string> configs = new HashSet<string>();
            HashSet<string> platforms = new HashSet<string>();

            string[] lines = File.ReadAllLines(filePath);
            int i = -1;
            while (++i < lines.Length)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("#"))
                    continue;

                string[] split = line.Split('=');
                switch (split.Length)
                {
                    default:
                    case 0:
                        throw new InvalidOperationException();
                    case 1:
                        string str = split[0];
                        if (str.StartsWith("Microsoft Visual Studio Solution File"))
                            continue;
                        else if (str.StartsWith("Global"))
                        {
                            while ((line = lines[++i].Trim()).StartsWith("GlobalSection"))
                            {
                                int startBracket = line.IndexOf('(') + 1;
                                int endBracket = line.IndexOf(')');
                                int len = endBracket - startBracket;
                                string sectionType = line.Substring(startBracket, len);

                                while (!(line = lines[++i].Trim()).EqualsInvariant("EndGlobalSection"))
                                {
                                    switch (sectionType)
                                    {
                                        case "SolutionConfigurationPlatforms": //preSolution
                                            {
                                                int lineIndex = line.IndexOf('|');

                                                string config = line.Substring(0, lineIndex).Trim();
                                                configs.Add(config);

                                                string plat = line.Substring(lineIndex + 1, line.IndexOf('=') - lineIndex - 1).Trim();
                                                platforms.Add(plat);
                                            }
                                            break;
                                        case "ProjectConfigurationPlatforms": //postSolution
                                            {
                                                int guidEndIndex = line.IndexOf('.');
                                                var guid = Guid.Parse(line.Substring(0, guidEndIndex));
                                                Project proj = projects.FirstOrDefault(x => x.Guid == guid);
                                                line = line.Substring(guidEndIndex + 1);

                                                int globalCfgEndIndex = line.IndexOf('.');
                                                string globalCfg = line.Substring(0, globalCfgEndIndex);
                                                //string[] globalCfgParts = globalCfg.Split('|');
                                                line = line.Substring(globalCfgEndIndex + 1);

                                                int cfgTypeIndex = line.IndexOf('=');
                                                string cfgType = line.Substring(0, cfgTypeIndex).Trim();

                                                string localCfg = line.Substring(cfgTypeIndex + 1).Trim();
                                                string[] localCfgParts = localCfg.Split('|');
                                                var localCfgTuple = (localCfgParts[0], localCfgParts[1]);

                                                switch (cfgType)
                                                {
                                                    case "ActiveCfg":
                                                        proj.ActiveCfg.Add(globalCfg, localCfgTuple);
                                                        break;
                                                    case "Build.0":
                                                        proj.Build0.Add(globalCfg, localCfgTuple);
                                                        break;
                                                }
                                            }
                                            break;
                                        case "SolutionProperties": //preSolution
                                            {
                                                if (line.StartsWith("HideSolutionNode", StringComparison.InvariantCulture))
                                                {
                                                    int startIndex = line.IndexOf('=') + 1;
                                                    string valueStr = line.Substring(startIndex).Trim();
                                                    HideSolutionNode = bool.Parse(valueStr);
                                                }
                                            }
                                            break;
                                        case "ExtensibilityGlobals": //postSolution
                                            {
                                                if (line.StartsWith("SolutionGuid", StringComparison.InvariantCulture))
                                                {
                                                    int startIndex = line.IndexOf('=') + 1;
                                                    string guidStr = line.Substring(startIndex).Trim();
                                                    Guid guid = new Guid(guidStr);
                                                    SolutionGuid = guid;
                                                }
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        string p1 = split[0].Trim();
                        string p2 = split[1].Trim();
                        if (p1.EqualsInvariant("VisualStudioVersion"))
                        {
                            VisualStudioVersion = p2;
                        }
                        else if (p1.EqualsInvariant("MinimumVisualStudioVersion"))
                        {
                            MinimumVisualStudioVersion = p2;
                        }
                        else if (p1.StartsWith("Project"))
                        {
                            Project project = new Project();

                            int startBracket = p1.IndexOf('{') + 1;
                            int endBracket = p1.IndexOf('}');
                            int len = endBracket - startBracket;
                            string typeGuidStr = p1.Substring(startBracket, len);
                            Guid typeGuid = new Guid(typeGuidStr);
                            project.TypeGuid = typeGuid;

                            string[] parts = p2.Split(',');
                            if (parts.Length == 3)
                            {
                                string nameStr = parts[0].Trim();
                                project.Name = nameStr.Substring(1, nameStr.Length - 2);

                                string pathStr = parts[1].Trim();
                                project.RelativePath = pathStr.Substring(1, pathStr.Length - 2);

                                string guidStr = parts[2].Trim();
                                project.Guid = new Guid(guidStr.Substring(1, guidStr.Length - 2));
                            }

                            while (!(line = lines[++i].Trim()).EqualsInvariant("EndProject"))
                            {
                                if (line.StartsWith("ProjectSection(ProjectDependencies)"))
                                {
                                    List<Guid> dependencies = new List<Guid>();
                                    while (!(line = lines[++i].Trim()).EqualsInvariant("EndProjectSection"))
                                    {
                                        string guidStr = line.Substring(0, line.IndexOf('=')).Trim();
                                        Guid guid = new Guid(guidStr);
                                        dependencies.Add(guid);
                                    }
                                    project.Dependencies = dependencies.ToArray();
                                }
                            }

                            projects.Add(project);
                        }
                        break;
                }
            }

            Projects = projects.ToArray();
            Platforms = platforms.ToArray();
            Configurations = configs.ToArray();
        }
        public override void ManualWrite3rdParty(string filePath)
        {
            List<string> lines = new List<string>();
            lines.AddRange(
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                "# Visual Studio Version 16",
                "VisualStudioVersion = " + VisualStudioVersion,
                "MinimumVisualStudioVersion = " + MinimumVisualStudioVersion);
            foreach (Project project in Projects)
            {
                lines.Add(project.GetLine());
                if (project.Dependencies != null && project.Dependencies.Length > 0)
                {

                }
                lines.Add("EndProject");
            }
            File.WriteAllLines(filePath, lines.ToArray());
        }
    }
}
