namespace TheraEngine.Components.Logic.Interaction
{
    /// <summary>
    /// Specifies that this actor can be interacted with.
    /// </summary>
    public class InteractableComponent : LogicComponent
    {
        public struct Nothing
        {
            public int Whatever;

            public int WhateverProp
            {
                get => Whatever;
                set => Whatever = value;
            }
        }
        public Nothing Thing { get; set; }
    }
}