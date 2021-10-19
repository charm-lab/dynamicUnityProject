//
// Sample.cpp : Defines the entry point for the console application.
//
#include <winsock2.h>
#include <WS2tcpip.h>
#include "stdafx.h"
#include <iostream>
#include <stdio.h>

#pragma comment(lib, "Ws2_32.lib")

//
// Forward declaration of error handler
// ====================================
//
// A very simple error handler is provided at the end of this file
// It simply takes error codes and converts them to message strings
// then outputs them to the console.
//
void errorHandler(int error);

//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//
//	Main program:
//
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
int main()
{
	//TracStar Start
	CSystem			ATC3DG;
	CSensor			*pSensor;
	CXmtr			*pXmtr;
	int				errorCode;
	int				i;
	int				sensorID;
	short			id;
	bool			recording = true;
	double			doubleID;
	char			output[256];
	double			outputnum[6];
	int				numberBytes;
	clock_t			goal;
	clock_t			wait=10;	// 10 ms delay

	//UDP Start
	WSADATA wsaData;
	SOCKET UnitySocket;
	sockaddr_in RecvAddr;
	int Port = 26950;   //port number
	char SendBuf[32] = "From port 26950";
	int BufLen = 32;
	char IP_ADDRESS_S[] = "127.0.0.1";    //IP Addresss
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	UnitySocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	RecvAddr.sin_family = AF_INET;
	RecvAddr.sin_port = htons(Port);
	InetPton(AF_INET, IP_ADDRESS_S, &RecvAddr.sin_addr.s_addr);

	printf("\n\n");
	printf("ATC3DG Sample Application\n");

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// Initialize the ATC3DG driver and DLL
	//
	// It is always necessary to first initialize the ATC3DG "system". By
	// "system" we mean the set of ATC3DG cards installed in the PC. All cards
	// will be initialized by a single call to InitializeBIRDSystem(). This
	// call will first invoke a hardware reset of each board. If at any time 
	// during operation of the system an unrecoverable error occurs then the 
	// first course of action should be to attempt to Recall InitializeBIRDSystem()
	// if this doesn't restore normal operating conditions there is probably a
	// permanent failure - contact tech support.
	// A call to InitializeBIRDSystem() does not return any information.
	//
	printf("Initializing ATC3DG system...\n");
	errorCode = InitializeBIRDSystem();
	if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// GET SYSTEM CONFIGURATION
	//
	// In order to get information about the system we have to make a call to
	// GetBIRDSystemConfiguration(). This call will fill a fixed size structure
	// containing amongst other things the number of boards detected and the
	// number of sensors and transmitters the system can support (Note: This
	// does not mean that all sensors and transmitters that can be supported
	// are physically attached)
	//
	errorCode = GetBIRDSystemConfiguration(&ATC3DG.m_config);
	if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// GET SENSOR CONFIGURATION
	//
	// Having determined how many sensors can be supported we can dynamically
	// allocate storage for the information about each sensor.
	// This information is acquired through a call to GetSensorConfiguration()
	// This call will fill a fixed size structure containing amongst other things
	// a status which indicates whether a physical sensor is attached to this
	// sensor port or not.
	//
	pSensor = new CSensor[ATC3DG.m_config.numberSensors];
	for(i=0;i<ATC3DG.m_config.numberSensors;i++)
	{
		errorCode = GetSensorConfiguration(i, &(pSensor+i)->m_config);
		if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);
	}

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// GET TRANSMITTER CONFIGURATION
	//
	// The call to GetTransmitterConfiguration() performs a similar task to the 
	// GetSensorConfiguration() call. It also returns a status in the filled
	// structure which indicates whether a transmitter is attached to this
	// port or not. In a single transmitter system it is only necessary to 
	// find where that transmitter is in order to turn it on and use it.
	//
	pXmtr = new CXmtr[ATC3DG.m_config.numberTransmitters];
	for(i=0;i<ATC3DG.m_config.numberTransmitters;i++)
	{
		errorCode = GetTransmitterConfiguration(i, &(pXmtr+i)->m_config);
		if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);
	}

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// Search for the first attached transmitter and turn it on
	//
	for(id=0;id<ATC3DG.m_config.numberTransmitters;id++)
	{
		if((pXmtr+id)->m_config.attached)
		{
			// Transmitter selection is a system function.
			// Using the SELECT_TRANSMITTER parameter we send the id of the
			// transmitter that we want to run with the SetSystemParameter() call
			errorCode = SetSystemParameter(SELECT_TRANSMITTER, &id, sizeof(id));
			if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);
			break;
		}
	}

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// Collect data from all birds
	// Loop through all sensors and get a data record if the sensor is attached.
	// Print result to screen
	// Note: The default data format is DOUBLE_POSITION_ANGLES. We can use this
	// format without first setting it.
	// 
	//
	DOUBLE_POSITION_ANGLES_RECORD record, *pRecord = &record;

	// Set up time delay for first loop
	// It only makes sense to request a data record from the sensor once per
	// measurement cycle. Therefore we set up a 10ms loop and request a record
	// only after at least 10ms have elapsed.
	//
	goal = wait + clock();

	// collect as many records as specified in the command line
	//for(i=0;i<records;i++)
	while(recording)
	{
		// delay 10ms between collecting data
		// wait till time delay expires
		while(goal>clock());
		// set up time delay for next loop
		goal = wait + clock();

		// scan the sensors and request a record
		for(sensorID=0;sensorID<ATC3DG.m_config.numberSensors;sensorID++)
		{
			// sensor attached so get record
			errorCode = GetAsynchronousRecord(sensorID, pRecord, sizeof(record));
			if(errorCode!=BIRD_ERROR_SUCCESS) {errorHandler(errorCode);}

			// get the status of the last data record
			// only report the data if everything is okay
			unsigned int status = GetSensorStatus( sensorID);

			if( status == VALID_STATUS)
			{
				// send output to console
				sprintf(output, "[%d] %8.3f %8.3f %8.3f: %8.2f %8.2f %8.2f\n",
					sensorID,
					record.x,
					record.y,
					record.z,
					record.a,
					record.e,
					record.r
					);
				doubleID = sensorID;
				outputnum[0] = { doubleID};
				outputnum[1] = { record.x };
				outputnum[2] = { record.y };
				outputnum[3] = { record.z };
				outputnum[4] = { record.a };
				outputnum[5] = { record.e };
				outputnum[6] = { record.r };
				numberBytes = strlen(output);
				printf("%s", output);// ng a datagram to the receiver...");
				sendto(UnitySocket, output, numberBytes, 0, (SOCKADDR *)&RecvAddr, sizeof(RecvAddr));
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//
	// Turn off the transmitter before exiting
	// We turn off the transmitter by "selecting" a transmitter with an id of "-1"
	//
	id = -1;
	errorCode = SetSystemParameter(SELECT_TRANSMITTER, &id, sizeof(id));
	if(errorCode!=BIRD_ERROR_SUCCESS) errorHandler(errorCode);

	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////
	//

	//Close UDP Socket
	printf("Finished sending. Closing socket.");
	closesocket(UnitySocket);
	printf("Exiting.");
	WSACleanup();

	//  Free memory allocations before exiting
	delete[] pSensor;
	delete[] pXmtr;

	return 0;
}


//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//
//	ERROR HANDLER
//	=============
//
// This is a simplified error handler.
// This error handler takes the error code and passes it to the GetErrorText()
// procedure along with a buffer to place an error message string.
// This error message string can then be output to a user display device
// like the console
// Specific error codes should be parsed depending on the application.
//
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
void errorHandler(int error)
{
	char			buffer[1024];
	char			*pBuffer = &buffer[0];
	int				numberBytes;

	while(error!=BIRD_ERROR_SUCCESS)
	{
		error = GetErrorText(error, pBuffer, sizeof(buffer), SIMPLE_MESSAGE);
		numberBytes = strlen(buffer);
		buffer[numberBytes] = '\n';		// append a newline to buffer
		printf("%s", buffer);
	}
	exit(0);
}


