#pragma once
#include "stdafx.h"
#include "FbxTime.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxTime;
		ref class FbxStringManaged;
		ref class FbxErrorManaged;
		/** This class contains functions for accessing global time settings.
		* \nosubgrouping
		*/
		public ref class FbxGlobalTimeSettings : IFbxNativePointer
		{			
			INTERNAL_CLASS_DECLARE(FbxGlobalTimeSettings,KFbxGlobalTimeSettings);
			REF_DECLARE(FbxGlobalTimeSettings,KFbxGlobalTimeSettings);
			DESTRUCTOR_DECLARE_2(FbxGlobalTimeSettings);
			INATIVEPOINTER_DECLARE(FbxGlobalTimeSettings,KFbxGlobalTimeSettings);		

		public:

			//! Restore default settings.
			void RestoreDefaultSettings();

			/** Get time mode.
			* \return     The currently set TimeMode.
			*/			
			/** Set time mode.
			* \param pTimeMode     One of the defined modes in class KTime.
			*/
			property FbxTime::TimeMode  Mode
			{
				FbxTime::TimeMode get();
				void set(FbxTime::TimeMode value);
			}			

			/** Get time protocol.
			* \return     The currently set TimeProtocol.
			*/			
			/** Set time protocol.
			* \param pTimeProtocol     One of the defined protocols in class KTime.
			*/
			property FbxTime::TimeProtocol  Protocol
			{
				FbxTime::TimeProtocol get();
				void set(FbxTime::TimeProtocol value);
			}				

			/** \enum ESnapOnFrameMode Snap on frame mode
			* - \e eNO_SNAP
			* - \e eSNAP_ON_FRAME
			* - \e ePLAY_ON_FRAME
			* - \e eSNAP_PLAY_ON_FRAME
			*/
			enum class SnapOnFrameMode
			{
				NoSnap = KFbxGlobalTimeSettings::eNO_SNAP,
				SnapOnFrame = KFbxGlobalTimeSettings::eSNAP_ON_FRAME ,
				PlayOnFrame = KFbxGlobalTimeSettings::ePLAY_ON_FRAME,
				SnapPlayOnFrame = KFbxGlobalTimeSettings::eSNAP_PLAY_ON_FRAME
			};

			/** Get snap on frame mode.
			* \return     The currently set FrameMode.
			*/			
			/** Set snap on frame mode.
			* \param pSnapOnFrameMode     One of the following values: eNO_SNAP, eSNAP_ON_FRAME, ePLAY_ON_FRAME, or eSNAP_PLAY_ON_FRAME.
			*/
			property SnapOnFrameMode  TimeSnapOnFrameMode
			{
				SnapOnFrameMode get();
				void set(SnapOnFrameMode value);
			}			

			/**
			* \name Timeline Time span
			*/
			//@{

			/** Set Timeline default time span
			* \param pTimeSpan The time span of the time line.
			*/
			void SetTimelineDefautTimeSpan(FbxTimeSpan^ timeSpan);

			/** Get Timeline default time span
			* \param pTimeSpan return the default time span for the time line.
			*/
			void GetTimelineDefautTimeSpan(FbxTimeSpan^ timeSpan);

			/**
			* \name Time Markers
			*/
			//@{

			ref struct FbxTimeMarker : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxTimeMarker,KFbxGlobalTimeSettings::KFbxTimeMarker);
				INATIVEPOINTER_DECLARE(FbxTimeMarker,KFbxGlobalTimeSettings::KFbxTimeMarker);			
			public:
				DEFAULT_CONSTRUCTOR(FbxTimeMarker,KFbxGlobalTimeSettings::KFbxTimeMarker);

				FbxTimeMarker(FbxTimeMarker^ timeMarker);
				void CopyFrom(FbxTimeMarker^ timeMarker);

				property String^ Name
				{
					String^ get();
					void set(String^ value);
				}
				REF_PROPERTY_GETSET_DECLARE(FbxTime,Time);

				property bool Loop
				{
					bool get();
					void set(bool value);
				}
			};

			/** Get number of time markers.
			* \return     The number of time markers.
			*/
			property int TimeMarkerCount
			{
				int get();
			}

			/** Set current time marker index.
			* \param pIndex     Current time marker index.
			* \return           \c true if successful, or \c false if pIndex is invalid.
			* \remarks          If pIndex is invalid, KFbxGlobalTimeSettings::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			bool SetCurrentTimeMarker(int index);

			/** Get current time marker index.
			* \return     Current time marker index, or -1 if the current time marker has not been set.
			*/			
			property int CurrentTimeMarker
			{
				int get();
			}

			/** Get time marker at given index.
			* \param pIndex     Time marker index.
			* \return           Pointer to the time marker at pIndex, or \c NULL if the index is out of range.
			* \remarks          If pIndex is out of range, KFbxGlobalTimeSettings::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			FbxTimeMarker^ GetTimeMarker(int index);

			/** Add a time marker.
			* \param     pTimeMarker New time marker.
			*/
			void AddTimeMarker(FbxTimeMarker^ timeMarker);

			//! Remove all time markers and set current time marker to -1.
			void RemoveAllTimeMarkers();

			//@}

			//! Assignment operator.
			void CopyFrom(FbxGlobalTimeSettings^ settings);

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			*  \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** Error identifiers.
			* Most of these are only used internally.
			* - \e eINDEX_OUT_OF_RANGE
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				IndexOutOfRange = KFbxGlobalTimeSettings::eINDEX_OUT_OF_RANGE,
				ErrorCount= KFbxGlobalTimeSettings::eERROR_COUNT
			};

			/** Get last error code.
			*  \return   Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			*  \return   Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}	

			//@}

			/**
			* \name Obsolete Functions
			* These functions still work but are no longer relevant.
			*/
			//@{

			/** Get snap on frame flag
			* \return      \c true if snap on frame mode is set to either eSNAP_ON_FRAME or ePLAY_ON_FRAME. \c false if snap on frame mode is set to \c eNO_SNAP.
			* \remarks     This function is replaced by KFbxGlobalTimeSettings::GetSnapOnFrameMode().
			*/
			/** Set snap on frame flag.
			* \param pSnapOnFrame     If \c true, snap on frame mode is set to eSNAP_ON_FRAME. If \c false, snap on frame mode is set to \c eNO_SNAP.
			* \remarks                This function is replaced by KFbxGlobalTimeSettings::SetSnapOnFrameMode().
			*/
			property bool SnapOnFrame
			{
				bool get();
				void set(bool value);
			}


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}