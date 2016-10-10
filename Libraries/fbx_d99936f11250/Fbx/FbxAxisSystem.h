#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		ref class FbxSceneManaged;
		ref class FbxNode;
		/** \brief This class represents the coordinate system of the scene, and can convert scenes from
		its coordinate system to other coordinate systems.
		* \nosubgrouping
		*/
		public ref class FbxAxisSystem : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxAxisSystem,KFbxAxisSystem);
			INATIVEPOINTER_DECLARE(FbxAxisSystem,KFbxAxisSystem);		
		internal:
			FbxAxisSystem(KFbxAxisSystem a)
			{
				_SetPointer(new KFbxAxisSystem(a),true);
			}
		public:
			/** \enum eUpVector Specifies which canonical axis represents up in the system. Typically Y or Z. 
			* - \e XAxis
			* - \e YAxis
			* - \e ZAxis
			*/
			enum class UpVector {
				XAxis =	1,
				YAxis =	2,
				ZAxis =	3        
			};

			/* \enum eFrontVector. Vector with origin at the screen pointing toward the camera.
			* This is a subset of enum eUpVector because axis cannot be repeated.
			* - \e ParityEven
			* - \e ParityOdd
			*/
			enum class FrontVector {
				ParityEven = 1,
				ParityOdd  = 2
			};

			/* \enum eCoorSystem.
			* - \e RightHanded
			* - \e LeftHanded
			*/
			enum class CoordinateSystem {
				RightHanded = 0,
				LeftHanded  = 1
			};

			/* \enum ePreDefinedAxisSystem.
			* - \e eMayaZUp
			* - \e eMayaYUp
			* - \e eMax
			* - \e eMotionBuilder
			* - \e eOpenGL
			* - \e eDirectX
			* - \e eLightwave
			*/
			enum class PreDefinedAxisSystem {
				MayaZUp = 0,
				MayaYUp,
				Max,
				MotionBuilder,
				OpenGL,
				DirectX,
				Lightwave
			};

			FbxAxisSystem(UpVector upVector, FrontVector frontVector, CoordinateSystem coorSystem);
			FbxAxisSystem(FbxAxisSystem^ axisSystem);
			FbxAxisSystem(PreDefinedAxisSystem axisSystem);			

			virtual bool Equals(System::Object^ obj)override
			{
				FbxAxisSystem^ o = dynamic_cast<FbxAxisSystem^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}
			void CopyFrom(FbxAxisSystem^ axisSystem);

			/*static const KFbxAxisSystem MayaZUp;
			static const KFbxAxisSystem MayaYUp;
			static const KFbxAxisSystem Max;
			static const KFbxAxisSystem Motionbuilder;
			static const KFbxAxisSystem OpenGL;
			static const KFbxAxisSystem DirectX;
			static const KFbxAxisSystem Lightwave;*/

			/** Convert a scene to this axis system. Sets the axis system of the
			* scene to this system unit. 
			* \param pScene     The scene to convert
			*/
			void ConvertScene(FbxSceneManaged^ scene);

			/** Convert a scene to this axis system by using the specified
			* node as an Fbx_Root. This is provided for backwards compatibility
			* only and ConvertScene(KFbxScene* pScene) should be used instead
			* when possible.
			* \param pScene       The scene to convert
			* \param pFbxRoot     The Fbx_Root node that will be transformed.
			*/
			void ConvertScene(FbxSceneManaged^ scene, FbxNode^ fbxRoot);

			/** Returns the eUpVector this axis system and get the sign of the axis.
			* \param pSign     The sign of the axis, 1 if up, -1 is down.
			*/
			UpVector GetUpVector(int %sign);

			/** Returns the eCoorSystem this axis system.
			*/
			property CoordinateSystem Coordinate_System
			{
				CoordinateSystem get();
			}

			/** Converts the children of the given node to this axis system.
			* Unlike the ConvertScene() method, this method does not set the axis system 
			* of the scene that the pRoot node belongs, nor does it adjust KFbxPoses
			* as they are not stored under the scene, and not under a particular node.
			*/
			void ConvertChildren(FbxNode^ root, FbxAxisSystem^ srcSystem);			
		};		
	}
}