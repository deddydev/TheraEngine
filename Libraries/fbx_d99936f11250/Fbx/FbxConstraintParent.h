#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"



{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxVector4;
		/** \brief This constraint class contains methods for accessing the properties of a parent constraint.
		* A parent constraint lets you constrain the translation, scaling, rotation of an object based on one or more parent objects.
		* \nosubgrouping
		*/
		public ref class FbxConstraintParent : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxConstraintParent);
		internal:
			FbxConstraintParent(KFbxConstraintParent* instance):FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			
			FBXOBJECT_DECLARE(FbxConstraintParent);

			/**
			* \name Properties
			*/
			//@{
			/*KFbxTypedProperty<fbxBool1>		Lock;
			KFbxTypedProperty<fbxBool1>		Active;

			KFbxTypedProperty<fbxBool1>		AffectTranslationX;
			KFbxTypedProperty<fbxBool1>		AffectTranslationY;
			KFbxTypedProperty<fbxBool1>		AffectTranslationZ;

			KFbxTypedProperty<fbxBool1>		AffectRotationX;
			KFbxTypedProperty<fbxBool1>		AffectRotationY;
			KFbxTypedProperty<fbxBool1>		AffectRotationZ;

			KFbxTypedProperty<fbxBool1>		AffectScalingX;
			KFbxTypedProperty<fbxBool1>		AffectScalingY;
			KFbxTypedProperty<fbxBool1>		AffectScalingZ;

			KFbxTypedProperty<fbxDouble1>	Weight;

			KFbxTypedProperty<fbxReference> ConstraintSources;
			KFbxTypedProperty<fbxReference> ConstrainedObject;*/
			//@}
		public:
		
			/** Retrieve the constraint lock state.
			* \return Current lock flag.
			*/
			/** Set the constraint lock.
			* \param pLock     State of the lock flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,Lock);			

			/** Retrieve the constraint active state.
			* \return Current active flag.
			*/
			/** Set the constraint active.
			* \param pActive State of the active flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,Active);						

			/** Retrieve the constraint X-axe translation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint X-axe translation effectiveness.
			* \param pAffect State of the translation effectivness on the X axe.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectTranslationX);
						
			/** Retrieve the constraint Y-axe translation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Y-axe translation effectiveness.
			* \param pAffect State of the translation effectivness on the Y axe.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectTranslationY);

			/** Retrieve the constraint Z-axe translation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Z-axe translation effectiveness.
			* \param pAffect State of the translation effectivness on the Z axe.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectTranslationZ);			

			/** Retrieve the constraint X-axe rotation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint X-axe rotation effectiveness.
			* \param pAffect State of the rotation effectivness on the X axe.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectRotationX);			
			
			/** Retrieve the constraint Y-axe rotation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Y-axe rotation effectiveness.
			* \param pAffect State of the rotation effectivness on the Y axe.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectRotationY);			

			/** Retrieve the constraint Z-axe rotation effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Z-axe rotation effectiveness.
			* \param pAffect State of the rotation effectivness on the Z axe.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectRotationZ);

			/** Set the translation offset.
			* \param pObject Source object.
			* \param pTranslation New offset vector.
			*/
			void SetTranslationOffset(FbxObjectManaged^ obj, FbxVector4^ translation);

			/** Retrieve the constraint translation offset.
			* \param pObject Object that we want the offset.
			* \return Current translation offset.
			*/
			FbxVector4^ GetTranslationOffset(FbxObjectManaged^ obj);

			/** Set the rotation offset.
			* \param pObject Source object.
			* \param pRotation New offset vector.
			*/
			virtual void SetRotationOffset(FbxObjectManaged^ obj, FbxVector4^ rotation);

			/** Retrieve the constraint rotation offset.
			* \param pObject Object that we want the offset.
			* \return Current translation offset.
			*/
			FbxVector4^ GetRotationOffset(FbxObjectManaged^ obj);

			/** Set the weight of the constraint.
			* \param pWeight New weight value.
			*/
			void SetWeight(double weight);

			/** Get the weight of a source.
			* \param pObject Index of the source.
			*/
			double GetSourceWeight(FbxObjectManaged^ obj);

			/** Add a source to the constraint.
			* \param pObject New source object.
			* \param pWeight Weight of the source object.
			*/
			//default weight = 100
			void AddConstraintSource(FbxObjectManaged^ obj, double weight);

			/** Retrieve the constraint source count.
			* \return Current constraint source count.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ConstraintSourceCount);

			/** Retrieve a constraint source object.
			* \param pIndex Index of the source
			* \return Current source at the specified index.
			*/
			FbxObjectManaged^ GetConstraintSource(int index);						

			/** Retrieve the constrainted object.
			* \return Current constrained object.
			*/
			/** Set the constrainted object.
			* \param pObject The constrained object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxObjectManaged,ConstrainedObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}