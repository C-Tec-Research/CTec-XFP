/***************************************************************************
DRQ:		200
PROJECT:	1 & 2 Loop CAST XFP fire panel

COMPANY:	C-TEC Research & Development Centre
Dark Lane
Mawdesley
Lancashire
L40 2QU  



MODULE:	


Description 

***************************************************************************/
#include "Essential Include Files.h"

#define Define_PCComms_Strings
#include "Text Definition.h"

#include "NetCommon.h"

#include "Loop Driver Interface.h"
#include "loop_commands.h"

#include "CAST H_W.h"

//  external text strings
extern const unsigned char * MM_ReconfiguringDevice_AL123;


//  external variables
extern u8 ReadTypeCodes[No_Of_Loops];
extern u8 MN_CommitPresenceMap[];
extern u8 NoNewDeviceNotification[No_Of_Loops];

extern const unsigned char * MM_RestartingLoop_AL123;

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
u16 PC_UART_No_Of_Bytes;
u8 PC_UART_Buffer_Empty;
u8 * PC_UART_S_Ptr;


#define RingSize 32
u8 RxRing[RingSize];
u8 RxEP;
u8 RxFP;

u8 RxChecksum;

u16 RError_Count_1;


#define PCBLength 50

u8 PCTxBuffer[PCBLength];
u8 PCRxBuffer[PCBLength];
u16 * I_PCTxBuffer;
u16 * I_PCRxBuffer;

u8 PCRxPointer;
u8 PCRxChecksum;
u8 PCRxState;

u8 PCTxBufferLen;
u8 PCRxBuffLen;
u8 PCByteCount;

u8 PCRxCmdType;

u8 PCRxAddress;
u8 PCRxCmd;
u8 RxdChecksum;

u8 ReconfigureAllDevices;
u8 Restart_Loop[No_Of_Loops];

const u8 * M_Ptr;

bit PCRxCmdError;

//  external variables
extern Updated_Flags_Format Update_Flags[No_Of_Loops * 256];

/***************************************************************************
*                               Function Prototypes                        *
***************************************************************************/

void InterpUDL(void);
void RunUploadFuncs(void);
void RunDownloadFuncs(void);
void ProcessRxData(u8 SCDR);
void PCSendAck(void);
void PCSendNak(void);
void SendSignOn(void);
void SendSignOff(void);
void SendPCTxBuffer(void);
void Return_Data_To_PC(u8 * S_Ptr);

void _Handle_UART_0(void);

u16 RBUsed(void);
u16 GetRxChar(void);
void SendTxBuffer(u8 * SPtr, u8 Len);

/***************************************************************************
*                                 PROGRAM                                  *
***************************************************************************/


//local functions

/*------------------------------------------------------------------------------------
Function Name   : Configuration_Changed

Description     : This function will return TRUE is any of the device's 
                  configuration parameters have changed and new ones need 
                  to be sent to the device.

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
static u8 Configuration_Changed(u16 DeviceAddress, DeviceRecordFormat * S_Ptr)
{
  u8 i;
  u8 RC = FALSE;                      //anticipate no re-configuration required
  DeviceRecordFormat Current_Data;
  
  Read_DeviceRecord(DeviceAddress, &Current_Data);
 
  if(Current_Data.Type != S_Ptr->Type)   
    return(TRUE);                      

  switch(Current_Data.Type)
  {
    case Discovery_Multi:
    case XP95_Multi:
    case Discovery_Optical:
    case XP95_Optical:    
    case Discovery_Heat:
    case Discovery_Hi_Temp:
    case XP95_Heat:
    case XP95_Hi_Temp:  
      if(Current_Data.Shared_Data[DR_Day_Sensitivity] != S_Ptr->Shared_Data[DR_Day_Sensitivity])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_Night_Sensitivity] != S_Ptr->Shared_Data[DR_Night_Sensitivity])
        RC = TRUE;
      break;
      
    case Discovery_Sounder_Beacon:
    case Discovery_Voice_Sounder_Beacon:
    case Discovery_Sounder:
    case XP95_Sounder:
      if(Current_Data.Shared_Data[DR_SA0_ZGS] != S_Ptr->Shared_Data[DR_SA0_ZGS])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_Volume_Day] != S_Ptr->Shared_Data[DR_Volume_Day])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_Volume_Night] != S_Ptr->Shared_Data[DR_Volume_Night])
        RC = TRUE;
      break;
 
    case Discovery_Manual_Callpoint:
      break;

    case XP95_IO_Unit:
    case Discovery_HS2:
    case NONLATCH_IO:
      if(Current_Data.Shared_Data[DR_SA0_ZGS] != S_Ptr->Shared_Data[DR_SA0_ZGS])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_SA1_ZGS] != S_Ptr->Shared_Data[DR_SA1_ZGS])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_SA2_ZGS] != S_Ptr->Shared_Data[DR_SA2_ZGS])
        RC = TRUE;
      if(Current_Data.Shared_Data[DR_SA3_ZGS] != S_Ptr->Shared_Data[DR_SA3_ZGS])
        RC = TRUE;
      break;
      
    default:
       break;
  }
  
  return(RC);
}




/*------------------------------------------------------------------------------------
Function Name   : RecConfigure_Sounders

Description     : 


Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/  
static void RecConfigure_Sounders(void)
{
  ADDRESS_FORMAT Address;
  u8 Loop;
  u8 ac[64];
        
  for(ALL_DEVICES_ON_SYSTEM)
  {
    if(Is_A_Sounder(Address, D_PROGRAMMED) == TRUE)
      if(Get_Address_Field(Address) > 0)
      {
        if(Read_DeviceType(Address) < D_Duplicate)
        {
          ClearDisplay();
          sprintf(ac, MM_ReconfiguringDevice_AL123, Get_Loop_Field(Address) + 1, Address & 255);
          printf(ac);
          
          Loop = Get_Loop_Field(Address);
          Update_Flags[Build_Address(Loop, Address)].Config_Changed = TRUE;
          
          OS_Delay(250);
        }
      }
  }
}

//  Public functions
/*------------------------------------------------------------------------------------
Function Name   : PC_Manager_Task

Description     : 


Input args      :
Return args     :

Special Notes   : This version is ran as a concurrent task under RTOS, if the
                  task has been created in main()

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void PC_Manager_Task(void)
{
  u8 Init_DataBase = FALSE;    
  u8 Init_Sounders = FALSE;
  u8 ch;
  
  u16 Timeout = 0;

  u8 PM_Suspended = FALSE;
  
  I_PCTxBuffer = (u16 *)&PCTxBuffer[0];
  I_PCRxBuffer = (u16 *)&PCRxBuffer[0];
  
  while(1)
  {    
    while(RBUsed() > 0)                             //while ever there are incoming characters
    {
      PC_Function = TRUE;  
      Timeout = 500;
      ProcessRxData(GetRxChar());                   //process them
    }
    
    if(Timeout > 0)
    {
      Timeout--;
      if(Timeout == 0)
      {
        PC_Function = FALSE;  
        PCRxCmdType = 0;
        
        if(Init_DataBase == TRUE)                           //function exited, so check if a database
        {                                                   //initialisation is needed
          Init_DataBase = FALSE;
          
          Copy_EEPROM_To_Flash();                           //and if so, update the Flash database
                    
          Data_Initialise(FORCE_UPDATE);
          
          Invalidate_Device_List();
          Rebuild_Device_List();
          
          Reset_System();                                   //need a full panel reset after this.....
          Reset_Loop(FALSE);
        }
        else                                                //full database init not needed, but maybe
          if(Flash_Data_Old == TRUE)                        //the Flash database needs updating. 
          {
            Flash_Data_Old = FALSE;
            
            Copy_EEPROM_To_Flash();                         //update it if so.
            Flash_Data_Old = FALSE;
          }

        if(Init_Sounders == TRUE)
        {
          Init_Sounders = FALSE;
          RecConfigure_Sounders();
        }
       
        if(PM_Suspended == TRUE)
        {
          OS_Resume(&TCBPM_Event_Manager_Task);        
          PM_Suspended = FALSE;
        }
      }
      else
      {
        if(PM_Suspended == FALSE)
        {
          OS_Suspend(&TCBPM_Event_Manager_Task);
          PM_Suspended = TRUE;
        }
      }
    }
      
    switch(PCRxCmdType)                             //now process the possible commands from the PC
    {
    case 0:             //no command for > Idle timeout period
    case 255:             //no command for > Idle timeout period
      break;
      
    case 1:             //Upload FROM panel TO PC requested
      break;
      
    case 4:             //Download requiring a DataBase initialisation
      Init_DataBase = TRUE;        
      //drop through to case 2 and 3 and 5
    case 5:             //Download requiring sounder initialisation
      Init_Sounders = TRUE;
      //drop through to case 2 and 3
    case 2:             //Download FROM PC TO panel requested
    case 3:
      Flash_Data_Old = TRUE;
      break;
      
    case 9:   //updating texts, LCD must be blanked
      PCRxCmdType = 9;
      PC_Idle_Timer = 255;
      break;
      
    }
    
    InterpUDL();                                      //go and service the up/download jobs
    
    OS_Delay(10);
  }




}


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
  u8 SST = 10;
  u8 D = TRUE;
  u8 Init_DataBase = FALSE;    
  u8 Init_Sounders = FALSE;

  app_msg_type app_msg;
  u8 Loop;
  
#ifndef TRAINING_LCD_OUTPUT
  if(Read_Repeater_Functions() == TRUE)
  {
    Repeater_ESC_Command = ESC_Comms_Stopped;     //send command to repeaters
    while(Repeater_ESC_Command == ESC_Comms_Stopped) //wait for it to go....
      Task_Sleep(1);
  }
#endif
  
  PC_Function = TRUE;
  ReconfigureAllDevices = FALSE;
  Restart_Loop[LOOP_1] = Restart_Loop[LOOP_2] = FALSE;

  Initialise_PCComms_Hardware();
  
  ClearDisplay(); 

//Mod 190116_6
  //issue a Loop Stop command to the LDI
  //LDI_LoopControl(0, LOOP_STOP);
  //LDI_LoopControl(1, LOOP_STOP);
  //OS_Delay(1000);
  
  OS_Suspend(&TCB_Loop_StateMachine_Task);
  OS_Suspend(&TCBPM_Event_Manager_Task);
  OS_Suspend(&TCB_Panel_Manager);
  OS_Suspend(&TCBEventManagerTask);
  OS_Suspend(&TCBMaintain_ProgramCRC);

#ifdef R_AND_D_MENUS //JG Connect to PC issue with Customer Release
  OS_Suspend(&TCBPC_Manager);  
#endif 
  
    
  while( (D == TRUE) &&
        (PC_Function == TRUE) )
  {
    if(D_Toggle == TRUE)                            //keep the HeartBeat going!!!!
    {
      D_Toggle = FALSE;
      
      if(SST == 0)                                  //and keep sending a summary object
      {                                             //so we don't appear 'missing' to the
        NetSendShortSummaryObject(0,0);             //network
        SST = 10;
      }
      else
        SST--;
    }
    
    if( (PCRxCmdType < 9) ||                            //while the PC is connected
        (PCRxCmdType == 255) )
    {
      LocateYX(1,1);                                //show the status
      printf(PC_Connected_AL3);
      ClearToEOL();
    }
    
    I_PCTxBuffer = (u16 *)&PCTxBuffer[0];
    I_PCRxBuffer = (u16 *)&PCRxBuffer[0];
    
    while(RBUsed() > 0)                             //while ever there are incoming characters
      ProcessRxData(GetRxChar());                   //process them
    
    if(GetNextKey() == K_ESCAPE)                    //check for ESCape from function
      D = FALSE;
    
    switch(PCRxCmdType)                             //now process the possible commands from the PC
    {
    case 0:             //no command for > Idle timeout period
    case 255:             //no command for > Idle timeout period
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
      //drop through to case 5 and 3 and 2
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
      PCRxCmdType = 255;                                //reset the command type
//      PCRxCmdType = 0;                                //reset the command type
    
    OS_Delay(10);
  }
  
  if(Init_DataBase == TRUE)                           //function exited, so check if a database
  {                                                   //initialisation is needed
    Copy_EEPROM_To_Flash();                           //and if so, update the Flash database
    
    ClearDisplay();                                 
    printf(PC_OK_AL3);
    LocateYX(2,1);
    printf(PC_MakeList_AL3);
    
    Data_Initialise(FORCE_UPDATE);

    Restart_Loop[LOOP_1] =
    Restart_Loop[LOOP_2] = TRUE;
    
    Invalidate_Device_List();
    Rebuild_Device_List();
    
    Reset_System();                                   //need a full panel reset after this.....
    Reset_Loop(FALSE);
  }
  else                                                //full database init not needed, but maybe
    if(Flash_Data_Old == TRUE)                        //the Flash database needs updating. 
    {
      Copy_EEPROM_To_Flash();                         //update it if so.
      Flash_Data_Old = FALSE;
    }

  if(Init_Sounders == TRUE)
    RecConfigure_Sounders();
  
  OS_Resume(&TCB_Loop_StateMachine_Task);
  OS_Resume(&TCBPM_Event_Manager_Task);
  OS_Resume(&TCB_Panel_Manager);
  OS_Resume(&TCBEventManagerTask);
  OS_Resume(&TCBMaintain_ProgramCRC);

#ifdef R_AND_D_MENUS //JG Connect to PC issue with Customer Release
  OS_Resume(&TCBPC_Manager);  
#endif 

  //Mod 190116_6
  //issue a Loop Re-Init command to the LDI, this will perform a Low Level loop Initialisation
  if(Restart_Loop[LOOP_1] || Restart_Loop[LOOP_2]  == TRUE) 
  {   
    ClearDisplay();
    printf(MM_RestartingLoop_AL123);
    
    for(Loop = 0; Loop < Loops_Fitted(); Loop++)
    {
      if(Restart_Loop[Loop] == TRUE)
      {
        //commit whats learned on the next loop init to the
        //permanent presence_map
        MN_CommitPresenceMap[Loop] = TRUE;
        
        //force a full Type Code read during the next low level init
        ReadTypeCodes[Loop] = TRUE;
        
        //tell low level init routine NOT to set the NewDevice flag!
        NoNewDeviceNotification[Loop] = TRUE;
        
        //prepare and send a LOOP_RESTART_REQUIRED to the Loop App
        app_msg.module_id       = MASTER_NODE_ID;
        app_msg.type            = APP_EVENT;
        app_msg.code            = LOOP_RESTART_REQUIRED;
        app_msg.pcode           = &app_msg.code;  
        app_msg.priority        = Loop_RestartTime(Loop);
        app_msg.loop_id         = Loop_Restart_PowerCycle(Loop);
        app_msg.hsdata_string   = NULL;
        app_msg.hsdata_length   = 0;
        app_msg.Loop            = Loop;
        
        PMN_PostMessage(&app_msg);
        
        //wait until the Loop Restart request has been processed
        while(app_msg.code == LOOP_RESTART_REQUIRED)
          OS_Delay(100);
      }
    }
  }
    
  //re-configure ALL devices if required e.g. a common setting has changed i.e.
  //the Blink Polling LED setting
  if(ReconfigureAllDevices == TRUE)
    LDI_ClearDeviceConfiguredTable(TRUE);

  PC_Function = FALSE;                                //no longer in the PC function
  PC_Detected = FALSE;
}

/*------------------------------------------------------------------------------------
Function Name   : InterpUDL

Description     : This function interprets the upload/download commands from the PC

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
    {
      Copy_EEPROM_To_Flash();                   //do it if we do.
      Flash_Data_Old = FALSE;
    }
    
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
u16 Scale_Timer_Up(u8 In_Value, u8 Multiplier)
{
  u16 V;
  
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
u8 Scale_Timer_Down(u8 * S_Ptr, u8 Divisor)
{
  u8 V;
  u16 In_Value;
  
  In_Value = (*(S_Ptr+1)) << 8;
  In_Value += *S_Ptr;
  
  if(In_Value != 65535)
    V  = In_Value / Divisor;
  else
    V = 255;
  
  return(V);
}



/*------------------------------------------------------------------------------------
Function Name   : RunUploadFuncs (sending data TO the PC)

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
   u16 DeviceNo;
   u8 ZoneNo;        
   RTC_TimeTypeDef T_Time;
   u16 I;
   u8 Temp[4];
   u16 EV_No;

   Special_CE_Format CE_EventData;

  DeviceRecordFormat DeviceRecord;
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
      
      PCTxBuffer[2]=T_Time.RTC_Year;
      PCTxBuffer[3]=T_Time.RTC_Month;
      PCTxBuffer[4]=T_Time.RTC_DOM;
      
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
      
      PCTxBuffer[2]=LD_CAST_ForTools;
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
      sprintf(&PCTxBuffer[2],"%02d%1c%02d",VersionMajor,VersionDividor,VersionMinor);
      
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
  
/*    
  case PCReqTextString: //(not used)
    {
      PCTxBuffer[0]=PCRetTextString;
      
      M_Ptr = (u8 *)BLOCK5;
      
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
*/    
  case PCReqC_EEvent:
    {
      PCTxBuffer[0]=PCRetC_EEvent;
      PCTxBuffer[1]=11;
      EV_No = PCRxBuffer[0] - 1;
      
      Read_Special_CE_Event(EV_No, &CE_EventData);

      PCTxBuffer[2] = CE_EventData.Event_Type;
      PCTxBuffer[3] = CE_EventData.Address & 255;
      PCTxBuffer[4] = CE_EventData.Address >> 8;

      PCTxBuffer[5] = CE_EventData.Trigger_Condition.Trig_Type;
      PCTxBuffer[6] = CE_EventData.Trigger_Condition.Address & 255;
      PCTxBuffer[7] = CE_EventData.Trigger_Condition.Address >> 8;
      
      PCTxBuffer[8] = CE_EventData.Normalise_Condition.Trig_Type;
      PCTxBuffer[9] = CE_EventData.Normalise_Condition.Address & 255;
      PCTxBuffer[10] = CE_EventData.Normalise_Condition.Address >> 8;
        
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
   u8 LoopNo;
   u16 DeviceNo;
   u8 ZoneNo;        
   u8 TempByte, TempByte1;               
   u16 I;
   u8 Temp[4];
   u8 * S_Ptr;

   Special_CE_Format CE_EventData;
 
   NetStatusFormat NS;
  
  RTC_TimeTypeDef T_Time;
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
        if (WPLinkFitted()==FALSE) PCTxBuffer[2]=0x00; else PCTxBuffer[2]=0xff;
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
      PCRxCmdType=2;
      if (PCRxCmdError==FALSE) 
      {
        LoopNo=PCRxBuffer[1]-1;
        DeviceNo=PCRxBuffer[0];
        
//        if( Compare_DeviceRecord(Build_Address(LoopNo, DeviceNo), (DeviceRecordFormat *)&PCRxBuffer[2]) == FALSE)
        if(Configuration_Changed(Build_Address(LoopNo, DeviceNo), (DeviceRecordFormat *)&PCRxBuffer[2]) == TRUE)
        { 
          //if the device type is changed, we MUST do a loop restart when we exit PC Connect mode
          if(Read_DeviceType(Build_Address(LoopNo, DeviceNo)) != PCRxBuffer[2]) 
          {
            PCRxCmdType=4;
            Restart_Loop[LoopNo] = TRUE;                

            Write_DeviceRecord(Build_Address(LoopNo, DeviceNo), (DeviceRecordFormat *)&PCRxBuffer[2]);

            Update_Flags[Build_Address(LoopNo, DeviceNo)].Config_Changed = TRUE;
            Update_Flags[Build_Address(LoopNo, DeviceNo)].UpdateBMS_Config = TRUE;
          }
          else
          {
            Write_DeviceRecord(Build_Address(LoopNo, DeviceNo), (DeviceRecordFormat *)&PCRxBuffer[2]);
            
            Update_Flags[Build_Address(LoopNo, DeviceNo)].Config_Changed = TRUE;
            Update_Flags[Build_Address(LoopNo, DeviceNo)].UpdateBMS_Config = TRUE;

            //device type is the same, so we can re-configure the device now
//              Update_PM_Module_Device_Config(Build_Address(LoopNo, DeviceNo), PCRxBuffer[2]);
          }
          
          //setting this flag will cause the device to be re-configured
//          Update_Flags[Build_Address(LoopNo, DeviceNo)].Updated_From_PC = TRUE;
        }
        
        Write_DeviceRecord(Build_Address(LoopNo, DeviceNo), (DeviceRecordFormat *)&PCRxBuffer[2]);
        
        PCSendAck(); 
      } else PCSendNak();
      break;
    }
  case PCSetMaintDate:
    {
      if (PCRxCmdError==FALSE) 
      {
        T_Time.RTC_Year = PCRxBuffer[0];
        T_Time.RTC_Month = PCRxBuffer[1];
        T_Time.RTC_DOM = PCRxBuffer[2];
        T_Time.RTC_Hour = PCRxBuffer[3];
        T_Time.RTC_Minute = PCRxBuffer[4];
        T_Time.RTC_Second = PCRxBuffer[5];
        
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
            PCRxCmdType=5;                   //force update of Sounders
          }
          if(Read_Cont_Tone() != TempByte1)
          {
            Write_Cont_Tone(TempByte1);
            PCRxCmdType=5;                   //force update of Sounders
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
        {
          //check to see if the Blink LED setting has changed,
          if(Read_Polling_LED_Enabled() != PCRxBuffer[2])
          {
            Write_Polling_LED_Enabled(PCRxBuffer[2]);
  
            //Mod 190116_2
            //when the setting has changed, ALL devices MUST be re-configured
            ReconfigureAllDevices = TRUE;
          }
        }
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
        CurrentTime.RTC_Year=PCRxBuffer[0];
        CurrentTime.RTC_Month=PCRxBuffer[1];
        CurrentTime.RTC_DOM=PCRxBuffer[2];
        CurrentTime.RTC_Hour=PCRxBuffer[3];
        CurrentTime.RTC_Minute=PCRxBuffer[4];
        CurrentTime.RTC_Second=PCRxBuffer[5];
        
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

        
        CE_EventData.Event_Type = PCRxBuffer[1];
        CE_EventData.Address = (PCRxBuffer[3] << 8) +
                               (PCRxBuffer[2]);
        
        CE_EventData.Trigger_Condition.Trig_Type = PCRxBuffer[4];
        CE_EventData.Trigger_Condition.Address = (PCRxBuffer[6] << 8) +
                                                 (PCRxBuffer[5]);
        
        CE_EventData.Normalise_Condition.Trig_Type = PCRxBuffer[7];
        CE_EventData.Normalise_Condition.Address = (PCRxBuffer[9] << 8) +
                                                 (PCRxBuffer[8]);
        
        Write_Special_CE_Event(ZoneNo, &CE_EventData);
        
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
    
  case PCSetTextString:
    //note this is NOT the text description of a field device.
    //it is a blue sky function for having a go at sending the MMI text strings
    //down from the PC to handle language variants etc. It has never been
    //implemented!
    break;
    
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
void Return_Data_To_PC(u8 * S_Ptr)
{
  u8 Len;
  
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
   u8 Count;
   u8 CheckSum;
  
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
void ProcessRxData(u8 SCDR)
{
   u8 Char;
  char ac[256];
  u8 i,i1;
  
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

      i1 = 0;
      i1 = sprintf(&ac[i1], "PC-->[%02X]", PCRxCmd);
      for(i = 0; i < PCRxPointer; i++)
      {
        i1 += sprintf(&ac[i1], "[%02X]", PCRxBuffer[i]);
      }
      sprintf(&ac[strlen(ac)], "\n\r");
      Trace_String(ac);
      
    }
    else PCRxCmdError=TRUE;
    
    PCRxState=0;
    break;
    
  default:
    PCRxState=0;
    break;
  }  
}


/*------------------------------------------------------------------------------------
Function Name   : RBUsed

Description     : returns the number of characters in the receiver ring buffer

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
u16 RBUsed(void)
{
  u16 c;
  
  c=((RxFP-RxEP)%RingSize);
  return(c);
}

/*------------------------------------------------------------------------------------
Function Name   : RBLeft

Description     : returns the number of free spaces in the receiver ring buffer

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
u16 RBLeft(void)
{
  u16 c;
  
  c=RingSize-RBUsed();
  return(c);
}

/*------------------------------------------------------------------------------------
Function Name   : GetRxChar

Description     : Attempts to retreive the oldest character from the ring buffer. If no
character is in, returns 0xffff

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
u16 GetRxChar(void)
/* This function returns the oldest data in the input ring buffer and
increments the Empty Pointer*/
{
  u8 x;
  
  if(RBUsed()==0)
    return 0xffff;
  
  x=RxRing[RxEP];
  RxEP=(RxEP+1)%RingSize;
  RxChecksum+=x;
  
  return(x);
}

/*------------------------------------------------------------------------------------
Function Name   : Flush_RX_Buffer

Description     : empties the receiver ring buffer by making both the fill and empty
pointers equal. 

Input args      :
Return args     :

Special Notes   : The actual data in the buffer will remain intact

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Flush_RX_Buffer(void)
{
  RxEP = RxFP = 0;
}

/*------------------------------------------------------------------------------------
Function Name   : Flush_TX_Buffer

Description     : empties the transmit ring buffer.

Input args      :
Return args     :

Special Notes   : Actually zeroes out the whole buffer

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Flush_TX_Buffer(void)
{
  PC_UART_No_Of_Bytes = 0;
  
  RS232_Finished = TRUE;
}


/*------------------------------------------------------------------------------------
Function Name   : AddRxChar

Description     : Adds the specified Data character to the receive ring buffer if there
is space

Input args      : TRUE if there was space
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
u8 AddRxChar(u8 Data)
{
  if(RBLeft()<1)
    return FALSE;
  
  RxRing[RxFP]=Data;
  RxFP=(RxFP+1) % RingSize;
  
  return TRUE;
}

/*------------------------------------------------------------------------------------
Function Name   : Send_Char

Description     : outputs the specified character to UART0

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Send_Char(u8 c)
{
  UART_PutChar(UART_0, c);
}

/*------------------------------------------------------------------------------------
Function Name   : SendTxBuffer

Description     : Starts off the transmission of the characters in the buffer
specified by S_Ptr. The 1st character is sent manually, then the
interrupt routine takes over for the remaining characters

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void SendTxBuffer(u8 * SPtr, u8 Len)
{
  char ac[512];
  u8 i, i1;
  
  i1 = 0;
  i1 = sprintf(&ac[i1], "PANEL-->");
  for(i = 0; i < Len; i++)
  {
    i1 += sprintf(&ac[i1], "[%02X]", SPtr[i]);
  }
  sprintf(&ac[strlen(ac)], "\n\r");
  Trace_String(ac);
  

  PC_UART_No_Of_Bytes = Len - 1;
  PC_UART_S_Ptr = SPtr + 1;
  
  UART_PutChar(UART_0, *SPtr);
}

/*------------------------------------------------------------------------------------
Function Name   : Send_RS232

Description     : This function sends the NULL terminated string specified

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Send_RS232(u8 * S_Ptr)
{
  u8 ch = *S_Ptr;
  
  PC_UART_No_Of_Bytes = 0;
  PC_UART_S_Ptr = S_Ptr + 1;
  
  while(*S_Ptr != NULL)                         //move the Fill pointer to the end
  {                                             //of the string
    PC_UART_No_Of_Bytes++;
    S_Ptr++;
  }

  RS232_Finished = FALSE;

  //check the incoming CTS signal, and only begin sending if it is in the 
  //correct state
  if(GPIO_ReadInputDataBit(GPIO_Port_0, GPIO_Pin_4) == Bit_RESET)
    UART_PutChar(UART_0, ch);
}

/*------------------------------------------------------------------------------------
Function Name   : Send_RS232_Counted

Description     : Sends the specified number of characters from S_Ptr

Input args      :
Return args     :

Special Notes   : 

--------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------*/
void Send_RS232_Counted(u8 * S_Ptr, u8 No_Bytes)
{
  PC_UART_No_Of_Bytes = No_Bytes - 1;
  PC_UART_S_Ptr = S_Ptr + 1;
  
  RS232_Finished = FALSE;
  UART_PutChar(UART_0, *S_Ptr);
}



void Initialise_PCComms_Hardware(void)
{  
  GPIO_InitTypeDef GPIO_InitStructure;
  
  // Assign Port 0 pins to UART0
  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2 | GPIO_Pin_3;
  GPIO_InitStructure.GPIO_PinDir = GPIO_PinDir_IP;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF1;
  GPIO_InitStructure.GPIO_Mask = GPIO_Pin_All;
  GPIO_InitStructure.GPIO_IRQ_Mode = GPIO_Mode_IRQ_None;
  GPIO_Init(GPIO_Port_0, &GPIO_InitStructure);

  //initialise PORT 0 Inputs (CTS)
  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4;
  GPIO_InitStructure.GPIO_PinDir = GPIO_PinDir_IP;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU;
  GPIO_InitStructure.GPIO_Mask = GPIO_Pin_All;
  GPIO_InitStructure.GPIO_IRQ_Mode = GPIO_Mode_IRQ_None;
  GPIO_Init(GPIO_Port_0, &GPIO_InitStructure);
  

  //set the baud rate to 9600
  UART_Init(UART_0, 9600, 8, 1,   UART_NO_PARITY);
  UART_IRQ_Enable(UART_0, UART_TxBufferEmpty_IRQ + UART_RxDataAvailable_IRQ);
  
  //install IRQ handler for Trace UART
  OS_ARM_InstallISRHandler (VIC_UART0, &_Handle_UART_0);
  OS_ARM_ISRSetPrio(VIC_UART0, 140);
  OS_ARM_EnableISR(VIC_UART0);
}

//this handler handles the UART 0 IRQs
void _Handle_UART_0(void)
{
  OS_EnterInterrupt();

  UART_RxCharTypeDef FIFO_Data;
  u32 UartIntId = U0IIR;
  u8 Counter;
  u8 RxData;
  
  // Recognize the interrupt event
  switch (UartIntId & 0xF)
  {
  case RLS_INTR_ID: // Receive Line Status
  case CDI_INTR_ID: // Character Time-out Indicator
  case RDA_INTR_ID: // Receive Data Available
    //trap CTI interrupts which can occur WITHOUT anything in the Rx FiFo!
    //so we need to clear the IRQ by a dummy read of the FiFo
    if( (U0LSR & RLS_ReceiverDataReady) == FALSE)
      RxData = U0RBR;

    while((FIFO_Data.LineStatus = U0LSR) & RLS_ReceiverDataReady) 
    {
      RxData = U0RBR;
      AddRxChar(RxData);

      BMS_AssemblePacket(RxData);
    }
    break;
    
  case THRE_INTR_ID:  // THRE Interrupt
    // Try to fill whole hardware transmit FIFO
    Counter = 16;
    while( (Counter > 0) &&
           (PC_UART_No_Of_Bytes > 0) )
    {
      U0THR = *PC_UART_S_Ptr;
      PC_UART_S_Ptr++;

      PC_UART_No_Of_Bytes--;
      Counter--;
    }
    RS232_Finished = PC_UART_No_Of_Bytes == 0;
    break;
  }

  OS_LeaveInterrupt();

}
