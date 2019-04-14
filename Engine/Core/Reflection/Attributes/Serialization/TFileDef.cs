namespace System.ComponentModel
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TFileDef : Attribute
    {
        public TFileDef(string userFriendlyName, string description = null)
        {
            UserFriendlyName = userFriendlyName;
            Description = description;
        }
        
        public string UserFriendlyName { get; set; }
        public string Description { get; set; }
    }
}
