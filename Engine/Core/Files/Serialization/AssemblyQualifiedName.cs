using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class AssemblyQualifiedName
    {
        private string _classPath;
        public string ClassPath
        {
            get => _classPath;
            set
            {
                _classPath = value;
                string[] classPathParts = ClassPath.Split(new char[] { '.', '+', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                ClassName = classPathParts[classPathParts.Length - 1];
            }
        }
        public string ClassName { get; private set; }
        public string AssemblyName { get; set; }
        public int VersionMax { get; set; }
        public int VersionMin { get; set; }
        public int VersionBuild { get; set; }
        public int VersionRev { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public byte[] PublicKeyToken { get; set; }

        public AssemblyName GetAssemblyName()
        {
            AssemblyName name = new AssemblyName(AssemblyName)
            {
                Version = new Version(VersionMax, VersionMin, VersionBuild, VersionRev),
                CultureInfo = CultureInfo
            };
            name.SetPublicKeyToken(PublicKeyToken);
            return name;
        }

        public AssemblyQualifiedName(string value)
        {
            string[] parts = value.Split(',');

            ClassPath = parts[0].Trim();
            AssemblyName = parts[parts.Length - 4].Trim();

            string version = parts[parts.Length - 3].Substring(parts[parts.Length - 3].IndexOf('=') + 1);
            string culture = parts[parts.Length - 2].Substring(parts[parts.Length - 2].IndexOf('=') + 1);
            string publicKeyToken = parts[parts.Length - 1].Substring(parts[parts.Length - 1].IndexOf('=') + 1);

            string[] nums = version.Split('.');
            VersionMax = int.Parse(nums[0]);
            VersionMin = int.Parse(nums[1]);
            VersionBuild = int.Parse(nums[2]);
            VersionRev = int.Parse(nums[3]);

            CultureInfo = culture == "neutral" ? null : new CultureInfo(culture);
            PublicKeyToken = publicKeyToken == "null" ? null : publicKeyToken.SelectEvery(2, x => byte.Parse(x[0].ToString() + x[1].ToString())).ToArray();
        }

        public override string ToString()
        {
            return $"{ClassPath}, {AssemblyName}, Version={VersionMax}.{VersionMin}.{VersionBuild}.{VersionRev}, Culture={CultureInfo?.ToString() ?? "neutral"}, PublicKeyToken={PublicKeyToken?.ToString() ?? "null"}";
        }
    }
}