using TheraEngine.Core.Files;
using TheraEngine.Input;
using TheraEngine.Actors;
using System;
using System.Collections.Generic;
using TheraEngine.Input.Devices;
using System.Linq;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering;
using System.Collections.Specialized;

namespace TheraEngine.GameModes
{
    public enum EInheritableBool
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
    public class ClassCreator<T> where T : class
    {
        public T New(params object[] args)
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
        public T2 New<T2>() where T2 : T, new()
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
        public T2 New<T2>(params object[] args) where T2 : T
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
    public interface IGameMode : IFileObject
    {
        bool IsPlaying { get; }
        bool DisallowPausing { get; set; }
        IEventList<BaseRenderPanel> TargetRenderPanels { get; }
        IWorld TargetWorld { get; }
        IEventList<LocalPlayerController> LocalPlayers { get; }

        Viewport LinkControllerToViewport(LocalPlayerController item);

        void BeginGameplay(World world);
        void EndGameplay();
        void AbortGameplay();

        void QueuePossession(IPawn pawn, ELocalPlayerIndex possessor);
        void QueuePossession(IPawn pawn, Queue<ELocalPlayerIndex> possessors);
        void ForcePossession(IPawn pawn, ELocalPlayerIndex possessor);

        void ResetLocalPlayerControllers();
        void DestroyLocalPlayerControllers();
        void FoundInput(InputDevice device);
    }
    [TFileExt("gm")]
    [TFileDef("Game Mode", "")]
    public abstract class BaseGameMode : TFileObject, IGameMode
    {
        public BaseGameMode()
        {
            _targetRenderPanels = new EventList<BaseRenderPanel>();
            _targetRenderPanels.CollectionChanged += _targetRenderPanels_CollectionChanged;
        }

        private void _targetRenderPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsPlaying)
                foreach (var player in LocalPlayers)
                    LinkControllerToViewport(player);
        }

        public virtual Viewport LinkControllerToViewport(LocalPlayerController item)
        {
            if (item == null)
                return null;

            item.Viewport?.UnregisterController(item);

            var panel = TargetRenderPanels.FirstOrDefault(x => x.ValidPlayerIndices.Contains(item.LocalPlayerIndex));
            if (panel != null)
            {
                Viewport v = panel.GetOrAddViewport(item.LocalPlayerIndex);
                v?.RegisterController(item);
                return v;
            }
            else
                Engine.LogWarning($"No target render panel found for player {item.LocalPlayerIndex.ToString().ToLowerInvariant()}.");

            return null;
        }

        //Queue of what pawns should be possessed next for each player index when they either first join the game, or have their controlled pawn set to null.
        private static Dictionary<ELocalPlayerIndex, Queue<IPawn>> _possessionQueues = new Dictionary<ELocalPlayerIndex, Queue<IPawn>>();

        [TSerialize]
        private bool _disallowPausing = false;

        public virtual bool DisallowPausing
        {
            get => _disallowPausing;
            set => _disallowPausing = value;
        }

        public bool IsPlaying { get; private set; }

        private IEventList<BaseRenderPanel> _targetRenderPanels;
        /// <summary>
        /// These are the render panels that will be used for the target world's local player controllers.
        /// </summary>
        [Browsable(false)]
        public IEventList<BaseRenderPanel> TargetRenderPanels => _targetRenderPanels;
        /// <summary>
        /// This is the world that is running this game mode.
        /// </summary>
        [Browsable(false)]
        public IWorld TargetWorld { get; private set; }
        /// <summary>
        /// These are the local players that are active in the world.
        /// </summary>
        [Browsable(false)]
        public IEventList<LocalPlayerController> LocalPlayers { get; } = new EventList<LocalPlayerController>();

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
        public void BeginGameplay(World world)
        {
            TargetWorld = world;
            Engine.PrintLine("Game mode {0} has begun play.", GetType().GetFriendlyName());
            CreateLocalPlayerControllers();
            OnBeginGameplay();
            IsPlaying = true;
        }
        protected virtual void OnEndGameplay() { }
        public void EndGameplay()
        {
            IsPlaying = false;
            DestroyLocalPlayerControllers();
            OnEndGameplay();
            Engine.PrintLine("Game mode {0} has ended play.", GetType().GetFriendlyName());
            TargetWorld = null;
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

        //public Queue<ELocalPlayerIndex> CollectPossessionQueueFor(IPawn pawn)
        //{
        //    Queue<ELocalPlayerIndex> indices = new Queue<ELocalPlayerIndex>();
            
        //}

        /// <summary>
        /// Enqueues a pawn to be possessed by the given local player as soon as its current controlled pawn is set to null.
        /// </summary>
        /// <param name="pawn">The pawn to possess.</param>
        /// <param name="possessor">The controller to possess the pawn.</param>
        public void QueuePossession(IPawn pawn, Queue<ELocalPlayerIndex> possessors)
        {
            while (possessors.Count > 0)
                QueuePossession(pawn, possessors.Dequeue());
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
        public void ResetLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in LocalPlayers)
                controller.UnlinkControlledPawn();
        }
        public void DestroyLocalPlayerControllers()
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
        public void FoundInput(InputDevice device)
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
        where PawnType : class, IActor, IPawn, new()
        where ControllerType : LocalPlayerController
    {
        public int _numSpectators, _numPlayers, _numComputers;
        private ClassCreator<PawnType> _pawnCreator = new ClassCreator<PawnType>();
        private ClassCreator<ControllerType> _controllerCreator = new ClassCreator<ControllerType>();

        public ClassCreator<PawnType> PawnSubClassCreator
        {
            get => _pawnCreator;
            set => _pawnCreator = value ?? new ClassCreator<PawnType>();
        }
        public ClassCreator<ControllerType> ControllerSubClassCreator
        {
            get => _controllerCreator;
            set => _controllerCreator = value ?? new ClassCreator<ControllerType>();
        }
        
        protected internal virtual void HandleLocalPlayerLeft(ControllerType item)
        {
            item.Viewport.HUD = null;
            item.Viewport.OwningPanel.UnregisterController(item);

            TargetWorld.DespawnActor(item.ControlledPawn as BaseActor);
            item.UnlinkControlledPawn();
        }
        protected internal virtual void HandleLocalPlayerJoined(ControllerType item)
        {
            LinkControllerToViewport(item);

            PawnType pawn = PawnSubClassCreator.New();
            item.EnqueuePosession(pawn);
            TargetWorld.SpawnActor(pawn);
        }
        protected internal override void CreateLocalController(ELocalPlayerIndex index, Queue<IPawn> queue)
            => InitController(ControllerSubClassCreator.New(index, queue));
        protected internal override void CreateLocalController(ELocalPlayerIndex index)
            => InitController(ControllerSubClassCreator.New(index));
        private void InitController(ControllerType t)
        {
            LocalPlayers.Add(t);
            HandleLocalPlayerJoined(t);
        }
    }
}
