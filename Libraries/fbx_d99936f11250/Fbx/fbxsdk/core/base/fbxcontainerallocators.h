#ifndef _FBXSDK_CORE_BASE_CONTAINER_ALLOCATORS_H_
#define _FBXSDK_CORE_BASE_CONTAINER_ALLOCATORS_H_
class FBXSDK_DLL FbxBaseAllocator
{
public:
FbxBaseAllocator(const size_t pRecordSize) :
mRecordSize(pRecordSize)
{
}
void Reserve(const size_t  )
{
}
void* AllocateRecords(const size_t pRecordCount=1)
{
return FbxMalloc(pRecordCount * mRecordSize);
}
void FreeMemory(void* pRecord)
{
FbxFree(pRecord);
}
size_t GetRecordSize() const
{
return mRecordSize;
}
};
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
}
};
};
#endif