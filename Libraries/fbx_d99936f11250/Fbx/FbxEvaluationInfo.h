#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		ref class FbxTime;
		ref class FbxSdkManagerManaged;

		/**	\brief This class contains evaluation info.
		* \nosubgrouping
		*/
		public ref class FbxEvaluationInfo : IFbxNativePointer
		{			
			INTERNAL_CLASS_DECLARE(FbxEvaluationInfo,KFbxEvaluationInfo);
			REF_DECLARE(FbxEvaluationInfo,KFbxEvaluationInfo);
			DESTRUCTOR_DECLARE_2(FbxEvaluationInfo);
			INATIVEPOINTER_DECLARE(FbxEvaluationInfo,KFbxEvaluationInfo);

		public:			

		//	// Overridable Test functions
		//public:
			/**
			* \name Create and Destroy
			*/
			//@{
			/** Create an instance.
			* \return The pointer to the created instance.
			*/
		//	static FbxEvaluationInfo^ Create(FbxSdkManager^ manager);

		//public:
		//
		//	//!Destroy an allocated version of the KFbxEvaluationInfo.
		//	void Destroy();
		//	//@}

		//	/**
		//	* \name Set and Change the evaluation info
		//	*/
		//	//@{

		//	/* Get time
		//	* \return The time value.
		//	*/
		//	/** Set time 
		//	* \param pTime The given time value .
		//	*/				
		//	VALUE_PROPERTY_GETSET_DECLARE(FbxTime^,Time);

		//	/** Get evaluation ID
		//	* \return The evaluation ID.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(kFbxEvaluationId,EvaluationId);			

		//	//! Update evaluation ID, the value get one more every time.
		//	void UpdateEvaluationId();
		};


	}
}