#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"



{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxErrorManaged;
		/**	FBX SDK video class.
		* \nosubgrouping
		*/
		public ref class FbxVideo : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxVideo);
		internal:
			FbxVideo(KFbxVideo* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}			
			FBXOBJECT_DECLARE(FbxVideo);
		protected:
			virtual void CollectManagedMemory() override;
		public:
			/**
			*\name Reset vedio
			*/
			//@{

			//! Reset the video to default values.
			void Reset();
			//@}

			/**
			* \name Video attributes Management
			*/
			//@{						

			/** Retrieve use MipMap state.
			* \return          MipMap flag state.
			*/
			/** Set the use of MipMap on the video.
			* \param pUseMipMap If \c true, use MipMap on the video.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ImageTextureMipMap);

			/** Retrieve the Video full filename.
			* \return          Video full filename.
			*/
			/** Specify the Video full filename.
			* \param pName     Video full filename.
			* \return          \c True,if update successfully, \c false otherwise.
			* \remarks         Update the texture filename if the connection exists.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,FileName);			

			/** Retrieve the Video relative filename.
			* \return         Video relative filename.
			*/
			/** Specify the Video relative filename.
			* \param pName     Video relative filename.
			* \return          \c True, if update successfully, \c false otherwise.
			* \remark          Update the texture filename if the connection exists.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,RelativeFileName);			

			/** Retrieve the Frame rate of the video clip.
			* \return        Frame rate.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,FrameRate);

			/** Retrieve the last frame of the video clip.
			* \return       Last frame number.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,LastFrame);

			/** Retrieve the clip width.
			* \return      Video image width.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Width);

			/** Retrieve the clip height.
			* \return      Video image height.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Height);			

			/** Retrieve the start frame of the video clip.
			* \return     Start frame number.
			*/
			/** Set the start frame of the video clip.
			* \param pStartFrame     Start frame number.
			* \remarks               The parameter value is not checked. It is the responsibility
			*                        of the caller to deal with bad frame numbers.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(int,StartFrame);			

			/** Retrieve the stop frame of the video clip.
			* \return     Stop frame number.
			*/
			/** Set the stop frame of the video clip.
			* \param pStopFrame     Stop frame number.
			* \remarks              The parameter value is not checked. It is the responsibility
			*                       of the caller to deal with bad frame numbers.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(int,StopFrame);
			
			/** Retrieve the play speed of the video clip.
			* \return Playback     speed.
			*/
			/** Set the play speed of the video clip.
			* \param pPlaySpeed     Playback speed of the clip.
			* \remarks             The parameter value is not checked. It is the responsibility
			*                      of the caller to deal with bad playback speed values.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,PlaySpeed);			

			/* Retrieve the time offset.
			* \return     The current time shift.
			*/
			/** Set the time offset.
			* The offset can be used to shift the playback start time of the clip.
			* \param pTime     Time offset of the clip.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTime^,Offset);
			
			/** Retrieve the Free Running state.
			* \return     Current free running flag.
			*/
			/** Set the Free Running state of the video clip.
			* The Free Running flag can be used by a client application to implement a
			* playback scheme that is independent of the main timeline.
			* \param pState     State of the Free running flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,FreeRunning);			

			/** Retrieve the Loop state.
			* \return     Current loop flag.
			*/
			/** Set the Loop state of the video clip.
			* The Loop flag can be used by a client application to implement the loop
			* playback of the video clip.
			* \param pLoop     State of the loop flag.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Loop);


			/** \enum EInterlaceMode Video interlace modes.
			* - \e Node
			* - \e Fields
			* - \e HalfEven
			* - \e HalfOdd
			* - \e FullEven
			* - \e FullOdd
			* - \e FullEvenOdd
			* - \e FullOddEven
			*/
			enum class InterlaceMode
			{
				None = KFbxVideo::None,       // Progressive frame (full frame)
				Fields = KFbxVideo::Fields,     // Alternate even/odd fields
				HalfEven = KFbxVideo::HalfEven,   // Half of a frame, even fields only
				HalfOdd = KFbxVideo::HalfOdd,    // Half of a frame, odd fields only
				FullEven = KFbxVideo::FullEven,   // Extract and use the even field of a full frame
				FullOdd = KFbxVideo::FullOdd,    // Extract and use the odd field of a full frame
				FullEvenOdd = KFbxVideo::FullEvenOdd, // Extract Fields and make full frame with each one beginning with Even (60fps)
				FullOddEven = KFbxVideo::FullOddEven
			};

			/** Retrieve the Interlace mode
			* \return     Interlace mode identifier.
			*/
			/** Set the Interlace mode.
			* \param pInterlaceMode     Interlace mode identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(InterlaceMode,Interlace_Mode);


			/** \enum EAccessMode Video clip access mode.
			* - \e Disk
			* - \e Memory
			* - \e DiskAsync
			*/
			enum class AccessMode
			{
				Disk = KFbxVideo::Disk,
				Memory = KFbxVideo::Memory,
				DiskAsync = KFbxVideo::DiskAsync
			};			

			/** Retrieve the clip Access Mode.
			* \return     Clip access mode identifier.
			*/
			/** Set the clip Access Mode.
			* \param pAccessMode     Clip access mode identifier.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(AccessMode,Access_Mode);
			//@}


			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eTAKE_NODE_ERROR
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				TakeNodeError = KFbxVideo::eTAKE_NODE_ERROR,
				ErrorCount = KFbxVideo::eERROR_COUNT
			};

			/** Get last error code.
			* \return     Last error code.
			*/
			VALUE_PROPERTY_GET_DECLARE(Error,LastErrorID);

			/** Get last error string.
			* \return     Textual description of the last error.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,LastErrorString);			

			//@}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE(); 

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}