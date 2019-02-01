using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TDeserializer
    {
        /// <summary>
        /// Reads the file at <paramref name="filePath"/> as a binary file.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        /// <param name="encryptionPassword">If encrypted, this is the password to use to decrypt.</param>
        public async Task<object> DeserializeXMLAsync(
            string filePath,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            Format = EProprietaryFileFormat.XML;
            Reader = new ReaderXML(this, filePath, progress, cancel, null);
            object file = await Reader.CreateObjectAsync();
            Engine.PrintLine("Deserialized XML file at {0}", filePath);
            return file;
        }
        public class ReaderXML : AbstractReader
        {
            public override EProprietaryFileFormatFlag Format => EProprietaryFileFormatFlag.XML;

            private readonly XmlReaderSettings _settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                Async = true,
            };

            private XmlReader _reader;

            public ReaderXML(
                TDeserializer owner,
                string filePath,
                IProgress<float> progress,
                CancellationToken cancel,
                XmlReaderSettings settings)
                : base(owner, filePath, progress, cancel)
            {
                if (settings != null)
                {
                    settings.Async = true;
                    _settings = settings;
                }
            }
            protected override async Task ReadTreeAsync()
            {
                try
                {
                    long currentBytes = 0L;
                    using (_stream = new ProgressStream(new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read), null, null))
                    {
                        _reader = XmlReader.Create(_stream, _settings);
                        
                        if (Progress != null)
                        {
                            float length = _stream.Length;
                            _stream.SetReadProgress(new BasicProgress<int>(i =>
                            {
                                currentBytes += i;
                                Progress.Report(currentBytes / length);
                            }));
                        }

                        await _reader.MoveToContentAsync();
                        RootNode = await ReadElementAsync();
                        
                        _reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
            }
            private async Task<SerializeElement> ReadElementAsync()
            {
                SerializeElement node = null, childNode;
                string name, value;
                
                while (!_reader.EOF)
                {
                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            name = _reader.Name;
                            if (node != null)
                            {
                                childNode = await ReadElementAsync();
                                node.ChildElements.Add(childNode);
                            }
                            else
                            {
                                node = new SerializeElement(null, new TSerializeMemberInfo(null, name));
                                if (_reader.HasAttributes)
                                {
                                    while (_reader.MoveToNextAttribute())
                                    {
                                        name = _reader.Name;
                                        value = await _reader.GetValueAsync();
                                        node?.Attributes?.Add(SerializeAttribute.FromString(name, value));
                                    }
                                    await _reader.MoveToContentAsync();
                                }
                                bool empty = _reader.IsEmptyElement;
                                await _reader.ReadAsync();
                                if (empty)
                                    return node;
                            }
                            break;
                        case XmlNodeType.Text:
                            value = await _reader.GetValueAsync();
                            if (node != null)
                                node.SetElementContentAsString(value);
                            await _reader.ReadAsync();
                            break;
                        case XmlNodeType.EndElement:
                            name = _reader.Name;
                            await _reader.ReadAsync();
                            if (node == null)
                                Engine.LogWarning("No start element read for " + name);
                            else if (!string.Equals(node.Name, name, StringComparison.InvariantCulture))
                                Engine.LogWarning("End element / start element mismatch.");                            
                            return node;
                        default:
                            await _reader.ReadAsync();
                            break;
                    }
                }
                return node;
            }
        }
    }
}
