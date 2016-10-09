#pragma once
#include "stdafx.h"
#include "FbxBindingTableBase.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxStringTypedProperty;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		/** \brief A binding table represents a collection of bindings
		* from source types such as KFbxObjects, or KFbxLayerElements
		* to destinations which can be of similar types. See KFbxBindingTableEntry.
		* \nosubgrouping
		*/
		public ref class FbxBindingTable : FbxBindingTableBase
		{
			REF_DECLARE(FbxEmitter,KFbxBindingTable);
		internal:
			FbxBindingTable(KFbxBindingTable* instance): FbxBindingTableBase(instance)
			{
				_Free = false;
			}

			FBXOBJECT_DECLARE(FbxBindingTable);
		protected:
			virtual void CollectManagedMemory()override;
		public:
			void CopyFrom(FbxBindingTable^ table);

			// Target name and type
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,TargetName);
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,TargetType);
			
			//			// Relative URL of file containing the shader implementation description
			//			// eg.: ./shader.mi
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,DescRelativeURL);

			//			// Absolute URL of file containing the shader implementation description
			//			// eg.: file:///usr/tmp/shader.mi
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,DescAbsoluteURL);

			//
			//			// Identifyies the shader to use in previous decription's URL
			//			// eg.: MyOwnShader
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,DescTAG);
			
			//			// Relative URL of file containing the shader implementation code
			//			// eg.: ./bin/shader.dll
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,CodeRelativeURL);
			
			//			// Absolute URL of file containing the shader implementation code
			//			// eg.: file:///usr/tmp/bin/shader.dll
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,CodeAbsoluteURL);

			//			// Identifyies the shader function entry to use in previous code's URL
			//			// eg.: MyOwnShaderFunc
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,CodeTAG);			

		/*protected:
			static System::String^ sTargetName;
			static System::String^ sTargetType;
			static System::String^ sDescRelativeURL;
			static System::String^ sDescAbsoluteURL;
			static System::String^ sDescTAG;
			static System::String^ sCodeRelativeURL;
			static System::String^ sCodeAbsoluteURL;
			static System::String^ sCodeTAG;

			static System::String^ sDefaultTargetName;
			static System::String^ sDefaultTargetType;
			static System::String^ sDefaultDescRelativeURL;
			static System::String^ sDefaultDescAbsoluteURL;
			static System::String^ sDefaultDescTAG;
			static System::String^ sDefaultCodeRelativeURL;
			static System::String^ sDefaultCodeAbsoluteURL;
			static System::String^ sDefaultCodeTAG;*/
		public:
			//
			//			//////////////////////////////////////////////////////////////////////////
			//			// Static values
			//			//////////////////////////////////////////////////////////////////////////
			//

			//			// property names
			//static property System::String^ STargetName
			//{
			//	System::String^ get();
			//}

			//static property System::String^ STargetType
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ SDescRelativeURL
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ SDescAbsoluteURL
			//{
			//	System::String^ get();
			//}

			//static property System::String^ SDescTAG
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ SCodeRelativeURL
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ SCodeAbsoluteURL
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ SCodeTAG
			//{
			//	System::String^ get();
			//}
			////
			////			// property default values			
			//static property System::String^ DefaultTargetName
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultTargetType
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultDescRelativeURL
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultDescAbsoluteURL
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultDescTAG
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultCodeRelativeURL
			//{
			//	System::String^ get();
			//}
			//
			//static property System::String^ DefaultCodeAbsoluteURL
			//{
			//	System::String^ get();
			//}			
			//static property System::String^ DefaultCodeTAG
			//{
			//	System::String^ get();
			//}

		};

	}
}