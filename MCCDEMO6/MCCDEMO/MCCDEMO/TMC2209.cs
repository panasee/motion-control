using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class TMC2209 : Form
{
	private readonly int INPUT0_EXMODE_GIN;

	private readonly int INPUT0_EXMODE_NEGLIMIT = 1;

	private readonly int INPUT0_EXMODE_POSMOVE = 2;

	private readonly int INPUT0_EXMODE_HOME = 3;

	private readonly int INPUT1_EXMODE_GIN;

	private readonly int INPUT1_EXMODE_POSLIMIT = 16;

	private readonly int INPUT1_EXMODE_NEGMOVE = 32;

	private readonly int INPUT1_EXMODE_HOME = 48;

	private readonly int INPUT2_EXMODE_GIN;

	private readonly int INPUT2_EXMODE_EMSTOP = 256;

	private readonly int INPUT2_EXMODE_STOPMOVE = 512;

	private readonly int INPUT2_EXMPDE_HOME = 768;

	private MainFrm pMainFrm = new MainFrm();

	private IContainer components;

	private GroupBox groupBox1;

	private RadioButton rbRegValHex;

	private RadioButton rbRegValDec;

	private Button bnRegSave;

	private Button bnReset;

	private Button bnDisable;

	private Button bnEnable;

	private Button bnRegWrite;

	private Button bnRegRead;

	private Label label1;

	private TextBox tbRegVal;

	private ComboBox cbRegIndex;

	private GroupBox groupBox2;

	private Label label3;

	private Label label2;

	private HScrollBar hsbIHold;

	private HScrollBar hsbIRun;

	private Label lbCrrntInfo;

	private GroupBox groupBox3;

	private GroupBox groupBox4;

	private GroupBox gbInput3;

	private GroupBox gbInput2;

	private GroupBox gbInput1;

	private RadioButton rbPulse51200;

	private RadioButton rbPulse25600;

	private RadioButton rbPulse12800;

	private RadioButton rbPulse6400;

	private RadioButton rbPulse3200;

	private RadioButton rbPulse1600;

	private RadioButton rbPulse800;

	private RadioButton rbPulse400;

	private TextBox tbPulses;

	private RadioButton rbInput3Stop;

	private RadioButton rbInput3GPInput;

	private RadioButton rbInput3Home;

	private RadioButton rbInput2MoveNeg;

	private RadioButton rbInput2GPInput;

	private RadioButton rbInput2RightLimit;

	private RadioButton rbInput1MovePos;

	private RadioButton rbInput1GPInput;

	private RadioButton rbInput1LeftLimit;

	private Button bnSetCurrent;

	public TMC2209()
	{
		InitializeComponent();
	}

	private void bnRegRead_Click(object sender, EventArgs e)
	{
		byte b = byte.Parse(((Control)cbRegIndex).Text.Substring(2, 2));
		int[] array = new int[1];
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegRead(b, array))
		{
			if (rbRegValDec.Checked)
			{
				((Control)tbRegVal).Text = array[0].ToString("x");
			}
			else if (rbRegValHex.Checked)
			{
				((Control)tbRegVal).Text = array[0].ToString();
			}
		}
		else
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_ReadSomeRegError, b));
		}
	}

	private void TMC2209_Load(object sender, EventArgs e)
	{
		pMainFrm = (MainFrm)(object)((Form)this).Owner;
		int[] array = new int[1];
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegRead(pMainFrm.Mccard.MotorDrv_ParaIndex_Pulse, array))
		{
			switch (array[0])
			{
			case 0:
				rbPulse51200.Checked = true;
				break;
			case 8:
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_NotDefineMicroSteps200StepPerTurn);
				break;
			case 7:
				rbPulse400.Checked = true;
				break;
			case 6:
				rbPulse800.Checked = true;
				break;
			case 5:
				rbPulse1600.Checked = true;
				break;
			case 4:
				rbPulse3200.Checked = true;
				break;
			case 3:
				rbPulse6400.Checked = true;
				break;
			case 2:
				rbPulse12800.Checked = true;
				break;
			case 1:
				rbPulse25600.Checked = true;
				break;
			default:
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_ReadMicroOkButFormatInValidPleaseReSet, array[0]));
				break;
			}
		}
		else
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_ReadMicroStepFailed);
		}
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegRead(pMainFrm.Mccard.MotorDrv_ParaIndex_IRun, array))
		{
			if (array[0] > 31)
			{
				((ScrollBar)hsbIRun).Value = 31;
			}
			else if (array[0] < 1)
			{
				((ScrollBar)hsbIRun).Value = 1;
			}
			else
			{
				((ScrollBar)hsbIRun).Value = array[0];
			}
		}
		else
		{
			((ScrollBar)hsbIRun).Value = 1;
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_ReadWorkCurrentFailed);
		}
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegRead(pMainFrm.Mccard.MotorDrv_ParaIndex_IHold, array))
		{
			if (array[0] > 31)
			{
				((ScrollBar)hsbIHold).Value = 31;
			}
			else if (array[0] < 1)
			{
				((ScrollBar)hsbIHold).Value = 1;
			}
			else
			{
				((ScrollBar)hsbIHold).Value = array[0];
			}
		}
		else
		{
			((ScrollBar)hsbIHold).Value = 1;
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_ReadHoldCurrentFailed);
		}
		fun_print2hsb(((ScrollBar)hsbIRun).Value, ((ScrollBar)hsbIHold).Value);
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegRead(pMainFrm.Mccard.MotorDrv_ParaIndex_InputFun, array))
		{
			if (INPUT0_EXMODE_GIN == (array[0] & 0xF))
			{
				rbInput1GPInput.Checked = true;
			}
			else if (INPUT0_EXMODE_NEGLIMIT == (array[0] & 0xF))
			{
				rbInput1LeftLimit.Checked = true;
			}
			else if (INPUT0_EXMODE_POSMOVE == (array[0] & 0xF))
			{
				rbInput1MovePos.Checked = true;
			}
			else
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_InputFunctionParameterError, 1));
			}
			if (INPUT1_EXMODE_GIN == (array[0] & 0xF0))
			{
				rbInput2GPInput.Checked = true;
			}
			else if (INPUT1_EXMODE_POSLIMIT == (array[0] & 0xF0))
			{
				rbInput2RightLimit.Checked = true;
			}
			else if (INPUT1_EXMODE_NEGMOVE == (array[0] & 0xF0))
			{
				rbInput2MoveNeg.Checked = true;
			}
			else
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_InputFunctionParameterError, 2));
			}
			if (INPUT2_EXMODE_GIN == (array[0] & 0xF00))
			{
				rbInput3GPInput.Checked = true;
			}
			else if (INPUT2_EXMODE_STOPMOVE == (array[0] & 0xF00))
			{
				rbInput3Stop.Checked = true;
			}
			else if (INPUT2_EXMPDE_HOME == (array[0] & 0xF00))
			{
				rbInput3Home.Checked = true;
			}
			else
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_InputFunctionParameterError, 3));
			}
		}
		else
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_ReadInputFunctionParameterFailed);
		}
	}

	private void bnRegWrite_Click(object sender, EventArgs e)
	{
		byte b = byte.Parse(((Control)cbRegIndex).Text.Substring(2, 2), NumberStyles.HexNumber);
		int regVal = 0;
		if (rbRegValDec.Checked)
		{
			if (((Control)tbRegVal).Text.Contains("0x"))
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_RegValueFormatError, "DEC"));
			}
			else
			{
				regVal = int.Parse(((Control)tbRegVal).Text);
			}
		}
		else if (rbRegValHex.Checked)
		{
			if (((Control)tbRegVal).Text.Contains("0x"))
			{
				regVal = int.Parse(((Control)tbRegVal).Text, NumberStyles.HexNumber);
			}
			else
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_RegValueFormatError, "HEX"));
			}
		}
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MotorDrive_RegWrite(b, regVal))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_WriteRegSucceed, b));
		}
		else
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_WriteRegFailed, b));
		}
	}

	private void bnEnable_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_Enable())
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_EnableDriverFailed);
		}
	}

	private void bnDisable_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_Disable())
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_DisableDriverFailed);
		}
	}

	private void bnRegSave_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_RegSave())
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SaveRegValueFailed);
		}
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM())
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SaveParameterToBoardFailed);
		}
	}

	private void bnReset_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_Reset())
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_ResetDriverFailed);
		}
	}

	private void rbRegValDec_CheckedChanged(object sender, EventArgs e)
	{
		if (((Control)tbRegVal).Text.Contains("0x"))
		{
			int num = int.Parse(((Control)tbRegVal).Text.Substring(2, ((Control)tbRegVal).Text.Length - 2), NumberStyles.HexNumber);
			((Control)tbRegVal).Text = num.ToString();
		}
	}

	private void rbRegValHex_CheckedChanged(object sender, EventArgs e)
	{
		if (!((Control)tbRegVal).Text.Contains("0x"))
		{
			int num = int.Parse(((Control)tbRegVal).Text);
			((Control)tbRegVal).Text = "0x" + num.ToString("x");
		}
	}

	private void fun_print2hsb(int nIRun, int nIHold)
	{
		((Control)lbCrrntInfo).Text = $"IRun:{nIRun * 100 / 31}% IHold:{nIHold * 100 / 31}%";
	}

	private void hsbIRun_Scroll(object sender, ScrollEventArgs e)
	{
		fun_print2hsb(((ScrollBar)hsbIRun).Value, ((ScrollBar)hsbIHold).Value);
	}

	private void hsbIHold_Scroll(object sender, ScrollEventArgs e)
	{
		fun_print2hsb(((ScrollBar)hsbIRun).Value, ((ScrollBar)hsbIHold).Value);
	}

	private void bnSetPulse_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_RegWrite(pMainFrm.Mccard.MotorDrv_ParaIndex_Pulse, int.Parse(((Control)tbPulses).Text)))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SetMicroStepFailed);
		}
	}

	private void rbPulse400_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		RadioButton val = (RadioButton)sender;
		int regVal = 255;
		if (((Control)rbPulse400).Name == ((Control)val).Name)
		{
			regVal = 7;
		}
		else if (((Control)rbPulse800).Name == ((Control)val).Name)
		{
			regVal = 6;
		}
		else if (((Control)rbPulse1600).Name == ((Control)val).Name)
		{
			regVal = 5;
		}
		else if (((Control)rbPulse3200).Name == ((Control)val).Name)
		{
			regVal = 4;
		}
		else if (((Control)rbPulse6400).Name == ((Control)val).Name)
		{
			regVal = 3;
		}
		else if (((Control)rbPulse12800).Name == ((Control)val).Name)
		{
			regVal = 2;
		}
		else if (((Control)rbPulse25600).Name == ((Control)val).Name)
		{
			regVal = 1;
		}
		else if (((Control)rbPulse51200).Name == ((Control)val).Name)
		{
			regVal = 0;
		}
		((Control)tbPulses).Text = regVal.ToString();
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_RegWrite(pMainFrm.Mccard.MotorDrv_ParaIndex_Pulse, regVal))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SetMicroStepFailed);
		}
	}

	private void rbInput1GPInput_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		int num = 255;
		RadioButton val = (RadioButton)sender;
		if (((Control)val).Name == ((Control)rbInput1GPInput).Name)
		{
			num = 0;
		}
		else if (((Control)val).Name == ((Control)rbInput1LeftLimit).Name)
		{
			num = 1;
		}
		else if (((Control)val).Name == ((Control)rbInput1MovePos).Name)
		{
			num = 2;
		}
		if (255 != num && pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_SetInputMode(0, num))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_SetInputFunctionFailed, 0));
		}
	}

	private void rbInput2GPInput_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		int num = 255;
		RadioButton val = (RadioButton)sender;
		if (((Control)val).Name == ((Control)rbInput2GPInput).Name)
		{
			num = 0;
		}
		else if (((Control)val).Name == ((Control)rbInput2RightLimit).Name)
		{
			num = 1;
		}
		else if (((Control)val).Name == ((Control)rbInput2MoveNeg).Name)
		{
			num = 2;
		}
		if (255 != num && pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_SetInputMode(1, num))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_SetInputFunctionFailed, 1));
		}
	}

	private void rbInput3GPInput_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		int num = 255;
		RadioButton val = (RadioButton)sender;
		if (((Control)val).Name == ((Control)rbInput3GPInput).Name)
		{
			num = 0;
		}
		else if (((Control)val).Name == ((Control)rbInput3Stop).Name)
		{
			num = 1;
		}
		else if (((Control)val).Name == ((Control)rbInput3Home).Name)
		{
			num = 3;
		}
		if (255 != num && pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_SetInputMode(2, num))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.TMC2209Frm_SetInputFunctionFailed, 2));
		}
	}

	private void bnSetCurrent_Click(object sender, EventArgs e)
	{
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_RegWrite(pMainFrm.Mccard.MotorDrv_ParaIndex_IRun, ((ScrollBar)hsbIRun).Value))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SetWorkCurrentFailed);
		}
		if (pMainFrm.Mccard.FunResErr == pMainFrm.Mccard.MotorDrive_RegWrite(pMainFrm.Mccard.MotorDrv_ParaIndex_IHold, ((ScrollBar)hsbIHold).Value))
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.TMC2209Frm_SetHoldCurrentFailed);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Expected O, but got Unknown
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TMC2209));
		groupBox1 = new GroupBox();
		rbRegValHex = new RadioButton();
		rbRegValDec = new RadioButton();
		bnRegSave = new Button();
		bnReset = new Button();
		bnDisable = new Button();
		bnEnable = new Button();
		bnRegWrite = new Button();
		bnRegRead = new Button();
		label1 = new Label();
		tbRegVal = new TextBox();
		cbRegIndex = new ComboBox();
		groupBox2 = new GroupBox();
		bnSetCurrent = new Button();
		lbCrrntInfo = new Label();
		label3 = new Label();
		label2 = new Label();
		hsbIHold = new HScrollBar();
		hsbIRun = new HScrollBar();
		groupBox3 = new GroupBox();
		gbInput3 = new GroupBox();
		rbInput3Stop = new RadioButton();
		rbInput3GPInput = new RadioButton();
		rbInput3Home = new RadioButton();
		gbInput2 = new GroupBox();
		rbInput2MoveNeg = new RadioButton();
		rbInput2GPInput = new RadioButton();
		rbInput2RightLimit = new RadioButton();
		gbInput1 = new GroupBox();
		rbInput1MovePos = new RadioButton();
		rbInput1GPInput = new RadioButton();
		rbInput1LeftLimit = new RadioButton();
		groupBox4 = new GroupBox();
		tbPulses = new TextBox();
		rbPulse51200 = new RadioButton();
		rbPulse25600 = new RadioButton();
		rbPulse12800 = new RadioButton();
		rbPulse6400 = new RadioButton();
		rbPulse3200 = new RadioButton();
		rbPulse1600 = new RadioButton();
		rbPulse800 = new RadioButton();
		rbPulse400 = new RadioButton();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)groupBox3).SuspendLayout();
		((Control)gbInput3).SuspendLayout();
		((Control)gbInput2).SuspendLayout();
		((Control)gbInput1).SuspendLayout();
		((Control)groupBox4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)rbRegValHex);
		((Control)groupBox1).Controls.Add((Control)(object)rbRegValDec);
		((Control)groupBox1).Controls.Add((Control)(object)bnRegSave);
		((Control)groupBox1).Controls.Add((Control)(object)bnReset);
		((Control)groupBox1).Controls.Add((Control)(object)bnDisable);
		((Control)groupBox1).Controls.Add((Control)(object)bnEnable);
		((Control)groupBox1).Controls.Add((Control)(object)bnRegWrite);
		((Control)groupBox1).Controls.Add((Control)(object)bnRegRead);
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)tbRegVal);
		((Control)groupBox1).Controls.Add((Control)(object)cbRegIndex);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(rbRegValHex, "rbRegValHex");
		((Control)rbRegValHex).Name = "rbRegValHex";
		((ButtonBase)rbRegValHex).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbRegValDec, "rbRegValDec");
		rbRegValDec.Checked = true;
		((Control)rbRegValDec).Name = "rbRegValDec";
		rbRegValDec.TabStop = true;
		((ButtonBase)rbRegValDec).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnRegSave, "bnRegSave");
		((Control)bnRegSave).Name = "bnRegSave";
		((ButtonBase)bnRegSave).UseVisualStyleBackColor = true;
		((Control)bnRegSave).Click += bnRegSave_Click;
		componentResourceManager.ApplyResources(bnReset, "bnReset");
		((Control)bnReset).Name = "bnReset";
		((ButtonBase)bnReset).UseVisualStyleBackColor = true;
		((Control)bnReset).Click += bnReset_Click;
		componentResourceManager.ApplyResources(bnDisable, "bnDisable");
		((Control)bnDisable).Name = "bnDisable";
		((ButtonBase)bnDisable).UseVisualStyleBackColor = true;
		((Control)bnDisable).Click += bnDisable_Click;
		componentResourceManager.ApplyResources(bnEnable, "bnEnable");
		((Control)bnEnable).Name = "bnEnable";
		((ButtonBase)bnEnable).UseVisualStyleBackColor = true;
		((Control)bnEnable).Click += bnEnable_Click;
		componentResourceManager.ApplyResources(bnRegWrite, "bnRegWrite");
		((Control)bnRegWrite).Name = "bnRegWrite";
		((ButtonBase)bnRegWrite).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnRegRead, "bnRegRead");
		((Control)bnRegRead).Name = "bnRegRead";
		((ButtonBase)bnRegRead).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbRegVal, "tbRegVal");
		((Control)tbRegVal).Name = "tbRegVal";
		componentResourceManager.ApplyResources(cbRegIndex, "cbRegIndex");
		((ListControl)cbRegIndex).FormattingEnabled = true;
		((Control)cbRegIndex).Name = "cbRegIndex";
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)bnSetCurrent);
		((Control)groupBox2).Controls.Add((Control)(object)lbCrrntInfo);
		((Control)groupBox2).Controls.Add((Control)(object)label3);
		((Control)groupBox2).Controls.Add((Control)(object)label2);
		((Control)groupBox2).Controls.Add((Control)(object)hsbIHold);
		((Control)groupBox2).Controls.Add((Control)(object)hsbIRun);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(bnSetCurrent, "bnSetCurrent");
		((Control)bnSetCurrent).Name = "bnSetCurrent";
		((ButtonBase)bnSetCurrent).UseVisualStyleBackColor = true;
		((Control)bnSetCurrent).Click += bnSetCurrent_Click;
		componentResourceManager.ApplyResources(lbCrrntInfo, "lbCrrntInfo");
		((Control)lbCrrntInfo).BackColor = SystemColors.ButtonShadow;
		((Control)lbCrrntInfo).Name = "lbCrrntInfo";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(hsbIHold, "hsbIHold");
		((ScrollBar)hsbIHold).LargeChange = 1;
		((ScrollBar)hsbIHold).Maximum = 31;
		((ScrollBar)hsbIHold).Minimum = 1;
		((Control)hsbIHold).Name = "hsbIHold";
		((ScrollBar)hsbIHold).Value = 1;
		((ScrollBar)hsbIHold).Scroll += new ScrollEventHandler(hsbIHold_Scroll);
		componentResourceManager.ApplyResources(hsbIRun, "hsbIRun");
		((ScrollBar)hsbIRun).LargeChange = 1;
		((ScrollBar)hsbIRun).Maximum = 31;
		((ScrollBar)hsbIRun).Minimum = 1;
		((Control)hsbIRun).Name = "hsbIRun";
		((ScrollBar)hsbIRun).Value = 1;
		((ScrollBar)hsbIRun).Scroll += new ScrollEventHandler(hsbIRun_Scroll);
		componentResourceManager.ApplyResources(groupBox3, "groupBox3");
		((Control)groupBox3).Controls.Add((Control)(object)gbInput3);
		((Control)groupBox3).Controls.Add((Control)(object)gbInput2);
		((Control)groupBox3).Controls.Add((Control)(object)gbInput1);
		((Control)groupBox3).Name = "groupBox3";
		groupBox3.TabStop = false;
		componentResourceManager.ApplyResources(gbInput3, "gbInput3");
		((Control)gbInput3).Controls.Add((Control)(object)rbInput3Stop);
		((Control)gbInput3).Controls.Add((Control)(object)rbInput3GPInput);
		((Control)gbInput3).Controls.Add((Control)(object)rbInput3Home);
		((Control)gbInput3).Name = "gbInput3";
		gbInput3.TabStop = false;
		componentResourceManager.ApplyResources(rbInput3Stop, "rbInput3Stop");
		((Control)rbInput3Stop).Name = "rbInput3Stop";
		rbInput3Stop.TabStop = true;
		((ButtonBase)rbInput3Stop).UseVisualStyleBackColor = true;
		rbInput3Stop.CheckedChanged += rbInput3GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput3GPInput, "rbInput3GPInput");
		((Control)rbInput3GPInput).Name = "rbInput3GPInput";
		rbInput3GPInput.TabStop = true;
		((ButtonBase)rbInput3GPInput).UseVisualStyleBackColor = true;
		rbInput3GPInput.CheckedChanged += rbInput3GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput3Home, "rbInput3Home");
		((Control)rbInput3Home).Name = "rbInput3Home";
		rbInput3Home.TabStop = true;
		((ButtonBase)rbInput3Home).UseVisualStyleBackColor = true;
		rbInput3Home.CheckedChanged += rbInput3GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(gbInput2, "gbInput2");
		((Control)gbInput2).Controls.Add((Control)(object)rbInput2MoveNeg);
		((Control)gbInput2).Controls.Add((Control)(object)rbInput2GPInput);
		((Control)gbInput2).Controls.Add((Control)(object)rbInput2RightLimit);
		((Control)gbInput2).Name = "gbInput2";
		gbInput2.TabStop = false;
		componentResourceManager.ApplyResources(rbInput2MoveNeg, "rbInput2MoveNeg");
		((Control)rbInput2MoveNeg).Name = "rbInput2MoveNeg";
		rbInput2MoveNeg.TabStop = true;
		((ButtonBase)rbInput2MoveNeg).UseVisualStyleBackColor = true;
		rbInput2MoveNeg.CheckedChanged += rbInput2GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput2GPInput, "rbInput2GPInput");
		((Control)rbInput2GPInput).Name = "rbInput2GPInput";
		rbInput2GPInput.TabStop = true;
		((ButtonBase)rbInput2GPInput).UseVisualStyleBackColor = true;
		rbInput2GPInput.CheckedChanged += rbInput2GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput2RightLimit, "rbInput2RightLimit");
		((Control)rbInput2RightLimit).Name = "rbInput2RightLimit";
		rbInput2RightLimit.TabStop = true;
		((ButtonBase)rbInput2RightLimit).UseVisualStyleBackColor = true;
		rbInput2RightLimit.CheckedChanged += rbInput2GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(gbInput1, "gbInput1");
		((Control)gbInput1).Controls.Add((Control)(object)rbInput1MovePos);
		((Control)gbInput1).Controls.Add((Control)(object)rbInput1GPInput);
		((Control)gbInput1).Controls.Add((Control)(object)rbInput1LeftLimit);
		((Control)gbInput1).Name = "gbInput1";
		gbInput1.TabStop = false;
		componentResourceManager.ApplyResources(rbInput1MovePos, "rbInput1MovePos");
		((Control)rbInput1MovePos).Name = "rbInput1MovePos";
		rbInput1MovePos.TabStop = true;
		((ButtonBase)rbInput1MovePos).UseVisualStyleBackColor = true;
		rbInput1MovePos.CheckedChanged += rbInput1GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput1GPInput, "rbInput1GPInput");
		((Control)rbInput1GPInput).Name = "rbInput1GPInput";
		rbInput1GPInput.TabStop = true;
		((ButtonBase)rbInput1GPInput).UseVisualStyleBackColor = true;
		rbInput1GPInput.CheckedChanged += rbInput1GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(rbInput1LeftLimit, "rbInput1LeftLimit");
		((Control)rbInput1LeftLimit).Name = "rbInput1LeftLimit";
		rbInput1LeftLimit.TabStop = true;
		((ButtonBase)rbInput1LeftLimit).UseVisualStyleBackColor = true;
		rbInput1LeftLimit.CheckedChanged += rbInput1GPInput_CheckedChanged;
		componentResourceManager.ApplyResources(groupBox4, "groupBox4");
		((Control)groupBox4).Controls.Add((Control)(object)tbPulses);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse51200);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse25600);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse12800);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse6400);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse3200);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse1600);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse800);
		((Control)groupBox4).Controls.Add((Control)(object)rbPulse400);
		((Control)groupBox4).Name = "groupBox4";
		groupBox4.TabStop = false;
		componentResourceManager.ApplyResources(tbPulses, "tbPulses");
		((Control)tbPulses).Name = "tbPulses";
		componentResourceManager.ApplyResources(rbPulse51200, "rbPulse51200");
		((Control)rbPulse51200).Name = "rbPulse51200";
		rbPulse51200.TabStop = true;
		((ButtonBase)rbPulse51200).UseVisualStyleBackColor = true;
		rbPulse51200.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse25600, "rbPulse25600");
		((Control)rbPulse25600).Name = "rbPulse25600";
		rbPulse25600.TabStop = true;
		((ButtonBase)rbPulse25600).UseVisualStyleBackColor = true;
		rbPulse25600.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse12800, "rbPulse12800");
		((Control)rbPulse12800).Name = "rbPulse12800";
		rbPulse12800.TabStop = true;
		((ButtonBase)rbPulse12800).UseVisualStyleBackColor = true;
		rbPulse12800.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse6400, "rbPulse6400");
		((Control)rbPulse6400).Name = "rbPulse6400";
		rbPulse6400.TabStop = true;
		((ButtonBase)rbPulse6400).UseVisualStyleBackColor = true;
		rbPulse6400.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse3200, "rbPulse3200");
		((Control)rbPulse3200).Name = "rbPulse3200";
		rbPulse3200.TabStop = true;
		((ButtonBase)rbPulse3200).UseVisualStyleBackColor = true;
		rbPulse3200.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse1600, "rbPulse1600");
		((Control)rbPulse1600).Name = "rbPulse1600";
		rbPulse1600.TabStop = true;
		((ButtonBase)rbPulse1600).UseVisualStyleBackColor = true;
		rbPulse1600.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse800, "rbPulse800");
		((Control)rbPulse800).Name = "rbPulse800";
		rbPulse800.TabStop = true;
		((ButtonBase)rbPulse800).UseVisualStyleBackColor = true;
		rbPulse800.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(rbPulse400, "rbPulse400");
		((Control)rbPulse400).Name = "rbPulse400";
		rbPulse400.TabStop = true;
		((ButtonBase)rbPulse400).UseVisualStyleBackColor = true;
		rbPulse400.CheckedChanged += rbPulse400_CheckedChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox4);
		((Control)this).Controls.Add((Control)(object)groupBox3);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "TMC2209";
		((Form)this).Load += TMC2209_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)groupBox3).ResumeLayout(false);
		((Control)gbInput3).ResumeLayout(false);
		((Control)gbInput3).PerformLayout();
		((Control)gbInput2).ResumeLayout(false);
		((Control)gbInput2).PerformLayout();
		((Control)gbInput1).ResumeLayout(false);
		((Control)gbInput1).PerformLayout();
		((Control)groupBox4).ResumeLayout(false);
		((Control)groupBox4).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
