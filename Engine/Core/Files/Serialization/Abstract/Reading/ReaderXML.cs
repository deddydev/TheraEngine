using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer
    {
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
                Deserializer owner,
                string filePath,
                TypeProxy fileType,
                Stream stream,
                IProgress<float> progress,
                CancellationToken cancel,
                XmlReaderSettings settings)
                : base(owner, filePath, fileType, stream, progress, cancel)
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
                    using (_stream = new ProgressStream(Stream, null, null))
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
                                node.Children.Add(childNode);
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
                                        node.Attributes?.Add(SerializeAttribute.FromString(name, value));
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
                            node?.Content.SetValueAsString(value);
                            await _reader.ReadAsync();
                            break;
                        case XmlNodeType.EndElement:
                            name = _reader.Name;
                            await _reader.ReadAsync();
                            if (node is null)
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
