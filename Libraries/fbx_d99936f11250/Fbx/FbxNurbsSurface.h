#pragma once
#include "stdafx.h"
#include "FbxGeometry.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxNode;
		ref class FbxSdkManagerManaged;
		/** A Nurbs surface is a type of parametric geometry. A Nurbs surface is defined by the
		degree, form, knot vector and control points in the U and V directions.

		For more information on the meaning of the form, knot vector and control points,
		see the documentation for the KFbxNurbsCurve. The same concepts for Nurbs curves
		apply to Nurbs surfaces. Nurbs surfaces simply have two dimensions (U and V).

		* \nosubgrouping
		*/
		public ref class FbxNurbsSurface : FbxGeometry
		{
			REF_DECLARE(FbxEmitter,KFbxNurbsSurface);
		internal:
			FbxNurbsSurface(KFbxNurbsSurface* instance) : FbxGeometry(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxNurbsSurface);
		public:
			//! Return the type of node attribute which is EAttributeType::eNURBS_SURFACE.
			//virtual EAttributeType GetAttributeType() const { return KFbxNodeAttribute::eNURBS_SURFACE; }

			//! Reset the nurb to default values.
			void Reset();

			/**
			* \name Nurb Properties
			*/
			//@{

			/** Get surface mode.
			* \return Currently set surface mode identifier.
			*/
			/** Set surface mode.
			* \param pMode Surface mode identifier (see class KfbxGeometry)
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxGeometry::SurfaceMode,Surface_Mode);
				

			/** \enum ENurbType Nurb types.
			* - \e ePERIODIC
			* - \e eCLOSED
			* - \e eOPEN
			*/
			enum class NurbType
			{
				Periodic = KFbxNurbsSurface::ePERIODIC,
				Closed = KFbxNurbsSurface::eCLOSED,
				Open = KFbxNurbsSurface::eOPEN
			};

			/** Allocate memory space for the array of control points as well as the knot
			* and multiplicity vectors.
			* \param pUCount Number of control points in U direction.
			* \param pUType Nurb type in U direction.
			* \param pVCount Number of control points in V direction.
			* \param pVType Nurb type in V direction.
			* \remarks This function should always be called after KFbxNurb::SetOrder().
			*/
			void InitControlPoints(int uCount, NurbType uType, int vCount, NurbType vType);

			/** Get number of control points in U direction.
			* \return Number of control points in U.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UCount);

			/** Get number of control points in V direction.
			* \return Number of control points in V.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VCount);

			/** Get nurb type in U direction.
			* \return Nurb type identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE(NurbType,NurbUType);

			/** Get nurb type in V direction.
			* \return Nurb type identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE(NurbType,NurbVType);

			/** Get the number of elements in the knot vector in U direction. See KFbxNurbsCurve for more information.
			* \return The number of control points in U direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UKnotCount);

			/** Get knot vector in U direction.
			* \return Pointer to the array of knots.
			*/
			VALUE_PROPERTY_GET_DECLARE(IntPtr,UKnotVector);

			/** Get the number of elements in the knot vector in V direction. See KFbxNurbsCurve for more information.
			* \returns The number of control points in V direction. Nurb order in V
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VKnotCount);

			/** Get knot vector in V direction.
			* \return Pointer to the array of knots.
			*/
			VALUE_PROPERTY_GET_DECLARE(IntPtr,VKnotVector);			

			/** Set order.
			* \param pUOrder Nurb order in U direction.
			* \param pVOrder Nurb order in V direction.
			*/
			void SetOrder(kUInt uOrder, kUInt vOrder);

			/** Get nurb order in U direction.
			* \return U order value.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UOrder);

			/** Get nurb order in V direction.
			* \return V order value.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VOrder);			

			/** Set step.
			* The step is the number of divisions between adjacent control points.
			* \param pUStep Steps in U direction.
			* \param pVStep Steps in V direction.
			*/
			void SetStep(int uStep, int vStep);

			/** Get the number of divisions between adjacent control points in U direction.
			* \return Step value in U direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UStep);

			/** Get the number of divisions between adjacent control points in V direction.
			* \return Step value in V direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VStep);

			/* Calculates the number of spans in the surface in the U direction.
			* See KFbxNurbsCurve::GetSpanCount() for more information.
			* \returns The number of spans in U if the surface has been initialized, -1 otherwise.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,USpanCount);

			/* Calculates the number of spans in the surface in the V direction.
			* See KFbxNurbsCurve::GetSpanCount() for more information.
			* \returns The number of spans in V if the surface has been initialized, -1 otherwise.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VSpanCount);

			//@}

			/**
			* \name Nurb Export Flags
			*/
			//@{			

			/** Get the flag inducing UV flipping at export time.
			* \return Current state of the UV flip flag.
			*/
			/** Set the flag inducing UV flipping at export time.
			* \param pFlag If \c true UV flipping will occur.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ApplyFlipUV);
			
			/** Get the flag inducing link flipping at export time.
			* \return Current state of the link flip flag.
			*/
			/** Set the flag inducing link flipping at export time.
			* \param pFlag If \c true the links control points indices will be flipped.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ApplyFlipLinks);			

			/** Get flip flags state.
			* \return \c true if we need to flip either the UV or the links.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,ApplyFlip);

			/** Add curve on surface
			* Adds a 2d, parameter space curve to this surface
			* \param pCurve The curve to add to the surface
			*/
			void AddCurveOnSurface(FbxNode^ curve );

			/* Retrieves a curve on this surface
			* \param pIndex Index of the curve to retrieve. Valid range is 0 to GetCurveOnSurfaceCount() - 1
			* \return The curve at the specified index, or NULL if pIndex is out of range.
			*/
			FbxNode^ GetCurveOnSurface( int index );
			//FbxNode^ const* GetCurveOnSurface( int pIndex ) const;

			/* \return The number of curves on this surface
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CurveOnSurfaceCount);

			/* Removes a curve from this surface.
			* \param pCurve The curve to remove
			* \return True if the curve was removed, false otherwise.
			*/
			bool RemoveCurveOnSurface(FbxNode^ curve);

			//@}

			/** Check if the surface has all rational control points.
			* \return true if rational, false otherwise
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsRational);


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////


#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:

			VALUE_PROPERTY_GETSET_DECLARE(bool,FlipNormals);

			// Clone
			CLONE_DECLARE();		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}