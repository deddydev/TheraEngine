#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		/** \brief Generic base class for streamable data.
		* Currently it is only used for the Unload/Load of
		* objects content when using the Peripheral objects.
		*/
		public ref class FbxStream : IFbxNativePointer
		{

			BASIC_CLASS_DECLARE(FbxStream,KFbxStream);
			INATIVEPOINTER_DECLARE(FbxStream,KFbxStream);

	/*	internal:
			bool isNew;
			KFbxStream* stream;
			FbxStream(KFbxStream* s);*/
		//public:
		//	typedef enum {
		//		eCLOSED = 0,
		//		eOPENED = 1,
		//		eEMPTY  = 2
		//	} eStreamState;

		//	KFbxStream() {};
		//	virtual ~KFbxStream() {};

		//	/** Query the current state of the stream.
		//	*/
		//	virtual eStreamState State() { return eCLOSED; }

		//	/** Open the stream.
		//	* \return True if successful.
		//	*/
		//	virtual bool Open()  { return false; };

		//	/** Close the stream.
		//	* \return True if successful.
		//	*/
		//	virtual bool Close() { return false; };

		//	/** Empties the internal data of the stream.
		//	* \return True if successful.
		//	*/
		//	virtual bool Flush() { return false; };

		//	/** Writes a memory block.
		//	* \param pData Pointer to the memory block to write.
		//	* \param pSize Size (in bytes) of the memory block to write.
		//	* \return The number of bytes written in the stream.
		//	*/
		//	virtual int Write(const void* pData, int pSize) { return 0; }

		//	int Write(const char* pData, int pSize) { return Write((void*)pData, pSize); }
		//	int Write(const int* pData, int pSize) { return Write((void*)pData, pSize); }

		//	/** Read bytes from the stream and store them in the memory block.
		//	* \param pData Pointer to the memory block where the read bytes are stored.
		//	* \param pSize Number of bytes read from the stream.
		//	* \return The actual number of bytes successfully read from the stream.
		//	*/
		//	virtual int Read (void* pData, int pSize) const { return 0; }

		//	int Read(char* pData, int pSize) const { return Read((void*)pData, pSize); }
		//	int Read(int* pData, int pSize) const { return Read((void*)pData, pSize); }

		};

	}
}