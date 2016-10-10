#pragma once
#include "stdafx.h"
#include "FbxGeometryBase.h"


{
	namespace FbxSDK
	{	
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** A shape describes the deformation on a set of control points.
		* \nosubgrouping
		* Shapes are associated with instances of class KFbxGeometry.
		*/
		public ref class FbxShape : FbxGeometryBase
		{
			REF_DECLARE(FbxEmitter,KFbxShape); 
		internal:
			FbxShape(KFbxShape* instance) :FbxGeometryBase(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxShape);
		public:

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
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