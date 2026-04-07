using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class MovePulseCtrl : Form
{
	private enum eScanMachineStt
	{
		eScanNull,
		eScanXMovePosStart,
		eScanXMovePosIng,
		eScanXMoveNegStart,
		eScanXMoveNegIng,
		eScanTestPulseCntPos,
		eScanTestPulseCntNeg,
		eScanYMoveStartXPos,
		eScanYMoveStartXNeg,
		eScanYMoveXPosIng,
		eScanYMoveXNegIng,
		eScanTestOver,
		eScanTestFailed
	}

	private MainFrm PMainFrm;

	private uint gbSysRunStt;

	private float gbYInterval;

	private int gbYStepCnt;

	private int gbPulseCmdCnt;

	private eScanMachineStt eScanStt;

	private IContainer components;

	private Button bnXMove;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private TextBox tbPulseWidX;

	private TextBox tbPulseIntervalX;

	private TextBox tbDistTailX;

	private TextBox tbDistHeadX;

	private TextBox tbVelX;

	private ComboBox cbOutputX;

	private Label cbOutput;

	private ComboBox cbAxisId;

	private Label label8;

	private ComboBox cbWidthMode;

	private Label label9;

	private Button button1;

	private Button bnStartPos;

	private Button bnStartNeg;

	private Timer timer1;

	private TextBox tbDistX;

	private Label lbXPos;

	private Label lbPulseCnt;

	private Button bnStopScan;

	private TextBox tbCycleCnt;

	private Label lbCycleCnt;

	private Button bnScanStart;

	private TextBox tbYSteps;

	private Label label11;

	private TextBox tbRightPulseCnt;

	private Label label10;

	private TextBox tbYInterval;

	private Label label7;

	private Label label12;

	private Label label13;

	private Label label14;

	private Label label15;

	private Label label16;

	private Label label17;

	private Label label18;

	private TextBox tbPosStart;

	private TextBox tbPosCrrnt;

	private TextBox tbPosPulseNext;

	private TextBox tbPosEnd;

	private TextBox tbPulseDiff;

	private TextBox tbPosLast;

	private Button tbAskInfo;

	private TabControl tabControl1;

	private TabPage tabPage1;

	private TabPage tabPage2;

	public MovePulseCtrl()
	{
		InitializeComponent();
	}

	private uint fun_MovePulse(bool bDir)
	{
		try
		{
			byte b = 0;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			byte b2 = 0;
			byte b3 = 0;
			byte b4 = 0;
			byte b5 = 0;
			num = float.Parse(((Control)tbDistX).Text);
			b = ((!bDir) ? ((byte)1) : ((byte)0));
			num2 = float.Parse(((Control)tbVelX).Text);
			num3 = float.Parse(((Control)tbDistHeadX).Text);
			num4 = float.Parse(((Control)tbDistTailX).Text);
			num5 = float.Parse(((Control)tbPulseIntervalX).Text);
			b2 = byte.Parse(((Control)tbPulseWidX).Text);
			b3 = (byte)((ListControl)cbWidthMode).SelectedIndex;
			b4 = (byte)((ListControl)cbAxisId).SelectedIndex;
			b5 = (byte)((ListControl)cbOutputX).SelectedIndex;
			return PMainFrm.Mccard.MoCtrCard_MCrlAxisRelMoveAndPulse(b4, b, b5, num, num3, num4, num2, num5, b2, b3);
		}
		catch (Exception)
		{
			return PMainFrm.Mccard.FunResErr;
		}
	}

	private void bnXMove_Click(object sender, EventArgs e)
	{
		if (float.Parse(((Control)tbDistX).Text) > 0f)
		{
			fun_MovePulse(bDir: true);
		}
		else
		{
			fun_MovePulse(bDir: false);
		}
	}

	private void MovePulseCtrl_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		((ListControl)cbOutputX).SelectedIndex = 0;
		((ListControl)cbWidthMode).SelectedIndex = 1;
		((ListControl)cbAxisId).SelectedIndex = 0;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		float[] array = new float[1];
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetAxisPos((byte)((ListControl)cbAxisId).SelectedIndex, array))
		{
			((Control)lbXPos).Text = array[0].ToString();
		}
		uint[] array2 = new uint[1];
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetMovePulseCnt((byte)((ListControl)cbAxisId).SelectedIndex, array2))
		{
			((Control)lbPulseCnt).Text = array2[0].ToString();
		}
		if (eScanMachineStt.eScanXMovePosStart == eScanStt)
		{
			uint[] array3 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetRunState(array3))
			{
				gbSysRunStt = array3[0];
				if (PMainFrm.Mccard.FunResOk == fun_MovePulse(bDir: true))
				{
					eScanStt = eScanMachineStt.eScanXMovePosIng;
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanXMovePosIng == eScanStt || eScanMachineStt.eScanXMoveNegIng == eScanStt)
		{
			uint[] array4 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetRunState(array4))
			{
				if ((array4[0] & PMainFrm.Mccard.Axis1IntrpltSttBit) != (gbSysRunStt & PMainFrm.Mccard.Axis1IntrpltSttBit))
				{
					if (eScanMachineStt.eScanXMovePosIng == eScanStt)
					{
						eScanStt = eScanMachineStt.eScanTestPulseCntPos;
					}
					else
					{
						eScanStt = eScanMachineStt.eScanTestPulseCntNeg;
					}
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanTestPulseCntNeg == eScanStt || eScanMachineStt.eScanTestPulseCntPos == eScanStt)
		{
			uint[] array5 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetMovePulseCnt((byte)((ListControl)cbAxisId).SelectedIndex, array5))
			{
				if (array5[0] != gbPulseCmdCnt)
				{
					eScanStt = eScanMachineStt.eScanTestFailed;
					return;
				}
				if (gbYStepCnt-- < 0)
				{
					eScanStt = eScanMachineStt.eScanTestOver;
					return;
				}
				((Control)tbCycleCnt).Text = gbYStepCnt.ToString();
				if (eScanMachineStt.eScanTestPulseCntPos == eScanStt)
				{
					eScanStt = eScanMachineStt.eScanYMoveStartXPos;
				}
				else
				{
					eScanStt = eScanMachineStt.eScanYMoveStartXNeg;
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanYMoveStartXPos == eScanStt || eScanMachineStt.eScanYMoveStartXNeg == eScanStt)
		{
			uint[] array6 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetRunState(array6))
			{
				gbSysRunStt = array6[0];
				if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_MCrlAxisRelMove(1, gbYInterval, 10f, 0f))
				{
					if (eScanMachineStt.eScanYMoveStartXPos == eScanStt)
					{
						eScanStt = eScanMachineStt.eScanYMoveXPosIng;
					}
					else
					{
						eScanStt = eScanMachineStt.eScanYMoveXNegIng;
					}
				}
				else
				{
					eScanStt = eScanMachineStt.eScanTestFailed;
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanYMoveXPosIng == eScanStt || eScanMachineStt.eScanYMoveXNegIng == eScanStt)
		{
			uint[] array7 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetRunState(array7))
			{
				if ((array7[0] & PMainFrm.Mccard.Axis2IntrpltSttBit) != (gbSysRunStt & PMainFrm.Mccard.Axis2IntrpltSttBit))
				{
					if (eScanMachineStt.eScanYMoveXPosIng == eScanStt)
					{
						eScanStt = eScanMachineStt.eScanXMoveNegStart;
					}
					else
					{
						eScanStt = eScanMachineStt.eScanXMovePosStart;
					}
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanXMoveNegStart == eScanStt)
		{
			uint[] array8 = new uint[1];
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetRunState(array8))
			{
				gbSysRunStt = array8[0];
				if (PMainFrm.Mccard.FunResOk == fun_MovePulse(bDir: false))
				{
					eScanStt = eScanMachineStt.eScanXMoveNegIng;
				}
			}
			else
			{
				eScanStt = eScanMachineStt.eScanTestFailed;
			}
		}
		else if (eScanMachineStt.eScanTestOver == eScanStt || eScanMachineStt.eScanTestFailed == eScanStt)
		{
			timer1.Enabled = false;
			bnStopScan_Click(sender, e);
			if (eScanMachineStt.eScanTestOver == eScanStt)
			{
				MessageBox.Show(GlobalData.GlobalLanguage.TriggerMoveFrm_TestEnd);
			}
			else
			{
				MessageBox.Show(GlobalData.GlobalLanguage.TriggerMoveFrm_TriggerCntError);
			}
		}
	}

	private void bnStartPos_Click(object sender, EventArgs e)
	{
		fun_MovePulse(bDir: true);
	}

	private void bnStartNeg_Click(object sender, EventArgs e)
	{
		fun_MovePulse(bDir: false);
	}

	private void button1_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		PMainFrm.Mccard.MoCtrCard_EmergencyStopAxisMov(byte.MaxValue);
	}

	private void bnScanStart_Click(object sender, EventArgs e)
	{
		try
		{
			gbPulseCmdCnt = int.Parse(((Control)tbRightPulseCnt).Text);
			gbYInterval = float.Parse(((Control)tbYInterval).Text);
			gbYStepCnt = int.Parse(((Control)tbYSteps).Text);
			((Control)tbCycleCnt).Text = ((Control)tbYSteps).Text;
			((Control)tbRightPulseCnt).Enabled = false;
			((Control)tbCycleCnt).Enabled = false;
			((Control)tbYSteps).Enabled = false;
			((Control)tbYInterval).Enabled = false;
			eScanStt = eScanMachineStt.eScanXMovePosStart;
			timer1.Enabled = false;
			timer1.Interval = 500;
			timer1.Enabled = true;
		}
		catch (Exception)
		{
		}
	}

	private void bnStopScan_Click(object sender, EventArgs e)
	{
		((Control)tbRightPulseCnt).Enabled = true;
		((Control)tbCycleCnt).Enabled = true;
		((Control)tbYSteps).Enabled = true;
		((Control)tbYInterval).Enabled = true;
		timer1.Enabled = false;
	}

	private void tbAskInfo_Click(object sender, EventArgs e)
	{
		int[] array = new int[6];
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_GetPulseMoveInfo(array, (byte)((ListControl)cbAxisId).SelectedIndex))
		{
			((Control)tbPosStart).Text = array[0].ToString();
			((Control)tbPosCrrnt).Text = array[1].ToString();
			((Control)tbPosPulseNext).Text = array[2].ToString();
			((Control)tbPosEnd).Text = array[3].ToString();
			((Control)tbPulseDiff).Text = array[4].ToString();
			((Control)tbPosLast).Text = array[5].ToString();
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MovePulseCtrl));
		bnXMove = new Button();
		label1 = new Label();
		label2 = new Label();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		tbPulseWidX = new TextBox();
		tbPulseIntervalX = new TextBox();
		tbDistTailX = new TextBox();
		tbDistHeadX = new TextBox();
		tbVelX = new TextBox();
		cbOutputX = new ComboBox();
		cbOutput = new Label();
		cbAxisId = new ComboBox();
		label8 = new Label();
		cbWidthMode = new ComboBox();
		label9 = new Label();
		button1 = new Button();
		timer1 = new Timer(components);
		bnStartPos = new Button();
		bnStartNeg = new Button();
		tbDistX = new TextBox();
		lbXPos = new Label();
		lbPulseCnt = new Label();
		bnStopScan = new Button();
		tbCycleCnt = new TextBox();
		lbCycleCnt = new Label();
		bnScanStart = new Button();
		tbYSteps = new TextBox();
		label11 = new Label();
		tbRightPulseCnt = new TextBox();
		label10 = new Label();
		tbYInterval = new TextBox();
		label7 = new Label();
		label12 = new Label();
		label13 = new Label();
		label14 = new Label();
		label15 = new Label();
		label16 = new Label();
		label17 = new Label();
		label18 = new Label();
		tbPosStart = new TextBox();
		tbPosCrrnt = new TextBox();
		tbPosPulseNext = new TextBox();
		tbPosEnd = new TextBox();
		tbPulseDiff = new TextBox();
		tbPosLast = new TextBox();
		tbAskInfo = new Button();
		tabControl1 = new TabControl();
		tabPage1 = new TabPage();
		tabPage2 = new TabPage();
		((Control)tabControl1).SuspendLayout();
		((Control)tabPage1).SuspendLayout();
		((Control)tabPage2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(bnXMove, "bnXMove");
		((Control)bnXMove).Name = "bnXMove";
		((ButtonBase)bnXMove).UseVisualStyleBackColor = true;
		((Control)bnXMove).Click += bnXMove_Click;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(tbPulseWidX, "tbPulseWidX");
		((Control)tbPulseWidX).Name = "tbPulseWidX";
		componentResourceManager.ApplyResources(tbPulseIntervalX, "tbPulseIntervalX");
		((Control)tbPulseIntervalX).Name = "tbPulseIntervalX";
		componentResourceManager.ApplyResources(tbDistTailX, "tbDistTailX");
		((Control)tbDistTailX).Name = "tbDistTailX";
		componentResourceManager.ApplyResources(tbDistHeadX, "tbDistHeadX");
		((Control)tbDistHeadX).Name = "tbDistHeadX";
		componentResourceManager.ApplyResources(tbVelX, "tbVelX");
		((Control)tbVelX).Name = "tbVelX";
		componentResourceManager.ApplyResources(cbOutputX, "cbOutputX");
		cbOutputX.AutoCompleteCustomSource.AddRange(new string[8]
		{
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource1"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource2"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource3"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource4"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource5"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource6"),
			componentResourceManager.GetString("cbOutputX.AutoCompleteCustomSource7")
		});
		((ListControl)cbOutputX).FormattingEnabled = true;
		cbOutputX.Items.AddRange(new object[8]
		{
			componentResourceManager.GetString("cbOutputX.Items"),
			componentResourceManager.GetString("cbOutputX.Items1"),
			componentResourceManager.GetString("cbOutputX.Items2"),
			componentResourceManager.GetString("cbOutputX.Items3"),
			componentResourceManager.GetString("cbOutputX.Items4"),
			componentResourceManager.GetString("cbOutputX.Items5"),
			componentResourceManager.GetString("cbOutputX.Items6"),
			componentResourceManager.GetString("cbOutputX.Items7")
		});
		((Control)cbOutputX).Name = "cbOutputX";
		componentResourceManager.ApplyResources(cbOutput, "cbOutput");
		((Control)cbOutput).Name = "cbOutput";
		componentResourceManager.ApplyResources(cbAxisId, "cbAxisId");
		cbAxisId.AutoCompleteCustomSource.AddRange(new string[8]
		{
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource1"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource2"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource3"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource4"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource5"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource6"),
			componentResourceManager.GetString("cbAxisId.AutoCompleteCustomSource7")
		});
		((ListControl)cbAxisId).FormattingEnabled = true;
		cbAxisId.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("cbAxisId.Items"),
			componentResourceManager.GetString("cbAxisId.Items1"),
			componentResourceManager.GetString("cbAxisId.Items2"),
			componentResourceManager.GetString("cbAxisId.Items3"),
			componentResourceManager.GetString("cbAxisId.Items4"),
			componentResourceManager.GetString("cbAxisId.Items5")
		});
		((Control)cbAxisId).Name = "cbAxisId";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(cbWidthMode, "cbWidthMode");
		cbWidthMode.AutoCompleteCustomSource.AddRange(new string[8]
		{
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource1"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource2"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource3"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource4"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource5"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource6"),
			componentResourceManager.GetString("cbWidthMode.AutoCompleteCustomSource7")
		});
		((ListControl)cbWidthMode).FormattingEnabled = true;
		cbWidthMode.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbWidthMode.Items"),
			componentResourceManager.GetString("cbWidthMode.Items1")
		});
		((Control)cbWidthMode).Name = "cbWidthMode";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(button1, "button1");
		((Control)button1).Name = "button1";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button1).Click += button1_Click;
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(bnStartPos, "bnStartPos");
		((Control)bnStartPos).Name = "bnStartPos";
		((ButtonBase)bnStartPos).UseVisualStyleBackColor = true;
		((Control)bnStartPos).Click += bnStartPos_Click;
		componentResourceManager.ApplyResources(bnStartNeg, "bnStartNeg");
		((Control)bnStartNeg).Name = "bnStartNeg";
		((ButtonBase)bnStartNeg).UseVisualStyleBackColor = true;
		((Control)bnStartNeg).Click += bnStartNeg_Click;
		componentResourceManager.ApplyResources(tbDistX, "tbDistX");
		((Control)tbDistX).Name = "tbDistX";
		componentResourceManager.ApplyResources(lbXPos, "lbXPos");
		lbXPos.BorderStyle = (BorderStyle)1;
		((Control)lbXPos).Name = "lbXPos";
		componentResourceManager.ApplyResources(lbPulseCnt, "lbPulseCnt");
		lbPulseCnt.BorderStyle = (BorderStyle)1;
		((Control)lbPulseCnt).Name = "lbPulseCnt";
		componentResourceManager.ApplyResources(bnStopScan, "bnStopScan");
		((Control)bnStopScan).Name = "bnStopScan";
		((ButtonBase)bnStopScan).UseVisualStyleBackColor = true;
		((Control)bnStopScan).Click += bnStopScan_Click;
		componentResourceManager.ApplyResources(tbCycleCnt, "tbCycleCnt");
		((Control)tbCycleCnt).Name = "tbCycleCnt";
		componentResourceManager.ApplyResources(lbCycleCnt, "lbCycleCnt");
		((Control)lbCycleCnt).Name = "lbCycleCnt";
		componentResourceManager.ApplyResources(bnScanStart, "bnScanStart");
		((Control)bnScanStart).Name = "bnScanStart";
		((ButtonBase)bnScanStart).UseVisualStyleBackColor = true;
		((Control)bnScanStart).Click += bnScanStart_Click;
		componentResourceManager.ApplyResources(tbYSteps, "tbYSteps");
		((Control)tbYSteps).Name = "tbYSteps";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(tbRightPulseCnt, "tbRightPulseCnt");
		((Control)tbRightPulseCnt).Name = "tbRightPulseCnt";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(tbYInterval, "tbYInterval");
		((Control)tbYInterval).Name = "tbYInterval";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label12, "label12");
		((Control)label12).BackColor = SystemColors.ControlDark;
		label12.BorderStyle = (BorderStyle)1;
		((Control)label12).ForeColor = Color.Red;
		((Control)label12).Name = "label12";
		componentResourceManager.ApplyResources(label13, "label13");
		((Control)label13).Name = "label13";
		componentResourceManager.ApplyResources(label14, "label14");
		((Control)label14).Name = "label14";
		componentResourceManager.ApplyResources(label15, "label15");
		((Control)label15).Name = "label15";
		componentResourceManager.ApplyResources(label16, "label16");
		((Control)label16).Name = "label16";
		componentResourceManager.ApplyResources(label17, "label17");
		((Control)label17).Name = "label17";
		componentResourceManager.ApplyResources(label18, "label18");
		((Control)label18).Name = "label18";
		componentResourceManager.ApplyResources(tbPosStart, "tbPosStart");
		((Control)tbPosStart).Name = "tbPosStart";
		componentResourceManager.ApplyResources(tbPosCrrnt, "tbPosCrrnt");
		((Control)tbPosCrrnt).Name = "tbPosCrrnt";
		componentResourceManager.ApplyResources(tbPosPulseNext, "tbPosPulseNext");
		((Control)tbPosPulseNext).Name = "tbPosPulseNext";
		componentResourceManager.ApplyResources(tbPosEnd, "tbPosEnd");
		((Control)tbPosEnd).Name = "tbPosEnd";
		componentResourceManager.ApplyResources(tbPulseDiff, "tbPulseDiff");
		((Control)tbPulseDiff).Name = "tbPulseDiff";
		componentResourceManager.ApplyResources(tbPosLast, "tbPosLast");
		((Control)tbPosLast).Name = "tbPosLast";
		componentResourceManager.ApplyResources(tbAskInfo, "tbAskInfo");
		((Control)tbAskInfo).Name = "tbAskInfo";
		((ButtonBase)tbAskInfo).UseVisualStyleBackColor = true;
		((Control)tbAskInfo).Click += tbAskInfo_Click;
		componentResourceManager.ApplyResources(tabControl1, "tabControl1");
		((Control)tabControl1).Controls.Add((Control)(object)tabPage1);
		((Control)tabControl1).Controls.Add((Control)(object)tabPage2);
		((Control)tabControl1).Name = "tabControl1";
		tabControl1.SelectedIndex = 0;
		componentResourceManager.ApplyResources(tabPage1, "tabPage1");
		((Control)tabPage1).Controls.Add((Control)(object)lbXPos);
		((Control)tabPage1).Controls.Add((Control)(object)bnScanStart);
		((Control)tabPage1).Controls.Add((Control)(object)lbPulseCnt);
		((Control)tabPage1).Controls.Add((Control)(object)label7);
		((Control)tabPage1).Controls.Add((Control)(object)bnStopScan);
		((Control)tabPage1).Controls.Add((Control)(object)tbYInterval);
		((Control)tabPage1).Controls.Add((Control)(object)tbCycleCnt);
		((Control)tabPage1).Controls.Add((Control)(object)label10);
		((Control)tabPage1).Controls.Add((Control)(object)lbCycleCnt);
		((Control)tabPage1).Controls.Add((Control)(object)tbRightPulseCnt);
		((Control)tabPage1).Controls.Add((Control)(object)label11);
		((Control)tabPage1).Controls.Add((Control)(object)tbYSteps);
		((Control)tabPage1).Name = "tabPage1";
		tabPage1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tabPage2, "tabPage2");
		((Control)tabPage2).Controls.Add((Control)(object)tbPosLast);
		((Control)tabPage2).Controls.Add((Control)(object)tbAskInfo);
		((Control)tabPage2).Controls.Add((Control)(object)label13);
		((Control)tabPage2).Controls.Add((Control)(object)label14);
		((Control)tabPage2).Controls.Add((Control)(object)tbPulseDiff);
		((Control)tabPage2).Controls.Add((Control)(object)label15);
		((Control)tabPage2).Controls.Add((Control)(object)tbPosEnd);
		((Control)tabPage2).Controls.Add((Control)(object)label16);
		((Control)tabPage2).Controls.Add((Control)(object)tbPosPulseNext);
		((Control)tabPage2).Controls.Add((Control)(object)label17);
		((Control)tabPage2).Controls.Add((Control)(object)tbPosCrrnt);
		((Control)tabPage2).Controls.Add((Control)(object)label18);
		((Control)tabPage2).Controls.Add((Control)(object)tbPosStart);
		((Control)tabPage2).Name = "tabPage2";
		tabPage2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)tabControl1);
		((Control)this).Controls.Add((Control)(object)label12);
		((Control)this).Controls.Add((Control)(object)bnStartNeg);
		((Control)this).Controls.Add((Control)(object)bnStartPos);
		((Control)this).Controls.Add((Control)(object)button1);
		((Control)this).Controls.Add((Control)(object)label9);
		((Control)this).Controls.Add((Control)(object)cbWidthMode);
		((Control)this).Controls.Add((Control)(object)label8);
		((Control)this).Controls.Add((Control)(object)cbAxisId);
		((Control)this).Controls.Add((Control)(object)cbOutput);
		((Control)this).Controls.Add((Control)(object)cbOutputX);
		((Control)this).Controls.Add((Control)(object)tbVelX);
		((Control)this).Controls.Add((Control)(object)tbDistHeadX);
		((Control)this).Controls.Add((Control)(object)tbDistTailX);
		((Control)this).Controls.Add((Control)(object)tbPulseIntervalX);
		((Control)this).Controls.Add((Control)(object)tbPulseWidX);
		((Control)this).Controls.Add((Control)(object)tbDistX);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)bnXMove);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "MovePulseCtrl";
		((Form)this).Load += MovePulseCtrl_Load;
		((Control)tabControl1).ResumeLayout(false);
		((Control)tabPage1).ResumeLayout(false);
		((Control)tabPage1).PerformLayout();
		((Control)tabPage2).ResumeLayout(false);
		((Control)tabPage2).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
