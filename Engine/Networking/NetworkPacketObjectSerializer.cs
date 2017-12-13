using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Networking
{
    public class NetworkPacketObjectSerializer : NetworkCommsDotNet.DPSBase.DataSerializer
    {
        protected override object DeserialiseDataObjectInt(Stream inputStream, Type resultType, Dictionary<string, string> options)
        {

        }

        protected override void SerialiseDataObjectInt(Stream ouputStream, object objectToSerialise, Dictionary<string, string> options)
        {

        }
    }
}
