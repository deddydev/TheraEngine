#pragma once
#include "stdafx.h"
#include "FbxShadingObject.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/**FBX SDK shading node class
		*\nosubgrouping
		*/
		public ref class FbxShadingNode : FbxShadingObject
		{
			REF_DECLARE(FbxEmitter,KFbxShadingNode);
		internal:
			FbxShadingNode(KFbxShadingNode* instance) : FbxShadingObject(instance)
			{
				_Free = false;
			}			

			FBXOBJECT_DECLARE(FbxShadingNode);

		protected:
			/**
			* \name Constructor and Destructor
			*/

			//@{

			/** Constructor.
			* \param pManager          FBX SDK object Manager
			* \param pName             Object name.
			*/
			//FbxShadingNode(FbxSdkManager^ manager, char const* pName);			
		};


	}
}