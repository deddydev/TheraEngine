using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeXMLAsync(
            object fileObject,
            string filePath,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            Writer = new WriterXML(this, fileObject, filePath, flags, progress, cancel, null);
            await Writer.WriteObjectAsync();
            Engine.PrintLine("Serialized XML file to {0}", filePath);
        }
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeXMLAsync(
            object fileObject,
            string targetDirectoryPath,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, EProprietaryFileFormat.XML, fileObject.GetType());
            Writer = new WriterXML(this, fileObject, filePath, flags, progress, cancel, null);
            await Writer.WriteObjectAsync();
            Engine.PrintLine("Serialized XML file to {0}", filePath);
        }
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
                TSerializer owner,
                object rootFileObject,
                string filePath,
                ESerializeFlags flags,
                IProgress<float> progress,
                CancellationToken cancel,
                XmlWriterSettings settings)
                : base(owner, rootFileObject, filePath, flags, progress, cancel)
            {
                if (settings != null)
                    _settings = settings;
            }
            protected override async Task WriteTreeAsync()
            {
                long currentBytes = 0L;
                using (_stream = new ProgressStream(new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None), null, null))
                using (_writer = XmlWriter.Create(_stream, _settings))
                {
                    if (Progress != null)
                    {
                        float length = _stream.Length;
                        _stream.SetWriteProgress(new BasicProgress<int>(i =>
                        {
                            currentBytes += i;
                            Progress.Report(currentBytes / length);
                        }));
                    }
                    
                    await _writer.WriteStartDocumentAsync();
                    await WriteElementAsync(RootNode);
                    await _writer.WriteEndDocumentAsync();
                }
            }
            private async Task WriteElementAsync(SerializeElement node)
            {
                await _writer.WriteStartElementAsync(null, SerializationCommon.FixElementName(node.Name), null);
                {
                    if (ReportProgress())
                    {
                        await _writer.WriteEndElementAsync();
                        return;
                    }
                    
                    List<SerializeAttribute> attributes = node.Attributes;
                    List<SerializeElement> childElements = node.ChildElements;
                    bool hasChildStringData = node.GetElementContentAsString(out string childStringData);
                    
                    foreach (SerializeAttribute attribute in attributes)
                    {
                        if (attribute.GetString(out string value))
                        {
                            await _writer.WriteAttributeStringAsync(null, attribute.Name, null, value);
                            if (ReportProgress())
                            {
                                await _writer.WriteEndElementAsync();
                                return;
                            }
                        }
                        else
                        {
                            //Engine.LogWarning($"Cannot write attribute as string.");
                        }
                    }

                    if (hasChildStringData)
                    {
                        await _writer.WriteStringAsync(childStringData);
                        if (ReportProgress())
                        {
                            await _writer.WriteEndElementAsync();
                            return;
                        }
                    }
                    else
                        foreach (SerializeElement childNode in childElements)
                        {
                            await WriteElementAsync(childNode);
                            if (ReportProgress())
                            {
                                await _writer.WriteEndElementAsync();
                                return;
                            }
                        }
                }
                await _writer.WriteEndElementAsync();
            }
        }
    }
}
