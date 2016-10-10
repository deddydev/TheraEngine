#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxType.h"

namespace FbxSDK
{		
	ref class FbxStringManaged;

	/**FBX SDK data type class
	*\nosubgrouping
	*/
	public ref class FbxDataTypeManaged : IFbxNativePointer
	{
		BASIC_CLASS_DECLARE(FbxDataTypeManaged,FbxDataType);			
		GET_NATIVEPOINTER_DECLARE_BASE(FbxDataTypeManaged,FbxDataType);
		IS_SAME_AS_DECLARE_BASE();

	internal:
		
		FbxDataTypeManaged(FbxDataType t)
		{
			_FbxDataTypeManaged = new FbxDataType();
			*_FbxDataTypeManaged = t;
			_Free = true;
		}
	public:
		static FbxDataTypeManaged^ Create(String^ name,FbxType type);
		static FbxDataTypeManaged^ Create(String^ name,FbxDataTypeManaged^ dataType);
		/**
		*\name Constructor and Destructor.
		*/
		//@{						

		//!Copy constructor.
		FbxDataTypeManaged(FbxDataTypeManaged^ dataType );

		//!Destroy this datatype.
		void Destroy();

		/**Constructor.
		*\param pTypeInfoHandle                Type information handle
		*/
		//FbxDataType( FbxTypeInfoHandle const &pTypeInfoHandle );

	//	//!Destructor.
	//	~KFbxDataType();
	//	//@}


	public:
		/**Assignment operator
		*\param pDataType               Datatype whose value is assigned to this datatype.
		*\return                        this datatype
		*/
		//inline KFbxDataType& operator = (const KFbxDataType &pDataType) { mTypeInfoHandle=pDataType.mTypeInfoHandle; return *this;  }
		void CopyFrom(FbxDataTypeManaged^ dataType);

		/**
		* \name boolean operation
		*/
		//@{

		/**Equality operator
		*\param pDataType                Datatype to compare to.
		*\return                         \c true if equal,\c false otherwise.
		*/
		virtual bool Equals(System::Object^ obj)override
		{
			FbxDataTypeManaged^ t = dynamic_cast<FbxDataTypeManaged^>(obj);
			if(t)
				return *t->_Ref() == *_Ref();
			return false;
		}
			
	public:

		/**Test whether this datatype is a valid datatype.
		*\return         \c true if valid, \c false otherwise.
		*/
		virtual VALUE_PROPERTY_GET_DECLARE(bool,IsValid);

		/** Test if this datatype is the specified datatype. 
		* \param pDataType               Datatype to compare to.
		* \return                        \c true if this datatype is the specified datatype, \c false otherwise. 
		*/
		bool Is(FbxDataTypeManaged^ dataType);

		/** Retrieve this data type.
		* \return     this data type.
		*/
		property FbxType Type
		{
			FbxType get();
		}

		/** Retrieve data type name.
		* \return     data type name.
		*/
		property String^ Name
		{
			String^ get();
		}		
	//public:
	//	/** Retrieve the information handle of this data type.
	//	* \return       information handle of this data type.
	//	*/
	//	inline KFbxTypeInfoHandle const &GetTypeInfoHandle() const  { return mTypeInfoHandle; }

	//	friend class KFbxSdkManager;
	};
}