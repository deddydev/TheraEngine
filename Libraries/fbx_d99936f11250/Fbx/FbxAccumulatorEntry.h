#pragma once
#include "stdafx.h"
#include "fbx.h"
#include <kfbxplugins/kfbxusernotification.h>



{
	namespace FbxSDK
	{	
		ref class FbxStringManaged;
		ref class FbxStringInfo;
		ref class FbxSdkManagerManaged;
		ref class FbxNode;

		/** \brief This class defines one entry object held by the KFbxUserNotification class.
		* \nosubgrouping
		* Direct manipulation of this object should not be required. At most, access to
		* its members can be granted for querying purposes.
		*/
		public ref class FbxAccumulatorEntry : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxAccumulatorEntry,AccumulatorEntry);
			INATIVEPOINTER_DECLARE(FbxAccumulatorEntry,AccumulatorEntry);				
		public:

			enum class AEClass {
				Error = 1,
				Warning = 2,
				Info = 4,
				Any = 7 //! cannot be used as a class ID
			};

			/** Constructor.
			*	\param pAEClass     Specify the category for this entry.
			*	\param pName        Identifies this entry (more than one object can have the same name).
			*	\param pDescr       The description of the entry. This is the common message. The details
			*	                    are added separately by the KFbxUserNotification classes.
			*	\param pDetail      A list of detail string that will be copied into the local array.
			* \param pMuteState   Whether this entry is muted.
			*	\remarks            By default the object is muted so it does not get processed by the lowlevel
			*                     output routines of the UserNotification accumulator. The entry gets activated 
			*                     (unmuted) by the calls to AddDetail() in the accumulator.		        
			*/
			FbxAccumulatorEntry(AEClass aeClass, String^ name, String^ descr, 
				String^ detail, bool muteState);
			FbxAccumulatorEntry(AEClass aeClass, String^ name, String^ descr);			


			//! Copy Constructor
			FbxAccumulatorEntry(FbxAccumulatorEntry^ ae, bool skipDetails);			
			//! Destructor.
			//~AccumulatorEntry();

			//! Returns the class of this entry.
			property AEClass Class
			{
				AEClass get();
			}

			//! Returns the name of this entry.
			VALUE_PROPERTY_GET_DECLARE(String^,Name);


			//! Returns the description of this entry.				
			VALUE_PROPERTY_GET_DECLARE(String^,Description);			

			//! Returns the number of details stored.
			property int DetailsCount
			{
				int get();
			}

			//! Returns a pointer to one specific detail string (or NULL if the id is invalid).
			String^ GetDetail(int id);

			//! Returns True if this entry is muted.
			property bool IsMuted
			{
				bool get();
			}			
		};

		/** This class iterates through the accumulated messages depending on the configuration
		* flags (filter). The iterator keeps a local copy of the data extracted from the
		* accumulator.
		*/
		//public ref class FbxUserNotificationFilteredIterator : IFbxNativePointer
		//{
		//	BASIC_CLASS_DECLARE(FbxUserNotificationFilteredIterator,KFbxUserNotificationFilteredIterator);
		//	INATIVEPOINTER_DECLARE(FbxUserNotificationFilteredIterator,KFbxUserNotificationFilteredIterator,"KFbxUserNotificationFilteredIterator");		
		//public:
		//	/** Constructor. 
		//	* \param pAccumulator     This reference is only used during construction for retrieving
		//	*                         the data required to fill the iterator.
		//	* \param pFilterClass     The bitwise combination of the AEClass identifiers. An AccumulatorEntry
		//	*                         element is copyed from the accumulator if its Class matches one of the
		//	*	                        bits of this flag.
		//	* \param pSrc	            Specify which data format is extracted from the accumulator.
		//	* \param pNoDetail	    This parameter is used ONLY if pSrc == eACCUMULATOR_ENTRY and, if set to
		//	*                         false, the details of the AccumulatorEntry are also sent to the output
		//	*						    devices. If left to its default value, only the description of the
		//	*						    AccumulatorEntry is sent.
		//	*/
		//	/*KFbxUserNotificationFilteredIterator(FbxUserNotification& pAccumulator, 
		//	int pFilterClass,
		//	KFbxUserNotification::OutputSource pSrc = KFbxUserNotification::eSEQUENCED_DETAILS,
		//	bool pNoDetail = true);*/			

		//	//! Returns the number of elements contained in this iterator.
		//	property int NbItems
		//	{
		//		int get();
		//	}

		//	//! Put the iterator in its reset state.
		//	void Reset();

		//	/** Get this iterator's first item. 
		//	* \return     NULL if the iterator is empty.
		//	*/
		//	property FbxAccumulatorEntryInfo^ First
		//	{
		//		FbxAccumulatorEntryInfo^ get();
		//	}

		//	/** Get this iterator's previous item.
		//	* \return     NULL if the iterator reached the beginning (or is empty).
		//	* \remarks    This method will also return NULL if it is called before
		//	*             or immediately after a call to First() and reset the iterator to
		//	*             its reset state (meaning that a call to First() is mandatory
		//	*             to be able to iterate again).
		//	*/				
		//	FbxAccumulatorEntryInfo^ Previous();

		//	/** Get this iterator's next item.
		//	* \return     NULL if the iterator reached the end (or is empty).
		//	* \remark     This method will also return NULL if it is called while 
		//	*             the iterator is in its reset state (called before
		//	*             First() or after a preceding call to Previous() reached 
		//	*             beyond the beginning).
		//	*/
		//	//AccumulatorEntry* const Next();	
		//	FbxAccumulatorEntryInfo^ Next();
		//};



		///** This class accumulates user notifications and sends them to any device opened
		//* by the derived classes. If this class is not derived, the data can only be
		//* sent to a log file. To send data to a log file, it must be opened before attempting
		//* to send data, otherwise, the messages will be lost.
		//*/
		//public ref class FbxUserNotification
		//{
		//internal:
		//	KFbxUserNotification* notification;
		//	bool isNew;			
		//	FbxUserNotification(KFbxUserNotification* n);
		//public:
		//	static FbxUserNotification^ Create(FbxSdkManager^ %manager, 
		//		FbxString^ logFileName, 
		//		FbxString^ sessionDescription);

		//	static void Destroy(FbxSdkManager^ %manager);

		//	/** Instanciate a KFbxUserNotification but leave it uninitialized. The caller must
		//	* explicitly call InitAccumulator to initialize it and ClearAccumulator when finished
		//	* using it.
		//	* \param pManager
		//	* \param pLogFileName            Name of the log file that will be open in the directory 
		//	*                                defined by the GetLogFilePath method.
		//	* \remarks                       If pLogFileName is an empty string the logfile does not get created and any
		//	*                                output sent to it is lost.
		//	* \param pSessionDescription     This string is used to separate session logs in the file.
		//	* \remarks                       If the specified logfile already exists, messages are appended to it. This
		//	*                                class never deletes the log file. Derived classes may delete the log file 
		//	*                                before opening (it must be done in the constructor because the log file is 
		//	*                                opened in the InitAccumulator) or at the end of the processing in the 
		//	*                                PostTerminate method.
		//	*/
		//	FbxUserNotification(FbxSdkManager^ manager,
		//		FbxString^ logFileName, 
		//		FbxString^  sessionDescription);
		//	~FbxUserNotification();
		//	!FbxUserNotification();

		//	/** This method must be called before using the Accumulator. It opens the log file and
		//	* calls AccumulatorInit followed by OpenExtraDevices. Failing to call this method
		//	* will prevent other actions except ClearAccumulator, GetLogFileName and GetLogFilePath.
		//	*/
		//	void InitializeAccumulator();

		//	/** This method must be called when the Accumulator is no longer needed. It calls 
		//	* CloseExtraDevices, followed by the AccumulatorClear, and then closes the log file.
		//	*/
		//	void ClearAccumulator();

		//	enum class AeID {
		//		BindPoseInvalidObject = KFbxUserNotification::eBINDPOSE_INVALIDOBJECT,
		//		BindPoseInvalidRoot = KFbxUserNotification::eBINDPOSE_INVALIDROOT,
		//		BindPoseNotAllAncestorsNodes = KFbxUserNotification::eBINDPOSE_NOTALLANCESTORS_NODES,
		//		BindPoseNotAllDeformingNodes= KFbxUserNotification::eBINDPOSE_NOTALLDEFORMING_NODES,
		//		BindPoseNotAllAncestorsDefNodes = KFbxUserNotification::eBINDPOSE_NOTALLANCESTORS_DEFNODES,
		//		BindPose_RelativeMatrix = KFbxUserNotification::eBINDPOSE_RELATIVEMATRIX,
		//		FileIONotification= KFbxUserNotification::eFILE_IO_NOTIFICATION, // this is generic for reader and writer to log notifications.
		//		FileIONotificationMaterial = KFbxUserNotification::eFILE_IO_NOTIFICATION_MATERIAL,
		//		StartID = KFbxUserNotification::eAE_START_ID // Starting ID for any Accumulator entry added by derived classes.
		//	};

		//	/**
		//	* \name Accumulator Management
		//	*/
		//	//@{
		//	/** Adds one entry into the accumulator.
		//	* \param pID          This entry unique ID.
		//	*	\param pName        This entry name.
		//	*	\param pDescr       The description of this entry.
		//	* \param pClass       The category of this entry.
		//	* \returns            The ID of the newly allocated entry. This ID is pEntryId.
		//	*/
		//	int AddEntry(int id, FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass);

		//	int AddEntry(int id, FbxString^ name, FbxString^ descr);

		//	/** Completes the accumulator entry (there can be more that one detail for each entry) and implicitly defines
		//	* the sequence of events. Each call to this method is internally recorded, making it possible to output each
		//	* notification in the order they have been defined. Also, when a detail is added to an entry, it is automatically unmuted
		//	* so it can be sent to the devices (muted AccumulatorEntry objects are not processed).
		//	* \param pEntryId     The entry index (as returned by AddEntry).
		//	* \return             The id of the detail in the recorded sequence of events. This Id should be used when the call to
		//	*                     Output has the eSEQUENCED_DETAILS set as a source. If an error occurs, the returned value is -1
		//	*/
		//	int AddDetail(int entryId);

		//	/** Completes the accumulator entry (there can be more that one detail for each entry) and implicitly defines
		//	* the sequence of events. Each call to this method is internally recorded, making it possible to output each
		//	* notification in the order they have been defined. Also, when a detail is added to an entry, it is automatically unmuted
		//	* so it can be sent to the devices (muted AccumulatorEntry objects are not processed).
		//	* \param pEntryId     The entry index (as returned by AddEntry).
		//	* \param pString      The detail string to add to the entry.
		//	* \return             The id of the detail in the recorded sequence of events. This Id should be used when the call to
		//	*                     Output has the eSEQUENCED_DETAILS set as a source. If an error occurs, the returned value is -1
		//	*/
		//	int AddDetail(int entryId, FbxString^ str);

		//	/** Completes the accumulator entry (there can be more that one detail for each entry) and implicitly defines
		//	* the sequence of events. Each call to this method is internally recorded, making it possible to output each
		//	* notification in the order they have been defined. Also, when a detail is added to an entry, it is automatically unmuted
		//	* so it can be sent to the devices (muted AccumulatorEntry objects are not processed).
		//	* \param pEntryId     The entry index (as returned by AddEntry).
		//	* \param pNode        The node to add to the entry.
		//	* \return             The id of the detail in the recorded sequence of events. This Id should be used when the call to
		//	*                     Output has the eSEQUENCED_DETAILS set as a source. If an error occurs, the returned value is -1
		//	*/
		//	int AddDetail(int entryId, FbxNode^ %node);

		//	//! Returns the number of AccumulatorEntries currently stored in this accumulator.
		//	property int  NbEntries
		//	{
		//		int get();
		//	}

		//	/** Get the specified AccumulatorEntry.
		//	* \param pEntryId     ID of the entry to retrieve.
		//	* \return             Pointer to the specified entry, otherwise \c NULL if either the id is invalid or the Accumulator
		//	*                     is not properly initialized.
		//	*/
		//	FbxAccumulatorEntryInfo^ GetEntry(int entryId);

		//	/** Get the AccumulatorEntry at the specified index.
		//	* \param pEntryIndex     index of the entry to retrieve.
		//	* \return                Pointer to the specified entry, otherwise \c NULL if either the index is invalid or the Accumulator
		//	*                        is not properly initialized..
		//	*/
		//	FbxAccumulatorEntryInfo^ GetEntryAt(int entryIndex);

		//	//! Returns the number of Details recorded so far in this accumulator.
		//	property int NbDetails
		//	{
		//		int get();
		//	}

		//	/** Get the specified detail.
		//	* \param pDetailId     Index of the detail. This is the idth detail of type pClass as inserted
		//	*                      when the AddDetail 
		//	* \param pAE           Pointer to the AccumulatorEntry object that contains the requested detail.
		//	*                      The returned valued can be NULL if an error occured.
		//	* \return              The index of the detail to be used when calling the GetDetail of the AccumulatorEntry.
		//	* \remarks             A value of -1 is acceptable and means that the AccumulatorEntry has no details. However,
		//	*                      if pAE is NULL, the return value is meaningless.
		//	*/
		//	/*int GetDetail(int detailId, FbxAccumulatorEntry^ %ae)
		//	{
		//	AccumulatorEntry *e;
		//	int result = notification->GetDetail(detailId,e);
		//	if(e)
		//	{
		//	ae->entry = e;
		//	ae->isNew = true;
		//	}
		//	return result;
		//	}*/

		//	//@}

		//	/**
		//	* \name Accumulator Output
		//	*/
		//	//@{
		//	enum class OutputSource {
		//		AccumulatorEntry = KFbxUserNotification::eACCUMULATOR_ENTRY,
		//		SequencesDetails= KFbxUserNotification::eSEQUENCED_DETAILS
		//	};


		//	/** Send the accumulator entries to the output devices.
		//	* This method needs to be explicitly called by the program that uses this
		//	* class. 
		//	* \param pOutSrc               Specify which data has to be sent to the output devices. Set to SEQUENCED_DETAILS
		//	*                              to send the Details in the recorded order. Set to ACCUMULATOR_ENTRY to send
		//	*                              each entry with its details regardless of the order in which the events occurred.
		//	* \param pIndex                If this parameter >= 0, only send the specified entry/detail index to the output devices.
		//	*                              Otherwise send all of them.
		//	* \param pExtraDevicesOnly     If this parameter is True, the output is not sent to the log file.
		//	* \remark                      The pExtraDevicesOnly parameter is ignored if the log file has been disabled.
		//	*/
		//	bool Output(OutputSource outSrc, int index, bool extraDevicesOnly);

		//	bool Output(OutputSource outSrc);
		//	bool Output(OutputSource outSrc, int index);

		//	/** Send the accumulator entry to the output devices.
		//	* \param pId		             Send the entry/detail that matching pIdx to the output devices,
		//	*                              otherwise send all of them.
		//	* \param pOutSrc               Specify which data has to be sent to the output devices. Set to SEQUENCED_DETAILS
		//	*                              to send the Details in the recorded order. Set to ACCUMULATOR_ENTRY to send
		//	*                              each entry with its details regardless of the order in which the events occurred..
		//	* \param pExtraDevicesOnly     If this parameter is True, the output is not sent to the log file.
		//	*/	  
		//	bool OutputById(AeID id, OutputSource outSrc, bool extraDevicesOnly );
		//	bool OutputById(AeID id);
		//	bool OutputById(AeID id, OutputSource outSrc);

		//	/** Send an immediate entry to the output devices.
		//	* This metohod bypasses the accumulator by sending the entry directly to the output devices
		//	* and discarding it right after. The internal accumulator lists are left unchanged by this call.
		//	*	\param pName                 This entry name.
		//	*	\param pDescr                The description of this entry.
		//	* \param pClass                The category of this entry.
		//	* \param pExtraDevicesOnly     If this parameter is True, the output is not sent to the log file.
		//	* \remarks                     The pExtraDevicesOnly parameter is ignored if the log file has been disabled.
		//	*/
		//	bool Output(FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass, bool extraDevicesOnly);		
		//	bool Output(FbxString^ name, FbxString^ descr, FbxAccumulatorEntry::AEClass aeClass);			

		//	/** Sends the content of the iterator to the output devices.
		//	* This metohod bypasses the accumulator by sending each entry in the iterator directly to 
		//	* the output devices. The internal accumulator lists are left unchanged by this call.
		//	*	\param pAEFIter              The Filtered AccumulatorEntry iterator object.
		//	* \param pExtraDevicesOnly     If this parameter is True, the output is not sent to the log file.
		//	* \remarks                     The pExtraDevicesOnly parameter is ignored if the log file has been disabled.
		//	*/
		//	bool Output(FbxUserNotificationFilteredIterator^ aeFIter, bool extraDevicesOnly);
		//	bool Output(FbxUserNotificationFilteredIterator^ aeFIter);

		//	/**
		//	* \name Utilities
		//	*/
		//	//@{
		//	/** Returns the absolute path to the log file. If this method is not overridden in a derived class, it
		//	*  returns the TEMP directory.
		//	* \param pPath     The returned path.
		//	*/
		//	virtual void GetLogFilePath(FbxString^ path);

		//	/** Returns the log file name 
		//	*/	
		//	/*FbxString^ GetLogFileName()
		//	{
		//	return gcnew FbxString(&notification->GetLogFileName());
		//	}*/
		//	//@}		

		//	static FbxUserNotificationFilteredIterator^ CreateFilteredIterator(FbxUserNotification^ accumulator, 
		//		int filterClass,
		//		FbxUserNotification::OutputSource src,
		//		bool noDetail);
		//};

#if 0
		/** \brief This class sends accumulated messages to a file handler specified by a string.
		* If the string argument is "StdOut" or "StdErr" (case insensitive), the standard C stdout/stderr devices are used. 
		* Similarly, "cout" and "cerr" can be used for standard out/error. Otherwise, the string argument is assumed to be 
		* a full filename and is used to open a text file for write. This class does not creates a log file by default.
		*/
		class KFBX_DLL KFbxUserNotificationFILE : public KFbxUserNotification
		{
		public:

			KFbxUserNotificationFILE(FbxString pFileDevice, FbxString pLogFileName="", FbxString pSessionDescription="");
			virtual ~KFbxUserNotificationFILE();

			virtual void OpenExtraDevices();
			virtual bool SendToExtraDevices(bool pOutputNow, KArrayTemplate<AccumulatorEntry*>& pEntries);
			virtual bool SendToExtraDevices(bool pOutputNow, KArrayTemplate<AESequence*>& pAESequence);
			virtual bool SendToExtraDevices(bool pOutputNow, const AccumulatorEntry* pAccEntry, int pDetailId = -1);
			virtual void CloseExtraDevices();

		private:
			FbxString mFileDevice;
			FILE* mFP;
			int   mUseStream;
		};
#endif

	}
}