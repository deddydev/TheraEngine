#ifndef _FBXSDK_CORE_MATH_VECTOR_2_H_
#define _FBXSDK_CORE_MATH_VECTOR_2_H_
class FBXSDK_DLL FbxVector2 : public FbxDouble2
{
public:
FbxVector2();
FbxVector2(const FbxVector2& pVector2);
FbxVector2(double pX, double pY);
FbxVector2& operator=(const FbxVector2& pVector2);
void Set(double pX, double pY);
FbxVector2 operator+(double pValue) const;
FbxVector2 operator-(double pValue) const;
FbxVector2 operator*(double pValue) const;
FbxVector2 operator/(double pValue) const;
FbxVector2& operator+=(double pValue);
FbxVector2& operator-=(double pValue);
FbxVector2& operator*=(double pValue);
FbxVector2& operator/=(double pValue);
FbxVector2 operator-() const;
FbxVector2 operator+(const FbxVector2& pVector) const;
FbxVector2 operator-(const FbxVector2& pVector) const;
FbxVector2 operator*(const FbxVector2& pVector) const;
FbxVector2 operator/(const FbxVector2& pVector) const;
FbxVector2& operator+=(const FbxVector2& pVector);
FbxVector2& operator-=(const FbxVector2& pVector);
FbxVector2& operator*=(const FbxVector2& pVector);
FbxVector2& operator/=(const FbxVector2& pVector);
double DotProduct(const FbxVector2& pVector) const;
bool operator==(const FbxVector2 & pVector) const;
bool operator!=(const FbxVector2 & pVector) const;
double Length() const;
double SquareLength() const;
double Distance(const FbxVector2& pVector) const;
void Normalize();
operator double* ();
operator const double* () const;
bool IsZero(int pSize=2) const;
void FixIncorrectValue();
};
#endif