#pragma once
#include "stdafx.h"
#include "FbxEmitter.h"

using namespace System;
namespace Skill
{
	namespace FbxSDK
	{			
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxStringManaged;
		/** Base class to handle plug connections.
		* \remarks This class is for the FBX SDK internal use only.
		* \nosubgrouping
		*/
		public ref class FbxPlug : Events::FbxEmitter
		{
		internal:
			FbxPlug(){}//must removed
			FbxPlug(KFbxPlug* p);
			REF_DECLARE(FbxEmitter,KFbxPlug);

			/**
			* \name Constructor and Destructors.
			*/
			//@{		
			FbxSdkManagerManaged^ sdkManager;
		protected:
			virtual void CollectManagedMemory() override;
		public:

			private:
				static FbxClassId^ _FbxPlugclassID;
			public:
				virtual FbxClassId^ GetClassId();
				static FbxPlug^ Create(FbxSdkManagerManaged^ manager , String^ name);			
				static property FbxClassId^ ClassId { FbxClassId^ get(); }

			/** Delete the object and Unregister from the FbxSdkManager
			*/
			virtual void Destroy(bool recursive, bool dependents);
			virtual void Destroy();			
			//@}

			/**
			* \name Object ownership and type management.
			*/
			//@{
		public:
			/** Get the KFbxSdkManager that created this object 
			* \return Pointer to the KFbxSdkManager
			*/
			virtual property Skill::FbxSDK::FbxSdkManagerManaged^ SdkManager
			{
				FbxSdkManagerManaged^ get();
			}
			/** Test if the class is a hierarchical children of the specified class type  
			* \param pClassId ClassId representing the class type 
			* \return Returns true if the object is of the type specified
			*/

		//	//template < class T >inline bool	Is(T *pFBX_TYPE) const					{ return Is(T::ClassId); }

			virtual VALUE_PROPERTY_GET_DECLARE(bool,IsRuntimePlug);			
			//@}						

			bool Is(FbxClassId^ classId);
			bool IsRuntime(FbxClassId^ classId);
			bool SetRuntimeClassId(FbxClassId^ classId);
			REF_PROPERTY_GET_DECLARE(FbxClassId,RuntimeClassId);			
		};


		public ref class FbxPlugInfo
		{

		internal:
			const KFbxPlug* plug;
			FbxPlugInfo(const KFbxPlug* p);

			//			/**
			//			* \name Constructor and Destructors.
			//			*/
			//			//@{
			//		public:
			//			/** Delete the object and Unregister from the FbxSdkManager
			//			*/
			//			virtual void Destroy(bool pRecursive=false, bool pDependents=false);
			//			//@}
			//
			//			/**
			//			* \name Object ownership and type management.
			//			*/
			//			//@{
			//		public:
			//			/** Get the KFbxSdkManager that created this object 
			//			* \return Pointer to the KFbxSdkManager
			//			*/
			//			virtual KFbxSdkManager*	GetFbxSdkManager() const { return 0; }
			//			/** Test if the class is a hierarchical children of the specified class type  
			//			* \param pClassId ClassId representing the class type 
			//			* \return Returns true if the object is of the type specified
			//			*/
			//			virtual bool					Is(kFbxClassId pClassId) const			{ return GetClassId().Is(pClassId);	}
			//			template < class T >inline bool	Is(T *pFBX_TYPE) const					{ return Is(T::ClassId); }
			//			virtual bool					IsRuntime(kFbxClassId pClassId) const	{ return GetRuntimeClassId().Is(pClassId);	}
			//			virtual bool					SetRuntimeClassId(kFbxClassId pClassId);
			//			virtual kFbxClassId				GetRuntimeClassId() const;
			//			virtual bool					IsRuntimePlug() const					{ return !( GetRuntimeClassId() == GetClassId() ); }
			//			//@}
			//
			//			///////////////////////////////////////////////////////////////////////////////
			//			//
			//			//  WARNING!
			//			//
			//			//	Anything beyond these lines may not be documented accurately and is 
			//			// 	subject to change without notice.
			//			//
			//			///////////////////////////////////////////////////////////////////////////////
			//#ifndef DOXYGEN_SHOULD_SKIP_THIS
			//		protected:
			//			inline KFbxPlug() {}
			//			inline KFbxPlug(KFbxSdkManager& pManager, const char* pName) {}
			//			virtual ~KFbxPlug() {}
			//
			//			virtual void Construct(const KFbxPlug* pFrom);
			//			virtual void Destruct(bool pRecursive, bool pDependents);
			//			friend class KFbxProperty;
			//			friend class KFbxObject;
			//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}