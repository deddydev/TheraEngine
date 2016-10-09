#pragma once
#include "stdafx.h"
#include "FbxLayerContainer.h"
using namespace System::Runtime::InteropServices;
namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxVector4;
		ref class FbxStream;
		ref class FbcDouble3TypedProperty;
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxLayerElementArrayTemplateVector4;
		ref class FbxLayerElementArrayTemplateInt32;
		/** \brief This class is the base class for managing control points.
		* Use the KFbxGeometryBase class to manage control points for mesh, nurbs, patches and normals (on Layer 0).
		* \nosubgrouping
		*/
		public ref class FbxGeometryBase : FbxLayerContainer
		{
			REF_DECLARE(FbxEmitter,KFbxGeometryBase);
		internal:
			FbxGeometryBase(KFbxGeometryBase* instance):FbxLayerContainer(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory() override;
		public:

			FBXOBJECT_DECLARE(FbxGeometryBase);			
			
			/**
			* \name Control Points and Normals Management.
			*/
			//@{

			/** Allocate memory space for the array of control points.
			* \param pCount     The number of control points.
			* \remarks          Any previously allocated array of control points will be cleared.
			*/
			virtual void InitControlPoints(int count);

			/** Allocate memory space for the array of normals.
			* \param pCount     The desired size for the normal array. If pCount is specified, the array will have the same size as pCount.
			*                   If pCount is not specified, the array will be the same length as the array of control points.
			* \remarks          This function must be called after function KFbxLayerContainer::InitControlPoints().
			* \remarks          The normals initialized with this function will have the ReferenceMode set to eDIRECT.
			*/
			void InitNormals(int count );


			/** Allocate memory space for the array of normals cloning them from the pSrc.
			* \param pSrc       The source geometry from wich we will clone the normals information (on Layer 0).
			* \remarks          This function must be called with the argument otherwise it will do nothing.
			*/
			void InitNormals(FbxGeometryBase^ src);

			/** Sets the control point and the normal values for a given index.
			* \param pCtrlPoint     The value of the control point.
			* \param pNormal        The value of the normal.
			* \param pIndex         The index of the control point/normal to be modified.
			* \param pI2DSearch     When true AND the normals array reference mode is eINDEX_TO_DIRECT, search pNormal in the
			*                       existing array to avoid inserting it if it already exist. NOTE: this feature uses a linear
			*                       search algorithm, therefore it can be time consuming if the DIRECT array of normals contains
			*                       a huge number of elements.
			* \remarks              If the arrays are not big enough to store the values at the given index, their size will be increased.
			*/
			virtual void SetControlPointAt(FbxVector4^ ctrlPoint , FbxVector4^ normal , int index, bool pI2DSearch);


			/** Sets the control point for a given index.
			* \param pCtrlPoint     The value of the control point.
			* \param pIndex         The index of the control point/normal to be modified.
			*
			* \remarks              If the arrays are not big enough to store the values at the given index, their size will be increased.
			*/
			virtual void SetControlPointAt(FbxVector4^ ctrlPoint , int index);

			/** Sets the the normal values for a given index.
			* \param pNormal        The value of the normal.
			* \param pIndex         The index of the control point/normal to be modified.
			* \param pI2DSearch     When true AND the normals array reference mode is eINDEX_TO_DIRECT, search pNormal in the
			*                       existing array to avoid inserting it if it already exist. NOTE: this feature uses a linear
			*                       search algorithm, therefore it can be time consuming if the DIRECT array of normals contains
			*                       a huge number of elements.
			* \remarks              If the arrays are not big enough to store the values at the given index, their size will be increased.
			*/
			virtual void SetControlPointNormalAt(FbxVector4^ ctrlPoint, int index, bool pI2DSearch);

			/** Get the number of control points.
			* \return     The number of control points allocated in the geometry.
			*/
			virtual  property int ControlPointsCount
			{
				int get();
			}


			/** Get a pointer to the array of control points.
			* \return      Pointer to the array of control points, or \c NULL if the array has not been allocated.
			* \remarks     Use the function KFbxGeometryBase::InitControlPoints() to allocate the array.
			*/
			virtual VALUE_PROPERTY_GETSET_DECLARE(array<FbxVector4^>^,ControlPoints);			

			virtual FbxVector4^ GetControlPointAt(int index);
			virtual void SetControlPointAt(int index ,FbxVector4^ point);

			/** Get a pointer to the array of normals.
			* \return      Pointer to array of normals, or \c NULL if the array hasn't been allocated yet.
			* \remarks     Use the function KFbxGeometryBase::InitNormals() to allocate the array.
			* \remarks     This method should not be called anymore since it will not put a lock to internal
			*              array. Use the other flavor instead.
			*/
			//FbxVector4^ GetNormals();



			/**
			* \name Public and fast access Properties
			*/
			//@{
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,BBoxMin);			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,BBoxMax);

			/** Compute the Bounding box of the ControlPoints.
			*/
			void ComputeBBox();


			/**
			* \name Off-loading Serialization section
			*/
			//@{
			//virtual bool ContentWriteTo(FbxStream^ stream);
			//virtual bool ContentReadFrom(FbxStream^ stream);
			//@}

			virtual property int MemoryUsage
			{
				int get();
			}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateVector4,Normals);
			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateInt32,NormalsIndices);			

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}