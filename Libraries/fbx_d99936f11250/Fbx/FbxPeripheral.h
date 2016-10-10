#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{		
		/**FBX SDK peripheral class
		* \nosubgrouping
		*/

		public ref class FbxPeripheral : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxPeripheral,KFbxPeripheral);
			INATIVEPOINTER_DECLARE(FbxPeripheral,KFbxPeripheral);

			public:
				/**
				* \name Constructor and Destructor
				*/
				//@{				

				///** Reset the peripheral to its initial state.
				//*/
				//virtual void Reset();

				///** Unload the content of pObject.
				//* \param pObject                 Object who's content is to be offloaded into 
				//* the peripheral storage area.
				//* \return                        \c true if the object content has been successfully transferred.
				//*/
				//virtual bool UnloadContentOf(FbxObject^ obj);

				///** Load the content of pObject.
				//* \param pObject                 Object who's content is to be loaded from
				//* the peripheral storage area.
				//* \return                        \c true if the object content has been successfully transferred.
				//*/
				//virtual bool LoadContentOf(FbxObject^ obj);

				///** Check if this peripheral can unload the given object content.
				//* \param pObject                 Object who's content has to be transferred.
				//* \return                        \c true if the peripheral can handle this object content AND/OR
				//* has enough space in its storage area.
				//*/
				//virtual bool CanUnloadContentOf(FbxObject^ obj);

				///** Check if this peripheral can load the given object content.
				//* \param pObject                  Object who's content has to be transferred.
				//* \return                         \c true if the peripheral can handle this object content
				//*/
				//virtual bool CanLoadContentOf(FbxObject^ obj);

				///** Initialize the connections of an object
				//* \param pObject                  Object on which the request for connection is done
				//*/
				//virtual void InitializeConnectionsOf(FbxObject^ obj);

				///** Uninitialize the connections of an object
				//* \param pObject                 Object on which the request for deconnection is done
				//*/
				//virtual void UninitializeConnectionsOf(FbxObject^ obj);
		};


	}
}