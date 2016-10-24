using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class MaterialFunction : IGLVarOwner
    {
        protected string _operation;
        private List<string> _keywords = new List<string>();
        private List<GLVar> 
            _inputs = new List<GLVar>(), 
            _outputs = new List<GLVar>();

        public List<GLVar> InputArguments { get { return _inputs; } }
        public List<GLVar> OutputArguments { get { return _outputs; } }
        /// <summary>
        /// Used when searching for commands.
        /// </summary>
        public ReadOnlyCollection<string> Keywords { get { return _keywords.AsReadOnly(); } }

        public MaterialFunction()
        {
            _keywords = GetKeywords();
            AddInput(GetInputArguments());
            AddOutput(GetOutputArguments());
            _operation = GetOperation();
        }

        protected virtual List<GLVar> GetInputArguments() { return null; }
        protected virtual List<GLVar> GetOutputArguments() { return null; }
        protected abstract List<string> GetKeywords();
        protected abstract string GetOperation();

        protected void AddInput(List<GLVar> input)
        {
            if (input != null)
                foreach (GLVar v in input)
                    AddInput(v);
        }
        protected void AddInput(GLVar input)
        {
            input.Setup(false, this);
            _inputs.Add(input);
        }
        protected void AddOutput(List<GLVar> output)
        {
            if (output != null)
                foreach (GLVar v in output)
                    AddOutput(v);
        }
        protected void AddOutput(GLVar output)
        {
            output.Setup(true, this);
            _outputs.Add(output);
        }
    }
}
