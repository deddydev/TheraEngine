#ifndef _FBXSDK_SCENE_VIDEO_H_
#define _FBXSDK_SCENE_VIDEO_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxVideo : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxVideo, FbxObject)
public:
		void Reset()
		void ImageTextureSetMipMap(bool pUseMipMap)
		bool ImageTextureGetMipMap() const
		bool SetFileName(const char* pName)
		FbxString GetFileName () const
		bool SetRelativeFileName(const char* pName)
		const char* GetRelativeFileName() const
		void SetImageSequence(bool pImageSequence)
		bool GetImageSequence() const
		void SetImageSequenceOffset(int pOffset)
		int GetImageSequenceOffset() const
		double GetFrameRate() const
		int GetLastFrame() const
		int GetWidth() const
		int GetHeight() const
		void SetStartFrame(int pStartFrame)
		int GetStartFrame() const
		void SetStopFrame(int pStopFrame)
		int GetStopFrame() const
		void SetPlaySpeed(double pPlaySpeed)
		double GetPlaySpeed() const
		void SetOffset(FbxTime pTime)
		FbxTime GetOffset() const
		void SetFreeRunning(bool pState)
		bool GetFreeRunning() const
		void SetLoop(bool pLoop)
		bool GetLoop() const
		enum EInterlaceMode
			eNone,           
			eFields,         
			eHalfEven,       
			eHalfOdd,        
			eFullEven,       
			eFullOdd,        
			eFullEvenOdd,    
			eFullOddEven     
		void SetInterlaceMode(EInterlaceMode pInterlaceMode)
		EInterlaceMode GetInterlaceMode() const
		enum EAccessMode
			eDisk,
			eMemory,
			eDiskAsync
		void SetAccessMode(EAccessMode pAccessMode)
		EAccessMode GetAccessMode() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
    virtual bool ConnectNotify(FbxConnectEvent const &pEvent)
public:
	virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxStringList GetTypeFlags() const
    void SetOriginalFormat(bool pState)
    bool GetOriginalFormat() const
    void SetOriginalFilename(const char* pOriginalFilename)
    const char* GetOriginalFilename() const
	FbxPropertyT<FbxBool>   ImageSequence
	FbxPropertyT<FbxInt> ImageSequenceOffset
    FbxPropertyT<FbxDouble> FrameRate
    FbxPropertyT<FbxInt> LastFrame
    FbxPropertyT<FbxInt> Width
    FbxPropertyT<FbxInt> Height
    FbxPropertyT<FbxString> Path
    FbxPropertyT<FbxInt> StartFrame
    FbxPropertyT<FbxInt> StopFrame
    FbxPropertyT<FbxDouble> PlaySpeed
    FbxPropertyT<FbxTime> Offset
    FbxPropertyT<EInterlaceMode> InterlaceMode
    FbxPropertyT<FbxBool> FreeRunning
    FbxPropertyT<FbxBool> Loop
    FbxPropertyT<EAccessMode> AccessMode
protected:
    void Init()
    bool		mUseMipMap
    bool		mOriginalFormat
    FbxString	mOriginalFilename
    FbxString	mRelativeFilename
#endif 