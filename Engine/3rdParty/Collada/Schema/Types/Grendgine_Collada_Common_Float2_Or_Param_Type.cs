using System.Xml;
using System.Xml.Serialization;
namespace grendgine_collada
{

    [System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Common_Float2_Or_Param_Type
	{
		[XmlElement(ElementName = "param")]
		public Grendgine_Collada_Param Param;	
		//TODO: cleanup to legit array

		[XmlTextAttribute()]
	    public string Value_As_String;
	}
}



