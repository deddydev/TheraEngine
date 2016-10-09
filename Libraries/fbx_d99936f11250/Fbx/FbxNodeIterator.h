#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxNode;
		/**	This class and iterator type accesses the FbxNode hierarchy.
		* \nosubgrouping
		*	The iterator takes a root node that can be any parent node in the KFbxScene. 
		* The iterator will then only travel within the children of a given root.
		*
		* Since the iterator becomes invalid when the scene hierarchy changes, the 
		* iterator should only used in a fixed scene hierarchy. 
		*/

		public ref class FbxNodeIterator : IFbxNativePointer
		{	
			BASIC_CLASS_DECLARE(FbxNodeIterator,KFbxNodeIterator);
			INATIVEPOINTER_DECLARE(FbxNodeIterator,KFbxNodeIterator);	
		public:

			/** \enum TraversalType  Method by which the node hierarchy is traversed.
			* - \e eDepthFirst           The leaf of the tree are first traversed
			* - \e eBreadthFirst         Each child is traversed before going down to the leafs
			* - \e eDepthFirstParent     Like depth first but the parent of the leafs are returned prior to the leafs themselves
			*/
			enum class TraversalType
			{
				DepthFirst = KFbxNodeIterator::eDepthFirst,
				BreadthFirst = KFbxNodeIterator::eBreadthFirst,
				DepthFirstParent = KFbxNodeIterator::eDepthFirstParent
			};

			/** Contructor
			* \param pRootNode     The root of the iterator hierarchy.
			* \param pType         The traversal type.
			*/
			FbxNodeIterator(FbxNode^ rootNode, FbxNodeIterator::TraversalType type);
			/** Copy Constructor
			* \param pCopy     Iterator to copy
			*/
			FbxNodeIterator(FbxNodeIterator^ copy);			

			/** Get a pointer to the current KFbxNode.
			* \return     The current KFbxNode pointer.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,Current);

			/** Get a pointer to the next KFbxNode.
			* \return     The next KFbxNode pointer, or \c NULL if the end is reached.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,Next);			

			/** Get a pointer to the previous KFbxNode pointer
			* \return     The previous KFbxNode pointer, or \c NULL if the root of the iterator is reached.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,Previous);			

			virtual void Reset();

		};

	}
}