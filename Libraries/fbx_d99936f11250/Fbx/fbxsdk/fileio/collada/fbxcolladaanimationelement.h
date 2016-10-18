#ifndef _FBXSDK_FILEIO_COLLADA_ANIMATION_ELEMENT_H_
#define _FBXSDK_FILEIO_COLLADA_ANIMATION_ELEMENT_H_
class AnimationElement : public ElementBase
{
public:
typedef ElementBase base_type;
AnimationElement();
virtual ~AnimationElement();
int GetChannelCount() const;
void FromCOLLADA(xmlNode * pElement, const SourceElementMapType & pSourceElements);
void FromFBX(const FbxAnimCurve * pCurve, double pUnitConversion = 1.0);
void ToFBX(FbxAnimCurve * pFBXCurve, int pChannelIndex,
double pUnitConversion = 1.0) const;
void ToFBX(FbxNode * pFBXNode, FbxAnimLayer * pAnimLayer,
double pUnitConversion = 1.0) const;
void ToCOLLADA(xmlNode * pAnimationLibrary, const char * pNodeID,
const char * pAttributeSID);
};
#endif