#pragma once
#include "stdafx.h"
#include "Fbx.h"

using System::Runtime::InteropServices::OutAttribute;


{
	namespace FbxSDK
	{		
		/** Class to encapsulate time units.
		* \nosubgrouping
		*/
		public ref class FbxTime : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxTime,KTime);
			INATIVEPOINTER_DECLARE(FbxTime,KTime);		
		internal:			
			FbxTime(KTime t)
			{
				_SetPointer(new KTime(),true);
				*_FbxTime = t;				
			}

		public:

			static property FbxTime^ Infinite{FbxTime^ get(){return gcnew FbxTime(KTIME_INFINITE);}}
			static property FbxTime^ MinusInfinite{FbxTime^ get(){return gcnew FbxTime(KTIME_MINUS_INFINITE);}}
			static property FbxTime^ Zero{FbxTime^ get(){return gcnew FbxTime(KTIME_ZERO);}}
			static property FbxTime^ Epsilon{FbxTime^ get(){return gcnew FbxTime(KTIME_EPSILON);}}
			static property FbxTime^ OneSecond{FbxTime^ get(){return gcnew FbxTime(KTIME_ONE_SECOND);}}
			static property FbxTime^ OneMinute{FbxTime^ get(){return gcnew FbxTime(KTIME_ONE_MINUTE);}}
			static property FbxTime^ OneHour{FbxTime^ get(){return gcnew FbxTime(KTIME_ONE_HOUR);}}
			static property FbxTime^ FiveSeconds{FbxTime^ get(){return gcnew FbxTime(KTIME_FIVE_SECONDS);}}			
			/** Constructor.
			* \param pTime Initial value.
			*/
			FbxTime(kLongLong time);
			DEFAULT_CONSTRUCTOR(FbxTime,KTime);

			/**
			* \name Time Modes and Protocols
			*/
			//@{

			/** Time modes.
			* \remarks
			* Mode \c eNTSC_DROP_FRAME is used for broadcasting operations where 
			* clock time must be (almost) in sync with timecode. To bring back color 
			* NTSC timecode with clock time, this mode drops 2 frames per minute
			* except for every 10 minutes (00, 10, 20, 30, 40, 50). 108 frames are 
			* dropped per hour. Over 24 hours the error is 2 frames and 1/4 of a 
			* frame.
			* 
			* \par
			* Mode \c eNTSC_FULL_FRAME represents a time address and therefore is NOT 
			* IN SYNC with clock time. A timecode of 01:00:00:00 equals a clock time 
			* of 01:00:03:18.
			* 
			* \par
			* Mode \c eFRAMES30_DROP drops 2 frames every minutes except for every 10 
			* minutes (00, 10, 20, 30, 40, 50). This timecode represents a time 
			* address and is therefore NOT IN SYNC with clock time. A timecode of
			* 01:00:03:18 equals a clock time of 01:00:00:00. It is the close 
			* counterpart of mode \c eNTSC_FULL_FRAME.
			*/
			//
			// *** Affected files when adding new enum values ***
			// (ktimeinline.h kfcurveview.cxx, kaudioview.cxx, kvideoview.cxx, kttimespanviewoptical.cxx,
			//  kttimespanview.cxx, ktcameraswitchertimelinecontrol.cxx, fbxsdk(fpproperties.cxx) )
			//
			/** \enum ETimeMode Time modes. 
			* - \e eDEFAULT_MODE	
			* - \e eFRAMES120		  120 frames/s
			* - \e eFRAMES100	      100 frames/s
			* - \e eFRAMES60          60 frames/s
			* - \e eFRAMES50          50 frames/s
			* - \e eFRAMES48          48 frame/s
			* - \e eFRAMES30          30 frames/s	 BLACK & WHITE NTSC
			* - \e eFRAMES30_DROP     30 frames/s use when diplay in frame is selected(equivalent to NTSC_DROP)
			* - \e eNTSC_DROP_FRAME   29.97002617 frames/s drop COLOR NTSC
			* - \e eNTSC_FULL_FRAME   29.97002617 frames/s COLOR NTSC
			* - \e ePAL               25 frames/s	 PAL/SECAM
			* - \e eCINEMA            24 frames/s
			* - \e eFRAMES1000        1000 milli/s (use for datetime)
			* - \e eCINEMA_ND	      23.976 frames/s
			* - \e eCUSTOM            Custom Framerate value
			*/
			enum class TimeMode
			{
				DefaultMode       = 0,
				Frames120          = 1,
				Frames100          = 2,
				Frames60           = 3,
				Frames50           = 4,
				Frames48           = 5,
				Frames30           = 6,
				Frames30Drop      = 7,
				NtscDropFrame    = 8,
				NtscFullFrame	= 9,
				Pal                = 10,
				Cinema             = 11,
				Frames1000         = 12,
				CinemaNd		= 13,
				Custom             = 14,
				TimeModeCcount    = 15
			};

			/** \enum ETimeProtocol Time protocols.
			* - \e eSMPTE             SMPTE Protocol
			* - \e eFRAME             Frame count
			* - \e eDEFAULT_PROTOCOL  Default protocol (initialized to eFRAMES)
			*/
			enum class TimeProtocol
			{
				Smpte = KTime::eSMPTE,
				Frame = KTime::eFRAME,
				DefaultProtocol = KTime::eDEFAULT_PROTOCOL,
				TimeProtocolCount = KTime::eTIME_PROTOCOL_COUNT
			};

			/** Set default time mode.
			* \param pTimeMode Time mode identifier.
			* \param pFrameRate in case of timemode = custom, we specify the custom framerate to use: EX:12.5
			* \remarks It is meaningless to set default time mode to \c eDEFAULT_MODE.
			*/
			static void SetGlobalTimeMode(TimeMode timeMode, double frameRate);							

			/** Get default time mode.
			* \return Currently set time mode identifier.
			* \remarks Default time mode initial value is eFRAMES30.
			*/
			/** Set default time mode.
			* \param pTimeMode Time mode identifier.
			* \param pFrameRate in case of timemode = custom, we specify the custom framerate to use: EX:12.5
			* \remarks It is meaningless to set default time mode to \c eDEFAULT_MODE.
			*/							
			static property TimeMode GlobalTimeMode
			{
				TimeMode get();void set(TimeMode value);
			}

			/** Set default time protocol.
			* \param pTimeProtocol Time protocol identifier.
			* \remarks It is meaningless to set default time protocol to \c eDEFAULT_PROTOCOL.
			*/
			/** Get default time protocol.
			* \return Currently set time protocol identifier.
			* \remarks Default time protocol initial value is eSMPTE.
			*/
			static property TimeProtocol GlobalTimeProtocol
			{
				TimeProtocol get();void set(TimeProtocol value);
			}															

			/** Get frame rate associated with time mode, in frames per second.
			* \param pTimeMode Time mode identifier.
			* \return Frame rate value.
			*/
			static double GetFrameRate(TimeMode timeMode);

			/** Get time mode associated with frame rate.
			* \param pFrameRate The frame rate value.
			* \param lPrecision The tolerance value.
			* \return The corresponding time mode identifier or \c eDEFAULT_MODE if no time 
			* mode associated to the given frame rate is found.
			*/
			static TimeMode ConvertFrameRateToTimeMode(double frameRate, double precision);
			static TimeMode ConvertFrameRateToTimeMode(double frameRate)
			{
				return ConvertFrameRateToTimeMode(frameRate,0.00000001);
			}

			//@}

			/**
			* \name Time Conversion
			*/
			//@{

			/** Set time in internal format.
			* \param pTime Time value to set.
			*/
			void Set(kLongLong time);

			/** Get time in internal format.
			* \return Time value.
			*/
			kLongLong Get();															

			/** Get time in milliseconds.
			* \return Time value.
			*/
			/** Set time in milliseconds.
			* \param pMilliSeconds Time value to set.
			*/
			VALUE_PROPERTY_GET_DECLARE(kLongLong,MilliSeconds);

			/** Get time in seconds.
			* \return Time value.
			*/			
			/** Set time in seconds.
			* \param pTime Time value to set.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,SecondDouble);																	


			/** Set time in hour/minute/second/frame/field format.
			* \param pHour The hours value.
			* \param pMinute The minutes value.
			* \param pSecond The seconds value.
			* \param pFrame The frames values.
			* \param pField The field value.
			* \param pTimeMode A time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \remarks Parameters pHour, pMinute, pSecond, pFrame and pField are summed together.
			* For example, it is possible to set the time to 83 seconds in the following
			* ways: SetTime(0,1,23) or SetTime(0,0,83).
			*/
			void SetTime(int hour,int minute,int second,int frame, 
				int field, TimeMode timeMode,double framerate);

			/** Set time in hour/minute/second/frame/field/residual format.
			* \param pHour The hours value.
			* \param pMinute The minutes value.
			* \param pSecond The seconds value.
			* \param pFrame The frames values.
			* \param pField The field value.
			* \param pResidual The hundreths of frame value.
			* \param pTimeMode A time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \remarks Parameters pHour, pMinute, pSecond, pFrame, pField and pResidual 
			* are summed together, just like above.
			* pResidual represents hundreths of frame, and won't necessarily
			* correspond to an exact internal value.
			*
			* \remarks The time mode can't have a default value, because
			*         otherwise SetTime(int, int, int, int, int, int)
			*         would be ambiguous. Please specify DEFAULT_MODE.
			*/
			void SetTime(int hour,int minute,int second,int frame,int field,int residual, 
				TimeMode timeMode,double framerate);

			/** Get time in hour/minute/second/frame/field/residual format.
			* \param pHour The returned hours value.
			* \param pMinute The returned minutes value.
			* \param pSecond The returned seconds value.
			* \param pFrame The returned frames values.
			* \param pField The returned field value.
			* \param pResidual The returned hundreths of frame value.
			* \param pTimeMode The time mode identifier which will dictate the extraction algorithm.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return \c true if the pTimeMode parameter is a valid identifier and thus the extraction
			* succeeded. If the function returns \c false, all the values are set to 0.
			*/
			bool GetTime([OutAttribute]kLongLong %hour,[OutAttribute]kLongLong minute,
				[OutAttribute]kLongLong %second,[OutAttribute]kLongLong %frame, 
				[OutAttribute]kLongLong %field,[OutAttribute]kLongLong %residual, 
				TimeMode timeMode,double framerate);

			// Return a Time Snapped on the NEAREST(rounded) Frame (if asked)
			FbxTime^ GetFramedTime(bool round);

			/** Get number of hours in time.
			* \param pCummul This parameter has no effect.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Hours value.
			*/
			kLongLong GetHour(bool cummul,TimeMode timeMode, double framerate);

			/** Get number of minutes in time.
			* \param pCummul If \c true, get total number of minutes. If \c false, get number of 
			* minutes exceeding last full hour.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Minutes value.
			*/
			kLongLong GetMinute(bool cummul,TimeMode timeMode, double framerate);

			/** Get number of seconds in time.
			* \param pCummul If \c true, get total number of seconds. If \c false, get number of 
			* seconds exceeding last full minute.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Seconds value.
			*/
			kLongLong GetSecond(bool cummul,TimeMode timeMode, double framerate);

			/** Get number of frames in time.
			* \param pCummul If \c true, get total number of frames. If \c false, get number of 
			* frames exceeding last full second.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Frames values.
			*/
			kLongLong GetFrame(bool cummul,TimeMode timeMode, double framerate);

			/** Get number of fields in time.
			* \param pCummul If \c true, get total number of fields. If \c false, get number of 
			* fields exceeding last full frame.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Fields value.
			*/
			kLongLong GetField(bool cummul,TimeMode timeMode, double framerate);

			/** Get residual time exceeding last full field.
			* \param pTimeMode Time mode identifier.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return Residual value.
			*/
			kLongLong GetResidual(TimeMode timeMode, double framerate);

			/** Get time in a human readable format.
			* \param pTimeString An array large enough to contain a minimum of 19 characters.
			* \param pInfo The amount of information if time protocol is \c eSMPTE:
			* <ul><li>1 means hours only
			*     <li>2 means hours and minutes
			*     <li>3 means hours, minutes and seconds
			*     <li>4 means hours, minutes, seconds and frames
			*     <li>5 means hours, minutes, seconds, frames and field
			*     <li>6 means hours, minutes, seconds, frames, field and residual value</ul>
			* \param pTimeMode Requested time mode.
			* \param pTimeFormat Requested time protocol.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			* \return pTimeString parameter filled with a time value or set to a empty string
			* if parameter pInfo is not valid.
			*/
			String^ GetTimeString (String^ timeString,int info,TimeMode timeMode, 
				TimeProtocol timeFormat,double framerate);

			/** Set time in a human readable format.
			* \param pTime An array of a maximum of 18 characters.
			* If time protocol is \c eSMPTE, pTimeString must be formatted this way:
			* "[hours:]minutes[:seconds[.frames[.fields]]]". Hours, minutes, seconds, 
			* frames and fields are parsed as integers and brackets indicate optional 
			* parts. 
			* If time protocol is \c eFRAME, pTimeString must be formatted this way:
			* "frames". Frames is parsed as a 64 bits integer.
			* \param pTimeMode Given time mode.
			* \param pTimeFormat Given time protocol.
			* \param pFramerate indicate custom framerate in case of ptimemode = eCUSTOM
			*/
			void SetTimeString(String^ time,TimeMode timeMode,TimeProtocol timeFormat, double framerate);

			//@}

			/**
			* \name Time Operators
			*/
			//@{

			//! Equality operator.
			virtual bool Equals(Object^ obj) override
			{
				FbxTime^ o = dynamic_cast<FbxTime^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}							


			//! Superior or equal to operator.
			static bool operator >=(FbxTime^ t1,FbxTime^ t2)
			{
				return *t1->_FbxTime >= *t2->_FbxTime;
			}

			//! Inferior or equal to operator.
			static bool operator <=(FbxTime^ t1,FbxTime^ t2)
			{
				return *t1->_FbxTime <= *t2->_FbxTime;
			}

			//! Superior to operator.
			static bool operator >(FbxTime^ t1,FbxTime^ t2)
			{
				return *t1->_FbxTime > *t2->_FbxTime;
			}

			//! Inferior to operator.
			static bool operator <(FbxTime^ t1,FbxTime^ t2)
			{
				return *t1->_FbxTime < *t2->_FbxTime;
			}

			//! Assignment operator.
			void CopyFrom(FbxTime^ time)
			{								
				*_FbxTime = *time->_FbxTime;							
			}

			//! Addition operator.
			static FbxTime^ operator +=(FbxTime^ t1,FbxTime^ t2)
			{
				*t1->_FbxTime += *t1->_FbxTime;
				return t1;
			}

			//! Subtraction operator.
			/*static FbxTime^ operator -=(FbxTime^ t1,FbxTime^ t2)
			{
			*t1->_FbxTime -= *t1->_FbxTime;
			return t1;
			}*/

			//! Addition operator.
			static FbxTime^ operator +(FbxTime^ t1,FbxTime^ t2)
			{
				return gcnew FbxTime(*t1->_FbxTime + *t1->_FbxTime);								
			}

			//! Subtraction operator.
			static FbxTime^ operator -(FbxTime^ t1,FbxTime^ t2)
			{
				return gcnew FbxTime(*t1->_FbxTime - *t1->_FbxTime);								
			}

			//! Multiplication operator.
			static FbxTime^ operator *(FbxTime^ t1,int v)
			{
				return gcnew FbxTime(*t1->_FbxTime * v);								
			}

			//! Division operator.
			static FbxTime^ operator /(FbxTime^ t1,FbxTime^ t2)
			{
				return gcnew FbxTime(*t1->_FbxTime / *t1->_FbxTime);								
			}

			//! Multiplication operator.
			static FbxTime^ operator *(FbxTime^ t1,FbxTime^ t2)
			{
				return gcnew FbxTime(*t1->_FbxTime * *t1->_FbxTime);								
			}

			//! Increment time of one unit of the internal format.
			//inline KTime &operator++() {mTime += 1; return (*this);}

			//! Decrement time of one unit of the internal format.
			//inline KTime &operator--() {mTime -= 1; return (*this);}

			//@}
			static FbxTime^ GetSystemTimer();
		};

		/** Class to encapsulate time intervals.
		* \nosubgrouping
		*/
		public ref class FbxTimeSpan : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxTimeSpan,KTimeSpan);
			INATIVEPOINTER_DECLARE(FbxTimeSpan,KTimeSpan);		

		public:

			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxTimeSpan,KTimeSpan);

			/** Constructor.
			* \param pStart Beginning of the time interval.
			* \param pStop  Ending of the time interval.
			*/
			FbxTimeSpan(FbxTime^ start, FbxTime^ stop);


			/** Set start and stop time.
			* \param pStart Beginning of the time interval.
			* \param pStop  Ending of the time interval.
			*/
			void Set(FbxTime^ start, FbxTime^ stop);

			/** Get start time.
			* \return Beginning of time interval.
			*/
			/** Set start time.
			* \param pStart Beginning of the time interval.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxTime,Start);

			/** Get stop time.
			* \return Ending of time interval.
			*/
			/** Set stop time.
			* \param pStop  Ending of the time interval.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxTime,Stop);				

			/** Get time interval in absolute value.
			* \return Time interval.
			*/
			FbxTime^ GetDuration();

			/** Get time interval.
			* \return Signed time interval.
			*/
			//FbxTime^ GetSignedDuration();

			/** Get direction of the time interval.
			* \return \c KTS_FORWARD if time interval is forward, \c KTS_BACKWARD if backward.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Direction);

			//! Return \c true if the time is inside the timespan.
			//bool operator&(KTime &pTime) const;

			//! Return the intersection of the two time spans.
			//KTimeSpan operator&(KTimeSpan &pTime) const;

			//! Return the intersection of the two time spans.
			//bool operator!=(KTimeSpan &pTime);					
		};


		public enum class FbxOldTimeMode
		{
			DefaultMode = eOLD_DEFAULT_MODE ,		//!< Default mode set using KTime::SetGlobalTimeMode (ETimeMode pTimeMode)
			Cinema = eOLD_CINEMA,			//!< 24 frameOLD_s/s
			Pal = eOLD_PAL,				//!< 25 frameOLD_s/s	 PAL/SECAM
			FrameS30 = eOLD_FRAMES30,			//!< 30 frameOLD_s/s	 BLACK & WHITE NTSC
			NtscDropFrame = eOLD_NTSC_DROP_FRAME,   //!< 29.97002617 frameOLD_s/s COLOR NTSC
			FrameS50 = eOLD_FRAMES50,			//!< 50 frameOLD_s/s
			FrameS60 = eOLD_FRAMES60,			//!< 60 frameOLD_s/s
			FrameS100 = eOLD_FRAMES100 ,			//!< 100 frameOLD_s/s
			FrameS120 = eOLD_FRAMES120 ,			//!< 120 frameOLD_s/s
			NtscFullFrame = eOLD_NTSC_FULL_FRAME ,	//!< 29.97002617 frameOLD_s/s COLOR NTSC
			FrameS30_DROP = eOLD_FRAMES30_DROP ,		//!< 30 frameOLD_s/s
			FrameS1000 = eOLD_FRAMES1000 ,		//!< 1000 frameOLD_s/s
			TimeModeCount = eOLD_TIME_MODE_COUNT 
		};

		//public ref class FbxTimeModeObject : IFbxNativePointer
		//{
		//	BASIC_CLASS_DECLARE(FbxTimeModeObject,KTimeModeObject);
		//	INATIVEPOINTER_DECLARE(FbxTimeModeObject,KTimeModeObject,"KTimeModeObject")		
		//	public:
		//		//double				mFrameRateValue;
		//		//char*				mFrameRateString;//!< *** use to store/retrieve the framerate in files, do not change existing value
		//		//KTime::ETimeMode    mTimeMode;
		//		//EOldTimeMode        mOldTimeMode;
		//		//char*				mTimeModeString;//!< Use to Display in the UI
		//		//int					mShowValue;     //!< UI in TC view -> bit 0 set to 1 ..... UI in FR view -> bit 1 set to 1
		//		////!< 0 = don't show this mode in TC or FR view, 1 = show in TC view only, 2 = show in FR only, 3 = show in FR and TC
		//		//bool                mSystemRate;    //!< Indicate that this is a built-in value, false indicate it is a custom value
		//		//bool                mExistInOldValue;//!< Indicate if this value exist in the old value enum
		//		//kLongLong           mOneFrameValue;  //!< indicate internal value for 1 frame
		//};


	}
}