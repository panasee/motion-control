using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class SignleLevCac : Form
{
	private MainFrm pMainFrm;

	private IContainer components;

	private GroupBox gbAxisChoose;

	private RadioButton rbAxisC;

	private RadioButton rbAxisB;

	private RadioButton rbAxisA;

	private RadioButton rbAxisZ;

	private RadioButton rbAxisY;

	private RadioButton rbAxisX;

	private GroupBox gbFunction;

	private CheckBox cbHardLimtEnable;

	private CheckBox cbSoftLimtEnable;

	private CheckBox cbComTable;

	private CheckBox cbHomeMode;

	private CheckBox cbPosLimitMode;

	private CheckBox cbNegLimitMode;

	private CheckBox cbComp;

	private Label label2;

	private TextBox tbParaVal;

	private Label label1;

	private ComboBox cbbHomeMode;

	public SignleLevCac()
	{
		InitializeComponent();
	}

	private void fun_UpdateFormAccrodingToPara(int nPara)
	{
		if (0 < (pMainFrm.Mccard.BitMask_LimitNeg & nPara))
		{
			cbNegLimitMode.Checked = true;
			((Control)cbNegLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_NegLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
		}
		else
		{
			cbNegLimitMode.Checked = false;
			((Control)cbNegLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_NegLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (0 < (pMainFrm.Mccard.BitMask_LimitPos & nPara))
		{
			cbPosLimitMode.Checked = true;
			((Control)cbPosLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_PosLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
		}
		else
		{
			cbPosLimitMode.Checked = false;
			((Control)cbPosLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_PosLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (0 < (pMainFrm.Mccard.BitMask_HomeSig & nPara))
		{
			cbHomeMode.Checked = true;
			((Control)cbHomeMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_Original + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
		}
		else
		{
			cbHomeMode.Checked = false;
			((Control)cbHomeMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_Original + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (0 < (pMainFrm.Mccard.BitMask_ComTable & nPara))
		{
			cbComTable.Checked = true;
			((Control)cbComTable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
		}
		else
		{
			cbComTable.Checked = false;
			((Control)cbComTable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (0 < (pMainFrm.Mccard.BitMask_SoftLimit & nPara))
		{
			cbSoftLimtEnable.Checked = true;
			((Control)cbSoftLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_SoftLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
		}
		else
		{
			cbSoftLimtEnable.Checked = false;
			((Control)cbSoftLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_SoftLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (0 < (pMainFrm.Mccard.BitMask_HardLimit & nPara))
		{
			cbHardLimtEnable.Checked = true;
			((Control)cbHardLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_HardLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
		}
		else
		{
			cbHardLimtEnable.Checked = false;
			((Control)cbHardLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_HardLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (0 < (pMainFrm.Mccard.BitMask_Com & nPara))
		{
			cbComp.Checked = true;
			((Control)cbComp).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
		}
		else
		{
			cbComp.Checked = false;
			((Control)cbComp).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		int selectedIndex = pMainFrm.Mccard.BitMask_HomeMode & nPara;
		((ListControl)cbbHomeMode).SelectedIndex = selectedIndex;
	}

	private void fun_CacParaFromForm()
	{
		int num = 0;
		if (cbNegLimitMode.Checked)
		{
			((Control)cbNegLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_NegLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
			num += pMainFrm.Mccard.BitMask_LimitNeg;
		}
		else
		{
			((Control)cbNegLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_NegLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (cbPosLimitMode.Checked)
		{
			((Control)cbPosLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_PosLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
			num += pMainFrm.Mccard.BitMask_LimitPos;
		}
		else
		{
			((Control)cbPosLimitMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_PosLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (cbHomeMode.Checked)
		{
			((Control)cbHomeMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_Original + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateClose;
			num += pMainFrm.Mccard.BitMask_HomeSig;
		}
		else
		{
			((Control)cbHomeMode).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_Original + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateOpen;
		}
		if (cbComTable.Checked)
		{
			((Control)cbComTable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
			num += pMainFrm.Mccard.BitMask_ComTable;
		}
		else
		{
			((Control)cbComTable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (cbSoftLimtEnable.Checked)
		{
			((Control)cbSoftLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_SoftLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
			num += pMainFrm.Mccard.BitMask_SoftLimit;
		}
		else
		{
			((Control)cbSoftLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_SoftLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (cbHardLimtEnable.Checked)
		{
			((Control)cbHardLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_HardLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
			num += pMainFrm.Mccard.BitMask_HardLimit;
		}
		else
		{
			((Control)cbHardLimtEnable).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_HardLimit + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (cbComp.Checked)
		{
			((Control)cbComp).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateEnable;
			num += pMainFrm.Mccard.BitMask_Com;
		}
		else
		{
			((Control)cbComp).Text = GlobalData.GlobalLanguage.CacSinglLevFrm_ComTable + " " + GlobalData.GlobalLanguage.CacSinglLevFrm_StateDisable;
		}
		if (((ListControl)cbbHomeMode).SelectedIndex == 0)
		{
			num = num;
		}
		else if (1 == ((ListControl)cbbHomeMode).SelectedIndex)
		{
			num += 256;
		}
		else if (2 == ((ListControl)cbbHomeMode).SelectedIndex)
		{
			num += 512;
		}
		((Control)tbParaVal).Text = num.ToString();
	}

	private void rbAxisX_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		RadioButton val = (RadioButton)sender;
		int num = 255;
		num = ((!(((Control)rbAxisX).Name == ((Control)val).Name)) ? ((((Control)rbAxisY).Name == ((Control)val).Name) ? 1 : ((((Control)rbAxisZ).Name == ((Control)val).Name) ? 2 : ((((Control)rbAxisA).Name == ((Control)val).Name) ? 3 : ((((Control)rbAxisB).Name == ((Control)val).Name) ? 4 : ((!(((Control)rbAxisC).Name == ((Control)val).Name)) ? 255 : 5))))) : 0);
		if (255 != num)
		{
			((Control)tbParaVal).Text = ((int)pMainFrm.SysPara[num]).ToString();
			fun_UpdateFormAccrodingToPara((int)pMainFrm.SysPara[num]);
		}
	}

	private void SignleLevCac_Load(object sender, EventArgs e)
	{
		pMainFrm = (MainFrm)(object)((Form)this).Owner;
		fun_UpdateFormAccrodingToPara(0);
	}

	private void cbNegLimitMode_CheckedChanged(object sender, EventArgs e)
	{
		fun_CacParaFromForm();
	}

	private void cbbHomeMode_SelectedIndexChanged(object sender, EventArgs e)
	{
		fun_CacParaFromForm();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SignleLevCac));
		gbAxisChoose = new GroupBox();
		rbAxisC = new RadioButton();
		rbAxisB = new RadioButton();
		rbAxisA = new RadioButton();
		rbAxisZ = new RadioButton();
		rbAxisY = new RadioButton();
		rbAxisX = new RadioButton();
		gbFunction = new GroupBox();
		label2 = new Label();
		tbParaVal = new TextBox();
		label1 = new Label();
		cbbHomeMode = new ComboBox();
		cbHardLimtEnable = new CheckBox();
		cbSoftLimtEnable = new CheckBox();
		cbComTable = new CheckBox();
		cbHomeMode = new CheckBox();
		cbPosLimitMode = new CheckBox();
		cbNegLimitMode = new CheckBox();
		cbComp = new CheckBox();
		((Control)gbAxisChoose).SuspendLayout();
		((Control)gbFunction).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(gbAxisChoose, "gbAxisChoose");
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisC);
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisB);
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisA);
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisZ);
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisY);
		((Control)gbAxisChoose).Controls.Add((Control)(object)rbAxisX);
		((Control)gbAxisChoose).Name = "gbAxisChoose";
		gbAxisChoose.TabStop = false;
		componentResourceManager.ApplyResources(rbAxisC, "rbAxisC");
		((Control)rbAxisC).Name = "rbAxisC";
		rbAxisC.TabStop = true;
		((ButtonBase)rbAxisC).UseVisualStyleBackColor = true;
		((Control)rbAxisC).Click += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(rbAxisB, "rbAxisB");
		((Control)rbAxisB).Name = "rbAxisB";
		rbAxisB.TabStop = true;
		((ButtonBase)rbAxisB).UseVisualStyleBackColor = true;
		((Control)rbAxisB).Click += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(rbAxisA, "rbAxisA");
		((Control)rbAxisA).Name = "rbAxisA";
		rbAxisA.TabStop = true;
		((ButtonBase)rbAxisA).UseVisualStyleBackColor = true;
		((Control)rbAxisA).Click += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(rbAxisZ, "rbAxisZ");
		((Control)rbAxisZ).Name = "rbAxisZ";
		rbAxisZ.TabStop = true;
		((ButtonBase)rbAxisZ).UseVisualStyleBackColor = true;
		((Control)rbAxisZ).Click += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(rbAxisY, "rbAxisY");
		((Control)rbAxisY).Name = "rbAxisY";
		rbAxisY.TabStop = true;
		((ButtonBase)rbAxisY).UseVisualStyleBackColor = true;
		((Control)rbAxisY).Click += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(rbAxisX, "rbAxisX");
		((Control)rbAxisX).Name = "rbAxisX";
		rbAxisX.TabStop = true;
		((ButtonBase)rbAxisX).UseVisualStyleBackColor = true;
		rbAxisX.CheckedChanged += rbAxisX_CheckedChanged;
		componentResourceManager.ApplyResources(gbFunction, "gbFunction");
		((Control)gbFunction).Controls.Add((Control)(object)label2);
		((Control)gbFunction).Controls.Add((Control)(object)tbParaVal);
		((Control)gbFunction).Controls.Add((Control)(object)label1);
		((Control)gbFunction).Controls.Add((Control)(object)cbbHomeMode);
		((Control)gbFunction).Controls.Add((Control)(object)cbHardLimtEnable);
		((Control)gbFunction).Controls.Add((Control)(object)cbSoftLimtEnable);
		((Control)gbFunction).Controls.Add((Control)(object)cbComTable);
		((Control)gbFunction).Controls.Add((Control)(object)cbHomeMode);
		((Control)gbFunction).Controls.Add((Control)(object)cbPosLimitMode);
		((Control)gbFunction).Controls.Add((Control)(object)cbNegLimitMode);
		((Control)gbFunction).Controls.Add((Control)(object)cbComp);
		((Control)gbFunction).Name = "gbFunction";
		gbFunction.TabStop = false;
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(tbParaVal, "tbParaVal");
		((Control)tbParaVal).Name = "tbParaVal";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(cbbHomeMode, "cbbHomeMode");
		((ListControl)cbbHomeMode).FormattingEnabled = true;
		cbbHomeMode.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbbHomeMode.Items"),
			componentResourceManager.GetString("cbbHomeMode.Items1"),
			componentResourceManager.GetString("cbbHomeMode.Items2")
		});
		((Control)cbbHomeMode).Name = "cbbHomeMode";
		cbbHomeMode.SelectedIndexChanged += cbbHomeMode_SelectedIndexChanged;
		componentResourceManager.ApplyResources(cbHardLimtEnable, "cbHardLimtEnable");
		((Control)cbHardLimtEnable).Name = "cbHardLimtEnable";
		((ButtonBase)cbHardLimtEnable).UseVisualStyleBackColor = true;
		cbHardLimtEnable.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbSoftLimtEnable, "cbSoftLimtEnable");
		((Control)cbSoftLimtEnable).Name = "cbSoftLimtEnable";
		((ButtonBase)cbSoftLimtEnable).UseVisualStyleBackColor = true;
		cbSoftLimtEnable.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbComTable, "cbComTable");
		((Control)cbComTable).Name = "cbComTable";
		((ButtonBase)cbComTable).UseVisualStyleBackColor = true;
		cbComTable.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbHomeMode, "cbHomeMode");
		((Control)cbHomeMode).Name = "cbHomeMode";
		((ButtonBase)cbHomeMode).UseVisualStyleBackColor = true;
		cbHomeMode.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbPosLimitMode, "cbPosLimitMode");
		((Control)cbPosLimitMode).Name = "cbPosLimitMode";
		((ButtonBase)cbPosLimitMode).UseVisualStyleBackColor = true;
		cbPosLimitMode.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbNegLimitMode, "cbNegLimitMode");
		((Control)cbNegLimitMode).Name = "cbNegLimitMode";
		((ButtonBase)cbNegLimitMode).UseVisualStyleBackColor = true;
		cbNegLimitMode.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(cbComp, "cbComp");
		((Control)cbComp).Name = "cbComp";
		((ButtonBase)cbComp).UseVisualStyleBackColor = true;
		cbComp.CheckedChanged += cbNegLimitMode_CheckedChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)gbFunction);
		((Control)this).Controls.Add((Control)(object)gbAxisChoose);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "SignleLevCac";
		((Form)this).Load += SignleLevCac_Load;
		((Control)gbAxisChoose).ResumeLayout(false);
		((Control)gbAxisChoose).PerformLayout();
		((Control)gbFunction).ResumeLayout(false);
		((Control)gbFunction).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
