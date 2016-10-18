#ifndef _FBXSDK_UTILS_MANIPULATORS_H_
#define _FBXSDK_UTILS_MANIPULATORS_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/core/math/fbxvector2.h>
#include <fbxsdk/core/math/fbxvector4.h>
#include <fbxsdk/scene/geometry/fbxcamera.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCameraManipulationState
class FBXSDK_DLL FbxCameraManipulator : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxCameraManipulator, FbxObject)
public:
	enum EAction
		eNone,		
		eOrbit,		
		eDolly,		
		ePan,		
		eFreePan	
	void Begin(EAction pAction, float pX, float pY)
	void Notify(float pX, float pY, float pScale=0)
	void End()
	void Update(const FbxTime& pTimeDelta=FBXSDK_TIME_ZERO)
	void Action(EAction pAction, float pX, float pY, float pScale=0)
	EAction GetCurrentAction() const
	void FrameAll(const FbxTime& pTime=FBXSDK_TIME_INFINITE)
	void FrameSelected(const FbxTime& pTime=FBXSDK_TIME_INFINITE)
	void FrameScreenPosition(float pX, float pY, bool pCulling=false, const FbxTime& pTime=FBXSDK_TIME_INFINITE)
	FbxPropertyT<FbxFloat> ViewportWidth
	FbxPropertyT<FbxFloat> ViewportHeight
	FbxPropertyT<FbxDouble> SmoothSpeed
	FbxPropertyT<FbxBool> InvertY
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	virtual void ConstructProperties(bool pForceSet)
	virtual bool ConnectNotify(const FbxConnectEvent& pEvent)
	virtual bool PropertyNotify(EPropertyNotifyType pType, FbxProperty& pProperty)
private:
	void		Reset()
	FbxCamera*	GetCamera() const
	FbxNode*	GetCameraNode() const
	FbxNode*	GetCameraLookAtNode() const
	FbxNode*	GetCameraTargetUpNode() const
	FbxVector4	GetCameraPosition() const
	void		SetCameraPosition(const FbxVector4& pPosition)
	FbxVector4	GetCameraRotation() const
	void		SetCameraRotation(const FbxVector4& pRotation)
	FbxVector4	GetCameraLookAtPosition() const
	void		SetCameraLookAtPosition(const FbxVector4& pPosition)
	FbxVector4	GetCameraTargetUpPosition() const
	void		SetCameraTargetUpPosition(const FbxVector4& pPosition)
	FbxAMatrix	GetCameraRotationMatrix() const
	void		SetCameraRotationMatrix(const FbxAMatrix& pRM)
	double		ComputeRotationAxis(FbxVector4& pFront, FbxVector4& pUp, FbxVector4& pRight, const FbxVector4& pEye, const FbxVector4& pLookAt, const FbxVector4& pUpVector) const
	void		ComputeRotationMatrix(FbxAMatrix& pRM, const FbxVector4& pEye, const FbxVector4& pLookAt, const FbxVector4& pUpVector)
	void		UpdateCameraRotation()
	bool		FrameObjects(bool pSelected, const FbxTime& pTime)
	FbxVector4	ComputePositionToFitBBoxInFrustum(const FbxVector4& pBBoxMin, const FbxVector4& pBBoxMax, const FbxVector4& pBBoxCenter, const FbxVector4& pCameraPosition, const FbxAMatrix& pCameraRM, const FbxTime& pTime)
	EAction		mCurrentAction
	FbxFloat	mBeginMouse[3], mLastMouse[3]
	FbxVector4	mBeginPosition, mBeginAxis[3]
	FbxBool		mBeginFlipped
	FbxDouble	mDestOrthoZoom
	FbxVector4	mDestPosition, mDestLookAt, mDestTargetUp
	FbxAMatrix	mDestRotation
  	FbxVector4	mInitialPosition, mInitialRotation, mInitialLookAt
#endif 