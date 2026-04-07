using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class ManCtrlSin : Form
{
	private MainFrm pMainFrm = new MainFrm();

	private IContainer components;

	private GroupBox groupBox1;

	private Label label2;

	private Label lbDist;

	private RadioButton rbMotionAbs;

	private TextBox tbSpdC;

	private TextBox tbDistC;

	private TextBox tbSpdB;

	private TextBox tbDistB;

	private TextBox tbSpdA;

	private TextBox tbDistA;

	private TextBox tbSpdZ;

	private TextBox tbDistZ;

	private TextBox tbSpdY;

	private TextBox tbDistY;

	private TextBox tbSpdX;

	private TextBox tbDistX;

	private Button bnParaStopC;

	private Button bnEmStopC;

	private Button bnParaStopB;

	private Button bnEmStopB;

	private Button bnParaStopA;

	private Button bnEmStopA;

	private Button bnParaStopZ;

	private Button bnEmStopZ;

	private Button bnParaStopY;

	private Button bnEmStopY;

	private Button bnParaStopX;

	private Button bnEmStopX;

	private Button CNeg;

	private Button CPos;

	private Button BNeg;

	private Button BPos;

	private Button TNeg;

	private Button TPos;

	private Button ZNeg;

	private Button ZPos;

	private Button XNeg;

	private Button YNeg;

	private Button YPos;

	private Button XPos;

	private RadioButton rbMotionRel;

	private Label label1;

	private TextBox tbCoefAC;

	private TextBox tbCoefAB;

	private TextBox tbCoefAA;

	private TextBox tbCoefAZ;

	private TextBox tbCoefAY;

	private TextBox tbCoefAX;

	public ManCtrlSin()
	{
		InitializeComponent();
	}

	private void rbMotionAbs_CheckedChanged(object sender, EventArgs e)
	{
		if (rbMotionAbs.Checked)
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModePos;
		}
		else
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModeDist;
		}
		((Control)tbDistX).Enabled = true;
		((Control)tbDistY).Enabled = true;
		((Control)tbDistZ).Enabled = true;
		((Control)tbDistA).Enabled = true;
		((Control)tbDistB).Enabled = true;
		((Control)tbDistC).Enabled = true;
	}

	private void rbMotionRel_CheckedChanged(object sender, EventArgs e)
	{
		if (rbMotionRel.Checked)
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModeDist;
		}
		else
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModePos;
		}
		((Control)tbDistX).Enabled = true;
		((Control)tbDistY).Enabled = true;
		((Control)tbDistZ).Enabled = true;
		((Control)tbDistA).Enabled = true;
		((Control)tbDistB).Enabled = true;
		((Control)tbDistC).Enabled = true;
	}

	private void ManualOpEx_Load(object sender, EventArgs e)
	{
		pMainFrm = (MainFrm)(object)((Form)this).Owner;
		rbMotionRel.Checked = true;
		((Control)tbDistX).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseXAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistY).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseYAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistZ).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseZAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistA).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseAAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistB).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseBAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistC).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseCAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbSpdX).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseXAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdY).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseYAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdZ).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseZAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdA).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseAAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdB).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseBAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdC).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseCAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
	}

	private void bnEmStopX_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)bnEmStopX).Name)
			{
				axisId = 0;
			}
			else if (((Control)val).Name == ((Control)bnEmStopY).Name)
			{
				axisId = 1;
			}
			else if (((Control)val).Name == ((Control)bnEmStopZ).Name)
			{
				axisId = 2;
			}
			else if (((Control)val).Name == ((Control)bnEmStopA).Name)
			{
				axisId = 3;
			}
			else if (((Control)val).Name == ((Control)bnEmStopB).Name)
			{
				axisId = 4;
			}
			else if (((Control)val).Name == ((Control)bnEmStopC).Name)
			{
				axisId = 5;
			}
			pMainFrm.Mccard.MoCtrCard_EmergencyStopAxisMov(axisId);
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void bnParaStopC_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		byte axisId = 0;
		float value = 0f;
		try
		{
			if (((Control)val).Name == ((Control)bnParaStopX).Name)
			{
				axisId = 0;
				value = float.Parse(((Control)tbCoefAX).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopY).Name)
			{
				axisId = 1;
				value = float.Parse(((Control)tbCoefAY).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopZ).Name)
			{
				axisId = 2;
				value = float.Parse(((Control)tbCoefAZ).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopA).Name)
			{
				axisId = 3;
				value = float.Parse(((Control)tbCoefAA).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopB).Name)
			{
				axisId = 4;
				value = float.Parse(((Control)tbCoefAB).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopC).Name)
			{
				axisId = 5;
				value = float.Parse(((Control)tbCoefAC).Text);
			}
			pMainFrm.Mccard.MoCtrCard_MCrlAxisSinAbsMove(axisId, Math.Abs(value), 0f);
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void XPos_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		float num = 0f;
		float value = 0f;
		float value2 = 0f;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)XPos).Name || ((Control)val).Name == ((Control)XNeg).Name)
			{
				if (((Control)val).Name == ((Control)XNeg).Name)
				{
					num = 0f - float.Parse(((Control)tbDistX).Text);
					value = 0f - float.Parse(((Control)tbSpdX).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAX).Text);
				}
				else
				{
					num = float.Parse(((Control)tbDistX).Text);
					value = float.Parse(((Control)tbSpdX).Text);
					value2 = float.Parse(((Control)tbCoefAX).Text);
				}
				axisId = 0;
			}
			else if (((Control)val).Name == ((Control)YPos).Name || ((Control)val).Name == ((Control)YNeg).Name)
			{
				if (((Control)val).Name == ((Control)YPos).Name)
				{
					num = float.Parse(((Control)tbDistY).Text);
					value = float.Parse(((Control)tbSpdY).Text);
					value2 = float.Parse(((Control)tbCoefAY).Text);
				}
				else
				{
					num = 0f - float.Parse(((Control)tbDistY).Text);
					value = 0f - float.Parse(((Control)tbSpdY).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAY).Text);
				}
				axisId = 1;
			}
			else if (((Control)val).Name == ((Control)ZPos).Name || ((Control)val).Name == ((Control)ZNeg).Name)
			{
				if (((Control)val).Name == ((Control)ZPos).Name)
				{
					num = float.Parse(((Control)tbDistZ).Text);
					value = float.Parse(((Control)tbSpdZ).Text);
					value2 = float.Parse(((Control)tbCoefAZ).Text);
				}
				else
				{
					num = 0f - float.Parse(((Control)tbDistZ).Text);
					value = 0f - float.Parse(((Control)tbSpdZ).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAZ).Text);
				}
				axisId = 2;
			}
			else if (((Control)val).Name == ((Control)TPos).Name || ((Control)val).Name == ((Control)TNeg).Name)
			{
				if (((Control)val).Name == ((Control)TPos).Name)
				{
					num = float.Parse(((Control)tbDistA).Text);
					value = float.Parse(((Control)tbSpdA).Text);
					value2 = float.Parse(((Control)tbCoefAA).Text);
				}
				else
				{
					num = 0f - float.Parse(((Control)tbDistA).Text);
					value = 0f - float.Parse(((Control)tbSpdA).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAA).Text);
				}
				axisId = 3;
			}
			else if (((Control)val).Name == ((Control)BPos).Name || ((Control)val).Name == ((Control)BNeg).Name)
			{
				if (((Control)val).Name == ((Control)BPos).Name)
				{
					num = float.Parse(((Control)tbDistB).Text);
					value = float.Parse(((Control)tbSpdB).Text);
					value2 = float.Parse(((Control)tbCoefAB).Text);
				}
				else
				{
					num = 0f - float.Parse(((Control)tbDistB).Text);
					value = 0f - float.Parse(((Control)tbSpdB).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAB).Text);
				}
				axisId = 4;
			}
			else if (((Control)val).Name == ((Control)CPos).Name || ((Control)val).Name == ((Control)CNeg).Name)
			{
				if (((Control)val).Name == ((Control)CPos).Name)
				{
					num = float.Parse(((Control)tbDistC).Text);
					value = float.Parse(((Control)tbSpdC).Text);
					value2 = float.Parse(((Control)tbCoefAC).Text);
				}
				else
				{
					num = 0f - float.Parse(((Control)tbDistC).Text);
					value = 0f - float.Parse(((Control)tbSpdC).Text);
					value2 = 0f - float.Parse(((Control)tbCoefAC).Text);
				}
				axisId = 5;
			}
			if (rbMotionAbs.Checked)
			{
				pMainFrm.Mccard.MoCtrCard_MCrlAxisSinAbsMove(axisId, Math.Abs(value2), Math.Abs(num), Math.Abs(value), 0f);
			}
			else if (rbMotionRel.Checked)
			{
				pMainFrm.Mccard.MoCtrCard_MCrlAxisSinRelMove(axisId, Math.Abs(value2), num, Math.Abs(value), 0f);
			}
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
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
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ManCtrlSin));
		groupBox1 = new GroupBox();
		label1 = new Label();
		tbCoefAC = new TextBox();
		tbCoefAB = new TextBox();
		tbCoefAA = new TextBox();
		tbCoefAZ = new TextBox();
		tbCoefAY = new TextBox();
		tbCoefAX = new TextBox();
		label2 = new Label();
		lbDist = new Label();
		rbMotionRel = new RadioButton();
		rbMotionAbs = new RadioButton();
		tbSpdC = new TextBox();
		tbDistC = new TextBox();
		tbSpdB = new TextBox();
		tbDistB = new TextBox();
		tbSpdA = new TextBox();
		tbDistA = new TextBox();
		tbSpdZ = new TextBox();
		tbDistZ = new TextBox();
		tbSpdY = new TextBox();
		tbDistY = new TextBox();
		tbSpdX = new TextBox();
		tbDistX = new TextBox();
		bnParaStopC = new Button();
		bnEmStopC = new Button();
		bnParaStopB = new Button();
		bnEmStopB = new Button();
		bnParaStopA = new Button();
		bnEmStopA = new Button();
		bnParaStopZ = new Button();
		bnEmStopZ = new Button();
		bnParaStopY = new Button();
		bnEmStopY = new Button();
		bnParaStopX = new Button();
		bnEmStopX = new Button();
		CNeg = new Button();
		CPos = new Button();
		BNeg = new Button();
		BPos = new Button();
		TNeg = new Button();
		TPos = new Button();
		ZNeg = new Button();
		ZPos = new Button();
		XNeg = new Button();
		YNeg = new Button();
		YPos = new Button();
		XPos = new Button();
		((Control)groupBox1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAC);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAB);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAA);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAY);
		((Control)groupBox1).Controls.Add((Control)(object)tbCoefAX);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)lbDist);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionRel);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionAbs);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdC);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistC);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdB);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistB);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdA);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistA);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdY);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistY);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdX);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistX);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopC);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopC);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopB);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopB);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopA);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopA);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopY);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopY);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopX);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopX);
		((Control)groupBox1).Controls.Add((Control)(object)CNeg);
		((Control)groupBox1).Controls.Add((Control)(object)CPos);
		((Control)groupBox1).Controls.Add((Control)(object)BNeg);
		((Control)groupBox1).Controls.Add((Control)(object)BPos);
		((Control)groupBox1).Controls.Add((Control)(object)TNeg);
		((Control)groupBox1).Controls.Add((Control)(object)TPos);
		((Control)groupBox1).Controls.Add((Control)(object)ZNeg);
		((Control)groupBox1).Controls.Add((Control)(object)ZPos);
		((Control)groupBox1).Controls.Add((Control)(object)XNeg);
		((Control)groupBox1).Controls.Add((Control)(object)YNeg);
		((Control)groupBox1).Controls.Add((Control)(object)YPos);
		((Control)groupBox1).Controls.Add((Control)(object)XPos);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbCoefAC, "tbCoefAC");
		((Control)tbCoefAC).Name = "tbCoefAC";
		componentResourceManager.ApplyResources(tbCoefAB, "tbCoefAB");
		((Control)tbCoefAB).Name = "tbCoefAB";
		componentResourceManager.ApplyResources(tbCoefAA, "tbCoefAA");
		((Control)tbCoefAA).Name = "tbCoefAA";
		componentResourceManager.ApplyResources(tbCoefAZ, "tbCoefAZ");
		((Control)tbCoefAZ).Name = "tbCoefAZ";
		componentResourceManager.ApplyResources(tbCoefAY, "tbCoefAY");
		((Control)tbCoefAY).Name = "tbCoefAY";
		componentResourceManager.ApplyResources(tbCoefAX, "tbCoefAX");
		((Control)tbCoefAX).Name = "tbCoefAX";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(lbDist, "lbDist");
		((Control)lbDist).Name = "lbDist";
		componentResourceManager.ApplyResources(rbMotionRel, "rbMotionRel");
		((Control)rbMotionRel).Name = "rbMotionRel";
		rbMotionRel.TabStop = true;
		((ButtonBase)rbMotionRel).UseVisualStyleBackColor = true;
		rbMotionRel.CheckedChanged += rbMotionRel_CheckedChanged;
		componentResourceManager.ApplyResources(rbMotionAbs, "rbMotionAbs");
		((Control)rbMotionAbs).Name = "rbMotionAbs";
		rbMotionAbs.TabStop = true;
		((ButtonBase)rbMotionAbs).UseVisualStyleBackColor = true;
		rbMotionAbs.CheckedChanged += rbMotionAbs_CheckedChanged;
		componentResourceManager.ApplyResources(tbSpdC, "tbSpdC");
		((Control)tbSpdC).Name = "tbSpdC";
		componentResourceManager.ApplyResources(tbDistC, "tbDistC");
		((Control)tbDistC).Name = "tbDistC";
		componentResourceManager.ApplyResources(tbSpdB, "tbSpdB");
		((Control)tbSpdB).Name = "tbSpdB";
		componentResourceManager.ApplyResources(tbDistB, "tbDistB");
		((Control)tbDistB).Name = "tbDistB";
		componentResourceManager.ApplyResources(tbSpdA, "tbSpdA");
		((Control)tbSpdA).Name = "tbSpdA";
		componentResourceManager.ApplyResources(tbDistA, "tbDistA");
		((Control)tbDistA).Name = "tbDistA";
		componentResourceManager.ApplyResources(tbSpdZ, "tbSpdZ");
		((Control)tbSpdZ).Name = "tbSpdZ";
		componentResourceManager.ApplyResources(tbDistZ, "tbDistZ");
		((Control)tbDistZ).Name = "tbDistZ";
		componentResourceManager.ApplyResources(tbSpdY, "tbSpdY");
		((Control)tbSpdY).Name = "tbSpdY";
		componentResourceManager.ApplyResources(tbDistY, "tbDistY");
		((Control)tbDistY).Name = "tbDistY";
		componentResourceManager.ApplyResources(tbSpdX, "tbSpdX");
		((Control)tbSpdX).Name = "tbSpdX";
		componentResourceManager.ApplyResources(tbDistX, "tbDistX");
		((Control)tbDistX).Name = "tbDistX";
		componentResourceManager.ApplyResources(bnParaStopC, "bnParaStopC");
		((Control)bnParaStopC).Name = "bnParaStopC";
		((ButtonBase)bnParaStopC).UseVisualStyleBackColor = true;
		((Control)bnParaStopC).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopC, "bnEmStopC");
		((Control)bnEmStopC).Name = "bnEmStopC";
		((ButtonBase)bnEmStopC).UseVisualStyleBackColor = true;
		((Control)bnEmStopC).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopB, "bnParaStopB");
		((Control)bnParaStopB).Name = "bnParaStopB";
		((ButtonBase)bnParaStopB).UseVisualStyleBackColor = true;
		((Control)bnParaStopB).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopB, "bnEmStopB");
		((Control)bnEmStopB).Name = "bnEmStopB";
		((ButtonBase)bnEmStopB).UseVisualStyleBackColor = true;
		((Control)bnEmStopB).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopA, "bnParaStopA");
		((Control)bnParaStopA).Name = "bnParaStopA";
		((ButtonBase)bnParaStopA).UseVisualStyleBackColor = true;
		((Control)bnParaStopA).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopA, "bnEmStopA");
		((Control)bnEmStopA).Name = "bnEmStopA";
		((ButtonBase)bnEmStopA).UseVisualStyleBackColor = true;
		((Control)bnEmStopA).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopZ, "bnParaStopZ");
		((Control)bnParaStopZ).Name = "bnParaStopZ";
		((ButtonBase)bnParaStopZ).UseVisualStyleBackColor = true;
		((Control)bnParaStopZ).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopZ, "bnEmStopZ");
		((Control)bnEmStopZ).Name = "bnEmStopZ";
		((ButtonBase)bnEmStopZ).UseVisualStyleBackColor = true;
		((Control)bnEmStopZ).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopY, "bnParaStopY");
		((Control)bnParaStopY).Name = "bnParaStopY";
		((ButtonBase)bnParaStopY).UseVisualStyleBackColor = true;
		((Control)bnParaStopY).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopY, "bnEmStopY");
		((Control)bnEmStopY).Name = "bnEmStopY";
		((ButtonBase)bnEmStopY).UseVisualStyleBackColor = true;
		((Control)bnEmStopY).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopX, "bnParaStopX");
		((Control)bnParaStopX).Name = "bnParaStopX";
		((ButtonBase)bnParaStopX).UseVisualStyleBackColor = true;
		((Control)bnParaStopX).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopX, "bnEmStopX");
		((Control)bnEmStopX).Name = "bnEmStopX";
		((ButtonBase)bnEmStopX).UseVisualStyleBackColor = true;
		((Control)bnEmStopX).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(CNeg, "CNeg");
		((Control)CNeg).Name = "CNeg";
		((ButtonBase)CNeg).UseVisualStyleBackColor = true;
		((Control)CNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(CPos, "CPos");
		((Control)CPos).Name = "CPos";
		((ButtonBase)CPos).UseVisualStyleBackColor = true;
		((Control)CPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(BNeg, "BNeg");
		((Control)BNeg).Name = "BNeg";
		((ButtonBase)BNeg).UseVisualStyleBackColor = true;
		((Control)BNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(BPos, "BPos");
		((Control)BPos).Name = "BPos";
		((ButtonBase)BPos).UseVisualStyleBackColor = true;
		((Control)BPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(TNeg, "TNeg");
		((Control)TNeg).Name = "TNeg";
		((ButtonBase)TNeg).UseVisualStyleBackColor = true;
		((Control)TNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(TPos, "TPos");
		((Control)TPos).Name = "TPos";
		((ButtonBase)TPos).UseVisualStyleBackColor = true;
		((Control)TPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(ZNeg, "ZNeg");
		((Control)ZNeg).Name = "ZNeg";
		((ButtonBase)ZNeg).UseVisualStyleBackColor = true;
		((Control)ZNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(ZPos, "ZPos");
		((Control)ZPos).Name = "ZPos";
		((ButtonBase)ZPos).UseVisualStyleBackColor = true;
		((Control)ZPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(XNeg, "XNeg");
		((Control)XNeg).Name = "XNeg";
		((ButtonBase)XNeg).UseVisualStyleBackColor = true;
		((Control)XNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(YNeg, "YNeg");
		((Control)YNeg).Name = "YNeg";
		((ButtonBase)YNeg).UseVisualStyleBackColor = true;
		((Control)YNeg).Click += XPos_Click;
		componentResourceManager.ApplyResources(YPos, "YPos");
		((Control)YPos).Name = "YPos";
		((ButtonBase)YPos).UseVisualStyleBackColor = true;
		((Control)YPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(XPos, "XPos");
		((Control)XPos).Name = "XPos";
		((ButtonBase)XPos).UseVisualStyleBackColor = true;
		((Control)XPos).Click += XPos_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Name = "ManCtrlSin";
		((Form)this).Load += ManualOpEx_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
