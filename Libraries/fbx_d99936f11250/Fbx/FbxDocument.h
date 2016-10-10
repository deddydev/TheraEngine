#pragma once
#include "stdafx.h"
#include "FbxCollection.h"


{
	namespace FbxSDK
	{
		namespace Arrays
		{		
			ref class FbxStringRefArray;
		}
	}
}

using ::FbxSDK::Arrays;



{
	namespace FbxSDK
	{		
		ref class FbxPeripheral;
		ref class FbxDocumentInfo;
		ref class FbxTakeInfo;
		ref class FbxErrorManaged;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** This class contains objects
		* \nosubgrouping
		* \par
		* This class also provides access to take information.
		*
		*/
		public ref class FbxDocumentManaged : FbxCollectionManaged
		{
		internal:
			REF_DECLARE(FbxEmitter,FbxDocument);
			FbxDocumentManaged(FbxDocument* instance) : FbxCollectionManaged(instance)
			{
				_Free = false;
			}			

			/**
			* \name Properties
			*/
			//@{
			//KFbxTypedProperty<fbxReference*>                    Roots;
			//@}

			/**
			* \name Document Member Manager
			*/
			//@{
		protected:
			virtual void CollectManagedMemory() override;
		public:
			FBXOBJECT_DECLARE(FbxDocumentManaged);
			//! Delete all contained objects.
			/*virtual void  Clear()
			{
			((KFbxDocument*)emitter)->Clear();
			}*/
			//! Add a member.
			/*void AddRootMember(FbxObject^ %member)
			{
			((KFbxDocument*)emitter)->AddRootMember((KFbxObject*)member->emitter);
			}*/
			//! Remove a member.
			/*void RootRootRemoveMember(FbxObject^ %member)
			{
				((KFbxDocument*)emitter)->RootRootRemoveMember((KFbxObject*)member->emitter);
			}*/
			//! Find a member.
			//template <class T> inline T *       FindRootMember(T const *pfbxType, char *pName) { return Roots.FindSrcObject(pfbxType, pName); }

			//! Return the number of objects in the collection.
			/*property int RootMemberCount
			{
				int get(){return ((KFbxDocument*)emitter)->GetRootMemberCount();}
			}*/
			//! Return the number of objects in the collection.
			//template < class T > inline int     GetRootMemberCount (T const *pFBX_TYPE) const { return Roots.GetSrcObjectCount(T::ClassId); }
			/*int GetRootMemberCount( FbxCriteria^ criteria )
			{
				return ((KFbxDocument*)emitter)->GetRootMemberCount(*criteria->criteria);
			}*/

			//! Return the index'th member of the collection.
			/*FbxObject^ GetRootMember (int index)
			{
				return gcnew FbxObject(((KFbxDocument*)emitter)->GetRootMember(index));
			}*/
			//! Return the index'th member of the collection.
			//template < class T > inline T*      GetRootMember (T const *pFBX_TYPE, int pIndex=0) const  { return (T *)Roots.GetSrcObject(T::ClassId,pIndex); }
			/*FbxObject^ GetRootMember (FbxCriteria^ criteria, int index)
			{
				return gcnew FbxObject(((KFbxDocument*)emitter)->GetRootMember(*criteria->criteria,index));
			}*/
			//! Is an object part of the collection.

			/*virtual bool IsRootMember(FbxObject^ %member)
			{
				return ((KFbxDocument*)emitter)->IsRootMember((KFbxObject*)member->emitter);
			}*/
			//@}


			/**
			* \name Scene information
			*/
			//@{
			/** Get the scene information.
			* \return Pointer to the scene information object.
			*/
			/** Set the scene information.
			* \param pSceneInfo Pointer to the scene information object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxDocumentInfo,DocumentInfo);	
			/**
			* \name Offloading management
			*
			* NOTE: The document does not own the peripheral therefore
			* it will not attempt to delete it at destruction time. Also, cloning
			* the document will share the pointer to the peripheral across
			* the cloned objects. And so will do the assignment operator.
			*/
			//@{
		public:					
			/** Retrieve the peripheral of that object.
			* \return Return the current peripheral for that object
			* \remark A peripheral manipulates the content of an object for instance, a peripheral can load the connections of an object on demand.
			*/
			/** Set the current peripheral.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxPeripheral,Peripheral);	

			/** Offload all the unloadable objects contained in the document using the
			* currently set offload peripheral.
			* \return The number of objects that the document have been able to unload.
			* \remark Errors that occured during the operation can be inspected using the
			* GetError() method.
			*/
			int UnloadContent();

			/** Load all the objects contained in the document with the data from the
			* currently set offload peripheral.
			* \return The number of objects reloaded.
			* \remark Errors that occured during the operation can be inspected using the
			* GetError() method.
			*/
			int LoadContent();

			//@}

			/**
			* \name Referencing management
			*/
			//@{

			/**
			* Erase then fills an array of pointers to documents that reference objects in this document.
			*
			* \param pReferencingDocuments array of pointers to documents
			* \returns number of documents that reference objects in this document.
			*/
			/*int GetReferencingDocuments(KArrayTemplate<KFbxDocument*> & pReferencingDocuments) const;*/

			/**
			* Erase then fills an array of pointers to objects in a given document (pFromDoc)
			* that reference objects in this document.
			*
			* \param pFromDoc pointer to the document containing referencing objects.
			* \param pReferencingObjects array of pointers to referencing objects.
			* \returns number of objects that reference objects in this document.
			*/
			/*int GetReferencingObjects(KFbxDocument const * pFromDoc, KArrayTemplate<KFbxObject*> & pReferencingObjects) const;*/

			/**
			* Erase then fills an array of pointers to documents that are referenced by objects in this document.
			*
			* \param pReferencedDocuments array of pointers to documents
			* \returns number of documents that are referenced by objects in this document.
			*/
			//int GetReferencedDocuments(KArrayTemplate<KFbxDocument*> & pReferencedDocuments) const;

			/**
			* Erase then fills an array of pointers to objects in a given document (pToDoc)
			* that are referenced by objects in this document.
			*
			* \param pToDoc pointer to the document containing referenced objects.
			* \param pReferencedObjects array of pointers to referenced objects.
			* \returns number of objects that are referenced by objects in this document.
			*/
			//int GetReferencedObjects(KFbxDocument const * pToDoc, KArrayTemplate<KFbxObject*> & pReferencedObjects) const;

			// Gets the path string to the root document if any.
			REF_PROPERTY_GET_DECLARE(FbxStringManaged,PathToRootDocument);

			// Gets the document path to the root document if any.
			//void GetDocumentPathToRootDocument(KArrayTemplate<KFbxDocument*> & pDocumentPath, bool pFirstCall = true) const;

			// Tells if this document is a root document.
			/*bool IsARootDocument()
			{
				return ((KFbxDocument*)emitter)->IsARootDocument();
			}*/
			//@}

			/**
			* \name Take Management
			*/
			//@{

			/** Create a take.
			* \param pName Created take name.
			* \return \c true if not a single node, texture or material in the
			* hierarchy had a take with this name before.
			* \return \c false if at least one node, texture or material in the
			* hierarchy had a take with this name before.
			* \return In the last case, KFbxDocument::GetLastErrorID() will return
			* \c eTAKE_ERROR.
			* \remarks This function will create a new take node for every node,
			* texture and material in the hierarchy. It may be more efficient to call
			* KFbxTakeNodeContainer::CreateTakeNode() on the relevant nodes, textures
			* and materials if a take only has a few of them with animation data.
			*/
			bool CreateTake(String^ name);

			/** Remove a take.
			* \param pName Name of the take to remove.
			* \return \c true if every node, texture and material in the hierarchy
			* have a take with this name.
			* \return \c false if at least one node, texture or material in the
			* hierarchy don't have a take with this name.
			* \return In the last case, KFbxDocument::GetLastErrorID() will return
			* \c eTAKE_ERROR.
			* \remarks Scans the node hierarchy, the texture list and the material
			* list to remove all take nodes found with that name.
			*/
			bool RemoveTake(String^ name);

			/** Set the current take.
			* \param pName Name of the take to set.
			* \return \c true if every node, texture and material in the hierarchy
			* have a take with this name.
			* \return \c false if at least one node, texture or material in the
			* hierarchy don't have a take with this name.
			* \return In the last case, KFbxDocument::GetLastErrorID() will return
			* \c eTAKE_ERROR.
			* \remarks Scans the node hierarchy, the texture list and the material
			* list to set all take nodes found with that name.
			* \remarks All nodes, textures and materials without a take node of the
			* requested name are set to default take node. It means that, if a node,
			* texture or material does not have the requested take, it is assumed
			* that this node is not animated in this take.
			*/
			bool SetCurrentTake(String^ name);

			/** Get current take name.
			* \return Current take name.
			* \return An empty string if the document has not been imported from a file
			* and function KFbxDocument::SetCurrentTake() has not been called previously
			* at least once.
			*/
			String^ GetCurrentTakeName();

			/** Fill a string array with all existing take names.
			* \param pNameArray An array of string objects.
			* \remarks Scans the node hierarchy, the texture list and the material
			* list to find all existing take node names.
			* \remarks The array of string is cleared before scanning the node
			* hierarchy.
			*/
			void FillTakeNameArray(FbxStringRefArray^ nameArray);

			//@}

			/**
			* \name Take Information Management
			*/
			//@{

			/** Set take information about an available take.
			* \param pTakeInfo Take information, field KFbxTakeInfo::mName specifies
			* the targeted take.
			* \return \c true if take is found and take information set.
			*/
			bool SetTakeInfo(FbxTakeInfo^ takeInfo);
			/** Get take information about an available take.
			* \param pTakeName Take name.
			* \return Pointer to take information or \c NULL if take isn't found or
			*   has no information set.
			*/
			FbxTakeInfo^ GetTakeInfo(FbxStringManaged^ takeName);
			//@}

			/**
			* \name Error Management
			* The same error object is shared among instances of this class.
			*/
			//@{

			/** Retrieve error object.
			* \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** Error identifiers.
			* Most of these are only used internally.
			*/
			enum class Error
			{
				TakeError = KFbxDocument::eTAKE_ERROR,
				FbxObjectIsNull = KFbxDocument::eKFBX_OBJECT_IS_NULL,
				FbxObjectAlreadyOwned = KFbxDocument::eKFBX_OBJECT_ALREADY_OWNED,
				FbxObjectUnknown = KFbxDocument::eKFBX_OBJECT_UNKNOWN,
				FbxMissingPeripheral = KFbxDocument::eKFBX_MISSING_PERIPHERAL,
				FbxObjectPeripheralFailure = KFbxDocument::eKFBX_OBJECT_PERIPHERAL_FAILURE,
				ErrorCount = KFbxDocument::eERROR_COUNT
			};

			/** Get last error code.
			* \return Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			* \return Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}

			//@}

			///////////////////////////////////////////////////////////////////////////////
			//  WARNING!
			//  Anything beyond these lines may not be Documented accurately and is
			//  subject to change without notice.
			///////////////////////////////////////////////////////////////////////////////
#ifndef DOXYGEN_SHOULD_SKIP_THIS
		public:
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};


	}
}