#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <kfbxplugins/kfbxproductinfo.h>


{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;
		/**FBX SDK product information class
		* \nosubgrouping
		*/
		public ref class FbxProductInfo : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxProductInfo,KFbxProductInfo);
			INATIVEPOINTER_DECLARE(FbxProductInfo,KFbxProductInfo);		

		public:
			///**
			//* \name Constructors and Destructor
			//*/
			////@{

			///** Constructor.
			//* \param pProduct                 Product name.
			//* \param pPackageVersion          Package version.
			//* \param pOS                      Operating System.
			//* \param pVersion                 Product version.
			//* \param pBuildNumber             Build number.
			//* \param pURL                     Product URL.
			//* \param pLang                    Product language
			//* \param pMessage                 Product message               
			//*/
			//FbxProductInfo(FbxString^ product, FbxString^ packageVersion, FbxString^ os, FbxString^ version,
			//	FbxString^ buildNumber, FbxString^ url, FbxString^ lang , FbxString^ message);

			///** Constructor.
			//* \param pProduct                 Product name.
			//* \param pPackageVersion          Package version.
			//* \param pOS                      Operating System.
			//* \param pVersion                 Product version.
			//* \param pBuildNumber             Build number.
			//* \param pURL                     Product URL.
			//* \param pMessageList             Product message list
			//* \param pShow                    \c True if product can show.
			//*/
			///*FbxProductInfo(FbxString^ product, FbxString^ packageVersion, FbxString^ OS, FbxString^ version,
			//	FbxString^ buildNumber, FbxString^ URL, FbxStringList *pMessageList = NULL, bool pShow = true);*/			
			////@}

			///**
			//* \name Access.
			//*/
			////@{

			///** Retrieve the product name.
			//*\return                 Product name.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,Product);

			///** Get whether product can show.
			//* \return                \c True if product can show, \c false otherwise.
			//*/
			//VALUE_PROPERTY_GET_DECLARE(bool,Show);

			///** Retrieve product package version.
			//*\return                 Product package version.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,PackageVersion);

			///** Retrieve the OS.
			//*\return                 OS.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,OS);

			///** Retrieve product version .
			//*\return                 Product version.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,Version);

			///** Retrieve product build number.
			//*\return                 Product build number.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,BuildNumber);

			///** Retrieve product URL.
			//*\return                 Product URL.
			//*/
			//REF_PROPERTY_GET_DECLARE(FbxString,URL);

			/** Retrieve product language count.
			* \return                Product language count.
			*/
			//VALUE_PROPERTY_GET_DECLARE(int,LanguageCount);

			///** Retrieve a product language specified by the index i.
			//* \param i               The index of product language.
			//* \return                Product language.
			//*/
			//FbxString^ GetLanguage(int i);

			///** Retrieve a product message specified by the index i.
			//* \param i               The index of product message.
			//* \return                Product message.
			//*/
			//FbxString^ GetMessageString(int i);			

		};

		/** FBX SDK product information builder class
		* \nosubgrouping
		*/
		public ref class FbxProductInfoBuilder
		{
		/*internal:
			KFbxProductInfoBuilder* builder;
			bool isNew;
			FbxProductInfoBuilder(KFbxProductInfoBuilder* b);*/
		//public:

		//	/**
		//	* \name Constructor and Destructor
		//	*/
		//	//@{

		//	//! Constructor.
		//	KFbxProductInfoBuilder();

		//	//! Destructor.
		//	virtual ~KFbxProductInfoBuilder ();

		//	//@}

		//	/** Load file product info;
		//	* Then fill an array of KFbxProductInfo for this pProduct (e.g. Maya, Max...),
		//	* pPackageVersion (7.0, 7.5...), pOS (Linux, Windows, Windows64, ...), 
		//	* (function pFCompareProducts returns 0 for these values)
		//	* and that are more recent than the given pVersion and pBuildNumber
		//	* (function pFCompareVersions returns 1 for these values).
		//	* \param pUrl                         Product URL.
		//	* \param pArrayProductInfo            Result array of KFbxProductInfos after comparison. 
		//	* \param pRefProductInfo              The product to compare with.
		//	* \param pFCompareProducts            The fuction to compare product, package version and OS.
		//	* \param pFCompareVersions            The fuction to compare version and buildnumber.
		//	* \return                             \c True if OK,\c false otherwise.
		//	*/
		//	bool FillArray(
		//		FbxString pUrl,
		//		KFbxArrayProductInfo &pArrayProductInfo,
		//		KFbxProductInfo *pRefProductInfo,
		//		int (*pFCompareProducts)(const void*, const void*),
		//		int (*pFCompareVersions)(const void*, const void*)
		//		);

		//private:
		//	// Array of all KFbxProductInfo found in mXmlDoc
		//	KFbxArrayProductInfo	mArrayProductInfo;
		};

	}
}