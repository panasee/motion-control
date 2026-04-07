using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MCC6DLL;

public class SpDataShow : Form
{
	private IContainer components = null;

	private FlowLayoutPanel flowLayoutPanel1;

	private TextBox tbSpData;

	public SpDataShow()
	{
		InitializeComponent();
	}

	public void mPrintStr(string mString)
	{
		if (((TextBoxBase)tbSpData).MaxLength - ((TextBoxBase)tbSpData).TextLength < mString.Length)
		{
			((Control)tbSpData).Text = "";
		}
		((Control)tbSpData).Text = ((Control)tbSpData).Text + mString;
		((TextBoxBase)tbSpData).Select(((Control)tbSpData).Text.Length, 0);
		((TextBoxBase)tbSpData).ScrollToCaret();
	}

	public void mPrintHex(byte mData)
	{
		((Control)tbSpData).Text = ((Control)tbSpData).Text + mData.ToString("X2") + " ";
		if (((TextBoxBase)tbSpData).TextLength >= ((TextBoxBase)tbSpData).MaxLength - 1)
		{
			((Control)tbSpData).Text = "";
		}
		((TextBoxBase)tbSpData).Select(((Control)tbSpData).Text.Length, 0);
		((TextBoxBase)tbSpData).ScrollToCaret();
	}

	public int mPrintHex(byte[] mData)
	{
		return mPrintHex(mData, (ushort)mData.Length);
	}

	public int mPrintHex(byte[] mData, ushort mLen)
	{
		uint num = 0u;
		if (((TextBoxBase)tbSpData).MaxLength - ((TextBoxBase)tbSpData).TextLength < mLen)
		{
			((Control)tbSpData).Text = "";
		}
		for (num = 0u; num < mLen; num++)
		{
			((Control)tbSpData).Text = ((Control)tbSpData).Text + mData[num].ToString("X2") + " ";
		}
		((Control)tbSpData).Text = ((Control)tbSpData).Text + "\r\n";
		((TextBoxBase)tbSpData).Select(((Control)tbSpData).Text.Length, 0);
		((TextBoxBase)tbSpData).ScrollToCaret();
		return mData.Length;
	}

	private void SpDataShow_FormClosing(object sender, FormClosingEventArgs e)
	{
		((CancelEventArgs)(object)e).Cancel = true;
	}

	private void tbSpData_DoubleClick(object sender, EventArgs e)
	{
		((TextBoxBase)tbSpData).Clear();
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		flowLayoutPanel1 = new FlowLayoutPanel();
		tbSpData = new TextBox();
		((Control)flowLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)flowLayoutPanel1).Controls.Add((Control)(object)tbSpData);
		((Control)flowLayoutPanel1).Location = new Point(2, 1);
		((Control)flowLayoutPanel1).Name = "flowLayoutPanel1";
		((Control)flowLayoutPanel1).Size = new Size(886, 275);
		((Control)flowLayoutPanel1).TabIndex = 1;
		((Control)tbSpData).Location = new Point(3, 3);
		((TextBoxBase)tbSpData).Multiline = true;
		((Control)tbSpData).Name = "tbSpData";
		tbSpData.ScrollBars = (ScrollBars)2;
		((Control)tbSpData).Size = new Size(883, 272);
		((Control)tbSpData).TabIndex = 1;
		((Control)tbSpData).DoubleClick += tbSpData_DoubleClick;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(887, 275);
		((Control)this).Controls.Add((Control)(object)flowLayoutPanel1);
		((Form)this).FormBorderStyle = (FormBorderStyle)1;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "SpDataShow";
		((Control)this).Text = "SpDataShow";
		((Control)flowLayoutPanel1).ResumeLayout(false);
		((Control)flowLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
