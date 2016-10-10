#pragma once
#include "stdafx.h"
#include "FbxTime.h"


{
	namespace FbxSDK
	{			

		void FbxTime::CollectManagedMemory()
		{
		}
		
		FbxTime::FbxTime(kLongLong time)
		{
			_SetPointer(new KTime(time),true);			
		}	
		
		void FbxTime::SetGlobalTimeMode(FbxTime::TimeMode timeMode, double frameRate)
		{
			KTime::SetGlobalTimeMode((KTime::ETimeMode)timeMode,frameRate);
		}
		
		FbxTime::TimeMode FbxTime::GlobalTimeMode::get()
		{
			return (FbxTime::TimeMode)KTime::GetGlobalTimeMode();
		}
		void FbxTime::GlobalTimeMode::set(FbxTime::TimeMode value)
		{
			KTime::SetGlobalTimeMode((KTime::ETimeMode)value);
		}


		FbxTime::TimeProtocol FbxTime::GlobalTimeProtocol::get()
		{
			return (FbxTime::TimeProtocol)KTime::GetGlobalTimeProtocol();
		}
		void FbxTime::GlobalTimeProtocol::set(FbxTime::TimeProtocol value)
		{
			KTime::SetGlobalTimeProtocol((KTime::ETimeProtocol)value);
		}

		double FbxTime::GetFrameRate(FbxTime::TimeMode timeMode)
		{
			return KTime::GetFrameRate((KTime::ETimeMode)timeMode);
		}
		FbxTime::TimeMode FbxTime::ConvertFrameRateToTimeMode(double frameRate, double precision)
		{
			return (FbxTime::TimeMode)KTime::ConvertFrameRateToTimeMode(frameRate,precision);
		}

		void FbxTime::Set(kLongLong time)
		{
			_Ref()->Set(time);
		}
		kLongLong FbxTime::Get()
		{
			return _Ref()->Get();
		}
		kLongLong FbxTime::MilliSeconds::get()
		{
			return _Ref()->GetMilliSeconds();
		}
		/*void FbxTime::MilliSeconds::set(kLongLong value)
		{
			_Ref()->SetMilliSeconds(value);
		}*/

		double FbxTime::SecondDouble::get()
		{
			return _Ref()->GetSecondDouble();
		}
		void FbxTime::SecondDouble::set(double value)
		{
			_Ref()->SetSecondDouble(value);
		}

		void FbxTime::SetTime(int hour,int minute,int second,int frame,int field, TimeMode timeMode,double framerate)
		{
			_Ref()->SetTime(hour,minute,second,frame,field,(int)timeMode,framerate);
		}
		void FbxTime::SetTime(int hour,int minute,int second,int frame,int field,int residual,TimeMode timeMode,double framerate)
		{
			_Ref()->SetTime(hour,minute,second,frame,field,residual,(int)timeMode,framerate);
		}

		bool FbxTime::GetTime([OutAttribute]kLongLong %hour,[OutAttribute]kLongLong minute,
					[OutAttribute]kLongLong %second,[OutAttribute]kLongLong %frame, 
					[OutAttribute]kLongLong %field,[OutAttribute]kLongLong %residual, 
					FbxTime::TimeMode timeMode,double framerate)
		{
			kLongLong h,m,s,f,fld,r;
			bool b = _Ref()->GetTime(h,m,s,f,fld,r,(int)timeMode,framerate);
			hour = h;
			minute = m;
			second = s;
			frame = f;
			field = fld;
			residual = r;
			return b;
		}
		
		FbxTime^ FbxTime::GetFramedTime(bool round)
		{
			return gcnew FbxTime(_Ref()->GetFramedTime(round));
		}
		kLongLong FbxTime::GetHour(bool cummul,TimeMode timeMode, double framerate)
		{
			return _Ref()->GetHour(cummul,(int)timeMode,framerate);
		}
		kLongLong FbxTime::GetMinute(bool cummul,TimeMode timeMode, double framerate)
		{
			return _Ref()->GetMinute(cummul,(int)timeMode,framerate);
		}
		kLongLong FbxTime::GetSecond(bool cummul,TimeMode timeMode, double framerate)
		{
			return _Ref()->GetSecond(cummul,(int)timeMode,framerate);
		}
		kLongLong FbxTime::GetFrame(bool cummul,TimeMode timeMode, double framerate)
		{
			return _Ref()->GetFrame(cummul,(int)timeMode,framerate);
		}
		kLongLong FbxTime::GetField(bool cummul,TimeMode timeMode, double framerate)
		{
			return _Ref()->GetField(cummul,(int)timeMode,framerate);
		}
		kLongLong FbxTime::GetResidual(TimeMode timeMode, double framerate)
		{
			return _Ref()->GetResidual((int)timeMode,framerate);
		}

		String^ FbxTime::GetTimeString(String^ timeString,int info,TimeMode timeMode, 
								TimeProtocol timeFormat,double framerate)
		{
			STRINGTOCHAR_ANSI(ts,timeString);
			String^ result = gcnew String(_Ref()->GetTimeString(ts,info,(int)timeMode, 
								(int)timeFormat,framerate));
			FREECHARPOINTER(ts);
			return result;
		}
		void FbxTime::SetTimeString(String^ time,TimeMode timeMode,TimeProtocol timeFormat, double framerate)
		{
			STRINGTOCHAR_ANSI(t,time);
			_Ref()->SetTimeString(t,(int)timeMode,(int)timeFormat,framerate);
			FREECHARPOINTER(t);
		}

		FbxTime^ FbxTime::GetSystemTimer()
		{
			return gcnew FbxTime(KTime::GetSystemTimer());
		}		

		void FbxTimeSpan::CollectManagedMemory()
		{
			_Stop = nullptr;
			_Start = nullptr;
		}
		FbxTimeSpan::FbxTimeSpan(FbxTime^ start, FbxTime^ stop)
		{
			_SetPointer(new KTimeSpan(*start->_Ref(),*stop->_Ref()),true);
		}
		void FbxTimeSpan::Set(FbxTime^ start, FbxTime^ stop)
		{
			_Ref()->Set(*start->_Ref(),*stop->_Ref());
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTimeSpan,GetStart(),FbxTime,Start);
		void FbxTimeSpan::Start::set(FbxTime^ value)
		{
			if(value)
				_Ref()->SetStart(*value->_Ref());
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTimeSpan,GetStop(),FbxTime,Stop);
		void FbxTimeSpan::Stop::set(FbxTime^ value)
		{
			if(value)
				_Ref()->SetStop(*value->_Ref());
		}
		
		FbxTime^ FbxTimeSpan::GetDuration()
		{
			return gcnew FbxTime(_Ref()->GetDuration());
		}
		/*FbxTime^ FbxTimeSpan::GetSignedDuration()
		{
			return gcnew FbxTime(_Ref()->GetSignedDuration());
		}*/
		int FbxTimeSpan::Direction::get()
		{
			return _Ref()->GetDirection();
		}


		/*void FbxTimeModeObject::CollectManagedMemory()
		{
		}*/
	}
}