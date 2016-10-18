#ifndef _FBXSDK_FILEIO_STATISTICS_H_
#define _FBXSDK_FILEIO_STATISTICS_H_
class FBXSDK_DLL FbxStatistics
{
public:
FbxStatistics();
virtual ~FbxStatistics();
void Reset();
int GetNbItems() const;
bool GetItemPair(int pNum, FbxString& pItemName, int& pItemCount) const;
FbxStatistics& operator=(const FbxStatistics& pStatistics);
virtual bool AddItem(FbxString&  , int  ) { return false; };
};
#endif