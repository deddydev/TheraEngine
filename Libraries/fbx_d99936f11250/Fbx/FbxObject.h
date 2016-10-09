#pragma once
#include "stdafx.h"
#include "FbxPlug.h"
#include "FbxPropertyDef.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxStringManaged;
		ref class FbxStream;
		ref class FbxEvaluationInfo;
		ref class FbxProperty;
		ref class FbxDataType;
		ref class FbxDocument;
		ref class FbxScene;
		ref class FbxLibrary;
		ref class FbxCriteria;
		ref class FbxObjectMetaData;

		public enum class FbxCompare
		{
			Properties
		};

		/** \brief Basic class for object type identification and instance naming.
		* \nosubgrouping
		*/
		public ref class FbxObjectManaged : FbxPlug
		{
		internal:
			FbxObjectManaged(){}// must removed
			FbxObjectManaged(FbxObject* obj);
			REF_DECLARE(FbxEmitter,FbxObject);
		protected:
			FbxObjectManaged^ refTo;
			virtual void CollectManagedMemory() override; 
		public:

			/** Types of clones that can be created for KFbxObjects.
			*/
			enum class CloneType
			{
				//Surface = FbxObject::eSURFACE_CLONE,     //!<
				Reference= FbxObject::eReferenceClone,   //!< Changes to original object propagate to clone. Changes to clone do not propagate to original.
				Deep = FbxObject::eDeepClone         //!< A deep copy of the object. Changes to either the original or clone do not propagate to each other.
			};

			FBXPLUG_DECLARE(FbxObjectManaged);
		public:
			static FbxObjectManaged^ Create(FbxObjectManaged^ container , String^ name);
			static FbxObjectManaged^ CreateForClone(Skill::FbxSDK::FbxSdkManagerManaged^ manager , String^ name,FbxObjectManaged^ from);
			FbxObjectManaged^ TypedClone(FbxObjectManaged^ container , CloneType cloneType);
			FbxObjectManaged^ TypedClone(FbxObjectManaged^ container);
			virtual bool Compare(FbxObjectManaged^ otherObject,FbxCompare fbxCompare);			

			/**
			* \name Cloning and references
			*/
			//@{
		public:
			// Clone

			/** Creates a clone of this object.
			* \param pContainer The object, typically a document or scene, that will contain the new clone. Can be NULL.
			* \param pCloneType The type of clone to create
			* \return The new clone, or NULL if the specified clone type is not supported.
			*/
			 virtual FbxObjectManaged^ Clone(FbxObjectManaged^ container, FbxObjectManaged::CloneType cloneType);

			/** Check if this object is a reference clone of another.
			* \return true if this object is a clone of another, false otherwise
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsAReferenceTo);			

			~FbxObjectManaged();
			!FbxObjectManaged();

			/** If this object is a reference clone, this method returns the original object from
			* which this one was cloned.
			* \return The original, or NULL if this object is not a reference clone.
			*/			
			VALUE_PROPERTY_GET_DECLARE(FbxObjectManaged^,ReferenceTo);

			/** Check if any objects were reference cloned from this one.
			* \return true If objects were cloned from this one, false otherwise.
			*/			
			VALUE_PROPERTY_GET_DECLARE(bool,IsReferencedBy);

			/** Get the number of objects that were reference cloned from this one.
			* \return The number of objects cloned from this one.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ReferencedByCount);			

			/** Get a reference clone of this object.
			* \param pIndex Valid values are [0, GetReferencedByCount())
			* \return The requested clone, or NULL if pIndex is out of range.
			*/
			FbxObjectManaged^ GetReferencedBy(int index);

			/** Return the full name of the object.
			* \return Return a \c NULL terminated string.
			*/
			/** Set the name of the object.
			* \param pName A \c NULL terminated string.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,Name);			

			/** Return the name of the object without the namespace qualifier.
			* \return Return the name in a temporary string.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,NameWithoutNameSpacePrefix);			

			/** Return the name of the object with the namespace qualifier.
			* \return Return the name in a temporary string.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,NameWithNameSpacePrefix);				
						
			/** Return the initial name of the object.
			* \return Return a \c NULL terminated string.
			*/			
			/** Set the initial name of the object.
			* \param pName A \c NULL terminated string.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,InitialName);			

			/** Return the namespace of the object.
			* \return Return a \c NULL terminated string.
			*/
			/** Set the namespace of the object.
			* \return Return a \c NULL terminated string.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,NameSpaceOnly);			


			/** Return an array of all the namespace of the object
			* \return Return a \c NULL terminated string.
			*/
			//KArrayTemplate<FbxString*> GetNameSpaceArray( char identifier );

			/** Get the name only (no namespace or prefix) of the object.
			* \return Return a \c NULL terminated string.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,NameOnly);			

			VALUE_PROPERTY_GET_DECLARE(String^,NameSpacePrefix);
			
			static String^ RemovePrefix(String^ name);
			static String^ StripPrefix(FbxStringManaged^ name);
			static String^ StripPrefix(String^ name);

			VALUE_PROPERTY_GET_DECLARE(KFbxObjectID,UniqueID);			
			//@}

			/**
			* \name UpdateId Management
			*/
			//@{		
			enum class FbxUpdateIdType
			{
				Object = KFbxObject::eUpdateId_Object,
				Dependenc = KFbxObject::eUpdateId_Dependency
			};

			virtual kFbxUpdateId GetUpdateId(FbxUpdateIdType updateId);
			kFbxUpdateId GetUpdateId()
			{
				return GetUpdateId(FbxUpdateIdType::Object);
			}

			/** Unload this object content using the offload peripheral currently set in the document
			* then flush it from memory.
			* \return 2 if the object's content is already unloaded or 1 if
			*         this object content has been successfully unloaded to the current
			*         peripheral.
			*
			* \remark If the content is locked more than once or the peripheral cannot handle
			* this object unload or an error occurred, the method will return 0 and the
			* content is not flushed.
			*/
			int ContentUnload();

			/** Load this object content using the offload peripheral currently set in the document.
			* \return 1 if this object content has been successfully loaded from the current
			*         peripheral. 2 If the content is already loaded and 0 if an error occurred or
			*         there is a lock on the object content.
			* \remark On a successful Load attempt, the object content is locked.
			*/
			int ContentLoad();

			/** Returns true if this object content is currently loaded.
			* \remark An object that has not been filled yet must be considered
			* unloaded.
			*/			
			VALUE_PROPERTY_GET_DECLARE(bool,ContentIsLoaded);			

			/**  Decrement the content lock count of an object. If the content lock count of an object
			*  is greater than 0, the content of the object is considered locked.
			*/
			void ContentDecrementLockCount();

			/** Increment the content lock count of an object. If the content lock count of an object
			* is greater than 0, the content of the object is considered locked.
			*/
			void ContentIncrementLockCount();

			/** Returns true if this object content is locked. The content is locked if the content lock count
			* is greater than 0
			* \remark A locked state prevents the object content to be unloaded from memory but
			* does not block the loading.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,ContentIsLocked);			


			/**
			* \name Off-loading Serialization section
			* The methods in this section are, usually, called by
			* a peripheral.
			*/
			//@{
			/** Write the content of the object to the given stream.
			* \param pStream The destination stream.
			* \return True if the content has been successfully processed
			* by the receiving stream.
			*/
			virtual bool ContentWriteTo(FbxStream^ stream);

			/** Read the content of the object from the given stream.
			* \param pStream The source streak.
			* \return True if the object has been able to fill itself with the received data
			* from the stream.
			*/
			virtual bool ContentReadFrom(FbxStream^ stream);
			//@}

			/**
			* \name Selection management
			*/
			//@{
		public:
			virtual VALUE_PROPERTY_GETSET_DECLARE(bool,Selected);						
			//@}

			/**
			* \name Evaluation Info
			*/			
			virtual bool Evaluate(FbxProperty^ prop,FbxEvaluationInfo^ evaluationInfo);			


			/**
			* \name Properties access
			*/
			//@{
		
			FbxProperty^ GetFirstProperty();

			FbxProperty^ GetNextProperty(FbxProperty^ prop);

			/** Find a property using its name and its data type.
			* \param pName The name of the property as a \c NULL terminated string.
			* \param pCaseSensitive
			* \return A valid KFbxProperty if the property was found, else
			*         an invalid KFbxProperty. See KFbxProperty::IsValid()
			*/
			FbxProperty^ FindProperty(String^ name, bool caseSensitive);
			FbxProperty^ FindProperty(String^ name)
			{
				return FindProperty(name, true);
			}

			FbxProperty^ FindProperty(String^ name, FbxDataType^ dataType, bool caseSensitive);
			FbxProperty^ FindProperty(String^ name, FbxDataType^ dataType)
			{
				return FindProperty(name,dataType,false);
			}

			
			FbxProperty^ FindPropertyHierarchical(String^ name, bool caseSensitive);
			FbxProperty^ FindPropertyHierarchical(String^ name)
			{
				return FindPropertyHierarchical(name, true);
			}

			/*FbxProperty^ FindPropertyHierarchical(String^ name, FbxDataType^ dataType, bool caseSensitive);
			FbxProperty^ FindPropertyHierarchical(String^ name, FbxDataType^ dataType)
			{
				return FindPropertyHierarchical(name,dataType,true);
			}*/
			
			FbxProperty^ GetRootProperty();			
			FbxProperty^ GetClassRootProperty();
		public:
			// SrcObjects
			bool ConnectSrcObject(FbxObjectManaged^ obj,FbxConnectionType type);
			bool IsConnectedSrcObject (FbxObjectManaged^ obj);
			bool DisconnectSrcObject (FbxObjectManaged^ obj);

			/*bool DisconnectAllSrcObject();
			bool DisconnectAllSrcObject(FbxCriteria^ criteria);
			bool DisconnectAllSrcObject(FbxClassId^ classId);
			bool DisconnectAllSrcObject(FbxClassId^ classId,FbxCriteria^ criteria);*/

			int GetSrcObjectCount();
			int GetSrcObjectCount(FbxCriteria^ criteria);
			int GetSrcObjectCount(FbxClassId^ classId);
			/*int GetSrcObjectCount(FbxClassId^ classId,FbxCriteria^ criteria);*/

			FbxObjectManaged^ GetSrcObject(int index);
			//FbxObject^ GetSrcObject(FbxCriteria^ criteria,int index);
			FbxObjectManaged^ GetSrcObject(FbxClassId^ classId,int index);
			//FbxObject^ GetSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,int index);

			/*FbxObject^ FindSrcObject(String^ name,int startIndex);
			FbxObject^ FindSrcObject(FbxCriteria^ criteria,String^ name,int startIndex);			
			FbxObject^ FindSrcObject(FbxClassId^ classId,String^ name,int startIndex);			
			FbxObject^ FindSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,String^ name,int startIndex);*/
//
//			template < class T > inline bool DisconnectAllSrcObject (T const *pFBX_TYPE) { return RootProperty.DisconnectAllSrcObject(pFBX_TYPE);   }
//			template < class T > inline bool DisconnectAllSrcObject (T const *pFBX_TYPE,KFbxCriteria const &pCriteria)  { return RootProperty.DisconnectAllSrcObject(pFBX_TYPE,pCriteria);  }
//			template < class T > inline int  GetSrcObjectCount(T const *pFBX_TYPE) const { return RootProperty.GetSrcObjectCount(pFBX_TYPE);    }
//			template < class T > inline int  GetSrcObjectCount(T const *pFBX_TYPE,KFbxCriteria const &pCriteria) const { return RootProperty.GetSrcObjectCount(pFBX_TYPE,pCriteria);    }
//			template < class T > inline T*   GetSrcObject(T const *pFBX_TYPE,int pIndex=0) const { return RootProperty.GetSrcObject(pFBX_TYPE,pIndex);  }
//			template < class T > inline T*   GetSrcObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,int pIndex=0) const { return RootProperty.GetSrcObject(pFBX_TYPE,pCriteria,pIndex);  }
//			template < class T > inline T*   FindSrcObject(T const *pFBX_TYPE,const char *pName,int pStartIndex=0) const { return RootProperty.FindSrcObject(pFBX_TYPE,pName,pStartIndex);  }
//			template < class T > inline T*   FindSrcObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,const char *pName,int pStartIndex=0) const { return RootProperty.FindSrcObject(pFBX_TYPE,pCriteria,pName,pStartIndex);  }
//
			// DstObjects			
			bool ConnectDstObject(FbxObjectManaged^ obj,FbxConnectionType type);
			bool IsConnectedDstObject (FbxObjectManaged^ obj);
			bool DisconnectDstObject (FbxObjectManaged^ obj);
			
			/*bool DisconnectAllDstObject();
			bool DisconnectAllDstObject(FbxCriteria^ criteria);
			bool DisconnectAllDstObject(FbxClassId^ classId);
			bool DisconnectAllDstObject(FbxClassId^ classId,FbxCriteria^ criteria);*/

			int GetDstObjectCount();
			//int GetDstObjectCount(FbxCriteria^ criteria);
			int GetDstObjectCount(FbxClassId^ classId);
			/*int GetDstObjectCount(FbxClassId^ classId,FbxCriteria^ criteria);*/

			//FbxObject^ GetDstObject(int index);
			//FbxObject^ GetDstObject(FbxCriteria^ criteria,int index);
			FbxObjectManaged^ GetDstObject(FbxClassId^ classId,int index);
			//FbxObject^ GetDstObject(FbxClassId^ classId,FbxCriteria^ criteria,int index);

			/*FbxObject^ FindDstObject(String^ name,int startIndex);
			FbxObject^ FindDstObject(FbxCriteria^ criteria,String^ name,int startIndex);			
			FbxObject^ FindDstObject(FbxClassId^ classId,String^ name,int startIndex);			
			FbxObject^ FindDstObject(FbxClassId^ classId,FbxCriteria^ criteria,String^ name,int startIndex);*/
//
//			template < class T > inline bool DisconnectAllDstObject (T const *pFBX_TYPE) { return RootProperty.DisconnectAllDstObject(pFBX_TYPE);   }
//			template < class T > inline bool DisconnectAllDstObject (T const *pFBX_TYPE,KFbxCriteria const &pCriteria)  { return RootProperty.DisconnectAllDstObject(pFBX_TYPE,pCriteria);  }
//			template < class T > inline int  GetDstObjectCount(T const *pFBX_TYPE) const { return RootProperty.GetDstObjectCount(pFBX_TYPE);    }
//			template < class T > inline int  GetDstObjectCount(T const *pFBX_TYPE,KFbxCriteria const &pCriteria) const { return RootProperty.GetDstObjectCount(pFBX_TYPE,pCriteria);    }
//			template < class T > inline T*   GetDstObject(T const *pFBX_TYPE,int pIndex=0) const { return RootProperty.GetDstObject(pFBX_TYPE,pIndex);  }
//			template < class T > inline T*   GetDstObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,int pIndex=0) const { return RootProperty.GetDstObject(pFBX_TYPE,pCriteria,pIndex);  }
//			template < class T > inline T*   FindDstObject(T const *pFBX_TYPE,const char *pName,int pStartIndex=0) const { return RootProperty.FindDstObject(pFBX_TYPE,pName,pStartIndex);  }
//			template < class T > inline T*   FindDstObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,const char *pName,int pStartIndex=0) const { return RootProperty.FindDstObject(pFBX_TYPE,pCriteria,pName,pStartIndex);  }
//			//@}
//
//			// Optimized routine
//			KFbxProperty FindProperty(const char* pName, int pStartIndex, int pSearchDomain) const;
//
//			/**
//			* \name General Property Connection and Relationship Management
//			*/
//			//@{
//			// Properties
//			inline bool         ConnectSrcProperty      (KFbxProperty const & pProperty) { return RootProperty.ConnectSrcProperty(pProperty); }
//			inline bool         IsConnectedSrcProperty  (KFbxProperty const & pProperty) { return RootProperty.IsConnectedSrcProperty(pProperty); }
//			inline bool         DisconnectSrcProperty   (KFbxProperty const & pProperty) { return RootProperty.DisconnectSrcProperty(pProperty); }
//			inline int          GetSrcPropertyCount     () const { return RootProperty.GetSrcPropertyCount(); }
//			inline KFbxProperty GetSrcProperty          (int pIndex=0) const { return RootProperty.GetSrcProperty(pIndex); }
//			inline KFbxProperty FindSrcProperty         (const char *pName,int pStartIndex=0) const { return RootProperty.FindSrcProperty(pName,pStartIndex); }
//
//			inline bool         ConnectDstProperty      (KFbxProperty const & pProperty) { return RootProperty.ConnectDstProperty(pProperty); }
//			inline bool         IsConnectedDstProperty  (KFbxProperty const & pProperty) { return RootProperty.IsConnectedDstProperty(pProperty); }
//			inline bool         DisconnectDstProperty   (KFbxProperty const & pProperty) { return RootProperty.DisconnectDstProperty(pProperty); }
//			inline int          GetDstPropertyCount     () const { return RootProperty.GetDstPropertyCount(); }
//			inline KFbxProperty GetDstProperty          (int pIndex=0) const { return RootProperty.GetDstProperty(pIndex); }
//			inline KFbxProperty FindDstProperty         (const char *pName,int pStartIndex=0) const { return RootProperty.FindDstProperty(pName,pStartIndex); }
//			//@}
//
//			/**
//			* \name User data
//			*/
//			//@{
//			void        SetUserDataPtr(KFbxObjectID const& pUserID, void* pUserData);
//			void*       GetUserDataPtr(KFbxObjectID const& pUserID) const;
//
//			inline void SetUserDataPtr(void* pUserData)     { SetUserDataPtr( GetUniqueID(), pUserData ); }
//			inline void* GetUserDataPtr() const             { return GetUserDataPtr( GetUniqueID() ); }
//			//@}
//
//
			/**
			* \name Document Management
			*/
			//@{
			/** Get a pointer to the document containing this object.
			* \return Return a pointer to the document containing this object or \c NULL if the
			* object does not belong to any document.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDocument,Document);

			/** Get a const pointer to the root document containing this object.
			* \return Return a const pointer to the root document containing this object or \c NULL if the
			* object does not belong to any document.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxDocument,RootDocument);

			/** Get a pointer to the scene containing this object.
			* \return Return a pointer to the scene containing this object or \c NULL if the
			* object does not belong to any scene.
			*/
			REF_PROPERTY_GET_DECLARE(FbxScene,Scene);
			//@}


			/**
			* \name Logging.
			*/
			//@{
			/** Emit a message in all available message emitter in the document or SDK manager.
			* \param pMessage the message to emit. Ownership is transfered, do not delete.
			*/
			//void EmitMessage(KFbxMessage * pMessage) const;
			//@}

			/**
			* \name Localization helper.
			*/
			//@{
			/** Localization helper function. Calls the FBX SDK manager implementation.
			* sub-classes which manage their own localization could over-ride this.
			* \param pID the identifier for the text to localize.
			* \param pDefault the default text. Uses pID if NULL.
			* \return the potentially localized text. May return the parameter passed in.
			*/
			virtual String^ Localize(String^ id,String^ Default);
			//@}

			/**
			* \name Application Implementation Management
			*/
			//@{
			//! Get a handle on the parent library if exists.
			REF_PROPERTY_GET_DECLARE(FbxLibrary,ParentLibrary);

			/** Adds an implementation.
			*
			* \param pImplementation a handle on an implementation
			*
			* \returns true on success, false otherwise
			*
			* \remarks to succeed this function needs to be called with an
			* implementation that has not already been added to this node.
			*/
			//bool AddImplementation(FbxImplementation* pImplementation);

			/** Removes an implementation.
			*
			* \param pImplementation a handle on an implementation
			*
			* \returns true on success, false otherwise
			*
			* \remarks to succeed this function needs to be called with an
			* implementation that has already been added to this node.
			*/
			//bool RemoveImplementation(KFbxImplementation* pImplementation);

			//! Tells if this shading node has a default implementation
			//bool HasDefaultImplementation(void) const;

			//! Returns the default implementation.
			//KFbxImplementation* GetDefaultImplementation(void) const;

			/** Sets the default implementation.
			*
			* \param pImplementation a handle on an implementation
			*
			* \returns true on success, false otherwise
			*
			* \remarks to succeed this function needs to be called with an
			* implementation that has already been added to this node.
			*/
			//bool SetDefaultImplementation(KFbxImplementation* pImplementation);

			/** Returns the number of implementations that correspond to a given criteria
			*
			* \param pCriteria filtering criteria that identifyies the kind of
			* implementations to take into account.
			*
			* \returns the number of implementation(s)
			*/
			/*int GetImplementationCount(
				const KFbxImplementationFilter* pCriteria = NULL
				) const;*/

			/** Returns a handle on the (pIndex)th implementation that corresponds
			* to the given criteria
			* \param pIndex
			* \param pCriteria filtering criteria that identifyies the kind of
			* implementations to take into account.
			*
			* \remarks returns NULL if the criteria or the index are invalid
			*/
			/*KFbxImplementation* GetImplementation(
				int                             pIndex,
				const KFbxImplementationFilter* pCriteria = NULL
				) const;*/
			//@}

			/**
			* \name Object Storage && Retrieval
			*/
			//@{
		public:
			virtual VALUE_PROPERTY_GETSET_DECLARE(String^,Url);
			virtual bool PopulateLoadSettings(FbxObjectManaged^ settings,String^ fileName);
			virtual bool Load(String^ fileName);
			virtual bool PopulateSaveSettings(FbxObjectManaged^ settings,String^ fileName);
			virtual bool Save(String^ fileName);
			//@}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		public:
			//virtual KFbxSdkManager* GetFbxSdkManager() const;
			virtual FbxClassId^ GetRuntimeClassId();

			enum class ObjectFlag
			{
				none                   = 0x00000000,
				SystemFlag            = 0x00000001,
				SavableFlag           = 0x00000002,
				HiddenFlag            = 0x00000008,

				SystemFlags           = 0x0000ffff,

				UserFlags             = 0x000f0000,

				// These flags are not saved to the fbx file
				SystemRuntimeFlags   = 0x0ff00000,

				ContentLoadedFlag    = 0x00100000,

				UserRuntimeFirstFlag = 0x10000000,
				UuserRuntimeFlags    = 0xf0000000,

				RuntimeFlags         = 0xfff00000
			};

			void SetObjectFlags(ObjectFlag flags, bool value);
			bool GetObjectFlags(ObjectFlag flags);

			// All flags replaced at once. This includes overriding the runtime flags, so
			// most likely you'd want to do something like this:
			//						
			VALUE_PROPERTY_GETSET_DECLARE(kUInt,ObjectFlags); // All flags at once, as a bitmask		
			//FbxObjectHandle &GetPropertyHandle() { return RootProperty.mPropertyHandle; }
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
			
		};

	}
}