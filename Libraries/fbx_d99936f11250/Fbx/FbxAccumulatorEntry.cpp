#pragma once
#include "stdafx.h"
#include "FbxAccumulatorEntry.h"
#include "FbxString.h"
#include "FbxSdkManager.h"
#include "FbxNode.h"
#include <kfbxplugins/kfbxusernotification.h>


namespace Skill
{
	namespace FbxSDK
	{	
		void FbxAccumulatorEntry::CollectManagedMemory()
		{		
		}

		FbxAccumulatorEntry::FbxAccumulatorEntry(AEClass aeClass, String^ name, String^ descr, 
				String^ detail, bool muteState)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(ds,descr);
			STRINGTO_CONSTCHAR_ANSI(d,detail);

			_SetPointer(new AccumulatorEntry((AccumulatorEntry::AEClass)aeClass,FbxString(n),FbxString(ds),FbxString(d),muteState) ,true);

			FREECHARPOINTER(n);
			FREECHARPOINTER(ds);
			FREECHARPOINTER(d);
		}
		FbxAccumulatorEntry::FbxAccumulatorEntry(AEClass aeClass, String^ name, String^ descr)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(ds,descr);			

			_SetPointer(new AccumulatorEntry((AccumulatorEntry::AEClass)aeClass,FbxString(n),FbxString(ds)) ,true);

			FREECHARPOINTER(n);
			FREECHARPOINTER(ds);			
		}
			
			FbxAccumulatorEntry::FbxAccumulatorEntry(FbxAccumulatorEntry^ ae, bool skipDetails)
			{
				_SetPointer(new AccumulatorEntry(*ae->_Ref(),skipDetails) ,true);
			}


			FbxAccumulatorEntry::AEClass FbxAccumulatorEntry::Class::get()
			{
				return (FbxAccumulatorEntry::AEClass)_Ref()->GetClass();
			}
			
			String^ FbxAccumulatorEntry::Name::get()
			{
				FbxString kstr = _Ref()->GetName();
				CONVERT_FbxString_TO_STRING(kstr,str);
				return str;
			}


			String^ FbxAccumulatorEntry::Description::get()
			{
				FbxString kstr = _Ref()->GetDescription();
				CONVERT_FbxString_TO_STRING(kstr,str);
				return str;
			}			

			int FbxAccumulatorEntry::DetailsCount::get()
			{
				return _Ref()->GetDetailsCount();
			}			
			String^ FbxAccumulatorEntry::GetDetail(int id)
			{
				const FbxString* kstr = _Ref()->GetDetail(id);
				String^ str = gcnew String(kstr->Buffer());
				return str;
			}
			
			bool FbxAccumulatorEntry::IsMuted::get()
			{
				return _Ref()->IsMuted();
			}	
						
		/*FbxUserNotificationFilteredIterator::FbxUserNotificationFilteredIterator(KFbxUserNotificationFilteredIterator* i)
		{
			iterator= i;
			isNew = false;
		}		

		FbxUserNotificationFilteredIterator::~FbxUserNotificationFilteredIterator()
		{
			this->!FbxUserNotificationFilteredIterator();
		}
		FbxUserNotificationFilteredIterator::!FbxUserNotificationFilteredIterator()
		{
			if(iterator && isNew)
				delete iterator;
			iterator = nullptr;
			isNew = false;
		}			
		int FbxUserNotificationFilteredIterator::NbItems::get()
		{
			return iterator->GetNbItems();
		}						
		void FbxUserNotificationFilteredIterator::Reset()
		{
			iterator->Reset();
		}

		FbxAccumulatorEntryInfo^ FbxUserNotificationFilteredIterator::First::get()
		{
			return gcnew FbxAccumulatorEntryInfo(iterator->First());
		}			
		FbxAccumulatorEntryInfo^ FbxUserNotificationFilteredIterator::Previous()
		{
			return gcnew FbxAccumulatorEntryInfo(iterator->Previous());
		}

		FbxAccumulatorEntryInfo^ FbxUserNotificationFilteredIterator::Next()
		{
			return gcnew FbxAccumulatorEntryInfo(iterator->Next());
		}

		FbxUserNotification::FbxUserNotification(KFbxUserNotification* n)
		{
			notification = n;
			isNew = false;				
		}				
		FbxUserNotification^ FbxUserNotification::Create(FbxSdkManager^ %manager, 
			FbxString^ logFileName, 
			FbxString^ sessionDescription)
		{
			FbxUserNotification^ u = 
				gcnew FbxUserNotification(KFbxUserNotification::Create(manager->manager,*logFileName->str,*sessionDescription->str));
			u->isNew = true;				
			return u;
		}

		void FbxUserNotification::Destroy(FbxSdkManager^ %manager)
		{
			KFbxUserNotification::Destroy(manager->manager);
		}			
		FbxUserNotification::FbxUserNotification(FbxSdkManager^ manager,
			FbxString^ logFileName, 
			FbxString^  sessionDescription)
		{
			notification = new KFbxUserNotification(manager->manager,*logFileName->str,*sessionDescription->str);				
			isNew = true;
		}			
		FbxUserNotification::~FbxUserNotification()
		{				
			this->!FbxUserNotification();
		}
		FbxUserNotification::!FbxUserNotification()
		{
			if(notification && isNew)
				delete notification;
			isNew = false;
			notification = nullptr;
		}
		void FbxUserNotification::InitializeAccumulator()
		{
			notification->InitAccumulator();
		}			
		void FbxUserNotification::ClearAccumulator()
		{
			notification->ClearAccumulator();
		}			
		int FbxUserNotification::AddEntry(int id, FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass)
		{
			return notification->AddEntry(id,*name->str,*descr->str,(AccumulatorEntry::AEClass)aeClass);
		}

		int FbxUserNotification::AddEntry(int id, FbxString^ name, FbxString^ descr)
		{
			return notification->AddEntry(id,*name->str,*descr->str);
		}
		int FbxUserNotification::AddDetail(int entryId)
		{
			return notification->AddDetail(entryId);
		}
		int FbxUserNotification::AddDetail(int entryId, FbxString^ str)
		{
			return notification->AddDetail(entryId,*str->str);
		}
		int FbxUserNotification::AddDetail(int entryId, FbxNode^ %node)
		{
			return notification->AddDetail(entryId,(KFbxNode*)node->emitter );
		}

		int  FbxUserNotification::NbEntries::get()
		{
			return notification->GetNbEntries();
		}			
		FbxAccumulatorEntryInfo^ FbxUserNotification::GetEntry(int entryId)
		{
			return gcnew FbxAccumulatorEntryInfo(notification->GetEntry(entryId));
		}
		FbxAccumulatorEntryInfo^ FbxUserNotification::GetEntryAt(int entryIndex)
		{
			return gcnew FbxAccumulatorEntryInfo(notification->GetEntryAt(entryIndex));
		}
		int FbxUserNotification::NbDetails::get()
		{
			return notification->GetNbDetails();
		}			
		bool FbxUserNotification::Output(OutputSource outSrc, int index, bool extraDevicesOnly)
		{
			return notification->Output((KFbxUserNotification::OutputSource)outSrc,index,extraDevicesOnly);
		}

		bool FbxUserNotification::Output(OutputSource outSrc)
		{
			return notification->Output((KFbxUserNotification::OutputSource)outSrc);
		}
		bool FbxUserNotification::Output(OutputSource outSrc, int index)
		{
			return notification->Output((KFbxUserNotification::OutputSource)outSrc,index);
		}
		bool FbxUserNotification::OutputById(AeID id, OutputSource outSrc, bool extraDevicesOnly )
		{
			return notification->OutputById((KFbxUserNotification::AEid)id,(KFbxUserNotification::OutputSource)outSrc,extraDevicesOnly);
		}			
		bool FbxUserNotification::OutputById(AeID id)
		{
			return notification->OutputById((KFbxUserNotification::AEid)id);
		}
		bool FbxUserNotification::OutputById(AeID id, OutputSource outSrc)
		{
			return notification->OutputById((KFbxUserNotification::AEid)id,(KFbxUserNotification::OutputSource)outSrc);
		}
		bool FbxUserNotification::Output(FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass, bool extraDevicesOnly)
		{
			return notification->Output(*name->str, *descr->str, (AccumulatorEntry::AEClass)aeClass,extraDevicesOnly);
		}			
		bool FbxUserNotification::Output(FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass)
		{
			return notification->Output(*name->str, *descr->str, (AccumulatorEntry::AEClass)aeClass);
		}
		bool FbxUserNotification::Output(FbxUserNotificationFilteredIterator^ aeFIter, bool extraDevicesOnly)
		{
			return notification->Output(*aeFIter->iterator,extraDevicesOnly);
		}
		bool FbxUserNotification::Output(FbxUserNotificationFilteredIterator^ aeFIter)
		{
			return notification->Output(*aeFIter->iterator);
		}
		void FbxUserNotification::GetLogFilePath(FbxString^ path)
		{
			notification->GetLogFilePath(*path->str);
		}
		FbxUserNotificationFilteredIterator^ FbxUserNotification::CreateFilteredIterator(FbxUserNotification^ accumulator, 
			int filterClass,
			FbxUserNotification::OutputSource src,
			bool noDetail)
		{
			FbxUserNotificationFilteredIterator^ f = 
				gcnew FbxUserNotificationFilteredIterator(new KFbxUserNotificationFilteredIterator(*accumulator->notification,filterClass,
				(KFbxUserNotification::OutputSource)src,noDetail));
			f->isNew = true;
			return f;
		}	*/

	}
}