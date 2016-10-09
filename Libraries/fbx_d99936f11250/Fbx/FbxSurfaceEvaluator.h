#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxWeightedMapping;
		/***************************************************************************
		Class KFBXSurfaceEvaluator
		***************************************************************************/

		public ref class FbxSurfaceEvaluator
		{
			//BASIC_CLASS_DECLARE(FbxSurfaceEvaluator,KFBXSurfaceEvaluator);
			//INATIVEPOINTER_DECLARE(FbxSurfaceEvaluator,KFBXSurfaceEvaluator,"KFBXSurfaceEvaluator");

		public :

			//! Constructor.
			//DEFAULT_CONSTRUCTOR(FbxSurfaceEvaluator,KFBXSurfaceEvaluator);			

			//!
			//virtual void EvaluateSurface(FbxWeightedMapping^ weightedMapping);

			//// Evaluation settings function

			////!
			//virtual VALUE_PROPERTY_GETSET_DECLARE(int,EvaluationModeU);

			////!
			//virtual VALUE_PROPERTY_GETSET_DECLARE(int,EvaluationModeV);

			////!
			//virtual VALUE_PROPERTY_GETSET_DECLARE(kUInt,EvaluationStepU);			

			////!
			//virtual VALUE_PROPERTY_GETSET_DECLARE(kUInt,EvaluationStepV);			

			//// Output Data

			////!
			//virtual VALUE_PROPERTY_GET_DECLARE(bool,BottomCapU);

			////!
			//virtual VALUE_PROPERTY_GET_DECLARE(bool,TopCapU);			

			////!

			//virtual VALUE_PROPERTY_GET_DECLARE(bool,BottomCapV);

			////!
			//virtual VALUE_PROPERTY_GET_DECLARE(bool,TopCapV);			

			////!
			//virtual void SetDestinationArray(array<double>^ arr);

			////!
			//virtual VALUE_PROPERTY_GET_DECLARE(kUInt,CurvePointCountX);			

			////!
			//virtual VALUE_PROPERTY_GET_DECLARE(kUInt,CurvePointCountY);			

			////!
			//virtual void SetDestinationNormalArray(array<double>^ arr);

			//// Input Data

			////!
			//virtual void SetSurfaceTensionU(double tensionU);

			////!
			//virtual void SetSurfaceTensionV(double tensionV);

			////!
			//virtual void SetSourceArray(array<double> arr, kUInt nPointX, kUInt nPointY);

			////!
			//virtual void SetAuxSourceArray(int identification, array<double>^ arr);

			////!
			//void Destroy(int isLocal);
			//void Destroy()
			//{
			//	Destroy(false);
			//}

			////!
			////void Set_U_Blending_Parameters(const double pMatrice4x4[16]);

			////!
			////void Set_V_Blending_Parameters (const double pMatrice4x4[16]);

			////!
			//void SetOrderU(kUInt orderU);

			////!
			//void SetOrderV(kUInt orderV);

			////!
			//void SetAfterStepU(kUInt uf);

			////!
			//void SetAfterStepV(kUInt vf);		
		};

	}
}