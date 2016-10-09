#pragma once
#include "stdafx.h"
#include "Fbx.h"

#define DECLARE_SRCDST_TEMPLATES(Class)\
ref class _##Class abstract sealed { public:\
	static bool FbxDisconnectAllSrc(FbxObject^ obj);\
	static int FbxGetSrcCount(FbxObject^ obj);\
	static int FbxGetSrcCount(FbxObject^ obj,FbxClassId^ classId);\
	static Class^ FbxGetSrc(FbxObject^ obj,int index);\
	static Class^ FbxGetSrc(FbxObject^ obj,int index,FbxClassId^ classId);\
	static Class^ FbxFindSrc(FbxObject^ obj,String^ name ,int index);\
	static Class^ FbxFindSrc(FbxObject^ obj,String^ name,FbxClassId^ classId ,int index);\
	static int FbxGetDstCount(FbxObject^ obj);\
	static int FbxGetDstCount(FbxObject^ obj,FbxClassId^ classId);\
	static Class^ FbxGetDst(FbxObject^ obj,int index);\
	static Class^ FbxGetDst(FbxObject^ obj,int index,FbxClassId^ classId);}

namespace Skill
{

	namespace FbxSDK
	{
		ref class FbxObjectManaged;
		ref class FbxObjectMetaData;
		ref class FbxClassId;
		ref class FbxMesh;

		public ref class FbxStatics abstract sealed
		{
		public:
			DECLARE_SRCDST_TEMPLATES(FbxObjectMetaData);
			DECLARE_SRCDST_TEMPLATES(FbxMesh);


			// need to be declared here instead or we get an INTERNAL COMPILER ERROR with VC6
			static bool FbxConnectSrc(FbxObjectManaged^ dstObject,FbxObjectManaged^ srcObject);
			static bool FbxConnectDst(FbxObjectManaged^ srcObject,FbxObjectManaged^ dstObject);
		};
	}
}