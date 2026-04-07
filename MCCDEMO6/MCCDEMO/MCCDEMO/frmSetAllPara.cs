using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MCCDEMO;

public class frmSetAllPara : Form
{
	private MainFrm PMainFrm;

	private SetSysParaFrm frmSetSysParaFrm;

	private AbsEncParaFrm frmAbsEncFrm;

	private InputFunCfgFrm frmInputFunCfgFrm;

	private SysCtrlParaCfgFrm frmSysCtrlParaFrm;

	private frmAxisExPara frmAxisExParaFrm;

	private IContainer components;

	private TabControl tabControl1;

	private TabPage tbpAxisPara;

	private TabPage tbpAbsEncPara;

	private TabPage tbpInputFunPara;

	private TabPage tbpSysCtrlPara;

	private TabPage tbpAxisParaEx;

	public frmSetAllPara()
	{
		InitializeComponent();
	}

	private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (((Control)tabControl1.SelectedTab).Name == "tbpAxisPara")
		{
			((Control)this).Height = 320;
			((Control)this).Width = 1085;
		}
		else if (((Control)tabControl1.SelectedTab).Name == "tbpAbsEncPara")
		{
			((Control)this).Height = 650;
			((Control)this).Width = 450;
		}
		else if (((Control)tabControl1.SelectedTab).Name == "tbpInputFunPara")
		{
			((Control)this).Height = 420;
			((Control)this).Width = 530;
		}
		else if (((Control)tabControl1.SelectedTab).Name == "tbpAxisParaEx")
		{
			((Control)this).Height = 310;
			((Control)this).Width = 510;
		}
		else if (((Control)tabControl1.SelectedTab).Name == "tbpSysCtrlPara")
		{
			((Control)this).Height = 250;
			((Control)this).Width = 400;
		}
	}

	private void frmSetAllPara_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		frmSetSysParaFrm = new SetSysParaFrm(((Form)this).Owner);
		((Form)frmSetSysParaFrm).TopLevel = false;
		((Control)frmSetSysParaFrm).Parent = (Control)(object)tabControl1.TabPages["tbpAxisPara"];
		((Control)frmSetSysParaFrm).Dock = (DockStyle)5;
		((Control)tabControl1.TabPages["tbpAxisPara"]).Controls.Add((Control)(object)frmSetSysParaFrm);
		((Control)frmSetSysParaFrm).Show();
		((Control)frmSetSysParaFrm).BringToFront();
		((Control)this).Height = 320;
		((Control)this).Width = 1085;
		frmAbsEncFrm = new AbsEncParaFrm(((Form)this).Owner);
		((Form)frmAbsEncFrm).TopLevel = false;
		((Control)frmAbsEncFrm).Parent = (Control)(object)tabControl1.TabPages["tbpAbsEncPara"];
		((Control)frmAbsEncFrm).Dock = (DockStyle)5;
		((Control)tabControl1.TabPages["tbpAbsEncPara"]).Controls.Add((Control)(object)frmAbsEncFrm);
		((Control)frmAbsEncFrm).Show();
		((Control)frmAbsEncFrm).BringToFront();
		frmInputFunCfgFrm = new InputFunCfgFrm(((Form)this).Owner);
		((Form)frmInputFunCfgFrm).TopLevel = false;
		((Control)frmInputFunCfgFrm).Parent = (Control)(object)tabControl1.TabPages["tbpInputFunPara"];
		((Control)frmInputFunCfgFrm).Dock = (DockStyle)5;
		((Control)tabControl1.TabPages["tbpInputFunPara"]).Controls.Add((Control)(object)frmInputFunCfgFrm);
		((Control)frmInputFunCfgFrm).Show();
		((Control)frmInputFunCfgFrm).BringToFront();
		frmSysCtrlParaFrm = new SysCtrlParaCfgFrm(((Form)this).Owner);
		((Form)frmSysCtrlParaFrm).TopLevel = false;
		((Control)frmSysCtrlParaFrm).Parent = (Control)(object)tabControl1.TabPages["tbpSysCtrlPara"];
		((Control)frmSysCtrlParaFrm).Dock = (DockStyle)5;
		((Control)tabControl1.TabPages["tbpSysCtrlPara"]).Controls.Add((Control)(object)frmSysCtrlParaFrm);
		((Control)frmSysCtrlParaFrm).Show();
		((Control)frmSysCtrlParaFrm).BringToFront();
		frmAxisExParaFrm = new frmAxisExPara(((Form)this).Owner);
		((Form)frmAxisExParaFrm).TopLevel = false;
		((Control)frmAxisExParaFrm).Parent = (Control)(object)tabControl1.TabPages["tbpAxisParaEx"];
		((Control)frmAxisExParaFrm).Dock = (DockStyle)5;
		((Control)tabControl1.TabPages["tbpAxisParaEx"]).Controls.Add((Control)(object)frmAxisExParaFrm);
		((Control)frmAxisExParaFrm).Show();
		((Control)frmAxisExParaFrm).BringToFront();
	}

	private void frmSetAllPara_ResizeEnd(object sender, EventArgs e)
	{
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmSetAllPara));
		tabControl1 = new TabControl();
		tbpAxisPara = new TabPage();
		tbpAbsEncPara = new TabPage();
		tbpInputFunPara = new TabPage();
		tbpSysCtrlPara = new TabPage();
		tbpAxisParaEx = new TabPage();
		((Control)tabControl1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tabControl1, "tabControl1");
		((Control)tabControl1).Controls.Add((Control)(object)tbpAxisPara);
		((Control)tabControl1).Controls.Add((Control)(object)tbpAbsEncPara);
		((Control)tabControl1).Controls.Add((Control)(object)tbpInputFunPara);
		((Control)tabControl1).Controls.Add((Control)(object)tbpSysCtrlPara);
		((Control)tabControl1).Controls.Add((Control)(object)tbpAxisParaEx);
		((Control)tabControl1).Name = "tabControl1";
		tabControl1.SelectedIndex = 0;
		tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(tbpAxisPara, "tbpAxisPara");
		((Control)tbpAxisPara).Name = "tbpAxisPara";
		tbpAxisPara.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbpAbsEncPara, "tbpAbsEncPara");
		((Control)tbpAbsEncPara).Name = "tbpAbsEncPara";
		tbpAbsEncPara.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbpInputFunPara, "tbpInputFunPara");
		((Control)tbpInputFunPara).Name = "tbpInputFunPara";
		tbpInputFunPara.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbpSysCtrlPara, "tbpSysCtrlPara");
		((Control)tbpSysCtrlPara).Name = "tbpSysCtrlPara";
		tbpSysCtrlPara.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbpAxisParaEx, "tbpAxisParaEx");
		((Control)tbpAxisParaEx).Name = "tbpAxisParaEx";
		tbpAxisParaEx.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)tabControl1);
		((Control)this).Name = "frmSetAllPara";
		((Form)this).Load += frmSetAllPara_Load;
		((Form)this).ResizeEnd += frmSetAllPara_ResizeEnd;
		((Control)tabControl1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
