using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MCCDEMO;

public class LuaCtrlSubFrmEditFrm : Form
{
	private IContainer components;

	private TextBox tbLuaEditItem;

	public LuaCtrlSubFrmEditFrm()
	{
		InitializeComponent();
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		tbLuaEditItem = new TextBox();
		((Control)this).SuspendLayout();
		((Control)tbLuaEditItem).Location = new Point(12, 12);
		((TextBoxBase)tbLuaEditItem).Multiline = true;
		((Control)tbLuaEditItem).Name = "tbLuaEditItem";
		((Control)tbLuaEditItem).Size = new Size(540, 126);
		((Control)tbLuaEditItem).TabIndex = 0;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(566, 150);
		((Control)this).Controls.Add((Control)(object)tbLuaEditItem);
		((Control)this).Name = "LuaCtrlSubFrmEditFrm";
		((Control)this).Text = "LuaCtrlSubFrmEditFrm";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
