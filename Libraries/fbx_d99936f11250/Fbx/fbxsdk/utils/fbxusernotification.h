#ifndef _FBXSDK_UTILS_USER_NOTIFICATION_H_
#define _FBXSDK_UTILS_USER_NOTIFICATION_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/core/base/fbxmultimap.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxLogFile
class FbxMessageEmitter
class FbxUserNotificationFilteredIterator
class FBXSDK_DLL FbxAccumulatorEntry
public:
    enum EClass
        eError=1,    
        eWarning=2,  
        eInformation=4,     
        eAny=7       
    FbxAccumulatorEntry(EClass pAEClass, const FbxString& pName, const FbxString& pDescr, 
                     FbxString pDetail="", bool pMuteState=true)
    FbxAccumulatorEntry(const FbxAccumulatorEntry& pAE, bool pSkipDetails)
    ~FbxAccumulatorEntry()
    EClass GetClass() const
    FbxString GetName() const
    FbxString    GetDescription() const
    int GetDetailsCount() const
    const FbxString* GetDetail(int id) const
    bool IsMuted() const
private:
    FbxArray<FbxString*>&	GetDetails()
    void					Mute(bool pState)
    bool					mMute
    EClass					mAEClass
    FbxString				mName
    FbxString				mDescr
    FbxArray<FbxString*>	mDetails
	friend class FbxUserNotification
class FBXSDK_DLL FbxUserNotification
public:
    static FbxUserNotification* Create(FbxManager* pManager, 
                                        const FbxString& pLogFileName, 
                                        const FbxString& pSessionDescription)
    static void Destroy(FbxManager* pManager)
    FbxUserNotification(FbxManager* pManager,
                         FbxString const& pLogFileName, 
                         FbxString const& pSessionDescription)
    virtual ~FbxUserNotification()
    void InitAccumulator()
    void ClearAccumulator()
    enum EEntryID
        eBindPoseInvalidObject,
        eBindPoseInvalidRoot,
        eBindPoseNotAllAncestorsNodes,
        eBindPoseNotAllDeformingNodes,
        eBindPoseNotAllAncestorsDefinitionNodes,
        eBindPoseRelativeMatrix,
        eEmbedMediaNotify,
        eFileIONotify,						
        eFileIONotifyMaterial,
        eFileIONotifyDXFNotSupportNurbs,
        eEntryStartID						
    int AddEntry(const int pID, const FbxString& pName, const FbxString& pDescr, FbxAccumulatorEntry::EClass pClass=FbxAccumulatorEntry::eWarning)
    int AddDetail(int pEntryId)
    int AddDetail(int pEntryId, FbxString pString)
    int AddDetail(int pEntryId, FbxNode* pNode)
    int  GetNbEntries() const
    const FbxAccumulatorEntry* GetEntry(int pEntryId)
    const FbxAccumulatorEntry* GetEntryAt(int pEntryIndex) const
    int GetNbDetails() const
    int GetDetail(int pDetailId, const FbxAccumulatorEntry*& pAE) const
    enum EOutputSource
        eAccumulatorEntry,     
        eSequencedDetails      
    bool Output(EOutputSource pOutSrc=eAccumulatorEntry, int pIndex = -1, bool pExtraDevicesOnly = false)
    bool OutputById(EEntryID pId, EOutputSource pOutSrc=eAccumulatorEntry, bool pExtraDevicesOnly = false)
    bool Output(const FbxString& pName, const FbxString& pDescr, FbxAccumulatorEntry::EClass pClass, bool pExtraDevicesOnly = false)
    bool Output(FbxUserNotificationFilteredIterator& pAEFIter, bool pExtraDevicesOnly = false)
    void SetLogMessageEmitter(FbxMessageEmitter * pLogMessageEmitter)
    virtual void GetLogFilePath(FbxString& pPath)
    class AESequence
    public:
        AESequence(FbxAccumulatorEntry* pAE, int pDetailId) :
            mAE(pAE),
            mDetailId(pDetailId)
        FbxAccumulatorEntry* AE() 
 return mAE
        int DetailId() 
 return mDetailId
    private:
        FbxAccumulatorEntry* mAE
        int mDetailId
    friend class FbxUserNotificationFilteredIterator
    virtual bool PostTerminate()
    virtual void AccumulatorInit()
    virtual void AccumulatorClear()
    virtual void OpenExtraDevices()
    virtual bool SendToExtraDevices(bool pOutputNow, FbxArray<FbxAccumulatorEntry*>& pEntries)
    virtual bool SendToExtraDevices(bool pOutputNow, FbxArray<AESequence*>& pAESequence)
    virtual bool SendToExtraDevices(bool pOutputNow, const FbxAccumulatorEntry* pAccEntry, int pDetailId = -1)
    virtual void CloseExtraDevices()
    void ResetAccumulator()
    void ResetSequence()
    void SendToLog(EOutputSource pOutSrc, int pId)
    void SendToLog(const FbxAccumulatorEntry* pAccEntry, int pDetailId = -1)
private:
    FbxString mLogFileName
    FbxString* mLog
    FbxLogFile* mLogFile
    FbxMessageEmitter* mLogMessageEmitter
    bool mProperlyInitialized
    FbxString mSessionDescription
    bool mProperlyCleaned
    FbxMultiMap mAccuHT
    FbxArray<FbxAccumulatorEntry*> mAccu
    FbxArray<AESequence*> mAESequence
    FbxManager*             mSdkManager
class FBXSDK_DLL FbxUserNotificationFilteredIterator
public:
    FbxUserNotificationFilteredIterator(FbxUserNotification& pAccumulator, 
            int pFilterClass,
            FbxUserNotification::EOutputSource pSrc = FbxUserNotification::eSequencedDetails,
            bool pNoDetail = true)
    virtual ~FbxUserNotificationFilteredIterator()
    int  GetNbItems() const
    void Reset()
    FbxAccumulatorEntry* const First()
    FbxAccumulatorEntry* const Previous()
    FbxAccumulatorEntry* const Next()
protected:
    virtual void BuildFilteredList(FbxUserNotification& pAccumulator)
    int                                    mIterator
    int                                    mFilterClass
    bool                                mNoDetail
    FbxUserNotification::EOutputSource    mAccuSrcData
    FbxArray<FbxAccumulatorEntry*>    mFilteredAE
#include <fbxsdk/fbxsdk_nsend.h>
#endif 