#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"


namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxDouble3TypedProperty;
		ref class FbxDouble1TypedProperty;
		ref class FbxVector4;

		public ref class FbxConstraintPosition : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxConstraintPosition);
		internal:
			FbxConstraintPosition(KFbxConstraintPosition* instance) : FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxConstraintPosition);

			/**
			* \name Properties
			*/
			//@{
			/*VALUE_PROPERTY_GETSET_DECLARE(bool,Lock);
			VALUE_PROPERTY_GETSET_DECLARE(bool,Active);

			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,AffectZ);

			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Translation);
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,Weight);*/
			//KFbxTypedProperty<fbxReference>	SourceWeights;

			//KFbxTypedProperty<fbxReference> ConstraintSources;
			//KFbxTypedProperty<fbxReference> ConstrainedObject;
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

			/** Retrieve the constraint translation offset.
			* \return Current translation offset.
			*/
			/** Set the translation offset.
			* \param pTranslation New offset vector.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,Offset);

			/** Set the weight of the constraint.
			* \param pWeight New weight value.
			*/
			void SetWeight(double weight);

			/** Get the weight of a source.
			* \param pObject Object that we want the weight.
			*/
			double GetSourceWeight(FbxObjectManaged^ obj);

			/** Add a source to the constraint.
			* \param pObject New source object.
			* \param pWeight Weight of the source object.
			*/
			//default weight= 100
			void AddConstraintSource(FbxObjectManaged^ obj, double weight);

			/** Remove a source from the constraint.
			* \param pObject Source object to remove.
			*/
			bool RemoveConstraintSource(FbxObjectManaged^ obj);

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