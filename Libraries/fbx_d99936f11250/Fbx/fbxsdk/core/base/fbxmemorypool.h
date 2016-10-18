#ifndef _FBXSDK_CORE_BASE_MEMORY_H_
#define _FBXSDK_CORE_BASE_MEMORY_H_
class FBXSDK_DLL FbxMemoryPool
{
public:
FbxMemoryPool(size_t pBlockSize, FbxInt64 pBlockCount=0, bool pResizable=true, bool pConcurrent=true);
~FbxMemoryPool();
void Reset();
void* Allocate();
void Release(void* pMemBlock);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif