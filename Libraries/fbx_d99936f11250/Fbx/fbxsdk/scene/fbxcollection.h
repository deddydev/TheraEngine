#ifndef _FBXSDK_SCENE_COLLECTION_H_
#define _FBXSDK_SCENE_COLLECTION_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCriteria
class FBXSDK_DLL FbxCollection : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxCollection, FbxObject)
public:
        virtual void Clear()
        virtual bool AddMember(FbxObject* pMember) 
 return ConnectSrcObject(pMember)
        virtual bool RemoveMember(FbxObject* pMember) 
 return DisconnectSrcObject(pMember)
        inline int GetMemberCount () const 
 return GetSrcObjectCount()
        inline FbxObject* GetMember(int pIndex=0) const 
 return GetSrcObject(pIndex)
        virtual bool IsMember(const FbxObject* pMember) const
		template <class T> inline int GetMemberCount() const 
 return GetSrcObjectCount<T>()
		template <class T> inline T* GetMember(int pIndex=0) const 
 return GetSrcObject<T>(pIndex)
		template <class T> inline T* FindMember(const char* pName) const 
 return FindSrcObject<T>(pName)
        inline int GetMemberCount(const FbxCriteria& pCriteria) const 
 return GetSrcObjectCount(pCriteria)
        inline FbxObject* GetMember(const FbxCriteria& pCriteria, int pIndex=0) const 
 return GetSrcObject(pCriteria, pIndex)
		inline FbxObject* FindMember(const FbxCriteria& pCriteria, const char* pName) const 
 return FindSrcObject(pCriteria, pName)
        virtual void SetSelectedAll(bool pSelection)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 