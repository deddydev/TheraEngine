using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IGLMultiArgument
    {
        List<IGLMultiArgument> SyncedArguments { get; }
        GLTypeName[] AllowedArgumentTypes { get; }
        GLTypeName CurrentArgumentType { get; }
    }
    public class GLMultiInput : BaseGLOutput
    {
        public List<IGLMultiArgument> SyncedArguments { get { return _syncedArgs; } }
        public GLTypeName[] AllowedArgumentTypes { get { return _allowedArgTypes; } }
        public GLTypeName CurrentArgumentType { get { return _currentArgType; } }

        private List<IGLMultiArgument> _syncedArgs = new List<IGLMultiArgument>();
        private GLTypeName[] _allowedArgTypes = null;
        private GLTypeName _currentArgType = GLTypeName.Invalid;

        public GLMultiInput(string name, params GLTypeName[] types) : base(name)
        {
            _allowedArgTypes = types;
        }
        public GLMultiInput(string name, MaterialFunction parent, params GLTypeName[] types) : base(name, parent)
        {
            _allowedArgTypes = types;
        }
        public GLMultiInput(string name, IGLMultiArgument linkedMultiArg) : base(name)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }
        public GLMultiInput(string name, MaterialFunction parent, IGLMultiArgument linkedMultiArg) : base(name, parent)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }
        public void SetSyncedArguments(params IGLMultiArgument[] args)
        {
            _syncedArgs = args.ToList();
        }
        public override GLTypeName GetArgType() { return CurrentArgumentType; }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null || other.IsOutput == IsOutput)
                return false;

            GLTypeName otherType = other.GetArgType();

            //Edge case: the other node is just invalid
            if (otherType == GLTypeName.Invalid && !(other is GLMultiInput))
                return false;

            GLTypeName thisType = GetArgType();
            if (thisType != GLTypeName.Invalid)
            {
                if (otherType != GLTypeName.Invalid)
                    return thisType == otherType;

                //Has to be a GLMultiArgument as per the edge case check above
                GLMultiInput otherMultiArg = (GLMultiInput)other;

                return otherMultiArg.AllowedArgumentTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType != GLTypeName.Invalid)
                    return _allowedArgTypes.Contains(otherType);

                //Has to be a GLMultiArgument as per the edge case check above
                GLMultiInput otherMultiArg = (GLMultiInput)other;

                //Returns true if there are any matching allowed types between the two
                return _allowedArgTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
