#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{	
		ref class FbxSceneManaged;
		ref class FbxNode;
		/**	\brief This class is used to describe the units of measurement used within a particular scene.
		* \nosubgrouping
		*/
		public ref class FbxSystemUnit : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxSystemUnit,KFbxSystemUnit);
			INATIVEPOINTER_DECLARE(FbxSystemUnit,KFbxSystemUnit);
		internal:
			FbxSystemUnit(KFbxSystemUnit u)
			{
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = u;
			}
		public:

			/** Defines various options that can be set for converting the units of a scene
			*/
			value struct FbxUnitConversionOptions
			{
			public:
				/**< Convert the intensity property of lights. */
				bool ConvertLightIntensity;
				/**< Convert the nodes that do not inheirit their parent's scale */
				bool ConvertRrsNodes;  
			};

			/** Constructor
			* \param pScaleFactor The equivalent number of centimeters in the new system unit. 
			*                     eg For an inch unit, use a scale factor of 2.54
			* \param pMultiplier  A multiplier factor of pScaleFactor.
			*/
			FbxSystemUnit(double scaleFactor, double multiplier);
			FbxSystemUnit(double scaleFactor);
			
			enum class PredefinedSystemUnit
			{
				Mm,Dm,Cm,M,Km,Inch,Foot,Mile,Yard
			};

			//FbxSystemUnit(PredefinedSystemUnit type);

			//static const KFbxSystemUnit *sPredefinedUnits; // points to an array of KFbxSystemUnit_sPredifinedUnitCount size

			static property FbxSystemUnit::FbxUnitConversionOptions DefaultConversionOptions
			{
				FbxSystemUnit::FbxUnitConversionOptions get();
			}

			/** Convert a scene from its system units to this unit.
			* \param pScene The scene to convert
			* \param pOptions Various conversion options. See KFbxSystemUnit::KFbxUnitConversionOptions
			*/
			void ConvertScene(FbxSceneManaged^ scene, FbxUnitConversionOptions options);

			/** Converts the children of the given node to this system unit.
			* Unlike the ConvertScene() method, this method does not set the axis system 
			* of the scene that the pRoot node belongs, nor does it adjust KFbxPoses
			* as they are not stored under the scene, and not under a particular node.
			*/
			void ConvertChildren(FbxNode^ root, FbxSystemUnit^ srcUnit,FbxUnitConversionOptions options);

			/** Convert a scene from its system units to this unit, using the specified 
			* Fbx_Root node. This method is provided for backwards compatibility only
			* and ConvertScene( KFbxScene* , const KFbxUnitConversionOptions&  ) should 
			* be used instead whenever possible.
			* \param pScene The scene to convert
			* \param pFbxRoot The Fbx_Root node to use in conversion
			* \param pOptions Conversion options. See KFbxSystemUnit::KFbxUnitConversionOptions
			*/
			void ConvertScene(FbxSceneManaged^ scene, FbxNode^ root,FbxUnitConversionOptions options);

			/** Gets the scale factor of this system unit, relative to centimeters.
			* This factor scales values in system units to centimeters.
			* For the purpose of scaling values to centimeters, this value should be used
			* and the "multiplier" (returned by GetMultiplier()) should be ignored.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,ScaleFactor);

			/** Returns a unit label for the current scale factor.
			*/
			String^ GetScaleFactorAsString(bool abbreviated);
			String^ GetScaleFactorAsString()
			{
				return GetScaleFactorAsString(true);
			}

			/** Returns a unit label for the current scale factor. Capital first letter + "s" added + foot -> feet
			*/
			String^ GetScaleFactorAsString_Plurial();

			/** Gets the multiplier factor of this system unit.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,Multiplier);

			virtual bool Equals(System::Object^ obj) override
			{
				FbxSystemUnit^ u = dynamic_cast<FbxSystemUnit^>(obj);
				if(u)
					return *u->_Ref() == *_Ref();
				return false;
			}			

			/** Returns the conversion factor from this unit to pTarget (does not include the muliplier factor).
			*/
			double GetConversionFactorTo(FbxSystemUnit^ target);

			/** Returns the conversion factor from pSource to this unit
			*/
			double GetConversionFactorFrom(FbxSystemUnit^ source);


		};

	}
}