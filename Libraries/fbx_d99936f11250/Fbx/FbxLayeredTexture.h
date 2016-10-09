#pragma once
#include "stdafx.h"
#include "FbxTexture.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/**FBX SDK layered texture class
		* \nosubgrouping
		*/
		public ref class FbxLayeredTexture : FbxTexture
		{
			REF_DECLARE(FbxEmitter,KFbxLayeredTexture);
		internal:
			FbxLayeredTexture(KFbxLayeredTexture* instance): FbxTexture(instance)
			{
				_Free = false;
			}
		public:

			FBXOBJECT_DECLARE(FbxLayeredTexture);

			//KFbxTypedProperty<EBlendMode> BlendMode;

			/** Equality operator
			* \param pOther                      The object to compare to.
			* \return                            \c true if pOther is equivalent to this object,\c false otherwise.
			*/
			virtual bool Equals(System::Object^ obj)override
			{
				FbxLayeredTexture^ l = dynamic_cast<FbxLayeredTexture^>(obj);
				if(l)
					return *_Ref() == *l->_Ref();
				return false;
			}

			/** Set the blending mode for a texture
			* \param pIndex                      The texture index.
			* \param pMode                       The blend mode to set.
			* \return                            \c true on success, \c false otherwise.
			*/
			/** Get the blending mode for a texture
			* \param pIndex                      The texture index.
			* \param pMode                       The blend mode is returned here.
			* \return                            \c true on success,\c false otherwise.
			*/
			property BlendMode default[int]
			{
				BlendMode get(int index);
				void set(int index , BlendMode value);
			}						
		};

	}
}