using TheraEngine;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;

namespace Thera.Worlds.Actors.Components.Scene
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
    public class InteractionZoneComponent : BoxComponent
    {
        public InteractionZoneComponent(Vec3 extents) : base(extents, null)
        {

        }

#if EDITOR
        public override bool IsDirty
        {
            get { return base.IsDirty; }
            set { base.IsDirty = value; }
        }
#endif

        protected List<TheraCharacter> _overlapping;
        protected List<TheraCharacter> _viewers;
        protected List<TheraCharacter> _pendingInteraction;

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

        public void OnViewed(TheraCharacter instigator, HitInfo hit)
        {

        }
        public void OnLeft(TheraCharacter instigator)
        {

        }
        public void ButtonDown(TheraCharacter instigator)
        {

        }
        public void ButtonUp(TheraCharacter instigator)
        {
            
        }
    }
}
