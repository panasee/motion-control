using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class SysCtrlParaCfgFrm : Form
{
	private MainFrm PMainFrm;

	private IContainer components;

	private Label label1;

	private Label label2;

	private Label label3;

	private ComboBox cbParaInit;

	private ComboBox cbHandWheelMode;

	private TextBox tbScanDelay;

	private Button bnCfgPara;

	private Button bnSaveToRom;

	private Button bnUpdate;

	public SysCtrlParaCfgFrm(object mOwner)
	{
		InitializeComponent();
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		PMainFrm = (MainFrm)mOwner;
	}

	private void fun_UpdateSysCtrlParaToFrm()
	{
		uint[] array = new uint[1];
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_ReadPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_RecoverDeafult, array))
		{
			((Control)cbParaInit).Text = GlobalData.GlobalLanguage.SysCtrlParaFrm_ReadSystemControlFailed;
		}
		else
		{
			((ListControl)cbParaInit).SelectedIndex = ((array[0] != 0) ? 1 : 0);
		}
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_ReadPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_HandWheelMode, array))
		{
			((Control)cbHandWheelMode).Text = GlobalData.GlobalLanguage.SysCtrlParaFrm_ReadSystemControlFailed;
		}
		else
		{
			((ListControl)cbHandWheelMode).SelectedIndex = ((array[0] != 0) ? 1 : 0);
		}
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_ReadPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_ScanDelay, array))
		{
			((Control)tbScanDelay).Text = GlobalData.GlobalLanguage.SysCtrlParaFrm_ReadSystemControlFailed;
		}
		else
		{
			((Control)tbScanDelay).Text = array[0].ToString();
		}
	}

	private void SysCtrlParaCfgFrm_Load(object sender, EventArgs e)
	{
		fun_UpdateSysCtrlParaToFrm();
	}

	private void bnUpdate_Click(object sender, EventArgs e)
	{
		fun_UpdateSysCtrlParaToFrm();
	}

	private void bnSaveToRom_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM())
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.SysCtrlParaFrm_SaveSystemControlParaFailed);
		}
	}

	private void bnCfgPara_Click(object sender, EventArgs e)
	{
		uint num = 0u;
		try
		{
			num = uint.Parse(((Control)tbScanDelay).Text);
			if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_ScanDelay, num))
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.SysCtrlParaFrm_SetLCDScanDelayParaFailed);
			}
			if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_RecoverDeafult, (uint)((ListControl)cbParaInit).SelectedIndex))
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.SysCtrlParaFrm_SetResetParameterFailed);
			}
			if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(PMainFrm.Mccard.ParaIDBase_SysCtrlPara, PMainFrm.Mccard.SysCtrlPara_HandWheelMode, (uint)((ListControl)cbHandWheelMode).SelectedIndex))
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.SysCtrlParaFrm_SetHandWheelFailed);
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.SysCtrlParaFrm_ParameterFormatError);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SysCtrlParaCfgFrm));
		label1 = new Label();
		label2 = new Label();
		label3 = new Label();
		cbParaInit = new ComboBox();
		cbHandWheelMode = new ComboBox();
		tbScanDelay = new TextBox();
		bnCfgPara = new Button();
		bnSaveToRom = new Button();
		bnUpdate = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(cbParaInit, "cbParaInit");
		((ListControl)cbParaInit).FormattingEnabled = true;
		cbParaInit.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbParaInit.Items"),
			componentResourceManager.GetString("cbParaInit.Items1")
		});
		((Control)cbParaInit).Name = "cbParaInit";
		componentResourceManager.ApplyResources(cbHandWheelMode, "cbHandWheelMode");
		((ListControl)cbHandWheelMode).FormattingEnabled = true;
		cbHandWheelMode.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHandWheelMode.Items"),
			componentResourceManager.GetString("cbHandWheelMode.Items1")
		});
		((Control)cbHandWheelMode).Name = "cbHandWheelMode";
		componentResourceManager.ApplyResources(tbScanDelay, "tbScanDelay");
		((Control)tbScanDelay).Name = "tbScanDelay";
		componentResourceManager.ApplyResources(bnCfgPara, "bnCfgPara");
		((Control)bnCfgPara).Name = "bnCfgPara";
		((ButtonBase)bnCfgPara).UseVisualStyleBackColor = true;
		((Control)bnCfgPara).Click += bnCfgPara_Click;
		componentResourceManager.ApplyResources(bnSaveToRom, "bnSaveToRom");
		((Control)bnSaveToRom).Name = "bnSaveToRom";
		((ButtonBase)bnSaveToRom).UseVisualStyleBackColor = true;
		((Control)bnSaveToRom).Click += bnSaveToRom_Click;
		componentResourceManager.ApplyResources(bnUpdate, "bnUpdate");
		((Control)bnUpdate).Name = "bnUpdate";
		((ButtonBase)bnUpdate).UseVisualStyleBackColor = true;
		((Control)bnUpdate).Click += bnUpdate_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)bnUpdate);
		((Control)this).Controls.Add((Control)(object)bnSaveToRom);
		((Control)this).Controls.Add((Control)(object)bnCfgPara);
		((Control)this).Controls.Add((Control)(object)tbScanDelay);
		((Control)this).Controls.Add((Control)(object)cbHandWheelMode);
		((Control)this).Controls.Add((Control)(object)cbParaInit);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label1);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "SysCtrlParaCfgFrm";
		((Form)this).Load += SysCtrlParaCfgFrm_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
