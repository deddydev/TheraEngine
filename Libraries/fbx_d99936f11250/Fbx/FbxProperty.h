#pragma once
#include "stdafx.h"
#include "FbxPropertyDef.h"
#include <kfbxplugins/kfbxproperty.h>

namespace Skill
{
	namespace FbxSDK
	{				
		ref class FbxDataType;
		ref class FbxObjectManaged;
		ref class FbxClassId;
		ref class FbxStringManaged;
		ref class FbxQuery;
		ref class FbxPropertyHandle;
		ref class FbxCriteria;
		ref class FbxCurveNode; 
		ref class FbxCurve;
		ref class FbxColor;
		ref class FbxDouble1;
		ref class FbxDouble2;
		ref class FbxDouble3;
		ref class FbxVector4;
		ref class FbxVector2;
		/** \brief Class to hold user properties.
		* \nosubgrouping
		*/
		public ref class FbxProperty : FbxPropertyFlags, IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxProperty,KFbxProperty);			
			GET_NATIVEPOINTER_DECLARE_BASE(FbxProperty,KFbxProperty);	
			IS_SAME_AS_DECLARE_BASE();
		internal:
			FbxProperty(KFbxProperty p)
			{
				_SetPointer(new KFbxProperty(),true);
				*this->_FbxProperty = p;				
			}

			/**
			* \name Constructor and Destructor.
			*/
			//@{
		public:

			virtual property bool IsValid { bool get();} 
			//			//using Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType;
			//			/*using FbxPropertyFlags::eNO_FLAG;
			//			using FbxPropertyFlags::eANIMATABLE;
			//			using FbxPropertyFlags::eUSER;
			//			using FbxPropertyFlags::eTEMPORARY;
			//			using FbxPropertyFlags::ePUBLISHED;
			//			using FbxPropertyFlags::ePSTATIC;
			//
			//			using FbxPropertyFlags::eNOT_SAVABLE;
			//			using FbxPropertyFlags::eHIDDEN;
			//
			//			using FbxPropertyFlags::eUI_DISABLED;
			//			using FbxPropertyFlags::eUI_GROUP;
			//			using FbxPropertyFlags::eUI_BOOLGROUP;
			//			using FbxPropertyFlags::eUI_EXPANDED;
			//			using FbxPropertyFlags::eUI_NOCAPTION;
			//			using FbxPropertyFlags::eUI_PANEL;*/
			//
			//         typedef FbxPropertyFlags::eFbxPropertyFlags EFlags;

			/** Create a property
			* \param pCompoundProperty
			* \param pName
			* \param pDataType
			* \param pLabel
			* \param pCheckForDuplicate
			* \param pWasFound
			*/
			static FbxProperty^ Create(FbxProperty^ compoundProperty,
				System::String^ name, FbxDataType^ dataType,System::String^  label,bool checkForDuplicate,bool %wasFound);

			static FbxProperty^ Create(FbxProperty^ compoundProperty,
				System::String^ name, FbxDataType^ dataType,System::String^  label);

			//         /** Create a property
			//           * \param pParentProperty
			//           * \param pName
			//           * \param pDataType
			//           * \param pValue
			//           * \param pFlags
			//           * \param pCheckForDuplicate
			//           * \param pForceSet
			//           */
			//         template<typename T> inline static KFbxProperty Create(KFbxProperty const &pParentProperty, char const* pName,KFbxDataType const &pDataType,T const &pValue,eFbxPropertyFlags pFlags=eNO_FLAG,bool pCheckForDuplicate=true,bool pForceSet=true)
			//         {
			//             if( !pCheckForDuplicate )
			//             {
			//                 KFbxProperty lProperty = Create(pParentProperty, pName, pDataType, "", pCheckForDuplicate);
			//                 lProperty.ModifyFlag(pFlags, true); // modify the flags before we set the value
			//                 lProperty.Set(pValue);
			//                 return lProperty;
			//             }

			//             // First check for a duplicate
			//             KFbxProperty lProperty = pParentProperty.Find(pName);

			//             // only set if we are forcing the set, or we are actually creating the property
			//             // (as opposed to returning an existing one)
			//             //bool lSetValue = pForceSet ? true : !lProperty.IsValid();

			//             if( !lProperty.IsValid() )
			//                 lProperty = Create(pParentProperty, pName, pDataType, "", false); // don't check because we already did

			//             lProperty.ModifyFlag(pFlags, true); // modify the flags before we set the value
			//             //if( lSetValue )
			//             //  lProperty.Set(pValue);
			//             lProperty.Set( &pValue,FbxTypeOf(pValue),!pForceSet);
			//             return lProperty;
			//         }

			/** Create a dynamic property
			* \param pObject
			* \param pName
			* \param pDataType
			* \param pLabel
			* \param pCheckForDuplicate
			* \param pWasFound
			*/
			static FbxProperty^ Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label,bool checkForDuplicate,bool %wasFound);
			static FbxProperty^ Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label);

			/** Create a dynamic property from an other property
			* \param pObject
			* \param pFromProperty
			* \param pCheckForDuplicate
			*/
			static FbxProperty^ Create(FbxObjectManaged^ obj, FbxProperty^ fromProperty, bool checkForDuplicate);
			static FbxProperty^ Create(FbxObjectManaged^ obj, FbxProperty^ fromProperty);

			/** Create a dynamic property from an other property
			* \param pCompoundProperty
			* \param pFromProperty
			* \param pCheckForDuplicate
			*/
			static FbxProperty^ Create(FbxProperty^ compoundProperty,
				FbxProperty^ fromProperty, bool checkForDuplicate);
			static FbxProperty^ Create(FbxProperty^ compoundProperty,
				FbxProperty^ fromProperty);

			/** Destroy a dynamic property
			*/
			void Destroy(bool recursive, bool dependents);
			void Destroy();

			/** Static Property Constructors
			*/
			DEFAULT_CONSTRUCTOR(FbxProperty,KFbxProperty);

			/** Copy constructor for properties
			*/
			FbxProperty(FbxProperty^ p);

			/** Copy constructor for properties
			*/
			FbxProperty(FbxPropertyHandle^ p);			

		public:
			//@}

			/**
			* \name Property Identification.
			*/
			//@{
		public:
			/** Get the property data type definition.
			* \return The properties KFbxDataType
			*/
			property FbxDataType^ PropertyDataType
			{
				FbxDataType^ get();
			}

			/** Get the property internal name.
			* \return Property internal name string.
			*/
			property FbxStringManaged^ Name
			{
				FbxStringManaged^ get();
			}

			/** Get the property internal name.
			* \return Property internal name string.
			*/			         
			property FbxStringManaged^ HierarchicalName
			{
				FbxStringManaged^ get();
			}

			/** Get the property label.
			* \param pReturnNameIfEmpty If \c true, allow this method to return the internal name.
			* \return The property label if set, or the property internal name if the pReturnNameIfEmpty
			* flag is set to \c true and the label has not been defined.
			* \remarks Some applications may choose to ignore the label field and work uniquely with the
			* internal name. Therefore, it should not be taken for granted that a label exists. Also, remember
			* that the label does not get saved in the FBX file. It only exist while the property object is
			* in memory.
			*/
			FbxStringManaged^ GetLabel(bool returnNameIfEmpty);


			/** Set a label to the property.
			* \param pLabel Label string.
			*/
			void SetLabel(FbxStringManaged^ label);

			/** Get the object that contains the property.
			* \return the property object owner or null if the property is an orphan.
			*/
			REF_PROPERTY_GET_DECLARE(FbxObjectManaged,Object);			

			//@}

			/**
			* \name User data
			*/
			//@{			         
			property int UserTag
			{
				int get();
				void set(int value);
			}
			//         void  SetUserDataPtr(void* pUserData);
			//         void* GetUserDataPtr();
			//     //@}

			/**
			* \name Property Flags.
			*/
			//@{
			/** Change the attributes of the property.
			* \param pFlag Property attribute identifier.
			* \param pValue New state.
			*/
			void ModifyFlag(FbxPropertyFlags::FbxPropertyFlagsType flag, bool value);

			/** Get the property attribute state.
			* \param pFlag Property attribute identifier.
			* \return The currently set property attribute state.
			*/
			bool GetFlag(FbxPropertyFlags::FbxPropertyFlagsType flag);

			/** Gets the inheritance type for the given flag. Similar to GetValueInheritType().
			* \param pFlag The flag to query
			* \return The inheritance type of the given flag
			*/
			FbxInheritType GetFlagInheritType( FbxPropertyFlags::FbxPropertyFlagsType flag );

			/** Sets the inheritance type for the given flag. Similar to SetValueInheritType().
			* \param pFlag The flag to set
			* \param pType The inheritance type to set
			* \return true on success, false otherwise.
			*/
			bool SetFlagInheritType(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flag, FbxInheritType type );

			/** Checks if the property's flag has been modified from its default value.
			* \param pFlag The flag to query
			* \return true if the value of this property has changed, false otherwise
			*/
			bool ModifiedFlag( FbxPropertyFlags::FbxPropertyFlagsType flag );
			//@}

			/**
			* \name Assignment and comparison operators
			*/
			//@{
			void CopyFrom(FbxProperty^ p);
			virtual bool Equals(System::Object^ obj) override
			{
				FbxProperty^ o = dynamic_cast<FbxProperty^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}
			//      static bool operator== (FbxProperty^ p1 ,int value)
			//{
			// return false;//*p1->pro == value;
			//}
			//      static bool operator!= (FbxProperty^ p1 ,int value)
			//{
			// return false;//*p1->pro != value;
			//}
			bool CompareValue(FbxProperty^ prop);
			//@}

			/** Copy value of a property.
			* \param pProp Property to get value from.
			* \return true if value has been copied, false if not.
			*/
			bool CopyValue(FbxProperty^ prop);

			/**
			* \name Value management.
			*/
			//@{
		public:			

			/** set value function
			* \param pValue Pointer to the new value
			* \param pValueType The data type of the new value
			* \param pCheckForValueEquality if true, the value is not set if it is equal to the default value.
			* \return true if it was succesfull and type were compatible.
			*/
			//bool Set(void const *pValue,EFbxType pValueType, bool pCheckForValueEquality);
			//inline bool Set(void const *pValue,EFbxType pValueType) { return Set( pValue, pValueType, true ); }

			/** get value function
			* \return true if it was succesfull and type were compatible.
			*/
			//bool Get(void *pValue,EFbxType pValueType) const;

			/** get and evaluate pulls on a value
			* \return true if it was succesfull and type were compatible.
			*/
			//bool Get(void *pValue,EFbxType pValueType,KFbxEvaluationInfo const *pEvaluateInfo);

			// usefull set and get functions
			//template <class T> inline bool  Set( T const &pValue )  { return Set( &pValue,FbxTypeOf(pValue), true ); }
			//template <class T> inline T     Get( T const *pFBX_TYPE) const { T lValue; Get( &lValue,FbxTypeOf(lValue) ); return lValue; }
			//template <class T> inline T     Get( T const *pFBX_TYPE,KFbxEvaluationInfo const *pEvaluateInfo) { T lValue; Get( &lValue,FbxTypeOf(lValue),pEvaluateInfo ); return lValue; }
			//template <class T> inline T     Get( KFbxEvaluationInfo const *pEvaluateInfo) { T lValue; Get( &lValue,FbxTypeOf(lValue),pEvaluateInfo ); return lValue; }
			//         /** get and evaluate pulls on a value
			//           * \return true if it was succesfull and type were compatible.
			//           */
			//         bool Get(void *pValue,EFbxType pValueType,KFbxEvaluationInfo *pEvaluateInfo) const;

			//         /** Query the inheritance type of the property.
			//           * Use this method to determine if this property's value is overriden from the default
			//           * value, or from the referenced object, if this object is a clone.
			//           * \return The inheritance type of the property.
			//           */
			property FbxInheritType ValueInheritType
			{
				FbxInheritType get();
			}

			/** Set the inheritance type of the property.
			* Use the method to explicitly override the default value of the property,
			* or the referenced object's property value, if this object is a clone.
			*
			* It can also be used to explicitly inherit the default value of the property,
			* or the referenced object's property value, if this object is a clone.
			*
			* \param pType The new inheritance type.
			* \return true on success, false otherwise.
			*/
			bool SetValueInheritType( FbxInheritType type );

			/** Checks if the property's value has been modified from its default value.
			* \return true if the value of this property has changed, false otherwise
			*/
			property bool Modified
			{
				bool get();
			}			

			/**
			* \name Property Limits.
			* Property limits are provided for convenience if some applications desire to
			* bound the range of possible values for a given type property. Note that these
			* limits are meaningless for the boolean type. It is the responsibility of the
			* calling application to implement the necessary instructions to limit the property.
			*/
			//@{
		public:
			/** Get the minimum limit value of the property.
			* \return Currently set minimum limit value.
			*/
			/** Set the minimum limit value of the property.
			* \param pMin Minimum value allowed.
			*/
			property double MinLimit
			{
				double get();
				void set(double value);
			}

			/** Returns whether a limit exists; calling GetMinLimit() when this returns
			* false will return in undefined behavior.
			* \return Whether or not a minimum limit has been set.
			*/
			property bool  HasMinLimit
			{
				bool get();
			}


			/** Returns whether a limit exists; calling GetMinLimit() when this returns
			* false will return in undefined behavior.
			* \return Whether or not a minimum limit has been set.
			*/
			property bool  HasMaxLimit
			{
				bool get();
			}

			/** Get the maximum limit value of the property.
			* \return Currently set maximum limit value.
			*/
			/** Set the maximum limit value of the property.
			* \param pMax Maximum value allowed.
			*/			         
			property double MaxLimit
			{
				double get();
				void set(double value);
			}

			/** Set the minimum and maximum limit value of the property.
			* \param pMin Minimum value allowed.
			* \param pMax Maximum value allowed.
			*/
			void SetLimits(double min, double max);
			//@}

			/**
			* \name Enum and property list
			*/
			//@{
		public:
			/** Add a string value at the end of the list.
			* \param pStringValue Value of the string to be added.
			* \return The index in the list where the string was added.
			* \remarks This function is only valid when the property type is eENUM.
			* Empty strings are not allowed.
			*/
			int AddEnumValue(System::String^ value);

			/** Insert a string value at the specified index.
			* \param pIndex Zero bound index.
			* \param pStringValue Value of the string for the specified index.
			* \remarks This function is only valid when the property type is eENUM.
			* pIndex must be in the range [0, ListValueGetCount()].
			* Empty strings are not allowed.
			*/
			void InsertEnumValue(int index, System::String^ value);

			/** Get the number of elements in the list.
			* \return The number of elements in the list.
			* \remarks This function will return -1 if the property type is not eENUM.
			*/
			property int EnumCount
			{
				int get();
			}

			/** Set a string value for the specified index.
			* \param pIndex Zero bound index.
			* \param pStringValue Value of the string for the specified index.
			* \remarks This function is only valid when the property type is eENUM.
			* The function will assign the specified string to the specified index.
			* A string value must exists at the specified index in order to be changed.
			* Empty strings are not allowed.
			*/
			void SetEnumValue(int index,System::String^ value);

			/** Remove the string value at the specified index.
			* \param pIndex of the string value to be removed.
			*/
			void RemoveEnumValue(int index);

			/** Get a string value for the specified index
			* \param pIndex Zero bound index.
			* \remarks This function is only valid when the property type is eENUM.
			*/
			System::String^ GetEnumValue(int index);
			//@}

			/**
			* \name Hierarchical properties
			*/
			//@{
			property bool IsRoot
			{
				bool get();
			}
			/*bool IsChildOf(FbxProperty^ parent)
			{
			return pro->IsChildOf(*parent->pro);
			}*/
			/*bool IsDescendentOf(FbxProperty^ ancestor)
			{
			return pro->IsDescendentOf(*ancestor->pro);
			}*/
			FbxProperty^ GetParent();
			bool SetParent(FbxProperty^ other );
			FbxProperty^ GetChild();
			FbxProperty^ GetSibling();

			/** Get the first property that is a descendent to this property
			* \return A valid KFbxProperty if the property was found, else
			*         an invalid KFbxProperty. See KFbxProperty::IsValid()
			*/
			FbxProperty^ GetFirstDescendent();
			/** Get the next property following pProperty that is a descendent to this property
			* \param pProperty The last found descendent.
			* \return A valid KFbxProperty if the property was found, else
			*         an invalid KFbxProperty. See KFbxProperty::IsValid()
			*/
			FbxProperty^ GetNextDescendent(FbxProperty^ p);

			/** Find a property using its name and its data type.
			* \param pCaseSensitive
			* \param pName The name of the property as a \c NULL terminated string.
			* \return A valid KFbxProperty if the property was found, else
			*         an invalid KFbxProperty. See KFbxProperty::IsValid()
			*/
			FbxProperty^ Find (System::String^ name,bool caseSensitive);
			FbxProperty^ Find (System::String^ name,FbxDataType^ dataType, bool caseSensitive);
			/** Fullname find
			* \param pCaseSensitive
			* \param pName The name of the property as a \c NULL terminated string.
			* \return A valid KFbxProperty if the property was found, else
			*         an invalid KFbxProperty. See KFbxProperty::IsValid()
			*/
			FbxProperty^ FindHierarchical (System::String^ name,bool caseSensitive);
			/*
			FbxProperty^  FindHierarchical (System::String^ name, FbxDataType^ dataType, bool caseSensitive )
			{
			char* n = new char(FbxString::NumCharToCreateString);
			FbxString::StringToChar(name,n);
			return gcnew FbxProperty(&pro->FindHierarchical(n,*dataType->type,caseSensitive));
			}*/

			//@}

			/**
			* \name Optimizations
			*/
			//@{
			void BeginCreateOrFindProperty();
			void EndCreateOrFindProperty();

			ref struct FbxPropertyNameCache : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxPropertyNameCache,KFbxProperty::KFbxPropertyNameCache);
				INATIVEPOINTER_DECLARE(FbxPropertyNameCache,KFbxProperty::KFbxPropertyNameCache);			
			public:
				FbxPropertyNameCache(FbxProperty^ prop);				
			};			         


			/**
			* \name Array Management
			*/
			//@{
			bool SetArraySize( int size, bool variableArray );
			int  GetArraySize();
			FbxProperty^ GetArrayItem(int index);
			property FbxProperty^ default[int]
			{
				FbxProperty^ get(int index);				
			}
			//@}

			/**
			* \name FCurve Management
			*/
			//@{
			/** Create a KFCurveNode on a take
			* \param pTakeName Name of the take to create the KFCurveNode on
			*/
			FbxCurveNode^ CreateKFCurveNode(String^ takeName);
			FbxCurveNode^ CreateKFCurveNode()
			{
				return CreateKFCurveNode(nullptr);
			}

			/** Get the KFCurveNode from a take
			* \param pTakeName Name of the take to get the KFCurveNode from
			* \param pCreateAsNeeded Create the KFCurveNode if not found.
			* \return Pointer to the KFCurveNode of the proprety on the given take.
			*/
			FbxCurveNode^ GetKFCurveNode(bool createAsNeeded, String^ takeName);
			FbxCurveNode^ GetKFCurveNode(bool createAsNeeded)
			{
				return GetKFCurveNode(createAsNeeded,nullptr);
			}
			FbxCurveNode^ GetKFCurveNode()
			{
				return GetKFCurveNode(false,nullptr);
			}

			/** Tries to get the KFCurve of the specified channel from the current take.
			* \param pChannel Name of the fcurve channel we are looking for.
			* \return Pointer to the FCurve if found, NULL in any other case.
			* \remark This method will fail if the KFCurveNode does not exist.
			* \remark If the pChannel is left NULL, this method retrieve the FCurve directly from the KFCurveNode
			* otherwise it will look recursively to find it.
			*/
			FbxCurve^ GetKFCurve(String^ channel);
			FbxCurve^ GetKFCurve()
			{
				return GetKFCurve(nullptr);
			}

			/** Tries to get the KFCurve of the specified channel from the given take.
			* \param pChannel Name of the fcurve channel we are looking for.
			* \param pTakeName Name of the take to get the KFCurve from.
			* \return Pointer to the FCurve if found, NULL in any other case.
			* \remark This method will fail if the KFCurveNode does not exist.
			* \remark If pTakeName is NULL, this function will look in the current take.
			* \remark If the pChannel is left NULL, this method retrieve the FCurve directly from the KFCurveNode
			* otherwise it will look recursively to find it.
			*/            
			FbxCurve^ GetKFCurve(String^ channel, String^ takeName);			
			//@}

			/**
			* \name Evaluation management
			*/
			//@{
			//bool    Evaluate(KFbxEvaluationInfo const *pEvaluationInfo);
			//@}




		public:
			// SrcObjects
			bool ConnectSrcObject (FbxObjectManaged^ obj,FbxConnectionType type);
			bool IsConnectedSrcObject (FbxObjectManaged^ obj);
			bool DisconnectSrcObject(FbxObjectManaged^ obj);

			bool DisconnectAllSrcObject();
			bool DisconnectAllSrcObject(FbxCriteria^ criteria);
			bool DisconnectAllSrcObject(FbxClassId^ classId);
			bool DisconnectAllSrcObject(FbxClassId^ classId, FbxCriteria^ criteria);

			property int SrcObjectCount
			{
				int get();
			}
			int GetSrcObjectCount(FbxCriteria^ criteria);
			int GetSrcObjectCount(FbxClassId^ classId);
			int GetSrcObjectCount(FbxClassId^ classId,FbxCriteria^ criteria);

			FbxObjectManaged^ GetSrcObject(int index);
			FbxObjectManaged^ GetSrcObject(FbxCriteria^ criteria,int index);
			FbxObjectManaged^ GetSrcObject(FbxClassId^ classId,int index);
			FbxObjectManaged^ GetSrcObject(FbxClassId^ classId, FbxCriteria^ criteria,int index);

			FbxObjectManaged^ FindSrcObject(System::String^ name,int startIndex);
			FbxObjectManaged^ FindSrcObject(FbxCriteria^ criteria,System::String^ name,int startIndex);
			FbxObjectManaged^ FindSrcObject(FbxClassId^ classId,System::String^ name,int startIndex);
			FbxObjectManaged^ FindSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex);

			////         template < class T > inline bool DisconnectAllSrcObject (T const *pFBX_TYPE){ return DisconnectAllSrcObject(T::ClassId);}
			////         template < class T > inline bool DisconnectAllSrcObject (T const *pFBX_TYPE,KFbxCriteria const &pCriteria)  { return DisconnectAllSrcObject(T::ClassId,pCriteria);  }
			////         template < class T > inline int  GetSrcObjectCount(T const *pFBX_TYPE) const{ return GetSrcObjectCount(T::ClassId); }
			////         template < class T > inline int  GetSrcObjectCount(T const *pFBX_TYPE,KFbxCriteria const &pCriteria) const { return GetSrcObjectCount(T::ClassId,pCriteria); }
			////         template < class T > inline T*   GetSrcObject(T const *pFBX_TYPE,int pIndex=0) const { return (T*)GetSrcObject(T::ClassId,pIndex); }
			////         template < class T > inline T*   GetSrcObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,int pIndex=0) const { return (T*)GetSrcObject(T::ClassId,pCriteria,pIndex); }
			////         template < class T > inline T*   FindSrcObject(T const *pFBX_TYPE,const char *pName,int pStartIndex=0) const { return (T*)FindSrcObject(T::ClassId,pName,pStartIndex); }
			////         template < class T > inline T*   FindSrcObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,const char *pName,int pStartIndex=0) const { return (T*)FindSrcObject(T::ClassId,pCriteria,pName,pStartIndex); }

			//         // DstObjects
			bool ConnectDstObject(FbxObjectManaged^ obj,FbxConnectionType type);
			bool IsConnectedDstObject(FbxObjectManaged^ obj);
			bool DisconnectDstObject(FbxObjectManaged^ obj);

			bool DisconnectAllDstObject();
			bool DisconnectAllDstObject(FbxCriteria^ criteria);
			bool DisconnectAllDstObject(FbxClassId^ classId);
			bool DisconnectAllDstObject(FbxClassId^ classId,FbxCriteria^ criteria);

			property int DstObjectCount
			{
				int get();
			}			
			int GetDstObjectCount(FbxCriteria^ criteria);
			int GetDstObjectCount(FbxClassId^ classId);
			int GetDstObjectCount(FbxClassId^ classId,FbxCriteria^ criteria);

			FbxObjectManaged^ GetDstObject(int index);
			FbxObjectManaged^ GetDstObject(FbxCriteria^ criteria,int index);
			FbxObjectManaged^ GetDstObject(FbxClassId^ classId,int index);
			FbxObjectManaged^ GetDstObject(FbxClassId^ classId, FbxCriteria^ criteria,int index);

			FbxObjectManaged^ FindDstObject(System::String^ name,int startIndex);
			FbxObjectManaged^ FindDstObject(FbxCriteria^ criteria,System::String^ name,int startIndex);
			FbxObjectManaged^ FindDstObject(FbxClassId^ classId,System::String^ name,int startIndex);
			FbxObjectManaged^ FindDstObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex);

			////         template < class T > inline bool DisconnectAllDstObject (T const *pFBX_TYPE){ return DisconnectAllDstObject(T::ClassId);    }
			////         template < class T > inline bool DisconnectAllDstObject (T const *pFBX_TYPE,KFbxCriteria const &pCriteria)  { return DisconnectAllDstObject(T::ClassId,pCriteria);  }
			////         template < class T > inline int  GetDstObjectCount(T const *pFBX_TYPE) const { return GetDstObjectCount(T::ClassId); }
			////         template < class T > inline int  GetDstObjectCount(T const *pFBX_TYPE,KFbxCriteria const &pCriteria) const { return GetDstObjectCount(T::ClassId,pCriteria); }
			////         template < class T > inline T*   GetDstObject(T const *pFBX_TYPE,int pIndex=0) const { return (T*)GetDstObject(T::ClassId,pIndex); }
			////         template < class T > inline T*   GetDstObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,int pIndex=0) const { return (T*)GetDstObject(T::ClassId,pCriteria,pIndex); }
			////         template < class T > inline T*   FindDstObject(T const *pFBX_TYPE,const char *pName,int pStartIndex=0) const { return (T*)FindDstObject(T::ClassId,pName,pStartIndex); }
			////         template < class T > inline T*   FindDstObject(T const *pFBX_TYPE,KFbxCriteria const &pCriteria,const char *pName,int pStartIndex=0) const { return (T*)FindDstObject(T::ClassId,pCriteria,pName,pStartIndex); }
			////     //@}

			/**
			* \name General Property Connection and Relationship Management
			*/
			//@{
		public:
			// Properties
			bool ConnectSrcProperty(FbxProperty^ p);
			bool IsConnectedSrcProperty (FbxProperty^ p);
			bool DisconnectSrcProperty (FbxProperty^p);
			property int SrcPropertyCount
			{
				int get();
			}
			FbxProperty^ GetSrcProperty (int index);
			FbxProperty^ FindSrcProperty(System::String^ name,int startIndex);

			bool ConnectDstProperty(FbxProperty^ p);
			bool IsConnectedDstProperty  (FbxProperty^ p);
			bool DisconnectDstProperty(FbxProperty^ p);
			property int DstPropertyCount
			{
				int get();
			}

			FbxProperty^ GetDstProperty (int index);
			FbxProperty^ FindDstProperty(System::String^ name,int startIndex);

			void ClearConnectCache();

			//@}
		protected:
			static System::String^ hierarchicalSeparator;
		public:
			/*static property System::String^ HierarchicalSeparator
			{
			System::String^ get();
			}*/


			enum class UserPropertyType
			{
				Unidentified = KFbxProperty::eUNIDENTIFIED,
				Bool= KFbxProperty::eBOOL,
				Real = KFbxProperty::eREAL,
				Color = KFbxProperty::eCOLOR,
				Integer = KFbxProperty::eINTEGER,
				Vector = KFbxProperty::eVECTOR,
				List = KFbxProperty::eLIST,
				Matrix = KFbxProperty::eMATRIX
			};
			// Deprecated function calls

			//     K_DEPRECATED static const char* GetPropertyTypeName(EUserPropertyType pType);
			//     K_DEPRECATED const char *       GetPropertyTypeName();
			//     K_DEPRECATED EUserPropertyType  GetPropertyType();

			//     K_DEPRECATED void SetDefaultValue(bool pValue);
			//     K_DEPRECATED void SetDefaultValue(double pValue);
			//     K_DEPRECATED void SetDefaultValue(KFbxColor& pValue);
			//     K_DEPRECATED void SetDefaultValue(int pValue);
			//     K_DEPRECATED void SetDefaultValue(double pValue1, double pValue2, double pValue3);
			//     K_DEPRECATED void GetDefaultValue(bool& pValue);
			//     K_DEPRECATED void GetDefaultValue(double& pValue);
			//     K_DEPRECATED void GetDefaultValue(KFbxColor& pValue);
			//     K_DEPRECATED void GetDefaultValue(int& pValue);
			//     K_DEPRECATED void GetDefaultValue(double& pValue1, double& pValue2, double& pValue3);									

			// For use with deprecated type functions
			static FbxDataType^ UserPropertyTypeToDataType(FbxProperty::UserPropertyType type);
			static FbxProperty::UserPropertyType DataTypeToUserPropertyType(FbxDataType^ dataType);

			bool GetValueAsBool();
			int GetValueAsInt();
			double GetValueAsDouble();
			FbxColor^ GetValueAsColor();			
			FbxDouble2^ GetValueAsDouble2();
			FbxDouble3^ GetValueAsDouble3();
			float GetValueAsFloat();
			String^ GetValueAsString();

			void Set(bool value);
			void Set(int value);
			void Set(double value);
			void Set(FbxColor^ value);
			void Set(FbxDouble2^ value);
			void Set(FbxDouble3^ value);
			void Set(FbxVector4^ value);
			void Set(FbxVector2^ value);
			void Set(float value);
			void Set(String^ value);
			
		};		
		public enum class FbxConnectEventType {
			FbxConnectRequest,
			FbxConnect,
			FbxConnected,
			FbxDisconnectRequest,
			FbxDisconnect,
			FbxDisconnected
		};

		public enum class FbxConnectEventDirection {
			ConnectEventSrc,
			ConnectEventDst
		};

		/** Class the handles Connection events.
		* \nosubgrouping
		*/
		public ref class FbxConnectEvent : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxConnectEvent,KFbxConnectEvent);
			INATIVEPOINTER_DECLARE(FbxConnectEvent,KFbxConnectEvent)		
		protected:
			FbxProperty^ _Source;
			FbxProperty^ _Destination;
			/***
			* \name Constructor and Destructors.
			*/
			//@{
		public:
			FbxConnectEvent(FbxConnectEventType type,
				FbxConnectEventDirection dir, FbxProperty^  src, FbxProperty^ dst);			
			//@}

			/***
			* \name Data Access.
			*/
			//@{
		public:
			property FbxConnectEventType Type
			{
				FbxConnectEventType get();
			}
			property FbxConnectEventDirection Direction
			{
				FbxConnectEventDirection get();
			}
			property FbxProperty^ Source
			{
				FbxProperty^ get();
			}			    
			property FbxProperty^ Destination
			{
				FbxProperty^ get();
			}

			//template < class T > inline T*  GetSrcIfObject(T const *pFBX_TYPE) const    { return mSrc->IsRoot() ? KFbxCast<T>(mSrc->GetFbxObject()) : (T*)0; }
			//template < class T > inline T*  GetDstIfObject(T const *pFBX_TYPE) const    { return mDst->IsRoot() ? KFbxCast<T>(mDst->GetFbxObject()) : (T*)0; }			
		};
	}
}