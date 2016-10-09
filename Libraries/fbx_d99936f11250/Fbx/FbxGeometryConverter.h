#pragma once
#include "stdafx.h"
#include "Fbx.h"



namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxMesh;
		ref class FbxPatch;
		ref class FbxNurb;
		ref class FbxNode;
		ref class FbxWeightedMapping;
		ref class FbxNurbsSurface;
		ref class FbxGeometry;
		/** 
		* \brief This class provides functions to triangulate and convert geometry node attributes.
		* \nosubgrouping
		*/
		public ref class FbxGeometryConverter : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxGeometryConverter,KFbxGeometryConverter);		
			INATIVEPOINTER_DECLARE(FbxGeometryConverter,KFbxGeometryConverter);
		public:
			FbxGeometryConverter(FbxSdkManagerManaged^ manager);			

			/** 
			* \name Triangulation
			*/
			//@{

			/** Triangulate a mesh.
			* \param pMesh     Pointer to the mesh to triangulate.
			* \return          Pointer to the new triangulated mesh.
			* \remarks         This method creates a new mesh, leaving the source mesh unchanged.
			*/
			FbxMesh^ TriangulateMesh(FbxMesh^ mesh);

			/** Triangulate a patch.
			* \param pPatch     Pointer to the patch to triangulate.
			* \return           Pointer to the new triangulated mesh.
			* \remarks          The links and shapes are also converted to fit the created mesh.
			*/
			FbxMesh^ TriangulatePatch(FbxPatch^ patch);

			/** Triangulate a nurb.
			* \param pNurb     Pointer to the nurb to triangulate.
			* \return          Pointer to the new triangulated mesh.
			* \remarks         The links and shapes are also converted to fit the created mesh.
			*/
			FbxMesh^ TriangulateNurb(FbxNurb^ nurb);

			/** Triangulate a mesh, patch or nurb contained in a node in order to preserve 
			* related animation channels.
			* \param pNode     Pointer to the node containng the geometry to triangulate.
			* \return          \c true on success, or \c false if the node attribute is not a mesh, a patch or a nurb.
			* \remarks         See the remarks for functions TriangulateMesh(), TriangulatePatch() and TriangulateNurb().
			*/
			bool TriangulateInPlace(FbxNode^ node);

			/** Add an "alternate" geometry to the node.
			* \param pNode                        Pointer to the node containing the geometry.
			* \param pSrcGeom                     Pointer to the source geometry.
			* \param pAltGeom                     Pointer to the alternate geometry.
			* \param pSrcToAltWeightedMapping     Pointer to the weighted mapping table (optional).
			* \param pConvertDeformations         Flag used only if parameter pSrcToAltWeightedMapping is a valid pointer to a weighted mapping table.
			*                                     Set to \c true to convert deformations using the weighted mapping table.
			* \return                             \c true on success, or \c false if the node attribute is not a mesh, a patch or a nurb.
			*/
			bool AddAlternateGeometry(
				FbxNode^ node, 
				FbxGeometry^ srcGeom, 
				FbxGeometry^ altGeom,
				FbxWeightedMapping^ srcToAltWeightedMapping,
				bool convertDeformations
				);

			/** Convert shape(s) and link(s) from souce to destination geometry.
			* \param pNode        Pointer to the node containng the geometry.
			* \param pSrcGeom     Pointer to the source geometry.
			* \param pDstGeom     Pointer to the destination geometry.
			* \return             \c true on success, \c false otherwise.
			* \remarks            Source and destination geometry must belong to the same node and must be linked by a geometry weighted map.
			*/
			bool ConvertGeometryAnimation(
				FbxNode^ node, 
				FbxGeometry^ srcGeom, 
				FbxGeometry^ dstGeom
				);

			/** Compute a "vertex-correspondance" table that helps passing from source to destination geometry.
			* \param pSrcGeom                     Pointer to the source geometry.
			* \param pDstGeom                     Pointer to the destination geometry.
			* \param pSrcToDstWeightedMapping     Pointer to the weighted mapping table.
			* \param pSwapUV                      Set to \c true to swap UVs.
			* \return                             \c true on success, \c false if the function fails to compute the correspondance.
			* \remarks                            Links and shapes are also converted to fit the alternate geometry.
			*/
			bool ComputeGeometryControlPointsWeightedMapping(
				FbxGeometry^ srcGeom, 
				FbxGeometry^ dstGeom, 
				FbxWeightedMapping^ srcToDstWeightedMapping,
				bool swapUV
				);

			/** 
			* \name Geometry Conversion
			*/
			//@{

			/** Convert from patch to nurb.
			* \param pPatch     Pointer to the patch to convert.
			* \return           Created nurb or \c NULL if the conversion fails.
			* \remarks          The patch must be of type eBSPLINE, eBEZIER or eLINEAR.
			*/
			FbxNurb^ ConvertPatchToNurb(FbxPatch^ patch);

			/** Convert a patch contained in a node to a nurb. Use this function to preserve the patch's related animation channels.
			* \param pNode     Pointer to the node containing the patch.
			* \return          \c true on success, \c false if the node attribute is not a patch.
			* \remarks         The patch must be of type eBSPLINE, eBEZIER or eLINEAR.
			*/
			bool ConvertPatchToNurbInPlace(FbxNode^ node);

			/** Convert a patch to nurb surface.
			* \param pPatch     Pointer to the patch to convert.
			* \return           Created nurb surface or \c NULL if conversion fails.
			* \remarks          The patch must be of type eBSPLINE, eBEZIER or eLINEAR.
			*/
			FbxNurbsSurface^ ConvertPatchToNurbsSurface(FbxPatch^ patch);

			/** Convert a patch contained in a node to a nurb surface. Use this function to preserve the patch's related animation channels.
			* \param pNode     Pointer to the node containing the patch.
			* \return          \c true on success, \c false if the node attribute is not a patch.
			* \remarks         The patch must be of type eBSPLINE, eBEZIER or eLINEAR.
			*/
			bool ConvertPatchToNurbsSurfaceInPlace(FbxNode^ node);
			/** Convert a KFbxNurb to a KFbxNurbsSurface
			* \param pNurb     Pointer to the original nurb
			* \return          A KFbxNurbsSurface that is equivalent to the original nurb.
			*/
			FbxNurbsSurface^ ConvertNurbToNurbsSurface( FbxNurb^ nurb );

			/** Convert a KFbxNurbsSurface to a KFbxNurb
			* \param pNurb     Pointer to the original nurbs surface
			* \return          A KFbxNurb that is equivalent to the original nurbs surface.
			*/
			FbxNurb^ ConvertNurbsSurfaceToNurb( FbxNurbsSurface^ nurb );

			/** Convert a nurb, contained in a node, to a nurbs surface. Use this function to preserve the nurb's related animation channels.
			* \param pNode     Pointer to the node containing the nurb.
			* \return          \c true on success, \c false otherwise
			*/
			bool ConvertNurbToNurbsSurfaceInPlace(FbxNode^ node);
			/** Convert a nurb contained in a node to a nurbs surface. Use this function to preserve the nurb's related animation channels.
			* \param pNode     Pointer to the node containing the nurbs surface.
			* \return          \c true on success, \c false otherwise
			*/
			bool ConvertNurbsSurfaceToNurbInPlace(FbxNode^ node);

			//@}

			/** 
			* \name Nurb UV and Links Swapping
			*/
			//@{

			/** Flip UV and/or links of a nurb.
			* \param pNurb             Pointer to the Source nurb.
			* \param pSwapUV           Set to \c true to swap the UVs.
			* \param pSwapClusters     Set to \c true to swap the control point indices of clusters.
			* \return                  A fliped kFbxNurb, or \c NULL if the function fails.
			*/
			FbxNurb^ FlipNurb(FbxNurb^ nurb, bool swapUV, bool swapClusters);

			/** Flip UV and/or links of a nurb surface.
			* \param pNurb             Pointer to the Source nurb surface.
			* \param pSwapUV           Set to \c true to swap the UVs.
			* \param pSwapClusters     Set to \c true to swap the control point indices of clusters.
			* \return                  A fliped kFbxNurbSurface, or \c NULL if the function fails.
			*/
			FbxNurbsSurface^ FlipNurbsSurface(FbxNurbsSurface^ nurb, bool swapUV, bool swapClusters);

			//@}

			/** 
			* \name Normals By Polygon Vertex Emulation
			*/
			//@{

			/** Emulate normals by polygon vertex mode for a mesh.
			* \param pMesh     Pointer to the mesh object.
			* \return          \c true on success, \c false if the number of normals in the 
			*                  mesh and in its associated shapes don't match the number of polygon
			*                  vertices.
			* \remarks         Since the FBX file format currently only supports normals by
			*                  control points, this function duplicates control points to equal the 
			*                  number of polygon vertices. Links and shapes are also converted.
			*                  As preconditions:
			*                       -# polygons must have been created
			*                       -# the number of normals in the mesh and in its associated shapes must match the 
			*                          number of polygon vertices.
			*/
			bool EmulateNormalsByPolygonVertex(FbxMesh^ mesh);

			/** Create edge smoothing information from polygon-vertex mapped normals.
			* Existing smoothing information is removed and edge data is created if
			* none exists on the mesh.
			* \param pMesh     The mesh used to generate edge smoothing.
			* \return          \c true on success, \c false otherwise.
			* \remarks         The edge smoothing data is placed on Layer 0 of the mesh.
			*                  Normals do not need to be on Layer 0, since the first layer with
			*                  per polygon vertex normals is used.
			*/
			bool ComputeEdgeSmoothingFromNormals(FbxMesh^ mesh);

			/** Convert edge smoothing to polygon smoothing group.
			* Existing smoothing information is replaced.
			* 
			* \param pMesh     The mesh that contains the smoothing to be converted.
			* \param pIndex    The index of the layer smoothing to be converted.
			* \return          \c true on success, \c false otherwise.
			* \remarks         The smoothing group is bitwise.  The each bit of the integer represents
			*                  one smoothing group.  Therefore, there is 32 smoothing groups maximum.
			*/
			bool ComputePolygonSmoothingFromEdgeSmoothing(FbxMesh^ mesh, int index);
			/** Convert polygon smoothing group to edge smoothing.
			* Existing smoothing information is replaced.
			* 
			* \param pMesh     The mesh that contains the smoothing to be converted.
			* \param pIndex    The index of the layer smoothing to be converted
			* \return          \c true on success, \c false otherwise.
			*/
			bool ComputeEdgeSmoothingFromPolygonSmoothing(FbxMesh^ mesh, int index);



			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:

			/** Add a "triangulated mesh" geometry to the node.
			* \param pNode Pointer to the node containng the geometry.
			* \return \c true on success, \c false if the node attribute is not a mesh, 
			* a patch or a nurb.
			* \remarks The remarks relative to functions TriangulateMesh(), TriangulatePatch()
			* , TriangulateNurb() and TriangulateInPlace() are applicable.
			*/
			bool AddTriangulatedMeshGeometry(FbxNode^ node, int UVStepCoeff);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

	}
}