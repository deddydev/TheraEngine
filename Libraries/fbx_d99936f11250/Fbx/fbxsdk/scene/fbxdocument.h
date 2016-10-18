#ifndef _FBXSDK_SCENE_DOCUMENT_H_
#define _FBXSDK_SCENE_DOCUMENT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxcollection.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxStatus
class FbxTakeInfo
class FbxPeripheral
class FbxDocumentInfo
class FBXSDK_DLL FbxDocument : public FbxCollection
    FBXSDK_OBJECT_DECLARE(FbxDocument, FbxCollection)
public:
		FbxPropertyT<FbxReference> Roots
        virtual void  Clear()
        inline void AddRootMember(FbxObject* pMember)
 AddMember(pMember)
 Roots.ConnectSrcObject(pMember)
        inline void RootRootRemoveMember(FbxObject* pMember)
 RemoveMember(pMember)
 Roots.DisconnectSrcObject(pMember)
		template <class T> inline T* FindRootMember(char* pName)
 return Roots.FindSrcObject<T>(pName)
        inline int GetRootMemberCount () const 
 return Roots.GetSrcObjectCount()
		template <class T> inline int GetRootMemberCount() const 
 return Roots.GetSrcObjectCount<T>()
        int GetRootMemberCount(FbxCriteria pCriteria) const
        inline FbxObject* GetRootMember(int pIndex=0) const 
 return Roots.GetSrcObject(pIndex)
		template <class T> inline T* GetRootMember(int pIndex=0) const  
 return Roots.GetSrcObject<T>(pIndex)
        FbxObject* GetRootMember(FbxCriteria pCriteria, int pIndex=0) const
        virtual bool IsRootMember(FbxObject* pMember) const
        FbxDocumentInfo* GetDocumentInfo() const
        void SetDocumentInfo(FbxDocumentInfo* pSceneInfo)
        void SetPeripheral(FbxPeripheral* pPeripheral)
        virtual FbxPeripheral* GetPeripheral()
        int UnloadContent(FbxStatus* pStatus = NULL)
        int LoadContent(FbxStatus* pStatus = NULL)
        int GetReferencingDocuments(FbxArray<FbxDocument*>& pReferencingDocuments) const
        int GetReferencingObjects(const FbxDocument* pFromDoc, FbxArray<FbxObject*>& pReferencingObjects) const
        int GetReferencedDocuments(FbxArray<FbxDocument*>& pReferencedDocuments) const
        int GetReferencedObjects(const FbxDocument* pToDoc, FbxArray<FbxObject*>& pReferencedObjects) const
        FbxString GetPathToRootDocument(void) const
        void GetDocumentPathToRootDocument(FbxArray<FbxDocument*>& pDocumentPath, bool pFirstCall = true) const
        bool IsARootDocument(void) 
 return (NULL == GetDocument())
        FbxPropertyT<FbxString> ActiveAnimStackName
        bool CreateAnimStack(const char* pName, FbxStatus* pStatus = NULL)
        bool RemoveAnimStack(const char* pName)
        void FillAnimStackNameArray(FbxArray<FbxString*>& pNameArray)
        bool SetTakeInfo(const FbxTakeInfo& pTakeInfo)
        FbxTakeInfo* GetTakeInfo(const FbxString& pTakeName) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	virtual void Compact()
	void ConnectVideos()
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
	virtual bool ConnectNotify(const FbxConnectEvent& pEvent)
	virtual void SetDocument(FbxDocument* pDocument)
	bool		 FindTakeName(const FbxString& pTakeName)
	FbxArray<FbxTakeInfo*>	mTakeInfoArray
private:
	FbxPeripheral*		mPeripheral
	FbxDocumentInfo*	mDocumentInfo
#endif 