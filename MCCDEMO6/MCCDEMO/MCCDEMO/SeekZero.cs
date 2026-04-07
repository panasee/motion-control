using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MCCDEMO;

public class SeekZero : Form
{
	private IContainer components;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private TextBox ParaX_Acc;

	private TextBox ParaX_HomeSpd;

	private TextBox ParaX_FanDist;

	private TextBox ParaY_Acc;

	private TextBox ParaY_HomeSpd;

	private TextBox ParaY_FanDist;

	private TextBox ParaZ_Acc;

	private TextBox ParaZ_HomeSpd;

	private TextBox ParaZ_FanDist;

	private TextBox ParaT_Acc;

	private TextBox ParaT_HomeSpd;

	private TextBox ParaT_FanDist;

	private Button BnConfirm;

	private ushort ParaIndex;

	private float ParaVal;

	private MainFrm PMainFrm;

	private TextBox ParaB_FanDist;

	private TextBox ParaB_HomeSpd;

	private TextBox ParaB_Acc;

	private Label label8;

	private TextBox ParaC_FanDist;

	private TextBox ParaC_HomeSpd;

	private TextBox ParaC_Acc;

	private Label label9;

	private byte ConstPara_FanDist;

	private byte ConstPara_HomeSpd;

	private byte ConstPara_Acc;

	private byte ConstPara_AxisA;

	private byte ConstPara_AxisB;

	private byte ConstPara_AxisC;

	private byte ConstPara_AxisX;

	private byte ConstPara_AxisY;

	private byte ConstPara_AxisZ;

	private Button bnSave;

	public SeekZero()
	{
		InitializeComponent();
	}

	private void SeekZero_Load(object sender, EventArgs e)
	{
		ParaIndex = 100;
		ParaVal = 0f;
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		ConstPara_FanDist = 6;
		ConstPara_HomeSpd = 3;
		ConstPara_Acc = 4;
		ConstPara_AxisX = 0;
		ConstPara_AxisY = (byte)(ConstPara_AxisX + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisZ = (byte)(ConstPara_AxisY + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisA = (byte)(ConstPara_AxisZ + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisB = (byte)(ConstPara_AxisA + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisC = (byte)(ConstPara_AxisB + PMainFrm.Mccard.ParaNumPerAxis);
		((Control)ParaX_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisX].ToString("f3");
		((Control)ParaX_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisX].ToString("f3");
		((Control)ParaX_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisX].ToString("f3");
		((Control)ParaY_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisY].ToString("f3");
		((Control)ParaY_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisY].ToString("f3");
		((Control)ParaY_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisY].ToString("f3");
		((Control)ParaZ_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisZ].ToString("f3");
		((Control)ParaZ_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisZ].ToString("f3");
		((Control)ParaZ_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisZ].ToString("f3");
		((Control)ParaT_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisA].ToString("f3");
		((Control)ParaT_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisA].ToString("f3");
		((Control)ParaT_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisA].ToString("f3");
		((Control)ParaB_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisB].ToString("f3");
		((Control)ParaB_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisB].ToString("f3");
		((Control)ParaB_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisB].ToString("f3");
		((Control)ParaC_Acc).Text = PMainFrm.SysPara[ConstPara_Acc + ConstPara_AxisC].ToString("f3");
		((Control)ParaC_HomeSpd).Text = PMainFrm.SysPara[ConstPara_HomeSpd + ConstPara_AxisC].ToString("f3");
		((Control)ParaC_FanDist).Text = PMainFrm.SysPara[ConstPara_FanDist + ConstPara_AxisC].ToString("f3");
	}

	private void SetSysParaToMCU(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		TextBox val = (TextBox)sender;
		if (((Control)ParaX_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisX);
			ParaVal = float.Parse(((Control)ParaX_Acc).Text);
		}
		else if (((Control)ParaX_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisX);
			ParaVal = float.Parse(((Control)ParaX_HomeSpd).Text);
		}
		else if (((Control)ParaX_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisX);
			ParaVal = float.Parse(((Control)ParaX_FanDist).Text);
		}
		else if (((Control)ParaY_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisY);
			ParaVal = float.Parse(((Control)ParaY_Acc).Text);
		}
		else if (((Control)ParaY_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisY);
			ParaVal = float.Parse(((Control)ParaY_HomeSpd).Text);
		}
		else if (((Control)ParaY_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisY);
			ParaVal = float.Parse(((Control)ParaY_FanDist).Text);
		}
		else if (((Control)ParaZ_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisZ);
			ParaVal = float.Parse(((Control)ParaZ_Acc).Text);
		}
		else if (((Control)ParaZ_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisZ);
			ParaVal = float.Parse(((Control)ParaZ_HomeSpd).Text);
		}
		else if (((Control)ParaZ_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisZ);
			ParaVal = float.Parse(((Control)ParaZ_FanDist).Text);
		}
		else if (((Control)ParaT_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisA);
			ParaVal = float.Parse(((Control)ParaT_Acc).Text);
		}
		else if (((Control)ParaT_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisA);
			ParaVal = float.Parse(((Control)ParaT_HomeSpd).Text);
		}
		else if (((Control)ParaT_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisA);
			ParaVal = float.Parse(((Control)ParaT_FanDist).Text);
		}
		else if (((Control)ParaB_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisB);
			ParaVal = float.Parse(((Control)ParaB_Acc).Text);
		}
		else if (((Control)ParaB_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisB);
			ParaVal = float.Parse(((Control)ParaB_HomeSpd).Text);
		}
		else if (((Control)ParaB_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisB);
			ParaVal = float.Parse(((Control)ParaB_FanDist).Text);
		}
		else if (((Control)ParaC_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_Acc + ConstPara_AxisC);
			ParaVal = float.Parse(((Control)ParaC_Acc).Text);
		}
		else if (((Control)ParaC_HomeSpd).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_HomeSpd + ConstPara_AxisC);
			ParaVal = float.Parse(((Control)ParaC_HomeSpd).Text);
		}
		else if (((Control)ParaC_FanDist).Name == ((Control)val).Name)
		{
			ParaIndex = (byte)(ConstPara_FanDist + ConstPara_AxisC);
			ParaVal = float.Parse(((Control)ParaC_FanDist).Text);
		}
		else
		{
			ParaIndex = 100;
			ParaVal = 0f;
		}
		if (ParaIndex < PMainFrm.Mccard.MaxAxisNum * PMainFrm.Mccard.ParaNumPerAxis && PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), ParaVal))
		{
			PMainFrm.SysPara[ParaIndex] = ParaVal;
		}
	}

	private void BnConfirm_Click(object sender, EventArgs e)
	{
		if (ParaIndex < PMainFrm.Mccard.MaxAxisNum * PMainFrm.Mccard.ParaNumPerAxis && PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), ParaVal))
		{
			PMainFrm.SysPara[ParaIndex] = ParaVal;
		}
	}

	private void bnSave_Click(object sender, EventArgs e)
	{
		PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM();
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SeekZero));
		label1 = new Label();
		label2 = new Label();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		label7 = new Label();
		ParaX_Acc = new TextBox();
		ParaX_HomeSpd = new TextBox();
		ParaX_FanDist = new TextBox();
		ParaY_Acc = new TextBox();
		ParaY_HomeSpd = new TextBox();
		ParaY_FanDist = new TextBox();
		ParaZ_Acc = new TextBox();
		ParaZ_HomeSpd = new TextBox();
		ParaZ_FanDist = new TextBox();
		ParaT_Acc = new TextBox();
		ParaT_HomeSpd = new TextBox();
		ParaT_FanDist = new TextBox();
		BnConfirm = new Button();
		ParaB_FanDist = new TextBox();
		ParaB_HomeSpd = new TextBox();
		ParaB_Acc = new TextBox();
		label8 = new Label();
		ParaC_FanDist = new TextBox();
		ParaC_HomeSpd = new TextBox();
		ParaC_Acc = new TextBox();
		label9 = new Label();
		bnSave = new Button();
		((Control)this).SuspendLayout();
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
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(ParaX_Acc, "ParaX_Acc");
		((Control)ParaX_Acc).Name = "ParaX_Acc";
		((Control)ParaX_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaX_HomeSpd, "ParaX_HomeSpd");
		((Control)ParaX_HomeSpd).Name = "ParaX_HomeSpd";
		((Control)ParaX_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaX_FanDist, "ParaX_FanDist");
		((Control)ParaX_FanDist).Name = "ParaX_FanDist";
		((Control)ParaX_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaY_Acc, "ParaY_Acc");
		((Control)ParaY_Acc).Name = "ParaY_Acc";
		((Control)ParaY_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaY_HomeSpd, "ParaY_HomeSpd");
		((Control)ParaY_HomeSpd).Name = "ParaY_HomeSpd";
		((Control)ParaY_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaY_FanDist, "ParaY_FanDist");
		((Control)ParaY_FanDist).Name = "ParaY_FanDist";
		((Control)ParaY_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_Acc, "ParaZ_Acc");
		((Control)ParaZ_Acc).Name = "ParaZ_Acc";
		((Control)ParaZ_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_HomeSpd, "ParaZ_HomeSpd");
		((Control)ParaZ_HomeSpd).Name = "ParaZ_HomeSpd";
		((Control)ParaZ_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_FanDist, "ParaZ_FanDist");
		((Control)ParaZ_FanDist).Name = "ParaZ_FanDist";
		((Control)ParaZ_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaT_Acc, "ParaT_Acc");
		((Control)ParaT_Acc).Name = "ParaT_Acc";
		((Control)ParaT_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaT_HomeSpd, "ParaT_HomeSpd");
		((Control)ParaT_HomeSpd).Name = "ParaT_HomeSpd";
		((Control)ParaT_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaT_FanDist, "ParaT_FanDist");
		((Control)ParaT_FanDist).Name = "ParaT_FanDist";
		((Control)ParaT_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(BnConfirm, "BnConfirm");
		((Control)BnConfirm).Name = "BnConfirm";
		((ButtonBase)BnConfirm).UseVisualStyleBackColor = true;
		((Control)BnConfirm).Click += BnConfirm_Click;
		componentResourceManager.ApplyResources(ParaB_FanDist, "ParaB_FanDist");
		((Control)ParaB_FanDist).Name = "ParaB_FanDist";
		((Control)ParaB_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaB_HomeSpd, "ParaB_HomeSpd");
		((Control)ParaB_HomeSpd).Name = "ParaB_HomeSpd";
		((Control)ParaB_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaB_Acc, "ParaB_Acc");
		((Control)ParaB_Acc).Name = "ParaB_Acc";
		((Control)ParaB_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(ParaC_FanDist, "ParaC_FanDist");
		((Control)ParaC_FanDist).Name = "ParaC_FanDist";
		((Control)ParaC_FanDist).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaC_HomeSpd, "ParaC_HomeSpd");
		((Control)ParaC_HomeSpd).Name = "ParaC_HomeSpd";
		((Control)ParaC_HomeSpd).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(ParaC_Acc, "ParaC_Acc");
		((Control)ParaC_Acc).Name = "ParaC_Acc";
		((Control)ParaC_Acc).Leave += SetSysParaToMCU;
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(bnSave, "bnSave");
		((Control)bnSave).Name = "bnSave";
		((ButtonBase)bnSave).UseVisualStyleBackColor = true;
		((Control)bnSave).Click += bnSave_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)bnSave);
		((Control)this).Controls.Add((Control)(object)ParaC_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaC_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaC_Acc);
		((Control)this).Controls.Add((Control)(object)label9);
		((Control)this).Controls.Add((Control)(object)ParaB_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaB_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaB_Acc);
		((Control)this).Controls.Add((Control)(object)label8);
		((Control)this).Controls.Add((Control)(object)BnConfirm);
		((Control)this).Controls.Add((Control)(object)ParaT_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaZ_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaY_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaX_FanDist);
		((Control)this).Controls.Add((Control)(object)ParaT_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaZ_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaY_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaX_HomeSpd);
		((Control)this).Controls.Add((Control)(object)ParaT_Acc);
		((Control)this).Controls.Add((Control)(object)ParaZ_Acc);
		((Control)this).Controls.Add((Control)(object)ParaY_Acc);
		((Control)this).Controls.Add((Control)(object)ParaX_Acc);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label7);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)label1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "SeekZero";
		((Form)this).Load += SeekZero_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
