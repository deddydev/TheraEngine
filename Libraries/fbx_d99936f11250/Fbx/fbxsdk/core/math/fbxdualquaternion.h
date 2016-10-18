#ifndef _FBXSDK_CORE_MATH_DUAL_QUATERNION_H_
#define _FBXSDK_CORE_MATH_DUAL_QUATERNION_H_
class FBXSDK_DLL FbxDualQuaternion
{
public:
FbxDualQuaternion();
FbxDualQuaternion(const FbxQuaternion& pV1, const FbxQuaternion& pV2);
FbxDualQuaternion(const FbxDualQuaternion& pV);
FbxDualQuaternion(const FbxQuaternion& pRotation, const FbxVector4& pTranslation);
FbxDualQuaternion(double pX1, double pY1, double pZ1, double pW1, double pX2, double pY2, double pZ2, double pW2);
~FbxDualQuaternion();
FbxDualQuaternion& operator=(const FbxDualQuaternion& pDualQuaternion);
void Set(double pX1, double pY1, double pZ1, double pW1, double pX2, double pY2, double pZ2, double pW2);
FbxQuaternion& GetFirstQuaternion();
FbxQuaternion& GetSecondQuaternion();
const FbxQuaternion& GetFirstQuaternion() const;
const FbxQuaternion& GetSecondQuaternion() const;
FbxQuaternion GetRotation() const;
FbxVector4 GetTranslation() const;
FbxDualQuaternion operator+(double pValue) const;
FbxDualQuaternion operator-(double pValue) const;
FbxDualQuaternion operator*(double pValue) const;
FbxDualQuaternion operator/(double pValue) const;
FbxDualQuaternion& operator+=(double pValue);
FbxDualQuaternion& operator-=(double pValue);
FbxDualQuaternion& operator*=(double pValue);
FbxDualQuaternion& operator/=(double pValue);
FbxDualQuaternion operator-() const;
FbxDualQuaternion operator+(const FbxDualQuaternion& pDualQuaternion) const;
FbxDualQuaternion operator-(const FbxDualQuaternion& pDualQuaternion) const;
FbxDualQuaternion operator*(const FbxDualQuaternion& pDualQuaternion) const;
FbxDualQuaternion operator/(const FbxDualQuaternion& pDualQuaternion) const;
FbxDualQuaternion& operator+=(const FbxDualQuaternion& pDualQuaternion);
FbxDualQuaternion& operator-=(const FbxDualQuaternion& pDualQuaternion);
FbxDualQuaternion& operator*=(const FbxDualQuaternion& pDualQuaternion);
FbxDualQuaternion& operator/=(const FbxDualQuaternion& pDualQuaternion);
FbxDualQuaternion operator*(const FbxVector4 pVector) const;
FbxDualQuaternion Product(const FbxDualQuaternion& pDualQuaternion) const;
void Normalize();
void Inverse();
FbxVector4 Deform(FbxVector4& pPoint);
void Conjugate();
void Dual();
void DualConjugate();
bool operator==(const FbxDualQuaternion & pV) const;
bool operator!=(const FbxDualQuaternion & pV) const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif