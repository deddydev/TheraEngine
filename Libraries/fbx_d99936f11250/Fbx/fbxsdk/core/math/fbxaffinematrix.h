#ifndef _FBXSDK_CORE_MATH_AFFINE_MATRIX_H_
#define _FBXSDK_CORE_MATH_AFFINE_MATRIX_H_
class FBXSDK_DLL FbxAMatrix : public FbxDouble4x4
{
public:
FbxAMatrix();
FbxAMatrix(const FbxAMatrix& pOther);
FbxAMatrix(const FbxVector4& pT, const FbxVector4& pR, const FbxVector4& pS);
~FbxAMatrix();
double Get(int pY, int pX) const;
FbxVector4 GetT() const;
FbxVector4 GetR() const;
FbxQuaternion GetQ() const;
FbxVector4 GetS() const;
FbxVector4 GetRow(int pY) const;
FbxVector4 GetColumn(int pX) const;
void SetIdentity();
void SetT(const FbxVector4& pT);
void SetR(const FbxVector4& pR);
void SetQ(const FbxQuaternion& pQ);
void SetS(const FbxVector4& pS);
void SetTRS(const FbxVector4& pT, const FbxVector4& pR, const FbxVector4& pS);
void SetTQS(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
FbxAMatrix& operator=(const FbxAMatrix& pM);
FbxAMatrix operator*(double pValue) const;
FbxAMatrix operator/(double pValue) const;
FbxAMatrix& operator*=(double pValue);
FbxAMatrix& operator/=(double pValue);
FbxVector4 MultT(const FbxVector4& pVector4) const;
FbxVector4 MultR(const FbxVector4& pVector4) const;
FbxQuaternion MultQ(const FbxQuaternion& pQuaternion) const;
FbxVector4 MultS(const FbxVector4& pVector4) const;
FbxAMatrix operator-() const;
FbxAMatrix operator*(const FbxAMatrix& pOther) const;
FbxAMatrix& operator*=(const FbxAMatrix& pOther);
FbxAMatrix Inverse() const;
FbxAMatrix Transpose() const;
FbxAMatrix Slerp(const FbxAMatrix& pOther, double pWeight) const;
bool operator==(const FbxAMatrix& pOther) const;
bool operator!=(const FbxAMatrix& pOther) const;
operator double* ();
operator const double* () const;
typedef const double(kDouble44)[4][4] ;
inline kDouble44 & Double44() const { return *((kDouble44 *)&mData[0][0]); }
bool IsIdentity(const double pThreshold=FBXSDK_TOLERANCE);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxAMatrix(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
void SetTRS(const FbxVector4& pT, const FbxAMatrix& pRM, const FbxVector4& pS);
void SetRow(int pY, const FbxVector4& pRow);
void SetTOnly(const FbxVector4& pT);
void SetROnly(const FbxVector4& pR);
void SetQOnly(const FbxQuaternion& pQ);
FbxVector4 GetROnly() const;
FbxQuaternion GetUnnormalizedQ() const;
void SetR(const FbxVector4& pV, const int pOrd);
FbxVector4 GetR(const int pOrd) const;
void MultRM(const FbxVector4& pR);
void MultSM(const FbxVector4& pS);
bool IsRightHand() const;
double Determinant() const;
int Compare(const FbxAMatrix pM, const double pThreshold=FBXSDK_TOLERANCE) const;
#endif
};
#endif