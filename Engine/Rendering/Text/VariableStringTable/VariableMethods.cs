using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Text
{
    /// <summary>
    /// Provides functionality for retrieving dynamic information for use in strings.
    /// Override this class to provide your own methods with their own tag names.
    /// </summary>
    [TFileDef("Variable String Table", 
        "Provides functionality for retrieving dynamic information for use in strings.")]
    [TFileExt("vtbl")]
    public partial class VariableStringTable : TFileObject
    {
        [TagName("currentTime")]
        public virtual string GetCurrentTime(string format)
        {
            string time;
            try
            {
                time = DateTime.Now.ToString(format);
            }
            catch (Exception ex)
            {
                time = "[" + ex.Message + "]";
            }
            return time;
        }
        [TagName("localPlayerName")]
        public virtual string GetLocalPlayerName(int index)
        {
            //if (Engine.LocalPlayers.IndexInRange(index))
            //    return Engine.LocalPlayers[index].PlayerInfo.UserName;
            //else
            //    return $"[local player index {index} is out of range]";
            return "";
        }
    }
}
