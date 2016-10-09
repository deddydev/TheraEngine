#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxDouble3;
		ref class FbxDouble3TypedProperty;
		ref class FbxNode;
		/**	\brief This class is the base class to all types of node attributes.
		* \nosubgrouping
		*	A node attribute is the content of a node. A \c NULL node attribute is set 
		* by calling function KFbxNode::SetNodeAttribute() with a \c NULL pointer.
		*/
		public ref class FbxNodeAttribute : FbxTakeNodeContainer
		{
		internal:
			FbxNodeAttribute(KFbxNodeAttribute* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}
			REF_DECLARE(FbxEmitter,KFbxNodeAttribute);
			FBXOBJECT_DECLARE(FbxNodeAttribute);			

		protected:
			virtual void CollectManagedMemory() override;
			//static FbxDouble3^ _SDefaultColor;			
		public:		
			/**
			* \name Property Names
			*/
			//static VALUE_PROPERTY_GET_DECLARE(String^,ColorName);

			/**
			* \name Property Default Values
			*/			
			//static VALUE_PROPERTY_GET_DECLARE(FbxDouble3^,SDefaultColor);

			//////////////////////////////////////////////////////////////////////////
			//
			// Properties
			//
			//////////////////////////////////////////////////////////////////////////

			/** This property handles the color.
			*
			* Default value is (0.8, 0.8, 0.8)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Color);



		public:
			// Node attribute types.
			enum class AttributeType
			{   
				Unidentified = KFbxNodeAttribute::eUNIDENTIFIED,
				Null= KFbxNodeAttribute::eNULL,
				Marker= KFbxNodeAttribute::eMARKER,
				Skeleton= KFbxNodeAttribute::eSKELETON, 
				Mesh= KFbxNodeAttribute::eMESH, 
				Nurb= KFbxNodeAttribute::eNURB, 
				Patch= KFbxNodeAttribute::ePATCH, 
				Camera= KFbxNodeAttribute::eCAMERA, 
				CameraSwitcher= KFbxNodeAttribute::eCAMERA_SWITCHER,
				Light= KFbxNodeAttribute::eLIGHT,
				OpticalReference= KFbxNodeAttribute::eOPTICAL_REFERENCE,
				OpticalMarker= KFbxNodeAttribute::eOPTICAL_MARKER,
				Constraint= KFbxNodeAttribute::eCONSTRAINT,
				NurbsCurve= KFbxNodeAttribute::eNURBS_CURVE,
				TrimNurbsSurface= KFbxNodeAttribute::eTRIM_NURBS_SURFACE,
				Boundary= KFbxNodeAttribute::eBOUNDARY,
				NurbsSurface= KFbxNodeAttribute::eNURBS_SURFACE,
				Shape= KFbxNodeAttribute::eSHAPE,
			};

			/** Return the type of node attribute.
			* This class is pure virtual.
			*/
			virtual property AttributeType AttribType
			{
				AttributeType get();
			}


			/** Return the node this attribute is set to.
			* \return     Pointer to the node, or \c NULL if the current attribute is not set to a node.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,Node);

			/**
			* \name Properties
			*/
			//@{			

			CLONE_DECLARE();

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

		};

	}
}