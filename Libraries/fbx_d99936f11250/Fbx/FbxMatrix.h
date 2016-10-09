#pragma once
#include "stdafx.h"
#include "FbxXMatrix.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxVector4;
		ref class FbxQuaternion;
		ref class FbxXMatrix;
		/**	FBX SDK matrix class.
		* \nosubgrouping
		*/
		public ref class FbxMatrix : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxMatrix,KFbxMatrix);
			INATIVEPOINTER_DECLARE(FbxMatrix,KFbxMatrix);
		internal:			
			FbxMatrix(KFbxMatrix m)
			{
				_SetPointer(new KFbxMatrix(),true);
				*_FbxMatrix = m;
			}

		public:			

			/**
			* \name Constructors and Destructor
			*/
			//@{

			//! Constructor. Constructs an identity matrix.
			DEFAULT_CONSTRUCTOR(FbxMatrix,KFbxMatrix);			

			//! Copy constructor.
			FbxMatrix(FbxMatrix^ m);

			/** Constructor.
			*	\param pT     Translation vector.
			*	\param pR     Euler rotation vector.
			*	\param pS     Scale vector.
			*/
			FbxMatrix(FbxVector4^ t,
				FbxVector4^ r,
				FbxVector4^ s);

			/** Constructor.
			*	\param pT     Translation vector.
			*	\param pQ     Quaternion.
			*	\param pS     Scale vector.
			*/
			FbxMatrix(FbxVector4^ t,
				FbxQuaternion^ q,				
				FbxVector4^ s);

			/** Constructor.
			* \param pM     Affine matrix
			*/
			FbxMatrix(FbxXMatrix^ m);

			/**
			* \name Access
			*/
			//@{

			/** Retrieve matrix element.
			*	\param pY     Row index.
			*	\param pX     Column index.
			* \return       Value at element [ pX, pY ] of the matrix.
			*/
			double Get(int y, int x);

			property double default[int,int]
			{
				double get(int y, int x);
				void set(int y, int x,double value);
			}

			/** Extract a row vector.
			*	\param pY     Row index.
			* \return       The row vector.
			*/
			FbxVector4^ GetRow(int y);

			/** Extract a column vector.
			*	\param pX      Column index.
			* \return        The column vector.
			*/
			FbxVector4^ GetColumn(int x);

			/** Set matrix element.
			*	\param pY          Row index.
			*	\param pX          Column index.
			*	\param pValue      New component value.
			*/
			void Set(int y, int x, double value);

			//! Set matrix to identity.
			void SetIdentity();

			/** Set matrix.
			*	\param pT     Translation vector.
			*	\param pR     Euler rotation vector.
			*	\param pS     Scale vector.
			*/
			void SetTRS(FbxVector4^ t,
				FbxVector4^ r,
				FbxVector4^ s);

			/** Set matrix.
			*	\param pT     Translation vector.
			*	\param pQ     Quaternion.
			*	\param pS     Scale vector.
			*/
			void SetTQS(FbxVector4^ t,
				FbxQuaternion^ q,
				FbxVector4^ s);

			/** Set a matrix row.
			*	\param pY       Row index.
			*	\param pRow	    Row vector.
			*/
			void SetRow(int y, FbxVector4^ row);

			/** Set a matrix column.
			*	\param pX           Column index.
			*	\param pColumn      Column vector.
			*/
			void SetColumn(int x, FbxVector4^ column);

			/** Assignment operator.
			*	\param pMatrix     Source matrix.
			*/			
			void CopyFrom(FbxMatrix^ m);

			//@}

			/**
			* \name Matrix Operations
			*/
			//@{	

			/**	Unary minus operator.
			* \return     A matrix where each element is multiplied by -1.
			*/
			FbxMatrix^ Negate();

			/** Add two matrices together.
			* \param pMatrix    A matrix.
			* \return           The result of this matrix + pMatrix.
			*/
			static FbxMatrix^ operator +(FbxMatrix^ m1,FbxMatrix^ m2)
			{
				KFbxMatrix m = *m1->_Ref() + *m2->_Ref();
				return gcnew FbxMatrix(m); 
			}

			/** Subtract a matrix from another matrix.
			* \param pMatrix     A matrix.
			* \return            The result of this matrix - pMatrix.
			*/
			static FbxMatrix^ operator -(FbxMatrix^ m1,FbxMatrix^ m2)
			{
				KFbxMatrix m = *m1->_Ref() - *m2->_Ref();
				return gcnew FbxMatrix(m); 
			}

			/** Multiply two matrices.
			* \param pMatrix     A matrix.
			* \return            The result of this matrix * pMatrix.
			*/
			static FbxMatrix^ operator *(FbxMatrix^ m1,FbxMatrix^ m2)
			{
				KFbxMatrix m = *m1->_Ref() * *m2->_Ref();
				return gcnew FbxMatrix(m); 
			}

			/** Add two matrices together.
			* \param pMatrix     A matrix.
			* \return            The result of this matrix + pMatrix, replacing this matrix.
			*/
			//KFbxMatrix& operator+=(KFbxMatrix& pMatrix);

			/** Subtract a matrix from another matrix.
			* \param pMatrix     A matrix.
			* \return            The result of this matrix - pMatrix, replacing this matrix.
			*/
			//KFbxMatrix& operator-=(KFbxMatrix& pMatrix);

			/** Multiply two matrices.
			* \param pMatrix     A matrix.
			* \return            The result of this matrix * pMatrix, replacing this matrix.
			*/
			//KFbxMatrix& operator*=(KFbxMatrix& pMatrix);

			/** Calculate the matrix transpose.
			* \return     This matrix transposed.
			*/
			FbxMatrix^ Transpose();

			//@}

			/**
			* \name Vector Operations
			*/
			//@{	

			/** Multiply this matrix by pVector, the w component is normalized to 1.
			* \param pVector     A vector.
			* \return            The result of this matrix * pVector.
			*/
			FbxVector4^ MultNormalize(FbxVector4^ vector);

		//	//@}

		//	/**
		//	* \name Boolean Operations
		//	*/
		//	//@{

		//	/**	Equivalence operator.
		//	* \param pM     The matrix to be compared against this matrix.
		//	* \return       \c true if the two matrices are equal (each element is within a 1.0e-6 tolerance), \c false otherwise.
		//	*/
			virtual bool Equals(System::Object^ obj)override;

		//	/**	Equivalence operator.
		//	* \param pM     The affine matrix to be compared against this matrix.
		//	* \return       \c true if the two matrices are equal (each element is within a 1.0e-6 tolerance), \c false otherwise
		//	*/
			bool EqualsWith(FbxXMatrix^ m);
			//@}

			/**
			* \name Casting
			*/
			//@{

			//! Cast the vector in a double pointer.
			//operator double* ();

			//typedef const double(kDouble44)[4][4] ;

			//inline kDouble44 & Double44() const { return *((kDouble44 *)&mData); }

			//@}

			// Matrix data.
			//	double mData[4][4];

		};

	}
}