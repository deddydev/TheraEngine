using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileClass("", "", IsSpecialDeserialize = true)]
    public class TextFile : FileObject
    {
        private string _text = null;
        [TSerialize("Text", XmlNodeType = EXmlNodeType.ElementString)]
        public string Text
        {
            get => _text ?? Load();
            set => _text = value;
        }

        public TextFile()
        {
            FilePath = null;
            Text = null;
        }
        public TextFile(string path)
        {
            FilePath = path;
            Text = null;
        }
        public static TextFile FromText(string text)
        {
            return new TextFile() { Text = text };
        }
        public static implicit operator string(TextFile textFile)
        {
            return textFile?.Text;
        }
        public static implicit operator TextFile(string text)
        {
            return FromText(text);
        }
        public string Load()
        {
            Text = null;
            if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                Text = File.ReadAllText(FilePath, GetEncoding(FilePath));
            return Text;
        }
        public async Task<string> LoadAsync()
        {
            Text = null;
            if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                Text = await Task.Run(() => File.ReadAllText(FilePath, GetEncoding(FilePath)));
            return Text;
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                file.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2B && bom[1] == 0x2F && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF) return Encoding.UTF8;
            if (bom[0] == 0xFF && bom[1] == 0xFE) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xFE && bom[1] == 0xFF) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xFE && bom[3] == 0xFF) return Encoding.UTF32;
            return Encoding.Default;
        }
    }
}