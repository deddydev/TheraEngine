using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Serializer
    {
        public class WriterXML : AbstractWriter
        {
            private XmlWriter _writer;

            public override EProprietaryFileFormatFlag Format => EProprietaryFileFormatFlag.XML;

            private readonly XmlWriterSettings _settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Async = true,
            };

            public WriterXML(
                Serializer owner,
                object rootFileObject,
                string filePath,
                Stream stream,
                ESerializeFlags flags,
                IProgress<float> progress,
                CancellationToken cancel,
                XmlWriterSettings settings)
                : base(owner, rootFileObject, filePath, stream, flags, progress, cancel)
            {
                if (settings != null)
                    _settings = settings;
            }
            protected override async Task WriteTreeAsync()
            {
                //TODO: verify that the file path can be written to (not in use, not readonly)
                //If not, write to a temp path and add to "saved but not where you want it" section
                //So the user can situate the area for the file and move it there properly
                long currentBytes = 0L;
                using (_stream = new ProgressStream(Stream, null, null))
                using (_writer = XmlWriter.Create(_stream, _settings))
                {
                    if (Progress != null)
                    {
                        float invLength = 1.0f / _stream.Length;
                        _stream.SetWriteProgress(new BasicProgress<int>(i =>
                        {
                            Interlocked.Add(ref currentBytes, i);
                            Progress.Report(currentBytes * invLength);
                        }));
                    }

                    await _writer.WriteStartDocumentAsync();
                    await WriteElementAsync(RootNode, true, false);
                    await _writer.WriteEndDocumentAsync();
                }
            }
            private async Task WriteElementAsync(SerializeElement node, bool root, bool isSharedObject)
            {
                string elementName = SerializationCommon.FixElementName(node.Name);
                if (!root && !isSharedObject && node.Object is TObject tobj && WritingSharedObjectIndices.ContainsKey(tobj.Guid))
                {
                    int index = WritingSharedObjectIndices[tobj.Guid];
                    await _writer.WriteStartElementAsync(null, elementName, null);
                    await _writer.WriteAttributeStringAsync(null, "SharedIndex", null, index.ToString());
                    await _writer.WriteEndElementAsync();

                    //await _writer.WriteElementStringAsync(null, "SharedObject", null, index.ToString());

                    return;
                }

                await _writer.WriteStartElementAsync(null, elementName, null);
                {
                    if (CancelRequested)
                    {
                        await _writer.WriteEndElementAsync();
                        return;
                    }
                    
                    if (node.ObjectType != node.MemberInfo.MemberType || root || isSharedObject)
                        node.InsertAttribute(0, SerializationCommon.TypeIdent, node.ObjectType.AssemblyQualifiedName);

                    IList<SerializeAttribute> attributes = node.Attributes;
                    IList<SerializeElement> childElements = node.Children;
                    bool hasChildStringData = node.Content.GetString(out string childStringData);
                    
                    foreach (SerializeAttribute attribute in attributes)
                    {
                        if (attribute.GetString(out string value))
                            await _writer.WriteAttributeStringAsync(null, attribute.Name, null, value);
                        
                        if (CancelRequested)
                        {
                            await _writer.WriteEndElementAsync();
                            return;
                        }
                    }

                    bool hasSharedObjects = root && !isSharedObject && WritingSharedObjects.Count > 0;
                    if (hasSharedObjects)
                    {
                        await _writer.WriteStartElementAsync(null, "SharedObjects", null);
                        {
                            await _writer.WriteAttributeStringAsync(null, "Count", null, WritingSharedObjects.Count.ToString());

                            SerializeElement elem;
                            foreach (var shared in WritingSharedObjects)
                            {
                                elem = shared.Value;
                                
                                await WriteElementAsync(elem, false, true);
                                if (CancelRequested)
                                {
                                    //Close SharedObjects
                                    await _writer.WriteEndElementAsync();

                                    //Close Node
                                    await _writer.WriteEndElementAsync();

                                    return;
                                }
                            }
                        }
                        await _writer.WriteEndElementAsync();
                    }

                    if (hasChildStringData)
                    {
                        bool needsSeparateElement = hasSharedObjects || childElements.Count > 0;

                        if (needsSeparateElement)
                            await _writer.WriteStartElementAsync(null, "StringData", null);

                        await _writer.WriteStringAsync(childStringData);

                        if (needsSeparateElement)
                            await _writer.WriteEndElementAsync();

                        if (CancelRequested)
                            return;
                    }
                
                    foreach (SerializeElement childNode in childElements)
                    {
                        await WriteElementAsync(childNode, false, false);
                        if (CancelRequested)
                            break;
                    }
                }
                await _writer.WriteEndElementAsync();
            }
        }
    }
}
