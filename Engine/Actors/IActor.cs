using System;
using System.Collections.Generic;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds;

namespace TheraEngine.Actors
{
    public interface IActor : IFileObject
    {
        event DelRootComponentChanged RootComponentChanged;
        event Action<IActor> SceneComponentCacheRegenerated;
        event Action<IActor> LogicComponentsChanged;

        IMap MapAttachment { get; set; }
        bool IsConstructing { get; }
        IWorld OwningWorld { get; }

        bool IsSpawned { get; }

        IReadOnlyCollection<ISceneComponent> SceneComponentCache { get; }
        IOriginRebasableComponent RootComponent { get; }
        
        EventList<ILogicComponent> LogicComponents { get; }

        T1 FindFirstLogicComponentOfType<T1>() where T1 : class, ILogicComponent;
        T1[] FindLogicComponentsOfType<T1>() where T1 : class, ILogicComponent;
        ILogicComponent FindFirstLogicComponentOfType(Type type);
        ILogicComponent[] FindLogicComponentsOfType(Type type);

        bool IsSpawnedIn(IWorld owningWorld);
        void Despawn();
        void RebaseOrigin(Vec3 newOrigin);
        void Spawned(IWorld world);
        void Despawned();
        void GenerateSceneComponentCache();
    }
}
