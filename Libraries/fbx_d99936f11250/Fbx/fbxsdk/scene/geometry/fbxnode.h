#ifndef _FBXSDK_SCENE_GEOMETRY_NODE_H_
#define _FBXSDK_SCENE_GEOMETRY_NODE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/core/math/fbxtransforms.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxStatus
class FbxNodeAttribute
class FbxCachedEffect
class FbxLODGroup
class FbxNull
class FbxMarker
class FbxSkeleton
class FbxGeometry
class FbxMesh
class FbxNurbs
class FbxNurbsCurve
class FbxLine
class FbxNurbsSurface
class FbxTrimNurbsSurface
class FbxPatch
class FbxCamera
class FbxCameraStereo
class FbxCameraSwitcher
class FbxLight
class FbxOpticalReference
class FbxSubDiv
class FbxCharacter
class FbxSurfaceMaterial
class FbxAnimStack
class FbxAnimCurveFilterMatrixConverter
class FBXSDK_DLL FbxNode : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxNode, FbxObject)
public:
		FbxNode* GetParent()
		const FbxNode* GetParent() const
		bool AddChild(FbxNode* pNode)
		FbxNode* RemoveChild(FbxNode* pNode)
		int GetChildCount(bool pRecursive = false) const
		FbxNode* GetChild(int pIndex)
		const FbxNode* GetChild(int pIndex) const
		FbxNode* FindChild(const char* pName, bool pRecursive=true, bool pInitial=false)
		void SetTarget(FbxNode* pNode)
		FbxNode* GetTarget() const
		void SetPostTargetRotation(FbxVector4 pVector)
		FbxVector4 GetPostTargetRotation() const
		void SetTargetUp(FbxNode* pNode)
		FbxNode* GetTargetUp() const
		void SetTargetUpVector(FbxVector4 pVector)
		FbxVector4 GetTargetUpVector() const
		void SetVisibility(bool pIsVisible)
		bool GetVisibility() const
		enum EShadingMode
			eHardShading,		
			eWireFrame,			
			eFlatShading,		
			eLightShading,		
			eTextureShading,	
			eFullShading		
		void SetShadingMode(EShadingMode pShadingMode)
		EShadingMode GetShadingMode() const
		FbxNodeAttribute* SetNodeAttribute(FbxNodeAttribute* pNodeAttribute)
		FbxNodeAttribute* GetNodeAttribute()
		const FbxNodeAttribute* GetNodeAttribute() const
		int GetNodeAttributeCount() const
		int GetDefaultNodeAttributeIndex() const
		bool SetDefaultNodeAttributeIndex(int pIndex, FbxStatus* pStatus = NULL)
		FbxNodeAttribute* GetNodeAttributeByIndex(int pIndex)
		const FbxNodeAttribute* GetNodeAttributeByIndex(int pIndex) const
		int GetNodeAttributeIndex(FbxNodeAttribute* pNodeAttribute, FbxStatus* pStatus = NULL) const
		bool AddNodeAttribute(FbxNodeAttribute* pNodeAttribute, FbxStatus* pStatus = NULL)
		FbxNodeAttribute* RemoveNodeAttribute(FbxNodeAttribute* pNodeAttribute)
		FbxNodeAttribute* RemoveNodeAttributeByIndex(int pIndex)
		FbxCachedEffect* GetCachedEffect()
		FbxLODGroup* GetLodGroup()
		FbxNull* GetNull()
		FbxMarker* GetMarker()
		FbxSkeleton* GetSkeleton()
		FbxGeometry* GetGeometry()
		FbxMesh* GetMesh()
		FbxNurbs* GetNurbs()
		FbxNurbsSurface* GetNurbsSurface()
		FbxNurbsCurve* GetNurbsCurve()
		FbxLine* GetLine()
		FbxTrimNurbsSurface* GetTrimNurbsSurface()
		FbxSubDiv* GetSubdiv()
		FbxPatch* GetPatch()
		FbxCamera* GetCamera()
		const FbxCamera* GetCamera() const
		FbxCameraStereo* GetCameraStereo()
		FbxCameraSwitcher* GetCameraSwitcher()
		FbxLight* GetLight()
		const FbxLight* GetLight() const
		FbxOpticalReference* GetOpticalReference()
		void SetTransformationInheritType(FbxTransform::EInheritType pInheritType)
		void GetTransformationInheritType(FbxTransform::EInheritType& pInheritType) const
		enum EPivotSet
			eSourcePivot,		
			eDestinationPivot	
		enum EPivotState
			ePivotActive,	
			ePivotReference	
		void SetPivotState(EPivotSet pPivotSet, EPivotState pPivotState)
		void GetPivotState(EPivotSet pPivotSet, EPivotState& pPivotState) const
		void SetRotationOrder(EPivotSet pPivotSet, EFbxRotationOrder pRotationOrder)
		void GetRotationOrder(EPivotSet pPivotSet, EFbxRotationOrder& pRotationOrder) const
		void SetUseRotationSpaceForLimitOnly(EPivotSet pPivotSet, bool pUseForLimitOnly)
		bool GetUseRotationSpaceForLimitOnly(EPivotSet pPivotSet) const
		void SetRotationActive(bool pVal)
		bool GetRotationActive() const
		void SetQuaternionInterpolation(EPivotSet pPivotSet, EFbxQuatInterpMode pQuatIterp)
		EFbxQuatInterpMode GetQuaternionInterpolation(EPivotSet pPivotSet) const
		void SetRotationStiffness(FbxVector4 pRotationStiffness)
		FbxVector4 GetRotationStiffness() const
		void SetMinDampRange(FbxVector4 pMinDampRange)
		FbxVector4 GetMinDampRange() const
		void SetMaxDampRange(FbxVector4 pMaxDampRange)
		FbxVector4 GetMaxDampRange() const
		void SetMinDampStrength(FbxVector4 pMinDampStrength)
		FbxVector4 GetMinDampStrength() const
		void SetMaxDampStrength(FbxVector4 pMaxDampStrength)
		FbxVector4 GetMaxDampStrength() const
		void SetPreferedAngle(FbxVector4 pPreferedAngle)
		FbxVector4 GetPreferedAngle() const
		void SetRotationOffset(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetRotationOffset(EPivotSet pPivotSet) const
		void SetRotationPivot(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetRotationPivot(EPivotSet pPivotSet) const
		void SetPreRotation(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetPreRotation(EPivotSet pPivotSet) const
		void SetPostRotation(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetPostRotation(EPivotSet pPivotSet) const
		void SetScalingOffset(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetScalingOffset(EPivotSet pPivotSet) const
		void SetScalingPivot(EPivotSet pPivotSet, FbxVector4 pVector)
		const FbxVector4& GetScalingPivot(EPivotSet pPivotSet) const
		void SetGeometricTranslation(EPivotSet pPivotSet, FbxVector4 pVector)
		FbxVector4 GetGeometricTranslation(EPivotSet pPivotSet) const
		void SetGeometricRotation(EPivotSet pPivotSet, FbxVector4 pVector)
		FbxVector4 GetGeometricRotation(EPivotSet pPivotSet) const
		void SetGeometricScaling(EPivotSet pPivotSet, FbxVector4 pVector)
		FbxVector4 GetGeometricScaling(EPivotSet pPivotSet) const
		void ResetPivotSet( FbxNode::EPivotSet pPivotSet )
		void ConvertPivotAnimationRecursive(FbxAnimStack* pAnimStack, EPivotSet pConversionTarget, double pFrameRate, bool pKeyReduce=true)
		void ResetPivotSetAndConvertAnimation(double pFrameRate=30.0, bool pKeyReduce=false, bool pToNodeCenter=true, bool pForceResetLimits=false)
		void SetRotationPivotAsCenterRecursive(FbxVector4 pParentGeometricOffset=FbxVector4())
		FbxAnimEvaluator* GetAnimationEvaluator() const
		FbxAMatrix& EvaluateGlobalTransform(FbxTime pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
		FbxAMatrix& EvaluateLocalTransform(FbxTime pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
		FbxVector4& EvaluateLocalTranslation(FbxTime pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
		FbxVector4& EvaluateLocalRotation(FbxTime pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
		FbxVector4& EvaluateLocalScaling(FbxTime pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
		bool EvaluateGlobalBoundingBoxMinMaxCenter(FbxVector4& pBBoxMin, FbxVector4& pBBoxMax, FbxVector4& pBBoxCenter, const FbxTime& pTime=FBXSDK_TIME_INFINITE)
		bool EvaluateRayIntersectionPoint(FbxVector4& pOut, const FbxVector4& pRayOrigin, const FbxVector4& pRayDir, bool pCulling=false, const FbxTime& pTime=FBXSDK_TIME_INFINITE)
		int GetCharacterLinkCount() const
		bool GetCharacterLink(int pIndex, FbxCharacter** pCharacter, int* pCharacterLinkType, int* pNodeId, int* pNodeSubId)
		int FindCharacterLink(FbxCharacter* pCharacter, int pCharacterLinkType, int pNodeId, int pNodeSubId) const
	bool GetAnimationInterval(FbxTimeSpan& pInterval, FbxAnimStack* pAnimStack=NULL, int pAnimLayerId=0)
		int AddMaterial(FbxSurfaceMaterial* pMaterial)
		bool RemoveMaterial(FbxSurfaceMaterial* pMaterial)
		int GetMaterialCount() const
		FbxSurfaceMaterial* GetMaterial(int pIndex) const
		void RemoveAllMaterials()
		int GetMaterialIndex(const char* pName) const
		FbxPropertyT<FbxDouble3> LclTranslation
		FbxPropertyT<FbxDouble3> LclRotation
		FbxPropertyT<FbxDouble3> LclScaling
		FbxPropertyT<FbxDouble> Visibility
		FbxPropertyT<FbxBool> VisibilityInheritance
		FbxPropertyT<EFbxQuatInterpMode> QuaternionInterpolate
		FbxPropertyT<FbxDouble3> RotationOffset
		FbxPropertyT<FbxDouble3> RotationPivot
		FbxPropertyT<FbxDouble3> ScalingOffset
		FbxPropertyT<FbxDouble3> ScalingPivot
		FbxPropertyT<FbxBool> TranslationActive
		FbxPropertyT<FbxDouble3> TranslationMin
		FbxPropertyT<FbxDouble3> TranslationMax
		FbxPropertyT<FbxBool> TranslationMinX
		FbxPropertyT<FbxBool> TranslationMinY
		FbxPropertyT<FbxBool> TranslationMinZ
		FbxPropertyT<FbxBool> TranslationMaxX
		FbxPropertyT<FbxBool> TranslationMaxY
		FbxPropertyT<FbxBool> TranslationMaxZ
		FbxPropertyT<EFbxRotationOrder> RotationOrder
		FbxPropertyT<FbxBool> RotationSpaceForLimitOnly
		FbxPropertyT<FbxDouble> RotationStiffnessX
		FbxPropertyT<FbxDouble> RotationStiffnessY
		FbxPropertyT<FbxDouble> RotationStiffnessZ
		FbxPropertyT<FbxDouble> AxisLen
		FbxPropertyT<FbxDouble3> PreRotation
		FbxPropertyT<FbxDouble3> PostRotation
		FbxPropertyT<FbxBool> RotationActive
		FbxPropertyT<FbxDouble3> RotationMin
		FbxPropertyT<FbxDouble3> RotationMax
		FbxPropertyT<FbxBool> RotationMinX
		FbxPropertyT<FbxBool> RotationMinY
		FbxPropertyT<FbxBool> RotationMinZ
		FbxPropertyT<FbxBool> RotationMaxX
		FbxPropertyT<FbxBool> RotationMaxY
		FbxPropertyT<FbxBool> RotationMaxZ
		FbxPropertyT<FbxTransform::EInheritType> InheritType
		FbxPropertyT<FbxBool> ScalingActive
		FbxPropertyT<FbxDouble3> ScalingMin
		FbxPropertyT<FbxDouble3> ScalingMax
		FbxPropertyT<FbxBool> ScalingMinX
		FbxPropertyT<FbxBool> ScalingMinY
		FbxPropertyT<FbxBool> ScalingMinZ
		FbxPropertyT<FbxBool> ScalingMaxX
		FbxPropertyT<FbxBool> ScalingMaxY
		FbxPropertyT<FbxBool> ScalingMaxZ
		FbxPropertyT<FbxDouble3> GeometricTranslation
		FbxPropertyT<FbxDouble3> GeometricRotation
		FbxPropertyT<FbxDouble3> GeometricScaling
		FbxPropertyT<FbxDouble> MinDampRangeX
		FbxPropertyT<FbxDouble> MinDampRangeY
		FbxPropertyT<FbxDouble> MinDampRangeZ
		FbxPropertyT<FbxDouble> MaxDampRangeX
		FbxPropertyT<FbxDouble> MaxDampRangeY
		FbxPropertyT<FbxDouble> MaxDampRangeZ
		FbxPropertyT<FbxDouble> MinDampStrengthX
		FbxPropertyT<FbxDouble> MinDampStrengthY
		FbxPropertyT<FbxDouble> MinDampStrengthZ
		FbxPropertyT<FbxDouble> MaxDampStrengthX
		FbxPropertyT<FbxDouble> MaxDampStrengthY
		FbxPropertyT<FbxDouble> MaxDampStrengthZ
		FbxPropertyT<FbxDouble> PreferedAngleX
		FbxPropertyT<FbxDouble> PreferedAngleY
		FbxPropertyT<FbxDouble> PreferedAngleZ
		FbxPropertyT<FbxReference> LookAtProperty
		FbxPropertyT<FbxReference> UpVectorProperty
		FbxPropertyT<FbxBool> Show
		FbxPropertyT<FbxBool> NegativePercentShapeSupport
		FbxPropertyT<FbxInt> DefaultAttributeIndex
		FbxPropertyT<FbxBool> Freeze
		FbxPropertyT<FbxBool> LODBox
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	class FBXSDK_DLL Pivot
	public:
		static const FbxVector4 sZeroVector
		static const FbxVector4 sOneVector
		Pivot()
			mRotationOffset = NULL
			mRotationPivot = NULL
			mPreRotation = NULL
			mPostRotation = NULL
			mScalingOffset = NULL
			mScalingPivot = NULL
			mGeometricTranslation = NULL
			mGeometricRotation = NULL
			mGeometricScaling = NULL
			Reset()
		~Pivot() 
 Reset()
		void Reset()
			FBX_SAFE_DELETE(mRotationOffset)
			FBX_SAFE_DELETE(mRotationPivot)
			FBX_SAFE_DELETE(mPreRotation)
			FBX_SAFE_DELETE(mPostRotation)
			FBX_SAFE_DELETE(mScalingOffset)
			FBX_SAFE_DELETE(mScalingPivot)
			FBX_SAFE_DELETE(mGeometricTranslation)
			FBX_SAFE_DELETE(mGeometricRotation)
			FBX_SAFE_DELETE(mGeometricScaling)
			mRotationOrder = eEulerXYZ
			mRotationSpaceForLimitOnly = false
			mPivotState = FbxNode::ePivotReference
			mQuaternionInterpolate  = eQuatInterpOff
		inline const FbxVector4& GetRotationOffset() const 
 return (mRotationOffset) ? *mRotationOffset : sZeroVector
		inline void SetRotationOffset(const FbxVector4& pV)
			if( !mRotationOffset )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mRotationOffset = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mRotationOffset = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mRotationOffset = pV
		inline const FbxVector4& GetRotationPivot() const 
 return (mRotationPivot) ? *mRotationPivot : sZeroVector
		inline void SetRotationPivot(const FbxVector4& pV)
			if( !mRotationPivot )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mRotationPivot = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mRotationPivot = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mRotationPivot = pV
		inline const FbxVector4& GetPreRotation() const 
 return (mPreRotation) ? *mPreRotation : sZeroVector
		inline void SetPreRotation(const FbxVector4& pV)
			if( !mPreRotation )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mPreRotation = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mPreRotation = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mPreRotation = pV
		inline const FbxVector4& GetPostRotation() const 
 return (mPostRotation) ? *mPostRotation : sZeroVector
		inline void SetPostRotation(const FbxVector4& pV)
			if( !mPostRotation )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mPostRotation = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mPostRotation = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mPostRotation = pV
		inline const FbxVector4& GetScalingOffset() const 
 return (mScalingOffset) ? *mScalingOffset : sZeroVector
		inline void SetScalingOffset(const FbxVector4& pV)
			if( !mScalingOffset )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mScalingOffset = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else                    
				mScalingOffset = FbxNew< FbxVector4 >(pV)
			#endif                        
			else
				*mScalingOffset = pV
		inline const FbxVector4& GetScalingPivot() const 
 return (mScalingPivot) ? *mScalingPivot : sZeroVector
		inline void SetScalingPivot(const FbxVector4& pV)
			if( !mScalingPivot )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mScalingPivot = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else                    
				mScalingPivot = FbxNew< FbxVector4 >(pV)
			#endif                        
			else
				*mScalingPivot = pV
		inline const FbxVector4& GetGeometricTranslation() const 
 return (mGeometricTranslation) ? *mGeometricTranslation : sZeroVector
		inline void SetGeometricTranslation(const FbxVector4& pV)
			if( !mGeometricTranslation )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mGeometricTranslation = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mGeometricTranslation = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mGeometricTranslation = pV
		inline const FbxVector4& GetGeometricRotation() const 
 return (mGeometricRotation) ? *mGeometricRotation : sZeroVector
		inline void SetGeometricRotation(const FbxVector4& pV)
			if( !mGeometricRotation )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mGeometricRotation = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mGeometricRotation = FbxNew< FbxVector4 >(pV)
			#endif
			else
				*mGeometricRotation = pV
		inline const FbxVector4& GetGeometricScaling() const 
 return (mGeometricScaling) ? *mGeometricScaling : sOneVector
		inline void SetGeometricScaling(const FbxVector4& pV)
			if( !mGeometricScaling )
			#if defined(__GNUC__) && (__GNUC__ < 4)
				mGeometricScaling = FbxNew< FbxVector4 >((FbxVector4&)pV)
			#else
				mGeometricScaling = FbxNew< FbxVector4 >(pV)
			#endif                        
			else
				*mGeometricScaling = pV
		inline EFbxRotationOrder GetRotationOrder() const 
 return mRotationOrder
		inline void SetRotationOrder(EFbxRotationOrder pROrder) 
 mRotationOrder = pROrder
		inline bool GetRotationSpaceForLimitOnly() const 
 return mRotationSpaceForLimitOnly
		inline void SetRotationSpaceForLimitOnly(bool pVal) 
 mRotationSpaceForLimitOnly = pVal
		inline EFbxQuatInterpMode GetQuaternionInterpolate() const 
 return mQuaternionInterpolate
		inline void SetQuaternionInterpolate(EFbxQuatInterpMode pVal) 
 mQuaternionInterpolate = pVal
		inline FbxNode::EPivotState GetPivotState() const 
 return mPivotState
		inline void SetPivotState(FbxNode::EPivotState pVal) 
 mPivotState = pVal
	private:
		FbxVector4*				mRotationOffset
		FbxVector4*				mRotationPivot
		FbxVector4*				mPreRotation
		FbxVector4*				mPostRotation
		FbxVector4*				mScalingOffset
		FbxVector4*				mScalingPivot
		FbxVector4*				mGeometricTranslation
		FbxVector4*				mGeometricRotation
		FbxVector4*				mGeometricScaling
		EFbxRotationOrder		mRotationOrder
		bool					mRotationSpaceForLimitOnly
		EFbxQuatInterpMode		mQuaternionInterpolate
		FbxNode::EPivotState	mPivotState
	class FBXSDK_DLL Pivots
	public:
		Pivots()
			for( int i = 0
 i < 2
 i++ )
				mIsDefault[i] = true
				mPivotState[i] = FbxNode::ePivotReference
				mPivot[i] = NULL
		~Pivots()
			FbxDelete(mPivot[0])
			FbxDelete(mPivot[1])
		Pivot& Get(int id)
			FBX_ASSERT(id == 0 || id == 1)
			if (mPivot[id] == NULL && mIsDefault[id])
				smDefaultPivot.SetPivotState(mPivotState[id])
				return smDefaultPivot
			if (!mPivot[id])
				mPivot[id] = FbxNew< Pivot >()
			FBX_ASSERT(mPivot[id] != NULL)
			if (mPivot[id])
				mPivot[id]->SetPivotState(mPivotState[id])
			return *mPivot[id]
		#define MACRO_PIVOT_VECTOR_FCTS(name, defVect) \
			inline const FbxVector4& Get##name(int id) const \
\
				FBX_ASSERT(id == 0 || id == 1)
 \
				Pivot* p = mPivot[id]
 \
				if (p == NULL) p = &smDefaultPivot
 \
				return p->Get##name()
 \
\
			inline void Set##name(int id, const FbxVector4& pV) \
\
				FBX_ASSERT(id == 0 || id == 1)
 \
				if (mIsDefault[id] && pV[0] == defVect[0] && pV[1] == defVect[1] && pV[2] == defVect[2]) return
 \
				mIsDefault[id] = false
 \
				Get(id).Set##name(pV)
 \
		MACRO_PIVOT_VECTOR_FCTS(RotationOffset, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(RotationPivot, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(PreRotation, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(PostRotation, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(ScalingOffset, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(ScalingPivot, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(GeometricTranslation, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(GeometricRotation, Pivot::sZeroVector)
		MACRO_PIVOT_VECTOR_FCTS(GeometricScaling, Pivot::sOneVector)
		#define MACRO_PIVOT_BOOL_FCTS(name) \
			inline bool Get##name(int id) const \
\
				FBX_ASSERT(id == 0 || id == 1)
 \
				Pivot* p = mPivot[id]
 \
				if (p == NULL) p = &smDefaultPivot
 \
				return p->Get##name()
 \
\
			inline void Set##name(int id, bool pV) \
\
				FBX_ASSERT(id == 0 || id == 1)
 \
				mIsDefault[id] = false
 \
				Get(id).Set##name(pV)
 \
		MACRO_PIVOT_BOOL_FCTS(RotationSpaceForLimitOnly)
		inline EFbxQuatInterpMode GetQuaternionInterpolate(int id) const
			FBX_ASSERT(id == 0 || id == 1)
			Pivot* p = mPivot[id]
			if (p == NULL) p = &smDefaultPivot
			return p->GetQuaternionInterpolate()
		inline void SetQuaternionInterpolate(int id, EFbxQuatInterpMode pV)
			FBX_ASSERT(id == 0 || id == 1)
			if (mIsDefault[id] && pV == eQuatInterpOff) return
			mIsDefault[id] = false
			Get(id).SetQuaternionInterpolate(pV)
		inline EFbxRotationOrder GetRotationOrder(int id) const
			FBX_ASSERT(id == 0 || id == 1)
			Pivot* p = mPivot[id]
			if (p == NULL) p = &smDefaultPivot
			return p->GetRotationOrder()
		inline void SetRotationOrder(int id, EFbxRotationOrder pROrder)
			FBX_ASSERT(id == 0 || id == 1)
			if (mIsDefault[id] && pROrder == eEulerXYZ) return
			mIsDefault[id] = false
			Get(id).SetRotationOrder(pROrder)
		inline FbxNode::EPivotState GetPivotState(int id) const
			FBX_ASSERT(id == 0 || id == 1)
			return mPivotState[id]
		inline void SetPivotState(int id, FbxNode::EPivotState pVal)
			FBX_ASSERT(id == 0 || id == 1)
			if (pVal == FbxNode::ePivotReference) return
			mPivotState[id] = pVal
			if (mPivot[id])
				mPivot[id]->SetPivotState(pVal)
		#undef MACRO_PIVOT_VECTOR_FCTS
		#undef MACRO_PIVOT_BOOL_FCTS
		void Reset()
			smDefaultPivot.Reset()
			for (int i = 0
 i < 2
 i++)
				mIsDefault[i] = true
				mPivotState[i] = FbxNode::ePivotReference
				if (mPivot[i]) mPivot[i]->Reset()
	private:
		Pivot*					mPivot[2]
		FbxNode::EPivotState	mPivotState[2]
		bool					mIsDefault[2]
		static Pivot			smDefaultPivot
	class FBXSDK_DLL LinkToCharacter
	public:
		bool operator==(LinkToCharacter& pLinkToCharacter)
			if (mCharacter == pLinkToCharacter.mCharacter &&
				mType == pLinkToCharacter.mType &&
				mIndex == pLinkToCharacter.mIndex &&
				mSubIndex == pLinkToCharacter.mSubIndex)
				return true
			else return false
		FbxCharacter* mCharacter
		int mType
		int mIndex
		int mSubIndex
    void					AddChildName(char* pChildName)
    char*					GetChildName(FbxUInt pIndex) const
    FbxUInt					GetChildNameCount() const
    FbxTransform&			GetTransform()
	FbxLimits&				GetTranslationLimits()
	FbxLimits&				GetRotationLimits()
	FbxLimits&				GetScalingLimits()
	Pivots&					GetPivots()
    void					UpdatePivotsAndLimitsFromProperties()
    void					UpdatePropertiesFromPivotsAndLimits()
    void					SetRotationActiveProperty(bool pVal)
    void					PivotSetToMBTransform(EPivotSet pPivotSet)
    int						AddCharacterLink(FbxCharacter* pCharacter, int pCharacterLinkType, int pNodeId, int pNodeSubId)
    int						RemoveCharacterLink(FbxCharacter* pCharacter, int pCharacterLinkType, int pNodeId, int pNodeSubId)
    FbxNode*                DeepCloneWithNodeAttributes()
    virtual FbxObject&		Copy(const FbxObject& pObject)
    virtual const char*		GetTypeName() const
    virtual FbxStringList	GetTypeFlags() const
    virtual bool			PropertyNotify(EPropertyNotifyType pType, FbxProperty& pProperty)
    enum ECullingType
		eCullingOff,
		eCullingOnCCW,
		eCullingOnCW
    ECullingType			mCullingType
    bool					mCorrectInheritType
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
	void				Reset()
	bool				GetAnimationIntervalRecursive(FbxTimeSpan& pTimeInterval, FbxAnimLayer* pAnimLayer)
private:
	typedef FbxSet<FbxHandle> GeomInstSet
	void				ResetLimitsRecursive(FbxNode* pNode)
	void				ConvertPivotAnimationRecurseLoop(FbxAnimStack* pAnimStack, const EPivotSet pConversionTarget, const double pFrameRate, const bool pKeyReduce, GeomInstSet& pGeomInstSet)
	void				ConvertPivotAnimation(FbxAnimStack* pAnimStack, const EPivotSet pConversionTarget, const double pFrameRate, const bool pKeyReduce, GeomInstSet& pGeomInstSet)
	bool				ConvertPivotAnimation_SetupMatrixConverter(FbxAnimCurveFilterMatrixConverter& pConverter, const EPivotSet& pSrcSet, const EPivotSet& pDstSet, const double pFrameRate, const bool pKeyReduce, GeomInstSet& pGeomInstSet)
	void				ConvertPivotAnimation_ApplyGeometryPivot(const EPivotSet& pSrcSet, const EPivotSet& pDstSet, GeomInstSet& pGeomInstSet)
	FbxTransform				mTransform
	Pivots						mPivots
	FbxObject*					mAnimCurveNodeContainer
	FbxArray<FbxString*>		mChildrenNameList
	FbxVector4					mPostTargetRotation
	FbxVector4					mTargetUpVector
	FbxNode::EShadingMode		mShadingMode
	FbxArray<LinkToCharacter>	mLinkToCharacter
#endif 