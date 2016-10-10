#pragma once
#include "stdafx.h"
#include "FbxDocument.h"



{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxObjectManaged;
		ref class FbxCriteria;
		/** \brief This object represents a shading node library.
		* \nosubgrouping
		*/
		public ref class FbxLibrary : FbxDocumentManaged
		{			
		internal:
			REF_DECLARE(FbxEmitter,KFbxLibrary);
			FbxLibrary(KFbxLibrary* instance) : FbxDocumentManaged(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxLibrary);
		protected:
			virtual void CollectManagedMemory()override;

		public:			

			//! Get a handle on the parent library if exists.
			REF_PROPERTY_GET_DECLARE(FbxLibrary,ParentLibrary);
			
			VALUE_PROPERTY_GETSET_DECLARE(bool,IsSystemLibrary);

			/**
			*  The prefix must not include the dash and language code, nor
			*  must it contain the extension.  But it can contain a folder,
			*  or sub-folder, if you want, such as:
			*
			*  locales/mydocloc
			*
			*  This will be resolved using the XRef Manager, with priority
			*  given to the library's .fbm folder, if it has one.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^ ,LocalizationBaseNamePrefix);

			// 
			//
			// Sub library
			//
			// 

			//! Adds a sub Library
			bool AddSubLibrary(FbxLibrary^ subLibrary);

			//! Removes a sub Library
			bool RemoveSubLibrary(FbxLibrary^ subLibrary);

			//! Gets the total number of sub libraries
			VALUE_PROPERTY_GET_DECLARE(int,SubLibraryCount);

			//! Sub library accessor
			FbxLibrary^ GetSubLibrary(int index);

			/** Get the number of sub libraries that contains shading node according
			* to their implementations.
			*
			* \param pCriteria filtering criteria that identifies the kind of
			* implementations to take into account.
			*
			* \returns the number of sub-libraries corresponding to the filtering parameters
			*/
			/*int GetSubLibraryCount(
				const KFbxImplementationFilter & pCriteria
				) const;*/

			/** Get a handle on the (pIndex)th sub-library that corresponds to the given filtering parameters.
			* \param pIndex
			* \param pCriteria filtering criteria that identifies the kind of
			* implementations to take into account.
			*
			* \returns a handle on the (pIndex)th sub-library that corresponds to the given filtering parameters
			*/
			/*KFbxLibrary* GetSubLibrary(
				int pIndex,
				const KFbxImplementationFilter & pCriteria
				) const;*/


			FbxObjectManaged^ CloneAsset(FbxObjectManaged^ toClone, FbxObjectManaged^ optionalDestinationContainer);

			//
			// Returns a criteria filter which can be used to ... filter ... objects
			// when iterating items in the library; only real 'assets' will be returned,
			// rather than FBX support objects.  This includes, at this time,
			// lights, environments, materials and textures (maps)
			//
			// This is typically used to IMPORT from a library.
			//
			static FbxCriteria^ GetAssetCriteriaFilter();

			//
			// Returns a filter which should be used when cloning / exporting objects --
			// this filters out objects that should stay in the asset library.
			//
			// This is typically used to EXPORT from a library (or CLONE from a library,
			// although CloneAsset does all the nitty gritty for you)
			//
			static FbxCriteria^ GetAssetDependentsFilter();

			/**
			* Transfer ownership from the src library to us for all assets meeting the filter.
			* Name clashing and other details are assumed to have been resolved beforehand.
			*
			* External asset files required by the assets are copied over -- not moved.  It's
			* up to the owner of pSrcLibrary to clean up (maybe they can't; the files may
			* be on a read-only transport).  If this document hasn't been commited yet, then
			* the assets WON'T be copied.
			*
			* Returns true if no assets meeting the filter were skipped.  If there are no
			* assets meeting the filter, then true would be returned, as nothing was skipped.
			*
			* This may leave the source library in an invalid state, if for instance you
			* decide to transfer texture objects to our library, but you keep materials in
			* the source library.
			*
			* To safeguard against this, the transfer will disconnect objects, and you'd thus
			* be left with materials without textures.
			*
			* When transfering an object, all its dependents come with it.  If you move
			* a material, it WILL grab its textures.  Just not the other way around.
			*
			**/
			bool ImportAssets(FbxLibrary^ srcLibrary);
			bool ImportAssets(FbxLibrary^ srcLibrary, FbxCriteria^ criteria);


			/** Return a new instance of a member of the library.
			* This instantiates the first object found that matches the filter.
			* \param pFBX_TYPE The type of member
			* \param pFilter A user specified filter
			* \param pRecurse Check sublibraries
			* \param pOptContainer Optional container for the cloned asset
			* \return A new instance of the member. Note that the new member is not inserted into this library.
			*/
			//template < class T > T* InstantiateMember( T const* pFBX_TYPE, const KFbxObjectFilter& pFilter, bool pRecurse = true, KFbxObject* pOptContainer = NULL);


			// 
			//
			// Localization
			//
			// 
			/** Get the localization manager for the library.
			*/

			//KFbxLocalizationManager & GetLocalizationManager() const;

			/** Localization helper function. Calls the FBX SDK manager implementation.
			* sub-classes which manage their own localization could over-ride this.
			* \param pID the identifier for the text to localize.
			* \param pDefault the default text. Uses pID if NULL.
			* \return the potentially localized text. May return the parameter passed in.
			*/
			//virtual String^ Localize(String^ ID, String^ Default);

			// 
			//
			// Shading Node
			//
			// 

			//! Adds a shading node
			bool AddShadingObject(FbxObjectManaged^ shadingObject);

			//! Removes a shading node
			bool RemoveShadingObject(FbxObjectManaged^ shadingObject);

			//! Gets the total number of shading nodes
			VALUE_PROPERTY_GET_DECLARE(int,ShadingObjectCount);

			//! Shading node accessor
			FbxObjectManaged^ GetShadingObject(int index);

			/** Get the number of shading nodes according to their implementations.
			*
			* \param pCriteria filtering criteria that identifies the kind of
			* implementations to take into account.
			*
			* \returns the number of shading nodes corresponding to the filtering parameters
			*/
			/*int GetShadingObjectCount(
				const KFbxImplementationFilter & pCriteria
				) const;*/

			/** Get a handle on the (pIndex)th sub-library that corresponds to the given filtering parameters.
			* \param pIndex
			* \param pCriteria filtering criteria that identifies the kind of
			* implementations to take into account.
			*
			* \returns a handle on the (pIndex)th shading node that corresponds to the given filtering parameters
			*/
			/*KFbxObject* GetShadingObject(
				int pIndex,
				const KFbxImplementationFilter & pCriteria
				) const;*/

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
			// Clone
			CLONE_DECLARE();		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};


	}
}