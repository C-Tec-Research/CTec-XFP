/*******************************************************************************************************************
  DRQ:		108
  PROJECT:	Analogue Fire Alarm control panel (XFP)

  COMPANY:	C-TEC
		    Research and Development division
            Dark Lane
            Mawdesley
            Lancashire
            L40 2QU 


  WRITTEN BY:       Anthony Roberts
  DATE:             .
  REV:              1.00
  SAVED AS:         PC_Comms.c


  Description       This module handles the transfer of data between the 
                    panel and the PC tools application running on a PC.
                    
                    The module also handles transfer of data between
                    remote network panels and the PC

***************************************************************************/


/*********************************** NOTES *********************************


***************************************************************************/
 

/***************************************************************************
*                               INCLUDE FILES                              *
***************************************************************************/


#include "IARConfig.h"
#include <stdio.h>

#include "Memory.h"
#include "Ports.h"
#include "Times.h"
#include "TSwitch.h"
#include "rs232.h"
#include "Events.h"
#include "M16_Flash.h"
#include "TSwitch.h"

#include "Netapp.h"
#include "Netcommon.h"

#define Define_PCComms_Strings
#include "Text Definition.h"

#include <stdlib.h>


#pragma language=extended


/************************************************************************/
/*            PC Serial constants                                       */
/************************************************************************/

#define PCAck 0x06
#define PCNak 0x15


#define PCRemoteData 0x3f

//define the commands
#define PCBaseFunction 0x40
#define PCReqUpload PCBaseFunction+0x01
#define PCRetUpload PCBaseFunction+0x01
#define PCReqDownload PCBaseFunction+0x02
#define PCRetDownload PCBaseFunction+0x02

#define PCReqPointName PCBaseFunction+0x03
#define PCRetPointName PCBaseFunction+0x03
#define PCSetPointName PCBaseFunction+0x04

#define PCReqZoneName  PCBaseFunction+0x05
#define PCRetZoneName  PCBaseFunction+0x05
#define PCSetZoneName  PCBaseFunction+0x06

#define PCReqMaintName  PCBaseFunction+0x07
#define PCRetMaintName  PCBaseFunction+0x07
#define PCSetMaintName  PCBaseFunction+0x08

#define PCReqPData  PCBaseFunction+0x09
#define PCRetPData  PCBaseFunction+0x09
#define PCSetPData  PCBaseFunction+0x0A

#define PCReqMaintDate  PCBaseFunction+0x0B
#define PCRetMaintDate  PCBaseFunction+0x0B
#define PCSetMaintDate  PCBaseFunction+0x0C

#define PCReqZGroup  PCBaseFunction+0x0D
#define PCRetZGroup  PCBaseFunction+0x0D
#define PCSetZGroup  PCBaseFunction+0x0E

#define PCReqAL2Code  PCBaseFunction+0x0F
#define PCRetAL2Code  PCBaseFunction+0x0F
#define PCSetAL2Code  PCBaseFunction+0x10

#define PCReqAL3Code  PCBaseFunction+0x11
#define PCRetAL3Code  PCBaseFunction+0x11
#define PCSetAL3Code  PCBaseFunction+0x12

#define PCReqPhasedSettings  PCBaseFunction+0x13
#define PCRetPhasedSettings  PCBaseFunction+0x13
#define PCSetPhasedSettings  PCBaseFunction+0x14

#define PCSetTimeDate    PCBaseFunction+0x15

#define PCReqLoopType  PCBaseFunction+0x16
#define PCRetLoopType  PCBaseFunction+0x16
#define PCSetLoopType  PCBaseFunction+0x17

#define PCReqFaultLockoutTime  PCBaseFunction+0x18
#define PCRetFaultLockoutTime  PCBaseFunction+0x18
#define PCSetFaultLockoutTime  PCBaseFunction+0x19

#define PCReqZoneTimers  PCBaseFunction+0x1A
#define PCRetZoneTimers  PCBaseFunction+0x1A
#define PCSetZoneTimers  PCBaseFunction+0x1B

#define PCReqMainVersion PCBaseFunction+0x1C
#define PCRetMainVersion PCBaseFunction+0x1C

#define PCReqRepeaterName  PCBaseFunction+0x1D
#define PCRetRepeaterName  PCBaseFunction+0x1D
#define PCSetRepeaterName  PCBaseFunction+0x1E

#define PCReqOutputName  PCBaseFunction+0x1F
#define PCRetOutputName  PCBaseFunction+0x1F
#define PCSetOutputName  PCBaseFunction+0x20

#define PCReqRepeaterFitted  PCBaseFunction+0x21
#define PCRetRepeaterFitted  PCBaseFunction+0x21
#define PCSetRepeaterFitted  PCBaseFunction+0x22

#define PCReqOutputFitted  PCBaseFunction+0x23
#define PCRetOutputFitted  PCBaseFunction+0x23
#define PCSetOutputFitted  PCBaseFunction+0x24

#define PCReqSegmentNo  PCBaseFunction+0x25
#define PCRetSegmentNo  PCBaseFunction+0x25
#define PCSetSegmentNo  PCBaseFunction+0x26

#define PCReqMainName  PCBaseFunction+0x27
#define PCRetMainName  PCBaseFunction+0x27
#define PCSetMainName  PCBaseFunction+0x28

#define PCReqZSet  PCBaseFunction+0x29
#define PCRetZSet  PCBaseFunction+0x29
#define PCSetZSet  PCBaseFunction+0x2A

#define PCReqNonFireFlags  PCBaseFunction+0x2B
#define PCRetNonFireFlags  PCBaseFunction+0x2B
#define PCSetNonFireFlags  PCBaseFunction+0x2C

#define PCReqQuiesName  PCBaseFunction+0x2d
#define PCRetQuiesName  PCBaseFunction+0x2d
#define PCSetQuiesName  PCBaseFunction+0x2e

#define PCReqDayNight  PCBaseFunction+0x2f
#define PCRetDayNight  PCBaseFunction+0x2f
#define PCSetDayNight  PCBaseFunction+0x30

#define PCReqEvent PCBaseFunction+0x31
#define PCRetEvent PCBaseFunction+0x31

#define PCReqC_EEvent PCBaseFunction+0x33
#define PCRetC_EEvent PCBaseFunction+0x33
#define PCSetC_EEvent PCBaseFunction+0x34

#define PCReq_NetPanelData PCBaseFunction+0x35
#define PCRet_NetPanelData PCBaseFunction+0x35
#define PCSet_NetPanelData PCBaseFunction+0x36

#define PCReqTextString PCBaseFunction+0x37
#define PCRetTextString PCBaseFunction+0x37
#define PCSetTextString PCBaseFunction+0x38

/***************************************************************************
*                               VARIABLE DEFINITIONS                       *
***************************************************************************/

#define PCBLength 50

unsigned char * PCTxBuffer;
unsigned char * PCRxBuffer;
unsigned int * I_PCTxBuffer;
unsigned int * I_PCRxBuffer;

unsigned char PCRxPointer;
unsigned char PCRxChecksum;
unsigned char PCRxState;

unsigned char PCTxBufferLen;
unsigned char PCRxBuffLen;
unsigned char PCByteCount;

unsigned char PCRxCmdType;

unsigned char PCRxAddress;
unsigned char PCRxCmd;
unsigned char RxdChecksum;

const unsigned char * M_Ptr;

bit PCRxCmdError;


/***************************************************************************
*                               Function Prototypes                        *
***************************************************************************/

void InterpUDL(void);
void RunUploadFuncs(void);
void RunDownloadFuncs(void);
void ProcessRxData(unsigned char SCDR);
void PCSendAck(void);
void PCSendNak(void);
void SendSignOn(void);
void SendSignOff(void);
void SendPCTxBuffer(void);
void Return_Data_To_PC(unsigned char * S_Ptr);
 
/***************************************************************************
*                                 PROGRAM                                  *
***************************************************************************/


 
/*------------------------------------------------------------------------------------
Function Name   : PC_Manager

Description     : This job is responsible for the communication between the panel and
                  PC. When called, it will suspend some tasks (including loop driver)
                  

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void PC_Manager(void)
{
    unsigned char SST = 10;
    unsigned char D = TRUE;
    unsigned char Init_DataBase = FALSE;    
    unsigned char Dummy;
    unsigned char Init_Sounders = FALSE;
        
    Repeater_ESC_Command = ESC_Comms_Stopped;     //send command to repeaters
    while(Repeater_ESC_Command == ESC_Comms_Stopped) //wait for it to go....
      Task_Sleep(1);
      
    Suspend_Loop_Task();                          //suspend unnecessary tasks
    Task_Suspend(T_Timer_Task, TRUE);
    Task_Suspend(T_PanelManager, TRUE);

    InitSerialManager(TRUE, FALSE);               //initialise the UART
    ClearDisplay(); 

    while( (D == TRUE) &&
           (PC_Function == TRUE) )
    {
      if(D_Toggle == TRUE)                            //keep the HeartBeat going!!!!
      {
        D_Toggle = FALSE;
        Heart_Beat_LED_Port |= Heart_Beat_LED;

        if(SST == 0)                                  //and keep sending a summary object
        {                                             //so we don't appear 'missing' to the
          NetSendShortSummaryObject(0,0);             //network
          SST = 10;
        }
        else
          SST--;
      }

      if(PCRxCmdType < 9)                             //while the PC is connected
      {
        LocateYX(1,1);                                //show the status
        printf(PC_Connected_AL3);
        ClearToEOL();
      }

      PCTxBuffer = &Common_Scratch[0];                //utilise the 100 byte scratch area
      PCRxBuffer = &Common_Scratch[50];               //for TX & RX buffers.
      I_PCTxBuffer = (unsigned int *)&Common_Scratch[0];
      I_PCRxBuffer = (unsigned int *)&Common_Scratch[50];

      while(RBUsed() > 0)                             //while ever there are incoming characters
        ProcessRxData(GetRxChar());                   //process them

      if(GetNextKey() == K_ESCAPE)                    //check for ESCape from function
        D = FALSE;

      switch(PCRxCmdType)                             //now process the possible commands from the PC
      {
        case 0:             //no command for > Idle timeout period
          LocateYX(2,1);
          printf(PC_PressESC_AL3);    
          break;

        case 1:             //Upload FROM panel TO PC requested
          LocateYX(2,1);
          printf(PC_PanelToPC_AL3);    
          ClearToEOL();
          break;

        case 4:             //Download requiring a DataBase initialisation
          Init_DataBase = TRUE;        
          //drop through to case 2 and 3 and 5
        case 5:             //Download requiring sounder initialisation
          Init_Sounders = TRUE;
          //drop through to case 2 and 3
        case 2:             //Download FROM PC TO panel requested
        case 3:
          LocateYX(2,1);
          printf(PC_PCToPanel_AL3);    
          ClearToEOL();
          Flash_Data_Old = TRUE;
          break;

        case 9:   //updating texts, LCD must be blanked
          ClearDisplay();
          PCRxCmdType = 9;
          PC_Idle_Timer = 255;
          break;

      }

      InterpUDL();                                      //go and service the up/download jobs

      if(PC_Idle_Timer == 0)                            //when nothing is received
        PCRxCmdType = 0;                                //reset the command type

      Task_Sleep(1);

      Heart_Beat_LED_Port &= ~Heart_Beat_LED;
    }

    if(Init_DataBase == TRUE)                           //function exited, so check if a database
    {                                                   //initialisation is needed
      Copy_EEPROM_To_Flash();                           //and if so, update the Flash database

      ClearDisplay();                                 
      printf(PC_OK_AL3);
      LocateYX(2,1);
      printf(PC_MakeList_AL3);
      
      Make_Device_List();                               //and make a new device list
      Copy_EEPROM_To_Flash();
      
      Reset_System();                                   //need a full panel reset after this.....
      Reset_Loop(TRUE);
    }
    else                                                //full database init not needed, but maybe
      if(Flash_Data_Old == TRUE)                        //the Flash database needs updating. 
        Copy_EEPROM_To_Flash();                         //update it if so.

    if(Init_Sounders == TRUE)
      Re_Inititialise_Dis_Sounders();
        
    Resume_Loop_Task();                                 //don't forget to resume the 
    Task_Suspend(T_Timer_Task, FALSE);                             //suspended tasks
    Task_Suspend(T_PanelManager, FALSE);
      
    PC_Function = FALSE;                                //no longer in the PC function
    PC_Detected = FALSE;
    InitSerialManager(FALSE, FALSE);                    //re-init the shared UART functions
}

/*------------------------------------------------------------------------------------
Function Name   : InterpUDL

Description     : This function interprets the uploaded command from the PC

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void InterpUDL(void)
{
    if (PCRxCmdWaiting==TRUE)                       //incoming command ready?
    {                                               //if so, check 1st to see if we
        if( (Flash_Data_Old == TRUE) &&             //need to update the Flash database
            (PCRxCmdType < 2) )
          Copy_EEPROM_To_Flash();                   //do it if we do.

        Task_Sleep(2);
        RunUploadFuncs();                           //process the up or download command
        RunDownloadFuncs();

        PCRxPointer=0x00;                           //then reset buffer pointers etc
        PCRxChecksum=0x00;
        PCRxState=0x00;
        PCRxCmdWaiting=FALSE;                       //and acknowledge the command
    }
}

/*------------------------------------------------------------------------------------
Function Name   : Scale_Timer_Up

Description     : This function is used to scale the specified value. It is normally
                  used to scale incoming timer values into the panels' working units

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
unsigned int Scale_Timer_Up(unsigned char In_Value, unsigned char Multiplier)
{
    unsigned int V;
  
    if(In_Value != 255)
      V  = In_Value * Multiplier;
    else
      V = 65535;

    return(V);
}

/*------------------------------------------------------------------------------------
Function Name   : Scale_Timer_Down

Description     : This function is used to scale the specified value. It is normally
                  used to scale timer values prior to sending to the PC tools application

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
unsigned char Scale_Timer_Down(unsigned char * S_Ptr, unsigned char Divisor)
{
  unsigned char V;
  unsigned int In_Value;
  
  In_Value = (*(S_Ptr+1)) << 8;
  In_Value += *S_Ptr;
  
    if(In_Value != 65535)
      V  = In_Value / Divisor;
    else
      V = 255;
            
    return(V);
}

       
       
/*------------------------------------------------------------------------------------
Function Name   : 

Description     : These functions provide a response to the PC for a request for data

Input args      :
Return args     :

Special Notes   : Format of PCTxBuffer
[0] = command
[1] = length of data
[2..n] = data
[n] = checksum

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void RunUploadFuncs(void)
{
auto unsigned char LoopNo;
auto unsigned int DeviceNo;
auto unsigned char ZoneNo;        
auto unsigned char TempData;
TimeFormat T_Time;
auto unsigned int I;
auto unsigned char Temp[4];
auto unsigned int EV_No;
Event_History_Format EV;

DeviceRecordFormat DeviceRecord;
Special_CE_Format CE_Event;
Remote_Panel_Format RPF;

//These functions deal with data sent TO THE PC

    switch (PCRxCmd)
    {
        case PCReqUpload:
        {
            if (PCRxCmdError==FALSE)
            {
                PCTxBuffer[0]=PCRetUpload;
                PCTxBuffer[1]=0x00;
                PCTxBuffLen=PCTxBuffer[1]+0x02;
                SendPCTxBuffer();
            } else PCSendNak();


            break;
        }
        case PCReqPointName:
        {
            DeviceNo = PCRxBuffer[0];                
            PCTxBuffer[0]=PCRetPointName;
            PCTxBuffer[1]=Place_Name_Length;

            if(Load_Description(DeviceNo, &PCTxBuffer[2]) == FALSE)
              PCTxBuffer[1]=1;

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqZoneName:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetZoneName;
            PCTxBuffer[1]=Zone_Name_Length;
            
            Read_Zone_Name(ZoneNo, &PCTxBuffer[2]);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqMaintName:
        {
            PCTxBuffer[0]=PCRetMaintName;
            PCTxBuffer[1]=40;

            Read_Maintainence_String(&PCTxBuffer[2]);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqQuiesName:
        {
            PCTxBuffer[0]=PCRetQuiesName;
            PCTxBuffer[1]=40;

            Read_Quiescent_String(&PCTxBuffer[2]);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqPData:
        {
            DeviceNo = (PCRxBuffer[1] << 8) + PCRxBuffer[0];                

            PCTxBuffer[0]=PCRetPData;
            PCTxBuffer[1]=11;

            Read_DeviceRecord(DeviceNo, &DeviceRecord);

            if (Device_Fitted(DeviceNo)==TRUE) PCTxBuffer[2]=0xFF; else PCTxBuffer[2]=0x00;
            if (Device_Fitted(DeviceNo)==TRUE) PCTxBuffer[3]=0xFF; else PCTxBuffer[3]=0x00;

            PCTxBuffer[4]=DeviceRecord.Type;
            for(I=0; I < 8; I++)       
              PCTxBuffer[5+I]=DeviceRecord.Shared_Data[I];

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqMaintDate:
        {
            Read_Maintainence_Date(&T_Time);
            PCTxBuffer[0]=PCRetMaintDate;
            PCTxBuffer[1]=3;

            PCTxBuffer[2]=T_Time.Year;
            PCTxBuffer[3]=T_Time.Month;
            PCTxBuffer[4]=T_Time.Date;

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqDayNight:
        {
            I = Read_Day_Mode_Time();
            PCTxBuffer[0]=PCRetDayNight;
            PCTxBuffer[1]=10;

            PCTxBuffer[2]=I & 255;
            PCTxBuffer[3]=I >> 8;

            I = Read_Night_Mode_Time();
            PCTxBuffer[4]=I & 255;
            PCTxBuffer[5]=I >> 8;

            I = Read_ReCal_Time();
            PCTxBuffer[6]=0;
            PCTxBuffer[7]=I;

            I = Read_Day_Enabled(0);
            PCTxBuffer[8]=I;
            I = Read_Night_Enabled(0);
            PCTxBuffer[9]=I;

            PCTxBuffer[10] = Read_BST_Correction_Enabled();

            PCTxBuffer[11] = Read_RealTime_Event_Output_Enabled();
            
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqZGroup:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetZGroup;
            PCTxBuffer[1]=MaxSounderGroups + 5;

            for(I=1; I <= MaxSounderGroups; I++)
              PCTxBuffer[1+I] = Read_Zone_Matrix(ZoneNo, I);
              
            PCTxBuffer[1+I] = Read_Panel_Sounder_Group(0);
            PCTxBuffer[2+I] = Read_Panel_Sounder_Group(1);

            PCTxBuffer[3+I] = Read_Int_Tone();
            PCTxBuffer[4+I] = Read_Cont_Tone();

            PCTxBuffer[5+I] = Read_NewFire_ReSound_Functions();

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqZSet:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetZSet;
            PCTxBuffer[1]=MaxOutputSets+4;

            for(I=0; I<MaxOutputSets; I++)
              PCTxBuffer[2+I] = Read_Zone_OP_Matrix(ZoneNo, I+2);

            PCTxBuffer[2+I] = Read_Zone_OP_Matrix(ZoneNo, 0); //panel relay 1
            PCTxBuffer[3+I] = Read_Zone_OP_Matrix(ZoneNo, 1); //panel relay 2

            I_PCTxBuffer[2+ (I / 2)]=Scale_Timer_Up(Read_Set_Phased_Delay_Timer(), 10);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            

        case PCReqAL2Code:
        {
            I = Read_Access_Code(2);
            PCTxBuffer[0]=PCRetAL2Code;
            PCTxBuffer[1]=0x02;

            PCTxBuffer[2]=I & 255;
            PCTxBuffer[3]=I >> 8;

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqAL3Code:
        {
            I = Read_Access_Code(3);
            PCTxBuffer[0]=PCRetAL3Code;
            PCTxBuffer[1]=6;

            PCTxBuffer[2]=I & 255;
            PCTxBuffer[3]=I >> 8;

            PCTxBuffer[4] = Read_Polling_LED_Enabled();   //tag the polling flag onto this record.

            PCTxBuffer[5] = Read_MCP_Debounce();   
            
            PCTxBuffer[6] = Read_Detector_Debounce();
            
            PCTxBuffer[7] = Read_IO_Debounce();

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqLoopType:
        {
            PCTxBuffer[0]=PCRetLoopType;
            PCTxBuffer[1]=0x02;

            PCTxBuffer[2]=Protocol;
            PCTxBuffer[3]=Loops_Fitted();

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqFaultLockoutTime: //not used anymore
        {
            PCTxBuffer[0]=PCRetFaultLockoutTime;
            PCTxBuffer[1]=0x01;
            PCTxBuffer[2]=0;
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqPhasedSettings:
        {
            PCTxBuffer[0]=PCRetPhasedSettings;
            PCTxBuffer[1]=0x08;

            I_PCTxBuffer[1]=Scale_Timer_Up(Read_Phased_Evacuation_Timer(), 10);
            I_PCTxBuffer[2]=Scale_Timer_Up(Read_Investigation_Timer(0),10);
            I_PCTxBuffer[3]=Scale_Timer_Up(Read_Investigation_Timer(1),10);
            
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }            
        case PCReqZoneTimers:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetZoneTimers;
            PCTxBuffer[1]=19;

            Read_Zone_Timers(ZoneNo, Temp);
            I_PCTxBuffer[1] =Scale_Timer_Up(Temp[0],10);
            I_PCTxBuffer[2] =Scale_Timer_Up(Temp[1],10);
            I_PCTxBuffer[3] =Scale_Timer_Up(Temp[2],10);
            I_PCTxBuffer[4] =Scale_Timer_Up(Temp[3],10);
    
            PCTxBuffer[10] = Read_Zone_Dependancy(ZoneNo, FALSE); //Day
            PCTxBuffer[11] = Read_Zone_Dependancy(ZoneNo, TRUE);  //Night

            Read_Zone_Dependancy_Timers(ZoneNo, Temp);
            I_PCTxBuffer[6] =Scale_Timer_Up(Temp[0],10);    //Day
            I_PCTxBuffer[7] =Scale_Timer_Up(Temp[1],10);
            I_PCTxBuffer[8] =Scale_Timer_Up(Temp[2],10);    //Night
            I_PCTxBuffer[9] =Scale_Timer_Up(Temp[3],10);

            PCTxBuffer[20] = Read_Zone_Functions(ZoneNo);            

            PCTxBuffer[20] |= Read_Zone_End_Delays(ZoneNo) << 2;

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqMainVersion:
        {
            PCTxBuffer[0]=PCRetMainVersion;
            PCTxBuffer[1]=5;
            sprintf(&PCTxBuffer[2],"%02d%1c%02x",VersionMajor,VersionDividor,VersionMinor);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqRepeaterName:
        {
            ZoneNo=PCRxBuffer[0] + MaxLocalZones;
            PCTxBuffer[0]=PCRetRepeaterName;
            PCTxBuffer[1]=Zone_Name_Length;
            
            Read_Zone_Name(ZoneNo, &PCTxBuffer[2]);

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqOutputName:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetOutputName;
            PCTxBuffer[1]=3;

            PCTxBuffer[2]='A';
            PCTxBuffer[3]='B';
            PCTxBuffer[4]='C';
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqRepeaterFitted:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetRepeaterFitted;
            PCTxBuffer[1]=1;
            PCTxBuffer[2]=0xff;
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqOutputFitted:
        {
            ZoneNo=PCRxBuffer[0];
            PCTxBuffer[0]=PCRetOutputFitted;
            PCTxBuffer[1]=1;
            PCTxBuffer[2]=0xff;
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqSegmentNo:
        {
            PCTxBuffer[0]=PCRetSegmentNo;
            PCTxBuffer[1]=0x01;
            PCTxBuffer[2]=1;
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }                 
        case PCReqMainName:
        {
            PCTxBuffer[0]=PCRetMainName;
            PCTxBuffer[1]=3;

            PCTxBuffer[2]='A';
            PCTxBuffer[3]='B';
            PCTxBuffer[4]='C';

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }

        case PCReqTextString:
        {
            PCTxBuffer[0]=PCRetTextString;

            M_Ptr = (unsigned char *)BLOCK5;

            EV_No += (PCRxBuffer[1] << 8);
            EV_No += PCRxBuffer[0];

            while(EV_No > 0)
            {
              while(*M_Ptr != NULL)
                M_Ptr++;
               
              M_Ptr++; 
              EV_No--;
            }
            
            LoopNo = 1;                               //just use LoopNo variable
            I = 0;                                    //as loop termination sentinel
            while( (LoopNo > 0) && (I < 80) )
            {
              PCTxBuffer[I + 2] = *M_Ptr;
              M_Ptr++;
              I++;

              if(*M_Ptr == 0)
                LoopNo = 0;
            }
            PCTxBuffer[I + 2] = 0;
            
            PCTxBuffer[1] = I + 1;
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }

        case PCReqC_EEvent:
        {
            PCTxBuffer[0]=PCRetC_EEvent;
            PCTxBuffer[1]=11;
            EV_No = PCRxBuffer[0] - 1;
            
            Read_Special_CE_Event(EV_No, (Special_CE_Format *)&PCTxBuffer[2]);

            I = Read_CE_Timer_Event(EV_No);
            PCTxBuffer[11] = I & 255;
            PCTxBuffer[12] = I >> 8;
            
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }

        case PCReqEvent:
        {
            EV_No = PCRxBuffer[1] << 8;
            EV_No += PCRxBuffer[0];
            ZoneNo = PCRxBuffer[2];                         //pinch ZoneNo variable for the Erase_After setting
            PCTxBuffer[0]=PCRetEvent;
            PCTxBuffer[1]=0;

            if( EV_No < Events_Used() )
              PCTxBuffer[1] = Output_Event_String(&PCTxBuffer[2], EV_No, FALSE, 100, ZoneNo, FALSE)-1;

            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }

        case PCReq_NetPanelData:
        {
            PCTxBuffer[0]=PCRet_NetPanelData;
            PCTxBuffer[1]=8;

            for(ZoneNo = 1; ZoneNo <= MaxPossiblePanels; ZoneNo++)
            {  
              Read_Remote_PanelRecord(ZoneNo, &RPF);
              I = (RPF.Fitted) == TRUE ? 1 : 0;
              I |= (RPF.Accept_Faults) == TRUE ? 2 : 0;
              I |= (RPF.Accept_Alarms) == TRUE ? 4 : 0;
              I |= (RPF.Accept_Control_Keys) == TRUE ? 8 : 0;
              I |= (RPF.Accept_Disablements) == TRUE ? 16 : 0;
              I |= (RPF.Accept_Occupied) == TRUE ? 32 : 0;
              
              PCTxBuffer[ZoneNo + 1] = I;
            }
               
            PCTxBuffLen=PCTxBuffer[1]+0x02;
            SendPCTxBuffer();
            PCRxCmdType=1;
            break;
        }


    }
}



/*------------------------------------------------------------------------------------
Function Name   : RunDownloadFuncs

Description     : This function is called whenever a command is received from the PC
                  which contains data for us to store. 

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void RunDownloadFuncs(void)
{
auto unsigned char LoopNo;
auto unsigned int DeviceNo;
auto unsigned char ZoneNo;        
auto unsigned char TempByte, TempByte1;               
auto unsigned char DevType;
auto unsigned int I;
auto unsigned char Temp[4];
auto unsigned char * S_Ptr;
NetStatusFormat NS;

TimeFormat T_Time;
Remote_Panel_Format RPF;

    if(PCRxCmd == PCSetTextString)
    {
      ClearDisplay();
      InhibitTaskSwitching();
      Tasks_Suspended_Timer = 255;
    }
    
    switch (PCRxCmd)
    {
        case PCReqDownload:         //are we connected OK?
        {
            if (PCRxCmdError==FALSE)
            {
                PCTxBuffer[0]=PCRetDownload;
                PCTxBuffer[1]=0x01;
                if (Read_IP(WP_Link)==FALSE) PCTxBuffer[2]=0x00; else PCTxBuffer[2]=0xff;
                PCTxBuffLen=PCTxBuffer[1]+0x02;
                SendPCTxBuffer();
            } else PCSendNak();
            break;
        }
        case PCSetPointName:
        {
            if (PCRxCmdError==FALSE) 
            {
                DeviceNo = PCRxBuffer[1] << 8;
                DeviceNo += PCRxBuffer[0];

                Store_Description(DeviceNo, &PCRxBuffer[2]);

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetZoneName:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0];
      
                Store_Zone_Name(ZoneNo, &PCRxBuffer[1]);
                
                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetMaintName:
        {
            if (PCRxCmdError==FALSE) 
            {
                Write_Maintainence_String(&PCRxBuffer[0]);

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetQuiesName:
        {
            if (PCRxCmdError==FALSE) 
            {
                Write_Quiescent_String(&PCRxBuffer[0]);

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetPData:
        {
            if (PCRxCmdError==FALSE) 
            {
                LoopNo=PCRxBuffer[1]-1;
                DeviceNo=PCRxBuffer[0];

                if( Compare_DeviceRecord( (LoopNo << 8)+DeviceNo, (DeviceRecordFormat *)&PCRxBuffer[2]) == FALSE)
                {
                  Write_DeviceRecord((LoopNo << 8)+DeviceNo, (DeviceRecordFormat *)&PCRxBuffer[2]);
//                  Request_ReCalibration((LoopNo << 8)+DeviceNo, TRUE);
//maybe we also need to reset the dynamic device data type code. This should also force a recalibration
                  Erase_Dynamic_Data((LoopNo << 8)+DeviceNo);
                }
                
                PCSendAck(); 
                PCRxCmdType=4;
            } else PCSendNak();
            break;
        }
        case PCSetMaintDate:
        {
            if (PCRxCmdError==FALSE) 
            {
                T_Time.Year = PCRxBuffer[0];
                T_Time.Month = PCRxBuffer[1];
                T_Time.Date = PCRxBuffer[2];
                T_Time.Hours = PCRxBuffer[3];
                T_Time.Minutes = PCRxBuffer[4];
                T_Time.Seconds = PCRxBuffer[5];
                
                Write_Maintainence_Date(&T_Time);

                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetZGroup:
        {
            PCRxCmdType=3;
            if (PCRxCmdError==FALSE) 
            {                   
                ZoneNo=PCRxBuffer[0];
                PCRxBuffer[0]=0;
                Write_Zone_Group_Matrix(ZoneNo, &PCRxBuffer[0]);
    
                if(ZoneNo == 1)                   //sent every time, so only write on 1st one!
                {
                  Write_Panel_Sounder_Group(0, PCRxBuffer[MaxSounderGroups + 1]);
                  Write_Panel_Sounder_Group(1, PCRxBuffer[MaxSounderGroups + 2]);
                  
                  if(PCRxBuffLen > MaxSounderGroups + 4)
                  {
                    TempByte = PCRxBuffer[MaxSounderGroups + 3];
                    TempByte1 = PCRxBuffer[MaxSounderGroups + 4];
                    
                  }
                  else                            //older version of tools connected, so set
                  {                               //the data to defaults
                    if(Protocol == LD_Hochiki)
                      TempByte = 2;
                    else
                      TempByte = 1;

                    TempByte1 = 1;
                  }
                  
                  if(Read_Int_Tone() != TempByte)     //if setting(s) change
                  {
                    Write_Int_Tone(TempByte);         //update with new value
                    PCRxCmdType=5;                   //force update of Disc. sounder beacons
                  }
                  if(Read_Cont_Tone() != TempByte1)
                  {
                    Write_Cont_Tone(TempByte1);
                    PCRxCmdType=5;                   //force update of Disc. sounder beacons
                  }
 
                  if(PCRxBuffLen > MaxSounderGroups + 5)
                    Write_NewFire_ReSound_Functions(PCRxBuffer[MaxSounderGroups + 5]);
                  else
                    Write_NewFire_ReSound_Functions(FALSE);
                }
                    
                PCSendAck();                
            } else PCSendNak();
            break;
        }
        case PCSetZSet:
        {
            if (PCRxCmdError==FALSE) 
            {                   
                ZoneNo=PCRxBuffer[0];

                Write_Zone_Set_Matrix(ZoneNo, &PCRxBuffer[1]);

                Write_Set_Phased_Delay_Timer(Scale_Timer_Down(&PCRxBuffer[19], 10));

                PCSendAck();                
                PCRxCmdType=3;
            } else PCSendNak();
            break;
        }
        case PCSetAL2Code:
        {
            if (PCRxCmdError==FALSE) 
            {                   
                I = PCRxBuffer[1] << 8;
                I += PCRxBuffer[0];

                Write_Access_Code(2, I);

//no distinction for keyswitch version anymore 24/5/11
//this means if the config file had 5555 stored as AL2 code, it will stay
//as 5555 i.e. no way to enter AL2 with code
//                if(I != 5555)
//                  KeySwitch_Timer = 0;
                  
                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetAL3Code:
        {
            if (PCRxCmdError==FALSE) 
            {                   
                I = PCRxBuffer[1] << 8;
                I += PCRxBuffer[0];
                
                Write_Access_Code(3, I);

                if(PCRxBuffLen > 2)                   //if the PC tools support it...
                  Write_Polling_LED_Enabled(PCRxBuffer[2]); //update the Disc. polling LED
                else                                  //if the tools don't support it
                  Write_Polling_LED_Enabled(FALSE);   //set the flag to disabled


                if(PCRxBuffLen > 5)                   //if the PC tools support it...
                {
                  Write_MCP_Debounce(PCRxBuffer[3]);
                  Write_Detector_Debounce(PCRxBuffer[4]);
                  Write_IO_Debounce(PCRxBuffer[5]);
                }
                
                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetPhasedSettings:
        {
            if (PCRxCmdError==FALSE) 
            {                   
                Write_Phased_Evacuation_Timer(Scale_Timer_Down(&PCRxBuffer[0], 10));
                Write_Investigation_Timer(0,Scale_Timer_Down(&PCRxBuffer[2], 10));
                Write_Investigation_Timer(1,Scale_Timer_Down(&PCRxBuffer[4], 10));

                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetLoopType:
        {
            if (PCRxCmdError==FALSE) 
            {                   

                PCSendAck();                
                PCRxCmdType=3;
            } else PCSendNak();
            break;
        }
        case PCSetFaultLockoutTime:
        {
            if (PCRxCmdError==FALSE) 
            {                   

                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetTimeDate:
        {
            if (PCRxCmdError==FALSE) 
            {                   
                CurrentTime.Year=PCRxBuffer[0];
                CurrentTime.Month=PCRxBuffer[1];
                CurrentTime.Date=PCRxBuffer[2];
                CurrentTime.Hours=PCRxBuffer[3];
                CurrentTime.Minutes=PCRxBuffer[4];
                CurrentTime.Seconds=PCRxBuffer[5];

                SetTime();

                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetZoneTimers:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0];

                Temp[0] = Scale_Timer_Down(&PCRxBuffer[1], 10);
                Temp[1] = Scale_Timer_Down(&PCRxBuffer[3], 10);
                Temp[2] = Scale_Timer_Down(&PCRxBuffer[5], 10);
                Temp[3] = Scale_Timer_Down(&PCRxBuffer[7], 10);
                Write_Zone_Timers(ZoneNo, Temp);

                Temp[0] = PCRxBuffer[9];          //day dependancy mode
                Temp[1] = Scale_Timer_Down(&PCRxBuffer[11],10);
                Temp[2] = Scale_Timer_Down(&PCRxBuffer[13],10);
                Write_Zone_Dependancy(ZoneNo, Temp);

                Temp[0] = PCRxBuffer[10];          //night dependancy mode
                Temp[1] = Scale_Timer_Down(&PCRxBuffer[15],10);
                Temp[2] = Scale_Timer_Down(&PCRxBuffer[17],10);
                Write_Zone_Dependancy(ZoneNo + MaxPossibleZones, Temp);

                Write_Zone_Functions(ZoneNo, PCRxBuffer[19]);

                Write_Zone_End_Delays(ZoneNo, (PCRxBuffer[19] >> 2) & 1);

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetRepeaterName:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0] + MaxLocalZones;
      
                Store_Zone_Name(ZoneNo, &PCRxBuffer[1]);
                
                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetOutputName:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0];

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetRepeaterFitted:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0];

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetOutputFitted:
        {
            if (PCRxCmdError==FALSE) 
            {
                ZoneNo=PCRxBuffer[0];

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetSegmentNo:
        {
            if (PCRxCmdError==FALSE) 
            {                   

                PCSendAck();                
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetMainName:
        {
            if (PCRxCmdError==FALSE) 
            {

                PCSendAck(); 
                PCRxCmdType=2;
            } else PCSendNak();
            break;
        }
        case PCSetNonFireFlags:
        {
            if (PCRxCmdError==FALSE) 
            {              

              PCSendAck();                            
              PCRxCmdType=2; 
            } else PCSendNak();
            break;
        } 

        case PCSetDayNight:
        {
            if (PCRxCmdError==FALSE) 
            {              
              I = PCRxBuffer[1] << 8;
              I += PCRxBuffer[0];
              Write_Day_Mode_Time(I);              

              I = PCRxBuffer[3] << 8;
              I += PCRxBuffer[2];
              Write_Night_Mode_Time(I);              

              I = PCRxBuffer[4];
              Write_ReCal_Time(I);              

              I = PCRxBuffer[6];
              Write_Day_Enabled(I);              
              I = PCRxBuffer[7];
              Write_Night_Enabled(I);              

              if(PCRxBuffLen > 8)                           //if pc tools support it....
                Write_BST_Correction_Enabled(PCRxBuffer[8]);

              if(PCRxBuffLen > 9)                           //if pc tools support it....
                Write_RealTime_Event_Output_Enabled(PCRxBuffer[9]);
              
              PCSendAck();                            
              PCRxCmdType=2; 
            } else PCSendNak();
            break;
        } 

        case PCSetC_EEvent:
        {
            if (PCRxCmdError==FALSE) 
            {              
              ZoneNo = PCRxBuffer[0] - 1;

              Write_Special_CE_Event(ZoneNo, (Special_CE_Format *)&PCRxBuffer[1]);

              I = PCRxBuffer[11] << 8;
              I += PCRxBuffer[10];
  
              if(ZoneNo <= 16)                              //theres only ever 16 time of day events
                Write_CE_Timer_Event(ZoneNo, I);
              
              PCSendAck();                            
              PCRxCmdType=2; 
            } else PCSendNak();
            break;
        } 

        case PCSet_NetPanelData:
        {
          if (PCRxCmdError==FALSE) 
          {              
            for(ZoneNo = 1; ZoneNo <= MaxPossiblePanels; ZoneNo++)
            {  
              I = PCRxBuffer[ZoneNo - 1];

              RPF.Fitted = (I & 1) == 1 ? TRUE : FALSE;
              RPF.Accept_Faults = (I & 2) == 2 ? TRUE : FALSE;
              RPF.Accept_Alarms = (I & 4) == 4 ? TRUE : FALSE;
              RPF.Accept_Control_Keys = (I & 8) == 8 ? TRUE : FALSE;
              RPF.Accept_Disablements = (I & 16) == 16 ? TRUE : FALSE;
              RPF.Accept_Occupied = (I & 32) == 32 ? TRUE : FALSE;
              
              Write_Remote_PanelRecord(ZoneNo, &RPF);
            }
               

            PCSendAck();                            
            PCRxCmdType=2; 
          } else PCSendNak();
          break;
        }

        case PCRemoteData:
        {
          NS = NetSendPCData(PCRxAddress, XFP_Address, &PCRxBuffer[0], PCRxBuffLen+1, FALSE);
          if(NS.Status == NET_TX_SUCCESS)
          {
            S_Ptr = TxMessages[NS.MsgNum].Reply.DPtr;
            if(S_Ptr != NULL)                   //ensure data actually got saved!
            {
              Return_Data_To_PC(S_Ptr);
              free(S_Ptr);
            }
            TxMessages[NS.MsgNum].State = NET_TX_IDLE;
          }
          break;
        }        

        case PCSetTextString: //note this is NOT the text description of a field device.
                              //it is a blue sky function for having a go at sending the MMI text strings
                              //down from the PC to handle language variants etc. It has never been
                              //implemented!
                         
        {
            if (PCRxCmdError==FALSE) 
            {
              DeviceNo = PCRxBuffer[1] << 8;
              DeviceNo += PCRxBuffer[0];

              Store_TextString(DeviceNo, &PCRxBuffer[2]);

              if(DeviceNo == 999)
                RestoreTaskSwitching();
              
              PCSendAck(); 
              PCRxCmdType=9;
            } else PCSendNak();
            break;
        }

    }
}
 
/*------------------------------------------------------------------------------------
Function Name   : PCSendAck

Description     : Sends an ACK packet back to the PC

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void PCSendAck(void)
{
    PCTxBuffer[0]=PCAck;
    PCTxBuffer[1]=0;

    if(Remote_PC_Connected == FALSE)  
      SendTxBuffer(PCTxBuffer, 1);
    else
      NetSendPCData(Remote_PC_Segment,XFP_Address,PCTxBuffer, 2, TRUE);
      
    Remote_PC_Connected = FALSE;  
}

/*------------------------------------------------------------------------------------
Function Name   : PCSendNak

Description     : Sends a NAK packet back to the PC

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void PCSendNak(void)
{
    PCTxBuffer[0]=PCNak;
    PCTxBuffer[1]=0;

    if(Remote_PC_Connected == FALSE)  
      SendTxBuffer(PCTxBuffer, 1);
    else
      NetSendPCData(Remote_PC_Segment,XFP_Address,PCTxBuffer, 2, TRUE);
      
    Remote_PC_Connected = FALSE;  
}

/*------------------------------------------------------------------------------------
Function Name   : Return_Data_To_PC

Description     : This function is called when the PCTxBuffer has been loaded with
                  only the command and length bytes. S_Ptr will point to the data
                  required to be added to PCTxBuffer.                 

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Return_Data_To_PC(unsigned char * S_Ptr)
{
    unsigned char Len;

    PCTxBuffLen = 0;    
    PCTxBuffer[PCTxBuffLen++] = *S_Ptr++;
    Len = PCTxBuffer[PCTxBuffLen++] = *S_Ptr++;

    if(Len > 0)                                   //if Len > 0 this means
    {                                             //there is actually data to return 
      Len++;
      while(Len > 0)
      {
        PCTxBuffer[PCTxBuffLen++] = *S_Ptr++;
        Len--;
      }
      SendTxBuffer(PCTxBuffer, PCTxBuffLen);      //to the PC
    }
    else    
      SendTxBuffer(PCTxBuffer, 1);                //otherwise, it must be an ACK/NAK character
    
}
 
/*------------------------------------------------------------------------------------
Function Name   : SendPCTxBuffer

Description     : This function calculates the checksum and appends it to the PCTxBuffer,
                  then actually intiates the sending of the buffer.

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void SendPCTxBuffer(void)
{
    auto unsigned char Count;
    auto unsigned char CheckSum;

    CheckSum=0x00;
 
    for (Count=0x00; Count<PCTxBuffLen; Count++)  //calculate the checksum
      CheckSum+=PCTxBuffer[Count];                  
    PCTxBuffer[PCTxBuffLen++]=CheckSum;           //and append it

    if(Remote_PC_Connected == FALSE)              //if talking directly to the PC
      SendTxBuffer(PCTxBuffer, PCTxBuffLen);      //send using this function
    else                                          //if talking via network, use this
      NetSendPCData(Remote_PC_Segment,XFP_Address,PCTxBuffer, PCTxBuffLen, TRUE);
      
    Remote_PC_Connected = FALSE;  
}

 
/*------------------------------------------------------------------------------------
Function Name   : ProcessRxData

Description     : 

Input args      :
Return args     :

Special Notes   : Incoming data format

[SOH][panel no][cmd][len][data][checksum]

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void ProcessRxData(unsigned char SCDR)
{
    auto unsigned char Char;
    
    if(PC_RxTimer==0)
      PCRxState=0;

    PC_RxTimer=50;

      switch(PCRxState)
      {
        case 0:                   //waiting for SOH
          if(SCDR == 01)
          {
            PCRxState++;

            PCRxCmdWaiting=FALSE;
            Char=SCDR;
            PCRxChecksum = 0;
          }
          break;   

        case 1:                   //received the Panel Number
          PCRxAddress = SCDR;
          PCRxChecksum += PCRxAddress;
          PCRxState++;
          break;
                  
        case 2:                   //Received the Command
          Char=SCDR;

          if( (PCRxAddress == NetSegment) ||
              (PCRxAddress == 0) ||      //message for me?
              (NetSegment == 0) )      //message for me?
          {
            PCRxCmd=Char;                   //yep, prepare to process it later
            PCRxPointer=0x00;                 // on start of message, reset buffer pointer
          }
          else                              
          {
            PCRxCmd = PCRemoteData;         //nope, prepare to pass it on later
            PCRxBuffer[0] = SOH;
            PCRxBuffer[1] = PCRxAddress;
            PCRxBuffer[2] = Char;
            PCRxPointer=4;
          }
            
          PCRxChecksum += Char;
          PCRxState++;
          break;
          
        case 3:                   //Received the LEN
          PCRxBuffLen=SCDR;
          PCByteCount=PCRxBuffLen;
          
          if (PCRxBuffLen==0) PCRxState=0x05; else PCRxState=0x04;

          if(PCRxCmd == PCRemoteData)
            PCRxBuffer[3] = PCRxBuffLen;
            
          PCRxChecksum+=PCRxBuffLen;
          break;
          
        case 4:                   //Receiving the characters
          PCRxBuffer[PCRxPointer++]=SCDR;                                   // save next byte to buffer
          PCRxChecksum+=SCDR;
        
          PCByteCount--;
          if (PCByteCount == 0) PCRxState=0x05;
          if(PCRxPointer >= PCBLength)PCRxState=0x05;
          break;
          
        case 5:                   //Received the checksum
          RxdChecksum=SCDR;
          if (PCRxChecksum==RxdChecksum) 
          {  
            PCRxCmdWaiting=TRUE;
            PCRxCmdError=FALSE;
            PC_Idle_Timer = 100;

            PCRxBuffer[PCRxPointer]=RxdChecksum;
          }
          else PCRxCmdError=TRUE;

          PCRxState=0;
          break;
          
        default:
          PCRxState=0;
          break;
      }  
}
 
