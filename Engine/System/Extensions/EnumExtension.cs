namespace System
{
    public static class EnumExtension
    {
        //Converts one enum to the other using each name.
        public static Enum Convert(this Enum e, Type otherEnum)
        {
            return Enum.Parse(otherEnum, e.ToString()) as Enum;
        }
    }
}
