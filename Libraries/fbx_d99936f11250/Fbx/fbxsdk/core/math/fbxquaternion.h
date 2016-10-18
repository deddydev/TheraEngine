#ifndef _FBXSDK_CORE_MATH_QUATERNION_H_
#define _FBXSDK_CORE_MATH_QUATERNION_H_
class FBXSDK_DLL FbxQuaternion : public FbxDouble4
{
public:
FbxQuaternion();
FbxQuaternion(const FbxQuaternion& pV);
FbxQuaternion(double pX, double pY, double pZ, double pW = 1.0);
FbxQuaternion(const FbxVector4& pAxis, double pDegree);
~FbxQuaternion();
FbxQuaternion& operator=(const FbxQuaternion& pQuaternion);
double& operator[](int pIndex);
const double& operator[](int pIndex) const;
double GetAt(int pIndex) const;
void SetAt(int pIndex, double pValue);
void Set(double pX, double pY, double pZ, double pW = 1.0);
FbxQuaternion operator+(double pValue) const;
FbxQuaternion operator-(double pValue) const;
FbxQuaternion operator*(double pValue) const;
FbxQuaternion operator/(double pValue) const;
FbxQuaternion& operator+=(double pValue);
FbxQuaternion& operator-=(double pValue);
FbxQuaternion& operator*=(double pValue);
FbxQuaternion& operator/=(double pValue);
FbxQuaternion operator-() const;
FbxQuaternion operator+(const FbxQuaternion& pQuaternion) const;
FbxQuaternion operator-(const FbxQuaternion& pQuaternion) const;
FbxQuaternion operator*(const FbxQuaternion& pOther) const;
FbxQuaternion operator/(const FbxQuaternion& pOther) const;
FbxQuaternion& operator+=(const FbxQuaternion& pQuaternion);
FbxQuaternion& operator-=(const FbxQuaternion& pQuaternion);
FbxQuaternion& operator*=(const FbxQuaternion& pOther);
FbxQuaternion& operator/=(const FbxQuaternion& pOther);
FbxQuaternion Product(const FbxQuaternion& pOther) const;
double DotProduct(const FbxQuaternion& pQuaternion) const;
void Normalize();
void Conjugate();
double Length();
void Inverse();
void SetAxisAngle(const FbxVector4& pAxis, double pDegree);
FbxQuaternion Slerp(const FbxQuaternion& pOther, double pWeight) const;
void ComposeSphericalXYZ(const FbxVector4 pEuler);
FbxVector4 DecomposeSphericalXYZ() const;
bool operator==(const FbxQuaternion & pV) const;
bool operator!=(const FbxQuaternion & pV) const;
operator double* ();
operator const double* () const;
int Compare(const FbxQuaternion &pQ2, const double pThreshold = FBXSDK_TOLERANCE) const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
void GetQuaternionFromPositionToPosition(const FbxVector4 &pP0, const FbxVector4 &pP1);
#endif
};
#endif