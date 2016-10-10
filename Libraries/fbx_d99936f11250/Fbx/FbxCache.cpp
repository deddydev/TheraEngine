#pragma once
#include "stdafx.h"
#include "FbxCache.h"
#include "FbxObject.h"
#include "FbxString.h"
#include "FbxTime.h"
#include "FbxError.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"



{
	namespace FbxSDK
	{				
		void FbxCacheManaged::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxObjectManaged::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxCacheManaged,KFbxCache);

		FbxCacheManaged::FileFormat FbxCacheManaged::CacheFileFormat::get()
		{
			return (FileFormat)_Ref()->GetCacheFileFormat();
		}
		void FbxCacheManaged::CacheFileFormat::set(FileFormat value)
		{
			_Ref()->SetCacheFileFormat((KFbxCache::EFileFormat)value);
		}			
		void FbxCacheManaged::SetCacheFileName(System::String^ relativeFileName, System::String^ absoluteFileName)
		{
			STRINGTO_CONSTCHAR_ANSI(r,relativeFileName);
			STRINGTO_CONSTCHAR_ANSI(a,absoluteFileName);
			_Ref()->SetCacheFileName(r,a);
			FREECHARPOINTER(r);
			FREECHARPOINTER(a);
		}
		void FbxCacheManaged::GetCacheFileName([OutAttribute]String^ %relativeFileName, [OutAttribute]String^ %absoluteFileName)
		{
			FbxString r;
			FbxString a;
			_Ref()->GetCacheFileName(r,a);

			CONVERT_FbxString_TO_STRING(r,rStr);
			CONVERT_FbxString_TO_STRING(a,aStr);

			relativeFileName = rStr;
			absoluteFileName = aStr;
		}
		bool FbxCacheManaged::OpenFileForRead()
		{
			return _Ref()->OpenFileForRead();
		}
		bool FbxCacheManaged::IsOpen::get()
		{
			return _Ref()->IsOpen();
		}			
		bool FbxCacheManaged::CloseFile()
		{
			return _Ref()->CloseFile();
		}
		double FbxCacheManaged::SamplingFrameRate::get()
		{
			return _Ref()->GetSamplingFrameRate();
		}			
		FbxTime^ FbxCacheManaged::CacheTimePerFrame::get()
		{
			return gcnew FbxTime(_Ref()->GetCacheTimePerFrame());
		}					
		bool FbxCacheManaged::OpenFileForWrite(MCFileCount fileCount, double samplingFrameRate, System::String^ channelName, MCDataType CDataType)
		{
			STRINGTO_CONSTCHAR_ANSI(ch,channelName);			
			bool b = _Ref()->OpenFileForWrite((KFbxCache::EMCFileCount)fileCount,samplingFrameRate, ch, (KFbxCache::EMCDataType)CDataType);
			FREECHARPOINTER(ch);
			return b;
		}
		bool FbxCacheManaged::OpenFileForWrite(MCFileCount fileCount, double samplingFrameRate, System::String^ channelName)
		{
			STRINGTO_CONSTCHAR_ANSI(ch,channelName);			
			bool b = _Ref()->OpenFileForWrite((KFbxCache::EMCFileCount)fileCount,samplingFrameRate, ch);
			FREECHARPOINTER(ch);
			return b;
		}
		int  FbxCacheManaged::ChannelCount::get()
		{
			return _Ref()->GetChannelCount();
		}			
		bool FbxCacheManaged::GetChannelName(int channelIndex, [OutAttribute]String^ %channelName)
		{
			FbxString k;
			bool b = _Ref()->GetChannelName(channelIndex,k);
			CONVERT_FbxString_TO_STRING(k,str);
			channelName = str;
			return b;
		}
		bool FbxCacheManaged::GetChannelDataType(int channelIndex, MCDataType %channelType)			
		{
			bool b = false;
			KFbxCache::EMCDataType d;
			b = _Ref()->GetChannelDataType(channelIndex,d);
			if(b)				
				channelType =(MCDataType)d;
			return b;
		}
		int FbxCacheManaged::GetChannelIndex(System::String^ channelName)
		{
			STRINGTO_CONSTCHAR_ANSI(ch,channelName);
			int i = _Ref()->GetChannelIndex(ch);
			FREECHARPOINTER(ch);
			return i;
		}
		bool FbxCacheManaged::GetAnimationRange(int channelIndex, FbxTime^ timeStart, FbxTime^ timeEnd)
		{
			return _Ref()->GetAnimationRange(channelIndex,*timeStart->_Ref(),*timeEnd->_Ref());
		}
		bool FbxCacheManaged::GetCacheType(MCFileCount %fileCount)
		{
			bool b;
			KFbxCache::EMCFileCount c;
			b = _Ref()->GetCacheType(c);
			if(b)
				fileCount = (MCFileCount)c;
			return b;
		}
		bool FbxCacheManaged::GetChannelInterpretation(int channelIndex, [OutAttribute]String^ %interpretation)
		{
			FbxString k;
			bool b = _Ref()->GetChannelInterpretation(channelIndex,k);
			CONVERT_FbxString_TO_STRING(k,str);
			interpretation = str;
			return b;
		}			
		bool FbxCacheManaged::GetChannelSamplingType(int channelIndex, FbxCacheManaged::MCSamplingType %samplingType)
		{
			bool b;
			KFbxCache::EMCSamplingType c;
			b = _Ref()->GetChannelSamplingType(channelIndex, c);
			if(b)
				samplingType = (MCSamplingType)c;
			return b;
		}
		bool FbxCacheManaged::GetChannelSamplingRate(int channelIndex, FbxTime^ samplingRate)
		{
			return _Ref()->GetChannelSamplingRate(channelIndex,*samplingRate->_Ref());
		}
		bool FbxCacheManaged::GetChannelSampleCount(int channelIndex, unsigned int %sampleCount)
		{
			bool b;
			unsigned int i;
			b = _Ref()->GetChannelSampleCount(channelIndex, i);
			sampleCount = i;
			return b;
		}
		bool FbxCacheManaged::GetChannelPointCount(int channelIndex, FbxTime^ time, unsigned int %pointCount)
		{
			bool b;
			unsigned int i;
			b = _Ref()->GetChannelPointCount(channelIndex,*time->_Ref(),i);
			pointCount = i;
			return b;
		}
		int  FbxCacheManaged::CacheDataFileCount::get()
		{
			return _Ref()->GetCacheDataFileCount();
		}			
		bool FbxCacheManaged::GetCacheDataFileName(int index, FbxStringManaged^ %relativeFileName, FbxStringManaged^ %absoluteFileName)
		{
			return _Ref()->GetCacheDataFileName(index,*relativeFileName->_Ref(),*absoluteFileName->_Ref());
		}
		bool FbxCacheManaged::EnableMultiChannelFetching(bool multiChannelFetching)
		{
			return _Ref()->EnableMultiChannelFetching(multiChannelFetching);
		}
		bool FbxCacheManaged::OpenFileForWrite(double frameStartOffset, double samplingFrameRate, unsigned int sampleCount, unsigned int pointCount)
		{
			return _Ref()->OpenFileForWrite(frameStartOffset,samplingFrameRate,sampleCount,pointCount);

		}
		unsigned int FbxCacheManaged::SampleCount::get()
		{
			return _Ref()->GetSampleCount();
		}			
		unsigned int FbxCacheManaged::PointCount::get()
		{
			return _Ref()->GetPointCount();
		}			
		double FbxCacheManaged::FrameStartOffset::get()
		{
			return _Ref()->GetFrameStartOffset();
		}
		bool FbxCacheManaged::ConvertFromPC2ToMC(MCFileCount fileCount, double samplingFrameRate)
		{
			return  _Ref()->ConvertFromPC2ToMC((KFbxCache::EMCFileCount)fileCount,samplingFrameRate);
		}
		bool FbxCacheManaged::ConvertFromMCToPC2(double samplingFrameRate, unsigned int channelIndex)
		{
			return  _Ref()->ConvertFromMCToPC2(samplingFrameRate,channelIndex);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCacheManaged,GetError(),FbxErrorManaged,KError);

		FbxCacheManaged::Error FbxCacheManaged::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}
		System::String^ FbxCacheManaged::LastErrorString::get()
		{
			return gcnew System::String(_Ref()->GetLastErrorString());
		}

		/*System::String^ FbxCache::CacheFilePropertyName::get()
		{			
		cacheFilePropertyName = gcnew System::String(KFbxCache::CacheFilePropertyName);
		return cacheFilePropertyName;
		}

		System::String^ FbxCache::CacheFileAbsolutePathPropertyName::get()
		{
		if(cacheFileAbsolutePathPropertyName == nullptr)
		cacheFileAbsolutePathPropertyName = gcnew System::String(KFbxCache::CacheFileAbsolutePathPropertyName);
		return cacheFileAbsolutePathPropertyName;
		}		
		System::String^ FbxCache::CacheFileTypePropertyName::get()
		{
		if(cacheFileTypePropertyName == nullptr)
		cacheFileTypePropertyName = gcnew System::String(KFbxCache::CacheFileTypePropertyName);
		return cacheFileTypePropertyName;
		}*/
#ifndef DOXYGEN_SHOULD_SKIP_THIS
		CLONE_DEFINITION(FbxCacheManaged,KFbxCache);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}