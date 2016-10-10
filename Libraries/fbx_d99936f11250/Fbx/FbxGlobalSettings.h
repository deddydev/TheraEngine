#pragma once
#include "stdafx.h"
#include "FbxObject.h"



{
	namespace FbxSDK
	{
		ref class FbxAxisSystem;		
		ref class FbxSystemUnit;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** \brief This class contains functions for accessing global settings.
		* \nosubgrouping
		*/
		public ref class FbxGlobalSettings : FbxObjectManaged
		{			
			REF_DECLARE(FbxEmitter,KFbxGlobalSettings);
			FBXOBJECT_DECLARE(FbxGlobalSettings);
		internal:
			FbxGlobalSettings(KFbxGlobalSettings* instance) : FbxObjectManaged(instance)
			{
				_Free =false;
			}
		public:
			//! Assignment operator.
			void CopyFrom(FbxGlobalSettings^ settings);

			/** 
			* \name Axis system
			*/
			//@{
			/** Get the scene's coordinate system.
			* \return     The coordinate system of the current scene, defined by the class kFbxAxisSystem.
			*/
			/** Set the coordinate system for the scene.
			* \param pAxisSystem     Coordinate system defined by the class kFbxAxisSystem.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxAxisSystem^,AxisSystem);



			/** 
			* \name System Units
			*/
			//@{

			/** Get the unit of measurement used by the system.
			* \return     The unit of measurement defined by the class kFbxSystemUnit.     
			*/
			/** Set the unit of measurement used by the system.
			* \param pOther     A unit of measurement defined by the class kFbxSystemUnit. 
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxSystemUnit^,SystemUnit);			


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		public:
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
		};

	}
}