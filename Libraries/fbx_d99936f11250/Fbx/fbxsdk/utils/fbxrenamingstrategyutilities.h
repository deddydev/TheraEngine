#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_UTILITIES_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_UTILITIES_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
#define NAMECLASH1_KEY      "_ncl1_" 
#define NAMECLASH2_KEY		"_ncl2_" 
#define UPPERTOLOWER_KEY	"ul"
#define LOWERTOUPPER_KEY	"lu"
class FBXSDK_DLL FbxRenamingStrategyUtils
public:
    static bool EncodeNonAlpha(FbxString &pString, bool pFirstCharMustBeAlphaOnly=false, FbxString pPermittedChars="", bool p8bitCharsOnly = true)
    static bool DecodeNonAlpha(FbxString &pString)
    static bool EncodeDuplicate(FbxString &pString, int pInstanceNumber=0)
    static bool DecodeDuplicate(FbxString &pString)
    static bool EncodeCaseInsensitive(FbxString &pString, const FbxString pString2)
    static bool DecodeCaseInsensitive(FbxString &pString)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 