using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine
{
    public enum ResourceType
    {
        Object                      = 0xFF00,
        
        World                       = 0x0000,
        Map                         = 0x0100,

        Component                   = 0x1000,
        InstanceComponent           = 0x1100,
        SceneComponent              = 0x1200,
        CharacterMovementComponent  = 0x1300,
        TransformMovementComponent  = 0x1400,
        InteractionComponent        = 0x1500,
        MovementComponent           = 0x1600,
        BoomComponent               = 0x1700,
        CameraComponent             = 0x1800,
        DecalComponent              = 0x1900,
        LoadingZoneComponent        = 0x1A00,
        ModelComponent              = 0x1B00,
        ShapeComponent              = 0x1C00,

        Actor                       = 0x2000,
        Pawn                        = 0x2100,
        Character                   = 0x2200,
        Vehicle                     = 0x2300,
        
        Emitter                     = 0x3000,
        Particle                    = 0x3100,
    }
}
