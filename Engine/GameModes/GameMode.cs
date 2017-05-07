using CustomEngine.Files;
using CustomEngine.Input;
using CustomEngine.Worlds.Actors;
using System;
using System.Runtime.InteropServices;

namespace CustomEngine.GameModes
{
    public enum InheritableBool
    {
        Inherited,
        True,
        False
    }
    public struct InheritableFloat
    {
        public InheritableFloat(float value, bool inherited = false)
        {
            _value = value;
            _inherited = inherited;
        }

        public float _value;
        public bool _inherited;

        public static implicit operator InheritableFloat(float value)
        {
            return new InheritableFloat(value);
        }
        public static implicit operator InheritableFloat(bool inherited)
        {
            return new InheritableFloat(0.0f, inherited);
        }
        public static implicit operator float(InheritableFloat value)
        {
            if (value._inherited)
                throw new Exception("Value not set.");
            return value._value;
        }
    }
    public struct InheritableInt
    {
        public InheritableInt(int value, bool inherited = false)
        {
            _value = value;
            _inherited = inherited;
        }
        
        public int _value;
        public bool _inherited;

        public static implicit operator InheritableInt(int value)
        {
            return new InheritableInt(value);
        }
        public static implicit operator InheritableInt(bool inherited)
        {
            return new InheritableInt(0, inherited);
        }
        public static implicit operator int(InheritableInt value)
        {
            if (value._inherited)
                throw new Exception("Value not set.");
            return value._value;
        }
    }
    public struct SubClassOf<T> where T : class, new()
    {
        public SubClassOf(Type t) { }
        public T CreateNew()
        {
            return new T();
        }
        public T2 CreateNew<T2>() where T2 : T, new()
        {
            return new T2();
        }
    }
    public abstract class BaseGameMode : FileObject
    {
        public abstract void BeginGameplay();
        public abstract void EndGameplay();
        public abstract void AbortGameplay();
    }
    public abstract class GameMode<PawnType> : BaseGameMode
        where PawnType : class, IPawn, new()
    {
        protected SubClassOf<PawnType> _pawnClass;

        public SubClassOf<PawnType> PawnClass
        {
            get => _pawnClass;
            set => _pawnClass = value;
        }
        
        public int _numSpectators, _numPlayers, _numComputers;
    }
}
