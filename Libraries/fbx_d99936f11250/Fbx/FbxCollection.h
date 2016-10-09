#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxQuery;
		ref class FbxCriteria;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		/** This class contains objects.
		* \nosubgrouping
		* This class also provides access to global settings and take information.
		*
		*/
		public ref class FbxCollectionManaged : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,FbxCollection);
			FBXOBJECT_DECLARE(FbxCollectionManaged);
		internal:
			FbxCollectionManaged(FbxCollection* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}

			/**
			* \name Collection member management
			*/
			//@{
		public:
			//! Delete all contained objects.
			virtual void Clear();
			//! Add a member.
			void AddMember(FbxObjectManaged^ member);
			//! Remove a member.
			void RemoveMember(FbxObjectManaged^ member);
			//! Find a member.
			//template <class T> inline T *       FindMember(T const *pfbxType, const char *pName) { return FindSrcObject(pfbxType, pName); }

			//! Return the number of objects in the collection.
			property int MemberCount
			{
				int get();
			}
			//! Return the number of objects in the collection.
			//template < class T > inline int     GetMemberCount (T const *pFBX_TYPE) const { return GetSrcObjectCount(T::ClassId); }
			int GetMemberCount(FbxCriteria^ criteria );

			//! Return the index'th member of the collection.
			FbxObjectManaged^ GetMember (int index);
			//! Return the index'th member of the collection.
			//template < class T > inline T*      GetMember (T const *pFBX_TYPE, int pIndex=0) const  { return (T *)GetSrcObject(T::ClassId,pIndex); }			
			FbxObjectManaged^ GetMember (FbxCriteria^ criteria,int index);
			//! Is an object part of the collection.

			virtual bool IsMember(FbxObjectManaged^ member);
			//@}

			/**
			* \name Selection managent
			*/
			//@{
		public:
			//! Select/Unselect all contained objects.
			virtual void SetSelectedAll(bool selection);
			//! Select/Unselect objects.
			virtual void SetSelected(FbxObjectManaged^ obj,bool selection);
			//! Get Select/Unselect .
			virtual bool GetSelected(FbxObjectManaged^ selection);

		};
	}
}