#ifndef _FBXSDK_DEFINITION_H_
#define _FBXSDK_DEFINITION_H_
#define FBX_SAFE_DELETE(p)   {FbxDelete(p);(p)=NULL;}
#define FBX_SAFE_DELETE_ARRAY(a) {FbxDeleteArray(a);(a)=NULL;}
#define FBX_SAFE_DESTROY(p)   if(p){(p)->Destroy();(p)=NULL;}
#define FBX_SAFE_FREE(p)   if(p){FbxFree(p);(p)=NULL;}
#endif