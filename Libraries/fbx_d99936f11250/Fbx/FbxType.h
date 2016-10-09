#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{	
		// Type definitions
		public enum class FbxType {
			Unidentified,
			Bool1,
			Integer1,
			Float1,
			Double1,
			Double2,
			Double3,
			Double4,
			Double44,
			Enum,
			String,
			Time,
			Reference,  // used as a port entry to reference object or properties
			Blob,
			Distance,
			DateTime,
			MaxTypes
		};
	}
}