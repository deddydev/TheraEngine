#ifndef _FBXSDK_CORE_EVENT_H_
#define _FBXSDK_CORE_EVENT_H_
class FBXSDK_DLL FbxEventBase
{
public:
virtual ~FbxEventBase();
virtual int GetTypeId() const = 0;
virtual const char* GetEventName() const = 0;
};
#define FBXSDK_EVENT_DECLARE(Class)            \
public: virtual const char* GetEventName() const { return FbxEventName(); } \
private: static const char* FbxEventName() { return #Class; }    \
#define FBXSDK_EVENT_TYPE_DECLARE(Class, FBXType)                                  \
public: virtual const char* GetEventName() const { return FbxEventName(); }      \
}                                                                                \
#define FBXSDK_EVENT_TEMPLATE_HEADER(ClassName, TemplateName)\
public: virtual const char* GetEventName() const {return FbxEventName();}\
}\
#define FBXSDK_EVENT_TEMPLATE_FOOTER()\
};
virtual ~FbxEvent(){}
}
}
}
}
}
};
#endif