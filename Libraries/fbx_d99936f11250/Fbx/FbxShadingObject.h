#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		/** \brief This class is a object for take nodes which contain shading data.
		* \nosubgrouping
		* \par
		* This class is similar to KFbxTakeNodeContainer.
		*/
		public ref class FbxShadingObject : FbxTakeNodeContainer
		{
		internal:
			FbxShadingObject(KFbxShadingObject* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}

			//FBXOBJECT_DECLARE(FbxShadingObject);
		};

	}
}