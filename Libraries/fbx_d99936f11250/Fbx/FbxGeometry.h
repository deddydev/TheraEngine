#pragma once
#include "stdafx.h"
#include "FbxGeometryBase.h"
#include "FbxDeformer.h"


{
	namespace FbxSDK
	{		
		ref class FbxGeometryWeightedMap;
		ref class FbxShape;
		ref class FbxCurve;
		ref class FbxXMatrix;
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		/** Contains common properties for mesh, nurb, and patch node attributes.
		* \nosubgrouping
		* A geometry node attribute has arrays of links, shapes, materials and
		* textures. It also has arrays for control points, normals, material indices,
		* texture indices, and texture UV coordinates. Some of these are only used
		* in mesh node attributes.
		*/
		public ref class FbxGeometry : FbxGeometryBase
		{
			REF_DECLARE(FbxEmitter,KFbxGeometry);
		internal:
			FbxGeometry(KFbxGeometry* instance) : FbxGeometryBase(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory() override;
		public:			
			FBXOBJECT_DECLARE(FbxGeometry);			
			/** Return the type of node attribute.
			* This class is pure virtual.
			*/
			//virtual AttributeType GetAttributeType() const;

			/**
			* \name Deformer Management
			*/
			//@{			

			/** Add a deformer.
			* \param pDeformer     Pointer to the deformer object to add.
			* \return              Index of added deformer.
			*/
			int AddDeformer(FbxDeformer^ deformer);

			/** Get the number of deformers.
			* \return     Number of deformers that have been added to this object.
			*/
			property int DeformerCount
			{
				int get();
			}

			/** Get deformer at given index.
			* \param pIndex     Index of deformer.
			* \return           Pointer to deformer or \c NULL if pIndex is out of range. In this case,
			*                   KFbxGeometry::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			FbxDeformer^ GetDeformer(int index);

			/** Get the number of deformers of a given type.
			* \param pType     Type of deformer to count
			* \return          Number of deformers that have been added to this object.
			*/
			int GetDeformerCount(FbxDeformer::DeformerType type);

			/** Get deformer of a gieven type at given index.
			* \param pIndex     Index of deformer.
			* \param pType      Type of deformer.
			* \return           Pointer to deformer or \c NULL if pIndex is out of range. In this case,
			*                   KFbxGeometry::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			FbxDeformer^ GetDeformer(int index, FbxDeformer::DeformerType type);

			//@}

			/**
			* \name Connected Geometry Weighted Map(s) Management
			*/
			//@{

			/** Return the source geometry weighted map connected.
			* \return     Pointer to the source geometry weighted map connected to this object if any.
			*/
			REF_PROPERTY_GET_DECLARE(FbxGeometryWeightedMap,SourceGeometryWeightedMap);

			/** Get the number of destination geometry weighted map(s) connected.
			* \return     Number of destination geometry weighted map(s) connected to this object.
			*/
			property int DestinationGeometryWeightedMapCount
			{
				int get();
			}

			/** Get destination geometry weighted map at a given index.
			* \param pIndex     Index of link.
			* \return           Pointer to the destination geometry weighted map connected to this object if any.
			*/
			FbxGeometryWeightedMap^ GetDestinationGeometryWeightedMap(int index);

			//@}

			/**
			* \name Shape Management
			*/
			//@{

			/** Add a shape and its associated name.
			* \param pShape         Pointer to the shape object.
			* \param pShapeName     Name given to the shape.
			* \return               Index of added shape, -1 if operation failed.
			*                       If the operation fails, KFbxGeometry::GetLastErrorID() can return one of the following:
			*                            - eNULL_PARAMETER: Pointer to shape is \c NULL.
			*                            - eSHAPE_ALREADY_ADDED: Shape has already been added.
			*                            - eSHAPE_INVALID_NAME: The provided name is empty.
			*                            - eSHAPE_NAME_CLASH: The provided name is already used by another shape.
			* \remarks             The provided name is stripped from surrounding whitespaces before being
			*                      compared with other shape names. It is recommended not to prefix the shape name with its
			*                      enclosing node name because MotionBuilder is known to strip this prefix and not save it back.
			*/
			virtual int AddShape(FbxShape^ shape, String^ shapeName);

			/** Removes all shapes without destroying them.
			* If shapes aren't explicitly destroyed before calling this function, they will be
			* destroyed along with the SDK manager.
			*/
			virtual void ClearShape();

			/** Get the number of shapes.
			* \return     Number of shapes that have been added to this object.
			*/
			virtual property int ShapeCount
			{
				int get();
			}

			/** Get shape at given index.
			* \param pIndex     Index of shape.
			* \return           Pointer to shape or \c NULL if pIndex is out of range. In this case,
			*                   KFbxGeometry::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			virtual FbxShape^ GetShape(int index);

			/** Get shape at given index.
			* \param pIndex     Index of shape.
			* \return           Pointer to shape or \c NULL if pIndex is out of range. In this case,
			*                   KFbxGeometry::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			//virtual FbxShape const* GetShape(int pIndex) const;

			/** Get shape name at given index.
			* \param pIndex     Index of shape.
			* \return           Shape name or \c NULL if pIndex is out of range. In this case,
			*                   KFbxGeometry::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			virtual String^ GetShapeName(int index);

			/** Get a shape channel.
			* The shape channel property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pShapeName      Shape Property name.
			* \param pCreateAsNeeded If true, the fcurve is created if not already present.
			* \param pTakeName       Take from which we want the FCurve (if NULL, use the current take).
			* \return                Animation curve or NULL if an error occurred. In this case,
			*                        KFbxGeometry::GetLastErrorID() returns one of the following:
			*                             - eINDEX_OUT_OF_RANGE: Shape index is out of range.
			*                             - eSHAPE_NO_CURVE_FOUND: Shape curve could not be found.
			*/
			virtual FbxCurve^ GetShapeChannel(String^ shapeName, bool createAsNeeded, String^ takeName);
			virtual FbxCurve^ GetShapeChannel(String^ shapeName, bool createAsNeeded);

			/** Get a shape channel.
			* The shape channel property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pIndex          Shape index.
			* \param pCreateAsNeeded If true, the fcurve is created if not already present.
			* \param pTakeName       Take from which we want the FCurve (if NULL, use the current take).
			* \return                Animation curve or NULL if an error occurred. In this case,
			*                        KFbxGeometry::GetLastErrorID() returns one of the following:
			*                             - eINDEX_OUT_OF_RANGE: Shape index is out of range.
			*                             - eSHAPE_NO_CURVE_FOUND: Shape curve could not be found.
			*/
			virtual FbxCurve^ GetShapeChannel(int index, bool createAsNeeded ,String^ takeName);


			//@}

			/** Surface modes
			* This information is only used in nurbs and patches.
			*/

			/** \enum ESurfaceMode Types of surfaces.
			* - \e eRAW
			* - \e eLOW_NO_NORMALS
			* - \e eLOW
			* - \e eHIGH_NO_NORMALS
			* - \e eHIGH
			*/
			enum class SurfaceMode
			{
				Raw,
				LowNoNormals,
				Low,
				HighNoNormals,
				High
			} ;

			/**
			* \name Pivot Management
			* The geometry pivot is used to specify additional translation, rotation,
			* and scaling applied to all the control points when the model is
			* exported.
			*/
			//@{

			/** Get pivot matrix.
			* \param pXMatrix     Placeholder for the returned matrix.
			* \return             Reference to the passed argument.
			*/
			FbxXMatrix^ GetPivot(FbxXMatrix^ matrix);

			/** Set pivot matrix.
			* \param pXMatrix     The Transformation matrix.
			*/
			void SetPivot(FbxXMatrix^ matrix);

			/** Apply the pivot matrix to all vertices/normals of the geometry.
			*/
			void ApplyPivot();

			//@}

			/**
			* \name Default Animation Values
			* These functions provides direct access to default
			* animation values specific to a geometry.
			* These functions only work if the geometry has been associated
			* with a node.
			*/
			//@{

			/** Set default deformation for a given shape.
			* The default shape property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pIndex       Shape index.
			* \param pPercent     Deformation percentage on a scale ranging from 0 to 100.
			* \remarks            This function has no effect if pIndex is out of range.
			*/
			void SetDefaultShape(int index, double percent);
			/** Set default deformation for a given shape.
			* The default shape property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pShapeName   Shape name.
			* \param pPercent     Deformation percentage on a scale ranging from 0 to 100.
			* \remarks            This function has no effect if pShapeName is invalid.
			*/
			void SetDefaultShape(String^ shapeName, double percent);

			/** Get default deformation for a given shape.
			* The default shape property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pIndex     Shape index.
			* \return           The deformation value for the given shape, or 0 if pIndex is out of range.
			*/
			double GetDefaultShape(int index);
			/** Get default deformation for a given shape.
			* The default shape property has a scale from 0 to 100, 100 meaning full shape deformation.
			* The default value is 0.
			* \param pShapeName     Shape name.
			* \return               The deformation value for the given shape, or 0 if pShapeName is invalid.
			*/
			double GetDefaultShape(String^ shapeName);

			//@}

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			*  \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eINDEX_OUT_OF_RANGE
			* - \e eNULL_PARAMETER
			* - \e eMATERIAL_NOT_FOUND
			* - \e eMATERIAL_ALREADY_ADDED
			* - \e eTEXTURE_NOT_FOUND
			* - \e eTEXTURE_ALREADY_ADDED
			* - \e eSHAPE_ALREADY_ADDED
			* - \e eSHAPE_INVALID_NAME
			* - \e eSHAPE_NAME_CLASH
			* - \e eSHAPE_NO_CURVE_FOUND
			* - \e eUNKNOWN_ERROR
			*/
			enum class Error
			{
				IndexOutOfRange =KFbxGeometry::eINDEX_OUT_OF_RANGE,
				NullParameter =KFbxGeometry::eNULL_PARAMETER ,
				MaterialNotFound =KFbxGeometry::eMATERIAL_NOT_FOUND,
				MaterialAlreadyAdded =KFbxGeometry::eMATERIAL_ALREADY_ADDED,
				TextureNotFound =KFbxGeometry::eTEXTURE_NOT_FOUND,
				TextureAlreadyAdded =KFbxGeometry::eTEXTURE_ALREADY_ADDED,
				ShapeAlreadyAdded =KFbxGeometry::eSHAPE_ALREADY_ADDED,
				ShapeInvalidName =KFbxGeometry::eSHAPE_INVALID_NAME,
				ShapeNameClash =KFbxGeometry::eSHAPE_NAME_CLASH,
				ShapeNoCurveFound =KFbxGeometry::eSHAPE_NO_CURVE_FOUND,
				UnknownError =KFbxGeometry::eUNKNOWN_ERROR,
				ErrorCount =KFbxGeometry::eERROR_COUNT
			};

			/** Get last error code.
			*  \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			*  \return     Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}

			//@}

			CLONE_DECLARE();

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS							

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}