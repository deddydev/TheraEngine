using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.HUD
{
    public class ModelHudComponent : RenderableHudComponent
    {
        SkeletalMesh _model;
        Camera _camera;
    }
}
