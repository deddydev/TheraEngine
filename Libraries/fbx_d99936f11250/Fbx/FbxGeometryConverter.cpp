#pragma once
#include "stdafx.h"
#include "FbxGeometryConverter.h"
#include "FbxSdkManager.h"
#include "FbxMesh.h"
#include "FbxPatch.h"
#include "FbxNurb.h"
#include "FbxNode.h"
#include "FbxWeightedMapping.h"
#include "FbxNurbsSurface.h"




{
	namespace FbxSDK
	{

		void FbxGeometryConverter::CollectManagedMemory()
		{
		}

		FbxGeometryConverter::FbxGeometryConverter(FbxSdkManagerManaged^ manager)
		{
			_SetPointer(new KFbxGeometryConverter(manager->_Ref()),true);			
		}

		FbxMesh^ FbxGeometryConverter::TriangulateMesh(FbxMesh^ mesh)
		{
			return gcnew FbxMesh(_Ref()->TriangulateMesh(mesh->_Ref()));
		}
		FbxMesh^ FbxGeometryConverter::TriangulatePatch(FbxPatch^ patch)
		{
			return gcnew FbxMesh(_Ref()->TriangulatePatch(patch->_Ref()));
		}
		FbxMesh^ FbxGeometryConverter::TriangulateNurb(FbxNurb^ nurb)
		{
			return gcnew FbxMesh(_Ref()->TriangulateNurb(nurb->_Ref()));
		}
		bool FbxGeometryConverter::TriangulateInPlace(FbxNode^ node)
		{
			return _Ref()->TriangulateInPlace(node->_Ref());
		}
		bool FbxGeometryConverter::AddAlternateGeometry(
			FbxNode^ node, 
			FbxGeometry^ srcGeom, 
			FbxGeometry^ altGeom,
			FbxWeightedMapping^ srcToAltWeightedMapping,
			bool convertDeformations)
		{
			return _Ref()->AddAlternateGeometry(
				node->_Ref(), 
				srcGeom->_Ref(), 
				altGeom->_Ref(),
				srcToAltWeightedMapping->_Ref(),
				convertDeformations	);
		}
		bool FbxGeometryConverter::ConvertGeometryAnimation(
			FbxNode^ node, 
			FbxGeometry^ srcGeom, 
			FbxGeometry^ dstGeom
			)
		{
			return _Ref()->ConvertGeometryAnimation(
				node->_Ref(), 
				srcGeom->_Ref(), 
				dstGeom->_Ref());
		}
		bool FbxGeometryConverter::ComputeGeometryControlPointsWeightedMapping(
			FbxGeometry^ srcGeom, 
			FbxGeometry^ dstGeom, 
			FbxWeightedMapping^ srcToDstWeightedMapping,
			bool swapUV	)
		{
			return _Ref()->ComputeGeometryControlPointsWeightedMapping(
				srcGeom->_Ref(), 
				dstGeom->_Ref(), 
				srcToDstWeightedMapping->_Ref(),
				swapUV
				);
		}
		FbxNurb^ FbxGeometryConverter::ConvertPatchToNurb(FbxPatch^ patch)
		{
			return gcnew FbxNurb(_Ref()->ConvertPatchToNurb(patch->_Ref()));
		}
		bool FbxGeometryConverter::ConvertPatchToNurbInPlace(FbxNode^ node)
		{
			return _Ref()->ConvertPatchToNurbInPlace(node->_Ref());
		}
		FbxNurbsSurface^ FbxGeometryConverter::ConvertPatchToNurbsSurface(FbxPatch^ patch)
		{
			return gcnew FbxNurbsSurface(_Ref()->ConvertPatchToNurbsSurface(patch->_Ref()));
		}
		bool FbxGeometryConverter::ConvertPatchToNurbsSurfaceInPlace(FbxNode^ node)
		{
			return _Ref()->ConvertPatchToNurbsSurfaceInPlace(node->_Ref());
		}
		FbxNurbsSurface^ FbxGeometryConverter::ConvertNurbToNurbsSurface(FbxNurb^ nurb )
		{
			return gcnew FbxNurbsSurface(_Ref()->ConvertNurbToNurbsSurface(nurb->_Ref()));
		}
		FbxNurb^ FbxGeometryConverter::ConvertNurbsSurfaceToNurb(FbxNurbsSurface^ nurb )
		{
			return gcnew FbxNurb(_Ref()->ConvertNurbsSurfaceToNurb(nurb->_Ref()));
		}
		bool FbxGeometryConverter::ConvertNurbToNurbsSurfaceInPlace(FbxNode^ node)
		{
			return _Ref()->ConvertNurbToNurbsSurfaceInPlace(node->_Ref());
		}
		bool FbxGeometryConverter::ConvertNurbsSurfaceToNurbInPlace(FbxNode^ node)
		{
			return _Ref()->ConvertNurbsSurfaceToNurbInPlace(node->_Ref());
		}
		FbxNurb^ FbxGeometryConverter::FlipNurb(FbxNurb^ nurb, bool swapUV, bool swapClusters)
		{
			return gcnew FbxNurb(_Ref()->FlipNurb(nurb->_Ref(),swapUV,swapClusters));
		}
		FbxNurbsSurface^ FbxGeometryConverter::FlipNurbsSurface(FbxNurbsSurface^ nurb, bool swapUV, bool swapClusters)
		{
			return gcnew FbxNurbsSurface(_Ref()->FlipNurbsSurface(nurb->_Ref(),swapUV,swapClusters));
		}
		bool FbxGeometryConverter::EmulateNormalsByPolygonVertex(FbxMesh^ mesh)
		{
			return _Ref()->EmulateNormalsByPolygonVertex(mesh->_Ref());
		}
		bool FbxGeometryConverter::ComputeEdgeSmoothingFromNormals(FbxMesh^ mesh)
		{
			return _Ref()->ComputeEdgeSmoothingFromNormals(mesh->_Ref());
		}
		bool FbxGeometryConverter::ComputePolygonSmoothingFromEdgeSmoothing(FbxMesh^ mesh, int index)
		{
			return _Ref()->ComputePolygonSmoothingFromEdgeSmoothing(mesh->_Ref(),index);
		}
		bool FbxGeometryConverter::ComputeEdgeSmoothingFromPolygonSmoothing(FbxMesh^ mesh, int index)
		{
			return _Ref()->ComputeEdgeSmoothingFromPolygonSmoothing(mesh->_Ref(),index);
		}
#ifndef DOXYGEN_SHOULD_SKIP_THIS


		bool FbxGeometryConverter::AddTriangulatedMeshGeometry(FbxNode^ node, int UVStepCoeff)
		{
			return _Ref()->AddTriangulatedMeshGeometry(node->_Ref(),UVStepCoeff);
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 		

	}
}