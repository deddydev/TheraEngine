#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		ref class FbxDouble3;
		/**	FBX SDK 4-elements vector class.
		* \nosubgrouping
		*/
		public ref class FbxVector4 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxVector4,KFbxVector4);
			INATIVEPOINTER_DECLARE(FbxVector4,KFbxVector4);
		internal:
			FbxVector4(KFbxVector4 v)
			{
				_SetPointer(new KFbxVector4(),true);
				*_FbxVector4 = v;
			}
		public:

			/**
			* \name Constructors and Destructor
			*/
			//@{

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxVector4,KFbxVector4);			

			//! Copy constructor.
			FbxVector4(FbxVector4^ vector4);

			/** Constructor.
			*	\param pX X component.
			*	\param pY Y component.
			*	\param pZ Z component.
			*	\param pW W component.
			*/
			FbxVector4(double x, double y, double z, double w);
			FbxVector4(double x, double y, double z);

			property double X {	double get(); void set(double value);}
			property double Y {	double get(); void set(double value);}
			property double Z {	double get(); void set(double value);}
			property double W {	double get(); void set(double value);}

			/** Constructor.
			*	\param pValue X,Y,Z,W components.
			*/
			//KFbxVector4(const double pValue[4]);

			/** Constructor.
			* \param pValue X,Y,Z components.
			* \remarks The fourth component of this object is assigned 1.
			*/
			FbxVector4(FbxDouble3^ value);
			

			//@}

			/**
			* \name Access
			*/
			//@{

			//! Assignment operation.
			void CopyFrom(FbxVector4^ v);

			//! Assignment operation.
			//KFbxVector4& operator=(const double* pValue);

			//! Assignment operation.
			//KFbxVector4& operator=(const fbxDouble3& pValue);

			/** Accessor.
			* \param pIndex The index of the component to access.
			* \return The reference to the indexed component.
			* \remarks          The parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			//double& operator[](int pIndex);
			//double const& operator[](int pIndex) const;

			/** Get a vector element.
			* \param pIndex The index of the component to access.
			* \return The value of the indexed component.
			* \remarks          The parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			double GetAt(int index);

			property double default[int]
			{
				double get(int index);
				void set(int index , double value);
			}

			/** Set a vector element.
			* \param pIndex The index of the component to set.
			* \param pValue The new value to set the component.
			* \remarks          The index parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			void SetAt(int index , double value);

			/** Set vector.
			* \param pX The X component value.
			* \param pY The Y component value.
			* \param pZ The Z component value.
			* \param pW The W component value.
			*/
			void Set(double x, double y, double z, double w);
			void Set(double x, double y, double z);

			//@}

			/**
			* \name Scalar Operations
			*/
			//@{

			/** Add a value to all vector components.
			* \param pValue The value to add to each component of the vector.
			* \return New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxVector4^ operator+(FbxVector4^ v,double value)
			{
				KFbxVector4 v1 = *v->_Ref() + value;
				return gcnew FbxVector4(v1);
			}

			/** Substract a value from all vector components.
			* \param pValue The value to substract from each component of the vector.
			* \return New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxVector4^ operator-(FbxVector4^ v,double value)
			{
				KFbxVector4 v1 = *v->_Ref() - value;
				return gcnew FbxVector4(v1);
			}
			/** Multiply a value to all vector components.
			* \param pValue The value multiplying each component of the vector.
			* \return New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxVector4^ operator*(FbxVector4^ v,double value)
			{
				KFbxVector4 v1 = *v->_Ref() * value;
				return gcnew FbxVector4(v1);
			}

			/**	Divide all vector components by a value.
			* \param pValue The value dividing each component of the vector.
			* \return New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxVector4^ operator/(FbxVector4^ v,double value)
			{
				KFbxVector4 v1 = *v->_Ref() / value;
				return gcnew FbxVector4(v1);
			}

			/** Add a value to all vector components.
			* \param pValue The value to add to each component of the vector.
			* \return \e this updated with the operation result.
			* \remarks          The passed value is not checked.
			*/
			//KFbxVector4& operator+=(double pValue);

			/** Subtract a value from all vector components.
			* \param pValue The value to subtract from each component of the vector.
			* \return \e this updated with the operation result.
			* \remarks          The passed value is not checked.
			*/
			//KFbxVector4& operator-=(double pValue);

			/** Multiply a value to all vector elements.
			* \param pValue The value multiplying each component of the vector.
			* \return \e this updated with the operation result.
			* \remarks          The passed value is not checked.
			*/
			//KFbxVector4& operator*=(double pValue);

			/**	Divide all vector elements by a value.
			* \param pValue The value dividing each component of the vector.
			* \return \e this updated with the operation result.
			* \remarks          The passed value is not checked.
			*/
			//KFbxVector4& operator/=(double pValue);

			//@}

			/**
			* \name Vector Operations
			*/
			//@{

			/**	Unary minus operator.
			* \return The vector that is the negation of \c this.
			*/
			FbxVector4^ Negate();

			/** Add two vectors together.
			* \param pVector Vector to add.
			* \return The vector v' = this + pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector4^ operator+(FbxVector4^ v1,FbxVector4^ v2)
			{
				KFbxVector4 v = *v1->_Ref() + *v2->_Ref();
				return gcnew FbxVector4(v);
			}

			/** Subtract a vector from another vector.
			* \param pVector Vector to subtract.
			* \return The vector v' = this - pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector4^ operator-(FbxVector4^ v1,FbxVector4^ v2)
			{
				KFbxVector4 v = *v1->_Ref() - *v2->_Ref();
				return gcnew FbxVector4(v);
			}

			/** Memberwise multiplication of two vectors.
			* \param pVector Multiplying vector.
			* \return The vector v' = this * pVector.
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector4^ operator*(FbxVector4^ v1,FbxVector4^ v2)
			{
				KFbxVector4 v = *v1->_Ref() * *v2->_Ref();
				return gcnew FbxVector4(v);
			}

			/** Memberwise division of a vector with another vector.
			* \param pVector Dividing vector.
			* \return The vector v[i]' = this[i] / pVector[i].
			* \remarks           The values in pVector are not checked.
			*/
			static FbxVector4^ operator/(FbxVector4^ v1,FbxVector4^ v2)
			{
				KFbxVector4 v = *v1->_Ref() / *v2->_Ref();
				return gcnew FbxVector4(v);
			}

			/** Add two vectors together.
			* \param pVector Vector to add.
			* \return \e this updated with the operation result.
			* \remarks           The values in pVector are not checked.
			*/
			//KFbxVector4& operator+=(KFbxVector4& pVector);

			/** Subtract a vector from another vector.
			* \param pVector Vector to subtract.
			* \return \e this updated with the operation result.
			* \remarks           The values in pVector are not checked.
			*/
			//KFbxVector4& operator-=(KFbxVector4& pVector);

			/** Memberwise multiplication of two vectors.
			* \param pVector Multiplying vector.
			* \return \e this updated with the operation result.
			* \remarks           The values in pVector are not checked.
			*/
			//KFbxVector4& operator*=(KFbxVector4& pVector);

			/** Memberwise division of a vector with another vector.
			* \param pVector Dividing vector.
			* \return \e this updated with the operation result.
			* \remarks           The values in pVector are not checked.
			*/
			//KFbxVector4& operator/=(KFbxVector4& pVector);

			/** Calculate the dot product of two vectors.
			* \param pVector The second vector.
			* \return The dot product value.
			* \remarks           Being considered as a XYZ vector with a weight, only the 3 first elements are considered in this operation.
			*/
			double DotProduct(FbxVector4^ vector);

			/** Calculate the cross product of two vectors.
			* \param pVector The second vector.
			* \return The cross product vector.
			* \remarks           Being considered as a XYZ vector with a weight, only the first 3 elements are considered in this operation.
			*/
			FbxVector4^ CrossProduct(FbxVector4^ vector);

			/** Calculate the Euler rotation required to align axis pAB-pA on pAB-pB.
			*	\param pAB The intersection of the 2 axis.
			*	\param pA A point on axis to be aligned.
			*	\param pB A point on reference axis.
			*	\param pAngles Resulting euler angles.
			*	\return \c true on success.
			* \remarks           Being considered as a XYZ vector with a weight, only the first 3 elements are considered in this operation.
			*/
			static bool AxisAlignmentInEulerAngle(FbxVector4^ AB, 
				FbxVector4^ A, 
				FbxVector4^ B, 
				FbxVector4^ angles);
			

			/**
			* \name Boolean Operations
			*/
			//@{

			/**	Equivalence operator.
			* \param pVector The vector to be compared to \e this.
			* \return            \c true if the two vectors are equal (each element is within a 1.0e-6 tolerance) and \c false otherwise.
			*/
			virtual bool Equals(Object^ obj) override
			{
				FbxVector4^ o = dynamic_cast<FbxVector4^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}			


			/**
			* \name Length
			*/
			//@{

			/** Get the vector's length.
			* \return The mathematical length of the vector.
			* \remarks     Being considered as a XYZ vector with a weight, only the first 3 elements are considered in this operation.
			*/
			double Length();

			/** Get the vector's length squared.
			* \return The mathematical square length of the vector.
			* \remarks     Being considered as a XYZ vector with a weight, only the first 3 elements are considered in this operation.
			*/
			double SquareLength();

			/** Find the distance between 2 vectors.
			* \param pVector The second vector.
			* \return The mathematical distance between the two vectors.
			* \remarks           Being considered as a XYZ vector with a weight, only the 3 first elements are considered in this operation.
			*/
			double Distance(FbxVector4^ vector);

			/** Normalize the vector, length set to 1.
			* \remarks     Being considered as a XYZ vector with a weight, only the first 3 elements are considered in this operation.
			*/
			void Normalize();

			virtual String^ ToString() override
			{
				return String::Format("X: {0}, Y: {1}, Z: {2}, W: {3}",X,Y,Z,W);				
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

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS	



#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}