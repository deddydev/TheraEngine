#include "VHACDInterface.h"

IVHACD* CreateClassName()
{
	return CreateVHACD();
}

void DisposeClassName(IVHACD* a_pObject)
{
	if (a_pObject != NULL)
	{
		delete a_pObject;
		a_pObject = NULL;
	}
}

void function(CClassName* a_pObject)
{
	if (a_pObject != NULL)
	{
		a_pObject->function();
	}
}