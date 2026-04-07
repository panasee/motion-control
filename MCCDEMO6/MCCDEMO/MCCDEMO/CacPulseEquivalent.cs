using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class CacPulseEquivalent : Form
{
	private IContainer components;

	private Button bnCac;

	private Label lbPitch;

	private TextBox tbPitch;

	private TextBox tbRatio;

	private Label label1;

	private TextBox tbPulsePerCycle;

	private Label label2;

	private SetSysParaFrm PSetParaFrm;

	public CacPulseEquivalent()
	{
		InitializeComponent();
	}

	private void CacPulseEquivalent_Load(object sender, EventArgs e)
	{
		PSetParaFrm = (SetSysParaFrm)(object)((Form)this).Owner;
		PSetParaFrm.fsFVal = 1f;
	}

	private void bnCac_Click(object sender, EventArgs e)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		float num = float.Parse(((Control)tbPitch).Text);
		float num2 = float.Parse(((Control)tbPulsePerCycle).Text);
		float num3 = float.Parse(((Control)tbRatio).Text);
		if (num > 0f && num2 > 0f && num3 > 0f)
		{
			float fsFVal = num2 * num3 / num;
			PSetParaFrm.fsFVal = fsFVal;
			((Form)this).Close();
		}
		else
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CacPulseEquivalent));
		bnCac = new Button();
		lbPitch = new Label();
		tbPitch = new TextBox();
		tbRatio = new TextBox();
		label1 = new Label();
		tbPulsePerCycle = new TextBox();
		label2 = new Label();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(bnCac, "bnCac");
		((Control)bnCac).Name = "bnCac";
		((ButtonBase)bnCac).UseVisualStyleBackColor = true;
		((Control)bnCac).Click += bnCac_Click;
		componentResourceManager.ApplyResources(lbPitch, "lbPitch");
		((Control)lbPitch).Name = "lbPitch";
		componentResourceManager.ApplyResources(tbPitch, "tbPitch");
		((Control)tbPitch).Name = "tbPitch";
		componentResourceManager.ApplyResources(tbRatio, "tbRatio");
		((Control)tbRatio).Name = "tbRatio";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbPulsePerCycle, "tbPulsePerCycle");
		((Control)tbPulsePerCycle).Name = "tbPulsePerCycle";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)tbPulsePerCycle);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)tbRatio);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)tbPitch);
		((Control)this).Controls.Add((Control)(object)lbPitch);
		((Control)this).Controls.Add((Control)(object)bnCac);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "CacPulseEquivalent";
		((Form)this).Load += CacPulseEquivalent_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
