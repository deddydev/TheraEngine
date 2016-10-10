#pragma once
#include "stdafx.h"
#include "FbxQuery.h"
#include "FbxProperty.h"
#include "FbxClassId.h"


{
	namespace FbxSDK
	{	

		void FbxQuery::CollectManagedMemory()
		{
		}		
		kFbxFilterId FbxQuery::UniqueId::get()
		{
			return _Ref()->GetUniqueId();
		}
		bool FbxQuery::IsPropertyValid(FbxPropertyManaged^ p)
		{
			return _Ref()->IsValid(*p->_Ref());
		}			
		bool FbxQuery::IsPropertyValid(FbxPropertyManaged^ p,FbxConnectionType type)
		{
			return _Ref()->IsValid(*p->_Ref(),(kFbxConnectionType)type);
		}
		bool FbxQuery::IsEqual(FbxQuery^ otherQuery)
		{
			return _Ref()->IsEqual(otherQuery->_Ref());
		}

		void FbxQuery::Ref()
		{
			_Ref()->Ref();
		}

		void FbxQuery::Unref()		
		{
			_Ref()->Unref();
		}		

		FbxQueryOperator^ FbxQueryOperator::Create(FbxQuery^ a,FbxQueryOperatorType op,FbxQuery^ b)
		{
			KFbxQueryOperator* k = KFbxQueryOperator::Create(a->_Ref(),(eFbxQueryOperator)op,b->_Ref());
			if(k)
			{
				FbxQueryOperator^ c = gcnew FbxQueryOperator(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}

		FbxUnaryQueryOperator^ FbxUnaryQueryOperator::Create(FbxQuery^ a,FbxUnaryQueryOperatorType op)
		{
			KFbxUnaryQueryOperator* k = KFbxUnaryQueryOperator::Create(a->_Ref(),(eFbxUnaryQueryOperator)op);
			if(k)
			{
				FbxUnaryQueryOperator^ c = gcnew FbxUnaryQueryOperator(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}

		FbxQueryClassId^ FbxQueryClassId::Create(FbxClassId^ classId)
		{
			KFbxQueryClassId* k = KFbxQueryClassId::Create(*classId->_Ref());
			if(k)
			{
				FbxQueryClassId^ c = gcnew FbxQueryClassId(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}

		FbxQueryIsA^ FbxQueryIsA::Create(FbxClassId^ classId)
		{
			KFbxQueryIsA* k = KFbxQueryIsA::Create(*classId->_Ref());
			if(k)
			{
				FbxQueryIsA^ c = gcnew FbxQueryIsA(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}		
		FbxQueryProperty^ FbxQueryProperty::Create()
		{
			KFbxQueryProperty* k = KFbxQueryProperty::Create();
			if(k)
			{
				FbxQueryProperty^ c = gcnew FbxQueryProperty(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}
		FbxQueryConnectionType^ FbxQueryConnectionType::Create(FbxConnectionType connectionType)
		{
			KFbxQueryConnectionType* k = KFbxQueryConnectionType::Create((kFbxConnectionType)connectionType);
			if(k)
			{
				FbxQueryConnectionType^ c = gcnew FbxQueryConnectionType(k);
				//c->_Free = true;
				return c;
			}
			return nullptr;
		}

		void FbxCriteria::CollectManagedMemory()
		{
			_Query = nullptr;
		}	

		FbxCriteria^ FbxCriteria::ConnectionType(FbxConnectionType connectionType)
		{
			FbxCriteria^ c = gcnew FbxCriteria(KFbxCriteria::ConnectionType((kFbxConnectionType)connectionType));
			c->_Free = true;
			return c;
		}

		FbxCriteria^ FbxCriteria::ObjectType(FbxClassId^ classId)
		{
			FbxCriteria^ c = gcnew FbxCriteria(KFbxCriteria::ObjectType(*classId->_Ref()));
			c->_Free = true;
			return c;
		}
		FbxCriteria^ FbxCriteria::ObjectIsA(FbxClassId^ classId)
		{
			FbxCriteria^ c = gcnew FbxCriteria(KFbxCriteria::ObjectIsA(*classId->_Ref()));
			c->_Free = true;
			return c;
		}
		FbxCriteria^ FbxCriteria::Property()
		{
			FbxCriteria^ c = gcnew FbxCriteria(KFbxCriteria::Property());
			c->_Free = true;
			return c;
		}

		FbxCriteria::FbxCriteria(FbxCriteria^ criteria)
		{
			_SetPointer(new KFbxCriteria(*criteria->_Ref()),true);
		}
		void  FbxCriteria::CopyFrom(FbxCriteria^ other)
		{
			*_FbxCriteria = *other->_Ref();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCriteria,KFbxQuery,GetQuery(),FbxQuery,Query);	
	}	
}