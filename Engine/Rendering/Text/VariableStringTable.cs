using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Rendering.Text
{
    public class TagNameAttribute : Attribute
    {
        private string _tagName;
        private bool _needsTickUpdate = false;
        public TagNameAttribute(string tagName)
        {
            _tagName = tagName;
        }

        /// <summary>
        /// The name put between the opening and closing tag.
        /// </summary>
        public string TagName { get => _tagName; set => _tagName = value; }
        /// <summary>
        /// If the string needs to be redrawn with each tick. Otherwise static.
        /// </summary>
        public bool NeedsTickUpdate { get => _needsTickUpdate; set => _needsTickUpdate = value; }
    }
    /// <summary>
    /// Used to retrieve various text information from the engine using <example> tags within strings drawn using the TextDrawer class.
    /// In order to draw < or > normally, put a \ before the bracket, like this: \<example\>
    /// Override and add your own methods to execute when the method's name is put between brackets.
    /// </summary>
    public class VariableStringTable
    {
        [TSerialize]
        private Dictionary<string, Func<string>> _table = new Dictionary<string, Func<string>>();
        public string this[string id]
        {
            get
            {
                if (!_table.ContainsKey(id))
                    throw new Exception("Invalid variable string id: " + id);
                return _table[id]();
            }
        }
        [TagName("hour_12")]
        public string GetHour12()
        {
            return DateTime.Now.Hour.ModRange(0, 12).ToString();
        }
        [TagName("time_am_pm")]
        public string GetTimeAmOrPm()
        {
            return DateTime.Now.Hour < 12 ? "am" : "pm";
        }
        [TagName("hour_24")]
        public string GetHour24()
        {
            return DateTime.Now.Hour.ToString();
        }
        [TagName("username_0")]
        public string GetUserName0()
        {
            return Engine.LocalPlayers.Count > 0 ? Engine.LocalPlayers[0].PlayerInfo.UserName : "[InvalidPlayerIndexError]";
        }
        [TagName("username_1")]
        public string GetUserName1()
        {
            return Engine.LocalPlayers.Count > 1 ? Engine.LocalPlayers[1].PlayerInfo.UserName : "[InvalidPlayerIndexError]";
        }
        [TagName("username_2")]
        public string GetUserName2()
        {
            return Engine.LocalPlayers.Count > 2 ? Engine.LocalPlayers[2].PlayerInfo.UserName : "[InvalidPlayerIndexError]";
        }
        [TagName("username_3")]
        public string GetUserName3()
        {
            return Engine.LocalPlayers.Count > 3 ? Engine.LocalPlayers[3].PlayerInfo.UserName : "[InvalidPlayerIndexError]";
        }
    }
}
