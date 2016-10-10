#pragma once
#include "stdafx.h"
#include "FbxProductInfo.h"
#include "FbxString.h"


{
	namespace FbxSDK
	{		

		void FbxProductInfo::CollectManagedMemory()
		{
			/*_BuildNumber = nullptr;
			_OS = nullptr;
			_PackageVersion = nullptr;
			_Product = nullptr;
			_URL = nullptr;
			_Version = nullptr;		*/	
		}

		//FbxProductInfo::FbxProductInfo(FbxString^ product, FbxString^ packageVersion, FbxString^ os, FbxString^ version,
		//		FbxString^ buildNumber, FbxString^ url, FbxString^ lang , FbxString^ message)
		//{
		//	/*_SetPointer(new KFbxProductInfo(*product->_Ref(),*packageVersion->_Ref(),*os->_Ref(),*version->_Ref(),
		//		*buildNumber->_Ref(),*url->_Ref(),*lang->_Ref(),*message->_Ref()),true);*/

		//	_BuildNumber = buildNumber;
		//	_OS = os;
		//	_PackageVersion = packageVersion;
		//	_Product = product;
		//	_URL = url;
		//	_Version = version;

		//}

		//FbxString^ FbxProductInfo::Product::get()
		//{
		//	return _Product;
		//}
		//bool FbxProductInfo::Show::get(){return _Ref()->Show();}
		//
		//FbxString^ FbxProductInfo::PackageVersion::get()
		//{
		//	return _PackageVersion;
		//}		
		//FbxString^ FbxProductInfo::OS::get()
		//{
		//	return _OS;
		//}		
		//FbxString^ FbxProductInfo::Version::get()
		//{
		//	return _Version;
		//}		
		//FbxString^ FbxProductInfo::BuildNumber::get()
		//{
		//	return _BuildNumber;
		//}		
		//FbxString^ FbxProductInfo::URL::get()
		//{
		//	return _URL;
		//}

		//int FbxProductInfo::LanguageCount::get(){return _Ref()->GetLangCount();}
		//FbxString^ FbxProductInfo::GetLanguage(int i)
		//{
		//	return gcnew FbxString(_Ref()->GetLang(i));
		//}
		//FbxString^ FbxProductInfo::GetMessageString(int i)
		//{
		//	return gcnew FbxString(_Ref()->GetMessageStr(i));
		//}
	}
}