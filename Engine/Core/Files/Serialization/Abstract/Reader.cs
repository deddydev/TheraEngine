using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TDeserializer : TBaseSerializer
    {
        public abstract class AbstractReader : TBaseAbstractReaderWriter
        {
            protected readonly TDeserializer _owner;
            private List<(string Name, object Value)> _attributes;

            protected AbstractReader(TDeserializer owner, TFileObject rootFileObject, string filePath, IProgress<float> progress, CancellationToken cancel)
                : base(rootFileObject, filePath, progress, cancel)
            {
                _owner = owner;
            }
            
            public IReadOnlyList<(string Name, object Value)> Attributes => _attributes;
            public string ElementName { get; protected set; }
            public string ElementValue { get; protected set; }
            public string AttributeName { get; protected set; }
            public string AttributeValue { get; protected set; }
            public int ElementsDeep { get; private set; } = 0;
            
            public abstract void MoveBackToElementClose();

            protected abstract Task<string> OnReadElementStringAsync();
            public async Task<string> ReadElementStringAsync()
            {
                ElementValue = await OnReadElementStringAsync();
                ReportProgress();
                return ElementValue;
            }

            protected abstract Task<bool> OnBeginElementAsync();
            public async Task<bool> BeginElementAsync()
            {
                bool exists = await OnBeginElementAsync();
                if (exists)
                {
                    _attributes = new List<(string, object)>();
                    ReportProgress();
                }
                return exists;
            }

            protected abstract Task<bool> OnReadAttributeAsync();
            public async Task<bool> ReadAttributeAsync()
            {
                bool exists = await OnReadAttributeAsync();
                if (exists)
                    ReportProgress();
                return exists;
            }

            public async Task ReadAllAttributeAsync()
            {
                bool exists = false;
                while (exists = await ReadAttributeAsync())
                    _attributes.Add((AttributeName, AttributeValue));

            }
            protected abstract Task<bool> OnEndElementAsync();
            public async Task<bool> EndElementAsync()
            {
                bool valid = await OnEndElementAsync();
                if (valid)
                    ReportProgress();
                return valid;
            }

            public abstract Task ManualReadAsync(TFileObject o);

            public bool AttributeNameEquals(string name)
                => string.Equals(AttributeName, name, StringComparison.InvariantCulture);
            public bool ElementNameEquals(string name)
                => string.Equals(ElementName, name, StringComparison.InvariantCulture);

            public async Task<bool> ExpectAttribute(string name)
                => await ReadAttributeAsync() && AttributeNameEquals(name);
            public async Task<bool> ExpectElement(string name)
                => await BeginElementAsync() && ElementNameEquals(name);
        }
    }
}
