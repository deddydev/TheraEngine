#pragma once
#include "stdafx.h"
#include "FbxCreator.h"
#include "FbxIO.h"
#include "FbxIOSettings.h"
#include "FbxStreamOptions.h"
#include "FbxCache.h"
#include "FbxBindingTableBase.h"
#include "FbxBindingTable.h"
#include "FbxCollection.h"
#include "FbxDocument.h"
#include "FbxCurveFilters.h"
#include "FbxTakeNodeContainer.h"
#include "FbxConstraint.h"
#include "FbxDeformer.h"
#include "FbxNodeAttribute.h"
#include "FbxLayerContainer.h"
#include "FbxGeometry.h"
#include "FbxGeometryBase.h"
#include "FbxSubDeformer.h"
#include "FbxLayerElement.h"
#include "FbxNodeFinder.h"
#include "FbxObjectFilter.h"
#include "FbxQuery.h"
#include "FbxShadingNode.h"
#include "FbxTexture.h"
#include "FbxSurfaceMaterial.h"
#include "FbxSurfaceLambert.h"
#include "FbxRenamingStrategy.h"		
#include "FbxObject.h"
#include "FbxPlug.h"
#include "FbxCameraManipulator.h"
#include "FbxCharacterPose.h"
#include "FbxDocumentInfo.h"
#include "FbxGeometryWeightedMap.h"
#include "FbxGlobalSettings.h"
#include "FbxImageConverterBuffer.h"
#include "FbxObjectMetaData.h"
#include "FbxPose.h"
#include "FbxThumbnail.h"
#include "FbxStreamOptions3ds.h"
#include "FbxStreamOptionsCollada.h"
#include "FbxStreamOptionsDxf.h"
#include "FbxStreamOptionsObj.h"
#include "FbxStreamOptionsFbx.h"
#include "FbxExporter.h"
#include "FbxImporter.h"
#include "FbxShape.h"
#include "FbxBoundary.h"
#include "FbxMesh.h"
#include "FbxNurb.h"
#include "FbxNurbsCurve.h"
#include "FbxNurbsSurface.h"
#include "FbxPatch.h"
#include "FbxProceduralGeometry.h"
#include "FbxSurfaceMaterial.h"
#include "FbxSurfaceLambert.h"
#include "FbxSurfacePhong.h"
#include "FbxBindingTable.h"
#include "FbxScene.h"
#include "FbxLibrary.h"
#include "FbxControlSet.h"
#include "FbxGenericNode.h"
#include "FbxNode.h"
#include "FbxVideo.h"
#include "FbxConstraintAim.h"
#include "FbxConstraintParent.h"
#include "FbxConstraintPosition.h"
#include "FbxConstraintRotation.h"
#include "FbxConstraintScale.h"
#include "FbxConstraintSingleChainIK.h"
#include "FbxVertexCacheDeformer.h"
#include "FbxSkin.h"
#include "FbxCamera.h"
#include "FbxCameraSwitcher.h"
#include "FbxLight.h"
#include "FbxMarker.h"
#include "FbxNull.h"
#include "FbxOpticalReference.h"
#include "FbxSkeleton.h"
#include "FbxCluster.h"
#include "FbxQuery.h"
#include "FbxLayeredTexture.h"




namespace Skill
{
	namespace FbxSDK
	{
		FbxEmitter^  FbxCreator::CreateFbxEmitter(KFbxEmitter* instance)
		{
			if(!instance)
				return nullptr;
			return CreateFbxPlug((KFbxPlug*)instance);
		}
		FbxPlug^ FbxCreator::CreateFbxPlug(KFbxPlug* instance)
		{
			if(!instance)
				return nullptr;
			return CreateFbxObject(dynamic_cast<KFbxObject*>(instance));
		}
		FbxObjectManaged^ FbxCreator::CreateFbxObject(KFbxObject* instance)
		{
			if(!instance)
				return nullptr;			
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxCache::ClassId))
				return gcnew FbxCacheManaged((KFbxCache*)instance);
			else if(id.Is(KFbxCameraManipulator::ClassId))
				return gcnew FbxCameraManipulator((KFbxCameraManipulator*)instance);
			else if(id.Is(KFbxCharacterPose::ClassId))
				return gcnew FbxCharacterPose((KFbxCharacterPose*)instance);
			else if(id.Is(KFbxDocumentInfo::ClassId))
				return gcnew FbxDocumentInfo((KFbxDocumentInfo*)instance);
			else if(id.Is(KFbxGeometryWeightedMap::ClassId))
				return gcnew FbxGeometryWeightedMap((KFbxGeometryWeightedMap*)instance);
			else if(id.Is(KFbxGlobalSettings::ClassId))
				return gcnew FbxGlobalSettings((KFbxGlobalSettings*)instance);
			else if(id.Is(KFbxImageConverter::ClassId))
				return gcnew FbxImageConverter((KFbxImageConverter*)instance);
			else if(id.Is(KFbxIOSettings::ClassId))
				return gcnew FbxIOSettings((KFbxIOSettings*)instance);
			else if(id.Is(KFbxObjectMetaData::ClassId))
				return gcnew FbxObjectMetaData((KFbxObjectMetaData*)instance);
			else if(id.Is(KFbxPose::ClassId))
				return gcnew FbxPose((KFbxPose*)instance);
			else if(id.Is(KFbxThumbnail::ClassId))
				return gcnew FbxThumbnail((KFbxThumbnail*)instance);

			FbxObjectManaged^ o = nullptr;
			o = CreateFbxBindingTableBase(dynamic_cast<KFbxBindingTableBase*>(instance));
			if(o)
				return o;
			o = CreateFbxCollection(dynamic_cast<KFbxCollection*>(instance));
			if(o)
				return o;
			o = CreateFbxIO(dynamic_cast<KFbxIO*>(instance));
			if(o)
				return o;
			o = CreateFbxCurveFilters(dynamic_cast<KFbxKFCurveFilters*>(instance));
			if(o)
				return o;
			o = CreateFbxStreamOptions(dynamic_cast<KFbxStreamOptions*>(instance));
			if(o)
				return o;
			o = CreateFbxTakeNodeContainer(dynamic_cast<KFbxTakeNodeContainer*>(instance));
			if(o)
				return o;
			if(id.Is(KFbxObject::ClassId))
				return gcnew FbxObjectManaged((KFbxObject*)instance);
			return nullptr;

		}

		FbxShadingObject^ FbxCreator::CreateFbxShadingObject(KFbxShadingObject* instance)
		{
			if(!instance)
				return nullptr;
			FbxShadingObject^ o = CreateFbxSurfaceMaterial(dynamic_cast<KFbxSurfaceMaterial*>(instance));
			if(o)
				return o;

			o = CreateFbxShadingNode(dynamic_cast<KFbxShadingNode*>(instance));
			if(o)
				return o;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxShadingObject::ClassId))
				return gcnew FbxShadingObject((KFbxShadingObject*)instance);
			return nullptr;
		}
		FbxStreamOptionsManaged^  FbxCreator::CreateFbxStreamOptions(KFbxStreamOptions* instance)
		{
			if(!instance)
				return nullptr;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxStreamOptionsFbxReader::ClassId))
				return gcnew FbxStreamOptionsFbxReader((KFbxStreamOptionsFbxReader*)instance);
			else if(id.Is(KFbxStreamOptionsFbxWriter::ClassId))
				return gcnew FbxStreamOptionsFbxWriter((KFbxStreamOptionsFbxWriter*)instance);
			else if(id.Is(KFbxStreamOptions3dsReader::ClassId))
				return gcnew FbxStreamOptions3dsReader((KFbxStreamOptions3dsReader*)instance);
			else if(id.Is(KFbxStreamOptions3dsWriter::ClassId))
				return gcnew FbxStreamOptions3dsWriter((KFbxStreamOptions3dsWriter*)instance);
			else if(id.Is(KFbxStreamOptionsColladaReader::ClassId))
				return gcnew FbxStreamOptionsColladaReader((KFbxStreamOptionsColladaReader*)instance);
			else if(id.Is(KFbxStreamOptionsColladaWriter::ClassId))
				return gcnew FbxStreamOptionsColladaWriter((KFbxStreamOptionsColladaWriter*)instance);
			else if(id.Is(KFbxStreamOptionsObjReader::ClassId))
				return gcnew FbxStreamOptionsObjReader((KFbxStreamOptionsObjReader*)instance);
			else if(id.Is(KFbxStreamOptionsObjWriter::ClassId))
				return gcnew FbxStreamOptionsObjWriter((KFbxStreamOptionsObjWriter*)instance);
			else if(id.Is(KFbxStreamOptionsDxfReader::ClassId))
				return gcnew FbxStreamOptionsDxfReader((KFbxStreamOptionsDxfReader*)instance);
			else if(id.Is(KFbxStreamOptionsDxfWriter::ClassId))
				return gcnew FbxStreamOptionsDxfWriter((KFbxStreamOptionsDxfWriter*)instance);
			else if(id.Is(KFbxStreamOptions::ClassId))
				return gcnew FbxStreamOptionsManaged((KFbxStreamOptions*)instance);
			return nullptr;
		}
		
		FbxIO^ FbxCreator::CreateFbxIO(KFbxIO* instance)
		{
			if(!instance)
				return nullptr;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxExporter::ClassId))
				return gcnew FbxExporter((KFbxExporter*)instance);
			else if(id.Is(KFbxImporter::ClassId))
				return gcnew FbxImporter((KFbxImporter*)instance);
			else if(id.Is(KFbxIO::ClassId))
				return gcnew FbxIO((KFbxIO*)instance);
			return nullptr;
		}
		FbxGeometryBase^ FbxCreator::CreateFbxGeometryBase(KFbxGeometryBase* instance)
		{
			if(!instance)
				return nullptr;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxShape::ClassId))
				return gcnew FbxShape((KFbxShape*)instance);
			FbxGeometryBase^ gb = CreateFbxGeometry(dynamic_cast<KFbxGeometry*>(instance));
			if(gb)
				return gb;
			if(id.Is(KFbxGeometryBase::ClassId))
				return gcnew FbxGeometryBase((KFbxGeometryBase*)instance);
			return nullptr;
		}
		FbxGeometry^ FbxCreator::CreateFbxGeometry(KFbxGeometry* instance)
		{
			if(!instance)
				return nullptr;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxBoundary::ClassId))
				return gcnew FbxBoundary((KFbxBoundary*)instance);			
			else if(id.Is(KFbxMesh::ClassId))
				return gcnew FbxMesh((KFbxMesh*)instance);			
			else if(id.Is(KFbxNurb::ClassId))
				return gcnew FbxNurb((KFbxNurb*)instance);			
			else if(id.Is(KFbxNurbsCurve::ClassId))
				return gcnew FbxNurbsCurve((KFbxNurbsCurve*)instance);			
			else if(id.Is(KFbxNurbsSurface::ClassId))
				return gcnew FbxNurbsSurface((KFbxNurbsSurface*)instance);			
			else if(id.Is(KFbxPatch::ClassId))
				return gcnew FbxPatch((KFbxPatch*)instance);
			else if(id.Is(KFbxProceduralGeometry::ClassId))
				return gcnew FbxProceduralGeometry((KFbxProceduralGeometry*)instance);
			else if(id.Is(KFbxTrimNurbsSurface::ClassId))
				return gcnew FbxTrimNurbsSurface((KFbxTrimNurbsSurface*)instance);
			else if(id.Is(KFbxGeometry::ClassId))
				return gcnew FbxGeometry((KFbxGeometry*)instance);
			return nullptr;
		}
		FbxSurfaceMaterial^ FbxCreator::CreateFbxSurfaceMaterial(KFbxSurfaceMaterial* instance)
		{
			if(!instance)
				return nullptr;			
			FbxSurfaceMaterial^ s = CreateFbxSurfaceLambert(dynamic_cast<KFbxSurfaceLambert*>(instance));
			if(s)
				return s;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxSurfaceMaterial::ClassId))
				return gcnew FbxSurfaceMaterial((KFbxSurfaceMaterial*)instance);
			return nullptr;
		}
		FbxSurfaceLambert^ FbxCreator::CreateFbxSurfaceLambert(KFbxSurfaceLambert* instance)
		{
			if(!instance)
				return nullptr;						
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxSurfacePhong::ClassId))
				return gcnew FbxSurfacePhong((KFbxSurfacePhong*)instance);
			else if(id.Is(KFbxSurfaceLambert::ClassId))
				return gcnew FbxSurfaceLambert((KFbxSurfaceLambert*)instance);
			return nullptr;
		}		
		FbxBindingTableBase^ FbxCreator::CreateFbxBindingTableBase(KFbxBindingTableBase* instance)
		{
			if(!instance)
				return nullptr;						
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxBindingTable::ClassId))
				return gcnew FbxBindingTable((KFbxBindingTable*)instance);
			else if(id.Is(KFbxBindingTableBase::ClassId))
				return gcnew FbxBindingTableBase((KFbxBindingTableBase*)instance);
			return nullptr;
		}
		
		FbxCollectionManaged^ FbxCreator::CreateFbxCollection(KFbxCollection* instance)
		{
			if(!instance)
				return nullptr;						
			FbxCollectionManaged^ c = CreateFbxDocument(dynamic_cast<KFbxDocument*>(instance));
			if(c)
				return c;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxCollection::ClassId))
				return gcnew FbxCollectionManaged((KFbxCollection*)instance);			
			return nullptr;
		}
		FbxDocumentManaged^ FbxCreator::CreateFbxDocument(KFbxDocument* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxScene::ClassId))
				return gcnew FbxSceneManaged((KFbxScene*)instance);			
			else if(id.Is(KFbxLibrary::ClassId))
				return gcnew FbxLibrary((KFbxLibrary*)instance);
			else if(id.Is(KFbxDocument::ClassId))
				return gcnew FbxDocumentManaged((KFbxDocument*)instance);
			return nullptr;
		}
		FbxCurveFilters^ FbxCreator::CreateFbxCurveFilters(KFbxKFCurveFilters* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxKFCurveFilterConstantKeyReducer::ClassId))
				return gcnew FbxCurveFilterConstantKeyReducer((KFbxKFCurveFilterConstantKeyReducer*)instance);			
			else if(id.Is(KFbxKFCurveFilterMatrixConverter::ClassId))
				return gcnew FbxCurveFilterMatrixConverter((KFbxKFCurveFilterMatrixConverter*)instance);
			else if(id.Is(KFbxKFCurveFilterResample::ClassId))
				return gcnew FbxCurveFilterResample((KFbxKFCurveFilterResample*)instance);
			else if(id.Is(KFbxKFCurveFilterUnroll::ClassId))
				return gcnew FbxCurveFilterUnroll((KFbxKFCurveFilterUnroll*)instance);
			else if(id.Is(KFbxKFCurveFilters::ClassId))
				return gcnew FbxCurveFilters((KFbxKFCurveFilters*)instance);
			return nullptr;
		}
		FbxTakeNodeContainer^ FbxCreator::CreateFbxTakeNodeContainer(KFbxTakeNodeContainer* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();			
			if(id.Is(KFbxControlSetPlug::ClassId))
				return gcnew FbxControlSetPlug((KFbxControlSetPlug*)instance);			
			else if(id.Is(KFbxGenericNode::ClassId))
				return gcnew FbxGenericNode((KFbxGenericNode*)instance);			
			else if(id.Is(KFbxNode::ClassId))
				return gcnew FbxNode((KFbxNode*)instance);			
			else if(id.Is(KFbxVideo::ClassId))
				return gcnew FbxVideo((KFbxVideo*)instance);			



			FbxTakeNodeContainer^ t = CreateFbxConstraint(dynamic_cast<KFbxConstraint*>(instance));
			if(t)
				return t;

			t = CreateFbxShadingObject(dynamic_cast<KFbxShadingObject*>(instance));
			if(t)
				return t;
			t = CreateFbxDeformer(dynamic_cast<KFbxDeformer*>(instance));
			if(t)
				return t;
			t = CreateFbxNodeAttribute(dynamic_cast<KFbxNodeAttribute*>(instance));
			if(t)
				return t;
			t = CreateFbxSubDeformer(dynamic_cast<KFbxSubDeformer*>(instance));
			if(t)
				return t;
			if(id.Is(KFbxTakeNodeContainer::ClassId))
				return gcnew FbxTakeNodeContainer((KFbxTakeNodeContainer*)instance);
			return nullptr;
		}
		FbxConstraint^ FbxCreator::CreateFbxConstraint(KFbxConstraint* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxCharacter::ClassId))
				return gcnew FbxCharacter((KFbxCharacter*)instance);
			else if(id.Is(KFbxConstraintAim::ClassId))
				return gcnew FbxConstraintAim((KFbxConstraintAim*)instance);
			else if(id.Is(KFbxConstraintParent::ClassId))
				return gcnew FbxConstraintParent((KFbxConstraintParent*)instance);
			else if(id.Is(KFbxConstraintPosition::ClassId))
				return gcnew FbxConstraintPosition((KFbxConstraintPosition*)instance);
			else if(id.Is(KFbxConstraintRotation::ClassId))
				return gcnew FbxConstraintRotation((KFbxConstraintRotation*)instance);
			else if(id.Is(KFbxConstraintScale::ClassId))
				return gcnew FbxConstraintScale((KFbxConstraintScale*)instance);
			else if(id.Is(KFbxConstraintSingleChainIK::ClassId))
				return gcnew FbxConstraintSingleChainIK((KFbxConstraintSingleChainIK*)instance);
			else if(id.Is(KFbxConstraint::ClassId))
				return gcnew FbxConstraint((KFbxConstraint*)instance);
			return nullptr;
		}
		FbxDeformer^ FbxCreator::CreateFbxDeformer(KFbxDeformer* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxSkin::ClassId))
				return gcnew FbxSkin((KFbxSkin*)instance);
			else if(id.Is(KFbxVertexCacheDeformer::ClassId))
				return gcnew FbxVertexCacheDeformer((KFbxVertexCacheDeformer*)instance);
			else if(id.Is(KFbxDeformer::ClassId))
				return gcnew FbxDeformer((KFbxDeformer*)instance);
			return nullptr;
		}
		FbxNodeAttribute^ FbxCreator::CreateFbxNodeAttribute(KFbxNodeAttribute* instance)
		{
			if(!instance)
				return nullptr;									
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxCamera::ClassId))
				return gcnew FbxCamera((KFbxCamera*)instance);
			else if(id.Is(KFbxCameraSwitcher::ClassId))
				return gcnew FbxCameraSwitcher((KFbxCameraSwitcher*)instance);
			else if(id.Is(KFbxLight::ClassId))
				return gcnew FbxLight((KFbxLight*)instance);
			else if(id.Is(KFbxMarker::ClassId))
				return gcnew FbxMarker((KFbxMarker*)instance);
			else if(id.Is(KFbxNull::ClassId))
				return gcnew FbxNull((KFbxNull*)instance);
			else if(id.Is(KFbxOpticalReference::ClassId))
				return gcnew FbxOpticalReference((KFbxOpticalReference*)instance);
			else if(id.Is(KFbxSkeleton::ClassId))
				return gcnew FbxSkeleton((KFbxSkeleton*)instance);

			FbxNodeAttribute^ a = CreateFbxLayerContainer(dynamic_cast<KFbxLayerContainer*>(instance)); 
			if(a)
				return a;
			if(id.Is(KFbxNodeAttribute::ClassId))
				return gcnew FbxNodeAttribute((KFbxNodeAttribute*)instance);
			return nullptr;
		}
		FbxLayerContainer^ FbxCreator::CreateFbxLayerContainer(KFbxLayerContainer* instance)
		{
			if(!instance)
				return nullptr;
			FbxLayerContainer^ l = CreateFbxGeometryBase(dynamic_cast<KFbxGeometryBase*>(instance));
			if(l)
				return l;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxLayerContainer::ClassId))
				return gcnew FbxLayerContainer((KFbxLayerContainer*)instance);			
			return nullptr;
		}
		FbxSubDeformer^ FbxCreator::CreateFbxSubDeformer(KFbxSubDeformer* instance)
		{
			if(!instance)
				return nullptr;			
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxCluster::ClassId))
				return gcnew FbxCluster((KFbxCluster*)instance);			
			else if(id.Is(KFbxSubDeformer::ClassId))
				return gcnew FbxSubDeformer((KFbxSubDeformer*)instance);			
			return nullptr;
		}
		FbxLayerElement^ FbxCreator::CreateFbxLayerElement(KFbxLayerElement* instance)
		{			
			if(!instance)
				return nullptr;
			return gcnew FbxLayerElement(instance);
		}
		FbxNodeFinder^ FbxCreator::CreateFbxNodeFinder(KFbxNodeFinder* instance)
		{
			if(!instance)
				return nullptr;
			KFbxNodeFinder* nf = dynamic_cast<KFbxNodeFinder*>(instance);
			if(nf)
				return gcnew FbxNodeFinder((KFbxNodeFinder*)instance);						
			return nullptr;
		}
		FbxObjectFilter^ FbxCreator::CreateFbxObjectFilter(KFbxObjectFilter* instance)
		{
			if(!instance)
				return nullptr;			
			
			KFbxNameFilter* nf = dynamic_cast<KFbxNameFilter*>(instance);
			if(nf)
				return gcnew FbxNameFilter((KFbxNameFilter*)instance);

			KFbxObjectFilter* no = dynamic_cast<KFbxObjectFilter*>(instance);
			if(no)
				return gcnew FbxObjectFilter((KFbxObjectFilter*)instance);
			
			return nullptr;
		}
		FbxQuery^ FbxCreator::CreateFbxQuery(KFbxQuery* instance)
		{
			if(!instance)
				return nullptr;			
			
			if(dynamic_cast<KFbxQueryClassId*>(instance))
				return gcnew FbxQueryClassId((KFbxQueryClassId*)instance);			
			else if(dynamic_cast<KFbxQueryConnectionType*>(instance))
				return gcnew FbxQueryConnectionType((KFbxQueryConnectionType*)instance);
			else if(dynamic_cast<KFbxQueryIsA*>(instance))
				return gcnew FbxQueryIsA((KFbxQueryIsA*)instance);
			else if(dynamic_cast<KFbxQueryOperator*>(instance))
				return gcnew FbxQueryOperator((KFbxQueryOperator*)instance);
			else if(dynamic_cast<KFbxQueryProperty*>(instance))
				return gcnew FbxQueryProperty((KFbxQueryProperty*)instance);
			else if(dynamic_cast<KFbxUnaryQueryOperator*>(instance))
				return gcnew FbxUnaryQueryOperator((KFbxUnaryQueryOperator*)instance);
			else if(dynamic_cast<KFbxQuery*>(instance))
				return gcnew FbxQuery((KFbxQuery*)instance);			
			return nullptr;
		}
		FbxShadingNode^ FbxCreator::CreateFbxShadingNode(KFbxShadingNode* instance)
		{
			if(!instance)
				return nullptr;
			FbxShadingNode^ s = CreateFbxTexture(dynamic_cast<KFbxTexture*>(instance));
			if(s)
				return s;
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxShadingNode::ClassId))
				return gcnew FbxShadingNode((KFbxShadingNode*)instance);
			return nullptr;
		}
		FbxTexture^ FbxCreator::CreateFbxTexture(KFbxTexture* instance)
		{
			if(!instance)
				return nullptr;			
			kFbxClassId id = instance->GetClassId();
			if(id.Is(KFbxLayeredTexture::ClassId))
				return gcnew FbxLayeredTexture((KFbxLayeredTexture*)instance);
			else if(id.Is(KFbxTexture::ClassId))
				return gcnew FbxTexture((KFbxTexture*)instance);
			return nullptr;
		}		
		FbxKRenamingStrategy^ FbxCreator::CreateFbxKRenamingStrategy(KRenamingStrategy* instance)
		{
			if(!instance)
				return nullptr;			
			if(dynamic_cast<KFbxRenamingStrategy*>(instance))				
				return gcnew FbxRenamingStrategy((KFbxRenamingStrategy*)instance);
			else if(dynamic_cast<KNumberRenamingStrategy*>(instance))
				return gcnew FbxNumberRenamingStrategy((KNumberRenamingStrategy*)instance);
			else if(dynamic_cast<KRenamingStrategy*>(instance))
				return gcnew FbxKRenamingStrategy((KRenamingStrategy*)instance);
			return nullptr;
		}
	}
}
