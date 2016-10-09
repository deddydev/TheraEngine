#pragma once
#include "stdafx.h"
#include "FbxSystemUnit.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxSceneManaged;
		ref class FbxAxisSystem;
		ref class FbxNode;
		/** \brief This class collects static functions for manipulating Fbx_Root nodes. 
		* Fbx_Root nodes were used to orient and scale scenes from other graphics applications. They have been replaced by the 
		* conversion routines in KFbxAxisSystem and KFbxSystemUnit. These methods are provided for backward compatibility only 
		* and will eventually be removed. Use the conversion routines in KFbxSystemUnit and KFbxAxisSystem when possible.
		*/
		public ref class FbxRootNodeUtility abstract sealed
		{
		public:

			//static String^ FbxRootNodePrefix;

			/** This method strips the scene of all Fbx_Root nodes.
			* \param pScene     The scene to convert
			* \return           \c true if successful, \c false otherwise.
			* \remarks          Converts the children of any Fbx_Roots to the orientation
			*                   and units that the Fbx_Root transformation represented.
			*                   The scene should look unchanged.
			*/
			static bool RemoveAllFbxRoots(FbxSceneManaged^ scene);

			/** Inserts an Fbx_Root node into the scene to orient the 
			* scene from its axis and unit systems to the specified ones.
			* \param pScene            The scene to convert
			* \param pDstAxis          Destination axis.
			* \param pDstUnit          Destination unit
			* \param pUnitOptions      Unit conversion options
			* 
			*/
			static bool InsertFbxRoot(FbxSceneManaged^ scene, 
				FbxAxisSystem^ dstAxis, 
				FbxSystemUnit^ dstUnit,
				FbxSystemUnit::FbxUnitConversionOptions unitOptions);
			static bool InsertFbxRoot(FbxSceneManaged^ scene, 
				FbxAxisSystem^ dstAxis, 
				FbxSystemUnit^ dstUnit);

			/** Check if a node is an Fbx_Root node
			* \param pNode     The node to query
			* \return          \c true if pNode is a Fbx_Root node, false otherwise
			*/
			static bool IsFbxRootNode(FbxNode^ node);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////		


#ifndef DOXYGEN_SHOULD_SKIP_THIS


#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}