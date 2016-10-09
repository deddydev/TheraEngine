#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxObjectManaged;
		/** \brief This object represents a filter criteria on an object.
		* \nosubgrouping
		*/
		public ref class FbxObjectFilter : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxObjectFilter,KFbxObjectFilter);
			INATIVEPOINTER_DECLARE(FbxObjectFilter,KFbxObjectFilter);

		public:

			//! Tells if this filter match the given object
			virtual bool Match(FbxObjectManaged^ objectPtr);

			//! Tells if this filter does NOT match the given object
			virtual bool NotMatch(FbxObjectManaged^ objectPtr);			
		};

		/**\brief This class represents a name filter on an object.
		*\nosubgrouping
		*/
		public ref class FbxNameFilter : FbxObjectFilter
		{
			REF_DECLARE(FbxObjectFilter,KFbxNameFilter);
		internal:
			FbxNameFilter(KFbxNameFilter* instance) :FbxObjectFilter(instance)
			{
				_Free = false;
			}
		public:
			/**
			* \name Constructor and Destructor
			*/
			//@{
			//!Constructor
			//FbxNameFilter(String^ targetName);
			//@}			
		
		};

	}
}