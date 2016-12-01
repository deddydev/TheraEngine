using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLMultiInput : BaseGLArgument
    {
        protected List<BaseGLMultiInput> _syncedArgs = new List<BaseGLMultiInput>();
        protected GLTypeName _currentArgType = GLTypeName.Invalid;
        internal GLTypeName[] _allowedArgTypes;
        
        public BaseGLMultiInput(string name, params GLTypeName[] types) : base(name)
        {
            _allowedArgTypes = types;
        }
        public BaseGLMultiInput(string name, BaseGLMultiInput linkedMultiArg) : base(name)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg._allowedArgTypes;
        }

        public void SetSyncedArguments(params BaseGLMultiInput[] args) { _syncedArgs.AddRange(args); }

        public GLTypeName CurrentArgumentType
        {
            get { return _currentArgType; }
            set
            {
                if (_allowedArgTypes.Contains(value))
                    _currentArgType = value;
                else
                    throw new InvalidOperationException();
            }
        }

        public override bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null || other.IsOutput == IsOutput)
                return false;

            GLTypeName otherType = other.GetArgType();

            //Edge case: the other node is just invalid
            if (otherType == GLTypeName.Invalid && !(other is BaseGLMultiInput))
                return false;

            GLTypeName thisType = GetArgType();
            if (thisType != GLTypeName.Invalid)
            {
                if (otherType != GLTypeName.Invalid)
                    return thisType == otherType;

                //Has to be a GLMultiArgument as per the edge case check above
                BaseGLMultiInput otherMultiArg = (BaseGLMultiInput)other;

                return otherMultiArg._allowedArgTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType != GLTypeName.Invalid)
                    return _allowedArgTypes.Contains(otherType);

                //Has to be a GLMultiArgument as per the edge case check above
                BaseGLMultiInput otherMultiArg = (BaseGLMultiInput)other;

                //Returns true if there are any matching allowed types between the two
                return _allowedArgTypes.Intersect(otherMultiArg._allowedArgTypes).ToArray().Length != 0;
            }
        }

        public override GLTypeName GetArgType() { return CurrentArgumentType; }
    }
}
