using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class frmAxisExPara : Form
{
	private MainFrm PMainFrm;

	private IContainer components;

	private TextBox tbMBEncPulseDLX;

	private Label lbDist;

	private GroupBox groupBox1;

	private Label label3;

	private TextBox tbMBEncPulseDLC;

	private Label label4;

	private TextBox tbMBEncPulseDLB;

	private Label label5;

	private TextBox tbMBEncPulseDLA;

	private Label label2;

	private TextBox tbMBEncPulseDLZ;

	private Label label1;

	private TextBox tbMBEncPulseDLY;

	private Button bnSetPara;

	private Button bnSaveRom;

	private Button bnUpdate;

	private GroupBox groupBox2;

	private Label label6;

	private TextBox tbEncOffst5;

	private Label label7;

	private TextBox tbEncOffst4;

	private Label label8;

	private TextBox tbEncOffst3;

	private Label label9;

	private TextBox tbEncOffst2;

	private Label label10;

	private TextBox tbEncOffst1;

	private Label label11;

	private TextBox tbEncOffst0;

	public frmAxisExPara(object mOwner)
	{
		InitializeComponent();
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		PMainFrm = (MainFrm)mOwner;
	}

	public ushort fun_UpdateAixsExPara(byte nParaClass, byte nParaIndx, float[] fVal)
	{
		ushort result = 1;
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_ReadPara(nParaClass, nParaIndx, fVal))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.AxisExParaFrm_ReadAxisParameterFailed);
			result = 0;
		}
		return result;
	}

	public ushort fun_UpdateAixsExPara(byte nParaClass, byte nParaIndx, int[] IVal)
	{
		ushort result = 1;
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_ReadPara(nParaClass, nParaIndx, IVal))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.AxisExParaFrm_ReadAxisParameterFailed);
			result = 0;
		}
		return result;
	}

	public ushort fun_SetAxisExPara(byte nParaClass, byte nParaIndx, int IVal)
	{
		ushort result = 1;
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(nParaClass, nParaIndx, IVal))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.AxisExParaFrm_SetAxisParameterFailed);
			result = 0;
		}
		return result;
	}

	public ushort fun_SetAxisExPara(byte nParaClass, byte nParaIndx, float fVal)
	{
		ushort result = 1;
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(nParaClass, nParaIndx, fVal))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.AxisExParaFrm_SetAxisParameterFailed);
			result = 0;
		}
		return result;
	}

	private void bnUpdate_Click(object sender, EventArgs e)
	{
		float[] array = new float[1];
		int[] array2 = new int[1];
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 0, array) > 0)
		{
			((Control)tbMBEncPulseDLX).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 1, array) > 0)
		{
			((Control)tbMBEncPulseDLY).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 2, array) > 0)
		{
			((Control)tbMBEncPulseDLZ).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 3, array) > 0)
		{
			((Control)tbMBEncPulseDLA).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 4, array) > 0)
		{
			((Control)tbMBEncPulseDLB).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExReg, 5, array) > 0)
		{
			((Control)tbMBEncPulseDLC).Text = array[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 0, array2) > 0)
		{
			((Control)tbEncOffst0).Text = array2[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 1, array2) > 0)
		{
			((Control)tbEncOffst1).Text = array2[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 2, array2) > 0)
		{
			((Control)tbEncOffst2).Text = array2[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 3, array2) > 0)
		{
			((Control)tbEncOffst3).Text = array2[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 4, array2) > 0)
		{
			((Control)tbEncOffst4).Text = array2[0].ToString();
		}
		if (fun_UpdateAixsExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 5, array2) > 0)
		{
			((Control)tbEncOffst5).Text = array2[0].ToString();
		}
	}

	private void bnSetPara_Click(object sender, EventArgs e)
	{
		float num = 0f;
		int num2 = 0;
		try
		{
			num = float.Parse(((Control)tbMBEncPulseDLX).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 0, num);
			num = float.Parse(((Control)tbMBEncPulseDLY).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 1, num);
			num = float.Parse(((Control)tbMBEncPulseDLZ).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 2, num);
			num = float.Parse(((Control)tbMBEncPulseDLA).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 3, num);
			num = float.Parse(((Control)tbMBEncPulseDLB).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 4, num);
			num = float.Parse(((Control)tbMBEncPulseDLC).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExReg, 5, num);
			num2 = int.Parse(((Control)tbEncOffst0).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 0, num2);
			num2 = int.Parse(((Control)tbEncOffst1).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 1, num2);
			num2 = int.Parse(((Control)tbEncOffst2).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 2, num2);
			num2 = int.Parse(((Control)tbEncOffst3).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 3, num2);
			num2 = int.Parse(((Control)tbEncOffst4).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 4, num2);
			num2 = int.Parse(((Control)tbEncOffst5).Text);
			fun_SetAxisExPara(PMainFrm.Mccard.ParaClassAxisExRegAbsOffset, 5, num2);
		}
		catch (Exception ex)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.AxisExParaFrm_ParameterFormatError + ex.ToString());
		}
	}

	private void bnSaveRom_Click(object sender, EventArgs e)
	{
		PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM();
	}

	private void frmAxisExPara_Load(object sender, EventArgs e)
	{
		bnUpdate_Click(sender, e);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmAxisExPara));
		tbMBEncPulseDLX = new TextBox();
		lbDist = new Label();
		groupBox1 = new GroupBox();
		label3 = new Label();
		tbMBEncPulseDLC = new TextBox();
		label4 = new Label();
		tbMBEncPulseDLB = new TextBox();
		label5 = new Label();
		tbMBEncPulseDLA = new TextBox();
		label2 = new Label();
		tbMBEncPulseDLZ = new TextBox();
		label1 = new Label();
		tbMBEncPulseDLY = new TextBox();
		bnSetPara = new Button();
		bnSaveRom = new Button();
		bnUpdate = new Button();
		groupBox2 = new GroupBox();
		label6 = new Label();
		tbEncOffst5 = new TextBox();
		label7 = new Label();
		tbEncOffst4 = new TextBox();
		label8 = new Label();
		tbEncOffst3 = new TextBox();
		label9 = new Label();
		tbEncOffst2 = new TextBox();
		label10 = new Label();
		tbEncOffst1 = new TextBox();
		label11 = new Label();
		tbEncOffst0 = new TextBox();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tbMBEncPulseDLX, "tbMBEncPulseDLX");
		((Control)tbMBEncPulseDLX).Name = "tbMBEncPulseDLX";
		componentResourceManager.ApplyResources(lbDist, "lbDist");
		((Control)lbDist).Name = "lbDist";
		((Control)groupBox1).Controls.Add((Control)(object)label3);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLC);
		((Control)groupBox1).Controls.Add((Control)(object)label4);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLB);
		((Control)groupBox1).Controls.Add((Control)(object)label5);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLA);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLZ);
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLY);
		((Control)groupBox1).Controls.Add((Control)(object)lbDist);
		((Control)groupBox1).Controls.Add((Control)(object)tbMBEncPulseDLX);
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(tbMBEncPulseDLC, "tbMBEncPulseDLC");
		((Control)tbMBEncPulseDLC).Name = "tbMBEncPulseDLC";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(tbMBEncPulseDLB, "tbMBEncPulseDLB");
		((Control)tbMBEncPulseDLB).Name = "tbMBEncPulseDLB";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(tbMBEncPulseDLA, "tbMBEncPulseDLA");
		((Control)tbMBEncPulseDLA).Name = "tbMBEncPulseDLA";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(tbMBEncPulseDLZ, "tbMBEncPulseDLZ");
		((Control)tbMBEncPulseDLZ).Name = "tbMBEncPulseDLZ";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbMBEncPulseDLY, "tbMBEncPulseDLY");
		((Control)tbMBEncPulseDLY).Name = "tbMBEncPulseDLY";
		componentResourceManager.ApplyResources(bnSetPara, "bnSetPara");
		((Control)bnSetPara).Name = "bnSetPara";
		((ButtonBase)bnSetPara).UseVisualStyleBackColor = true;
		((Control)bnSetPara).Click += bnSetPara_Click;
		componentResourceManager.ApplyResources(bnSaveRom, "bnSaveRom");
		((Control)bnSaveRom).Name = "bnSaveRom";
		((ButtonBase)bnSaveRom).UseVisualStyleBackColor = true;
		((Control)bnSaveRom).Click += bnSaveRom_Click;
		componentResourceManager.ApplyResources(bnUpdate, "bnUpdate");
		((Control)bnUpdate).Name = "bnUpdate";
		((ButtonBase)bnUpdate).UseVisualStyleBackColor = true;
		((Control)bnUpdate).Click += bnUpdate_Click;
		((Control)groupBox2).Controls.Add((Control)(object)label6);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst5);
		((Control)groupBox2).Controls.Add((Control)(object)label7);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst4);
		((Control)groupBox2).Controls.Add((Control)(object)label8);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst3);
		((Control)groupBox2).Controls.Add((Control)(object)label9);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst2);
		((Control)groupBox2).Controls.Add((Control)(object)label10);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst1);
		((Control)groupBox2).Controls.Add((Control)(object)label11);
		((Control)groupBox2).Controls.Add((Control)(object)tbEncOffst0);
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(tbEncOffst5, "tbEncOffst5");
		((Control)tbEncOffst5).Name = "tbEncOffst5";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(tbEncOffst4, "tbEncOffst4");
		((Control)tbEncOffst4).Name = "tbEncOffst4";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(tbEncOffst3, "tbEncOffst3");
		((Control)tbEncOffst3).Name = "tbEncOffst3";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(tbEncOffst2, "tbEncOffst2");
		((Control)tbEncOffst2).Name = "tbEncOffst2";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(tbEncOffst1, "tbEncOffst1");
		((Control)tbEncOffst1).Name = "tbEncOffst1";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(tbEncOffst0, "tbEncOffst0");
		((Control)tbEncOffst0).Name = "tbEncOffst0";
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)bnUpdate);
		((Control)this).Controls.Add((Control)(object)bnSaveRom);
		((Control)this).Controls.Add((Control)(object)bnSetPara);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Name = "frmAxisExPara";
		((Form)this).Load += frmAxisExPara_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
