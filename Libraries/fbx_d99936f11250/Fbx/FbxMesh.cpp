#pragma once
#include "stdafx.h"
#include "FbxMesh.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxVector4.h"
#include "FbxVector2.h"



{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxMesh,KFbxMesh);		
		
		void FbxMesh::BeginPolygon(int material, int texture , int group, bool legacy)
		{
			_Ref()->BeginPolygon(material,texture ,group,legacy);
		}
		void FbxMesh::BeginPolygon()
		{
			_Ref()->BeginPolygon();
		}

		void FbxMesh::BeginPolygonExt(int material, array<int>^ textures)
		{
			int* arr = new int[textures->Length];
			for(int i = 0;i<textures->Length;i++)
				arr[i] = textures[i];
			_Ref()->BeginPolygonExt(material,arr);
			delete[] arr;
		}

		void FbxMesh::AddPolygon(int index, int textureUVIndex)
		{
			_Ref()->AddPolygon(index,textureUVIndex);
		}
		void FbxMesh::EndPolygon()
		{
			_Ref()->EndPolygon();
		}
		int FbxMesh::PolygonCount::get()
		{
			return _Ref()->GetPolygonCount();
		}
		int FbxMesh::GetPolygonSize(int polygonIndex)
		{
			return _Ref()->GetPolygonSize(polygonIndex);
		}		
		int FbxMesh::GetPolygonGroup(int polygonIndex)
		{
			return _Ref()->GetPolygonGroup(polygonIndex);
		}		
		int FbxMesh::GetPolygonVertex(int polygonIndex, int positionInPolygon)
		{
			return _Ref()->GetPolygonVertex(polygonIndex,positionInPolygon);
		}
		void FbxMesh::GetPolygonVertexNormal(int polyIndex, int vertexIndex, FbxVector4^ normal)
		{
			_Ref()->GetPolygonVertexNormal(polyIndex,vertexIndex,*normal->_Ref());
		}

		array<int>^ FbxMesh::PolygonVertices::get()
		{
			array<int>^ arr = gcnew array<int>(_Ref()->GetPolygonVertexCount()*3);
			int* pvs = _Ref()->GetPolygonVertices();
			for	(int i = 0 ;i< arr->Length;i++)
				arr[i] = pvs[i];
			return arr;
		}
		void FbxMesh::PolygonVertices::set(array<int>^ value)
		{
			if(_Ref()->GetPolygonVertexCount()*3 != value->Length)
				throw gcnew System::ArgumentException(" size of given array missmatch by size of internal array");
			int* pvs = _Ref()->GetPolygonVertices();
			for	(int i = 0 ;i< value->Length;i++)
				pvs[i] = value[i];
		}


		int FbxMesh::PolygonVertexCount::get()
		{
			return _Ref()->GetPolygonVertexCount();
		}
		int FbxMesh::GetPolygonVertexIndex( int polygonIndex )
		{
			return _Ref()->GetPolygonVertexIndex(polygonIndex);
		}
		int FbxMesh::RemovePolygon(int polygonIndex)
		{
			return _Ref()->RemovePolygon(polygonIndex);
		}
		void FbxMesh::InitTextureUV(int count, FbxLayerElement::LayerElementType typeIdentifier)
		{
			_Ref()->InitTextureUV(count, (KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		void FbxMesh::AddTextureUV(FbxVector2^ UV, FbxLayerElement::LayerElementType typeIdentifier)
		{
			return _Ref()->AddTextureUV(*UV->_Ref(), (KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		int FbxMesh::GetTextureUVCount(FbxLayerElement::LayerElementType typeIdentifier)
		{
			return _Ref()->GetTextureUVCount((KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		int FbxMesh::UVLayerCount::get()
		{
			return _Ref()->GetUVLayerCount();
		}
		void FbxMesh::InitMaterialIndices(FbxLayerElement::MappingMode mappingMode)
		{
			_Ref()->InitMaterialIndices((KFbxLayerElement::EMappingMode)mappingMode);
		}
		void FbxMesh::InitTextureIndices(FbxLayerElement::MappingMode mappingMode, FbxLayerElement::LayerElementType textureType)
		{
			_Ref()->InitTextureIndices((KFbxLayerElement::EMappingMode)mappingMode,(KFbxLayerElement::ELayerElementType)textureType);
		}
		void FbxMesh::InitTextureUVIndices(FbxLayerElement::MappingMode mappingMode, FbxLayerElement::LayerElementType typeIdentifier)
		{
			_Ref()->InitTextureUVIndices((KFbxLayerElement::EMappingMode)mappingMode,(KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		int FbxMesh::GetTextureUVIndex(int polygonIndex, int positionInPolygon, FbxLayerElement::LayerElementType typeIdentifier)
		{
			return _Ref()->GetTextureUVIndex(polygonIndex,positionInPolygon,(KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		void FbxMesh::SetTextureUVIndex(int polygonIndex, int positionInPolygon, int index,FbxLayerElement::LayerElementType typeIdentifier)
		{
			return _Ref()->SetTextureUVIndex(polygonIndex,positionInPolygon,index,(KFbxLayerElement::ELayerElementType)typeIdentifier);
		}
		void FbxMesh::Reset()
		{
			_Ref()->Reset();
		}
		void FbxMesh::ComputeVertexNormals(bool CW)
		{
			_Ref()->ComputeVertexNormals(CW);
		}
		bool FbxMesh::CheckIfVertexNormalsCCW::get()
		{
			return _Ref()->CheckIfVertexNormalsCCW();
		}

		void FbxMesh::DuplicateVertex::CollectManagedMemory()
		{
			_Normal = nullptr;	
			_UV = nullptr;
		}
		VALUE_PROPERTY_GETSET_DEFINATION(FbxMesh::DuplicateVertex,lVertexPolyIndex,int,VertexPolyIndex);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxMesh::DuplicateVertex,lNewVertexIndex,int,NewVertexIndex);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxMesh::DuplicateVertex,lEdgeIndex,int,EdgeIndex);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxMesh::DuplicateVertex,lNormal,FbxVector4,Normal);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxMesh::DuplicateVertex,lUV,FbxVector2,UV);

		void FbxMesh::VertexNormalInfo::CollectManagedMemory()
		{
			_TotalNormal = nullptr;
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxMesh::VertexNormalInfo,mTotalNormal,FbxVector4,TotalNormal);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxMesh::VertexNormalInfo,mNumNormal,int,NumNormal);

		bool FbxMesh::CheckSamePointTwice::get()
		{
			return _Ref()->CheckSamePointTwice();
		}
		int FbxMesh::RemoveBadPolygons()
		{
			return _Ref()->RemoveBadPolygons();
		}
		void FbxMesh::BuildMeshEdgeArray()
		{
			_Ref()->BuildMeshEdgeArray();
		}
		int FbxMesh::MeshEdgeCount::get()
		{
			return _Ref()->GetMeshEdgeCount();
		}
		void FbxMesh::MeshEdgeCount::set(int value)
		{
			_Ref()->SetMeshEdgeCount(value);
		}
		int FbxMesh::GetMeshEdgeIndex( int startVertexIndex, int endVertexIndex, bool %reversed)
		{
			bool b = reversed;
			int i = _Ref()->GetMeshEdgeIndex(startVertexIndex,endVertexIndex,b);
			reversed = b;
			return i;
		}
		int FbxMesh::GetMeshEdgeIndexForPolygon( int polygon, int positionInPolygon )
		{
			return _Ref()->GetMeshEdgeIndexForPolygon(polygon,positionInPolygon );
		}
		void FbxMesh::GetMeshEdgeVertices( int edgeIndex, int %startVertexIndex, int %endVertexIndex)
		{
			int s=-1,e=-1;
			_Ref()->GetMeshEdgeVertices(edgeIndex,s,e);
			startVertexIndex = s;
			endVertexIndex = e;
		}

		void FbxMesh::BeginGetMeshEdgeVertices()
		{
			_Ref()->BeginGetMeshEdgeVertices();
		}

		void FbxMesh::EndGetMeshEdgeVertices()
		{
			_Ref()->EndGetMeshEdgeVertices();
		}

		void FbxMesh::SetMeshEdge(int edgeIndex, int value )
		{
			_Ref()->SetMeshEdge(edgeIndex,value);
		}

		int FbxMesh::AddMeshEdgeIndex(int startVertexIndex, int endVertexIndex, bool checkForDuplicates )
		{
			return _Ref()->AddMeshEdgeIndex(startVertexIndex,endVertexIndex,checkForDuplicates);
		}

		int FbxMesh::SetMeshEdgeIndex( int edgeIndex, int startVertexIndex, int endVertexIndex, bool checkForDuplicates )
		{
			return _Ref()->SetMeshEdgeIndex(edgeIndex,startVertexIndex,endVertexIndex,checkForDuplicates );
		}

		void FbxMesh::BeginAddMeshEdgeIndex()
		{
			_Ref()->BeginAddMeshEdgeIndex();
		}
		void FbxMesh::EndAddMeshEdgeIndex()
		{
			_Ref()->EndAddMeshEdgeIndex();
		}

		int FbxMesh::AddMeshEdgeIndexForPolygon( int polygonIndex, int positionInPolygon )
		{
			return _Ref()->AddMeshEdgeIndexForPolygon(polygonIndex,positionInPolygon );
		}
		bool FbxMesh::SetMeshEdgeIndex( int edgeIndex, int polygonIndex, int positionInPolygon )
		{
			return _Ref()->SetMeshEdgeIndex(edgeIndex,polygonIndex,positionInPolygon);
		}
		bool FbxMesh::IsTriangleMesh::get()
		{
			return _Ref()->IsTriangleMesh();
		}


#ifndef DOXYGEN_SHOULD_SKIP_THIS		
			/*void FbxMesh::ReservePolygonCount(int count)
			{
				_Ref()->ReservePolygonCount(count);
			}
			void FbxMesh::ReservePolygonVertexCount(int count)
			{
				_Ref()->ReservePolygonVertexCount(count);
			}*/
			CLONE_DEFINITION(FbxMesh,KFbxMesh);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}