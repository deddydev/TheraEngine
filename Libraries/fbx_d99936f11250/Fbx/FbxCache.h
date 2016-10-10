#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"

using  namespace System::Runtime::InteropServices;



{
	namespace FbxSDK
	{
		ref class FbxStringManaged;
		ref class FbxTime;
		ref class FbxErrorManaged;			
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/**
		* \brief This object contains methods for accessing point animation in a cache file.
		* 
		* The FBX SDK supports two point cache file formats :
		*    - \e ePC2: the 3ds Max Point Cache 2 file format.
		*    - \e eMC: the Maya Cache file format.
		*
		* Accessing cache data using these formats differ significantly. To address this difference, two sets of methods have been created.
		* Use the GetCacheFileFormat() function to determine which set of methods to use.
		*/

		public ref class FbxCacheManaged : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,FbxCache);
		internal:
			FbxCacheManaged(FbxCache* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory() override;
			FBXOBJECT_DECLARE(FbxCacheManaged);
		public:
			enum class FileFormat
			{
				Unknown,
				Pc2,
				Mc
			};				
			property FileFormat CacheFileFormat
			{
				FileFormat get();
				void set(FileFormat value);
			}
			void SetCacheFileName(System::String^ relativeFileName, System::String^ absoluteFileName);
			void GetCacheFileName([OutAttribute]String^ %relativeFileName, [OutAttribute]String^ %absoluteFileName);
			bool OpenFileForRead();
			property bool IsOpen
			{
				bool get();
			}
			bool CloseFile();
			property double SamplingFrameRate
			{
				double get();
			}
			property FbxTime^ CacheTimePerFrame
			{
				FbxTime^ get();
			}
			enum class MCFileCount
			{
				OneFile,
				OneFilePerFrame
			};
			enum class MCDataType
			{
				UnknownData,
				Double,             //kDouble
				DoubleArray,        //KArrayTemplate<kDouble>
				DoubleVectorArray,  //KArrayTemplate<fbxDouble3> 
				Int32Array,         //KArrayTemplate<kInt>
				FloatVectorArray    //KArrayTemplate<fbxFloat3>
			};
			bool OpenFileForWrite(MCFileCount fileCount, double samplingFrameRate, System::String^ channelName, MCDataType CDataType);
			bool OpenFileForWrite(MCFileCount fileCount, double samplingFrameRate, System::String^ channelName);
			property int  ChannelCount
			{
				int get();
			}
			bool GetChannelName(int channelIndex, [OutAttribute]String^ %channelName);
			bool GetChannelDataType(int channelIndex, MCDataType %channelType);
			int  GetChannelIndex(System::String^ channelName);

			/** Read a sample at a given time.
			* \param pChannelIndex     The index of the animation channel, between 0 and GetChannelCount().
			* \param pTime             Time at which the point animation must be evaluated.
			* \param pBuffer           The place where the point value will be copied. This buffer must be
			*                          of size 3*pPointCount.
			* \param pPointCount       The number of points to read from the point cache file.
			* \return                  \c true if successful, \c false otherwise. See the error management
			*                          functions for error details.
			*/
			//bool Read(int channelIndex, FbxTime^ %time, double* pBuffer, unsigned int pPointCount);

			/** Read a sample at a given time.
			* \param pChannelIndex     The index of the animation channel, between 0 and GetChannelCount().
			* \param pTime             Time at which the point animation must be evaluated.
			* \param pBuffer           The place where the point value will be copied. This buffer must be
			*                          of size 3*pPointCount.
			* \param pPointCount       The number of points to read from the point cache file.
			* \return                  \c true if successful, \c false otherwise. See the error management
			*                          functions for error details.
			*/
			//bool Read(int pChannelIndex, KTime& pTime, float* pBuffer, unsigned int pPointCount);

			/** Write a sample at a given time.
			* \param pChannelIndex   The index of the animation channel, between 0 and GetChannelCount().
			* \param pTime           Time at which the point animation must be inserted.
			* \param pBuffer         Point to the values to be copied. This buffer must be
			*                        of size 3*pPointCount.
			* \param pPointCount     The number of points to write in the point cache file.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			//bool Write(int pChannelIndex, KTime& pTime, double* pBuffer, unsigned int pPointCount);

			/** Write a sample at a given time.
			* \param pChannelIndex   The index of the animation channel, between 0 and GetChannelCount().
			* \param pTime           Time at which the point animation must be inserted.
			* \param pBuffer         Point to the values to be copied. This buffer must be
			*                        of size 3*pPointCount.
			* \param pPointCount     The number of points to write in the point cache file.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			//bool Write(int pChannelIndex, KTime& pTime, float* pBuffer, unsigned int pPointCount);

			/** Get the Animation Range of the specified channel.
			* \param pChannelIndex    The index of the channel.
			* \param pTimeStart       The Channel's Animation Start Time
			* \param pTimeEnd         The Channel's Animation End Time
			* \return                 \c true if successful, \c false otherwise. See the error management
			*                         functions for error details.
			*/
			bool GetAnimationRange(int channelIndex, FbxTime^ timeStart, FbxTime^ timeEnd);

			/** Get the cache type
			* \param pFileCount       The cache type.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			bool GetCacheType(MCFileCount %fileCount);

			/** Get the cache channel interpretation.
			* \param pChannelIndex    The index of the animation channel, between 0 and GetChannelCount().
			* \param pInterpretation  The channel interpretation; user-defined.
			* \return                    \c true if successful, \c false otherwise. See the error management
			*                               functions for error details.
			*/
			bool GetChannelInterpretation(int channelIndex, [OutAttribute]String^ %interpretation);

			/** \enum EMCSamplingType cache channel sampling types.
			* - \e eSAMPLING_REGULAR      Regular sampling
			* - \e eSAMPLING_IRREGULAR    Irregular sampling
			*/
			enum class MCSamplingType
			{
				Regular,
				Irregular
			};

			/** Get the cache channel sampling type.
			* \param pChannelIndex    The index of the animation channel, between 0 and GetChannelCount().
			* \param pSamplingType    The sampling type of the channel.
			* \return                    \c true if successful, \c false otherwise. See the error management
			*                               functions for error details.
			*/

			bool GetChannelSamplingType(int channelIndex, MCSamplingType %samplingType);

			/** Get the cache channel sampling rate, in frames per second.
			* \param pChannelIndex   The index of the animation channel, between 0 and GetChannelCount().
			* \param pSamplingRate   The sampling rate of the channel.  The channel must have a regular
			*                        sampling type.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			bool GetChannelSamplingRate(int channelIndex, FbxTime^ samplingRate);

			/** Get the number of data points for a channel.
			* \param pChannelIndex   The index of the animation channel, between 0 and GetChannelCount().
			* \param pSampleCount    Number of available samples.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			bool GetChannelSampleCount(int channelIndex, unsigned int %sampleCount);

			/** Get the number of points animated in the cache file, for a channel, for a given time.
			* \param pChannelIndex   The index of the animation channel, between 0 and GetChannelCount().
			* \param pTime           Reference time; must be within the boundaries of the animation.
			* \param pPointCount     Number of available points.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			bool GetChannelPointCount(int channelIndex, FbxTime^ time, unsigned int %pointCount);

			/** Returns the number of cache data files.
			* \return     The count returned does not include the main cache file, and depends on the
			*             cache type.  WIll return -1 point cache support is not enabled.
			*/
			property int  CacheDataFileCount
			{
				int get();
			}

			/** Get the nth cache file name.
			* \param pIndex                Index of the cache file to return; index is zero-based, and must be
			*                                 < GetCacheDataFileCount().
			* \param pRelativeFileName     Return the point cache file name, relative to the FBX File name.
			* \param pAbsoluteFileName     Return the point cache file absolute path.
			* \return                      \c true if successful, \c false otherwise. See the error management
			*                         functions for error details.
			*/
			bool GetCacheDataFileName(int index, FbxStringManaged^ %relativeFileName, FbxStringManaged^ %absoluteFileName);

			/** Enable multi-channel fetching.
			* \param pMultiChannelFetching Enable/disable multi-channel fetching.  When multi-channel is enabled,
			*                              any load of data on a channel at a specific time will prefetch data
			*                              from all channels, for that specific time.  This can reduce disk
			*                              access, and increase performance (but requires more memory).
			* \return                      \c true if successful, \c false otherwise. See the error management
			*                              functions for error details.
			*/
			bool EnableMultiChannelFetching(bool multiChannelFetching);

			//@}

			/**
			* \name ePC2 Format Specific Functions.
			*/
			//@{

			/** Open a cache file for writing.
			* \param pFrameStartOffset      Start time of the animation, in frames.
			* \param pSamplingFrameRate     Number of frames per second.
			* \param pSampleCount           The number of samples to write to the file.
			* \param pPointCount            The number of points to write in the point cache file.
			* \return                       \c true if successful, \c false otherwise. See the error management
			*                               functions for error details.
			*/
			bool OpenFileForWrite(double frameStartOffset, double samplingFrameRate, unsigned int sampleCount, unsigned int pointCount);
			/** Get the number of frames of animation found in the point cache file.
			* \return     The number of frames of animation.
			*/
			property unsigned int SampleCount
			{
				unsigned int get();
			}

			/** Get the number of points animated in the cache file.
			* \return     The number of points.
			*/			
			property unsigned int PointCount
			{
				unsigned int get();
			}


			/** Get the start time of the animation
			* \return     The start time of the animation, in frames.
			*/
			property double FrameStartOffset
			{
				double get();
			}


			/** Get the sampling frame rate of the cache file.
			* \return     The sampling frame rate of the cache file, in frames per second.
			*/

			/** Read a sample at a given frame index.
			* \param pFrameIndex     The index of the animation frame, between 0 and GetSampleCount().
			* \param pBuffer         The place where the point value will be copied. This buffer must be
			*                        of size 3*pPointCount.
			* \param pPointCount     The number of points to read from the point cache file.
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			*/
			//bool Read(unsigned int pFrameIndex, double* pBuffer, unsigned int pPointCount);

			/** Write a sample at a given frame index.
			* \param pFrameIndex     The index of the animation frame.
			* \param pBuffer         Point to the values to be copied. This buffer must be
			*                        of size 3*pPointCount, as passed to the function OpenFileForWrite().
			* \return                \c true if successful, \c false otherwise. See the error management
			*                        functions for error details.
			* \remarks               Successive calls to Write() must use successive index.
			*/
			//bool Write(unsigned int pFrameIndex, double* pBuffer);

			//@}

			/**
			* \name File conversion Functions.
			*/
			//@{

			/** Create an \e eMC cache file from an ePC2 cache file.
			* \param pFileCount             Create one file for each frame of animation, or one file for all the frames.
			* \param pSamplingFrameRate     Number of frames per second used to resample the point animation.
			* \return                       \c true if successful, \c false otherwise. See the error management
			*                               functions for error details.
			* \remarks                      The created point cache file will be located in the _fpc folder associate with the FBX file.
			*/
			bool ConvertFromPC2ToMC(MCFileCount fileCount, double samplingFrameRate);

			/** Create an \e ePC2 cache file from an eMC cache file.
			* \param pSamplingFrameRate     Number of frames per second to resample the point animation.
			* \param pChannelIndex          Index of the channel of animation to read from.
			* \return                       \c true if successful, \c false otherwise. See the error management
			*                               functions for error details.
			* \remarks                      The created point cache file will be located in the _fpc folder associate with the FBX file.
			*/
			bool ConvertFromMCToPC2(double samplingFrameRate, unsigned int channelIndex);

			//@}

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eUNSUPPORTED_ARCHITECTURE
			* - \e eINVALID_ABSOLUTE_PATH
			* - \e eINVALID_SAMPLING_RATE
			* - \e eINVALID_CACHE_FORMAT
			* - \e eUNSUPPORTED_FILE_VERSION
			* - \e eCONVERSION_FROM_PC2_FAILED
			* - \e eCONVERSION_FROM_MC_FAILED
			* - \e eCACHE_FILE_NOT_FOUND
			* - \e eCACHE_FILE_NOT_OPENED
			* - \e eCACHE_FILE_NOT_CREATED
			* - \e eINVALID_OPEN_FLAG
			* - \e eERROR_WRITING_SAMPLE
			* - \e eERROR_READING_SAMPLE
			* - \e eERROR_DATATYPE
			* - \e eERROR_INVALIDCHANNELINDEX
			* - \e eERROR_IRREGULARCHANNELSAMPLING
			* - \e eERROR_CHANNELINTERPRETATION
			* - \e eERROR_CHANNELSAMPLING
			* - \e eERROR_INVALID_FILEINDEX
			* - \e eERROR_CACHEDATAFILENAME
			* - \e eERROR_COUNT
			* - \e eERROR_CHANNELSTARTTIME
			* - \e eERROR_CHANNELPOINTCOUNT
			* - \e eERROR_INVALIDTIME
			*/
			enum class Error
			{
				UnsupportedArchitecture,
				InvalidAbsolutePath,
				InvalidSamplingRate,
				InvalidCacheFormat,
				UnsupportedFileVersion,
				ConversionFromPc2Failed,
				ConversionFromMCFailed,
				CacheFileNotFound,
				CacheFileNotOpened,
				CacheFileNotCreated,
				InvalidOpenFlag,
				ErrorWritingSample,
				ErrorReadingSample,
				ErrorDatatype,
				ErrorInvalidChannelIndex,
				ErrorIrregularChannelSampling,
				ErrorChannelInterpretation,
				ErrorChannelSampling,
				ErrorInvalidFileIndex,
				ErrorCacheDataFilename,
				ErrorChannelStartTime,
				ErrorChannelPointCount,
				ErrorInvalidTime,
				ErrorCount
			};

			/** Get last error code.
			* \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			* \return     Textual description of the last error.
			*/
			property System::String^ LastErrorString
			{
				System::String^ get();
			}					

			//! Assignment operator.
			//KFbxCache& operator=( const KFbxCache& pOther );
			//
			//
			//
			//			///////////////////////////////////////////////////////////////////////////////
			//			//
			//			//  WARNING!
			//			//
			//			//  Anything beyond these lines may not be documented accurately and is
			//			//  subject to change without notice.
			//			//
			//			///////////////////////////////////////////////////////////////////////////////
#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:

			/*static property System::String^ CacheFilePropertyName
			{
			System::String^ get();
			}
			static property System::String^ CacheFileAbsolutePathPropertyName
			{
			System::String^ get();
			}
			static property System::String^ CacheFileTypePropertyName
			{
			System::String^ get();
			}*/

			// Clone
			CLONE_DECLARE();

			enum class OpenFlag
			{
				ReaddOnly,
				WriteOnly
			};
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}