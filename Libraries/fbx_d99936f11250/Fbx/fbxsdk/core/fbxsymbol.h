#ifndef _FBXSDK_CORE_SYMBOL_H_
#define _FBXSDK_CORE_SYMBOL_H_
class FBXSDK_DLL FbxSymbol
{
public:
FbxSymbol(const char* pName, const char* pRealm);
~FbxSymbol();
unsigned int GetID() const;
bool operator==(FbxSymbol const& pSymbol) const;
bool operator!=(FbxSymbol const& pSymbol) const;
};
typedef FbxMap< FbxString, int, FbxStringCompare > FbxStringSymbolMap;
inline operator const char*() const { return mItem ? ((const char*) mItem->GetKey()) : NULL; }
}
};
#endif