#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"



{
	namespace FbxSDK
	{		
		ref class FbxObjectManaged;
		ref class FbxVector4;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** \brief This constraint class contains methods for accessing the properties of an aim constraint.
		* A aim constraint lets you constrain an object based on the properties of one or more aim objects.
		* \nosubgrouping
		*/
		public ref class FbxConstraintAim : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxConstraintAim);
		internal:
			FbxConstraintAim(KFbxConstraintAim* instance) : FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxConstraintAim);
			//
			//			/**
			//			* \name Properties
			//			*/
			//			//@{
			//			KFbxTypedProperty<fbxBool1>		Lock;
			//			KFbxTypedProperty<fbxBool1>		Active;
			//
			//			KFbxTypedProperty<fbxDouble1>	Weight;
			//			KFbxTypedProperty<fbxDouble3>	RotationOffset;
			//
			//			KFbxTypedProperty<fbxReference> AimAtObjects;
			//			KFbxTypedProperty<fbxReference> ConstrainedObject;
			//
			//			KFbxTypedProperty<fbxEnum>		WorldUpType;
			//			KFbxTypedProperty<fbxReference> WorldUpObject;
			//			KFbxTypedProperty<fbxDouble3>	WorldUpVector;
			//			KFbxTypedProperty<fbxDouble3>	UpVector;
			//			KFbxTypedProperty<fbxDouble3>	AimVector;
			//
			//			KFbxTypedProperty<fbxBool1>		AffectX;
			//			KFbxTypedProperty<fbxBool1>		AffectY;
			//			KFbxTypedProperty<fbxBool1>		AffectZ;
			//			//@}
			//
			//
		public:
			enum class AimConstraintWoldUpType
			{
				AtSceneUp = KFbxConstraintAim::eAimAtSceneUp,
				AtObjectUp = KFbxConstraintAim::eAimAtObjectUp,
				AtObjectRotationUp = KFbxConstraintAim::eAimAtObjectRotationUp,
				AtVector = KFbxConstraintAim::eAimAtVector,
				AtNone = KFbxConstraintAim::eAimAtNone,
				AtCount = KFbxConstraintAim::eAimAtCount
			};				

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
			* \param pObject Source object.
			*/
			double GetSourceWeight(FbxObjectManaged^ obj);

			/** Add a source to the constraint.
			* \param pObject New source object.
			* \param pWeight Weight of the source object.
			*/
			void AddConstraintSource(FbxObjectManaged^ obj, double weight);

			/** Retrieve the constraint source count.
			* \return Current constraint source count.
			*/
			property int ConstraintSourceCount
			{
				int get();
			}

			/** Retrieve a constraint source object.
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

			/** Retrieve the world up type.
			* \return Current world up type.
			*/
			/** Set the world up type.
			* \param pType The world up type.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(AimConstraintWoldUpType,WorldUpType);								

			/** Retrieve the world up object.
			* \return Current world up object.
			*/
			/** Set the world up object.
			* \param pObject The world up object.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxObjectManaged,WorldUpObject);
			
			/** Retrieve the world up vector.
			* \return Current world up vector.
			*/
			/** Set the world up vector.
			* \param pVector The world up vector.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,WorldUpVector);
			
			/** Retrieve the up vector.
			* \return Current up vector.
			*/			
			/** Set the up vector.
			* \param pVector The up vector.
			*/						
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,UpVector);

			/** Retrieve the aim vector.
			* \return Current up vector.
			*/			
			/** Set the aim vector.
			* \param pVector The up vector.
			*/						
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,AimVector);

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

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}