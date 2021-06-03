using System.Collections.Generic;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;

namespace TheraEngine.Actors.Types
{
    public sealed class ActorGroupComponent : TransformComponent
    {
        [TSerialize]
        public EventList<FileRef<BaseActor>> Actors { get; } = new EventList<FileRef<BaseActor>>();

        private void Actors_PostAnythingRemoved(FileRef<BaseActor> item)
        {
            BaseActor actor = item.File;
            if (actor is null)
                return;

            if (IsSpawned)
            {
                OwningWorld.DespawnActor(actor);
                ChildSockets.Remove(actor.RootComponentGeneric);
            }
        }
        private void Actors_PostAnythingAdded(FileRef<BaseActor> item)
        {
            BaseActor actor = item.File;
            if (actor is null)
                return;

            if (IsSpawned)
            {
                OwningWorld.SpawnActor(actor);
                ChildSockets.Add(actor.RootComponentGeneric);
            }
        }
        protected override void OnSpawned()
        {
            base.OnSpawned();

            foreach (var actor in Actors)
            {
                OwningWorld.SpawnActor(actor.File);
                ChildSockets.Add(actor.File.RootComponentGeneric);
            }

            Actors.PostAnythingAdded += Actors_PostAnythingAdded;
            Actors.PostAnythingRemoved += Actors_PostAnythingRemoved;
        }
        protected override void OnDespawned()
        {
            Actors.PostAnythingAdded -= Actors_PostAnythingAdded;
            Actors.PostAnythingRemoved -= Actors_PostAnythingRemoved;

            foreach (var actor in Actors)
            {
                OwningWorld.DespawnActor(actor.File);
                ChildSockets.Remove(actor.File.RootComponentGeneric);
            }

            base.OnDespawned();
        }
    }
    /// <summary>
    /// Groups a collection of actors under a single transform.
    /// </summary>
    [TFileDef("Group Actor", "Groups a collection of actors under a single transform.")]
    public sealed class GroupActor : Actor<ActorGroupComponent>
    {
        public GroupActor() : base("NewFolder", false) { }
    }
}
