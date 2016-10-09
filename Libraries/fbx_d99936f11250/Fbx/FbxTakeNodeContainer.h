#pragma once
#include "stdafx.h"
#include "FbxObject.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxTakeNode;
		ref class FbxTime;
		ref class FbxProperty;
		/**	\brief This class is a container for take nodes which contain animation data.
		* \nosubgrouping
		* A take node contains the animation keys of the container for a given take.
		*  
		* A default take node is always created. This take is used to store default animation 
		* values. This take is at index 0, and should not contain animation keys or 
		* be removed in any situation.
		* 
		* Be careful when processing take animation. If the current 
		* take does not exist for a container, KFbxTakeNodeContainer::GetCurrentTakeNode() and 
		* KFbxTakeNodeContainer::GetDefaultTakeNode() will return the same KFbxTakeNode object. 
		*	If both the default and the current take node are processed without
		*	appropriate testing, the same data will be processed twice.
		*/
		public ref class FbxTakeNodeContainer : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxTakeNodeContainer);
		internal:
			FbxTakeNodeContainer(KFbxTakeNodeContainer* instance): FbxObjectManaged(instance)
			{
				_Free = false;
			}

		//	/**
		//	* \name Take Node Management
		//	*/
		//	//@{
		public:

			/**	Create a take node.
			*	\param pName New take node name.
			*	\return Pointer to the created take node or,  if a take node with the same name
			* already exists in the node, \c NULL.
			* In this case, KFbxNode::GetLastErrorID() returns eTAKE_NODE_ERROR and
			* KFbxNode::GetLastErrorString() returns "Take node already exists".
			*/
			FbxTakeNode^ CreateTakeNode(String^ name);			

			/**	Remove take node by index.
			*	\param pIndex Index of take node to remove.
			*	\return \c true on success, \c false otherwise.
			*	In the last case, KFbxNode::GetLastErrorID() can return one of the following:
			*		- eCANNOT_REMOVE_DEFAULT_TAKE_NODE: The default take node can't be removed.
			*		- eINDEX_OUT_OF_RANGE: The index is out of range.
			*/
			bool RemoveTakeNode(int index);

			/**	Remove take node by name.
			*	\param pName Take node name.
			*	\return \c true on success, \c false otherwise.
			*	In the last case, KFbxNode::GetLastErrorID() can return one of the following:
			*		- eCANNOT_REMOVE_DEFAULT_TAKE_NODE: The default take node can't be removed.
			*		- eUNKNOWN_TAKE_NODE_NAME: No take node with this name exists in the current node.
			*/
			bool RemoveTakeNode(String^ name);

			/**	Get take node count.
			*	\return The number of take nodes including the default take node.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,TakeNodeCount);			

			/**	Get name of take node at a given index.
			*	\param pIndex Take node index.
			*	\return Name of take node  or \c NULL if the index is out of range.
			* In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*	\remarks Index 0 returns the name of the default take.
			*/
			String^ GetTakeNodeName(int index);

			/**	Set current take node by index.
			*	\param pIndex Index of the current take node.
			*	\return \c true on success, \c false otherwise.
			*	In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE
			* and the current take node is set to index 0, the default take node.
			*/
			virtual bool SetCurrentTakeNode(int index);

			/**	Set current take node by name.
			*	\param pName Name of the current take node.
			*	\return \c true on success, \c false otherwise.
			*	In the last case, KFbxNode::GetLastErrorID() returns eUNKNOWN_TAKE_NODE_NAME
			* and the current take node is set to index 0, the default take node.
			*/
			virtual bool SetCurrentTakeNode(String^ name);


			/**	Get the name of the current take node.
			*	\return Name of the current take node.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,CurrentTakeNodeName);


			/**	Get index of the current take node.
			*	\return Index of the current take node.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CurrentTakeNodeIndex);

			/** Find the start and end time of the current take.
			*	\param pStart Reference to store start time.
			*	\param pStop Reference to store end time.
			*	\return \c true on success, \c false otherwise.
			*/
			virtual bool GetAnimationInterval(FbxTime^ start, FbxTime^ stop);

			//@}

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			///////////////////////////////////////////////////////////////////////////////
			//  WARNING!
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			///////////////////////////////////////////////////////////////////////////////
			//bool IsAnimated();
			//bool IsChannelAnimated(char* pGroup, char* pSubGroup, char* pName);
			//bool IsChannelAnimated(char* pGroup, char* pSubGroup, KDataType* pDataType);

			//void RegisterDefaultTakeCallback(KFbxProperty& pProperty, int pComponentIndex, HKFCurve pFCurve);

		public:
			void Init();
			void Reset();

			virtual void PropertyAdded(FbxProperty^ prop);
			virtual void PropertyRemoved(FbxProperty^ prop);		        

			void UpdateFCurveFromProperty(FbxProperty^ prop,FbxTakeNode^ takeNode);
			void CreateChannelsForProperty(FbxProperty^ prop,FbxTakeNode^ takeNode);

			//void UnregisterDefaultTakeCallback(FbxDefaultTakeCallback*& pTC);
			void UnregisterAllDefaultTakeCallback();
			//friend void DefaultTakeValueChangedCallback(KFCurve *pFCurve, KFCurveEvent *pFCurveEvent, void* pObject);		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS			

		};

	}
}