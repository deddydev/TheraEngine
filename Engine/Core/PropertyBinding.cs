using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEngine.Core
{
    public class PropertyBinding : TObject
    {
        public PropertyBinding(
            INotifyPropertyChanged registree, INotifyPropertyChanged source,
            string registreePropertyName, string sourcePropertyName,  
            Func<object, object> oneWayMethod, bool toSource = false)
        {
            Mode = toSource ? EBindingMode.OneWayToSource : EBindingMode.OneWay;

            Registree = registree;
            Source = source;

            Converter = toSource ? 
                new TMethodPropertyConverter(null, oneWayMethod) : 
                new TMethodPropertyConverter(oneWayMethod, null);

            RegistreePropertyName = registreePropertyName;
            SourcePropertyName = sourcePropertyName;

            RegistreeProperty = Registree.GetType().GetProperty(nameof(RegistreePropertyName));
            SourceProperty = Source.GetType().GetProperty(nameof(SourcePropertyName));

            Mode = EBindingMode.OneWay;
        }

        public PropertyBinding() { }
        public PropertyBinding(
            INotifyPropertyChanged registree,
            INotifyPropertyChanged source,
            string registreePropertyName,
            string sourcePropertyName,
            EBindingMode mode = EBindingMode.OneWay,
            TPropertyConverter converter = null)
        {
            Mode = mode;

            Registree = registree;
            Source = source;

            Converter = converter;

            RegistreePropertyName = registreePropertyName;
            SourcePropertyName = sourcePropertyName;

            RegistreeProperty = Registree.GetType().GetProperty(nameof(RegistreePropertyName));
            SourceProperty = Source.GetType().GetProperty(nameof(SourcePropertyName));

            switch (mode)
            {
                case EBindingMode.OneTime:
                    TransferValue(
                        Registree, RegistreeProperty,
                        Source, SourceProperty,
                        Converter is null ? null : (Func<object, object>)Converter.ConvertFromSource);
                    break;
                case EBindingMode.OneWay:
                    Source.PropertyChanged += Source_PropertyChanged;
                    break;
                case EBindingMode.OneWayToSource:
                    Registree.PropertyChanged += Registree_PropertyChanged;
                    break;
                case EBindingMode.TwoWay:
                    Source.PropertyChanged += Source_PropertyChanged;
                    Registree.PropertyChanged += Registree_PropertyChanged;
                    break;
            }
        }

        [TSerialize]
        public INotifyPropertyChanged Registree { get; set; }
        [TSerialize]
        public INotifyPropertyChanged Source { get; set; }
        [TSerialize]
        public EBindingMode Mode { get; set; }
        [TSerialize]
        public string SourcePropertyName { get; set; }
        [TSerialize]
        public string RegistreePropertyName { get; set; }

        //TODO: Serialize these somehow
        public PropertyInfo RegistreeProperty { get; private set; }
        public PropertyInfo SourceProperty { get; private set; }
        public TPropertyConverter Converter { get; set; }

        private static void TransferValue(
            INotifyPropertyChanged from, PropertyInfo fromProp,
            INotifyPropertyChanged to, PropertyInfo toProp,
            Func<object, object> converter)
        {
            var obj = fromProp?.GetValue(from);

            if (converter != null)
                obj = converter(obj);

            toProp?.SetValue(to, obj);
        }

        private void Registree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, RegistreePropertyName, StringComparison.InvariantCultureIgnoreCase))
                TransferValue(
                    Registree, RegistreeProperty,
                    Source, SourceProperty, 
                    Converter is null ? null : (Func<object, object>)Converter.ConvertToSource);
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, SourcePropertyName, StringComparison.InvariantCultureIgnoreCase))
                TransferValue(
                    Source, SourceProperty,
                    Registree, RegistreeProperty,
                    Converter is null ? null : (Func<object, object>)Converter.ConvertFromSource);
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
