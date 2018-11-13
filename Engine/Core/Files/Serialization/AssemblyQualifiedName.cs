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
                ClassName = classPathParts.Length == 0 ? null : classPathParts[classPathParts.Length - 1];
            }
        }
        public string ClassName { get; private set; }
        public string AssemblyName { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public int VersionBuild { get; set; }
        public int VersionRevision { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public byte[] PublicKeyToken { get; set; }

        public AssemblyName GetAssemblyName()
        {
            AssemblyName name = new AssemblyName(AssemblyName)
            {
                Version = new Version(VersionMajor, VersionMinor, VersionBuild, VersionRevision),
                CultureInfo = CultureInfo
            };
            name.SetPublicKeyToken(PublicKeyToken);
            return name;
        }

        public AssemblyQualifiedName(string value)
        {
            string[] parts = value.Split(',');

            ClassPath = "";
            for (int i = 0; i < parts.Length - 4; ++i)
            {
                if (i != 0)
                    ClassPath += ",";
                ClassPath += parts[i];
            }

            AssemblyName = parts[parts.Length - 4].Trim();

            string version = parts[parts.Length - 3].Substring(parts[parts.Length - 3].IndexOf('=') + 1);
            string culture = parts[parts.Length - 2].Substring(parts[parts.Length - 2].IndexOf('=') + 1);
            string publicKeyToken = parts[parts.Length - 1].Substring(parts[parts.Length - 1].IndexOf('=') + 1);

            string[] nums = version.Split('.');
            VersionMajor = int.Parse(nums[0]);
            VersionMinor = int.Parse(nums[1]);
            VersionBuild = int.Parse(nums[2]);
            VersionRevision = int.Parse(nums[3]);

            CultureInfo = culture == "neutral" ? null : new CultureInfo(culture);
            PublicKeyToken = publicKeyToken == "null" ? null : publicKeyToken.SelectEvery(2, x => byte.Parse(x[0].ToString() + x[1].ToString())).ToArray();
        }

        public override string ToString()
        {
            return $"{ClassPath}, {AssemblyName}, Version={VersionMajor}.{VersionMinor}.{VersionBuild}.{VersionRevision}, Culture={CultureInfo?.ToString() ?? "neutral"}, PublicKeyToken={PublicKeyToken?.ToString() ?? "null"}";
        }
    }
}