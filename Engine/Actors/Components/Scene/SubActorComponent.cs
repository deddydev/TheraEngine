using System;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Scene
{
    public class SubActorComponent<T> : SceneComponent where T : class, IActor
    {
        [TSerialize]
        public LocalFileRef<T> Actor { get; set; }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            SceneComponent root = Actor?.File?.RootComponent;
            if (root != null)
            {
                localTransform = root.WorldMatrix;
                inverseLocalTransform = root.InverseWorldMatrix;
            }
            else
            {
                localTransform = Matrix4.Identity;
                inverseLocalTransform = Matrix4.Identity;
            }
        }
    }
    public class SubActorComponent : SceneComponent
    {
        [TSerialize]
        public LocalFileRef<Actor> Actor { get; set; }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            SceneComponent root = Actor?.File?.RootComponent;
            if (root != null)
            {
                localTransform = root.WorldMatrix;
                inverseLocalTransform = root.InverseWorldMatrix;
            }
            else
            {
                localTransform = Matrix4.Identity;
                inverseLocalTransform = Matrix4.Identity;
            }
        }
    }
}
