#pragma once
#include "stdafx.h"
#include "FbxKEventBase.h"
#include "kfcurve/kfcurvenode.h"

using namespace System::Runtime::InteropServices;


{
	namespace FbxSDK
	{		
		public enum class FbxCurveNodeEventType
		{
			None = 0, 
			Selection = 1, 
			Destroy = 2, 
			FCurve = 4, 
			Timewarp = 8, 
			CtrlCurve = 16, 
			DataUpdated = 32
		};

		ref class FbxTime;

		public ref struct FbxExternalTimingInformation : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxExternalTimingInformation,KExternalTimingInformation);
			INATIVEPOINTER_DECLARE(FbxExternalTimingInformation,KExternalTimingInformation);
		public:
			REF_PROPERTY_GETSET_DECLARE(FbxTime,LclOffset);
			REF_PROPERTY_GETSET_DECLARE(FbxTime,Duration);			
		};


		// Curve node event class.
		public ref class FbxCurveNodeEvent : FbxKEventBase
		{
			REF_DECLARE(FbxKEventBase,KFCurveNodeEvent);
		internal:
			FbxCurveNodeEvent(KFCurveNodeEvent* instance) :FbxKEventBase(instance)
			{
				_Free = false;
			}

		public:			
			/* Nature (s) of this node event.
			*	It is a bit field.  Value defined in the enum stated before 
			*	the class can composited. 
			* stored in mType
			*/
			// Event count.
			VALUE_PROPERTY_GETSET_DECLARE(int,EventCount);

			// Data.
			VALUE_PROPERTY_GETSET_DECLARE(IntPtr,Data);				

			// Clear the curve node event object.
			void Clear ();

			/* Add an event of type pWhat.
			*	\param pWhat	Use EKFCurveNodeEventType to identify what pWhat means.
			*/
			void Add (int what);

			// Set the data contained by the node event.
			//void SetData(void* pData) {mData = pData;}
		};



		public ref struct FbxCurveNodeCallback 
		{
			/*internal:
			KFCurveNodeCallback* cn;
			bool isNew;
			FbxCurveNodeCallback(KFCurveNodeCallback* cn);*/

			//KFCurveNodeEvent mEvent;
			//KArrayUL mCallbackFunctions;   // no delete on object must use array ul
			//KArrayUL mCallbackObjects;	   // no delete on object must use array ul
			//bool mCallbackEnable;
		};
		public ref class FbxCurveNodeCandidateState : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCurveNodeCandidateState,KFCurveNodeCandidateState);
			INATIVEPOINTER_DECLARE(FbxCurveNodeCandidateState,KFCurveNodeCandidateState);		
		public:
			DEFAULT_CONSTRUCTOR(FbxCurveNodeCandidateState,KFCurveNodeCandidateState);			

			void Dump(int level);
			void Dump()
			{
				Dump(0);
			}

			//void SetCandidateTotalTime (FbxTime^ candidateTime );
			//void SetCandidateSpecificTime (FbxTime^ candidateTime );			

		};

		ref class FbxCurve;
		public ref class FbxCurveNode : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCurveNode,KFCurveNode);
			INATIVEPOINTER_DECLARE(FbxCurveNode,KFCurveNode);		

		protected:
			FbxCurve^ _FbxCurve;
		public:

			/**
			* \name Constructors and Destructor
			*/
			//@{

			/** Constructor.
			* \param pNodeName Curve node name.
			* \param pTimeWarpName Curve node.
			* \param pDataType Curve node type.
			* \param pLayerType LayerType.
			* \param pLayerID LayerID.
			* \remarks No function curve is created in the new curve node.
			*/
			/*FbxCurveNode(String^ nodeName, String^ timeWarpName, 
				FbxDataType^ dataType,int layerType,int	layerID);*/

			/** Template constructor.
			* \param pTemplateCurveNode Template curve node. 
			* \remarks This is not a copy constructor. This constructor makes the
			* created curve node share a structure that defines the node name, node 
			* display name and node type name. No function curve is created or 
			* copied in the new curve node.
			*/
			//FbxCurveNode(FbxCurveNode^ templateCurveNode);					

#ifdef K_PLUGIN
			void Destroy (int local);
			void Destroy ()
			{
				Destroy (0);
			}
#else
			IObject_Declare (Implementation) 
#endif

				//@}

				/**
				* \name FCurve Creation
				*/
				//@{

				/** Create the FCurve for this node.
				* This function is called recursively for all children curve nodes.
				*/
				void CreateFCurve( );

			/** Test to know if the FCurve is created.
			* This function is called recursively for all children curve nodes.
			* It returns false as soon as one child does not have its FCurve.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,FCurveCreated);

			//@}

			/**
			* \name Cloning and Copy Functions
			*/
			//@{

			/** Return a duplicate of this node.
			* This function is called recursively for all children curve nodes.
			* The function curve is copied in the new curve node.
			*	\param pKeepAttached If \c true, the created object will share a structure 
			* that defines the node name, node display name and node type name. Otherwise,
			* this structure is copied.
			* \remarks This function is equal to a call to KFCurveNode::CloneTemplate() 
			* with parameter \c pCloneFCurves set to \c true.
			* \return A partial or complete copy of the current object.
			*/
			FbxCurveNode^ Clone(bool keepAttached);

			/** Return a duplicate of this node.
			* This function is called recursively for all children curve nodes.
			*	\param pKeepAttached If \c true, the created object will share a structure 
			* that defines the node name, node display name and node type name. Otherwise,
			* this structure is copied.
			*	\param pCloneFCurves If \c true and the current curve node has a function curve, 
			* the function curve is copied in the new curve node. 
			* \param pCreateCurves If \c true, \c pCloneFCurve is set to \c false and the 
			* current curve node has a function curve, an empty function curve is created 
			* in the new curve node.
			* \param pLayerID LayerID.
			* \return A partial or complete copy of the current object.
			*/
			//default is false and layerID = -1;
			FbxCurveNode^ CloneTemplate(bool keepAttached, bool cloneFCurves, bool createCurves, int layerID);

			/**	Copy the function curve contained in the given curve node.
			* \param pSource Given curve node to copy the function curve from.
			* \param pTransferCurve If \c true, curve data is transferred instead of being copied.
			* \remarks This function is called recursively. The first child 
			* curve node of \c pSource is copied in the first child curve node 
			* of the current object and so on.
			*/
			void CopyFrom(FbxCurveNode^ source,bool transferCurve);
			void CopyFrom(FbxCurveNode^ source)
			{
				CopyFrom(source,false);
			}

			/** Create a copy of the current curve node.
			* The structure defining the node name, node display name and node 
			* type name is copied, not shared. Only the animation keys in a 
			* given time range are copied.
			* \param pStart Start time for range of animation keys copied.
			* \param pStop Stop time for range of animation keys copied.
			* \return A partial or complete copy of the current object.
			*/
			//default is infinite time
			FbxCurveNode^ Copy(FbxTime^ start, FbxTime^ stop);

			//@}

			/**
			* \name Node Identification
			*/
			//@{

			//! Retrieve the node's name.
			VALUE_PROPERTY_GET_DECLARE(String^,Name);

			//! Retrieve the node's timewrap name.
			VALUE_PROPERTY_GET_DECLARE(String^,TimeWarpName);				

			//! Retrieve the node type name.
			VALUE_PROPERTY_GET_DECLARE(String^,TypeName);				

			//@}

			/**
			* \name Function Curve Management
			*/
			//@{

			/** Get pointer to the node's function curve.
			* \return Pointer to the curve or \c NULL pointer if there's none. 
			*/
			FbxCurve^ FCurveGet();

			/** Set the function curve of this curve node.
			*	\param pCurve New function curve.
			*	\param pDestroyOldCurve Set to \c true to delete the function 
			* curve already contained in the curve node.
			*	\return Function curve previously contained in the curve node
			* if there was any and if parameter \c pDestroyOldCurve is set 
			* to \c false. Otherwise, \c NULL is returned.
			*/
			//default of destroyOldCurve = false
			FbxCurve^ FCurveSet(FbxCurve^ curve, bool destroyOldCurve);

			/** Replace the function curve of this curve node.
			*	Previous function curve is automatically destroyed.
			*	\param pCurve New function curve.
			* \remarks This function is equal to a call to KFCurveNode::FCurveSet() 
			* with parameter \c pDestroyOldCurve set to \c true.
			*/
			void FCurveReplace (FbxCurve^ curve) ;

			/** Return translation, rotation and scaling curves of the children of this curve node.
			* This function is meant to be called on the root KFCurveNode object found in class
			* KFbxTakeNode.
			*	\param pT Array to receive X, Y and Z translation function curves. 
			*	\param pR Array to receive X, Y and Z rotation function curves. 
			*	\param pS Array to receive X, Y and Z scaling function curves. 
			*	\return \c true if all function curves have been found, \c false otherwise.
			*/
			//bool GetTransformCurves(FbxCurve^ pT[3], KFCurve* pR[3], KFCurve* pS[3]) ;

			//@}

			/**
			* \name Curve Node Management
			*/
			//@{

			/** Clear this node.
			* Delete the function curve and all the children curve nodes.
			*/
			virtual void Clear();

			//! Add a child curve node to this node.
			virtual int Add(FbxCurveNode^ curveNode);

			/** Remove a child curve node by index.
			* \param pIndex Index of child curve node to be removed.
			*/
			virtual void Remove(int index) ;

			/** Remove and delete a child curve node by index.
			* \param pIndex Index of child curve node to be deleted.
			*/
			virtual void Delete(int index) ;

			//! Get children curve nodes count.
			virtual VALUE_PROPERTY_GET_DECLARE(int,Count) ;

			/** Get pointer to child curve node by index.
			* \param pIndex Index of child curve node.
			* \return Pointer to child curve node or \c NULL is index is out of bounds.
			*/
			virtual FbxCurveNode^ Get(int index) ;

			/** Ask if a given curve node is a child of the current curve node.
			* \param pCurveNode Curve node searched.
			* \param pRecursive Set to \c true to search recursively. Set to \c false
			* to search only in the immediate children curve nodes.
			* \return \c true if the curve node searched is found, \c false otherwise.
			*/
			//default of recursive is false
			virtual bool IsChild(FbxCurveNode^ curveNode, bool recursive) ;

			/** Get index of child curve node by name.
			* \param pName Name of searched child curve node.
			*	\return Index or -1 if a child curve node with this name could not be found. 
			* \remarks This function is not recursive.
			*/
			virtual int Find (String^ name);

			/** Get index of child curve node by pointer.
			* \param pNode Pointer to searched child curve node.
			*	\return Index or -1 if a child curve node with this pointer could not be found. 
			* \remarks This function is not recursive.
			*/
			virtual int Find(FbxCurveNode^ node);

			/** Recursively find the first child curve node by name.
			* \param pName Name of searched child curve node.
			* \return Pointer to child curve node or \c NULL if this name could not be found. 
			*/
			FbxCurveNode^ FindRecursive(String^ name);

			/** Find a child node by name or create it if it can not be found.
			* \param pName Name of child node to find.
			*	\param pFindOrCreateCurve If \c true and the function curve does not exist, create 
			* one in the curve node found or created.
			* \remarks This function is not recursive.
			*/
			//findOrCreateCurve is true
			FbxCurveNode^ FindOrCreate(String^ name, bool findOrCreateCurve);

			/** Get the parent curve node.
			*	\return Pointer to the parent curve node or \c NULL if there is none. 
			*/
			//! Set the parent curve node.
			virtual REF_PROPERTY_GETSET_DECLARE(FbxCurveNode,Parent);											

			//@}

			/**
			* \name Key Management
			*/
			//@{

			/** Get the number of animation keys.
			* Return the total number of keys contained in the function curves 
			* included in the current node and its children curve nodes.
			*/
			//default is false
			int KeyGetCount(bool recursiveInLayers);

			/** Get the number of animation keys.
			* \param pCurveCount returns the number of fcurves held by this node
			* \param pTotalCount returns the number of keys on all the fcurves held by this node
			* \param pMinCount returns the minimum number of keys on an fcurve held by this node
			* \param pMaxCount returns the maximum number of keys on an fcurve held by this node
			*/
			void KeyGetCount([OutAttribute]int %curveCount, [OutAttribute]int %totalCount, [OutAttribute]int %minCount, [OutAttribute]int %maxCount);

			// Get the fcurve that has the earliest key. If many fcurves
			// has a key at the earliest time, returns one of the fcurves.
			//void GetCurveWithFirstKey(FCurve*& lCurveWithFirstKey, bool *pCurveMask /* = NULL */, bool pRecursiveInLayers);
			// Get the fcurve that has the latest key. If many fcurves has
			// a key at the latest time, returns one of the fcurves.
			//void GetCurveWithLastKey(KFCurve*& lCurveWithLastKey, bool *pCurveMask /* = NULL */, bool pRecursiveInLayers);

			// Get the first key time in the children specified by the curve mask (NULL = all children)
			// If there is no key, return KTIME_MINUS_INFINITE.
			//FbxTime^ GetFirstKeyTime(bool *pCurveMask = NULL, bool pRecursiveInLayers = false);
			// Get the last key time in the children specified by the curve mask (NULL = all children)
			// If there is no key, return KTIME_INFINITE.
			//KTime GetLastKeyTime(bool *pCurveMask = NULL, bool pRecursiveInLayers = false);

			/** Recursively add a set of keys in the curve node tree.
			* Depth first key adding in the function curves included in the current 
			* node and its children curve nodes.
			* \param pTime Time to add the keys.
			* \param pValue Array containing all the key values. 
			* This array must contain as many elements as the number of function 
			* curves included in the current node and its children curve nodes.
			* \return Value of parameter \c pValue added with an offset equal to the
			* number of affected function curves times \c sizeof(kDouble).
			*/
			//double* KeyAdd (KTime pTime, double* pValue) ;

			/** Recursively append a set of keys in the curve node tree.
			* Depth first key appending in the function curves included in the current 
			* node and its children curve nodes.
			* \param pTime Time set in the appended keys. Make sure this time has a 
			* greater value than any of the last keys in the affected function curves.
			* \param pValue Array containing all the key values. 
			* This array must contain as many elements as the number of function 
			* curves included in the current node and its children curve nodes.
			* \param pCheckForContinuousRotation Flag, when set to true we will check for continuous rotation values.
			* This is like an unroll filter on the fly. Relevant only for rotation fcurve nodes.
			* \return Value of parameter \c pValue added with an offset equal to the
			* number of function curves affected times \c sizeof(kDouble).
			*/
			//double* KeyAppend (KTime pTime, double* pValue, bool pCheckForContinuousRotation = false) ;

			/** Find out start and stop time of the animation for a curve node and recursively in its children.
			*	\param pStart Reference to store start time. 
			* \c pStart is overwritten only if the start time found is lower than \c pStart value.
			* Initialize to KTIME_INFINITE to make sure the start time is overwritten in any case.
			*	\param pStop Reference to store end time.
			* \c pStop is overwritten only if the stop time found is higher than \c pStop value.
			* Initialize to KTIME_MINUS_INFINITE to make sure the stop time is overwritten in any case.
			*	\return \c true if at least one key has been found in all the function 
			* curves scanned.
			*/
			bool GetAnimationInterval (FbxTime^ start, FbxTime^ stop);

			/** Find out start and stop time of the animation for a curve node and recursively in it's children.
			*	\param pStart Reference to receive the smaller key time, set to KTIME_INFINITE if no key could be found.
			*	\param pStop Reference to receive the larger key time, set to KTIME_MINUS_INFINITE if no key could be found.
			*/
			void GetTimeSpan(FbxTime^ start, FbxTime^ stop) ;	

			/** Recursively check if keys exist at a given time in the curve node tree.
			* Check if a key exists at the given time in the function curves included 
			* in the current node and its children curve nodes.
			* \param pTime Given time to check key occurence.
			* \param pLast Function curve index to speed up search. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			* \param pCurveMask allows you to use only specific children in the CurveNode. NULL means all of them.
			* \param pKeyIndexTolerance allows the test to be less strict when matching the time with a key index.
			*	\param	pMin	Minimal Value to Consider the Key.
			*	\param	pMax	Maximal Value to Consider the Key.
			*	\return \c ISKEY_NONE if no function curve has a key at the given time,
			*	\c ISKEY_SOME if some function curves have a key at the given time or
			*	\c ISKEY_ALL if all function curves have a key at the given time.
			*/
			//int IsKey (KTime& pTime, int *pLast, bool* pCurveMask = NULL, double pKeyIndexTolerance = 0.0, double pMin = -K_DOUBLE_MAX,  double pMax = K_DOUBLE_MAX );

			/** Recursively delete keys at a given time in the curve node tree.
			* Delete keys at the given time in the function curves included 
			* in the current node and its children curve nodes.
			* \param pTime Given time to delete keys.
			* \param pCurveMask allows you to use only specific children in the CurveNode. NULL means all of them.
			* \param pKeyIndexTolerance allows to be less strict when matching the time with a key index.
			*	\return \c true if at least one key has been found at the given 
			* time and deleted.
			*/
			//bool DeleteKey (KTime& pTime, bool* pCurveMask = NULL, double pKeyIndexTolerance = 0.0 );

			/** Recursively find the closest later time at which a key can be found compared to a given time.
			* Find the closest but later time at which a key can be found 
			* compared to a given time in the function curves included in 
			* the current node and its children curve nodes.
			* \param pTime Given time to search the next key time.
			* \param pCurveMask allows you to use only specific children in the CurveNode. NULL means all of them.
			* \param pRecursiveInLayers
			*	\return Next key time or KTIME_INFINITE if there exists no key
			* with a time larger than the given time.
			*/
			//KTime GetNextKeyTime (KTime pTime, bool* pCurveMask = NULL, bool pRecursiveInLayers = false);

			/** Recursively find the closest ealier time at which a key can be found compared to a given time.
			* Find the closest earlier time at which a key can be found 
			* compared to a given time in the function curves included in 
			* the current node and its children curve nodes.
			* \param pTime Given time to search the previous key time.
			* \param pCurveMask allows you to use only specific children in the CurveNode. NULL means all of them.
			*	\return Previous key time or KTIME_MINUS_INFINITE if there exists no key
			* with a time smaller than the given time.
			*/
			//KTime GetPrevKeyTime (KTime pTime, bool* pCurveMask = NULL );

			/** Recursively evaluate the curve node tree.
			* Depth first evaluation of the function curves included in the current 
			* node and its children curve nodes.
			* \param pValue Array to receive all the function curve evaluations. 
			* This array must be long enough to contain as many elements as the
			* number of function curves included in the current node and its 
			* children curve nodes.
			* \param pTime Time of evaluation.
			* \param pLast Function curve index to speed up evaluation. If this 
			* function is called in a loop, initialize this value to 0 and let it 
			* be updated by each call.
			* \return Value of parameter \c pValue added with an offset equal to the
			* number of function curves evaluated times \c sizeof(double).
			*/
			//double* Evaluate (double* pValue, KTime pTime, kFCurveIndex* pLast=NULL) ;

			/** Recursively get the default values of the curve node tree.
			* Depth first access to the default values of the function curves 
			* included in the current node and its children curve nodes.
			* \param pValue Array to receive all the function curve default values. 
			* This array must be long enough to contain as many elements as the
			* number of function curves included in the current node and its 
			* children curve nodes.
			* \return Value of parameter \c pValue added with an offset equal to the
			* number of function curves accessed times \c sizeof(double).
			*/
			//double* GetValue (double* pValue) ;

			/** Recursively set the default values of the curve node tree.
			* Depth first setting of the default values of the function curves 
			* included in the current node and its children curve nodes.
			* \param pValue Array containing all the function curve default values. 
			* This array must contain as many elements as the number of function 
			* curves included in the current node and its children curve nodes.
			* \return Value of parameter \c pValue added with an offset equal to the
			* number of function curves set times \c sizeof(double).
			*/
			//double* SetValue (double* pValue) ;

			/** Delete all the keys found within a given time range.
			* Delete all the keys found within a given time range in the function 
			* curves included in the current node and its children curve nodes.
			* \param pStart Beginning of time range.
			* \param pStop End of time range.
			*/
			//default is infinity
			void Delete(FbxTime^ start, FbxTime^ stop);

			/** Replace all the keys found within a given time range.
			* Replace all the keys found within a given time range in the function 
			* curves included in the current node and its children curve nodes.
			* \param pSource Source node tree containing the replacement keys. The 
			* curve node tree below \c pSource must be identical to the curve node 
			* tree below the current node.
			* \param pStart Beginning of time range.
			* \param pStop End of time range.
			* \param pUseGivenSpan false = original behavior where the time of the first and last key was used
			* \param pKeyStartEndOnNoKey Inserts a key at the beginning and at the end of the range if there is no key to insert.
			* \param pTimeSpanOffset
			*/
			//default is infinity
			//useGivenSpan is false
			//keyStartEndOnNoKey is true
			//timeSpanOffset is zero
			void Replace(FbxCurveNode^ source, FbxTime^ start, FbxTime^ stop , bool useGivenSpan , bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset);

			//@}
//
//			///////////////////////////////////////////////////////////////////////////////
//			//
//			//  WARNING!
//			//
//			//	Anything beyond these lines may not be documented accurately and is 
//			// 	subject to change without notice.
//			//
//			///////////////////////////////////////////////////////////////////////////////
//
//#ifndef DOXYGEN_SHOULD_SKIP_THIS
//
//			/**
//			* \name Color
//			*/
//			//@{
//
//			//! Get the color of the curve node.
//			//float* GetColor();
//
//			//! Set The color of the curve node.
//			//void SetColor (float pColor[3]) ;
//
//			//@}
//
//			/**
//			* \name Referencing
//			*/
//			//@{
//
//			//! Increment and return the number of references.
//			int IncReferenceCount();
//
//			//! Decrement and return the number of references.
//			int DecReferenceCount();
//
//			//! Return the number of references.
//			VALUE_PROPERTY_GET_DECLARE(int,ReferenceCount);
//
//			//@}
//
//			/**
//			* \name Take Type
//			*/
//			//@{
//
//			//! Get the take type.
//			//! Set the take type.
//			VALUE_PROPERTY_GETSET_DECLARE(int,TakeType);							
//
//			//@}
//
//			/**
//			* \name Selection
//			*/
//			//@{
//
//			//! Get the child node currently selected.				
//			VALUE_PROPERTY_GET_DECLARE(bool,GetVisibility );
//
//			//! Set the selected child node.
//			//recursive=false
//			//recurseLayer = false;
//			//childIndex = -1
//			void SetVisibility (bool visible, bool recursive, bool recurseLayer, int childIndex);
//
//			//@}
//
//			/**
//			* \name Data Node Management
//			*/
//			//@{
//
//			/** Set the data node.
//			* \param pDataNode	Data node to set.
//			*	\param pRecursive	\c true will set this data node to all children node. 
//			*/
//			//recursive = true
//			void DataNodeSet(FbxCurveNode^ dataNode, bool recursive);
//
//			/** Retrieve the data node.
//			*	\return Pointer to the node or NULL pointer if there is none. 
//			*/
//			FbxCurveNode^ DataNodeGet();
//
//			/** Set to FCurve the Post or Pre Extrapolation Data.  If pRespectUserLock is on, no change is applied when in UserLock
//			*	\return success or not. 
//			*/
//			bool SetPreExtrapolation(kUInt newPreExtrapolation, bool respectUserLock);
//			bool SetPreExtrapolationCount(kUInt newPreExtrapolationCount, bool respectUserLock);
//			bool SetPostExtrapolation(kUInt newPreExtrapolation, bool respectUserLock);
//			bool SetPostExtrapolationCount(kUInt newPreExtrapolationCount, bool respectUserLock);
//
//			//@}
//
//			/**
//			* \name Container Type
//			*/
//			//@{
//
//			//! Get the container type.
//			//! Set the container type.
//			VALUE_PROPERTY_GETSET_DECLARE(int,ContainerType);
//
//			//@}
//
//			/**
//			* \name I/O Type Management 
//			*/
//			//@{
//
//			//! Get the IO Type.							
//			//! Set the IO Type.
//			VALUE_PROPERTY_GETSET_DECLARE(int,InOutType);				
//
//			//@}
//
//			/**
//			* \name Function Curve Editor Options 
//			*/
//			//@{
//
//			//! If the node is expended in FCurveEditor
//			//! Sets if the node is expended in FCurveEditor.
//			VALUE_PROPERTY_GETSET_DECLARE(bool,Expended);				
//
//			//@}
//
//			/**
//			* \name Layer Options 
//			*/
//			//@{
//
//			//! If the node can have layers	
//			//! Sets if the node is expended in FCurveEditor.
//			VALUE_PROPERTY_GETSET_DECLARE(bool,MultiLayer);				
//
//			//! Get the fcurve node for the specified layer.
//			FbxCurveNode^ GetLayerNode(int layerID);
//
//			//! Extract Keys in the Specified Range
//#ifndef K_PLUGIN
//			void ExtractKeysIndex( KArraykInt &pArray, KTimeSpan pTimeSpan, double pMinValue =  -K_DOUBLE_MAX, double pMaxValue =  K_DOUBLE_MAX);
//#endif
//
//			//@}
//
//			/**
//			* \name Analysis
//			*/
//			//@{
//
//			/*! Check if keys may come from a plot operation.
//			* \param pThresholdPeriod If the keys period is larger than this threshold, the function will return false.
//			*/
//			bool LookLikeSampledData(FbxTime^ thresholdPeriod);
//
//			//@}
//
//			/**
//			* \name Udpate
//			*/
//			//@{
//
//			//! Update id.
//			VALUE_PROPERTY_GET_DECLARE(int,UpdateId) ;
//
//			//! Update id.
//			VALUE_PROPERTY_GET_DECLARE(int,ValuesUpdateId) ;				
//
//			//! Node update id.				
//			int GetNodeUpdateId();
//
//
//			//@}
//
//			/**
//			* \name Callback
//			*/
//			//@{
//
//			//!
//			bool CallbackEnable (bool enable) ;
//
//			//!
//			bool CallbackIsEnable() ;
//
//			//!
//			void CallbackClear() ;
//
//			//!
//			void CallbackAddEvent(int what) ;
//
//			//!
//			//void CallbackRegister (kFCurveNodeCallback, void* pObject) ;
//
//			//!
//			//void CallbackUnregister (kFCurveNodeCallback, void* pObject) ;
//
//			//@}
//
//#ifndef K_PLUGIN
//			/**
//			* \name External Timing Information (for transportation only: optional, not used internally, and not persisted).
//			*/
//			//@{
//
//			//! Time interface attached to this node.
//			void SetETI(IKFCurveETI * pFCurveETI);
//			IKFCurveETI * GetETI() const;
//
//			//! Helpers for time conversions etc.
//			KTime ETINodeTime2GlobalTime(KTime const & pTime);
//			KTime ETIGlobalTime2NodeTime(KTime const & pTime);
//			KTime ETIGetDuration();
//			KTime ETIGetTrimOffset();
//			KTime ETIGetGlobalOffset();
//			double ETIGetScaleFactor();
//			bool ETIGetColor(KgeRGBVector & pColor);
//			bool ETIIsGlobalTimeInSpan(KTime const & pTime, bool pIncludeStop = true);
//			typedef KTime (IKFCurveETI::*TETITimeFetcher)();
//			KTime ETIGetSpecifiedTime(TETITimeFetcher pFunc);
//			bool ETIIsSelectedDuration();
//#endif
//
//			//@}
//
//			/*bool	FbxStore (KFbx* pFbx, bool pOnlyDefaults = false, bool pReplaceLclTRSName = false, bool pSkipTimeWarpName = false, char* pFieldName = "Channel", bool pColor = true, bool pIsVersion5 = false ) ;
//			bool	FbxRetrieve (KFbx* pFbx, bool pOnlyDefaults = false, bool pCreateIfNotFound = true,bool pColor = false);
//			bool	FbxInternalRetrieve (KFbx* pFbx, bool pOnlyDefaults = false, bool pCreateIfNotFound = true,bool pColor = false) ;
//			bool    FbxTimeWarpNameRetrieve(KFbx* pFbx);
//			bool    FbxLayerAndTimeWarpRetrieve(KFbx* pFbx, bool pOnlyDefaults = false, bool pCreateIfNotFound = true,bool pColor = false) ;
//			*/
//			//@{
//			/**
//			* \name AntiGimble
//			*/
//
//			VALUE_PROPERTY_GETSET_DECLARE(bool,UseQuaternion);				
//
//		public:
//			//@{
//			/**
//			* \name Undo state management
//			*/
//
//			void GetCandidateState(FbxCurveNodeCandidateState^ state);
//			void SetCandidateState(FbxCurveNodeCandidateState^ state, bool destroyMissingLayers);
//
//			int		GetCandidateSpecificMethod();
//			int		GetCandidateTotalMethod();
//			FbxTime^ GetCandidateTotalTime();
//			//double*	GetCandidateTotalValue();
//			int		GetCandidateTotalValueSize();
//
//			void	SetCandidateSpecificMethod(int method);
//			void	SetCandidateTotalMethod(int method);
//			void	SetCandidateTotalTime(FbxTime^ time);
//			//void	SetCandidateTotalValue(double* value);
//
//			//@}
//
//			//void GetQuaternionInterpolationBezierPoints(FbxTime^ currentTime, KgeQuaternion &pQ0, KgeQuaternion &lQ1, KgeQuaternion &lQ2, KgeQuaternion &lQ3);					
//
//		public:				
//			VALUE_PROPERTY_GETSET_DECLARE(int,RotationOrder);				
//
//			//! Get layer Type ( = Mult, Add or Rotational node )
//			VALUE_PROPERTY_GET_DECLARE(int,LayerType);		
//
//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};


	}
}