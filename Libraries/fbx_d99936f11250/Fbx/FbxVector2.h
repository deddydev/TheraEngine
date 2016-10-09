#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		/**	FBX SDK 2-elements vector class.
		* \nosubgrouping
		*/
		public ref class FbxVector2 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxVector2,KFbxVector2);
			INATIVEPOINTER_DECLARE(FbxVector2,KFbxVector2);
		internal:			
			FbxVector2(KFbxVector2 v)
			{
				_SetPointer(new KFbxVector2(),true);
				*_FbxVector2 = v;
			}

		public:

			property double X {	double get(); void set(double value);}
			property double Y {	double get(); void set(double value);}

			/**
			* \name Constructors and Destructor
			*/
			//@{

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxVector2,KFbxVector2);

			//! Copy constructor.
			FbxVector2(FbxVector2^ vector2);

			/** Constructor.
			*	\param pX X component.
			*	\param pY Y component.
			*/
			FbxVector2(double x, double y);		
			//@}

			/**
			* \name Access
			*/
			//@{

			//! Assignment operation.
			void CopyFrom(FbxVector2^ other);

			/** Accessor.
			* \param pIndex The index of the component to access.
			* \return The reference to the indexed component.
			* \remarks          The pIndex parameter is not checked for values out of bounds. The valid values are 0 and 1.
			*/
			property double default[int]
			{
				double get(int index);
				void set(int index,double value);
			}

			/** Get a vector element.
			* \param pIndex The index of the component to access.
			* \return The value of the indexed component.
			* \remarks          The pIndex parameter is not checked for values out of bounds. The valid values are 0 and 1.
			*/
			double GetAt(int index);	

			/** Set a vector element.
			* \param pIndex The index of the component to set.
			* \param pValue The new value to set the component.
			* \remarks          The pIndex parameter is not checked for values out of bounds. The valid values are 0 and 1.
			*/
			void SetAt(int index, double value);

			/** Set vector.
			* \param pX The X component value.
			* \param pY The Y component value.
			*/
			void Set(double x, double y);

			//@}

			/**
			* \name Scalar Operations
			*/
			//@{

			/** Add a value to all vector components.
			* \param pValue The value to add to each component of the vector.
			* \return           A new vector with the result of adding pValue to each component of this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator + (FbxVector2^ v,double value)
			{
				return gcnew FbxVector2(*v->_Ref() + value);
			}

			/** Subtract a value from all vector components.
			* \param pValue The value to subtract from each component of the vector.
			* \return           A new vector with the result of subtracting pValue from each component of this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator -(FbxVector2^ v,double value)
			{
				return gcnew FbxVector2(*v->_Ref() - value);
			}

			/** Multiply a value to all vector components.
			* \param pValue The value multiplying each component of the vector.
			* \return           A new vector with the result of multiplying each component of this vector by pValue.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator *(FbxVector2^ v,double value)
			{
				return gcnew FbxVector2(*v->_Ref() * value);
			}

			/**	Divide all vector components by a value.
			* \param pValue The value dividing each component of the vector.
			* \return           A new vector with the result of dividing each component of this vector by pValue.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator /(FbxVector2^ v,double value)
			{
				return gcnew FbxVector2(*v->_Ref() / value);
			}

			/** Add a value to all vector components.
			* \param pValue The value to add to each component of the vector.
			* \return           The result of adding pValue to each component of this vector, replacing this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator +=(FbxVector2^ v,double value)
			{
				*v->_Ref() += value;
				return v;
			}

			/** Subtract a value from all vector components.
			* \param pValue The value to subtract from each component of the vector.
			* \return           The result of subtracting pValue from each component of this vector, replacing this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator -=(FbxVector2^ v,double value)
			{
				*v->_Ref() -= value;
				return v;
			}

			/** Multiply a value to all vector elements.
			* \param pValue The value multiplying each component of the vector.
			* \return           The result of multiplying each component of this vector by pValue, replacing this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator *=(FbxVector2^ v,double value)
			{
				*v->_Ref() *= value;
				return v;
			}

			/**	Divide all vector elements by a value.
			* \param pValue The value dividing each component of the vector.
			* \return           The result of multiplying each component of this vector by pValue, replacing this vector.
			* \remarks          The pValue parameter is not checked.
			*/
			static FbxVector2^ operator /=(FbxVector2^ v,double value)
			{
				*v->_Ref() /= value;
				return v;
			}

			//@}

			/**
			* \name Vector Operations
			*/
			//@{

			/**	Unary minus operator.
			* \return The vector that is the negation of \c this.
			*/
			FbxVector2^ Negate();

			/** Add two vectors together.
			* \param pVector Vector to add.
			* \return            The result of this vector + pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator +(FbxVector2^ v1,FbxVector2^ v2)
			{
				return gcnew FbxVector2(*v1->_Ref() + *v2->_Ref());
			}

			/** Subtract a vector from another vector.
			* \param pVector Vector to subtract.
			* \return            The result of this vector - pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator -(FbxVector2^ v1,FbxVector2^ v2)
			{
				return gcnew FbxVector2(*v1->_Ref() - *v2->_Ref());
			}

			/** Memberwise multiplication of two vectors.
			* \param pVector      Multiplying vector.
			* \return             The result of this vector * pVector.
			* \remarks            The values in pVector are not checked.
			*/
			static FbxVector2^ operator *(FbxVector2^ v1,FbxVector2^ v2)
			{
				return gcnew FbxVector2(*v1->_Ref() * *v2->_Ref());
			}

			/** Memberwise division of a vector with another vector.
			* \param pVector     Dividing vector.
			* \return            The result of this vector / pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator /(FbxVector2^ v1,FbxVector2^ v2)
			{
				return gcnew FbxVector2(*v1->_Ref() / *v2->_Ref());
			}

			/** Add two vectors together.
			* \param pVector Vector to add.
			* \return            The result of this vector + pVector, replacing this vector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator +=(FbxVector2^ v1,FbxVector2^ v2)
			{
				*v1->_Ref() += *v2->_Ref();
				return v1;
			}

			/** Subtract a vector from another vector.
			* \param pVector Vector to subtract.
			* \return            The result of this vector - pVector, replacing this vector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator -=(FbxVector2^ v1,FbxVector2^ v2)
			{
				*v1->_Ref() -= *v2->_Ref();
				return v1;
			}

			/** Memberwise multiplication of two vectors.
			* \param pVector Multiplying vector.
			* \return            The result of this vector * pVector, replacing this vector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator *=(FbxVector2^ v1,FbxVector2^ v2)
			{
				*v1->_Ref() *= *v2->_Ref();
				return v1;
			}

			/** Memberwise division of a vector with another vector.
			* \param pVector Dividing vector.
			* \remarks           The values in pVector are not checked.
			* \return            The result of this vector / pVector, replacing this vector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector2^ operator /=(FbxVector2^ v1,FbxVector2^ v2)
			{
				*v1->_Ref() /= *v2->_Ref();
				return v1;
			}

			/** Calculate the dot product of two vectors.
			* \param pVector The second vector.
			* \return The dot product value.
			* \remarks          pVector is considered a XYZ vector with fourth weight element, so only the first 3 elements are considered.
			*/
			double DotProduct(FbxVector2^ vector);

			//@}

			/**
			* \name Boolean Operations
			*/
			//@{

			/**	Equivalence operator.
			* \param pVector The vector to be compared to \e this.
			* \return            \c true if the two vectors are equal (each element is within a 1.0e-6 tolerance), \c false otherwise.
			*/
			virtual bool Equals(Object^ obj) override
			{
				FbxVector2^ o = dynamic_cast<FbxVector2^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}			

			/**	Non-equivalence operator.
			* \param pVector The vector to be compared to \e this.
			* \return            \c false if the two vectors are equal (each element is within a 1.0e-6 tolerance), \c true otherwise.
			*/			

			//@}

			/**
			* \name Length
			*/
			//@{

			/** Get the vector's length.
			* \return The mathematical length of the vector.
			*/
			double Length();

			/** Get the vector's length squared.
			* \return The mathematical square length of the vector.
			*/
			double SquareLength();

			/** Find the distance between 2 vectors.
			* \param pVector The second vector.
			* \return The mathematical distance between the two vectors.
			*/
			double Distance(FbxVector2^ vector);

			//! Normalize the vector, length set to 1.
			void Normalize();

			virtual String^ ToString() override
			{
				return String::Format("X: {0}, Y: {1}",X,Y);				
			}

			//@}

			/**
			* \name Casting
			*/
			//@{

			//! Cast the vector in a double pointer.
			//operator double* ();

			//! Cast the vector in a const double pointer.
			//operator const double* ();

			//operator const double* () const;

			//@}

		};

	}
}