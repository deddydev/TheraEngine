#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;
		/** Name class.
		*	Provides two stings, current and an initial name, for reversible
		* renaming.  This is especially useful for name clashing, renaming
		*	strategies and merging back to a former 3D scene using the initial
		*	names.
		*/
		public ref class FbxName : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxName,KName);
			INATIVEPOINTER_DECLARE(FbxName,KName);
		internal:			
			FbxName(KName n)
			{
				_SetPointer(new KName(),true);
				*_FbxName = n;
			}
		public:

			/** Constructor.
			* \param pInitialName Name string used to initialize both members (initialName and currentName)
			* of this class.
			*/
			FbxName(String^ initialName);
			DEFAULT_CONSTRUCTOR(FbxName,KName);
			//! Copy constructor.
			FbxName(FbxName^ name);


			/** Get initial name.
			* \return Pointer to the InitialName string buffer.
			*/
			/** Set initial name.
			* \param pInitialName New string for the initial name.
			*	\remarks The current name will also be changed to this value.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,InitialName);			

			/** Get current name.
			* \return Pointert to the CurrentName string buffer.
			*/
			/** Set current name.
			* \param pNewName New string for the current name.
			* \remarks The initial name is not affected.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^,CurrentName);					

			/** Get the namespace.
			* \return Pointert to the CurrentName string buffer.
			*/			
			/** Set the namespace.
			* \param pNameSpace New string for the namespace.
			* \remarks The initial name is not affected.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^,NameSpace);					

			/** Check if the current name and internal name match.
			* \return \c true if current name isn't identical to initial name.
			*/
			property bool IsRenamed
			{
				bool get();
			}


			//! Assignment operator
			void CopyFrom(FbxName^ name);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			/** Get the namespaces in a string pointer array format.
			* \return KArrayTemplate<FbxString*> .
			*/
			//KArrayTemplate<FbxString*> GetNameSpaceArray(char identifier);								

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}	
}