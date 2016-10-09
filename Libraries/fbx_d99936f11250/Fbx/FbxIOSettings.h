#pragma once
#include "stdafx.h"
#include "FbxObject.h"
#include "FbxTime.h"
#include "FbxPropertyDef.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxTime;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxProperty;
		ref class FbxDataType;
		ref class FbxStringManaged;		
		namespace IO
		{
			// class to handle old StreamOptions information
			public ref class FbxSoInfoManaged : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxSoInfoManaged,KsoInfo);
				INATIVEPOINTER_DECLARE(FbxSoInfoManaged,KsoInfo);			
			public: 
				enum class ImpExp
				{
					Import = 0,
					Export
				};
			public: 
				DEFAULT_CONSTRUCTOR(FbxSoInfoManaged,KsoInfo);				

				//! Set values to default 
				void Reset(ImpExp impExp);

				//! If pTimeMode is set to KTime::eDEFAULT_MODE, pCustomFrameRate is used instead.
				void SetTimeMode(FbxTime::TimeMode timeMode, double customFrameRate);

				//! A time mode set to KTime::eDEFAULT_MODE means a custom frame rate is set.
				property FbxTime::TimeMode TimeMode
				{
					FbxTime::TimeMode get();
				}

				//! Get frame period associated with time mode or a custom frame period if time mode is set to KTime::eDEFAULT_MODE.
				property FbxTime^ FramePeriod
				{
					FbxTime^ get();
				}
			protected:
				FbxObjectManaged^ ASF;
			public:

				void SetASFScene(FbxObjectManaged^ ASFScene, bool ASFSceneOwned);
				FbxObjectManaged^ GetASFScene();
			};

			public enum class FbxUILanguage
			{
				ENU = 0,			        // MAX 409 English - United States
				DEU,           	        // MAX 407 German - Germany
				FRA,            	        // MAX 40c French - France
				JPN,           	        // MAX 411 Japanese - Japan
				KOR,            	        // MAX 412 Korean(Extended Wansung) - Korea 
				CHS,	                    // MAX 804 Chinese - PRC
				UILanguageCount
			};

			// this class is located in the UserDataPtr of each KFbxIOSettings properties
			//public ref class PropInfo
			//{
			//internal:
			//	fbxsdk_200901::PropInfo*
			//public:
			//	PropInfo();
			//	~PropInfo();

			//	void            * UIWidget;            // UI widget for showing the property
			//	void            * cbValueChanged;      // call back when value changed
			//	void            * cbDirty;             // call back when value changed
			//	FbxStringList       labels;              // list of labels in many languages
			//};


			/** KFbxIOSettings
			*	Class used as container for IO settings.
			*   All settings are organised in a propertie hierarchie.
			*   A node of settings may contain a bunch of information related to an operation (ex: import / export)
			*   it may also contain informations related to a user dialog offering user options
			*/
			public ref class FbxIOSettingsManaged : FbxObjectManaged
			{
			internal:
				REF_DECLARE(FbxEmitter,FbxIOSettings);
			public:				
				FbxIOSettingsManaged(FbxIOSettings* instance) : FbxObjectManaged(instance)
				{
					_Free = false;
				}
			protected:
				static FbxIOSettingsManaged^ refIO;
				FbxSoInfoManaged^ impInfo;			
				FbxSoInfoManaged^ expInfo;

				virtual void CollectManagedMemory() override;
			public:				

				FBXOBJECT_DECLARE(FbxIOSettingsManaged);				

				//! Global allocator
				static void AllocateIOSettings(FbxSdkManagerManaged^ manager);

				static bool IsIOSettingsAllocated();

				//! Release memory
				void FreeIOSettings();

				//! Ref accessor
				static property FbxIOSettingsManaged^ IOSettingsRef
				{
					FbxIOSettingsManaged^ get();
				}

				//! Empty the settings hierarchies
				//virtual void Clear() override;

				//! Add a property group under the root prop
				FbxProperty^ AddPropertyGroup(String^ name, FbxDataType^ dataType, String^ label);
				FbxProperty^ AddPropertyGroup(String^ name);

				//! Add a property group under another parent prop
				FbxProperty^ AddPropertyGroup(	FbxProperty^ parentProperty, 
					String^ name,
					FbxDataType^ dataType, 
					String^ label,
					bool visible,
					bool savable,
					bool enabled);
				FbxProperty^ AddPropertyGroup(	FbxProperty^ parentProperty, 
					String^ name);

				//! Add a property under another parent prop with a value to set
				/*FbxProperty^ AddProperty(FbxProperty^ parentProperty, 
				String^ name, 
				FbxDataType^ dataType , 
				String^ label,
				void const*            pValue          = NULL,
				EFbxType               pValueType      = eUNIDENTIFIED,
				bool					pVisible		= true,
				bool					pSavable		= true,
				bool					pEnabled		= true
				);*/

				//! Add a property under another parent prop with a value to set and a min max values
				/*public: KFbxProperty AddPropertyMinMax(    KFbxProperty const     &pParentProperty, 
				char const*            pName, 
				KFbxDataType const     &pDataType      = KFbxDataType(), 
				char const*            pLabel          = "",
				void const*            pValue          = NULL,
				double const*			pMinValue		= NULL,
				double const*			pMaxValue		= NULL,
				EFbxType               pValueType      = eUNIDENTIFIED,									 
				bool					pVisible		= true,
				bool					pSavable		= true,
				bool					pEnabled		= true
				);*/


				//! get a prop by description ex: "Export|Animation|Bake"
				FbxProperty^ GetProperty(String^ name);

				//! get prop by description from a parent property
				FbxProperty^ GetProperty(FbxProperty^ parentProperty,String^ name);

				//! get/set a bool prop by prop path
				bool GetBoolProp(String^ name, bool defValue);
				void SetBoolProp(String^ name, bool value);

				//! get/set a double prop by prop path
				double GetDoubleProp(String^ name, double defValue);
				void   SetDoubleProp(String^ name, double value);

				//! get/set a int prop by prop path
				int    GetIntProp(String^ name, int defValue);
				void   SetIntProp(String^ name, int value);

				//! get/set a KTime prop by prop path
				FbxTime^  GetFbxTimeProp(String^ name, FbxTime^ defValue);
				void   SetFbxTimeProp(String^ name, FbxTime^ value);

				//! get/set an enum prop by prop path
				FbxStringManaged^ GetEnumProp(String^ name, FbxStringManaged^ defValue);
				int     GetEnumProp(String^ name, int defValue);
				int     GetEnumIndex(String^ name, FbxStringManaged^ value);

				void SetEnumProp(String^ name, FbxStringManaged^ value);
				void SetEnumProp(String^ name, int value);

				void RemoveEnumPropValue(String^ name, FbxStringManaged^ value);
				void EmptyEnumProp(String^ name);

				// set a specific flag value on a specific property
				bool SetFlag(String^ name, Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType propFlag, bool value);

				//! get/set a string prop by prop path
				FbxStringManaged^ GetStringProp(String^ name, FbxStringManaged^ defValue);
				void SetStringProp(String^ name, FbxStringManaged^ value);


				//! Load the settings hierarchie from an XML file
				virtual bool ReadXMLFile(String^ path);

				//! Write the settings hierarchie to an XML file
				virtual bool WriteXMLFile(String^ path);

				bool WriteXmlPropToFile(String^ fullPath, String^ propPath);

				//! Write a property branch to the registry - can be used to save a "Preset"
				bool WriteXmlPropToRegistry(String^ regKeyName, String^ regValueName, String^ propPath, String^ regPath);

				//! Read a property branch from the registry - can be used to read a "Preset"
				bool ReadXmlPropFromRegistry(String^ regKeyName, String^ regValueName, String^ regPath);

				//! Read an XML file from MyDocument dir
				bool ReadXmlPropFromMyDocument(String^ subDir, String^ filename);

				//! Write property branch to an XML file in MyDocument dir
				bool WriteXmlPropToMyDocument(String^ subDir, String^ filename, String^ propPath);

				static property String^ UserMyDocumentDir
				{
					String^ get();
				}
				enum class LoadMode
				{
					Merge			= FbxIOSettings::eMerge,
					ExclusiveMerge	= FbxIOSettings::eExclusiveMerge,
					Create			= FbxIOSettings::eCreate
				};
				enum class QuaternionMode
				{
					AsQuaternion	=	FbxIOSettings::eAsQuaternion,
					AsEuler			=	FbxIOSettings::eAsEuler,
					Resample		=	FbxIOSettings::eResample
				};
				enum class ObjectDerivation
				{
					ByLayer		=	FbxIOSettings::eByLayer, 
					ByEntity	=	FbxIOSettings::eByEntity, 
					ByBlock		=	FbxIOSettings::eByBlock
				}; 

				enum class SysUnits
				{
					User		=	FbxIOSettings::eUnitsUser,
					Inches		=	FbxIOSettings::eUnitsInches,
					Feet		=	FbxIOSettings::eUnitsFeet,
					Yards		=	FbxIOSettings::eUnitYards,
					Miles		=	FbxIOSettings::eUnitsMiles,
					Millimeters	=	FbxIOSettings::eUnitsMillimeters,
					Centimeters	=	FbxIOSettings::eUnitsCentimeters,
					Meters		=	FbxIOSettings::eUnitsMeters,
					Kilometers	=	FbxIOSettings::eUnitsKilometers
				};

				enum class SysFrameRate
				{
					User			=	FbxIOSettings::eFrameRateUser,
					Hour			=	FbxIOSettings::eFrameRateHours,
					Minutes			=	FbxIOSettings::eFrameRateMinutes,
					Seconds			=	FbxIOSettings::eFrameRateSeconds,
					Milliseconds	=	FbxIOSettings::eFrameRateMilliseconds,
					Games_15		=	FbxIOSettings::eFrameRateGames15,
					Film_24			=	FbxIOSettings::eFrameRateFilm24,
					Pal_25			=	FbxIOSettings::eFrameRatePAL25,
					Ntsc_30			=	FbxIOSettings::eFrameRateNTSC30,
					Showscan_48		=	FbxIOSettings::eFrameRateShowScan48,
					Palfield_50		=	FbxIOSettings::eFrameRatePALField50,
					Ntscfield_60	=	FbxIOSettings::eFrameRateNTSCField60
				};

				// Max
				enum class EnveloppeSystem
				{
					SkinModifier			=	FbxIOSettings::eSkinModifier,
					Physique				=	FbxIOSettings::ePhysic,
					Bonespro				=	FbxIOSettings::eBonePro,
					EnveloppeSystemCount	=	FbxIOSettings::eEnveloppeSystemCount
				};

				// Max
				enum class GeometryType
				{
					Triangle			=	FbxIOSettings::eTriangle,
					SimplifiedPoly		=	FbxIOSettings::eSimplifiedPoly,
					Poly				=	FbxIOSettings::ePolygon,
					Nurb				=	FbxIOSettings::eNurbs,
					Patch				=	FbxIOSettings::ePatch,
					GeometryTypeCount	=	FbxIOSettings::eGeometryTypeCount
				};

				REF_PROPERTY_GET_DECLARE(FbxSoInfoManaged,ImpInfo);
				REF_PROPERTY_GET_DECLARE(FbxSoInfoManaged,ExpInfo);				

				property FbxUILanguage UILanguage
				{
					FbxUILanguage get();
					void set(FbxUILanguage value);
				}				

				//FbxPropInfo^ GetPropInfo(FbxProperty^ prop);
				String^ GetLanguageLabel(FbxProperty^ prop);
				void SetLanguageLabel(FbxProperty^ prop, String^ label);
				FbxUILanguage GetMaxRuntimeLanguage(String^ regLocation);

				bool IsEnumExist(FbxProperty^ prop, String^ enumString);
				int GetEnumIndex(FbxProperty^ prop, String^ enumString, bool noCase);

				CLONE_DECLARE();
			};
		}
	}
}