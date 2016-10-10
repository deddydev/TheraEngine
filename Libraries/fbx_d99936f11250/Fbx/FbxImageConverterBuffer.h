#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"



{
	namespace FbxSDK
	{		
		ref class FbxObjectManaged;
		ref class FbxStringManaged;
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		/** image converter buffer
		*\nosubgrouping
		*/
		public ref class FbxImageConverterBuffer : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxImageConverterBuffer,ImageConverterBuffer);			
			GET_NATIVEPOINTER_DECLARE_BASE(FbxImageConverterBuffer,ImageConverterBuffer);
			IS_SAME_AS_DECLARE_BASE();
		public:
			/**
			* \name Constructor and Destructor. 
			*/
			//@{

			//!Constructor.
			DEFAULT_CONSTRUCTOR(FbxImageConverterBuffer,ImageConverterBuffer);
			//@}

			/** Check if this object is correctly initialized.
			* \return     \c true if the object has been initialized with acceptable values.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(bool,IsValid);

			/** Tells where the data to use is located.
			* \return     \c true if the data is in the allocated memory buffer. \c false if the data is from an external file on disk.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,UseDataBuffer);

			/** Get the width of the image.
			* \return     The number of horizontal pixels.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Width);

			/** Get the height of the image.
			* \return     The number of vertical pixels.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Height);

			/** Get the color space of the image.
			* \return     Either ColorSpaceRGB or ColorSpaceYUV.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ColorSpace);

			/** Get the number of bytes per pixel.
			* \return     Either 3 for RGB images, or 4 for RGBA images.
			*/
			VALUE_PROPERTY_GET_DECLARE(char,PixelSize);

			/** Get access to the image data.
			* \return     Pointer to the array of pixels.
			*/
			VALUE_PROPERTY_GET_DECLARE(IntPtr,Data);

			/** Tells if the image has not been converted from its original format.
			* \return     \c true if the image is stored in its original format, \c false if the image has been converted.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,OriginalFormat);

			/** Get the original filename of the image file before conversion.
			* \return      The original filename.
			* \remarks     When a conversion to another format occurs, the converted image is given a different filename. 
			*              The original filename can be stored in the FBX file so that the original file can be extracted
			*              from the converted image (also stored in the FBX file). 
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,OriginalFileName);

			//	/** Initialize the object.
			//	* \param pWidth             Image width.
			//	* \param pHeight            Image height.
			//	* \param pUseDataBuffer     Set to \c true if the image buffer needs to be allocated.
			//	* \param pColorSpace        The image color space.
			//	* \param pPixelSize         The number of bytes per pixel.
			//	* \remarks                  The total number of bytes allocated (if the pUseDataBuffer is \c true) is:
			//	*                           total = pWidth * pHeight * pPixelSize
			//	*/
			void Initialize(int width, int height, bool useDataBuffer, int colorSpace, char pixelSize);

			/** Set the original format flag.
			* \param pState     The value of the original format flag.
			*/
			void SetOriginalFormat(bool state);

			/** Set the original filename string.
			* \param pFilename     The filename to use.
			*/
			void SetOriginalFileName(String^ filename);
		};		


		// This function should return 0 if successful and any non zero value otherwise. And should
		// init the pBuffer with the correct values.
		public delegate int FbxImageConverterFunction(int direction,String^ fileName,FbxImageConverterBuffer^ buffer);


		/*public ref class FbxImageConverterFunctionCaller
		{
		public:
			FbxImageConverterFunction^ Function;		
			FbxImageConverterFunctionCaller()
			{
				ImgConverterFunction = &FbxImageConverterFunctionCaller::NativeFunction;
			}
		internal:
			int NativeFunction(int pDirection, FbxString& pFileName, ImageConverterBuffer& pBuffer)
			{
				int result = -1;
				if(Function)
				{
					CONVERT_FbxString_TO_STRING(pFileName,fileName);
					FbxImageConverterBuffer^ buffer = gcnew FbxImageConverterBuffer(&pBuffer);
					result = Function(pDirection,fileName,buffer);
				}
				return result;
			}			
			ImageConverterFunction ImgConverterFunction;
		};*/

		/*This function should return 0 if successful and any non zero value otherwise. And should
		init the pBuffer with the correct values.
		public delegate int ImageConverterFunc(int direction, FbxString^ fileName, FbxImageConverterBuffer^ buffer);

		! Provides a placeholder of functions to convert from a file format to "raw" data and vice et versa.*/
		public ref class FbxImageConverter : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxImageConverter); 
		internal:
			FbxImageConverter(KFbxImageConverter* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxImageConverter);			

		public:

			/** Register a user converter function into the system.
			* \param pFileExt     The image file extension the registered function can handle.
			* \param pFct         The function that can convert the image file.
			* \remarks            If the function can handle multiple image files, each file extension
			*                     has to be registered individually (the same function can be used more than once
			*                     in the RegisterConverterFunction).
			*/
			/*void RegisterConverterFunction(String^ fileExt, FbxImageConverterFunctionCaller^ fctc)
			{
				STRINGTO_CONSTCHAR_ANSI(f,fileExt);
				FbxString fe(f);
				FREECHARPOINTER(f);
				_Ref()->RegisterConverterFunction(fe,fctc->ImgConverterFunction);
			}*/

			/** Removes a user converter function from the system.
			* \param pFct     The function to be removed from the list of converters.
			*/
			///void UnregisterConverterFunction(ImageConverterFunction pFct);

			/** Perform the actual conversion.
			* \param pDirection     Either FileToBuffer (0) or BufferToFile (1).
			* \param pFileName      The destination filename (can be changed by the ImageConverterFunction).
			* \param pBuffer        The data placeholder.
			* \return               \c true if the conversion is successful, \c false otherwise.
			*/
			bool Convert(int direction, String^ fileName, FbxImageConverterBuffer^ buffer);							
		};
	}
}