#ifndef _FBXSDK_CORE_MATH_VECTOR_4_H_
#define _FBXSDK_CORE_MATH_VECTOR_4_H_
class FbxQuaternion;
class FBXSDK_DLL FbxVector4 : public FbxDouble4
{
public:
FbxVector4();
FbxVector4(const FbxVector4& pVector4);
FbxVector4(double pX, double pY, double pZ, double pW=1.0);
FbxVector4(const double pValue[4]);
FbxVector4(const FbxDouble3& pValue);
FbxVector4& operator=(const FbxVector4& pVector4);
FbxVector4& operator=(const double* pValue);
FbxVector4& operator=(const FbxDouble3& pValue);
void Set(double pX, double pY, double pZ, double pW=1.0);
FbxVector4 operator+(double pValue) const;
FbxVector4 operator-(double pValue) const;
FbxVector4 operator*(double pValue) const;
FbxVector4 operator/(double pValue) const;
FbxVector4& operator+=(double pValue);
FbxVector4& operator-=(double pValue);
FbxVector4& operator*=(double pValue);
FbxVector4& operator/=(double pValue);
FbxVector4 operator-() const;
FbxVector4 operator+(const FbxVector4& pVector) const;
FbxVector4 operator-(const FbxVector4& pVector) const;
FbxVector4 operator*(const FbxVector4& pVector) const;
FbxVector4 operator/(const FbxVector4& pVector) const;
FbxVector4& operator+=(const FbxVector4& pVector);
FbxVector4& operator-=(const FbxVector4& pVector);
FbxVector4& operator*=(const FbxVector4& pVector);
FbxVector4& operator/=(const FbxVector4& pVector);
double DotProduct(const FbxVector4& pVector) const;
FbxVector4 CrossProduct(const FbxVector4& pVector) const;
static bool AxisAlignmentInEulerAngle(const FbxVector4& pAB, const FbxVector4& pA, const FbxVector4& pB, FbxVector4& pAngles);
bool operator==(const FbxVector4 & pVector) const;
bool operator!=(const FbxVector4 & pVector) const;
double Length() const;
double SquareLength() const;
double Distance(const FbxVector4& pVector) const;
void Normalize();
void SetXYZ(const FbxQuaternion pQuat);
operator double* ();
operator const double* () const;
bool IsZero(int pSize=4) const;
void FixIncorrectValue();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
int Compare(const FbxVector4& pV, const double pThreshold=FBXSDK_TOLERANCE) const;
#endif
};
#endif