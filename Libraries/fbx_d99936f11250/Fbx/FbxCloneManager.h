#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"


namespace Skill
{
	namespace FbxSDK
	{
		//ref class FbxObject;		
		/** \brief The clone manager is a utility for cloning entire networks of KFbxObjects.
		*        Options are availible for specifying how the clones inherit the connections
		*        of the original.
		*
		* \nosubgrouping
		*/
		public ref class FbxCloneManager : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCloneManager,KFbxCloneManager);
			INATIVEPOINTER_DECLARE(FbxCloneManager,KFbxCloneManager);

		public:		

			//! Maximum depth to clone dependents.
			//static property int MaximumCloneDepth
			//{
			//	int get();
			//}

			////! connect to objects that are connected to original object						
			//static property int ConnectToOriginal
			//{
			//	int get();
			//}

			///** connect to clones of objects that are connected to original object
			//* (only if those original objects are also in the clone set)
			//*/						
			//static property int ConnectToClone
			//{
			//	int get();
			//}

			/** This represents an element in a set of objects to be cloned
			*/
			//ref struct CloneSetElement
			//{
			//internal:
			//	KFbxCloneManager::CloneSetElement* setClone;
			//	bool isNew;
			//	CloneSetElement(KFbxCloneManager::CloneSetElement* s);
			//public:
			//	CloneSetElement( int srcPolicy,
			//		int externalDstPolicy,
			//		FbxObject::CloneType cloneType);
			//	CloneSetElement();
			//	~CloneSetElement();
			//	!CloneSetElement();

			//	//! the type of cloning to perform
			//	property FbxObject::CloneType Type
			//	{
			//		FbxObject::CloneType  get();
			//		void set(FbxObject::CloneType value);

			//	}

			//	/** Policy on how to handle source connections on the original object. Valid values are 0
			//	* or any bitwise OR'd combination of sConnectToOriginal, and sConnectToClone.
			//	*/
			//	property int SrcPolicy
			//	{
			//		int get();
			//		void set(int value);
			//	}

			//	/** policy on how to handle destination connections on the original object to
			//	* objects NOT in the clone set. (Destination connections to objects in the set
			//	* are handled by that object's source policy) Valid values are 0 or sConnectToOriginal.
			//	*/							
			//	property int ExternalDstPolicy
			//	{
			//		int get();
			//		void set(int value);
			//	}

			//	/** This is a pointer to the newly created clone.
			//	* It is set after the call to KFbxCloneManager::Clone()
			//	*/
			//	property FbxObject^ ObjectClone
			//	{
			//		FbxObject^ get();
			//	}
			//};

			/** Functor to compare object pointers
			*/
			/*ref class FbxObjectCompare : System::Collections::Generic::IComparer<FbxObject^>
			{
			public:
				virtual int Compare(FbxObject^ obj1,FbxObject^ obj2);
			};*/

			/** The CloneSet is a collection of pointers to objects that will be cloned in Clone()
			* Attached to each object is a CloneSetElement. Its member variables dictate how
			* the corresponding object will be cloned, and how it will inherit connections
			* on the original object.
			*/
			//typedef KMap<KFbxObject*,CloneSetElement,KFbxObjectCompare> CloneSet;

			/** Constructor
			*/
			DEFAULT_CONSTRUCTOR(FbxCloneManager,KFbxCloneManager);

			/** Destructor
			*/
			//virtual ~KFbxCloneManager();

			/** Clone all objects in the set using the given policies for duplication
			* of connections. Each CloneSetElement in the set will have its mObjectClone
			* pointer set to the newly created clone.
			* \param pSet Set of objects to clone
			* \param pContainer This object (typically a scene or document) will contain the new clones
			* \return true if all objects were cloned, false otherwise.
			*/
			/*virtual bool Clone( FbxCloneManager::c CloneSet& pSet, KFbxObject* pContainer = NULL ) const;*/

			//			/** Add all dependents of the given object to the CloneSet.
			//			* Dependents of items already in the set are ignored to prevent
			//			* infinite recursion on cyclic dependencies.
			//			* \param pSet The set to add items.
			//			* \param pObject Object to add dependents to
			//			* \param pCloneOptions  
			//			* \param pTypes Types of dependent objects to consider
			//			* \param pDepth Maximum recursive depth. Valid range is [0,sMaximumCloneDepth]
			//			*/
			//			virtual void AddDependents( CloneSet& pSet,
			//				const KFbxObject* pObject,
			//				const CloneSetElement& pCloneOptions = CloneSetElement(),
			//				KFbxCriteria pTypes = KFbxCriteria::ObjectType(KFbxObject::ClassId),
			//				int pDepth = sMaximumCloneDepth ) const;
			//
			//
			//			///////////////////////////////////////////////////////////////////////////////
			//			//
			//			//  WARNING!
			//			//
			//			//  Anything beyond these lines may not be documented accurately and is
			//			//  subject to change without notice.
			//			//
			//			///////////////////////////////////////////////////////////////////////////////			
		};

	}
}