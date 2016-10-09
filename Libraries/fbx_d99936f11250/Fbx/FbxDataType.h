#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxType.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;

		/**FBX SDK data type class
		*\nosubgrouping
		*/
		public ref class FbxDataType : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxDataType,KFbxDataType);			
			GET_NATIVEPOINTER_DECLARE_BASE(FbxDataType,KFbxDataType);
			IS_SAME_AS_DECLARE_BASE();

		internal:
		
			FbxDataType(KFbxDataType t)
			{
				_FbxDataType = new KFbxDataType();
				*_FbxDataType = t;
				_Free = true;
			}
		public:
			static FbxDataType^ Create(String^ name,FbxType type);
			static FbxDataType^ Create(String^ name,FbxDataType^ dataType);
			/**
			*\name Constructor and Destructor.
			*/
			//@{						

			//!Copy constructor.
			FbxDataType(FbxDataType^ dataType );

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
			void CopyFrom(FbxDataType^ dataType);

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
				FbxDataType^ t = dynamic_cast<FbxDataType^>(obj);
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
			bool Is(FbxDataType^ dataType);

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
}