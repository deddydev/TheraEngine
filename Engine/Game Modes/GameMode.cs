using TheraEngine.Core.Files;
using TheraEngine.Input;
using TheraEngine.Actors;
using System;
using System.Collections.Generic;
using TheraEngine.Input.Devices;
using System.Linq;
using System.ComponentModel;
using TheraEngine.Worlds;

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
    public static class ClassCreator<T> where T : class
    {
        public static T New(params object[] args)
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), args);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            return default;
        }
        public static T2 New<T2>() where T2 : T, new()
        {
            try
            {
                return new T2();
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            return default;
        }
        public static T2 New<T2>(params object[] args) where T2 : T
        {
            try
            {
                return (T2)Activator.CreateInstance(typeof(T2), args);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            return default;
        }
    }
    public interface IGameMode
    {
        void BeginGameplay();
        void EndGameplay();
        void AbortGameplay();
    }
    public abstract class BaseGameMode : TFileObject, IGameMode
    {
        //Queue of what pawns should be possessed next for each player index when they either first join the game, or have their controlled pawn set to null.
        private static Dictionary<ELocalPlayerIndex, Queue<IPawn>> _possessionQueues = new Dictionary<ELocalPlayerIndex, Queue<IPawn>>();

        [TSerialize]
        private bool _disallowPausing = false;

        public virtual bool DisallowPausing
        {
            get => _disallowPausing;
            set => _disallowPausing = value;
        }
        public World TargetWorld { get; internal set; }
        public List<LocalPlayerController> LocalPlayers { get; } = new List<LocalPlayerController>();

        /// <summary>
        /// Creates a local player controller with methods and properties that may pertain to this specific game mode.
        /// </summary>
        /// <param name="index">The player that will provide input to the controller.</param>
        /// <returns>The new local player controller.</returns>
        protected internal abstract void CreateLocalController(ELocalPlayerIndex index);
        /// <summary>
        /// Creates a local player controller with methods and properties that may pertain to this specific game mode.
        /// </summary>
        /// <param name="index">The player that will provide input to the controller.</param>
        /// <param name="possessionQueue">The queue of pawns that want to be possessed by the controller.</param>
        /// <returns>The new local player controller.</returns>
        protected internal abstract void CreateLocalController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue);

        protected virtual void OnBeginGameplay() { }
        public void BeginGameplay()
        {
            Engine.PrintLine("Game mode {0} has begun play.", GetType().GetFriendlyName());
            CreateLocalPlayerControllers();
            OnBeginGameplay();
        }
        protected virtual void OnEndGameplay() { }
        public void EndGameplay()
        {
            DestroyLocalPlayerControllers();
            OnEndGameplay();
            Engine.PrintLine("Game mode {0} has ended play.", GetType().GetFriendlyName());
        }
        protected virtual void OnAbortGameplay() { }
        public void AbortGameplay()
        {
            OnAbortGameplay();
        }
        /// <summary>
        /// Enqueues a pawn to be possessed by the given local player as soon as its current controlled pawn is set to null.
        /// </summary>
        /// <param name="pawn">The pawn to possess.</param>
        /// <param name="possessor">The controller to possess the pawn.</param>
        public void QueuePossession(IPawn pawn, ELocalPlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < LocalPlayers.Count)
                LocalPlayers[index].EnqueuePosession(pawn);
            else if (_possessionQueues.ContainsKey(possessor))
                _possessionQueues[possessor].Enqueue(pawn);
            else
            {
                Queue<IPawn> queue = new Queue<IPawn>();
                queue.Enqueue(pawn);
                _possessionQueues.Add(possessor, queue);
            }
        }
        public void ForcePossession(IPawn pawn, ELocalPlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < LocalPlayers.Count)
                LocalPlayers[index].ControlledPawn = pawn;
        }

        private void ActivePlayers_Removed(LocalPlayerController item)
        {
            //ActiveGameMode?.HandleLocalPlayerLeft(item);

            //TODO: remove controller from the server
        }
        private void ActivePlayers_Added(LocalPlayerController item)
        {
            //ActiveGameMode?.HandleLocalPlayerJoined(item);

            //TODO: create controller on the server
        }
        internal void ResetLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in LocalPlayers)
                controller.UnlinkControlledPawn();
        }
        internal void DestroyLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in LocalPlayers)
                controller.Destroy();
            LocalPlayers.Clear();
        }
        private void CreateLocalPlayerControllers()
        {
            InputDevice[] gamepads = InputDevice.CurrentDevices[EInputDeviceType.Gamepad];
            InputDevice[] keyboards = InputDevice.CurrentDevices[EInputDeviceType.Keyboard];
            InputDevice[] mice = InputDevice.CurrentDevices[EInputDeviceType.Mouse];

            if (keyboards.Any(x => x != null) || mice.Any(x => x != null) || gamepads.Any(x => x != null && x.Index == 0))
                CreateLocalController(ELocalPlayerIndex.One);
            
            for (int i = 0; i < 4; ++i)
            {
                InputDevice gp = gamepads[i];
                if (gp != null && gp.Index > 0)
                    CreateLocalController(ELocalPlayerIndex.One + gp.Index);
            }
        }
        internal void FoundInput(InputDevice device)
        {
            if (device is BaseKeyboard || device is BaseMouse)
            {
                if (LocalPlayers.Count == 0)
                {
                    ELocalPlayerIndex index = ELocalPlayerIndex.One;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(index);
                    }
                    else
                        CreateLocalController(index);
                }
                else
                    LocalPlayers[0].Input.UpdateDevices();
            }
            else
            {
                if (device.Index >= LocalPlayers.Count)
                {
                    ELocalPlayerIndex index = (ELocalPlayerIndex)LocalPlayers.Count;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(index);
                    }
                    else
                        CreateLocalController(index);
                }
                else
                    LocalPlayers[device.Index].Input.UpdateDevices();
            }
        }
    }
    public class GameMode<PawnType, ControllerType> : BaseGameMode
        where PawnType : BaseActor, IPawn, new()
        where ControllerType : LocalPlayerController
    {
        public int _numSpectators, _numPlayers, _numComputers;
        
        protected internal virtual void HandleLocalPlayerLeft(ControllerType item)
        {
            item.Viewport.HUD = null;

            TargetWorld.RenderPanel.UnregisterController(item);

            TargetWorld.DespawnActor(item.ControlledPawn as BaseActor);
            item.UnlinkControlledPawn();
        }
        protected internal virtual void HandleLocalPlayerJoined(ControllerType item)
        {
            PawnType pawn = new PawnType();

            TargetWorld.RenderPanel.GetOrAddViewport(item.LocalPlayerIndex)?.RegisterController(item);
            
            item.EnqueuePosession(pawn);
            TargetWorld.SpawnActor(pawn);
        }
        protected internal override void CreateLocalController(ELocalPlayerIndex index, Queue<IPawn> queue)
            => InitController(ClassCreator<ControllerType>.New(index, queue));
        protected internal override void CreateLocalController(ELocalPlayerIndex index)
            => InitController(ClassCreator<ControllerType>.New(index));
        private void InitController(ControllerType t)
        {
            LocalPlayers.Add(t);
            HandleLocalPlayerJoined(t);
        }
    }
}
