#pragma once
#include "stdafx.h"
#include "FbxRenamingStrategy.h"
#include "FbxString.h"
#include "FbxNode.h"
#include "FbxScene.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxRenamingStrategy::FbxRenamingStrategy(Mode mod, bool onCreationRun)
			:FbxKRenamingStrategy(new KFbxRenamingStrategy((KFbxRenamingStrategy::EMode)mod,onCreationRun))
		{
			_Free = true;
		}

		FbxRenamingStrategy::FbxRenamingStrategy(Mode mod)
			:FbxKRenamingStrategy(new KFbxRenamingStrategy((KFbxRenamingStrategy::EMode)mod))
		{
			_Free = true;
		}

		void FbxRenamingStrategy::SetClashSoverType(ClashType type)
		{
			_Ref()->SetClashSoverType((KFbxRenamingStrategy::EClashType)type);
		}
		FbxKRenamingStrategy^ FbxRenamingStrategy::Clone()
		{
			return gcnew FbxRenamingStrategy((KFbxRenamingStrategy*)_Ref()->Clone());
		}

		String^ FbxRenamingStrategy::NoPrefixName(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			String^ str = gcnew String(KFbxRenamingStrategy::NoPrefixName(n));
			FREECHARPOINTER(n);
			return str;
		}
		String^ FbxRenamingStrategy::NoPrefixName (FbxStringManaged^ name)
		{
			String^ str = gcnew String(KFbxRenamingStrategy::NoPrefixName(*name->_Ref()));
			return str;
		}

		String^ FbxRenamingStrategy::NameSpace::get()
		{
			return gcnew String(_Ref()->GetNameSpace());
		}

		void FbxRenamingStrategy::SetInNameSpaceSymbol(FbxStringManaged^ nameSpaceSymbol)
		{
			_Ref()->SetInNameSpaceSymbol(*nameSpaceSymbol->_Ref());
		}

		void FbxRenamingStrategy::SetOutNameSpaceSymbol(FbxStringManaged^ nameSpaceSymbol)
		{
			_Ref()->SetOutNameSpaceSymbol(*nameSpaceSymbol->_Ref());
		}

		void FbxRenamingStrategy::SetCaseSensibility(bool isCaseSensitive)
		{
			_Ref()->SetCaseSensibility(isCaseSensitive);
		}

		void FbxRenamingStrategy::SetReplaceNonAlphaNum(bool replaceNonAlphaNum)
		{
			_Ref()->SetReplaceNonAlphaNum(replaceNonAlphaNum);
		}

		void FbxRenamingStrategy::SetFirstNotNum(bool firstNotNum)
		{
			_Ref()->SetFirstNotNum(firstNotNum);
		}

		bool FbxRenamingStrategy::RenameUnparentNameSpace(FbxNode^ node, bool isRoot)
		{
			return _Ref()->RenameUnparentNameSpace(node->_Ref(),isRoot);
		}

		bool FbxRenamingStrategy::RemoveImportNameSpaceClash(FbxNode^ node)
		{
			return _Ref()->RemoveImportNameSpaceClash(node->_Ref());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		bool FbxRenamingStrategy::PropagateNameSpaceChange(FbxNode^ node, FbxStringManaged^ oldNS, FbxStringManaged^ newNS)
		{
			return _Ref()->PropagateNameSpaceChange(node->_Ref(),*oldNS->_Ref(), *newNS->_Ref());
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		void FbxSceneRenamer::CollectManagedMemory()
		{
			_Scene = nullptr;
		}


		FbxSceneManaged^  FbxSceneRenamer::Scene::get()
		{
			return _Scene;
		}

		FbxSceneRenamer::FbxSceneRenamer(FbxSceneManaged^ scene)		
		{
			_SetPointer(new KFbxSceneRenamer(scene->_Ref()),true);
		}
		void FbxSceneRenamer::RenameFor(RenamingMode mode)
		{
			_Ref()->RenameFor((KFbxSceneRenamer::ERenamingMode)mode);
		}
		void FbxSceneRenamer::ResolveNameClashing(bool fromFbx, bool ignoreNS, bool isCaseSensitive,
			bool replaceNonAlphaNum, bool firstNotNum,
			FbxStringManaged^ inNameSpaceSymbol, FbxStringManaged^ outNameSpaceSymbol,
			bool noUnparentNS /*for MB < 7.5*/, bool removeNameSpaceClash)
		{
			_Ref()->ResolveNameClashing(fromFbx,ignoreNS,isCaseSensitive,replaceNonAlphaNum,firstNotNum,
				*inNameSpaceSymbol->_Ref(),*outNameSpaceSymbol->_Ref(),
				noUnparentNS ,removeNameSpaceClash);
		}

	}
}