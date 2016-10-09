#pragma once
#include "stdafx.h"
#include "FbxPropertyDef.h"

namespace Skill
{
	namespace FbxSDK
	{

		/**	\brief Class to manage property handle.
		* \nosubgrouping
		*/
		public ref class FbxPropertyHandle : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxPropertyHandle,KFbxPropertyHandle);
			INATIVEPOINTER_DECLARE(FbxPropertyHandle,KFbxPropertyHandle);	
			/**
			* \name Constructor and Destructor
			*/
			//@{
		internal:
			FbxPropertyHandle(KFbxPropertyHandle instance)
			{
				_SetPointer(new KFbxPropertyHandle(),true);
				*_Ref() = instance;
			}
		public:

		//	//! Create an instance
		//	static FbxPropertyHandle^ Create();
		//	//! Create an instance with given instance.
		//	static FbxPropertyHandle^ Create(FbxPropertyHandle^ instanceOf);
		//	//! Create an instance with given name and type.
		//	static FbxPropertyHandle^  Create(String^ name,FbxType type);
		//	//! Create an instance with given name and typeinfo.
		//	static FbxPropertyHandle^  Create(String^ name, FbxPropertyHandle^ typeInfo);
		//	/** If this property is root property,delete the property page.
		//	* \return  False
		//	*/
		//	bool Destroy();
		//public:
		//	//! Default constructor. 
		//	DEFAULT_CONSTRUCTOR(FbxPropertyHandle,KFbxPropertyHandle);
		//	//! Copy constructor.
		//	FbxPropertyHandle(FbxPropertyHandle^ address);				

		//public:
		//	//!  Character constructor. 
		//	//FbxPropertyHandle(KFbxPropertyPage* pPage,kFbxPropertyId pId=kFbxProperyIdRoot);


		//	//@}

		//	/**
		//	* \name Assignment and basic info
		//	*/
		//	//@{
		//public:
		//	// Assignment
		//	//! KFbxPropertyHandle assignment operator.
		//	//KFbxPropertyHandle &operator =	(KFbxPropertyHandle const &pHandle);
		//	//! Equality operator.
		//	virtual bool Equals(System::Object^ obj)
		//	{
		//		FbxPropertyHandle^ h = dynamic_cast<FbxPropertyHandle^>(obj);
		//		if(h)
		//			return *_Ref() == *h->_Ref();
		//		return false;
		//	}
		//	//! Inequality operator.
		//	//bool				operator !=	(KFbxPropertyHandle const &pHandle) const;
		//	//! Compare type info together
		//	bool Is(FbxPropertyHandle^ handle);
		//	//! Judge validity
		//	bool Valid();

		//	//! Get the property name
		//	VALUE_PROPERTY_GET_DECLARE(String^,Name);
		//	//! Get the property type
		//	VALUE_PROPERTY_GET_DECLARE(FbxType,Type);
		//	//! Get the property type info
		//	VALUE_PROPERTY_GET_DECLARE(FbxPropertyHandle^,TypeInfo);
		//	//! Get the property label
		//	/** Set a label to the property
		//	* \param pLabel    The given label string
		//	* \return  If succeed, return true.
		//	*/
		//	VALUE_PROPERTY_GETSET_DECLARE(String^,Label);				

		//	// Flag management
		//	//! Get the property attribute state
		//	VALUE_PROPERTY_GET_DECLARE(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType,Flags);

		//	/**	According the given parameter Change the attributes of the property.
		//	* \param pFlags The given flags used as mask.
		//	* \param pValue If pValue is true, set mask with given flags, otherwise unset mask with given flags.
		//	* \return  If succeed, return true.
		//	*/
		//	bool ModifyFlags(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flags, bool value);

		//	/**	Gets the inheritance type for the given flag. 
		//	* \param pFlags The flag to query
		//	* \param pCheckReferences Decide whether check instance. If it is true, check instance.
		//	* \return The inheritance type
		//	*/
		//	FbxInheritType	GetFlagsInheritType(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flags, bool checkReferences);

		//	/**Sets the inheritance type for the given flag
		//	* \param pFlags The flag to set 
		//	* \param pType The inheritance type to set 
		//	* \return  If succeed, return true.
		//	*/
		//	bool SetFlagsInheritType( Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flags, FbxInheritType type );


		//	//! Get the property user data.
		//	/** Set user data to the property
		//	* \param pUserData The given user data
		//	* \return  If succeed, return true.
		//	*/
		//	VALUE_PROPERTY_GETSET_DECLARE(System::IntPtr,UserData);							

		//	//! Get the property user tag
		//	int	GetUserTag();

		//	/** Set user tag to the property
		//	* \param pUserData The given user tag
		//	* \return  If succeed, return true.
		//	*/
		//	bool SetUserTag(int userData);
		//	//@}

		//	/**
		//	* \name Enum management
		//	*/
		//	//@{
		//public:

		//	/** Add new value at the end of the enumlist in the property.
		//	* \param pStringValue The given new value
		//	* \return  The index of the value.
		//	*/
		//	int	AddEnumValue(String^ stringValue);

		//	/** Insert new value at the given index of the enumlist in property.
		//	* \param pIndex The given index
		//	* \param pStringValue The given new value
		//	*/
		//	void InsertEnumValue(int index, String^ stringValue);

		//	/** Get the enum count of enumlist in property
		//	* \return The enum count of enumlist in property
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(int,EnumCount);

		//	/** Set value at the given index of the enumlist in the property.
		//	* \param pIndex  The given index
		//	* \param pStringValue The given new value used to instead the old value.
		//	*/
		//	void SetEnumValue(int index,String^ stringValue);

		//	/** Remove the value at the index of the enumlist in the property.
		//	* \param pIndex    The given index
		//	*/
		//	void RemoveEnumValue(int index);

		//	/** Get the value at the index of enumlist in the property.
		//	* \param pIndex    The given index
		//	* \return The value at the given index
		//	*/
		//	//char *	GetEnumValue(int pIndex);
		//	//@}


		//	/**
		//	* \name Child and Struct management
		//	*/
		//	//@{
		//public:
		//	/** Add a property to the property page.
		//	* \param pName The name of property.
		//	* \param pTypeInfo The added property's type info.
		//	* \return The handle of the new added property
		//	*/
		//	FbxPropertyHandle^	Add(String^ name,FbxPropertyHandle^ typeInfo);

		//	//! Creat the map for find property in the property page
		//	void BeginCreateOrFindProperty();
		//	//! Clear the map which created for find property.
		//	void EndCreateOrFindProperty();

		//	/** Judge if the property is the root property.
		//	* \return Return true if this property is root property.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,IsRoot);

		//	/** Judge if the property is the child property of the given parent property.
		//	* \param pParent The given parent property handle
		//	* \return Return true if this property is child of given property.
		//	*/
		//	bool IsChildOf(FbxPropertyHandle^ parent);

		//	/** Judge if the property is descendent property of the given property.
		//	* \param pParent The given parent property handle
		//	* \return Return true if this property is descendant of given property.
		//	*/
		//	bool IsDescendentOf(FbxPropertyHandle^ parent);

		//	/** Get parent property
		//	* \return If the parent property is exist, return the property handle,otherwise return -1.
		//	*/
		//	FbxPropertyHandle^	GetParent() const;

		//	/** Set parent property handle.No matter what enters,the result is always false.
		//	* \return False
		//	*/
		//	bool SetParent( FbxPropertyHandle^ other );

		//	/**  Get child property 
		//	* \return  If the child property is exist, return the property handle,otherwise return -1.
		//	*/
		//	FbxPropertyHandle^ GetChild();

		//	/**  Get sibling property  
		//	* \return If the sibling property is exist, return the property handle,otherwise return -1.
		//	*/
		//	FbxPropertyHandle^ GetSibling();

		//	/**  Get first descendent property 
		//	* \return If the descendent property is exist, return the first descendent property handle,otherwise return -1.
		//	*/
		//	FbxPropertyHandle^	GetFirstDescendent();

		//	/**  Get first descendent property which after the given property 
		//	* \param pHandle The given property handle
		//	* \return If the descendent property can be found after the given property, 
		//	* return the first found property handle,otherwise return -1.
		//	*/
		//	FbxPropertyHandle^ GetNextDescendent(FbxPropertyHandle^ handle);

		//	/** Find the property with given name 
		//	* \param pName The given property name
		//	* \param pCaseSensitive Decide if the given property name is case sensitive
		//	* \return  Return a property handle which be created with the found property   
		//	*/
		//	FbxPropertyHandle^ Find (String^ name,bool caseSensitive);

		//	/** Find the property with given name and type info.
		//	* \param pName The given property name
		//	* \param pTypeInfo The given property type info
		//	* \param pCaseSensitive Decide if the given property name is case sensitive
		//	* \return  Return a property handle which be created with the found property   
		//	*/
		//	FbxPropertyHandle^ Find (String^ name,FbxPropertyHandle^ typeInfo,bool caseSensitive);

		//	/** Separate the given name by  children separator string and then find the property.The step is  
		//	*  strip the first part of the name and search, if the property can be found, stip the second part  
		//	*  of the name and continue search, until no property be found,then return the last found property.
		//	* \param pName The given property name
		//	* \param pChildrenSeparator The given children separator string 
		//	* \param pCaseSensitive Decide if the given property name is case sensitive
		//	* \return  Return a property handle which be created with the found property   
		//	*/
		//	FbxPropertyHandle^	Find (String^ name,String^ childrenSeparator,bool caseSensitive);

		//	/** Separate the given name by  children separator string and then find the property.The step is  
		//	*  strip the first part of the name and search, if the property can be found, stip the second part  
		//	*  of the name and continue search, until no property be found,then return the last found property.
		//	* \param pName The given property name
		//	* \param pChildrenSeparator The given children separator string 
		//	* \param pTypeInfo The given property type info
		//	* \param pCaseSensitive Decide if the given property name is case sensitive
		//	* \return  Return a property handle which be created with the found property   
		//	*/
		//	FbxPropertyHandle^	Find (String^ name,String^ childrenSeparator,FbxPropertyHandle^ typeInfo,bool caseSensitive);
		//	//@}

		//	/**
		//	* \name Connection management
		//	*/
		//	//@{
		//public:

		//	/** Connect source property.
		//	* \param pSrc    The given source property
		//	* \param pType    The given property type
		//	* \return If connect successfully, return true,otherwise, return false.
		//	*/
		//	bool ConnectSrc(FbxPropertyHandle^ src,FbxConnectionType type); 

		//	/** Get source properties' count.
		//	* \param pFilter    The filter used to get sub connection point.If it is not zero,return the source count of the sub connection poin.
		//	* Otherwise, return the src count of this property.
		//	* \return The count of source properties
		//	*/
		//	int	GetSrcCount(FbxConnectionPointFilter^ filter); 

		//	/** Get source property with the given index.
		//	* \param pFilter    The filter used to get sub connection point.If it is not zero,return the source property of the sub connection poin.
		//	* Otherwise, return the source property of this property.
		//	* \param pIndex    The given index
		//	* \return The source property handle.
		//	*/
		//	FbxPropertyHandle^ GetSrc(FbxConnectionPointFilter^ filter,int index); 

		//	/** Disconnect source property.
		//	* \param pSrc    The given source property
		//	* \return If disconnect successfully, return true,otherwise, return false.
		//	*/
		//	bool DisconnectSrc(FbxPropertyHandle^ src);

		//	/** Judge if it is connected with the given source property.
		//	* \param pSrc    The given source property
		//	* \return If it is connected, return true,otherwise, return false.
		//	*/
		//	bool IsConnectedSrc(FbxPropertyHandle^ src);

		//	/** Connect destination property.
		//	* \param pDst    The given destination property
		//	* \param pType    The given property type
		//	* \return If connect successfully, return true,otherwise, return false.
		//	*/
		//	bool ConnectDst(FbxPropertyHandle^ dst,FbxConnectionType type); 

		//	/** Get destination properties' count.
		//	* \param pFilter    The filter used to get sub connection point.If it is not zero,return the destination count of the sub connection poin.
		//	* Otherwise, return the destination count of this property.
		//	* \return The count of destination properties
		//	*/
		//	int GetDstCount (FbxConnectionPointFilter^ filter); 

		//	/** Get destination property with the given index.
		//	* \param pFilter    The filter used to get sub connection point.If it is not zero,return the destination property of the sub connection poin.
		//	* Otherwise, return the destination property of this property.
		//	* \param pIndex    The given index
		//	* \return The destination property handle.
		//	*/
		//	FbxPropertyHandle^ GetDst (FbxConnectionPointFilter^ filter,int index); 

		//	/** Disconnect destination property.
		//	* \param pDst    The given destination property
		//	* \return If disconnect successfully, return true,otherwise, return false.
		//	*/
		//	bool DisconnectDst(FbxPropertyHandle^ dst);

		//	/** Judge if it is connected with the given destination property.
		//	* \param pDst    The given destination property
		//	* \return If it is connected, return true,otherwise, return false.
		//	*/
		//	bool IsConnectedDst(FbxPropertyHandle^ dst);

		//	//! Clear connect cache
		//	void ClearConnectCache();
		//	//@}

		//	/**
		//	* \name Min and Max
		//	*/
		//	//@{
		//public:

		//	/** Judge if this property has min value.
		//	* \return If min value exist, return true,otherwise, return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,HasMin);

		//	/** Get the min value and value type of this property.
		//	* \param pValue    The min value of this property.
		//	* \param pValueType The value type of this property.
		//	* \return If the min value exist, return true,otherwise, return false.
		//	*/
		//	bool GetMin(IntPtr value,FbxType valueType);

		//	/** Set the min value and value type for this property.
		//	* \param pValue    The given min value .
		//	* \param pValueType The given value type .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	bool SetMin (IntPtr value,FbxType valueType);

		//	/** According the given value and its value type, set the min value and value type for this property.
		//	* \param pValue    The given value .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	//template <class T> inline bool  SetMin		( T const &pValue )			{ return SetMin( &pValue,FbxTypeOf(pValue) ); }

		//	/** Get the min value of this property.
		//	* \param pFBX_TYPE    Not be used in this function .
		//	* \return The min value of this property
		//	*/
		//	//template <class T> inline T		GetMin		( T const *pFBX_TYPE) const	{ T lValue; GetMin( &lValue,FbxTypeOf(lValue) ); return lValue; }

		//	/** Judge if this property has soft min value.
		//	* \return If soft min value exist, return true,otherwise, return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool, HasSoftMin);

		//	/** Get the soft min value and value type of this property.
		//	* \param pValue    The soft min value of this property.
		//	* \param pValueType The value type of this property.
		//	* \return If the soft min value exist, return true,otherwise, return false.
		//	*/
		//	bool GetSoftMin(IntPtr value,FbxType valueType);

		//	/** Set the soft min value and value type for this property.
		//	* \param pValue    The given soft min value .
		//	* \param pValueType The given value type .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	bool SetSoftMin(IntPtr value,FbxType valueType);

		//	/** According the given value and its value type, set the soft min value and value type for this property.
		//	* \param pValue    The given value .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	//template <class T> inline bool  SetSoftMin	( T const &pValue )			{ return SetSoftMin( &pValue,FbxTypeOf(pValue) ); }

		//	/** Get the soft min value of this property.
		//	* \param pFBX_TYPE    Not be used in this function .
		//	* \return The soft min value of this property
		//	*/
		//	//template <class T> inline T		GetSoftMin	( T const *pFBX_TYPE) const	{ T lValue; GetSoftMin( &lValue,FbxTypeOf(lValue) ); return lValue; }

		//	/** Judge if this property has max value.
		//	* \return If max value exist, return true,otherwise, return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,HasMax);

		//	/** Get the max value and value type of this property.
		//	* \param pValue    The max value of this property.
		//	* \param pValueType The value type of this property.
		//	* \return If the max value exist, return true,otherwise, return false.
		//	*/
		//	bool GetMax	(IntPtr,value,FbxType valueType);

		//	/** Set the max value and value type for this property.
		//	* \param pValue    The given max value .
		//	* \param pValueType The given value type .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	bool SetMax (IntPtr value,FbxType valueType);

		//	/** According the given value and its value type, set the max value and value type for this property.
		//	* \param pValue    The given value .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	//template <class T> inline bool  SetMax		( T const &pValue )			{ return SetMax( &pValue,FbxTypeOf(pValue) ); }

		//	/** Get the max value of this property.
		//	* \param pFBX_TYPE    Not be used in this function .
		//	* \return The max value of this property
		//	*/
		//	//template <class T> inline T		GetMax		( T const *pFBX_TYPE) const	{ T lValue; GetMax( &lValue,FbxTypeOf(lValue) ); return lValue; }

		//	/** Judge if this property has soft max value.
		//	* \return If soft max value exist, return true,otherwise, return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,HasSoftMax);

		//	/** Get the soft max value and value type of this property.
		//	* \param pValue    The soft max value of this property.
		//	* \param pValueType The value type of this property.
		//	* \return If the soft max value exist, return true,otherwise, return false.
		//	*/
		//	bool GetSoftMax(IntPtr value,FbxType valueType);

		//	/** Set the soft max value and value type for this property.
		//	* \param pValue    The given soft max value .
		//	* \param pValueType The given value type .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	bool SetSoftMax	(IntPtr value,FbxType valueType);

		//	/** According the given value and its value type, set the soft max value and value type for this property.
		//	* \param pValue    The given value .
		//	* \return If it be set successfully, return true,otherwise, return false.
		//	*/
		//	//template <class T> inline bool  SetSoftMax	( T const &pValue )			{ return SetSoftMax( &pValue,FbxTypeOf(pValue) ); }

		//	/** Get the soft max value of this property.
		//	* \param pFBX_TYPE    Not be used in this function .
		//	* \return The soft max value of this property
		//	*/
		//	//template <class T> inline T		GetSoftMax	( T const *pFBX_TYPE) const	{ T lValue; GetSoftMax( &lValue,FbxTypeOf(lValue) ); return lValue; }


		//	/**
		//	* \name Value 
		//	*/
		//	//@{
		//public:

		//	/** Get value inherit type of this property.
		//	* \param pCheckReferences   If it is true,check instance of this property page,otherwise,only check this page.
		//	* \return The value inherit type of this property
		//	*/
		//	FbxInheritType GetValueInheritType(bool checkReferences);

		//	/** Set value inherit type for this property .
		//	* \param pType  The given value inherit type.
		//	* \return If set successfully, return true,otherwise, return false.
		//	*/
		//	bool SetValueInheritType(FbxInheritType type);

		//	/** Get default value and value type of this property .
		//	* \param pValue  The gotten default value of this property.
		//	* \param pValueType The gotten default value type of this property.
		//	* \return If default value be gotten successfully, return true,otherwise, return false.
		//	*/
		//	bool GetDefaultValue(IntPtr value,FbxType valueType);

		//	/** Get value and value type of this property .
		//	* \param pValue  The gotten value of this property.
		//	* \param pValueType The gotten value type of this property.
		//	* \return If value be gotten successfully, return true,otherwise, return false.
		//	*/
		//	bool Get(IntPtr value,FbxType valueType);

		//	/** Set property value and value type for this property.
		//	* \param pValue    The given property value .
		//	* \param pValueType The given property value type 
		//	* \param pCheckValueEquality If it is true, when the given value is equal with
		//	* the property value, the property value will not be set.
		//	* \return If the property value be set successfully, return true,otherwise, return false.
		//	*/
		//	bool Set(IntPtr value, FbxType valueType,bool checkValueEquality);

		//	/** Set property value with the given value .
		//	* \param pValue  The given value .
		//	* \return If set successfully, return true,otherwise, return false.
		//	*/
		//	//template <class T> inline bool  Set( T const &pValue )			{ return Set( &pValue,FbxTypeOf(pValue) ); }

		//	/** get property value.
		//	* \param pFBX_TYPE  Not be used.
		//	* \return The gotten property value.
		//	*/
		//	//template <class T> inline T		Get( T const *pFBX_TYPE) const	{ T lValue; Get( &lValue,FbxTypeOf(lValue) ); return lValue; }
		//	//@}

		//	/**
		//	* \name Page settings
		//	*/
		//	//@{
		//public:

		//	/** Set the property page data pointer.
		//	* \param pData  The given page data pointer.
		//	*/
		//	/** Get property page data pointer.
		//	* \return The gotten property page data pointer.
		//	*/
		//	VALUE_PROPERTY_GETSET_DECLARE(IntPtr,PageDataPtr);							

		//	//@}

		//	/**
		//	* \name Page Internal Entry Management
		//	*/
		//	//@{

		//	/** Push properties to parent instance.
		//	* \return If push successful return true,otherwise,return false.
		//	*/
		//	bool PushPropertiesToParentInstance();


		//	//@}

		//	/**
		//	* \name Reference Management
		//	*/
		//	//@{
		//public:
		//	/** Judge if this property page is a instance of other page.
		//	* \return If this property page is a instance of other page, return true,otherwise,return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,IsAReferenceTo);

		//	/** Get the property page which this property page make reference to
		//	* \return The property page which this property page make reference to
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(IntPtr,ReferenceTo);

		//	/** Judge if this property page is referenced by other pages.
		//	* \return If this property page is referenced by other pages, return true,otherwise,return false.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(bool,IsReferencedBy);

		//	/** Get the count of property pages which make reference to this property page.
		//	* \return The count of property pages which make reference to this property page.
		//	*/
		//	VALUE_PROPERTY_GET_DECLARE(int,ReferencedByCount);

		//	/** According the given index,get the property page which make reference to this property page.
		//	* \param pIndex The given index
		//	* \return The pointer to the property page which reference to this property page and be found by index.
		//	*/
		//	IntPtr GetReferencedBy(int index); 
		//	//@}

		};


	}
}