#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "FbxProperty.h"
#include "FbxString.h"

	namespace FbxSDK
	{
		namespace IO
		{						

			bool FbxStreamOptionsManaged::SetOption(String^ name, int value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				bool b = _Ref()->SetOption<int>(n,value);					
				FREECHARPOINTER(n);
				return b;
			}
			bool FbxStreamOptionsManaged::SetOption(String^ name, bool value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				bool b = _Ref()->SetOption<bool>(n,value);					
				FREECHARPOINTER(n);
				return b;
			}
			bool FbxStreamOptionsManaged::SetOption(String^ name, String^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				FbxString kstr(n);
				bool b = _Ref()->SetOption<FbxString>(n,kstr);
				FREECHARPOINTER(n);
				return b;
			}
										
			void FbxStreamOptionsManaged::Reset()
			{
				_Ref()->Reset();
			}
			FbxPropertyManaged^ FbxStreamOptionsManaged::GetOption(FbxString^ name)
			{
				return gcnew FbxPropertyManaged(_Ref()->GetOption(*name->_Ref()));
			}
			FbxPropertyManaged^ FbxStreamOptionsManaged::GetOption(String^ name)
			{				
				STRINGTO_CONSTCHAR_ANSI(n,name);
				FbxPropertyManaged^ p = gcnew FbxPropertyManaged(&_Ref()->GetOption(n));
				FREECHARPOINTER(n);
				return p;
			}
			bool FbxStreamOptionsManaged::SetOption(FbxPropertyManaged^ fProperty)
			{
				return _Ref()->SetOption(*fProperty->_Ref());
			}
			bool FbxStreamOptionsManaged::CopyFrom(FbxStreamOptionsManaged^ streamOptionsSrc)
			{
				return _Ref()->CopyFrom(streamOptionsSrc->_Ref());
			}
#ifndef DOXYGEN_SHOULD_SKIP_THIS			

			CLONE_DEFINITION(FbxStreamOptionsManaged,KFbxStreamOptions);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS			
		}
	}