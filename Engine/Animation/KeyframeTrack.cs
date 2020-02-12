using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Animation
{
    public interface IKeyframe
    {
        Type ValueType { get; }
        float Second { get; set; }
    }
    public interface IPlanarKeyframe : IKeyframe
    {
        object InValue { get; set; }
        object OutValue { get; set; }
        object InTangent { get; set; }
        object OutTangent { get; set; }
        EVectorInterpType InterpolationType { get; set; }

        void UnifyKeyframe(EUnifyBias bias);
        void UnifyValues(EUnifyBias bias);
        void UnifyTangents(EUnifyBias bias);
        void UnifyTangentDirections(EUnifyBias bias);
        void UnifyTangentMagnitudes(EUnifyBias bias);
        void MakeOutLinear();
        void MakeInLinear();
        //void ParsePlanar(string inValue, string outValue, string inTangent, string outTangent);
        //void WritePlanar(out string inValue, out string outValue, out string inTangent, out string outTangent);
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
    public delegate void DelLengthChange(float oldValue, BaseKeyframeTrack track);
    public abstract class BaseKeyframeTrack : TFileObject, IEnumerable<Keyframe>
    {
        public event Action<BaseKeyframeTrack> Changed;
        public event DelLengthChange LengthChanged;

        protected internal void OnChanged() => Changed?.Invoke(this);
        protected internal void OnLengthChanged(float prevLength) => LengthChanged?.Invoke(prevLength, this);

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
        public void SetLength(float seconds, bool stretch, bool notifyLengthChanged = true, bool notifyChanged = true)
        {
            float prevLength = LengthInSeconds;
            _lengthInSeconds = seconds;
            if (stretch && prevLength > 0.0f)
            {
                float ratio = seconds / prevLength;
                Keyframe key = FirstKey;
                while (key != null)
                {
                    key.Second *= ratio;
                    key = key.Next;
                }
            }
            //else
            //{
            //    //Keyframe key = FirstKey;
            //    //while (key != null)
            //    //{
            //    //    if (key.Second < 0 || key.Second > LengthInSeconds)
            //    //        key.Remove();
            //    //    if (key.Next == FirstKey)
            //    //        break;
            //    //    key = key.Next;
            //    //}
            //}
            if (notifyChanged)
            {
                OnLengthChanged(prevLength);
                OnChanged();
            }
        }

        public void SetFrameCount(int numFrames, float framesPerSecond, bool stretchAnimation, bool notifyLengthChanged = true, bool notifyChanged = true)
            => SetLength(numFrames / framesPerSecond, stretchAnimation, notifyLengthChanged, notifyChanged);

        public Keyframe GetKeyBeforeGeneric(float second)
        {
            Keyframe bestKey = null;
            foreach (Keyframe key in this)
                if (second >= key.Second)
                    bestKey = key;
                else
                    break;
            return bestKey;
        }

        public abstract IEnumerator<Keyframe> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    [TFileExt("kf", ManualXmlConfigSerialize = true, ManualXmlStateSerialize = true, ManualBinConfigSerialize = true, ManualBinStateSerialize = true)]
    [TFileDef("Keyframe Track")]
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
        [Browsable(false)]
        public object SyncRoot { get; } = null;
        [Browsable(false)]
        public bool IsSynchronized { get; } = false;

        public T this[int index]
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
                            Keyframe sibling = key.Prev ?? key.Next;
                            key.Remove();
                            sibling.UpdateLink(value);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        object IList.this[int index]
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
                            Keyframe sibling = key.Prev ?? key.Next;
                            key.Remove();
                            sibling.UpdateLink(keyValue);
                            break;
                        }
                        ++i;
                    }
                }
            }
        }

        public void Add(IEnumerable<T> keys)
            => keys.ForEach(x => Add(x));
        public void Add(params T[] keys)
            => keys.ForEach(x => Add(x));
        
        public void Add(T key)
        {
            if (key is null)
                return;
            
            if (First is null)
            {
                //Reset key location before adding
                key.Remove();
                First = key;
                Last = key;
                Count = 1;
                OnChanged();
            }
            else
                First.UpdateLink(key);
        }
        public void RemoveLast()
        {
            if (Last is null)
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
            if (First is null)
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
            T bestKey = null;
            foreach (T key in this)
                if (key.Second <= second)
                    bestKey = key;
                else
                    break;

            //if (bestKey != null)
                return bestKey;

            //if (returnLastBeforeFirst)
            //{
            //    if (returnLastBeforeAnimEnd)
            //        return GetKeyBefore(LengthInSeconds, false, false);
            //    else
            //        return Last;
            //}
            //else
            //    return bestKey;
        }
        public override IEnumerator<Keyframe> GetEnumerator()
        {
            Keyframe node = First;
            do
            {
                if (node is null)
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
                if (node is null)
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
            Keyframe k = keyframes.First, temp;
            while (k != null)
            {
                temp = k.Next;
                k.Remove();
                k.Second += LengthInSeconds;
                Add(k);
                k = temp;
            }
        }

        #region Reading / Writing

        public override void ManualRead(SerializeElement node)
        {
            //if (!string.Equals(node.MemberInfo.Name, "KeyframeTrack", StringComparison.InvariantCulture))
            //{
            //    LengthInSeconds = 0.0f;
            //    return;
            //}

            if (node.GetAttributeValue(nameof(LengthInSeconds), out float length))
                LengthInSeconds = length;
            else
                LengthInSeconds = 0.0f;

            Clear();
            
            if (!node.GetAttributeValue(nameof(Count), out int keyCount))
                return;

            Type t = typeof(T);
            if (!typeof(IPlanarKeyframe).IsAssignableFrom(t))
                return;

            T kf = new T();
            Type valueType = kf.ValueType;
            Array array = Array.CreateInstance(valueType, keyCount);
            Type arrayType = array.GetType();

            float[] seconds = new float[keyCount];
            object inValues = null;
            object outValues = null;
            object inTans = null;
            object outTans = null;
            EVectorInterpType[] interpTypes = new EVectorInterpType[keyCount];
            
            //Read all keyframe information, split into separate element arrays
            foreach (SerializeElement element in node.Children)
            {
                switch (element.Name)
                {
                    case "Seconds": element.Content.GetObjectAs(out seconds); break;
                    case "InValues": element.Content.GetObject(arrayType, out inValues); break;
                    case "OutValues": element.Content.GetObject(arrayType, out outValues); break;
                    case "InTangents": element.Content.GetObject(arrayType, out inTans); break;
                    case "OutTangents": element.Content.GetObject(arrayType, out outTans); break;
                    case "InterpTypes": element.Content.GetObjectAs(out interpTypes); break;
                }
            }

            object defaultObj = valueType.GetDefaultValue();
            for (int i = 0; i < keyCount; ++i)
            {
                float sec                   = seconds       is null || !seconds.IndexInRange(i)                     ? 0.0f                   : seconds[i];
                EVectorInterpType interp    = interpTypes   is null || !interpTypes.IndexInRange(i)                 ? EVectorInterpType.Step : interpTypes[i];
                object inVal                = inValues      is null || !((Array)inValues).IndexInRangeGeneric(i)    ? defaultObj             : ((Array)inValues).GetValue(i);
                object outVal               = outValues     is null || !((Array)outValues).IndexInRangeGeneric(i)   ? defaultObj             : ((Array)outValues).GetValue(i);
                object inTan                = inTans        is null || !((Array)inTans).IndexInRangeGeneric(i)      ? defaultObj             : ((Array)inTans).GetValue(i);
                object outTan               = outTans       is null || !((Array)outTans).IndexInRangeGeneric(i)     ? defaultObj             : ((Array)outTans).GetValue(i);

                kf = new T();

                IPlanarKeyframe kfp = (IPlanarKeyframe)kf;
                kfp.InterpolationType = interp;
                kfp.Second = sec;
                kfp.InValue = inVal;
                kfp.OutValue = outVal;
                kfp.InTangent = inTan;
                kfp.OutTangent = outTan;

                Add(kf);
            }
        }
        public override void ManualWrite(SerializeElement node)
        {
            //node.MemberInfo.Name = "KeyframeTrack";
            node.AddAttribute(nameof(LengthInSeconds), LengthInSeconds);
            node.AddAttribute(nameof(Count), Count);

            if (Count <= 0)
                return;
            
            Type t = typeof(T);
            if (!typeof(IPlanarKeyframe).IsAssignableFrom(t))
                return;

            float[] seconds     = new float[Count];
            object[] inValues   = new object[Count];
            object[] outValues  = new object[Count];
            object[] inTans     = new object[Count];
            object[] outTans    = new object[Count];
            EVectorInterpType[] interpTypes = new EVectorInterpType[Count];

            int i = 0;
            foreach (IPlanarKeyframe kf in this)
            {
                seconds[i] = kf.Second;
                interpTypes[i] = kf.InterpolationType;
                inValues[i] = kf.InValue;
                outValues[i] = kf.OutValue;
                inTans[i] = kf.InTangent;
                outTans[i] = kf.OutTangent;
                ++i;
            }

            node.AddChildElementObject("Seconds",        seconds);
            node.AddChildElementObject("InValues",       inValues);
            node.AddChildElementObject("OutValues",      outValues);
            node.AddChildElementObject("InTangents",     inTans);
            node.AddChildElementObject("OutTangents",    outTans);
            node.AddChildElementObject("InterpTypes",    interpTypes);
        }
        #endregion
    }
    public enum ERadialInterpType
    {
        Step,
        Linear,
        CubicBezier
    }
    public enum EVectorInterpType
    {
        Step,
        Linear,
        CubicHermite,
        CubicBezier
    }
    public abstract class Keyframe : TObject, IKeyframe, ISerializableString
    {
        [TSerialize(nameof(Second), NodeType = ENodeType.Attribute)]
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
                Keyframe kf = Prev ?? Next;
                kf?.UpdateLink(this, false);
                OwningTrack?.OnChanged();
            }
        }

        [Browsable(false)]
        public Keyframe Next => _next;
        [Browsable(false)]
        public Keyframe Prev => _prev;
        [Browsable(false)]
        [Category("Keyframe")]
        public bool IsFirst => _prev is null;
        [Browsable(false)]
        [Category("Keyframe")]
        public bool IsLast => _next is null;
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

        [Browsable(false)]
        public abstract Type ValueType { get; }

        private BaseKeyframeTrack _owningTrack = null;
        public void UpdateLink(Keyframe key, bool notifyChange = true)
        {
            if (key is null || key == this)
                return;

            //if (_next == key)
            //    return;

            //resize track length if second is outside of range
            //if (key.Second > OwningTrack.LengthInSeconds)
            //    OwningTrack.LengthInSeconds = key.Second;
            
            key.Remove(key.OwningTrack != OwningTrack);
            key.Second = key.Second.RemapToRange(0.0f, OwningTrack.LengthInSeconds + 0.0001f);

            //Second is within this keyframe and the next?
            if (key.Second >= Second)
            {
                //After current key
                if (Next is null || key.Second < Next.Second)
                {
                    if (_next != key)
                    {
                        key._next = _next;
                        if (_next != null)
                            _next._prev = key;
                    }

                    key._prev = this;
                    _next = key;

                    PostKeyLinkUpdate(key, notifyChange);
                }
                else
                {
                    //Recursive link to next key
                    //next not null, greater than next, and next is not before this
                    if (_next != null && key.Second >= _next.Second && _next.Second >= Second)
                        _next.UpdateLink(key, notifyChange);
                }
            }
            else
            {
                //Before current key
                if (Prev is null || key.Second >= Prev.Second)
                {
                    if (_prev != key)
                    {
                        key._prev = _prev;
                        if (_prev != null)
                            _prev._next = key;
                    }

                    key._next = this;
                    _prev = key;

                    PostKeyLinkUpdate(key, notifyChange);
                }
                else 
                {
                    //Recursive link to prev key
                    //prev not null, less than this, and prev is not after this
                    if (_prev != null && _prev.Second < Second)
                        _prev.UpdateLink(key, notifyChange);
                }
            }
        }

        private void PostKeyLinkUpdate(Keyframe key, bool notifyChange)
        {
            //Get owning track from next or prev
            if (!key.IsFirst)
                key.OwningTrack = key.Prev.OwningTrack;
            else
            {
                if (!key.IsLast)
                    key.OwningTrack = key.Next.OwningTrack;
                else
                    throw new Exception();
            }

            //Update owning track first and last references
            if (key.IsFirst)
                key.OwningTrack.FirstKey = key;
            if (key.IsLast)
                key.OwningTrack.LastKey = key;

            if (OwningTrack != null)
            {
                ++OwningTrack.Count;
                if (notifyChange)
                    OwningTrack.OnChanged();
            }

            if (key._prev == key ||
                key._next == key ||
                (key._next == key._prev && key._next != null))
                throw new Exception();
        }

        public abstract string WriteToString();
        public abstract void ReadFromString(string str);
        public override string ToString() => WriteToString();

        public void Remove(bool notifyChange = true)
        {
            if (_next != null)
                _next._prev = _prev;
            if (_prev != null)
                _prev._next = _next;

            if (OwningTrack != null)
            {
                if (IsFirst)
                    OwningTrack.FirstKey = _next;
                if (IsLast)
                    OwningTrack.LastKey = _prev;

                --OwningTrack.Count;
                if (notifyChange)
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
        //public static bool operator ==(Keyframe left, Keyframe right)
        //    => left?.Equals(right) ?? right is null;
        //public static bool operator !=(Keyframe left, Keyframe right)
        //    => left is null ? !(right is null) : !left.Equals(right);
        //public override bool Equals(object obj)
        //{
        //    if (obj is null)
        //        return false;
        //    if (obj.GetType() != GetType())
        //        return false;

        //    return Second == ((Keyframe)obj).Second;
        //}
        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }
}
