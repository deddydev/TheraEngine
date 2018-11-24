using System;

namespace TheraEngine.Rendering.Text
{
    public class LString
    {
        public string _id;
        public string _namespace;
        public bool _global;

        public string GetLocalizedString()
            => Engine.Game.LocalizedStringTable[_id, _namespace];
        
        public override string ToString()
            => GetLocalizedString();

        public static implicit operator string(LString str) 
            => str.GetLocalizedString();
    }
}
