#pragma once
#include "stdafx.h"
#include "FbxObject.h"
#include "KFbxIO/kfbxstreamoptions.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxProperty;
		ref class FbxStringManaged;
		namespace IO
		{
			/**	\brief This class provides the structure to build a KFbx Stream Option.
			*	This class is a composite class that contains stream options management services. 
			* The content of a Kfbx Stream Option is stored in a property (KFbxProperty).
			*/

			public ref class FbxStreamOptionsManaged : FbxObjectManaged
			{
			internal:
				FbxStreamOptionsManaged(KFbxStreamOptions* instance) : FbxObjectManaged(instance)
				{
					_Free = false;
				}
				REF_DECLARE(FbxEmitter,KFbxStreamOptions);				
			public:


				
				/** Reset all the options to default value
				*/
				virtual void Reset();

				/** Get a Stream Option by Stream Option Name.
				* \param pName     The name of the Stream Option
				* \return          A KFbxProperty if the name is valid.
				* \remarks         In the last case, an assert is raised
				*/
				FbxProperty^ GetOption(FbxStringManaged^ name);


				/** Get a Stream Option by Stream Option Name.
				* \return     A KFbxProperty if the name is valid.
				* \remarks    In the last case, an assert is raised
				*/
				FbxProperty^ GetOption(String^ name);											

				/** Set a Stream Option by Stream Option Name and a Value.
				* \param pName     Name of the option where a change is needed.
				* \param pValue    Value to be set.
				* \return          \c true if the Stream Option was found and the value has been set.
				*/
				bool SetOption(String^ name, int value);
				bool SetOption(String^ name, bool value);
				bool SetOption(String^ name, String^ value);

				
				/** Set a Stream Option by a Property (KFbxProperty).
				* \param pProperty     Property containing the value to be set.
				* \return              \c true if the Property has been set, otherwise \c false.
				*/
				bool SetOption(FbxProperty^ fProperty);

				/** Copies the properties of another KFbxStreamOptions.
				* \param pKFbxStreamOptionsSrc     Contains the properties to be copied
				*/
				bool CopyFrom(FbxStreamOptionsManaged^ streamOptionsSrc);

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
				//clone
				CLONE_DECLARE();
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
			};
		}
	}
}