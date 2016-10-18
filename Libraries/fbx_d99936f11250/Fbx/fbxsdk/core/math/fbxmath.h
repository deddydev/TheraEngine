#ifndef _FBXSDK_CORE_MATH_H_
#define _FBXSDK_CORE_MATH_H_
#if defined(FBXSDK_ENV_MAC)
extern "C" int isnan (double);
#endif
#if defined(FBXSDK_ENV_WIN)
#ifndef isnan
#define isnan _isnan
#endif
#ifndef finite
#define finite _finite
#endif
#endif
#define FBXSDK_PI    3.1415926535897932384626433832795028841971693993751
#define FBXSDK_PI_DIV_2   1.5707963267948966192313216916397514420985846996875
#define FBXSDK_PI_DIV_180  0.017453292519943295769236907684886127134428718885417
#define FBXSDK_180_DIV_PI  57.295779513082320876798154814105170332405472466565
#define FBXSDK_1_DIV_LN2  1.4426950408889634073599246810018921374266459541530
#define FBXSDK_DEG_TO_RAD  FBXSDK_PI_DIV_180
#define FBXSDK_RAD_TO_DEG  FBXSDK_180_DIV_PI
#define FBXSDK_IN_TO_CM   2.54
#define FBXSDK_MM_TO_CM   0.1
#define FBXSDK_CM_TO_IN   0.393700787
#define FBXSDK_IN_TO_MM   25.4
#define FBXSDK_MM_TO_IN   0.0393700787
#define FBXSDK_FT_TO_M   0.3048
#define FBXSDK_M_TO_FT   3.2808399
#define FBXSDK_YD_TO_FT   3
#define FBXSDK_FT_TO_YD   0.333333333
#define FBXSDK_KM_TO_MILE  0.621371192
#define FBXSDK_MILE_TO_KM  1.609344
#define FBXSDK_YD_TO_M   0.9144
#define FBXSDK_M_TO_YD   1.0936133
#define FBXSDK_EULER_DEGENERATE  FbxEuler::DegenerateThreshold()
class FBXSDK_DLL FbxEuler
{
public:
enum EAxis {eAxisX=0, eAxisY=1, eAxisZ=2};
enum EOrder
{
eOrderXYZ,
eOrderXZY,
eOrderYZX,
eOrderYXZ,
eOrderZXY,
eOrderZYX,
eOrderSphericXYZ
};
static bool IsParityOdd(EOrder pOrder);
static bool IsRepeat(EOrder pOrder);
static const int AxisTableSize;
static const int AxisTable[][3];
static void SetDegenerateThreshold(double pThreshold=16.0*FBXSDK_FLOAT_EPSILON);
static inline double DegenerateThreshold() { return FbxEuler::mDegenerateThreshold; }
private:
static double mDegenerateThreshold;
};
#define EFbxRotationOrder FbxEuler::EOrder
#define eEulerXYZ   FbxEuler::eOrderXYZ
#define eEulerXZY   FbxEuler::eOrderXZY
#define eEulerYZX   FbxEuler::eOrderYZX
#define eEulerYXZ   FbxEuler::eOrderYXZ
#define eEulerZXY   FbxEuler::eOrderZXY
#define eEulerZYX   FbxEuler::eOrderZYX
#define eSphericXYZ   FbxEuler::eOrderSphericXYZ
enum EFbxQuatInterpMode
{
eQuatInterpOff,
eQuatInterpClassic,
eQuatInterpSlerp,
eQuatInterpCubic,
eQuatInterpTangentDependent,
eQuatInterpCount
};
extern FBXSDK_DLL const FbxDouble FbxIdentityMatrix[4][4];
extern FBXSDK_DLL const FbxVector4 FbxZeroVector4;
inline float FbxFloor(const float x)
{
return float(floor(x));
}
inline double FbxFloor(const double x)
{
return floor(x);
}
inline float FbxCeil(const float x)
{
return float(ceil(x));
}
inline double FbxCeil(const double x)
{
return ceil(x);
}
template<class T> inline T FbxSign(const T x)
{
return (x < 0) ? T(-1) : T(1);
}
template<class T> inline T FbxRound(const T x)
{
T y = FbxFloor(x);
return (x - y < T(0.5)) ? y : y + T(1);
}
inline FbxUChar FbxAbs(const FbxUChar x)
{
return x;
}
inline FbxUShort FbxAbs(const FbxUShort x)
{
return x;
}
inline FbxUInt FbxAbs(const FbxUInt x)
{
return x;
}
#ifndef FBXSDK_SYSTEM_IS_LP64
inline FbxULong FbxAbs(const FbxULong x)
{
return x;
}
#endif
inline FbxULongLong FbxAbs(const FbxULongLong x)
{
return x;
}
inline FbxFloat FbxAbs(const FbxFloat x)
{
return (FbxFloat)fabs(x);
}
inline FbxDouble FbxAbs(const FbxDouble x)
{
return fabs(x);
}
template<class T> inline T FbxAbs(const T x)
{
return (x >= 0) ? x : ((x > FbxMin(x)) ? -x : FbxMax(x));
}
template<class T> inline T FbxClamp(const T value, const T min, const T max)
{
return (value < min) ? min : ((value > max) ? max : value);
}
template<class T> inline bool FbxEqual(const T x, const T y, const T e=(T)FBXSDK_TOLERANCE)
{
return FbxAbs(x - y) <= e;
}
inline bool FbxEqual(const FbxDouble2& x, const FbxDouble2& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x.mData[0], y.mData[0], e) && FbxEqual(x.mData[1], y.mData[1], e) );
}
inline bool FbxEqual(const FbxDouble3& x, const FbxDouble3& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x.mData[0], y.mData[0], e) && FbxEqual(x.mData[1], y.mData[1], e) && FbxEqual(x.mData[2], y.mData[2], e) );
}
inline bool FbxEqual(const FbxDouble4& x, const FbxDouble4& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x.mData[0], y.mData[0], e) && FbxEqual(x.mData[1], y.mData[1], e) && FbxEqual(x.mData[2], y.mData[2], e) && FbxEqual(x.mData[3], y.mData[3], e) );
}
inline bool FbxEqual(const FbxDouble4x4& x, const FbxDouble4x4& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x[0], y[0], e) && FbxEqual(x[1], y[1], e) && FbxEqual(x[2], y[2], e) && FbxEqual(x[3], y[3], e) );
}
inline bool FbxEqual(const FbxVector2& x, const FbxVector2& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x.mData[0], y.mData[0], e) && FbxEqual(x.mData[1], y.mData[1], e) );
}
inline bool FbxEqual(const FbxVector4& x, const FbxVector4& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x.mData[0], y.mData[0], e) && FbxEqual(x.mData[1], y.mData[1], e) && FbxEqual(x.mData[2], y.mData[2], e) && FbxEqual(x.mData[3], y.mData[3], e) );
}
inline bool FbxEqual(const FbxMatrix& x, const FbxMatrix& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x[0], y[0], e) && FbxEqual(x[1], y[1], e) && FbxEqual(x[2], y[2], e) && FbxEqual(x[3], y[3], e) );
}
inline bool FbxEqual(const FbxAMatrix& x, const FbxAMatrix& y, const double e=FBXSDK_TOLERANCE)
{
return ( FbxEqual(x[0], y[0], e) && FbxEqual(x[1], y[1], e) && FbxEqual(x[2], y[2], e) && FbxEqual(x[3], y[3], e) );
}
inline FbxDouble FbxMod(const FbxFloat x, FbxFloat& i)
{
return modff(x, &i);
}
inline FbxDouble FbxMod(const FbxDouble x, FbxDouble& i)
{
return modf(x, &i);
}
inline FbxDouble FbxMod(const FbxFloat x)
{
FbxFloat i;
return modff(x, &i);
}
inline FbxDouble FbxMod(const FbxDouble x)
{
FbxDouble i;
return modf(x, &i);
}
template<class T> inline T FbxReciprocal(const T x)
{
return T(1) / x;
}
inline double FbxSqrt(const double x)
{
return sqrt(x);
}
inline float FbxSqrt(const float x)
{
return sqrtf(x);
}
template<class T> inline T FbxSqrt(const T x)
{
if( x > 1 )
{
T z, y = x >> 1;
do
{
z = y;
y = (y + (x / y)) >> 1;
}
while(y < z);
return z;
}
else
{
return x;
}
}
inline float FbxExp(const float x)
{
return expf(x);
}
inline double FbxExp(const double x)
{
return exp(x);
}
inline float FbxLog(const float x)
{
return float(log(x));
}
inline double FbxLog(const double x)
{
return log(x);
}
template<class T> inline T FbxPow(const T x, const T y)
{
return (T)FbxExp(y * FbxLog((double)x));
}
template<class T> inline T FbxLog2(const T x)
{
return (T)(FbxLog(x) * FBXSDK_1_DIV_LN2);
}
inline float FbxSin(const float x)
{
return sinf(x);
}
inline double FbxSin(const double x)
{
return sin(x);
}
inline float FbxCos(const float x)
{
return cosf(x);
}
inline double FbxCos(const double x)
{
return cos(x);
}
inline float FbxTan(const float x)
{
return tanf(x);
}
inline double FbxTan(const double x)
{
return tan(x);
}
template<class T> inline T FbxSinCos(const T x, T* y)
{
return *y = FbxCos(x), FbxSin(x);
}
template<class T> inline T FbxSinCosd(const T x, T* y)
{
return FbxSinCos(T(x * FBXSDK_PI_DIV_180), y);
}
inline float FbxASin(const float x)
{
return asinf(x);
}
inline double FbxASin(const double x)
{
return asin(x);
}
template<class T> inline T FbxASind(const T x)
{
return (T)(FbxASin((double)x) * FBXSDK_180_DIV_PI);
}
inline float FbxACos(const float x)
{
return acosf(x);
}
inline double FbxACos(const double x)
{
return acos(x);
}
template<class T> inline T FbxACosd(const T x)
{
return (T)(FbxACos(x) * FBXSDK_180_DIV_PI);
}
inline float FbxATan(const float x)
{
return atanf(x);
}
inline double FbxATan(const double x)
{
return atan(x);
}
template<class T> inline T FbxATand(const T x)
{
return (T)(FbxATan(x) * FBXSDK_180_DIV_PI);
}
inline float FbxATan(const float y, const float x)
{
return atan2f(y, x);
}
inline double FbxATan(const double y, const double x)
{
return atan2(y, x);
}
template<class T> inline T FbxATand(const T y, const T x)
{
return (T)(FbxATan(y, x) * FBXSDK_180_DIV_PI);
}
template<class T> inline T FbxNorm(const T x, const T y)
{
return FbxSqrt(x * x + y * y);
}
template<class T> inline T FbxNorm(const T x, const T y, const T z)
{
return FbxSqrt(x * x + y * y + z * z);
}
template<class T> inline T FbxNorm(const T w, const T x, const T y, const T z)
{
return FbxSqrt(w * w + x * x + y * y + z * z);
}
template<class T> inline T FbxHypot(const T x, const T y)
{
return FbxSqrt(x * x + y * y);
}
template<class T> inline T FbxHypot(const T x, const T y, const T z)
{
return FbxSqrt(x * x + y * y + z * z);
}
template<class T> inline T FbxHypot(const T w, const T x, const T y, const T z)
{
return FbxSqrt(w * w + x * x + y * y + z * z);
}
inline FbxVector4 FbxRejection(const FbxVector4& a, const FbxVector4& b)
{
return a - b * (a.DotProduct(b) / b.DotProduct(b));
}
template<class T> inline int FbxBitCount(const T x)
{
int n = 0;
T c = x;
while( c )
{
n += int(c & 1);
c = (c >> 1);
}
return n;
}
template<class T> inline void FbxFixInfinite(T& x)
{
if( x != x || x > FbxMax(x) || x < -FbxMax(x) )
{
x = T(0);
}
}
template<class T> inline T FbxExp(const T x);
template<class T> inline T FbxLog(const T x);
template<class T> inline T FbxSin(const T x);
template<class T> inline T FbxCos(const T x);
template<class T> inline T FbxASin(const T x);
template<class T> inline T FbxACos(const T x);
template<class T> inline T FbxATan(const T x);
template<class T> inline T FbxATan(const T y, const T x);
#endif