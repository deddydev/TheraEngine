#pragma once
#include "stdafx.h"
#include "FbxKRenamingStrategy.h"


{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;
		ref class FbxNode;
		/** \brief This class contains the description of the FBX renaming strategy.
		* \nosubgrouping
		* The KFbxRenamingStrategy object can be setup to rename all the objects in a scene.
		* It can remove nameclashing, remove illegal characters, manage namespaces, and manage backward compatibility.
		*
		*/

		public ref class FbxRenamingStrategy : FbxKRenamingStrategy
		{
			REF_DECLARE(FbxKRenamingStrategy,KFbxRenamingStrategy);
		internal:
			FbxRenamingStrategy(KFbxRenamingStrategy* instance) : FbxKRenamingStrategy(instance)
			{
				_Free = false;
			}
		public:

			/** \enum EMode
			* - \e eTO_FBX
			* - \e eFROM_FBX
			* - \e eMODE_COUNT
			*/
			enum class Mode
			{
				ToFbx = KFbxRenamingStrategy::eTO_FBX,
				FromFbx = KFbxRenamingStrategy::eFROM_FBX,
				ModeCount = KFbxRenamingStrategy::eMODE_COUNT
			};

			/** \enum EClashType
			* - \e eNAMECLASH_AUTO
			* - \e eNAMECLASH_TYPE1
			* - \e eNAMECLASH_TYPE2
			*/
			enum class ClashType
			{
				NameclashAuto = KFbxRenamingStrategy::eNAMECLASH_AUTO,
				NameclashType1 = KFbxRenamingStrategy::eNAMECLASH_TYPE1,
				NameclashType2 = KFbxRenamingStrategy::eNAMECLASH_TYPE2
			};


			//! Constructor.
			FbxRenamingStrategy(Mode mod, bool onCreationRun);
			FbxRenamingStrategy(Mode mod);

			//! Setup the strategy to perform this algorithm
			void SetClashSoverType(ClashType type);

			/** Rename.
			* \param pName     New name.
			* \return          \c true if successful, \c false otherwise.
			*/
			//virtual bool Rename(KName& pName);

			//! Empty all memories about given names
			//virtual void Clear();

			/** Spawn mechanism.  
			* Create a dynamic renaming strategy instance of the same type.
			* \return     new KRenamingStrategy
			*/
			virtual FbxKRenamingStrategy^ Clone() override;

			/** Returns a name with its prefix removed.
			* \param pName    A name containning a prefix.
			* \return         The part of pName following the "::"
			*/
			static String^ NoPrefixName (String^ name);
			static String^ NoPrefixName (FbxStringManaged^ name);

			/** Get the namespace of the last renamed object.
			* \return     Char pointer to the namespace.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(String^,NameSpace);

			/** Sets the current scene namespace symbol.
			* \param pNameSpaceSymbol     namespace symbol.
			*/
			virtual void SetInNameSpaceSymbol(FbxStringManaged^ nameSpaceSymbol);

			/** Sets the wanted scene namespace symbol.
			* \param pNameSpaceSymbol     namespace symbol.
			*/
			virtual void SetOutNameSpaceSymbol(FbxStringManaged^ nameSpaceSymbol);

			/** Sets case sensitivity for nameclashing.
			* \param pIsCaseSensitive     Set to \c true to make the nameclashing case sensitive.
			*/
			virtual void SetCaseSensibility(bool isCaseSensitive);

			/** Sets the flag for character acceptance during renaming.
			* \param pReplaceNonAlphaNum     Set to \c true to replace illegal characters with an underscore ("_").  
			*/
			virtual void SetReplaceNonAlphaNum(bool replaceNonAlphaNum);

			/** Sets the flag for first character acceptance during renaming.
			* \param pFirstNotNum     Set to \c true to add an underscore to the name if the first character is a number.
			*/
			virtual void SetFirstNotNum(bool firstNotNum);

			/** Recusively renames all the unparented namespaced objects (Prefix mode) starting from this node.
			* \param pNode       Parent node.
			* \param pIsRoot     The root node.
			* \remarks           This function adds "_NSclash" when it encounters an unparented namespaced object.
			*/
			virtual bool RenameUnparentNameSpace(FbxNode^ node, bool isRoot);
			bool RenameUnparentNameSpace(FbxNode^ node)
			{
				return RenameUnparentNameSpace(node,false);
			}

			/** Recusively removes all the unparented namespaced "key" starting from this node.
			* \param pNode     Parent node.
			* \remarks         This function removes "_NSclash" when encountered. This is the opposite from RenameUnparentNameSpace.
			*/
			virtual bool RemoveImportNameSpaceClash(FbxNode^ node);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			//virtual void GetParentsNameSpaceList(KFbxNode* pNode, KArrayTemplate<FbxString*> &pNameSpaceList);
			virtual bool PropagateNameSpaceChange(FbxNode^ node, FbxStringManaged^ oldNS, FbxStringManaged^ newNS);		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

		ref class FbxSceneManaged;

		/** \brief This class contains the description of the FBX scene renamer.
		* \nosubgrouping
		* The KFbxSceneRenamer provides a way to easily rename objects in a scene without 
		* using the KFbxRenamingStrategy class. KFbxSceneRenamer removes nameclashing, illegal characters, and manages namespaces.
		* 
		*
		*/

		public ref class FbxSceneRenamer : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxSceneRenamer,KFbxSceneRenamer);
			INATIVEPOINTER_DECLARE(FbxSceneRenamer,KFbxSceneRenamer);
		public:

			/** Create an object of type KFbxSceneRenamer.
			* \param pScene     Scene to be renamed.
			*/
			FbxSceneRenamer(FbxSceneManaged^ scene);			

			/** \enum ERenamingMode
			* - \e eNONE
			* - \e eMAYA_TO_FBX5
			* - \e eMAYA_TO_FBX_MB75
			* - \e eMAYA_TO_FBX_MB70
			* - \e eFBXMB75_TO_FBXMB70
			* - \e eFBX_TO_FBX
			* - \e eMAYA_TO_FBX
			* - \e eFBX_TO_MAYA
			* - \e eLW_TO_FBX
			* - \e eFBX_TO_LW
			* - \e eXSI_TO_FBX
			* - \e eFBX_TO_XSI
			* - \e eMAX_TO_FBX
			* - \e eFBX_TO_MAX
			* - \e eMB_TO_FBX
			* - \e eFBX_TO_MB
			* - \e eDAE_TO_FBX
			* - \e eFBX_TO_DAE
			*/
			enum class RenamingMode
			{ 
				None = KFbxSceneRenamer::eNONE,
				MayaToFbx5 = KFbxSceneRenamer::eMAYA_TO_FBX5,
				MayaToFbxMb75 = KFbxSceneRenamer::eMAYA_TO_FBX_MB75 ,
				MayaToFbxMb70 = KFbxSceneRenamer::eMAYA_TO_FBX_MB70,
				FbxMb75ToFbxMb70 = KFbxSceneRenamer::eFBXMB75_TO_FBXMB70,
				FbxToFbx = KFbxSceneRenamer::eFBX_TO_FBX,
				MayaToFbx = KFbxSceneRenamer::eMAYA_TO_FBX,
				FbxToMaya = KFbxSceneRenamer::eFBX_TO_MAYA,
				LwToFbx = KFbxSceneRenamer::eLW_TO_FBX,
				FbxToLw = KFbxSceneRenamer::eFBX_TO_LW,
				XsiToFbx = KFbxSceneRenamer::eXSI_TO_FBX,
				FbxToXsi = KFbxSceneRenamer::eFBX_TO_XSI,
				MaxToFbx = KFbxSceneRenamer::eMAX_TO_FBX,
				FbxToMax = KFbxSceneRenamer::eFBX_TO_MAX,
				MbToFbx = KFbxSceneRenamer::eMB_TO_FBX,
				FbxToMb = KFbxSceneRenamer::eFBX_TO_MB,
				DaeToFbx = KFbxSceneRenamer::eDAE_TO_FBX,
				FbxToDae = KFbxSceneRenamer::eFBX_TO_DAE
			};

			REF_PROPERTY_GET_DECLARE(FbxSceneManaged,Scene);

			void RenameFor(RenamingMode mode);

			/** Rename all object to remove name clashing.
			* \param pFromFbx                  Set to \c true to enable this flag.
			* \param pIgnoreNS                 Set to \c true to enable this flag.
			* \param pIsCaseSensitive          Set to \c true to enable case sensitive renaming.
			* \param pReplaceNonAlphaNum       Set to \c true to replace non-alphanumeric characters with underscores ("_").
			* \param pFirstNotNum              Set to \c true toadd a leading _ if first char is a number (for xs:NCName).
			* \param pInNameSpaceSymbol        Identifier of a namespace.
			* \param pOutNameSpaceSymbol       Identifier of a namespace.
			* \param pNoUnparentNS             Set to \c true to not not allow unparent namespace.
			* \param pRemoveNameSpaceClash     Set to \c true to remove NameSpaceClash token.
			* \return void.
			*/
			void ResolveNameClashing(bool fromFbx, bool ignoreNS, bool isCaseSensitive,
				bool replaceNonAlphaNum, bool firstNotNum,
				FbxStringManaged^ inNameSpaceSymbol, FbxStringManaged^ outNameSpaceSymbol,
				bool noUnparentNS /*for MB < 7.5*/, bool removeNameSpaceClash);		
		};


	}
}