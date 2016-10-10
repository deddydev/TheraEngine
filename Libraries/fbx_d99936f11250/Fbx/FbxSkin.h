#pragma once
#include "stdafx.h"
#include "FbxDeformer.h"


{	
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxGeometry;
		ref class FbxCluster;
		ref class FbxNode;
		/** FBX SDK skin class
		* \nosubgrouping
		*/

		public ref class  FbxSkin : FbxDeformer
		{
			REF_DECLARE(FbxEmitter,KFbxSkin);
		internal:
			FbxSkin(KFbxSkin* instance);

			FBXOBJECT_DECLARE(FbxSkin);
		protected:
			virtual void CollectManagedMemory()override;
			System::Collections::Generic::List<FbxCluster^>^ _list;
		public:			

			/** Get deformation accuracy.
			* \return                        deformation accuracy value.
			*/
			/** Set deformation accuracy.
			* \param pDeformAccuracy         value for deformation accuracy.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DeformAccuracy);

			/** Get the node affected by this skin deformer.
			* \return                        a pointer to the node if set or NULL.
			*/
			/** Set the node affected by this skin deformer.
			* \param pNode                   Pointer to the node object to set.
			* \return                        \c true on success, \c false otherwise. 
			*/
			//REF_PROPERTY_GETSET_DECLARE(FbxNode,Node);			

			/** Get the geometry affected by this skin deformer.
			* \return                        a pointer to the geometry if set or NULL.
			*/
			/** Set the geometry affected by this skin deformer.
			* \param pGeometry               Pointer to the geometry object to set.
			* \return                        \c true on success, \c false otherwise.
			*/			
			//REF_PROPERTY_GETSET_DECLARE(FbxGeometry,Geometry);			

			/** Add a cluster.
			* \param pCluster                Pointer to the cluster object to add.
			* \return                        \c true on success, \c false otherwose.
			*/
						bool AddCluster(FbxCluster^ cluster);
			
						/** Remove cluster at given index.
						* \param pCluster                Pointer to the cluster to remove from this skin deformer.
						* \return                        Pointer to cluster or \c NULL if pCluster is not owned by this skin deformer.
						*/
						FbxCluster^ RemoveCluster(FbxCluster^ cluster);
			
						/** Get the number of clusters.
						* \return                        Number of clusters that have been added to this object.
						*/
						VALUE_PROPERTY_GET_DECLARE(int,ClusterCount);
			
						/** Get cluster at given index.
						* \param pIndex                  Index of cluster.
						* \return                        Pointer to cluster or \c NULL if index is out of range.
						*/
						FbxCluster^ GetCluster(int index);
			
						/** Get cluster at given index.
						* \param pIndex                  Index of cluster.
						* \return                        Pointer to cluster or \c NULL if index is out of range.
						*/
						//KFbxCluster const* GetCluster(int pIndex) const;
			
						/** Get the type of the deformer.
						* \return                        Deformer type identifier.
						*/
						//EDeformerType GetDeformerType()  const {return eSKIN; };
			
						///////////////////////////////////////////////////////////////////////////////
						//
						//  WARNING!
						//
						//  Anything beyond these lines may not be documented accurately and is
						//  subject to change without notice.
						//
						///////////////////////////////////////////////////////////////////////////////
			
			#ifndef DOXYGEN_SHOULD_SKIP_THIS
			
						// Clone
						CLONE_DECLARE();
			
			#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}