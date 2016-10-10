#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "kfcurve/kfcurveutils.h"



{
	namespace FbxSDK
	{		
		/** Curve utilities.
		* This class groups methods allowing to test and convert a KFCurve into
		*   another KFCurve data set. Easier to use for some applications.
		*
		* \par
		*   The class is used to encapsulate the methods and enums, helping to not
		*   further clog the global space.
		*
		* \par
		*   As an example, some 3D software use a single interpolation or tangent
		* mode for a whole curve.  By providing a way to make sure the KFCurve to
		* be converted uses a single interpolation and tangent for all keys, the
		* conversion task is made a lot easier.
		*/
		public ref class FbxCurveUtils abstract sealed
		{			

			//			public:
			//
			//				/** Compare two curves.
			//				*   \return \c true if curves are equivalent.
			//				*/
			//				static bool CompareCurve(KFCurve* pCurveA, KFCurve* pCurveB);
			//
			//				/** Check if all keys of a curve are of the same interpolation type.
			//				* \param pGlobalInterpolation Receives interpolation type of all
			//				* keys if function returns \c true, undetermined otherwise.
			//				* \param pCurve Curve to scan.
			//				*   \return \c true if there is at least one key and all keys are of
			//				* the same interpolation type, \c false otherwise.
			//				*/
			//				static bool GetGlobalInterpolation(kFCurveInterpolation& pGlobalInterpolation, KFCurve& pCurve);
			//
			//				/** Check if all keys of a curve are of the same tangent mode.
			//				* \param pGlobalTangent Receives tangent mode of all
			//				* keys if function returns \c true, undetermined otherwise.
			//				* \param pCurve Curve to scan.
			//				*   \return \c true if there is at least one key and all keys are of
			//				* the same interpolation type, \c false otherwise.
			//				*/
			//				static bool GetGlobalTangeant(kFCurveTangeantMode& pGlobalTangent, KFCurve& pCurve);
			//
			//				//! Enum useful for method KFCurveUtils::GetInterpolationAndTangeantCount().
			//				enum EInterpolationAndTangeant
			//				{
			//					eCONSTANT,
			//					eLINEAR,
			//					eCUBIC_AUTO,
			//					eCUBIC_AUTO_BREAK,
			//					eCUBIC_USER,
			//					eCUBIC_BREAK,
			//					eCUBIC_TCB,
			//					eINTERPOLATION_AND_TANGEANT_COUNT
			//				};
			//
			//				//! Get the number of keys in each category of interpolation type and tangent mode.
			//				static bool GetInterpolationAndTangeantCount(KFCurve& pCurve, int pArray[eINTERPOLATION_AND_TANGEANT_COUNT]);
			//
			//				/** Convert all curve keys contained in the current KFCurveNode and its children to the tangent mode and interpolation specified.
			//				* \param pTargetGlobalInterpolation Interpolation type given to all keys.
			//				* \param pTargetGlobalTangent Tangent mode given to all keys if interpolation type is cubic.
			//				* \param pCurveNode Curve node to be modified.
			//				* \return Reference to pCurveNode parameter.
			//				*   \remarks In most cases, this function modifies the curve shape between the keys.
			//				*/
			//				static KFCurveNode& ConvertCurveInterpolationRecursive
			//					(
			//					kFCurveInterpolation    pTargetGlobalInterpolation,
			//					kFCurveTangeantMode     pTargetGlobalTangent,
			//					KFCurveNode&            pCurveNode
			//					);
			//
			//				/** Convert curve keys so they are of a single interpolation type and a single tangent mode.
			//				* As opposed to function KFCurveUtils::ConvertCurve(), this function preserves the curve
			//				* shape between the keys. Part of the curve is resampled if needed.
			//				* \param pSourceCurve Source curve.
			//				* \param pTargetCurve Curve to be modified.
			//				* \param pTargetGlobalInterpolation Interpolation type given to all keys.
			//				* \param pTargetGlobalTangent Tangent mode given to all keys if interpolation type is cubic.
			//				* \param pFramePeriod Resampling period in segments to resample.
			//				* \return Reference to pTargetCurve parameter.
			//				*/
			//				static KFCurve& ConvertCurve
			//					(
			//					KFCurve& pSourceCurve,
			//					KFCurve& pTargetCurve,
			//					kFCurveInterpolation pTargetGlobalInterpolation,
			//					kFCurveTangeantMode pTargetGlobalTangent,
			//					KTime pFramePeriod
			//					);
			//
			//				/** Resample a curve.
			//				* Interpolation type of each created curve key is set equal to
			//				* parameter \c pInterpolation. Tangent mode of each created curve
			//				* key is set equal to \c pTangentMode.
			//				* \param pSourceCurve Source curve.
			//				* \param pTargetCurve Curve to be modified.
			//				* \param pStart Start time of resampling.
			//				* \param pStop Stop time of resampling.
			//				* \param pPeriod Resampling period.
			//				* \param pInterpolation Interpolation type given to all keys.
			//				* \param pTangentMode Tangent mode given to all keys if interpolation type is cubic.
			//				* \param pAddStopKey Set to \c true to make sure a key is added exactly at time pStop.
			//				*/
			//				static void Resample
			//					(
			//					KFCurve &pSourceCurve,
			//					KFCurve &pTargetCurve,
			//					KTime &pStart,
			//					KTime &pStop,
			//					KTime &pPeriod,
			//					kFCurveInterpolation pInterpolation,
			//					kFCurveTangeantMode pTangentMode,
			//					bool pAddStopKey = false
			//					);
			//
			//				/** Resample a curve.
			//				* Interpolation type of each created curve key is set equal to
			//				* the interpolation type of the closest curve key encountered.
			//				* Tangent mode of each created curve key is set equal to
			//				* the tangent mode of the closest curve key encountered.
			//				* \param pSourceCurve Source curve.
			//				* \param pTargetCurve Curve to be modified.
			//				* \param pStart Start time of resampling.
			//				* \param pStop Stop time of resampling.
			//				* \param pPeriod Resampling period.
			//				* \param pAddStopKey Set to \c true to make sure a key is added exactly at time pStop.
			//				*/
			//				static void Resample
			//					(
			//					KFCurve &pSourceCurve,
			//					KFCurve &pTargetCurve,
			//					KTime &pStart,
			//					KTime &pStop,
			//					KTime &pPeriod,
			//					bool pAddStopKey = false
			//					);
			//
			//				/** Resample a curve.
			//				* Interpolation type of each created curve key is set to KFCURVE_INTERPOLATION_CUBIC.
			//				* Tangent mode of each created curve key is set to KFCURVE_TANGEANT_AUTO.
			//				* \param pCurve Curve to be modified.
			//				* \param pPeriod Resampling period.
			//				* \param pStart Start time of resampling. Set to KTIME_MINUS_INFINITE for whole curve.
			//				* \param pStop Stop time of resampling. Set to KTIME_INFINITE for whole curve.
			//				* \param pKeysOnFrame Set to \c true if keys are to be added on frames.
			//				*/
			//				static void Resample
			//					(
			//					KFCurve &pCurve,
			//					KTime   pPeriod,
			//					KTime   pStart = KTIME_MINUS_INFINITE,
			//					KTime   pStop = KTIME_INFINITE,
			//					bool    pKeysOnFrame = false
			//					);
			//
			//				/** Merge \c pCurveIn and \c pCurveMerge into \c pCurveOut.
			//				* Copy \c pCurveIn in \c pCurveOut and then overwrite keys
			//				* in the time span covered by \c pCurveMerge.
			//				* \param pCurveIn Source curve.
			//				* \param pCurveMerge Curve containing keys to merge in source curve.
			//				* \param pCurveOut Destination for source curve and keys to merge.
			//				*/
			//				static void Merge(KFCurve &pCurveIn, KFCurve &pCurveMerge, KFCurve &pCurveOut);
			//
			//				//! Compute time difference between the first key of a curve and a given time.
			//				static KTime FindTimeOffsetBefore(KFCurve pCurve, KTime& pTime);
			//
			//				//! Compute time difference between the last key of a curve and a given time.
			//				static KTime FindTimeOffsetAfter(KFCurve pCurve, KTime& pTime);
			//
			//				/**
			//				* \name Error Management
			//				*/
			//				//@{
			//
			//				//! Status codes.
			//				enum EError
			//				{
			//					// Interpolation & tangent methods
			//					eNoKey,
			//					eMultiple,
			//
			//					// Curve sync methods
			//					eMultipleKeyCount,
			//					eUnsynchedKeys,
			//
			//					// Filtering methods
			//					eLeftUnchanged,
			//
			//					eErrorCount
			//				};
			//
			//				//! Retrieve error object.
			//				static KError& GetError();
			//
			//				//! Get last error code.
			//				static EError GetLastErrorID();
			//
			//				//! Get last error code.
			//				static const char* GetLastErrorString();
			//
			//				//@}
			//
			//				///////////////////////////////////////////////////////////////////////////////
			//				//
			//				//  WARNING!
			//				//
			//				//  Anything beyond these lines may not be documented accurately and is
			//				//  subject to change without notice.
			//				//
			//				///////////////////////////////////////////////////////////////////////////////
			//
			//#ifndef DOXYGEN_SHOULD_SKIP_THIS
			//
			//			protected:
			//
			//				//! No instance of this class needs to be created.
			//				KFCurveUtils();
			//
			//				static KError smError;
			//
			//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};	
	}
}