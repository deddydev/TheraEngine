using TheraEngine.Files;
using TheraEngine.Input;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using TheraEngine.Input.Devices;
using System.Linq;
using TheraEngine.Rendering;
using System.ComponentModel;

namespace TheraEngine.GameModes
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
    public struct SubClassOf<T> where T : class
    {
        public SubClassOf(Type t) { }

        public T CreateNew(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
        public T2 CreateNew<T2>() where T2 : T, new()
        {
            return new T2();
        }
        public T2 CreateNew<T2>(params object[] args) where T2 : T
        {
            return (T2)Activator.CreateInstance(typeof(T2), args);
        }
    }
    public interface IGameMode
    {
        void BeginGameplay();
        void EndGameplay();
        void AbortGameplay();
    }
    [FileClass("MODE", "Game Mode")]
    public abstract class BaseGameMode : FileObject, IGameMode
    {
        private bool _disallowPausing = false;

        public virtual bool DisallowPausing
        {
            get => _disallowPausing;
            set => _disallowPausing = value;
        }

        //protected internal abstract void HandleLocalPlayerLeft(LocalPlayerController item);
        //protected internal abstract void HandleLocalPlayerJoined(LocalPlayerController item);

        /// <summary>
        /// Creates a local player controller with methods and properties that may pertain to this specific game mode.
        /// </summary>
        /// <param name="index">The player that will provide input to the controller.</param>
        /// <returns>The new local player controller.</returns>
        protected internal abstract void CreateLocalController(LocalPlayerIndex index);
        /// <summary>
        /// Creates a local player controller with methods and properties that may pertain to this specific game mode.
        /// </summary>
        /// <param name="index">The player that will provide input to the controller.</param>
        /// <param name="possessionQueue">The queue of pawns that want to be possessed by the controller.</param>
        /// <returns>The new local player controller.</returns>
        protected internal abstract void CreateLocalController(LocalPlayerIndex index, Queue<IPawn> possessionQueue);

        protected virtual void OnBeginGameplay() { }
        public void BeginGameplay()
        {
            CreateLocalPlayerControllers();
            OnBeginGameplay();
        }
        protected virtual void OnEndGameplay() { }
        public void EndGameplay()
        {
            Engine.DestroyLocalPlayerControllers();
            OnEndGameplay();
        }
        protected virtual void OnAbortGameplay() { }
        public void AbortGameplay()
        {
            OnAbortGameplay();
        }

        private void CreateLocalPlayerControllers()
        {
            InputDevice[] gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            InputDevice[] keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            InputDevice[] mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];

            if (keyboards.Any(x => x != null) ||
                mice.Any(x => x != null) ||
                gamepads.Any(x => x != null && x.Index == 0))
            {
                CreateLocalController(LocalPlayerIndex.One);
            }
            for (int i = 0; i < 4; ++i)
            {
                InputDevice gp = gamepads[i];
                if (gp != null && gp.Index > 0)
                    CreateLocalController(LocalPlayerIndex.One + gp.Index);
            }
        }
    }
    public class GameMode<PawnType, ControllerType> : BaseGameMode
        where PawnType : class, IPawn, new()
        where ControllerType : LocalPlayerController
    {
        protected SubClassOf<PawnType> _pawnClass;
        protected SubClassOf<ControllerType> _controllerClass;
        
        public SubClassOf<PawnType> PawnClass
        {
            get => _pawnClass;
            set => _pawnClass = value;
        }
        public SubClassOf<ControllerType> ControllerClass
        {
            get => _controllerClass;
            set => _controllerClass = value;
        }

        public int _numSpectators, _numPlayers, _numComputers;
        
        protected internal virtual void HandleLocalPlayerLeft(ControllerType item)
        {
            RenderPanel.GamePanel?.UnregisterController(item);
        }
        protected internal virtual void HandleLocalPlayerJoined(ControllerType item)
        {
            RenderPanel p = RenderPanel.GamePanel;
            if (p != null)
            {
                Viewport v = p.GetViewport((int)item.LocalPlayerIndex) ?? p.AddViewport();
                if (v != null)
                    v.RegisterController(item);
            }
            PawnType pawn = _pawnClass.CreateNew();
            if (item.ControlledPawn == null)
                item.ControlledPawn = pawn;
            else
                item.EnqueuePosession(pawn);
            Engine.World.SpawnActor(pawn);
        }
        protected internal override void CreateLocalController(LocalPlayerIndex index, Queue<IPawn> queue)
        {
            ControllerType t = ControllerClass.CreateNew(index, queue);
            Engine.ActivePlayers.Add(t);
            HandleLocalPlayerJoined(t);
        }
        protected internal override void CreateLocalController(LocalPlayerIndex index)
        {
            ControllerType t = ControllerClass.CreateNew(index);
            Engine.ActivePlayers.Add(t);
            HandleLocalPlayerJoined(t);
        }
    }
}
