using System;
using System.Collections.Generic;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace TheraEngine.Actors
{
    public interface IActor : IFileObject
    {
        event DelRootComponentChanged RootComponentChanged;
        event Action<BaseActor> SceneComponentCacheRegenerated;
        event Action<BaseActor> LogicComponentsChanged;

        Map MapAttachment { get; }
        bool IsConstructing { get; }
        World OwningWorld { get; }

        bool IsSpawned { get; }

        IReadOnlyCollection<SceneComponent> SceneComponentCache { get; }
        OriginRebasableComponent RootComponent { get; }
        
        EventList<LogicComponent> LogicComponents { get; }

        T1 FindFirstLogicComponentOfType<T1>() where T1 : LogicComponent;
        T1[] FindLogicComponentsOfType<T1>() where T1 : LogicComponent;
        LogicComponent FindFirstLogicComponentOfType(Type type);
        LogicComponent[] FindLogicComponentsOfType(Type type);
    }
}
