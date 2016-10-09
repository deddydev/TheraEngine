#pragma once
#include "stdafx.h"
#include "FbxVideo.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxError.h"
#include "FbxTime.h"


namespace Skill
{
	namespace FbxSDK
	{	
		void FbxVideo::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxVideo,KFbxVideo);
		void FbxVideo::Reset()
		{
			_Ref()->Reset();
		}		
		bool FbxVideo::ImageTextureMipMap::get()
		{
			return _Ref()->ImageTextureGetMipMap();
		}
		void FbxVideo::ImageTextureMipMap::set(bool value)
		{
			_Ref()->ImageTextureSetMipMap(value);
		}
		String^ FbxVideo::FileName::get()
		{
			FbxString kstr = _Ref()->GetFileName();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		void FbxVideo::FileName::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);						
			_Ref()->SetFileName(v);
			FREECHARPOINTER(v);
		}

		String^ FbxVideo::RelativeFileName::get()
		{
			FbxString kstr = _Ref()->GetRelativeFileName();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		void FbxVideo::RelativeFileName::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);						
			_Ref()->SetRelativeFileName(v);
			FREECHARPOINTER(v);
		}

		double FbxVideo::FrameRate::get()
		{
			return _Ref()->GetFrameRate();
		}

		int FbxVideo::LastFrame::get()
		{
			return _Ref()->GetLastFrame();			
		}

		int FbxVideo::Width::get()
		{
			return _Ref()->GetWidth();			
		}
		int FbxVideo::Height::get()
		{
			return _Ref()->GetHeight();			
		}

		int FbxVideo::StartFrame::get()
		{
			return _Ref()->GetStartFrame();			
		}
		void FbxVideo::StartFrame::set(int value)
		{
			_Ref()->SetStartFrame(value);			
		}

		int FbxVideo::StopFrame::get()
		{
			return _Ref()->GetStopFrame();			
		}
		void FbxVideo::StopFrame::set(int value)
		{
			_Ref()->SetStopFrame(value);			
		}

		double FbxVideo::PlaySpeed::get()
		{
			return _Ref()->GetPlaySpeed();
		}
		void FbxVideo::PlaySpeed::set(double value)
		{
			_Ref()->SetPlaySpeed(value);
		}

		FbxTime^ FbxVideo::Offset::get()
		{
			return gcnew FbxTime(_Ref()->GetOffset());
		}
		void FbxVideo::Offset::set(FbxTime^ value)
		{
			_Ref()->SetOffset(*value->_Ref());
		}

		bool FbxVideo::FreeRunning::get()
		{
			return _Ref()->GetFreeRunning();
		}
		void FbxVideo::FreeRunning::set(bool value)
		{
			_Ref()->SetFreeRunning(value);
		}

		bool FbxVideo::Loop::get()
		{
			return _Ref()->GetLoop();
		}
		void FbxVideo::Loop::set(bool value)
		{
			_Ref()->SetLoop(value);
		}

		FbxVideo::InterlaceMode FbxVideo::Interlace_Mode::get()
		{
			return (FbxVideo::InterlaceMode)_Ref()->GetInterlaceMode();
		}
		void FbxVideo::Interlace_Mode::set(FbxVideo::InterlaceMode value)
		{
			_Ref()->SetInterlaceMode((KFbxVideo::EInterlaceMode)value);
		}

		FbxVideo::AccessMode FbxVideo::Access_Mode::get()
		{
			return (FbxVideo::AccessMode)_Ref()->GetAccessMode();
		}
		void FbxVideo::Access_Mode::set(FbxVideo::AccessMode value)
		{
			_Ref()->SetAccessMode((KFbxVideo::EAccessMode)value);
		}		

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxVideo,GetError(),FbxErrorManaged,KError);

		FbxVideo::Error FbxVideo::LastErrorID::get()
		{
			return (FbxVideo::Error)_Ref()->GetLastErrorID();
		}
		String^ FbxVideo::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxVideo,KFbxVideo); 

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}