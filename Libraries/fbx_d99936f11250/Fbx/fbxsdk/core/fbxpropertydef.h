#ifndef _FBXSDK_CORE_PROPERTY_DEFINITION_H_
#define _FBXSDK_CORE_PROPERTY_DEFINITION_H_
#define FBXSDK_PROPERTY_ID_NULL -1
#define FBXSDK_PROPERTY_ID_ROOT 0
class FbxPropertyPage;
class FBXSDK_DLL FbxPropertyFlags
{
public:
enum EInheritType
{
eOverride,
eInherit,
eDeleted
};
enum EFlags
{
eNone = 0,
eStatic = 1 << 0,
eAnimatable = 1 << 1,
eAnimated = 1 << 2,
eImported = 1 << 3,
eUserDefined = 1 << 4,
eHidden = 1 << 5,
eNotSavable = 1 << 6,
eLockedMember0 = 1 << 7,
eLockedMember1 = 1 << 8,
eLockedMember2 = 1 << 9,
eLockedMember3 = 1 << 10,
eLockedAll = eLockedMember0 | eLockedMember1 | eLockedMember2 | eLockedMember3,
eMutedMember0 = 1 << 11,
eMutedMember1 = 1 << 12,
eMutedMember2 = 1 << 13,
eMutedMember3 = 1 << 14,
eMutedAll = eMutedMember0 | eMutedMember1 | eMutedMember2 | eMutedMember3,
eUIDisabled = 1 << 15,
eUIGroup = 1 << 16,
eUIBoolGroup = 1 << 17,
eUIExpanded = 1 << 18,
eUINoCaption = 1 << 19,
eUIPanel = 1 << 20,
eUILeftLabel = 1 << 21,
eUIHidden = 1 << 22,
eCtrlFlags = eStatic | eAnimatable | eAnimated | eImported | eUserDefined | eHidden | eNotSavable | eLockedAll | eMutedAll,
eUIFlags = eUIDisabled | eUIGroup | eUIBoolGroup | eUIExpanded | eUINoCaption | eUIPanel | eUILeftLabel | eUIHidden,
eAllFlags = eCtrlFlags | eUIFlags,
eFlagCount = 23,
};
bool SetFlags(FbxPropertyFlags::EFlags pMask, FbxPropertyFlags::EFlags pFlags);
FbxPropertyFlags::EFlags GetFlags() const;
FbxPropertyFlags::EFlags GetMergedFlags(FbxPropertyFlags::EFlags pFlags) const;
bool ModifyFlags(FbxPropertyFlags::EFlags pFlags, bool pValue);
FbxPropertyFlags::EInheritType GetFlagsInheritType(FbxPropertyFlags::EFlags pFlags) const;
bool SetMask(FbxPropertyFlags::EFlags pFlags);
bool UnsetMask(FbxPropertyFlags::EFlags pFlags);
FbxPropertyFlags::EFlags GetMask() const;
bool Equal(const FbxPropertyFlags& pOther, FbxPropertyFlags::EFlags pFlags) const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxPropertyFlags();
explicit FbxPropertyFlags(FbxPropertyFlags::EFlags pFlags);
FbxPropertyFlags Clone(FbxPropertyPage* pPage);
static const int sLockedMembersMax = 4;
static const int sLockedMembersBitOffset = 7;
static const int sMutedMembersMax = 4;
static const int sMutedMembersBitOffset = 11;
#endif
};
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif