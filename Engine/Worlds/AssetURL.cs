using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Files;

namespace TheraEngine.Worlds
{
    public class AssetURL
    {
        public PathReference WorldFilePath { get; set; }
        public List<string> MemberPath { get; set; }
        public bool ResolveObject(out object asset, bool onlyIfWorldLoaded = true)
        {
            asset = null;
            if (!Engine.GlobalFileInstances.TryGetValue(WorldFilePath.Absolute, out IGlobalFileRef reference))
                return false;
            
            if (onlyIfWorldLoaded && !reference.IsLoaded)
                return false;

            IFileObject world = reference.File;
            Type currentType = world.GetType();
            object currentObject = world;
            for (int i = 0; i < MemberPath.Count; ++i)
            {
                string member = MemberPath[i];
                var field = currentType.GetField(member);
                var property = currentType.GetProperty(member);
                currentObject = field.GetValue(currentObject);
            }
        }
    }
}
