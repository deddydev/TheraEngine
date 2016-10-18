#ifndef _FBXSDK_FILEIO_GOBO_H_
#define _FBXSDK_FILEIO_GOBO_H_
class FbxGobo
{
public:
FbxGobo(char* pName) :
mName(pName)
{
}
FbxString mName;
FbxString mFileName;
bool mDrawGroundProjection;
bool mVolumetricLightProjection;
bool mFrontVolumetricLightProjection;
};
#endif