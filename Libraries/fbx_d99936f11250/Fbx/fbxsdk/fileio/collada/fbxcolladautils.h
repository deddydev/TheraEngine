#ifndef _FBXSDK_FILEIO_COLLADA_UTILS_H_
#define _FBXSDK_FILEIO_COLLADA_UTILS_H_
#include <fbxsdk.h>
#include <fbxsdk/fileio/collada/fbxcolladatokens.h>
#include <fbxsdk/fileio/collada/fbxcolladaiostream.h>
#include <components/libxml2-2.7.8/include/libxml/globals.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
#ifndef INT_MAX
	#define INT_MAX 0x7FFFFFFF
#endif
#ifndef CENTIMETERS_TO_INCHES
	#define CENTIMETERS_TO_INCHES 2.54f
#endif
#ifndef RADIANS_TO_DEGREES
	#define RADIANS_TO_DEGREES 57.295799f
#endif
enum DAE_Flow 
 kCOLLADAFlowIn, kCOLLADAFlowOut, kCOLLADAFlowInOut 
const int MATRIX_STRIDE = 16
const int VECTOR_STRIDE = 3
#define COLLADA_ID_PROPERTY_NAME "COLLADA_ID"
class XmlNodeDeletionPolicy
public:
    static inline void DeleteIt(xmlNode ** ptr)
        if (*ptr != NULL)
            xmlFreeNode(*ptr)
            *ptr = NULL
typedef FbxAutoPtr<xmlNode, XmlNodeDeletionPolicy> XmlNodePtr
typedef FbxMap< FbxString, xmlNode* > SourceElementMapType
typedef FbxMap< FbxString, xmlNode* > SkinMapType
struct ColladaLayerTraits
    ColladaLayerTraits() 
		: mLayerType(FbxLayerElement::eUnknown), mLayerElementLength(0) 
    ColladaLayerTraits(FbxLayerElement::EType pType, int pLength)
        : mLayerType(pType), mLayerElementLength(pLength) 
    FbxLayerElement::EType mLayerType
    int mLayerElementLength
    static const ColladaLayerTraits GetLayerTraits(const FbxString & pLabel)
void DAE_AddNotificationError(const FbxManager * pSdkManger, const FbxString & pErrorMessage)
void DAE_AddNotificationWarning(const FbxManager * pSdkManger, const FbxString & pWarningMessage)
void DAE_ExportArray(xmlNode* parentXmlNode, const char* id, FbxArray<FbxVector4>& arr)
void DAE_ExportArray(xmlNode* parentXmlNode, const char* id, FbxArray<FbxVector2>& arr)
void DAE_ExportArray(xmlNode* parentXmlNode, const char* id, FbxArray<FbxColor>& arr)
void DAE_ExportArray(xmlNode* parentXmlNode, const char* id, FbxArray<double>& arr)
void DAE_ExportArray(xmlNode* parentXmlNode, const char* id, FbxStringList& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxStringList& accessorParams, FbxArray<double>& arr, bool isCommonProfile=true)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxArray<FbxVector4>& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxArray<FbxVector2>& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxArray<FbxColor>& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxArray<FbxAMatrix>& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxArray<FbxMatrix>& arr)
xmlNode* DAE_ExportSource14(xmlNode* parentXmlNode, const char* id, FbxStringList& arr, const char* type, bool isCommonProfile=true)
void DAE_ExportSourceArray(xmlNode* sourceNode, const char* id, FbxArray<FbxColor>& arr)
void DAE_ExportSourceArray14(xmlNode* sourceNode, const char* id, FbxArray<FbxColor>& arr)
xmlNode* DAE_ExportAccessor(xmlNode* parentXmlNode, const char* id, const char* arrayRef, int count, int stride, const char* name, const char* type)
xmlNode* DAE_ExportAccessor14(xmlNode* parentXmlNode, const char* id, const char* arrayRef, int count, int stride, const char* name, const char* type)
void DAE_AddXYZAccessor(xmlNode* parentXmlNode, const char* profile, const char* arrayName, const char* arrayRef, int count)
void DAE_AddSTAccessor(xmlNode* parentXmlNode, const char* profile, const char* arrayName, const char* arrayRef, int count)
void DAE_AddFlow(xmlNode* node, DAE_Flow flow)
void DAE_AddXYZAccessor14(xmlNode* parentXmlNode, const char* profile, const char* arrayName, const char* arrayRef, int count)
void DAE_AddSTAccessor14(xmlNode* parentXmlNode, const char* profile, const char* arrayName, const char* arrayRef, int count)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const FbxColor& color, DAE_Flow flow)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const FbxVector4& vector, DAE_Flow flow)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, double value, DAE_Flow flow)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, bool value, DAE_Flow flow)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const char* type, const char* value, DAE_Flow flow)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const FbxDouble3& color)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const FbxColor& color)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const FbxVector4& vector)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, double value)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, bool value)
xmlNode* DAE_AddParameter(xmlNode* parentXmlNode, const char* name, const char* type, const char* value)
xmlNode* DAE_AddTechnique(xmlNode* parentXmlNode, const char* technique)
void DAE_AddInput(xmlNode* parentXmlNode, const char* semantic, const char* source, int idx = -1)
void DAE_AddInput14(xmlNode* parentXmlNode, const char* semantic, const char* source, int offset = -1, int set=-1)
FbxString matrixToString(const FbxAMatrix& mx)
typedef FbxArray<xmlNode*> CNodeList
void findChildrenByType(xmlNode* pParentElement, const FbxSet<FbxString>& pTypes, CNodeList& pChildrenElements)
void findChildrenByType(xmlNode* pParentElement, const char * pType, CNodeList& pChildrenElements)
xmlNode* getSourceAccessor(xmlNode* sourceNode)
xmlNode* getTechniqueNode(xmlNode* parent, const char * profile)
inline double inchesToCentimeters(double val) 
 return FbxFloor(val / CENTIMETERS_TO_INCHES * 100000) / 100000
inline double centimetersToInches(double val) 
 return FbxFloor(val * CENTIMETERS_TO_INCHES * 100000) / 100000
inline double degreesToRadians(double val) 
 return FbxFloor(val / RADIANS_TO_DEGREES * 100000) / 100000
inline double radiansToDegrees(double val) 
 return FbxFloor(val * RADIANS_TO_DEGREES * 100000) / 100000
xmlNode* DAE_FindChildElementByAttribute(xmlNode* pParentElement, const char * pAttributeName,
                                        const char * pAttributeValue, const char * pDefaultAttributeValue = "")
xmlNode* DAE_FindChildElementByTag(xmlNode* pParentElement, const char * pTag, xmlNode* pFindFrom = NULL)
template <typename TYPE>
void DAE_GetElementContent(xmlNode * pElement, TYPE & pData)
    if (pElement != NULL)
        FbxAutoFreePtr<xmlChar> lContent(xmlNodeGetContent(pElement))
        FromString(&pData, (const char *)lContent.Get())
bool DAE_CheckCompatibility(xmlNode * pNodeElement)
void DAE_GetElementTag(xmlNode * pElement, FbxString & pTag)
const FbxString DAE_GetElementAttributeValue(xmlNode * pElement, const char * pAttributeName)
template <typename TYPE>
bool DAE_GetElementAttributeValue(xmlNode * pElement, const char * pAttributeName, TYPE & pData)
    if (!pElement || !pAttributeName)
        return false
    FbxAutoFreePtr<xmlChar> lPropertyValue(xmlGetProp(pElement, (const xmlChar *)pAttributeName))
    if (lPropertyValue)
        FromString(&pData, (const char *)lPropertyValue.Get())
        return true
    return false
template <>
inline bool DAE_GetElementAttributeValue(xmlNode * pElement,
                                         const char * pAttributeName,
                                         FbxString & pData)
    if (!pElement || !pAttributeName)
        return false
    FbxAutoFreePtr<xmlChar> lPropertyValue(xmlGetProp(pElement, (const xmlChar *)pAttributeName))
    if (lPropertyValue)
        pData = (const char *)lPropertyValue.Get()
        return true
    return false
bool DAE_CompareAttributeValue(xmlNode * pElement,
                                      const char * pAttributeName,
                                      const char * pValue)
const FbxString DAE_GetIDFromUrlAttribute(xmlNode * pElement)
const FbxString DAE_GetIDFromSourceAttribute(xmlNode * pElement)
const FbxString DAE_GetIDFromTargetAttribute(xmlNode * pElement)
void DAE_SetName(FbxObject * pObject, const FbxString & pName, const FbxString & pID)
xmlNode * DAE_GetSourceWithSemantic(xmlNode * pConsumerElement, const char * pSemantic,
                                    const SourceElementMapType & pSourceElements)
template <typename T>
xmlNode * DAE_AddChildElement(xmlNode * pParentElement, const char * pTag,
                              const T & pContent)
    const FbxString lRepr = ToString(pContent)
    return xmlNewChild(pParentElement, NULL, (xmlChar *)pTag,
        (xmlChar *)lRepr.Buffer())
inline xmlNode * DAE_AddChildElement(xmlNode * pParentElement, const char * pTag)
    return DAE_AddChildElement(pParentElement, pTag, FbxString())
inline xmlNode * DAE_NewElement(const char * pTag)
    return xmlNewNode(NULL, reinterpret_cast<xmlChar*>(const_cast<char *>(pTag)))
template <typename T>
xmlAttr * DAE_AddAttribute(xmlNode * pElement, const FbxString & pAttributeName,
                           const T & pAttributeValue)
    const FbxString lRepr = ToString(pAttributeValue)
    return xmlNewProp(pElement, (xmlChar *)pAttributeName.Buffer(),
        (xmlChar *)lRepr.Buffer())
const FbxSystemUnit DAE_ImportUnit(xmlNode * pUnitElement)
void IncreaseLclTranslationAnimation(FbxNode * pNode, FbxDouble3 & pOffset)
void RecursiveSearchElement(xmlNode * pBaseElement, const char * pTag, FbxArray<xmlNode*> & pResult)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 