#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		public enum class FbxRotationOrder
		{ 
			EulerXYZ = 0, 
			EulerXZY, 
			EulerYZX, 
			EulerYXZ, 
			EulerZXY, 
			EulerZYX,
			SphericXYZ
		};
		public enum class FbxTransformInheritType
		{
			RrSs = 0, 
			RSrs, 
			Rrs 
		};
	}
}