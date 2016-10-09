#pragma once
#include "stdafx.h"
#include "FbxObject.h"


namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxErrorManaged;
		ref class FbxWeightedMapping;	
		ref class FbxGeometry;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** \brief This class provides the structure to build a correspondance between 2 geometries.
		*
		* This correspondance is done at the vertex level. Which means that for each vertex in the
		* source geometry, you can have from 0 to N corresponding vertices in the destination
		* geometry. Each corresponding vertex is weighted.
		*
		* For example, if the source geometry is a NURB and the destination geometry is a mesh,
		* the correspondance object will express the correspondance between the NURB's control vertices
		* and the mesh's vertices.
		*
		* If the mesh corresponds to a tesselation of the NURB, the correspondance object can be used
		* to transfer any deformation that affect the NURB's control vertices to the mesh's vertices.
		*
		* See KFbxWeightedMapping for more details.
		*/
		public ref class FbxGeometryWeightedMap : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxGeometryWeightedMap);
		internal:
			FbxGeometryWeightedMap(KFbxGeometryWeightedMap* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory() override;
		public:

			FBXOBJECT_DECLARE(FbxGeometryWeightedMap);
			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				Error,
				ErrorCount
			};

			/** Get last error code.
			* \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			* \return     Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}

			//@}

			/** Set correspondance values.
			* \param pWeightedMappingTable     Pointer to the table containing values
			* \return                          Pointer to previous correspondance values table.
			*/
			FbxWeightedMapping^ SetValues(FbxWeightedMapping^ weightedMappingTable);

			/** Return correspondance values.
			* \return     Pointer to the correspondance values table.
			*/
			FbxWeightedMapping^ GetValues();

			/** Return source geometry.
			* \return     Pointer to the source geometry, or \c NULL if there is no connected source geometry
			*/
			REF_PROPERTY_GET_DECLARE(FbxGeometry,SourceGeometry);

			/** Return destination geometry.
			* \return     Pointer to the destination geometry, or \c NULL if there is no connected destination geometry
			*/			
			REF_PROPERTY_GET_DECLARE(FbxGeometry,DestinationGeometry);

			//! Assigment operator
			//KFbxGeometryWeightedMap& operator= (KFbxGeometryWeightedMap const& pGeometryWeightedMap);

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
			CLONE_DECLARE();
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};
	}
}