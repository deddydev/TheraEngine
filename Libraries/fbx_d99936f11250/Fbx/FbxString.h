#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <vcclr.h>

using namespace std;
using namespace System;

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxStringInfo;
		ref class FbxMemoryAllocator;
		/** Utility class to manipulate strings.
		* \nosubgrouping
		*/
		public ref class FbxStringManaged : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxStringManaged,FbxString);
			INATIVEPOINTER_DECLARE(FbxStringManaged,FbxString);

		internal :
			FbxStringManaged(FbxString s)
			{
				this->_FbxString = new FbxString();
				*this->_FbxString = s;
				_Free = true;
			}
		public :
			///<summary>
			/// Default is 100
			/// If you know length of your string is bigger than 100
			/// change this value to internal convert between System.String to 
			/// FbxString work correctly
			///</summary>
			//static int NumCharToCreateString = 100;			

			///<summary>
			/// pass pointer to wchar_t of size NumCharToCreateString
			///</summary>
			//static void CharToWchar_t(char* orig , wchar_t* wcstring);

			//static System::String^ CharToString(char* orig);

			///<summary>
			/// pass pointer to char of size NumCharToCreateString
			///</summary>
			//static void Wchar_tToChar(wchar_t* orig , char* nstring);

			//static System::String^ Wchar_tToString(wchar_t* orig);

			///<summary>
			/// pass pointer to char of size NumCharToCreateString
			///</summary>
			//static void StringToChar(System::String^ orig , char* nstring);

			///<summary>
			/// pass pointer to wchar_t of size NumCharToCreateString
			///</summary>
			//static void StringToWchar_t(System::String^ orig,wchar_t* wcstring);

		public:
			/**
			* \name Constructors and Destructor
			*/
			//@{
			//! Create an instance.
			static FbxStringManaged^ Create();
			//! Create an instance if not already allocated ( is null )
			static FbxStringManaged^ Create(FbxStringManaged^ str);
			//! Destroy an allocated version of the string
			void Destroy();
			//! Destroy the allocated space if empty
			static FbxStringManaged^ DestroyIfEmpty(FbxStringManaged^ str);
			//! Destroy the allocated space if empty
			static FbxStringManaged^ StringOrEmpty(FbxStringManaged^ str);
		public:
			/**
			* \name Constructors and Destructor
			*/
			//@{

			///<summary>
			//! Default constructor.
			///</summary>
			DEFAULT_CONSTRUCTOR(FbxStringManaged,FbxString);

			//! Copy constructor.
			FbxStringManaged(FbxStringManaged^ str);


			//! String constructor.
			FbxStringManaged(char* pStr);

			///<summary>
			//! String constructor.
			///</summary>
			FbxStringManaged(System::String^ str);


			//! Character constructor.
			FbxStringManaged(char c, size_t pNbRepeat);
			///<summary>			
			///Character constructor.
			///</summary>			
			FbxStringManaged(char c);

			//! String constructor with maximum length.
			FbxStringManaged(char* pCharPtr, size_t pLength);

			//! Int constructor.
			FbxStringManaged(int value);

			//! Float constructor.
			FbxStringManaged(float value);

			//! Double constructor.
			FbxStringManaged(double value);			


			//! Destructor.
			//~FbxString();
			//@}

			/**
			* \name Instance Validation.
			*/
			//@{
			//! Return \c true if string is valid.
			property bool IsOK
			{
				bool get();
			}

			//! Invalidate string.
			FbxStringManaged^ Invalidate();

			//! Get string length like "C" strlen().
			property size_t Length
			{
				size_t get();
			}

			//! Return \c true if string is of length 0.
			property bool IsEmpty
			{
				bool get();
			}

			//! Discard the content of the object.
			FbxStringManaged^ Empty();
			//@}

			/**
			* \name Buffer Access
			*/
			//@{

			//! Access by reference.
			//char& operator[](int pIndex);

			//! Access by copy.
			char GetChar(int index);

			property char default[int]
			{
				char get(int index);
			}

			//! Cast operator.
			//inline operator const char*() const;

			//! Non-const buffer access.
			//inline char* Buffer();

			//! const buffer access.
			//inline const char* Buffer()const;

			//@}

			/**
			* \name Assignment Operators
			*/
			//@{

			//! FbxString assignment operator.
			void CopyFrom(FbxStringManaged^ str);
			//! Character assignment operator.			
			void CopyFrom(char c);

			//! String assignment operator.
			void CopyFrom(char* c);

			//! Int assignment operator.			
			void CopyFrom(int value);

			//! Float assignment operator.			
			void CopyFrom(float value);

			//! Double assignment operator.			
			void CopyFrom(double value);

			//! String assignment function.
			/*FbxString^ Copy(size_t length, char* pStr)
			{
			FbxString^ s = gcnew 
			}*/


			// Swap the contents of two FbxString objects; no allocation is performed.
			void Swap(FbxStringManaged^ str);
			//@}

			/**
			* \name Append and Concatenation
			*/
			//@{


			//! Append as "C" strcat().
			void Append(char* pStr);
			void Append(String^ str);


			//! Append as "C" strncat().
			//const FbxString& AppendN(const char* pStr, size_t pLength);

			//! FbxString append.
			//const FbxString& operator+=(const FbxString& pKStr);

			FbxStringManaged^ FbxStringManaged::operator+(FbxStringManaged^ other)
			{
				(*_FbxString) += *other->_Ref();
				return this;
			}
			//! Character append.
			FbxStringManaged^ FbxStringManaged::operator+(char c)
			{
				(*_FbxString) += c;				
				return this;
			}			
			//! String append.
			FbxStringManaged^ FbxStringManaged::operator+(const char* c)
			{
				(*_FbxString) += c;
				return this;
			}
			//! Int append.
			FbxStringManaged^ FbxStringManaged::operator+(int value)
			{

				(*_FbxString) += value;
				return this;
			}
			//! Float append.
			FbxStringManaged^ FbxStringManaged::operator+(float value)
			{
				(*_FbxString) += value;
				return this;
			}
			//! FbxString concatenation.
			static FbxStringManaged^ operator +(FbxStringManaged^ str1, FbxStringManaged^ str2)
			{
				FbxStringManaged^ s = gcnew FbxStringManaged(((*str1->_Ref()) + *str2->_Ref()));				
				return s;
			}
			//! Character concatenation.
			static FbxStringManaged^ operator +(FbxStringManaged^ str1, char c)
			{
				FbxStringManaged^ s = gcnew FbxStringManaged(((*str1->_Ref()) + c));				
				return s;
			}
			//! String concatenation.
			static FbxStringManaged^ operator +(FbxStringManaged^ str1, const char* c)
			{
				FbxStringManaged^ s = gcnew FbxStringManaged(((*str1->_Ref()) + c));				
				return s;
			}
			//! Int concatenation.
			static FbxStringManaged^ operator +(FbxStringManaged^ str1, int value)
			{
				FbxStringManaged^ s = gcnew FbxStringManaged(((*str1->_Ref()) + value));				
				return s;
			}
			//! Float concatenation.
			static FbxStringManaged^ operator +(FbxStringManaged^ str1,  float value)
			{
				FbxStringManaged^ s = gcnew FbxStringManaged(((*str1->_Ref()) + value));				
				return s;
			}			

			virtual String^ ToString() override;

			/**
			* \name String Comparison
			*/
			//@{

			//! Compare as "C" strcmp().
			int Compare(const char* pStr);
			int Compare(String^ s);

			//! Compare as "C" stricmp().
			int CompareNoCase( const char * pStr );
			int CompareNoCase(String^ s);

			//! Equality operator.
			virtual bool Equals(Object^ obj) override
			{
				FbxStringManaged^ o = dynamic_cast<FbxStringManaged^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}
			bool EqualsWith(String^ str)
			{				
				STRINGTO_CONSTCHAR_ANSI(ps,str);
				bool b = *_Ref() == ps;
				FREECHARPOINTER(ps);
				return b;
			}


			//! Inequality operator.			


			//! Inferior to operator.
			static bool operator <(FbxStringManaged^ str1, FbxStringManaged^ str2)
			{
				return *str1->_Ref() < *str2->_Ref();
			}
			static bool operator <(FbxStringManaged^ str1, String^ str2)
			{
				STRINGTO_CONSTCHAR_ANSI(ps,str2);				
				bool b = *str1->_Ref() < ps;
				FREECHARPOINTER(ps);
				return b;
			}


			//! Inferior or equal to operator.
			static bool operator <=(FbxStringManaged^ str1, FbxStringManaged^ str2)
			{
				return *str1->_Ref() <= *str2->_Ref();
			}
			static bool operator <=(FbxStringManaged^ str1, String^ str2)
			{				
				STRINGTO_CONSTCHAR_ANSI(ps,str2);
				bool b = *str1->_Ref() <= ps;				
				FREECHARPOINTER(ps);
				return b;
			}

			//! Superior or equal to operator.
			static bool operator >=(FbxStringManaged^ str1, FbxStringManaged^ str2)
			{
				return *str1->_Ref() >= *str2->_Ref();
			}
			static bool operator >=(FbxStringManaged^ str1, String^ str2)
			{				
				STRINGTO_CONSTCHAR_ANSI(ps,str2);
				bool b = *str1->_Ref() >= ps;	
				FREECHARPOINTER(ps);
				return b;
			}

			//! Superior to operator.
			static bool operator >(FbxStringManaged^ str1, FbxStringManaged^ str2)
			{
				return *str1->_Ref() > *str2->_Ref();
			}
			static bool operator >(FbxStringManaged^ str1, String^ str2)
			{				
				STRINGTO_CONSTCHAR_ANSI(ps,str2);
				bool b = *str1->_Ref() > ps;	
				FREECHARPOINTER(ps);
				return b;
			}

			//! Equality operator.			
			bool EqualsWith(const char *pStr)
			{
				return *_Ref() == pStr;
			}			

			//! Inferior to operator.
			static bool operator <(FbxStringManaged^ str1, const char *pStr)
			{
				return *str1->_Ref()< pStr;
			}

			//! Inferior or equal to operator.
			static bool operator <=(FbxStringManaged^ str1, const char *pStr)
			{
				return *str1->_Ref() <= pStr;
			}

			//! Superior or equal to operator.
			static bool operator >=(FbxStringManaged^ str1, const char *pStr)
			{
				return *str1->_Ref() >= pStr;
			}

			//! Superior to operator.
			static bool operator >(FbxStringManaged^ str1, const char *pStr)
			{
				return *str1->_Ref() > pStr;
			}
			//@}

			/**
			* \name Substring Extraction
			*/
			//@{


			//! Extract middle string for a given length.
			FbxStringManaged^ Mid(size_t first, size_t count);

			//! Extract middle string up to the end.
			FbxStringManaged^ Mid(size_t first);

			//! Extract left string.
			FbxStringManaged^ Left(size_t count);

			//! Extract right string.			
			FbxStringManaged^ Right(size_t count);

			//@}

			/**
			* \name Padding
			*/
			//@{

			/** \enum PaddingType      Padding types.
			* - \e eRight
			* - \e eLeft
			* - \e eBoth
			*/
			enum class PaddingType 
			{ 
				Right,
				Left, 
				Both
			};


			//! Add padding characters.
			FbxStringManaged^ Pad(PaddingType padding, size_t length, char car);

			//! Remove padding characters.
			FbxStringManaged^ UnPad(PaddingType padding);

			//@}

			/**
			* \name Conversion
			*/

			//@{
			//! Uppercase conversion
			FbxStringManaged^ Upper();

			//! Lowercase conversion
			FbxStringManaged^ Lower();

			//! Reverse conversion
			FbxStringManaged^ Reverse();

			//! Convert to Unix, changes \\r\\n characters for a single \\n
			FbxStringManaged^ ConvertToUnix();

			//! Convert to Windows, changes \\n character for both \\r\\n
			FbxStringManaged^ ConvertToWindows();
			//@}

			/**
			* \name Search
			*/

			/* Look for a single character match, like "C" strchr().
			* \return Index or -1 if not found.
			*/			
			int Find(char c, size_t startPosition);

			/* Look for a single character match, like "C" strchr().
			* \return Index or -1 if not found.
			*/			
			int Find(char c);


			/** Look for a substring match, like "C" strstr().
			* \return Starting index or -1 if not found.
			*/
			int Find(const char* strSub, size_t startPosition);
			int Find(String^ s, size_t startPosition);
			int Find(const char* strSub);
			int Find(String^ s);

			/** Look for a single character match, like "C" strrchr().
			* \return Index or -1 if not found.
			*/
			int ReverseFind(char c);

			/** Look for a single character match, like "C" strpbrk().
			* \return Index or -1 if not found.
			*/
			int FindOneOf(const char * strCharSet, size_t startPosition);
			int FindOneOf(String^ str, size_t startPosition);

			int FindOneOf(const char * strCharSet);
			int FindOneOf(String^ str);

			/** Replace a substring.
			* \return \c true if substring found and replaced.
			*/
			bool FindAndReplace(const char* find, const char* replaceBy, size_t startPosition);
			bool FindAndReplace(String^ find, String^ replaceBy, size_t startPosition);

			bool FindAndReplace(const char* find, const char* replaceBy);
			bool FindAndReplace(String^ find, String^ replaceBy);

			/** Replace a character.
			* \return \c true if character found and replaced.
			*/
			bool ReplaceAll( char find, char replaceBy );

			//@}

			/**
			* \name Token Extraction
			*/
			//@{
			//! Get number of tokens.
			int GetTokenCount(const char* spans);

			int GetTokenCount(String^ s);

			//! Get token at given index.
			FbxStringManaged^ GetToken(int tokenIndex, const char* spans);
			FbxStringManaged^ GetToken(int tokenIndex, String^ spans);
			//@}

			/**
			* \name Memory Pool Management
			*/
			//@{
			//public:
			/*static FbxMemoryAllocator^ AllocatorGet()
			{
			FbxMemoryAllocator^ a = gcnew FbxMemoryAllocator(FbxString::AllocatorGet());
			a->isNew = true;
			return a;
			}*/
			static void AllocatorPurge();
			static void AllocatorRelease();


		};
		/** Functor class suitable for use in KMap.
		/*
		public ref class FbxStringCompare
		{
		public:
		inline int operator()(FbxString const &pKeyA, FbxString const &pKeyB) const
		{
		return (pKeyA < pKeyB) ? -1 : ((pKeyB < pKeyA) ? 1 : 0);
		}
		};

		class KCharCompare
		{
		public:
		inline int operator()(char const* pKeyA, char const* pKeyB) const
		{
		return strcmp( pKeyA, pKeyB );
		}
		};
		*/


	}
}