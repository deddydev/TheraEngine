#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{		
		public enum class FbxCharacterOffAutoUser
		{
			ParamModeOff = kCharacterOffAutoUser::eParamModeOff,
			ParamModeAuto= kCharacterOffAutoUser::eParamModeAuto,
			ParamModeUser= kCharacterOffAutoUser::eParamModeUser
		};
		public enum class FbxCharacterAutoUser
		{
			ParamModeAuto2 = kCharacterAutoUser::eParamModeAuto2,
			eParamModeUser2= kCharacterAutoUser::eParamModeUser2
		};

		public enum class FbxCharacterPosture
		{
			ParamPostureBiped = kCharacterPosture ::ParamPostureBiped,
			ParamPostureQuadriped = kCharacterPosture::ParamPostureQuadriped,
			LastParamPosture = kCharacterPosture ::LastParamPosture
		};
		public enum class FbxCharacterFloorPivot
		{
			ParamFloorPivotAuto = kCharacterFloorPivot ::ParamFloorPivotAuto,
			ParamFloorPivotAnkle = kCharacterFloorPivot ::ParamFloorPivotAnkle,
			ParamFloorPivotToes = kCharacterFloorPivot ::ParamFloorPivotToes,
			LastParamFloorPivot = kCharacterFloorPivot ::LastParamFloorPivot
		};


		public enum class FbxCharacterRollExtractionMode
		{
			ParamRelativeRollExtraction = kCharacterRollExtractionMode ::ParamRelativeRollExtraction,
			ParamAbsoluteRollExtraction = kCharacterRollExtractionMode::ParamAbsoluteRollExtraction,
			LastRollExtractionMode = kCharacterRollExtractionMode ::LastRollExtractionMode
		};
		public enum class FbxCharacterHipsTranslationMode
		{
			ParamHipsTranslationWorldRigid = kCharacterHipsTranslationMode ::ParamHipsTranslationWorldRigid,
			ParamHipsTranslationBodyRigid = kCharacterHipsTranslationMode ::ParamHipsTranslationBodyRigid,
			LastHipsTranslationMode = kCharacterHipsTranslationMode ::LastHipsTranslationMode 
		};
		public enum class FbxCharacterFootContactType
		{ 
			ParamFootTypeNormal = kCharacterFootContactType	::ParamFootTypeNormal,
			ParamFootTypeAnkle = kCharacterFootContactType	::ParamFootTypeAnkle,
			ParamFootTypeToeBase = kCharacterFootContactType	::ParamFootTypeToeBase,
			ParamFootTypeHoof = kCharacterFootContactType	::ParamFootTypeHoof,
			LastFootContactType = kCharacterFootContactType	::LastFootContactType 
		};
		public enum class FbxCharacterHandContactType
		{
			ParamHandTypeNormal = kCharacterHandContactType::ParamHandTypeNormal,
			ParamHandTypeWrist= kCharacterHandContactType::ParamHandTypeWrist,
			ParamHandTypeFingerBase= kCharacterHandContactType::ParamHandTypeFingerBase,
			ParamHandTypeHoof= kCharacterHandContactType::ParamHandTypeHoof,
			LastHandContactType= kCharacterHandContactType::LastHandContactType 
		};
		public enum class FbxCharacterFingerContactMode 
		{
			ParamFingerContactModeSticky = kCharacterFingerContactMode::ParamFingerContactModeSticky,
			ParamFingerContactModeSpread= kCharacterFingerContactMode::ParamFingerContactModeSpread,
			ParamFingerContactModeStickySpread= kCharacterFingerContactMode::ParamFingerContactModeStickySpread, 
			LastFingerContactMode = kCharacterFingerContactMode::LastFingerContactMode
		};
		public enum class FbxCharacterContactBehaviour 
		{ 
			ParamContactNeverSync = kCharacterContactBehaviour::ParamContactNeverSync,
			ParamContactSyncOnKey= kCharacterContactBehaviour::ParamContactSyncOnKey,
			ParamContactAlwaysSync= kCharacterContactBehaviour::ParamContactAlwaysSync,
			LastContactBehaviour= kCharacterContactBehaviour::LastContactBehaviour 
		};

		public enum class FbxCharacterPropertyUnit
		{
			kPropertyNoUnit = kCharacterPropertyUnit::kPropertyNoUnit,
			kPropertyPercent= kCharacterPropertyUnit::kPropertyPercent,
			kPropertySecond= kCharacterPropertyUnit::kPropertySecond,
			kPropertyCentimeter= kCharacterPropertyUnit::kPropertyCentimeter,
			kPropertyDegree= kCharacterPropertyUnit::kPropertyDegree,
			kPropertyEnum= kCharacterPropertyUnit::kPropertyEnum,
			kPropertyReal= kCharacterPropertyUnit::kPropertyReal 
		}; 		
	}
}