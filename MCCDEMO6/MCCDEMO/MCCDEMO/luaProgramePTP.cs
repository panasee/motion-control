using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MCCDEMO;

public class luaProgramePTP : Form
{
	private IContainer components;

	private Button bnPTPConfirm;

	private Button button2;

	private GroupBox groupBox1;

	private RadioButton rbPTPCmndMoveJB;

	private RadioButton rbPTPCmndMoveJR;

	private RadioButton rbPTPCmndMoveJ;

	private RadioButton rbPTPCmndMoveJBR;

	private RadioButton rbStopB;

	private RadioButton rbStop;

	private RadioButton rbEStop;

	private ComboBox cbAxisID;

	private Label label1;

	private TextBox tbPTPDist;

	private Label label2;

	private TextBox tbPTPAcc;

	private Label label4;

	private TextBox tbPTPVel;

	private Label label3;

	private RadioButton radioButton1;

	private RadioButton radioButton2;

	private RadioButton radioButton3;

	public luaProgramePTP()
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		bnPTPConfirm = new Button();
		button2 = new Button();
		groupBox1 = new GroupBox();
		rbPTPCmndMoveJ = new RadioButton();
		rbPTPCmndMoveJR = new RadioButton();
		rbPTPCmndMoveJB = new RadioButton();
		rbPTPCmndMoveJBR = new RadioButton();
		rbStop = new RadioButton();
		rbStopB = new RadioButton();
		rbEStop = new RadioButton();
		cbAxisID = new ComboBox();
		label1 = new Label();
		label2 = new Label();
		tbPTPDist = new TextBox();
		tbPTPVel = new TextBox();
		label3 = new Label();
		tbPTPAcc = new TextBox();
		label4 = new Label();
		radioButton1 = new RadioButton();
		radioButton2 = new RadioButton();
		radioButton3 = new RadioButton();
		((Control)groupBox1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)bnPTPConfirm).Location = new Point(210, 91);
		((Control)bnPTPConfirm).Name = "bnPTPConfirm";
		((Control)bnPTPConfirm).Size = new Size(63, 48);
		((Control)bnPTPConfirm).TabIndex = 0;
		((Control)bnPTPConfirm).Text = "确认";
		((ButtonBase)bnPTPConfirm).UseVisualStyleBackColor = true;
		((Control)button2).Location = new Point(196, 255);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(75, 23);
		((Control)button2).TabIndex = 1;
		((Control)button2).Text = "button2";
		((ButtonBase)button2).UseVisualStyleBackColor = true;
		((Control)groupBox1).Controls.Add((Control)(object)radioButton1);
		((Control)groupBox1).Controls.Add((Control)(object)radioButton2);
		((Control)groupBox1).Controls.Add((Control)(object)radioButton3);
		((Control)groupBox1).Controls.Add((Control)(object)tbPTPAcc);
		((Control)groupBox1).Controls.Add((Control)(object)label4);
		((Control)groupBox1).Controls.Add((Control)(object)bnPTPConfirm);
		((Control)groupBox1).Controls.Add((Control)(object)tbPTPVel);
		((Control)groupBox1).Controls.Add((Control)(object)label3);
		((Control)groupBox1).Controls.Add((Control)(object)tbPTPDist);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisID);
		((Control)groupBox1).Controls.Add((Control)(object)rbEStop);
		((Control)groupBox1).Controls.Add((Control)(object)rbStopB);
		((Control)groupBox1).Controls.Add((Control)(object)rbStop);
		((Control)groupBox1).Controls.Add((Control)(object)rbPTPCmndMoveJBR);
		((Control)groupBox1).Controls.Add((Control)(object)rbPTPCmndMoveJB);
		((Control)groupBox1).Controls.Add((Control)(object)rbPTPCmndMoveJR);
		((Control)groupBox1).Controls.Add((Control)(object)rbPTPCmndMoveJ);
		((Control)groupBox1).Dock = (DockStyle)1;
		((Control)groupBox1).Location = new Point(0, 0);
		((Control)groupBox1).Name = "groupBox1";
		((Control)groupBox1).Size = new Size(283, 249);
		((Control)groupBox1).TabIndex = 2;
		groupBox1.TabStop = false;
		((Control)groupBox1).Text = "单轴指令";
		((Control)rbPTPCmndMoveJ).AutoSize = true;
		((Control)rbPTPCmndMoveJ).Location = new Point(6, 20);
		((Control)rbPTPCmndMoveJ).Name = "rbPTPCmndMoveJ";
		((Control)rbPTPCmndMoveJ).Size = new Size(47, 16);
		((Control)rbPTPCmndMoveJ).TabIndex = 0;
		rbPTPCmndMoveJ.TabStop = true;
		((Control)rbPTPCmndMoveJ).Text = "MovJ";
		((ButtonBase)rbPTPCmndMoveJ).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJR).AutoSize = true;
		((Control)rbPTPCmndMoveJR).Location = new Point(70, 20);
		((Control)rbPTPCmndMoveJR).Name = "rbPTPCmndMoveJR";
		((Control)rbPTPCmndMoveJR).Size = new Size(53, 16);
		((Control)rbPTPCmndMoveJR).TabIndex = 1;
		rbPTPCmndMoveJR.TabStop = true;
		((Control)rbPTPCmndMoveJR).Text = "MovJR";
		((ButtonBase)rbPTPCmndMoveJR).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJB).AutoSize = true;
		((Control)rbPTPCmndMoveJB).Location = new Point(140, 20);
		((Control)rbPTPCmndMoveJB).Name = "rbPTPCmndMoveJB";
		((Control)rbPTPCmndMoveJB).Size = new Size(53, 16);
		((Control)rbPTPCmndMoveJB).TabIndex = 2;
		rbPTPCmndMoveJB.TabStop = true;
		((Control)rbPTPCmndMoveJB).Text = "MovJB";
		((ButtonBase)rbPTPCmndMoveJB).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJBR).AutoSize = true;
		((Control)rbPTPCmndMoveJBR).Location = new Point(210, 20);
		((Control)rbPTPCmndMoveJBR).Name = "rbPTPCmndMoveJBR";
		((Control)rbPTPCmndMoveJBR).Size = new Size(59, 16);
		((Control)rbPTPCmndMoveJBR).TabIndex = 3;
		rbPTPCmndMoveJBR.TabStop = true;
		((Control)rbPTPCmndMoveJBR).Text = "MovJRB";
		((ButtonBase)rbPTPCmndMoveJBR).UseVisualStyleBackColor = true;
		((Control)rbStop).AutoSize = true;
		((Control)rbStop).Location = new Point(6, 42);
		((Control)rbStop).Name = "rbStop";
		((Control)rbStop).Size = new Size(53, 16);
		((Control)rbStop).TabIndex = 4;
		rbStop.TabStop = true;
		((Control)rbStop).Text = "StopJ";
		((ButtonBase)rbStop).UseVisualStyleBackColor = true;
		((Control)rbStopB).AutoSize = true;
		((Control)rbStopB).Location = new Point(70, 42);
		((Control)rbStopB).Name = "rbStopB";
		((Control)rbStopB).Size = new Size(59, 16);
		((Control)rbStopB).TabIndex = 5;
		rbStopB.TabStop = true;
		((Control)rbStopB).Text = "StopJB";
		((ButtonBase)rbStopB).UseVisualStyleBackColor = true;
		((Control)rbEStop).AutoSize = true;
		((Control)rbEStop).Location = new Point(140, 42);
		((Control)rbEStop).Name = "rbEStop";
		((Control)rbEStop).Size = new Size(53, 16);
		((Control)rbEStop).TabIndex = 6;
		rbEStop.TabStop = true;
		((Control)rbEStop).Text = "EStop";
		((ButtonBase)rbEStop).UseVisualStyleBackColor = true;
		((ListControl)cbAxisID).FormattingEnabled = true;
		cbAxisID.Items.AddRange(new object[6] { "X", "Y", "Z", "A", "B", "C" });
		((Control)cbAxisID).Location = new Point(37, 91);
		((Control)cbAxisID).Name = "cbAxisID";
		((Control)cbAxisID).Size = new Size(53, 20);
		((Control)cbAxisID).TabIndex = 7;
		((Control)label1).AutoSize = true;
		((Control)label1).Location = new Point(14, 97);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(17, 12);
		((Control)label1).TabIndex = 8;
		((Control)label1).Text = "ID";
		((Control)label2).AutoSize = true;
		((Control)label2).Location = new Point(2, 121);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(29, 12);
		((Control)label2).TabIndex = 9;
		((Control)label2).Text = "距离";
		((Control)tbPTPDist).Location = new Point(37, 118);
		((Control)tbPTPDist).Name = "tbPTPDist";
		((Control)tbPTPDist).Size = new Size(53, 21);
		((Control)tbPTPDist).TabIndex = 10;
		((Control)tbPTPDist).Text = "10.0";
		((Control)tbPTPVel).Location = new Point(145, 91);
		((Control)tbPTPVel).Name = "tbPTPVel";
		((Control)tbPTPVel).Size = new Size(53, 21);
		((Control)tbPTPVel).TabIndex = 12;
		((Control)tbPTPVel).Text = "10.0";
		((Control)label3).AutoSize = true;
		((Control)label3).Location = new Point(110, 96);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(29, 12);
		((Control)label3).TabIndex = 11;
		((Control)label3).Text = "速度";
		((Control)tbPTPAcc).Location = new Point(145, 118);
		((Control)tbPTPAcc).Name = "tbPTPAcc";
		((Control)tbPTPAcc).Size = new Size(53, 21);
		((Control)tbPTPAcc).TabIndex = 14;
		((Control)tbPTPAcc).Text = "0.5";
		((Control)label4).AutoSize = true;
		((Control)label4).Location = new Point(98, 123);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(41, 12);
		((Control)label4).TabIndex = 13;
		((Control)label4).Text = "加速度";
		((Control)radioButton1).AutoSize = true;
		((Control)radioButton1).Location = new Point(140, 64);
		((Control)radioButton1).Name = "radioButton1";
		((Control)radioButton1).Size = new Size(47, 16);
		((Control)radioButton1).TabIndex = 17;
		radioButton1.TabStop = true;
		((Control)radioButton1).Text = "QHom";
		((ButtonBase)radioButton1).UseVisualStyleBackColor = true;
		((Control)radioButton2).AutoSize = true;
		((Control)radioButton2).Location = new Point(70, 64);
		((Control)radioButton2).Name = "radioButton2";
		((Control)radioButton2).Size = new Size(47, 16);
		((Control)radioButton2).TabIndex = 16;
		radioButton2.TabStop = true;
		((Control)radioButton2).Text = "HomB";
		((ButtonBase)radioButton2).UseVisualStyleBackColor = true;
		((Control)radioButton3).AutoSize = true;
		((Control)radioButton3).Location = new Point(6, 64);
		((Control)radioButton3).Name = "radioButton3";
		((Control)radioButton3).Size = new Size(41, 16);
		((Control)radioButton3).TabIndex = 15;
		radioButton3.TabStop = true;
		((Control)radioButton3).Text = "Hom";
		((ButtonBase)radioButton3).UseVisualStyleBackColor = true;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(283, 333);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Controls.Add((Control)(object)button2);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).Name = "luaProgramePTP";
		((Control)this).Text = "luaProgramePTP";
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
