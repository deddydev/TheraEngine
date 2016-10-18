#ifndef _FBXSDK_FILEIO_EXTERNAL_DOCREF_LISTENER_H_
#define _FBXSDK_FILEIO_EXTERNAL_DOCREF_LISTENER_H_
struct FBXSDK_DLL FbxExternalDocumentInfo
{
FbxString mDocumentName;
FbxString mClassName;
FbxString mParentFullName;
FbxString mFilePathUrl;
};
class FBXSDK_DLL FbxEventReferencedDocument : public FbxEvent<FbxEventReferencedDocument>, public FbxExternalDocumentInfo
{
FBXSDK_EVENT_DECLARE(FbxEventReferencedDocument);
public:
FbxEventReferencedDocument() {}
};
class FbxExternalDocRefListenerData;
class FBXSDK_DLL FbxExternalDocRefListener : public FbxListener
{
public:
FbxExternalDocRefListener( FbxManager & pManager, const FbxString & pDocFilePath );
virtual ~FbxExternalDocRefListener();
virtual void SetDocumentFilePath( const FbxString & pDocFilePath );
virtual bool AreAllExternalDocumentsStillValid() const;
virtual bool WereAllExternalDocumentsValid() const;
virtual void UnloadExternalDocuments();
virtual void HandleEvent(const FbxEventReferencedDocument * pEvent);
};
#endif