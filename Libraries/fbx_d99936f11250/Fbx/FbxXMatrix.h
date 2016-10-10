#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxTransformation.h"


{
	namespace FbxSDK
	{
		ref class FbxVector4;
		ref class FbxQuaternion;
		/**	FBX SDK affine matrix class.
		* \nosubgrouping
		* Matrices are defined using the Column Major scheme. When a KFbxXMatrix represents a transformation (translation, rotation and scale), 
		* the last row of the matrix represents the translation part of the transformation.
		*
		* \remarks It is important to realize that an affine matrix must respect a certain structure.  To be sure the structure is respected,
		* use SetT, SetR, SetS, SetQ, SetTRS or SetTQS.  If by mistake bad data is entered in this affine matrix, some functions such as 
		* Inverse() will yield wrong results.  If a matrix is needed to hold values that aren't associate with an affine matrix, please use KFbxMatrix instead.
		*/
		public ref class FbxXMatrix : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxXMatrix,KFbxXMatrix);
			INATIVEPOINTER_DECLARE(FbxXMatrix,KFbxXMatrix);
		internal:			
			FbxXMatrix(KFbxXMatrix x)
			{
				_SetPointer(new KFbxXMatrix(),true);
				*_FbxXMatrix = x;
			}
		public:

			/**
			* \name Constructors and Destructor
			*/
			//@{

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxXMatrix,KFbxXMatrix);

			//! Copy constructor.
			FbxXMatrix(FbxXMatrix^ xMatrix);

			/** Constructor.
			*	\param pT     Translation vector.
			*	\param pR     Euler rotation vector.
			*	\param pS     Scale vector.
			*/
			FbxXMatrix(FbxVector4^ t,
				FbxVector4^ r,
				FbxVector4^ s);			

			//@}

			/**
			* \name Access
			*/
			//@{

			/** Retrieve matrix element.
			*	\param pY     Row index.
			*	\param pX     Column index.
			* \return       Cell [ pX, pY ] value.
			*/
			double Get(int y, int x);

			property double default[int,int]
			{
				double get(int y , int x);				
			}

			/** Extract translation vector.
			* \return     Translation vector.
			*/
			/** Set matrix's translation.
			* \param pT     Translation vector.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,T);

			/** Extract rotation vector.
			* \return     Rotation vector.
			*/
			/** Set matrix's Euler rotation.
			* \param pR     X, Y and Z rotation values expressed as a vector.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,R);			

			/** Extract quaternion vector.
			* \return     Quaternion vector.
			*/
			/** Set matrix's quaternion.
			* \param pQ     The new quaternion.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxQuaternion,Q);			

			/** Extract scale vector.
			* \return     Scale vector.
			*/
			/** Set matrix's scale.
			* \param pS     X, Y and Z scaling factors expressed as a vector.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,S);

			/** Extract a row vector.
			*	\param pY     Row index.
			* \return       The row vector.
			*/
			FbxVector4^ GetRow(int y);

			/** Extract a column vector.
			*	\param pX     Column index.
			* \return       The column vector.
			*/
			FbxVector4^ GetColumn(int x);

			//! Set matrix to identity.
			void SetIdentity();
													

			/** Set matrix.
			*	\param pT     Translation vector.
			*	\param pR     Rotation vector.
			*	\param pS     Scale vector.
			*/
			void SetTRS(FbxVector4^ t,
				FbxVector4^ r,
				FbxVector4^ s);

			/** Set matrix.
			*	\param pT     Translation vector.
			*	\param pQ     Quaternion vector.
			*	\param pS     Scale vector.
			*/
			void SetTQS(FbxVector4^ t,
				FbxQuaternion^ q,
				FbxVector4^ s);

			//! Assignment operator.
			//KFbxXMatrix& operator=(const KFbxXMatrix& pM);
			virtual bool Equals(System::Object^ obj) override;

			//@}

			/**
			* \name Scalar Operations
			*/
			//@{

			/** Multiply matrix by a scalar value.
			* \param pValue     Scalar value.
			* \return           The scaled matrix.
			* \remarks          The passed value is not checked.
			*/
			static FbxXMatrix^ operator *(FbxXMatrix^ m, double value)
			{
				return gcnew FbxXMatrix(*m->_Ref()*value);
			}

			/** Divide matrix by a scalar value.
			* \param pValue     Scalar value.
			* \return           The divided matrix.
			* \remarks          The passed value is not checked.
			*/
			static FbxXMatrix^ operator /(FbxXMatrix^ m, double value)
			{
				return gcnew FbxXMatrix(*m->_Ref()/value);
			}

			/** Multiply matrix by a scalar value.
			* \param pValue     Scalar value.
			* \return           \e this updated with the result of the multipication.
			* \remarks          The passed value is not checked.
			*/			
			//KFbxXMatrix& operator*=(double pValue);

			/** Divide matrix by a scalar value.
			* \param pValue     Scalar value.
			* \return           \e this updated with the result of the division.
			* \remarks          The passed value is not checked.
			*/
			//KFbxXMatrix& operator/=(double pValue);

			//@}

			/**
			* \name Vector Operations
			*/
			//@{

			/** Multiply matrix by a translation vector.
			* \param pVector4     Translation vector.
			* \return             t' = M * t
			*/
			FbxVector4^ MultT(FbxVector4^ vector4);

			/** Multiply matrix by an Euler rotation vector.
			* \param pVector4     Euler Rotation vector.
			* \return             r' = M * r
			*/
			FbxVector4^ MultR(FbxVector4^ vector4);

			/** Multiply matrix by a quaternion.
			* \param pQuaternion     Rotation value.
			* \return                q' = M * q
			*/
			FbxQuaternion^ MultQ(FbxQuaternion^ quaternion);

			/** Multiply matrix by a scale vector.
			* \param pVector4     Scaling vector.
			* \return             s' = M * s
			*/
			FbxVector4^ MultS(FbxVector4^ vector4);

			//@}

			/**
			* \name Matrix Operations
			*/
			//@{	

			/**	Unary minus operator.
			* \return     A matrix where each element is multiplied by -1.
			*/
			FbxXMatrix^ Negate();

			/** Multiply two matrices together.
			* \param pXMatrix     A Matrix.
			* \return             this * pMatrix.
			*/
			static FbxXMatrix^ operator *(FbxXMatrix^ m1,FbxXMatrix^ m2)
			{
				return gcnew FbxXMatrix(*m1->_Ref() * *m2->_Ref());
			}

			/** Multiply two matrices together.
			* \param pXMatrix     A Matrix.
			* \return             \e this updated with the result of the multiplication.
			*/
			//KFbxXMatrix& operator*=(KFbxXMatrix& pXMatrix);

			/** Calculate the matrix inverse.
			* \return     The inverse matrix of \e this.
			*/
			FbxXMatrix^ Inverse();

			/** Calculate the matrix transpose.
			* \return     The transposed matrix of \e this.
			*/
			FbxXMatrix^ Transpose();

			//@}

			/**
			* \name Boolean Operations
			*/
			//@{			

			/**	Equivalence operator.
			* \param pXMatrix     The matrix to be compared to \e this.
			* \return             \c true if the two matrices are equal (each element is within a 1.0e-6 tolerance) and \c false otherwise.
			*/
			//bool operator==(KFbxXMatrix const& pXMatrix) const;

			/**	Non-equivalence operator.
			* \param pXMatrix     The matrix to be compared to \e this.
			* \return            \c false if the two matrices are equal (each element is within a 1.0e-6 tolerance) and \c true otherwise.
			*/			

			/**	Non-equivalence operator.
			* \param pXMatrix     The matrix to be compared to \e this.
			* \return            \c false if the two matrices are equal (each element is within a 1.0e-6 tolerance) and \c true otherwise.
			*/
			//bool operator!=(KFbxXMatrix const& pXMatrix) const;

			//@}

			/**
			* \name Casting
			*/
			//@{

			//! Cast the matrix in a double pointer.
			//operator double* ();

			//typedef const double(kDouble44)[4][4] ;

			//inline kDouble44 & Double44() const { return *((kDouble44 *)&mData); }
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
			void CreateKFbxXMatrixRotation(double x, double y, double z);
			void V2M(FbxXMatrix^ matrix, FbxVector4^ vector, FbxRotationOrder rotationOrder);
			void M2V(FbxVector4^ vector, FbxXMatrix^ matrix, FbxRotationOrder rotationOrder);

			/**
			* \name Internal Casting
			*/
			//@{

			//KFbxXMatrix& operator=(const KgeAMatrix& pAMatrix);
			//operator KgeAMatrix& ();

			//@}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}