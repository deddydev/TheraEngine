using System.Xml;
using System.Xml.Serialization;
namespace grendgine_collada
{

    [System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Ref_String
	{
		[XmlAttribute("ref")]
		public string Ref;
	}
}

