using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Files;

namespace TheraEngine.Core.Files
{
    [File3rdParty("txt", "rtf")]
    [FileDef("Text File")]
    public class TextFile : TFileObject, ITextSource
    {
        public event Action TextChanged;

        private string _text = null;
        [TString(true, false, false, true)]
        [TSerialize(IsXmlElementString = true)]
        public string Text
        {
            get => _text ?? LoadText();
            set
            {
                _text = value;
                OnTextChanged();
            }
        }

        protected void OnTextChanged() => TextChanged?.Invoke();

        public TextFile()
        {
            FilePath = null;
            Text = "";
        }
        public TextFile(string path)
        {
            FilePath = path;
            Text = null;
        }

        public static TextFile FromText(string text)
            => new TextFile() { Text = text };

        public static implicit operator string(TextFile textFile)
            => textFile?.Text;
        public static implicit operator TextFile(string text)
            => FromText(text);

        protected internal override void Read3rdParty(string filePath)
        {
            FilePath = filePath;
            Text = LoadText();
        }
        protected internal override void Write3rdParty(string filePath)
        {
            File.WriteAllText(filePath, Text);
        }
        public string LoadText()
        {
            _text = null;
            if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                _text = File.ReadAllText(FilePath, GetEncoding(FilePath));
            return _text;
        }
        public void UnloadText()
        {
            _text = null;
        }
        public async Task<string> LoadAsync()
        {
            _text = null;
            if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                _text = await Task.Run(() => File.ReadAllText(FilePath, GetEncoding(FilePath)));
            return _text;
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