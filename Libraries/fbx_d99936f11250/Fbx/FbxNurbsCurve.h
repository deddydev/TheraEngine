#pragma once
#include "stdafx.h"
#include "FbxGeometry.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/**
		A Non-Uniform Rational B-Spline (Nurbs) curve is a type of parametric geometry. A Nurbs
		curve is defined by the degree, form, knot vector and control points. 

		Let M be the degree of the curve.
		Let N be the number of control points of the curve.

		The form of the curve can be open, closed or periodic. A curve with end points
		that do not meet is defined as an open curve. The number of knots in an open curve
		is defined as N+(M+1). 

		A closed curve simply has its last control point equal to its first control point. 
		Note that this does not imply tangent continuity at the end point.  The curve may 
		have a kink at this point.  In FBX the last control point is not specified by the user
		in the InitControlPoints() method. For example, if there are to be 10 control points in
		total, and the curve is to be closed, than only 9 control points need to be passed 
		into the InitControlPoints() method. The last control point is implied to be equal
		to the first control point. Thus N represents the number of unique CVs. 

		A periodic curve has its last M control points equal to its first M control points. 
		A periodic curve is tangent continuous at the ends. Similiar to a closed curve,
		when creating a periodic curve, only the unique control points need to be set. For
		example a periodic curve of degree 3 with 10 control points requires only 7 CVs to 
		be specified in the InitControlPoints() method. The last 3 CVs, which are the same as
		the first 3, are not included. 

		The calculation of the number of knots in closed and periodic curves is more complex. 
		Since we have excluded one CV in N in a closed curve, the number of knots is N+(M+1)+1. 
		Similiarly, we excluded M CVs in periodic curves so the number of knots is N+(M+1)+M. 

		Note that FBX stores one extra knot at the beginning and and end of the knot vector,
		compared to some other graphics applications such as Maya. The two knots are not 
		used in calculation, but they are included so that no data is lost when converting
		from file formats that do store the extra knots.

		* \nosubgrouping
		*/
		public ref class FbxNurbsCurve : FbxGeometry 
		{
			REF_DECLARE(FbxEmitter,KFbxNurbsCurve);
		internal:
			FbxNurbsCurve(KFbxNurbsCurve* instance) :  FbxGeometry(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxNurbsCurve);
		public:
			// inhierited from KFbxNodeAttribute
			//virtual KFbxNodeAttribute::EAttributeType GetAttributeType() const;

			/** \enum EDimension  The dimension of the CVs
			* - \e e2D The CVs are two dimensional points
			* - \e e3D The CVs are three dimensional points
			*/
			enum class Dimension
			{
				E2D = 2,
				E3D,
				DimensionsCount = 2
			};

			/** \enum EType The form of the curve
			* - \e eOPEN
			* - \e eCLOSED
			* - \e ePERIODIC
			*/
			enum class Type
			{
				Open = KFbxNurbsCurve::eOPEN,
				Closed = KFbxNurbsCurve::eCLOSED,
				Periodic = KFbxNurbsCurve::ePERIODIC,
				TypeCount = KFbxNurbsCurve::eTYPE_COUNT
			}; 

			/** Allocate memory space for the array of control points as well as the knot 
			* vector.
			* \param pCount Number of control points.
			* \param pVType Nurb type in V direction.
			* \remarks This function should always be called after KFbxNurb::SetOrder(). 
			*/
			void InitControlPoints(int count, FbxNurbsCurve::Type vType);

			/** Get knot vector.
			* \return Pointer to the array of knots.
			*/
			VALUE_PROPERTY_GET_DECLARE(IntPtr,KnotVector);

			/** Get the number of elements in the knot vector.
			* \return The number of knots. See KFbxNurbsCurve description for more details. 
			*/
			VALUE_PROPERTY_GET_DECLARE(int,KnotCount);			

			/** Get nurb curve order.
			* \return Order value.
			*/
			// Sets the order of the curve
			// Must be set before InitControlPoints() is called. 
			VALUE_PROPERTY_GET_DECLARE(int,Order);				

			/** Gets the dimension of the control points.
			* \return The dimension of the curve
			*/
			/** Sets the dimension of the CVs
			* For 3D curves: control point = ( x, y, z, w ), where w is the weight
			* For 2D curves: control point = ( x, y, 0, w ), where the z component is unused, and w is the weight. 
			* \param pDimension - the dimension of the control points. (3d or 2d)
			*/
			VALUE_PROPERTY_GETSET_DECLARE(Dimension,DimensionType);

			/** Determines if the curve is rational or not
			* \return True if the curve is rational, false otherwise
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsRational); 

			/** Calculates the number of spans in the curve using the following:
			* Where
			* S = Number of spans
			* N = Number of CVs
			* M = Order of the curve
			*
			* S = N + M + 1;
			*
			* In this calculation N includes the duplicate CVs for closed and periodic curves. 
			* 
			* \return The number of spans if the curve has been initialized, -1 otherwise.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,SpanCount);

			/** Get nurb type.
			* \return Nurb curve type identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE( FbxNurbsCurve::Type,NurbsCurveType);

			/** Checks if the curve is a poly line. (A polyline is a 
			* linear nurbs curve )
			*
			* \return \c true if curve is a poly line, false otherwise.
			*/
			//VALUE_PROPERTY_GET_DECLARE(bool,IsPolyline);

			/** Bezier curves are a special case of nurbs curve. This function
			* determines if this nurbs curve is a Bezier curve.
			*
			* \return \c true if curve is a Bezier curve, false otherwise.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsBezier);

			// step? 
			// Need to know multiplicity?

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

		public:
			// Clone
			CLONE_DECLARE();

			bool FullMultiplicity();		
		};

	}
}