#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <kfbxplugins/kfbxlimitsutilities.h>


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxNodeLimits;
		ref class FbxNode;
		ref class FbxVector4;

		public ref class FbxLimitsUtilities : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxLimitsUtilities,KFbxLimitsUtilities);
			INATIVEPOINTER_DECLARE(FbxLimitsUtilities,KFbxLimitsUtilities);		
		public:

			enum class LimitType
			{
				T = KFbxLimitsUtilities::eT,
				R = KFbxLimitsUtilities::eR,
				S = KFbxLimitsUtilities::eS,
			};

			enum class RotationType 
			{ 
				Quaternion = KFbxLimitsUtilities::eROTATION_TYPE_QUATERNION, 
				Euler = KFbxLimitsUtilities::eROTATION_TYPE_EULER, 
			};

			enum class RotationClampType
			{ 
				Rectangular = KFbxLimitsUtilities::eROTATION_CLAMP_TYPE_RECTANGULAR, 
				Elipsoid = KFbxLimitsUtilities::eROTATION_CLAMP_TYPE_ELIPSOID, 
			};


			FbxLimitsUtilities(FbxNodeLimits^ limits);
			REF_PROPERTY_GETSET_DECLARE(FbxNodeLimits,Limits);

			void SetAuto(FbxLimitsUtilities::LimitType type, bool Auto);
			bool GetAuto(FbxLimitsUtilities::LimitType type);

			void SetEnable(FbxLimitsUtilities::LimitType type, bool enable);
			bool GetEnable(FbxLimitsUtilities::LimitType type);

			void SetDefault(FbxLimitsUtilities::LimitType type, FbxVector4^ Default);
			FbxVector4^ GetDefault(FbxLimitsUtilities::LimitType type);

			void SetMin(FbxLimitsUtilities::LimitType type, FbxVector4^ min);
			FbxVector4^ GetMin(FbxLimitsUtilities::LimitType type);

			void SetMax(FbxLimitsUtilities::LimitType type, FbxVector4^ max);
			FbxVector4^ GetMax(FbxLimitsUtilities::LimitType type);
			
			VALUE_PROPERTY_GETSET_DECLARE(FbxLimitsUtilities::RotationType,Rotation_Type);
			
			VALUE_PROPERTY_GETSET_DECLARE(FbxLimitsUtilities::RotationClampType,RotationClamp_Type);
			
			VALUE_PROPERTY_GETSET_DECLARE(FbxVector4^,RotationAxis);
			
			VALUE_PROPERTY_GETSET_DECLARE(double,AxisLength);

			void UpdateAutomatic(FbxNode^ node);
			FbxVector4^ GetEndPointTranslation(FbxNode^ node);
			FbxVector4^ GetEndSite(FbxNode^ node);			
		};


	}
}