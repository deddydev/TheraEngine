#ifndef _FBXSDK_FILEIO_STATISTICS_FBX_H_
#define _FBXSDK_FILEIO_STATISTICS_FBX_H_
#ifndef DOXYGEN_SHOULD_SKIP_THIS
class FbxStatisticsFbx : public FbxStatistics
{
public:
virtual bool AddItem(FbxString& pItemName, int pItemCount)
{
mItemName.Add( FbxNew< FbxString >(pItemName) );
mItemCount.Add( pItemCount);
return true;
};
};
#endif
#endif