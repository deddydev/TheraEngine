#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxVector4;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** \brief This constraint class contains methods for accessing the properties of a rotation constraint.
		* A rotation constraint lets you constrain the rotation of an object based on the rotation of one or more sources.
		* \nosubgrouping
		*/
		public ref class FbxConstraintRotation : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxConstraintRotation);
		internal:
			FbxConstraintRotation(KFbxConstraintRotation* instance) : FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxConstraintRotation);

			/**
			* \name Properties
			*/
			//@{
			/*KFbxTypedProperty<fbxBool1>		Lock;
			KFbxTypedProperty<fbxBool1>		Active;

			KFbxTypedProperty<fbxBool1>		AffectX;
			KFbxTypedProperty<fbxBool1>		AffectY;
			KFbxTypedProperty<fbxBool1>		AffectZ;

			KFbxTypedProperty<fbxDouble1>	Weight;
			KFbxTypedProperty<fbxDouble3>	Rotation;
			KFbxTypedProperty<fbxReference>	SourceWeights;

			KFbxTypedProperty<fbxReference> ConstraintSources;
			KFbxTypedProperty<fbxReference> ConstrainedObject;*/


			//@}

		public:						

			/** Retrieve the constraint lock state.
			* \return Current lock flag.
			*/
			/** Set the constraint lock.
			* \param pLock State of the lock flag.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Lock);			

			/** Retrieve the constraint active state.
			* \return Current active flag.
			*/
			/** Set the constraint active.
			* \param pActive State of the active flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,Active);
			
			/** Retrieve the constraint X-axe effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint X-axe effectiveness.
			* \param pAffect State of the effectivness on the X axe.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectX);
		
			/** Retrieve the constraint Y-axe effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Y-axe effectiveness.
			* \param pAffect State of the effectivness on the X axe.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectY);
			
			/** Retrieve the constraint Z-axe effectiveness.
			* \return Current state flag.
			*/
			/** Set the constraint Z-axe effectiveness.
			* \param pAffect State of the effectivness on the X axe.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectZ);

			/** Set the weight of the constraint.
			* \param pWeight New weight value.
			*/
			void SetWeight(double weight);
			
			/** Retrieve the constraint rotation offset.
			* \return Current rotation offset.
			*/
			/** Set the rotation offset.
			* \param pRotation New offset vector.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,Offset);

			/** Get the weight of a source.
			* \param pObject Source object
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
			* \param pIndex Index of the source object
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