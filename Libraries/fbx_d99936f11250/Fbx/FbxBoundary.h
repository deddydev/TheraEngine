#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxGeometry.h"




{
	namespace FbxSDK
	{	
		ref class FbxGeometry;
		ref class FbxShape;
		ref class FbxNurbsCurve;
		ref class FbxVector4;
		ref class FbxNurbsSurface;
		ref class FbxCurve;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** KFbxBoundary Describes a trimming boundary for a trimmed nurbs object.
		* Note that outer boundaries run counter-clockwise in UV space and inner
		* boundaries run clockwise. An outer boundary represents the outer edges
		* of the trimmed surface whereas the inner boundaries define "holes" in
		* the surface.
		*/
		public ref class FbxBoundary : FbxGeometry
		{
			REF_DECLARE(FbxEmitter,KFbxBoundary);
		internal:
			FbxBoundary(KFbxBoundary* instance);
		protected:
			System::Collections::Generic::List<FbxNurbsCurve^>^ _list;
			virtual void CollectManagedMemory()override;

			FBXOBJECT_DECLARE(FbxBoundary);
		public:

			/*static property System::String^ OuterFlag
			{
				System::String^ get();
			}*/
			/** Add an edge to this boundary
			\param pCurve The curve to append to the end of this boundary
			*/
			void AddCurve(FbxNurbsCurve^ curve);

			/** \return The number of edges in this boundary
			*/
			property int CurveCount
			{
				int get();
			}

			/** Access the edge at index pIndex
			* \param pIndex The index of the edge to return.  No bounds checking is done
			* \return The edge at index pIndex if
			*  pIndex is in the range [0, GetEdgeCount() ),
			*  otherwise the return value is undefined
			*/
			FbxNurbsCurve^ GetCurve( int index );
			//			/** Access the edge at index pIndex
			//			* \param pIndex The index of the edge to return.  No bounds checking is done
			//			* \return The edge at index pIndex if
			//			*  pIndex is in the range [0, GetEdgeCount() ),
			//			*  otherwise the return value is undefined
			//			*/
			//			KFbxNurbsCurve const* GetCurve( int pIndex ) const;
			//
			//			virtual EAttributeType GetAttributeType() const { return KFbxNodeAttribute::eBOUNDARY; }
			//
			bool IsPointInControlHull( FbxVector4^ point );

			FbxVector4^ ComputePointInBoundary();

		public:
			CLONE_DECLARE();

			void ClearCurves();

			void CopyCurves(FbxBoundary^ other );

			virtual property bool IsValid
			{
				bool get() override;
			}

			property bool IsCounterClockwise
			{
				bool get();
			}					
		};

		/** KFbxTrimNurbsSurface Describes a nurbs surface with regions
		trimmed or cut away with trimming boundaries.
		*/
		public ref class FbxTrimNurbsSurface : FbxGeometry
		{
			REF_DECLARE(FbxEmitter,KFbxTrimNurbsSurface);
		internal:			
			FbxTrimNurbsSurface(KFbxTrimNurbsSurface* instance) : FbxGeometry(instance)
			{
				_Free = false;
			}
		protected:			
			virtual void CollectManagedMemory() override;
			FBXOBJECT_DECLARE(FbxTrimNurbsSurface);
		public:
			//! Return the type of node attribute
			/*virtual property FbxNodeAttribute::AttributeType AttribType
			{
			FbxNodeAttribute::AttributeType get()
			{
			return (FbxNodeAttribute::AttributeType)
			((KFbxTrimNurbsSurface*)emitter)->GetAttributeType();
			}
			}*/


			/** Returns the number of regions on this trimmed nurbs surface.
			* Note there is at always at least one trim region.
			* \return The number of regions
			*/
			VALUE_PROPERTY_GET_DECLARE(int,TrimRegionCount);

			/** Call this before adding boundaries for a new trim region.
			* The number of regions is incremented on this call.
			*/
			void BeginTrimRegion();

			/** Call this after the last boundary for a given region is added.
			* If no boundaries are added inbetween calls to BeginTrimRegion
			* and EndTrimRegion, the last region is removed.
			*/
			void EndTrimRegion();

			/** Appends a trimming boundary to the set of trimming boundaries.
			* The first boundary specified for a given trim region should be
			* the outer boundary. All other boundaries are inner boundaries.
			* This must be called after a call to BeginTrimRegion(). Boundaries
			* cannot be shared among regions. Duplicate the boundary if nessecary.
			* See KFbxBoundary
			* \param pBoundary The boundary to add.
			* \return true if the boundary was added,
			*         false otherwise
			*/
			bool AddBoundary( FbxBoundary^ boundary );

			/** Gets the boundary at a given index for a given region
			* \param pIndex The index of the boundary to retrieve.  No bounds checking is done.
			* \param pRegionIndex The index of the region which is bound by the boundary.
			* \return The trimming boundary at index pIndex,
			* if pIndex is in the range [0, GetBoundaryCount() )
			* otherwise the result is undefined.
			*/
			FbxBoundary^  GetBoundary( int index, int regionIndex );

			//FbxBoundaryInfo^ GetBoundaryInfo( int index, int regionIndex);

			/** Gets the number of boundaries on this surface
			* \return The number of trim boundaries
			*/
			int GetBoundaryCount(int regionIndex);
			int GetBoundaryCount();

			/** Gets the untrimmed surface that is trimmed by the trim boundaries.
			* \return Pointer to the (untrimmed) nurbs surface.
			*/
			/** Set the nurbs surface that will be trimmed by the trimming boundaries.
			* \param pNurbs Nurbs
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxNurbsSurface,NurbsSurface);

			/** Gets the untrimmed surface that is trimmed by the trim boundaries.
			//			* \return Pointer to the (untrimmed) nurbs surface.
			//			*/
			//			KFbxNurbsSurface const* GetNurbsSurface() const;
			//

			/** Check if the normals are flipped
			* \return True if normals are flipped, false otherwise
			*/
			/** The normals of the surface can be reversed to reverse the surface
			* \param pFlip If true, the surface is reversed, else the surface is not reversed.
			*/
			property bool FlipNormals
			{
				bool get();
				void set(bool value);
			}														



			/**
			* \name Shape Management
			*/
			//@{

			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->AddShape()
			* See KFbxGeometry::AddShape() for method description.
			*/
			//virtual int AddShape(FbxShape^ shape, String^ shapeName);

			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->ClearShape()
			* See KFbxGeometry::ClearShape() for method description.
			*/
			/*virtual void ClearShape()
			{
			((KFbxTrimNurbsSurface*)emitter)->ClearShape();
			}*/

			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->GetShapeCount()
			* See KFbxGeometry::GetShapeCount() for method description.
			*/
			/*virtual property int ShapeCount
			{
			int get(){return ((KFbxTrimNurbsSurface*)emitter)->GetShapeCount(); }
			}*/

			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->GetShape()
			* See KFbxGeometry::GetShape() for method description.
			*/
			/*virtual FbxShape^ GetShape(int index)
			{
			return gcnew FbxShape(((KFbxTrimNurbsSurface*)emitter)->GetShape(index));
			}*/

			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->GetShape()
			* See KFbxGeometry::GetShape() for method description.
			*/
			//virtual FbxShape const* GetShape(int pIndex) const;


			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->GetShapeName()
			* See KFbxGeometry::GetShapeName() for method description.
			*/
			/*virtual String^ GetShapeName(int index)
			{
			return gcnew String(((KFbxTrimNurbsSurface*)emitter)->GetShapeName(index));
			}*/


			/** Shapes on trim nurbs are stored on the untrimmed surface.
			* Thus, this is equivalent to calling GetNurbsSurface()->GetShapeChannel()
			* See KFbxGeometry::GetShapeChannel() for method description.
			*/
			/*virtual FbxCurve^ GetShapeChannel(int index, bool createAsNeeded, String^ takeName)
			{
			if(takeName )
			{
			char* c = new char(FbxString::NumCharToCreateString);
			FbxString::StringToChar(takeName,c);
			return gcnew FbxCurve(((KFbxTrimNurbsSurface*)emitter)->GetShapeChannel(index,
			createAsNeeded,c));
			}
			else
			{
			return gcnew FbxCurve(((KFbxTrimNurbsSurface*)emitter)->GetShapeChannel(index,
			createAsNeeded));
			}
			}*/
			virtual FbxCurve^ GetShapeChannel(int index);
			//@}


			/*virtual property int ControlPointsCount
			{
			int get(){return ((KFbxTrimNurbsSurface*)emitter)->GetControlPointsCount();}
			}*/

			virtual void SetControlPointAt(FbxVector4^ ctrlPoint, FbxVector4^ normal , int index);

			/*virtual FbxVector4^ GetControlPoints()
			{
			return gcnew FbxVector4(((KFbxTrimNurbsSurface*)emitter)->GetControlPoints());
			}*/

			//#ifndef DOXYGEN_SHOULD_SKIP_THIS
			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

		public:
			// Clone
			CLONE_DECLARE();

			virtual property bool IsValid
			{
				bool get() override;
			}


		};

	}
}