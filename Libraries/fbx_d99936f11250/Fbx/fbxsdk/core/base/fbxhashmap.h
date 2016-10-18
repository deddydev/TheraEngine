#ifndef _FBXSDK_CORE_BASE_HASHMAP_H_
#define _FBXSDK_CORE_BASE_HASHMAP_H_
template<class T> class FbxNoOpDestruct { public: static inline void DoIt(T&) {} };
template<class T> class FbxPtrDestruct  { public: static inline void DoIt(T& v) { FbxDelete(v); v = NULL; } };
template<class T> class FbxDefaultComparator{ public: static inline bool CompareIt( const T& t1, const T& t2 ) { return t1 == t2; } };
template< typename KEY, typename VALUE, typename HASH, class Destruct = FbxNoOpDestruct<VALUE>, class Comparator = FbxDefaultComparator<KEY> >
class FbxHashMap
{
public:
typedef KEY KeyType;
typedef VALUE ValueType;
typedef HASH HashFunctorType;
}
}
};
typedef ListItem ListItemType;
typedef FbxPair< KeyType, ValueType > KeyValuePair;
}
~Iterator(){};
}
}
}
}
}
}
}
}
}
}
}
}
};
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
FbxHashMap( const FbxHashMap& pOther ) {};
};
#endif