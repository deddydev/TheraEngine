using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            private FileStream _stream;
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
                using (_stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, FileOptions.RandomAccess))
                using (_reader = XmlReader.Create(_stream, _settings))
                {
                    _stream.Position = 0;
                    await _reader.MoveToContentAsync();
                    RootNode = await ReadElementAsync();
                }
            }
            private async Task<MemberTreeNode> ReadElementAsync()
            {
                MemberTreeNode node = null, childNode;
                string name, value;

                while (_reader.NodeType != XmlNodeType.EndElement && !_reader.EOF)
                {
                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            name = _reader.Name;
                            if (node != null)
                            {
                                childNode = await ReadElementAsync();
                                node.ChildElementMembers.Add(childNode);
                            }
                            else
                            {
                                node = new MemberTreeNode(null, new TSerializeMemberInfo(null, name));
                                if (_reader.HasAttributes)
                                {
                                    while (_reader.MoveToNextAttribute())
                                    {
                                        name = _reader.Name;
                                        value = await _reader.GetValueAsync();
                                        node?.ChildAttributeMembers?.Add(SerializeAttribute.FromString(name, value));
                                    }
                                }
                                if (_reader.IsEmptyElement)
                                {
                                    await _reader.ReadAsync();
                                    return node;
                                }
                            }
                            break;
                        case XmlNodeType.Text:
                            value = await _reader.GetValueAsync();
                            if (node != null)
                                node.ChildElementObjectMemberAsString = value;
                            break;
                        default:
                            break;
                    }
                    await _reader.ReadAsync();
                }
                return node;
            }
        }
    }
}
