unit Main;

interface

{$H+}

uses
  SysUtils, Windows, Dialogs, Forms,
  Classes, Controls, ExtCtrls, Menus,
  StdCtrls, ComCtrls, Spin, Mask, Buttons, Tabnotbk, ToolWin,
  LoopFrame, Grids, Graphics, ImgData, CheckLst, Inifiles,
  commspanel,
  PanelComms,
  Common, Registry,
  SigComport, StopDownload,
  Dependancy_Form,
  TimeSpinButton,
  Device_Selection,
  StrUtils,
  UnitSiteConfig,
  //UnitCfgFile,
  Choose_Device_Form,
  XFPPanelComms,
  //USBPanel,
  Translate_Text,
  Math,
  Companel,
  System.UITypes,
  SigFile,
  ProtocolButtons,
  AdvancedCommsSelDlg,
  Themes;
    //AntiFlicker;

type
  TDataType = (dtNone, dtLoadLoop1, dtLoadLoop2, dtLoadZoneList, dtLoadZoneNameOnly,
    dtLoadZoneNonFire, dtLoadGroupList, dtLoadSetList, dtLoadSite, dtLoadRepeaterList,
    dtCheckNVM, dtSavePName1, dtSavePData1, dtSavePName2, dtSavePData2, dtSaveZoneName,
    dtSaveZoneNonFire, dtSaveZoneTimer, dtSaveZoneNameOnly, dtZoneNonFire, dtSaveZGroup,
    dtSaveZoneSet, dtSaveMaintName, dtSaveMaintDate, dtSaveAL2Code, dtSaveAL3Code,
    dtSaveFaultLockoutTime, dtSavePCTime, dtSavePhasedSettings, dtSaveRepeaterName,
    dtSaveRepeaterFitted, dtSaveSegmentNumber, dtSavePanelName, dtSaveOutputName,
    dtSaveOutputFitted, dtLoadEventHistory, dtSaveQuiesName, dtSaveDayNight, dtLoadPName,
    dtSaveCE_Event, dtLoadCE_Event, dtLoadNetworkData, dtSaveNetworkData, dtLoadFlashMemory,
    dtSaveFlashMemory);

  TCTecSpinEdit = class(TTimeSpinButton)
  end;

  TMainForm = class(TForm)
    MainMenu: TMainMenu;
    HelpContentsItem: TMenuItem;
    HelpAboutItem: TMenuItem;
    SpeedBar: TPanel;
    btnNew: TSpeedButton;
    btnOpen: TSpeedButton;
    btnSave: TSpeedButton;
    btnSaveAs: TSpeedButton;
    btnLoadAllFromPanel: TSpeedButton;
    btnSaveAllToPanel: TSpeedButton;
    Setting1: TMenuItem;
    SelectCommPort1: TMenuItem;
    RetryCountLab: TLabel;
    N5: TMenuItem;
    MenuAppolloSystem: TMenuItem;
    btnPrint: TSpeedButton;
    btnLoadFromPanel: TButton;
    btnSavetoPanel: TButton;
    N4: TMenuItem;
    Disconnect1: TMenuItem;
    ClearLogButton: TBitBtn;
    LoadLogButton: TBitBtn;
    SaveLogButton: TBitBtn;
    ConfigPages: TPageControl;
    tabLoopSummary: TTabSheet;
    lstGroups: TListBox;
    sgrdZone: TStringGrid;
    radgrpZoneGroup: TRadioGroup;
    stbarLoopInfo: TStatusBar;
    tabLoop1: TTabSheet;
    frmLoop1: TfrmLoop;
    tabLoop2: TTabSheet;
    frmLoop2: TfrmLoop;
    tabZoneCfg: TTabSheet;
    lblZoneDescription: TLabel;
    lblTimerDelay: TLabel;
    lblTimerSounders: TLabel;
    lblTimerRemoteOP: TLabel;
    lblWithMCPs: TLabel;
    lblFunctioningWith: TLabel;
    lblWithDetectors: TLabel;
    tabZoneConfig: TTabSheet;
    bvlGroupConfig: TBevel;
    lblGroupConfigurationGrid: TLabel;
    dwgGroupConfig: TDrawGrid;
    GroupConfigBar: TToolBar;
    btnSounderOff: TToolButton;
    btnSounderCont: TToolButton;
    btnSounderPulsed: TToolButton;
    btnGrpConfigSep1: TToolButton;
    btnSetAllOff: TBitBtn;
    ToolButton16: TToolButton;
    btnSetAllCont: TBitBtn;
    ToolButton17: TToolButton;
    btnSetAllPulsed: TBitBtn;
    tabNotUsedNow: TTabSheet;
    pnlGroupSounderConfig: TPanel;
    tabSiteCfg: TTabSheet;
    pnlSiteConfig: TPanel;
    pnlPanelModes: TPanel;
    pnlMaintenance: TPanel;
    lblMaintenanceString: TLabel;
    lblMaintenanceDate: TLabel;
    txtMaintString: TEdit;
    txtMaintDate: TMaskEdit;
    pnlLockoutTime: TPanel;
    UploadTimeSelect: TCheckBox;
    pnlClient_Install_Info: TPanel;
    lblServiceNo: TLabel;
    lblEngineer: TLabel;
    lblClientName: TLabel;
    lblClientAdd: TLabel;
    lblPanelLocation: TLabel;
    lblInstallName: TLabel;
    lblInstallAdd: TLabel;
    lblSoftwareVersionNo: TLabel;
    txtServiceNo: TEdit;
    txtEngineer: TEdit;
    txtClientName: TEdit;
    txtClientAdd1: TEdit;
    txtClientAdd2: TEdit;
    txtClientAdd3: TEdit;
    txtClientAdd4: TEdit;
    txtPanelLocation: TEdit;
    txtInstallName: TEdit;
    txtInstallAdd1: TEdit;
    txtInstallAdd2: TEdit;
    txtInstallAdd3: TEdit;
    txtInstallAdd4: TEdit;
    txtInstallAdd5: TEdit;
    txtClientAdd5: TEdit;
    pnlMainVersion: TPanel;
    stxtMain: TStaticText;
    tabNetworkCfg: TTabSheet;
    RepeaterPanel: TPanel;
    pnlRepeaterConfig: TPanel;
    lblRepeaterAdd1: TLabel;
    lblRepeaterName: TLabel;
    lblRepeaterFitted: TLabel;
    lblRepeaterAdd: TLabel;
    lblRepeaterAdd2: TLabel;
    lblRepeaterAdd3: TLabel;
    lblRepeaterAdd4: TLabel;
    lblRepeaterAdd5: TLabel;
    lblRepeaterAdd6: TLabel;
    lblRepeaterAdd7: TLabel;
    lblRepeaterAdd8: TLabel;
    bvlRepeaterConfigurationSeparator: TBevel;
    PanelName1: TEdit;
    PanelName2: TEdit;
    PanelName3: TEdit;
    PanelName4: TEdit;
    PanelName5: TEdit;
    PanelName6: TEdit;
    PanelName7: TEdit;
    PanelName8: TEdit;
    RepFitted1: TCheckBox;
    RepFitted2: TCheckBox;
    RepFitted3: TCheckBox;
    RepFitted4: TCheckBox;
    RepFitted5: TCheckBox;
    RepFitted6: TCheckBox;
    RepFitted7: TCheckBox;
    RepFitted8: TCheckBox;
    tabViewLog: TTabSheet;
    pnlLog: TPanel;
    EventLog: TRichEdit;
    tabComment: TTabSheet;
    pnlComments: TPanel;
    CommentsEdit: TRichEdit;
    tabSetConfig: TTabSheet;
    dwgSetsGrid: TDrawGrid;
    pnlSetsTitle: TPanel;
    stxtCurrentTime: TStaticText;
    stxtCurrentDate: TStaticText;
    Label1: TLabel;
    Label2: TLabel;
    txtQuiesString: TEdit;
    Label3: TLabel;
    UpDown1: TUpDown;
    UpDown2: TUpDown;
    UpDown3: TUpDown;
    UpDown4: TUpDown;
    Label4: TLabel;
    Label5: TLabel;
    TSetsControl: TToolBar;
    ToolButton21: TToolButton;
    ToolButton19: TToolButton;
    ToolButton18: TToolButton;
    btnClearAllSet: TToolButton;
    ToolButton20: TToolButton;
    scrlZoneCfg: TScrollBox;
    bvlZoneDesc: TBevel;
    lblZPZone1: TLabel;
    bvlFunctioningWith: TBevel;
    txtZPZone1: TEdit;
    chkZPDetector1: TCheckBox;
    chkZPMCP1: TCheckBox;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    txtZPDependancy1: TEdit;
    spnZPSounder1: TTimeSpinButton;
    spnZPOutput1: TTimeSpinButton;
    spnZPRelay11: TTimeSpinButton;
    spnZPRelay21: TTimeSpinButton;
    HochikiProtocol1: TMenuItem;
    SpeedButton1: TSpeedButton;
    Device_Selection_Frame1: TDevice_Selection_Frame;
    grpSummaryOptions: TGroupBox;
    radgrpLoop: TRadioGroup;
    sgrdLoop: TStringGrid;
    Timer1: TTimer;
    tabC_E_Events: TTabSheet;
    CEPanel1: TPanel;
    Panel1: TPanel;
    chkSetSil1: TCheckBox;
    chkSetSil2: TCheckBox;
    chkSetSil3: TCheckBox;
    chkSetSil4: TCheckBox;
    chkSetSil5: TCheckBox;
    chkSetSil6: TCheckBox;
    chkSetSil7: TCheckBox;
    chkSetSil8: TCheckBox;
    chkSetSil9: TCheckBox;
    chkSetSil10: TCheckBox;
    chkSetSil11: TCheckBox;
    chkSetSil12: TCheckBox;
    chkSetSil13: TCheckBox;
    chkSetSil14: TCheckBox;
    chkSetSil15: TCheckBox;
    chkSetSil16: TCheckBox;
    chkSetSil17: TCheckBox;
    chkSetSil18: TCheckBox;
    Label16: TLabel;
    Label17: TLabel;
    updReCal: TUpDown;
    lblReCal: TLabel;
    txtNight_Begin: TMaskEdit;
    txtNight_End: TMaskEdit;
    txtReCal: TMaskEdit;
    CEPanel: TScrollBox;
    Device_Selection2CE1: TComboBox;
    Loop31: TComboBox;
    Device_Selection3CE1: TComboBox;
    Panel4: TPanel;
    Label11: TLabel;
    EventNo: TLabel;
    lblAND1CE1: TLabel;
    AND_Event2CE1: TComboBox;
    lblAND2CE1: TLabel;
    Timer2: TTimer;
    Panel3: TPanel;
    Label13: TLabel;
    lblT1: TLabel;
    Label19: TLabel;
    T1: TMaskEdit;
    Label20: TLabel;
    PanelComms: TXFPPanelComms;
    PanelNumber: TSpinEdit;
    spnPanelSounder1: TSpinEdit;
    spnPanelSounder2: TSpinEdit;
    Label21: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    PanelNameEdit: TEdit;
    Panel5: TPanel;
    Label37: TLabel;
    Label36: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    Label30: TLabel;
    Label31: TLabel;
    Label32: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    Label35: TLabel;
    CheckBox1: TCheckBox;
    CheckBox9: TCheckBox;
    CheckBox17: TCheckBox;
    CheckBox25: TCheckBox;
    CheckBox2: TCheckBox;
    CheckBox10: TCheckBox;
    CheckBox18: TCheckBox;
    CheckBox26: TCheckBox;
    CheckBox3: TCheckBox;
    CheckBox11: TCheckBox;
    CheckBox19: TCheckBox;
    CheckBox27: TCheckBox;
    CheckBox4: TCheckBox;
    CheckBox12: TCheckBox;
    CheckBox20: TCheckBox;
    CheckBox28: TCheckBox;
    CheckBox5: TCheckBox;
    CheckBox13: TCheckBox;
    CheckBox21: TCheckBox;
    CheckBox29: TCheckBox;
    CheckBox6: TCheckBox;
    CheckBox14: TCheckBox;
    CheckBox22: TCheckBox;
    CheckBox30: TCheckBox;
    CheckBox7: TCheckBox;
    CheckBox15: TCheckBox;
    CheckBox23: TCheckBox;
    CheckBox31: TCheckBox;
    CheckBox8: TCheckBox;
    CheckBox16: TCheckBox;
    CheckBox24: TCheckBox;
    CheckBox32: TCheckBox;
    Label38: TLabel;
    Label39: TLabel;
    DBG_Panel1: TPanel;
    ComboBox1: TComboBox;
    Edit1: TEdit;
    Edit2: TEdit;
    Edit3: TEdit;
    Edit4: TEdit;
    Edit5: TEdit;
    Edit6: TEdit;
    tabLanguage: TTabSheet;
    ReadFlashBtn: TButton;
    WriteStrings: TButton;
    LoadTexts: TButton;
    SaveTexts: TButton;
    TextStrings: TStringGrid;
    Memo1: TMemo;
    StatusBar1: TStatusBar;
    SpinEdit1: TSpinEdit;
    Delayed: TToolButton;
    btnDelayTime: TTimeSpinButton;
    Label40: TLabel;
    NightCB1: TCheckBox;
    NightCB2: TCheckBox;
    NightCB3: TCheckBox;
    NightCB4: TCheckBox;
    NightCB5: TCheckBox;
    NightCB6: TCheckBox;
    NightCB7: TCheckBox;
    Label41: TLabel;
    Label42: TLabel;
    Label43: TLabel;
    Label44: TLabel;
    Label45: TLabel;
    Label46: TLabel;
    Label47: TLabel;
    Label48: TLabel;
    DayCB1: TCheckBox;
    DayCB2: TCheckBox;
    DayCB3: TCheckBox;
    DayCB4: TCheckBox;
    DayCB5: TCheckBox;
    DayCB6: TCheckBox;
    DayCB7: TCheckBox;
    Panel2: TPanel;
    chkEnableZoneChanges: TCheckBox;
    Label49: TLabel;
    Label10: TLabel;
    spnInvestigation: TTimeSpinButton;
    Label6: TLabel;
    spnInvestigation1: TTimeSpinButton;
    lblPhasedDelayTime: TLabel;
    PhasedDelaySpin: TTimeSpinButton;
    Label50: TLabel;
    Label51: TLabel;
    Label52: TLabel;
    Label53: TLabel;
    Label54: TLabel;
    pnlAccessCodes: TPanel;
    lblAccessLvl2: TLabel;
    txtLvl2: TEdit;
    lblAccessLvl3: TLabel;
    txtLvl3: TEdit;
    SendToPanel: TCheckBox;
    Label55: TLabel;
    Device_Selection1CE1: TComboBox;
    Label12: TLabel;
    Label15: TLabel;
    Loop21: TComboBox;
    AND_Event1CE1: TComboBox;
    Label14: TLabel;
    lblT_F1CE1: TLabel;
    lblT_F2CE1: TLabel;
    Bevel1: TBevel;
    Bevel2: TBevel;
    Bevel3: TBevel;
    T_F1CE1: TComboBox;
    T_F2CE1: TComboBox;
    Edit7: TEdit;
    Edit8: TEdit;
    Edit9: TEdit;
    Label56: TLabel;
    Label57: TLabel;
    Keyswitch: TCheckBox;
    Timer3: TTimer;
    Int_Tone_Box: TComboBox;
    Cont_Tone_Box: TComboBox;
    Int_Label: TLabel;
    Cont_Label: TLabel;
    BSGroupBox: TComboBox;
    ZGSGroupBox: TComboBox;
    chkPolling_LED: TCheckBox;
    pnlMCP_Confirmation: TPanel;
    spnMCP_Delay: TSpinEdit;
    spnIO_Delay: TSpinEdit;
    spnDetector_Delay: TSpinEdit;
    A: TLabel;
    Label58: TLabel;
    Label59: TLabel;
    Label60: TLabel;
    chkReSound: TCheckBox;
    Label61: TLabel;
    Label62: TLabel;
    Label63: TLabel;
    Label64: TLabel;
    CheckBox33: TCheckBox;
    CheckBox34: TCheckBox;
    CheckBox35: TCheckBox;
    CheckBox36: TCheckBox;
    CheckBox37: TCheckBox;
    CheckBox38: TCheckBox;
    CheckBox39: TCheckBox;
    CheckBox40: TCheckBox;
    Cont_Tone_Box_Apollo: TComboBox;
    Int_Tone_Box_Apollo: TComboBox;
    BST_AdjustSelect: TCheckBox;
    chkZPEndDelays1: TCheckBox;
    Label65: TLabel;
    Label66: TLabel;
    Panel7: TPanel;
    Loop11: TComboBox;
    Panel8: TPanel;
    chkRealTime_Event_Output: TCheckBox;
    MenuCASTSystem: TMenuItem;
    Edit10: TEdit;
    Edit11: TEdit;
    SpinEdit2: TSpinEdit;
    SerialNo3: TSpinEdit;
    SerialNo2: TSpinEdit;
    SerialNo1: TSpinEdit;
    SerialNo0: TSpinEdit;
    Button1: TButton;
    Import1: TMenuItem;
    OldStyleFile1: TMenuItem;
    Cont_Tone_Box_CAST: TComboBox;
    Int_Tone_Box_CAST: TComboBox;
    bvlZoneCfgHeader: TBevel;
    ToolBar1: TToolBar;
    pnlOther: TPanel;
    procedure Button1Click(Sender: TObject);

    procedure Loop11CloseUp(Sender: TObject);

    procedure Cont_Tone_Box_ApolloChange(Sender: TObject);
    procedure BST_AdjustSelectClick(Sender: TObject);
    procedure RealTime_Event_OutputClick(Sender: TObject);
    procedure chkReSoundClick(Sender: TObject);
    procedure spnDetector_DelayChange(Sender: TObject);
    procedure spnIO_DelayChange(Sender: TObject);
    procedure spnMCP_DelayChange(Sender: TObject);
    procedure ChangeProtocolWarning( const pNewProtocol : TSystemType; const pProtocolText : string );
    procedure changeToApollo(Sender: TObject);
    procedure changeToNittan(Sender: TObject);
    procedure changeToSysSensor(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure cboReportTypeChange(Sender: TObject);
    procedure chkDetectorClick(Sender: TObject);
    procedure chkMCPClick(Sender: TObject);
    procedure chkZPEndDelays1Click(Sender: TObject);
    procedure txtZoneChange(Sender: TObject);
    procedure radgrpLoopClick(Sender: TObject);
    procedure sgrdLoopSelectCell(Sender: TObject; ACol, ARow: Integer;
      var CanSelect: Boolean);
    procedure radgrpZoneGroupClick(Sender: TObject);
    procedure Z01G01Enter(Sender: TObject);
    procedure Z01G01Exit(Sender: TObject);
    procedure txtMaintDateExit(Sender: TObject);
    procedure sgrdZoneSelectCell(Sender: TObject; ACol, ARow: Integer;
      var CanSelect: Boolean);
    procedure sgrdZoneSetEditText(Sender: TObject; ACol,
      ARow: Integer; const Value: String);
    procedure sgrdZoneKeyPress(Sender: TObject; var Key: Char);
    procedure OpenInifile(Sender: TObject);
    procedure btnLoadAllFromPanelClick(Sender: TObject);
    procedure PanelCommsTick(Sender: TObject);
    procedure PanelCommsPacketReceived(Sender: TObject; CmdChar: Char;
      Packet: String);
    procedure PanelCommsTimeOut(Sender: TObject);
    procedure btnLoadFromPanelClick(Sender: TObject);
    procedure PanelCommsInvalidChecksum(Sender: TObject);
    procedure btnLoadLoop1Click(Sender: TObject);
    procedure btnLoadLoop2Click(Sender: TObject);
    procedure SelectCommPort1Click(Sender: TObject);
    procedure PanelCommsHandshakeReceived(Sender: TObject);
    procedure btnLoadZoneConfigClick(Sender: TObject);
    procedure btnLoadSetConfigClick(Sender: TObject);
    procedure btnLoadSiteConfigClick(Sender: TObject);
    procedure btnLoadNetworkConfigClick(Sender: TObject);
    procedure PanelCommsAck(Sender: TObject);
    procedure btnSaveSiteConfigClick(Sender: TObject);
    procedure btnSavetoPanelClick(Sender: TObject);
    procedure btnSaveLoop1Click(Sender: TObject);
    procedure btnSaveLoop2Click(Sender: TObject);
    procedure btnSaveZoneConfigClick(Sender: TObject);
    procedure btnSaveSetConfigClick(Sender: TObject);
    procedure btnSaveCE_DataClick(Sender: TObject);
    procedure btnSaveNetworkConfigClick(Sender: TObject);
    procedure PanelName1Change(Sender: TObject);
    procedure RepFitted1Click(Sender: TObject);
    procedure btnSaveAllToPanelClick(Sender: TObject);
    procedure PanelCommsNak(Sender: TObject);
    procedure txtClientNameChange(Sender: TObject);
    procedure txtClientAdd1Change(Sender: TObject);
    procedure txtInstallAdd1Change(Sender: TObject);
    procedure txtPanelLocationChange(Sender: TObject);
    procedure txtEngineerChange(Sender: TObject);
    procedure txtServiceNoChange(Sender: TObject);
    procedure txtMaintStringChange(Sender: TObject);
    procedure txtQuiesStringChange(Sender: TObject);
    procedure Z1EditChange(Sender: TObject);
    procedure ZOP1FittedClick(Sender: TObject);
    procedure chkNonFireClick(Sender: TObject);
    procedure btnSounderOffClick(Sender: TObject);
    procedure btnSounderContClick(Sender: TObject);
    procedure btnSounderPulsedClick(Sender: TObject);
    procedure dwgGroupConfigDrawCell(Sender: TObject; ACol, ARow: Integer;
      Rect: TRect; State: TGridDrawState);
    procedure dwgGroupConfigSelectCell(Sender: TObject; ACol,
      ARow: Integer; var CanSelect: Boolean);
    procedure dwgGroupConfigMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure btnSetAllOffClick(Sender: TObject);
    procedure btnLoadGroupConfigClick(Sender: TObject);
    procedure btnSaveGroupConfigClick(Sender: TObject);
    procedure frmLoop1dwgLoopSelectCell(Sender: TObject; ACol,
      ARow: Integer; var CanSelect: Boolean);
    procedure frmLoop2dwgLoopSelectCell(Sender: TObject; ACol,
      ARow: Integer; var CanSelect: Boolean);
    procedure txtInstallNameChange(Sender: TObject);
    procedure txtMaintDateChange(Sender: TObject);
    procedure UploadTimeSelectClick(Sender: TObject);
    procedure txtLvl2Change(Sender: TObject);
    procedure txtLvl3Change(Sender: TObject);
    procedure PanelNameEditChange(Sender: TObject);
    procedure btnPrintClick(Sender: TObject);
    procedure SaveLogButtonClick(Sender: TObject);
    procedure LoadLogButtonClick(Sender: TObject);
    procedure ClearLogButtonClick(Sender: TObject);
    procedure HelpAboutItemClick(Sender: TObject);
    procedure txtLvl2KeyPress(Sender: TObject; var Key: Char);
    procedure PrinterPropertiesClick(Sender: TObject);
    procedure ConfigPagesChange(Sender: TObject);
    procedure txtMaintDateKeyPress(Sender: TObject; var Key: Char);
    procedure sgrdLoopKeyPress(Sender: TObject; var Key: Char);
    procedure HelpContentsItemClick(Sender: TObject);
    procedure txtLvl3KeyPress(Sender: TObject; var Key: Char);
    procedure txtLvl3Exit(Sender: TObject);
    procedure txtLvl2Exit(Sender: TObject);
    procedure ZSounderExit(Sender: TObject);
    procedure PhasedDelaySpinExit(Sender: TObject);
    procedure ZRemoteExit(Sender: TObject);
    procedure Disconnect1Click(Sender: TObject);
    procedure ConfigPagesChanging(Sender: TObject;
      var AllowChange: Boolean);
    procedure PanelCommsInput(Sender: TObject; ConvertedString: String);
    procedure PanelCommsOutputTimeout(Sender: TObject);
    procedure txtLvl2Enter(Sender: TObject);
    procedure chkEnableZoneChangesClick(Sender: TObject);
    procedure stxtMainDblClick(Sender: TObject);
    procedure DrawGrid1SelectCell(Sender: TObject; ACol, ARow: Integer;
      var CanSelect: Boolean);
    procedure DrawGrid1DrawCell(Sender: TObject; ACol, ARow: Integer;
      Rect: TRect; State: TGridDrawState);
    procedure dwgSetsGridMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure Timer1Timer(Sender: TObject);
    procedure UpDown1Click(Sender: TObject; Button: TUDBtnType);
    procedure UpDown2Click(Sender: TObject; Button: TUDBtnType);
    procedure UpDown3Click(Sender: TObject; Button: TUDBtnType);
    procedure UpDown4Click(Sender: TObject; Button: TUDBtnType);
    procedure updReCalClick(Sender: TObject; Button: TUDBtnType);
    procedure btnSetButtonsClick(Sender: TObject);
    procedure txtZPDependancy1Click(Sender: TObject);
    procedure CreateZonePage;
    procedure CreateC_EPage;
    procedure chkSetSil13Click(Sender: TObject);
    procedure changeToHochiki(Sender: TObject);
    procedure changeToCAST(Sender: TObject);

    procedure Loop11Change(Sender: TObject);
    procedure txtNight_BeginChange(Sender: TObject);
    procedure txtNight_EndChange(Sender: TObject);
    procedure txtReCalChange(Sender: TObject);
    procedure spnZPSounder1Change(Sender: TObject);
    procedure spnZPOutput1Change(Sender: TObject);
    procedure spnZPRelay11Change(Sender: TObject);
    procedure spnZPRelay21Change(Sender: TObject);
    procedure Device_Selection1CE1Change(Sender: TObject);
    procedure Loop21Change(Sender: TObject);
    procedure Loop21DropDown(Sender: TObject);
    procedure Device_Selection2CE1Change(Sender: TObject);
    procedure Loop31Change(Sender: TObject);
    procedure Loop31DropDown(Sender: TObject);
    procedure Device_Selection3CE1Change(Sender: TObject);
    procedure AND_Event1CE1Change(Sender: TObject);
    procedure AND_Event2CE1Change(Sender: TObject);
    procedure lblT_F1CE1Click(Sender: TObject);
    procedure lblT_F2CE1Click(Sender: TObject);
    procedure Timer2Timer(Sender: TObject);
    procedure Device_Selection1CE1DblClick(Sender: TObject);
    procedure Device_Selection2CE1DblClick(Sender: TObject);
    procedure Device_Selection3CE1DblClick(Sender: TObject);
    procedure T1Change(Sender: TObject);
    procedure spnPanelSounder1Change(Sender: TObject);
    procedure spnPanelSounder2Change(Sender: TObject);
    procedure PanelNumberChange(Sender: TObject);
    procedure CheckBox1Click(Sender: TObject);
    procedure CheckBox9Click(Sender: TObject);
    procedure CheckBox17Click(Sender: TObject);
    procedure CheckBox25Click(Sender: TObject);
    procedure CheckBox33Click(Sender: TObject);
    procedure PhasedDelaySpinChange(Sender: TObject);
    procedure spnInvestigationChange(Sender: TObject);
    procedure spnInvestigation1Change(Sender: TObject);
    procedure sgrdLoopDblClick(Sender: TObject);
    procedure ReadFlashBtnClick(Sender: TObject);
    procedure WriteStringsClick(Sender: TObject);
    procedure LoadTextsClick(Sender: TObject);
    procedure SaveTextsClick(Sender: TObject);
    procedure TextStringsSelectCell(Sender: TObject; ACol,
              ARow: Integer; var CanSelect: Boolean);
    procedure NightCB1Click(Sender: TObject);
    procedure DayCB1Click(Sender: TObject);
    procedure Edit7Click(Sender: TObject);
    procedure sgrdLoopExit(Sender: TObject);
    procedure KeyswitchClick(Sender: TObject);
    procedure Timer3Timer(Sender: TObject);
    procedure BSGroupBoxChange(Sender: TObject);
    procedure ZGSGroupBoxChange(Sender: TObject);
    procedure ZGSGroupBoxClick(Sender: TObject);
    procedure chkPolling_LEDClick(Sender: TObject);


    procedure ReadSerialNumber;
    procedure IncrementSerialNumber;
    procedure Import1Click(Sender: TObject);
    procedure Int_Tone_BoxChange(Sender: TObject);
    procedure Cont_Tone_BoxChange(Sender: TObject);
    procedure Int_Tone_Box_ApolloChange(Sender: TObject);
    procedure CommentsEditChange(Sender: TObject);
    procedure btnNewClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure btnSaveClick(Sender: TObject);
    procedure btnSaveAsClick(Sender: TObject);
    procedure Cont_Tone_Box_CASTChange(Sender: TObject);
    procedure Int_Tone_Box_CASTChange(Sender: TObject);

  private
    procedure LoadProtocol_Buttons;

    procedure BeginUpdate;
    procedure EndUpdate;

   private
    //FAntiFlicker : TAntiFlicker;
    FShowFittedOnly: Boolean;
    FLoop: integer;
    FCurrentRow: integer;
    FZoneGroupIndex: integer;
    FZoneModified: Boolean;
    FIniFilename: String;
    //Inifile: TInifile;
    //FModified: Boolean;
    FDataType: TDataType;
    FDataCount: integer;
    FLoadAll: Boolean;
    FLoadLoopType: Boolean;
    FSaveLoopType: Boolean;
    FSaveAll: Boolean;
    FDataRetry: integer;
    FRecordSent: string;
    FSounderSelected: integer;
    FSetSelected: integer;
    FEELinkFitted: Boolean;
    FCheckNVM: Boolean;
    FCodeOK: Boolean;
    //FNextData : TDataType;
    FEvent_Clear : integer;
    ConvertFromAFP : boolean;
    VNeedToSendTexts : boolean;
    //VNoOfLoops : integer;
    fProtocolButtonsFile: TProtocolButtonsFile;

    procedure OnNew( Sender : TObject );  // after new file
    procedure OnLoad(Sender : TObject; var pOK : boolean; const pFileName : string );  // after new file
    procedure OnDirtyChange( const pChangedObject : TSigBaseProperty; const NewState : boolean );

    procedure ShowSummaryByDevice( const frmLoop : TFrmLoop );
    procedure ShowDeviceProperties( const frmLoop : TFrmLoop; iIndex: integer; Building_List : boolean);
    function GetGridDevIndex( const frmLoop : TFrmLoop; ARow: integer): integer;
    procedure UpdateZoneGroupList;
    procedure LoadConfigData( const IniFile : TIniFile );
    //procedure SaveConfigData;
    procedure UpdateConfigData;
    procedure LoadLoopInfo(const IniFile : TIniFile; const pLoop: TAFPLoop; Loop: integer; LoopType: integer);
    //procedure SaveLoopInfo(frmLoop: TfrmLoop; Loop: integer; Looptype: integer);
    function ValidFileType( const IniFile : TIniFile ): Boolean;
    //procedure SetModified(const Value: Boolean);
    procedure SetDataRetry(const Value: integer);
    procedure SetRecordSent(const Value: string);
    procedure SetDataType(const Value: TDataType);
    //procedure ClearInfo;
    procedure Update_Description_Indices( const frmLoop : TFrmLoop );
    procedure SetNoOfLoops(Value : integer);

    procedure UpdateNetworkConfigPage;
    procedure UpdateSetConfigPage;
    procedure UpdateLoop1Page;
    procedure UpdateLoop2Page;
    procedure UpdateZoneConfigPage;
    procedure UpdateGroupConfigPage;
    procedure UpdateLoopSummaryPage;
    procedure UpdateSiteConfigPage;
    procedure RefreshC_EPage;


    function NormaliseDependancy(Inp: char): integer;
    function NormaliseDependancy_Out(Inp: integer): char;
    function Load_Integer(CmdData : string; iIndex : integer): integer;
    function Save_Integer(In_Value : integer): string;
    //function ConstructFileName(FileName : string) : string;
    function GetC_Table: TC_ETable;
    function GetXFP_Network: TXFP_Network;
    function GetSiteFile: TSiteFile;
    function GetNoOfLoops: integer;
    function GetMax_OutputDelay: integer;
    function GetCAST_Protocol_Available: boolean;

  protected
    procedure ShowSummaryByDeviceType( const frmLoop : TFrmLoop );
    procedure ShowSummaryByZone( const frmLoop : TFrmLoop );
    property Loop: integer read FLoop write FLoop;
    property CurrentRow: integer read FCurrentRow write FCurrentRow;
    property ZoneGroupIndex: integer read FZoneGroupIndex write FZoneGroupIndex;

    function RemapOldDevType(OldType: integer; LoopType: integer): integer;
    function RemapNewDevType(NewType: integer; LoopType: integer): integer;

    { These procedures are used to display the loop summary }
    procedure UpdateZoneDefs;
    procedure UpdateLoopSummary;
    property ZoneModified: Boolean read FZoneModified write FZoneModified;
//    property Modified: Boolean read FModified write SetModified;
    property LoadAll: Boolean read FLoadAll write FLoadAll;
    property SaveAll: Boolean read FSaveAll write FSaveAll;
    property CheckNVM: Boolean read FCheckNVM write FCheckNVM;
    property EELinkFitted: Boolean read FEELinkFitted write FEELinkFitted;
    property Event_Clear : integer read FEvent_Clear write FEvent_Clear;

    function CheckVersionDependancy(): boolean;

    { These procedures are used to load info from the AFP panel }
    procedure EstablishComms;
    procedure AllowDownloads (CmdData: string);
    function BuildXFerString( const pPreamble : char; const pPayload : string ) : string;
    procedure LoadInfo;
    procedure LoadMainVersion (CmdData: string);
    procedure LoadZGroup (CmdData: string);
    procedure LoadPointName (CmdData: string);
    procedure LoadPointData (CmdData: string);
    procedure LoadLoopType (CmdData: string);
    procedure LoadRepeaterName (CmdData: string);
    procedure LoadRepeaterFitted (CmdData: string);
    procedure LoadNetworkData(CmdData: string);
    procedure LoadOutputName (CmdData: string);
    procedure LoadOutputFitted (CmdData: string);
    procedure LoadSegNo (CmdData: string);
    procedure LoadPanelName (CmdData: string);
    procedure LoadMaintName (CmdData: string);
    procedure LoadMaintDate (CmdData: string);
    procedure LoadAL2Code (CmdData: string);
    procedure LoadAL3Code (CmdData: string);
    procedure LoadFaultLockoutTime (CmdData: string);
    procedure LoadPhasedSettings (CmdData: string);
    procedure LoadZoneName (CmdData: string);
    procedure LoadZoneTimers (CmdData: string);
    procedure LoadZoneNonFire(CmdData: string);
    procedure LoadZoneSet(CmdData: string);
    procedure LoadQuiesName (CmdData: string);
    procedure LoadDayNightSettings (CmdData: string);
    procedure LoadEventHistory (CmdData: string);
    procedure LoadC_EData (CmdData: string);
    procedure LoadFlashData (CmdData: string);

    procedure SaveLoopType;
    procedure SavePointName;
    procedure SavePointData (Loop: integer);
    procedure SaveZName;
    procedure SaveZNonFire;
    procedure SaveZTimer;
    procedure SaveZGroup;
    procedure SaveZoneSet;
    procedure SaveMaintText;
    procedure SaveQuiescentName;
    procedure SaveMaintDate;
    procedure SaveAL2Code;
    procedure SaveAL3Code;
    procedure SaveFaultLockoutTime;
    procedure SavePCTime;
    procedure SavePhasedSettings;
    procedure SaveDayNight;
    procedure SaveRepeaterNames;
    procedure SaveRepeaterFitted;
    procedure SaveSegmentNumber;
    Procedure SaveNetworkData;
    procedure SavePanelName;
    procedure SaveOutputName;
    procedure SaveOutputFitted;
    procedure SaveC_EData;
    procedure SaveText;

    procedure ShowEventArray;
    Procedure UpdateSubNames;

    property DataCount: integer read FDataCount;
    { Procedures to recover from invalid checksum or time out }
    property DataRetry: integer read FDataRetry write SetDataRetry;
    property RecordSent: string read FRecordSent write SetRecordSent;

    property SounderSelected: integer read FSounderSelected write FSounderSelected;
    property SetSelected: integer read FSetSelected write FSetSelected;
    property C_ETable : TC_ETable
             read GetC_Table;
    property XFP_Network : TXFP_Network read GetXFP_Network;

  public
    //frmLoop: TfrmLoop;

    property SiteFile : TSiteFile
             read GetSiteFile;
    property ProtocolButtonsFile : TProtocolButtonsFile
             read fProtocolButtonsFile;

    property ShowFittedOnly: Boolean read FShowFittedOnly write FShowFittedOnly;
    property Inifilename: String read FIniFilename write FIniFilename;
    property DataType: TDataType read FDataType write SetDataType;
    //property Modified: Boolean read FModified write SetModified;
    property NeedToSendTexts: Boolean read VNeedToSendTexts write VNeedToSendTexts;
    property NoOfLoops: integer read GetNoOfLoops write SetNoOfLoops;
    property Max_OutputDelay : integer read GetMax_OutputDelay;
    property CAST_Protocol_Available : boolean read GetCAST_Protocol_Available;

    //procedure ChangeProtocol;
    procedure RefreshProtocolControls;
    procedure RefreshAllPages;
    procedure RefreshControls;
    procedure Update_Device_Names_Database;

    procedure FormInitialise;

  end;

var
  MainForm: TMainForm;
  Device_Type : Integer;
  Device_Number : Integer;
  Max_OutputDelay : integer;

  SerialNumber : array[0..3] of integer;

implementation

uses
  Constants, CommsStatus,
  //CommSel,
  PrintSel, about, Devconfig,
  DeviceNameForm,
  Delay_Time_Prompt, Version_Dependancy_Form;


{$R *.DFM}

{ TMainForm }

procedure TMainForm.SetNoOfLoops(Value : integer);
begin
  SiteFile.NoOfLoops := Value;
  if NoOfLoops = 1 then
  begin
    tabLoop2.TabVisible := FALSE;
    if radgrpLoop.Items.Count > 1 then
      radgrpLoop.Items.Delete(1);
  end else
  begin
    tabLoop2.TabVisible := TRUE;
    if radgrpLoop.Items.Count < 2 then
      radgrpLoop.Items.Add('Loop 2');
  end;



end;

(*
function TMainForm.ConstructFileName(FileName : string) : string;
var
   FName : string;
   p : integer;
begin

    FName := FileName;

    if Pos('_Panel', Filename) = 0 then
    begin
      FName := ExtractFileName(FileName);
      p := Pos('.', FName);
      FName := LeftStr(Fname, p-1);
      FName := FName + '_Panel' + IntToStr(PanelNumber.Value);
      FName := ExtractFilePath(FileName) + FName + ExtractFileExt(FileName);
    end;

    Result := FName;

end;
*)

procedure TMainForm.Cont_Tone_BoxChange(Sender: TObject);
begin
  SiteFile.ContinuousTone := Cont_Tone_Box.ItemIndex;
end;

procedure TMainForm.Cont_Tone_Box_ApolloChange(Sender: TObject);
begin
  Int_Tone_Box_Apollo.ItemIndex := Cont_Tone_Box_Apollo.ItemIndex;
  SiteFile.ContinuousTone := Cont_Tone_Box_Apollo.ItemIndex;
end;

procedure TMainForm.Cont_Tone_Box_CASTChange(Sender: TObject);
begin
  SiteFile.ContinuousTone := Cont_Tone_Box_CAST.ItemIndex;
end;

procedure TMainForm.UpdateSubNames;
var
   Device : integer;
   SubAddress : integer;
begin

     for Device := 1 to frmLoop1.AFPLoop.NoLoopDevices do
     begin
       {first, update the name for sub-address 0, which is the .Name field}
       frmLoop1.AFPLoop.Device[Device].Name
         := frmDeviceName.ComboBox1.Items[frmLoop1.AFPLoop.Device[Device].SharedData[2]];
       frmLoop2.AFPLoop.Device[Device].Name
         := frmDeviceName.ComboBox1.Items[frmLoop2.AFPLoop.Device[Device].SharedData[2]];

       {Then, do the sub-address names}
       for SubAddress := 1 to 3 do
       begin
         if frmLoop1.AFPLoop.Has_SubAddresses(Device) = TRUE then
           frmLoop1.AFPLoop.Device[Device].SubName[SubAddress]
              := frmDeviceName.ComboBox1.Items[frmLoop1.AFPLoop.Device[Device].SharedData[(SubAddress * 2) + 2]]
         else
           frmLoop1.AFPLoop.Device[Device].SubName[SubAddress] := 'No name allocated';

         if frmLoop2.AFPLoop.Has_SubAddresses(Device) = TRUE then
           frmLoop2.AFPLoop.Device[Device].SubName[SubAddress]
              := frmDeviceName.ComboBox1.Items[frmLoop2.AFPLoop.Device[Device].SharedData[(SubAddress * 2) + 2]]
         else
           frmLoop2.AFPLoop.Device[Device].SubName[SubAddress] := 'No name allocated';

       end;
     end;
end;

procedure TMainForm.RefreshAllPages;
var
   TCurrentPage : TTabSheet;

begin

  if SiteFile.MainVersion >= '09A06' then
  begin
    PgmData.MAX_CE_EVENTS := DEFAULT_MAX_CE_EVENTS
  end
  else
  begin
    PgmData.MAX_CE_EVENTS := 16;
  end;


  { Now update all pages, returning to the current page once done }
  TCurrentPage := ConfigPages.ActivePage;

  UpdateLoopSummaryPage;
  UpdateZoneConfigPage;
  UpdateGroupConfigPage;
  UpdateSetConfigPage;
  UpdateSiteConfigPage;
  UpdateNetworkConfigPage;
  UpdateLoop1Page;
  UpdateLoop2Page;

  ConfigPages.ActivePage := TCurrentPage;

//  if TCurrentPage = tabC_E_Events then
  RefreshC_EPage;

end;

procedure TMainForm.ChangeProtocolWarning(const pNewProtocol: TSystemType;
  const pProtocolText: string);
var
  Dummy : boolean;
begin
  if MessageDlg ('You are switching to ' + pProtocolText + ' protocol. This will clear your current ' +
    'setup. Do you wish to continue?', mtConfirmation, [mbYes, mbNo], 0) = mrYes then
  begin

    sgrdLoopSelectCell(Self, 0, 0, Dummy);

    SiteFile.IsDirty := FALSE;
    Sitefile.New;
    SiteFile.SystemType := pNewProtocol;


    RefreshProtocolControls;
  end;
end;

procedure TMainForm.changeToApollo(Sender: TObject);
begin
  ChangeProtocolWarning( stApolloLoop, 'Apollo' );
end;

procedure TMainForm.changeToNittan(Sender: TObject);
begin
  ChangeProtocolWarning( stNittanLoop, 'Nittan' );
end;

procedure TMainForm.changeToSysSensor(Sender: TObject);
begin
  MessageDlg ('The System Sensor protocol is not implemented at this time', mtInformation, [mbOk], 0);

  exit;

  ChangeProtocolWarning( stSysSensorLoop, 'System Sensor' );

end;

procedure TMainForm.changeToHochiki(Sender: TObject);
begin
  ChangeProtocolWarning( stHochikiLoop, 'Hochiki' );
end;

procedure TMainForm.changeToCAST(Sender: TObject);
begin
  ChangeProtocolWarning( stCASTLoop, 'CAST' );
end;

(*
procedure TMainForm.ChangeProtocol;
var
  CurrentPage: integer;
begin

  CurrentPage := ConfigPages.ActivePageIndex;

  Device_Selection_Frame1.Protocol := integer(frmLoop1.AFPLoop.SystemType);

  if frmLoop1.AFPLoop.SystemType = stHochikiLoop then
  begin
    Int_Tone_Box.Visible := TRUE;
    Cont_Tone_Box.Visible := TRUE;
    Int_Tone_Box_Apollo.Visible := FALSE;
    Cont_Tone_Box_Apollo.Visible := FALSE;
    Int_Label.Visible := TRUE;
    Cont_Label.Visible := TRUE;
    DeviceConfig.Apollo_Detector_Config_Frame1.chkBase_Sounder.Caption := 'Has Addr. Base Sounder';
  end else
  begin
    Int_Tone_Box.Visible := FALSE;
    Cont_Tone_Box.Visible := FALSE;
    Int_Tone_Box_Apollo.Visible := TRUE;
    Cont_Tone_Box_Apollo.Visible := TRUE;
    Int_Label.Visible := TRUE;
    Cont_Label.Visible := TRUE;
    DeviceConfig.Apollo_Detector_Config_Frame1.chkBase_Sounder.Caption := 'Has Ancillary Base Sounder';
  end;

  { Now bring up a new set up using the new protocol }
  ClearInfo;
  ConfigPages.ActivePageIndex := CurrentPage;
end;
*)

procedure TMainForm.FormCreate(Sender: TObject);
begin
  //FAntiFlicker := TAntiFlicker.Create( self, TRUE );
    MainForm.Caption := MAINFORM_CAPTION;
end;

procedure TMainForm.FormDestroy(Sender: TObject);
begin
  //FAntiFlicker.Free;
end;

procedure TMainForm.FormInitialise;
begin
  PgmData.Initialise;

  SiteFile.Editor := self;
  Sitefile.SaveDialog := PgmData.SigSaveDialogXFP;
  SiteFile.OnPrint := btnPrintClick;
  SiteFile.OnPrinterSetup := PrinterPropertiesClick;
  SiteFile.MainMenu := MainMenu;
  SiteFile.OnDirtyChange := OnDirtyChange;

  PgmData.MAX_CE_EVENTS := DEFAULT_MAX_CE_EVENTS;

  SiteFile.OnNew := OnNew;
  SiteFile.OnLoad := OnLoad;

  frmLoop1.AFPLoop := SiteFile.AFPLoop1;
  frmLoop2.AFPLoop := SiteFile.AFPLoop2;
  frmDeviceName.AFPLoop1 := SiteFile.AFPLoop1;;
  frmDeviceName.AFPLoop2 := SiteFile.AFPLoop2;

  NoOfLoops := 2;

  LoadProtocol_Buttons;

  SiteFile.IsDirty := FALSE;

end;

procedure TMainForm.FormShow(Sender: TObject);
var
  Port: integer;
  i: Integer;
begin

  MainForm.Caption := MAINFORM_CAPTION;

  { Language tab still inder development }
  //tabLanguage.TabVisible := TRUE;
  tabLanguage.TabVisible := FALSE;

  { Acquire last port number from registry }
  Port := PgmData.SigRegistryXFP.ReadInteger( 'Port' , 1 );

  { Change serial port }
  PanelComms.Port := Port;
  {
  case Port of
    1: PanelComms.ComPort := pnCOM1;
    2: PanelComms.Comport := pnCOM2;
    3: PanelComms.Comport := pnCOM3;
    4: PanelComms.Comport := pnCOM4;
    5: PanelComms.Comport := pnCOM4;
    6: PanelComms.Comport := pnCOM4;
  end;
  }

  { initialise the panel number }
  PanelComms.PanelNumber := PanelNumber.Value;


(*
try
  { create the panel network matrix }
  //XFP_Network := TXFP_Network.Create;

except
  Application.MessageBox('Err 021', 'Form Show');
end;
*)

(*
  This component moved to site file so does not need to be created now.
try
  { Create the multiple Visual Components at run-time }
  C_ETable := TC_ETable.Create;
except
  Application.MessageBox('Err 022', 'Form Show');
end;
*)

  try
    CreateZonePage;
  except
    Application.MessageBox('Err 023', 'Form Show');
  end;

  try
    CreateC_EPage;
  except
    Application.MessageBox('Err 024', 'Form Show');
  end;

  try
    //{ Update serial port selection dialog box }
    //CommSelectForm.radgrpComms.ItemIndex := (Port - 1);

    { Ensure that both group and set list boxes indexes are set correctly }
    lstGroups.ItemIndex := 0;

  except
    Application.MessageBox('Err 003', 'Form Show');
  end;

  SiteFile.New;

  { Ensure that it is OK to load by setting the Code OK to true }
  FCodeOK := true;

  try
    { Set the active page to the Site Config page }
    ConfigPages.ActivePage := tabSiteCfg;
    ConfigPagesChange(self);

    { Reset the modified flag }
    //Modified := false;
  except
    Application.MessageBox('Err 008', 'Form Show');
  end;


  //re-size the Loop Summary string grid columns
  sgrdLoop.ColWidths[0] := stbarLoopInfo.Panels[0].Width - 3;
  sgrdLoop.ColWidths[1] := stbarLoopInfo.Panels[1].Width - 1;
  sgrdLoop.ColWidths[2] := stbarLoopInfo.Panels[2].Width - 1;
  sgrdLoop.ColWidths[3] := stbarLoopInfo.Panels[3].Width - 1;
  sgrdLoop.ColWidths[4] := stbarLoopInfo.Panels[4].Width - 1;
  sgrdLoop.ColWidths[5] := stbarLoopInfo.Panels[5].Width - 1;
  sgrdLoop.ColWidths[6] := stbarLoopInfo.Panels[6].Width - 1;

  //re-size the Loop Summary->Zones selection string grid columns
  sgrdZone.ColWidths[0] := 30;
  sgrdZone.ColWidths[1] := 120;


  //re-size the Sets Config string grid column widths
  for i := 1 to 22 do
  begin
    dwgSetsGrid.ColWidths[i] := 28;

  end;
  dwgSetsGrid.ColWidths[0] := 50;
  dwgSetsGrid.ColWidths[6] := 15;
  dwgSetsGrid.ColWidths[12] := 15;
  dwgSetsGrid.ColWidths[18] := 15;
  dwgSetsGrid.ColWidths[20] := 15;


  //re-size the Group Config string grid column widths
  for i := 1 to 19 do
  begin
    dwgGroupConfig.ColWidths[i] := 35;

  end;
  dwgGroupConfig.ColWidths[0] := 50;
  dwgGroupConfig.ColWidths[6] := 15;
  dwgGroupConfig.ColWidths[12] := 15;
  dwgGroupConfig.ColWidths[18] := 15;
end;

procedure TMainForm.OnDirtyChange( const pChangedObject : TSigBaseProperty; const NewState : boolean );
begin
  btnSave.Enabled := NewState;
end;

(*
procedure TMainForm.SetModified(const Value: Boolean);
{ If any component has been modified, highlight the save, and reflect the modified
status in the tool bar }
begin
  FModified := Value;
  btnSave.Enabled := Value;
  FileSaveItem.Enabled := Value;
  { If MainForm is showing, then put path and name of configuration file in the main window }
  if MainForm.Showing then begin
    if IniFileName = '' then
      { Show the name of the loaded file }
      MainForm.Caption := MAINFORM_CAPTION + 'New file'
    else
      MainForm.Caption := MAINFORM_CAPTION + InifileName;

    { If modified, add modified to caption }
    if Value then
      MainForm.Caption := MainForm.Caption + ' - Modified'
  end;
end;
*)


procedure TMainForm.UpdateLoopSummary;
var
  TCurrentEnableSetting: boolean;
  TCurrentRow: integer;
  TCurrentCol: integer;

  frmLoop : TfrmLoop;
begin

  { save the current settings for this form, so we can try and
    return to them at the end}

  TCurrentRow := sgrdLoop.Row;
  TCurrentCol := sgrdLoop.Col;

  TCurrentEnableSetting := chkEnableZoneChanges.Checked;

  { make the Enable Edits check box false. This will stop any
    inadvertant changes caused by refreshing the string grid}
  chkEnableZoneChanges.Checked := FALSE;

  { Determine which loop is to be interrogated }
  if radgrpLoop.ItemIndex = 0 then
    frmLoop := frmLoop1
  else if radgrpLoop.ItemIndex = 1 then
    frmLoop := frmLoop2
  else
    frmLoop := nil;

  Loop := radgrpLoop.ItemIndex + 1;

  sgrdLoop.RowCount := frmLoop1.AFPLoop.MaxCellCount;

   ShowSummaryByDevice(frmLoop);

  { Show the loop type on the status bar }
  case frmLoop.AFPLoop.SystemType of
    stCASTLoop: stbarLoopInfo.Panels[7].Text := 'Loop Type=  CAST ' + IntToStr(NoOfLoops) + ' Loop';
    stApolloLoop: stbarLoopInfo.Panels[7].Text := 'Loop Type=  Apollo ' + IntToStr(NoOfLoops) + ' Loop';
    stSysSensorLoop: stbarLoopInfo.Panels[7].Text := 'Loop Type=  System Sensor ' + IntToStr(NoOfLoops) + ' Loop';
    stNittanLoop: stbarLoopInfo.Panels[7].Text := 'Loop Type= Nittan ' + IntToStr(NoOfLoops) + ' Loop';
    stHochikiLoop: stbarLoopInfo.Panels[7].Text := 'Loop Type= Hochiki ' + IntToStr(NoOfLoops) + ' Loop';
  end;

  { try and return the focus to the same col & row as when we entered
    but if this is not possible (because the number of rows may have
    reduced) then place it at the max row}

  if TCurrentRow < sgrdLoop.RowCount then begin
    sgrdLoop.Row := TCurrentRow;
    sgrdLoop.Col := TCurrentCol;
  end else begin
    sgrdLoop.Row := sgrdLoop.RowCount-1;
    sgrdLoop.Col := TCurrentCol;
  end;

  { also return this check box to its original value when we entered
    this procedure}
  chkEnableZoneChanges.Checked := TCurrentEnableSetting;

end;

procedure TMainForm.UpdateZoneGroupList;
var
  iIndex: integer;
  IsZone: Boolean;
begin
  IsZone := (radgrpZoneGroup.ItemIndex = 0);
  { Determine whether to show groups or zones }
  if IsZone then begin
    { Set the number of rows, remembering to include the header row, and the row
    for no zone allocated }
    sgrdZone.RowCount := MAX_LOCAL_ZONES + 2;

    { Fill in the header row }
    sgrdZone.Cells[0,0] := 'No.';
    sgrdZone.Cells[1,0] := 'Description';

    { For each zone/group/set, show the number and the description }
    for iIndex := 0 to MAX_LOCAL_ZONES do begin
      sgrdZone.Cells[0, iIndex + 1] := IntToStr (iIndex);
      sgrdZone.Cells[1, iIndex + 1] := SiteFile.Zonelist.Zone[iIndex].Name
    end;
  end;

  { Determine which box to show }
  case radgrpZoneGroup.ItemIndex of
    0: sgrdZone.BringToFront;
    1: lstGroups.BringToFront;
  end;
end;

procedure TMainForm.cboReportTypeChange(Sender: TObject);
begin
  { Update the displayed image }
  UpdateLoopSummary;
end;

procedure TMainForm.UpdateZoneDefs;
var
  i: integer;
  iEdit : TEdit;
begin
  { Place zone definitions into edit boxes on zone configuration page }
  for i := 1 to MAX_ZONE do begin
    iEdit := FindComponent ('txtZPZone' + IntToStr (i)) as TEdit;
    if iEdit.Text <> SiteFile.ZoneList.Zone[i].Name then
    begin
      iEdit.Text := SiteFile.ZoneList.Zone[i].Name;
    end;
  end;
end;

procedure TMainForm.chkDetectorClick(Sender: TObject);
begin
  { The zone number is stored in the tag property of the check box.  The checked
  status of this check box is stored }
  SiteFile.ZoneList.Zone[(Sender as TCheckBox).Tag].Detector := (Sender as TCheckBox).Checked;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.chkMCPClick(Sender: TObject);
begin
  { The zone number is stored in the tag property of the check box.  The checked
  status of this check box is stored }
  SiteFile.ZoneList.Zone[(Sender as TCheckBox).Tag].MCP := (Sender as TCheckBox).Checked;
end;

procedure TMainForm.chkZPEndDelays1Click(Sender: TObject);
begin
  { The zone number is stored in the tag property of the check box.  The checked
  status of this check box is stored }
  SiteFile.ZoneList.Zone[(Sender as TCheckBox).Tag].EndDelays := (Sender as TCheckBox).Checked;
end;

procedure TMainForm.chkNonFireClick(Sender: TObject);
var
  iIndex: integer;
  CheckStatus: Boolean;
begin
  { Determine the index of the check box selected }
  iIndex := (Sender as TCheckBox).Tag;
  CheckStatus := (Sender as TCheckBox).Checked;
  { The zone number is stored in the tag property of the check box.  The checked
  status of this check box is stored }
  SiteFile.ZoneList.Zone[iIndex].NonFire := CheckStatus;
  { If the zone is a non fire zone, then disable the zone timers }
  TCtecSpinEdit (FindComponent ('ZSounder' + IntToStr(iIndex))).Enabled := not CheckStatus;
  TCTecSpinEdit (FindComponent ('ZRemote' + IntToStr(iIndex))).Enabled := not CheckStatus;
end;

procedure TMainForm.txtZoneChange(Sender: TObject);
begin
  { Whenever this edit box is changed, the change is stored in the relevant zone.
  The zone is accessed via the tag property of the edit box }
  SiteFile.ZoneList.Zone[(Sender as TEdit).Tag].Name := (Sender as TEdit).Text;
end;

procedure TMainForm.txtZPDependancy1Click(Sender: TObject);
begin
     frmDependancy.lblZone.Caption := 'Zone ' + IntToStr((Sender as TEdit).Tag);
     frmDependancy.lblZone.Tag := (Sender as TEdit).Tag;
     frmDependancy.Showmodal;

     UpdateZoneConfigPage;
end;

procedure TMainForm.ShowSummaryByDevice( const frmLoop : TFrmLoop );
var
  iCount: integer;
begin
  { Initialise current row of string grid }
  CurrentRow := -1;
  for iCount := 1 to frmLoop.AFPLoop.NoLoopDevices do begin
    ShowDeviceProperties ( frmLoop, iCount, TRUE);
  end;
end;

procedure TMainForm.ShowSummaryByZone( const frmLoop : TFrmLoop );
{ This procedure sorts the devices by zone, then places each device in a zone
in numerical device number order }
var
  DevCount: integer;
  iCount: integer;
  iIndex: integer;
begin
  { Initialise current row of string grid }
  CurrentRow := -1;
  { Go through all the zone-based devices first }
  for iCount := 1 to (MAX_ZONE + 1) do begin
    { MAX_ZONE + 1 will be remapped to zone 0, to ensure that devices belonging
    to no zones are after the devices in zones}
    if iCount = MAX_ZONE + 1 then iIndex := 0
    else iIndex := iCount;
    { Check for all devices }
    for DevCount := 1 to frmLoop1.AFPLoop.NoLoopDevices do begin
      { Check to see if device belongs to a zone, and that zone is the one we
      currently are checking }
      if ((not frmLoop.AFPLoop.IsGroup (DevCount)) and      // Is device in zone
          (frmLoop.AFPLoop.Device[DevCount].Zone = iIndex))  // Does device have correct zone number
      then begin
        ShowDeviceProperties ( frmLoop, DevCount, TRUE);
      end; {if device is zone-based}
    end; {for DevCount}
  end; {for iCount}

  { Repeat for all group based devices }
  for iCount := 1 to (MAX_GROUP + 1) do begin
    { MAX_GROUP + 1 will be remapped to Group 0, to ensure that devices belonging
    to no groups are after the devices in groups}
    if iCount = MAX_GROUP + 1 then iIndex := 0
    else iIndex := iCount;
    { Check for all devices }
    for DevCount := 1 to frmLoop1.AFPLoop.NoLoopDevices do begin
      { Check to see if device belongs to a group, and that Group is the one we
      currently are checking }
      if ((frmLoop.AFPLoop.IsGroup (DevCount)) and      // Is device in Group
          (frmLoop.AFPLoop.Device[DevCount].Zone = iIndex))  // Does device have correct group number
      then begin
        ShowDeviceProperties (frmLoop, DevCount, TRUE);
      end; {if device is group-based}
    end; {for DevCount}
  end; {for iCount}

end;

procedure TMainForm.ShowDeviceProperties( const frmLoop : TFrmLoop; iIndex: integer; Building_List : boolean);
var
   Sub_Channel : string;
   Name : string;
   TypeString : string;
begin
  { its not the right place here, but refresh this devices' base sounder flag}
  if frmLoop.AFPLoop.SystemType <> stCASTLoop then  //JG CAST/Hochiki Base Sndr
  begin
    if iIndex < 128 then
    begin
      if frmLoop.AFPLoop.Classify_Device(iIndex + 127) = Sounder then
      begin
        frmLoop.AFPLoop.Device[iIndex].HasBaseSounder := TRUE
      end
      else
      begin
        frmLoop.AFPLoop.Device[iIndex].HasBaseSounder := FALSE;
      end;
    end;
  end;

  if iIndex > frmLoop.AFPLoop.MaxCellCount then Exit;
  if CurrentRow > frmLoop.AFPLoop.MaxCellCount-1 then Exit;

  if (frmLoop.AFPLoop.Device[iIndex].DevType >= D_NOT_FITTED) and ShowFittedOnly then Exit;

  { Go to next line }
  if Building_List = TRUE then
  begin
    CurrentRow := CurrentRow + 1;
  end;

  { Display the loop number }
  sgrdLoop.Cells [0, CurrentRow] := IntToStr (radgrpLoop.ItemIndex + 1);

  Sub_Channel := '';
  if frmLoop.AFPLoop.Has_SubAddresses(iIndex) = TRUE then Sub_Channel := '+';

  { Display device number - remember that each loop for the system sensor is split
  into 2, S1 - S100, and M1 - M100. Other protocols are the device number }
  if frmLoop.AFPLoop.SystemType = stSysSensorLoop then
  begin
    { If it is, show the correct device number }
    if iIndex > HALF_MAX_POINTS_SYS_SENSOR then
    begin
      sgrdLoop.Cells [1, CurrentRow] := 'M' + IntToStr (iIndex - HALF_MAX_POINTS_SYS_SENSOR) + Sub_Channel;
    end
    else
    begin
      sgrdLoop.Cells [1, CurrentRow] := 'S' + IntToStr (iIndex) + Sub_Channel;
    end;
  end
  else
  begin
    sgrdLoop.Cells [1, CurrentRow] := IntToStr (iIndex) + Sub_Channel;
  end;

  { Next is device type }
  TypeString := frmLoop.AFPLoop.ShortDeviceName (frmLoop.AFPLoop.Device[iIndex].DevType);
  if frmLoop.AFPLoop.Device [iIndex].TypeChanged = TRUE then
  begin
    TypeString := TypeString + ' -Changed';
  end;

  sgrdLoop.Cells [2, CurrentRow] := TypeString;

  if TypeString = UNKNOWN_TYPE then
  begin
    sgrdLoop.Cells [2, CurrentRow] := TypeString + ' (0x' + IntToHex(frmLoop.AFPLoop.Device[iIndex].DevType, 2) + ')';

    sgrdLoop.Cells [3, CurrentRow] := '';
    sgrdLoop.Cells [4, CurrentRow] := '';
    sgrdLoop.Cells [5, CurrentRow] := '';
    sgrdLoop.Cells [6, CurrentRow] := '';

    Exit;
  end;

  if frmLoop.AFPLoop.Device[iIndex].DevType >= D_UnKnown then
  begin
    sgrdLoop.Cells [3, CurrentRow] := '';
    sgrdLoop.Cells [4, CurrentRow] := '';
    sgrdLoop.Cells [5, CurrentRow] := '';
    sgrdLoop.Cells [6, CurrentRow] := '';

    Exit;
  end;

  { if device has sub-addresses, then display simply 'Click for I/O Configuration.' }
   if( (frmLoop.AFPLoop.Has_SubAddresses(iIndex) = TRUE)  and
       (frmLoop.AFPLoop.Device[iIndex].DevType < Null_Type) = TRUE) then
   begin
//      if frmLoop.Loop.Device[iIndex].SharedData[1] > 128 then
//        sgrdLoop.Cells[3, CurrentRow] := Data.GroupList.OutputSet[frmLoop.Loop.Device[iIndex].Zone and 31].Name
//      else
//        sgrdLoop.Cells [3, CurrentRow] := 'Zone ' + IntToStr (frmLoop.Loop.Device[iIndex].Zone) + '- ' +
//                                          Data.ZoneList.Zone[frmLoop.Loop.Device[iIndex].Zone].Name;
       sgrdLoop.Cells[3, CurrentRow] := 'Click for I/O Configuration.';
   end
   else
   begin
    { Determine whether device is group-based or zone-based }
     if frmLoop.AFPLoop.IsGroup (iIndex) then
     begin
       { Show the name of the group }
       sgrdLoop.Cells[3, CurrentRow] := SiteFile.GroupList.Group[frmLoop.AFPLoop.Device[iIndex].Zone].Name;

     end
     else if frmLoop.AFPLoop.IsSet (iIndex) then
     begin
       { Show the name of the set - This is the number of the zone associated with the
       object }
       sgrdLoop.Cells[3, CurrentRow] := SiteFile.GroupList.OutputSet[frmLoop.AFPLoop.Device[iIndex].Zone].Name;
     end
     else
     begin
       { If the zone is allocated, state the zone }
       if frmLoop.AFPLoop.Device[iIndex].Zone > 0 then
       begin
         sgrdLoop.Cells [3, CurrentRow] := 'Zone ' + IntToStr (frmLoop.AFPLoop.Device[iIndex].Zone) + '- ';
       end
       else
       begin
         { Otherwise clear the cell }
         sgrdLoop.Cells [3, CurrentRow] := '';
       end;
      { Show the name of the zone }
       sgrdLoop.Cells [3,CurrentRow] := sgrdLoop.Cells [3, CurrentRow] +
        SiteFile.ZoneList.Zone[frmLoop.AFPLoop.Device[iIndex].Zone].Name;
    end;
  end;

  { next the point name}
  Name := frmDeviceName.ComboBox1.Items.Strings[frmLoop.AFPLoop.Device[iIndex].SharedData[2]];
  sgrdLoop.Cells [4, CurrentRow] := Name;

  { Finally whether there is a base sounder attached }
  if frmLoop.AFPLoop.Device[iIndex].HasBaseSounder = TRUE then
  begin
    sgrdLoop.Cells [6, CurrentRow] := 'Group ' + IntToStr(frmLoop.AFPLoop.Device[iIndex + 127].Zone);
  end
  else
  begin
    sgrdLoop.Cells [6, CurrentRow] := 'no';
  end;
end;

procedure TMainForm.radgrpLoopClick(Sender: TObject);
begin
  { Store the loop number }
  Loop := radgrpLoop.ItemIndex + 1;

  { Change the caption of the load/save buttons }
  btnLoadfromPanel.Caption := 'Load Loop ' + IntToStr (Loop) +
    ' From Panel ' + IntToStr(PanelNumber.Value);
  btnSavetoPanel.Caption := 'Save Loop ' + IntToStr (Loop) +
    ' To Panel ' + IntToStr(PanelNumber.Value);
  { Update the loop summary information in the string grid }

  if Loop > 1 then
  begin
   if NoOfLoops = 1 then
   begin
     btnLoadfromPanel.Enabled := FALSE;
     btnSaveToPanel.Enabled := FALSE;
   end else
   begin
     btnLoadfromPanel.Enabled := TRUE;
     btnSaveToPanel.Enabled := TRUE;
   end;
  end else
  begin
   btnLoadfromPanel.Enabled := TRUE;
   btnSaveToPanel.Enabled := TRUE;
  end;

  UpdateLoopSummary;
end;




procedure TMainForm.Update_Description_Indices( const frmLoop : TFrmLoop );
var
  DevIndex: integer;
begin
  { update the point description indices for all devices }
  if MainForm.Edit7.Visible = FALSE then
    exit;

  for DevIndex := 1 to frmLoop.AFPLoop.MaxCellCount do
  begin
    if frmLoop.AFPLoop.Device[DevIndex].DevType < D_NOT_FITTED then
    begin
      sgrdLoop.Cells [5, DevIndex -1] := IntToStr(frmLoop.AFPLoop.Device[DevIndex ].SharedData[2]);
    end;
  end;
end;

procedure TMainForm.sgrdLoopSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
var
  iCount: integer;
  DevIndex: integer;
  DevIndex1: integer;
  DevType : integer;
  WasDetector : boolean;
  WasInSet : boolean;
  frmLoop : TFrmLoop;
begin

  case Loop of
    1: frmLoop := frmLoop1;
    2: frmLoop := frmLoop2;
    else exit;
  end;

  DevIndex := GetGridDevIndex (frmLoop, ARow);
  Device_Number := DevIndex;
  frmLoop1.DevIndex := DevIndex;
  frmLoop2.DevIndex := DevIndex;

  Update_Device_Names_Database;
  Update_Description_Indices( frmLoop );

  { Set modified flag }
  //Modified := true;


  ZGSGroupBox.Visible := FALSE;
  BSGroupBox.Visible := FALSE;

  case ACol of
    0:
    begin  // Loop number
      { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];
      sgrdLoop.Options := sgrdLoop.Options - [goAlwaysShowEditor];
    end;

    1:
    begin // Device number
      { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];
      sgrdLoop.Options := sgrdLoop.Options - [goAlwaysShowEditor];

      if (frmLoop.AFPLoop.ShortDeviceName (frmLoop.AFPLoop.Device[DevIndex].DevType) = UNKNOWN_TYPE) or
           (frmLoop.AFPLoop.Device[DevIndex].DevType >= D_UnKnown) then
      begin
        exit;
      end;

      frmDeviceName.AFPLoop1 := frmLoop1.AFPLoop;
      frmDeviceName.AFPLoop2 := frmLoop2.AFPLoop;
      frmLoop.EditPointDevice;

      sgrdLoop.Col := 0;
      UpdateLoopSummaryPage;
    end;

    2:
    begin // Device type
    { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];
      sgrdLoop.Options := sgrdLoop.Options - [goAlwaysShowEditor];

      if chkEnableZoneChanges.Checked = FALSE then
      begin
        MessageDlg('You must check the Enable Changes option', mtWarning,
                    [mbOk], 0);
        Exit;
      end;

      //maybe here if a change is detected, ask for confirmation.....
      WasDetector := frmLoop.AFPLoop.IsDetector(DevIndex);
      WasInSet := frmLoop.AFPLoop.IsSet(DevIndex);

      DevType := Device_Selection_Frame1.Device_Type;
      if DevType >= D_NOT_FITTED then
      begin
        frmLoop.AFPLoop.RemoveDevice(DevIndex);
      end;

      { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];

      { Now store the device type for this device }
      frmLoop.AFPLoop.Device[DevIndex].DevType := DevType;

      {If new device type is not a sounder and it is in address range
       112-126, advise user about the Group command restriction}
      if SiteFile.SystemType = stApolloLoop then
      begin
        if DevIndex > 111  then
        begin
          if DevType < D_NOT_FITTED then
          begin
            if frmLoop.AFPLoop.Classify_Device(DevIndex) <> Sounder then
            begin
              if MessageDlg('Group commands will be disabled to this Sounder Group, Continue?', mtConfirmation,
                      [mbYes, mbNo], 0) = mrNo then
              begin
                frmLoop.AFPLoop.Device[DevIndex].DevType := D_NOT_FITTED;
                        //DevType := D_NOT_FITTED;
              end;
            end;
          end;
        end;
      end;

      { If no data present, reset to defaults}
      if frmLoop.AFPLoop.Device[DevIndex].SharedData[2] = 0 then
      begin
        frmLoop.AFPLoop.SetDefaults(DevIndex);
      end;
      if frmLoop.AFPLoop.IsSet(DevIndex) = TRUE then
      begin
        if WasInSet = FALSE then
        begin
          frmLoop.AFPLoop.SetDefaults(DevIndex);
        end;
      end;

      if WasInSet then
      begin
        if not frmLoop.AFPLoop.IsSet(DevIndex) then
        begin
          frmLoop.AFPLoop.SetDefaults(DevIndex);
        end;
      end;

      { Update the display for the  properties for this device }
      CurrentRow := ARow;


      {if new device type is not a detector, or if the old device
       type was not a detector, then clear the base sounder attributes}
      if frmLoop.AFPLoop.SystemType = stApolloLoop then begin
        if (frmLoop.AFPLoop.IsDetector(DevIndex) = FALSE) or
           (frmLoop.AFPLoop.IsDetector(DevIndex) <> WasDetector) then
        begin
          frmLoop.AFPLoop.Device[DevIndex].HasBaseSounder := FALSE;
          if( (DevIndex + 127) <= frmLoop.AFPLoop.NoLoopDevices) then
          begin
            frmLoop.AFPLoop.Device[DevIndex+127].DevType := D_NOT_FITTED;
          end;
        end;
      end;

      frmLoop.AFPLoop.Device[DevIndex].TypeChanged := FALSE;


      if ShowFittedOnly then
      begin
        UpdateLoopSummary;
      end
      else
      begin
        ShowDeviceProperties (frmLoop, DevIndex, FALSE);
      end;
    end;

    3:
    begin // Zone/group/set
      { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];
      sgrdLoop.Options := sgrdLoop.Options - [goAlwaysShowEditor];

      { If the enable zone/set/group changes check box is cleared, then do not
        allow the zone to be updated }
      if chkEnableZoneChanges.Checked = FALSE then
      begin
        MessageDlg('You must check the Enable Changes option', mtWarning,
                      [mbOk], 0);
        Exit;
      end;

      { If this column is selected, the user should be able to determine the zone }
      if frmLoop.AFPLoop.Device[DevIndex].DevType >= D_NOT_FITTED then
      begin
        { If there is no device fitted, do not allow a change of zone/group/set }
        Exit;
      end;

      {if the device is IO based, open up the Edit IO config box}
      if frmLoop.AFPLoop.Has_SubAddresses(DevIndex) = TRUE then
      begin
        ZGSGroupBox.Visible := FALSE;
        frmLoop.EditPointDevice;

        sgrdLoop.Col := 0;
        UpdateLoopSummaryPage;
        Exit;
      end;


      //new code to use drop downs for group/zone selection
      //{        //otherwise, populate and display the Zone/Group drop down list
      if frmLoop.AFPLoop.IsGroup (DevIndex) then
      begin
        ZGSGroupBox.Clear;
        ZGSGroupBox.Items.Add('Use in Special C&E');
        for iCount := 1 to MAX_GROUP do
        begin
          ZGSGroupBox.Items.Add('Group ' + IntToStr(iCount));
        end;
      end
      else
      begin
        ZGSGroupBox.Clear;
        ZGSGroupBox.Items.Add('Use in Special C&E');
        for iCount := 1 to MAX_LOCAL_ZONES do
        begin
          ZGSGroupBox.Items.Add('Zone ' + IntToStr(iCount) + '- ' +
          SiteFile.Zonelist.Zone[iCount].Name);
        end;
      end;

      DevIndex1 := GetGridDevIndex (frmLoop, sgrdLoop.TopRow);
      ZGSGroupBox.Top := sgrdLoop.Top + ( (ARow - DevIndex1 + 1) * (sgrdLoop.DefaultRowHeight + 1));

      if frmLoop.AFPLoop.IsGroup (DevIndex) then
      begin
        ZGSGroupBox.ItemIndex := lstGroups.ItemIndex;
      end
      else
      begin
        ZGSGroupBox.ItemIndex := ZoneGroupIndex;
      end;

      ZGSGroupBox.Visible := TRUE;
      ZGSGroupBox.OnClick(self);
      exit;

      if frmLoop.AFPLoop.IsGroup (DevIndex) and (radgrpZoneGroup.ItemIndex = 1) then
      begin
        frmLoop.AFPLoop.Device[DevIndex].Zone := lstGroups.ItemIndex;
        { Update the properties for this device }
        CurrentRow := ARow;
        ShowDeviceProperties (frmLoop, DevIndex, FALSE);
      end
      else if (not frmLoop.AFPLoop.IsGroup (DevIndex)) and
                (not frmLoop.AFPLoop.IsSet (DevIndex)) and
                (radgrpZoneGroup.ItemIndex = 0) then
      begin
        { Is device zone based and the zone list showing }
        frmLoop.AFPLoop.Device[DevIndex].Zone := ZoneGroupIndex;
        { Update the properties for this device }
        CurrentRow := ARow;
        ShowDeviceProperties (frmLoop, DevIndex, FALSE);
      end;

    end;

    4:
    begin // Description
      { Disable direct editing of the string grid }
      sgrdLoop.Options := sgrdLoop.Options - [goEditing];
      sgrdLoop.Options := sgrdLoop.Options - [goAlwaysShowEditor];

      if frmLoop.AFPLoop.Device[DevIndex].DevType < D_NOT_FITTED then
      begin
        frmDeviceName.Rebuild_List( SiteFile.AFPLoop1, SiteFile.AFPLoop2 );

        sgrdLoop.Options := sgrdLoop.Options + [goEditing];
        sgrdLoop.Options := sgrdLoop.Options + [goAlwaysShowEditor];

        CurrentRow := ARow;
      end;
    end;


    6:
    begin         //Base Sounder
      if frmLoop.AFPLoop.BS_Allowed(DevIndex) = TRUE then
      begin
        DevIndex1 := GetGridDevIndex (frmLoop, sgrdLoop.TopRow);
        BSGroupBox.Top := sgrdLoop.Top + ( (ARow - DevIndex1 + 1) * (sgrdLoop.DefaultRowHeight + 1));
        BSGroupBox.ItemIndex := frmLoop.AFPLoop.Device[DevIndex + 127].Zone;
        BSGroupBox.Visible := TRUE;
      end
      else
      begin
        BSGroupBox.Visible := FALSE;
      end;
    end;

  end; { Case ACol }
end;

function TMainForm.GetCAST_Protocol_Available: boolean;
begin
  Result := ProtocolButtonsFile.CAST_Protocol_Available;
end;

function TMainForm.GetC_Table: TC_ETable;
begin
  Result := SiteFile.C_ETable;
end;

function TMainForm.GetGridDevIndex ( const frmLoop : TFrmLoop; ARow: integer): integer;
var
  SysSensorChar: string;
  P : integer;
begin
  { Determine the device number of the device selected }
  if frmLoop.AFPLoop.SystemType = stSysSensorLoop then
  begin
    { Strip the first character from the device }
    SysSensorChar := Copy (sgrdLoop.Cells[1, ARow], 1, 1);
    { Obtain the device number for that section }
    Result := StrToInt (Copy (sgrdLoop.Cells[1, ARow], 2, 2));

    if SysSensorChar = 'M' then
      { If in module, ie second section, then increment device number accordingly }
      Result := Result + HALF_MAX_POINTS_SYS_SENSOR;
  end else
  begin  { The device number is a pure integer, but may have a '+' indicating the device
          has sub-addresses active}
    P := AnsiPos('+',sgrdLoop.Cells[1, ARow]);
    if P > 0 then
      Result := StrToInt (LeftStr(sgrdLoop.Cells[1, ARow], P-1))
    else
      Result := StrToInt (sgrdLoop.Cells[1, ARow]);
  end;
end;

function TMainForm.GetMax_OutputDelay: integer;
begin
  Result := ProtocolButtonsFile.Max_OutputDelay;
end;

function TMainForm.GetNoOfLoops: integer;
begin
  Result := SiteFile.NoOfLoops;
end;

function TMainForm.GetSiteFile: TSiteFile;
begin
  Result := PgmData.SiteFile;
end;

function TMainForm.GetXFP_Network: TXFP_Network;
begin
  Result := SiteFile.XFP_Network;
end;

procedure TMainForm.radgrpZoneGroupClick(Sender: TObject);
begin
  UpdateZoneGroupList;
end;

procedure TMainForm.ShowSummaryByDeviceType( const frmLoop : TFrmLoop );
{ This procedure sorts the devices by device type, with the order determined by
the order of devices, and not by alphabetical order}
var
  DevCount: integer;
  iCount: integer;
  iIndex: integer;
begin
  { Initialise current row of string grid }
  CurrentRow := -1;

  { Go through all the zone-based devices first }
  for iCount := 1 to (MAX_DEVICE_TYPE + 1) do begin
    { MAX_DEVICE_TYPE + 1 will be remapped to device type 0, to ensure that points
    not fitted with a device are shown after points fitted with devices }
    if iCount = MAX_DEVICE_TYPE + 1 then iIndex := 0
    else iIndex := iCount;
    { Check for all devices }
    for DevCount := 1 to frmLoop1.AFPLoop.NoLoopDevices do begin
      { Check to see if device belongs to a zone, and that zone is the one we
      currently are checking }
      if (frmLoop.AFPLoop.Device[DevCount].DevType = iIndex)  then begin
        ShowDeviceProperties (frmLoop, DevCount, TRUE);
      end; {if device is zone-based}
    end; {for DevCount}
  end; {for iCount}
end;

procedure TMainForm.Z01G01Enter(Sender: TObject);
begin
  { Change the background to yellow to show that it has the focus }
  (Sender as TCheckBox).Color := clYellow;
end;

procedure TMainForm.Z01G01Exit(Sender: TObject);
begin
  { Since this check box has lost the focus, change the background colour back to grey }
  (Sender as TCheckBox).Color := clBtnFace;
end;

procedure TMainForm.txtMaintDateExit(Sender: TObject);
begin
  { Check to see if a correct date has been entered }
  try
    SiteFile.MaintenanceDate := StrToDate (txtMaintDate.Text);
    if SiteFile.MaintenanceDate < Date then begin
      { Check to see if the date has already passed }
      MessageDlg ('Today''s date is ' + FormatDateTime ('dd/mm/yyyy', Date) + '. Please enter a date ' +
      'after this date.', mtWarning, [mbOK], 0);
      txtMaintDate.SetFocus;
    end;
  except
    MessageDlg ('The date that you entered is invalid. Please check and re-enter the date.', mtError, [mbOK], 0);
    txtMaintDate.SetFocus;
  end;
end;

procedure TMainForm.sgrdZoneSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
begin
  { If description is selected, allow the user to edit the grid, otherwise don't }
  if (ACol = 1) and (ARow > 1) then
    sgrdZone.Options := sgrdZone.Options + [goEditing]
  else begin
    sgrdZone.Options := sgrdZone.Options - [goEditing];
  end;

  { Record the currently selected zone/group/set }
  ZoneGroupIndex := ARow - 1;

  { Check to see if the zone has been modified }
  if FZoneModified then begin
    { If it has, update the loop summary grid with the new description }
    FZoneModified := false;
    UpdateLoopSummary;
  end;
end;

procedure TMainForm.sgrdZoneSetEditText(Sender: TObject; ACol,
  ARow: Integer; const Value: String);
var
  Description: string;
begin
  { If the second column is chosen, then update the name with the type description }
  if (ACol = 1) then begin
    { Limit the number of characters in this column to 15 }
//    if Length (Value) > 15 then begin
//      Description := Copy (Value, 1, 15);
//      sgrdZone.Cells [ACol, ARow] := Description;
//    end
//    else
      Description := Value;

    SiteFile.ZoneList.Zone[ARow - 1].Name := Description;
    { Flag that the zone has been modified }
    ZoneModified := true;
  end;
end;

procedure TMainForm.sgrdZoneKeyPress(Sender: TObject; var Key: Char);
begin
  with sgrdZone do begin
    { If the return key is pressed}
    if (Key = chr (VK_RETURN)) then begin
      { If it is not the last row, go to the next row, which will automatically
      update the loop summary grid }
      if Row < (RowCount - 1) then begin
        Row := Row + 1;
      end;
    end;
    { If the character is not a non-printing character, and the maximum length for
    the description has been reached, set the character to null so it will not
    print }
    if Row = 1 then
      Key := #0;

    if (Length (Cells [1, Row]) > 999) and (Key >= ' ') then begin
      Key := #0;
      MessageBeep (MB_ICONEXCLAMATION);
    end;
  end;
end;

procedure TMainForm.OnLoad(Sender : TObject; var pOK : boolean; const pFileName : string );
begin
  OnNew( Sender );
end;

procedure TMainForm.OnNew(Sender: TObject);
var
  WhichPage: integer;
begin
  sgrdLoop.Col := 0;
  sgrdLoop.Row := 1;

  frmLoop1.AFPLoop := SiteFile.AFPLoop1;
  frmLoop2.AFPLoop := SiteFile.AFPLoop2;
  frmDeviceName.AFPLoop1 := SiteFile.AFPLoop1;;
  frmDeviceName.AFPLoop2 := SiteFile.AFPLoop2;

  Int_Tone_Box.ItemIndex := SiteFile.IntermittentTone;
  Int_Tone_Box_Apollo.ItemIndex := SiteFile.IntermittentTone;
  Int_Tone_Box_CAST.ItemIndex := SiteFile.IntermittentTone;
  Cont_Tone_Box.ItemIndex := SiteFile.ContinuousTone;
  Cont_Tone_Box_Apollo.ItemIndex := SiteFile.ContinuousTone;
  Cont_Tone_Box_CAST.ItemIndex := SiteFile.ContinuousTone;

  WhichPage:=ConfigPages.ActivePageIndex;

  //ClearInfo;
  //RefreshControls;
  //Modified := false;
  //RefreshAllPages;

  PanelNumber.Value := SiteFile.PanelNumber;

  RefreshProtocolControls;
  ConfigPages.ActivePageIndex:=WhichPage;

  { Clear View event log - this is not saved with the file }
  EventLog.Clear;
end;

procedure TMainForm.OpenInifile(Sender: TObject);
begin
  SiteFile.Load;
end;

function TMainForm.ValidFileType( const IniFile : TIniFile ): Boolean;
begin
  Result := (Inifile.ReadString ('AFP Header', 'FileType', '') = 'Prog Tools');
end;

{ In the following functions, frmLoop1 is used by default for all loop objects,
since both Loop1 & 2 will be of the same protocol. }
procedure TMainForm.LoadConfigData( const IniFile : TIniFile );
var
  iIndex: integer;
  iCount: integer;
  LoopType: integer;
  RTFName: string;
  S, S1 : string;
  P, P1 : integer;
begin
  { Load the version numbers so version dependant stuff will work}
  SiteFile.MainVersion := Inifile.ReadString ('AFP Header', 'Main Version', 'Not Available');
  SiteFile.FrontPanel := Inifile.ReadString ('AFP Header', 'Front Panel', 'Not Available');
  SiteFile.Loop1 := Inifile.ReadString ('AFP Header', 'Loop 1', 'Not Available');
  SiteFile.Loop2 := Inifile.ReadString ('AFP Header', 'Loop 2', 'Not Available');

//here we can perhaps recognise an AFP type of file and ask if the user wants a conversion
//to the XFP file type.....

  ConvertFromAFP := FALSE;
  if Inifile.ReadString ('AFP Header', 'Panel Type', 'AFP') <> 'XFP' then
  begin
    if SiteFile.Loop2 <> '' then
    begin
      if MessageDlg('Do you want to convert from this AFP configuration file?',
        mtConfirmation, [mbYes, mbNo], 0) = mrYes then
      begin
        ConvertFromAFP := TRUE;
      end;
    end;
    end;

  { Load the loop type }
  LoopType := Inifile.ReadInteger ('Loop', 'Type', 0);
  if ConvertFromAFP = TRUE then begin
    LoopType := LoopType - 2;
  end;

  { Set loop type depending upon integer read }
  case LoopType of
    0:
    begin
      //frmLoop1.AFPLoop.SystemType := stApolloLoop;
      //frmLoop2.AFPLoop.SystemType := stApolloLoop;
      SiteFile.AFPLoop1.SystemType := stApolloLoop;
      SiteFile.AFPLoop2.SystemType := stApolloLoop;
    end;
    1:
    begin
      //  frmLoop1.AFPLoop.SystemType := stHochikiLoop;
      //  frmLoop2.AFPLoop.SystemType := stHochikiLoop;
      SiteFile.AFPLoop1.SystemType := stHochikiLoop;
      SiteFile.AFPLoop2.SystemType := stHochikiLoop;
    end;
    2:
    begin
      //frmLoop1.AFPLoop.SystemType := stSysSensorLoop;
      //frmLoop2.AFPLoop.SystemType := stSysSensorLoop;
      SiteFile.AFPLoop1.SystemType := stSysSensorLoop;
      SiteFile.AFPLoop2.SystemType := stSysSensorLoop;
    end;
    3:
    begin
      //frmLoop1.AFPLoop.SystemType := stNittanLoop;
      //frmLoop2.AFPLoop.SystemType := stNittanLoop;
      SiteFile.AFPLoop1.SystemType := stNittanLoop;
      SiteFile.AFPLoop2.SystemType := stNittanLoop;
    end;
    4:
    begin
      //frmLoop1.AFPLoop.SystemType := stCASTLoop;
      //frmLoop2.AFPLoop.SystemType := stCASTLoop;
      SiteFile.AFPLoop1.SystemType := stCASTLoop;
      SiteFile.AFPLoop2.SystemType := stCASTLoop;
    end;
  end;

  { Load information for loop 1}
  LoadLoopInfo (IniFile, SiteFile.AFPLoop1, 1, LoopType);

  { Load information for loop 2}
  LoadLoopInfo (IniFile, SiteFile.AFPLoop2, 2, LoopType);

  { Load the zone configuration data }
  for iIndex := 1 to MAX_ZONE do
  begin
    with SiteFile.Zonelist.Zone[iIndex] do
    begin
      Name := Inifile.ReadString ('Zone Config', 'Zone' + IntToStr (iIndex),
        'Zone ' + IntToStr (iIndex));
      { For the delays, the value is 10x actual, so value can be stored as integer }
      SounderDelay := Inifile.ReadInteger ('SounderTimers', 'Zone' + IntToStr (iIndex), 0);
      RemoteDelay := Inifile.ReadInteger ('RemoteTimers', 'Zone' + IntToStr (iIndex), 0);
      Relay1Delay := Inifile.ReadInteger ('Relay1Timer', 'Zone' + IntToStr (iIndex), 0);
      Relay2Delay := Inifile.ReadInteger ('Relay2Timer', 'Zone' + IntToStr (iIndex), 0);


      Dependancy[1] := Inifile.ReadInteger('Dependancy1',  'Zone' + IntToStr (iIndex), 4);
      Dependancy[2] := Inifile.ReadInteger('Dependancy2',  'Zone' + IntToStr (iIndex), 0);
      Dependancy[3] := Inifile.ReadInteger('Dependancy3',  'Zone' + IntToStr (iIndex), 0);
      Dependancy[4] := Inifile.ReadInteger('Dependancy4',  'Zone' + IntToStr (iIndex), 4);
      Dependancy[5] := Inifile.ReadInteger('Dependancy5',  'Zone' + IntToStr (iIndex), 0);
      Dependancy[6] := Inifile.ReadInteger('Dependancy6',  'Zone' + IntToStr (iIndex), 0);

      Detector := Inifile.ReadBool ('SounderEnable', 'Zone' + IntToStr (iIndex), false);
      MCP := Inifile.ReadBool ('MCPEnable', 'Zone' + IntToStr (iIndex), false);
      NonFire := Inifile.ReadBool ('NonFireEnable', 'Zone' + IntToStr (iIndex), false);
      EndDelays := Inifile.ReadBool ('EndDelays', 'Zone' + IntToStr (iIndex), false);

      {Load the group configuration - do this for each zone }
      for iCount := 1 to MAX_GROUP do
      begin
        Group[iCount] := IniFile.ReadInteger ('GroupConfig', 'Z' + IntToStr (iIndex) +
          'G' + IntToStr (iCount), 0);
      end;
      { Load the output set configuration data - do this for each zone }
      for iCount := 1 to MAX_ZONE_SET do
      begin
        ZoneSet[iCount] := Inifile.ReadInteger ('SetConfig', 'Z' + IntToStr (iIndex) +
          'S' + IntToStr (iCount), 0);
      end;

    end; { with Data.Zonelist.Zone }
  end; { for each zone }

  SiteFile.DelayTimer := Inifile.ReadInteger('SetConfig',  'DelayTimer', 0);

  { Load the Set Silencable flags to the ZoneSet[0] record}
  for iCount := 0 to MAX_ZONE_SET do begin
    SiteFile.Zonelist.Zone[0].ZoneSet[iCount] := Inifile.ReadInteger ('SetConfig', 'Z' + IntToStr (0) + 'S'
                                             + IntToStr (iCount),0);
  end;

  { Load info for site config }
  with Inifile, SiteFile do
  begin
    MaintenanceString := ReadString ('Maintenance', 'Text', '');
    QuiescentString := ReadString ('Quiescent', 'Text', '');
    MaintenanceDate := ReadDate ('Maintenance', 'Date', Date);
    DateEnabled := ReadBool ('Maintenance', 'Enabled', false);
    AL2Code := ReadString ('AccessCodes', 'L2', '');
    AL3Code := ReadString ('AccessCodes', 'L3', '');
    FaultLockOut := ReadInteger ('FaultLockout', 'Time', 10);
    PhasedDelay := ReadInteger ('PhasedDelay', 'Time', 65535);
    SoundersPulsed := ReadBool ('PhasedDelay', 'Pulsed On', true);
    ClientName := ReadString ('Site Config', 'Client Name', '');
    ClientAddress1 := ReadString ('Site Config', 'Client Address 1', '');
    ClientAddress2 := ReadString ('Site Config', 'Client Address 2', '');
    ClientAddress3 := ReadString ('Site Config', 'Client Address 3', '');
    ClientAddress4 := ReadString ('Site Config', 'Client Address 4', '');
    ClientAddress5 := ReadString ('Site Config', 'Client Address 5', '');
    PanelLocation := ReadString ('Site Config', 'Alarm Position', '');
    InstallerName := ReadString ('Site Config', 'Installer Name', '');
    InstallerAddress1 := ReadString ('Site Config', 'Installer Address 1', '');
    InstallerAddress2 := ReadString ('Site Config', 'Installer Address 2', '');
    InstallerAddress3 := ReadString ('Site Config', 'Installer Address 3', '');
    InstallerAddress4 := ReadString ('Site Config', 'Installer Address 4', '');
    InstallerAddress5 := ReadString ('Site Config', 'Installer Address 5', '');
    Engineer := ReadString ('Site Config', 'Engineer Name', '');
    EngineerNo := ReadString ('Site Config', 'Service Number', '');
    NightBegin := ReadString ('Site Config',' Night Begins', '00:00');
    NightEnd := ReadString ('Site Config',' Night Ends', '00:00');
    ReCalTime := ReadString ('Site Config', 'ReCalibrate at', '04:00');
    InvestigationPeriod := ReadInteger ('Investigation', 'Period', 300);
    InvestigationPeriod1 := ReadInteger ('Investigation', 'Period1', 180);
    PanelSounder1_Group := ReadInteger('Panel_Sounder_Groups','P1', 1);
    PanelSounder2_Group := ReadInteger('Panel_Sounder_Groups','P2', 2);
    Day_Enable_Flags := ReadInteger('Enable_Flags','Day',0);
    Night_Enable_Flags := ReadInteger('Enable_Flags','Night',0);
    Discovery_Polling_LED := ReadBool ('Polling_LED', 'Enabled', false);
    MCP_Delay := ReadInteger('Site Config','MCP Delay', 1);
    Detector_Delay := ReadInteger('Site Config','Detector Delay', 9);
    IO_Delay := ReadInteger('Site Config','IO Delay', 1);
    ReSound_Function := ReadBool('Site Config', 'Re_Sound', FALSE);
    BST_Adjustment := ReadBool ('BST Adjustment', 'Enabled', TRUE);
    RealTime_Event_Output := ReadBool ('RealTime Event Output', 'Enabled', FALSE);
  end;

  SiteFile.IntermittentTone := Inifile.ReadInteger('Tone',  'Int', 1);
  SiteFile.ContinuousTone := Inifile.ReadInteger('Tone',  'Cont',0);

  { Load info for network config }
  for iCount := 1 to MAX_REPEATER do begin
    { Repeaters }
    SiteFile.RepeaterList.Repeater[iCount].Name :=
      Inifile.ReadString ('Repeater Name', 'Repeater' + IntToStr (iCount), '');
    SiteFile.RepeaterList.Repeater[iCount].Fitted :=
      Inifile.ReadBool ('Repeater Fitted', 'Repeater' + IntToStr (iCount), false);
  end;

  for iCount := 1 to MAX_PANELS do begin
    with SiteFile.XFP_Network do begin
      PAccept_Faults[iCount] := Inifile.ReadBool('Accept Faults','Panel' + IntToStr(iCount), FALSE);
      PAccept_Alarms[iCount] := Inifile.ReadBool('Accept Alarms','Panel' + IntToStr(iCount), FALSE);
      PAccept_Controls[iCount] := Inifile.ReadBool('Accept Controls','Panel' + IntToStr(iCount), FALSE);
      PAccept_Disablements[iCount] := Inifile.ReadBool('Accept Disablements','Panel' + IntToStr(iCount), FALSE);
      PAccept_Occupied[iCount] := Inifile.ReadBool('Accept Occupied','Panel' + IntToStr(iCount), FALSE);
    end;
  end;


  { Load main panel name }
  SiteFile.RepeaterList.PanelName := Inifile.ReadString ('Network Config', 'Panel Name', 'Main Panel');
  { Load segment number }
  SiteFile.RepeaterList.Segment := Inifile.ReadInteger ('Network Config', 'Segment Number', 1);

  { Save comments as RTF file with same name as configuration file, merely different
  extension }
  RTFName := Copy (Inifile.Filename, 1, Length (Inifile.Filename) - 3) + 'RTF';
  { If this file exists, then load it into the comments memo, otherwise clear the
  comments memo }
  if FileExists (RTFName) then
    CommentsEdit.Lines.LoadFromFile (RTFName)
  else
    CommentsEdit.Clear;

  SiteFile.Comments.Text := CommentsEdit.Text;

  { Re-Load the version numbers because they will have been cleared when ChangeProtocol called}
  SiteFile.MainVersion := Inifile.ReadString ('AFP Header', 'Main Version', 'Not Available');
  SiteFile.FrontPanel := Inifile.ReadString ('AFP Header', 'Front Panel', 'Not Available');
  SiteFile.Loop1 := Inifile.ReadString ('AFP Header', 'Loop 1', 'Not Available');
  SiteFile.Loop2 := Inifile.ReadString ('AFP Header', 'Loop 2', 'Not Available');

  { Load up the device name list }

  { Now redundant because both point to the same instances of PgmDat.AFPLoopn
  frmDeviceName.Loop1 := frmLoop1.Loop;
  frmDeviceName.Loop2 := frmLoop2.Loop;
  }
  frmDeviceName.Rebuild_List( SiteFile.AFPLoop1, SiteFile.AFPLoop2 );
  {
  frmLoop1.Loop := frmDeviceName.Loop1;
  frmLoop2.Loop := frmDeviceName.Loop2;
  }

  { Load the Cause & Effect equations }
  for iIndex := 1 to PgmData.MAX_CE_EVENTS do begin
    S := Inifile.ReadString ('C_EEvent', 'Event' + IntToStr (iIndex), '0/0/0/0/0/0/');
    P := 1;
    for iCount := 1 to 6 do
    begin
      P1 := Pos('/', S);
      S1 := MidStr(S, P, P1-P);
      SiteFile.C_ETable.C_ETable[iIndex].C_EArray[iCount] := StrToInt(S1);

      //S := RightStr(S, StrLen(pchar(S))-P1);
      S := RightStr(S, Length(S)-P1);
    end;
  end;

  { Load the Cause & Effect Time event markers }
  for iIndex := 1 to MAX_CE_TIMER_EVENTS do
    SiteFile.C_ETable.Time_Events[iIndex] := Inifile.ReadString ('C_ETimes', 'Event' + IntToStr (iIndex), '00:00');


  { Now update all pages, returning to the current page once done }
  //RefreshAllPages; now done in OnNew

  { Ensure that the configuration is unmodified when it is loaded }
  //FModified := false;

  OnNew( Self ); // force update of components
end;

procedure TMainForm.LoadLoopInfo ( const IniFile : TIniFile; const pLoop: TAFPLoop; Loop: integer; LoopType: integer);
var
  iIndex: integer;
  I : integer;
  NoDevices: integer;
  DevIndex: integer;
begin
  { Determine the number of devices for this protocol }
  //NoDevices := frmLoop1.AFPLoop.NoLoopDevices;
  NoDevices := pLoop.NoLoopDevices;

  { Load the device information for the loop }
  for iIndex := 1 to NoDevices do
  begin
    with pLoop.Device[iIndex] do
    begin
      { If the loop type is system sensor, and the iIndex number indicates that it
      lies in the module section, then increment the iIndex by 1 since the iIndex
      number for modules start at 101, not 100 }
      if (pLoop.SystemType = stSysSensorLoop) and (iIndex > HALF_MAX_POINTS_SYS_SENSOR) then
      begin
        DevIndex := iIndex + 1;
      end
      else
      begin
        DevIndex := iIndex;
      end;

      { Device description }
      DevType := Inifile.ReadInteger ('Detector Type', 'L' + IntToStr (Loop) +
        'D' + IntToStr (DevIndex), D_NOT_FITTED);

      if ConvertFromAFP = TRUE then
      begin
        if DevType < 8 then
        begin
          DevType := DevType * 4;
        end;
        if DevType = 0 then
        begin
          DevType := D_NOT_FITTED;
        end;
      end;

      if DevType < D_NOT_FITTED then
      begin
        Name := Inifile.ReadString ('Detector Names', 'L' + IntToStr (Loop) + 'D' +
                                     IntToStr (DevIndex), 'No name allocated');
        SubName[1] := Inifile.ReadString ('Detector SubName1', 'L' + IntToStr (Loop) + 'D' +
                                     IntToStr (DevIndex), 'No name allocated');
        SubName[2] := Inifile.ReadString ('Detector SubName2','L' + IntToStr (Loop) + 'D' +
                                     IntToStr (DevIndex), 'No name allocated');
        SubName[3] := Inifile.ReadString ('Detector SubName3','L' + IntToStr (Loop) + 'D' +
                                     IntToStr (DevIndex), 'No name allocated');

        Zone := Inifile.ReadInteger ('Detector Zone', 'L' + IntToStr (Loop) +
          'D' + IntToStr (DevIndex), 1);
      end
      else
      begin
        Name := 'No name allocated';
        SubName[1] := 'No name allocated';
        SubName[2] := 'No name allocated';
        SubName[3] := 'No name allocated';
        Zone := 0;
      end;

      { Check to see if it is a valid Zone/Group/Set. If not, reset it to 1 }
      if Zone < 0 then
      begin
        Zone := 1;
      end;

      if Zone > MAX_ZONE then
      begin
        Zone := 1;
      end;

      if ConvertFromAFP = TRUE then
      begin
        if Zone > MAX_LOCAL_ZONES then
        begin
          Zone := Zone - 33;
        end;
      end;

      { Read the SharedData[] array}
      for I := 1 to 8 do
      begin
        SharedData[I] := Inifile.ReadInteger('SharedData',
                          'L' + IntToStr (Loop) +
                          'D' + IntToStr (DevIndex) +
                          'SD' + IntToStr (I),255);
      end;
      if SharedData[1] = 255 then
      begin
        pLoop.SetDefaults(DevIndex);
      end;
    end;
  end;
end;

procedure TMainForm.UpdateConfigData;
begin
  { Determine which is the active page, and only update that page }
  case ConfigPages.ActivePageIndex of
    0: UpdateLoopSummaryPage;
    1: UpdateLoop1Page;
    2: UpdateLoop2Page;
    3: UpdateZoneConfigPage;
    4: UpdateGroupConfigPage;
    5: UpdateSetConfigPage;
    6: UpdateSiteConfigPage;
    8: UpdateNetworkConfigPage;
  end;
  RefreshC_EPage;
end;

procedure TMainForm.UpdateLoopSummaryPage;
begin
  { Update the zone/group/set list }
  UpdateZoneGroupList;

  { Show the loop summary report }
  UpdateLoopSummary;
end;

procedure TMainForm.UpdateLoop1Page;
begin
  frmLoop1.UpdateTools;
  frmLoop1.FrameResize (Self);
  frmLoop1.Hide;
  frmLoop1.Show;
end;

procedure TMainForm.UpdateLoop2Page;
begin
  frmLoop2.UpdateTools;
  frmLoop2.FrameResize (Self);
  frmLoop2.Hide;
  frmLoop2.Show;
end;

procedure TMainForm.UpdateZoneConfigPage;
var
  iIndex: integer;
  spedValue: integer;
  chk: Boolean;
  TString : string;
begin
  { Do not update if the form is being loaded as the zonelist object has not been
  fully created yet }
  if (csLoading in ComponentState) then Exit;

  BeginUpdate;
  try
    { Update the zone config page }
    for iIndex := 1 to MAX_ZONE do begin
      TEdit (FindComponent ('txtZPZone' + IntToStr (iIndex))).Text := SiteFile.Zonelist.Zone[iIndex].Name;
      spedValue := SiteFile.ZoneList.Zone[iIndex].SounderDelay;
      TCTecSpinEdit (FindComponent ('spnZPSounder' + IntToStr (iIndex))).LoadValue (spedValue);
      spedValue := SiteFile.Zonelist.Zone[iIndex].RemoteDelay;
      TCTecSpinEdit (FindComponent ('spnZPOutput' + IntToStr (iIndex))).LoadValue (spedValue);
      spedValue := SiteFile.Zonelist.Zone[iIndex].Relay1Delay;
      TCTecSpinEdit (FindComponent ('spnZPRelay1' + IntToStr (iIndex))).LoadValue (spedValue);
      spedValue := SiteFile.Zonelist.Zone[iIndex].Relay2Delay;
      TCTecSpinEdit (FindComponent ('spnZPRelay2' + IntToStr (iIndex))).LoadValue (spedValue);

      if iIndex <= MAX_ZONE-MAX_PANELS then
      begin
        if (SiteFile.ZoneList.Zone[iIndex].Dependancy[1] <> 4) or
           (SiteFile.ZoneList.Zone[iIndex].Dependancy[4] <> 4) then
        begin
          TString := 'Click for dependency options......';
          TEdit (FindComponent ('txtZPDependancy' + IntToStr (iIndex))).Text := TString;
          TEdit (FindComponent ('txtZPDependancy' + IntToStr (iIndex))).Color := clYellow;
        end else
        begin
          TString := 'Normal, Click to change...';
          TEdit (FindComponent ('txtZPDependancy' + IntToStr (iIndex))).Text := TString;
          TEdit (FindComponent ('txtZPDependancy' + IntToStr (iIndex))).Color := clWhite;
        end;

        chk := SiteFile.Zonelist.Zone[iIndex].Detector;
        TCheckBox (FindComponent ('chkZPDetector' + IntToStr (iIndex))).Checked := chk;
        chk := SiteFile.Zonelist.Zone[iIndex].MCP;
        TCheckBox (FindComponent ('chkZPMCP' + IntToStr (iIndex))).Checked := chk;
        chk := SiteFile.Zonelist.Zone[iIndex].EndDelays;
        TCheckBox (FindComponent ('chkZPEndDelays' + IntToStr (iIndex))).Checked := chk;
       end;
    end;

    for iIndex := 1 to MAX_ZONE
    do
    begin
      if (SiteFile.ZoneList.Zone[iIndex].Dependancy[1] = 5) or
           (SiteFile.ZoneList.Zone[iIndex].Dependancy[4] = 5) then
      begin
        if SiteFile.InvestigationPeriod + SiteFile.InvestigationPeriod1 +
            SiteFile.ZoneList.Zone[iIndex].SounderDelay > Max_OutputDelay then
        begin
          TTimeSpinButton (FindComponent ('spnZPSounder' + IntToStr (iIndex))).Font.Color := clRed;

          if frmDelay_Time_Prompt.CheckBox1.State = cbUnChecked then
          begin
            frmDelay_Time_Prompt.ShowModal;
          end;
        end
        else
        begin
          TTimeSpinButton (FindComponent ('spnZPSounder' + IntToStr (iIndex))).Font.Color := clBlack;
        end;

        if SiteFile.InvestigationPeriod + SiteFile.InvestigationPeriod1 +
            SiteFile.ZoneList.Zone[iIndex].RemoteDelay > Max_OutputDelay then
        begin
          TTimeSpinButton (FindComponent ('spnZPOutput' + IntToStr (iIndex))).Font.Color := clRed;

          if frmDelay_Time_Prompt.CheckBox1.State = cbUnChecked then
          begin
            frmDelay_Time_Prompt.ShowModal;
          end;
        end
        else
        begin
          TTimeSpinButton (FindComponent ('spnZPOutput' + IntToStr (iIndex))).Font.Color := clBlack;
        end;
      end
      else
      begin
        TTimeSpinButton (FindComponent ('spnZPSounder' + IntToStr (iIndex))).Font.Color := clBlack;
       TTimeSpinButton (FindComponent ('spnZPOutput' + IntToStr (iIndex))).Font.Color := clBlack;
      end;
    end;
  finally
    EndUpdate;
  end;
end;

procedure TMainForm.UpdateGroupConfigPage;
begin
  { Do not update if the form is being loaded as the zonelist object has not been
  fully created yet }
  if not (csLoading in ComponentState) then
  begin
    BeginUpdate;
    try
      dwgGroupConfig.Refresh;
      spnPanelSounder1.Value := SiteFile.PanelSounder1_Group;
      spnPanelSounder2.Value := SiteFile.PanelSounder2_Group;


    finally
      EndUpdate;
    end;
  end;
end;


procedure TMainForm.UpdateSetConfigPage;
var
   i : integer;
begin
  { Update set config page }
  BeginUpdate;
  try
    dwgSetsGrid.Invalidate;

    { Update the silenceable check boxes }

    for i := 1 to MAX_ZONE_SET do
    begin
      TCheckBox (FindComponent ('chkSetSil' + IntToStr (i))).Checked := (SiteFile.ZoneList.Zone[0].ZoneSet[i] = 1);
    end;
    {Files before Version 6.3 have blank Set Names, if blank create new ones}
    if PgmData.SiteFile.GroupList.OutputSet[0].Name = '' then
    begin
      PgmData.SiteFile.GroupList.OutputSet[0].Name := OUTPUT_SET_ZERO_NAME;
      for i := 1 to MAX_ZONE_SET do
      begin
        PgmData.SiteFile.GroupList.OutputSet[i].Name := 'Set ' + IntToStr( i );
      end;
    end;

  finally
    EndUpdate;
  end;
end;

procedure TMainForm.UpdateSiteConfigPage;
begin
  BeginUpdate;
  try
    { Update the site configuration page }
    with SiteFile do begin
      txtClientName.Text := ClientName;
      txtClientAdd1.Text := ClientAddress1;
      txtClientAdd2.Text := ClientAddress2;
      txtClientAdd3.Text := ClientAddress3;
      txtClientAdd4.Text := ClientAddress4;
      txtClientAdd5.text := ClientAddress5;
      txtInstallName.Text := InstallerName;
      txtInstallAdd1.Text := InstallerAddress1;
      txtInstallAdd2.Text := InstallerAddress2;
      txtInstallAdd3.Text := InstallerAddress3;
      txtInstallAdd4.Text := InstallerAddress4;
      txtInstallAdd5.Text := InstallerAddress5;
      txtLvl2.Text := AL2Code;
      txtLvl3.Text := AL3Code;
      txtPanelLocation.Text := PanelLocation;
      txtEngineer.Text := Engineer;
      txtServiceNo.Text := EngineerNo;
      txtMaintString.Text := MaintenanceString;
      txtQuiesString.Text := QuiescentString;
      txtMaintDate.Text := FormatDateTime ('dd/mm/yyyy', MaintenanceDate);
      PhasedDelaySpin.LoadValue (PhasedDelay);
      UploadTimeSelect.Checked := CopyTime;
      txtNight_Begin.Text := NightBegin;
      txtNight_End.Text := NightEnd;
      txtReCal.Text := ReCalTime;
      stxtMain.Caption := MainVersion;
      spnInvestigation.LoadValue(InvestigationPeriod);
      spnInvestigation1.LoadValue(InvestigationPeriod1);

      if (Day_Enable_Flags and 1) > 0 then
        DayCB1.State := cbChecked
      else
        DayCB1.State := cbUnChecked;
      if (Day_Enable_Flags and 2) > 0 then
        DayCB2.State := cbChecked
      else
        DayCB2.State := cbUnChecked;
      if (Day_Enable_Flags and 4) > 0then
        DayCB3.State := cbChecked
      else
        DayCB3.State := cbUnChecked;
      if (Day_Enable_Flags and 8) > 0then
        DayCB4.State := cbChecked
      else
        DayCB4.State := cbUnChecked;
      if (Day_Enable_Flags and 16) > 0then
        DayCB5.State := cbChecked
      else
        DayCB5.State := cbUnChecked;
      if (Day_Enable_Flags and 32) > 0then
        DayCB6.State := cbChecked
      else
        DayCB6.State := cbUnChecked;
      if (Day_Enable_Flags and 64) > 0 then
        DayCB7.State := cbChecked
      else
        DayCB7.State := cbUnChecked;

      if (Night_Enable_Flags and 1) > 0 then
        NightCB1.State := cbChecked
      else
        NightCB1.State := cbUnChecked;
      if (Night_Enable_Flags and 2) > 0 then
        NightCB2.State := cbChecked
      else
        NightCB2.State := cbUnChecked;
      if (Night_Enable_Flags and 4) > 0 then
        NightCB3.State := cbChecked
      else
        NightCB3.State := cbUnChecked;
      if (Night_Enable_Flags and 8) > 0 then
        NightCB4.State := cbChecked
      else
        NightCB4.State := cbUnChecked;
      if (Night_Enable_Flags and 16) > 0 then
        NightCB5.State := cbChecked
      else
        NightCB5.State := cbUnChecked;
      if (Night_Enable_Flags and 32) > 0 then
        NightCB6.State := cbChecked
      else
        NightCB6.State := cbUnChecked;
      if (Night_Enable_Flags and 64) > 0 then
        NightCB7.State := cbChecked
      else
        NightCB7.State := cbUnChecked;

      if Discovery_Polling_LED = true then
        chkPolling_LED.State := cbChecked
      else
        chkPolling_LED.State := cbUnChecked;

      if BST_Adjustment = true then
        BST_AdjustSelect.State := cbChecked
      else
        BST_AdjustSelect.State := cbUnChecked;

      BST_AdjustSelect.Visible := (SiteFile.MainVersion >= '09A00');

      if RealTime_Event_Output = true then
        chkRealTime_Event_Output.State := cbChecked
      else
        chkRealTime_Event_Output.State := cbUnChecked;

      chkRealTime_Event_Output.Visible := (SiteFile.MainVersion >= '09A07');

      spnMCP_Delay.Value := SiteFile.MCP_Delay;
      spnDetector_Delay.Value := SiteFile.Detector_Delay;
      spnIO_Delay.Value := SiteFile.IO_Delay;

      if (SiteFile.MainVersion > '08A43') then
        pnlMCP_Confirmation.Visible := true
      else
        pnlMCP_Confirmation.Visible := false;

      if (SiteFile.SystemType = stApolloLoop) and (SiteFile.MainVersion > '08A31') then
      begin
        chkPolling_LED.Visible := true;
        lblReCal.Visible := true;
        updReCal.Visible := true;
        txtReCal.Visible := true;
      end
      else if (SiteFile.SystemType = stCASTLoop) then
      begin
        chkPolling_LED.Visible := true;
        lblReCal.Visible := false;
        updReCal.Visible := false;
        txtReCal.Visible := false;
      end
      else if (SiteFile.SystemType = stHochikiLoop) then
      begin
        chkPolling_LED.Visible := false;
        lblReCal.Visible := true;
        updReCal.Visible := true;
        txtReCal.Visible := true;
      end;
    end;
  finally
    EndUpdate;
  end;
end;


procedure TMainForm.UpdateNetworkConfigPage;
var
  i: integer;
  iCheckBox : TCheckBox;
begin

  BeginUpdate;
  try
    for i := 1 to MAX_PANELS do
    begin
      with SiteFile.XFP_Network do
      begin
        TCheckBox (FindComponent ('RepFitted' + IntToStr (i))).Checked :=
          SiteFile.RepeaterList.Repeater[i].Fitted;

        iCheckBox := TCheckBox (FindComponent ('CheckBox' + IntToStr (i)));
        iCheckBox.Checked := PAccept_Faults[i];
        iCheckBox .Enabled := i <> PanelNumber.Value;

        iCheckBox := TCheckBox (FindComponent ('CheckBox' + IntToStr (i + 8)));
        iCheckBox.Checked := PAccept_Alarms[i];
        iCheckBox.Enabled := i <> PanelNumber.Value;

        iCheckBox :=TCheckBox (FindComponent ('CheckBox' + IntToStr (i + 16)));
        iCheckBox.Checked := PAccept_Controls[i];
        iCheckBox.Enabled := i <> PanelNumber.Value;

        iCheckBox := TCheckBox (FindComponent ('CheckBox' + IntToStr (i + 24)));
        iCheckBox.Checked := PAccept_Disablements[i];
        iCheckBox.Enabled := i <> PanelNumber.Value;

        iCheckBox := TCheckBox (FindComponent ('CheckBox' + IntToStr (i + 32)));
        iCheckBox.Checked := PAccept_Occupied[i];
        iCheckBox.Enabled := i <> PanelNumber.Value;
      end;

      TEdit (FindComponent ('PanelName' + IntToStr (i))).Text := SiteFile.RepeaterList.Repeater[i].Name;
    end;

    PanelNameEdit.Text := SiteFile.ZoneList.Zone[PanelNumber.Value + MAX_LOCAL_ZONES].Name;
  finally
    EndUpdate;
  end;
end;


(*
procedure TMainForm.SaveConfigData;
var
  LoopType: integer;
  iIndex: integer;
  iCount: integer;
  RTFName: string;
  S : string;
begin

    frmStatus.Show;
    frmStatus.btnCancel.Visible := false;
    frmStatus.StatusText := 'Saving Data to Disk, Please Wait...';
    Application.ProcessMessages;

  { Create/open the ini configuration file }
  Inifile := TInifile.Create (Inifilename);

  { Write ini file header for check that it is a valid inifile }
  Inifile.WriteString ('AFP Header', 'FileType', 'Prog Tools');
  Inifile.WriteString ('AFP Header', 'Main Version', SiteFile.MainVersion);
  Inifile.WriteString ('AFP Header', 'Panel Type', 'XFP');

  { Store loop type }
  if frmLoop1.AFPLoop.SystemType = stApolloLoop then LoopType := 0
  else if frmLoop1.AFPLoop.SystemType = stHochikiLoop then LoopType := 1
  else if frmLoop1.AFPLoop.SystemType = stSysSensorLoop then LoopType := 2
  else if frmLoop1.AFPLoop.SystemType = stNittanLoop then LoopType := 3
  else if frmLoop1.AFPLoop.SystemType = stCASTLoop then LoopType := 4
  else LoopType := 0; // Default to apollo loop on error

  { Save the loop type }
  Inifile.WriteInteger ('Loop', 'Type', LoopType);

  { Save information for loop 1}
  SaveLoopInfo (frmLoop1, 1, LoopType);

  { Save information for loop 2}
  SaveLoopInfo (frmLoop2, 2, LoopType);

  { Save the zone configuration data }
  for iIndex := 1 to SiteFile.MAX_ZONE do with SiteFile.Zonelist.Zone[iIndex] do begin
    Inifile.WriteString ('Zone Config', 'Zone' + IntToStr (iIndex), Name);
    Inifile.WriteInteger ('SounderTimers', 'Zone' + IntToStr (iIndex), SounderDelay);
    Inifile.WriteInteger ('RemoteTimers', 'Zone' + IntToStr (iIndex), RemoteDelay);
    Inifile.WriteInteger ('Relay1Timer', 'Zone' + IntToStr (iIndex), Relay1Delay);
    Inifile.WriteInteger ('Relay2Timer', 'Zone' + IntToStr (iIndex), Relay2Delay);

    Inifile.WriteInteger('Dependancy1',  'Zone' + IntToStr (iIndex), Dependancy[1]);
    Inifile.WriteInteger('Dependancy2',  'Zone' + IntToStr (iIndex), Dependancy[2]);
    Inifile.WriteInteger('Dependancy3',  'Zone' + IntToStr (iIndex), Dependancy[3]);
    Inifile.WriteInteger('Dependancy4',  'Zone' + IntToStr (iIndex), Dependancy[4]);
    Inifile.WriteInteger('Dependancy5',  'Zone' + IntToStr (iIndex), Dependancy[5]);
    Inifile.WriteInteger('Dependancy6',  'Zone' + IntToStr (iIndex), Dependancy[6]);

    Inifile.WriteBool ('SounderEnable', 'Zone' + IntToStr (iIndex), Detector);
    Inifile.WriteBool ('MCPEnable', 'Zone' + IntToStr (iIndex), MCP);
    Inifile.WriteBool ('NonFireEnable', 'Zone' + IntToStr (iIndex), NonFire);
    Inifile.WriteBool ('EndDelays', 'Zone' + IntToStr (iIndex), EndDelays);

    { Save the group configuration data - do this for each zone }
    for iCount := 1 to MAX_GROUP do begin
      IniFile.WriteInteger ('GroupConfig', 'Z' + IntToStr (iIndex) + 'G' + IntToStr (iCount),
        Group[iCount]);
    end;
    { Save the output set configuration data - do this for each zone }
    for iCount := 1 to MAX_ZONE_SET do begin
      Inifile.WriteInteger ('SetConfig', 'Z' + IntToStr (iIndex) + 'S' + IntToStr (iCount),
        ZoneSet[iCount]);
    end;

    Inifile.WriteInteger('SetConfig',  'DelayTimer', btnDelayTime.Value);

  end; { for each zone }


  { Save the Set Silencable flags from the ZoneSet[0] record}
  for iCount := 1 to MAX_ZONE_SET do begin
    Inifile.WriteInteger ('SetConfig', 'Z' + IntToStr (0) + 'S' + IntToStr (iCount),
       SiteFile.Zonelist.Zone[0].ZoneSet[iCount]);
  end;


  { Save info for site config }
  with Inifile, SiteFile.SiteConfig do begin
    WriteString ('Maintenance', 'Text', MaintenanceString);
    WriteDateTime ('Maintenance', 'Date', MaintenanceDate);
    WriteBool ('Maintenance', 'Enabled', DateEnabled);
    WriteString ('Quiescent', 'Text', QuiescentString);
    WriteString ('AccessCodes', 'L2', AL2Code);
    WriteString ('AccessCodes', 'L3', AL3Code);
    WriteInteger ('FaultLockout', 'Time', FaultLockout);
    WriteInteger ('PhasedDelay', 'Time', PhasedDelay);
    WriteBool ('PhasedDelay', 'Pulsed On', SoundersPulsed);
    WriteString ('Site Config', 'Client Name', ClientName);
    WriteString ('Site Config', 'Client Address 1', ClientAddress1);
    WriteString ('Site Config', 'Client Address 2', ClientAddress2);
    WriteString ('Site Config', 'Client Address 3', ClientAddress3);
    WriteString ('Site Config', 'Client Address 4', ClientAddress4);
    WriteString ('Site Config', 'Client Address 5', ClientAddress5);
    WriteString ('Site Config', 'Alarm Position', PanelLocation);
    WriteString ('Site Config', 'Installer Name', InstallerName);
    WriteString ('Site Config', 'Installer Address 1', InstallerAddress1);
    WriteString ('Site Config', 'Installer Address 2', InstallerAddress2);
    WriteString ('Site Config', 'Installer Address 3', InstallerAddress3);
    WriteString ('Site Config', 'Installer Address 4', InstallerAddress4);
    WriteString ('Site Config', 'Installer Address 5', InstallerAddress5);
    WriteString ('Site Config', 'Engineer Name', Engineer);
    WriteString ('Site Config', 'Service Number', EngineerNo);
    WriteString ('Site Config', 'Night Begins', NightBegin);
    WriteString ('Site Config', 'Night Ends', NightEnd);
    WriteString ('Site Config', 'ReCalibrate at', ReCalTime);
    WriteInteger ('Investigation', 'Period', InvestigationPeriod);
    WriteInteger ('Investigation', 'Period1', InvestigationPeriod1);
    WriteInteger('Panel_Sounder_Groups','P1', PanelSounder1_Group);
    WriteInteger('Panel_Sounder_Groups','P2', PanelSounder2_Group);

    WriteInteger('Enable_Flags','Day',Day_Enable_Flags);
    WriteInteger('Enable_Flags','Night',Night_Enable_Flags);

    WriteBool('Polling_LED', 'Enabled', Discovery_Polling_LED);

    WriteInteger('Site Config','MCP Delay', MCP_Delay);
    WriteInteger('Site Config','Detector Delay', Detector_Delay);
    WriteInteger('Site Config','IO Delay', IO_Delay);

    WriteBool('SiteConfig','ReSound', ReSound_Function);
    WriteBool('BST Adjustment', 'Enabled', BST_Adjustment);
    WriteBool('RealTime Event Output', 'Enabled', RealTime_Event_Output);
  end;

  if frmLoop1.AFPLoop.SystemType = stHochikiLoop then
  begin
    Inifile.WriteInteger('Tone',  'Int', Int_Tone_Box.ItemIndex);
    Inifile.WriteInteger('Tone',  'Cont', Cont_Tone_Box.ItemIndex);
  end else
  begin
    Inifile.WriteInteger('Tone',  'Int', Int_Tone_Box_Apollo.ItemIndex);
    Inifile.WriteInteger('Tone',  'Cont', Cont_Tone_Box_Apollo.ItemIndex);
  end;

  { Save info for network config }
  for iCount := 1 to SiteFile.MAX_REPEATER do begin
    { Repeaters }
    Inifile.WriteString ('Repeater Name', 'Repeater' + IntToStr (iCount),
      SiteFile.RepeaterList.Repeater[iCount].Name);
    Inifile.WriteBool ('Repeater Fitted', 'Repeater' + IntToStr (iCount),
      SiteFile.RepeaterList.Repeater[iCount].Fitted);
  end;

  for iCount := 1 to SiteFile.MAX_PANELS do begin
    with SiteFile.XFP_Network do begin
      Inifile.WriteBool('Accept Faults','Panel' + IntToStr(iCount), PAccept_Faults[iCount]);
      Inifile.WriteBool('Accept Alarms','Panel' + IntToStr(iCount), PAccept_Alarms[iCount]);
      Inifile.WriteBool('Accept Controls','Panel' + IntToStr(iCount), PAccept_Controls[iCount]);
      Inifile.WriteBool('Accept Disablements','Panel' + IntToStr(iCount), PAccept_Disablements[iCount]);
      Inifile.WriteBool('Accept Occupied','Panel' + IntToStr(iCount), PAccept_Occupied[iCount]);
    end;
  end;


  { Save Main panel name }
  IniFile.WriteString('Network Config', 'Panel Name', SiteFile.RepeaterList.PanelName);

  { Save segment number }
  IniFile.WriteInteger('Network Config', 'Segment Number', SiteFile.RepeaterList.Segment);

  { Save the Cause & Effect equations }
  for iIndex := 1 to SiteFile.MAX_CE_EVENTS do begin
    S := '';
    for iCount := 1 to 6 do
      S := S + IntToStr(SiteFile.C_ETable.C_ETable[iIndex].C_EArray[iCount]) + '/';

    Inifile.WriteString ('C_EEvent', 'Event' + IntToStr (iIndex), S);
  end;

  { Save the Cause & Effect Time event markers }
  for iIndex := 1 to MAX_CE_TIMER_EVENTS do
    Inifile.WriteString ('C_ETimes', 'Event' + IntToStr (iIndex), SiteFile.C_ETable.Time_Events[iIndex]);

  { Save comments as RTF file with same name as configuration file, merely different
  extension }
  RTFName := Copy (Inifile.Filename, 1, Length (Inifile.Filename) - 3) + 'RTF';
  CommentsEdit.Lines.SaveToFile (RTFName);

  { Reset the modified flag }
  Modified := false;

  frmStatus.Close;
end;
*)

(*
procedure TMainForm.SaveLoopInfo (frmLoop: TfrmLoop; Loop: integer; LoopType: integer);
var
  iIndex: integer;
  I : integer;
  NoDevices: integer;
  DetName: string;
  DevIndex: integer;
begin
  { Determine the number of devices for this protocol }
  NoDevices := frmLoop1.Loop.NoLoopDevices;

  { Save the device information for the loop }
  for iIndex := 1 to NoDevices do with frmLoop.Loop.Device[iIndex] do begin
     { If the loop type is system sensor, and the iIndex number indicates that it
    lies in the module section, then increment the iIndex by 1 since the iIndex
    number for modules start at 101, not 100 }
    if (frmLoop.Loop.SystemType = stSysSensorLoop) and (iIndex > HALF_MAX_POINTS_SYS_SENSOR) then
      DevIndex := iIndex + 1
    else
      DevIndex := iIndex;
    { Device description }
    Detname := 'L' + IntToStr (Loop) + 'D' + IntToStr (DevIndex);

//    Inifile.WriteString ('Detector Names', DetName, frmLoop.Loop.Device[iIndex].Name);
//    Inifile.WriteString ('Detector SubName1', DetName, frmLoop.Loop.Device[iIndex].SubName[1]);
//    Inifile.WriteString ('Detector SubName2', DetName, frmLoop.Loop.Device[iIndex].SubName[2]);
//    Inifile.WriteString ('Detector SubName3', DetName, frmLoop.Loop.Device[iIndex].SubName[3]);

    Inifile.WriteString ('Detector Names', DetName, Name);
    Inifile.WriteString ('Detector SubName1', DetName, SubName[1]);
    Inifile.WriteString ('Detector SubName2', DetName, SubName[2]);
    Inifile.WriteString ('Detector SubName3', DetName, SubName[3]);

    Inifile.WriteInteger ('Detector Type', 'L' + IntToStr (Loop) +
        'D' + IntToStr (DevIndex), DevType);

    { Each device is automatically enabled }
    Inifile.WriteBool ('Detector Disabled', 'L' + IntToStr (Loop) +
      'D' + IntToStr (DevIndex), false);
    { Zone device occupied }
     Inifile.WriteInteger ('Detector Zone', 'L' + IntToStr (Loop) +
       'D' + IntToStr (DevIndex), Zone);

    { write the SharedData[] array}
    for I := 1 to 8 do
    begin
      Inifile.WriteInteger('SharedData',
                           'L' + IntToStr (Loop) +
                           'D' + IntToStr (DevIndex) +
                           'SD' + IntToStr (I),SharedData[I]);

    end;

    TypeChanged := FALSE;
  end;
end;
*)
(*
{Save the information to the inifile}
procedure TMainForm.SaveInifile(Sender: TObject);
begin
  { Check to see if the code is OK on the site config page }
  ConfigPages.SetFocus;
  if not FCodeOK then Exit;
  { Check to see if the inifile has a name }
  if Inifilename = '' then begin
    { If it has not, then treat it as a Save As }
    SaveInifileAs (Sender);
    Modified := FALSE;
  end
  else begin
    { Otherwise use the same inifile name as already provided }
    SaveConfigData;
    Modified := FALSE
  end;
end;
*)

(*
procedure TMainForm.SaveInifileAs(Sender: TObject);
begin
  { Check to see if the code is OK on the site config page }
  ConfigPages.SetFocus;
  if not FCodeOK then Exit;
  { Open the Save dialog box }
  if SiteFile.SaveDialog.Execute then begin
    { Store the name of the inifile }

    Inifilename := SiteFile.SaveDialog.Filename;
    Inifilename := ConstructFileName(Inifilename);
    Inifile := TInifile.Create (InifileName);

    { Since we have the name of the Inifile, treat this as if the user pushed the
    Save button }
    SaveConfigData;

    { Update the caption of the main form to reflect the change in name }
    MainForm.Caption := MAINFORM_CAPTION + Inifilename;
  end;
end;
*)

procedure TMainForm.btnLoadAllFromPanelClick(Sender: TObject);
begin
  { Check to see if the code is OK on the site config page }
  ConfigPages.SetFocus;
  if not FCodeOK then Exit;
  LoadAll := true;
  FLoadLoopType := true;
  DataType := dtLoadLoop1;
  EstablishComms;
end;

procedure TMainForm.EstablishComms;
begin
  { Ensure the comms status form is shown, and the Cancel flag has been resest }
  frmStatus.btnCancel.Visible := TRUE;
  PanelComms.Cancel := false;
  frmStatus.Show;
  { Flush the buffers to ensure that there are no extraneous characters there }
  PanelComms.FlushBuffers (true, true);
  { If there has not been a handshake, and the comms is open, close the comms to
  ensure that the handshaking routines will start again from scratch }
  if (not PanelComms.HandshakeReceived) and PanelComms.IsOpen then
    PanelComms.CloseComms;

  { If comms has not been established, try to establish comms }
  if not PanelComms.IsOpen then
  begin
    PanelComms.ProtocolType := ppNone;
    frmStatus.Progress.Position := 1;
    frmStatus.Progress.Max := 101;
    frmStatus.StatusText :=
      'Establishing communications with the XFP';
    with PanelComms do begin
      CommsFault := false;
      HandshakeReceived := false;
      { On the fist OnTick event of the comms panel, it will detect that the
      protocol is ppNone. This will change the protocol to ppNew which then
      starts the entire handshaking procedure }
      OpenComms;

      RecordSent := TxReqMainVersion + #0;

      { If the comms is not open, flag a fault }
      if not IsOpen then begin
        frmStatus.Close;
        DataType := dtNone;
        CommsFault := true;
        MessageDlg ('Cannot open serial port', mtError, [mbOK], 0);
      end;
    end;
  end
  else if CheckNVM then
  begin
    { Check to see if the NVM link has been fitted }
    FCheckNVM := false;
    FDataRetry := 0;
    RecordSent := ReqDownload + #0;
  end
  else
  begin
    if SiteFile.MainVersion = 'Not Available' then
    begin
      RecordSent := TxReqMainVersion + #0;
    end
    else
    begin
       LoadInfo;
    end;
  end;
end;

procedure TMainForm.LoadInfo;
{ This procedure determines which info is required, then sends the relevant string }
var
  iIndex: integer;
  sTemp: string;
begin
  if FLoadLoopType then
  begin
    frmStatus.StatusText := 'Loading loop type';
    FDataCount:= 1;
    RecordSent := TxReqLoopType + #0;
  end
  else if FSaveLoopType then
  begin
    FDataCount := 1;
    frmStatus.StatusText := 'Saving loop type';
    if CheckVersionDependancy = true then SaveLoopType;
  end
  else
  begin
    { Now load the relevant data }
    case FDataType of
      dtLoadLoop1:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := frmLoop1.AFPLoop.NoLoopDevices;
        frmStatus.StatusText := 'Loading data for Loop 1 - Point 1';
        RecordSent := TxReqPData + #2 + #1 + #0;
      end;

      dtLoadLoop2:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := frmLoop1.AFPLoop.NoLoopDevices;
        frmStatus.StatusText := 'Loading data for Loop 2 - Point 1';
        RecordSent := TxReqPData + #2 + #1 + #1;
      end;

      dtLoadZoneList:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := MAX_ZONE;
        frmStatus.StatusText := 'Loading data for zone 1';
        RecordSent := TxReqZoneName + #1 + #1;
      end;

      dtLoadGroupList:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := MAX_ZONE;
        frmStatus.StatusText := 'Loading group data for zone 1';
        RecordSent := TxReqZGroup + #1 + #1;
      end;

      dtLoadSetList:
      begin
         FDataCount := 0;
         frmStatus.Progress.Position := 1;
         frmStatus.Progress.Max := MAX_ZONE;
         frmStatus.StatusText := 'Loading output set data for zone 1';
         RecordSent := TxReqZoneSet + #1 + #0;
      end;

      dtLoadSite:
      begin
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := 4;
        frmStatus.StatusText := 'Loading site configuration';
        RecordSent := TxReqMaintName + #0;
      end;

      dtLoadPName:
      begin
        FDataCount := 0;
        frmStatus.Progress.Position := 1;
        frmStatus.StatusText := 'Loading Device Names';
        RecordSent := TxReqPointName + #1 + chr(FDataCount);
      end;

      dtLoadEventHistory:
      begin;
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := MAX_EVENTS;
        frmStatus.StatusText := 'Loading Event History';

        if Event_Clear = mrYes then
        begin
          RecordSent := TxReqEvent + #3 + #0 + #0 + #1
        end
        else
        begin
          RecordSent := TxReqEvent + #3 + #0 + #0 + #0;
        end;
      end;

      dtLoadRepeaterList:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := MAX_REPEATER;
        frmStatus.StatusText := 'Loading Network data for Panel 1';
        RecordSent := TxReqRepeaterName + #1 + #1;
      end;

      dtLoadCE_Event:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := PgmData.MAX_CE_EVENTS;
        frmStatus.StatusText := 'Loading data for Event 1';
        RecordSent := TxReqC_EEvent + #1 + chr(FDataCount);
      end;

      dtLoadNetworkData:
      begin
        FDataCount := 1;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := 1;
        frmStatus.StatusText := 'Loading Network Matrix';
        RecordSent := TxReq_NetPanelData + #0;
      end;

      dtSavePData1:
      begin
        Loop := 1;
        FDataCount := 0;
        frmStatus.Progress.Max := SiteFile.AFPLoop1.NoLoopDevices;
        if CheckVersionDependancy = true then
        begin
          SavePointData (1);
        end;
      end;

      dtSavePData2:
      begin
        Loop := 2;
        FDataCount := 0;
        frmStatus.Progress.Max := SiteFile.AFPLoop1.NoLoopDevices;    // ???
        if CheckVersionDependancy = true then
        begin
          SavePointData (2);
        end;
      end;

      dtSavePName1:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := frmDeviceName.ComboBox1.Items.Count;
        if CheckVersionDependancy then
        begin
          SavePointName;
        end;
      end;

      dtSaveZoneTimer:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := MAX_ZONE;
        if CheckVersionDependancy = true then SaveZName;
      end;

      dtSaveZoneNonFire:
      begin
        FDataCount := 0;
        { Reset the number of retries }
        FDataRetry := 0;
        { Construct string to send }
        sTemp := '';
        for iIndex := 1 to MAX_ZONE do
        begin
          if SiteFile.Zonelist.Zone[iIndex].NonFire then
          begin
            sTemp := sTemp + 'T'
          end
          else
          begin
            sTemp := sTemp + 'F';
          end;
        { Change the time out value to 5 seconds }
        end;
        PanelComms.InputTimeOut := 5000;
        { Send constructed string }
        sTemp := BuildXFerString( TxSetZoneNonFire, sTemp );

        if CheckVersionDependancy then
        begin
          RecordSent := sTemp;
        end;
      end;

      dtSaveZGroup:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := MAX_ZONE;
        if CheckVersionDependancy then
        begin
          SaveZGroup;
        end;
      end;
      dtSaveZoneSet:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := MAX_ZONE;
        if CheckVersionDependancy then
        begin
          SaveZoneSet;
        end;
      end;

      dtSaveMaintName:
      begin
        frmStatus.StatusText := 'Saving site configuration';
        if CheckVersionDependancy then
        begin
          //RecordSent := TxSetMaintName + Chr (Length (txtMaintString.Text)+1) +  txtMaintString.Text + #0;
          RecordSent := BuildXFerString( TxSetMaintName, SiteFile.MaintenanceString + #0 );
        end;
      end;

      dtSaveCE_Event:
      begin
        FDataCount := 0;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := PgmData.MAX_CE_EVENTS;
        frmStatus.StatusText := 'Saving data for Event 1';
        SaveC_EData;
      end;

      dtSaveRepeaterFitted:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := MAX_REPEATER;
        if CheckVersionDependancy = true then SaveRepeaterFitted;
      end;

       dtSaveNetworkData:
      begin
        FDataCount := 0;
        frmStatus.Progress.Max := 1;
        if CheckVersionDependancy = true then SaveNetworkData;
      end;

    dtLoadFlashMemory:
      begin
        FDataCount := SpinEdit1.Value;
        frmStatus.Progress.Position := 1;
        frmStatus.Progress.Max := 500;
        frmStatus.StatusText := 'Loading Texts';
        RecordSent := TxReq_FlashData + #2 + #0 + #0;
      end;

      dtSaveFlashMemory:
        begin
          FDataCount := 0;
          frmStatus.Progress.Position := 1;
          frmStatus.Progress.Max := TextStrings.RowCount;
          frmStatus.StatusText := 'Saving Texts';
          if not SaveAll then if CheckVersionDependancy = true then SaveText;
        end;

    end; { case FDataType }
  end; { else not loop type }

end;

function TMainForm.CheckVersionDependancy(): boolean;
begin
     result := TRUE;
end;




procedure TMainForm.PanelCommsTick(Sender: TObject);
begin
  { If the comms panel is not open, the Cancel button has been pressed, or the user
  is on the View Event Log page, do nothing }
  if PanelComms.Cancel or (ConfigPages.ActivePage = tabViewLog)
  or (not PanelComms.IsOpen) then Exit;

  if PanelComms.CommsFault then begin
    { Since we cannot establish comms, close the channel }
    PanelComms.CloseComms;
    { Show warning message that there is a problem establishing comms with the AFP }
    MessageBeep( 0 );
    MessageDlg('An error occured opening the connection to the XFP.' +
               ' Please check that the correct COM port is selected,' +
               ' and that nothing else is using the COM port.',
               mtError, [mbok], 0);
    frmStatus.Close;
    DataType := dtNone;
    { To be extra certain, reset the the protocol type }
    PanelComms.ProtocolType := ppNone;
  end;

  { Update the progress counter }
  if not (PanelComms.HandshakeReceived or PanelComms.CommsFault) then begin
    case PanelComms.ProtocolType of
      ppNew:
        begin
          frmStatus.Progress.Position := frmStatus.Progress.Position + 1;
          if frmStatus.Progress.Position = frmStatus.Progress.Max then begin
            frmStatus.Progress.Max := 101;
            frmStatus.Progress.Position := 0;
          end;
        end;

      ppOld:
        begin
          { Increment the progress bar }
          frmStatus.Progress.Position := frmStatus.Progress.Position + 1;
          if frmStatus.Progress.Position = frmStatus.Progress.Max then begin
            frmStatus.Progress.Max := 130;
            frmStatus.Progress.Position := 0;
          end;
        end;
      ppOldInverted:
        begin
          { Increment the progress bar }
          frmStatus.Progress.Position := frmStatus.Progress.Position + 1;
          { If the position reaches its maximum position, then signal a comms fault }
          if frmStatus.Progress.Position = frmStatus.Progress.Max then begin
            PanelComms.CommsFault := true;
          end;
        end;
    end;
  end;
end;

procedure TMainForm.PanelCommsPacketReceived(Sender: TObject;
  CmdChar: Char; Packet: String);
begin
  if PanelComms.Cancel then begin
    { If the cancel button has been pressed, hide the status form and abort }
    frmStatus.Close;
    DataType := dtNone;
    Exit;
  end;
  { If packet is received, and it matches the handshake packet, signal that
  handshaking is successful }
  if (Packet = HandshakePacket) and not PanelComms.HandshakeReceived then begin
    PanelComms.HandshakeReceived := true;
    frmStatus.StatusText := 'Communications established with XFP';
  end;

  if PanelComms.HandshakeReceived then begin
    { Determine what has loaded by the command received }
    case CmdChar of
      RetDownload: AllowDownloads (Packet);
      RxRetMainVersion: LoadMainVersion (Packet);
      RxRetLoopType: LoadLoopType (Packet);
      RxRetPointName: LoadPointName (Packet);
      RxRetPData: LoadPointData (Packet);
      RxRetZGroup: LoadZGroup (Packet);
      RxRetZoneSet: LoadZoneSet (Packet);
      RxRetRepeaterName: LoadRepeaterName (Packet);
      RxRetRepeaterFitted: LoadRepeaterFitted (Packet);
      RxRetOutputName: LoadOutputName (Packet);
      RxRetOutputFitted: LoadOutputFitted (Packet);
      RxRet_NetPanelData: LoadNetworkData (Packet);
      RxRetSegNo: LoadSegNo (Packet);
      RxRetPanelName: LoadPanelName (Packet);
      RxRetMaintName: LoadMaintName (Packet);
      RxRetMaintDate: LoadMaintDate (Packet);
      RxRetAL2Code: LoadAL2Code (Packet);
      RxRetAL3Code: LoadAL3Code (Packet);
      RxRetFaultLockOutTime: LoadFaultLockoutTime (Packet);
      RxRetPhasedSettings: LoadPhasedSettings (Packet);
      RxRetZoneName: LoadZoneName (Packet);
      RxRetZoneTimers: LoadZoneTimers (Packet);
      RxRetZoneNonFire: LoadZoneNonFire (Packet);
      RxRetQuiesName: LoadQuiesName (Packet);
      RxRetDayNight: LoadDayNightSettings (Packet);
      RxRetEvent: LoadEventHistory(Packet);
      RxRetC_EEvent: LoadC_EData(Packet);
      RxRet_FlashData : LoadFlashData(Packet);

    else
      { Otherwise retry to obtain data }
      { DSM - IMHO this is the last thing you want to do here -
        it just makes matters worse. Better to wait for timeout
        before retrying
      }
      //if (FDataType <> dtNone) then DataRetry := DataRetry + 1;
    end;
  end;
end;

procedure TMainForm.PanelCommsTimeOut(Sender: TObject);
begin
  DataRetry := DataRetry + 1;
end;

procedure TMainForm.btnLoadFromPanelClick(Sender: TObject);
begin
  { Determine which is the active page, then load the relevant information }
  case  ConfigPages.ActivePageIndex of
    0: begin
      { Do not load all the data }
      FLoadAll := false;
      { Determine which loop is to be loaded then load the information for that loop }
      if radgrpLoop.ItemIndex = 0 then begin
        { Do not load all the data }
        LoadAll := false;
        FLoadLoopType := true;
        DataType := dtLoadLoop1;
        EstablishComms;
      end
      else if radgrpLoop.ItemIndex = 1 then begin
        { Do not load all the data }
        LoadAll := false;
        FLoadLoopType := true;
        DataType := dtLoadLoop2;
        EstablishComms;
      end;
    end;

    1: begin
      { Do not load all the data }
      LoadAll := false;
      FLoadLoopType := true;
      DataType := dtLoadLoop1;
      EstablishComms;
    end;

    2: begin
      { Do not load all the data }
      LoadAll := false;
      FLoadLoopType := true;
      DataType := dtLoadLoop2;
      EstablishComms;
    end;

    3: begin
      { Do not load all the data }
      LoadAll := false;
      DataType := dtLoadZoneList;
      EstablishComms;
    end;

    4: begin
      { Do not load all the data }
      FLoadAll := false;
      DataType := dtLoadGroupList;
      EstablishComms;
    end;

    5: begin
      { Do not load all the data }
      LoadAll := false;
      DataType := dtLoadSetList;
      EstablishComms;
    end;

    6: begin
      { Check to see if the code is OK on the site config page }
      ConfigPages.SetFocus;
      if not FCodeOK then Exit;
      { Do not load all the data }
      LoadAll := false;
      DataType := dtLoadSite;
      EstablishComms;
    end;

    7: begin         //Cause & Effect table
      { Do not load all the data }
      LoadAll := false;
      DataType := dtLoadCE_Event;
      EstablishComms;
    end;

    8: begin        //network page data
      { Do not load all the data }
      LoadAll := false;
      DataType := dtLoadRepeaterList;
      EstablishComms;
    end;

    9: begin
      { Do not load all the data }
      Event_Clear := MessageDlg ('Do you want to clear the events as we go?',
                   mtWarning, [mbYes, mbNo],0);
      LoadAll := false;
      DataType := dtLoadEventHistory;
      PanelComms.IsASCIIOnly := FALSE;
      EstablishComms;
    end;

  end; { case Config page iIndex }
end;

procedure TMainForm.PanelCommsInvalidChecksum(Sender: TObject);
begin
  DataRetry := DataRetry + 1;
end;

procedure TMainForm.btnLoadLoop1Click(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  FLoadLoopType := true;
  DataType := dtLoadLoop1;
  EstablishComms;
end;

procedure TMainForm.btnLoadLoop2Click(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  FLoadLoopType := true;
  DataType := dtLoadLoop2;
  EstablishComms;
end;

procedure TMainForm.SelectCommPort1Click(Sender: TObject);
var
  iPort : integer;
  iOpen : boolean;
begin
  //CommSelectForm.ShowModal;
  iPort := PanelComms.Port;
  iOpen := PanelComms.IsOpen;
  if iOpen then
  begin
    PanelComms.CloseComms;
  end;
  if DlgSelCommPort.Execute( iPort ) then
  begin
    PanelComms.Port := iPort;
    PgmData.SigRegistryXFP.WriteInteger( 'Port' , iPort );
  end;
  if iOpen then
  begin
    PanelComms.OpenComms;
  end;
end;

procedure TMainForm.PanelCommsHandshakeReceived(Sender: TObject);
begin
  { Check to see if the NVM link has been fitted }
  if CheckNVM then begin
    FCheckNVM := false;
    FDataRetry := 0;
    RecordSent := ReqDownload + #0;
  end
  else begin
    { Otherwise, received handshake, so obtain the version numbers }
    frmStatus.StatusText := 'Loading version information';
    RecordSent := TxReqMainVersion + #0;
  end;
end;

procedure TMainForm.btnLoadZoneConfigClick(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  DataType := dtLoadZoneList;
  EstablishComms;
end;

procedure TMainForm.btnNewClick(Sender: TObject);
begin
  SiteFile.New;
end;

procedure TMainForm.btnLoadSetConfigClick(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  DataType := dtLoadSetList;
  EstablishComms;
end;

procedure TMainForm.btnLoadSiteConfigClick(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  DataType := dtLoadSite;
  EstablishComms;
end;

procedure TMainForm.btnLoadNetworkConfigClick(Sender: TObject);
begin
  { Do not load all the data }
  LoadAll := false;
  DataType := dtLoadRepeaterList;
  EstablishComms;
end;

procedure TMainForm.PanelCommsAck(Sender: TObject);
begin
  { Determine which data has successfully been saved }
  if not PanelComms.Cancel then begin
    case FDataType of
      { Device name for loop 1 }
      dtSavePName1: SavePointName;
      { Device data for loop 1 }
      dtSavePData1: SavePointData (1);
      { Device data for loop 2 }
      dtSavePData2: SavePointData (2);
      { Zone names }
      dtSaveZoneName: SaveZName;
      { Zone timers }
      dtSaveZoneTimer: SaveZTimer;
      { Zone Non Fire }
      dtSaveZoneNonFire: SaveZNonFire;
      { Group data for zones }
      dtSaveZGroup: SaveZGroup;
      { Output set data for zones }
      dtSaveZoneSet: SaveZoneSet;
      { Maintenance string }
      dtSaveMaintName: SaveMaintText;
      { Maintenance date }
      dtSaveMaintDate: SaveMaintDate;
      { AL2 code }
      dtSaveAL2Code: SaveAL2Code;
      { AL3 code }
      dtSaveAL3Code: SaveAL3Code;
      { Fault lock out time }
      dtSaveFaultLockoutTime: SaveFaultLockoutTime;
      { Copy PC Time }
      dtSavePCTime: SavePCTime;
      { Phased settings }
      dtSavePhasedSettings: SavePhasedSettings;
      { Repeater names }

      dtSaveRepeaterName: SaveRepeaterNames;
      { Repeater fitted }
      dtSaveRepeaterFitted: SaveRepeaterFitted;
      { Zonal output name }
      dtSaveOutputName: SaveOutputName;
      { Zonal output fitted }
      dtSaveOutputFitted: SaveOutputFitted;
      { Segment number }
      dtSaveNetworkData: SaveNetworkData;

      dtSaveSegmentNumber: SaveSegmentNumber;
      { Panel name }
      dtSavePanelName: SavePanelName;

      dtSaveQuiesName: SaveQuiescentName;

      dtSaveDayNight: SaveDayNight;

      dtSaveCE_Event: SaveC_EData;

      dtSaveFlashMemory: SaveText;

    end; { Case FDataType }
  end
  else begin
    frmStatus.Close;
    DataType := dtNone;
  end;
end;

procedure TMainForm.btnSaveSiteConfigClick(Sender: TObject);
begin
  { Check to see if the code is OK on the site config page }
  ConfigPages.SetFocus;
  if not FCodeOK then Exit;
  SaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveMaintName;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSavetoPanelClick(Sender: TObject);
Var
iIndex : Integer;
Proceed : boolean;
begin

  Proceed := TRUE;

  if Proceed = TRUE  then
  begin
    { Determinee the relevant page }
    case ConfigPages.ActivePageIndex of
      0:
      begin
        if radgrpLoop.ItemIndex = 0 then
        begin
          btnSaveLoop1Click (Self);
        end
        else
         begin
          btnSaveLoop2Click (Self);
        end;
      end;
      1:
      begin
        btnSaveLoop1Click (Self);
      end;
      2:
      begin
        btnSaveLoop2Click (Self);
      end;
      3:
      begin
        for iIndex := 1 to MAX_ZONE do
        begin
          if (SiteFile.ZoneList.Zone[iIndex].Dependancy[1] = 5) or
                  (SiteFile.ZoneList.Zone[iIndex].Dependancy[4] = 5) then
          begin
            if SiteFile.InvestigationPeriod + SiteFile.InvestigationPeriod1 +
                     SiteFile.ZoneList.Zone[iIndex].SounderDelay > Max_OutputDelay then
            begin
              MessageDlg ('There are output delays exceeding 10 Minutes which MUST be resolved', mtError, [mbOk], 0);
              Proceed := FALSE;
            end;
          end;
        end;
        if Proceed = TRUE then
        begin
          btnSaveZoneConfigClick (Self);
        end;
      end;
      4:
      begin
        btnSaveGroupConfigClick (Self);
      end;
      5:
      begin
        btnSaveSetConfigClick (Self);
      end;
      6:
      begin
        btnSaveSiteConfigClick (Self);
      end;
      7:
      begin
        btnSaveCE_DataClick (Self);
      end;
      8:
      begin
        btnSaveNetworkConfigClick (Self);
      end;
    end;
  end;
end;

procedure TMainForm.btnSaveLoop1Click(Sender: TObject);
begin
  FLoadLoopType := TRUE;

  DataType := dtSavePData1;
  SaveAll := false;
  FSaveLoopType := true;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSaveLoop2Click(Sender: TObject);
begin
  FLoadLoopType := TRUE;

  DataType := dtSavePData2;
  SaveAll := false;
  FSaveLoopType := true;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSaveZoneConfigClick(Sender: TObject);
begin
  SaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveZoneTimer;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSaveSetConfigClick(Sender: TObject);
begin
  SaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveZoneSet;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSaveCE_DataClick(Sender: TObject);
begin
  SaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveCE_Event;
  CheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.btnSaveClick(Sender: TObject);
begin
  SiteFile.Save();
end;

procedure TMainForm.btnSaveNetworkConfigClick(Sender: TObject);
begin
  SaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveRepeaterFitted;
  CheckNVM := true;
  EstablishComms;
end;

function TMainForm.Save_Integer(In_Value : integer): string;
begin
     result := chr(In_Value and 255);
     result := result + chr(In_Value div 256);
end;


procedure TMainForm.SaveLoopType;
var
  sTemp: string;

begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Save the loop type }
  sTemp := TxSetLoopType + #1;
  case frmLoop1.AFPLoop.SystemType of
    stApolloLoop: sTemp := sTemp + #0;
    stHochikiLoop: sTemp := sTemp + #1;
    stSysSensorLoop: sTemp := sTemp + #2;
    stNittanLoop: sTemp := sTemp + #3;
    stCASTLoop: sTemp := sTemp + #4;
  end;
  { Since we are about to save the loop type, it should automatically go on to load
  the relevant loop type }
  FSaveLoopType := false;
  FDataCount := 0;
  RecordSent := sTemp;
end;



{Site data downloading. Procedures appear in the order of downloading}
procedure TMainForm.SaveMaintText;
var
  sTemp: string;
  Year, Month, Day: word;
begin
  DataType := dtSaveMaintDate;

  if SendToPanel.Checked = TRUE then
  begin
    try
      { Try to decode the date. If it fails, bomb out }
      DecodeDate (SiteFile.MaintenanceDate, Year, Month, Day);
      { Construct the string to send to the panel }
      sTemp := BuildXFerString( TxSetMaintDate, chr (Year - Base_Year) + chr (Month) + chr (Day)+
                              chr(0) + chr(0) + chr(0) );

      { calculate the length field }
      //sTemp[2] := chr(Length(sTemp)-2);
      RecordSent := sTemp;
    except
      { If there is an error, typically an invalid date, move to the next item }
      SaveMaintDate;
    end;
  end else
  SaveMaintDate;
end;

procedure TMainForm.SaveMaintDate;
var
  sTemp: string;
  Code : integer;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  DataType := dtSaveAL2Code;

  Code := StrToInt(txtLvl2.Text);
  sTemp := {TxSetAL2Code + #0 +} Save_Integer(Code);

  { calculate the length field }
  //sTemp[2] := chr(Length(sTemp)-2);
  RecordSent := BuildXFerString( TxSetAL2Code, sTemp );
end;

procedure TMainForm.SaveAL2Code;
var
  sTemp: string;
  Code : integer;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  DataType := dtSaveAL3Code;
  Code := StrToInt(txtLvl3.Text);

  frmStatus.StatusText := 'Saving Password data';
  //sTemp := TxSetAL3Code + #0 + Save_Integer(Code);
  sTemp := Save_Integer(Code);

  if SiteFile.Discovery_Polling_LED = true then
    sTemp := sTemp + chr(1)
  else
    sTemp := sTemp + chr(0);

    sTemp := sTemp + chr(SiteFile.MCP_Delay - 1);
    sTemp := sTemp + chr(SiteFile.Detector_Delay - 1);
    sTemp := sTemp + chr(SiteFile.IO_Delay - 1);

  { calculate the length field }
  //sTemp[2] := chr(Length(sTemp)-2);
  RecordSent := BuildXFerString( TxSetAL3Code, sTemp );
end;

procedure TMainForm.SaveAL3Code;
var
  sTemp: string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  DataType := dtSaveFaultLockoutTime;
  //sTemp := TxSetFaultLockoutTime + #0 + Chr (SiteFile.FaultLockout);
  sTemp := Chr (SiteFile.FaultLockout);

  { calculate the length field }
  //sTemp[2] := chr(Length(sTemp)-2);
  RecordSent := BuildXFerString( TxSetFaultLockoutTime, sTemp );
end;

procedure TMainForm.SaveFaultLockoutTime;
var
  sTemp: string;
  Year, Month, Day, Hour, Min, Sec, MSec: Word;
begin
  { Check to see if the PC time is to be copied to the panel }
  if UpLoadTimeSelect.Checked then begin
    { Reset the number of retries }
    FDataRetry := 0;
    DataType := dtSavePCTime;
    { Acquire current PC time }
    DecodeDate (Now, Year, Month, Day);
    DecodeTime (Now, Hour, Min, Sec, MSec);
    //sTemp := TxSetTimeDate + #0 +
    //  chr (Year - Base_Year) + chr (Month) + chr (Day) +
    //  chr (Hour) + chr (Min)   + chr (Sec);
    sTemp := chr (Year - Base_Year) + chr (Month) + chr (Day) +
      chr (Hour) + chr (Min)   + chr (Sec);

    { calculate the length field }
    //sTemp[2] := chr(Length(sTemp)-2);
    { Send PC time to panel }
    RecordSent := BuildXFerString( TxSetTimeDate, sTemp);
  end
  { otherwise save the phased settings }
  else SavePCTime;
end;

procedure TMainForm.SavePCTime;
var
  sTemp: string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  DataType := dtSavePhasedSettings;

  //sTemp := TxSetPhasedSettings + #0 + Save_Integer(SiteFile.PhasedDelay) +
  //                                    Save_Integer(SiteFile.InvestigationPeriod) +
  //                                    Save_Integer(SiteFile.InvestigationPeriod1);
  sTemp := Save_Integer(SiteFile.PhasedDelay) + Save_Integer(SiteFile.InvestigationPeriod) +
                                      Save_Integer(SiteFile.InvestigationPeriod1);

  { calculate the length field }
  //sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXFerString( TxSetPhasedSettings, sTemp );
end;

procedure TMainForm.SavePhasedSettings;
//var
//   sTemp : string;
begin
  FDataRetry := 0;
  DataType := dtSaveQuiesName;

  frmStatus.StatusText := 'Saving Quiescent String';
  //STemp := TxSetQuiesName + #0 +  txtQuiesString.Text + #0;

  { calculate the length field }
  //sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXFerString( TxSetQuiesName, txtQuiesString.Text + #0 );
end;

procedure TMainForm.SaveQuiescentName;
var
   sTemp : string;
   NHour, NMin, DHour, DMin, RHour, RMin : integer;
   P : integer;

begin
  { Reset the number of retries }
  FDataRetry := 0;
  DataType := dtSaveDayNight;

  P := Pos( ':',SiteFile.NightEnd);
  DHour := StrToInt (Copy(SiteFile.NightEnd,1,P-1));
  DMin := StrToInt (Copy(SiteFile.NightEnd,P+1,2));

  P := Pos( ':',SiteFile.NightBegin);
  NHour := StrToInt (Copy(SiteFile.NightBegin,1,P-1));
  NMin := StrToInt (Copy(SiteFile.NightBegin,P+1,2));

  P := Pos( ':',SiteFile.ReCalTime);
  RHour := StrToInt (Copy(SiteFile.ReCalTime,1,P-1));
  RMin := StrToInt (Copy(SiteFile.ReCalTime,P+1,2));

  sTemp := {TxSetDayNight + #0 +}
  chr (DMin) + chr (DHour) + chr (NMin) + chr (NHour) + chr (RHour) + chr(RMin);

  sTemp := sTemp + chr(SiteFile.Day_Enable_Flags);
  sTemp := sTemp + chr(SiteFile.Night_Enable_Flags);

  sTemp := sTemp + iff( SiteFile.BST_Adjustment, 1, 0 )
           + iff( SiteFile.RealTime_Event_Output, 1, 0 );

  FDataCount := 0;

  { calculate the length field }
  //sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXFerString( TxSetDayNight, sTemp );
end;

procedure TMainForm.SaveDayNight;
begin
  { If save all flag has been set, go to network config }
  if FSaveAll and not PanelComms.Cancel then begin
    DataType := dtSaveRepeaterFitted;
    LoadInfo;
  end
  else begin
    frmStatus.Close;
    DataType := dtNone;
  end;
end;


procedure TMainForm.SaveRepeaterFitted;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Check if there are more repeaters to process }
  if FDataCount < MAX_REPEATER then begin
    { Go to next repeater }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Saving Network data for Panel ' + IntToStr (FDataCount);
    DataType := dtSaveRepeaterName;
    { Save repeater name }
    RecordSent := TxSetRepeaterName + chr (Length (SiteFile.RepeaterList.Repeater[FDataCount].Name) + 2) +
        chr (FDataCount) + SiteFile.RepeaterList.Repeater[FDataCount].Name + #0;
  end
  else begin
    { Save the segment number }
    FDataCount := 0;
    FDataRetry := 0;
    DataType := dtSaveNetworkData;
    RecordSent := TxSetSegNo + chr(1) + chr (SiteFile.RepeaterList.Segment);
  end;
end;

procedure TMainForm.SaveRepeaterNames;
var
  sTemp: string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Zonal output fitted settings have been saved - now save repeater fitted settings }
  DataType := dtSaveRepeaterFitted;
  sTemp := {TxSetRepeaterFitted + #2 + }chr (FDataCount);
  if SiteFile.RepeaterList.Repeater[FDataCount].Fitted then sTemp := sTemp + #255
  else sTemp := sTemp + #0;

  { calculate the length field }
  //sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXFerString( TxSetRepeaterFitted, sTemp );
end;

procedure TMainForm.SaveOutputName;
var
  sTemp: string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Zonal output names have been saved - now save zonal output fitted settings }
  DataType := dtSaveOutputFitted;
  sTemp := {TxSetOutputFitted + #2 +} chr (FDataCount);
  if SiteFile.RepeaterList.Output[FDataCount].Fitted then sTemp := sTemp + #255
  else sTemp := sTemp + #0;

  { calculate the length field }
  //sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXFerString( TxSetOutputFitted, sTemp );
end;

procedure TMainForm.SaveOutputFitted;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Check if there are more outputs to process }
  if FDataCount < MAX_ZONAL_OUTPUT then begin
    { Go to next output }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Saving data for zonal output ' + IntToStr (FDataCount);
    DataType := dtSaveOutputName;
    { Save output name }
    RecordSent := TxSetOutputName + chr (Length (SiteFile.RepeaterList.Output[FDataCount].Name) + 2) +
        chr (FDataCount) + SiteFile.RepeaterList.Output[FDataCount].Name + #0;
  end
  else begin
    { Save the segment number }
    DataType := dtSaveNetworkData;
    SaveSegmentNumber;
  end;
end;

procedure TMainForm.SaveNetworkData;
var
  sTemp: string;
  PanelNo : integer;
  d : integer;
begin

//construct the string to send to the panel
  sTemp := ''; //TxSet_NetPanelData + #0;

  for PanelNo := 1 to MAX_PANELS do
  begin
    d := 0;
    with SiteFile.XFP_Network do
    begin
      if SiteFile.RepeaterList.Repeater[PanelNo].Fitted then d := d or 1;
      if PAccept_Faults[PanelNo]                        then d := d or 2;
      if PAccept_Alarms[PanelNo]                        then d := d or 4;
      if PAccept_Controls[PanelNo]                      then d := d or 8;
      if PAccept_Disablements[PanelNo]                  then d := d or 16;
      if PAccept_Occupied[PanelNo]                      then d := d or 32;
    end;

    sTemp := sTemp + chr(d);
  end;

  RecordSent := BuildXFerString( TxSet_NetPanelData, sTemp );

  DataType := dtSaveSegmentNumber;
  LoadInfo;
end;

procedure TMainForm.SaveSegmentNumber;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Segment number saved - save panel name }
  DataType := dtSavePanelName;
  RecordSent := TxSetPanelName + chr (Length (PanelNameEdit.Text)) + PanelNameEdit.Text;
end;


procedure TMainForm.SavePanelName;
begin

  if FSaveAll and not PanelComms.Cancel then begin
    DataType := dtSaveCE_Event;
    LoadInfo;
  end
  else begin
    frmStatus.Close;
    DataType := dtNone;
  end;

end;



procedure TMainForm.SavePointData(Loop: integer);
var
  sTemp: string;
  iDevType: integer;
  frmLoop : TfrmLoop;
  i : integer;
begin
  { Check to see if it is the last point }
  if FDataCount < frmLoop1.AFPLoop.NoLoopDevices then begin
    { If first entry, then determine the max value of the progress bar }
    if FDataCount = 0 then frmStatus.Progress.Max := frmLoop1.AFPLoop.NoLoopDevices;
    { Reset the number of retries }
    FDataRetry := 0;
    { Increment the point number }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    if (frmLoop1.AFPLoop.SystemType = stSysSensorLoop) then begin
      if FDataCount <= HALF_MAX_POINTS_SYS_SENSOR then begin
        { Construct string for saving point name }
        frmStatus.StatusText := 'Saving data for Loop ' + IntToStr (Loop) + ' - Point S' +
          IntToStr (FDataCount);
        //iIndex := FDataCount;
      end
      else begin
        { Construct string for saving point name }
        frmStatus.StatusText := 'Saving data for Loop ' + IntToStr (Loop) + ' - Point M' +
          IntToStr (FDataCount - HALF_MAX_POINTS_SYS_SENSOR);
        //iIndex := FDataCount + 1;
      end;
    end
    else begin
      { Construct string for saving point name }
      frmStatus.StatusText := 'Saving data for Loop ' + IntToStr (Loop) + ' - Point ' +
        IntToStr (FDataCount);
      //iIndex := FDataCount;
    end;

//now construct the actual Point Data string to send to the panel
     sTemp := ''; //TxSetPData + #12;

    case Loop of
      1:
      begin
        frmLoop := frmLoop1;
         DataType := dtSavePData1;
      end;
      2:
      begin
        if NoOfLoops = 1 then
        begin
          frmStatus.Close;
          DataType := dtNone;
          UpdateConfigData;
          exit;
        end;
        frmLoop := frmLoop2;
        DataType := dtSavePData2;
      end;
      else
      begin
        frmLoop := nil;
      end;
    end;

    { If loop is of type system sensor, and the count is in the module section,
    remember that the panel expects the number to be one higher than it actually is }
    if (frmLoop.AFPLoop.SystemType = stSysSensorLoop) and (FDataCount > HALF_MAX_POINTS_SYS_SENSOR) then begin
      sTemp := sTemp + chr(FDataCount + 1);
    end
    else sTemp := sTemp + chr (FDataCount);

    sTemp := sTemp + chr (Loop);

    iDevType := frmLoop.AFPLoop.Device[FDataCount].DevType;
    sTemp := sTemp + chr (iDevType);

    if frmLoop.AFPLoop.Has_SubAddresses(FDataCount) = TRUE then
      i := frmLoop.AFPLoop.Device [FDataCount].SharedData[1]
    else
      i := frmLoop.AFPLoop.Device [FDataCount].Zone;

    {ensure the Group flag is set for sounders & sounder I/O devices}
    if frmLoop.AFPLoop.Classify_Device(FDataCount) = Sounder then
      i := i or 128;
    if frmLoop.AFPLoop.Classify_Device(FDataCount) = Sounder_I_O then
      i := i or 128;

    sTemp := sTemp + chr (i);

    {save the SharedData[] array. Note that Zone is held at CmdData[6], and is stored in the
     Zone property of loop data as well as SharedData[1]}
    for i := 2 to 8 do
      sTemp := sTemp + chr(frmLoop.AFPLoop.Device [FDataCount].SharedData[i]);

    { Now send constructed string }
    { calculate the length field }
    //sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXFerString( TxSetPData, sTemp );

     frmLoop.AFPLoop.Device [FDataCount].TypeChanged := FALSE;
  end
//past the end of the data, so move on to the next bit of data to send
  else begin
    if FSaveAll then
    begin
      if (Loop = 1) and (Loop < NoOfLoops) then
      begin
         DataType := dtSavePData2;
      end else
      begin
        DataType := dtSavePName1;
      end;
    end else
    begin
      DataType := dtSavePName1;
    end;

    LoadInfo;
  end;
end;

procedure TMainForm.SavePointName;
var
  sTemp: string;

begin
  { Reset the number of retries }
  FDataRetry := 0;

  if FDataCount <= frmDeviceName.ComboBox1.Items.Count then begin
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Saving Point Description ' + IntToStr (FDataCount);

    sTemp := {TxSetPointName + #0 +} chr(FDataCount and 255) + chr(FDataCount div 256);

    sTemp := sTemp + frmDeviceName.ComboBox1.Items.Strings[FDataCount] + #0;

    { Now send constructed string }
    { calculate the length field }
    // sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXFerString( TxSetPointName, sTemp );
    inc(FDataCount);
  end
  else
  begin
    if FDataCount < 999 then
    begin
      FDataCount := 999;
       sTemp := {TxSetPointName + #0 +} chr(FDataCount and 255) + chr(FDataCount div 256);

      sTemp := sTemp + '' + #0;

      { Now send constructed string }
      { calculate the length field }
      // sTemp[2] := chr(Length(sTemp)-2);
      RecordSent := BuildXferString( TxSetPointName, sTemp );
    end else
    begin
      { If the save all flag has been set, go to next section }
      if FSaveAll then begin
      { Loop 1 goes to loop 2 (or Zone Data on 1 loop panel), loop2 goes to zone data }
        case Loop of
            1: begin
                 if (Loop < NoOfLoops) then
                   DataType := dtSavePData2
                 else
                   DataType := dtSaveZoneTimer;
               end;
                 
            2: DataType := dtSaveZoneTimer;
        end;
        LoadInfo;
      end
      { if on the loop summary page, and the Save All button has not been pressed,
      then save the zone names }
      else if ConfigPages.ActivePageIndex = 0 then begin
        { Now save the zone names }
        btnSaveZoneConfigClick(Self);
      end
      else begin
        frmStatus.Close;
        DataType := dtNone;
        UpdateConfigData;
      end;
    end;
  end;
end;







function TMainForm.NormaliseDependancy_Out(Inp: integer): char;
begin
     result := 'N';

     if Inp = 1 then result := 'A';
     if Inp = 2 then result := 'B';
     if Inp = 3 then result := 'C';
     if Inp = 4 then result := 'N';
     if Inp = 5 then result := 'I';
     if Inp = 6 then result := 'D';
end;

procedure TMainForm.SaveZGroup;
var
  sTemp: string;
  iIndex: integer;
begin

  { Check to see if this is the last zone }
  if FDataCount < MAX_ZONE then begin
    { Reset the number of retries }
    FDataRetry := 0;
    { If not, increment the zone number }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Saving group data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Saving group data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    { Construct the string to send to panel }
    sTemp := {TxSetZGroup + chr (MAX_GROUP + 1) +} chr (FDataCount);

    for iIndex := 1 to MAX_GROUP do
      sTemp := sTemp + chr(SiteFile.ZoneList.Zone[FDataCount].Group[iIndex]);

    { Append the panel sounder groups }
    sTemp := sTemp + chr(SiteFile.PanelSounder1_Group);
    sTemp := sTemp + chr(SiteFile.PanelSounder2_Group);

    { Append the Hochiki/Apollo/CAST tone types}
    if frmLoop1.AFPLoop.SystemType = stHochikiLoop then
    begin
      sTemp := sTemp + chr(Int_Tone_Box.ItemIndex + 1);
      sTemp := sTemp + chr(Cont_Tone_Box.ItemIndex + 1);
    end else if frmLoop1.AFPLoop.SystemType = stApolloLoop then
    begin
      sTemp := sTemp + chr(Int_Tone_Box_Apollo.ItemIndex + 1);
      sTemp := sTemp + chr(Cont_Tone_Box_Apollo.ItemIndex + 1);
    end else
    begin
      sTemp := sTemp + chr(Int_Tone_Box_CAST.ItemIndex + 1);
      sTemp := sTemp + chr(Cont_Tone_Box_CAST.ItemIndex + 1);
    end;

    sTemp := sTemp + iff( SiteFile.ReSound_Function, 1, 0 );

    { calculate the length field }
    // sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXferString( TxSetZGroup, sTemp );
  end
  else begin
    { If save all flag has been set, go to site config }
    if FSaveAll then begin
      DataType := dtSaveZoneSet;
      LoadInfo;
    end
    else begin
//      frmStatus.Close;
//      DataType := dtNone;
SavePCTime;
    end;
  end;
end;

procedure TMainForm.SaveZoneSet;
var
  sTemp: string;
  iIndex: integer;
begin

  { Check to see if this is the last zone }
  if FDataCount <= MAX_ZONE then begin
    { Reset the number of retries }
    FDataRetry := 0;
    { If not, increment the zone number }
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Saving output set data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Saving output set data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    { Construct the string to send to panel }
    sTemp := {TxSetZoneSet + #0 +} chr (FDataCount);

    sTemp := sTemp + chr(SiteFile.ZoneList.Zone[FDataCount].ZoneSet[RELAY1]);
    sTemp := sTemp + chr(SiteFile.ZoneList.Zone[FDataCount].ZoneSet[RELAY2]);

    for iIndex := 1 to MAX_ZONE_SET-2 do
      sTemp := sTemp + chr(SiteFile.ZoneList.Zone[FDataCount].ZoneSet[iIndex]);

    sTemp := sTemp + Save_Integer(btnDelayTime.Value);

    { calculate the length field }
    // sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXferString( TxSetZoneSet, sTemp );

    inc (FDataCount);
  end
  else begin
    { If save all flag has been set, go to site config }
    if FSaveAll then begin
      DataType := dtSaveMaintName;
      LoadInfo;
    end
    else begin
      frmStatus.Close;
      DataType := dtNone;
    end;
  end;
end;

procedure TMainForm.SaveZName;
var
  iValue: integer;
  sTemp: string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Acquire zone timers for zone }
  DataType := dtSaveZoneTimer;
  { Construct string to send }

  sTemp := {TxSetZoneTimers + #0 +} chr (FDataCount);

  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].SounderDelay);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Relay1Delay);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Relay2Delay);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].RemoteDelay);

  sTemp := sTemp + NormaliseDependancy_Out(SiteFile.ZoneList.Zone[FDataCount].Dependancy[1]);
  sTemp := sTemp + NormaliseDependancy_Out(SiteFile.ZoneList.Zone[FDataCount].Dependancy[4]);

  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Dependancy[3]);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Dependancy[2]);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Dependancy[6]);
  sTemp := sTemp + Save_Integer(SiteFile.ZoneList.Zone[FDataCount].Dependancy[5]);

  iValue := 0;
  if SiteFile.ZoneList.Zone[FDataCount].Detector = TRUE then
    iValue := iValue + 1;

  if SiteFile.ZoneList.Zone[FDataCount].MCP = TRUE then
    iValue := iValue + 2;

  if SiteFile.ZoneList.Zone[FDataCount].EndDelays = TRUE then
    iValue := iValue + 4;

  sTemp := sTemp + chr(iValue);

  { calculate the length field }
  // sTemp[2] := chr(Length (sTemp)-2);
  RecordSent := BuildXferString( TxSetZoneTimers, sTemp );
end;

procedure TMainForm.SaveZNonFire;
begin
  { Before anything else, change the time out value back to the default }
  PanelComms.InputTimeOut := 1000;
  if FSaveAll then begin
    DataType := dtSaveZGroup;
    LoadInfo;
  end
  else begin
  //    DataType := dtNone;
//    frmStatus.Close;

SavePCTime;

  end;
end;

procedure TMainForm.SaveZTimer;
begin
  { Check to see if this is the last zone }
  if FDataCount < MAX_ZONE then begin
    { Reset the number of retries }
    FDataRetry := 0;
    { If not, go to next zone, and save zone name }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Saving data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Saving data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    { If not on the loop summary page, or the Save All button has been pressed,
    then change the data type }
    if (ConfigPages.ActivePageIndex <> 0) or SaveAll then
      DataType := dtSaveZoneName;
    RecordSent := TxSetZoneName + chr (Length (SiteFile.Zonelist.Zone[FDataCount].Name) + 2) +
        chr (FDataCount) + SiteFile.ZoneList.Zone[FDataCount].Name + #0;
  end
  else begin
    if (ConfigPages.ActivePageIndex <> 0) or SaveAll then begin
      { Only save the zone non fire info if the panel version >= 8 }
      DataType := dtSaveZoneNonFire;
      LoadInfo;
    end
    else begin
//      DataType := dtNone;
//      frmStatus.Close;

      DataType := dtSaveAL2Code;
      LoadInfo;

    end;
  end;
end;


procedure TMainForm.SaveC_EData;
var
  sTemp, S: string;
  P : integer;
  Hour, Min : integer;
begin
  { Check to see if this is the last zone }
  if FDataCount < PgmData.MAX_CE_EVENTS then begin
    { Reset the number of retries }
    FDataRetry := 0;
    { If not, go to next zone, and save zone name }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Saving data for Event ' + IntToStr (FDataCount);
    { If not on the loop summary page, or the Save All button has been pressed,
    then change the data type }

    sTemp := {TxSetC_EEvent + #0 +} chr(FDataCount);

    sTemp := sTemp + chr(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[1]);
    sTemp := sTemp + Save_Integer(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[2]);
    sTemp := sTemp + chr(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[3]);
    sTemp := sTemp + Save_Integer(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[4]);
    sTemp := sTemp + chr(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[5]);
    sTemp := sTemp + Save_Integer(SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[6]);

    if(FDataCount <= MAX_CE_TIMER_EVENTS) then begin
      { Append time event marker to the end }
      S := SiteFile.C_ETable.Time_Events[FDataCount];
      P := AnsiPos( ':',S);
      Hour := StrToInt (Copy(S,1,P-1));
      Min  := StrToInt (Copy(S,P+1,2));
      sTemp := sTemp + chr(Min) + chr(Hour);
    end;

    { calculate the length field }
    // sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXferString( TxSetC_EEvent, sTemp );
  end
  else begin
      DataType := dtNone;
      frmStatus.Close;
  end;
end;


procedure TMainForm.PanelName1Change(Sender: TObject);
begin
  { Whenever the text is changed, store the changes }
  SiteFile.RepeaterList.Repeater[(Sender as TEdit).Tag].Name := (Sender as TEdit).Text;
  SiteFile.ZoneList.Zone[(Sender as TEdit).Tag + MAX_LOCAL_ZONES].Name := (Sender as TEdit).Text;
  PanelNameEdit.Text := SiteFile.ZoneList.Zone[PanelNumber.Value + MAX_LOCAL_ZONES].Name;

  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.RepFitted1Click(Sender: TObject);
begin
  { Store the fitted status of the repeater whenever it is changed }
  SiteFile.RepeaterList.Repeater[(Sender as TCheckBox).Tag].Fitted :=
    (Sender as TCheckBox).Checked;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.Z1EditChange(Sender: TObject);
begin
  { Whenever the text is changed, store the changes }
  SiteFile.RepeaterList.Output[(Sender as TEdit).Tag].Name := (Sender as TEdit).Text;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.ZOP1FittedClick(Sender: TObject);
begin
  { Store the fitted status of the repeater whenever it is changed }
  SiteFile.RepeaterList.Output[(Sender as TCheckBox).Tag].Fitted :=
    (Sender as TCheckBox).Checked;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.btnSaveAllToPanelClick(Sender: TObject);
var
Proceed : boolean;
iIndex : integer;
begin
  Proceed := TRUE;
  if (MessageDlg ('This will replace ALL data in the Panel. Do you wish to continue?',
    mtConfirmation, [mbYes, mbNo], 0) = mrYes) then
  begin

  for iIndex := 1 to MAX_ZONE
  do begin
    if (SiteFile.ZoneList.Zone[iIndex].Dependancy[1] = 5) or
       (SiteFile.ZoneList.Zone[iIndex].Dependancy[4] = 5) then
    begin
      if SiteFile.InvestigationPeriod + SiteFile.InvestigationPeriod1 +
         SiteFile.ZoneList.Zone[iIndex].SounderDelay > Max_OutputDelay then
       begin
         MessageDlg ('There are output delays exceeding 10 Minutes which MUST be resolved', mtError, [mbOk], 0);
         Proceed := FALSE;
       end;
     end;
  end;


  { Check to see if the code is OK on the site config page }
    ConfigPages.SetFocus;
    if not FCodeOK then Exit;
    if Proceed = FALSE then exit;
    
    SaveAll := true;
    FLoadLoopType := true;
    FSaveLoopType := true;
    DataType := dtSavePData1;
    CheckNVM := true;
    EstablishComms;
    Loop := 1;
  end;
end;

procedure TMainForm.btnSaveAsClick(Sender: TObject);
begin
  SiteFile.SaveAs();
end;

procedure TMainForm.PanelCommsNak(Sender: TObject);
begin
  { Retry sending last record }
  { But not if loading }
  if not PanelComms.Cancel then begin
    case FDataType of
      { Device name for loop 1 }
      dtSavePName1,
      { Device data for loop 1 }
      dtSavePData1,
      { Device data for loop 2 }
      dtSavePData2,
      { Zone names }
      dtSaveZoneName,
      { Zone timers }
      dtSaveZoneTimer,
      { Zone Non Fire }
      dtSaveZoneNonFire,
      { Group data for zones }
      dtSaveZGroup,
      { Output set data for zones }
      dtSaveZoneSet,
      { Maintenance string }
      dtSaveMaintName,
      { Maintenance date }
      dtSaveMaintDate,
      { AL2 code }
      dtSaveAL2Code,
      { AL3 code }
      dtSaveAL3Code,
      { Fault lock out time }
      dtSaveFaultLockoutTime,
      { Copy PC Time }
      dtSavePCTime,
      { Phased settings }
      dtSavePhasedSettings,
      { Repeater names }
      dtSaveRepeaterName,
      { Repeater fitted }
      dtSaveRepeaterFitted,
      { Zonal output name }
      dtSaveOutputName,
      { Zonal output fitted }
      dtSaveOutputFitted,
      { Segment number }
      dtSaveNetworkData,
      dtSaveSegmentNumber,
      { Panel name }
      dtSavePanelName,
      dtSaveQuiesName,
      dtSaveDayNight,
      dtSaveCE_Event,
      dtSaveFlashMemory:
      begin
        DataRetry := DataRetry + 1;
      end;
    end; { Case FDataType }
  end

  //DataRetry := DataRetry + 1;
end;

procedure TMainForm.SetDataRetry(const Value: integer);
begin
  { Check the protocol type - if it is ppNone, re-establish communication }
  if PanelComms.ProtocolType = ppNone then begin
    PanelComms.CloseComms;
    PanelComms.HandshakeReceived := false;
    EstablishComms;
  end
  { otherwise try to resend the last command, but only if the data type has not
  been reset to zero }
  else if FDataType <> dtNone then begin
    { Store the number of retries }
    FDataRetry := Value;
    { If the number of retries is greater than the maximum allowed retries }
    if Value > MAX_DATA_RETRIES then begin
      { Cancel the whole operation }
      MessageDlg ('Error: cannot communicate with XFP', mtError, [mbOK], 0);
      PanelComms.Cancel := true;
      DataRetry := 0;
      frmStatus.Close;
      DataType := dtNone;

      { Reset the protocol - have to re-establish handshake }
      PanelComms.ProtocolType := ppNone;
      PanelComms.CloseComms;

//need to update the display with whatever data we may have received
      RefreshAllPages;

    end
    { Now attempt to send last record again, but only after it has failed twice }
    else
      if FDataRetry > 0 then                        //not on initialisation to zero!
        PanelComms.SendString (RecordSent);
  end;
end;

procedure TMainForm.txtClientNameChange(Sender: TObject);
begin
  SiteFile.ClientName := txtClientName.Text;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.txtClientAdd1Change(Sender: TObject);
var
  iIndex: integer;
begin
  iIndex := (Sender as TEdit).Tag;
  case iIndex of
    1: SiteFile.ClientAddress1 := (Sender as TEdit).Text;
    2: SiteFile.ClientAddress2 := (Sender as TEdit).Text;
    3: SiteFile.ClientAddress3 := (Sender as TEdit).Text;
    4: SiteFile.ClientAddress4 := (Sender as TEdit).Text;
    5: SiteFile.ClientAddress5 := (Sender as TEdit).Text;
  end;
  { Set modified flag }
  ////Modified := true;
end;

procedure TMainForm.txtInstallAdd1Change(Sender: TObject);
var
  iIndex: integer;
begin
  iIndex := (Sender as TEdit).Tag;
  case iIndex of
    1: SiteFile.InstallerAddress1 := (Sender as TEdit).Text;
    2: SiteFile.InstallerAddress2 := (Sender as TEdit).Text;
    3: SiteFile.InstallerAddress3 := (Sender as TEdit).Text;
    4: SiteFile.InstallerAddress4 := (Sender as TEdit).Text;
    5: SiteFile.InstallerAddress5 := (Sender as TEdit).Text;
  end;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.txtPanelLocationChange(Sender: TObject);
begin
  SiteFile.PanelLocation := txtPanelLocation.Text;
  { Set modified flag }
  //Modified := true;
end;

procedure TMainForm.txtEngineerChange(Sender: TObject);
begin
  SiteFile.Engineer := txtEngineer.Text;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtServiceNoChange(Sender: TObject);
begin
  SiteFile.EngineerNo := txtServiceNo.Text;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtMaintStringChange(Sender: TObject);
begin
  SiteFile.MaintenanceString := txtMaintString.Text;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtQuiesStringChange(Sender: TObject);
begin
  SiteFile.QuiescentString := txtQuiesString.Text;
  { Set modified flag }
  // Modified := true;
end;

function TMainForm.Load_Integer(CmdData : string; iIndex : integer): integer;
begin
     Result := Ord(CmdData[iIndex+1]) * 256;
     Result := Result + Ord(CmdData[iIndex]);
end;

procedure TMainForm.LoadMainVersion (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;

  { Obtain version information }
   SiteFile.MainVersion := Copy (CmdData, 3, 5);
  SiteFile.Loop1 := Copy (CmdData, 8, 5);
  SiteFile.FrontPanel := Copy (CmdData, 18, 5);
  SiteFile.Loop2 := Copy (CmdData, 13, 5);

  { Now re-configure version dependant options }
  if SiteFile.MainVersion >= '09A06' then
    PgmData.MAX_CE_EVENTS := DEFAULT_MAX_CE_EVENTS
  else
    PgmData.MAX_CE_EVENTS := 16;

  { Now continue to load the required information }
  LoadInfo;
end;

procedure TMainForm.LoadZGroup (CmdData: string);
var
  iIndex: word;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  if (FDataCount <= MAX_ZONE) then begin
    { Store the zone groups }
    for iIndex := 1 to MAX_GROUP do begin
      SiteFile.ZoneList.Zone[FDataCount].Group[iIndex] := Ord (CmdData[iIndex + 2]);
    end;
    { Update panel sounder groups }
    SiteFile.PanelSounder1_Group := Ord(CmdData[MAX_GROUP + 3]);
    SiteFile.PanelSounder2_Group := Ord(CmdData[MAX_GROUP + 4]);

    if Ord(CmdData[2]) > $12 then begin              //if supported!
      Int_Tone_Box.ItemIndex := Ord(CmdData[MAX_GROUP + 5]) - 1;
      Cont_Tone_Box.ItemIndex  := Ord(CmdData[MAX_GROUP + 6]) - 1;
      Int_Tone_Box_Apollo.ItemIndex := Ord(CmdData[MAX_GROUP + 5]) - 1;
      Cont_Tone_Box_Apollo.ItemIndex  := Ord(CmdData[MAX_GROUP + 6]) - 1;
      Int_Tone_Box_CAST.ItemIndex := Ord(CmdData[MAX_GROUP + 5]) - 1;
      Cont_Tone_Box_CAST.ItemIndex  := Ord(CmdData[MAX_GROUP + 6]) - 1;
    end  ;

    if Ord(CmdData[2]) > $13 then begin              //if supported!
      if Ord(CmdData[MAX_GROUP + 7]) = 1 then
        SiteFile.ReSound_Function := TRUE
    else
        SiteFile.ReSound_Function := FALSE;

      chkReSound.Checked := SiteFile.ReSound_Function;
    end;

    { Go to next zone }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Loading group data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Loading group data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    RecordSent := TxReqZGroup + #1 + Chr(FDataCount);
  end
  else begin
    { If all data is to be loaded, go to next section which is site configuration }
    if FLoadAll then begin
      DataType := dtLoadSetList;
      LoadInfo;
    end
    else begin
      { Otherwise hide the status form and update the on-screen data }

//8/7/05
DataType := dtLoadSite;
LoadInfo;

//      frmStatus.Close;
//      DataType := dtNone;
      dwgGroupConfig.Invalidate;
    end;
  end;
end;

procedure TMainForm.LoadZoneSet (CmdData: string);
var
  iIndex: word;

begin

  { Reset the number of retries }
  FDataRetry := 0;

  if (FDataCount <= MAX_ZONE) then begin
    { Store the zone groups }
    iIndex := 1;
    while (iIndex <= MAX_ZONE_SET) do begin
       SiteFile.ZoneList.Zone[FDataCount].ZoneSet[iIndex] := Ord(CmdData[iIndex + 2]);
       inc(iIndex);
    end;

    { Load Set Delay Timer, if supported by the XFP version }
    if ord(CmdData[2]) > $12 then
      btnDelayTime.LoadValue (Load_Integer(CmdData, iIndex + 2));

    { Go to next zone }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Loading output set data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Loading output set data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    RecordSent := TxReqZoneSet + #1 + Chr(FDataCount);
  end
  else begin
    { If all data is to be loaded, go to next section which is site configuration }
    if FLoadAll then begin
      DataType := dtLoadSite;
      LoadInfo;
    end
    else begin
      { Otherwise hide the status form and update the on-screen data }
      frmStatus.Close;
      DataType := dtNone;
      UpdateConfigData;
    end;
  end;
end;

procedure TMainForm.LoadZoneNonFire (CmdData: string);
var
  iIndex: integer;
begin
  { Go along the response, and set the non fire flags accordingly }
  for iIndex := 1 to Ord (CmdData[2]) do begin
    SiteFile.Zonelist.Zone[iIndex].NonFire := (CmdData[iIndex + 2] = 'T');
  end;
  { If all data is to be loaded, go to the next section which is Group config }
  if FLoadAll then begin
    DataType := dtLoadGroupList;
    LoadInfo;
  end
  else begin
    { Otherwise hide the status form and update the on-screen data }
    frmStatus.Close;
    DataType := dtNone;
    UpdateConfigData;
  end;
end;

procedure TMainForm.LoadPointName (CmdData: string);
var
  NameString : string;
begin

    if ord(CmdData[2]) > 1 then         //length of 1 indicates end of list
    begin
      if FDataCount = 0 then
        frmDeviceName.ComboBox1.Clear;

      NameString := Copy (CmdData, 3, Ord (CmdData[2]));
      frmDeviceName.ComboBox1.Items.Add(NameString);

      inc(FDataCount);
       frmStatus.StatusText := 'Loading Device Name ' + IntToStr(FDataCount);
      RecordSent := TxReqPointName + #1 + chr(FDataCount);
    end else
    begin                               //end of list found.
      { Now place all the device names back into the main database }
      UpdateSubNames;

      { If all data is to be loaded, then go to the next section which is load the Zone List}
      if FLoadAll then begin
        DataType := dtLoadZoneList;
        LoadInfo;
      end
      else if ConfigPages.ActivePageIndex = 0 then begin
        DataType := dtLoadZoneList;
        LoadInfo;
      end
      else begin
        { Hide the status form and update the on-screen data }
        frmStatus.Close;
        DataType := dtNone;
        UpdateConfigData;
      end;
    end;
end;

procedure TMainForm.LoadPointData (CmdData: string);
var
  //LoopType: integer;
  frmLoop: TfrmLoop;
  iIndex: integer;
begin
  { Determine the loop number }
  case FDataType of
    dtLoadLoop1: begin
      Loop := 1;
      frmLoop := frmLoop1;
    end;

    dtLoadLoop2: begin
      if NoOfLoops = 1 then
      begin
        frmStatus.Close;
        DataType := dtNone;
        UpdateConfigData;
        exit;
      end;

      Loop := 2;
      frmLoop := frmLoop2;
    end;
  else
    Loop := 0;
    frmLoop := frmLoop1;
  end;

(*
  { Reset the number of retries }
  FDataRetry := 0;
  { Remap the old device type as the new type }
  case frmLoop.AFPLoop.SystemType of
    stApolloLoop: LoopType := 0;
    stHochikiLoop: LoopType := 1;
    stSysSensorLoop: LoopType := 2;
    stNittanLoop: LoopType := 3;
    stCASTLoop: LoopType := 4;
  else
    LoopType := 0;
  end;
*)

  {Now log whether the uploaded device type is the same as the one already present}
  //suppressed function at the moment AR 22/8/05
  if (frmLoop.AFPLoop.Device [FDataCount].DevType <> Ord (CmdData[5])) and
     (frmLoop.AFPLoop.Device [FDataCount].DevType < D_NotFitted) then
     frmLoop.AFPLoop.Device [FDataCount].TypeChanged := TRUE
  else
     frmLoop.AFPLoop.Device [FDataCount].TypeChanged := FALSE;

  frmLoop.AFPLoop.Device [FDataCount].DevType := Ord (CmdData[5]);
  frmLoop.AFPLoop.Device [FDataCount].Zone := Ord (CmdData[6]) and 63;


  frmLoop.AFPLoop.Device [FDataCount].HasBaseSounder := FALSE;

  {load the SharedData[] array. Note that Zone is held at CmdData[6], and is stored in the
   Zone property of loop data as well as SharedData[1]}
  for iIndex := 1 to 8 do
  begin
   frmLoop.AFPLoop.Device [FDataCount].SharedData[iIndex] := ord(CmdData[iIndex + 5]);
  end;


  if (FDataCount < frmLoop.AFPLoop.NoLoopDevices) then begin
    { Load next point. Increment point counter by one }
    inc (FDataCount);
    if (frmLoop.AFPLoop.SystemType = stSysSensorLoop) and (FDataCount > HALF_MAX_POINTS_SYS_SENSOR) then
      iIndex := FDataCount + 1
    else
      iIndex := FDataCount;
    { Update progress bar }
    frmStatus.Progress.Position := FDataCount;
    { System sensor is to be treated differently from the other protocols }
    if frmLoop.AFPLoop.SystemType = stSysSensorLoop then begin
      if FDataCount <= HALF_MAX_POINTS_SYS_SENSOR then begin
        frmStatus.StatusText := 'Loading data for Loop ' + IntToStr (Loop) +
          ' - Point S' + IntToStr (FDataCount);
      end
      else begin
        frmStatus.StatusText := 'Loading data for Loop ' + IntToStr (Loop) +
          ' - Point M' + IntToStr (FDataCount - HALF_MAX_POINTS_SYS_SENSOR);
      end
    end
    else begin
      frmStatus.StatusText := 'Loading data for Loop ' + IntToStr (Loop) +
        ' - Point ' + IntToStr (FDataCount);
    end;

    iIndex := ((Loop-1) * 256) + iIndex;
    RecordSent := TxReqPData + #2 + Chr(iIndex and 255) + Chr (iIndex div 256);
  end
  else begin
    if FLoadAll then
    begin
      if (Loop = 1) and (Loop < NoOfLoops) then
        DataType := dtLoadLoop2
      else
        DataType := dtLoadPName;
    end else
    begin
        DataType := dtLoadPName;
    end;
    LoadInfo;
  end;
end;

procedure TMainForm.LoadLoopType (CmdData: string);
var
  iLoopType: TSystemType;
begin
  FLoadLoopType := false;
  { Reset the number of retries }
  FDataRetry := 0;
  { Obtain the loop type }
  iLoopType := TSystemType( Ord (CmdData[3]));

  if Ord(CmdData[2]) > 1 then
    NoOfLoops := ord(CmdData[4])
  else
    NoOfLoops := 2;

  if FSaveLoopType = TRUE then
  begin
    if SiteFile.SystemType <> iLoopType then
    begin
      MessageDlg('The panel protocol does not match the tools', mtWarning,
                    [mbOk], 0);
      DataType := dtNone;
      FSaveLoopType := FALSE; //BUG0000099
      frmStatus.Close;
      Exit;
    end;
  end
  else //JG CAST Panel Lockout
  begin
    if iLoopType = stCASTLoop then
    begin
      if CAST_Protocol_Available = FALSE then
      begin
        MessageDlg('Panel Protocol unavailable', mtWarning, [mbOk], 0);
        DataType := dtNone;
        frmStatus.Close;
        Exit;
      end;
    end;
  end;

  { Store the loop type for loop1}
  case iLoopType of
    stApolloLoop,
    stHochikiLoop,
    stSysSensorLoop,
    stNittanLoop,
    stCASTLoop:
    begin
      if SiteFile.SystemType <> iLoopType then
      begin
        SiteFile.SystemType := iLoopType;
        RefreshProtocolControls;
      end;
    end
    else
    begin
      PanelComms.Cancel := true;
      DataType := dtNone;
      MessageDlg ('Error: Invalid loop type', mtError, [mbOK], 0);
      frmStatus.Close;
      Exit;
    end;
  end;

  { Show the status form, and ensure that the cancel flag has been reset }
  PanelComms.Cancel := false;
  frmStatus.Show;
  LoadInfo;
end;

procedure TMainForm.LoadRepeaterName (CmdData: string);
var
   TString : string;
begin
  { Reset the number of retries }
  FDataRetry := 0;

  TString := Copy (CmdData, 3, Ord (CmdData[2]));
  SiteFile.RepeaterList.Repeater[FDataCount].Name := Trim (TString);

  RecordSent := TxReqRepeaterFitted + #1 + chr (FDataCount);
end;

procedure TMainForm.LoadRepeaterFitted (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;
  SiteFile.RepeaterList.Repeater[FDataCount].Fitted := (CmdData[3] = #255);
  if (FDataCount < MAX_REPEATER) then begin
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Loading Network data for Panel ' + IntToStr (FDataCount);
    RecordSent := TxReqRepeaterName + #1 + Chr (FDataCount);
  end
  else begin
    FDataCount := 1;
    RecordSent := TxReq_NetPanelData + #0;
  end;
end;

procedure TMainForm.LoadOutputName (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;
  SiteFile.RepeaterList.Output[FDataCount].Name := Copy (CmdData, 3, Ord (CmdData[2]));
  RecordSent := TxReqOutputFitted + #1 + chr (FDataCount);
end;

procedure TMainForm.LoadOutputFitted (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;
  SiteFile.RepeaterList.Output[FDataCount].Fitted := (CmdData[3] = #255);
  if (FDataCount < MAX_ZONAL_OUTPUT) then begin
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Loading data for zonal output ' + IntToStr (FDataCount);
    RecordSent := TxReqOutputName + #1 + Chr (FDataCount);
  end
  else
    RecordSent := TxReqSegNo + #0;
end;

procedure TMainForm.LoadNetworkData(CmdData: string);
var
   PanelNo : integer;
   d : integer;
begin

//get the stuff and put into the array positions.
    for PanelNo := 1 to MAX_PANELS do begin
      d := ord(CmdData[PanelNo + 2]);
      with SiteFile.XFP_Network do begin
        SiteFile.RepeaterList.Repeater[PanelNo].Fitted := (d and 1) = 1;
        PAccept_Faults[PanelNo] := (d and 2) = 2;
        PAccept_Alarms[PanelNo] := (d and 4) = 4;
        PAccept_Controls[PanelNo] := (d and 8) = 8;
        PAccept_Disablements[PanelNo] := (d and 16) = 16;
        PAccept_Occupied[PanelNo] := (d and 32) = 32;
      end;

    end;
    RecordSent := TxReqSegNo + #0;
end;

procedure TMainForm.LoadSegNo (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;
  RecordSent := TxReqPanelName + #0;
end;

procedure TMainForm.LoadPanelName (CmdData: string);
var
   TString : string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  TString := Copy (CmdData, 3, Ord (CmdData[2]));
  PanelNameEdit.Text := Trim (TString);

  if FLoadAll then begin
    DataType := dtLoadCE_Event;
    LoadInfo;
  end
  else begin
    { Otherwise hide the status form and update the on-screen data }
    frmStatus.Close;
    DataType := dtNone;
    UpdateConfigData;
  end;
end;

procedure TMainForm.LoadMaintName (CmdData: string);
var
   TString : string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Obtain the maintenance string }

  TString := Copy (CmdData, 3, Ord (CmdData[2]));
  SiteFile.MaintenanceString := Trim (TString);
  txtMaintString.Text := SiteFile.MaintenanceString;

  RecordSent := TxReqMaintDate + #0;
end;

procedure TMainForm.LoadMaintDate (CmdData: string);
var
  sDate: TDateTime;
begin
{ Reset the number of retries }
  FDataRetry := 0;
  { Obtain the maintenance date - remembering that the year is offset from
  the base year }

  try
    sDate := EncodeDate (Ord (CmdData[3]) + Base_Year, Ord (CmdData[4]), Ord (CmdData[5]));
    { If the last data character is 255, then maintenance date is enabled }
    SiteFile.MaintenanceDate := sDate;
  finally
    RecordSent := TxReqAL2Code + #0;
  end;
end;

procedure TMainForm.LoadAL2Code (CmdData: string);
var
   I : integer;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Obtain the level 2 code }
  I := Load_Integer(CmdData, 3);
  txtLvl2.Text := IntToStr(I);

  RecordSent := TxReqAL3Code + #0;
end;

procedure TMainForm.LoadAL3Code (CmdData: string);
var
   I : integer;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Obtain the level 3 code }
  I := Load_Integer(CmdData, 3);
  txtLvl3.Text := IntToStr(I);

  if Ord(CmdData[2]) > 2 then
    if Ord(CmdData[5]) = 1 then
      SiteFile.Discovery_Polling_LED := true
    else
      SiteFile.Discovery_Polling_LED := false;

  if Ord(CmdData[2]) > 5 then
  begin
      SiteFile.MCP_Delay := Ord(CmdData[6])+ 1;
      SiteFile.Detector_Delay := Ord(CmdData[7]) + 1;
      SiteFile.IO_Delay := Ord(CmdData[8]) + 1;
  end;
  RecordSent := TxReqFaultLockOutTime + #0;
end;

procedure TMainForm.LoadFaultLockoutTime (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Obtain the fault lock out time, but it isn't used anymore 30/4/2001}
//  FaultLockoutSpin.LoadValue (Ord (CmdData[3]));
  RecordSent := TxReqPhasedSettings + #0;
end;

procedure TMainForm.LoadPhasedSettings (CmdData: string);
begin
  { Reset the number of retries }
  FDataRetry := 0;

  SiteFile.PhasedDelay := Load_Integer(CmdData, 3);
  PhasedDelaySpin.LoadValue (SiteFile.PhasedDelay);
  SiteFile.InvestigationPeriod := Load_Integer(CmdData, 5);
  spnInvestigation.LoadValue (SiteFile.InvestigationPeriod);
  SiteFile.InvestigationPeriod1 := Load_Integer(CmdData, 7);
  spnInvestigation1.LoadValue (SiteFile.InvestigationPeriod1);
  RecordSent := TxReqQuiesName + #0;
end;

procedure TMainForm.LoadQuiesName (CmdData: string);
var
   TString : string;
begin
  { Reset the number of retries }
  FDataRetry := 0;

  TString := Copy (CmdData, 3, Ord (CmdData[2]));
  SiteFile.QuiescentString := Trim(TString);
  txtQuiesString.Text := Trim (TString);

  RecordSent := TxReqDayNight + #0;
end;


procedure TMainForm.LoadDayNightSettings (CmdData: string);
var
   Hour : integer;
   Minute : integer;
begin

//This MUST be the last piece of data from the Site Data page

  Hour := Ord(CmdData[4]);
  Minute := Ord(CmdData[3]);
  SiteFile.NightEnd := Format('%2.2d', [Hour]) + ':' + Format('%2.2d',[Minute]);

  Hour := Ord(CmdData[6]);
  Minute := Ord(CmdData[5]);
  SiteFile.NightBegin := Format('%2.2d', [Hour]) + ':' + Format('%2.2d',[Minute]);

  Hour := Ord(CmdData[8]);
  Minute := Ord(CmdData[7]);
  SiteFile.ReCalTime := Format('%2.2d', [Hour]) + ':' + Format('%2.2d',[Minute]);

  //if the version of XFP has the day/night flags capability, then read them
  if Ord(CmdData[2]) > 6 then
  begin
    SiteFile.Day_Enable_Flags := Ord(CmdData[9]);
    SiteFile.Night_Enable_Flags := Ord(CmdData[10]);
  end;

  if Ord(CmdData[2]) > 8 then   //if included...
    if Ord(CmdData[11]) = 1 then
      SiteFile.BST_Adjustment := true
    else
      SiteFile.BST_Adjustment := false;

  if Ord(CmdData[2]) > 9 then   //if included...
    if Ord(CmdData[12]) = 1 then
      SiteFile.RealTime_Event_Output := true
    else
      SiteFile.RealTime_Event_Output := false;

  { If all the data is to be loaded, go to the next section which is repeater config}
  if FLoadAll then begin
    DataType := dtLoadRepeaterList;
    LoadInfo;
  end
  { Otherwise hide the status form }
  else begin
    frmStatus.Close;
    DataType := dtNone;
    UpdateConfigData;
  end;
end;

procedure TMainForm.LoadZoneName (CmdData: string);
var
   TString : string;
begin
  { Reset the number of retries }
  FDataRetry := 0;
  { Store zone name }
  SiteFile.ZoneList.Zone[FDataCount].Name := Copy (CmdData, 3, Ord (CmdData[2]));

  TString := Copy (CmdData, 3, Ord (CmdData[2]));
  SiteFile.ZoneList.Zone[FDataCount].Name := Trim (TString);

  { Request information about the timers }
  RecordSent := TxReqZoneTimers + #1 + Chr (FDataCount);
end;

function TMainForm.NormaliseDependancy(Inp: char): integer;
begin
     {
     if Inp = 'A' then result := 1;
     if Inp = 'B' then result := 2;
     if Inp = 'C' then result := 3;
     if Inp = 'N' then result := 4;
     if Inp = 'I' then result := 5;
     }
  case Inp of
    'A': Result := 1;
    'B': Result := 2;
    'C': Result := 3;
    'N': Result := 4;
    'I': Result := 5;
    'D': Result := 6;
    else Result := 0;
  end;
end;

procedure TMainForm.LoadZoneTimers (CmdData: string);
var
   I : integer;
begin
  { Reset the number of retries }
  FDataRetry := 0;

  SiteFile.ZoneList.Zone[FDataCount].SounderDelay := Load_Integer(CmdData, 3);
  SiteFile.ZoneList.Zone[FDataCount].Relay1Delay := Load_Integer(CmdData, 5);
  SiteFile.ZoneList.Zone[FDataCount].Relay2Delay := Load_Integer(CmdData, 7);
  SiteFile.ZoneList.Zone[FDataCount].RemoteDelay := Load_Integer(CmdData, 9);

  SiteFile.ZoneList.Zone[FDataCount].Dependancy[1] := NormaliseDependancy (CmdData[11]);
  SiteFile.ZoneList.Zone[FDataCount].Dependancy[4] := NormaliseDependancy (CmdData[12]);

  SiteFile.ZoneList.Zone[FDataCount].Dependancy[2] := Load_Integer(CmdData, 15);
  SiteFile.ZoneList.Zone[FDataCount].Dependancy[3] := Load_Integer(CmdData, 13);
  SiteFile.ZoneList.Zone[FDataCount].Dependancy[5] := Load_Integer(CmdData, 19);
  SiteFile.ZoneList.Zone[FDataCount].Dependancy[6] := Load_Integer(CmdData, 17);

  I := ord(CmdData[21]);
  if (I and 1) = 1 then
    SiteFile.ZoneList.Zone[FDataCount].Detector := TRUE
  else
    SiteFile.ZoneList.Zone[FDataCount].Detector := FALSE;

  if (I and 2) = 2 then
    SiteFile.ZoneList.Zone[FDataCount].MCP := TRUE
  else
    SiteFile.ZoneList.Zone[FDataCount].MCP := FALSE;

  if (I and 4) = 4 then
    SiteFile.ZoneList.Zone[FDataCount].EndDelays := TRUE
  else
    SiteFile.ZoneList.Zone[FDataCount].EndDelays := FALSE;

  { If this is not the last zone }
  if (FDataCount < MAX_ZONE) then begin
    { Increment the count, then update the progress bar }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;

    if FDataCount <= MAX_LOCAL_ZONES then
      frmStatus.StatusText := 'Loading data for zone ' + IntToStr (FDataCount)
    else
      frmStatus.StatusText := 'Loading data for panel ' + IntToStr (FDataCount - MAX_LOCAL_ZONES);

    { Send request for name of next zone }
    RecordSent := TxReqZoneName + #1 + Chr (FDataCount);
  end
  else begin
    if FLoadAll then begin
       DataType := dtLoadGroupList;
      LoadInfo;
    end
    else begin
      { Otherwise hide the status form and update the on-screen data }

//8/7/2005
DataType := dtLoadSite;
LoadInfo;

//      frmStatus.Close;
//      DataType := dtNone;
//      UpdateConfigData;
    end;
  end; { else last zone has been processed }
end;

procedure TMainForm.LoadEventHistory (CmdData: string);
var
   Another : boolean;
   TString : string;
begin
  { Reset the number of retries }
     FDataRetry := 0;

     Another := FALSE;

     if Ord(CmdData[2]) > 0 then
     begin
       TString := Copy (CmdData, 3, Ord (CmdData[2]));
       EventLog.SetFocus;
       EventLog.Lines.Add(TString);

       EventLog.SelStart := length(EventLog.Text);
       EventLog.SelLength := 0;
       EventLog.SelText := '';

       inc(FDataCount);

       frmStatus.SetFocus;
       frmStatus.Progress.Position := FDataCount;
       frmStatus.StatusText := 'Loading event #' + IntToStr (FDataCount);

       Another := TRUE;
     end;

     if Another = TRUE then
     begin
       if Event_Clear = mrYes then
         RecordSent := TxReqEvent + #3 + #0 + #0 + #1
       else
         RecordSent := TxReqEvent + #3 + Chr (FDataCount and 255) + chr(FDataCount div 256) + #0;
     end else
     begin
       frmStatus.Close;
       DataType := dtNone;
       PanelComms.IsASCIIOnly := TRUE;   //allow only ASCII data i.e. Print Event History
     end;
end;


procedure TMainForm.LoadC_EData (CmdData: string);
var
   Hour, Minute : integer;
begin
  { Reset the number of retries }
   FDataRetry := 0;

  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[1] := ord(CmdData[3]);
  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[2] := Load_Integer(CmdData, 4);
  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[3] := ord(CmdData[6]);
  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[4] := Load_Integer(CmdData, 7);
  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[5] := ord(CmdData[9]);
  SiteFile.C_ETable.C_ETable[FDataCount].C_EArray[6] := Load_Integer(CmdData, 10);

  { Load the time event marker }
  Hour := Ord(CmdData[13]);
  Minute := Ord(CmdData[12]);
  if(FDataCount <= MAX_CE_TIMER_EVENTS) then
    SiteFile.C_ETable.Time_Events[FDataCount] := Format('%2.2d', [Hour]) + ':' + Format('%2.2d',[Minute]);

  { If this is not the last zone }
  if (FDataCount < PgmData.MAX_CE_EVENTS) then begin
    { Increment the count, then update the progress bar }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Loading data for Event ' + IntToStr (FDataCount);
    { Send request for name of next zone }
    RecordSent := TxReqC_EEvent + #1 + Chr (FDataCount);
  end
  else begin
      frmStatus.Close;
      DataType := dtNone;
      UpdateConfigData;
  end; { else last zone has been processed }
end;


procedure TMainForm.SetRecordSent(const Value: string);
begin
  { Store the record }
  FRecordSent := Value;

  PanelComms.PanelNumber := PanelNumber.Value;
  { Now send that record }
  PanelComms.SendString (Value);
end;

{ The sounder selected property stores the image iIndex of the button pressed }
procedure TMainForm.btnSounderOffClick(Sender: TObject);
begin
  SounderSelected := 0;
  btnSounderOff.Down := TRUE;
end;

procedure TMainForm.btnSounderContClick(Sender: TObject);
begin
  SounderSelected := 2;
  btnSounderCont.Down := TRUE;
end;

procedure TMainForm.btnSounderPulsedClick(Sender: TObject);
begin
  SounderSelected := 1;
  btnSounderPulsed.Down := TRUE;
end;

function TMainForm.BuildXFerString(const pPreamble: char;
  const pPayload: string): string;
begin
  Result := pPreamble + char( Length( pPayload )) + pPayload;
end;

procedure TMainForm.Button1Click(Sender: TObject);
begin
  IncrementSerialNumber;
end;

procedure TMainForm.dwgGroupConfigDrawCell(Sender: TObject; ACol,
  ARow: Integer; Rect: TRect; State: TGridDrawState);
var
  GroupIndex: integer;
  Bitmap: TBitmap;
begin
  with dwgGroupConfig.Canvas do begin
    { If it is not the first row, determine the correct offset for the column }
    if ARow = 0 then begin
      case ACol of
        1..5: TextOut (Rect.Left + 10, Rect.Top, IntToStr (ACol));
        7..11: TextOut (Rect.Left + 10, Rect.Top, IntToStr (ACol - 1));
        13..17: TextOut (Rect.Left + 10, Rect.Top, IntToStr (ACol - 2));
        19..19: TextOut (Rect.Left + 10, Rect.Top, IntToStr (ACol - 3));
      end;
    end
    else if (ACol = 6) or (ACol = 12) or (ACol = 18) then begin
      Brush.Color := cl3dLight;
      FillRect (Rect);
      Brush.Color := clWindow;
    end
    else begin
      if ACol = 0 then         //fill in the Zone/Panel number
      begin
       if ARow <= (MAX_ZONE - MAX_PANELS) then
         TextOut (Rect.Left + 1, Rect.Top + 5, 'Zone ' + IntToStr(ARow))
       else
         TextOut (Rect.Left + 1, Rect.Top + 5, 'Panel ' + IntToStr(ARow - (MAX_ZONE - MAX_PANELS)));
      end else begin
        { Create a temporary holder for the bitmap image }
        Bitmap := TBitmap.Create;

        try
          { Determine the correct iIndex for the group from the column number }
          case ACol of
            1..5: GroupIndex := ACol;
            7..11: GroupIndex := ACol - 1;
            13..17: GroupIndex := ACol - 2;
            19..19: GroupIndex := ACol - 3;
          else
            GroupIndex := 0;
          end;
          { If it is a group, then determine the correct bitmap }
           PgmData.imglstGroupConfig.GetBitmap (SiteFile.ZoneList.Zone[ARow].Group[GroupIndex], Bitmap);
          {Draw circle}
          if (ACol <> 6) and (ACol <> 12) and (ACol <> 18) then begin
            { Show image }
            dwgGroupConfig.Canvas.Draw(Rect.Left + 8, rect.Top, BitMap);
          end;
        finally
          Bitmap.Free;
        end;
      end; { if ACol = 0 }
    end; { if ARow > 0 }
  end; { with dwgGroupConfig }
end;

procedure TMainForm.dwgGroupConfigSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
begin
  if SounderSelected = -1 then
    exit;

  { Store the value of the sounder selected in the relevant place }
  case ACol of
    1..5: SiteFile.ZoneList.Zone[ARow].Group[ACol] := SounderSelected;
    { 6 is a space }
    7..11: SiteFile.ZoneList.Zone[ARow].Group[ACol - 1] := SounderSelected;
    { 12 is a space }
    13..17: SiteFile.ZoneList.Zone[ARow].Group[ACol - 2] := SounderSelected;
    { 18 is a space }
    19..19: SiteFile.ZoneList.Zone[ARow].Group[ACol - 3] := SounderSelected;
  end;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.dwgGroupConfigMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  ARow, ACol: integer;
  Rect: TGridRect;
begin
  if SounderSelected = -1 then
    exit;

  if Button = mbLeft then begin
    Rect := dwgGroupConfig.Selection;
    for ARow := Rect.Top to Rect.Bottom do begin
      for ACol := Rect.Left to Rect.Right do begin
        if (ARow > 0) then begin
          case ACol of
            1..5: SiteFile.ZoneList.Zone[ARow].Group[ACol] := SounderSelected;
            { 6 is a space }
            7..11: SiteFile.ZoneList.Zone[ARow].Group[ACol - 1] := SounderSelected;
            { 12 is a space }
            13..17: SiteFile.ZoneList.Zone[ARow].Group[ACol - 2] := SounderSelected;
            { 18 is a space }
            19..19: SiteFile.ZoneList.Zone[ARow].Group[ACol - 3] := SounderSelected;
          end; {Case Col}
        end; {if ARow > 0 }
      end; { for ACol }
    end; {for ARow }
  end; { if left button was released }
  dwgGroupConfig.Invalidate;
end;

procedure TMainForm.btnSetAllOffClick(Sender: TObject);
var
  iZone: integer;
  iGroup: integer;
  SetAll: integer;
begin
  { Determine what to set all buttons as }
  SetAll := (Sender as TBitBtn).Tag;
  { Mark all groups in all zones as being set to the appropriate value }
  for iZone := 1 to MAX_ZONE do begin
    for iGroup := 1 to MAX_GROUP do begin
      SiteFile.Zonelist.Zone[iZone].Group[iGroup] := SetAll;
    end;
  end; { for iZone }
  { Now update the grid appropriately }
  dwgGroupConfig.Invalidate;
  btnSounderOff.Down := FALSE;
  btnSounderPulsed.Down := FALSE;
  btnSounderCont.Down := FALSE;
  SounderSelected := -1;

end;

procedure TMainForm.btnLoadGroupConfigClick(Sender: TObject);
begin
  { Do not load all the data }
  FLoadAll := false;
  DataType := dtLoadGroupList;
  EstablishComms;
end;

procedure TMainForm.btnSaveGroupConfigClick(Sender: TObject);
begin
  FSaveAll := false;
  FSaveLoopType := false;
  DataType := dtSaveZGroup;
  FCheckNVM := true;
  EstablishComms;
end;

procedure TMainForm.frmLoop1dwgLoopSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
begin
  frmLoop1.dwgLoopSelectCell(Sender, ACol, ARow, CanSelect);
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.frmLoop2dwgLoopSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
begin
  frmLoop2.dwgLoopSelectCell(Sender, ACol, ARow, CanSelect);
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtInstallNameChange(Sender: TObject);
begin
  SiteFile.InstallerName := txtInstallName.Text;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtMaintDateChange(Sender: TObject);
begin
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.UploadTimeSelectClick(Sender: TObject);
begin
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.txtLvl2Change(Sender: TObject);
begin
  { Set modified flag }
  SiteFile.AL2Code := txtLvl2.Text;
  // Modified := true;

  if txtLvl2.Text = '5555' then
  begin
    txtLvl2.Visible := FALSE;
    Keyswitch.Checked := TRUE;
    lblAccessLvl2.Caption := 'KeySwitch Operated';
  end else
  begin
    txtLvl2.Visible := TRUE;
    Keyswitch.Checked := FALSE;
    lblAccessLvl2.Caption := 'Access Level 2 Code';
  end
end;

procedure TMainForm.txtLvl3Change(Sender: TObject);
begin
  { Set modified flag }
  SiteFile.AL3Code := txtLvl3.Text;
  // Modified := true;
end;

procedure TMainForm.PanelNameEditChange(Sender: TObject);
begin
  SiteFile.RepeaterList.PanelName := PanelNameEdit.Text;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.btnPrintClick(Sender: TObject);
begin
  { Check to see if the code is OK on the site config page }
  ConfigPages.SetFocus;
  if FCodeOK then
    frmPrint.ShowModal;
end;

procedure TMainForm.AllowDownloads(CmdData: string);
var
  DlgResponse: integer;
begin
  { This procedure sets the EELinkFitted determined by the data received }
  FEELinkFitted := (CmdData[3] = #255);
  FCheckNVM := false;
  if FEELinkFitted then begin
    { If link is fitted, retrieve the version numbers for the panel }
    frmStatus.StatusText := 'Loading version information';
    RecordSent := TxReqMainVersion + #0;
  end
  else begin
    DlgResponse := MessageDlg ('The NVM Program link is not fitted in the Fire panel. ' +
                   'Please fit the link and select Retry. Press Abort to exit.',
                   mtWarning, [mbRetry, mbAbort],0);
    if DlgResponse = mrRetry then begin
      RecordSent := ReqDownload + #0;
    end
    else begin
      PanelComms.Cancel := true;
      frmStatus.Close;
      DataType := dtNone;
    end;
  end;
end;

procedure TMainForm.SaveLogButtonClick(Sender: TObject);
begin
  if PgmData.SaveLogDialog.Execute then begin
    EventLog.Lines.SaveToFile (PgmData.SaveLogDialog.Filename);
  end;
end;

procedure TMainForm.LoadLogButtonClick(Sender: TObject);
begin
  if PgmData.OpenLogDialog.Execute then begin
    EventLog.Lines.LoadFromFile (PgmData.OpenLogDialog.Filename);
  end;
end;

procedure TMainForm.ClearLogButtonClick(Sender: TObject);
begin
  EventLog.Clear;
end;

procedure TMainForm.SetDataType(const Value: TDataType);
begin
  FDataType := Value;
end;

procedure TMainForm.HelpAboutItemClick(Sender: TObject);
begin
  AboutBox.Version.Caption := 'Version :' + ABOUT_CAPTION;
  AboutBox.ShowModal;
end;

procedure TMainForm.txtLvl2KeyPress(Sender: TObject; var Key: Char);
begin
  { If an alphanumeric has been pressed, inc punctuation, then do not display that
  character unless it is either 1, 2, 3, or 4 }
  if (Key >= ' ' ) and (Key < 'z') then
    if (Key < '1') or (Key > '4') then Key := #0;

  if Key > #0 then
    Keyswitch.Checked := FALSE;
end;

procedure TMainForm.PrinterPropertiesClick(Sender: TObject);
begin
  PgmData.PrintSetup.Execute;
end;

(*
procedure TMainForm.btnNewClick(Sender: TObject);
var
  WhichPage: integer;

begin
  { Check to see if the user wishes to set up a new sheet }
  if (MessageDlg ('This will clear the current setup. Do you wish to continue?',
    mtConfirmation, [mbYes, mbNo], 0) = mrYes)
  then begin
    sgrdLoop.Col := 0;
    sgrdLoop.Row := 1;

    WhichPage:=ConfigPages.ActivePageIndex;
    IniFileName := '';
    ClearInfo;
    Modified := false;
    RefreshAllPages;
    ConfigPages.ActivePageIndex:=WhichPage;
  end;
end;
*)

(*
procedure TMainForm.ClearInfo;
var
  iIndex, I1: integer;
  ZoneIndex: integer;
  Year, Month, Day: Word;
begin
  NoOfLoops := 2;

  { Clear the loop object }
  for iIndex := 1 to frmLoop1.AFPLoop.NoLoopDevices do begin
    { Clear all device information for loop 1 }
    with frmLoop1.AFPLoop.Device[iIndex] do begin
      DevType := D_NOT_FITTED;
      TypeChanged := FALSE;
      Name := 'No name allocated';
      SubName[1] := Name;
      SubName[2] := Name;
      SubName[3] := Name;
      Group := 0;
      Zone := 1;
      Hint := '';
      Name_Index := 0;
      SharedData[1] := 1;
      SharedData[2] := 0;
      SharedData[3] := 0;
      SharedData[4] := 0;
      SharedData[5] := 0;
      SharedData[6] := 0;
      SharedData[7] := 0;
      SharedData[8] := 0;
    end;

    { Clear all device information for loop 2 }
    with frmLoop2.AFPLoop.Device[iIndex] do begin
      DevType := D_NOT_FITTED;
      TypeChanged := FALSE;
      Name := 'No name allocated';
      SubName[1] := Name;
      SubName[2] := Name;
      SubName[3] := Name;
      Group := 0;
      Zone := 1;
      Hint := '';
      Name_Index := 0;
      SharedData[1] := 1;
      SharedData[2] := 0;
      SharedData[3] := 0;
      SharedData[4] := 0;
      SharedData[5] := 0;
      SharedData[6] := 0;
      SharedData[7] := 0;
      SharedData[8] := 0;
    end;
  end;

  { Clear all zone and set information }
  for ZoneIndex := 1 to MAX_ZONE do with SiteFile.ZoneList.Zone[ZoneIndex] do begin
    for iIndex := 1 to 5 do Group[iIndex] := 2;
    for iIndex := 6 to MAX_GROUP do Group[iIndex] := 0;
    for iIndex := 0 to MAX_ZONE_SET do ZoneSet[iIndex] := 0;
    for iIndex := 17 to MAX_ZONE_SET do ZoneSet[iIndex] := 2;


    if ZoneIndex <= (MAX_ZONE - MAX_PANELS) then
      Name := 'Zone ' + IntToStr(ZoneIndex)
    else
      Name := 'Panel ' + IntToStr(ZoneIndex - (MAX_ZONE - MAX_PANELS));

    SounderDelay := 0;
    RemoteDelay := 0;
    Relay1Delay := 0;
    Relay2Delay := 0;
    Dependancy[1] := 4;
    Dependancy[4] := 4;

    Dependancy[2] := 60;                    //default detector reset to 1 minute
    Dependancy[3] := 300;                   //and alarm reset to 5 minutes
    Dependancy[5] := 60;
    Dependancy[6] := 300;


    Detector := true;
    MCP := false;
    NonFire := false;
  end;
  btnDelayTime.Value := 300;

  {make panel relay1 (Set 17) silencable}
  SiteFile.ZoneList.Zone[0].ZoneSet[17] := 1;

  { Clear all site information }
  with SiteFile.SiteConfig do begin
    ClientName := '';
    ClientAddress1 := '';
    ClientAddress2 := '';
    ClientAddress3 := '';
    ClientAddress4 := '';
    ClientAddress5 := '';
    InstallerName := '';
    InstallerAddress1 := '';
    InstallerAddress2 := '';
    InstallerAddress3 := '';
    InstallerAddress4 := '';
    InstallerAddress5 := '';
    AL2Code := '3333';
    AL3Code := '4444';
    PanelLocation := '';
    Engineer := '';
    EngineerNo := '';
    MaintenanceString := 'Call the Engineer';
    QuiescentString := 'XFP Fire Panel';

//    MDate := IncMonth(Now, 12);               //add 12 months from today
    DecodeDate (Now, Year, Month, Day);      //set maintainence date
    MaintenanceDate := EncodeDate(2099,1,1);  //to jan 1st 2099

    DateEnabled := false;
    PhasedDelay := 65535;
    SoundersPulsed := false;
    FaultLockout := 10;
    CopyTime := false;
    InvestigationPeriod := 300;
    InvestigationPeriod1 := 180;
    NightBegin := '00:00';
    NightEnd := '00:00';
    ReCalTime := '04:00';

    PanelSounder1_Group := 1;
    PanelSounder2_Group := 2;

    Day_Enable_Flags := 0;
    Night_Enable_Flags := 0;

    Discovery_Polling_LED := TRUE;

    BST_Adjustment := TRUE;
    RealTime_Event_Output := FALSE;

    MCP_Delay := 1;
    IO_Delay := 1;
    Detector_Delay := 5;
  end;

  { initialise the network stuff }
  with SiteFile.XFP_Network do begin
    for I1 := 1 to MAX_PANELS do begin
      PAccept_Faults[I1] := FALSE;
      PAccept_Alarms[I1] := FALSE;
      PAccept_Controls[I1] := FALSE;
      PAccept_Disablements[I1] := FALSE;
      PAccept_Occupied[I1] := FALSE;
    end;
  end;

  { Clear all repeater network information }
  with SiteFile.RepeaterList do begin
    for iIndex := 1 to MAX_REPEATER do begin
      Repeater[iIndex].Name := 'Panel ' + IntToStr (iIndex);
      Repeater[iIndex].Fitted := false;
    end;
    Repeater[1].Fitted := TRUE;
    PanelName := 'Main Panel';
    Segment := 1;
  end;

  { Clear the cause & effect equations}
  for iIndex := 1 to SiteFile.MAX_CE_EVENTS do
    for I1 := 1 to 6 do
      SiteFile.C_ETable.C_ETable[iIndex].C_EArray[I1] := 0;

  { default 1st C&E to All sounder groups Evac from input 1}
  SiteFile.C_ETable.C_ETable[1].C_EArray[1] := 50;
  SiteFile.C_ETable.C_ETable[1].C_EArray[3] := 55;
  SiteFile.C_ETable.C_ETable[1].C_EArray[5] := 55;

  { default 2nd C&E to All sounder groups Alert from input 2}
  SiteFile.C_ETable.C_ETable[2].C_EArray[1] := 40;
  SiteFile.C_ETable.C_ETable[2].C_EArray[3] := 55;
  SiteFile.C_ETable.C_ETable[2].C_EArray[4] := 1;
  SiteFile.C_ETable.C_ETable[2].C_EArray[5] := 55;
  SiteFile.C_ETable.C_ETable[2].C_EArray[6] := 1;

  for iIndex := 1 to MAX_CE_TIMER_EVENTS do
     TMaskEdit(FindComponent ('T' + IntToStr(iIndex))).Text := '00:00';

  { Clear View event log }
  EventLog.Clear;

  { Clear Comments page }
  CommentsEdit.Clear;
  CommentsEdit.Lines := RichEdit1.Lines;

  { Ensure that the following pages are correctly updated }
  RefreshAllPages;

   { initialise version information }
   SiteFile.MainVersion := 'Not Available';
  SiteFile.Loop1 := 'Not Available';
  SiteFile.FrontPanel := 'Not Available';
  SiteFile.Loop2 := 'Not Available';

  txtNight_Begin.Text := '00:00';
  txtNight_End.Text := '00:00';
  txtReCal.Text := '04:00';

  if frmLoop1.AFPLoop.SystemType = stHochikiLoop then
  begin
    Int_Tone_Box.ItemIndex := 1;
    Cont_Tone_Box.ItemIndex := 0;
  end else
  begin
    Int_Tone_Box_Apollo.ItemIndex := 0;
    Cont_Tone_Box_Apollo.ItemIndex := 0;
  end;

  SiteFile.ReSound_Function := FALSE;
  chkReSound.Checked := FALSE;

  SendToPanel.Checked := FALSE;

  { These now both point to SiteFile.AFPLoopn so this assignment is not required
  frmDeviceName.AFPLoop1 := frmLoop1.AFPLoop;
  frmDeviceName.AFPLoop2 := frmLoop2.AFPLoop;
  }
  frmDeviceName.Rebuild_List( frmLoop1.AFPLoop, frmLoop2.AFPLoop );

  { Ensure the modified flag is now cleared }////
  // Modified := false;
end;
*)

procedure TMainForm.CommentsEditChange(Sender: TObject);
begin
  SiteFile.Comments.Text := CommentsEdit.Text;
end;

procedure TMainForm.ConfigPagesChange(Sender: TObject);
begin
  BeginUpdate;
  try
    {also align the string grid for the loop summary onto row 1, col 1, which
     should stop overwriting of device 1 type. Also ensure that the very first
     thing we do is to disable changes}

    chkEnableZoneChanges.Checked := false;
    sgrdLoop.Col := 0;
    sgrdLoop.Row := 1;

    {ensure Up/Down load buttons are enabled on entry}
    btnLoadFromPanel.Enabled := TRUE;
    btnSaveToPanel.Enabled := TRUE;
    ConfigPages.HelpContext := 100;

    PanelComms.IsASCIIOnly := FALSE;
    { Ensure that the correct captions are on the load and save buttons }

    case ConfigPages.ActivePageIndex of
      0:
      begin
        { Set up load/save buttons depending upon preselected loop }
        radgrpLoopClick (Self);

        { Clear Enable zone/set/group edit check box }
        chkEnableZoneChanges.Checked := false;
        ConfigPages.HelpContext := 10;
      end;

      1:
      begin

        btnLoadFromPanel.Caption := 'Load Loop 1 From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Loop 1 To Panel ' + IntToStr(PanelNumber.Value);
        frmLoop1.UpdateTools;
        frmLoop1.FrameResize (Self);
        ConfigPages.HelpContext := 20;

        btnLoadfromPanel.Enabled := TRUE;
        btnSaveToPanel.Enabled := TRUE;
      end;

      2:
      begin

        btnLoadFromPanel.Caption := 'Load Loop 2 From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Loop 2 To Panel ' + IntToStr(PanelNumber.Value);
        frmLoop2.UpdateTools;
        frmLoop2.FrameResize (Self);
        ConfigPages.HelpContext := 20;

        if NoOfLoops = 1 then
        begin
          btnLoadfromPanel.Enabled := FALSE;
          btnSaveToPanel.Enabled := FALSE;
        end else
        begin
          btnLoadfromPanel.Enabled := TRUE;
          btnSaveToPanel.Enabled := TRUE;
        end;
      end;

      3:
      begin
        UpdateZoneDefs;
        btnLoadFromPanel.Caption := 'Load Zone Configuration From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Zone Configuration To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 30;
      end;

      4:
      begin
        btnLoadFromPanel.Caption := 'Load Group Config From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Group Config To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 40;
      end;

      5:
      begin
        btnLoadFromPanel.Caption := 'Load Set Config From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Set Config To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 50;
      end;

      6: begin
        UpdateSiteConfigPage;
        btnLoadFromPanel.Caption := 'Load Site Config From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Site Config To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 60;
      end;

      7:
      begin                    //cause & effect page
        btnLoadFromPanel.Caption := 'Load Cause && Effect table From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Cause && Effect table To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 90;
        RefreshC_EPage;
      end;

      8: begin
        btnLoadFromPanel.Caption := 'Load Network Configuration From Panel ' + IntToStr(PanelNumber.Value);
        btnSaveToPanel.Caption := 'Save Network Configuration To Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 70;
      end;

      9:
      begin { View event log }
        if not PanelComms.IsOpen then
          PanelComms.OpenComms;
        PanelComms.IsASCIIOnly := TRUE;
        PanelComms.ClearASCIIBuffer;
        btnLoadFromPanel.Caption := 'Load Event History From Panel ' + IntToStr(PanelNumber.Value);
        ConfigPages.HelpContext := 80;
      end;

    end;

    { Disable the load/save buttons on last to pages }
    btnLoadFromPanel.Visible := not (ConfigPages.ActivePageIndex = 10);
    btnSaveToPanel.Visible := not ((ConfigPages.ActivePageIndex = 9) or (ConfigPages.ActivePageIndex = 10));
    ClearLogButton.Visible := (ConfigPages.ActivePageIndex = 9);
    LoadLogButton.Visible := (ConfigPages.ActivePageIndex = 9);
    SaveLogButton.Visible := (ConfigPages.ActivePageIndex = 9);

    { Ensure that the information on the current page is refreshed and up-to-date,
    but only if the modified flag has been set }
    {if Modified then}
    if SiteFile.IsDirty then UpdateConfigData;


    //RefreshAllPages;

  finally
    EndUpdate;
  end;
end;

procedure TMainForm.txtMaintDateKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = #13 then txtMaintDateExit (Sender);
end;

procedure TMainForm.sgrdLoopKeyPress(Sender: TObject; var Key: Char);
var
   TString : string;
   Dummy : boolean;
begin
  with sgrdLoop do begin
    { If the return key is pressed}
    if (Key = chr (VK_RETURN)) then
    begin
      { If it is not the last row, go to the next row, which will automatically
      update the loop summary grid }
      if Row < (RowCount - 1) then begin
        Row := Row + 1;
        sgrdLoopSelectCell(Sender, Col, Row, Dummy);
      end else
        sgrdLoopSelectCell(Sender, Col, Row, Dummy);

      Key := #0;
    end else
    begin
      if ComboBox1.Items.Count >= MAX_DESCRIPTIONS then
      begin
        TString := 'There is a maximum of ' + IntToStr(MAX_DESCRIPTIONS) + ' different descriptions allowed';
        MessageDlg(TString, mtError, [mbYes, mbNo], 0);
      end else
      begin
      { If the character is not a non-printing character, and the maximum length for
      the description has been reached, set the character to null so it will not
      print }
        if (Length (Cells [4, Row]) >= 999) and (Key >= ' ') then begin
          Key := #0;
          MessageBeep (MB_ICONEXCLAMATION);
        end;
      end;
    end;
  end;
end;

procedure TMainForm.HelpContentsItemClick(Sender: TObject);
begin
  { Show the help file at the appropriate topic}

  Application.HelpContext (ConfigPages.HelpContext);

end;

procedure TMainForm.txtLvl3KeyPress(Sender: TObject; var Key: Char);
begin
  { If an alphanumeric has been pressed, inc punctuation, then do not display that
  character unless it is either 1, 2, 3, or 4 }
  if (Key >= ' ' ) and (Key < 'z') then
    if (Key < '1') or (Key > '4') then Key := #0;
end;

procedure TMainForm.txtLvl2Exit(Sender: TObject);
begin
  { Check to see that the level 2 code is not the same as the level 3 code }
  if SiteFile.AL2Code = SiteFile.AL3Code then begin
    MessageDlg ('Warning. Cannot have the same code as Access Level 3', mtWarning,
      [mbOK], 0);
    txtLvl2.SetFocus;
  end
  { Check that the code is 4 digits long }
  else if Length (SiteFile.AL2Code) <> 4 then begin
    MessageDlg ('Error. Access Level 2 code must have 4 digits', mtError, [mbOK], 0);
    txtLvl2.SetFocus;
  end
  { otherwise the code is OK }
  else FCodeOK := true;
end;

procedure TMainForm.txtLvl3Exit(Sender: TObject);
begin
  { Check to see that the level 2 code is not the same as the level 3 code }
  if SiteFile.AL2Code = SiteFile.AL3Code then begin
    MessageDlg ('Warning. Cannot have the same code as Access Level 2', mtWarning,
      [mbOK], 0);
    txtLvl3.SetFocus;
  end
  { Check that the code is 4 digits long }
  else if Length (SiteFile.AL3Code) <> 4 then begin
    MessageDlg ('Error. Access Level 3 code must have 4 digits', mtError, [mbOK], 0);
    txtLvl3.SetFocus;
  end
  { otherwise the code is OK }
  else FCodeOK := true;

end;

(*
procedure TMainForm.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
var
  DlgResult: integer;
begin
  { Only allow the form to close if there is no modified information }
  if Modified then begin
    { Allow user to save information if required.  }
    DlgResult := MessageDlg ('Configuration is not saved. Do you wish to save it now?', mtConfirmation,
      [mbYes, mbNo, mbCancel], 0);

    case dlgResult of
      { If yes, then treat this as if the user pressed the save button }
      mrYes: SaveIniFile (Self);
      { If cancel, do not close after all }
      mrCancel: CanClose := false;
    end;
  end;
end;
*)

procedure TMainForm.ZSounderExit(Sender: TObject);
var
  Value: integer;
begin
   Value := Round ((Sender as TCTecSpinEdit).Value * 10);
  { The zone number is stored in the tag property of the spin edit box. The
  sounder delay value for this zone is stored }
  if Value < 100 then
    SiteFile.ZoneList.Zone[(Sender as TCtecSpinEdit).Tag].SounderDelay := Value
  else
    SiteFile.ZoneList.Zone[(Sender as TCtecSpinEdit).Tag].SounderDelay := 255;

  { Set modified flag }
  // Modified := true;

end;

procedure TMainForm.PhasedDelaySpinExit(Sender: TObject);
begin
  { Acquire the integer value of the phased delay }
  SiteFile.PhasedDelay := Round (PhasedDelaySpin.Value * 10);
  { now check if > 99, and if so, then max it out at 255}
  if SiteFile.PhasedDelay > 99 then SiteFile.PhasedDelay := 255;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.ZRemoteExit(Sender: TObject);
var
  Value: integer;
begin
  { Acquire the value of this component }
  Value := Round ((Sender as TCtecSpinEdit).Value * 10);
  { The zone number is stored in the tag property of the spin edit box. The
  remote O/P delay value for this zone is stored }
  if Value < 100 then
    SiteFile.ZoneList.Zone[(Sender as TCtecSpinEdit).Tag].RemoteDelay := Value
  else
    SiteFile.ZoneList.Zone[(Sender as TCtecSpinEdit).Tag].RemoteDelay := 255;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.Disconnect1Click(Sender: TObject);
begin
  PanelComms.CloseComms;
  ShowMessage ('Disconnected');
end;

procedure TMainForm.ConfigPagesChanging(Sender: TObject;
  var AllowChange: Boolean);
begin
  { When changing from View event log page, change parity of comms port back if
  the original parity was 9 bit }
  {also align the string grid for the loop summary onto row 1, col 1, which
   should stop overwriting of device 1 type, and ensure the first thing we
   do is disable changes check box}

   sgrdLoop.Col := 0;
   sgrdLoop.Row := 1;
end;

procedure TMainForm.PanelCommsInput(Sender: TObject;
  ConvertedString: String);
var
  LogString: string;
begin
  { Only append if there is no set data type and there are no null characters in the
   converted string }
  if (FDataType = dtNone) and (Pos (#0, ConvertedString) = 0) then begin
    { If on the View Event Log page, automatically make the Rich Edit box the focus }
    if ConfigPages.ActivePage = tabViewLog then begin
      EventLog.SetFocus;
      { Acquire the string, stripping off the CR and checksum characters at the end }
      LogString := Copy (ConvertedString, 1, Length (ConvertedString) - 1);
      EventLog.Lines.Append (LogString);
    end;
  end;
end;

procedure TMainForm.PanelCommsOutputTimeout(Sender: TObject);
begin
  ShowMessage ('Timed out');
end;

procedure TMainForm.txtLvl2Enter(Sender: TObject);
begin
  { Assume that the code is not OK. This will become true once it has been checked }
  FCodeOK := false;
end;

{
procedure TMainForm.frmLoop1dwgLoopClick(Sender: TObject);
begin
  if not frmLoop1.EditPoint then Modified := true;
end;

procedure TMainForm.frmLoop2dwgLoopClick(Sender: TObject);
begin
  if not frmLoop2.EditPoint then Modified := true;
end;
}

procedure TMainForm.chkEnableZoneChangesClick(Sender: TObject);
begin

    sgrdZone.Options := sgrdZone.Options - [goEditing];
    if ChkEnableZoneChanges.Checked then sgrdZone.Options := sgrdZone.Options + [goEditing];
end;

procedure TMainForm.stxtMainDblClick(Sender: TObject);
begin
//  Data.MainVersion := '07A99';
//  UpdateSiteConfigPage;
end;


procedure TMainForm.DrawGrid1SelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
begin
  { Store the value of the sounder selected in the relevant place }
  { Store the value of the sounder selected in the relevant place }
  if SetSelected = -1 then exit;

  case frmLoop1.AFPLoop.SystemType of
    //stApolloLoop: ;
    stHochikiLoop: ;
    //stSysSensorLoop: ;
    //stNittanLoop: ;
    stCASTLoop: ;
    else
    begin
      if SetSelected = 1 then
      begin
        if ACol < 21 then
        begin
          ACol := 0;
        end;
      end;
    end;
  end;
{
  if frmLoop1.AFPLoop.SystemType <> stHochikiLoop then
  begin
    if SetSelected = 1 then
      if ACol < 21 then
        ACol := 0;
  end;
}

  case ACol of
    1..5: {Sets 1 - 5}
      SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol] := SetSelected;
    { 6 is a space }
    7..11: {Sets 6-10 }
      SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 1] := SetSelected;
    { 12 is a space }
    13..17: {Sets 11 - 15}
      SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 2] := SetSelected;
    { 18 is a space }
    19..19: {Set 16}
      SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 3] := SetSelected;
    { 20 is a space }
     21..22: {Panel relay 1 & 2}
      SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 4] := SetSelected;
  end;
  { Set modified flag }//
  // Modified := true;

end;

procedure TMainForm.btnSetButtonsClick(Sender: TObject);
var
  iZone: integer;
  iSet: integer;
  SetAll: integer;
begin
  { Determine what to set all buttons as }
  SetAll := (Sender as TToolButton).Tag;
  if(Sender as TToolButton).Name = 'btnClearAllSet' then begin
    { Mark all groups in all zones as being set to the appropriate value }
    for iZone := 1 to MAX_ZONE do begin
      for iSet := 1 to MAX_ZONE_SET do begin
        SiteFile.Zonelist.Zone[iZone].ZoneSet[iSet] := SetAll;
      end;
    end; { for iZone }
    { Now update the grid appropriately }
    dwgSetsGrid.Invalidate;

    Delayed.Down := false;
    ToolButton18.Down := false;
    ToolButton19.Down := false;
    ToolButton21.Down := false;
    SetSelected := -1;
  end else
    SetSelected := SetAll;
end;

procedure TMainForm.DrawGrid1DrawCell(Sender: TObject; ACol, ARow: Integer;
  Rect: TRect; State: TGridDrawState);
var
  SetIndex: integer;
  BM: integer;
  Bitmap: TBitmap;
begin
  if ARow > MAX_ZONE then exit;

  with dwgSetsGrid.Canvas do begin
    { If it is not the first row, determine the correct offset for the column }
    if ARow = 0 then begin
      case ACol of
        1..5: TextOut (Rect.Left + 6, Rect.Top, IntToStr (ACol));
        7..11: TextOut (Rect.Left + 6, Rect.Top, IntToStr (ACol - 1));
        13..17: TextOut (Rect.Left + 6, Rect.Top, IntToStr (ACol - 2));
        19..19: TextOut (Rect.Left + 6, Rect.Top, IntToStr (ACol - 3));
        21..22: TextOut (Rect.Left + 6, Rect.Top, IntToStr (ACol - 20));

        23: TextOut (Rect.Left + 6, Rect.Top, 'Silenceable');
        6, 12, 18, 20: begin
          Brush.Color := clBtnFace;
          dwgSetsGrid.Canvas.FillRect (Rect);
          Brush.Color := clWindow;
        end;
      end;
    end
    else if (ACol = 6) or (ACol = 12) or (ACol = 18)
         or (ACol = 20) then begin
      Brush.Color := cl3dLight;
      FillRect (Rect);
      Brush.Color := clWindow;
    end
    else if ACol = 99 then begin
      Brush.Color := clBtnFace;
      FillRect (Rect);
      Brush.Color := clWindow;
    end
    else begin
      if ACol = 0 then
      begin
        if ARow <= (MAX_ZONE - MAX_PANELS) then
          TextOut (Rect.Left + 1, Rect.Top + 5, 'Zone ' + IntToStr(ARow))
        else
          TextOut (Rect.Left + 1, Rect.Top + 5, 'Panel ' + IntToStr(ARow - (MAX_ZONE - MAX_PANELS)));
      end else begin
        { Create a temporary holder for the bitmap image }
        Bitmap := TBitmap.Create;

        try
          { Determine the correct iIndex for the Set from the column number }
          case ACol of
            1..5: SetIndex := ACol;
            7..11: SetIndex := ACol - 1;
            13..17: SetIndex := ACol - 2;
            19..20: SetIndex := ACol - 3;
            21..23: SetIndex := ACol - 4;
          else
            SetIndex := 0;
          end;
          { Fill the box with the correct image for this Zone/Set }
          if ACol < 23 then begin
              BM := SiteFile.ZoneList.Zone[ARow].ZoneSet[SetIndex];
              PgmData.imglstGroupConfig.GetBitmap (BM+4, Bitmap);
          end;
          { Update the display if the column selected was not a divider}
          if (ACol <> 6) and (ACol <> 12) and (ACol <> 18)
             and (ACol <> 24) and (ACol <> 30) and (ACol <> 36)then begin
            { Show image }
            dwgSetsGrid.Canvas.Draw(Rect.Left + 8, rect.Top, BitMap);
          end;
        finally
          Bitmap.Free;
        end;
      end; { if ACol = 0 }
    end; { if ARow > 0 }
  end; { with dwgSetsGrid }

end;


procedure TMainForm.dwgSetsGridMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  ARow, ACol: integer;
  Rect: TGridRect;
begin
  if SetSelected = -1 then
    exit;

  Rect := dwgSetsGrid.Selection;

  case frmLoop1.AFPLoop.SystemType of
    //stApolloLoop: ;
    stHochikiLoop: ;
    //stSysSensorLoop: ;
    //stNittanLoop: ;
    stCASTLoop: ;
    else
    begin
      // all commented out protocls
      if SetSelected = 1 then
      begin
        if Rect.Left < 21 then
        begin
          Button := mbRight;
        end;
      end;
    end;
  end;
{
  if frmLoop1.AFPLoop.SystemType <> stHochikiLoop then
  begin
    if SetSelected = 1 then
      if Rect.Left < 21 then
        Button := mbRight;
  end;
}
    if Button = mbLeft then begin
    for ARow := Rect.Top to Rect.Bottom do begin
      for ACol := Rect.Left to Rect.Right do begin
        if (ARow > 0) then begin
          case ACol of
            1..5: SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol] := SetSelected;
            { 6 is a space }
            7..11: SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 1] := SetSelected;
            { 12 is a space }
            12..17: SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 2] := SetSelected;
            { 18 is a space }
            19..19: SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 3] := SetSelected;
            { 20 is a space }
            21..23: SiteFile.ZoneList.Zone[ARow].ZoneSet[ACol - 4] := SetSelected;
          end; {Case Col}
        end; {if ARow > 0 }
      end; { for ACol }
    end; {for ARow }
   end; { if left button was released }
   dwgSetsGrid.Invalidate;
end;

procedure TMainForm.Timer1Timer(Sender: TObject);
begin
  stxtCurrentDate.Caption := FormatDateTime('dd/mm/yyyy',Date);  // display the date on the form's caption
  stxtCurrentTime.Caption := FormatDateTime('HH:mm:ss',Time);  // display the time on the form's caption in 12 hour format

end;

procedure TMainForm.UpDown1Click(Sender: TObject; Button: TUDBtnType);
var
   Direction: integer;
   Hours: integer;
   HoursString: string;
   MinutesString: string;
begin
{ comes from the up/down component associated with the Hours field of the Night
 mode Begin time selection}

     MinutesString := Copy(txtNight_Begin.Text,4,2);
     HoursString :=  Copy(txtNight_Begin.Text,1,2);

     try
       Hours := StrToInt(HoursString);
     except
       Hours := 0;
     end;

     if Button = btNext then Direction := 1 else Direction := -1;
     Hours := Hours + (UpDown1.Increment * Direction);
     if Hours > 23 then Hours := 0;
     if Hours < 0 then Hours := 23;
     HoursString := IntToStr(Hours);
     while Length(HoursString) < 2 do HoursString := '0' + HoursString;
     txtNight_Begin.Text := HoursString + ':' + Copy(MinutesString,1,2);
end;

procedure TMainForm.UpDown2Click(Sender: TObject; Button: TUDBtnType);
var
   Direction: integer;
   Minutes: integer;
   HoursString: string;
   MinutesString: string;
begin
{ comes from the up/down component associated with the Miniutes field of the Night
 mode Begin time selection}

     MinutesString := Copy(txtNight_Begin.Text,4,2);
     HoursString :=  Copy(txtNight_Begin.Text,1,2);

     try
       Minutes := StrToInt(MinutesString);
     except
       Minutes := 0;
     end;

     if Button = btNext then Direction := 1 else Direction := -1;
     Minutes := Minutes + (UpDown2.Increment * Direction);
     if Minutes > 59 then Minutes := 0;
     if Minutes < 0 then Minutes := 59;
     MinutesString := IntToStr(Minutes);
     while Length(MinutesString) < 2 do MinutesString := '0' + MinutesString;
     txtNight_Begin.Text := HoursString + ':' + Copy(MinutesString,1,2);

end;

procedure TMainForm.UpDown3Click(Sender: TObject; Button: TUDBtnType);
var
   Direction: integer;
   Hours: integer;
   HoursString: string;
   MinutesString: string;
begin
{ comes from the up/down component associated with the Hours field of the Night
 mode End time selection}

     MinutesString := Copy(txtNight_End.Text,4,2);
     HoursString :=  Copy(txtNight_End.Text,1,2);

     try
       Hours := StrToInt(HoursString);
     except
       Hours := 0;
     end;

     if Button = btNext then Direction := 1 else Direction := -1;
     Hours := Hours + (UpDown3.Increment * Direction);
     if Hours > 23 then Hours := 0;
     if Hours < 0 then Hours := 23;
     HoursString := IntToStr(Hours);
     while Length(HoursString) < 2 do HoursString := '0' + HoursString;
     txtNight_End.Text := HoursString + ':' + Copy(MinutesString,1,2);

end;

procedure TMainForm.UpDown4Click(Sender: TObject; Button: TUDBtnType);
var
   Direction: integer;
   Minutes: integer;
   HoursString: string;
   MinutesString: string;
begin
{ comes from the up/down component associated with the Miniutes field of the Night
 mode End time selection}

     MinutesString := Copy(txtNight_End.Text,4,2);
     HoursString :=  Copy(txtNight_End.Text,1,2);

     try
       Minutes := StrToInt(MinutesString);
     except
       Minutes := 0;
     end;

     if Button = btNext then Direction := 1 else Direction := -1;
     Minutes := Minutes + (UpDown4.Increment * Direction);
     if Minutes > 59 then Minutes := 0;
     if Minutes < 0 then Minutes := 59;
     MinutesString := IntToStr(Minutes);
     while Length(MinutesString) < 2 do MinutesString := '0' + MinutesString;
     txtNight_End.Text := HoursString + ':' + Copy(MinutesString,1,2);
end;


procedure TMainForm.updReCalClick(Sender: TObject; Button: TUDBtnType);
var
   Direction: integer;
   Hours: integer;
   HoursString: string;
   MinutesString: string;
begin
{ comes from the up/down component associated with the Hour field of the ReCal time selection}

     MinutesString := Copy(txtReCal.Text,4,2);
     HoursString :=  Copy(txtReCal.Text,1,2);

     try
       Hours := StrToInt(HoursString);
     except
       Hours := 0;
     end;

     if Button = btNext then Direction := 1 else Direction := -1;
     Hours := Hours + (updReCal.Increment * Direction);
     if Hours > 23 then Hours := 0;
     if Hours < 0 then Hours := 23;
     HoursString := IntToStr(Hours);
     while Length(HoursString) < 2 do HoursString := '0' + HoursString;
     txtReCal.Text := HoursString + ':' + Copy(MinutesString,1,2);
end;



procedure TMainForm.CreateZonePage;
var
  i : integer;
  tmpEditBox : TEdit;
  tmpLabel : TLabel;
  tmpSpnEdit : TCTecSpinEdit;
  tmpCheckBox : TCheckBox;

  VerticalSpacing : integer;
  TopMargin : integer;
begin

     TopMargin := txtZPZone1.Top;
     VerticalSpacing := txtZPZone1.Height + 5;
     for i := 2 to MAX_ZONE do
     begin
       tmpEditBox := TEdit.Create( self );
       tmpEditBox.Parent := scrlZoneCfg;
       tmpEditBox.Name := 'txtZPZone'+IntToStr(i);
       tmpEditBox.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpEditBox.Left := txtZPZone1.Left;
       tmpEditBox.Width := txtZPZone1.Width;
       tmpEditBox.MaxLength := txtZPZone1.MaxLength;
       tmpEditBox.Tag := i;
       tmpEditBox.Cursor :=txtZPZone1.Cursor;
       tmpEditBox.OnChange := txtZPZone1.OnChange;
       //tmpEditBox.Text:= '';
       tmpEditBox.Text:= SiteFile.ZoneList.Zone[ i ].Name;

       tmpLabel := TLabel.Create(self);
       tmpLabel.Parent := scrlZoneCfg;
       tmpLabel.Name := 'lblZPZone'+IntToStr(i);
       tmpLabel.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpLabel.Left := lblZPZone1.Left;
       tmpLabel.Width := lblZPZone1.Width;
       tmpLabel.Font := lblZPZone1.Font;

       if i <= (MAX_ZONE - MAX_PANELS) then
         tmpLabel.Caption := 'Zone ' + IntToStr(i)
       else
         tmpLabel.Caption := 'Panel ' + IntToStr(i - (MAX_ZONE - MAX_PANELS));

       tmpSpnEdit := TCTecSpinEdit.Create(self);
       tmpSpnEdit.Parent := scrlZoneCfg;
       tmpSpnEdit.Name := 'spnZPSounder'+IntToStr(i);
       tmpSpnEdit.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpSpnEdit.Left := spnZPSounder1.Left;
       tmpSpnEdit.Width := spnZPSounder1.Width;
       tmpSpnEdit.MinValue := spnZPSounder1.MinValue;
       tmpSpnEdit.MaxValue := spnZPSounder1.MaxValue;
       tmpSpnEdit.Increment := spnZPSounder1.Increment;
       tmpSpnEdit.Tag := i;
       tmpSpnEdit.OnChange := spnZPSounder1.OnChange;
       tmpSpnEdit.IsInfinityAllowed := false;

       tmpSpnEdit := TCTecSpinEdit.Create(self);
       tmpSpnEdit.Parent := scrlZoneCfg;
       tmpSpnEdit.Name := 'spnZPOutput'+IntToStr(i);
       tmpSpnEdit.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpSpnEdit.Left := spnZPOutput1.Left;
       tmpSpnEdit.Width := spnZPOutput1.Width;
       tmpSpnEdit.MinValue := spnZPOutput1.MinValue;
       tmpSpnEdit.MaxValue := spnZPOutput1.MaxValue;
       tmpSpnEdit.Increment := spnZPOutput1.Increment;
       tmpSpnEdit.Tag := i;
       tmpSpnEdit.OnChange := spnZPOutput1.OnChange;
       tmpSpnEdit.IsInfinityAllowed := false;

       tmpSpnEdit := TCTecSpinEdit.Create(self);
       tmpSpnEdit.Parent := scrlZoneCfg;
       tmpSpnEdit.Name := 'spnZPRelay1'+IntToStr(i);
       tmpSpnEdit.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpSpnEdit.Left := spnZPRelay11.Left;
       tmpSpnEdit.Width := spnZPRelay11.Width;
       tmpSpnEdit.MinValue := spnZPRelay11.MinValue;
       tmpSpnEdit.MaxValue := spnZPRelay11.MaxValue;
       tmpSpnEdit.Increment := spnZPRelay11.Increment;
       tmpSpnEdit.Tag := i;
       tmpSpnEdit.OnChange := spnZPRelay11.OnChange;
       tmpSpnEdit.IsInfinityAllowed := false;

       tmpSpnEdit := TCTecSpinEdit.Create(self);
       tmpSpnEdit.Parent := scrlZoneCfg;
       tmpSpnEdit.Name := 'spnZPRelay2'+IntToStr(i);
       tmpSpnEdit.Top := TopMargin + (VerticalSpacing * (i-1));
       tmpSpnEdit.Left := spnZPRelay21.Left;
       tmpSpnEdit.Width := spnZPRelay21.Width;
       tmpSpnEdit.MinValue := spnZPRelay21.MinValue;
       tmpSpnEdit.MaxValue := spnZPRelay21.MaxValue;
       tmpSpnEdit.Increment := spnZPRelay21.Increment;
       tmpSpnEdit.Tag := i;
       tmpSpnEdit.OnChange := spnZPRelay21.OnChange;
       tmpSpnEdit.IsInfinityAllowed := false;


       if i <= (MAX_ZONE - MAX_PANELS) then
       begin
         tmpEditBox := TEdit.Create(self);
         tmpEditBox.Parent := scrlZoneCfg;
         tmpEditBox.Name := 'txtZPDependancy'+IntToStr(i);
         tmpEditBox.Top := TopMargin + (VerticalSpacing * (i-1));
         tmpEditBox.Left := txtZPDependancy1.Left;
         tmpEditBox.Width := txtZPDependancy1.Width;
         tmpEditBox.OnClick := txtZPDependancy1.OnClick;
         tmpEditBox.Tag := i;
         tmpEditBox.Text := '';

         tmpCheckBox := TCheckBox.Create(self);
         tmpCheckBox.Parent := scrlZoneCfg;
         tmpCheckBox.Name := 'chkZPDetector'+IntToStr(i);
         tmpCheckBox.Top := TopMargin + (VerticalSpacing * (i-1));
         tmpCheckBox.Left := chkZPDetector1.Left;
         tmpCheckBox.Width := chkZPDetector1.Width;
         tmpCheckBox.OnClick := chkZPDetector1.OnClick;
         tmpCheckBox.Tag := i;
         tmpCheckBox.Caption := '';

         tmpCheckBox := TCheckBox.Create(self);
         tmpCheckBox.Parent := scrlZoneCfg;
         tmpCheckBox.Name := 'chkZPMCP'+IntToStr(i);
         tmpCheckBox.Top := TopMargin + (VerticalSpacing * (i-1));
         tmpCheckBox.Left := chkZPMCP1.Left;
         tmpCheckBox.Width := chkZPMCP1.Width;
         tmpCheckBox.OnClick := chkZPMCP1.OnClick;
         tmpCheckBox.Tag := i;
         tmpCheckBox.Caption := '';

         tmpCheckBox := TCheckBox.Create(self);
         tmpCheckBox.Parent := scrlZoneCfg;
         tmpCheckBox.Name := 'chkZPEndDelays'+IntToStr(i);
         tmpCheckBox.Top := TopMargin + (VerticalSpacing * (i-1));
         tmpCheckBox.Left := chkZPEndDelays1.Left;
         tmpCheckBox.Width := chkZPEndDelays1.Width;
         tmpCheckBox.OnClick := chkZPEndDelays1.OnClick;
         tmpCheckBox.Tag := i;
         tmpCheckBox.Caption := '';

       end;

       bvlZoneDesc.Height :=  tmpLabel.Top + tmpLabel.Height + 15;
       bvlFunctioningWith.Height :=  tmpLabel.Top + tmpLabel.Height + 15;

     end;



//    TEdit (FindComponent ('iAreaEqnEditBox' + IntToStr (i))).Text := s;
end;

procedure TMainForm.CreateC_EPage;
var
  i : integer;
  tmpComboBox : TComboBox;
  tmpLabel : TLabel;
  tmpMaskEdit : TMaskEdit;

  VerticalSpacing : longint;
  TopMargin : integer;
begin

  try
    TopMargin := Device_Selection1CE1.Top;
    VerticalSpacing := Device_Selection1CE1.Height + 5;

  except
    Application.MessageBox('Err 024/0', 'Form Show');
    exit;
  end;

  for i := 2 to DEFAULT_MAX_CE_EVENTS do
  begin

    try
      tmpLabel := TLabel.Create(self);
      tmpLabel.Parent := CEPanel;
      tmpLabel.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpLabel.Left := EventNo.Left;
      tmpLabel.Caption := IntToStr(i);
      tmpLabel.Font := EventNo.Font;

    except
      MessageDlg('Err 024/1.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Loop1'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Loop11.Left;
      tmpComboBox.OnChange := Loop11.OnChange;
      tmpComboBox.Items.AddStrings(Loop11.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Loop11.DropDownCount;
      tmpComboBox.Width := Loop11.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := TRUE;
      tmpComboBox.Style := csDropDownList;
      tmpComboBox.Color := clWhite;

    except
      MessageDlg('Err 024/2.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Device_Selection1CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Device_Selection1CE1.Left;
      tmpComboBox.OnChange := Device_Selection1CE1.OnChange;
      tmpComboBox.OnDblClick := Device_Selection1CE1.OnDblClick;
      tmpComboBox.Items.AddStrings(Device_Selection1CE1.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Device_Selection1CE1.DropDownCount;
      tmpComboBox.Width := Device_Selection1CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;
     except
      MessageDlg('Err 024/3.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'AND_Event1CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := AND_Event1CE1.Left;
      tmpComboBox.OnChange := AND_Event1CE1.OnChange;
      tmpComboBox.Items.AddStrings(AND_Event1CE1.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := AND_Event1CE1.DropDownCount;
      tmpComboBox.Width := AND_Event1CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;
     except
      MessageDlg('Err 024/4.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Loop2'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Loop21.Left;
      tmpComboBox.OnChange := Loop21.OnChange;
      tmpComboBox.Items.AddStrings(Loop21.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Loop21.DropDownCount;
      tmpComboBox.Width := Loop21.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;
    except
      MessageDlg('Err 024/5.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Device_Selection2CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Device_Selection2CE1.Left;
      tmpComboBox.OnChange := Device_Selection2CE1.OnChange;
      tmpComboBox.OnDblClick := Device_Selection2CE1.OnDblClick;
      tmpComboBox.Items := Device_Selection2CE1.Items;
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Device_Selection2CE1.DropDownCount;
      tmpComboBox.Width := Device_Selection2CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;
    except
      MessageDlg('Err 024/6.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try

      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Loop3'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Loop31.Left;
      tmpComboBox.OnChange := Loop31.OnChange;
      tmpComboBox.Items.AddStrings(Loop31.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Loop31.DropDownCount;
      tmpComboBox.Width := Loop31.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;

    except
      MessageDlg('Err 024/7.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'Device_Selection3CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := Device_Selection3CE1.Left;
      tmpComboBox.OnChange := Device_Selection3CE1.OnChange;
      tmpComboBox.OnDblClick := Device_Selection3CE1.OnDblClick;
      tmpComboBox.Items := Device_Selection3CE1.Items;
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := Device_Selection3CE1.DropDownCount;
      tmpComboBox.Width := Device_Selection3CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;

    except
      MessageDlg('Err 024/8.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    tmpComboBox := TComboBox.Create(self);
    try
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'AND_Event2CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := AND_Event2CE1.Left;
      tmpComboBox.OnChange := AND_Event2CE1.OnChange;
      tmpComboBox.Items.AddStrings(AND_Event2CE1.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.DropDownCount := AND_Event2CE1.DropDownCount;
      tmpComboBox.Width := AND_Event2CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;

    except
      MessageDlg('Err 024/9.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpLabel := TLabel.Create(self);
      tmpLabel.Parent := CEPanel;
      tmpLabel.Name := 'lblAND1CE'+IntToStr(i);
      tmpLabel.Top := tmpComboBox.Top + 5;
      tmpLabel.Left := lblAND1CE1.Left;
      tmpLabel.Caption := lblAND1CE1.Caption;
      tmpLabel.Font := lblAND1CE1.Font;
      tmpLabel.Visible := FALSE;

      tmpLabel := TLabel.Create(self);
      tmpLabel.Parent := CEPanel;
      tmpLabel.Name := 'lblAND2CE'+IntToStr(i);
      tmpLabel.Top := tmpComboBox.Top + 5;
      tmpLabel.Left := lblAND2CE1.Left;
      tmpLabel.Caption := lblAND2CE1.Caption;
      tmpLabel.OnClick := lblAND2CE1.OnClick;
      tmpLabel.Font := lblAND2CE1.Font;
      tmpLabel.Visible := FALSE;

      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'T_F1CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := T_F1CE1.Left;
      tmpComboBox.OnChange := T_F1CE1.OnChange;
      tmpComboBox.Items.AddStrings(T_F1CE1.Items);
      tmpComboBox.ItemIndex := 1;
      tmpComboBox.Width := T_F1CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;

      tmpComboBox := TComboBox.Create(self);
      tmpComboBox.Parent := CEPanel;
      tmpComboBox.Name := 'T_F2CE'+IntToStr(i);
      tmpComboBox.Top := TopMargin + (VerticalSpacing * (i-1));
      tmpComboBox.Left := T_F2CE1.Left;
      tmpComboBox.OnChange := T_F2CE1.OnChange;
      tmpComboBox.Items.AddStrings(T_F2CE1.Items);
      tmpComboBox.ItemIndex := 0;
      tmpComboBox.Width := T_F2CE1.Width;
      tmpComboBox.Tag := i;
      tmpComboBox.SelLength := 0;
      tmpComboBox.Visible := FALSE;
      tmpComboBox.Style := csDropDownList;


{       tmpLabel := TLabel.Create(self);
       tmpLabel.Parent := CEPanel;
       tmpLabel.Name := 'lblT_F1CE'+IntToStr(i);
       tmpLabel.Top := tmpComboBox.Top + 5;
       tmpLabel.Left := lblT_F1CE1.Left;
       tmpLabel.Caption := lblT_F1CE1.Caption;
       tmpLabel.Font := lblT_F1CE1.Font;
       tmpLabel.OnClick := lblT_F1CE1.OnClick;
       tmpLabel.Tag := i;

       tmpLabel := TLabel.Create(self);
       tmpLabel.Parent := CEPanel;
       tmpLabel.Name := 'lblT_F2CE'+IntToStr(i);
       tmpLabel.Top := tmpComboBox.Top + 5;
       tmpLabel.Left := lblT_F2CE1.Left;
       tmpLabel.Caption := lblT_F2CE1.Caption;
       tmpLabel.Font := lblT_F2CE1.Font;
       tmpLabel.OnClick := lblT_F2CE1.OnClick;
       tmpLabel.Tag := i;
}
    except
      MessageDlg('Err 024/10.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

  end;
  for i := 2 to MAX_CE_TIMER_EVENTS do
  begin

    try
      tmpMaskEdit := TMaskEdit.Create(self);
      tmpMaskEdit.Parent := Panel3;
      tmpMaskEdit.Name := 'T'+IntToStr(i);
      tmpMaskEdit.Top := T1.Top;
      tmpMaskEdit.Left := T1.Left + (T1.Width * (I-1)) + 5;
      tmpMaskEdit.Width := T1.Width;
      tmpMaskEdit.EditMask := T1.EditMask;
      tmpMaskEdit.Text := T1.Text;
      tmpMaskEdit.OnChange := T1.OnChange;
      tmpMaskEdit.Tag := i;
    except
      MessageDlg('Err 024/11.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;

    try
      tmpLabel := TLabel.Create(self);
      tmpLabel.Parent := Panel3;
      tmpLabel.Name := 'lblT'+IntToStr(i);
      tmpLabel.Top := lblT1.Top;
      tmpLabel.Left := T1.Left + (T1.Width * (I-1)) + 5;
      tmpLabel.Caption := 'T' + IntToStr(I);
      tmpLabel.Font := lblT1.Font;

    except
      MessageDlg('Err 024/12.' + IntToStr( i ), mtWarning, [mbOK], 0);
    end;


  end;
end;

procedure TMainForm.chkSetSil13Click(Sender: TObject);
var
   ZoneNo : integer;
   SetNo : integer;
begin
     ZoneNo := 0;
     SetNo := (Sender as TCheckBox).Tag;

     if (Sender as TCheckBox).Checked = TRUE then
       SiteFile.ZoneList.Zone[ZoneNo].ZoneSet[SetNo] := 1
     else
       SiteFile.ZoneList.Zone[ZoneNo].ZoneSet[SetNo] := 0;

     // Modified := TRUE;
end;

function TMainForm.RemapOldDevType (OldType: integer; LoopType: integer): integer;
begin
  case LoopType of
    0 : { Apollo Loop }
    begin
      case OldType of
        0 : Result := 0;
        1 : Result := 1;
        2 : Result := 2;
        3 : Result := 3;
        4 : Result := 4;
        5 : Result := 5;
        6 : Result := 6;
        7: Result := 11;
        12 : Result := 7;
        13 : Result := 15;
        14 : Result := 9;
        31 : Result := 10;
        16: Result := 12;
        21: Result := 14;
        29: Result := 13;
      else
        Result := 0;
      end;
    end;

    1 : { Hochiki Loop }
    begin
      case OldType of
        0 : Result := 0;
        1 : Result := 1;
        2 : Result := 2;
        3 : Result := 3;
        4 : Result := 4;
        5 : Result := 5;
        6 : Result := 6;
        7: Result := 11;
        12 : Result := 7;
        13 : Result := 15;
        14 : Result := 9;
        31 : Result := 10;
        16: Result := 12;
        21: Result := 14;
        29: Result := 13;
      else
        Result := 0;
      end;
    end;


    2: { System sensor loop }
    begin
      { No remapping required }
      Result := OldType;
    end;

    3: { Nittan loop }
    begin
      case OldType of
        0: Result := 0;
        1: Result := 1;
        2: Result := 2;
        3: Result := 3;
        4: Result := 4;
        5: Result := 5;
        6: Result := 6;
        12: Result := 8;
        31: Result := 7;
      else
        Result := 0;
      end;
    end;
  else
    Result := 0;
  end; { case LoopType }
end;


function TMainForm.RemapNewDevType(NewType, LoopType: integer): integer;
begin
  case LoopType of
    0 : { Apollo Loop }
    begin
      case NewType of
        0 : Result := 0;
        1 : Result := 1;
        2 : Result := 2;
        3 : Result := 3;
        4 : Result := 4;
        5 : Result := 5;
        6 : Result := 6;
        7 : Result := 12;
        8 : Result := 13;
        9 : Result := 14;
        10 : Result := 31;
        11: Result := 7;
        12: Result := 16;
        13: Result := 29;
        14: Result := 21;
        15: Result := 13;
      else
        Result := 0;
      end;
    end;

    1 : { Hochiki Loop }
    begin
      case NewType of
        0 : Result := 0;
        1 : Result := 1;
        2 : Result := 2;
        3 : Result := 3;
        4 : Result := 4;
        5 : Result := 5;
        6 : Result := 6;
        7 : Result := 12;
        8 : Result := 13;
        9 : Result := 14;
        10 : Result := 31;
        11: Result := 7;
        12: Result := 16;
        13: Result := 29;
        14: Result := 21;
        15: Result := 13;
      else
        Result := 0;
      end;
    end;

    2: { System sensor loop }
    begin
      { No remapping required }
      Result := NewType;
    end;

    3: { Nittan loop }
    begin
      case NewType of
        0: Result := 0;
        1: Result := 1;
        2: Result := 2;
        3: Result := 3;
        4: Result := 4;
        5: Result := 5;
        6: Result := 6;
        7: Result := 31;
        8: Result := 12;
      else
        Result := 0;
      end;
    end;
  else
    Result := 0;
  end; { case LoopType }
end;


procedure TMainForm.Loop11Change(Sender: TObject);
var
  Event_Type : integer;
  I, I1, I2 : integer;
  iComboBox : TComboBox;
begin
  I := (Sender as TComboBox).ItemIndex;
  Event_Type := I * 10;                           //Event Type
  I2 := (Sender as TCOmboBox).Tag;                //get event number

  //for newer events, popup version dependancy dialog box
//remove this dialog, it is over 10 years old now!
{
  case Event_Type  of
    190, 200, 210, 220, 230:
    begin
      if frmVersionDependancy.CheckBox1.State = cbUnChecked then
        if ConfigPages.ActivePageIndex = 7 then
            frmVersionDependancy.ShowModal;
    end;
  end;
}
  if Event_Type = 0 then
  begin
    TComboBox (FindComponent ('Device_Selection1CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Visible := FALSE;
    TLabel (FindComponent ('lblAND1CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2))).Visible := FALSE;
    TLabel (FindComponent ('lblAND2CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('T_F1CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('T_F2CE' + IntToStr(I2))).Visible := FALSE;

    for I := 1 to 6 do
    begin
      SiteFile.C_ETable.C_ETable[I2].C_EArray[I] := 0;
    end;
  end;


  iComboBox := TComboBox (FindComponent ('Device_Selection1CE' + IntToStr (I2)));
  case Event_Type of
    10, 20, 170, 180:        //Loop 1/2 device
    begin
      iComboBox.Clear;
      for I1 := 1 to frmLoop1.AFPLoop.NoLoopDevices do
      begin
        iComboBox.Items.Add(IntToStr(I1));
      end;

      iComboBox.Visible := TRUE;
    end;

    30:              //Relay trigger
    begin
      iComboBox.Clear;
      iComboBox.Items.Add('Relay 1');
      iComboBox.Items.Add('Relay 2');
      iComboBox.Items.Add('Relay 3');

      iComboBox.Visible := TRUE;
    end;

    40, 50, 160, 230:            //Sounder group Alert/Evac/Disable/Beacon trigger
    begin
      iComboBox.Clear;
      iComboBox.Items.Add('All Groups');

      for I := 1 to MAX_GROUP do
      begin
        iComboBox.Items.Add('Group ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    220:            //Network Event
    begin
      iComboBox.Clear;

      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    60, 70, 110, 120, 130, 140, 150, 190, 200:
    //Panel silence / Reset, Day, Abstract, Mute, Night mode, Output delay disable,
    //Override Phased evac, Override Zone Delays, Evacuate building
    begin
      iComboBox.Visible := FALSE;
      TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Visible := TRUE;
    end;

    80, 210:         //Zone disable, Put zone into Alarm
    begin;
      iComboBox.Clear;
      for I := 1 to MAX_LOCAL_ZONES do
      begin
        iComboBox.Items.Add('Zone ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    90:         //Set disable
    begin;
      iComboBox.Clear;
      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Set ' + IntToStr(I));
      end;

      iComboBox.Items.Add('Relay 1');
      iComboBox.Items.Add('Relay 2');
      iComboBox.Items.Add('Relay 3');

      iComboBox.Visible := TRUE;
    end;

    100:         //Output set trigger
    begin;
      iComboBox.Clear;
      for I := 1 to MAX_ZONE_SET-2 do
      begin
        iComboBox.Items.Add('Set ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

  end;

  SiteFile.C_ETable.C_ETable[I2].C_EArray[1] := Event_Type;

     // Modified := TRUE;
end;

procedure TMainForm.Loop11CloseUp(Sender: TObject);
var
   Event_Type : integer;
   I : integer;
begin
     I := (Sender as TComboBox).ItemIndex;
     Event_Type := I * 10;                           //Event Type
     //I2 := (Sender as TCOmboBox).Tag;                //get event number

     //for newer events, popup version dependancy dialog box
//remove this dialog, it is over 10 years old now!
{
     case Event_Type  of
       190, 200, 210, 220:
       begin
         if frmVersionDependancy.CheckBox1.State = cbUnChecked then
           if ConfigPages.ActivePageIndex = 7 then
             frmVersionDependancy.ShowModal;
        end;
     end;
}
end;

procedure TMainForm.Device_Selection1CE1Change(Sender: TObject);
var
  I2 : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number

   (FindComponent ('Loop2' + IntToStr(I2)) as TComboBox).Visible := TRUE;

  SiteFile.C_ETable.C_ETable[I2].C_EArray[2] :=  (Sender as TComboBox).ItemIndex;
     // Modified := TRUE;
end;


procedure TMainForm.Loop21DropDown(Sender: TObject);
var
  //Event_Type : integer;
  {I,} I2 : integer;
begin
  //I := (Sender as TComboBox).ItemIndex + 1;
  //Event_Type := I * 5;                            //Event Type
  I2 := (Sender as TComboBox).Tag;                //get event number

  TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Width := 142;
end;


procedure TMainForm.Loop21Change(Sender: TObject);
var
  Event_Type : integer;
  I, I1, I2 : integer;
  iComboBox, iComboBox2 : TComboBox;
begin
  I := (Sender as TComboBox).ItemIndex + 1;
  Event_Type := I * 5;                            //Event Type
  I2 := (Sender as TComboBox).Tag;                //get event number

  TComboBox (FindComponent ('T_F1CE' + IntToStr(I2))).Visible := TRUE;

  //for newer events, popup version dependancy dialog box
//remove this dialog, it is over 10 years old now!
{
  case Event_Type  of
    105, 110, 115, 120:
    begin
      if frmVersionDependancy.CheckBox1.State = cbUnChecked then
      begin
        if ConfigPages.ActivePageIndex = 7 then
        begin
          frmVersionDependancy.ShowModal;
        end;
      end;
    end;
  end;
}


  if (Event_Type = 65) Or (Event_Type = 125) then
  begin
    TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Width := 55;
    TLabel (FindComponent ('lblAND1CE' + IntToStr(I2))).Visible := TRUE;
    TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).Visible := TRUE;
  end
  else
  begin
    TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Width := 142;
    TLabel (FindComponent ('lblAND1CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).Visible := FALSE;
  end;

  iComboBox := TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2)));
  iComboBox2 := TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2)));
  case Event_Type of
    5,10, 40, 45:     //Loop 1/2 device
    begin
      iComboBox.Clear;
      for I1 := 1 to frmLoop1.AFPLoop.NoLoopDevices do
      begin
        iComboBox.Items.Add(IntToStr(I1));
      end;

      iComboBox.Visible := TRUE;
    end;

    15, 85:         //Zone in fire, Zone not normal
    begin;
      iComboBox.Clear;
      for I := 1 to MAX_LOCAL_ZONES do
      begin
        iComboBox.Items.Add('Zone ' + IntToStr(I));
      end;
      for I := 1 to MAX_PANELS do
      begin
        iComboBox.Items.Add('Panel ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    20, 25, 30, 35, 50, 70, 75, 90, 95, 100, 105, 110, 120:
    begin
      SiteFile.C_ETable.C_ETable[I2].C_EArray[4] := 0;
      iComboBox.Visible := FALSE;
      TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Visible := TRUE;
    end;

    55:         //Panel input asserted
    begin
      iComboBox.Clear;
      iComboBox.Items.Add('Input 1');
      iComboBox.Items.Add('Input 2');

      iComboBox.Visible := TRUE;
    end;

    60:         //Other event
    begin;
      iComboBox.Clear;
      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    115:       //Network Event
    begin
      iComboBox.Clear;
      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    65:         //Event AND Event
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to pgmData.MAX_CE_EVENTS do
      begin
        iComboBox2.Items.Add(IntToStr(I));
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;
    end;

    125:         //Zone AND Zone
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to MAX_LOCAL_ZONES do
      begin
        iComboBox2.Items.Add(IntToStr(I));
        iComboBox.Items.Add('Zone ' + IntToStr(I));
      end;
    end;

    80:         //Timer Event
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Time ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

  end;

  if TComboBox( FindComponent ('T_F1CE' + IntToStr (I2))).ItemIndex = 0 then
  begin
    Event_Type := Event_Type or 128;
  end;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[3] := Event_Type;


end;

procedure TMainForm.Device_Selection2CE1Change(Sender: TObject);
var
  I2 : integer;
  iVal : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number

  (FindComponent ('Loop3' + IntToStr(I2)) as TComboBox).Visible := TRUE;

  //SiteFile.C_ETable.C_ETable[I2].C_EArray[4] := 0;
  if (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 65) Or
     (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 125) then
  begin
    //SiteFile.C_ETable.C_ETable[I2].C_EArray[4] := (TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).ItemIndex) * 256;
    iVal := (TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).ItemIndex) * 256;
  end
  else
  begin
    iVal := 0;
  end;

  //SiteFile.C_ETable.C_ETable[I2].C_EArray[4] :=  SiteFile.C_ETable.C_ETable[I2].C_EArray[4] +
  //                                         TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))).ItemIndex;
  iVal :=  iVal + (FindComponent ('Device_Selection2CE' + IntToStr(I2)) as TComboBox).ItemIndex;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[4] :=  iVal;
end;

procedure TMainForm.Loop31DropDown(Sender: TObject);
var
  //Event_Type : integer;
  {I,} I2 : integer;
begin
  //I := (Sender as TComboBox).ItemIndex + 1;
  //Event_Type := I * 5;                            //Event Type
  I2 := (Sender as TComboBox).Tag;                //get event number

  TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Width := 142;
end;


procedure TMainForm.Loop31Change(Sender: TObject);
var
  Event_Type : integer;
  I, I1, I2 : integer;
  iComboBox, iComboBox2 : TComboBox;
begin
  I := (Sender as TComboBox).ItemIndex + 1;
  Event_Type := I * 5;                            //Event Type
  I2 := (Sender as TComboBox).Tag;                //get event number

  TComboBox (FindComponent ('T_F2CE' + IntToStr(I2))).Visible := TRUE;

  //for newer events, popup version dependancy dialog box
//remove this dialog, it is over 10 years old now!
{  case Event_Type  of
    105, 110, 115, 120:
    begin
      if frmVersionDependancy.CheckBox1.State = cbUnChecked then
      begin
        if ConfigPages.ActivePageIndex = 7 then
        begin
          frmVersionDependancy.ShowModal;
        end;
      end;
    end;
  end;
}
  if (Event_Type = 65) Or (Event_Type = 125) then
  begin
    TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Width := 55;
    TLabel (FindComponent ('lblAND2CE' + IntToStr(I2))).Visible := TRUE;
    TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2))).Visible := TRUE;
  end
  else
  begin
    TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Width := 142;
    TLabel (FindComponent ('lblAND2CE' + IntToStr(I2))).Visible := FALSE;
    TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2))).Visible := FALSE;
  end;

  iComboBox := TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2)));
  iComboBox2 := TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2)));
  case Event_Type of
    5, 10, 40, 45:     //Loop 1/2 device
    begin
      iComboBox.Clear;
      for I1 := 1 to frmLoop1.AFPLoop.NoLoopDevices do
      begin
        iComboBox.Items.Add(IntToStr(I1));
      end;

      iComboBox.Visible := TRUE;
    end;

    15, 85:         //Zone in fire, zone not normal
    begin;
      iComboBox.Clear;
      for I := 1 to MAX_LOCAL_ZONES do
      begin
        iComboBox.Items.Add('Zone ' + IntToStr(I));
      end;
      for I := 1 to MAX_PANELS do
      begin
        iComboBox.Items.Add('Panel ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    20, 25, 30, 35, 50, 70, 75, 90, 95, 100, 105, 110, 120:
    begin
      SiteFile.C_ETable.C_ETable[I2].C_EArray[6] := 0;
      iComboBox.Visible := FALSE;
    end;

    55:         //Panel input asserted
    begin
      iComboBox.Clear;
      iComboBox.Items.Add('Input 1');
      iComboBox.Items.Add('Input 2');

      iComboBox.Visible := TRUE;
    end;

    60:         //Other event
    begin;
      iComboBox.Clear;
      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    115:       //Network Event
    begin
      iComboBox.Clear;
      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

    65:         //Event AND Event
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to pgmData.MAX_CE_EVENTS do
      begin
        iComboBox2.Items.Add(IntToStr(I));
        iComboBox.Items.Add('Event ' + IntToStr(I));
      end;
    end;

    125:         //Zone AND Zone
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to MAX_LOCAL_ZONES do
      begin
        iComboBox2.Items.Add(IntToStr(I));
        iComboBox.Items.Add('Zone ' + IntToStr(I));
      end;
    end;

    80:         //Timer Event
    begin
      iComboBox2.Clear;
      iComboBox.Clear;

      for I := 1 to 16 do
      begin
        iComboBox.Items.Add('Time ' + IntToStr(I));
      end;

      iComboBox.Visible := TRUE;
    end;

  end;

  if TComboBox( FindComponent ('T_F2CE' + IntToStr (I2))).ItemIndex = 1 then
  begin
    Event_Type := Event_Type or 128;
  end;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[5] := Event_Type;

end;

procedure TMainForm.Device_Selection3CE1Change(Sender: TObject);
var
  I2 : integer;
  iVal : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number

  //SiteFile.C_ETable.C_ETable[I2].C_EArray[6] := 0;
  if (SiteFile.C_ETable.C_ETable[I2].C_EArray[5] = 65) Or
     (SiteFile.C_ETable.C_ETable[I2].C_EArray[5] = 125) then
  begin
    iVal := ( (FindComponent ('AND_Event2CE' + IntToStr(I2)) as TComboBox).ItemIndex) *256 ;
  end
  else
  begin
    iVal := 0;
  end;

  iVal :=  iVal + TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2))).ItemIndex;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[6] := iVal;

end;

procedure TMainForm.AND_Event1CE1Change(Sender: TObject);
var
  I2 : integer;
  iVal : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number
  TComboBox (FindComponent ('Device_Selection2CE' + IntToStr (I2))).Visible := TRUE;

  //SiteFile.C_ETable.C_ETable[I2].C_EArray[4] := SiteFile.C_ETable.C_ETable[I2].C_EArray[4] and 255;
  iVal := SiteFile.C_ETable.C_ETable[I2].C_EArray[4] and 255;

  iVal := iVal + (TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).ItemIndex) * 256;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[4] := iVal;
end;

procedure TMainForm.AND_Event2CE1Change(Sender: TObject);
var
  I2 : integer;
  iVal : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number
  (FindComponent ('Device_Selection3CE' + IntToStr (I2)) as TComboBox).Visible := TRUE;

  //SiteFile.C_ETable.C_ETable[I2].C_EArray[6] := SiteFile.C_ETable.C_ETable[I2].C_EArray[6] and 255;
  iVal := SiteFile.C_ETable.C_ETable[I2].C_EArray[6] and 255;

  iVal := iVal + ((FindComponent ('AND_Event2CE' + IntToStr(I2)) as TComboBox).ItemIndex) * 256;
  SiteFile.C_ETable.C_ETable[I2].C_EArray[6] := iVal;

end;

procedure TMainForm.lblT_F1CE1Click(Sender: TObject);
var
  I2 : integer;
begin
  I2 := (Sender as TComboBox).Tag;                //get event number
  {   T_F := TLabel (FindComponent ('lblT_F1CE' + IntToStr(I2))).Caption;
   if T_F = '= TRUE' then
     T_F := '= FALSE'
   else
     T_F := '= TRUE';

   TLabel (FindComponent ('lblT_F1CE' + IntToStr(I2))).Caption := T_F;
   if T_F = '= FALSE' then
  }
  if TComboBox (FindComponent ('T_F1CE' + IntToStr(I2))).ItemIndex = 0 then
  begin
    SiteFile.C_ETable.C_ETable[I2].C_EArray[3] := SiteFile.C_ETable.C_ETable[I2].C_EArray[3] or 128;
  end
  else
  begin
     SiteFile.C_ETable.C_ETable[I2].C_EArray[3] := SiteFile.C_ETable.C_ETable[I2].C_EArray[3] and 127;
  end;
end;

procedure TMainForm.lblT_F2CE1Click(Sender: TObject);
var
     I2 : integer;
begin
   I2 := (Sender as TComboBox).Tag;                //get event number
{   T_F := TLabel (FindComponent ('lblT_F2CE' + IntToStr(I2))).Caption;
   if T_F = '= TRUE' then
     T_F := '= FALSE'
   else
     T_F := '= TRUE';

   TLabel (FindComponent ('lblT_F2CE' + IntToStr(I2))).Caption := T_F;
}
   if TComboBox (FindComponent ('T_F2CE' + IntToStr(I2))).ItemIndex = 1 then
     SiteFile.C_ETable.C_ETable[I2].C_EArray[5] := SiteFile.C_ETable.C_ETable[I2].C_EArray[5] or 128
   else
     SiteFile.C_ETable.C_ETable[I2].C_EArray[5] := SiteFile.C_ETable.C_ETable[I2].C_EArray[5] and 127;

     // Modified := TRUE;
end;

procedure TMainForm.ShowEventArray;
var
   i : integer;
begin
     i := ComboBox1.ItemIndex + 1;
     if i > 0 then
     begin
       Edit1.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[1]);
       Edit2.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[2]);
       Edit3.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[3]);
       Edit4.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[4]);
       Edit5.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[5]);
       Edit6.Text := IntToStr(SiteFile.C_ETable.C_ETable[i].C_EArray[6]);
     end;
end;

procedure TMainForm.Timer2Timer(Sender: TObject);
begin
     ShowEventArray;
     Edit7.Text := IntToStr(frmDeviceName.ComboBox1.Items.Count) + '/256 Names used';
     Edit8.Text := IntToStr(frmDeviceName.NoOfChars) + ' Total Characters';
     Edit9.Text := IntToStr(8192 - frmDeviceName.NoOfChars) + ' Characters Left';
end;


procedure TMainForm.RefreshControls;
var
  iIndex{, I1}: integer;
begin
(*
procedure TMainForm.ClearInfo;
  ZoneIndex: integer;
  Year, Month, Day: Word;
begin




end;
*)
  btnDelayTime.Value := SiteFile.DelayTimer;

  for iIndex := 1 to MAX_CE_TIMER_EVENTS do
  begin
    TMaskEdit(FindComponent ('T' + IntToStr(iIndex))).Text := SiteFile.C_ETable.Time_Events[ iIndex ];
  end;

  CommentsEdit.Text := SiteFile.Comments.Text;

  { Ensure that the following pages are correctly updated? }
  RefreshAllPages;

  Int_Tone_Box.ItemIndex := SiteFile.IntermittentTone;
  Cont_Tone_Box.ItemIndex := SiteFile.ContinuousTone;

  chkReSound.Checked := SiteFile.ReSound_Function;

  SendToPanel.Checked := FALSE;

  frmDeviceName.Rebuild_List( SiteFile.AFPLoop1, SiteFile.AFPLoop2 );
end;

procedure TMainForm.RefreshC_EPage;
var
   I, I2 : integer;
   iComboBox : TComboBox;
begin
  BeginUpdate;
  try
    for I2 := 1 to PgmData.MAX_CE_EVENTS do
    begin
      I := SiteFile.C_ETable.C_ETable[I2].C_EArray[1];
      I := (I div 10);
      iComboBox := TComboBox (FindComponent ('Loop1' + IntToStr(I2)));
      iComboBox.ItemIndex := I;
      Loop11Change( iComboBox );

      if I > 0 then
      begin
      //  iComboBox := TComboBox (FindComponent ('Loop2' + IntToStr(I2)));
      //  SiteFile.C_ETable.C_ETable[I2].C_EArray[2] :=  iComboBox.ItemIndex;
        I := SiteFile.C_ETable.C_ETable[I2].C_EArray[2];
        iComboBox := TComboBox (FindComponent ('Device_Selection1CE' + IntToStr(I2)));
        iComboBox.ItemIndex := I;
        Device_Selection1CE1Change(iComboBox);

        iComboBox := TComboBox (FindComponent ('T_F1CE' + IntToStr(I2)));
        I := SiteFile.C_ETable.C_ETable[I2].C_EArray[3];
        if I > 127 then
        begin
          iComboBox.ItemIndex := 0;
        end
        else
        begin
          iComboBox .ItemIndex := 1;
        end;

        iComboBox := TComboBox (FindComponent ('T_F2CE' + IntToStr(I2)));
        I := SiteFile.C_ETable.C_ETable[I2].C_EArray[5];
        if I > 127 then
        begin
          iComboBox .ItemIndex := 1;
        end
        else
        begin
          iComboBox .ItemIndex := 0;
        end;


        I := SiteFile.C_ETable.C_ETable[I2].C_EArray[3] and 127;
        I := I - 1;
        I := (I div 5);
        iComboBox := TComboBox (FindComponent ('Loop2' + IntToStr(I2)));
        iComboBox.ItemIndex := I;
        Loop21Change(iComboBox);

        if (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 65) Or
           (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 125) then
        begin
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[4] div 256;
          iComboBox := TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2)));
          iComboBox.ItemIndex := I;
          AND_Event1CE1Change(iComboBox);
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[4] and 255;
        end
        else
        begin
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[4];
        end;

        iComboBox := TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2)));
        iComboBox.ItemIndex := I;
        Device_Selection2CE1Change(iComboBox);

        I := SiteFile.C_ETable.C_ETable[I2].C_EArray[5] and 127;
        I := I - 1;
        I := (I div 5);
        iComboBox := TComboBox (FindComponent ('Loop3' + IntToStr(I2)));
        iComboBox.ItemIndex := I;
        Loop31Change(iComboBox);

        if (SiteFile.C_ETable.C_ETable[I2].C_EArray[5] = 65) Or
           (SiteFile.C_ETable.C_ETable[I2].C_EArray[5] = 125) then
        begin
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[6] div 256;
          iComboBox := TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2)));
          iComboBox.ItemIndex := I;
          AND_Event2CE1Change(iComboBox);
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[6] and 255;
        end
        else
        begin
          I := SiteFile.C_ETable.C_ETable[I2].C_EArray[6];
        end;

        IComboBox := TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2)));
        iComboBox.ItemIndex := I;
        Device_Selection3CE1Change(iComboBox);
      end;
    end;

    I2 := PgmData.MAX_CE_EVENTS + 1;

    while I2 <= DEFAULT_MAX_CE_EVENTS do
    begin
      TComboBox (FindComponent ('Loop1' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('Device_Selection1CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('T_F1CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('T_F2CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('Loop2' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('AND_Event1CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('Loop3' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('AND_Event2CE' + IntToStr(I2))).Visible := FALSE;
      TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2))).Visible := FALSE;

      I2 := I2 + 1;
    end;

    { now update the time event markers }
    for I2 := 1 to MAX_CE_TIMER_EVENTS do
    begin
      TMaskEdit(FindComponent ('T' + IntToStr(I2))).Text := SiteFile.C_ETable.Time_Events[I2];
    end;

  finally
    EndUpdate;
  end;
end;

procedure TMainForm.RefreshProtocolControls;
var
  CurrentPage: integer;
begin

  CurrentPage := ConfigPages.ActivePageIndex;

  Device_Selection_Frame1.Protocol := SiteFile.SystemType;

  if SiteFile.SystemType = stHochikiLoop then
  begin
    Int_Tone_Box.Visible := TRUE;
    Cont_Tone_Box.Visible := TRUE;
    Int_Tone_Box_Apollo.Visible := FALSE;
    Cont_Tone_Box_Apollo.Visible := FALSE;
    Int_Tone_Box_CAST.Visible := FALSE;
    Cont_Tone_Box_CAST.Visible := FALSE;
    Int_Label.Visible := TRUE;
    Cont_Label.Visible := TRUE;

    Int_Tone_Box.ItemIndex := SiteFile.IntermittentTone;
    Cont_Tone_Box.ItemIndex := SiteFile.ContinuousTone;

    DeviceConfig.Apollo_Detector_Config_Frame1.chkBase_Sounder.Caption := 'Has Addr. Base Sounder';
  end
  else if SiteFile.SystemType = stApolloLoop then
  begin
    Int_Tone_Box.Visible := FALSE;
    Cont_Tone_Box.Visible := FALSE;
    Int_Tone_Box_Apollo.Visible := TRUE;
    Cont_Tone_Box_Apollo.Visible := TRUE;
    Int_Tone_Box_CAST.Visible := FALSE;
    Cont_Tone_Box_CAST.Visible := FALSE;
    Int_Label.Visible := TRUE;
    Cont_Label.Visible := TRUE;

    Int_Tone_Box_Apollo.ItemIndex := SiteFile.IntermittentTone;
    Cont_Tone_Box_Apollo.ItemIndex := SiteFile.ContinuousTone;

    DeviceConfig.Apollo_Detector_Config_Frame1.chkBase_Sounder.Caption := 'Has Ancillary Base Sounder';
  end
  else
  begin
    if CAST_Protocol_Available = TRUE then //JG CAST Panel Lockout
    begin
      Int_Tone_Box.Visible := FALSE;
      Cont_Tone_Box.Visible := FALSE;
      Int_Tone_Box_Apollo.Visible := FALSE;
      Cont_Tone_Box_Apollo.Visible := FALSE;
      Int_Tone_Box_CAST.Visible := TRUE;
      Cont_Tone_Box_CAST.Visible := TRUE;
      Int_Label.Visible := TRUE;
      Cont_Label.Visible := TRUE;
      Int_Tone_Box_CAST.ItemIndex := SiteFile.IntermittentTone;
      Cont_Tone_Box_CAST.ItemIndex := SiteFile.ContinuousTone;
    end
    else
    begin
      MessageDlg('Panel Protocol unavailable', mtWarning, [mbOk], 0);
      SiteFile.SystemType := stApolloLoop;
      SiteFile.IsDirty := FALSE;
      Sitefile.New;
    end;
  end;

  { Now bring up a new set up using the new protocol }
  RefreshControls;
  // ClearInfo;
  // ClearInfo;
  ConfigPages.ActivePageIndex := CurrentPage;
end;

procedure TMainForm.Device_Selection1CE1DblClick(Sender: TObject);
var
   I : integer;
   I2 : integer;
begin

{double clicking on the 1st column for loop device. This is the 'what happens' column}
     I2 := (Sender as TComboBox).Tag;                //get event number
     if (SiteFile.C_ETable.C_ETable[I2].C_EArray[1] = 10) or
        (SiteFile.C_ETable.C_ETable[I2].C_EArray[1] = 20) then
     begin
       if SiteFile.C_ETable.C_ETable[I2].C_EArray[1] = 10 then
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop1.AFPLoop
       else
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop2.AFPLoop;

       frmChoose_Device.frmLoop1.UpdateTools;
       frmChoose_Device.ShowModal;

       I := frmChoose_Device.DevIndex;
       TComboBox (FindComponent ('Device_Selection1CE' + IntToStr(I2))).ItemIndex := I-1;
       Device_Selection1CE1Change(TComboBox (FindComponent ('Device_Selection1CE' + IntToStr(I2))));
     end;
end;

procedure TMainForm.Device_Selection2CE1DblClick(Sender: TObject);
var
   I : integer;
   I2 : integer;
begin
{double clicking on the 2nd column for loop device. This is the event trigger columnn}

     I2 := (Sender as TComboBox).Tag;                //get event number
     if (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 5) or
        (SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 10) then
     begin
       if SiteFile.C_ETable.C_ETable[I2].C_EArray[3] = 5 then
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop1.AFPLoop
       else
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop2.AFPLoop;

       frmChoose_Device.frmLoop1.UpdateTools;
       frmChoose_Device.ShowModal;

       I := frmChoose_Device.DevIndex;
       TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))).ItemIndex := I-1;
       Device_Selection2CE1Change(TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))));
     end;
end;

procedure TMainForm.Device_Selection3CE1DblClick(Sender: TObject);
var
   I : integer;
   I2 : integer;
begin
{double clicking on the 3rd column for loop device. This is the event reset columnn}
     I2 := (Sender as TComboBox).Tag;                //get event number
     if (C_ETable.C_ETable[I2].C_EArray[5] = 5) or
        (C_ETable.C_ETable[I2].C_EArray[5] = 10) then
     begin
       if C_ETable.C_ETable[I2].C_EArray[5] = 5 then
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop1.AFPLoop
       else
         frmChoose_Device.frmLoop1.AFPLoop := frmLoop2.AFPLoop;

       frmChoose_Device.frmLoop1.UpdateTools;
       frmChoose_Device.ShowModal;

       I := frmChoose_Device.DevIndex;
       TComboBox (FindComponent ('Device_Selection3CE' + IntToStr(I2))).ItemIndex := I-1;
       Device_Selection3CE1Change(TComboBox (FindComponent ('Device_Selection2CE' + IntToStr(I2))));
     end;
end;

procedure TMainForm.T1Change(Sender: TObject);
var
   I2 : integer;
begin
     I2 := (Sender as TMaskEdit).Tag;                //get event number

     C_ETable.Time_Events[I2] := (Sender as TMaskEdit).Text;
     // Modified := TRUE;
end;

//end of the special cause & effect processing

procedure TMainForm.txtNight_BeginChange(Sender: TObject);
begin
     SiteFile.NightBegin := txtNight_Begin.Text;
     // Modified := TRUE;
end;

procedure TMainForm.txtNight_EndChange(Sender: TObject);
begin
     SiteFile.NightEnd := txtNight_End.Text;
     // Modified := TRUE;
end;

procedure TMainForm.txtReCalChange(Sender: TObject);
begin
     SiteFile.RecalTime := txtRecal.Text;
     // Modified := TRUE;
end;

procedure TMainForm.spnZPSounder1Change(Sender: TObject);
var
   Zone : integer;
begin
     Zone := (Sender as TTimeSpinButton).Tag;
     SiteFile.ZoneList.Zone[Zone].SounderDelay := (Sender as TTimeSpinButton).Value;

     if (SiteFile.ZoneList.Zone[Zone].Dependancy[1] = 5) or
        (SiteFile.ZoneList.Zone[Zone].Dependancy[4] = 5) then
     begin
       if SiteFile.InvestigationPeriod +
         SiteFile.InvestigationPeriod1 +
         SiteFile.ZoneList.Zone[Zone].SounderDelay > Max_OutputDelay then
       begin
          TTimeSpinButton (FindComponent ('spnZPSounder' + IntToStr (Zone))).Font.Color := clRed;
         if frmDelay_Time_Prompt.CheckBox1.State = cbUnChecked then
           frmDelay_Time_Prompt.ShowModal;
       end else
         TTimeSpinButton (FindComponent ('spnZPSounder' + IntToStr (Zone))).Font.Color := clBlack;
     end;
      //Modified := TRUE;
end;

procedure TMainForm.spnZPOutput1Change(Sender: TObject);
var
   Zone : integer;
begin
     Zone := (Sender as TTimeSpinButton).Tag;
     SiteFile.ZoneList.Zone[Zone].RemoteDelay := (Sender as TTimeSpinButton).Value;

     if (SiteFile.ZoneList.Zone[Zone].Dependancy[1] = 5) or
        (SiteFile.ZoneList.Zone[Zone].Dependancy[4] = 5) then
     begin
       if SiteFile.InvestigationPeriod +
         SiteFile.InvestigationPeriod1 +
         SiteFile.ZoneList.Zone[Zone].RemoteDelay > Max_OutputDelay then
       begin
          TTimeSpinButton (FindComponent ('spnZPOutput' + IntToStr (Zone))).Font.Color := clRed;
         if frmDelay_Time_Prompt.CheckBox1.State = cbUnChecked then
           frmDelay_Time_Prompt.ShowModal;
       end else
         TTimeSpinButton (FindComponent ('spnZPOutput' + IntToStr (Zone))).Font.Color := clBlack;
     end;

     // Modified := TRUE;
end;

procedure TMainForm.spnZPRelay11Change(Sender: TObject);
var
   Zone : integer;
begin
     Zone := (Sender as TTimeSpinButton).Tag;
     SiteFile.ZoneList.Zone[Zone].Relay1Delay := (Sender as TTimeSpinButton).Value;
     // Modified := TRUE;
end;

procedure TMainForm.spnZPRelay21Change(Sender: TObject);
var
   Zone : integer;
begin
     Zone := (Sender as TTimeSpinButton).Tag;
     SiteFile.ZoneList.Zone[Zone].Relay2Delay := (Sender as TTimeSpinButton).Value;
     // Modified := TRUE;
end;

procedure TMainForm.spnPanelSounder1Change(Sender: TObject);
begin
     SiteFile.PanelSounder1_Group := spnPanelSounder1.Value;
     // Modified := TRUE;
end;

procedure TMainForm.spnPanelSounder2Change(Sender: TObject);
begin
     SiteFile.PanelSounder2_Group := spnPanelSounder2.Value;
     // Modified := TRUE;
end;

procedure TMainForm.PanelNumberChange(Sender: TObject);
begin
  SiteFile.PanelNumber := PanelNumber.Value;

  PanelComms.PanelNumber := PanelNumber.Value;
  ConfigPagesChange(self);
     //Modified := TRUE;
end;

procedure TMainForm.CheckBox1Click(Sender: TObject);
var
  Panel_No : integer;
begin
     Panel_No := (sender as TCheckBox).Tag;
     SiteFile.XFP_Network.PAccept_Faults[Panel_No] := (sender as TCheckBox).Checked;
     // Modified := TRUE;
end;

procedure TMainForm.CheckBox9Click(Sender: TObject);
var
  Panel_No : integer;
begin
     Panel_No := (sender as TCheckBox).Tag;
     XFP_Network.PAccept_Alarms[Panel_No] := (sender as TCheckBox).Checked;
     // Modified := TRUE;
end;

procedure TMainForm.CheckBox17Click(Sender: TObject);
var
  Panel_No : integer;
begin
     Panel_No := (sender as TCheckBox).Tag;
     XFP_Network.PAccept_Controls[Panel_No] := (sender as TCheckBox).Checked;
     // Modified := TRUE;
end;

procedure TMainForm.CheckBox25Click(Sender: TObject);
var
  Panel_No : integer;
begin
     Panel_No := (sender as TCheckBox).Tag;
     XFP_Network.PAccept_Disablements[Panel_No] := (sender as TCheckBox).Checked;// //
     // Modified := TRUE;
end;

procedure TMainForm.CheckBox33Click(Sender: TObject);
var
  Panel_No : integer;
begin
     Panel_No := (sender as TCheckBox).Tag;
     XFP_Network.PAccept_Occupied[Panel_No] := (sender as TCheckBox).Checked;
     // Modified := TRUE;
end;

procedure TMainForm.PhasedDelaySpinChange(Sender: TObject);
begin
  SiteFile.PhasedDelay := (Sender as TTimeSpinButton).Value;;
  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.spnInvestigationChange(Sender: TObject);
begin
  SiteFile.InvestigationPeriod := (Sender as TTimeSpinButton).Value;;
  UpdateZoneConfigPage;


  { Set // modified flag }
  // Modified := true;
end;

procedure TMainForm.spnIO_DelayChange(Sender: TObject);
begin
  SiteFile.IO_Delay := (Sender as TSpinEdit).Value;
  // Modified := true;

end;

procedure TMainForm.spnMCP_DelayChange(Sender: TObject);
begin
  SiteFile.MCP_Delay := (Sender as TSpinEdit).Value;
  // Modified := true;
end;

procedure TMainForm.spnDetector_DelayChange(Sender: TObject);
begin
  SiteFile.Detector_Delay := (Sender as TSpinEdit).Value;
  // Modified := true;

end;

procedure TMainForm.spnInvestigation1Change(Sender: TObject);
begin
  SiteFile.InvestigationPeriod1 := (Sender as TTimeSpinButton).Value;
  UpdateZoneConfigPage;

  { Set modified flag }
  // Modified := true;
end;

procedure TMainForm.sgrdLoopDblClick(Sender: TObject);
begin
exit;

  if sgrdLoop.Col = 1 then                   //only if column 1 is clicked on
  begin
    if Loop = 1 then
      frmLoop1.EditPointDevice
    else
      frmLoop2.EditPointDevice;

    sgrdLoop.Col := 0;
    UpdateLoopSummaryPage;
  end;
end;

procedure TMainForm.ReadFlashBtnClick(Sender: TObject);
begin
     TextStrings.RowCount := 1;

     LoadAll := false;
     DataType := dtLoadFlashMemory;
     EstablishComms;
end;

procedure TMainForm.LoadFlashData (CmdData: string);
var
   I, D : integer;
   TString : string;
begin
  { Reset the number of retries }
   FDataRetry := 0;

   I := 1;
   TString := '';
   D := 0;
   while I < 81 do
   begin
     D := Ord(CmdData[I + 2]);
     if (D = 0) or (D > 127) or (D < 10) then
       I := 100
     else
       TString := TString + Chr(D);
     I := I + 1;
   end;

   TextStrings.Cells[0,TextStrings.RowCount-1] := IntToStr(TextStrings.RowCount);
   TextStrings.Cells[1,TextStrings.RowCount-1] := TString;
   TextStrings.Cells[2,TextStrings.RowCount-1] := TString;
   TextStrings.RowCount := TextStrings.RowCount + 1;

  { If this is not the last zone }
  if D = 0 then D := 32;
  if (D < 128) and (D > 9) then
  begin

    { Increment the count, then update the progress bar }
    inc (FDataCount);
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Loading Texts ' + IntToStr (FDataCount);

    { Send request for next data}
    RecordSent := TxReq_FlashData + #2 + Chr (FDataCount and 255) + Chr(FDataCount div 256);
  end
  else begin
      frmStatus.Close;
      DataType := dtNone;
      TextStrings.RowCount := TextStrings.RowCount - 2;  //always lose the last row
  end;
end;


procedure TMainForm.WriteStringsClick(Sender: TObject);
begin
      LoadAll := false;
      DataType := dtSaveFlashMemory;
      EstablishComms;
end;


procedure TMainForm.SaveText;
var
  sTemp: string;

begin
  { Reset the number of retries }
  FDataRetry := 0;

  if FDataCount <= TextStrings.RowCount then begin
    frmStatus.Progress.Position := FDataCount;
    frmStatus.StatusText := 'Saving Texts ' + IntToStr (FDataCount + 1);

    sTemp := {TxSet_FlashData + #0 +} chr(FDataCount and 255) + chr(FDataCount div 256);
    sTemp := sTemp + TextStrings.Cells[2, FDataCount] + #0;

    { Now send constructed string }
    { calculate the length field }
    // sTemp[2] := chr(Length (sTemp)-2);
    RecordSent := BuildXferString( TxSet_FlashData, sTemp );
    inc(FDataCount);
  end
  else
  begin
    if FDataCount < 999 then
    begin
      FDataCount := 999;
       sTemp := {TxSet_FlashData + #0 +} chr(FDataCount and 255) + chr(FDataCount div 256);
      sTemp := sTemp + '' + #0;

      { Now send constructed string }
      { calculate the length field }
      // sTemp[2] := chr(Length (sTemp)-2);
      RecordSent := BuildXferString( TxSet_FlashData, sTemp );
    end else
    begin
        frmStatus.Close;
        DataType := dtNone;
        UpdateConfigData;
    end;
  end;
end;


procedure TMainForm.ReadSerialNumber;
var
  i : integer;
  TString : string;
  DateString : string;
  SerialNoString : string;
begin

  //load current record of serial numbers
  Memo1.Lines.LoadFromFile('Serial.txt');

  //get the last string
  i := 0;
  while(Memo1.Lines[i] <> '') do
  begin
    i := i + 1;
  end;
  TString := Memo1.Lines[i - 1];

  //split into date and serial number
  DateString := Copy(TString, 1, pos(' ',TString) - 1);
  SerialNoString :=Copy(TString, pos(' ',TString) + 1, length(TString));

  Edit10.Text := DateString;
  Edit11.Text := SerialNoString;

  i := StrToInt(SerialNoString);
  SpinEdit2.Value := i;

  //extract the 4 bytes which form the serial number, these
  //will be sent to the node
  i := trunc(SpinEdit2.Value / $1000000) AND 255;
  SerialNo3.Value := i;
  i := trunc(SpinEdit2.Value / $1000) AND 255;
  SerialNo2.Value := i;
  i := trunc(SpinEdit2.Value / $100) AND 255;
  SerialNo1.Value := i;
  i := trunc(SpinEdit2.Value) AND 255;
  SerialNo0.Value := i;

  SerialNumber[0] :=  SerialNo0.Value;
  SerialNumber[1] :=  SerialNo1.Value;
  SerialNumber[2] :=  SerialNo2.Value;
  SerialNumber[3] :=  SerialNo3.Value;

end;

procedure TMainForm.Import1Click(Sender: TObject);
var
  CurrentPage: integer;
  P : integer;
  Inifile: TInifile;
begin
  // Import old style file
  { Load the information from ini file, and update the components with this
  information }

  CurrentPage := ConfigPages.ActivePageIndex;
  if SiteFile.New then
  begin
    if PgmData.OpenDialog.Execute then
    begin
      frmStatus.Show;
      frmStatus.btnCancel.Visible := false;
      frmStatus.StatusText := 'Loading Data from Disk, Please Wait...';
      Application.ProcessMessages;

      Inifilename := PgmData.OpenDialog.Filename;
      P := Pos( '_Panel', InifileName);
      if P > 0 then
      begin
        PanelNumber.Value := StrToInt(AnsiMidStr(InifileName,P + 6,1));
      end;

      Inifile := TInifile.Create (InifileName);
      try
        if ValidFileType( IniFile ) then
        begin
          LoadConfigData( Inifile );
          //SiteFile.SaveAs( ChangeFileExt( IniFileName, '.xfp' ) )
        end
        else
        begin
          MessageDlg ('The selected file is not a valid XFP Programming Tools file',
            mtError, [mbOK], 0);
        end;
      finally
        IniFile.Free;
      end;
    end;
  end;

  { Ensure that the form is unmodified }
  // Modified := false;
  ConfigPages.ActivePageIndex := CurrentPage;
  ConfigPages.Visible := TRUE;
  frmStatus.Close;
  chkEnableZoneChanges.State := cbUnchecked;
end;

procedure TMainForm.IncrementSerialNumber;
var
  i : integer;
  DateString : string;
  SerialNoString : string;
begin
    //increment the serial number and append it to the file
  DateString := FormatDateTime('dd/mm/yyyy',Date);  // display the date on the form's caption
  SpinEdit2.Value := SpinEdit2.Value + 1;
  SerialNoString := IntToStr(SpinEdit2.Value);
  Memo1.Lines.Add(DateString + ' ' + SerialNoString);
  Memo1.Lines.SaveToFile('Serial.txt');

  //extract the 4 bytes which form the serial number, these
  //will be sent to the node
  i := trunc(SpinEdit2.Value / $1000000) AND 255;
  SerialNo3.Value := i;
  i := trunc(SpinEdit2.Value / $1000) AND 255;
  SerialNo2.Value := i;
  i := trunc(SpinEdit2.Value / $100) AND 255;
  SerialNo1.Value := i;
  i := trunc(SpinEdit2.Value) AND 255;
  SerialNo0.Value := i;

  SerialNumber[0] :=  SerialNo0.Value;
  SerialNumber[1] :=  SerialNo1.Value;
  SerialNumber[2] :=  SerialNo2.Value;
  SerialNumber[3] :=  SerialNo3.Value;

end;

procedure TMainForm.Int_Tone_BoxChange(Sender: TObject);
begin
  SiteFile.IntermittentTone := Int_Tone_Box.ItemIndex;
end;

procedure TMainForm.Int_Tone_Box_ApolloChange(Sender: TObject);
begin
  SiteFile.IntermittentTone := Int_Tone_Box_Apollo.ItemIndex;
end;

procedure TMainForm.Int_Tone_Box_CASTChange(Sender: TObject);
begin
  SiteFile.IntermittentTone := Int_Tone_Box_CAST.ItemIndex;
end;

procedure TMainForm.LoadTextsClick(Sender: TObject);
var
  TString : string;

  FileHandle : Integer;
  NotEOF : Integer;
  Buffer : array[1..80] of byte;
  c : integer;
  i : integer;
begin
  if PgmData.OpenDialogTexts.Execute then
  begin
    TextStrings.RowCount := 1;

    FileHandle := FileOpen(PgmData.OpenDialogTexts.FileName, fmOpenRead or fmShareDenyNone);
    if FileHandle > 0 then
    begin
      NotEOF := 1;
      while(NotEOF > 0) do
      begin
        c := 1;
        i := 1;
        Buffer[1] := 0;
        TString := '';
        while c <> 10 do
        begin
          if FileRead(FileHandle, Buffer[i], 1) = 0 then
          begin
            NotEOF := 0;
            c := 10;
          end else
          begin
            c := ord(Buffer[i]);
            if (C <> 10) and (c <> 13) then
            begin
              TString := TString + chr(c);
              i := i + 1;
            end;
          end;
        end;

        if NotEOF > 0 then
        begin
          TextStrings.Cells[0,TextStrings.RowCount-1] := IntToStr(TextStrings.RowCount);
          TextStrings.Cells[1,TextStrings.RowCount-1] := TString;
          TextStrings.Cells[2,TextStrings.RowCount-1] := TString;
          TextStrings.RowCount := TextStrings.RowCount + 1;

        end else
        begin
          FileClose(FileHandle);
          TextStrings.RowCount := TextStrings.RowCount - 1;  //always lose the last row
        end;
      end;
    end;
  end;

  ReadSerialNumber;
  IncrementSerialNumber;



end;

procedure TMainForm.SaveTextsClick(Sender: TObject);
var
  i : integer;
begin
  if PgmData.OpenDialogTexts.Execute then
  begin
    Memo1.Clear;
    for i := 0 to TextStrings.RowCount do
    begin
      Memo1.Lines.Add(TextStrings.Cells[2,i]);
    end;
    Memo1.Lines.SaveToFile(PgmData.OpenDialogTexts.FileName);
  end;


end;

procedure TMainForm.TextStringsSelectCell(Sender: TObject; ACol,
  ARow: Integer; var CanSelect: Boolean);
var
   TString : string;

begin
     if ACol <> 2 then
       Exit;

     TString := TextStrings.Cells[ACol, ARow];
     FrmTranslation.MaskEdit1.Text := TString;
     FrmTranslation.MaskEdit2.Text := TString;
     FrmTranslation.ShowModal;

     TString := FrmTranslation.MaskEdit2.Text;
     TextStrings.Cells[ACol, ARow] := TString;
end;

procedure TMainForm.NightCB1Click(Sender: TObject);
var
   i : integer;
   const Pow_Two : array[0..7] of integer = (1,2,4,8,16,32,64,128);

begin
     i := (Sender as TCheckBox).Tag - 1;
     i := Pow_Two[i];

     SiteFile.Night_Enable_Flags := SiteFile.Night_Enable_Flags and not i;
     if(Sender as TCheckBox).Checked = TRUE then
       SiteFile.Night_Enable_Flags := SiteFile.Night_Enable_Flags or i;

     // Modified := TRUE;
end;


procedure TMainForm.DayCB1Click(Sender: TObject);
var
   i : integer;
   const Pow_Two : array[0..7] of integer = (1,2,4,8,16,32,64,128);

begin
     i := (Sender as TCheckBox).Tag - 1;
     i := Pow_Two[i];

     SiteFile.Day_Enable_Flags := SiteFile.Day_Enable_Flags and not i;
     if(Sender as TCheckBox).Checked = TRUE then
       SiteFile.Day_Enable_Flags := SiteFile.Day_Enable_Flags or i;

     // Modified := TRUE;
end;

procedure TMainForm.Update_Device_Names_Database;
var
   i : integer;
begin

     for i := 1 to frmLoop1.AFPLoop.NoLoopDevices do
     begin
       if Loop = 1 then
       begin
         if frmLoop1.AFPLoop.Device[i].DevType < D_NOT_FITTED then
           frmLoop1.AFPLoop.Device[i].Name := sgrdLoop.Cells[4, i-1];
       end
       else
       begin
         if frmLoop2.AFPLoop.Device[i].DevType < D_NOT_FITTED then
           frmLoop2.AFPLoop.Device[i].Name := sgrdLoop.Cells[4, i-1];
       end;
     end;


end;

procedure TMainForm.LoadProtocol_Buttons;
begin

  fProtocolButtonsFile := TProtocolButtonsFile.Create;
  if fProtocolButtonsFile.Load() then
  begin
    MenuCASTSystem.Visible := CAST_Protocol_Available;
    { Specify fixed path for Help File }
    Application.HelpFile := GetCurrentDir + '\' + ProtocolButtonsFile.HelpFile;
  end
  else
  begin
    raise Exception.Create('Unable to find Protocol Buttons file. Please reinstall application');
  end;

end;


procedure TMainForm.Edit7Click(Sender: TObject);
begin
  frmDeviceName.ShowModal;
end;

procedure TMainForm.EndUpdate;
begin
  //FAntiFlicker.EndUpdate;
end;

procedure TMainForm.sgrdLoopExit(Sender: TObject);
var
  frmLoop : TFrmLoop;
begin
  case Loop of
    1: frmLoop := frmLoop1;
    2: frmLoop := frmLoop2;
    else exit;
  end;

  Update_Device_Names_Database;
  frmDeviceName.Rebuild_List( frmLoop1.AFPLoop, frmLoop2.AFPLoop );
  Update_Description_Indices( frmLoop );
end;

procedure TMainForm.KeyswitchClick(Sender: TObject);
begin

//keyswitch event now fully removed
//1/4/07. NB the visual properties have been set to
//disabled also.
{     if Keyswitch.Checked = TRUE then
     begin
       txtLvl2.Text := '5555';
       txtLvl2.Visible := FALSE;
     end else
     begin
       txtLvl2.Visible := TRUE;
       txtLvl2.Text := '3333';
     end;
}
//this was the code from 4.0, but it doesn't work since
//when the AL2code is read from the panel, this procedure
//is called, thus destroying any keyswitch setting
{    if Keyswitch.Checked = TRUE then
    begin
      txtLvl2.Visible := TRUE;
      txtLvl2.Text := '3333';
      Keyswitch.Checked := FALSE;
    end;
}
end;


procedure TMainForm.Timer3Timer(Sender: TObject);
var
   S : string;
begin
{check that the current description is not too long}

       with sgrdLoop do
       begin
         S := Cells[4, Row];
         //if strlen(PAnsiChar(S)) > MAX_PLACENAME_LENGTH then
         if length(S) > MAX_PLACENAME_LENGTH then
         begin
           MessageBeep (MB_ICONEXCLAMATION);
           S := leftstr(S,MAX_PLACENAME_LENGTH);
           Cells[4, Row] := S;

         end;
       end;

       with sgrdZone do
       begin
         S := Cells[1, Row];
         if Row > 1 then
           //if strlen(PAnsiChar(S)) > MAX_ZONENAME_LENGTH then
           if length(S) > MAX_ZONENAME_LENGTH then
           begin
             MessageBeep (MB_ICONEXCLAMATION);
             S := leftstr(S,MAX_ZONENAME_LENGTH);
             Cells[1, Row] := S;
           end;
       end;
end;


procedure TMainForm.BeginUpdate;
begin
  //FAntiFlicker.BeginUpdate;
end;

procedure TMainForm.BSGroupBoxChange(Sender: TObject);
var
   Group : integer;
   DevIndex: integer;
   frmLoop : TFrmLoop;
begin

  case Loop of
    1: frmLoop := frmLoop1;
    2: frmLoop := frmLoop2;
    else exit;
  end;

     DevIndex := GetGridDevIndex (frmLoop, sgrdLoop.Row);

     Group := BSGroupBox.ItemIndex;
     if Group > 0 then
     begin
       frmLoop.AFPLoop.Device[DevIndex].HasBaseSounder := TRUE;

      case frmLoop.AFPLoop.SystemType of
         stApolloLoop: frmLoop.AFPLoop.Device[DevIndex+127].DevType := 4;
         stHochikiLoop: frmLoop.AFPLoop.Device[DevIndex+127].DevType := $5E;
       end;
      frmLoop.AFPLoop.SetDefaults(DevIndex+127);
      frmLoop.AFPLoop.Device[DevIndex+127].Zone := Group;
     end else
     begin
       frmLoop.AFPLoop.Device[DevIndex].HasBaseSounder := FALSE;
       frmLoop.AFPLoop.Device[DevIndex+127].DevType := D_NOT_FITTED;
     end;

     BSGroupBox.Visible := FALSE;

     UpdateLoopSummaryPage;

    end;

procedure TMainForm.BST_AdjustSelectClick(Sender: TObject);
begin
     if BST_AdjustSelect.State = cbChecked then
       SiteFile.BST_Adjustment := true
     else
       SiteFile.BST_Adjustment := false;


end;

procedure TMainForm.RealTime_Event_OutputClick(Sender: TObject);
begin
     if chkRealTime_Event_Output.State = cbChecked then
       SiteFile.RealTime_Event_Output := true
     else
       SiteFile.RealTime_Event_Output := false;


end;

procedure TMainForm.ZGSGroupBoxChange(Sender: TObject);
var
  frmLoop : TFrmLoop;
begin

  case Loop of
    1: frmLoop := frmLoop1;
    2: frmLoop := frmLoop2;
    else exit;
  end;
     frmLoop.AFPLoop.Device[Device_Number].Zone := ZGSGroupBox.ItemIndex;

     ZGSGroupBox.Visible := FALSE;

     UpdateLoopSummaryPage;

end;

procedure TMainForm.ZGSGroupBoxClick(Sender: TObject);
var
  frmLoop : TFrmLoop;
begin

  case Loop of
    1: frmLoop := frmLoop1;
    2: frmLoop := frmLoop2;
    else exit;
  end;

     frmLoop.AFPLoop.Device[Device_Number].Zone := ZGSGroupBox.ItemIndex;

     UpdateLoopSummaryPage;
end;

procedure TMainForm.chkPolling_LEDClick(Sender: TObject);
begin
     if chkPolling_LED.State = cbChecked then
       SiteFile.Discovery_Polling_LED := true
     else
       SiteFile.Discovery_Polling_LED := false;

end;

procedure TMainForm.chkReSoundClick(Sender: TObject);
begin
  SiteFile.ReSound_Function := chkReSound.Checked;
end;

end.
