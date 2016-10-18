#ifndef _FBXSDK_CORE_ARCH_DEBUG_H_
#define _FBXSDK_CORE_ARCH_DEBUG_H_
#define FBXSDK_ASSERT_ENVSTR "FBXSDK_ASSERT"
typedef void (*FbxAssertProc)(const char* pFileName, const char* pFunctionName, const unsigned int pLineNumber, const char* pMessage);
FBXSDK_DLL void FbxAssertSetProc(FbxAssertProc pAssertProc);
FBXSDK_DLL void FbxAssertSetDefaultProc();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FBXSDK_DLL void _FbxAssert(const char* pFileName, const char* pFunctionName, const unsigned int pLineNumber, bool pFormat, const char* pMessage, ...);
FBXSDK_DLL void _FbxTrace(const char* pMessage, ...);
#ifdef _DEBUG
template <bool x> struct FbxStaticAssertType;
template<> struct FbxStaticAssertType<true>   {enum{value=1};};
template<> struct FbxStaticAssertType<false>  {enum{value=-1};};
#define FBX_ASSERT(Condition)      {if(!(Condition)){_FbxAssert(__FILE__,__FUNCTION__,__LINE__,false,#Condition);}}
#define FBX_ASSERT_MSG(Condition, Message, ...)  {if(!(Condition)){_FbxAssert(__FILE__,__FUNCTION__,__LINE__,true,Message,##__VA_ARGS__);}}
#define FBX_ASSERT_NOW(Message, ...)    _FbxAssert(__FILE__,__FUNCTION__,__LINE__,true,Message,##__VA_ARGS__);
#define FBX_ASSERT_RETURN(Condition)    {if(!(Condition)){FBX_ASSERT_NOW(#Condition); return;}}
#define FBX_ASSERT_RETURN_VALUE(Condition, Value) {if(!(Condition)){FBX_ASSERT_NOW(#Condition); return Value;}}
#define FBX_ASSERT_STATIC(Condition)    typedef char FbxBuildBreakIfFalse[FbxStaticAssertType<(bool)(Condition)>::value];
#define FBX_TRACE(Message, ...)      {_FbxTrace(Message,##__VA_ARGS__);}
#else
#define FBX_ASSERT(Condition)      ((void)0)
#define FBX_ASSERT_MSG(Condition, Message, ...)  ((void)0)
#define FBX_ASSERT_NOW(Message, ...)    ((void)0)
#define FBX_ASSERT_RETURN(Condition)    if(!(Condition)){return;}
#define FBX_ASSERT_RETURN_VALUE(Condition, Value) if(!(Condition)){return Value;}
#define FBX_ASSERT_STATIC(Condition)
#define FBX_TRACE(Message, ...)      ((void)0)
#endif
template<typename T> struct FbxIncompatibleWithArray{ enum {value = 0}; };
#define FBXSDK_INCOMPATIBLE_WITH_ARRAY_TEMPLATE(T)\
struct FbxIncompatibleWithArray< T >{\
union {\
T t();\
} catcherr;\
enum {value = 1};}
#define FBXSDK_INCOMPATIBLE_WITH_ARRAY(T)\
template<> FBXSDK_INCOMPATIBLE_WITH_ARRAY_TEMPLATE(T)
#define FBXSDK_IS_INCOMPATIBLE_WITH_ARRAY(T) ((bool) FbxIncompatibleWithArray<T>::value)
#endif
#endif