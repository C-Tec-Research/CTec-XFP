typedef struct
{
  u8 Type;
  u8 Shared_Data[8];
} DeviceRecordFormat;

//define the positions within Shared_Data for the various
//different fields
#define DR_SA0_ZGS 0
#define DR_SA0_Description 1
#define DR_SA1_ZGS 2
#define DR_SA1_Description 3
#define DR_SA2_ZGS 4
#define DR_SA2_Description 5
#define DR_SA3_ZGS 6
#define DR_SA3_Description 7

#define DR_Day_Sensitivity 2
#define DR_Night_Sensitivity 3
#define DR_Multi_Mode_Day 4
#define DR_CAST_PRO_Group 6
#define DR_Remote_LED 7

#define DR_Volume_Day 2
#define DR_Volume_Night 3
