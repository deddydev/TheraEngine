#pragma once
#include "stdafx.h"
#include "Fbx.h"



{
	namespace FbxSDK
	{
		/**	FBX SDK weighted mapping class.
		* \nosubgrouping
		*/
		public ref class FbxWeightedMapping : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxWeightedMapping,KFbxWeightedMapping);
			INATIVEPOINTER_DECLARE(FbxWeightedMapping,KFbxWeightedMapping);

		public:

			enum class Set
			{
				Source = KFbxWeightedMapping::eSOURCE,
				Destination = KFbxWeightedMapping::eDESTINATION
			};

			value struct Element
			{
			public:
				int Index;
				double Weight;
			};

			/** 
			* \name Constructor and Destructor
			*/
			//@{

			/** Constructor
			* \param pSourceSize       Source set size
			* \param pDestinationSize  Destination set size
			*/
			FbxWeightedMapping(int sourceSize, int destinationSize);			


			/** Remove all weighted relations and give new source and destination sets sizes.
			* \param pSourceSize       New source set size
			* \param pDestinationSize  New destination set size
			*/
			void Reset(int sourceSize, int destinationSize);

			/** Add a weighted relation.
			* \param pSourceIndex      
			* \param pDestinationIndex 
			* \param pWeight           
			*/
			void Add(int sourceIndex, int destinationIndex, double weight);

			/** Get the number of elements of a set.
			* \param pSet              
			*/
			int GetElementCount(Set set);

			/** Get the number of relations an element of a set is linked to.
			* \param pSet               
			* \param pElement          
			*/
			int GetRelationCount(Set set, int element);

			/** Get one of the relations an element of a set is linked to.
			* \param pSet              
			* \param pElement          
			* \param pIndex            
			* \return                  KElement gives the index of an element in the other set and the assigned weight.
			*/
			Element GetRelation(Set set, int element, int index);

			/** Given the index of an element in the other set, get the index of one of the relations 
			*  an element of a set is linked to. Returns -1 if there is not relation between these elements.
			* \param pSet
			* \param pElementInSet
			* \param pElementInOtherSet
			* \return                  the index of one of the relations, -1 if there is not relation between these elements.         
			*/
			int GetRelationIndex(Set set, int elementInSet, int elementInOtherSet);

			/** Get the sum of the weights from the relations an element of a set is linked to.
			* \param pSet
			* \param pElement
			* \param pAbsoluteValue
			* \return                 the sum of the weights  from the relations.
			*/
			double GetRelationSum(Set set, int element, bool absoluteValue);


			/** Normalize the weights of the relations of all the elements of a set.
			* \param pSet
			* \param pAbsoluteValue
			*/
			void Normalize(Set set, bool absoluteValue);		

		};		

	}
}