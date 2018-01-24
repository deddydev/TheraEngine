namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FileDef : Attribute
    {
        public FileDef(string userFriendlyName, string description = null)
        {
            UserFriendlyName = userFriendlyName;
            Description = description;
        }
        
        public string UserFriendlyName { get; set; }
        public string Description { get; set; }
    }
}
