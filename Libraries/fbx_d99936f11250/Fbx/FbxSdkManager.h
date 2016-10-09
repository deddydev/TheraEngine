#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxType.h"
#include "FbxNativePointer.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{
			ref class FbxIOSettingsManaged;
		}
	}
}

using namespace Skill::FbxSDK::IO;

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxStringManaged;
		ref class FbxXRefManager;
		ref class FbxDataType;
		ref class FbxErrorManaged;
		ref class FbxMemoryAllocator;
		ref class FbxIOPluginRegistry;		
		ref class FbxDataType;
		ref class FbxLibrary;
		ref class FbxSceneManaged;
		ref class FbxObjectManaged;
		ref class FbxPlug;
		/** 
		* SDK object manager.
		*   The SDK manager is in charge of:
		*     \li scene element allocation
		*     \li scene element deallocation
		*     \li scene element search and access.
		*
		* Upon destruction, all objects allocated by the SDK manager and
		* not explicitly destroyed are destroyed as well. A derived
		* class can be defined to allocate and deallocate a set of
		* specialized scene elements.
		* \nosubgrouping
		*/
		public ref class FbxSdkManagerManaged :IFbxNativePointer
		{			
		internal:
			INTERNAL_CLASS_DECLARE(FbxSdkManagerManaged,FbxSdkManager);
			REF_DECLARE(FbxSdkManagerManaged,FbxSdkManager);
		public:
			~FbxSdkManagerManaged(){this->CollectManagedMemory();this->!FbxSdkManager();}
			!FbxSdkManagerManaged()
			{
				if(_Free && _FbxSdkManager  && !_disposed)
					_FbxSdkManager->Destroy();
				_Free = false;_FbxSdkManager = nullptr;_disposed = true;
			}
			INATIVEPOINTER_DECLARE(FbxSdkManagerManaged,KFbxSdkManager);			

			/**
			* \name Memory Management
			*/
			//@{			

			/** SDK Memory management.
			* \nosubgrouping
			* Use this method to specify custom memory management routines.
			* The FBX SDK will use the provided routines to allocate and
			* deallocate memory.
			* \remarks Important: If you plan to specify custom memory management
			* routines, you must do so BEFORE creating the first SDK manager. Those
			* routines are global and thus shared between different instances of SDK managers.
			*/
			static bool SetMemoryAllocator(FbxMemoryAllocator^ memoryAllocator);
			//@}


			/** Access to the unique UserNotification object.
			* \return The pointer to the user notification or \c NULL \c if the object
			* has not been allocated.
			*/
			//FbxUserNotification^ GetUserNotification();

			//void SetUserNotification(FbxUserNotification^ %value);
			/**
			* \name FBX SDK Manager Creation/Destruction
			*/
			//@{
			/** SDK manager allocation method.
			* \return A pointer to the SDK manager or \c NULL if this is an
			* evaluation copy of the FBX SDK and it is expired.
			*/
			static FbxSdkManagerManaged^ Create();

			/** Destructor.
			* Deallocates all object previously created by the SDK manager.
			*/
			virtual void Destroy();			

			//@}


			//      /**
			//        * \name Plug and Object Definition and Management
			//        */
			//      //@{
			//      public:
			//          /** Class registration.
			//            * \param pFBX_TYPE_Class
			//            * \param pFBX_TYPE_ParentClass
			//            * \param pName
			//            * \param pFbxFileTypeName
			//            * \param pFbxFileSubTypeName
			//            * \return The class Id of the newly register class
			//            *
			//            */
			//          template <typename T1,typename T2> inline kFbxClassId RegisterFbxClass(char const *pName,T1 const *pFBX_TYPE_Class,T2 const *pFBX_TYPE_ParentClass,const char *pFbxFileTypeName=0,const char *pFbxFileSubTypeName=0) {
			//              T1::ClassId  = Internal_RegisterFbxClass(pName,T2::ClassId,(kFbxPlugConstructor)T1::SdkManagerCreate,pFbxFileTypeName,pFbxFileSubTypeName );
			//              return T1::ClassId;
			//          }

			//          template <typename T> inline kFbxClassId    RegisterRuntimeFbxClass(char const *pName,T const *pFBX_TYPE_ParentClass,const char *pFbxFileTypeName=0,const char *pFbxFileSubTypeName=0) {
			//              return Internal_RegisterFbxClass(pName,T::ClassId,(kFbxPlugConstructor)T::SdkManagerCreate,pFbxFileTypeName,pFbxFileSubTypeName );
			//          }

			//          inline void UnregisterRuntimeFbxClass(char const* pName)
			//          {
			//              kFbxClassId lClassId = FindClass(pName);

			//              if( !(lClassId == kFbxClassId()) )
			//              {
			//                  Internal_UnregisterFbxClass(lClassId);
			//              }
			//          }

			//          template <typename T1,typename T2> inline kFbxClassId OverrideFbxClass(T1 const *pFBX_TYPE_Class,T2 const *pFBX_TYPE_OverridenClass) {
			//              T1::ClassId  = Internal_OverrideFbxClass(T2::ClassId,(kFbxPlugConstructor)T1::SdkManagerCreate );
			//              return T1::ClassId;
			//          }

			//          KFbxPlug*       CreateClass(kFbxClassId pClassId, char const *pName, const char* pFBXType=0, const char* pFBXSubType=0);
			//          KFbxPlug*       CreateClass(KFbxObject* pContainer, kFbxClassId pClassId, const char* pName, const char* pFBXType=0, const char* pFBXSubType=0);
			//          kFbxClassId     FindClass(const char* pClassName);
			//          kFbxClassId     FindFbxFileClass(const char* pFbxFileTypeName, const char* pFbxFileSubTypeName);

			//          template <typename T> inline void UnregisterFbxClass( T const* pFBX_TYPE_Class )
			//          {
			//              Internal_UnregisterFbxClass( T::ClassId );
			//              T::ClassId = kFbxClassId();
			//          }

			//      //@}

			/**
			* \name Error Management
			*/
			//@{					
		public:
			/** Retrieve error object.
			*  \return Reference to the Manager's error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error codes
			*/
			enum class Error
			{
				ObjectNotFound,    /**< The requested object was not found in the Manager's database. */
				NameAlreadyInUse, /**< A name clash occurred.                                        */
				ErrorCount          /**< Mark the end of the error enum.                               */
			};

			/** Get last error code.
			*  \return Last error code.
			*/
			VALUE_PROPERTY_GET_DECLARE(Error,LastErrorID);			

			/** Get last error string.
			*  \return Textual description of the last error.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,LastErrorString);			

			//@}

			/**
			* \name Data Type Management
			*/
			//@{
			/** Register a new data type to the manager
			*  \return true if it success
			*/
			FbxDataType^ CreateFbxDataType(String^ name,FbxType type);

			/** Find a data type
			*  \return the found datatype. return null if not found
			*/
			//KFbxDataType const &GetFbxDataTypeFromName(const char *pDataType);

			/** List the data types
			*  \return the number of registered datatypes
			*/
			VALUE_PROPERTY_GET_DECLARE(int,FbxDataTypeCount);			

			/** Find a data types
			*  \return the found datatype. return null if not found
			*/
			FbxDataType^ GetFbxDataType(int index);

			/** Retrieve the main object Libraries
			* \return The Root library
			*/
			REF_PROPERTY_GET_DECLARE(FbxLibrary,RootLibrary);
			REF_PROPERTY_GET_DECLARE(FbxLibrary,SystemLibraries);
			REF_PROPERTY_GET_DECLARE(FbxLibrary,UserLibraries);
			//@}			

			/**
			* \name Message Emitter (for Message Logging)
			*/
			//@{
			/** Access to the unique KFbxMessageEmitter object.
			* \return The pointer to the message emitter.
			*/
			//KFbxMessageEmitter & GetMessageEmitter();
			/** Sets to the unique KFbxMessageEmitter object.
			* \param pMessageEmitter the emitter to use, passing NULL will reset to the default emitter.
			* The object will be deleted when the SDK manager is destroyed, thus ownership is transfered.
			*/
			//bool SetMessageEmitter(KFbxMessageEmitter * pMessageEmitter);
			//@}

		public:
			/**
			* \name Localization Hierarchy
			*/
			//@{
			/** Add a localization object to the known localization providers.
			* \param pLocManager the localization object to register.
			*/
			//void AddLocalization( KFbxLocalizationManager * pLocManager );
			/** Remove a localization object from the known localization providers.
			* \param pLocManager the localization object to remove.
			*/
			//void RemoveLocalization( KFbxLocalizationManager * pLocManager );
			/** Select the current locale for localization.
			* \param pLocale the locale name, for example "fr" or "en-US".
			*/
			bool SetLocale(String^ locale);
			/** Localization helper function. Calls each registered localization manager
			* until one can localizes the text.
			* \param pID the identifier for the text to localize.
			* \param pDefault the default text. Uses pID if NULL.
			* \return the potentially localized text. May return the parameter passed in.
			*/
			String^ Localize(String^ ID, String^ Default);
			String^ Localize(String^ ID);
				//@}

				/**
				* \name Sub-Manager Management
				*/
				//@{

				/** Retrieve the manager responsible for managing object previews.
				* \return The Preview manager for this SDK manager.
				*/
				//FbxPreviewManager& GetPreviewManager();

				//@}

				/**
				* \name XRef Manager
				*/
				//@{
				/** Retrieve the manager responsible for managing object XRef resolution.
				* \return The XRef manager for this SDK manager.
				*/
			REF_PROPERTY_GET_DECLARE(FbxXRefManager,XRefManager);				
			//@}

			/**
			* \name Library Management
			*/
			//@{
			
				//@}

				/**
				* \name Plug-in Registry Object
				*/
				//@{
				/** Access to the unique KFbxIOPluginRegistry object.
				* \return The pointer to the user KFbxIOPluginRegistry
				*/
			REF_PROPERTY_GET_DECLARE(FbxIOPluginRegistry,IOPluginRegistry);				

			//@}

			/**
			* \name Fbx Generic Plugins Management
			*/
			//@{
			/** Load plug-ins directory
			*/
			bool LoadPluginsDirectory (String^ filename,String^ extensions);
			/** Load plug-in
			*/
			bool LoadPlugin (String^ filename);
			/** Unload all plug-ins
			*/
			bool UnloadPlugins();

			void RegisterObject(FbxPlug^ plug);
			void UnregisterObject(FbxPlug^ plug);			  
			//bool FbxEmitPluginsEvent(KFbxEventBase const &pEvent);
			//KArrayTemplate<KFbxPlugin const*> GetPlugins() const;
			//@}


			/**
			* \name IO Settings
			*/
			//@{
			/** Add IOSettings in hierarchy from different modules
			*/
			void FillIOSettingsForReadersRegistered(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void FillIOSettingsForWritersRegistered(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void FillCommonIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios, bool import);
			void Create_Common_Import_IOSettings_Groups(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Create_Common_Export_IOSettings_Groups(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Add_Common_Import_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Add_Common_Export_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Add_Common_RW_Import_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Add_Common_RW_Export_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Fill_Motion_Base_ReaderIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			void Fill_Motion_Base_WriterIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios);
			//@}

			// Temporary for Managing global objects
			/**
			* \name Global Object Management
			*/
			//@{        

		public:
			/** Register object
			*/

			//      public:
			//          inline int  GetSrcObjectCount(KFbxImageConverter const *)                                   { return mKFbxImageConverterArray.GetCount();  }
			//          inline KFbxImageConverter*      GetSrcObject(KFbxImageConverter const *,int pIndex=0)       { return mKFbxImageConverterArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxNode *>  mKFbxNodeArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxNode const *)                         { return mKFbxNodeArray.GetCount();  }
			//          inline KFbxNode*        GetSrcObject(KFbxNode const *,int pIndex=0)     { return mKFbxNodeArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxTexture *>   mKFbxTextureArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxTexture const *)                          { return mKFbxTextureArray.GetCount();  }
			//          inline KFbxTexture*     GetSrcObject(KFbxTexture const *,int pIndex=0)      { return mKFbxTextureArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxCluster *>   mKFbxClusterArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxCluster const *)                          { return mKFbxClusterArray.GetCount();  }
			//          inline KFbxCluster*     GetSrcObject(KFbxCluster const *,int pIndex=0)      { return mKFbxClusterArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxScene *> mKFbxSceneArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxScene const *)                            { return mKFbxSceneArray.GetCount();  }
			//          inline KFbxScene*       GetSrcObject(KFbxScene const *,int pIndex=0)        { return mKFbxSceneArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxDocument *>  mKFbxDocumentArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxDocument const *)                         { return mKFbxDocumentArray.GetCount();  }
			//          inline KFbxDocument*        GetSrcObject(KFbxDocument const *,int pIndex=0)     { return mKFbxDocumentArray.GetAt(pIndex); }
			//      private:
			//          KArrayTemplate<KFbxSurfaceMaterial *>   mKFbxSurfaceMaterialArray;
			//      public:
			//          inline int  GetSrcObjectCount(KFbxSurfaceMaterial const *)                          { return mKFbxSurfaceMaterialArray.GetCount();  }
			//          inline KFbxSurfaceMaterial*     GetSrcObject(KFbxSurfaceMaterial const *,int pIndex=0)      { return mKFbxSurfaceMaterialArray.GetAt(pIndex); }

			      public:
			          /** Add a prefix to a name.
			            * \param pPrefix The prefix to be added to the \c pName. This
			            * string must contain the "::" characters in order to be considered
			            * as a prefix.
			            * \param pName The name to be prefix.
			            * \return The prefixed string
			            * \remarks If a prefix already exists, it is removed before
			            * adding \c pPrefix.
			            */
					 static String^ PrefixName(String^ prefix,String^ name);


			  ///////////////////////////////////////////////////////////////////////////////
			  //
			  //  WARNING!
			  //
			  //  Anything beyond these lines may not be documented accurately and is
			  //  subject to change without notice.
			  //
			  ///////////////////////////////////////////////////////////////////////////////

			  #ifndef DOXYGEN_SHOULD_SKIP_THIS

			  public:
				  bool CanDestroyFbxSrcObject(FbxObjectManaged^ obj, FbxObjectManaged^ srcObj, bool recursive, bool dependents);

				  void CreateMissingBindPoses(FbxSceneManaged^ scene);
				  int  GetBindPoseCount(FbxSceneManaged^ scene);

				  //FbxPlug^ CreateClassFrom(FbxClassId^ , const char *pName, const KFbxObject* pFrom, const char* pFBXType=0, const char* pFBXSubType=0);  

			  #endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}