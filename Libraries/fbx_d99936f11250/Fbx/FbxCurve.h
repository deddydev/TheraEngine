#pragma once
#include "stdafx.h"
#include "FbxKEventBase.h"
#include "kfcurve/kfcurve.h"

#define CONST_STRING_DEFINE(PropName,Value)\
	public: static property System::String^ FBXCURVE_##PropName { System::String^ get(){ return Value;}}


namespace Skill
{
	namespace FbxSDK
	{

		ref class FbxTime;
		ref class FbxKEventBase;
		//! KFCurveKey data indices for cubic interpolation tangent information.
		public enum class FbxCurveDataIndex
		{
			// User and Break tangent mode (data are doubles).
			CurvekeyRightSlope			= 0, 
			CurvekeyNextLeftSlope		= 1, 

			// User and Break tangent break mode (data are kInt16 thken from mwight and converted to doubles).
			CurvekeyWeights				= 2, 
			CurvekeyRightWeight			= 2, 
			CurvekeyNextLeftWeight		= 3, 

			// Velocity mode
			CurvekeyVelocity			= 4,
			CurvekeyRightVelocity		= 4,
			CurvekeyNextLeftVelocity	= 5, 

			// TCB tangent mode (data are floats).
			CurvekeyTcbTension			= 0, 
			CurvekeyTcbContinuity		= 1, 
			CurvekeyTcbBias				= 2,

			CurvekeyRightAuto			= 0,
			CurvekeyNextLeftAuto		= 1
		};
		// Curve event class.
		public ref class FbxCurveEvent : FbxKEventBase
		{
			REF_DECLARE(FbxKEventBase,KFCurveEvent);
		internal:
			FbxCurveEvent(KFCurveEvent* instance) : FbxKEventBase(instance)
			{
				_Free = false;
			}
		public:			
			// Curve event type, the enum stated above allow composition of type (bitfield). 
			// Stored in mType

			// Start key index.
			property int KeyIndexStart
			{
				int get();
				void set(int value);
			}

			//	Stop key index.				
			property int KeyIndexStop
			{
				int get();
				void set(int value);
			}

			// Count of events.				
			property int EventCount
			{
				int get();
				void set(int value);
			}

			// Clear curve event.
			void Clear ();

			// Add a curve event of type pWhat to a curve event object.
			void Add (int what, int index);
		};

		/** Defines a tangent derivative and weight
		*	\remarks Implementation was made for performance.
		* \nosubgrouping
		*/
		public ref class FbxCurveTangentInfo : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCurveTangentInfo,KFCurveTangeantInfo);
			INATIVEPOINTER_DECLARE(FbxCurveTangentInfo,KFCurveTangeantInfo);		
		internal:
			FbxCurveTangentInfo(KFCurveTangeantInfo info)
			{
				_SetPointer(new KFCurveTangeantInfo(),true);
				*_FbxCurveTangentInfo = info;
			}
		public:
			DEFAULT_CONSTRUCTOR(FbxCurveTangentInfo,KFCurveTangeantInfo);
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Derivative);
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Weight);
			VALUE_PROPERTY_GETSET_DECLARE(bool,Weighted);
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Velocity);
			VALUE_PROPERTY_GETSET_DECLARE(bool,HasVelocity);				
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Auto);				
		};



		public ref class FbxCurveKey : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCurveKey,KFCurveKey);
			INATIVEPOINTER_DECLARE(FbxCurveKey,KFCurveKey);		
		internal:
			FbxCurveKey(KFCurveKey key)
			{
				_SetPointer(new KFCurveKey(),true);
				*_FbxCurveKey = key;
			}
		public:
			DEFAULT_CONSTRUCTOR(FbxCurveKey,KFCurveKey);

			//! Key interpolation type.

			enum class KeyInterpolation : unsigned int
			{ 
				//! Constant value until next key.
				Constant  = 0x00000002,
				//! Linear progression to next key.
				Linear	  = 0x00000004,	
				//! Cubic progression to next key.
				Cubic = 0x00000008,		
				All  =	KFCURVE_INTERPOLATION_CONSTANT|KFCURVE_INTERPOLATION_LINEAR|KFCURVE_INTERPOLATION_CUBIC,
				Count = 3
			};

			//! Key constant mode.

			enum class KeyConstantMode : unsigned int
			{
				STANDARD		  =	0x00000000,
				NEXT			  =	0x00000100,
				ALL			  =	KFCURVE_CONSTANT_STANDARD | KFCURVE_CONSTANT_NEXT,
				COUNT		= 2	
			};

			//! Key tangent mode for cubic interpolation.

			enum class KeyTangentMode : unsigned int
			{
				TangentAuto	=	0x00000100, 	//! Spline cardinal.
				TangentTcb		=	0x00000200,		//! Spline TCB.
				TangentUser	=	0x00000400, 	//! Slope at the left equal to slope at the right.
				GenericBreak	=	0x00000800, 	//! Independent left and right slopes.
				GenericClamp	=	0x00001000, 	//! Auto key should be flat if next or prev keys have same value
				TangentBreak	= KFCURVE_TANGEANT_USER|KFCURVE_GENERIC_BREAK,
				TangentAutoBreak  = KFCURVE_TANGEANT_AUTO|KFCURVE_GENERIC_BREAK,
				TangentAll		   = KFCURVE_TANGEANT_AUTO|KFCURVE_TANGEANT_TCB|KFCURVE_TANGEANT_USER|KFCURVE_GENERIC_BREAK|KFCURVE_GENERIC_CLAMP,
				TangentTypeMask   = KFCURVE_TANGEANT_AUTO|KFCURVE_TANGEANT_TCB|KFCURVE_TANGEANT_USER|KFCURVE_TANGEANT_BREAK, // Break is part of the modes for historic reasons, should be part of overrides
				TangentOverridesMask  = KFCURVE_GENERIC_CLAMP	
			};

			//! Selection mode.	
			enum class KeySelectionMode : unsigned int
			{
				POINT =	0x00010000,
				LEFT  =	0x00020000, 
				RIGHT =	0x00040000, 
				ALL	  =	KFCURVE_SELECT_POINT|KFCURVE_SELECT_LEFT|KFCURVE_SELECT_RIGHT
			};

			//! Manipulation flag
			enum class KeyManipulationFlag : unsigned int
			{
				MarkedForManip = 0x00080000,
				MarkedAll       = MarkedForManip,
			};

			//! Tangent visibility.
			enum class KeyTangentVisibility : unsigned int
			{
				ShowNone		  = 0x00000000, 
				ShowLeft		  = 0x00100000, 
				ShowRight		  = 0x00200000, 
				ShowBoth		  = KFCURVE_TANGEANT_SHOW_LEFT|KFCURVE_TANGEANT_SHOW_RIGHT	
			};

			//! Continuity flag
			enum class KeyContinuityFlag : unsigned int
			{
				Continuity  	  = 0x00000000,
				ContinuityFlat    = 0x00100000,
				ContinuityBreak   = 0x00200000,
				ContinuityInsert  = 0x00400000   // Used to prevent the curve shape from changing when inserting a key
			};

			//! Weighted mode.	
			enum class KeyTangentWeightMode : unsigned int
			{
				None			  =	0x00000000,
				Right			  =	0x01000000,
				NextLeft		  =	0x02000000,
				All			  =	KFCURVE_WEIGHTED_RIGHT|KFCURVE_WEIGHTED_NEXT_LEFT
			};

			// !Velocity mode	
			enum class KeyTangentVelocityMode : unsigned int
			{
				None			  = 0x00000000,
				Right			  = 0x10000000,
				NextLeft		  = 0x20000000,
				All			  = KFCURVE_VELOCITY_RIGHT | KFCURVE_VELOCITY_NEXT_LEFT
			};

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			static const float FBXCURVE_WEIGHT_DIVIDER = 9999;       // precise enough and can be divided by 3 without error
			static const float FBXCURVE_DEFAULT_WEIGHT = ((kFCurveDouble)(1.0/3.0));
			static const float FBXCURVE_MIN_WEIGHT = ((kFCurveDouble)(1.0/KFCURVE_WEIGHT_DIVIDER));
			static const float FBXCURVE_MAX_WEIGHT = ((kFCurveDouble)0.99);
			static const float FBXCURVE_DEFAULT_VELOCITY = 0.0;

#endif // DOXYGEN_SHOULD_SKIP_THIS

			//! Extrapolation mode for function curve extremities.
			enum class KeyExtrapolationMode : unsigned int
			{
				Const				= 1, 
				Repetition		= 2, 
				MirrorRepetition	= 3,
				KeepSlope		= 4,
			};

			static const int KFCURVE_BEZIER	= 0; 
			static const int KFCURVE_SAMPLE	= 1; 
			static const int KFCURVE_ISO	= 2;				

			enum class  CurveEvent : unsigned int
			{
				None		=0, // default event value
				Candidate	=1 << 0, // curve value (not candidate) changed
				Unused1    =1 << 1,
				Unused2    =1 << 2,
				Unused3    =1 << 3,
				Key		=1 << 4, // key changed (add, removed, edited); see bits 11-15 for precisions
				Deprecated5 =1 << 5,
				Unused6    =1 << 6,
				Unused7    =1 << 7,
				Selection	=1 << 8, // key selection changed
				Destroy	=1 << 9, // fcurve destruction
				Deprecated10 =1 << 10,
				Keyadd     =1 << 11,
				Keyremove  =1 << 12,
				Editvalue  =1 << 13,
				Edittime   =1 << 14,
				Editother  =1 << 15				
			};

			/** Set a key.
			*	Use SetTCB() to set a key with cubic interpolation and TCB tangent type.
			*	\param pTime			Key time.
			*	\param pValue			Key value.
			*	\param pInterpolation	Key interpolation type.	Interpolation types are: 
			*							KFCURVE_INTERPOLATION_CONSTANT, 
			*							KFCURVE_INTERPOLATION_LINEAR,
			*							KFCURVE_INTERPOLATION_CUBIC
			*	\param pTangentMode		Key tangent mode (meaningful for cubic 
			*							interpolation only). Tangent modes are: 
			*							KFCURVE_TANGEANT_AUTO,
			*							KFCURVE_TANGEANT_USER,
			*							KFCURVE_TANGEANT_BREAK
			*	\param pData0			Right slope.
			*	\param pData1			Next left slope.
			*	\param pTangentWeightMode	Weight mode if used
			*								KFCURVE_WEIGHTED_NONE
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*	\param pWeight0				Right slope weight.
			*	\param pWeight1				Next left slope weight.
			*	\param pVelocity0			Right velocity.
			*	\param pVelocity1			Next left velocity.
			*/
			void Set(FbxTime^ time,kFCurveDouble value,FbxCurveKey::KeyInterpolation interpolation, 
				FbxCurveKey::KeyTangentMode tangentMode, 
				kFCurveDouble data0,kFCurveDouble data1,
				FbxCurveKey::KeyTangentWeightMode tangentWeightMode, 
				kFCurveDouble weight0,kFCurveDouble weight1,
				kFCurveDouble velocity0,kFCurveDouble velocity1
				);

			/**	Set a key with cubic interpolation, TCB tangent mode.
			*	\param pTime	Key time.
			*	\param pValue	Key value.
			*	\param pData0	Tension.
			*	\param pData1	Continuity.
			*	\param pData2	Bias.
			*/
			void SetTCB(FbxTime^ time, kFCurveDouble value,float data0, float data1,float data2);

			/** Key assignment.
			*	\param pSource	Source key to be copied.
			*/
			void Set(FbxCurveKey^ source);

			/** Get key interpolation type.
			*	Interpolation types are: KFCURVE_INTERPOLATION_CONSTANT, 
			*							 KFCURVE_INTERPOLATION_LINEAR,
			*							 KFCURVE_INTERPOLATION_CUBIC
			*/
			/** Set key interpolation type.
			*	\param pInterpolation Key interpolation type.
			*	Interpolation types are: KFCURVE_INTERPOLATION_CONSTANT, 
			*							 KFCURVE_INTERPOLATION_LINEAR,
			*							 KFCURVE_INTERPOLATION_CUBIC
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyInterpolation,Interpolation);

			/** Get key constant mode.
			*	Warning: This method is meaningful for constant interpolation only.
			*			 Using this method for non constant interpolated key will return unpredicted value.
			* Constant modes are:		KFCURVE_CONSTANT_STANDARD
			*							KFCURVE_CONSTANT_NEXT
			*	\return Key constant mode.
			*/
			/** Set key constant mode.
			*	Warning: This method is meaningful for constant interpolation only.
			*	\param pMode Key consant mode.
			*	Constant modes are:		KFCURVE_CONSTANT_STANDARD
			*							KFCURVE_CONSTANT_NEXT
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyConstantMode,ConstantMode);

			KeyTangentMode GetTangentMode( bool includeOverrides);
			/** Get key tangent mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*			 Using this method for non cubic interpolated key will return unpredicted value.
			*	Tangent modes are: KFCURVE_TANGEANT_AUTO,
			*					   KFCURVE_TANGEANT_AUTO_BREAK
			*					   KFCURVE_TANGEANT_TCB,
			*					   KFCURVE_TANGEANT_USER,
			*					   KFCURVE_TANGEANT_BREAK
			*	\return Key tangent mode.
			*/
			/** Set key tangent mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\param pTangent Key tangent mode.
			*	Tangent modes are: KFCURVE_TANGEANT_AUTO,
			*					   KFCURVE_TANGEANT_AUTO_BREAK
			*					   KFCURVE_TANGEANT_TCB,
			*					   KFCURVE_TANGEANT_USER,
			* 				   KFCURVE_TANGEANT_BREAK
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyTangentMode ,TangentMode);

			/** Get key tangent weight mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	Tangent weight modes are:	KFCURVE_WEIGHTED_NONE,
			*								KFCURVE_WEIGHTED_RIGHT,
			*								KFCURVE_WEIGHTED_NEXT_LEFT,
			*								KFCURVE_WEIGHTED_ALL
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyTangentWeightMode,TangentWeightMode);			

			/** Get key tangent velocity mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	Tangent weight modes are:	KFCURVE_VELOCITY_NONE,
			*								KFCURVE_VELOCITY_RIGHT,
			*								KFCURVE_VELOCITY_NEXT_LEFT,
			*								KFCURVE_VELOCITY_ALL
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyTangentVelocityMode,TangentVelocityMode);											

			/** Set key tangent weight mode as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\param pTangentWeightMode	Weight mode
			*								KFCURVE_WEIGHTED_NONE
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*	\param pMask				Used to select the affected tangents
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*/

			void SetTangentWeightMode(KeyTangentWeightMode tangentWeightMode, KeyTangentWeightMode mask);			

			/** Set key tangent velocity mode as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\param pTangentVelocityMode	Weight mode
			*								KFCURVE_VELOCITY_NONE
			*								KFCURVE_VELOCITY_RIGHT
			*								KFCURVE_VELOCITY_NEXT_LEFT
			*								KFCURVE_VELOCITY_ALL
			*	\param pMask				Used to select the affected tangents
			*								KFCURVE_VELOCITY_RIGHT
			*								KFCURVE_VELOCITY_NEXT_LEFT
			*								KFCURVE_VELOCITY_ALL
			*/

			void SetTangentVelocityMode(KeyTangentVelocityMode tangentVelocityMode, KeyTangentVelocityMode mask);			


			/** Get key data as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: Using this method for other than cubic interpolated 
			*			 key (linear, constant) will return unpredicted values.
			*	Warning: Slope data is inconsistent for automatic tangent mode.
			*			 Use KFCurve::EvaluateLeftDerivative() and 
			*			 KFCurve::EvaluateRightDerivative() to find
			*			 slope values.
			*	Warning: Using this method for TCB tangent mode key will return 
			*			 unpredicted values. Use KFCurve::GetDataFloat() instead.
			*	\param pIndex Data index, either	KFCURVEKEY_RIGHT_SLOPE,
			*										KFCURVEKEY_NEXT_LEFT_SLOPE.
			*										KFCURVEKEY_NEXT_RIGHT_WEIGHT.
			*										KFCURVEKEY_NEXT_LEFT_WEIGHT
			*/
			kFCurveDouble GetDataDouble(FbxCurveDataIndex index);

			/**	Set data as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: Using this method for other than cubic interpolated 
			*			 key (linear, constant) is irrelevant.
			*	Warning: Slope data is inconsistent for automatic tangent mode.
			*			 Therefore, it is irrelevant to use this method on automatic 
			*			 tangent mode keys.
			*	Warning: Using this method for a TCB tangent mode key will result
			*			 in unpredictable curve behavior for this key. Use KFCurve::SetDataFloat() 
			*			 instead.
			*	\param pIndex Data index, either	KFCURVEKEY_RIGHT_SLOPE,
			*										KFCURVEKEY_NEXT_LEFT_SLOPE.
			*										KFCURVEKEY_NEXT_RIGHT_WEIGHT.
			*										KFCURVEKEY_NEXT_LEFT_WEIGHT
			*	\param pValue	The data value to set (a slope or a weight).
			*/
			void SetDataDouble(FbxCurveDataIndex index, kFCurveDouble value);

			/** Get key data as float value (cubic interpolation, TCB tangent mode).
			*	Warning: Using this method for any key but a cubic interpolated,
			*			 in TCB tangent mode, will return unpredicted values.
			*	\param pIndex	Data index, either KFCURVEKEY_TCB_TENSION, KFCURVEKEY_TCB_CONTINUITY or KFCURVEKEY_TCB_BIAS.
			*/	
			float GetDataFloat(FbxCurveDataIndex index);

			/**	Set data as float value (cubic interpolation, TCB tangent mode).
			*	Warning: Using this method for any key but a cubic interpolated,
			*			 in TCB tangent mode, will return unpredicted values.
			*	\param pIndex	Data index, either KFCURVEKEY_TCB_TENSION, KFCURVEKEY_TCB_CONTINUITY or KFCURVEKEY_TCB_BIAS.
			*	\param pValue	The data value to set.
			*/
			void SetDataFloat(FbxCurveDataIndex index, float value);

			/**	Get key data as a pointer
			*	Warning: not supported in 'double' mode.
			*/
			//float* GetDataPtr();

			//!	Get key value.
			//! Set key value.
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Value);

			/** Increment key value.
			*	\param pValue Value by which key value is incremented.
			*/
			void IncValue(kFCurveDouble value);

			/** Multiply key value.
			*	\param pValue Value by which the key value is multiplied.
			*/
			void MultValue(kFCurveDouble value);

			/** Multiply key tangents.
			*	Note: When multiplying a key value, tangents must be
			*         multiplied to conserve the same topology.
			*	\param pValue Value by which key tangents are multiplied.
			*/
			void MultTangent(kFCurveDouble value);

			/** Get key time
			*	\return Key time (time at which this key is occurring).
			*/
			/** Set key time.
			*	\param pTime Key time (time at which this key is occurring).
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTime^,Time);

			/** Increment key time.
			*	\param pTime Time value by which the key time is incremented.
			*/
			void IncTime(FbxTime^ time);

			/** Return if key is currently selected.
			*	\return Selection flag.
			*/
			/** Set if key is currently selected.
			*	\param pSelected Selection flag.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Selected);	

			/** Return if key is currently marked for manipulation.
			*	\return Mark flag.
			*/
			/** Set if key is currently marked for manipulation.
			*	\param pMark Mark flag.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,MarkedForManipulation);

			/** Return tangent visibility mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\return Tangent visibility mode.
			*	Tangent visibility modes are: KFCURVE_TANGEANT_SHOW_NONE
			*			                      KFCURVE_TANGEANT_SHOW_LEFT
			*			                      KFCURVE_TANGEANT_SHOW_RIGHT
			*/
			/** Set tangent visibility mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\param pVisibility	Tangent visibility mode.
			*	Tangent visibility modes are: KFCURVE_TANGEANT_SHOW_NONE
			*						          KFCURVE_TANGEANT_SHOW_LEFT
			*						          KFCURVE_TANGEANT_SHOW_RIGHT
			*/
			VALUE_PROPERTY_GETSET_DECLARE(KeyTangentVisibility,TangentVisibility);											

			/** Get if tangent is break
			* Only valid for User and Auto keys
			*/
			/** Set/Unset Break tangent
			* Only valid for User and Auto keys
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Break);

			/////////////////////////////////////////////////////////////////////////////////
			////
			////  WARNING!
			////
			////	Anything beyond these lines may not be documented accurately and is 
			//// 	subject to change without notice.
			////
			/////////////////////////////////////////////////////////////////////////////////

			void Initialize();			

		};


		public ref class FbxCurve : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCurve,KFCurve);
			INATIVEPOINTER_DECLARE(FbxCurve,KFCurve);		
		public:

			/*private: static String^ _KFCURVENODE_TRANSFORM;
			public: static property String^ PropName { String^ get(){ if(_KFCURVENODE_TRANSFORM == nullptr) _KFCURVENODE_TRANSFORM = "Transform";return _KFCURVENODE_TRANSFORM; }}*/
			CONST_STRING_DEFINE(TRANSFORM,"Transform");
			CONST_STRING_DEFINE(T,"T");
			CONST_STRING_DEFINE(T_X,"X");
			CONST_STRING_DEFINE(T_Y,"Y");
			CONST_STRING_DEFINE(T_Z,"Z");
			CONST_STRING_DEFINE(R,"R");
			CONST_STRING_DEFINE(R_X,"X");
			CONST_STRING_DEFINE(R_Y,"Y");
			CONST_STRING_DEFINE(R_Z,"Z");
			CONST_STRING_DEFINE(R_W,"W");
			CONST_STRING_DEFINE(S,"S");
			CONST_STRING_DEFINE(S_X,"X");
			CONST_STRING_DEFINE(S_Y,"Y");
			CONST_STRING_DEFINE(S_Z,"Z");
			CONST_STRING_DEFINE(LCL_T,"Lcl Translation");
			CONST_STRING_DEFINE(LCL_R,"Lcl Rotation");
			CONST_STRING_DEFINE(LCL_S,"Lcl Scaling");
			CONST_STRING_DEFINE(VISIBILITY,"Visibility");

			// Addition for the optical marker node attribute.
			CONST_STRING_DEFINE(OCCLUSION,"Occlusion");

			// Addition for the IK effector marker node attribute.
			CONST_STRING_DEFINE(IK_REACH_TRANSLATION,"IK Reach Translation");
			CONST_STRING_DEFINE(IK_REACH_ROTATION,"IK Reach Rotation");

			// Addition for the light node attribute.
			CONST_STRING_DEFINE(LIGHT_CONEANGLE,"Cone angle");
			CONST_STRING_DEFINE(LIGHT_FOG,"Fog");
			CONST_STRING_DEFINE(LIGHT_INTENSITY,"Intensity");
			CONST_STRING_DEFINE(COLOR,"Color");
			CONST_STRING_DEFINE(COLOR_RED,"X");
			CONST_STRING_DEFINE(COLOR_GREEN,"Y");
			CONST_STRING_DEFINE(COLOR_BLUE,"Z");

			// Addition for the camera node attribute.
			CONST_STRING_DEFINE(CAMERA_FIELDOFVIEW,"FieldOfView");
			CONST_STRING_DEFINE(CAMERA_FIELDOFVIEWX,"FieldOfViewX");
			CONST_STRING_DEFINE(CAMERA_FIELDOFVIEWY,"FieldOfViewY");
			CONST_STRING_DEFINE(CAMERA_FOCALLENGTH,"FocalLength");
			CONST_STRING_DEFINE(CAMERA_OPTICALCENTERX,"OpticalCenterX");
			CONST_STRING_DEFINE(CAMERA_OPTICALCENTERY,"OpticalCenterY");
			CONST_STRING_DEFINE(CAMERA_ROLL,"Roll");
			CONST_STRING_DEFINE(CAMERA_TURNTABLE,"TurnTable");
			CONST_STRING_DEFINE(BACKGROUND_COLOR,"BackgroundColor");
			CONST_STRING_DEFINE(BACKGROUND_COLOR_RED,"X");
			CONST_STRING_DEFINE(BACKGROUND_COLOR_GREEN,"Y");
			CONST_STRING_DEFINE(BACKGROUND_COLOR_BLUE,"Z");

			// Addition for the camera switcher node attribute.
			CONST_STRING_DEFINE(CAMERA_INDEX,"Camera Index");

			// Addition for the texture.
			CONST_STRING_DEFINE(TEXTURE_TRANSLATION,"Translation");
			CONST_STRING_DEFINE(TEXTURE_TRANSLATION_X,"X");
			CONST_STRING_DEFINE(TEXTURE_TRANSLATION_Y,"Y");
			CONST_STRING_DEFINE(TEXTURE_TRANSLATION_Z,"Z");
			CONST_STRING_DEFINE(TEXTURE_ROTATION,"Rotation");
			CONST_STRING_DEFINE(TEXTURE_ROTATION_X,"X");
			CONST_STRING_DEFINE(TEXTURE_ROTATION_Y,"Y");
			CONST_STRING_DEFINE(TEXTURE_ROTATION_Z,"Z");
			CONST_STRING_DEFINE(TEXTURE_SCALING,"Scaling");
			CONST_STRING_DEFINE(TEXTURE_SCALING_X,"X");
			CONST_STRING_DEFINE(TEXTURE_SCALING_Y,"Y");
			CONST_STRING_DEFINE(TEXTURE_SCALING_Z,"Z");
			CONST_STRING_DEFINE(TEXTURE_ALPHA,"Alpha");

			// Addition for the material.
			CONST_STRING_DEFINE(MATERIAL_EMISSIVE,"Emissive");
			CONST_STRING_DEFINE(MATERIAL_EMISSIVE_R,"X");
			CONST_STRING_DEFINE(MATERIAL_EMISSIVE_G,"Y");
			CONST_STRING_DEFINE(MATERIAL_EMISSIVE_B,"Z");
			CONST_STRING_DEFINE(MATERIAL_AMBIENT,"Ambient");
			CONST_STRING_DEFINE(MATERIAL_AMBIENT_R,"X");
			CONST_STRING_DEFINE(MATERIAL_AMBIENT_G,"Y");
			CONST_STRING_DEFINE(MATERIAL_AMBIENT_B,"Z");
			CONST_STRING_DEFINE(MATERIAL_DIFFUSE,"Diffuse");
			CONST_STRING_DEFINE(MATERIAL_DIFFUSE_R,"X");
			CONST_STRING_DEFINE(MATERIAL_DIFFUSE_G,"Y");
			CONST_STRING_DEFINE(MATERIAL_DIFFUSE_B,"Z");
			CONST_STRING_DEFINE(MATERIAL_SPECULAR,"Specular");
			CONST_STRING_DEFINE(MATERIAL_SPECULAR_R,"X");
			CONST_STRING_DEFINE(MATERIAL_SPECULAR_G,"Y");
			CONST_STRING_DEFINE(MATERIAL_SPECULAR_B,"Z");
			CONST_STRING_DEFINE(MATERIAL_OPACITY,"Opacity");
			CONST_STRING_DEFINE(MATERIAL_REFLECTIVITY,"Reflectivity");
			CONST_STRING_DEFINE(MATERIAL_SHININESS,"Shininess");

			// Addition for the generic vector property.
			CONST_STRING_DEFINE(USER_PROPERTY_VECTOR_X,"X");
			CONST_STRING_DEFINE(USER_PROPERTY_VECTOR_Y,"Y");
			CONST_STRING_DEFINE(USER_PROPERTY_VECTOR_Z,"Z");

			// Addition for the generic color property.
			CONST_STRING_DEFINE(USER_PROPERTY_COLOR_R,"X");
			CONST_STRING_DEFINE(USER_PROPERTY_COLOR_G,"Y");
			CONST_STRING_DEFINE(USER_PROPERTY_COLOR_B,"Z");


			// Addition of generic matrix
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX,"Matrix");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_0,"0");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_1,"1");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_2,"2");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_3,"3");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_4,"4");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_5,"5");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_6,"6");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_7,"7");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_8,"8");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_9,"9");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_A,"A");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_B,"B");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_C,"C");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_D,"D");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_E,"E");
			CONST_STRING_DEFINE(USER_PROPERTY_MATRIX_F,"F");




		public:

			/**
			* \name Constructor and Destructor
			*/
			//@{

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxCurve,KFCurve);			

#ifdef K_PLUGIN
			void Destroy(int Local);
			void Destroy()
			{
				Destroy(0);
			}
#else
			IObject_Declare(Implementation)
#endif

				//	//@}

				/**	Get function curve color.
				*	\return Pointer to an array of 3 elements: RGB values on a scale from 0 to 1.
				*/
				//float* GetColor();

			VALUE_PROPERTY_GETSET_DECLARE(float,RColor);
			VALUE_PROPERTY_GETSET_DECLARE(float,GColor);
			VALUE_PROPERTY_GETSET_DECLARE(float,BColor);

			/** Set function curve color.
			*	\param pColor Pointer to an array of 3 elements: RGB values on a scale from 0 to 1.
			*/
			//void SetColor(float *pColor);

			/** Get default value.
			* Default value is used when there is no key in the function curve.
			*	\return Default value.
			*/
			/** Set default value.
			* Default value is used when there is no key in the function curve.
			*	\param pValue Default value.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(kFCurveDouble,Value);						

			/**
			* \name Key Management
			*/
			//@{

			/** Resize fcurve buffer to hold a certain number of key.
			* \param pKeyCount Number of key the function curve will eventually hold.
			*/
			void ResizeKeyBuffer(int keyCount);

			/** Call this function prior to modifying the keys of a function curve.
			* Call function KFCurve::KeyModifyEnd() after modification of the keys
			* are completed.
			*/
			void KeyModifyBegin();

			/** Call this function after modification of the keys of a function curve.
			* Call function KFCurve::KeyModifyBegin() prior to modifying the keys.
			*/
			void KeyModifyEnd();

			//! Get the number of keys.
			VALUE_PROPERTY_GET_DECLARE(int,KeyCount);

			//! Get the number of selected keys.
			VALUE_PROPERTY_GET_DECLARE(int,KeySelectionCount);

			// Select all keys.
			void KeySelectAll ();

			// Unselect all keys.
			void KeyUnselectAll ();

			/** Get key at given index.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			FbxCurveKey^ KeyGet(kFCurveIndex index);

			//! Remove all the keys and free buffer memory.
			void KeyClear ();

			//! Minimize use of buffer memory.
			void KeyShrink();

			/** Set key at given index.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* \return true if key time is superior to previous key 
			* and inferior to next key.
			*/
			bool KeySet(kFCurveIndex index, FbxCurveKey^ key);
			bool	KeySet(kFCurveIndex index, FbxCurve^ sourceCurve, int sourceIndex);

			/** Change time of key found at given index.
			*	\param pIndex Index of key to move.
			*	\param pTime Destination time.
			*	\return New index of moved key.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			int KeyMove(kFCurveIndex index, FbxTime^ time);

			/** Add time and value offsets to keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pDeltaTime Time offset added to keys.
			*	\param pDeltaValue Value offset added to keys.
			*	\return true on success.
			*/
			bool KeyMoveOf(bool selectedOnly,FbxTime^ deltaTime,kFCurveDouble deltaValue);

			/** Set value of keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pValue Value set to keys.
			*	\return true on success.
			*/
			bool KeyMoveValueTo(bool selectedOnly, kFCurveDouble value);

			/** Scale value of keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pMultValue Scale applied on key values.
			*	\return true on success.
			*/
			bool KeyScaleValue (bool selectedOnly, kFCurveDouble multValue);

			/** Scale tangent of keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pMultValue Scale applied on key tangents.
			*	\return true on success.
			*/
			bool KeyScaleTangent (bool selectedOnly, kFCurveDouble multValue);

			/** Scale value and tangent of keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pMultValue Scale applied on key values and tangents.
			*	\return true on success.
			*/
			bool KeyScaleValueAndTangent (bool selectedOnly, kFCurveDouble multValue);

			/** Remove key at given index.
			*	\param pIndex Index of key to remove.
			*	\return true on success.
			*/
			bool KeyRemove(kFCurveIndex index);

			/** Insert a key at given time.
			*	This function SHOULD be used instead of KFCurve::KeyAdd() if the key 
			* is to be added in the curve and not at the end. It inserts the key in 
			* respect to the interpolation type and tangents of the neighboring keys. 
			* If there is already a key a the given time, the key is modified and no 
			* new key is added.
			*	\param pTime Time to insert the key.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Index of the key at given time, no matter if it was inserted 
			* or already present.
			* \remarks Key value must be set explicitly afterwards. The 
			* interpolation type and tangent mode are copied from the previous key.
			*/
			int KeyInsert( FbxTime^ time, kFCurveIndex %last);
			int KeyInsert( FbxTime^ time);

			/** Add a key at given time.
			*	Function KFCurve::KeyInsert() SHOULD be used instead if the key 
			* is to be added in the curve and not at the end. This function does not
			* respect the interpolation type and tangents of the neighboring keys. 
			* If there is already a key at the given time, the key is modified and no 
			* new key is added.
			*	\param pTime Time to add the key.
			* \param pKey Key to add.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Index of the key at given time, no matter if it was added 
			* or already present.
			* \remarks Key value, interpolation type and tangent mode must be set 
			* explicitly afterwards.
			*/
			int KeyAdd (FbxTime^ time, FbxCurveKey^ key, kFCurveIndex %last);
			int KeyAdd (FbxTime^ time, FbxCurveKey^ key);
			int KeyAdd(FbxTime^ time, FbxCurve^ sourceCurve, int sourceIndex, kFCurveIndex %last);
			int KeyAdd(FbxTime^ time, FbxCurve^ sourceCurve, int sourceIndex);

			/** Add a key at given time.
			*	Function KFCurve::KeyInsert() SHOULD be used instead if the key 
			* is to be added in the curve and not at the end. This function does not
			* respect of the interpolation type and tangents of the neighboring keys. 
			* If there is already a key a the given time, no key is added.
			*	\param pTime Time to add the key.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Index of the key at given time, no matter if it was added 
			* or already present.
			* \remarks Key value, interpolation type and tangent mode must be set 
			* explicitely afterwards.
			*/
			int KeyAdd(FbxTime^ time, kFCurveIndex %last);
			int KeyAdd(FbxTime^ time);

			/** Append a key at the end of the function curve.
			* \param pAtTime Time of appended key, must be superior to the 
			* last key time.
			* \param pSourceCurve Source curve.
			* \param pSourceIndex Index of the source key in the source curve.
			* \return Index of appended key.
			*/
			int KeyAppend(FbxTime^ atTime, FbxCurve^ sourceCurve, int sourceIndex);

			/** Append a key at the end of the function curve.
			* \param pTime Time of appended key, must be superior to the 
			* last key time.
			* \param pValue Value of appended key.
			* \return Index of appended key.
			* \remarks Interpolation type of the appended key is set to 
			* KFCURVE_INTERPOLATION_CUBIC and tangent mode is set to 
			* KFCURVE_TANGEANT_AUTO.
			*/
			int KeyAppendFast(FbxTime^ time, kFCurveDouble value);

			/** Find key index for a given time.
			*	\param pTime Time of the key looked for.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Key index. The integer part of the key index gives the 
			* index of the closest key with a smaller time. The decimals give
			* the relative position of given time compared to previous and next
			* key times. Returns -1 if function curve has no key.
			*/
			double KeyFind(FbxTime^ time, kFCurveIndex %last);	
			double KeyFind(FbxTime^ time);	

			//@}

			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/


			/**
			* \name Key Manipulation
			*/
			//@{

			/** Set a key.
			*	Use SetTCB() to set a key with cubic interpolation and TCB tangent type.
			*   \param pKeyIndex        Key index
			*	\param pTime			Key time.
			*	\param pValue			Key value.
			*	\param pInterpolation	Key interpolation type.	Interpolation types are: 
			*							KFCURVE_INTERPOLATION_CONSTANT, 
			*							KFCURVE_INTERPOLATION_LINEAR,
			*							KFCURVE_INTERPOLATION_CUBIC
			*	\param pTangentMode		Key tangent mode (meaningful for cubic 
			*							interpolation only). Tangent modes are: 
			*							KFCURVE_TANGEANT_AUTO,
			*							KFCURVE_TANGEANT_USER,
			*							KFCURVE_TANGEANT_BREAK
			*	\param pData0			Right slope.
			*	\param pData1			Next left slope.
			*	\param pTangentWeightMode	Weight mode if used
			*								KFCURVE_WEIGHTED_NONE
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*	\param pWeight0				Right slope weight.
			*	\param pWeight1				Next left slope weight.
			*	\param pVelocity0			Right velocity.
			*	\param pVelocity1			Next left velocity.
			*/
			void KeySet(kFCurveIndex keyIndex,FbxTime^ time, 
				kFCurveDouble value, 
				FbxCurveKey::KeyInterpolation interpolation, 
				FbxCurveKey::KeyTangentMode tangentMode, 
				kFCurveDouble data0,kFCurveDouble data1,
				FbxCurveKey::KeyTangentWeightMode tangentWeightMode, 
				kFCurveDouble weight0,kFCurveDouble weight1,
				kFCurveDouble velocity0,
				kFCurveDouble velocity1);

			void KeySet(kFCurveIndex keyIndex,FbxTime^ time, 
				kFCurveDouble value, 
				FbxCurveKey::KeyInterpolation interpolation);

			void KeySet(
				kFCurveIndex keyIndex,
				FbxTime^ time, 
				kFCurveDouble value, 
				kFCurveInterpolation interpolation, 
				kFCurveTangeantMode tangentMode, 
				kFCurveDouble data0,
				kFCurveDouble data1,
				kFCurveTangeantWeightMode tangentWeightMode, 
				kFCurveDouble weight0,
				kFCurveDouble weight1,
				kFCurveDouble velocity0,
				kFCurveDouble velocity1);
			void KeySet(
				kFCurveIndex keyIndex,
				FbxTime^ time, 
				kFCurveDouble value, 
				kFCurveInterpolation interpolation,
				kFCurveTangeantMode tangentMode);

			/**	Set a key with cubic interpolation, TCB tangent mode.
			*   \param pKeyIndex  Key index
			*	\param pTime	Key time.
			*	\param pValue	Key value.
			*	\param pData0	Tension.
			*	\param pData1	Continuity.
			*	\param pData2	Bias.
			*/
			void KeySetTCB(kFCurveIndex keyIndex,FbxTime^ time, 
				kFCurveDouble value, 
				float data0,float data1,float data2);

			/** Get key interpolation type.
			*	Interpolation types are: KFCURVE_INTERPOLATION_CONSTANT, 
			*							 KFCURVE_INTERPOLATION_LINEAR,
			*							 KFCURVE_INTERPOLATION_CUBIC
			*   \param pKeyIndex         Key index
			*   \return                  Key interpolation type
			*/
			FbxCurveKey::KeyInterpolation KeyGetInterpolation(kFCurveIndex keyIndex);

			/** Set key interpolation type.
			*   \param pKeyIndex      Key index
			*	\param pInterpolation Key interpolation type.
			*	Interpolation types are: KFCURVE_INTERPOLATION_CONSTANT, 
			*							 KFCURVE_INTERPOLATION_LINEAR,
			*							 KFCURVE_INTERPOLATION_CUBIC
			*/
			void KeySetInterpolation(kFCurveIndex keyIndex, FbxCurveKey::KeyInterpolation interpolation);

			/** Get key constant mode.
			*	Warning: This method is meaningful for constant interpolation only.
			*			 Using this method for non constant interpolated key will return unpredicted value.
			* Constant modes are:		KFCURVE_CONSTANT_STANDARD
			*							KFCURVE_CONSTANT_NEXT
			*   \param pKeyIndex      Key index
			*	\return Key constant mode.
			*/
			FbxCurveKey::KeyConstantMode KeyGetConstantMode(kFCurveIndex keyIndex);

			/** Get key tangent mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*			 Using this method for non cubic interpolated key will return unpredicted value.
			*	Tangent modes are: KFCURVE_TANGEANT_AUTO,
			*					   KFCURVE_TANGEANT_AUTO_BREAK
			*					   KFCURVE_TANGEANT_TCB,
			*					   KFCURVE_TANGEANT_USER,
			*					   KFCURVE_TANGEANT_BREAK
			*	\return Key tangent mode.
			*/
			FbxCurveKey::KeyTangentMode KeyGetTangentMode(kFCurveIndex keyIndex, bool includeOverrides);
			FbxCurveKey::KeyTangentMode KeyGetTangentMode(kFCurveIndex keyIndex)
			{
				return KeyGetTangentMode(keyIndex, false);
			}

			/** Get key tangent weight mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	Tangent weight modes are:	KFCURVE_WEIGHTED_NONE,
			*								KFCURVE_WEIGHTED_RIGHT,
			*								KFCURVE_WEIGHTED_NEXT_LEFT,
			*								KFCURVE_WEIGHTED_ALL
			*/
			FbxCurveKey::KeyTangentWeightMode KeyGetTangentWeightMode(kFCurveIndex keyIndex);

			/** Get key tangent velocity mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	Tangent weight modes are:	KFCURVE_VELOCITY_NONE,
			*								KFCURVE_VELOCITY_RIGHT,
			*								KFCURVE_VELOCITY_NEXT_LEFT,
			*								KFCURVE_VELOCITY_ALL
			*/
			FbxCurveKey::KeyTangentVelocityMode KeyGetTangentVelocityMode(kFCurveIndex keyIndex);

			/** Set key constant mode.
			*	Warning: This method is meaningful for constant interpolation only.
			*   \param pKeyIndex            Key index
			*	\param pMode Key consant mode.
			*	Constant modes are:		KFCURVE_CONSTANT_STANDARD
			*							KFCURVE_CONSTANT_NEXT
			*/
			void KeySetConstantMode(kFCurveIndex keyIndex, FbxCurveKey::KeyConstantMode mode);

			/** Set key tangent mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*   \param pKeyIndex   Key index
			*	\param pTangent Key tangent mode.
			*	Tangent modes are: KFCURVE_TANGEANT_AUTO,
			*					   KFCURVE_TANGEANT_AUTO_BREAK
			*					   KFCURVE_TANGEANT_TCB,
			*					   KFCURVE_TANGEANT_USER,
			* 				   KFCURVE_TANGEANT_BREAK
			*/
			void KeySetTangentMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentMode tangent);

			/** Set key tengent weight mode as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: This method is meaningful for cubic interpolation only.
			*   \param pKeyIndex   Key index
			*	\param pTangentWeightMode	Weight mode
			*								KFCURVE_WEIGHTED_NONE
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*	\param pMask				Used to select the affected tangents
			*								KFCURVE_WEIGHTED_RIGHT
			*								KFCURVE_WEIGHTED_NEXT_LEFT
			*								KFCURVE_WEIGHTED_ALL
			*/

			void KeySetTangentWeightMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentWeightMode tangentWeightMode, FbxCurveKey::KeyTangentWeightMode mask);
			void KeySetTangentWeightMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentWeightMode tangentWeightMode);

			/** Set key tengent velocity mode as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: This method is meaningful for cubic interpolation only.
			*   \param pKeyIndex   Key index
			*	\param pTangentVelocityMode	Weight mode
			*								KFCURVE_VELOCITY_NONE
			*								KFCURVE_VELOCITY_RIGHT
			*								KFCURVE_VELOCITY_NEXT_LEFT
			*								KFCURVE_VELOCITY_ALL
			*	\param pMask				Used to select the affected tangents
			*								KFCURVE_VELOCITY_RIGHT
			*								KFCURVE_VELOCITY_NEXT_LEFT
			*								KFCURVE_VELOCITY_ALL
			*/

			void KeySetTangentVelocityMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVelocityMode tangentVelocityMode, FbxCurveKey::KeyTangentVelocityMode mask);
			void KeySetTangentVelocityMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVelocityMode tangentVelocityMode);


			/** Get key data as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: Using this method for other than cubic interpolated 
			*			 key (linear, constant) will return unpredicted values.
			*	Warning: Slope data is inconsistent for automatic tangent mode.
			*			 Use KFCurve::EvaluateLeftDerivative() and 
			*			 KFCurve::EvaluateRightDerivative() to find
			*			 slope values.
			*	Warning: Using this method for TCB tangent mode key will return 
			*			 unpredicted values. Use KFCurve::GetDataFloat() instead.
			*   \param pKeyIndex   Key index
			*	\param pIndex Data index, either	KFCURVEKEY_RIGHT_SLOPE,
			*										KFCURVEKEY_NEXT_LEFT_SLOPE.
			*										KFCURVEKEY_NEXT_RIGHT_WEIGHT.
			*										KFCURVEKEY_NEXT_LEFT_WEIGHT
			*/
			kFCurveDouble KeyGetDataDouble(kFCurveIndex keyIndex, FbxCurveDataIndex index);

			/**	Set data as double value (cubic interpolation, non TCB tangent mode).
			*	Warning: Using this method for other than cubic interpolated 
			*			 key (linear, constant) is irrelevant.
			*	Warning: Slope data is inconsistent for automatic tangent mode.
			*			 Therefore, it is irrelevant to use this method on automatic 
			*			 tangent mode keys.
			*	Warning: Using this method for a TCB tangeant mode key will result
			*			 in unpredicted curve behavior for this key. Use KFCurve::SetDataFloat() 
			*			 instead.
			*   \param pKeyIndex   Key index
			*	\param pIndex Data index, either	KFCURVEKEY_RIGHT_SLOPE,
			*										KFCURVEKEY_NEXT_LEFT_SLOPE.
			*										KFCURVEKEY_NEXT_RIGHT_WEIGHT.
			*										KFCURVEKEY_NEXT_LEFT_WEIGHT
			*	\param pValue	The data value to set (a slope or a weight).
			*/
			void KeySetDataDouble(kFCurveIndex keyIndex, FbxCurveDataIndex index, kFCurveDouble value);

			/** Get key data as float value (cubic interpolation, TCB tangent mode).
			*	Warning: Using this method for any key but a cubic interpolated,
			*			 in TCB tangent mode, will return unpredicted values.
			*   \param pKeyIndex   Key index
			*	\param pIndex	Data index, either KFCURVEKEY_TCB_TENSION, KFCURVEKEY_TCB_CONTINUITY or KFCURVEKEY_TCB_BIAS.
			*/	
			float KeyGetDataFloat(kFCurveIndex keyIndex, FbxCurveDataIndex index);

			/**	Set data as float value (cubic interpolation, TCB tangent mode).
			*	Warning: Using this method for any key but a cubic interpolated,
			*			 in TCB tangent mode, will return unpredicted values.
			*   \param pKeyIndex   Key index
			*	\param pIndex	Data index, either KFCURVEKEY_TCB_TENSION, KFCURVEKEY_TCB_CONTINUITY or KFCURVEKEY_TCB_BIAS.
			*	\param pValue	The data value to set.
			*/
			void KeySetDataFloat(kFCurveIndex keyIndex, FbxCurveDataIndex index, float value);

			/**	Get key data as a pointer
			*	Warning: not supported in 'double' mode.
			*/
			//KFCURVE_INLINE const float* KeyGetDataPtr(kFCurveIndex pKeyIndex);

			//!	Get key value.
			kFCurveDouble KeyGetValue(kFCurveIndex keyIndex);

			//! Set key value.
			void KeySetValue(kFCurveIndex keyIndex, kFCurveDouble value);

			/** Increment key value.
			*   \param pKeyIndex   Key index
			*	\param pValue Value by which key value is incremented.
			*/
			void KeyIncValue(kFCurveIndex keyIndex, kFCurveDouble value);

			/** Multiply key value.
			*   \param pKeyIndex   Key index
			*	\param pValue Value by which the key value is multiplied.
			*/
			void KeyMultValue(kFCurveIndex keyIndex, kFCurveDouble value);

			/** Multiply key tangents.
			*	Note: When multiplying a key value, tangents must be
			*         multiplied to conserve the same topology.
			*   \param pKeyIndex   Key index
			*	\param pValue Value by which key tangents are multiplied.
			*/
			void KeyMultTangent(kFCurveIndex keyIndex, kFCurveDouble value);

			/** Get key time
			*   \param pKeyIndex   Key index
			*	\return Key time (time at which this key is occuring).
			*/
			FbxTime^ KeyGetTime(kFCurveIndex keyIndex);

			/** Set key time.
			*   \param pKeyIndex   Key index
			*	\param pTime Key time (time at which this key is occuring).
			*/
			void KeySetTime(kFCurveIndex keyIndex, FbxTime^ time);

			/** Increment key time.
			*   \param pKeyIndex   Key index
			*	\param pTime Time value by which the key time is incremented.
			*/
			void KeyIncTime(kFCurveIndex keyIndex, FbxTime^ time);

			/** Set if key is currently selected.
			*   \param pKeyIndex   Key index
			*	\param pSelected Selection flag.
			*/
			void KeySetSelected(kFCurveIndex keyIndex, bool selected);	

			/** Return if key is currently selected.
			*	\return Selection flag.
			*/
			bool KeyGetSelected(kFCurveIndex keyIndex);

			/** Set if key is currently marked for manipulation.
			*   \param pKeyIndex   Key index
			*	\param pMark Mark flag.
			*/
			void KeySetMarkedForManipulation(kFCurveIndex keyIndex, bool mark);	

			/** Return if key is currently marked for manipulation.
			*	\return Mark flag.
			*/
			bool KeyGetMarkedForManipulation(kFCurveIndex keyIndex);

			/** Set tangent visibility mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*   \param pKeyIndex   Key index
			*	\param pVisibility	Tangent visibility mode.
			*	Tangent visibility modes are: KFCURVE_TANGEANT_SHOW_NONE
			*						          KFCURVE_TANGEANT_SHOW_LEFT
			*						          KFCURVE_TANGEANT_SHOW_RIGHT
			*/
			void KeySetTangentVisibility(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVisibility visibility);	

			/** Return tangent visibility mode.
			*	Warning: This method is meaningful for cubic interpolation only.
			*	\return Tangent visibility mode.
			*	Tangent visibility modes are: KFCURVE_TANGEANT_SHOW_NONE
			*			                      KFCURVE_TANGEANT_SHOW_LEFT
			*			                      KFCURVE_TANGEANT_SHOW_RIGHT
			*/
			FbxCurveKey::KeyTangentVisibility KeyGetTangentVisibility(kFCurveIndex keyIndex);

			/** Set/Unset Break tangeant
			* Only valid for User and Auto keys
			*/
			void KeySetBreak(kFCurveIndex keyIndex, bool val); 

			/** Get if tangeant is break
			* Only valid for User and Auto keys
			*/
			bool KeyGetBreak(kFCurveIndex keyIndex); 

			//@}

			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/
			/************************************************************************************************/

			/**
			* \name Key Tangent Management
			*/
			//@{

			/** Set interpolation type on keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pInterpolation Interpolation type.
			*/
			void KeyTangentSetInterpolation(bool selectedOnly, FbxCurveKey::KeyInterpolation interpolation);

			/** Set tangent mode on keys, all or selected only.
			*	\param pSelectedOnly If set to \c true, only selected keys are affected.
			* Otherwise, all keys are affected.
			*	\param pTangentMode Tangent mode.
			* \remarks Tangent mode is only relevant on keys with a cubic interpolation type.
			*/
			void KeyTangentSetMode(bool selectedOnly, FbxCurveKey::KeyTangentMode tangentMode);

			/** Get the left derivative of a key.
			*	\param pIndex Index of key.
			*	\return Left derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			kFCurveDouble KeyGetLeftDerivative(kFCurveIndex index);

			/** Set the left derivative of a key.
			*	\param pIndex Index of key.
			*	\param pValue Left derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER, KFCURVE_TANGEANT_BREAK or KFCURVE_TANGEANT_AUTO. 
			*/
			void KeySetLeftDerivative(kFCurveIndex index, kFCurveDouble value);

			/** Get the left auto parametric of a key.
			*	\param pIndex Index of key.
			*	\param pApplyOvershootProtection Clamp is taking into account.
			*	\return left auto parametric.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			kFCurveDouble KeyGetLeftAuto(kFCurveIndex index, bool applyOvershootProtection);
			kFCurveDouble KeyGetLeftAuto(kFCurveIndex index)
			{
				return KeyGetLeftAuto(index,false);
			}

			/** Set the left auto parametric  of a key.
			*	\param pIndex Index of key.
			*	\param pValue Left auto parametric .
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER, KFCURVE_TANGEANT_BREAK or KFCURVE_TANGEANT_AUTO.
			*/
			void KeySetLeftAuto(kFCurveIndex index, kFCurveDouble value);	

			/** Get the left derivative info of a key.
			*	\param pIndex Index of key.
			*	\return Left derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			FbxCurveTangentInfo^ KeyGetLeftDerivativeInfo(kFCurveIndex index);

			/** Set the left derivative info of a key.
			*	\param pIndex Index of key.
			*	\param pValue Left derivative.
			*   \param pForceDerivative
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/

			void KeySetLeftDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value, bool forceDerivative);
			void KeySetLeftDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value)
			{
				KeySetLeftDerivativeInfo(index,value,false);
			}


			/** Increment the left derivative of a key.
			*	\param pIndex Index of key.
			*	\param pInc Increment to left derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeyIncLeftDerivative(kFCurveIndex index, kFCurveDouble inc);

			/** Get the right derivative of a key.
			*	\param pIndex Index of key.
			*	\return Right derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			kFCurveDouble KeyGetRightDerivative(kFCurveIndex index);

			/** Set the right derivative of a key.
			*	\param pIndex Index of key.
			*	\param pValue Right derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER, KFCURVE_TANGEANT_BREAK or KFCURVE_TANGEANT_AUTO.
			*/
			void KeySetRightDerivative(kFCurveIndex index, kFCurveDouble value);

			/** Get the right auto parametric of a key.
			*	\param pIndex Index of key.
			*	\param pApplyOvershootProtection Clamp is taking into account.
			*	\return Right auto parametric.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			kFCurveDouble KeyGetRightAuto(kFCurveIndex index, bool applyOvershootProtection);
			kFCurveDouble KeyGetRightAuto(kFCurveIndex index)
			{
				return KeyGetRightAuto(index,false);
			}


			/** Set the right auto parametric  of a key.
			*	\param pIndex Index of key.
			*	\param pValue Right auto parametric .
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER, KFCURVE_TANGEANT_BREAK or KFCURVE_TANGEANT_AUTO.
			*/
			void KeySetRightAuto(kFCurveIndex index, kFCurveDouble value);


			/** Get the right derivative info of a key.
			*	\param pIndex Index of key.
			*	\return Right derivative info.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			FbxCurveTangentInfo^ KeyGetRightDerivativeInfo(kFCurveIndex index);

			/** Set the right derivative info of a key.
			*	\param pIndex Index of key.
			*	\param pValue Right derivative.
			*   \param pForceDerivative
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeySetRightDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value, bool forceDerivative);
			void KeySetRightDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value)
			{
				KeySetRightDerivativeInfo(index,value,false);
			}


			/** Increment the right derivative of a key.
			*	\param pIndex Index of key.
			*	\param pInc Increment to right derivative.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeyIncRightDerivative(kFCurveIndex index, kFCurveDouble inc);

			//! This function is disabled and always return 0.
			kFCurveDouble KeyGetRightBezierTangent(kFCurveIndex index);

			/** Set the left derivative of a key as a Bezier tangent.
			*	\param pIndex Index of key.
			*	\param pValue Left derivative as a Bezier tangent.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeySetLeftBezierTangent(kFCurveIndex index, kFCurveDouble value);

			//! This function is disabled and always returns 0.
			kFCurveDouble KeyGetLeftBezierTangent(kFCurveIndex index);

			/** Set the right derivative of a key as a Bezier tangent.
			*	\param pIndex Index of key.
			*	\param pValue Right derivative as a Bezier tangent.
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeySetRightBezierTangent(kFCurveIndex index, kFCurveDouble value);


			/** Multiply the Derivative of a key.
			*	\param pIndex Index of key.
			*	\param pMultValue Value that multiply Derivative
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			* This function is only relevant if key interpolation is 
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void KeyMultDerivative(kFCurveIndex index, kFCurveDouble multValue);

			/** Get the left tangent weight mode of a key
			*	\param pIndex Index of key.
			*	\return true if the key is weighted
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			bool KeyIsLeftTangentWeighted(kFCurveIndex index);

			/** Get the right tangent weight mode of a key
			*	\param pIndex Index of key.
			*	\return true if the key is weighted
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			bool KeyIsRightTangentWeighted(kFCurveIndex index);

			/** Set the left tangent weight mode of a key
			*	\param pIndex Index of key.
			*	\param pWeighted Weighted state of the tangent
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void   KeySetLeftTangentWeightedMode( kFCurveIndex index, bool weighted );

			/** Set the right tangent weight mode of a key
			*	\param pIndex Index of key.
			*	\param pWeighted Weighted state of the tangent
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void   KeySetRightTangentWeightedMode( kFCurveIndex index, bool weighted );

			/** Get the weight value component of the left tangent of a key
			*	\param pIndex Index of key.
			*	\return right tangen weight
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC
			*/
			kFCurveDouble KeyGetLeftTangentWeight(kFCurveIndex index);

			/** Get the weight value component of the right tangent of a key
			*	\param pIndex Index of key.
			*	\return right tangen weight
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC
			*/		
			kFCurveDouble KeyGetRightTangentWeight(kFCurveIndex index);

			/** Set the left tangent weight of a key
			*	\param pIndex Index of key.
			*	\param pWeight Weight
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK. The tangent is 
			* automatically set in weighted mode.
			*/
			void   KeySetLeftTangentWeight( kFCurveIndex index, kFCurveDouble weight );

			/** Set the right tangent weight of a key
			*	\param pIndex Index of key.
			*	\param pWeight Weight
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK. The tangent is 
			* automatically set in weighted mode.
			*/
			void   KeySetRightTangentWeight( kFCurveIndex index, kFCurveDouble weight );

			/** Get the left tangent velocity mode of a key
			*	\param pIndex Index of key.
			*	\return true if the key has velocity
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			bool KeyIsLeftTangentVelocity(kFCurveIndex index);

			/** Get the right tangent velocity mode of a key
			*	\param pIndex Index of key.
			*	\return true if the key has velocity
			* \remarks Result is undetermined if function curve has no key or index 
			* is out of bounds.
			*/
			bool KeyIsRightTangentVelocity(kFCurveIndex index);

			/** Set the left tangent velocity mode of a key
			*	\param pIndex Index of key.
			*	\param pVelocity Velocity state of the tangent
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void   KeySetLeftTangentVelocityMode( kFCurveIndex index, bool velocity );

			/** Set the right tangent velocity mode of a key
			*	\param pIndex Index of key.
			*	\param pVelocity Velocity state of the tangent
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK.
			*/
			void   KeySetRightTangentVelocityMode( kFCurveIndex index, bool velocity);

			/** Get the velocity value component of the left tangent of a key
			*	\param pIndex Index of key.
			*	\return right tangen velocity
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC
			*/
			kFCurveDouble KeyGetLeftTangentVelocity(kFCurveIndex index);

			/** Get the velocity value component of the right tangent of a key
			*	\param pIndex Index of key.
			*	\return right tangen velocity
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC
			*/		
			kFCurveDouble KeyGetRightTangentVelocity(kFCurveIndex index);

			/** Set the left tangent velocity of a key
			*	\param pIndex Index of key.
			*	\param pVelocity Velocity
			* This function is only relevant if previous key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK. The tangent is 
			* automatically set in velocity mode.
			*/
			void KeySetLeftTangentVelocity(kFCurveIndex index, kFCurveDouble velocity );

			/** Set the right tangent velocity of a key
			*	\param pIndex Index of key.
			*	\param pVelocity Velocity
			* This function is only relevant if key interpolation
			* type is KFCURVE_INTERPOLATION_CUBIC and tangent mode is
			* KFCURVE_TANGEANT_USER or KFCURVE_TANGEANT_BREAK. The tangent is 
			* automatically set in velocity mode.
			*/
			void KeySetRightTangentVelocity(kFCurveIndex index, kFCurveDouble velocity);

			//@}

			/**
			* \name Extrapolation 
			* Extrapolation defines the function curve value before and after the keys.
			* Pre-extrapolation defines the function curve value before first key.
			* Post-extrapolation defines the function curve value after last key.
			* <ul><li>KFCURVE_EXTRAPOLATION_CONST means a constant value matching the first/last key
			*	    <li>KFCURVE_EXTRAPOLATION_REPETITION means the entire function curve is looped
			*		<li>KFCURVE_EXTRAPOLATION_MIRROR_REPETITION means the entire function curve is looped once backward, once forward and so on 
			*		<li>KFCURVE_EXTRAPOLATION_KEEP_SLOPE means a linear function with a slope matching the first/last key</ul>
			*/
			//@{

			//! Get pre-extrapolation mode.
			//! Set pre-extrapolation mode.
			//VALUE_PROPERTY_GET_DECLARE(kFCurveExtrapolationMode,PreExtrapolation);												

			/** Get pre-extrapolation count.
			*	\return Number of repetitions if pre-extrapolation mode is
			* KFCURVE_EXTRAPOLATION_REPETITION or KFCURVE_EXTRAPOLATION_MIRROR_REPETITION.
			/** Set pre-extrapolation count.
			*	\param pCount Number of repetitions if pre-extrapolation mode is
			* KFCURVE_EXTRAPOLATION_REPETITION or KFCURVE_EXTRAPOLATION_MIRROR_REPETITION.
			*/

			//VALUE_PROPERTY_GET_DECLARE(kULong,PreExtrapolationCount);


			//! Get post-extrapolation mode.
			//! Set post-extrapolation mode.
			//VALUE_PROPERTY_GET_DECLARE(kFCurveExtrapolationMode,PostExtrapolation);											

			/** Get post-extrapolation count.
			*	\return Number of repetitions if post-extrapolation mode is
			* KFCURVE_EXTRAPOLATION_REPETITION or KFCURVE_EXTRAPOLATION_MIRROR_REPETITION.
			*/
			/** Set post-extrapolation count.
			*	\param pCount Number of repetitions if post-extrapolation mode is
			* KFCURVE_EXTRAPOLATION_REPETITION or KFCURVE_EXTRAPOLATION_MIRROR_REPETITION.
			*/
			//VALUE_PROPERTY_GET_DECLARE(kULong,PostExtrapolationCount);

			/** Get total number of keys taking extrapolation into account.
			* The total number of keys includes repetitions of the function 
			* curve if pre-extrapolation and/or post-extrapolation are of
			* mode KFCURVE_EXTRAPOLATION_REPETITION or KFCURVE_EXTRAPOLATION_MIRROR_REPETITION.
			*	\return Total number of keys taking extrapolation into account.
			*/
			int KeyGetCountAll();

			/** Find key index for a given time taking extrapolation into account.
			*	\param pTime Time of the key looked for.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Key index between 0 and KFCurve::KeyGetCount() - 1.The 
			* integer part of the key index gives the index of the closest key 
			* with a smaller time. The decimals give the relative position of 
			* given time compared to previous and next key times. Return -1 if 
			* function curve has no key.
			*/
			double KeyFindAll(FbxTime^ time, kFCurveIndex %last);
			double KeyFindAll(FbxTime^ time);

			//@}

			/**
			* \name Evaluation and Analysis
			*/
			//@{

			/**	Evaluate function curve value at a given time.
			*	\param pTime Time of evaluation.
			* If time falls between two keys, function curve value is 
			* interpolated according to previous key interpolation type and
			* tangent mode if relevant.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Function curve value or default value if function curve
			* has no key.
			* \remarks This function takes extrapolation into account.
			*/
			kFCurveDouble Evaluate (FbxTime^ time, kFCurveIndex %last);
			kFCurveDouble Evaluate (FbxTime^ time);

			/**	Evaluate function curve value at a given key index.
			*	\param pIndex Any value between 0 and KFCurve::KeyGetCount() - 1.
			* If key index is not an integer value, function curve value is 
			* interpolated according to previous key interpolation type and
			* tangent mode if relevant.
			*	\return Function curve value or default value if function curve
			* has no key.
			* \remarks This function does not take extrapolation into account.
			*/
			kFCurveDouble EvaluateIndex( double index);

			/**	Evaluate function left derivative at given time.
			*	\param pTime Time of evaluation.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Left derivative at given time.
			* \remarks This function does not take extrapolation into account.
			*/
			kFCurveDouble EvaluateLeftDerivative (FbxTime^ time, kFCurveIndex %last);
			kFCurveDouble EvaluateLeftDerivative (FbxTime^ time);

			/**	Evaluate function right derivative at given time.
			*	\param pTime Time of evaluation.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			*	\return Right derivative at given time.
			* \remarks This function does not take extrapolation into account.
			*/
			kFCurveDouble EvaluateRightDerivative(FbxTime^ time, kFCurveIndex %last);
			kFCurveDouble EvaluateRightDerivative(FbxTime^ time);

			/**	Find the peaks time between 2 keys (a local minimum and/or maximum).
			*	\param pLeftKeyIndex Left key index (there must be a right key).
			*	\param pPeakTime1 First peak time.
			*	\param pPeakTime2 Second peak time.
			*	\return Number of peaks found.
			* \remarks Result is undetermined if function curve has no key or 
			* index is out of bounds.
			*/
			int FindPeaks(kFCurveIndex leftKeyIndex, FbxTime^ peakTime1, FbxTime^ peakTime2);

			/**	Find the peaks value between 2 keys (a local minimum and/or maximum).
			*	\param pLeftKeyIndex Left key index (there must be a right key).
			*	\param pPeak1 First peak value.
			*	\param pPeak2 Second peak value.
			*	\return Number of peaks found.
			* \remarks Result is undetermined if function curve has no key or 
			* index is out of bounds.
			*/
			int FindPeaks(kFCurveIndex leftKeyIndex, kFCurveDouble %peak1, kFCurveDouble %peak2);

			/**	Find the peaks time and value between 2 keys (a local minimum and/or maximum).
			*	\param pLeftKeyIndex Left key index (there must be a right key).
			*	\param pPeakTime1 First peak time.
			*	\param pPeak1 First peak value.
			*	\param pPeakTime2 Second peak time.
			*	\param pPeak2 Second peak value.
			*	\return Number of peaks found.
			* \remarks Result is undetermined if function curve has no key or 
			* index is out of bounds.
			*/
			int FindPeaks(kFCurveIndex leftKeyIndex, FbxTime^ peakTime1, kFCurveDouble %peak1, FbxTime^ peakTime2, kFCurveDouble %peak2);

			/** Get key period statistics. If pAveragePeriod == pMinPeriod, we have iso-sampled data.
			*	\param pAveragePeriod Average key period.
			*	\param pMinPeriod Minimum period found.
			*	\param pMaxPeriod Maximum period found.
			*/
			void KeyGetPeriods(FbxTime^ averagePeriod,FbxTime^ minPeriod, FbxTime^ maxPeriod);

			//@}

			/**
			* \name Copy, Insert, Replace and Delete Functions
			*/
			//@{

			/** Create a new function curve and copy keys found between a given time range.
			* Time range is inclusive.
			*	\param pStart Start of time range.
			*	\param pStop End of time range.
			*	\return Created function curve.
			* \remarks 
			*/
			FbxCurve^ Copy(FbxTime^ start, FbxTime^ stop);

			/** Copy a function curve content into current function curve.
			*	\param pSource Source function curve.
			*	\param pWithKeys If \c true, clear keys in current function curve and copy
			* keys from source function curve. If \c false, keys in current function curve
			* are left as is.
			*/
			void CopyFrom(FbxCurve^ source, bool withKeys);

			/**	Replace keys within a range in current function curve with keys found in a source function curve.
			* \param pSource Source function curve.
			* \param	pStart Start of time range.
			* \param	pStop End of time range.
			* \param pUseExactGivenSpan false = original behavior where time of first and last key was used
			* \param pKeyStartEndOnNoKey Inserts a key at the beginning and at the end of the range if there is no key to insert.
			* \param pTimeSpanOffset
			*/
			void Replace(FbxCurve^ source, FbxTime^ start,FbxTime^ stop, bool useExactGivenSpan, bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset);

			/**	Replace keys within a range in current function curve with keys found in a source function curve.
			* The copied keys have their value scaled with a factor varying 
			* linearly in time within the given time range.
			* \param pSource Source function curve.
			* \param pStart Start of time range.
			* \param pStop End of time range.
			* \param pScaleStart Scale factor applied at start of time range. 
			* \param pScaleStop Scale factor applied at end of time range. 
			* \param pUseExactGivenSpan false = original behavior where time of first and last key was used
			* \param pKeyStartEndOnNoKey Inserts a key at the beginning and at the end of the range if there is no key to insert.
			* \param pTimeSpanOffset
			*/
			void ReplaceForQuaternion(FbxCurve^ source, FbxTime^ start, FbxTime^ stop, kFCurveDouble scaleStart, kFCurveDouble scaleStop, bool useExactGivenSpan, bool keyStartEndOnNoKey,FbxTime^ timeSpanOffset);

			/**	Replace keys within a range in current function curve with keys found in a source function curve.
			* \param pSource Source function curve.
			* \param pStart Start of time range.
			* \param pStop End of time range.
			* \param pAddFromStart Offset applied to copied key values within the time range.
			* \param pAddAfterStop Offset applied to key values after the time range.
			* \param pValueSubOffsetAfterStart If \c true, copied key values within 
			* the time range are substracted from time offset specified by parameter
			* \c pAddFromStart. If \c false, copied key values within the time range are 
			* added to time offset specified by parameter \c pAddFromStart. 
			* \param pValueSubOffsetAfterStop If \c true, key values after 
			* the time range are substracted from time offset specified by parameter
			* \c pAddAfterStop. If \c false, key values after the time range are 
			* added to time offset specified by parameter \c pAddAfterStop. 
			* \param pUseExactGivenSpan false = original behavior where time of first and last key was used
			* \param pKeyStartEndOnNoKey Inserts a key at the beginning and at the end of the range if there is no key to insert
			* \param pTimeSpanOffset
			*/
			void ReplaceForEulerXYZ(FbxCurve^ source,FbxTime^ start, FbxTime^ stop, kFCurveDouble addFromStart, kFCurveDouble addAfterStop, bool valueSubOffsetAfterStart, bool valueSubOffsetAfterStop, bool useExactGivenSpan, bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset);	

			/**	Insert all keys found in a source function curve in current function curve.
			* A time offset is added to copied keys so that the first copied key occurs 
			* at the given insertion time. Keys from the source function curve are merged into 
			* the current function curve. In other words, no existing key in the current function
			* curve is destroyed unless there is an overlap with a copied key.
			* \param pSource Source function curve.
			* \param pInsertTime Insert time of the first key found in the source function curve.
			* \param pFirstKeyLeftDerivative First key left derivative.
			* \param pFirstKeyIsWeighted  First key left weighted state (true if weighted).
			* \param pFirstKeyWeight First key left weight
			*/

			void Insert(FbxCurve^ source, FbxTime^ insertTime, kFCurveDouble firstKeyLeftDerivative, bool firstKeyIsWeighted, kFCurveDouble firstKeyWeight);

			/**	Insert all keys found in a source function curve in current function curve.
			* A time offset is added to copied keys so that the first copied key occurs 
			* at the given insertion time. Keys from the source function curve are merged into 
			* the current function curve. In other words, no existing key in the current function
			* curve is destroyed unless there is an overlap with a copied key.
			* \param pSource Source function curve.
			* \param pInsertTime Insert time of the first key found in the source function curve.
			* \param pFirstKeyLeftDerivative First key left derivative info.
			*/
			void Insert(FbxCurve^ source, FbxTime^ insertTime, FbxCurveTangentInfo^ firstKeyLeftDerivative );

			/** Delete keys within an index range.
			* Index range is inclusive.
			* This function is much faster than multiple removes.
			*	\param pStartIndex Index of first deleted key.
			*	\param pStopIndex Index of last deleted key.
			*	\return \c true if the function curve contains keys, \c false otherwise.
			* \remarks Result is undetermined if function curve has keys but an 
			* index is out of bounds.
			*/
			bool Delete(kFCurveIndex startIndex , kFCurveIndex stopIndex);									

			/** Delete keys within a time range.
			* Time range is inclusive.
			* This function is much faster than multiple removes.
			*	\param pStart Start of time range.
			*	\param pStop End of time range.
			*	\return \c true if the function curve contains keys, \c false otherwise.
			*/	
			bool Delete(FbxTime^ start, FbxTime^ stop);
			bool Delete(FbxTime^ start);
			bool Delete();

			/** Get if interpolation is cubic and that the tangents and weightings are untouched.
			*	\param	pKeyIndex	Index of the key to test.
			*	\return				Returns true if the interpolation is a pure cubic auto.
			*/
			bool IsKeyInterpolationPureCubicAuto(kFCurveIndex keyIndex);

#ifndef K_PLUGIN
			/** Extract All Keys in the Given Selection Span
			*	\param	pArray	    Array where to Stored Found Keys.
			*	\param	pMinIndex	Index where to start the Search.
			*	\param	pMaxIndex	Index where to stop the Search (the last index is the limit, the Key at this index is not tested).
			*	\param	pMinValue	Minimal Value to Consider the Key.
			*	\param	pMaxValue	Maximal Value to Consider the Key.
			*/
			void ExtractKeysIndex( KArraykInt &pArray, int pMinIndex, int pMaxIndex, double pMinValue =  -K_DOUBLE_MAX, double pMaxValue =  K_DOUBLE_MAX);
#endif

			//@}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			/*bool	FbxStore (KFbx* pFbx, bool pOnlyDefaults = false, bool pColor = true, bool pIsVersion5 = false );
			bool	FbxRetrieve (KFbx* pFbx, bool pOnlyDefaults = false, bool pColor = false );
			bool	FbxInternalRetrieve (KFbx* pFbx, bool pOnlyDefaults = false, bool pColor = false );

			double CandidateEvaluate (KTime pTime, kFCurveIndex* pLast = NULL);
			bool CandidateClear ();
			bool CandidateSet (KTime pTime, double pValue);
			bool IsCandidate ();
			double CandidateGet ();
			KTime CandidateGetTime ();

			bool CandidateKey
			(
			kFCurveIndex	*pLast				= NULL, 
			int	pInterpolation = KFCURVE_INTERPOLATION_CUBIC, 
			int	pTanMode = KFCURVE_TANGEANT_USER, 
			int pContinuity = KFCURVE_CONTINUITY,
			bool			pTangentOverride	= true,
			KTime			pCandidateTime		= KTIME_INFINITE,
			double			pKeyIndexTolerance  = 0.0
			);

			bool NormalsSeemsToComeFromAPlot();

			void SetWasData (int pType);
			int GetWasData ();
			int GuessWasData (KTime* pStart = NULL, KTime* pStep = NULL);

			void KeyTangentHide ();

			int GetUpdateId ();
			int GetValuesUpdateId ();

			void CallbackRegister (kFCurveCallback pCallback, void* pObject);
			void CallbackUnregister (kFCurveCallback pCallback, void* pObject);
			void CallbackEnable (bool pEnable);
			void CallbackClear ();		*/
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};



	}
}