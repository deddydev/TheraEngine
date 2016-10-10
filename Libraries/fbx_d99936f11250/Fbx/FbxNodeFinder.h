#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <kfbxplugins/kfbxnodefinder.h>




{
	namespace FbxSDK
	{
		ref class FbxNode;
		//! Class KFbxNodeFinder
		public ref class FbxNodeFinder : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxNodeFinder,KFbxNodeFinder);
			INATIVEPOINTER_DECLARE(FbxNodeFinder,KFbxNodeFinder);		

		public:
			//! Constructor.
			//DEFAULT_CONSTRUCTOR(FbxNodeFinder,KFbxNodeFinder);			

			/** Find all the node corresponding to the research criterium.
			*	\param iSearchRoot
			*	\return
			*/			
			array<FbxNode^>^ Apply(FbxNode^ searchRoot);

			//! Reset the finder object
			virtual void Reset();		
		};

	}
}