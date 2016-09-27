﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.World.Actors.Components
{
    public enum MovementMode
    {
        Walking,
        Falling,
        Swimming,
        Flying,
    }
    public class CharacterMovementComponent : MovementComponent
    {
        private MovementMode _currentMovementMode;
        private bool _isCrouched;

        public void Jump()
        {

        }
        public void ToggleCrouch()
        {

        }
        public void SetCrouched()
        {

        }
    }
}
