using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Worlds;

namespace TheraEngine.Core.Files.References
{
    //public class WorldFileRef
    //{
    //    [TSerialize]
    //    public bool UseWorldCollection { get; set; }
    //    [TSerialize]
    //    public int WorldCollectionIndex { get; set; }
    //    [TSerialize]
    //    public GlobalFileRef<TWorld> InternalReference
    //    {
    //        get => _fileRef;
    //        set => _fileRef = value ?? new GlobalFileRef<TWorld>();
    //    }
    //    [TSerialize]
    //    public GlobalFileRef<TWorld> WorldCollectionReference
    //    {
    //        get
    //        {
    //            if (UseWorldCollection && (Engine.Game?.WorldCollection?.IndexInRange(WorldCollectionIndex) ?? false))
    //                return Engine.Game.WorldCollection[WorldCollectionIndex];
    //            return null;
    //        }
    //        set
    //        {
    //            if (UseWorldCollection && (Engine.Game?.WorldCollection?.IndexInRange(WorldCollectionIndex) ?? false))
    //                Engine.Game.WorldCollection[WorldCollectionIndex] = value ?? new GlobalFileRef<TWorld>();
    //        }
    //    }
    //    public GlobalFileRef<TWorld> GetFileRef()
    //    {
    //        if (UseWorldCollection && (Engine.Game?.WorldCollection?.IndexInRange(WorldCollectionIndex) ?? false))
    //            return Engine.Game.WorldCollection[WorldCollectionIndex];
    //        else
    //            return InternalReference;
    //    }
    //}
}
