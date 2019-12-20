using System;
using System.ComponentModel;

namespace TheraEngine.Core
{
    public class PropertyBinding
    {
        public PropertyBinding() { }
        public PropertyBinding(
            TObject registree,
            TObject source,
            string sourcePropertyName,
            EBindingMode mode = EBindingMode.OneWay,
            TPropertyConverter converter = null,
            bool convertInverse = false)
        {
            Registree = registree;
            Source = source;
            Mode = mode;
            Converter = converter;
            ConvertInverse = convertInverse;
            SourcePropertyName = sourcePropertyName;
        }

        public TObject Registree { get; set; }
        public TObject Source { get; set; }
        public TPropertyConverter Converter { get; set; }
        public bool ConvertInverse { get; set; } //Applies to mode for 'to source' and converter to and back
        public EBindingMode Mode { get; set; }
        public string SourcePropertyName { get; set; }

        public object Get()
        {
            var sourceObj = Source.GetType().GetProperty(SourcePropertyName).GetValue(Source);

            if (Converter != null)
                sourceObj = Converter.ConvertFromSource(sourceObj);

            return sourceObj;
        }
        public void Set<T>(T newValue)
        {
            object sourceObj = newValue;

            if (Converter != null)
                sourceObj = Converter.ConvertToSource(sourceObj);

            Source.GetType().GetProperty(SourcePropertyName).SetValue(Source, sourceObj);
        }
    }

    public enum EBindingMode
    {
        /// <summary>
        /// Value is grabbed once on initialization.
        /// </summary>
        OneTime,
        /// <summary>
        /// Value is pulled from source but not pushed back.
        /// </summary>
        OneWay,
        /// <summary>
        /// Value is only set to source.
        /// </summary>
        OneWayToSource,
        /// <summary>
        /// Value is synchronized with source value if set on either side.
        /// </summary>
        TwoWay,
    }
}
