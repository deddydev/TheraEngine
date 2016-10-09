#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxPlug.h"
#include <kfbxplugins/kfbxplug.h>

namespace Skill
{
	namespace FbxSDK
	{			
		//ref class Fbxplug;
		ref class FbxSdkManagerManaged;
		ref class FbxStringManaged;
		ref class FbxPropertyHandle;

		//public delegate FbxPlug^ FbxPlugConstructor(Skill::FbxSDK::FbxSdkManager^ manager, String^ name, FbxPlugInfo^ from ,String^ fbxType, String^ fbxSubType);		

		ref class FbxPlugConstructorController abstract sealed
		{		
		/*private:
			static int numFunc = -1;
			static System::Collections::Generic::Dictionary<int ,FbxPlugConstructor^>^ collection;
			static FbxPlugConstructorController();
		public:
			static int Add(FbxPlugConstructor^ func);

			static KFbxPlug* CallFunction(int index,KFbxSdkManager& pManager, const char* pName, const KFbxPlug* pFrom, const char* pFBXType, const char* pFBXSubType);*/
		};

		/** Base To define the ClassId of an object
		* \remarks This class Id helps the fbxsdk identify the class hierarchy of plugs and objects. Each Plug is identified by a class id and a parent classId
		* \nosubgrouping
		*/
		public ref class FbxClassId : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxClassId,kFbxClassId);			
			GET_NATIVEPOINTER_DECLARE_BASE(FbxClassId,kFbxClassId);
			IS_SAME_AS_DECLARE_BASE();

		internal:		

			FbxClassId(kFbxClassId id)
			{
				_SetPointer(new kFbxClassId(),true);
				*_FbxClassId = id;
			}
		//	int constractorIndex;
		//	KFbxPlug* ConstractorFunction(KFbxSdkManager& pManager, const char* pName, const KFbxPlug* pFrom, const char* pFBXType, const char* pFBXSubType);
		//		kFbxPlugConstructor PlugConstructor;
		protected:
			FbxClassId^ parent;
		public:
			/** 
			* \name Constructor and Destructor.
			*/
			//@{

			//!Constructor.
			FbxClassId();

			/** Constructor.
			* \param pClassName         Class name.
			* \param pParentClassId     Parent class id.
			* \param pConstructor       Pointer to a function which constructs KFbxPlug.
			* \param pFBXType           Fbx file type name.
			* \param pFBXSubType        Fbx file subtype name.
			*/
			//FbxClassId(System::String^ className, FbxClassId^ parentClassId, FbxPlugConstructor^ constructor,
			//	String^ fbxType,String^ fbxSubType)
			//{
			//	char* n = new char(FbxString::NumCharToCreateString);
			//	FbxString::StringToChar(className,n);

			//	char* t = new char(FbxString::NumCharToCreateString);
			//	FbxString::StringToChar(fbxType,t);

			//	char* s = new char(FbxString::NumCharToCreateString);
			//	FbxString::StringToChar(fbxSubType,s);


			//	constractorIndex = FbxPlugConstructorController::Add(constructor);
			//	//PlugConstructor = &this->ConstractorFunction;
			//	id = new kFbxClassId(n,*parentClassId->id,&this->ConstractorFunction,t,s);
			//	isNew = true;
			//}


			//! Delete this class id.
			void Destroy();
			//@}

			/** Retrieve the class name. 
			* \return              Class name.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,Name);			

			/**Retrieve the parent class id.
			*\return      Parent class id.
			*/
			REF_PROPERTY_GET_DECLARE(FbxClassId,Parent);			

			/** Creat a KFbxPlug from the specified KFbxPlug
			* \param pManager              The object manager
			* \param pName                 KFbxPlug name
			* \param pFrom                 The specified KFbxPlug
			* \return                      KFbxPlug
			*/
			FbxPlug^ Create(FbxSdkManagerManaged^ manager, String^ name, FbxPlug^ from);
			//
			//	/** Override the KFbxPlug constructor.
			//	  * \param pConstructor         New KFbxPlug constructor.
			//	  */
			//	bool Override(kFbxPlugConstructor pConstructor);
			//
			/** Test if this class is a hierarchical children of the specified class type  
			* \param pId                   Representing the class type  
			* \return                      \c true if the object is a hierarchical children of the type specified, \c false otherwise.               
			*/
			bool Is(FbxClassId^ id);

			/** Equivalence operator.
			* \param pClassId             Another class id to be compared with this class id.
			* \return                     \c true if equal, \c false otherwise.
			*/

			virtual bool Equals(Object^ obj) override
			{
				FbxClassId^ o = dynamic_cast<FbxClassId^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}			

			/** Retrieve the information of this class id.
			* \return                     the class information.
			*/
			//inline KFbxClassIdInfo* GetClassIdInfo() { return mClassInfo; }
			//
			//   /** Retrieve the information of this class id.
			//	 * \return                      the class information.
			//	 */
			//    inline const KFbxClassIdInfo* GetClassIdInfo() const  { return mClassInfo; }
			//			
		public:
			/** Retrieve the fbx file type name.
			* \param pAskParent   a flag on whether to ask the parent for file type name.
			* \return             the fbx file type name.
			*/
			String^ GetFbxFileTypeName(bool askParent);
			String^ GetFbxFileTypeName();

			//!Retrieve the fbx file subtype name.				
			String^ GetFbxFileSubTypeName();


			/** Get whether this class type is valid.
			* \return             \c ture if valid, \c false otherwise.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(bool,IsValid);			

			//! Get object type prefix.
			//! Set object type prefix.				
			VALUE_PROPERTY_GETSET_DECLARE(String^,ObjectTypePrefix);

			//!Get the default property handle of root class.
			FbxPropertyHandle^ GetRootClassDefaultPropertyHandle();

			/**Increase the instance reference count of this class type.
			* \return             the instance reference of this type after increase.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ClassInstanceIncRef);			

			/**Decrease the instance reference count of this class type.
			* \return             the instance reference of this type after decrease.
			*/				
			VALUE_PROPERTY_GET_DECLARE(int,ClassInstanceDecRef);			

			/**Retrieve the instance reference count of this class type.
			* \return             the instance reference of this type.
			*/				
			VALUE_PROPERTY_GET_DECLARE(int,GetInstanceRef);				
		};		
	}
}