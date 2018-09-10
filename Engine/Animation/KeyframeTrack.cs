using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using System.IO;
using System.Xml;
using System.Linq;
using EnumsNET;

namespace TheraEngine.Animation
{
    public interface IKeyframe
    {
        float Second { get; set; }
    }
    public interface IPlanarKeyframe : IKeyframe
    {
        object InValue { get; set; }
        object OutValue { get; set; }
        object InTangent { get; set; }
        object OutTangent { get; set; }
        EPlanarInterpType InterpolationType { get; set; }

        void AverageKeyframe();
        void AverageValues();
        void AverageTangents();
        void MakeOutLinear();
        void MakeInLinear();
        void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent);
        void WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent);
    }
    public interface IPlanarKeyframe<T> : IPlanarKeyframe where T : unmanaged
    {
        new T InValue { get; set; }
        new T OutValue { get; set; }
        new T InTangent { get; set; }
        new T OutTangent { get; set; }
    }
    public interface IRadialKeyframe
    {

    }
    public interface IStepKeyframe
    {

    }
    public abstract class BaseKeyframeTrack : TFileObject
    {
        public event Action Changed;
        public event DelFloatChange LengthChanged;

        protected internal void OnChanged() => Changed?.Invoke();
        
        protected internal abstract Keyframe FirstKey { get; internal set; }
        protected internal abstract Keyframe LastKey { get; internal set; }
        
        private float _lengthInSeconds = 0.0f;

        [Browsable(false)]
        public int Count { get; internal set; } = 0;
        [Browsable(false)]
        public float LengthInSeconds
        {
            get => _lengthInSeconds;
            set => SetLength(value, false);
        }
        public void SetLength(float seconds, bool stretch)
        {
            float prevLength = LengthInSeconds;
            float ratio = seconds / LengthInSeconds;
            _lengthInSeconds = seconds;
            if (stretch)
            {
                Keyframe key = FirstKey;
                while (key != null)
                {
                    key.Second *= ratio;
                    key = key.Next;
                }
            }
            else
            {
                //Keyframe key = FirstKey;
                //while (key != null)
                //{
                //    if (key.Second < 0 || key.Second > LengthInSeconds)
                //        key.Remove();
                //    if (key.Next == FirstKey)
                //        break;
                //    key = key.Next;
                //}
            }
            LengthChanged?.Invoke(prevLength, LengthInSeconds);
            OnChanged();
        }

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation)
            => SetLength(numFrames / framesPerSecond, stretchAnimation);
    }
    [FileExt("kf", ManualXmlConfigSerialize = true)]
    [FileDef("Keyframe Track")]
    public class KeyframeTrack<T> : BaseKeyframeTrack, IList, IList<T>, IEnumerable<T> where T : Keyframe, new()
    {
        private T _first = null;
        private T _last = null;

        [Browsable(false)]
        public T First
        {
            get => _first;
            private set
            {
                _first = value;
                if (_first != null)
                    _first.OwningTrack = this;
            }
        }
        [Browsable(false)]
        public T Last
        {
            get => _last;
            private set
            {
                _last = value;
                if (_last != null)
                    _last.OwningTrack = this;
            }
        }

        protected internal override Keyframe FirstKey
        {
            get => First;
            internal set => First = value as T;
        }
        protected internal override Keyframe LastKey
        {
            get => Last;
            internal set => Last = value as T;
        }

        [Browsable(false)]
        public bool IsReadOnly => false;
        [Browsable(false)]
        public bool IsFixedSize => false;
        //public int Count => base.Count;
        [Browsable(false)]
        public object SyncRoot { get; } = null;
        [Browsable(false)]
        public bool IsSynchronized { get; } = false;

        T IList<T>.this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                            return key;
                        ++i;
                    }
                }
                return null;
            }
            set
            {
                if (index >= 0 && index <= Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(value);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                            return key;
                        ++i;
                    }
                }
                return null;
            }
            set
            {
                if (value is T keyValue && index >= 0 && index <= Count)
                {
                    int i = 0;
                    foreach (T key in this)
                    {
                        if (i == index)
                        {
                            Keyframe prev = key.Prev;
                            key.Remove();
                            prev.Link(keyValue);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        public void Add(IEnumerable<T> keys)
        {
            keys.ForEach(x => Add(x));
        }
        public void Add(params T[] keys)
        {
            keys.ForEach(x => Add(x));
        }
        public void Add(T key)
        {
            if (key == null)
                return;

            //Reset key location before adding
            key.Remove();

            if (First == null)
            {
                Count = 1;
                First = key;
                Last = key;
                OnChanged();
            }
            else if (key.Second < First.Second)
            {
                T temp = First;
                First = key;
                temp.Link(key);
            }
            else if (key.Second > Last.Second)
            {
                T temp = Last;
                Last = key;
                temp.Link(key);
            }
            else
                First.Link(key);
        }
        public void RemoveLast()
        {
            if (Last == null)
                return;

            if (First == Last)
            {
                First = null;
                Last = null;
                --Count;
                OnChanged();
            }
            else
            {
                Keyframe temp = Last;
                Last = (T)Last.Prev;
                temp.Remove();
            }
        }
        public void RemoveFirst()
        {
            if (First == null)
                return;

            if (First == Last)
            {
                First = null;
                Last = null;
                --Count;
                OnChanged();
            }
            else
            {
                Keyframe temp = First;
                First = (T)First.Next;
                temp.Remove();
            }
        }
        public T GetKeyBefore(float second)
        {
            foreach (T key in this)
                if (second >= key.Second)
                    return key;
            return null;
        }
        public IEnumerator GetEnumerator()
        {
            Keyframe node = First;
            do
            {
                if (node == null)
                    break;
                yield return node;
                node = node.Next;
            }
            while (node != null);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Keyframe node = First;
            do
            {
                if (node == null)
                    break;
                yield return (T)node;
                node = node.Next;
            }
            while (node != null);
        }
        
        public int Add(object value)
        {
            if (value is T key)
            {
                Add(key);
                return 0;//key.TrackIndex;
            }
            return -1;
        }

        public bool Contains(object value)
        {
            if (value is T keyValue)
                foreach (T key in this)
                    if (key == keyValue)
                        return true;
            return false;
        }

        public void Clear()
        {
            _first = null;
            _last = null;
            Count = 0;
        }

        public int IndexOf(object value)
        {
            int i = 0;
            if (value is T keyValue)
                foreach (T key in this)
                {
                    if (key == keyValue)
                        return i;
                    ++i;
                }

            return -1;
        }

        public void Insert(int index, object value)
        {

        }

        public void Remove(object value)
        {

        }

        public void RemoveAt(int index)
        {

        }

        public void CopyTo(Array array, int index)
        {

        }

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
            
        }

        public bool Contains(T item)
        {
            return false;
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {

        }

        public bool Remove(T item)
        {
            if (item.OwningTrack == this)
            {
                item.Remove();
                return true;
            }
            return false;
        }

        //TODO: write keyframe append method
        public void Append(KeyframeTrack<T> keyframes)
        {
            foreach (Keyframe k in keyframes)
            {

            }
        }

        protected internal override void Read(XMLReader reader)
        {
            if (string.Equals(reader.Name, "KeyframeTrack", StringComparison.InvariantCulture))
            {
                if (reader.ReadAttribute() && string.Equals(reader.Name, nameof(LengthInSeconds), StringComparison.InvariantCulture))
                    LengthInSeconds = float.TryParse(reader.Value, out float length) ? length : 0.0f;

                Clear();

                if (reader.ReadAttribute() &&
                    string.Equals(reader.Name, "Count", StringComparison.InvariantCulture) &&
                    !int.TryParse(reader.Value, out int keyCount))
                {
                    Type t = typeof(T);
                    if (typeof(IPlanarKeyframe).IsAssignableFrom(t))
                    {
                        string[] seconds = null, inValues = null, outValues = null, inTans = null, outTans = null, interpolation = null;
                        while (reader.BeginElement())
                        {
                            switch (reader.Name)
                            {
                                case "Second":
                                    seconds = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                                case "InValues":
                                    inValues = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                                case "OutValues":
                                    outValues = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                                case "InTangents":
                                    inTans = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                                case "OutTangents":
                                    outTans = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                                case "Interpolation":
                                    interpolation = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    break;
                            }
                            reader.EndElement();
                        }
                        for (int i = 0; i < keyCount; ++i)
                        {
                            T kf = new T { Second = float.Parse(seconds[i]) };
                            IPlanarKeyframe kfp = (IPlanarKeyframe)kf;
                            kfp.InterpolationType = Enums.Parse<EPlanarInterpType>(interpolation[i]);
                            kfp.ParsePlanar(inValues[i], outValues[i], inTans[i], outTans[i]);
                            Add(kf);
                        }
                    }
                }
            }
            else
            {
                LengthInSeconds = 0;
            }
        }
        protected internal override void Write(XmlWriter writer, ESerializeFlags flags)
        {
            writer.WriteStartElement("KeyframeTrack");
            {
                writer.WriteAttributeString(nameof(LengthInSeconds), LengthInSeconds.ToString());
                writer.WriteAttributeString("Count", Count.ToString());
                if (Count > 0)
                {
                    Type t = typeof(T);
                    if (typeof(IPlanarKeyframe).IsAssignableFrom(t))
                    {
                        string seconds = "", inval = "", outval = "", intan = "", outtan = "", interp = "";
                        bool first = true;
                        foreach (IPlanarKeyframe kf in this)
                        {
                            if (first)
                                first = false;
                            else
                            {
                                seconds += ",";
                                inval += ",";
                                outval += ",";
                                intan += ",";
                                outtan += ",";
                                interp += ",";
                            }
                            seconds += kf.Second;
                            interp += kf.InterpolationType;
                            kf.WritePlanar(out inval, out outval, out intan, out outtan);
                        }
                        writer.WriteElementString("Second", seconds);
                        writer.WriteElementString("InValues", inval);
                        writer.WriteElementString("OutValues", outval);
                        writer.WriteElementString("InTangents", intan);
                        writer.WriteElementString("OutTangents", outtan);
                        writer.WriteElementString("Interpolation", interp);
                    }
                }
            }
            writer.WriteEndElement();
        }
    }
    public enum ERadialInterpType
    {
        Step,
        Linear,
        CubicBezier
    }
    public enum EPlanarInterpType
    {
        Step,
        Linear,
        CubicHermite,
        CubicBezier
    }
    public class InterpPlanarKeyframe<T> : Keyframe where T : unmanaged
    {
        public delegate T DelInterpPlanar(InterpPlanarKeyframe<T> key1, InterpPlanarKeyframe<T> key2, float time);
        public override void ReadFromString(string str)
        {
            throw new NotImplementedException();
        }
        public override string WriteToString()
        {
            throw new NotImplementedException();
        }
    }
    public abstract class Keyframe : IParsable
    {
        [TSerialize(nameof(Second), XmlNodeType = EXmlNodeType.Attribute)]
        private float _second;
        
        protected Keyframe _next;
        protected Keyframe _prev;

        public Keyframe()
        {
            _next = null;
            _prev = null;
            OwningTrack = null;
        }

        [Category("Keyframe")]
        public float Second
        {
            get => _second;
            set
            {
                _second = value.ClampMin(0.0f);
                if (float.IsNaN(_second))
                    _second = 0.0f;
                if (Prev != null)
                    Prev.Relink(this);
            }
        }

        [Browsable(false)]
        public Keyframe Next => _next;
        [Browsable(false)]
        public Keyframe Prev => _prev;
        [Browsable(false)]
        [Category("Keyframe")]
        public bool IsFirst => Prev == null;
        [Browsable(false)]
        [Category("Keyframe")]
        public bool IsLast => Next == null;
        [Browsable(false)]
        public BaseKeyframeTrack OwningTrack
        {
            get => _owningTrack;
            internal set
            {
                _owningTrack = value;
                if (Next != null && Next.OwningTrack != _owningTrack)
                    Next.OwningTrack = _owningTrack;
                if (Prev != null && Prev.OwningTrack != _owningTrack)
                    Prev.OwningTrack = _owningTrack;
            }
        }
        private BaseKeyframeTrack _owningTrack = null;
        private void Relink(Keyframe key)
        {
            if (key == null || key == this)
                return;

            //not null, greater than next, and next is not before this
            if (_next != null && key.Second > _next.Second && _next.Second > Second)
            {
                _next.Relink(key);
                return;
            }
            //not null, less than this, and prev is not after this
            if (_prev != null && key.Second < Second && _prev.Second < Second)
            {
                _prev.Relink(key);
                return;
            }

            if (_next == key)
                return;

            //resize track length if second is outside of range
            //if (key.Second > OwningTrack.LengthInSeconds)
            //    OwningTrack.LengthInSeconds = key.Second;

            //Set key's next and prev
            key._next = _next;
            key._prev = this;

            //update next key
            if (_next != null)
                _next._prev = key;
            _next = key;

            //update track's first and last references
            if (key.Next != null && key.Next.IsFirst && key.Second < key.Next.Second)
                key.Next.OwningTrack.FirstKey = key;
            if (key.Prev != null && key.Prev.IsLast && key.Second > key.Prev.Second)
                key.Prev.OwningTrack.LastKey = key;

            if (!key.IsFirst)
                key.OwningTrack = key.Prev.OwningTrack;
            else if (!key.IsLast)
                key.OwningTrack = key.Next.OwningTrack;
            
            OwningTrack?.OnChanged();
        }

        public Keyframe Link(Keyframe key)
        {
            if (OwningTrack != null)
                ++OwningTrack.Count;
            Relink(key);
            return key;
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);
        public override string ToString() => WriteToString();

        public void Remove()
        {
            if (_next != null)
                _next._prev = Prev;
            if (_prev != null)
                _prev._next = Next;

            if (OwningTrack != null)
            {
                if (IsFirst)
                    OwningTrack.FirstKey = Next;
                if (IsLast)
                    OwningTrack.LastKey = Prev;

                --OwningTrack.Count;
                OwningTrack.OnChanged();
            }

            _next = _prev = null;
            OwningTrack = null;
        }

        public Keyframe GetFirstInSequence()
        {
            Keyframe temp = this;
            while (temp.Prev != null)
                temp = temp.Prev;
            return temp;
        }
        public Keyframe GetLastInSequence()
        {
            Keyframe temp = this;
            while (temp.Next != null)
                temp = temp.Next;
            return temp;
        }
        public int GetCountInSequence()
        {
            int count = 1;
            Keyframe temp = Prev;
            while (temp != null)
            {
                ++count;
                temp = temp.Prev;
            }
            temp = Next;
            while (temp != null)
            {
                ++count;
                temp = temp.Next;
            }
            return count;
        }
        public int GetSequence(out Keyframe first, out Keyframe last)
        {
            int count = 1;
            first = this;
            last = this;
            Keyframe temp = Prev;
            while (temp != null)
            {
                ++count;
                first = temp;
                temp = temp.Prev;
            }
            temp = Next;
            while (temp != null)
            {
                ++count;
                last = temp;
                temp = temp.Next;
            }
            return count;
        }
    }
}
