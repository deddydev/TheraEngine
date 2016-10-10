#pragma once
#include "stdafx.h"
#include "Fbx.h"



{
	namespace FbxSDK
	{
		/** Simple class to hold RGBA values.
		* \nosubgrouping
		*/
		public ref class FbxColor : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxColor,KFbxColor);			
			GET_NATIVEPOINTER_DECLARE_BASE(FbxColor,KFbxColor);			
			IS_SAME_AS_DECLARE_BASE();
		internal:
			FbxColor(KFbxColor c)
			{
				_SetPointer(new KFbxColor(),true);
				*_FbxColor = c;
			}
		public:

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxColor,KFbxColor);			

			/** Constructor.
			* \param pRed       The Red component value.
			* \param pGreen     The Green component value.
			* \param pBlue      The Blue component value.
			* \param pAlpha     The alpha value of the color.
			*/
			FbxColor(double red, double green, double blue, double alpha);
			FbxColor(double red, double green, double blue);

			/** Re-initialize the color object with their new values.
			* \param pRed       The Red component value.
			* \param pGreen     The Green component value.
			* \param pBlue      The Blue component value.
			* \param pAlpha     The alpha value of the color.
			*/
			void Set(double red, double green, double blue, double alpha);
			void Set(double red, double green, double blue);

			/** Indicate if all the members in the color objects are within their valid range.
			* \return     \c true if all the members are within their valid range.
			*/
			virtual property bool IsValid
			{
				bool get();
			}

			/**
			* \name Operators
			*/
			//@{

			//! Assignment operator.
			//KFbxColor& operator=(const KFbxColor& pColor);
			void Set(FbxColor^ color);

			//! Equality operator.
			virtual bool Equals(Object^ obj) override
			{
				FbxColor^ o = dynamic_cast<FbxColor^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}			
			//@}

			/**
			* name Public Members
			*/
			//@{

			//! Valid range is from 0.0 to 1.0.
			property double Red
			{
				double get();
				void set(double value);
			}

			//! Valid range is from 0.0 to 1.0.				
			property double Green
			{
				double get();
				void set(double value);
			}

			//! Valid range is from 0.0 to 1.0.				
			property double Blue
			{
				double get();
				void set(double value);
			}

			//! Valid range is from 0.0 to 1.0.				
			property double Alpha
			{
				double get();
				void set(double value);
			}	

			virtual String^ ToString() override
			{
				return String::Format("Red: {0:F}, Green: {1:F}, Blue:{2:F}, Alpha:{3:F}",Red,Green,Blue,Alpha);
			}
		};

	}
}