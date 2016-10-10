#pragma once
#include "stdafx.h"
#include "FbxXRefManager.h"
#include "FbxProperty.h"
#include "FbxString.h"
#include "FbxDocument.h"


{
	namespace FbxSDK
	{	
		void FbxXRefManager::CollectManagedMemory()
		{
		}
		int FbxXRefManager::GetUrlCount(FbxPropertyManaged^ p)
		{
			return KFbxXRefManager::GetUrlCount(*p->_Ref());
		}
		int FbxXRefManager::GetUrlCount(FbxStringManaged^ url)
		{
			return KFbxXRefManager::GetUrlCount(*url->_Ref());
		}
		int FbxXRefManager::GetUrlCount(String^ url)
		{
			STRINGTO_CONSTCHAR_ANSI(u,url);
			FbxString s(u);
			FREECHARPOINTER(u);
			return KFbxXRefManager::GetUrlCount(s);
		}

		bool FbxXRefManager::IsRelativeUrl(FbxPropertyManaged^ p,int index)
		{
			return KFbxXRefManager::IsRelativeUrl(*p->_Ref(),index);
		}

		String^ FbxXRefManager::GetUrl(FbxPropertyManaged^ p,int index)
		{
			FbxString s = KFbxXRefManager::GetUrl(*p->_Ref(),index);
			CONVERT_FbxString_TO_STRING(s,str);
			return str;
		}
		void FbxXRefManager::GetUrl(FbxPropertyManaged^ p,int index,FbxStringManaged^ urlOut)
		{
			*urlOut->_Ref() = KFbxXRefManager::GetUrl(*p->_Ref(),index);			
		}

		/** Return The nth Relative Url stored in the property
		* upon return the pXRefProject will return the name of the XRef Project closest to the Url of the property
		* \return The Url if it is valid relative
		*/
		bool FbxXRefManager::GetResolvedUrl(FbxPropertyManaged^ p ,int index,FbxStringManaged^ resolvedPath)
		{
			return _Ref()->GetResolvedUrl(*p->_Ref(),index,*resolvedPath->_Ref());
		}
		bool FbxXRefManager::GetResolvedUrl(FbxPropertyManaged^ p ,int index,String^ resolvedPath)
		{
			STRINGTO_CONSTCHAR_ANSI(r,resolvedPath);
			FbxString s(r);
			FREECHARPOINTER(r);
			return _Ref()->GetResolvedUrl(*p->_Ref(),index,s);
		}	


		bool FbxXRefManager::GetResolvedUrl(String^ url,FbxDocumentManaged^ doc, String^ resolvedPath)
		{
			STRINGTO_CONSTCHAR_ANSI(u,url);
			STRINGTO_CONSTCHAR_ANSI(r,resolvedPath);
			FbxString s(r);			
			bool b = _Ref()->GetResolvedUrl(u,doc->_Ref(),s);
			FREECHARPOINTER(r);
			FREECHARPOINTER(u);
			return b;
		}

		bool FbxXRefManager::GetFirstMatchingUrl(String^ prefix, String^ optExt,FbxDocumentManaged^ doc,String^ resolvedPath)
		{

			STRINGTO_CONSTCHAR_ANSI(pf,prefix);
			STRINGTO_CONSTCHAR_ANSI(pe,optExt);
			STRINGTO_CONSTCHAR_ANSI(r,resolvedPath);
			bool b = _Ref()->GetFirstMatchingUrl(pf,pe,doc->_Ref(),FbxString(r));
			FREECHARPOINTER(pf);
			FREECHARPOINTER(pe);
			FREECHARPOINTER(r);
			return b;
		}
		bool FbxXRefManager::AddXRefProject   (String^ name,String^ url)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(u,url);
			bool b = _Ref()->AddXRefProject(n,u);
			FREECHARPOINTER(n);
			FREECHARPOINTER(u);
			return b;
		}
		bool FbxXRefManager::AddXRefProject(String^ name,String^ extension,String^ url)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(u,url);
			STRINGTO_CONSTCHAR_ANSI(e,extension);
			bool b = _Ref()->AddXRefProject(n,e,u);
			FREECHARPOINTER(n);
			FREECHARPOINTER(u);
			FREECHARPOINTER(e);
			return b;
		}

		bool FbxXRefManager::AddXRefProject(FbxDocumentManaged^ doc)
		{
			return _Ref()->AddXRefProject(doc->_Ref());
		}
		bool FbxXRefManager::RemoveXRefProject(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			bool b = _Ref()->RemoveXRefProject(n);
			FREECHARPOINTER(n);
			return b;
		}
		bool FbxXRefManager::RemoveAllXRefProjects()
		{
			return _Ref()->RemoveAllXRefProjects();
		}

		int FbxXRefManager::XRefProjectCount::get()
		{
			return _Ref()->GetXRefProjectCount();
		}
		String^ FbxXRefManager::GetXRefProjectName(int index)
		{
			return gcnew String(_Ref()->GetXRefProjectName(index));
		}

		String^ FbxXRefManager::GetXRefProjectUrl(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			const char* nStr = _Ref()->GetXRefProjectUrl(n);
			FREECHARPOINTER(n);
			if(nStr)
				return gcnew String(nStr);			
			return nullptr;
		}
		String^ FbxXRefManager::GetXRefProjectUrl(int index)
		{
			const char* n = _Ref()->GetXRefProjectUrl(index);
			if(n)
				return gcnew String(n);
			return nullptr;
		}		
		/*bool FbxXRefManager::HasXRefProject(String^ name)		
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			bool b = _Ref()->HasXRefProject(n);
			FREECHARPOINTER(n);
			return b;
		}*/

		bool FbxXRefManager::GetResolvedUrl(String^ url,String^ resolvePath)
		{
			STRINGTO_CONSTCHAR_ANSI(u,url);
			STRINGTO_CONSTCHAR_ANSI(r,resolvePath);
			bool b = _Ref()->GetResolvedUrl(u,FbxString(r));
			FREECHARPOINTER(u);
			FREECHARPOINTER(r);
			return b;
		}
	}	
}