using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files;

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
        public async Task<TFileObject> DeserializeXMLAsync(
            string filePath,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            Format = EProprietaryFileFormat.Binary;
            Type fileType = SerializationCommon.DetermineType(filePath);
            TFileObject rootFileObject = SerializationCommon.CreateObject(fileType) as TFileObject;
            Reader = new ReaderXML(this, rootFileObject, filePath, progress, cancel, null);
            await Reader.ReadTree();

            Engine.PrintLine("Deserialized binary file at {0}", filePath);
            return Reader.RootFileObject;
        }
        private class ReaderXML : AbstractReader<XMLMemberTreeNode>
        {
            private FileStream _stream;
            private XmlReader _reader;

            private readonly XmlReaderSettings _settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                Async = true,
            };
            
            public ReaderXML(TDeserializer owner, XmlReaderSettings settings) : base(owner)
            {
                if (settings != null)
                    _settings = settings;
            }

            protected internal override Task ReadTree()
            {

                _reader.Dispose();
                _stream.Dispose();
            }

            public override XMLMemberTreeNode CreateNode(XMLMemberTreeNode parent, MemberInfo memberInfo)
                => new XMLMemberTreeNode(parent, memberInfo);
            public override XMLMemberTreeNode CreateNode(object root)
                => new XMLMemberTreeNode(root);
        }
    }
}
