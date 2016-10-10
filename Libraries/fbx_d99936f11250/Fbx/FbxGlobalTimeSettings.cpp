#pragma once
#include "stdafx.h"
#include "FbxGlobalTimeSettings.h"
#include "FbxTime.h"
#include "FbxString.h"
#include "FbxError.h"



{
	namespace FbxSDK
	{

		void FbxGlobalTimeSettings::CollectManagedMemory()
		{
			_KError = nullptr;
		}

		void FbxGlobalTimeSettings::RestoreDefaultSettings()
		{
			_Ref()->RestoreDefaultSettings();
		}
		FbxTime::TimeMode  FbxGlobalTimeSettings::Mode::get()
		{
			return (FbxTime::TimeMode)_Ref()->GetTimeMode();
		}
		void FbxGlobalTimeSettings::Mode::set(FbxTime::TimeMode value)
		{
			_Ref()->SetTimeMode((KTime::ETimeMode)value);
		}
		FbxTime::TimeProtocol  FbxGlobalTimeSettings::Protocol::get()
		{
			return (FbxTime::TimeProtocol)_Ref()->GetTimeProtocol();
		}
		void FbxGlobalTimeSettings::Protocol::set(FbxTime::TimeProtocol value)
		{
			_Ref()->SetTimeProtocol((KTime::ETimeProtocol)value);
		}
		FbxGlobalTimeSettings::SnapOnFrameMode  FbxGlobalTimeSettings::TimeSnapOnFrameMode::get()
		{
			return (SnapOnFrameMode)_Ref()->GetSnapOnFrameMode();
		}
		void FbxGlobalTimeSettings::TimeSnapOnFrameMode::set(FbxGlobalTimeSettings::SnapOnFrameMode value)
		{
			_Ref()->SetSnapOnFrameMode((KFbxGlobalTimeSettings::ESnapOnFrameMode)value);
		}
		void FbxGlobalTimeSettings::SetTimelineDefautTimeSpan(FbxTimeSpan^ timeSpan)
		{
			_Ref()->SetTimelineDefautTimeSpan(*timeSpan->_Ref());
		}
		void FbxGlobalTimeSettings::GetTimelineDefautTimeSpan(FbxTimeSpan^ timeSpan)
		{
			_Ref()->GetTimelineDefautTimeSpan(*timeSpan->_Ref());
		}


		void FbxGlobalTimeSettings::FbxTimeMarker::CollectManagedMemory()
		{			
			_Time = nullptr;
		}

		FbxGlobalTimeSettings::FbxTimeMarker::FbxTimeMarker(FbxTimeMarker^ timeMarker)
		{
			_SetPointer(new KFbxGlobalTimeSettings::KFbxTimeMarker(*timeMarker->_Ref()),true);			
		}
		void FbxGlobalTimeSettings::FbxTimeMarker::CopyFrom(FbxTimeMarker^ timeMarker)
		{
			*this->_Ref() = *timeMarker->_Ref();
		}

		String^ FbxGlobalTimeSettings::FbxTimeMarker::Name::get()
		{
			CONVERT_FbxString_TO_STRING(_Ref()->mName,Str);
			return Str;
		}
		void FbxGlobalTimeSettings::FbxTimeMarker::Name::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->mName = FbxString(v);
			FREECHARPOINTER(v);
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxGlobalTimeSettings::FbxTimeMarker,mTime,FbxTime,Time);

		bool FbxGlobalTimeSettings::FbxTimeMarker::Loop::get()
		{
			return _Ref()->mLoop;
		}
		void FbxGlobalTimeSettings::FbxTimeMarker::Loop::set(bool value)
		{
			_Ref()->mLoop = value;
		}

		int FbxGlobalTimeSettings::TimeMarkerCount::get()
		{
			return _Ref()->GetTimeMarkerCount();
		}			
		bool FbxGlobalTimeSettings::SetCurrentTimeMarker(int index)
		{
			return _Ref()->SetCurrentTimeMarker(index);
		}
		int FbxGlobalTimeSettings::CurrentTimeMarker::get()
		{
			return _Ref()->GetCurrentTimeMarker();
		}
		FbxGlobalTimeSettings::FbxTimeMarker^ FbxGlobalTimeSettings::GetTimeMarker(int index)
		{
			KFbxGlobalTimeSettings::KFbxTimeMarker* k = _Ref()->GetTimeMarker(index);
			if(k)
				return gcnew FbxGlobalTimeSettings::FbxTimeMarker(k);
			return nullptr;
		}
		void FbxGlobalTimeSettings::AddTimeMarker(FbxGlobalTimeSettings::FbxTimeMarker^ timeMarker)
		{
			_Ref()->AddTimeMarker(*timeMarker->_Ref());
		}
		void FbxGlobalTimeSettings::RemoveAllTimeMarkers()
		{
			_Ref()->RemoveAllTimeMarkers();
		}
		void FbxGlobalTimeSettings::CopyFrom(FbxGlobalTimeSettings^ settings)
		{
			*this->_Ref() = *settings->_Ref();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGlobalTimeSettings,GetError(),FbxErrorManaged,KError);

		FbxGlobalTimeSettings::Error FbxGlobalTimeSettings::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}			
		String^ FbxGlobalTimeSettings::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}
		bool FbxGlobalTimeSettings::SnapOnFrame::get()
		{
			return _Ref()->GetSnapOnFrame();
		}
		void FbxGlobalTimeSettings::SnapOnFrame::set(bool value)
		{
			_Ref()->SetSnapOnFrame(value);
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS


	}
}