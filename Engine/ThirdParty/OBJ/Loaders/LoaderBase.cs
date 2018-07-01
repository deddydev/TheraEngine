using System.IO;

namespace ObjLoader.Loader.Loaders
{
    public abstract class LoaderBase
    {
        private StreamReader _lineStreamReader;
        private string _path;

        protected void StartLoad(string path)
        {
            _path = path;
            if (!File.Exists(path))
                return;
            using (_lineStreamReader = new StreamReader(File.Open(_path, FileMode.Open, FileAccess.Read)))
            {
                while (!_lineStreamReader.EndOfStream)
                {
                    ParseLine();
                }
            }
        }

        private void ParseLine()
        {
            var currentLine = _lineStreamReader.ReadLine();

            if (string.IsNullOrWhiteSpace(currentLine) || currentLine[0] == '#')
                return;

            var fields = currentLine.Trim().Split(null, 2);
            if (fields.Length == 0)
                return;

            var keyword = fields[0].Trim();
            string data = null;

            if (fields.Length != 1)
                data = fields[1].Trim();

            if (keyword.Equals("mtllib"))
            {
                if (!data.Contains(":"))
                {
                    if (data.StartsWith("\\"))
                        data.Substring(1);
                    data = Path.GetDirectoryName(_path) + "\\" + data;
                }
            }

            ParseLine(keyword, data);
        }

        protected abstract void ParseLine(string keyword, string data);
    }
}