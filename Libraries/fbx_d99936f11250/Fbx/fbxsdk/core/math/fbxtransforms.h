#ifndef _FBXSDK_CORE_MATH_TRANSFORMS_H_
#define _FBXSDK_CORE_MATH_TRANSFORMS_H_
class FBXSDK_DLL FbxLimits
{
public:
FbxLimits();
FbxLimits& operator=(const FbxLimits& pLimits);
bool GetActive() const;
void SetActive(const bool pActive);
bool GetMinXActive() const;
bool GetMinYActive() const;
bool GetMinZActive() const;
void GetMinActive(bool& pXActive, bool& pYActive, bool& pZActive) const;
FbxDouble3 GetMin() const;
void SetMinXActive(bool pActive);
void SetMinYActive(bool pActive);
void SetMinZActive(bool pActive);
void SetMinActive(bool pXActive, bool pYActive, bool pZActive);
void SetMin(const FbxDouble3& pMin);
bool GetMaxXActive() const;
bool GetMaxYActive() const;
bool GetMaxZActive() const;
void GetMaxActive(bool& pXActive, bool& pYActive, bool& pZActive) const;
FbxDouble3 GetMax() const;
void SetMaxXActive(bool pActive);
void SetMaxYActive(bool pActive);
void SetMaxZActive(bool pActive);
void SetMaxActive(bool pXActive, bool pYActive, bool pZActive);
void SetMax(const FbxDouble3& pMax);
bool GetAnyMinMaxActive() const;
FbxDouble3 Apply(const FbxDouble3& pVector);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
enum EMask {eActive=1<<0, eMinX=1<<1, eMinY=1<<2, eMinZ=1<<3, eMaxX=1<<4, eMaxY=1<<5, eMaxZ=1<<6, eAll=eMinX|eMinY|eMinZ|eMaxX|eMaxY|eMaxZ};
#endif
};
};
enum EInheritType {eInheritRrSs, eInheritRSrs, eInheritRrs};
enum EMask {eHasNothing=0, eHasPreRotM=1<<0, eHasPostRotM=1<<1};
};
enum EMask {eHasNothing=0, eHasRotOffset=1<<0, eHasRotPivot=1<<1, eHasScaleOffset=1<<2, eHasScalePivot=1<<3};
};
#endif