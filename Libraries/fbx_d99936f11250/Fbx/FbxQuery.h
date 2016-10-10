#pragma once
#include "stdafx.h"
#include "FbxPropertyDef.h"


{
	namespace FbxSDK
	{		
		ref class FbxPropertyManaged;
		/**	\brief Class to manage query.
		* \nosubgrouping
		*/
		public ref class FbxQuery : IFbxNativePointer
		{			
			INTERNAL_CLASS_DECLARE(FbxQuery,KFbxQuery);
			REF_DECLARE(FbxQuery,KFbxQuery);
			DESTRUCTOR_DECLARE_2(FbxQuery);
			INATIVEPOINTER_DECLARE(FbxQuery,KFbxQuery);

		public:

			//! Get unique filter Id
			virtual VALUE_PROPERTY_GET_DECLARE(kFbxFilterId ,UniqueId);

			//! Judge if the given property is valid.
			virtual bool IsPropertyValid(FbxPropertyManaged^ p);
			//! Judge if the given property and connection type are valid.
			virtual bool IsPropertyValid(FbxPropertyManaged^ p,FbxConnectionType type);

			/**This compares whether two KFbxQuery are the same, NOT whether the query
			* matches or not.  It's strictly the equivalent of an operator==, but virtual.
			* \param pOtherQuery The given KFbxQuery
			*/
			virtual bool IsEqual(FbxQuery^ otherQuery);

			//! Add one to ref count .
			void Ref();
			//! Minus one to ref count, if ref count is zero, delete this query object.
			void Unref();		
		};



		//***********************************************
		//KFbxQueryOperator (binary operators)
		//************************************************/
		public enum class FbxQueryOperatorType
		{
			And = eFbxAnd,
			Or= eFbxOr
		} ;

		/**	\brief Class to manage query operator.
		* \nosubgrouping
		*/

		public ref class  FbxQueryOperator : FbxQuery
		{
		internal:
			FbxQueryOperator(KFbxQueryOperator* instance) : FbxQuery(instance)
			{
				_Free = false;
			}

		public:
			//! Create new query operator.
			static FbxQueryOperator^ Create(FbxQuery^ a,FbxQueryOperatorType op,FbxQuery^ b);			
			// Test functions			
		};

		///***********************************************
		//KFbxUnaryQueryOperator
		//************************************************/
		public enum class FbxUnaryQueryOperatorType 
		{
			Not = eFbxNot 
		};

		/**	\brief Class to manage unary query operator.
		* \nosubgrouping
		*/
		public ref class FbxUnaryQueryOperator : FbxQuery
		{
		internal:
			FbxUnaryQueryOperator(KFbxUnaryQueryOperator* instance):FbxQuery(instance)
			{
			}
			//
		public:
			//! Create new unary query operator.
			static FbxUnaryQueryOperator^ Create(FbxQuery^ a,FbxUnaryQueryOperatorType op);
		};

		ref class FbxClassId;
		///***********************************************
		//KFbxQueryClassId -- match anywhere in the hierarchy of an object.
		//************************************************/
		///**	\brief Class to manage query class Id.
		//* \nosubgrouping
		//*/
		public ref class FbxQueryClassId : FbxQuery 
		{
		internal:
			FbxQueryClassId(KFbxQueryClassId* instance):FbxQuery(instance)
			{
				_Free = false;
			}				
		public:
			//! Creat a new query class Id.
			static FbxQueryClassId^ Create(FbxClassId^ classId);			
		};

		///***********************************************
		//KFbxQueryIsA -- Exact match.
		//************************************************/

		///**	\brief Class to manage query property .
		//* \nosubgrouping
		//*/
		public ref class FbxQueryIsA : FbxQuery 
		{
		internal:
			FbxQueryIsA(KFbxQueryIsA* instance) : FbxQuery(instance)
			{
				_Free =false;
			}

			//
		public:
			//! Create a new query IsA object
			static FbxQueryIsA^ Create(FbxClassId^ classId);			
		};

		///***********************************************
		//KFbxQueryProperty
		//************************************************/

		///**	\brief Class to manage query property .
		//* \nosubgrouping
		//*/
		public ref class FbxQueryProperty : FbxQuery 
		{
		internal:
			FbxQueryProperty(KFbxQueryProperty* instance):FbxQuery(instance)
			{
				_Free = false;
			}				
		public:
			//! Create new query property
			static FbxQueryProperty^ Create();			
		};

		///***********************************************
		//KFbxQueryConnectionType
		//************************************************/

		///**	\brief Class to manage query connection type.
		//* \nosubgrouping
		//*/
		public ref class FbxQueryConnectionType : FbxQuery 
		{
		internal:
			FbxQueryConnectionType(KFbxQueryConnectionType* instance):FbxQuery(instance)
			{
				_Free = false;
			}
		public:

			//! Create a new query connection type
			static FbxQueryConnectionType^ Create(FbxConnectionType connectionType);			
		};

		///***********************************************
		//KFbxCriteria
		//************************************************/
		public ref class FbxCriteria : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCriteria,KFbxCriteria);
			INATIVEPOINTER_DECLARE(FbxCriteria,KFbxCriteria);
		internal:
			FbxCriteria(KFbxCriteria c)
			{
				_SetPointer(new KFbxCriteria(),true);
				*_FbxCriteria = c;
			}

		public:
			static FbxCriteria^ ConnectionType(FbxConnectionType connectionType);

			static FbxCriteria^ ObjectType(FbxClassId^ classId);
			static FbxCriteria^ ObjectIsA(FbxClassId^ classId);
			static FbxCriteria^ Property();

			DEFAULT_CONSTRUCTOR(FbxCriteria,KFbxCriteria);

			FbxCriteria(FbxCriteria^ criteria);

		public:
			void  CopyFrom(FbxCriteria^ other);

			/*inline KFbxCriteria operator && (KFbxCriteria const &pCriteria) const
			{
				return KFbxCriteria(KFbxQueryOperator::Create(GetQuery(),eFbxAnd,pCriteria.GetQuery()));
			}
			inline KFbxCriteria operator || (KFbxCriteria const &pCriteria) const
			{
				return KFbxCriteria(KFbxQueryOperator::Create(GetQuery(),eFbxOr,pCriteria.GetQuery()));
			}
			inline KFbxCriteria operator !() const
			{
				return KFbxCriteria(KFbxUnaryQueryOperator::Create(GetQuery(), eFbxNot));
			}*/

		public:
			REF_PROPERTY_GET_DECLARE(FbxQuery,Query);		
		};
	}	
}