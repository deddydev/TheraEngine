#pragma once
#include "stdafx.h"
#include "FbxGeometry.h"
#include "FbxNodeAttribute.h"

namespace Skill
{
	namespace FbxSDK
	{
		/** \brief A patch is a type of parametric geometry node attribute.
		* \nosubgrouping
		*/
		ref class FbxStream;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		public ref class FbxPatch : FbxGeometry
		{
			REF_DECLARE(FbxEmitter,KFbxPatch);
		internal:
			FbxPatch(KFbxPatch* instance) : FbxGeometry(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxPatch);

		public:			
			//! Return the type of node attribute which is EAttributeType::ePATCH.
			//virtual AttributeType GetAttributeType() ;

			//! Reset the patch to default values.
			void Reset();

			/**
			* \name Patch Properties
			*/
			//@			

			/** Get surface mode.
			* \return     Currently set surface mode identifier.
			*/
			/** Set surface mode.
			* \param pMode     Surface mode identifier (see Class KFbxGeometry)
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(FbxGeometry::SurfaceMode,Surface_Mode);

			/** \enum EPatchType Patch types.
			* - \e eBEZIER
			* - \e eBEZIER_QUADRIC
			* - \e eCARDINAL
			* - \e eBSPLINE
			* - \e eLINEAR
			*/
			enum class PatchType
			{
				Bezier         = 0,
				BezierQuadric = 1,
				Cardinal       = 2,
				Bspline        = 3,
				Linear         = 4
			} ;

			/** Allocate memory space for the array of control points.
			* \param pUCount     Number of control points in U direction.
			* \param pUType      Patch type in U direction.
			* \param pVCount     Number of control points in V direction.
			* \param pVType      Patch type in V direction.
			*/
			void InitControlPoints(int UCount, PatchType UType, int VCount, PatchType VType);

			/** Get number of control points in U direction.
			* \return     Number of control points in U.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UCount);

			/** Get number of control points in V direction.
			* \return     Number of control points in V.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VCount);

			/** Get patch type in U direction.
			* \return     Patch type identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE(PatchType,PatchUType);

			/** Get patch type in V direction.
			* \return     Patch type identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE(PatchType,PatchVType);

			/** Set step.
			* The step is the number of divisions between adjacent control points.
			* \param pUStep     Steps in U direction.
			* \param pVStep     Steps in V direction.
			*/
			void SetStep(int UStep, int VStep);

			/** Get the number of divisions between adjacent control points in U direction.
			* \return     Step value in U direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,UStep);

			/** Get the number of divisions between adjacent control points in V direction.
			* \return     Step value in V direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,VStep);

			/** Set closed flags.
			* \param pU     Set to \c true if the patch is closed in U direction.
			* \param pV     Set to \c true if the patch is closed in V direction.
			*/
			void SetClosed(bool U, bool V);

			/** Get state of the U closed flag.
			* \return     \c true if the patch is closed in U direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,UClosed);

			/** Get state of the V closed flag.
			* \return     \c true if the patch is closed in V direction.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,VClosed);

			/** Set U capped flags.
			* \param pUBottom     Set to \c true if the patch is capped at the bottom in the U direction.
			* \param pUTop \c     Set to \c true if the patch is capped at the top in the U direction.
			* \remarks            Capping options are saved but not loaded by Motionbuilder because they
			*                     are computed from the patch topography.
			*/
			void SetUCapped(bool UBottom, bool UTop);

			/** Get U capped bottom flag state.
			* \return     \c true if the patch is capped at the bottom.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,UCappedBottom);

			/** Get U capped top flag state.
			* \return     \c true if the patch is capped at the top.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,UCappedTop);

			/** Set V capped flags.
			* \param pVBottom     Set to \c true if the patch is capped at the bottom in the V direction.
			* \param pVTop        Set to \c true if the patch is capped at the top in the V direction.
			* \remarks            Capping options are saved but not loaded by Motionbuilder because they
			*                     are computed from the patch topography.
			*/
			void SetVCapped(bool VBottom, bool VTop);

			/** Get V capped bottom flag state.
			* \return     \c true if the patch is capped at the bottom.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,VCappedBottom);

			/** Get V capped top flag state.
			* \return     \c true if the patch is capped at the top.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,VCappedTop);

			//@}

			/**
			* \name Off-loading Serialization section
			*/
			//@{
			//virtual bool ContentWriteTo(FbxStream^ Stream) override;
			//virtual bool ContentReadFrom(FbxStream^ Stream) override;
			//@}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:

			// Clone
			CLONE_DECLARE();			
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}