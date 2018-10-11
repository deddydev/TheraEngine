using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;
using SevenZip;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public AbstractWriter Writer { get; private set; }
        public MemberTreeNode RootNode { get; internal set; }

        public async Task SerializeXMLAsync(
            TFileObject obj,
            string dirPath,
            string name,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string filePath = TFileObject.GetFilePath(dirPath, name, EProprietaryFileFormat.XML, obj.GetType());
            Writer = new WriterXML(this, obj, filePath, flags, progress, cancel, null);
            await Serialize();
        }
        public async Task SerializeBinaryAsync(
            TFileObject obj,
            string dirPath,
            string name,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder order,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            ICodeProgress compressionProgress)
        {
            string filePath = TFileObject.GetFilePath(dirPath, name, EProprietaryFileFormat.Binary, obj.GetType());
            Writer = new WriterBinary(this, obj, filePath, flags, progress, cancel, order, encrypted, compressed, encryptionPassword, compressionProgress);
            await Serialize();
        }
        private async Task Serialize()
        {
            RootNode = Writer.CreateNode(Writer.RootFileObject);
            await RootNode.CollectSerializedMembers();
            await Writer.WriteTree(RootNode);
        }
        public void StartElement(string name)
        {

        }
        public void EndElement()
        {

        }
        public void WriteAttributeString(string name, string value)
        {

        }
        public void WriteElementString(string name, string value)
        {

        }
    }
}
