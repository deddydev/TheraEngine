﻿using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.Bullet
{
    public interface IBulletConstraint
    {
        TypedConstraint Constraint { get; }
    }
}
