using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MCC6DLL;

namespace SerialPortLibrary;

public class SPLibClass
{
	private struct stPlatformComCmndType
	{
		public byte subCmnd;

		public byte zAxisID;

		public byte xAxisID;

		public byte yAxisID;
	}

	private struct stCmndHeadType
	{
		public byte u8SpCom_Headr;

		public byte u8SpCom_DtLen;

		public byte u8SpCom_CardIndx;

		public byte u8SpCom_Check;

		public byte u8SpDt_CheckSum;

		public byte u8SpDt_CmndType;

		public byte u8SpDt_Len;

		public byte u8SpDt_CmndNo;
	}

	private struct ManualOperateType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_ManulOpTyp stManualOp;

		public void Init(byte nCmndType, byte nSubCmnd, byte nAxisId, sbyte nSpdDir, float fPos, float fVel, float fAcc = 0f, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stManualOp));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stManualOp.SubCmnd = nSubCmnd;
			stManualOp.SpdDir = nSpdDir;
			stManualOp.AxisID = nAxisId;
			stManualOp.dummy = 0;
			stManualOp.PosCmd = fPos;
			stManualOp.VelCmd = fVel;
			stManualOp.AccCmd = fAcc;
		}
	}

	private struct ReadParaCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_ReadParaTyp stReadPara;

		public void Init(byte nCmndType, byte nAxisId, byte nParaIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stReadPara));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stReadPara.AxisId = nAxisId;
			stReadPara.ParaIndex = nParaIndx;
		}
	}

	private struct WriteParaCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetParaTyp stWritePara;

		public void Init(byte nCmndType, byte nAxisId, byte nParaIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stWritePara));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stWritePara.AxisId = nAxisId;
			stWritePara.ParaIndex = nParaIndx;
			stWritePara.dummy1 = 0;
			stWritePara.dummy2 = 0;
			stWritePara.IVal = 0;
		}

		public void SetVal(float fVal)
		{
			stWritePara.fVal = fVal;
		}

		public void SetVal(int IVal)
		{
			stWritePara.IVal = IVal;
		}

		public void SetVal(uint UIVal)
		{
			stWritePara.UIVal = UIVal;
		}
	}

	private struct ReadMCCardInfoCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_AskSttInfoTyp stAskInfo;

		public void Init(byte nCmndType, byte nAxisId, byte nParaIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stAskInfo));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stAskInfo.AxisId = nAxisId;
			stAskInfo.ParaIndex = nParaIndx;
		}
	}

	private struct RestAxisPosCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_RestPosTyp stRestPos;

		public void Init(byte nSubCmnd, byte nAxisId, float fPos, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stRestPos));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 248;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stRestPos.SubCmnd = nSubCmnd;
			stRestPos.AxisID = nAxisId;
			stRestPos.dummy = 0;
			stRestPos.PosCmd = fPos;
		}
	}

	private struct SetOutputCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetOutputTyp stSetOutput;

		public void Init(byte nSubCmnd, byte nOutpIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stSetOutput));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 249;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSetOutput.SubCmnd = nSubCmnd;
			stSetOutput.OutputIndex = nOutpIndx;
			stSetOutput.dummy = 0;
		}
	}

	private struct SetJoyStickCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetAnaEnTyp stSetAnaEn;

		public void Init(byte nAxisId, byte bEn, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stSetAnaEn));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 250;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSetAnaEn.AxisID = nAxisId;
			stSetAnaEn.bEn = bEn;
		}
	}

	private struct EnableSomeFunctionCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_EnableSomeFunctionTyp stEnableSomeFunction;

		public void Init(byte nAxisId, byte nFunType, byte bEn, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stEnableSomeFunction));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 251;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stEnableSomeFunction.AxisID = nAxisId;
			stEnableSomeFunction.FunType = nFunType;
			stEnableSomeFunction.bEnOrNot = bEn;
			stEnableSomeFunction.dummy = 0;
		}
	}

	private struct SetEncoderValueCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetEncoderCmndTyp stSetEncoder;

		public void Init(byte nAxisId, int IVal, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stSetEncoder));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 252;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSetEncoder.AxisID = nAxisId;
			stSetEncoder.dummy1 = 0;
			stSetEncoder.dummy2 = 0;
			stSetEncoder.EnCoderVal = IVal;
		}
	}

	private struct ClearInternalPulseCntCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_ClearInternalPulseCntTyp stClrInternalPulCnt;

		public void Init(byte nAxisId, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stClrInternalPulCnt));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 209;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stClrInternalPulCnt.AxisID = nAxisId;
		}
	}

	private struct SetProfileCurveParaCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetProfileCurveParaTyp stSetProfileCurvePara;

		public void Init(byte nAxisId, byte nParaIndx, byte nSubCmnd, byte nDir, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stSetProfileCurvePara));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 128;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSetProfileCurvePara.AxisID = nAxisId;
			stSetProfileCurvePara.paraIndx = nParaIndx;
			stSetProfileCurvePara.subCmnd = nSubCmnd;
			stSetProfileCurvePara.Dir = nDir;
			stSetProfileCurvePara.fPara = new float[8];
			Array.Clear(stSetProfileCurvePara.fPara, 0, 8);
		}

		public void SetVal(float[] fPara)
		{
			Array.Copy(fPara, stSetProfileCurvePara.fPara, 8);
		}
	}

	private struct ProfileMoveCmndType
	{
		private stCmndHeadType stCmndHead;

		private stmcCmnd_ProfileMoveCmndTyp stProfileMoveCmnd;

		public void Init(byte nAxisId, byte nParaIndx, byte nSubCmnd, byte nDir, float fPeriod, ushort nMaxSec, ushort nCycleCnt, ushort nPeriodicType, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stProfileMoveCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 128;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stProfileMoveCmnd.AxisID = nAxisId;
			stProfileMoveCmnd.subCmnd = nSubCmnd;
			stProfileMoveCmnd.paraIndx = nParaIndx;
			stProfileMoveCmnd.dDir = nDir;
			stProfileMoveCmnd.fPeriod = fPeriod;
			stProfileMoveCmnd.nMaxSec = nMaxSec;
			stProfileMoveCmnd.nMaxCycleNum = nCycleCnt;
			stProfileMoveCmnd.nPeriodicType = nPeriodicType;
		}
	}

	private struct GearCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMcCmnd_SetGearParaTyp stSetGearPara;

		public void Init(byte nCmndType, byte nMasterId, byte nSlvId, float fGear, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stSetGearPara));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSetGearPara.bMstrId = nMasterId;
			stSetGearPara.bSlvId = nSlvId;
			stSetGearPara.dummy1 = 0;
			stSetGearPara.fGear = fGear;
		}
	}

	private struct GCodeCmndType
	{
		private stCmndHeadType stCmndHead;

		public stGCodeCmmndTyp stGCode;

		public void Init(byte nCmndType, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stGCode));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
		}
	}

	private struct TriggerMoveCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMoveAndPulseType stMoveAndPulseCmnd;

		public void Init(byte nSubCmnd, byte nAxisId, byte nSpdDir, byte nPulseMode, byte nOutMode, byte nOutIndx, byte nPulseWidth, byte nPosMode, float fDist, float fVel, float fAcc, float fDistHead, float fDistTail, float fDistPulse, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stMoveAndPulseCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 140;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stMoveAndPulseCmnd.SubCmnd = nSubCmnd;
			stMoveAndPulseCmnd.AxisId = nAxisId;
			stMoveAndPulseCmnd.spdDir = nSpdDir;
			stMoveAndPulseCmnd.pulseMode = nPulseMode;
			stMoveAndPulseCmnd.SetOutMode = nOutMode;
			stMoveAndPulseCmnd.OutputIndex = nOutIndx;
			stMoveAndPulseCmnd.nPulseWidth = nPulseWidth;
			stMoveAndPulseCmnd.nPosMode = nPosMode;
			stMoveAndPulseCmnd.fDist = fDist;
			stMoveAndPulseCmnd.fVel = fVel;
			stMoveAndPulseCmnd.fAcc = fAcc;
			stMoveAndPulseCmnd.fDistHead = fDistHead;
			stMoveAndPulseCmnd.fDistTail = fDistTail;
			stMoveAndPulseCmnd.fDistPulse = fDistPulse;
		}
	}

	private struct SimpleHarmonicMoveCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMoveSineType stMoveSine;

		public void Init(byte nSubCmnd, byte nAxisId, byte nPosMode, uint nCycle, float fCoeA, float fCoeT, float fCoeFai, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stMoveSine));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 141;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stMoveSine.SubCmnd = nSubCmnd;
			stMoveSine.AxisId = nAxisId;
			stMoveSine.PosMode = nPosMode;
			stMoveSine.dummy = 0;
			stMoveSine.nCycle = nCycle;
			stMoveSine.fCoefA = fCoeA;
			stMoveSine.fCoeT = fCoeT;
			stMoveSine.fCoeFai = fCoeFai;
		}
	}

	private struct MoveAsSimpleHarmonicCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMoveSinPosType stMoveSinePos;

		public void Init(byte nSubCmnd, byte nAxisId, sbyte nSpdDir, float fPos, float fVel, float fAcc, float fCoeA, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stMoveSinePos));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 226;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stMoveSinePos.SubCmnd = nSubCmnd;
			stMoveSinePos.AxisID = nAxisId;
			stMoveSinePos.SpdDir = nSpdDir;
			stMoveSinePos.dummy = 0;
			stMoveSinePos.PosCmd = fPos;
			stMoveSinePos.VelCmd = fVel;
			stMoveSinePos.AccCmd = fAcc;
			stMoveSinePos.fCoefA = fCoeA;
		}
	}

	private struct CompensateSingleAxisCmndType
	{
		private stCmndHeadType stCmndHead;

		private stComTableCmndType stComTable;

		public void Init(byte nSubCmnd, byte nAxisId, ushort nParaIndex, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stComTable));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 142;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stComTable.SubCmnd = nSubCmnd;
			stComTable.AxisId = nAxisId;
			stComTable.ParaIndx = nParaIndex;
		}
	}

	private struct FollowExternalPosCmndType
	{
		private stCmndHeadType stCmndHead;

		private stExPosFollowCmndType stExPosFollow;

		public void Init(byte nSubCmnd, byte nAxisEnBit, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stExPosFollow));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 143;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stExPosFollow.SubCmnd = nSubCmnd;
			stExPosFollow.AxisEnBit = nAxisEnBit;
		}
	}

	private struct ScopeDeviceCmndType
	{
		private stCmndHeadType stCmndHead;

		private stScopeCmndType stScopeCmnd;

		public void Init(byte nSubCmnd, byte nAxisId, byte nChannelIndx, byte nTargetInfo, byte nChannelNum, byte nSampleTick, uint nSampleSum, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stScopeCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 129;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stScopeCmnd.SubCmnd = nSubCmnd;
			stScopeCmnd.AxisId = nAxisId;
			stScopeCmnd.ChannelIndx = nChannelIndx;
			stScopeCmnd.TargetInfo = nTargetInfo;
			stScopeCmnd.nChannelNum = nChannelNum;
			stScopeCmnd.SampleTick = nSampleTick;
			stScopeCmnd.SampleCnt = nSampleSum;
			stScopeCmnd.dummy1 = 0;
			stScopeCmnd.dummy2 = 0;
		}
	}

	private struct CompensatePlatformCmndType
	{
		private stCmndHeadType stCmndHead;

		private stGetPlatformLevInfoCmndType stGetPlatformLevInfoCmnd;

		public void Init(byte nSubCmnd, byte nParaIndx, byte xDirIndx, byte yDirIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stGetPlatformLevInfoCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 142;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stGetPlatformLevInfoCmnd.subCmnd = nSubCmnd;
			stGetPlatformLevInfoCmnd.paraIndx = nParaIndx;
			stGetPlatformLevInfoCmnd.xDirIndx = xDirIndx;
			stGetPlatformLevInfoCmnd.yDirIndx = yDirIndx;
		}
	}

	private struct EnablePlatformLevCmndType
	{
		private stCmndHeadType stCmndHead;

		private stPlatformComCmndType stPlatformCmnd;

		public void Init(byte nSubCmnd, byte xAxisId, byte yAxisId, byte zAxisId, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stPlatformCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 233;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stPlatformCmnd.subCmnd = nSubCmnd;
			stPlatformCmnd.xAxisID = xAxisId;
			stPlatformCmnd.yAxisID = yAxisId;
			stPlatformCmnd.zAxisID = zAxisId;
		}
	}

	private struct MotorDriverCmndType
	{
		private stCmndHeadType stCmndHead;

		private stMotorCmndType stMotorCmnd;

		public void Init(byte nSubCmnd, short nRegIndx, byte nCardIndx = 0)
		{
			byte b = (byte)(4 + Marshal.SizeOf((object)stMotorCmnd));
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = 64;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stMotorCmnd.nSubCmnd = nSubCmnd;
			stMotorCmnd.nRegIndex = nRegIndx;
			stMotorCmnd.dummy = 0;
			stMotorCmnd.IVal = 0;
		}

		public void SetVal(float fVal)
		{
			stMotorCmnd.fVal = fVal;
		}

		public void SetVal(uint UIVal)
		{
			stMotorCmnd.UIVal = UIVal;
		}

		public void SetVal(int IVal)
		{
			stMotorCmnd.IVal = IVal;
		}
	}

	private struct NoParaCmndType
	{
		private stCmndHeadType stCmndHead;

		public void Init(byte nCmndType, byte nCardIndx = 0)
		{
			byte b = 4;
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
		}
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct stReturnValType
	{
		[FieldOffset(0)]
		public float fVal;

		[FieldOffset(0)]
		public int IVal;

		[FieldOffset(0)]
		public uint IUVal;

		[FieldOffset(0)]
		public short IWVal1;

		[FieldOffset(2)]
		public short IWVal2;
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct stUploadNcSttType
	{
		[FieldOffset(0)]
		public byte u8CmndtType;

		[FieldOffset(1)]
		public byte u8SubCmnd;

		[FieldOffset(2)]
		public byte u8Len;

		[FieldOffset(3)]
		public byte u8CmndStt;

		[FieldOffset(4)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public stReturnValType[] Val;

		public void Init()
		{
			Val = new stReturnValType[64];
		}
	}

	private struct stMcCmnd_ManulOpTyp
	{
		public byte SubCmnd;

		public byte AxisID;

		public sbyte SpdDir;

		public byte dummy;

		public float PosCmd;

		public float VelCmd;

		public float AccCmd;
	}

	private struct stMcCmnd_ReadParaTyp
	{
		public byte AxisId;

		public byte ParaIndex;
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct stMcCmnd_SetParaTyp
	{
		[FieldOffset(0)]
		public byte AxisId;

		[FieldOffset(1)]
		public byte ParaIndex;

		[FieldOffset(2)]
		public byte dummy1;

		[FieldOffset(3)]
		public byte dummy2;

		[FieldOffset(4)]
		public float fVal;

		[FieldOffset(4)]
		public int IVal;

		[FieldOffset(4)]
		public uint UIVal;
	}

	private struct stMcCmnd_AskSttInfoTyp
	{
		public byte AxisId;

		public byte ParaIndex;
	}

	private struct stMcCmnd_RestPosTyp
	{
		public byte SubCmnd;

		public byte AxisID;

		public ushort dummy;

		public float PosCmd;
	}

	private struct stMcCmnd_SetOutputTyp
	{
		public byte SubCmnd;

		public byte OutputIndex;

		public ushort dummy;
	}

	private struct stMcCmnd_SetAnaEnTyp
	{
		public byte AxisID;

		public byte bEn;
	}

	private struct stMcCmnd_EnableSomeFunctionTyp
	{
		public byte AxisID;

		public byte FunType;

		public byte bEnOrNot;

		public byte dummy;
	}

	private struct stMcCmnd_SetEncoderCmndTyp
	{
		public byte AxisID;

		public byte dummy1;

		public ushort dummy2;

		public int EnCoderVal;
	}

	private struct stMcCmnd_ClearInternalPulseCntTyp
	{
		public byte AxisID;
	}

	private struct stMcCmnd_SetProfileCurveParaTyp
	{
		public byte AxisID;

		public byte paraIndx;

		public byte subCmnd;

		public byte Dir;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public float[] fPara;
	}

	private struct stmcCmnd_ProfileMoveCmndTyp
	{
		public byte AxisID;

		public byte paraIndx;

		public byte subCmnd;

		public byte dDir;

		public float fPeriod;

		public ushort nMaxSec;

		public ushort nMaxCycleNum;

		public ushort nPeriodicType;
	}

	private struct stMcCmnd_SetGearParaTyp
	{
		public byte bMstrId;

		public byte bSlvId;

		public ushort dummy1;

		public float fGear;
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct stGCodeCmmndTyp
	{
		[FieldOffset(0)]
		public byte GCode;

		[FieldOffset(1)]
		public byte AxisEn;

		[FieldOffset(2)]
		public byte CmndIndx;

		[FieldOffset(3)]
		public byte CmndMode;

		[FieldOffset(4)]
		public float TargtPosX;

		[FieldOffset(4)]
		public uint WaitInputs;

		[FieldOffset(8)]
		public float TargtPosY;

		[FieldOffset(12)]
		public float TargtPosZ;

		[FieldOffset(16)]
		public float TargtPosA;

		[FieldOffset(20)]
		public float TargtPosB;

		[FieldOffset(24)]
		public float TargtPosC;

		[FieldOffset(28)]
		public float CmmndVelX;

		[FieldOffset(32)]
		public float CmmndVelY;

		[FieldOffset(36)]
		public float CmmndVelZ;

		[FieldOffset(40)]
		public float CmmndVelA;

		[FieldOffset(44)]
		public float CmmndVelB;

		[FieldOffset(48)]
		public float CmmndVelC;

		[FieldOffset(52)]
		public float TargtAccX;

		[FieldOffset(56)]
		public float TargtAccY;

		[FieldOffset(60)]
		public float TargtAccZ;

		[FieldOffset(64)]
		public float TargtAccA;

		[FieldOffset(68)]
		public float TargtAccB;

		[FieldOffset(72)]
		public float TargtAccC;

		[FieldOffset(76)]
		public float Radius;

		[FieldOffset(80)]
		public float DelayTime;
	}

	private struct stMoveAndPulseType
	{
		public byte SubCmnd;

		public byte AxisId;

		public byte spdDir;

		public byte pulseMode;

		public byte SetOutMode;

		public byte OutputIndex;

		public byte nPulseWidth;

		public byte nPosMode;

		public float fDist;

		public float fVel;

		public float fAcc;

		public float fDistHead;

		public float fDistTail;

		public float fDistPulse;
	}

	private struct stMoveSineType
	{
		public byte SubCmnd;

		public byte AxisId;

		public byte PosMode;

		public byte dummy;

		public uint nCycle;

		public float fCoefA;

		public float fCoeT;

		public float fCoeFai;
	}

	private struct stMoveSinPosType
	{
		public byte SubCmnd;

		public byte AxisID;

		public sbyte SpdDir;

		public byte dummy;

		public float PosCmd;

		public float VelCmd;

		public float AccCmd;

		public float fCoefA;
	}

	private struct stScopeCmndType
	{
		public byte SubCmnd;

		public byte AxisId;

		public byte ChannelIndx;

		public byte TargetInfo;

		public byte nChannelNum;

		public byte SampleTick;

		public byte dummy1;

		public byte dummy2;

		public uint SampleCnt;
	}

	private struct stComTableCmndType
	{
		public byte SubCmnd;

		public byte AxisId;

		public ushort ParaIndx;
	}

	private struct stGetPlatformLevInfoCmndType
	{
		public byte subCmnd;

		public byte paraIndx;

		public byte xDirIndx;

		public byte yDirIndx;
	}

	private struct stExPosFollowCmndType
	{
		public byte SubCmnd;

		public byte AxisEnBit;
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct stMotorCmndType
	{
		[FieldOffset(0)]
		public byte nSubCmnd;

		[FieldOffset(1)]
		public byte dummy;

		[FieldOffset(2)]
		public short nRegIndex;

		[FieldOffset(4)]
		public float fVal;

		[FieldOffset(4)]
		public uint UIVal;

		[FieldOffset(4)]
		public int IVal;
	}

	private struct stLuaFileHeadType
	{
		public ushort fileId;

		public ushort fileLen;

		private uint dummy1;

		private uint dummy2;

		private uint dummy3;

		private uint dummy4;

		private uint dummy5;

		private uint dummy6;

		private ushort dummy8;

		public ushort CrcCheck;
	}

	private enum eLuaResultType
	{
		eLuaResNull,
		eLuaResFileMaxLen,
		eLuaResFileName,
		eLuaResFileEmpty,
		eLuaResFileSendErr,
		eLuaResFileException,
		eSpiResMaxLen,
		eLuaResDoStringLen
	}

	private struct stLuaSubManualCmndInfoType
	{
		public byte nSubCmnd;

		public byte dummy;

		public ushort fID;
	}

	private struct stLuaDoStringArrayType
	{
		public byte nSubCmnd;

		public byte dummy1;

		public byte dummy2;

		public byte dummy3;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public char[] val;
	}

	private struct stLuaManualCmndType
	{
		public stCmndHeadType stCmndHead;

		public stLuaSubManualCmndInfoType stManualCmnd;

		public void Init(byte nCmndType, byte nSubCmnd, ushort nFID, byte nCardIndx = 0)
		{
			byte b = (byte)(Marshal.SizeOf((object)stManualCmnd) + 4);
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stManualCmnd.nSubCmnd = nSubCmnd;
			stManualCmnd.fID = nFID;
			stManualCmnd.dummy = 0;
		}
	}

	private struct stLuaDoStringCmndType
	{
		public stCmndHeadType stCmndHead;

		public stLuaDoStringArrayType stLuaDoString;

		public void Init(byte nCmndType, byte nSubCmnd, byte nLen, byte nCardIndx = 0)
		{
			byte b = (byte)(nLen + 4 + 4);
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stLuaDoString.nSubCmnd = nSubCmnd;
			stLuaDoString.dummy1 = 0;
			stLuaDoString.dummy2 = 0;
			stLuaDoString.dummy3 = 0;
			stLuaDoString.val = new char[128];
			Array.Clear(stLuaDoString.val, 0, nLen);
		}

		public eLuaResultType CopyDt(byte[] nSource, int nLen)
		{
			eLuaResultType result = eLuaResultType.eLuaResNull;
			if (nLen > 128)
			{
				result = eLuaResultType.eLuaResDoStringLen;
			}
			else
			{
				Array.Copy(nSource, stLuaDoString.val, nLen);
			}
			return result;
		}
	}

	private struct stSPIFlashSubWriteCmndType
	{
		public byte nSubCmnd;

		public byte dummy1;

		public byte dummy2;

		public byte dummy3;

		public ushort nPageIndx;

		public ushort nDtLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 240)]
		public byte[] val;
	}

	private struct stSPIFlashSubReadCmndType
	{
		public byte nSubCmnd;

		public byte dummy1;

		public ushort nPageIndx;

		public ushort nDtLen;
	}

	private struct stSPIFlashWriteCmndType
	{
		public stCmndHeadType stCmndHead;

		public stSPIFlashSubWriteCmndType stSPIFlashWr;

		public void Init(byte nCmndType, byte nSubCmnd, ushort nPagIndx, ushort nLen, byte nCardIndx = 0)
		{
			byte b = (byte)(nLen + 8 + 4);
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSPIFlashWr.nSubCmnd = nSubCmnd;
			stSPIFlashWr.nPageIndx = nPagIndx;
			stSPIFlashWr.nDtLen = b;
			stSPIFlashWr.dummy1 = 0;
			stSPIFlashWr.dummy2 = 0;
			stSPIFlashWr.dummy3 = 0;
			stSPIFlashWr.val = new byte[240];
			Array.Clear(stSPIFlashWr.val, 0, 240);
		}

		public eLuaResultType CopyDt(byte[] nSource, int nLen)
		{
			eLuaResultType result = eLuaResultType.eLuaResNull;
			if (nLen > 240)
			{
				result = eLuaResultType.eSpiResMaxLen;
			}
			else
			{
				Array.Copy(nSource, stSPIFlashWr.val, nLen);
			}
			return result;
		}
	}

	private struct stSPIFlashReadCmndType
	{
		public stCmndHeadType stCmndHead;

		public stSPIFlashSubReadCmndType stSPIFlashRd;

		public void Init(byte nCmndType, byte nSubCmnd, ushort nPagIndx, ushort nLen, byte nCardIndx = 0)
		{
			byte b = (byte)(Marshal.SizeOf((object)stSPIFlashRd) + 4);
			stCmndHead.u8SpCom_Headr = 170;
			stCmndHead.u8SpCom_DtLen = b;
			stCmndHead.u8SpCom_Check = (byte)(~b);
			stCmndHead.u8SpCom_CardIndx = nCardIndx;
			stCmndHead.u8SpDt_CmndType = nCmndType;
			stCmndHead.u8SpDt_CmndNo = 0;
			stCmndHead.u8SpDt_Len = b;
			stCmndHead.u8SpDt_CheckSum = 0;
			stSPIFlashRd.nSubCmnd = nSubCmnd;
			stSPIFlashRd.nPageIndx = nPagIndx;
			stSPIFlashRd.nDtLen = nLen;
			stSPIFlashRd.dummy1 = 0;
		}
	}

	private const uint DLL_VER = 539035650u;

	private const byte CMMND_MDICMND = 241;

	private const byte CMMND_SETPARA = 242;

	private const byte CMMND_GCODECMND = 243;

	private const byte CMMND_MANUAL_OP = 244;

	private const byte CMMND_ASKPARA = 245;

	private const byte CMMND_ASKMOVEINFO = 246;

	private const byte CMMND_SYSTEM_OP = 247;

	private const byte CMMND_RESETPOS = 248;

	private const byte CMMND_SETOUTPUT = 249;

	private const byte CMMND_ANALOGOP = 250;

	private const byte CMMND_SETINPUTLEV = 251;

	private const byte CMMND_SETENCODER = 252;

	private const byte CMMND_RESETZ = 253;

	private const byte CMMND_SAVESYSPARA = 254;

	private const byte CMMND_COMPENSATE = 208;

	private const byte CMMND_CLRPULSECNTINTERNAL = 209;

	private const byte CMMND_SETGEAR_PARA = 224;

	private const byte CMMND_GEAR_OP = 225;

	private const byte CMMND_SINPOS_MOVE = 226;

	private const byte CMMND_LUA = 227;

	private const byte CMMND_SPIFLASH = 228;

	private const byte CMMND_DS402HOME = 232;

	private const byte CMMND_PLATFORM_COM = 233;

	private const byte Cmnd_ProfileMove_Cmnd = 128;

	private const byte Cmnd_Scope_Cmnd = 129;

	private const byte CMMND_OFFLINERUN_PARA = 136;

	private const byte CMMND_OFFLINERUN_START = 137;

	private const byte CMMND_OFFLINERUN_STOP = 138;

	private const byte CMMND_OFFLINERUN_CLEAR = 139;

	private const byte CMMND_MOVEPULSE = 140;

	private const byte CMMND_MOVESINE = 141;

	private const byte CMMND_BOARD_MAINTIANCE = 142;

	private const byte CMMND_EXPOS_FOLLOW = 143;

	private const byte CMMND_TMC2209 = 64;

	private const byte MANOP_MOVEINNERDIST = 0;

	private const byte MANOP_MOVERELDISTNOPARA = 1;

	private const byte MANOP_MOVEABSDISTNOPARA = 2;

	private const byte MANOP_MOVERELDIST = 3;

	private const byte MANOP_MOVEABSDIST = 4;

	private const byte MANOP_SPDMOVENOPARA = 5;

	private const byte MANOP_SPDMOVE = 6;

	private const byte MANOP_MOVECHAGEVEL = 7;

	private const byte MANOP_SPDMOVEWITHINTIME = 8;

	private const byte MANOP_MOVEABS_SYNCEXIN = 9;

	private const byte MANOP_MOVEREL_SYNCEXIN = 10;

	private const byte MANOP_UPDATECOMPENSATE = 32;

	private const byte MANOP_EMGERNCYSTOP = 128;

	private const byte MANOP_PARASTOP = 129;

	private const byte MANOP_PAUSE = 130;

	private const byte MANOP_RESTARTMOVE = 144;

	private const byte MANOP_QUITEMOVE = 145;

	private const byte MANOP_HOMINGNOPARA = 160;

	private const byte MANOP_HOMING = 161;

	private const byte MANOP_HOMINGCANCLE = 162;

	private const byte MANOP_HOMINGWITHINDIST = 163;

	private const byte MANOP_HOMINGWITHSWIPT = 164;

	private const byte RESETPOS_ABS = 1;

	private const byte RESETPOS_REL = 2;

	private const byte MANAGE_SETOUTPUT = 1;

	private const byte subCmnd_SetSigLev_HardLimitEn = 1;

	private const byte subCmnd_SetSigLev_SoftLimitEn = 2;

	private const byte subCmnd_SetSigLev_ComEn = 3;

	private const byte subCmnd_setSigLev_LimitLev = 4;

	private const byte subCmnd_SetSigLev_OriLev = 5;

	private const byte subCmnd_SetSigLev_ComTable = 6;

	private const byte SubCmnd_SetSigLev_HomeMode = 7;

	private const byte subCmnd_Profile_SetProfile = 0;

	private const byte subCmnd_Profile_SetPara = 1;

	private const byte subCmnd_Profile_Start = 2;

	private const byte subCmnd_Profile_Stop = 3;

	private const byte subCmnd_Scope_Set = 0;

	private const byte subCmnd_Scope_Reset = 1;

	private const byte subCmnd_Scope_Start = 2;

	private const byte subCmnd_Scope_Stop = 3;

	private const byte subCmnd_BoardMtn_SetComTable = 0;

	private const byte subCmnd_BoardMtn_ReadComTable = 1;

	private const byte subCmnd_BoardMtn_ReadPlatformTable = 2;

	private const byte subCmnd_ExPosFollow_Start = 1;

	private const byte subCmnd_ExPosFollow_Stop = 2;

	private const byte subCmnd_Lua_CmndSendRun = 1;

	private const byte subCmnd_Lua_RunFileRun = 2;

	private const byte subCmnd_Lua_StopFileRun = 3;

	private const byte subCmnd_Lua_PauseFileRun = 4;

	private const byte subCmnd_Lua_ContinueFileRun = 5;

	private const byte subCmnd_Lua_GetRunningInfo = 16;

	private const byte subCmnd_TMC2209_RegWrite = 1;

	private const byte subCmnd_TMC2209_RegRead = 2;

	private const byte subCmnd_TMC2209_RegSave = 3;

	private const byte subCmnd_TMC2209_Enable = 4;

	private const byte subCmnd_TMC2209_Disable = 5;

	private const byte subCmnd_TMC2209_Reset = 6;

	private const byte subCmnd_TMC2209_SetInputMode = 7;

	public readonly short MotorDrv_ParaIndex_InputFun = 256;

	public readonly short MotorDrv_ParaIndex_Pulse = 257;

	public readonly short MotorDrv_ParaIndex_IRun = 258;

	public readonly short MotorDrv_ParaIndex_IHold = 259;

	public readonly short MotorDrv_TMCState = 260;

	public readonly short MotorDrv_TMCAlarm = 261;

	private const byte AskInfo_Pos_Each = 0;

	private const byte AskInfo_Spd_Each = 1;

	private const byte AskInfo_Ana_Each = 2;

	private const byte AskInfo_Encdr_Each = 3;

	private const byte AskInfo_HomStt_Each = 4;

	private const byte AskInfo_Input_L = 5;

	private const byte AskInfo_Input_H = 6;

	private const byte AskInfo_Output_L = 7;

	private const byte AskInfo_Output_H = 8;

	private const byte AskInfo_Intrupt_L = 9;

	private const byte AskInfo_Intrupt_H = 10;

	private const byte AskInfo_NcStt = 11;

	private const byte AskInfo_HomeStt = 12;

	private const byte AskInfo_ComDiff_Each = 13;

	private const byte AskInfo_LimitStt_Each = 14;

	private const byte AskInfo_OfflineMove = 32;

	private const byte AskInfo_MovePulseCnt = 33;

	private const byte AskInfo_MovePulsePos = 34;

	private const byte AskInfo_MovePulseInfo = 35;

	private const byte AskInfo_Pos_Grp = 128;

	private const byte AskInfo_Spd_Grp = 129;

	private const byte AskInfo_Ana_Grp = 130;

	private const byte AskInfo_Encdr_Grp = 131;

	private const byte AskInfo_ComDiff_Grp = 132;

	private const byte AskInfo_LimitStt_Grp = 133;

	private const byte AskInfo_CustomInfo_wy1 = 176;

	private const byte AskInfo_McuId = 224;

	private const byte AskInfo_AllCfgInfo = 254;

	private const byte AskInfo_Version = byte.MaxValue;

	private const byte OutputSet = 1;

	private const byte OutputReset = 2;

	private const byte MCU_CMND_EXECUTE_OK = 0;

	public readonly ushort FunResOk = 1;

	public readonly ushort FunResErr = 2;

	private const byte CMND_INFO_NOTHING = 0;

	private const byte CMND_INFO_SYSRUNNING = 1;

	private const byte CMND_INFO_INDEXERROR = 2;

	private const byte CMND_INFO_CMNDCODEERR = 3;

	private const byte CMND_INFO_OUTPUTINDEX = 4;

	private const byte CMND_INFO_OPCMNDINDEX = 5;

	private const byte CMND_INFO_JIAOYANHE = 6;

	private const byte CMND_INFO_INVALIDCMND = 7;

	private const byte SPCOMFRAMEHEADR = 170;

	private const byte SPCOMFRAMEHEADRLEN = 4;

	private const byte SPCOMDATAHEADRLEN = 4;

	private const byte SPCOMNCSTTHEADRLEN = 4;

	private const byte spComFrameCheckSumPos = 4;

	private const byte FRAME_HEAD = 245;

	private const byte FRAME_TAIL = 90;

	private const byte INPUT_CMMND_SIZE = 12;

	private const byte OUTPUT_CMMND_SIZE = 14;

	private const byte subCmnd_PlatformLevel_Stop = 0;

	private const byte subCmnd_PlatformLevel_Start = 1;

	public readonly uint Axis1IntrpltSttBit = 1u;

	public readonly uint Axis1DelaySttBit = 2u;

	public readonly uint Axis1RunSttBit = 4u;

	public readonly uint Axis2IntrpltSttBit = 16u;

	public readonly uint Axis2DelaySttBit = 32u;

	public readonly uint Axis2RunSttBit = 64u;

	public readonly uint Axis3IntrpltSttBit = 256u;

	public readonly uint Axis3DelaySttBit = 512u;

	public readonly uint Axis3RunSttBit = 1024u;

	public readonly uint Axis4IntrpltSttBit = 4096u;

	public readonly uint Axis4DelaySttBit = 8192u;

	public readonly uint Axis4RunSttBit = 16384u;

	public readonly uint Axis5IntrpltSttBit = 65536u;

	public readonly uint Axis5DelaySttBit = 131072u;

	public readonly uint Axis5RunSttBit = 262144u;

	public readonly uint Axis6IntrpltSttBit = 1048576u;

	public readonly uint Axis6DelaySttBit = 2097152u;

	public readonly uint Axis6RunSttBit = 4194304u;

	public readonly uint GrpIntrpltSttBit = 16777216u;

	public readonly uint GrpDelaySttBit = 33554432u;

	public readonly uint GrpRunSttBit = 67108864u;

	public readonly uint NcCtrlStt = 268435456u;

	public readonly int ParaIndex_ManDist = 0;

	public readonly int ParaIndex_HighSpd = 1;

	public readonly int ParaIndex_LowSpd = 2;

	public readonly int ParaIndex_HomeSpd = 3;

	public readonly int ParaIndex_Acc = 4;

	public readonly int ParaIndex_Pulse = 5;

	public readonly int ParaIndex_HomeDist = 6;

	public readonly int ParaIndex_MotortInputLev = 7;

	public readonly int ParaIndex_AnaEn = 8;

	public readonly int ParaIndex_AnaZeroScope = 9;

	public readonly int ParaIndex_ComWdith = 10;

	public readonly int ParaIndex_ComScope = 11;

	public readonly int ParaIndex_EncPulse = 12;

	public readonly int ParaIndex_CmndEncGear = 13;

	public readonly int ParaIndex_SoftLimitNeg = 14;

	public readonly int ParaIndex_SoftLimitPos = 15;

	public readonly int ParaIndex_BaseXAxis = 0;

	public readonly int ParaIndex_BaseYAxis = 16;

	public readonly int ParaIndex_BaseZAxis = 32;

	public readonly int ParaIndex_BaseAAxis = 48;

	public readonly int ParaIndex_BaseBAxis = 64;

	public readonly int ParaIndex_BaseCAxis = 80;

	public readonly int ParaIDBase_AbsEncoder = 10;

	public readonly int AbsEncParaIndex_ClkScale0 = 0;

	public readonly int AbsEncParaIndex_ClkTm0 = 1;

	public readonly int AbsEncParaIndex_DataBit0 = 2;

	public readonly int AbsEncParaIndex_DataFormat0 = 3;

	public readonly int AbsEncParaIndex_ClkScale1 = 4;

	public readonly int AbsEncParaIndex_ClkTm1 = 5;

	public readonly int AbsEncParaIndex_DataBit1 = 6;

	public readonly int AbsEncParaIndex_DataFormat1 = 7;

	public readonly int AbsEncParaIndex_EncChoose = 8;

	public readonly byte ParaIDBase_InputExFun = 11;

	public readonly byte InputExFunParaIndex_MaskL = 0;

	public readonly byte InputExFunParaIndex_MaskH = 1;

	public readonly byte ParaIDBase_SysCtrlPara = 12;

	public readonly byte SysCtrlPara_RecoverDeafult = 0;

	public readonly byte SysCtrlPara_HandWheelMode = 1;

	public readonly byte SysCtrlPara_ScanDelay = 2;

	public readonly int BitMask_LimitNeg = 1;

	public readonly int BitMask_LimitPos = 2;

	public readonly int BitMask_LimitSig = 3;

	public readonly int BitMask_HomeSig = 4;

	public readonly int BitMask_ComTable = 16;

	public readonly int BitMask_SoftLimit = 32;

	public readonly int BitMask_Com = 64;

	public readonly int BitMask_HardLimit = 128;

	public readonly int BitMask_HomeMode = 768;

	public readonly byte ParaClassAbsEnc = 10;

	public readonly byte ParaClassInputCfgReg = 11;

	public readonly byte ParaClassSysCtrlReg = 12;

	public readonly byte ParaClassAxisExReg = 13;

	public readonly byte ParaClassAxisExRegAbsOffset = 14;

	public bool CommLinkEnable;

	private byte MccCardNo = 0;

	private const byte MAX_AXIS_NUM = 6;

	private const byte PARA_NUM_PER_AXIS = 16;

	private const byte PARA_SUM = 96;

	public readonly byte MaxAxisNum = 6;

	public readonly byte ParaNumPerAxis = 16;

	public readonly byte ParaSum = 96;

	public readonly byte EncParaSum = 9;

	private const byte GroupMoveModeMask_Group = 48;

	private readonly byte GroupMoveModeMask_SyncExIn = 128;

	private readonly byte GroupMoveModeMask_Axis = 15;

	public float[] gbAxisPos = new float[6];

	public float[] gbAxisSpd = new float[6];

	public int[] gbAxisEnc = new int[6];

	public int[] gbComDiff = new int[6];

	public uint[] gbMovePulseCnt = new uint[6];

	public float[] gbMovePulsePos = new float[6];

	public uint[] gbLimitStt = new uint[6];

	public uint gbInput;

	public uint gbOutput;

	public uint gbRunState;

	private const ushort LuaFileBufMaxLen = 4096;

	private const ushort LuaFileEachSendLen = 256;

	private const ushort LuaFileMaxLen = 4062;

	private eLuaResultType gbLuaRes = eLuaResultType.eLuaResNull;

	private const byte subCmnd_SpiFlash_Write = 1;

	private const byte subCmnd_SpiFlash_Read = 2;

	private const byte SPI_WRITE_BUFF_SIZE = 240;

	public stUploadNcSttType stNcStt = default(stUploadNcSttType);

	private byte[] CommSendBuffer = new byte[512];

	private byte[] CommReceiveBuffer = new byte[512];

	private int SpIntrThrdHod;

	private int DtLenWanted;

	private SpDataShow mDataShow;

	private bool bSpDataShowFlag;

	private DataExchange mCommPort;

	public SPLibClass()
	{
		CommLinkEnable = false;
		SpIntrThrdHod = 1;
		MaxAxisNum = 6;
		ParaNumPerAxis = 16;
		ParaSum = 96;
		stNcStt.Init();
		mDataShow = new SpDataShow();
		bSpDataShowFlag = false;
		mCommPort = new DataExchange();
	}

	~SPLibClass()
	{
		mCommPort.Fun_CloseLink();
	}

	private ushort Fun_DeCodeCmmndState(byte[] revBuf)
	{
		uint num = 0u;
		ushort num2;
		for (num2 = 0; num2 < DtLenWanted + 4; num2++)
		{
			num += revBuf[num2];
		}
		num = (byte)num;
		if (revBuf[num2] == num)
		{
			stNcStt = (stUploadNcSttType)BytesToStruct(revBuf, typeof(stUploadNcSttType));
			if (stNcStt.u8CmndStt == 0)
			{
				return FunResOk;
			}
			return FunResErr;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_SendMDICommand(string MDICmndStr)
	{
		return MoCtrCard_SendMDICommand(MDICmndStr, 0, 48);
	}

	public ushort MoCtrCard_SendMDICommand(string MDICmndStr, byte CmndIndx = 0, byte GrpOrAxis = 48)
	{
		GCodeCmndType gCodeCmndType = default(GCodeCmndType);
		string[] array = new string[28]
		{
			"^G[ ]*[0-9]*", "[Q]?X[ ]*-?[0-9]+\\.?[0-9]*", "FX[ ]*-?[0-9]+\\.?[0-9]*", "ACCX[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?Y[ ]*-?[0-9]+\\.?[0-9]*", "FY[ ]*-?[0-9]+\\.?[0-9]*", "ACCY[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?Z[ ]*-?[0-9]+\\.?[0-9]*", "FZ[ ]*-?[0-9]+\\.?[0-9]*", "ACCZ[ ]*-?[0-9]+\\.?[0-9]*",
			"[Q]?A[ ]*-?[0-9]+\\.?[0-9]*", "FA[ ]*-?[0-9]+\\.?[0-9]*", "ACCA[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?B[ ]*-?[0-9]+\\.?[0-9]*", "FB[ ]*-?[0-9]+\\.?[0-9]*", "ACCB[ ]*-?[0-9]+\\.?[0-9]*", "[0-9]+C[ ]*-?[0-9]+\\.?[0-9]*", "FC[ ]*-?[0-9]+\\.?[0-9]*", "ACCC[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?F[ ]*-?[0-9]+\\.?[0-9]*",
			"[Q]?ACC[ ]*-?[0-9]+\\.?[0-9]*", "R[ ]*-?[0-9]+\\.?[0-9]*", "D[ ]*-?[0-9]+\\.?[0-9]*", "CLEAR[ ]*[0-9]+", "SET[ ]*[0-9]+", "HOME[ ]*([X|Y|Z|T][0|1|2|3])+", "WAIT[ ]*-?[0-9]+\\.?[0-9]*", "SYNC[ ]*-?[0-9]+\\.?[0-9]*"
		};
		string pattern = "[^a-z|A-Z][0-9|\\.]*";
		ushort num = 0;
		if (0 < CmndIndx)
		{
			gCodeCmndType.Init(136, MccCardNo);
		}
		else
		{
			gCodeCmndType.Init(243, MccCardNo);
		}
		gCodeCmndType.stGCode.AxisEn = 0;
		gCodeCmndType.stGCode.CmndIndx = CmndIndx;
		gCodeCmndType.stGCode.CmndMode = GrpOrAxis;
		for (num = 0; num < array.Length; num++)
		{
			MatchCollection matchCollection = Regex.Matches(MDICmndStr, array[num]);
			if (matchCollection.Count > 0)
			{
				MatchCollection matchCollection2 = Regex.Matches(matchCollection[0].Value.ToString(), pattern);
				string text = ((matchCollection2.Count <= 1) ? matchCollection2[0].Value.ToString() : matchCollection2[1].Value.ToString());
				if (num == 0)
				{
					gCodeCmndType.stGCode.GCode = byte.Parse(text.Trim());
					if (gCodeCmndType.stGCode.GCode >= 80)
					{
						gCodeCmndType.stGCode.GCode -= 80;
						gCodeCmndType.stGCode.GCode += 128;
					}
				}
				else if (1 == num)
				{
					gCodeCmndType.stGCode.TargtPosX = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 1;
				}
				else if (2 == num)
				{
					gCodeCmndType.stGCode.CmmndVelX = float.Parse(text.Trim());
				}
				else if (3 == num)
				{
					gCodeCmndType.stGCode.TargtAccX = float.Parse(text.Trim());
				}
				else if (4 == num)
				{
					gCodeCmndType.stGCode.TargtPosY = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 2;
				}
				else if (5 == num)
				{
					gCodeCmndType.stGCode.CmmndVelY = float.Parse(text.Trim());
				}
				else if (6 == num)
				{
					gCodeCmndType.stGCode.TargtAccY = float.Parse(text.Trim());
				}
				else if (7 == num)
				{
					gCodeCmndType.stGCode.TargtPosZ = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 4;
				}
				else if (8 == num)
				{
					gCodeCmndType.stGCode.CmmndVelZ = float.Parse(text.Trim());
				}
				else if (9 == num)
				{
					gCodeCmndType.stGCode.TargtAccZ = float.Parse(text.Trim());
				}
				else if (10 == num)
				{
					gCodeCmndType.stGCode.TargtPosA = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 8;
				}
				else if (11 == num)
				{
					gCodeCmndType.stGCode.CmmndVelA = float.Parse(text.Trim());
				}
				else if (12 == num)
				{
					gCodeCmndType.stGCode.TargtAccA = float.Parse(text.Trim());
				}
				else if (13 == num)
				{
					gCodeCmndType.stGCode.TargtPosB = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 16;
				}
				else if (14 == num)
				{
					gCodeCmndType.stGCode.CmmndVelB = float.Parse(text.Trim());
				}
				else if (15 == num)
				{
					gCodeCmndType.stGCode.TargtAccB = float.Parse(text.Trim());
				}
				else if (16 == num)
				{
					gCodeCmndType.stGCode.TargtPosC = float.Parse(text.Trim());
					gCodeCmndType.stGCode.AxisEn |= 32;
				}
				else if (17 == num)
				{
					gCodeCmndType.stGCode.CmmndVelC = float.Parse(text.Trim());
				}
				else if (18 == num)
				{
					gCodeCmndType.stGCode.TargtAccC = float.Parse(text.Trim());
				}
				else if (19 == num)
				{
					gCodeCmndType.stGCode.CmmndVelX = float.Parse(text.Trim());
				}
				else if (20 == num)
				{
					gCodeCmndType.stGCode.TargtAccX = float.Parse(text.Trim());
				}
				else if (21 == num)
				{
					gCodeCmndType.stGCode.Radius = float.Parse(text.Trim());
				}
				else if (22 == num)
				{
					gCodeCmndType.stGCode.DelayTime = float.Parse(text.Trim());
				}
				else if (23 == num)
				{
					gCodeCmndType.stGCode.GCode = 5;
					gCodeCmndType.stGCode.TargtPosX = float.Parse(text.Trim());
					gCodeCmndType.stGCode.CmmndVelX = 0f;
				}
				else if (24 == num)
				{
					gCodeCmndType.stGCode.GCode = 5;
					gCodeCmndType.stGCode.TargtPosX = float.Parse(text.Trim());
					gCodeCmndType.stGCode.CmmndVelX = 1f;
				}
				else if (25 == num)
				{
					gCodeCmndType.stGCode.GCode = 6;
					if (text.IndexOf("0") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 1;
					}
					if (text.IndexOf("1") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 2;
					}
					if (text.IndexOf("2") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 4;
					}
					if (text.IndexOf("3") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 8;
					}
					if (text.IndexOf("4") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 16;
					}
					if (text.IndexOf("5") >= 0)
					{
						gCodeCmndType.stGCode.AxisEn |= 32;
					}
				}
				else if (26 == num)
				{
					gCodeCmndType.stGCode.GCode = 7;
					gCodeCmndType.stGCode.WaitInputs = uint.Parse(text.Trim());
				}
				else if (27 == num)
				{
					gCodeCmndType.stGCode.CmndMode |= GroupMoveModeMask_SyncExIn;
				}
			}
		}
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(gCodeCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	private ushort MoCtrCard_SendCmmndBufViaComm(byte[] sendBuf)
	{
		ushort num = 0;
		ushort result = FunResErr;
		if (bSpDataShowFlag)
		{
			mDataShow.mPrintStr("Send(" + sendBuf.Length.ToString("D3") + "):");
			mDataShow.mPrintHex(sendBuf, (ushort)sendBuf.Length);
		}
		for (num = 0; num < 4; num++)
		{
			if (mCommPort.Fun_SendBuffer(sendBuf, sendBuf.Length, 0) && mCommPort.Fun_GetBuffer(SpIntrThrdHod, CommReceiveBuffer))
			{
				if (bSpDataShowFlag)
				{
					mDataShow.mPrintStr("Receive(" + SpIntrThrdHod.ToString("D3") + "):");
					mDataShow.mPrintHex(CommReceiveBuffer, (ushort)SpIntrThrdHod);
				}
				if (FunResOk == Fun_DeCodeCmmndState(CommReceiveBuffer))
				{
					CommLinkEnable = true;
					result = FunResOk;
					break;
				}
				CommLinkEnable = true;
			}
			if (num >= 3)
			{
				CommLinkEnable = false;
				break;
			}
		}
		return result;
	}

	public ushort MoCtrCard_Initial(string IPAddr, int nPort)
	{
		ushort funResErr = FunResErr;
		try
		{
			if (mCommPort.Fun_CreateLink_TCP(IPAddr, nPort))
			{
				float[] resPos = new float[6];
				if (FunResOk == MoCtrCard_GetAxisPos(0, resPos))
				{
					CommLinkEnable = true;
					return FunResOk;
				}
				return FunResErr;
			}
			return FunResErr;
		}
		catch (Exception)
		{
			return FunResErr;
		}
	}

	public ushort MoCtrCard_Initial(string SerialPortName)
	{
		ushort result = FunResErr;
		if (mCommPort.Fun_CreateLink_RS232(SerialPortName))
		{
			float[] resPos = new float[6];
			if (FunResOk == MoCtrCard_GetAxisPos(0, resPos))
			{
				CommLinkEnable = true;
				result = FunResOk;
			}
		}
		return result;
	}

	public ushort MoCtrCard_Unload()
	{
		mCommPort.Fun_CloseLink();
		return FunResOk;
	}

	public ushort MoCtrCard_GetAxisPos(byte AxisId, float[] ResPos)
	{
		byte b = 0;
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		if (AxisId < 6)
		{
			b = 0;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (ResPos.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 128;
			DtLenWanted = 4 * MaxAxisNum;
		}
		readMCCardInfoCmndType.Init(246, AxisId, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				ResPos[0] = stNcStt.Val[0].fVal;
				gbAxisPos[AxisId] = ResPos[0];
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					ResPos[i] = stNcStt.Val[i].fVal;
					gbAxisPos[i] = ResPos[i];
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetAxisSpd(byte AxisId, float[] ResSpd)
	{
		byte b = 0;
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		if (AxisId < 6)
		{
			b = 1;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (ResSpd.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 129;
			DtLenWanted = 4 * MaxAxisNum;
		}
		readMCCardInfoCmndType.Init(246, AxisId, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				ResSpd[0] = stNcStt.Val[0].fVal;
				gbAxisPos[AxisId] = ResSpd[0];
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					ResSpd[i] = stNcStt.Val[i].fVal;
					gbAxisPos[i] = ResSpd[i];
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetRunState(uint[] RunState)
	{
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, 11, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			RunState[0] = stNcStt.Val[0].IUVal;
			gbRunState = RunState[0];
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetHomeState(uint[] HomeState)
	{
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, 12, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 8;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			for (int i = 0; i < 2; i++)
			{
				HomeState[i] = stNcStt.Val[i].IUVal;
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetAdVal(byte AxisId, int[] ResAd)
	{
		byte b = 0;
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		if (AxisId < 6)
		{
			b = 2;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (ResAd.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 130;
			DtLenWanted = 4 * MaxAxisNum;
		}
		readMCCardInfoCmndType.Init(246, AxisId, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				ResAd[0] = stNcStt.Val[0].IVal;
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					ResAd[i] = stNcStt.Val[i].IVal;
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetOutState(byte OutGrpIndx, uint[] ResOut)
	{
		byte b = 0;
		switch (OutGrpIndx)
		{
		case 0:
			b = 7;
			break;
		case 1:
			b = 8;
			break;
		default:
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, OutGrpIndx, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ResOut[0] = stNcStt.Val[0].IUVal;
			gbOutput = ResOut[0];
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetInputState(byte InGrpIndx, uint[] ResIn)
	{
		byte b = 0;
		switch (InGrpIndx)
		{
		case 0:
			b = 5;
			break;
		case 1:
			b = 6;
			break;
		default:
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, InGrpIndx, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ResIn[0] = stNcStt.Val[0].IUVal;
			gbInput = ResIn[0];
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetIntInputState(byte InGrpIndx, uint[] ResIntIn)
	{
		byte b = 0;
		switch (InGrpIndx)
		{
		case 0:
			b = 9;
			break;
		case 1:
			b = 10;
			break;
		default:
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, InGrpIndx, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ResIntIn[0] = stNcStt.Val[0].IUVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetPulseMoveInfo(int[] ResInfo, byte AxisId = 0)
	{
		if (ResInfo.GetLength(0) < 6)
		{
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, 35, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 24;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			for (int i = 0; i < 6; i++)
			{
				ResInfo[i] = stNcStt.Val[i].IVal;
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetCustomInfo(int[] ResInfo)
	{
		if (ResInfo.GetLength(0) < 6)
		{
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, 176, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 24;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			for (int i = 0; i < 6; i++)
			{
				ResInfo[i] = stNcStt.Val[i].IVal;
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetAllConfigInfo(uint[] ResInfo)
	{
		if (ResInfo.GetLength(0) < 8)
		{
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, 254, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 32;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			for (int i = 0; i < 8; i++)
			{
				ResInfo[i] = stNcStt.Val[i].IUVal;
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_SendPara(byte AxisId, byte ParaIndx, int ParaVal)
	{
		WriteParaCmndType writeParaCmndType = default(WriteParaCmndType);
		writeParaCmndType.Init(242, AxisId, ParaIndx, MccCardNo);
		writeParaCmndType.SetVal(ParaVal);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(writeParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SendPara(byte AxisId, byte ParaIndx, float ParaVal)
	{
		WriteParaCmndType writeParaCmndType = default(WriteParaCmndType);
		writeParaCmndType.Init(242, AxisId, ParaIndx, MccCardNo);
		writeParaCmndType.SetVal(ParaVal);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(writeParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SendPara(byte AxisId, byte ParaIndx, uint ParaVal)
	{
		WriteParaCmndType writeParaCmndType = default(WriteParaCmndType);
		writeParaCmndType.Init(242, AxisId, ParaIndx, MccCardNo);
		writeParaCmndType.SetVal(ParaVal);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(writeParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisMove(byte AxisId, sbyte SpdDir)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 0, AxisId, SpdDir, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisRelMove(byte AxisId, float fPos, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 3, AxisId, 0, fPos, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisRelMove(byte AxisId, float fPos)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 1, AxisId, 0, fPos, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisAbsMove(byte AxisId, float fPos, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 4, AxisId, 0, fPos, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisAbsMove(byte AxisId, float fPos)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 2, AxisId, 0, fPos, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisAbsMoveSyncInput(byte AxisId, float fPos, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 9, AxisId, 0, fPos, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisRelMoveSyncInput(byte AxisId, float fPos, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 10, AxisId, 0, fPos, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisMoveAtSpd(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 5, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisMoveAtSpd(byte AxisId, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 6, AxisId, 0, 0f, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisMoveAtSpdWithInTime(byte AxisId, float fSpd, float fAcc, float fTime)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 8, AxisId, 0, fTime, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisMoveAtSpdWithInTime(byte AxisId, float fSpd, float fTime)
	{
		return MoCtrCard_MCrlAxisMoveAtSpdWithInTime(AxisId, fSpd, 0f, fTime);
	}

	public ushort MoCtrCard_ChangeMotionPara(byte AxisId, float fSpd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 7, AxisId, 0, 0f, fSpd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ChangeMotionPara(byte AxisId, float fSpd)
	{
		return MoCtrCard_ChangeMotionPara(AxisId, fSpd, 0f);
	}

	public ushort MoCtrCard_SeekZero(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 160, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SeekZero(byte AxisId, float fVCmnd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 161, AxisId, 0, 0f, fVCmnd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SeekZero(byte AxisId, float fVCmnd, float fAcc, float fDist)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 164, AxisId, 0, fDist, fVCmnd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SeekZeroWithInDist(byte AxisId, float fPosCmnd, float fVCmnd, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 163, AxisId, 0, fPosCmnd, fVCmnd, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_CancelSeekZero(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 162, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ResetCoordinate(byte AxisId, float RestPos)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(248, 1, AxisId, 0, RestPos, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ReStartAxisMov(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 144, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_PauseAxisMov(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 130, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ReadPara(byte AxisId, byte ParaIdndx, float[] ParaVal)
	{
		ReadParaCmndType readParaCmndType = default(ReadParaCmndType);
		readParaCmndType.Init(245, AxisId, ParaIdndx, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readParaCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ParaVal[0] = stNcStt.Val[0].fVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_ReadPara(byte AxisId, byte ParaIdndx, uint[] ParaVal)
	{
		ReadParaCmndType readParaCmndType = default(ReadParaCmndType);
		readParaCmndType.Init(245, AxisId, ParaIdndx, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readParaCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ParaVal[0] = stNcStt.Val[0].IUVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_ReadPara(byte AxisId, byte ParaIdndx, int[] ParaVal)
	{
		ReadParaCmndType readParaCmndType = default(ReadParaCmndType);
		readParaCmndType.Init(245, AxisId, ParaIdndx, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readParaCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ParaVal[0] = stNcStt.Val[0].IVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_SaveSystemParaToROM()
	{
		NoParaCmndType noParaCmndType = default(NoParaCmndType);
		noParaCmndType.Init(254, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(noParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SetOutput(byte OutputIndex, bool OutputVal)
	{
		byte b = 0;
		b = (byte)(OutputVal ? 1 : 2);
		SetOutputCmndType setOutputCmndType = default(SetOutputCmndType);
		setOutputCmndType.Init(b, OutputIndex, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(setOutputCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_SetJoyStickEnable(byte AxisId, byte bEnable)
	{
		SetJoyStickCmndType setJoyStickCmndType = default(SetJoyStickCmndType);
		setJoyStickCmndType.Init(AxisId, bEnable, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(setJoyStickCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_QuiteMotionControl(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 145, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public void MoCtrCard_DelayMS(ushort DelayTime)
	{
		Thread.Sleep(DelayTime);
	}

	public uint MoCtrCard_GetDLLVersion()
	{
		return 539035650u;
	}

	public uint MoCtrCard_GetCardVersion(uint[] ResVersion)
	{
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, byte.MaxValue, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			ResVersion[0] = stNcStt.Val[0].IUVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public uint MoCtrCard_GetEncoderVal(byte AxisId, int[] EncoderPos)
	{
		byte b = 0;
		if (AxisId < 6)
		{
			b = 3;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (EncoderPos.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 131;
			DtLenWanted = 4 * MaxAxisNum;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, b, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				EncoderPos[0] = stNcStt.Val[0].IVal;
				gbAxisEnc[AxisId] = EncoderPos[0];
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					EncoderPos[i] = stNcStt.Val[i].IVal;
					gbAxisEnc[i] = EncoderPos[i];
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public uint MoCtrCard_SetEncoderPos(byte AxisId, int EncoderPos)
	{
		SetEncoderValueCmndType setEncoderValueCmndType = default(SetEncoderValueCmndType);
		setEncoderValueCmndType.Init(AxisId, EncoderPos, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(setEncoderValueCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_ClearInternalPulseCount(byte AxisId)
	{
		ClearInternalPulseCntCmndType clearInternalPulseCntCmndType = default(ClearInternalPulseCntCmndType);
		clearInternalPulseCntCmndType.Init(AxisId, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(clearInternalPulseCntCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_RstZ(ushort AxisIndex)
	{
		NoParaCmndType noParaCmndType = default(NoParaCmndType);
		noParaCmndType.Init(253, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(noParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_SetProfileCurvePara(byte AxisId, byte ProfileIndx, float[] fPara)
	{
		if (fPara.Length < 8)
		{
			return FunResErr;
		}
		SetProfileCurveParaCmndType setProfileCurveParaCmndType = default(SetProfileCurveParaCmndType);
		setProfileCurveParaCmndType.Init(AxisId, ProfileIndx, 0, 0, MccCardNo);
		setProfileCurveParaCmndType.SetVal(fPara);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(setProfileCurveParaCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_StartProfileMove(byte AxisId, byte MoveDir)
	{
		ProfileMoveCmndType profileMoveCmndType = default(ProfileMoveCmndType);
		profileMoveCmndType.Init(AxisId, 0, 2, MoveDir, 0f, 0, 0, 0, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(profileMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_StopProfileMove(byte AxisId)
	{
		ProfileMoveCmndType profileMoveCmndType = default(ProfileMoveCmndType);
		profileMoveCmndType.Init(AxisId, 0, 3, 0, 0f, 0, 0, 0, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(profileMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_SetProfilePara(byte AxisId, ushort nMaxSec, float nT, ushort nWorkT, ushort nPeriodicType)
	{
		ProfileMoveCmndType profileMoveCmndType = default(ProfileMoveCmndType);
		profileMoveCmndType.Init(AxisId, 0, 1, 0, nT, nMaxSec, nWorkT, nPeriodicType, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(profileMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_SetGearPara(byte slvId, byte mstrId, float fGear)
	{
		GearCmndType gearCmndType = default(GearCmndType);
		gearCmndType.Init(224, mstrId, slvId, fGear, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(gearCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_ManGearOp(byte slvId, byte bGearCmndr)
	{
		GearCmndType gearCmndType = default(GearCmndType);
		gearCmndType.Init(225, slvId, bGearCmndr, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(gearCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public uint MoCtrCard_ManGearStart(byte slvId)
	{
		return MoCtrCard_ManGearOp(slvId, 1);
	}

	public uint MoCtrCard_ManGearStop(byte slvId)
	{
		return MoCtrCard_ManGearOp(slvId, 0);
	}

	public ushort MoCtrCard_EmergencyStopAxisMov(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 128, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ParaStopAxisMov(byte AxisId, float fAcc)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 129, AxisId, 0, 0f, 0f, fAcc, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_Test(int[] EncoderPos)
	{
		EncoderPos[0] = 1;
		EncoderPos[1] = 2;
		EncoderPos[2] = 3;
		EncoderPos[3] = 4;
		return 55;
	}

	public uint MoCtrCard_GetBoardHardInfo(uint[] HardInfo)
	{
		if (HardInfo.GetLength(0) < 3)
		{
			return FunResErr;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, 0, 224, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 12;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			for (int i = 0; i < 3; i++)
			{
				HardInfo[i] = stNcStt.Val[i].IUVal;
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_UpdateCompensateParameter(byte AxisId)
	{
		ManualOperateType manualOperateType = default(ManualOperateType);
		manualOperateType.Init(244, 32, AxisId, 0, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(manualOperateType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_GetComDiff(byte AxisId, int[] ResDif)
	{
		byte b = 0;
		if (AxisId < 6)
		{
			b = 13;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (ResDif.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 132;
			DtLenWanted = 4 * MaxAxisNum;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, b, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				ResDif[0] = stNcStt.Val[0].IVal;
				gbComDiff[AxisId] = ResDif[0];
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					ResDif[i] = stNcStt.Val[i].IVal;
					gbComDiff[i] = ResDif[i];
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetLimitState(byte AxisId, uint[] ResLimitStt)
	{
		byte b = 0;
		if (AxisId < 6)
		{
			b = 14;
			DtLenWanted = 4;
		}
		else
		{
			if (byte.MaxValue != AxisId)
			{
				return FunResErr;
			}
			if (ResLimitStt.GetLength(0) < MaxAxisNum)
			{
				return FunResErr;
			}
			b = 133;
			DtLenWanted = 4 * MaxAxisNum;
		}
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, b, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			if (AxisId < 6)
			{
				ResLimitStt[0] = stNcStt.Val[0].IUVal;
				gbLimitStt[AxisId] = ResLimitStt[0];
			}
			else if (byte.MaxValue == AxisId)
			{
				for (int i = 0; i < MaxAxisNum; i++)
				{
					ResLimitStt[i] = stNcStt.Val[i].IUVal;
					gbLimitStt[i] = ResLimitStt[i];
				}
			}
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_SetGrpOfflineMovePara(string strMdiCmnd, byte CmndIndx)
	{
		return MoCtrCard_SendMDICommand(strMdiCmnd, CmndIndx, 48);
	}

	public ushort MoCtrCard_SetAxisOfflineMovePara(string strMdiCmnd, byte CmndIndx, byte AxisId)
	{
		if (AxisId > 6)
		{
			return FunResErr;
		}
		return MoCtrCard_SendMDICommand(strMdiCmnd, CmndIndx, (byte)(AxisId + 1));
	}

	public void MoCtrCard_OpenSpDebugForm()
	{
		((Control)mDataShow).Show();
		bSpDataShowFlag = true;
	}

	public void MoCtrCard_CloseSpDebugForm()
	{
		((Control)mDataShow).Hide();
		bSpDataShowFlag = false;
	}

	public ushort MoctrCard_EnableSomeFunction(byte AxisId, byte nFun, byte EnOrNot)
	{
		EnableSomeFunctionCmndType enableSomeFunctionCmndType = default(EnableSomeFunctionCmndType);
		enableSomeFunctionCmndType.Init(AxisId, nFun, EnOrNot, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(enableSomeFunctionCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoctrCard_EnableCompensateFunction(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 3, EnOrNot);
	}

	public ushort MoctrCard_EnableSoftLimitFunction(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 2, EnOrNot);
	}

	public ushort MoctrCard_EnableHardLimitFunction(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 1, EnOrNot);
	}

	public ushort MoctrCard_SetHardLimitSigTrigLev(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 4, EnOrNot);
	}

	public ushort MoctrCard_SetHomeSigTrigLev(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 5, EnOrNot);
	}

	public ushort MoctrCard_EnablePlarformCom(byte zAxsId, byte xAxsId, byte yAxsId)
	{
		EnablePlatformLevCmndType enablePlatformLevCmndType = default(EnablePlatformLevCmndType);
		enablePlatformLevCmndType.Init(1, xAxsId, yAxsId, zAxsId, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(enablePlatformLevCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoctrCard_DisablePlarformCom(byte AxisId)
	{
		EnablePlatformLevCmndType enablePlatformLevCmndType = default(EnablePlatformLevCmndType);
		enablePlatformLevCmndType.Init(0, 0, 0, AxisId, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(enablePlatformLevCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisRelMoveAndPulse(byte AxisId, byte spdDir, byte outputIndex, float fDist, float fDistHead, float fDistTail, float fVel, float fDistPulse, byte nPulseWidth, byte nMode)
	{
		TriggerMoveCmndType triggerMoveCmndType = default(TriggerMoveCmndType);
		triggerMoveCmndType.Init(0, AxisId, spdDir, 1, nMode, outputIndex, nPulseWidth, 0, fDist, fVel, 0f, fDistHead, fDistTail, fDistPulse, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(triggerMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_StopAxisRelMoveAndPulse(byte AxisId)
	{
		TriggerMoveCmndType triggerMoveCmndType = default(TriggerMoveCmndType);
		triggerMoveCmndType.Init(1, AxisId, 0, 1, 0, 0, 0, 0, 0f, 0f, 0f, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(triggerMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_GetMovePulseCnt(byte AxisId, uint[] PulseCnt)
	{
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, 33, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			PulseCnt[0] = stNcStt.Val[0].IUVal;
			gbMovePulseCnt[AxisId] = PulseCnt[0];
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetMovePulsePos(byte AxisId, float[] PulsePos)
	{
		ReadMCCardInfoCmndType readMCCardInfoCmndType = default(ReadMCCardInfoCmndType);
		readMCCardInfoCmndType.Init(246, AxisId, 34, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(readMCCardInfoCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			PulsePos[0] = stNcStt.Val[0].fVal;
			gbMovePulsePos[AxisId] = PulsePos[0];
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_StartSimpleHarmonicMove(byte AxisId, byte bRelAbs, float fA, float fW, float fPhase, uint nCycle)
	{
		SimpleHarmonicMoveCmndType simpleHarmonicMoveCmndType = default(SimpleHarmonicMoveCmndType);
		simpleHarmonicMoveCmndType.Init(1, AxisId, bRelAbs, nCycle, fA, fW, fPhase, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(simpleHarmonicMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_StopSimpleHarmonicMove(byte AxisId)
	{
		SimpleHarmonicMoveCmndType simpleHarmonicMoveCmndType = default(SimpleHarmonicMoveCmndType);
		simpleHarmonicMoveCmndType.Init(2, AxisId, 0, 0u, 0f, 0f, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(simpleHarmonicMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ChangeSimpleHarmonicT(byte AxisId, float fW)
	{
		SimpleHarmonicMoveCmndType simpleHarmonicMoveCmndType = default(SimpleHarmonicMoveCmndType);
		simpleHarmonicMoveCmndType.Init(3, AxisId, 0, 0u, 0f, fW, 0f, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(simpleHarmonicMoveCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	private ushort MoCtrCard_FillGCodeCmndByCmndInfo(byte GCode, byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc, byte OptionMode = 48)
	{
		ushort num = 0;
		GCodeCmndType gCodeCmndType = default(GCodeCmndType);
		gCodeCmndType.Init(243, MccCardNo);
		gCodeCmndType.stGCode.GCode = GCode;
		gCodeCmndType.stGCode.CmndIndx = 0;
		gCodeCmndType.stGCode.CmndMode = OptionMode;
		gCodeCmndType.stGCode.AxisEn = 0;
		for (num = 0; num < 6; num++)
		{
			if (bAxisEn[num] > 0)
			{
				gCodeCmndType.stGCode.AxisEn |= (byte)(1 << (int)num);
			}
		}
		gCodeCmndType.stGCode.CmmndVelX = fSpd[0];
		gCodeCmndType.stGCode.CmmndVelY = fSpd[1];
		gCodeCmndType.stGCode.CmmndVelZ = fSpd[2];
		gCodeCmndType.stGCode.CmmndVelA = fSpd[3];
		gCodeCmndType.stGCode.CmmndVelB = fSpd[4];
		gCodeCmndType.stGCode.CmmndVelC = fSpd[5];
		gCodeCmndType.stGCode.TargtPosX = fDist[0];
		gCodeCmndType.stGCode.TargtPosY = fDist[1];
		gCodeCmndType.stGCode.TargtPosZ = fDist[2];
		gCodeCmndType.stGCode.TargtPosA = fDist[3];
		gCodeCmndType.stGCode.TargtPosB = fDist[4];
		gCodeCmndType.stGCode.TargtPosC = fDist[5];
		gCodeCmndType.stGCode.TargtAccX = fAcc[0];
		gCodeCmndType.stGCode.TargtAccY = fAcc[1];
		gCodeCmndType.stGCode.TargtAccZ = fAcc[2];
		gCodeCmndType.stGCode.TargtAccA = fAcc[3];
		gCodeCmndType.stGCode.TargtAccB = fAcc[4];
		gCodeCmndType.stGCode.TargtAccC = fAcc[5];
		gCodeCmndType.stGCode.Radius = 0f;
		gCodeCmndType.stGCode.DelayTime = 0f;
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(gCodeCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlGroupRelMovePTP(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(128, bAxisEn, fDist, fSpd, fAcc, 48);
	}

	public ushort MoCtrCard_MCrlGroupAbsMovePTP(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(0, bAxisEn, fDist, fSpd, fAcc, 48);
	}

	public ushort MoCtrCard_MCrlGroupRelMove(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(129, bAxisEn, fDist, fSpd, fAcc, 48);
	}

	public ushort MoCtrCard_MCrlGroupAbsMove(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(1, bAxisEn, fDist, fSpd, fAcc, 48);
	}

	public ushort MoCtrCard_MCrlGroupRelMovePTPSyncInput(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(128, bAxisEn, fDist, fSpd, fAcc, (byte)(GroupMoveModeMask_SyncExIn | 0x30));
	}

	public ushort MoCtrCard_MCrlGroupAbsMovePTPSyncInput(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(0, bAxisEn, fDist, fSpd, fAcc, (byte)(GroupMoveModeMask_SyncExIn | 0x30));
	}

	public ushort MoCtrCard_MCrlGroupRelMoveSyncInput(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(129, bAxisEn, fDist, fSpd, fAcc, (byte)(GroupMoveModeMask_SyncExIn | 0x30));
	}

	public ushort MoCtrCard_MCrlGroupAbsMoveSyncInput(byte[] bAxisEn, float[] fDist, float[] fSpd, float[] fAcc)
	{
		return MoCtrCard_FillGCodeCmndByCmndInfo(1, bAxisEn, fDist, fSpd, fAcc, (byte)(GroupMoveModeMask_SyncExIn | 0x30));
	}

	public ushort MoCtrCard_MCrlGroupSpdMove(byte[] bAxisEn, float[] fSpd, float[] fAcc)
	{
		float[] fDist = new float[6];
		return MoCtrCard_FillGCodeCmndByCmndInfo(138, bAxisEn, fDist, fSpd, fAcc, 48);
	}

	public ushort MoCtrCard_MCtrlGroupParaStop(float[] fAcc)
	{
		float[] fDist = new float[6];
		float[] fSpd = new float[6];
		byte[] bAxisEn = new byte[6];
		return MoCtrCard_FillGCodeCmndByCmndInfo(139, bAxisEn, fDist, fSpd, fAcc, 0);
	}

	public ushort MoCtrCard_Scope_Start(byte ChannelCnt, uint SampleCnt, byte SampleTick)
	{
		ScopeDeviceCmndType scopeDeviceCmndType = default(ScopeDeviceCmndType);
		scopeDeviceCmndType.Init(2, 0, 0, 0, ChannelCnt, SampleTick, SampleCnt, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(scopeDeviceCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_Scope_Stop()
	{
		ScopeDeviceCmndType scopeDeviceCmndType = default(ScopeDeviceCmndType);
		scopeDeviceCmndType.Init(3, 0, 0, 0, 0, 0, 0u, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(scopeDeviceCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_Scope_SetChannelPara(byte nChannelId, byte nDevId, byte nSampleObiect)
	{
		ScopeDeviceCmndType scopeDeviceCmndType = default(ScopeDeviceCmndType);
		scopeDeviceCmndType.Init(0, nDevId, nChannelId, nSampleObiect, 0, 0, 0u, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(scopeDeviceCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_Scope_Reset()
	{
		ScopeDeviceCmndType scopeDeviceCmndType = default(ScopeDeviceCmndType);
		scopeDeviceCmndType.Init(1, 0, 0, 0, 0, 0, 0u, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(scopeDeviceCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_GetComTableInfo(byte AxisId, short[] wValidSec, short[] isPeriodical, float[] fModVal)
	{
		CompensateSingleAxisCmndType compensateSingleAxisCmndType = default(CompensateSingleAxisCmndType);
		compensateSingleAxisCmndType.Init(1, AxisId, 0, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(compensateSingleAxisCmndType);
		DtLenWanted = 8;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			wValidSec[0] = stNcStt.Val[0].IWVal1;
			isPeriodical[0] = stNcStt.Val[0].IWVal2;
			fModVal[0] = stNcStt.Val[1].fVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetComTableData(byte AxisId, short PosIndx, float[] PosSample, float[] fComDist)
	{
		CompensateSingleAxisCmndType compensateSingleAxisCmndType = default(CompensateSingleAxisCmndType);
		compensateSingleAxisCmndType.Init(1, AxisId, (ushort)(PosIndx + 1), MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(compensateSingleAxisCmndType);
		DtLenWanted = 8;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			PosSample[0] = stNcStt.Val[0].fVal;
			fComDist[0] = stNcStt.Val[1].fVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_GetPlatformLevTableInfo(int[] nXPosCnt, int[] nYPosCnt)
	{
		CompensatePlatformCmndType compensatePlatformCmndType = default(CompensatePlatformCmndType);
		compensatePlatformCmndType.Init(2, 0, 0, 0, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(compensatePlatformCmndType);
		DtLenWanted = 8;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_GetPlatformLevTableData(int nXPosCnt, int nYPosCnt, float[] xPosVal, float[] yPosVal, float[] zPosVal, float[] yDirCoeA, float[] yDirCoeB)
	{
		CompensatePlatformCmndType compensatePlatformCmndType = default(CompensatePlatformCmndType);
		compensatePlatformCmndType.Init(2, 1, (byte)nXPosCnt, (byte)nYPosCnt, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(compensatePlatformCmndType);
		DtLenWanted = 20;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			xPosVal[0] = stNcStt.Val[0].fVal;
			yPosVal[0] = stNcStt.Val[1].fVal;
			zPosVal[0] = stNcStt.Val[2].fVal;
			yDirCoeA[0] = stNcStt.Val[3].fVal;
			yDirCoeB[0] = stNcStt.Val[4].fVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoctrCard_EnableComTableFunction(byte AxisId, byte EnOrNot)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 6, EnOrNot);
	}

	public ushort MoctrCard_SetHomeMethord(byte AxisId, byte HomeMetheord)
	{
		return MoctrCard_EnableSomeFunction(AxisId, 7, HomeMetheord);
	}

	public ushort MoCtrCard_ExPosFollow_Start(byte AxisEnBit)
	{
		FollowExternalPosCmndType followExternalPosCmndType = default(FollowExternalPosCmndType);
		followExternalPosCmndType.Init(1, AxisEnBit, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(followExternalPosCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_ExPosFollow_Stop(byte AxisDisEnBit)
	{
		FollowExternalPosCmndType followExternalPosCmndType = default(FollowExternalPosCmndType);
		followExternalPosCmndType.Init(2, AxisDisEnBit, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(followExternalPosCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisSinAbsMove(byte AxisId, float fCoefA, float fPos)
	{
		return MoCtrCard_MCrlAxisSinAbsMove(AxisId, fCoefA, fPos, 0f, 0f);
	}

	public ushort MoCtrCard_MCrlAxisSinAbsMove(byte AxisId, float fCoefA, float fPos, float fSpd, float fAcc)
	{
		MoveAsSimpleHarmonicCmndType moveAsSimpleHarmonicCmndType = default(MoveAsSimpleHarmonicCmndType);
		moveAsSimpleHarmonicCmndType.Init(4, AxisId, 0, fPos, fSpd, fAcc, fCoefA, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(moveAsSimpleHarmonicCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_MCrlAxisSinRelMove(byte AxisId, float fCoefA, float fPos)
	{
		return MoCtrCard_MCrlAxisSinRelMove(AxisId, fCoefA, fPos, 0f, 0f);
	}

	public ushort MoCtrCard_MCrlAxisSinRelMove(byte AxisId, float fCoefA, float fPos, float fSpd, float fAcc)
	{
		MoveAsSimpleHarmonicCmndType moveAsSimpleHarmonicCmndType = default(MoveAsSimpleHarmonicCmndType);
		moveAsSimpleHarmonicCmndType.Init(3, AxisId, 0, fPos, fSpd, fAcc, fCoefA, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(moveAsSimpleHarmonicCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public void MoCtrCard_SetMCCardIndex(byte nIndex)
	{
		MccCardNo = nIndex;
	}

	private ushort MoCtrCard_SPIFlash_PageWrite(byte[] wBuf, ushort nPage, ushort nLen)
	{
		stSPIFlashWriteCmndType stSPIFlashWriteCmndType2 = default(stSPIFlashWriteCmndType);
		stSPIFlashWriteCmndType2.Init(228, 1, nPage, nLen, MccCardNo);
		gbLuaRes = stSPIFlashWriteCmndType2.CopyDt(wBuf, nLen);
		if (gbLuaRes == eLuaResultType.eLuaResNull)
		{
			byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(stSPIFlashWriteCmndType2);
			DtLenWanted = 0;
			SpIntrThrdHod = DtLenWanted + 5;
			return MoCtrCard_SendCmmndBufViaComm(sendBuf);
		}
		return FunResErr;
	}

	private ushort MoCtrCard_SPIFlash_PageRead(byte[] RdBuf, ushort nPage, ushort nLen)
	{
		if (RdBuf.Length < nLen)
		{
			gbLuaRes = eLuaResultType.eSpiResMaxLen;
			return FunResErr;
		}
		stSPIFlashReadCmndType stSPIFlashReadCmndType2 = default(stSPIFlashReadCmndType);
		stSPIFlashReadCmndType2.Init(228, 2, nPage, nLen, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(stSPIFlashReadCmndType2);
		DtLenWanted = 256;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			Array.Copy(CommReceiveBuffer, 4, RdBuf, 0, nLen);
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MoCtrCard_Lua_GetResult()
	{
		return (ushort)gbLuaRes;
	}

	public ushort MoCtrCard_Lua_DoString(string strCmnd)
	{
		if (strCmnd.Length > 128)
		{
			gbLuaRes = eLuaResultType.eLuaResDoStringLen;
			return FunResErr;
		}
		stLuaDoStringCmndType stLuaDoStringCmndType2 = default(stLuaDoStringCmndType);
		stLuaDoStringCmndType2.Init(227, 1, (byte)strCmnd.Length, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(stLuaDoStringCmndType2);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	private ushort MoCtrCard_AR_ManualOperationCmnd(byte nCmnd, ushort luaFileID)
	{
		stLuaManualCmndType stLuaManualCmndType2 = default(stLuaManualCmndType);
		stLuaManualCmndType2.Init(227, nCmnd, luaFileID, 0);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(stLuaManualCmndType2);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MoCtrCard_AR_Pause()
	{
		return MoCtrCard_AR_ManualOperationCmnd(4, 0);
	}

	public ushort MoCtrCard_AR_Stop()
	{
		return MoCtrCard_AR_ManualOperationCmnd(3, 0);
	}

	public ushort MoCtrCard_AR_Continue()
	{
		return MoCtrCard_AR_ManualOperationCmnd(5, 0);
	}

	public ushort MoCtrCard_AR_Run(ushort fID)
	{
		return MoCtrCard_AR_ManualOperationCmnd(2, fID);
	}

	public ushort MoCtrCard_AR_DownloadFile(byte[] fileMem, int lenMem, ushort fID)
	{
		stLuaFileHeadType stLuaFileHeadType2 = default(stLuaFileHeadType);
		int destinationIndex = Marshal.SizeOf((object)stLuaFileHeadType2);
		byte[] array = new byte[4096];
		byte[] array2 = new byte[256];
		int num = 0;
		ushort num2 = 0;
		int num3 = 0;
		byte[] array3 = new byte[2];
		try
		{
			if (lenMem > 4062)
			{
				gbLuaRes = eLuaResultType.eLuaResFileMaxLen;
				return FunResErr;
			}
			if (lenMem == 0)
			{
				gbLuaRes = eLuaResultType.eLuaResFileEmpty;
				return FunResErr;
			}
			stLuaFileHeadType2.fileId = fID;
			stLuaFileHeadType2.fileLen = (ushort)lenMem;
			Array.Copy(fileMem, 0, array, destinationIndex, lenMem);
			byte[] array4 = StructToBytes(stLuaFileHeadType2);
			for (int i = 0; i < 10; i++)
			{
				array4[i] = (byte)i;
			}
			destinationIndex = 10;
			ushort num4 = crcCls.CRC16_Modbus(array4, destinationIndex - 2, array3);
			array4[30] = array3[1];
			array4[31] = array3[0];
			Array.Copy(array4, 0, array, 0, destinationIndex);
			num = stLuaFileHeadType2.fileLen + destinationIndex;
			ushort num5 = crcCls.CRC16_Modbus(array, num, array3);
			array[num++] = array3[0];
			array[num++] = array3[1];
			ushort num6 = 0;
			num3 = 0;
			do
			{
				if (num > 256)
				{
					num -= 256;
					Array.Copy(array, num3, array2, 0, 256);
					num3 += 256;
					num2 = 256;
				}
				else
				{
					Array.Copy(array, num3, array2, 0, num);
					num3 += num;
					num2 = (ushort)num;
					num = 0;
				}
				if (FunResErr == MoCtrCard_SPIFlash_PageWrite(array2, (ushort)(fID * 16 + num6), num2))
				{
					gbLuaRes = eLuaResultType.eLuaResFileSendErr;
					return FunResErr;
				}
				num6++;
			}
			while (num != 0);
		}
		catch (Exception)
		{
			gbLuaRes = eLuaResultType.eLuaResFileException;
			return FunResErr;
		}
		return FunResOk;
	}

	public ushort MoCtrCard_AR_ReadFile(ushort fID, byte[] rdBuf, out ushort fileLen)
	{
		ushort num = FunResOk;
		fileLen = 0;
		byte[] array = new byte[4096];
		byte[] array2 = new byte[256];
		ushort num2 = 0;
		ushort num3 = 0;
		ushort num4 = 0;
		ushort num5 = 0;
		stLuaFileHeadType stLuaFileHeadType2 = default(stLuaFileHeadType);
		num2 = 0;
		while (num2 < 16)
		{
			if (FunResOk == MoCtrCard_SPIFlash_PageRead(array2, (ushort)(fID * 16 + num2), 256))
			{
				Array.Copy(array2, 0, array, 256 * num2, 256);
				num2++;
				continue;
			}
			num = FunResErr;
			break;
		}
		if (FunResOk == num)
		{
			Array.Copy(array, 0, array2, 0, Marshal.SizeOf((object)stLuaFileHeadType2));
			stLuaFileHeadType2 = (stLuaFileHeadType)BytesToStruct(array2, typeof(stLuaFileHeadType));
			num3 = stLuaFileHeadType2.fileLen;
			num4 = stLuaFileHeadType2.fileId;
			num5 = stLuaFileHeadType2.CrcCheck;
			ushort num6 = crcCls.CRC16_Modbus(array2, Marshal.SizeOf((object)stLuaFileHeadType2) - 2);
			if (num6 == num5)
			{
				Array.Copy(array, Marshal.SizeOf((object)stLuaFileHeadType2), rdBuf, 0, stLuaFileHeadType2.fileLen);
				fileLen = stLuaFileHeadType2.fileLen;
			}
			else
			{
				num = FunResErr;
			}
		}
		return num;
	}

	public static byte[] StructToBytes(object structObj)
	{
		int num = Marshal.SizeOf(structObj);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(structObj, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public static object BytesToStruct(byte[] bytes, Type type)
	{
		int num = Marshal.SizeOf(type);
		if (num > bytes.Length)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(bytes, 0, intPtr, num);
		object result = Marshal.PtrToStructure(intPtr, type);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	private byte[] fun_Assistant_CacCheckSumAndFillComBuff(object oCmnd)
	{
		uint num = 0u;
		byte[] array = StructToBytes(oCmnd);
		int num2 = array.Length;
		for (ushort num3 = 4; num3 < num2; num3++)
		{
			num += array[num3];
		}
		array[4] = (byte)num;
		return array;
	}

	public ushort MotorDrive_RegRead(short nRegIndex, int[] regVal)
	{
		MotorDriverCmndType motorDriverCmndType = default(MotorDriverCmndType);
		motorDriverCmndType.Init(2, nRegIndex, MccCardNo);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(motorDriverCmndType);
		DtLenWanted = 4;
		SpIntrThrdHod = DtLenWanted + 5;
		if (FunResOk == MoCtrCard_SendCmmndBufViaComm(sendBuf))
		{
			regVal[0] = stNcStt.Val[0].IVal;
			return FunResOk;
		}
		return FunResErr;
	}

	public ushort MotorDrive_RegWrite(short nRegIndex, int regVal)
	{
		MotorDriverCmndType motorDriverCmndType = default(MotorDriverCmndType);
		motorDriverCmndType.Init(1, nRegIndex, MccCardNo);
		motorDriverCmndType.SetVal(regVal);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(motorDriverCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	private ushort MotorDrive_ManualOP(byte OpCmnd, byte nRegIndex = 0, int IVal = 0)
	{
		MotorDriverCmndType motorDriverCmndType = default(MotorDriverCmndType);
		motorDriverCmndType.Init(OpCmnd, nRegIndex, MccCardNo);
		motorDriverCmndType.SetVal(IVal);
		byte[] sendBuf = fun_Assistant_CacCheckSumAndFillComBuff(motorDriverCmndType);
		DtLenWanted = 0;
		SpIntrThrdHod = DtLenWanted + 5;
		return MoCtrCard_SendCmmndBufViaComm(sendBuf);
	}

	public ushort MotorDrive_Enable()
	{
		return MotorDrive_ManualOP(4, 0);
	}

	public ushort MotorDrive_Disable()
	{
		return MotorDrive_ManualOP(5, 0);
	}

	public ushort MotorDrive_Reset()
	{
		return MotorDrive_ManualOP(6, 0);
	}

	public ushort MotorDrive_RegSave()
	{
		return MotorDrive_ManualOP(3, 0);
	}

	public ushort MotorDrive_SetInputMode(byte nIndx, int nModVal)
	{
		return MotorDrive_ManualOP(7, nIndx, nModVal);
	}
}
