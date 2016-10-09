#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStringManaged;

		//! Error class.
		public ref class FbxErrorManaged : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxErrorManaged,KError);
			INATIVEPOINTER_DECLARE(FbxErrorManaged,KError);
//		internal:
//			KError* er;
//			bool isNew;
//			FbxError(KError* e);
//
//		public:
//
//			/** Constructor.
//			*/
//			FbxError();
//			~FbxError();
//			!FbxError();
//
//			/** Constructor.
//			*   \param pStringArray The error string table in use.
//			*   \param pErrorCount Number of elements in the error string table.
//			*/
//			FbxError(array<String^>^ stringArray);
//
//
//			/** Get the size of the error string table.
//			*   \return Number of elements in the error string table.
//			*/
//			property int ErrorCount
//			{
//				int get();
//			}
//
//			/** Get the error message string.
//			*   \param pIndex Error index.
//			*   \return Error string.
//			*/
//			String^ GetErrorString(int index);
//
//			/** Set the last error ID and the last error string.
//			*   \param pIndex Error index.
//			* \param pString Error string.
//			*   \remarks This method will also set the last error string to the default
//			*   string value contained in the error string table for this error ID.
//			*/
//			void SetLastError(int index, String^ str);
//
//			/** Return the last error index.
//			*   \return The last error index or -1 if none is set.
//			*/							
//			/** Set the last error index.
//			*   \param pIndex Error index.
//			*   \remarks This method will also set the last error string to the default
//			*   string value contained in the error string table for this error index.
//			*/
//			property int LastErrorID
//			{
//				int get();
//				void set(int value);				
//			}										
//
//			/** Get the message string associated with the last error.
//			*   \return Error string or empty string if none is set.
//			*/
//			/** Set the message string associated with the last error.
//			*   \param pString Error string.
//			*   This method should be called after KError::SetLastErrorID()
//			* in order to customize the error string.
//			*/
//			property String^ LastErrorString
//			{
//				String^ get();
//				void set(String^ value);
//			}																	
//
//			//! Reset the last error.
//			void ClearLastError();
//
//			///////////////////////////////////////////////////////////////////////////////
//			//
//			//  WARNING!
//			//
//			//  Anything beyond these lines may not be documented accurately and is
//			//  subject to change without notice.
//			//
//			///////////////////////////////////////////////////////////////////////////////
//
//#ifndef DOXYGEN_SHOULD_SKIP_THIS								
//
//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}