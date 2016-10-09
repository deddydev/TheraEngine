#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;
		/** This class contains the data structure support for char pointer set.
		*/

		public ref class FbxCharPtrSet : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCharPtrSet,KCharPtrSet);
			INATIVEPOINTER_DECLARE(FbxCharPtrSet,KCharPtrSet);		

		public:

			/** Class constructor
			*	\param pItemPerBlock Number of item per block. Default is 20.
			*/
			FbxCharPtrSet( int itemPerBlock);
			DEFAULT_CONSTRUCTOR(FbxCharPtrSet,KCharPtrSet);			

			/** Add a new item.
			*	\param pReference char pointer reference to the item.
			*	\param pItem kReference to the item.
			*/
			/*void Add(System::String^ reference, fbxReference^ item )
			{
			char* c = new char(FbxString::NumCharToCreateString);
			FbxString::StringToChar(reference,c);
			ptrSet->Add(c,*item->);
			}*/

			/** Removes an item.
			*	\param pReference char reference to the item.
			* \return true if succes.
			*/
			bool Remove (String^ reference);

			/** Get an item's reference.
			*	\param pReference char reference to the item.
			*	\param PIndex index to the item.
			* \return kReference to the item, NULL if fails.
			*/
			//kReference Get    ( const char* pReference, int* PIndex = NULL );

			/** Get an item's reference from index.
			*	\param pIndex index to the item.
			* \return kReference to the item, NULL if fails.
			*/
			//kReference& operator[]( int pIndex );

			/** Get an item's reference from index.
			*	\param pIndex index to the item.
			*	\param pReference char reference to the item.
			* \return kReference to the item, NULL if fails.
			*/
			//kReference	GetFromIndex	( int pIndex, const char* *pReference = NULL);

			/** Removes an item by index.
			*	\param pIndex index to the item.
			*/
			void RemoveFromIndex(int index);

			/** Get the number of item in the array.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Count);

			/** Sorts the array.
			*/        
			void Sort();

			/** Clears the array.
			*/
			void Clear();

		};

	}
}