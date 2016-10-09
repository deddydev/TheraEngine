#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxName;
		/** Renaming strategy mechanism.
		*	Base class describing how the renaming process is handled.
		*	This class is intented to be derived into a specialised 
		*	renaming class.
		*
		*	Basicaly, the Rename is called everytime a new element is added to
		*   an entity.  the strategy keep
		*/
		public ref class FbxKRenamingStrategy : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxKRenamingStrategy,KRenamingStrategy);
			INATIVEPOINTER_DECLARE(FbxKRenamingStrategy,KRenamingStrategy);

		public:			
			

			//! Empty all memories about given names
			virtual void Clear();

			/** Rename.
			*	\param pName
			*	\return how the operation went.
			*/
			virtual bool Rename(FbxName^ name);

			/** Spawn mechanism.  
			*	Create a dynamic renaming strategy instance of the same type
			*	the child class.
			*	\return new KRenamingStrategy;	
			*/
			virtual FbxKRenamingStrategy^ Clone();

		};


		/** Usual renaming numbering renaming strategy.
		*	This renaming strategy will be used by the FBXSDK if no other is specified.
		*/
		public ref class FbxNumberRenamingStrategy : FbxKRenamingStrategy
		{
			REF_DECLARE(FbxKRenamingStrategy,KNumberRenamingStrategy);
		internal:
			FbxNumberRenamingStrategy(KNumberRenamingStrategy* instance):FbxKRenamingStrategy(instance)
			{
				_Free = false;
			}
		public:	

			//! Constructor.
			FbxNumberRenamingStrategy();

			//! Destructor.
			//virtual ~KNumberRenamingStrategy ();

			//! Empty all memories about given names
			//virtual void Clear();

			/** Rename.
			*	\param pName
			*	\return how the operation went.
			*/
			//virtual bool Rename(KName& pName);

			/** Spawn mechanism.  
			*	Create a dynamic renaming strategy instance of the same type
			*	the child class.
			*	\return new KNumberRenamingStrategy;	
			*/
			virtual FbxKRenamingStrategy^ Clone() override;
		};		
	}
}