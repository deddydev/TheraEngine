using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Input.Devices
{
    public abstract class BaseGamepadConfiguration : TFileObject
    {
        public BaseGamepadConfiguration()
        {
            ButtonMap = new Dictionary<EGamePadButton, EGamePadButton>();
            AxisMap = new Dictionary<EGamePadAxis, EGamePadAxis>();

            for (int i = 0; i < 14; ++i)
                ButtonMap.Add((EGamePadButton)i, (EGamePadButton)i);

            for (int i = 0; i < 6; ++i)
                AxisMap.Add((EGamePadAxis)i, (EGamePadAxis)i);
        }

        [TSerialize]
        public Dictionary<EGamePadButton, EGamePadButton> ButtonMap { get; set; }
        [TSerialize]
        public Dictionary<EGamePadAxis, EGamePadAxis> AxisMap { get; set; }
    }
}
