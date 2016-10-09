#pragma once
#include "stdafx.h"
#include "FbxObject.h"
#include <kfbxplugins/kfbxkfcurvefilters.h>


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxTime;
		ref class FbxCurve;
		ref class FbxCurveNode;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxErrorManaged;
		ref class FbxXMatrix;
		/** \brief Base class for KFCurveNode and KFCurve filtering.
		* \nosubgrouping
		* A class is necessary to hold the parameters of a filtering
		* algorithm.  Independent UI can then be attached to those
		* parameters.
		*/
		public ref class FbxCurveFilters : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxKFCurveFilters);
		internal:
			FbxCurveFilters(KFbxKFCurveFilters* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxCurveFilters);
		protected:
			virtual void CollectManagedMemory() override;
		public:			
			/** Get the Name of the Filter
			* \return     Pointer to name.
			*/
			//virtual const char* GetName() {return NULL;}

			/** Get the Start Time
			* \return     The time expressed as KTime.
			*/
			/** Set the Start Time
			* \param pTime     The time to be set.
			*/
			virtual REF_PROPERTY_GETSET_DECLARE(FbxTime,StartTime);

			/** Get the Stop Time
			* \return     The time expressed as KTime.
			*/
			/** Set the Stop Time
			* \param pTime     The time to be set.
			*/
			virtual REF_PROPERTY_GETSET_DECLARE(FbxTime,StopTime);


			/** Get the Start Key
			* \param pCurve     Curve on which we want to retrieve the start key
			* \return           The position of the start key
			*/
			virtual int GetStartKey(FbxCurve^ curve);

			/** Get the Stop Key
			* \param pCurve     Curve on which we want to retrieve the stop key
			* \return           The position of the stop key
			*/
			virtual int GetStopKey(FbxCurve^ curve);

			/** Check if the KFCurveNode need an application of the filter.
			* \param pCurveNode     Curve to test if it needs application of filter
			* \param pRecursive     Check recursively through the Curve
			* \return               \c true if the KFCurveNode need an application of the filter.
			*/
			virtual bool NeedApply(FbxCurveNode^ curveNode, bool recursive);
			bool NeedApply(FbxCurveNode^ curveNode)
			{
				return NeedApply(curveNode,true);
			}

			/** Check if one KFCurve in an array needs an application of the filter.
			* \param pCurve     Array of Curves to test if it needs application of filter
			* \param pCount     Number of Curves in array to test
			* \return           \c true if one KFCurve in an array need an application of the filter.
			*/
			//virtual bool NeedApply(KFCurve** pCurve, int pCount){return false;}

			/** Check if a KFCurve need an application of the filter.
			* \param pCurve     Curve to test if it needs application of filter
			* \return           \c true if the KFCurve need an application of the filter.
			*/
			virtual bool NeedApply(FbxCurve^ curve);

			/** Apply filter on a KFCurveNode.
			* \param pCurveNode     Curve to apply the filter
			* \param pRecursive     Apply recursively through the Curve
			* \return               \c true if successful, \c false otherwise.
			*/
			virtual bool Apply(FbxCurveNode^ curveNode, bool recursive);
			bool Apply(FbxCurveNode^ curveNode)
			{
				return Apply(curveNode, true);
			}

			/** Apply filter on a number of KFCurve.
			* \param pCurve     Array of curves to apply the filter
			* \param pCount     Number of curves in array to apply the filter
			* \return           \c true if successful, \c false otherwise.
			*/
			//virtual bool Apply(KFCurve** pCurve, int pCount){return false;}

			/** Apply filter on a KFCurve.
			* \param pCurve         Curve to apply the filter
			* \return               \c true if successful, \c false otherwise.
			*/
			virtual bool Apply(FbxCurve^ curve);

			/** Reset default parameters.
			*/
			virtual void Reset();

			/** Retrieve error object.
			* \return     Error object.
			*/
			virtual REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** Get last error ID.
			* \return     Last error ID.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(int,LastErrorID);

			/** Get last error name.
			* \return     Last error name.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(String^,LastErrorString);			
		};




		/** \brief Key reducing filter.
		* \nosubgrouping
		* Filter to test if each key is really necessary to define the curve
		* at a definite degree of precision. It filters recursively from the
		* strongest difference first. All useless keys are eliminated.
		*/
		public ref class FbxCurveFilterConstantKeyReducer : FbxCurveFilters
		{
			REF_DECLARE(FbxEmitter,KFbxKFCurveFilterConstantKeyReducer);
		internal:
			FbxCurveFilterConstantKeyReducer(KFbxKFCurveFilterConstantKeyReducer* instance) : FbxCurveFilters(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxCurveFilterConstantKeyReducer);			

		public:			

			/** Get the derivative tolerance.
			* \return     The value of the derivative tolerance.
			*/
			/** Set the derivative tolerance.
			* \param pValue     Value derivative tolerance.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DerivativeTolerance);				

			/** Get the tolerance value.
			* \return     The tolerance value.
			*/			
			/** Set the tolerance value.
			* \param pValue     Tolerance value.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,ValueTolerance);			

			/** Get the state of the KeepFirstAndLastKeys flag.
			* \return      \c true if the filter keeps the first and last keys.
			*/			
			/** Set the state of the KeepFirstAndLastKeys flag.
			* \param pKeepFirstAndLastKeys     Set to \c true if you want the filter to keep the first and last keys.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,KeepFirstAndLastKeys);			

			/** Get the state of the KeepOneKey flag.
			* \return     \c true if the filter keeps one keys.
			*/			
			/** Set the state of the KeepOneKey flag.
			* \param pKeepOneKey     Set to \c true if you want the filter to keep one key.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,KeepOneKey);			

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			//
			//  If ValueTolerance is default, we use the thresholds here, otherwise
			//  it is the ValueTolerance that is used. (Mainly for backward compatibility)
			//
			void SetTranslationThreshold ( double translationThreshold );
			void SetRotationThreshold    ( double rotationThreshold );
			void SetScalingThreshold     ( double scalingThreshold );
			void SetDefaultThreshold     ( double defaultThreshold );		
#endif
		};




		/** Matrix conversion filter.
		* \nosubgrouping
		*/
		public ref class FbxCurveFilterMatrixConverter : FbxCurveFilters
		{
			REF_DECLARE(FbxEmitter,KFbxKFCurveFilterMatrixConverter);
		internal:
			FbxCurveFilterMatrixConverter(KFbxKFCurveFilterMatrixConverter* instance):FbxCurveFilters(instance)
			{
				_Free =false;
			}
			FBXOBJECT_DECLARE(FbxCurveFilterMatrixConverter);


		public:
			

			/** \enum EMatrixID Matrix ID
			* - \e ePreGlobal
			* - \e ePreTranslate
			* - \e ePostTranslate
			* - \e ePreRotate
			* - \e ePreScale
			* - \e ePostGlobal
			* - \e eScaleOffset
			* - \e eInactivePre
			* - \e eInactivePost
			* - \e eRotationPivot
			* - \e eScalingPivot
			* - \e eMatrixCount
			*/
			enum class MatrixID
			{
				PreGlobal = KFbxKFCurveFilterMatrixConverter::ePreGlobal,
				PreTranslate = KFbxKFCurveFilterMatrixConverter::ePreTranslate,
				PostTranslate = KFbxKFCurveFilterMatrixConverter::ePostTranslate,
				PreRotate = KFbxKFCurveFilterMatrixConverter::ePreRotate,
				PostRotate = KFbxKFCurveFilterMatrixConverter::ePostRotate,
				PreScale = KFbxKFCurveFilterMatrixConverter::ePreScale,
				PostScale = KFbxKFCurveFilterMatrixConverter::ePostScale,
				PostGlobal = KFbxKFCurveFilterMatrixConverter::ePostGlobal,
				ScaleOffset = KFbxKFCurveFilterMatrixConverter::eScaleOffset,
				InactivePre = KFbxKFCurveFilterMatrixConverter::eInactivePre,
				InactivePost = KFbxKFCurveFilterMatrixConverter::eInactivePost,
				RotationPivot = KFbxKFCurveFilterMatrixConverter::eRotationPivot,
				ScalingPivot = KFbxKFCurveFilterMatrixConverter::eScalingPivot,
				MatrixCount = KFbxKFCurveFilterMatrixConverter::eMatrixCount
			};

			/** Get the Translation Rotation Scaling source matrix
			* \param pIndex      The matrix ID.
			* \param pMatrix     The matrix used to receive the source matrix.
			*/
			void GetSourceMatrix(MatrixID index, FbxXMatrix^ matrix);

			/** Set the Translation Rotation Scaling source matrix.
			* \param pIndex      The matrix ID.
			* \param pMatrix     The matrix used to set the source matrix.
			*/
			void SetSourceMatrix(MatrixID index, FbxXMatrix^ matrix);

			/** Get the Translation Rotation Scaling destination matrix.
			* \param pIndex      The matrix ID.
			* \param pMatrix     The matrix used to receive the destination matrix.
			*/
			void GetDestMatrix(MatrixID index, FbxXMatrix^ matrix);

			/** Set the Translation Rotation Scaling destination matrix.
			* \param pIndex      The matrix ID.
			* \param pMatrix     The matrix used to set the destination matrix.
			*/
			void SetDestMatrix(MatrixID index, FbxXMatrix^ matrix);

			/** Get the Resampling Period.
			* \return     the Resampling Period.
			*/
			/** Set the Resampling period.
			* \param pResamplingPeriod     The Resampling Period to be set.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTime^,ResamplingPeriod);				

			/** Check if the last key is exactly at the end time.
			* \return     \c true if last key is set exactly at end time.
			*/
			/** Set the last key to be is exactly at end time or not
			* \param pFlag     value to set if last key is set exactly at end time.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,GenerateLastKeyExactlyAtEndTime);				

			/** Check if resampling is on frame rate multiple
			* \return     \c true if resampling is on a frame rate multiple.
			*/			
			/** Set the resample on a frame rate multiple.
			* \param pFlag     The value to be set
			* \remarks         It might be necessary that the starting time of the converted
			*                  animation starts at an multiple of frame period starting from time 0.
			*                  Most softwares play their animation at a definite frame rate, starting
			*                  from time 0.  As resampling occurs when we can't garantee interpolation,
			*                  keys must match with the moment when the curve is evaluated.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ResamplingOnFrameRateMultiple);			

			/** Get if Apply Unroll is used
			* \return     \c true if unroll is applied.
			*/			
			/** Set if Apply Unroll is used
			* \param pFlag     Value to set
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ApplyUnroll);			

			/** Get if constant key reducer is used
			* \return     \c true if constant key reducer is applied.
			*/			
			/** Set if constant key reducer is used
			* \param pFlag     value to set
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ApplyConstantKeyReducer);			

			/** Get if the Resample Translation is used
			* \return      \c true if translation data is resampled upon conversion.
			* \remarks     If this flag isn't set, translation data must be calculated
			*              after the conversion process, overriding the resampling process.
			*/			
			/** Set the resample translation data.
			* \param pFlag     Value to be set.
			* \remarks         If this flag isn't set, translation data must be calculated
			*                  after the conversion process, overriding the resampling process.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ResampleTranslation);			

			/** Set the Rotation Order of the Source
			* \param pOrder     the order to be set
			*/
			void SetSrcRotateOrder(int order);

			/** Set the Rotation Order of the Destination
			* \param pOrder     the order to be set
			*/
			void SetDestRotateOrder(int order);						

			/** Get if the force apply is used
			* \return     \c true if the force apply is used
			*/
			/** Set to force apply even if source and destination matrices are equivalent
			* \param pVal     If the forces apply is to be used
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ForceApply);									
		};




		/** Resampling filter.
		* \nosubgrouping
		*/
		public ref class FbxCurveFilterResample : FbxCurveFilters
		{
			REF_DECLARE(FbxEmitter,KFbxKFCurveFilterResample);
		internal:
			FbxCurveFilterResample(KFbxKFCurveFilterResample* instance):FbxCurveFilters(instance)
			{
				_Free =false;
			}
			FBXOBJECT_DECLARE(FbxCurveFilterResample);			

		public:			

			/** Get if the keys are on frame
			* \return     Value if keys are on frame multiples.
			*/
			/** Set if the keys are on frame
			* \param pKeysOnFrame     value if keys are set on frame multiples.
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(bool,KeysOnFrame);

			/** Get the Resampling period
			* \return     The Resampling period
			*/			
			/** Set the Resampling Period
			* \param pPeriod     The Resampling Period to be set
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTime^,PeriodTime);			


			/** Get the Intelligent Mode
			* \return     the Intelligent Mode
			*/			
			/** Set the Intelligent Mode
			* \param pIntelligent     the Intelligent Mode to be set
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,IntelligentMode);
		};

		/**Unroll filter
		*\nosubgrouping
		*/
		public ref class FbxCurveFilterUnroll : FbxCurveFilters
		{
			REF_DECLARE(FbxEmitter,KFbxKFCurveFilterUnroll);
		internal:
			FbxCurveFilterUnroll(KFbxKFCurveFilterUnroll* instance): FbxCurveFilters(instance)
			{
				_Free= false;
			}
			FBXOBJECT_DECLARE(FbxCurveFilterUnroll);
		public:			



			/** Get quality tolerance.
			* \return     The Quality Tolerance
			*/
			/** Set quality tolerance.
			* \param pQualityTolerance     Value to be set.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,QualityTolerance);						

			/** Get if the test path is enabled
			* \return     \c true if test for path is enabled.
			*/			
			/** Set if the test path is enabled
			* \param pTestForPath     Value to set if test for path is to be enabled.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,TestForPath);			
		};

	}
}