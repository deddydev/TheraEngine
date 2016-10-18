#ifndef _FBXSDK_SCENE_SHADING_BINDING_OPERATOR_H_
#define _FBXSDK_SCENE_SHADING_BINDING_OPERATOR_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxbindingtablebase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBindingOperator : public FbxBindingTableBase
    FBXSDK_OBJECT_DECLARE(FbxBindingOperator, FbxBindingTableBase)
public:
    template <class FBXTYPE>
    bool Evaluate(const FbxObject* pObject, FBXTYPE* pResult) const
        EFbxType lResultType
        void* lResult = NULL
        bool lSuccess = Evaluate(pObject, &lResultType, &lResult)
        if (lSuccess)
            FbxTypeCopy(*pResult, lResult, lResultType)
        FreeEvaluationResult(lResultType, lResult)
        return lSuccess
    template <class FBXTYPE>
    bool ReverseEvaluation(const FbxObject* pObject, FBXTYPE * pInOut, 
                            bool setObj=false, int index=0) const
        const void* lIn = pInOut
        void* lOut = NULL
        EFbxType lOutType
        bool lSuccess = ReverseEvaluate(pObject, lIn, &lOut, &lOutType, setObj, index)
        if (lSuccess)
            FbxTypeCopy(*pInOut, lOut, lOutType)
        FreeEvaluationResult(lOutType, lOut)
        return lSuccess
    template <class FBXTYPE>
    bool EvaluateEntry(const FbxObject* pObject, const char* pEntryDestinationName, FBXTYPE* pResult) const
        EFbxType lResultType
        void* lResult = NULL
        bool lSuccess = EvaluateEntry(pObject, pEntryDestinationName, &lResultType, &lResult)
        if (lSuccess)
            FbxTypeCopy(*pResult, lResult, lResultType)
        FreeEvaluationResult(lResultType, lResult)
        return lSuccess
    FbxPropertyT<FbxString> FunctionName
    FbxPropertyT<FbxString> TargetName
    static const char* sFunctionName
    static const char* sTargetName
    static const char* sDefaultFunctionName
    static const char* sDefaultTargetName
    static void RegisterFunctions()
    static void UnregisterFunctions()
    class FBXSDK_DLL Function
    public:
        virtual ~Function() 
        virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const = 0
		virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const = 0
    class FBXSDK_DLL FunctionCreatorBase
    public:
        virtual ~FunctionCreatorBase() 
        virtual const char* GetFunctionName() const = 0
        virtual Function* CreateFunction() const = 0
    template <class FUNCTION>
    class FunctionCreator : public FunctionCreatorBase
    public:
        virtual const char* GetFunctionName() const
            return FUNCTION::FunctionName
        virtual Function* CreateFunction() const
            return FbxNew< FUNCTION >()
    class FBXSDK_DLL FunctionRegistry
    public:
        static void RegisterFunctionCreator(FunctionCreatorBase const& pCreator)
            sRegistry.Insert(pCreator.GetFunctionName(), &pCreator)
        static void UnregisterFunctionCreator(FunctionCreatorBase const& pCreator)
            sRegistry.Remove(pCreator.GetFunctionName())
        static const FunctionCreatorBase* FindCreator(const char* pName)
            RegistryType::RecordType* lRecord = sRegistry.Find(pName)
            if (lRecord)
                return lRecord->GetValue()
            else
                return NULL
    private:
        typedef FbxMap<const char*, const FunctionCreatorBase*, FbxCharPtrCompare> RegistryType
        static RegistryType sRegistry
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    bool EvaluateEntry(const FbxObject* pObject, const char* pEntryDestinationName, EFbxType* pResultType, void** pResult) const
    bool GetEntryProperty(const FbxObject* pObject, const char* pEntryDestinationName, FbxProperty & pProp) const
protected:
    virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    virtual void ConstructProperties(bool pForceSet)
    void InstantiateFunction()
    bool Evaluate(const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    bool ReverseEvaluate(const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
    void FreeEvaluationResult(EFbxType pResultType, void* pResult) const
    Function* mFunction
#endif 
class FbxNodePositionBOF : public FbxBindingOperator::Function
public:
	static const char* FunctionName
	virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
	virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	FbxNodePositionBOF()
	virtual ~FbxNodePositionBOF()
#endif 
class FbxNodeDirectionBOF : public FbxBindingOperator::Function
public:
	static const char* FunctionName
	virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
	virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	FbxNodeDirectionBOF()
	virtual ~FbxNodeDirectionBOF()
#endif 
class FbxAssignBOF : public FbxBindingOperator::Function
public:
    static const char* FunctionName
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
	virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxAssignBOF()
    virtual ~FbxAssignBOF()
#endif 
class FbxConditionalBOF : public FbxBindingOperator::Function
public:
    static const char* FunctionName
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxConditionalBOF()
    virtual ~FbxConditionalBOF()
#endif 
class FbxSwitchBOF : public FbxBindingOperator::Function
public:
    static const char* FunctionName
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxSwitchBOF()
    virtual ~FbxSwitchBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxTRSToMatrixBOF()
    virtual ~FbxTRSToMatrixBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxAddBOF()
    virtual ~FbxAddBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxSubstractBOF()
    virtual ~FbxSubstractBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
	virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxMultiplyBOF()
    virtual ~FbxMultiplyBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxMultiplyDistBOF()
    virtual ~FbxMultiplyDistBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxOneOverXBOF()
    virtual ~FbxOneOverXBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxPowerBOF()
    virtual ~FbxPowerBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxDegreeToRadianBOF()
    virtual ~FbxDegreeToRadianBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxVectorDegreeToVectorRadianBOF()
    virtual ~FbxVectorDegreeToVectorRadianBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxSphericalToCartesianBOF()
    virtual ~FbxSphericalToCartesianBOF()
#endif 
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxIsYupBOF()
    virtual ~FbxIsYupBOF()
#endif 
class FbxSymbolIDBOF : public FbxBindingOperator::Function
public:
    static const char* FunctionName
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
    virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxSymbolIDBOF()
    virtual ~FbxSymbolIDBOF()
#endif 
class FbxSpotDistributionChooserBOF : public FbxBindingOperator::Function
public:
    static const char* FunctionName
    virtual bool Evaluate(const FbxBindingOperator* pOperator, const FbxObject* pObject, EFbxType* pResultType, void** pResult) const
	virtual bool ReverseEvaluate(const FbxBindingOperator* pOperator, const FbxObject* pTarget, const void* pIn, void** pOut, EFbxType* pOutType, bool setObj, int index) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxSpotDistributionChooserBOF()
    virtual ~FbxSpotDistributionChooserBOF()
#endif 