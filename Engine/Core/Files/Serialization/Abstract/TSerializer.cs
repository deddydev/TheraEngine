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

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        public AbstractWriter Writer { get; private set; }
        
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
        private async Task Serialize()
        {
            await Writer.Start();
            await Writer.Finish();
        }
    }
}
