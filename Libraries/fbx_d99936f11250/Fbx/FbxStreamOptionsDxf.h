#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "KFbxIO/kfbxstreamoptionsDxf.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		namespace IO
		{
			public ref class FbxStreamOptionsDxf abstract sealed
			{
			public:
				static String^ DEFORMATION = "DEFORMATION";
				static String^ TRIANGULATE = "TRIANGULATE";
				static String^ WELD_VERTICES = "WELD VERTICES";
				static String^ OBJECT_DERIVATION_LABEL = "OBJECT DERIVATION LABEL";
				static String^ OBJECT_DERIVATION = "OBJECT DERIVATION";
				static String^ REFERENCENODE = "REFERENCENODE";
			};

			/**	\brief This class is used for accessing the Import options of Dxf files.
			* The content of KfbxStreamOptionsDxf is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsDxfReader : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsDxfReader(KFbxStreamOptionsDxfReader* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}

				REF_DECLARE(FbxEmitter,KFbxStreamOptionsDxfReader);
				FBXOBJECT_DECLARE(FbxStreamOptionsDxfReader);				
			public:

				/** Reset all options to default values
				*/
				//void Reset();

				/** \enum EObjectDerivation   Shading modes
				* - \e eBY_LAYER       
				* - \e eBY_ENTITY
				* - \e eBY_BLOCK
				*/
				enum class ObjectDerivation
				{
					ByLayer,
					ByEntity,
					ByBlock
				};

				/** Sets the Create Root Node Option
				* \param pCreateRootNode     The boolean value to be set. 
				*/
				//void SetCreateRootNode(bool createRootNode);

				///** Sets the Weld Vertices Option
				//* \param pWeldVertices     The boolean value to be set. 
				//*/
				//void SetWeldVertices(bool weldVertices);

				/** Gets the Object Derivation
				* \return     The object variation. 
				*/
				/** Sets the Object Derivation
				* \param pDerivation     The object variation to be set. 
				*/
				property ObjectDerivation ObjDerivation
				{
					ObjectDerivation get();
					void set(ObjectDerivation value);
				}


#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();
#endif
			};



			/**	\brief This class is used for accessing the Export options of Dxf files.
			* The content of KfbxStreamOptionsDxf is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsDxfWriter : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsDxfWriter(KFbxStreamOptionsDxfWriter* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
				FBXOBJECT_DECLARE(FbxStreamOptionsDxfWriter);				
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsDxfWriter);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();
				//void Reset();
#endif
			};
			//
		}
	}
}