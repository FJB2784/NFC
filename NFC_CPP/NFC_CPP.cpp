
#include "pch.h"
#include <iostream>
#include <Windows.h>
#include "felica.h"
#include "felica_basic_lite_s.h"
#include "felica_error.h"
#include "rw_error.h"

#pragma comment(lib, "felica.lib")

int main()
{
	if (!initialize_library())
	{
		printf("Failed\ninitialize_library()\n");
		return 0;
	}

	if (!open_reader_writer_auto())
	{
		printf("Failed\nopen_reader_writer_auto()\n");
		return 0;
	}

	structure_device_information *dev = new structure_device_information();

	if (!get_device_information(dev))
	{
		printf("Failed\nget_device_information()\n");
		return 0;
	}
	else
	{
		printf("Success\n");
		
		if (!dev->device_info_connect)
		{
			printf("内蔵のNFCポートです。\n");
		}
		else
		{
			printf("外付けのNFCポートです。\n");
		}

		
	}
}
