#ifndef _FBXSDK_CORE_MATH_MATRIX_H_
#define _FBXSDK_CORE_MATH_MATRIX_H_
class FbxAMatrix;
class FBXSDK_DLL FbxMatrix : public FbxDouble4x4
{
public:
FbxMatrix();
FbxMatrix(const FbxMatrix& pM);
FbxMatrix(const FbxAMatrix& pM);
FbxMatrix(const FbxVector4& pT, const FbxVector4& pR, const FbxVector4& pS);
FbxMatrix(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
FbxMatrix( const double p00, const double p10, const double p20, const double p30,
const double p01, const double p11, const double p21, const double p31,
const double p02, const double p12, const double p22, const double p32,
const double p03, const double p13, const double p23, const double p33);
~FbxMatrix();
double Get(int pY, int pX) const;
FbxVector4 GetRow(int pY) const;
FbxVector4 GetColumn(int pX) const;
void Set(int pY, int pX, double pValue);
void SetTRS(const FbxVector4& pT, const FbxVector4& pR, const FbxVector4& pS);
void SetTQS(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
void SetRow(int pY, const FbxVector4& pRow);
void SetColumn(int pX, const FbxVector4& pColumn);
void GetElements(FbxVector4& pTranslation, FbxQuaternion& pRotation, FbxVector4& pShearing, FbxVector4& pScaling, double& pSign) const;
void GetElements(FbxVector4& pTranslation, FbxVector4& pRotation, FbxVector4& pShearing, FbxVector4& pScaling, double& pSign) const;
FbxMatrix& operator=(const FbxMatrix& pMatrix);
FbxMatrix operator-() const;
FbxMatrix operator+(const FbxMatrix& pMatrix) const;
FbxMatrix operator-(const FbxMatrix& pMatrix) const;
FbxMatrix operator*(const FbxMatrix& pMatrix) const;
FbxMatrix& operator+=(const FbxMatrix& pMatrix);
FbxMatrix& operator-=(const FbxMatrix& pMatrix);
FbxMatrix& operator*=(const FbxMatrix& pMatrix);
bool operator==(const FbxMatrix& pM) const;
bool operator==(const FbxAMatrix& pM) const;
bool operator!=(const FbxMatrix& pM) const;
bool operator!=(const FbxAMatrix& pM) const;
operator double* ();
operator const double* () const;
typedef const double(kDouble44)[4][4] ;
inline kDouble44 & Double44() const { return *((kDouble44 *)&mData[0][0]); }
FbxMatrix Inverse() const;
FbxMatrix Transpose() const;
void SetIdentity();
void SetLookToLH(const FbxVector4& pEyePosition, const FbxVector4& pEyeDirection, const FbxVector4& pUpDirection);
void SetLookToRH(const FbxVector4& pEyePosition, const FbxVector4& pEyeDirection, const FbxVector4& pUpDirection);
void SetLookAtLH(const FbxVector4& pEyePosition, const FbxVector4& pLookAt, const FbxVector4& pUpDirection);
void SetLookAtRH(const FbxVector4& pEyePosition, const FbxVector4& pLookAt, const FbxVector4& pUpDirection);
FbxVector4 MultNormalize(const FbxVector4& pVector) const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
int Compare(const FbxMatrix pM, const double pThreshold = FBXSDK_TOLERANCE) const;
int Compare(const FbxAMatrix pM, const double pThreshold = FBXSDK_TOLERANCE) const;
FbxMatrix operator*(double pValue) const;
FbxMatrix& operator*=(double pValue);
double LUDecomposition(FbxVector4& pVector);
FbxMatrix LUMult(FbxMatrix pM, const FbxVector4& pVector) const;
double Determinant() const;
#endif
};
#endif