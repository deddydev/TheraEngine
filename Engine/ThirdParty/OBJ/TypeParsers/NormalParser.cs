using Extensions;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;
using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.TypeParsers
{
    public class NormalParser : TypeParserBase, INormalParser
    {
        private readonly INormalDataStore _normalDataStore;

        public NormalParser(INormalDataStore normalDataStore)
        {
            _normalDataStore = normalDataStore;
        }

        protected override string Keyword
        {
            get { return "vn"; }
        }

        public override void Parse(string line)
        {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            var normal = new Vec3(x, y, z);
            _normalDataStore.AddNormal(normal);
        }
    }
}