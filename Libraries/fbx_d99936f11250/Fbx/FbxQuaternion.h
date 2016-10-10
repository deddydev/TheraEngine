#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		ref class FbxVector4;
		/**	FBX SDK quaternion class.
		* \nosubgrouping
		*/
		public ref class FbxQuaternion : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxQuaternion,KFbxQuaternion);
			INATIVEPOINTER_DECLARE(FbxQuaternion,KFbxQuaternion);
		internal:
			FbxQuaternion(KFbxQuaternion q)
			{
				_SetPointer(new KFbxQuaternion(),true);
				*_FbxQuaternion = q;
			}
		public:

			/**
			* \name Constructors and Destructor
			*/
			//@{

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxQuaternion,KFbxQuaternion);

			//! Copy constructor.
			FbxQuaternion(FbxQuaternion^ q);

			/** Constructor.
			* \param pX     The X component.
			* \param pY     The Y component.
			* \param pZ     The Z component.
			* \param pW     The W component.
			*/
			FbxQuaternion(double x, double y, double z, double w);
			FbxQuaternion(double x, double y, double z);

			/**
			* \name Access
			*/
			//@{

			//! Assignment operation.
			void CopyFrom(FbxQuaternion^ q);

			/** Accessor.
			* \param pIndex     The index of the component to access.
			* \return           The reference to the indexed component.
			* \remarks          The index parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			//double& operator[](int pIndex);

			/** Get a vector element.
			* \param pIndex     The index of the component to access.
			* \return           The value of the indexed component.
			* \remarks          The index parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			double GetAt(int index);

			/** Set a vector element.
			* \param pIndex     The index of the component to set.
			* \param pValue     The new value to set the component.
			* \remarks          The index parameter is not checked for values out of bounds. The valid range is [0,3].
			*/
			void SetAt(int index, double value);

			property double default[int]
			{
				double get(int index);
				void set(int index , double value);
			}

			property double X {double get(); void set(double value);}
			property double Y {double get(); void set(double value);}
			property double Z {double get(); void set(double value);}
			property double W {double get(); void set(double value);}

			/** Set vector.
			* \param pX     The X component value.
			* \param pY     The Y component value.
			* \param pZ     The Z component value.
			* \param pW     The W component value.
			*/
			void Set(double x, double y, double z, double w );
			void Set(double x, double y, double z);

			//@}

			/**
			* \name Scalar Operations
			*/
			//@{

			/** Add a value to all vector components.
			* \param pValue     The value to add to each component of the vector.
			* \return           New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator +(FbxQuaternion^ q,double value)
			{
				KFbxQuaternion q1 = *q->_Ref() + value;
				return gcnew FbxQuaternion(q1);
			}

			/** Subtract a value from all vector components.
			* \param pValue     The value to subtract from each component of the vector.
			* \return           New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator -(FbxQuaternion^ q,double value)
			{
				KFbxQuaternion q1 = *q->_Ref() - value;
				return gcnew FbxQuaternion(q1);
			}

			/** Multiply all vector components by a value.
			* \param pValue     The value multiplying each component of the vector.
			* \return           New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator *(FbxQuaternion^ q,double value)
			{
				KFbxQuaternion q1 = *q->_Ref() * value;
				return gcnew FbxQuaternion(q1);
			}

			/**	Divide all vector components by a value.
			* \param pValue     The value dividing each component of the vector.
			* \return           New vector.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator /(FbxQuaternion^ q,double value)
			{
				KFbxQuaternion q1 = *q->_Ref() / value;
				return gcnew FbxQuaternion(q1);
			}

			/** Add a value to all vector components.
			* \param pValue     The value to add to each component of the vector.
			* \return           The result of adding pValue to each component of the vector, replacing this quaternion.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator +=(FbxQuaternion^ q,double value)
			{
				*q->_Ref() += value;
				return q;
			}

			/** Subtract a value from all vector components.
			* \param pValue     The value to subtract from each component of the vector.
			* \return           The result of subtracting pValue from each component of the vector, replacing this quaternion.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator -=(FbxQuaternion^ q,double value)
			{
				*q->_Ref() -= value;
				return q;
			}

			/** Multiply a value to all vector elements.
			* \param pValue     The value multiplying each component of the vector.
			* \return           The result of multiplying each component of the vector by pValue, replacing this quaternion.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator *=(FbxQuaternion^ q,double value)
			{
				*q->_Ref() *= value;
				return q;
			}

			/**	Divide all vector elements by a value.
			* \param pValue     The value dividing each component of the vector.
			* \return           The result of dividing each component of the vector by pValue, replacing this quaternion.
			* \remarks          The passed value is not checked.
			*/
			static FbxQuaternion^ operator /=(FbxQuaternion^ q,double value)
			{
				*q->_Ref() /= value;
				return q;
			}

			//@}

			/**
			* \name Vector Operations
			*/
			//@{

			/**	Unary minus operator.
			* \return      A quaternion where each component is multiplied by -1.
			*/
			FbxQuaternion^ Negate();

			/** Add two vectors together.
			* \param pQuaternion     Quaternion to add.
			* \return                The quaternion v' = this + pQuaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator +(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				KFbxQuaternion q = *q1->_Ref() + *q2->_Ref();
				return gcnew FbxQuaternion(q);
			}

			/** Subtract a quaternion from another quaternion.
			* \param pQuaternion     Quaternion to subtract.
			* \return                The quaternion v' = this - pQuaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator -(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				KFbxQuaternion q = *q1->_Ref() - *q2->_Ref();
				return gcnew FbxQuaternion(q);
			}

			/** Memberwise multiplication of two vectors.
			* \param pQuaternion     Multiplying quaternion.
			* \return                The quaternion v' = this * pQuaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator *(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				KFbxQuaternion q = *q1->_Ref() * *q2->_Ref();
				return gcnew FbxQuaternion(q);
			}

			/** Memberwise division of a quaternion with another quaternion.
			* \param pQuaternion     Dividing quaternion.
			* \return                The quaternion v' = this / pQuaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator /(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				KFbxQuaternion q = *q1->_Ref() / *q2->_Ref();
				return gcnew FbxQuaternion(q);
			}

			/** Add two quaternions together.
			* \param pQuaternion     Quaternion to add.
			* \return                The quaternion v' = this + pQuaternion, replacing this quaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator +=(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				*q1->_Ref() += *q2->_Ref();
				return q1;
			}

			/** Subtract a quaternion from another vector.
			* \param pQuaternion     Quaternion to subtract.
			* \return                The quaternion v' = this - pQuaternion, replacing this quaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator -=(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				*q1->_Ref() -= *q2->_Ref();
				return q1;
			}

			/** Memberwise multiplication of two quaternions.
			* \param pQuaternion     Multiplying quaternion.
			* \return                The quaternion v' = this * pQuaternion, replacing this quaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator *=(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				*q1->_Ref() *= *q2->_Ref();
				return q1;
			}

			/** Memberwise division of a quaternion by another quaternion.
			* \param pQuaternion     Dividing quaternion.
			* \return                The quaternion v' = this / pQuaternion, replacing this quaternion.
			* \remarks               The values in pQuaternion are not checked.
			*/
			static FbxQuaternion^ operator /=(FbxQuaternion^ q1,FbxQuaternion^ q2)
			{
				*q1->_Ref() /= *q2->_Ref();
				return q1;
			}

			/** Return quaternion product.
			* \param pQuaternion     product quaternion.
			* \return                This quarternion replace with the quaternion product.
			*/
			FbxQuaternion^ Product(FbxQuaternion^ q);

			/** Normalize the quaternion, length set to 1.
			*/
			void Normalize();

			/** Calculate the quaternion inverse.
			* \return      The inverse of this quaternion. Set XYZ at -XYZ.
			*/
			void Conjugate();

			void ComposeSphericalXYZ(FbxVector4^ euler);
			FbxVector4^ DecomposeSphericalXYZ();

			//@}

			/**
			* \name Boolean Operations
			*/
			//@{

			/**	Equivalence operator.
			* \param pV     The quaternion to be compared to this quarternion.
			* \return       \c true if the two quaternions are equal (each element is within a 1.0e-6 tolerance), \c false otherwise.
			*/
			virtual bool Equals(System::Object^ obj) override
			{
				FbxQuaternion^ q = dynamic_cast<FbxQuaternion^>(obj);
				if(q)
					return *_Ref() == *q->_Ref();
				return false;
			}				

			virtual String^ ToString() override
			{
				return String::Format("X: {0}, Y: {1}, Z: {2}, W: {3}",X,Y,Z,W);				
			}


			/**
			* \name Casting
			*/


			//! Cast the vector in a double pointer.
			//operator double* ();

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
			void GetKFbxQuaternionFromPositionToPosition(FbxVector4^ p0,FbxVector4^ p1);
#endif //doxygen

		};

	}
}