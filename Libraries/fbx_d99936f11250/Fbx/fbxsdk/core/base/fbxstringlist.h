#ifndef _FBXSDK_CORE_BASE_STRING_LIST_H_
#define _FBXSDK_CORE_BASE_STRING_LIST_H_
class FbxStringListItem
{
public:
FbxStringListItem(){ mReference = 0; }
FbxStringListItem(const char* pString, FbxHandle pRef=0){ mString = pString; mReference = pRef; }
FbxString mString;
FbxHandle  mReference;
};
inline int FbxCompareStringListSort(const void* E1, const void* E2)
{
return FBXSDK_stricmp((*(FbxStringListItem**)E1)->mString.Buffer(), (*(FbxStringListItem**)E2)->mString.Buffer());
}
inline int FbxCompareStringListFindEqual(const void* E1, const void* E2)
{
return FBXSDK_stricmp((*(FbxStringListItem*)E1).mString.Buffer(), (*(FbxStringListItem**)E2)->mString.Buffer());
}
inline int FbxCompareCaseSensitiveStringList(const void *E1,const void *E2)
{
return strcmp((*(FbxStringListItem*)E1).mString.Buffer(), (*(FbxStringListItem**)E2)->mString.Buffer());
}
template <class Type> class FbxStringListT
{
protected:
FbxArray<Type*> mList;
public:
int  AddItem( Type* pItem )  { return mList.Add( pItem ); }
int  InsertItemAt( int pIndex, Type* pItem ) { return mList.InsertAt( pIndex, pItem ); }
Type*   GetItemAt( int pIndex ) const { return mList[pIndex]; }
int  FindItem( Type* pItem ) const { return mList.Find( pItem ); }
public :
FbxStringListT()
{
}
virtual ~FbxStringListT() { Clear(); }
void RemoveLast() { RemoveAt( mList.GetCount()-1 ); }
inline int  GetCount() const { return mList.GetCount(); }
FbxString&   operator[](int pIndex) { return mList[pIndex]->mString; }
FbxHandle  GetReferenceAt(int pIndex) const { return mList[pIndex]->mReference; }
void   SetReferenceAt(int pIndex, FbxHandle pRef) { mList[pIndex]->mReference = pRef; }
char*  GetStringAt(int pIndex) const { if (pIndex<mList.GetCount()) return mList[pIndex]->mString.Buffer(); else return NULL; }
virtual bool SetStringAt(int pIndex, const char* pString)
{
if (pIndex<mList.GetCount())
{
mList[pIndex]->mString = pString;
return true;
} else return false;
}
int Find( Type& pItem ) const
{
for (int Count=0; Count<mList.GetCount(); Count++) {
if (mList[Count]==&pItem) {
return Count;
}
}
return -1;
}
int FindIndex( FbxHandle pReference ) const
{
for (int Count=0; Count<mList.GetCount(); Count++) {
if (mList[Count]->mReference==pReference) {
return Count;
}
}
return -1;
}
int FindIndex( const char* pString ) const
{
for (int lCount=0; lCount<mList.GetCount(); lCount++) {
if (mList[lCount]->mString==pString) {
return lCount;
}
}
return -1;
}
FbxHandle FindReference(const char* pString ) const
{
int lIndex = FindIndex( pString );
if (lIndex!=-1) {
return mList[lIndex]->mReference;
}
return 0;
}
bool Remove ( Type& pItem )
{
int lIndex = Find( pItem );
if (lIndex>=0) {
RemoveAt( lIndex );
return true;
}
return false;
}
bool Remove (const char* pString )
{
int lIndex = FindIndex( pString );
if (lIndex>=0) {
RemoveAt( lIndex );
return true;
}
return false;
}
bool RemoveIt ( Type& pItem )
{
int lIndex = Find( pItem );
if (lIndex>=0) {
RemoveAt( lIndex );
return true;
}
return false;
}
void Sort( )
{
qsort( &(mList.GetArray()[0]),mList.GetCount(),sizeof(FbxStringListItem*),FbxCompareStringListSort );
}
void* FindEqual(const char* pString) const
{
FbxStringListItem Key(pString);
if (mList.GetCount() != 0)
{
return bsearch ( &Key, &(mList.GetArray()[0]),mList.GetCount(),sizeof(FbxStringListItem*),FbxCompareStringListFindEqual );
}
else
{
return NULL ;
}
}
void* FindCaseSensitive(const char* pString) const
{
FbxStringListItem Key(pString);
if (mList.GetCount() != 0)
{
return bsearch ( &Key, &(mList.GetArray()[0]),mList.GetCount(),sizeof(FbxStringListItem*), FbxCompareCaseSensitiveStringList);
}
else
{
return NULL ;
}
}
int Add( const char* pString, FbxHandle pItem=0 )
{
return InsertAt( mList.GetCount(),pString,pItem );
}
virtual int InsertAt( int pIndex, const char* pString, FbxHandle pItem=0 )
{
return mList.InsertAt( pIndex,FbxNew< Type >( pString,(FbxHandle)pItem ));
}
virtual void RemoveAt(int pIndex)
{
FbxDelete(mList.RemoveAt(pIndex));
}
virtual void Clear()
{
FbxArrayDelete(mList);
}
virtual void GetText(FbxString& pText) const
{
int lCount;
for (lCount=0; lCount<mList.GetCount(); lCount++)
{
pText += mList[lCount]->mString;
if (lCount<mList.GetCount()-1)
{
pText += "~";
}
}
}
virtual int SetText(const char* pList)
{
int  lPos=0, lOldPos = 0;
int  lLastIndex=0;
FbxString lName=pList;
Clear();
for (lPos=0; lName.Buffer()[lPos]!=0; lPos++) {
if (lName.Buffer()[lPos]=='~') {
lName.Buffer()[lPos]=0;
lLastIndex = Add(lName.Buffer()+lOldPos);
lOldPos=lPos+1;
}
}
if(lOldPos != lPos)
{
lLastIndex = Add(lName.Buffer()+lOldPos);
}
return lLastIndex;
}
};
class FBXSDK_DLL FbxStringList : public FbxStringListT<FbxStringListItem>
{
public:
FbxStringList();
FbxStringList( const FbxStringList& pOriginal );
void CopyFrom( const FbxStringList* pOriginal  );
FbxStringList& operator=(const FbxStringList& pOriginal);
};
#endif