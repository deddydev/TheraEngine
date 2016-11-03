using CustomEngine;
using CustomEngine.Collision;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Worlds.Actors.Components.Scene
{
    public enum RequestedHand
    {
        LeftOnly,
	    LeftPreferred,
	    RightOnly,
	    RightPreferred,
	    DominantOnly,
	    DominantPreferred,
	    NonDominantOnly,
	    NonDominantPreferred,
    }
    public class InteractionZoneComponent : ShapeComponent<Box>
    {
        public InteractionZoneComponent()
        {

        }

#if EDITOR
        public override bool IsDirty
        {
            get { return base.IsDirty; }
            set { base.IsDirty = value; }
        }
#endif

        protected List<GameCharacter> _overlapping;
        protected List<GameCharacter> _viewers;
        protected List<GameCharacter> _pendingInteraction;

        protected bool _lookAtInteraction;
        protected bool _viewMustBeInProximity;
        protected bool _actorMustBeInProximity;
        protected bool _interactOnBeginOverlap;
        protected bool _interactOnEndOverlap;
        protected bool _requiresAvailableHand;
        protected RequestedHand _beginOverlapHand;
        protected RequestedHand _endOverlapHand;
        protected string _interactionMessage;

        public bool PendingAnyInteraction { get { return _pendingInteraction.Count > 0; } }

        public void OnViewed(GameCharacter instigator, HitInfo hit)
        {

        }
        public void OnLeft(GameCharacter instigator)
        {

        }
        public void ButtonDown(GameCharacter instigator)
        {

        }
        public void ButtonUp(GameCharacter instigator)
        {
            
        }
    }
}
