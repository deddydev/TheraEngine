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
        WorldState                  = 0x0001,
        WorldSettings               = 0x0002,

        Map                         = 0x0100,
        MapState                    = 0x0101,
        MapSettings                 = 0x0102,

        Actor                       = 0x0200,
        ActorState                  = 0x0201,
        ActorSettings               = 0x0202,

        Camera                      = 0x0300,
        CameraState                 = 0x0301,
        CameraSettings              = 0x0302,

        Component                   = 0x1000,
        ComponentState              = 0x1001,
        ComponentSettings           = 0x1002,

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

        Shape                       = 0x2000,

        Pawn                        = 0x2100,
        Character                   = 0x2200,
        Vehicle                     = 0x2300,
        
        Emitter                     = 0x3000,
        Particle                    = 0x3100,

        AnimationContainer          = 0x3200,
        AnimationBool               = 0x3300,
        AnimationString             = 0x3400,
        AnimationScalar             = 0x3500,
        AnimationVec2               = 0x3600,
        AnimationVec3               = 0x3700,
        AnimationVec4               = 0x3800,
        KeyframeTrack               = 0x3900,

        SkeletalMesh,
        SkeletalRigidSubMesh,
        SkeletalSoftSubMesh,
        StaticMesh,
        StaticRigidSubMesh,
        StaticSoftSubMesh,
        RigidPhysicsDriver,
        SoftPhysicsDriver,
        Material,
        Texture,
        Skeleton,
        Bone,
        FrameState,

        Cutscene,

        EngineSettings,
        UserSettings,

        Sound,
        
        SingleFileRef,
        MultiFileRef,

        Rotator,
        Plane,
    }
}
