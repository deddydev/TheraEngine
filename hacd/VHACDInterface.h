#pragma once
#include "VHACD.h"
class VHACDInterface
{
#ifdef __cplusplus
	extern "C" 
	{
#endif
		__declspec(dllexport) IVHACD* CreateClassName();
		__declspec(dllexport) void DisposeClassName(IVHACD* a_pObject);
		__declspec(dllexport) void Cancel();
		__declspec(dllexport) void Compute(
			IVHACD* a_pObject, 
			const float* const points,
			const unsigned int stridePoints,
			const unsigned int countPoints,
			const int* const triangles,
			const unsigned int strideTriangles,
			const unsigned int countTriangles,
			const Parameters& params);
#ifdef __cplusplus
	}
#endif
};
