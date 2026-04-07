using System;
using System.Collections.Generic;
using System.Xml;

namespace MccCustomLanguage;

public class Language
{
	public string MainFrm_NoSystemParaXMLFileInExecuteDir = "";

	public string MainFrm_NoSystemParaXMLFileInExecuteDirUseDefaultPara = "";

	public string MainFrm_DecodeXmlFileOK;

	public string MainFrm_DecodeXmlFileFailed;

	public string MainFrm_PulseDLIs0ResetParaAndRestart;

	public string MainFrm_PositiveLimitEnableStop;

	public string MainFrm_NegtiveLimitEnableStop;

	public string MainFrm_AllProgrameHasSended;

	public string MainFrm_SendSomeProgrameSucceed;

	public string MainFrm_SendSomeProgrameFailed;

	public string MainFrm_ParameterDownloading;

	public string MainFrm_ParameterDownloadFailed;

	public string MainFrm_EncoderParameterDownloading;

	public string MainFrm_EncoderParameterDownloadFailed;

	public string MainFrm_SystemParameterUploading;

	public string MainFrm_SystemParameterUploadFailed;

	public string MainFrm_EncoderParameterUploading;

	public string MainFrm_EncoderarameterUploadFailed;

	public string MainFrm_ChooseProgrameLineToEdit;

	public string MainFrm_OpenProgrameFileSucceed;

	public string MainFrm_SaveProgrameFileTo;

	public string MainFrm_SaveProgrameFileFailedBeacuseNoPrigrame;

	public string MainFrm_ConnectMCCardFailed;

	public string MainFrm_ConnectMCCardSucceed;

	public string MainFrm_CloseMCCardSucceed;

	public string MainFrm_CloseMCCardFailed;

	public string MainFrm_NoSysParaFileFristOpenItFirst;

	public string MainFrm_UnloadFailed;

	public string MainFrm_ProgrameRunningCanNotStartAutoRunAgain;

	public string MainFrm_SendProgrameCommandFailedCheckComunication;

	public string MainFrm_UpdateSystemParameterFailed;

	public string MainFrm_UpdateSystemParameterSucceed;

	public string MainFrm_UpdateEncoderParameterFailed;

	public string MainFrm_UpdateEncoderParameterSucceed;

	public string MainFrm_TCPConnectFailedCheckIPAndPort;

	public string MainFrm_TCPConnectSucceed;

	public string MainFrm_TCPCloseFailed;

	public string MainFrm_TCPCloseSucceed;

	public string TestIOFrm_TCPCloseSucceed;

	public string SysCtrlParaFrm_SaveSystemControlParaFailed;

	public string SysCtrlParaFrm_SetLCDScanDelayParaFailed;

	public string SysCtrlParaFrm_SetResetParameterFailed;

	public string SysCtrlParaFrm_SetHandWheelFailed;

	public string SysCtrlParaFrm_ParameterFormatError;

	public string SysCtrlParaFrm_ReadSystemControlFailed;

	public string USARTAssitantFrm_OperateSucceed;

	public string USARTAssitantFrm_OperateFailed;

	public string USARTAssitantFrm_OpenPort;

	public string USARTAssitantFrm_ClosePort;

	public string CacSinglLevFrm_StateOpen;

	public string CacSinglLevFrm_StateClose;

	public string CacSinglLevFrm_StateEnable;

	public string CacSinglLevFrm_StateDisable;

	public string CacSinglLevFrm_NegLimit;

	public string CacSinglLevFrm_PosLimit;

	public string CacSinglLevFrm_Original;

	public string CacSinglLevFrm_ComTable;

	public string CacSinglLevFrm_HardLimit;

	public string CacSinglLevFrm_SoftLimit;

	public string TriggerMoveFrm_TestEnd;

	public string TriggerMoveFrm_TriggerCntError;

	public string ManualOpExFrm_CommandModeDist;

	public string ManualOpExFrm_CommandModePos;

	public string ManualOpExFrm_MoveParameterError;

	public string ManualOpExFrm_NotSupportGroupMoveInJogMode;

	public string ManualOpExFrm_NotChooseAxisInGroupMode;

	public string InputFunCfgFrm_ReadInputCfgParaError;

	public string InputFunCfgFrm_SetLow16BitsInputFunFailed;

	public string InputFunCfgFrm_SetHigh16BitsInputFunFailed;

	public string InputFunCfgFrm_SaveParameterToControllerFailed;

	public string AxisExParaFrm_ReadAxisParameterFailed;

	public string AxisExParaFrm_SetAxisParameterFailed;

	public string AxisExParaFrm_ParameterFormatError;

	public string ExPosFollowFrm_DataError;

	public string ExPosFollowFrm_DecodeFileDone;

	public string ExPosFollowFrm_ExceptionRiseWhenDecodeFile;

	public string ExPosFollowFrm_OpenComunicationPortSucceed;

	public string ExPosFollowFrm_OpenComunicationPortFailed;

	public string ExPosFollowFrm_ClosePortSucceed;

	public string ExPosFollowFrm_ClosePortFailed;

	public string ExPosFollowFrm_StopSendExPositionPoint;

	public string ExPosFollowFrm_SomeCommandSendedOKAndCurrentCount;

	public string ExPosFollowFrm_SomeCommandSendedFailed;

	public string ScopeFrm_ScopeVersionInfo;

	public string ScopeFrm_NoChannelPleaseConfigThemFirst;

	public string ScopeFrm_SendStartSampleCommandFailed;

	public string ScopeFrm_SendSomeChannelConfigCommandFailed;

	public string ScopeFrm_ConfigAllChannelSucceed;

	public string ScopeFrm_NotConfigChannelCanNotDownloadChannelParameters;

	public string ScopeFrm_DeleteScopConfigFailed;

	public string ScopeFrm_CloseScopFailed;

	public string ScopeFrm_SaveScopeConfigFileSucceed;

	public string ScopeFrm_LoadScopeConfigFileSucceed;

	public string ScopeFrm_ConfigFileEmpty;

	public string ScopeFrm_WordChannel;

	public string ScopeFrm_ChannelConfigDonDownloadConfigParameters;

	public string ScopeFrm_OpenAndDeleteAllConfigInfo;

	public string ScopeFrm_OpenScopeFailed;

	public string ScopeFrm_StartDownloadConfigParametes;

	public string ScopeFrm_SaveFileTo;

	public string ScopeFrm_CanNotSaveFileBecauseFileNameNull;

	public string ScopeFrm_SaveFileFailed;

	public string ScopeFrm_OpenFileSucceed;

	public string ScopeFrm_WordNotConfig;

	public string ScopeFrm_SendStopSampleCommandFailed;

	public string ScopeFrm_OpenPortFailed;

	public string ScopeFrm_OpenPortSucceed;

	public string ScopeFrm_ClosePortSucceed;

	public string ScopeFrm_ClosePortFailed;

	public string ScopeFrm_SaveHistoryDataSucceed;

	public string CurveDefineFrm_ChooseCurveBeforeDownloadCurveParameters;

	public string CurveDefineFrm_SendSomeParameterFailed;

	public string CurveDefineFrm_SendMovementParameterSucceed;

	public string CurveDefineFrm_PleaseDefineMasterAndSlave;

	public string CurveDefineFrm_DefineGearRatio;

	public string CurveDefineFrm_SendGearParameterSucceed;

	public string CurveDefineFrm_SendGearParameterFailed;

	public string CurveDefineFrm_ChooseSlaveAxis;

	public string CurveDefineFrm_SendGearInCommandSucceed;

	public string CurveDefineFrm_SendGearInCommandFailed;

	public string CurveDefineFrm_SendGearOutCommandSucceed;

	public string CurveDefineFrm_SendGearOutCommandFailed;

	public string CurveDefineFrm_SendCamInCommandFailed;

	public string CurveDefineFrm_SendCamInCommandSucceed;

	public string CurveDefineFrm_SendCamOutCommandFailed;

	public string CurveDefineFrm_SendCamOutCommandSucceed;

	public string CurveDefineFrm_NotAllParameterDefined;

	public string CurveDefineFrm_DefineMovementPointCntAndT;

	public string CurveDefineFrm_DataError;

	public string CurveDefineFrm_SetCamParameterFailed;

	public string CurveDefineFrm_SetCamParameterSucceed;

	public string CurveDefineFrm_CamProfileEmpty;

	public string ComParaFrm_WordComTable;

	public string ComParaFrm_WordCom;

	public string ComParaFrm_WordEnable;

	public string ComParaFrm_WordDisable;

	public string ComParaFrm_SetParameterFailed;

	public string ComParaFrm_ConnectMCCard;

	public string ComParaFrm_ParameterIndxeError;

	public string ComParaFrm_EnablePlatformCompensate;

	public string ComParaFrm_EnablePlatformCompensateFailed;

	public string ComParaFrm_DisablePlatformCompensate;

	public string ComParaFrm_DisablePlatformCompensateFailed;

	public string CacPulseDLFrm_DataFormatError;

	public string AbsEncFrm_EncoderFormatError;

	public string AbsEncFrm_CheckFrequencyParamereUnitIsHz;

	public string AbsEncFrm_ValueTooLargeCheckInput;

	public string TMC2209Frm_ReadSomeRegError;

	public string TMC2209Frm_NotDefineMicroSteps200StepPerTurn;

	public string TMC2209Frm_ReadMicroOkButFormatInValidPleaseReSet;

	public string TMC2209Frm_ReadMicroStepFailed;

	public string TMC2209Frm_ReadWorkCurrentFailed;

	public string TMC2209Frm_ReadHoldCurrentFailed;

	public string TMC2209Frm_InputFunctionParameterError;

	public string TMC2209Frm_ReadInputFunctionParameterFailed;

	public string TMC2209Frm_RegValueFormatError;

	public string TMC2209Frm_WriteRegFailed;

	public string TMC2209Frm_WriteRegSucceed;

	public string TMC2209Frm_EnableDriverFailed;

	public string TMC2209Frm_DisableDriverFailed;

	public string TMC2209Frm_SaveRegValueFailed;

	public string TMC2209Frm_SaveParameterToBoardFailed;

	public string TMC2209Frm_ResetDriverFailed;

	public string TMC2209Frm_SetMicroStepFailed;

	public string TMC2209Frm_SetInputFunctionFailed;

	public string TMC2209Frm_SetWorkCurrentFailed;

	public string TMC2209Frm_SetHoldCurrentFailed;

	protected Dictionary<string, string> DicLanguage = new Dictionary<string, string>();

	public Language()
	{
		XmlLoad(GlobalData.SystemLanguage);
		BindLanguageText();
	}

	protected void XmlLoad(string language)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		try
		{
			XmlDocument val = new XmlDocument();
			string text = AppDomain.CurrentDomain.BaseDirectory + "Languages\\" + language + ".xml";
			val.Load(text);
			foreach (XmlNode childNode in ((XmlNode)val.DocumentElement).ChildNodes)
			{
				XmlNode val2 = childNode;
				DicLanguage.Add(val2.Name, val2.InnerText);
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void BindLanguageText()
	{
		try
		{
			MainFrm_NoSystemParaXMLFileInExecuteDir = DicLanguage["MainFrm_NoSystemParaXMLFileInExecuteDir"];
			MainFrm_NoSystemParaXMLFileInExecuteDirUseDefaultPara = DicLanguage["MainFrm_NoSystemParaXMLFileInExecuteDirUseDefaultPara"];
			MainFrm_DecodeXmlFileOK = DicLanguage["MainFrm_DecodeXmlFileOK"];
			MainFrm_DecodeXmlFileFailed = DicLanguage["MainFrm_DecodeXmlFileFailed"];
			MainFrm_PulseDLIs0ResetParaAndRestart = DicLanguage["MainFrm_PulseDLIs0ResetParaAndRestart"];
			MainFrm_PositiveLimitEnableStop = DicLanguage["MainFrm_PositiveLimitEnableStop"];
			MainFrm_NegtiveLimitEnableStop = DicLanguage["MainFrm_NegtiveLimitEnableStop"];
			MainFrm_AllProgrameHasSended = DicLanguage["MainFrm_AllProgrameHasSended"];
			MainFrm_SendSomeProgrameSucceed = DicLanguage["MainFrm_SendSomeProgrameSucceed"];
			MainFrm_SendSomeProgrameFailed = DicLanguage["MainFrm_SendSomeProgrameFailed"];
			MainFrm_ParameterDownloading = DicLanguage["MainFrm_ParameterDownloading"];
			MainFrm_ParameterDownloadFailed = DicLanguage["MainFrm_ParameterDownloadFailed"];
			MainFrm_EncoderParameterDownloading = DicLanguage["MainFrm_EncoderParameterDownloading"];
			MainFrm_EncoderParameterDownloadFailed = DicLanguage["MainFrm_EncoderParameterDownloadFailed"];
			MainFrm_SystemParameterUploading = DicLanguage["MainFrm_SystemParameterUploading"];
			MainFrm_SystemParameterUploadFailed = DicLanguage["MainFrm_SystemParameterUploadFailed"];
			MainFrm_EncoderParameterUploading = DicLanguage["MainFrm_EncoderParameterUploading"];
			MainFrm_EncoderarameterUploadFailed = DicLanguage["MainFrm_EncoderarameterUploadFailed"];
			MainFrm_ChooseProgrameLineToEdit = DicLanguage["MainFrm_ChooseProgrameLineToEdit"];
			MainFrm_OpenProgrameFileSucceed = DicLanguage["MainFrm_OpenProgrameFileSucceed"];
			MainFrm_SaveProgrameFileTo = DicLanguage["MainFrm_SaveProgrameFileTo"];
			MainFrm_SaveProgrameFileFailedBeacuseNoPrigrame = DicLanguage["MainFrm_SaveProgrameFileFailedBeacuseNoPrigrame"];
			MainFrm_ConnectMCCardFailed = DicLanguage["MainFrm_ConnectMCCardFailed"];
			MainFrm_ConnectMCCardSucceed = DicLanguage["MainFrm_ConnectMCCardSucceed"];
			MainFrm_CloseMCCardSucceed = DicLanguage["MainFrm_CloseMCCardSucceed"];
			MainFrm_CloseMCCardFailed = DicLanguage["MainFrm_CloseMCCardFailed"];
			MainFrm_NoSysParaFileFristOpenItFirst = DicLanguage["MainFrm_NoSysParaFileFristOpenItFirst"];
			MainFrm_ProgrameRunningCanNotStartAutoRunAgain = DicLanguage["MainFrm_ProgrameRunningCanNotStartAutoRunAgain"];
			MainFrm_SendProgrameCommandFailedCheckComunication = DicLanguage["MainFrm_SendProgrameCommandFailedCheckComunication"];
			MainFrm_UpdateSystemParameterFailed = DicLanguage["MainFrm_UpdateSystemParameterFailed"];
			MainFrm_UpdateSystemParameterSucceed = DicLanguage["MainFrm_UpdateSystemParameterSucceed"];
			MainFrm_UpdateEncoderParameterFailed = DicLanguage["MainFrm_UpdateEncoderParameterFailed"];
			MainFrm_UpdateEncoderParameterSucceed = DicLanguage["MainFrm_UpdateEncoderParameterSucceed"];
			MainFrm_TCPConnectFailedCheckIPAndPort = DicLanguage["MainFrm_TCPConnectFailedCheckIPAndPort"];
			MainFrm_TCPConnectSucceed = DicLanguage["MainFrm_TCPConnectSucceed"];
			MainFrm_TCPCloseFailed = DicLanguage["MainFrm_TCPCloseFailed"];
			MainFrm_TCPCloseSucceed = DicLanguage["MainFrm_TCPCloseSucceed"];
			TestIOFrm_TCPCloseSucceed = DicLanguage["TestIOFrm_TCPCloseSucceed"];
			SysCtrlParaFrm_SaveSystemControlParaFailed = DicLanguage["SysCtrlParaFrm_SaveSystemControlParaFailed"];
			SysCtrlParaFrm_SetLCDScanDelayParaFailed = DicLanguage["SysCtrlParaFrm_SetLCDScanDelayParaFailed"];
			SysCtrlParaFrm_SetResetParameterFailed = DicLanguage["SysCtrlParaFrm_SetResetParameterFailed"];
			SysCtrlParaFrm_SetHandWheelFailed = DicLanguage["SysCtrlParaFrm_SetHandWheelFailed"];
			SysCtrlParaFrm_ParameterFormatError = DicLanguage["SysCtrlParaFrm_ParameterFormatError"];
			SysCtrlParaFrm_ReadSystemControlFailed = DicLanguage["SysCtrlParaFrm_ReadSystemControlFailed"];
			USARTAssitantFrm_OperateSucceed = DicLanguage["USARTAssitantFrm_OperateSucceed"];
			USARTAssitantFrm_OperateFailed = DicLanguage["USARTAssitantFrm_OperateFailed"];
			USARTAssitantFrm_OpenPort = DicLanguage["USARTAssitantFrm_OpenPort"];
			USARTAssitantFrm_ClosePort = DicLanguage["USARTAssitantFrm_ClosePort"];
			CacSinglLevFrm_StateOpen = DicLanguage["CacSinglLevFrm_StateOpen"];
			CacSinglLevFrm_StateClose = DicLanguage["CacSinglLevFrm_StateClose"];
			CacSinglLevFrm_StateEnable = DicLanguage["CacSinglLevFrm_StateEnable"];
			CacSinglLevFrm_StateDisable = DicLanguage["CacSinglLevFrm_StateDisable"];
			CacSinglLevFrm_NegLimit = DicLanguage["CacSinglLevFrm_NegLimit"];
			CacSinglLevFrm_PosLimit = DicLanguage["CacSinglLevFrm_PosLimit"];
			CacSinglLevFrm_Original = DicLanguage["CacSinglLevFrm_Original"];
			CacSinglLevFrm_ComTable = DicLanguage["CacSinglLevFrm_ComTable"];
			CacSinglLevFrm_HardLimit = DicLanguage["CacSinglLevFrm_HardLimit"];
			CacSinglLevFrm_SoftLimit = DicLanguage["CacSinglLevFrm_SoftLimit"];
			TriggerMoveFrm_TestEnd = DicLanguage["TriggerMoveFrm_TestEnd"];
			TriggerMoveFrm_TriggerCntError = DicLanguage["TriggerMoveFrm_TriggerCntError"];
			ManualOpExFrm_CommandModeDist = DicLanguage["ManualOpExFrm_CommandModeDist"];
			ManualOpExFrm_CommandModePos = DicLanguage["ManualOpExFrm_CommandModePos"];
			ManualOpExFrm_MoveParameterError = DicLanguage["ManualOpExFrm_MoveParameterError"];
			ManualOpExFrm_NotSupportGroupMoveInJogMode = DicLanguage["ManualOpExFrm_NotSupportGroupMoveInJogMode"];
			ManualOpExFrm_NotChooseAxisInGroupMode = DicLanguage["ManualOpExFrm_NotChooseAxisInGroupMode"];
			InputFunCfgFrm_ReadInputCfgParaError = DicLanguage["InputFunCfgFrm_ReadInputCfgParaError"];
			InputFunCfgFrm_SetLow16BitsInputFunFailed = DicLanguage["InputFunCfgFrm_SetLow16BitsInputFunFailed"];
			InputFunCfgFrm_SetHigh16BitsInputFunFailed = DicLanguage["InputFunCfgFrm_SetHigh16BitsInputFunFailed"];
			InputFunCfgFrm_SaveParameterToControllerFailed = DicLanguage["InputFunCfgFrm_SaveParameterToControllerFailed"];
			AxisExParaFrm_ReadAxisParameterFailed = DicLanguage["AxisExParaFrm_ReadAxisParameterFailed"];
			AxisExParaFrm_SetAxisParameterFailed = DicLanguage["AxisExParaFrm_SetAxisParameterFailed"];
			AxisExParaFrm_ParameterFormatError = DicLanguage["AxisExParaFrm_ParameterFormatError"];
			ExPosFollowFrm_DataError = DicLanguage["ExPosFollowFrm_DataError"];
			ExPosFollowFrm_DecodeFileDone = DicLanguage["ExPosFollowFrm_DecodeFileDone"];
			ExPosFollowFrm_ExceptionRiseWhenDecodeFile = DicLanguage["ExPosFollowFrm_ExceptionRiseWhenDecodeFile"];
			ExPosFollowFrm_OpenComunicationPortSucceed = DicLanguage["ExPosFollowFrm_OpenComunicationPortSucceed"];
			ExPosFollowFrm_OpenComunicationPortFailed = DicLanguage["ExPosFollowFrm_OpenComunicationPortFailed"];
			ExPosFollowFrm_ClosePortSucceed = DicLanguage["ExPosFollowFrm_ClosePortSucceed"];
			ExPosFollowFrm_ClosePortFailed = DicLanguage["ExPosFollowFrm_ClosePortFailed"];
			ExPosFollowFrm_StopSendExPositionPoint = DicLanguage["ExPosFollowFrm_StopSendExPositionPoint"];
			ExPosFollowFrm_SomeCommandSendedOKAndCurrentCount = DicLanguage["ExPosFollowFrm_SomeCommandSendedOKAndCurrentCount"];
			ExPosFollowFrm_SomeCommandSendedFailed = DicLanguage["ExPosFollowFrm_SomeCommandSendedFailed"];
			ScopeFrm_ScopeVersionInfo = DicLanguage["ScopeFrm_ScopeVersionInfo"];
			ScopeFrm_NoChannelPleaseConfigThemFirst = DicLanguage["ScopeFrm_NoChannelPleaseConfigThemFirst"];
			ScopeFrm_SendStartSampleCommandFailed = DicLanguage["ScopeFrm_SendStartSampleCommandFailed"];
			ScopeFrm_SendSomeChannelConfigCommandFailed = DicLanguage["ScopeFrm_SendSomeChannelConfigCommandFailed"];
			ScopeFrm_ConfigAllChannelSucceed = DicLanguage["ScopeFrm_ConfigAllChannelSucceed"];
			ScopeFrm_NotConfigChannelCanNotDownloadChannelParameters = DicLanguage["ScopeFrm_NotConfigChannelCanNotDownloadChannelParameters"];
			ScopeFrm_DeleteScopConfigFailed = DicLanguage["ScopeFrm_DeleteScopConfigFailed"];
			ScopeFrm_CloseScopFailed = DicLanguage["ScopeFrm_CloseScopFailed"];
			ScopeFrm_SaveScopeConfigFileSucceed = DicLanguage["ScopeFrm_SaveScopeConfigFileSucceed"];
			ScopeFrm_LoadScopeConfigFileSucceed = DicLanguage["ScopeFrm_LoadScopeConfigFileSucceed"];
			ScopeFrm_ConfigFileEmpty = DicLanguage["ScopeFrm_ConfigFileEmpty"];
			ScopeFrm_WordChannel = DicLanguage["ScopeFrm_WordChannel"];
			ScopeFrm_ChannelConfigDonDownloadConfigParameters = DicLanguage["ScopeFrm_ChannelConfigDonDownloadConfigParameters"];
			ScopeFrm_OpenScopeFailed = DicLanguage["ScopeFrm_OpenScopeFailed"];
			ScopeFrm_StartDownloadConfigParametes = DicLanguage["ScopeFrm_StartDownloadConfigParametes"];
			ScopeFrm_SaveFileTo = DicLanguage["ScopeFrm_SaveFileTo"];
			ScopeFrm_CanNotSaveFileBecauseFileNameNull = DicLanguage["ScopeFrm_CanNotSaveFileBecauseFileNameNull"];
			ScopeFrm_SaveFileFailed = DicLanguage["ScopeFrm_SaveFileFailed"];
			ScopeFrm_OpenFileSucceed = DicLanguage["ScopeFrm_OpenFileSucceed"];
			ScopeFrm_WordNotConfig = DicLanguage["ScopeFrm_WordNotConfig"];
			ScopeFrm_SendStopSampleCommandFailed = DicLanguage["ScopeFrm_SendStopSampleCommandFailed"];
			ScopeFrm_OpenPortFailed = DicLanguage["ScopeFrm_OpenPortFailed"];
			ScopeFrm_OpenPortSucceed = DicLanguage["ScopeFrm_OpenPortSucceed"];
			ScopeFrm_ClosePortSucceed = DicLanguage["ScopeFrm_ClosePortSucceed"];
			ScopeFrm_ClosePortFailed = DicLanguage["ScopeFrm_ClosePortFailed"];
			ScopeFrm_SaveHistoryDataSucceed = DicLanguage["ScopeFrm_SaveHistoryDataSucceed"];
			CurveDefineFrm_ChooseCurveBeforeDownloadCurveParameters = DicLanguage["CurveDefineFrm_ChooseCurveBeforeDownloadCurveParameters"];
			CurveDefineFrm_SendSomeParameterFailed = DicLanguage["CurveDefineFrm_SendSomeParameterFailed"];
			CurveDefineFrm_SendMovementParameterSucceed = DicLanguage["CurveDefineFrm_SendMovementParameterSucceed"];
			CurveDefineFrm_PleaseDefineMasterAndSlave = DicLanguage["CurveDefineFrm_PleaseDefineMasterAndSlave"];
			CurveDefineFrm_DefineGearRatio = DicLanguage["CurveDefineFrm_DefineGearRatio"];
			CurveDefineFrm_SendGearParameterSucceed = DicLanguage["CurveDefineFrm_SendGearParameterSucceed"];
			CurveDefineFrm_SendGearParameterFailed = DicLanguage["CurveDefineFrm_SendGearParameterFailed"];
			CurveDefineFrm_ChooseSlaveAxis = DicLanguage["CurveDefineFrm_ChooseSlaveAxis"];
			CurveDefineFrm_SendGearInCommandSucceed = DicLanguage["CurveDefineFrm_SendGearInCommandSucceed"];
			CurveDefineFrm_SendGearInCommandFailed = DicLanguage["CurveDefineFrm_SendGearInCommandFailed"];
			CurveDefineFrm_SendGearOutCommandSucceed = DicLanguage["CurveDefineFrm_SendGearOutCommandSucceed"];
			CurveDefineFrm_SendGearOutCommandFailed = DicLanguage["CurveDefineFrm_SendGearOutCommandFailed"];
			CurveDefineFrm_SendCamInCommandFailed = DicLanguage["CurveDefineFrm_SendCamInCommandFailed"];
			CurveDefineFrm_SendCamInCommandSucceed = DicLanguage["CurveDefineFrm_SendCamInCommandSucceed"];
			CurveDefineFrm_SendCamOutCommandFailed = DicLanguage["CurveDefineFrm_SendCamOutCommandFailed"];
			CurveDefineFrm_SendCamOutCommandSucceed = DicLanguage["CurveDefineFrm_SendCamOutCommandSucceed"];
			CurveDefineFrm_NotAllParameterDefined = DicLanguage["CurveDefineFrm_NotAllParameterDefined"];
			CurveDefineFrm_DefineMovementPointCntAndT = DicLanguage["CurveDefineFrm_DefineMovementPointCntAndT"];
			CurveDefineFrm_DataError = DicLanguage["CurveDefineFrm_DataError"];
			CurveDefineFrm_SetCamParameterFailed = DicLanguage["CurveDefineFrm_SetCamParameterFailed"];
			CurveDefineFrm_SetCamParameterSucceed = DicLanguage["CurveDefineFrm_SetCamParameterSucceed"];
			CurveDefineFrm_CamProfileEmpty = DicLanguage["CurveDefineFrm_CamProfileEmpty"];
			ComParaFrm_WordComTable = DicLanguage["ComParaFrm_WordComTable"];
			ComParaFrm_WordCom = DicLanguage["ComParaFrm_WordCom"];
			ComParaFrm_WordEnable = DicLanguage["ComParaFrm_WordEnable"];
			ComParaFrm_WordDisable = DicLanguage["ComParaFrm_WordDisable"];
			ComParaFrm_SetParameterFailed = DicLanguage["ComParaFrm_SetParameterFailed"];
			ComParaFrm_ConnectMCCard = DicLanguage["ComParaFrm_ConnectMCCard"];
			ComParaFrm_ParameterIndxeError = DicLanguage["ComParaFrm_ParameterIndxeError"];
			ComParaFrm_EnablePlatformCompensate = DicLanguage["ComParaFrm_EnablePlatformCompensate"];
			ComParaFrm_EnablePlatformCompensateFailed = DicLanguage["ComParaFrm_EnablePlatformCompensateFailed"];
			ComParaFrm_DisablePlatformCompensate = DicLanguage["ComParaFrm_DisablePlatformCompensate"];
			ComParaFrm_DisablePlatformCompensateFailed = DicLanguage["ComParaFrm_DisablePlatformCompensateFailed"];
			CacPulseDLFrm_DataFormatError = DicLanguage["CacPulseDLFrm_DataFormatError"];
			AbsEncFrm_EncoderFormatError = DicLanguage["AbsEncFrm_EncoderFormatError"];
			AbsEncFrm_CheckFrequencyParamereUnitIsHz = DicLanguage["AbsEncFrm_CheckFrequencyParamereUnitIsHz"];
			AbsEncFrm_ValueTooLargeCheckInput = DicLanguage["AbsEncFrm_ValueTooLargeCheckInput"];
			TMC2209Frm_ReadSomeRegError = DicLanguage["TMC2209Frm_ReadSomeRegError"];
			TMC2209Frm_NotDefineMicroSteps200StepPerTurn = DicLanguage["TMC2209Frm_NotDefineMicroSteps200StepPerTurn"];
			TMC2209Frm_ReadMicroOkButFormatInValidPleaseReSet = DicLanguage["TMC2209Frm_ReadMicroOkButFormatInValidPleaseReSet"];
			TMC2209Frm_ReadWorkCurrentFailed = DicLanguage["TMC2209Frm_ReadWorkCurrentFailed"];
			TMC2209Frm_ReadHoldCurrentFailed = DicLanguage["TMC2209Frm_ReadHoldCurrentFailed"];
			TMC2209Frm_InputFunctionParameterError = DicLanguage["TMC2209Frm_InputFunctionParameterError"];
			TMC2209Frm_ReadInputFunctionParameterFailed = DicLanguage["TMC2209Frm_ReadInputFunctionParameterFailed"];
			TMC2209Frm_RegValueFormatError = DicLanguage["TMC2209Frm_RegValueFormatError"];
			TMC2209Frm_WriteRegSucceed = DicLanguage["TMC2209Frm_WriteRegSucceed"];
			TMC2209Frm_WriteRegFailed = DicLanguage["TMC2209Frm_WriteRegFailed"];
			TMC2209Frm_EnableDriverFailed = DicLanguage["TMC2209Frm_EnableDriverFailed"];
			TMC2209Frm_DisableDriverFailed = DicLanguage["TMC2209Frm_DisableDriverFailed"];
			TMC2209Frm_SaveRegValueFailed = DicLanguage["TMC2209Frm_SaveRegValueFailed"];
			TMC2209Frm_SaveParameterToBoardFailed = DicLanguage["TMC2209Frm_SaveParameterToBoardFailed"];
			TMC2209Frm_ResetDriverFailed = DicLanguage["TMC2209Frm_ResetDriverFailed"];
			TMC2209Frm_SetMicroStepFailed = DicLanguage["TMC2209Frm_SetMicroStepFailed"];
			TMC2209Frm_SetInputFunctionFailed = DicLanguage["TMC2209Frm_SetInputFunctionFailed"];
			TMC2209Frm_SetWorkCurrentFailed = DicLanguage["TMC2209Frm_SetWorkCurrentFailed"];
			TMC2209Frm_SetHoldCurrentFailed = DicLanguage["TMC2209Frm_SetHoldCurrentFailed"];
		}
		catch (Exception)
		{
		}
	}
}
