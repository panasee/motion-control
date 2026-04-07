using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MCCDEMO;

public class ManCtrlEx : Form
{
	private IContainer components;

	private GroupBox groupBox1;

	private RadioButton rbA;

	private RadioButton rbZ;

	private RadioButton rbY;

	private RadioButton rbX;

	private GroupBox groupBox2;

	private RadioButton rbMoveRel;

	private RadioButton rbMoveAbs;

	private Label lbDist;

	private Label label1;

	private Label label2;

	private TextBox tbDist;

	private TextBox tbVel;

	private TextBox tbAcc;

	private Button bnMove;

	private Button bnParaStop;

	private Button bnEmergencyStop;

	private Button bnClear;

	private MainFrm PMainFrm;

	private Button bnChageVel;

	private RadioButton rbC;

	private RadioButton rbB;

	public ManCtrlEx()
	{
		InitializeComponent();
	}

	private void bnMove_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbX.Checked ? 1 : 0);
		b = (byte)(rbY.Checked ? 2 : b);
		b = (byte)(rbZ.Checked ? 3 : b);
		b = (byte)(rbA.Checked ? 4 : b);
		b = (byte)(rbB.Checked ? 5 : b);
		b = (byte)(rbC.Checked ? 6 : b);
		b--;
		if (rbMoveAbs.Checked)
		{
			PMainFrm.Mccard.MoCtrCard_MCrlAxisAbsMove(b, float.Parse(((Control)tbDist).Text), float.Parse(((Control)tbVel).Text), float.Parse(((Control)tbAcc).Text));
		}
		else
		{
			PMainFrm.Mccard.MoCtrCard_MCrlAxisRelMove(b, float.Parse(((Control)tbDist).Text), float.Parse(((Control)tbVel).Text), float.Parse(((Control)tbAcc).Text));
		}
	}

	private void ManCtrlEx_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		rbX.Checked = true;
		rbY.Checked = false;
		rbZ.Checked = false;
		rbA.Checked = false;
		rbB.Checked = false;
		rbC.Checked = false;
		rbMoveAbs.Checked = true;
		rbMoveRel.Checked = false;
		((Control)tbAcc).Text = "0.1";
		((Control)tbVel).Text = "10.0";
		((Control)tbDist).Text = "20.0";
	}

	private void bnClear_Click(object sender, EventArgs e)
	{
		rbX.Checked = true;
		rbY.Checked = false;
		rbZ.Checked = false;
		rbA.Checked = false;
		rbB.Checked = false;
		rbC.Checked = false;
		rbMoveAbs.Checked = true;
		rbMoveRel.Checked = false;
		((Control)tbAcc).Text = "0.0";
		((Control)tbVel).Text = "0.0";
		((Control)tbDist).Text = "0.0";
	}

	private void bnChageVel_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbX.Checked ? 1 : 0);
		b = (byte)(rbY.Checked ? 2 : b);
		b = (byte)(rbZ.Checked ? 3 : b);
		b = (byte)(rbA.Checked ? 4 : b);
		b = (byte)(rbB.Checked ? 5 : b);
		b = (byte)(rbC.Checked ? 6 : b);
		b--;
		PMainFrm.Mccard.MoCtrCard_ChangeMotionPara(b, float.Parse(((Control)tbVel).Text), float.Parse(((Control)tbAcc).Text));
	}

	private void bnParaStop_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbX.Checked ? 1 : 0);
		b = (byte)(rbY.Checked ? 2 : b);
		b = (byte)(rbZ.Checked ? 3 : b);
		b = (byte)(rbA.Checked ? 4 : b);
		b = (byte)(rbB.Checked ? 5 : b);
		b = (byte)(rbC.Checked ? 6 : b);
		b--;
		PMainFrm.Mccard.MoCtrCard_ParaStopAxisMov(b, float.Parse(((Control)tbAcc).Text));
	}

	private void bnEmergencyStop_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbX.Checked ? 1 : 0);
		b = (byte)(rbY.Checked ? 2 : b);
		b = (byte)(rbZ.Checked ? 3 : b);
		b = (byte)(rbA.Checked ? 4 : b);
		b = (byte)(rbB.Checked ? 5 : b);
		b = (byte)(rbC.Checked ? 6 : b);
		b--;
		PMainFrm.Mccard.MoCtrCard_EmergencyStopAxisMov(b);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ManCtrlEx));
		groupBox1 = new GroupBox();
		rbC = new RadioButton();
		rbB = new RadioButton();
		rbA = new RadioButton();
		rbZ = new RadioButton();
		rbY = new RadioButton();
		rbX = new RadioButton();
		groupBox2 = new GroupBox();
		rbMoveRel = new RadioButton();
		rbMoveAbs = new RadioButton();
		lbDist = new Label();
		label1 = new Label();
		label2 = new Label();
		tbDist = new TextBox();
		tbVel = new TextBox();
		tbAcc = new TextBox();
		bnMove = new Button();
		bnParaStop = new Button();
		bnEmergencyStop = new Button();
		bnClear = new Button();
		bnChageVel = new Button();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)rbC);
		((Control)groupBox1).Controls.Add((Control)(object)rbB);
		((Control)groupBox1).Controls.Add((Control)(object)rbA);
		((Control)groupBox1).Controls.Add((Control)(object)rbZ);
		((Control)groupBox1).Controls.Add((Control)(object)rbY);
		((Control)groupBox1).Controls.Add((Control)(object)rbX);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(rbC, "rbC");
		((Control)rbC).Name = "rbC";
		rbC.TabStop = true;
		((ButtonBase)rbC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbB, "rbB");
		((Control)rbB).Name = "rbB";
		rbB.TabStop = true;
		((ButtonBase)rbB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbA, "rbA");
		((Control)rbA).Name = "rbA";
		rbA.TabStop = true;
		((ButtonBase)rbA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbZ, "rbZ");
		((Control)rbZ).Name = "rbZ";
		rbZ.TabStop = true;
		((ButtonBase)rbZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbY, "rbY");
		((Control)rbY).Name = "rbY";
		rbY.TabStop = true;
		((ButtonBase)rbY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbX, "rbX");
		((Control)rbX).Name = "rbX";
		rbX.TabStop = true;
		((ButtonBase)rbX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)rbMoveRel);
		((Control)groupBox2).Controls.Add((Control)(object)rbMoveAbs);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(rbMoveRel, "rbMoveRel");
		((Control)rbMoveRel).Name = "rbMoveRel";
		rbMoveRel.TabStop = true;
		((ButtonBase)rbMoveRel).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMoveAbs, "rbMoveAbs");
		((Control)rbMoveAbs).Name = "rbMoveAbs";
		rbMoveAbs.TabStop = true;
		((ButtonBase)rbMoveAbs).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(lbDist, "lbDist");
		((Control)lbDist).Name = "lbDist";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(tbDist, "tbDist");
		((Control)tbDist).Name = "tbDist";
		componentResourceManager.ApplyResources(tbVel, "tbVel");
		((Control)tbVel).Name = "tbVel";
		componentResourceManager.ApplyResources(tbAcc, "tbAcc");
		((Control)tbAcc).Name = "tbAcc";
		componentResourceManager.ApplyResources(bnMove, "bnMove");
		((Control)bnMove).Name = "bnMove";
		((ButtonBase)bnMove).UseVisualStyleBackColor = true;
		((Control)bnMove).Click += bnMove_Click;
		componentResourceManager.ApplyResources(bnParaStop, "bnParaStop");
		((Control)bnParaStop).Name = "bnParaStop";
		((ButtonBase)bnParaStop).UseVisualStyleBackColor = true;
		((Control)bnParaStop).Click += bnParaStop_Click;
		componentResourceManager.ApplyResources(bnEmergencyStop, "bnEmergencyStop");
		((Control)bnEmergencyStop).Name = "bnEmergencyStop";
		((ButtonBase)bnEmergencyStop).UseVisualStyleBackColor = true;
		((Control)bnEmergencyStop).Click += bnEmergencyStop_Click;
		componentResourceManager.ApplyResources(bnClear, "bnClear");
		((Control)bnClear).Name = "bnClear";
		((ButtonBase)bnClear).UseVisualStyleBackColor = true;
		((Control)bnClear).Click += bnClear_Click;
		componentResourceManager.ApplyResources(bnChageVel, "bnChageVel");
		((Control)bnChageVel).Name = "bnChageVel";
		((ButtonBase)bnChageVel).UseVisualStyleBackColor = true;
		((Control)bnChageVel).Click += bnChageVel_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)bnChageVel);
		((Control)this).Controls.Add((Control)(object)bnClear);
		((Control)this).Controls.Add((Control)(object)bnEmergencyStop);
		((Control)this).Controls.Add((Control)(object)bnParaStop);
		((Control)this).Controls.Add((Control)(object)bnMove);
		((Control)this).Controls.Add((Control)(object)tbAcc);
		((Control)this).Controls.Add((Control)(object)tbVel);
		((Control)this).Controls.Add((Control)(object)tbDist);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)lbDist);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "ManCtrlEx";
		((Form)this).Load += ManCtrlEx_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
