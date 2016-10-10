#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"



{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxVector4;

		/** \brief This constraint class contains methods for accessing the properties of a single chain IK constraint.
		* \nosubgrouping
		*/
		public ref class FbxConstraintSingleChainIK : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxConstraintSingleChainIK);
		internal:
			FbxConstraintSingleChainIK(KFbxConstraintSingleChainIK* instance) : FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxConstraintSingleChainIK);
			//
			//			/**
			//			* \name Properties
			//			*/
			//			//@{
			//			KFbxTypedProperty<fbxBool1>		Lock;
			//			KFbxTypedProperty<fbxBool1>		Active;
			//
			//			KFbxTypedProperty<fbxEnum>		PoleVectorType;
			//			KFbxTypedProperty<fbxEnum>		SolverType;
			//			KFbxTypedProperty<fbxEnum>		EvaluateTSAnim;
			//
			//			KFbxTypedProperty<fbxDouble1>	Weight;
			//			//KFbxTypedProperty<fbxReference> PoleVectorObjectWeights;
			//			KFbxTypedProperty<fbxReference>	PoleVectorObjects;
			//			KFbxTypedProperty<fbxDouble3>	PoleVector;
			//			KFbxTypedProperty<fbxDouble1>	Twist;
			//
			//			KFbxTypedProperty<fbxReference> FirstJointObject;
			//			KFbxTypedProperty<fbxReference> EndJointObject;
			//			KFbxTypedProperty<fbxReference> EffectorObject;
			//			//@}
			//
		public:
			/** \enum ESolverType Pole vector type.
			* - \e eRP_SOLVER
			* - \e eSC_SOLVER
			*/
			enum class SolverType
			{
				RP = KFbxConstraintSingleChainIK::eRP_SOLVER,
				SC = KFbxConstraintSingleChainIK::eSC_SOLVER
			};

			/** \enum EPoleVectorType Pole vector type.
			* - \e ePOLE_VECTOR_TYPE_VECTOR
			* - \e ePOLE_VECTOR_TYPE_OBJECT
			*/
			enum class PoleVectorType
			{
				Vector = KFbxConstraintSingleChainIK::ePOLE_VECTOR_TYPE_VECTOR,
				Object = KFbxConstraintSingleChainIK::ePOLE_VECTOR_TYPE_OBJECT
			};

			/** \enum EEvalTS If the constaints read its animation on Translation and Scale for the nodes it constraints.
			* - \e eEVALTS_NEVER
			* - \e eEVALTS_AUTODETECT
			* = \e eEVALTS_ALWAYS
			*/
			enum class EvalTS
			{
				Never = KFbxConstraintSingleChainIK::eEVAL_TS_NEVER,
				AutoDetect = KFbxConstraintSingleChainIK::eEVAL_TS_AUTO_DETECT,
				Always = KFbxConstraintSingleChainIK::eEVAL_TS_ALWAYS
			};

			/** Retrieve the constraint lock state.
			* \return     Current lock flag.
			*/
			/** Set the constraint lock.
			* \param pLock     State of the lock flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,Lock);			

			/** Retrieve the constraint active state.
			* \return     Current active flag.
			*/
			/** Set the constraint active.
			* \param pActive     State of the active flag.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(bool,Active);						

			/** Retrieve the pole vector type.
			* \return     Current pole vector type.
			*/
			/** Set the Pole Vector type.
			* \param pType     New type for the pole vector.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(PoleVectorType,PoleVector_Type);
			
			/** Retrieve the solver type.
			* \return     Current solver type.
			*/
			/** Set the Solver type.
			* \param pType     New type for the solver.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(SolverType,Solver_Type);
			
			/** Retrieve the EvalTS
			* \return     The current EvalTS type
			*/
			/** Sets the EvalTS
			* \param pEval     New type of EvalTS 
			*/
			VALUE_PROPERTY_GETSET_DECLARE(EvalTS,Eval_TS);

			/** Get the weight of the constraint.
			* \return     The current weight value.
			*/
			/** Set the weight of the constraint.
			* \param pWeight     New weight value.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,Weight);			

			/** Get the weight of a source.
			* \param pObject     Source object that we want the weight.
			*/
			double GetPoleVectorObjectWeight(FbxObjectManaged^ obj);

			/** Add a source to the constraint.
			* \param pObject     New source object.
			* \param pWeight     Weight value of the source object expressed as a percentage.
			* \remarks           pWeight value is 100 percent by default.
			*/
			//default weight = 100 
			void AddPoleVectorObject(FbxObjectManaged^ obj, double weight);

			/** Retrieve the constraint source count.
			* \return     Current constraint source count.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ConstraintPoleVectorCount);

			/** Retrieve a constraint source object.
			* \param pIndex     Index of constraint source object.
			* \return           Current source at the specified index.
			*/
			FbxObjectManaged^ GetPoleVectorObject(int index);
			
			/** Retrieve the pole vector.
			* \return     Current pole vector.
			*/
			/** Set the pole vector.
			* \param pVector     New pole vector.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,PoleVector);
			
			/** Retrieve the twist value.
			* \return     Current twist value.
			*/
			/** Set the twist value.
			* \param pTwist    New twist value.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,Twist);

			/** Retrieve the first joint object.
			* \return Current first joint object.
			*/
			/** Set the first joint object.
			* \param pObject     The first joint object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxObjectManaged,FirstJointObject);						

			/** Retrieve the end joint object.
			* \return     Current end joint object.
			*/
			/** Set the end joint object.
			* \param pObject     The end joint object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxObjectManaged,EndJointObject);
						
			/** Retrieve the effector object.
			* \return     Current effector object.
			*/
			/** Set the effector object.
			* \param pObject     The effector object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxObjectManaged,EffectorObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}