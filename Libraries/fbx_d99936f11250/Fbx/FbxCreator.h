#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <kfbxplugins/kfbxkfcurvefilters.h>
#include <kfbxplugins/kfbxnodefinder.h>


namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{
			ref class FbxStreamOptionsManaged;			
			ref class FbxIOManaged;
		}
		namespace Events
		{
			ref class FbxEmitter;
		}
	}
}

using namespace Skill::FbxSDK::IO;
using namespace Skill::FbxSDK::Events;

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxBindingTableBase;		
		ref class FbxCollectionManaged;
		ref class FbxDocumentManaged;
		ref class FbxCurveFilters;
		ref class FbxTakeNodeContainer;
		ref class FbxConstraint;
		ref class FbxDeformer;
		ref class FbxNodeAttribute;
		ref class FbxLayerContainer;
		ref class FbxGeometry;
		ref class FbxGeometryBase;
		ref class FbxSubDeformer;
		ref class FbxLayerElement;
		ref class FbxNodeFinder;
		ref class FbxObjectFilter;
		ref class FbxQuery;
		ref class FbxShadingNode;
		ref class FbxTexture;
		ref class FbxSurfaceMaterial;
		ref class FbxSurfaceLambert;
		ref class FbxKRenamingStrategy;		
		ref class FbxObjectManaged;
		ref class FbxPlug;
		ref class FbxShadingObject;

		ref class FbxCreator abstract sealed
		{
		public : 

			static FbxEmitter^  CreateFbxEmitter(KFbxEmitter* instance);
			static FbxStreamOptionsManaged^  CreateFbxStreamOptions(KFbxStreamOptions* instance);
			static FbxObjectManaged^ CreateFbxObject(KFbxObject* instance);
			static FbxShadingObject^ CreateFbxShadingObject(KFbxShadingObject* instance);
			static FbxIO^ CreateFbxIO(KFbxIO* instance);
			static FbxGeometryBase^ CreateFbxGeometryBase(KFbxGeometryBase* instance);
			static FbxGeometry^ CreateFbxGeometry(KFbxGeometry* instance);
			static FbxSurfaceMaterial^ CreateFbxSurfaceMaterial(KFbxSurfaceMaterial* instance);			
			static FbxBindingTableBase^ CreateFbxBindingTableBase(KFbxBindingTableBase* instance);
			static FbxPlug^ CreateFbxPlug(KFbxPlug* instance);
			static FbxCollectionManaged^ CreateFbxCollection(KFbxCollection* instance);
			static FbxDocumentManaged^ CreateFbxDocument(KFbxDocument* instance);
			static FbxCurveFilters^ CreateFbxCurveFilters(KFbxKFCurveFilters* instance);
			static FbxTakeNodeContainer^ CreateFbxTakeNodeContainer(KFbxTakeNodeContainer* instance);
			static FbxConstraint^ CreateFbxConstraint(KFbxConstraint* instance);
			static FbxDeformer^ CreateFbxDeformer(KFbxDeformer* instance);
			static FbxNodeAttribute^ CreateFbxNodeAttribute(KFbxNodeAttribute* instance);
			static FbxLayerContainer^ CreateFbxLayerContainer(KFbxLayerContainer* instance);
			static FbxSubDeformer^ CreateFbxSubDeformer(KFbxSubDeformer* instance);
			static FbxLayerElement^ CreateFbxLayerElement(KFbxLayerElement* instance);
			static FbxNodeFinder^ CreateFbxNodeFinder(KFbxNodeFinder* instance);
			static FbxObjectFilter^ CreateFbxObjectFilter(KFbxObjectFilter* instance);
			static FbxQuery^ CreateFbxQuery(KFbxQuery* instance);
			static FbxShadingNode^ CreateFbxShadingNode(KFbxShadingNode* instance);
			static FbxTexture^ CreateFbxTexture(KFbxTexture* instance);
			static FbxSurfaceLambert^ CreateFbxSurfaceLambert(KFbxSurfaceLambert* instance);
			static FbxKRenamingStrategy^ CreateFbxKRenamingStrategy(KRenamingStrategy* instance);

		};

	}
}
