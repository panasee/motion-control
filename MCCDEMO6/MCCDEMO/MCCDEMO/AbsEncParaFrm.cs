using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class AbsEncParaFrm : Form
{
	private MainFrm PMainFrm;

	private readonly ushort EncTypeInc;

	private readonly ushort EncTypeAbs0 = 2;

	private readonly ushort EncTypeAbs1 = 3;

	private IContainer components;

	private GroupBox groupBox1;

	private RadioButton rbEncXAbsChoose1;

	private RadioButton rbEncXAbsChoose0;

	private RadioButton rbEncXIncChoose;

	private GroupBox groupBox2;

	private RadioButton rbEncYAbsChoose1;

	private RadioButton rbEncYAbsChoose0;

	private RadioButton rbEncYIncChoose;

	private GroupBox groupBox3;

	private RadioButton rbEncZAbsChoose1;

	private RadioButton rbEncZAbsChoose0;

	private RadioButton rbEncZIncChoose;

	private GroupBox groupBox4;

	private RadioButton rbEncAAbsChoose1;

	private RadioButton rbEncAAbsChoose0;

	private RadioButton rbEncAIncChoose;

	private GroupBox groupBox5;

	private RadioButton rbEncBAbsChoose1;

	private RadioButton rbEncBAbsChoose0;

	private RadioButton rbEncBIncChoose;

	private GroupBox groupBox6;

	private RadioButton rbEncCAbsChoose1;

	private RadioButton rbEncCAbsChoose0;

	private RadioButton rbEncCIncChoose;

	private GroupBox groupBox7;

	private TextBox tbAbsEnc0TmOri;

	private Label label4;

	private TextBox tbAbsEnc0DataBit;

	private Label label3;

	private Button bnAbsEnc0CacTm;

	private TextBox tbAbsEnc0FramePeriod;

	private Label label2;

	private Button bnCacEnc0Scale;

	private TextBox tbAbsEnc0Scale;

	private TextBox tbAbsEnc0FClk;

	private Label label1;

	private GroupBox groupBox8;

	private TextBox tbAbsEnc1TmOri;

	private Label label5;

	private TextBox tbAbsEnc1DataBit;

	private Label label6;

	private Button bnAbsEnc1CacTm;

	private TextBox tbAbsEnc1FramePeriod;

	private Label label7;

	private Button bnCacEnc1Scale;

	private TextBox tbAbsEnc1Scale;

	private TextBox tbAbsEnc1FClk;

	private Label label8;

	private Button button5;

	private Button button6;

	private Button button7;

	private ComboBox AbsEnc0DataFormat;

	private Label label9;

	private ComboBox AbsEnc1DataFormat;

	private Label label10;

	public AbsEncParaFrm(object mOwner)
	{
		InitializeComponent();
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		PMainFrm = (MainFrm)mOwner;
	}

	private void AbsEncParaFrm_Load(object sender, EventArgs e)
	{
		ushort num = 2;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncXIncChoose.Checked = false;
			num = 1;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncXAbsChoose1.Checked = true;
				rbEncXAbsChoose0.Checked = false;
			}
			else
			{
				rbEncXAbsChoose1.Checked = false;
				rbEncXAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncXIncChoose.Checked = true;
			rbEncXAbsChoose0.Checked = false;
			rbEncXAbsChoose1.Checked = false;
		}
		num = 8;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncYIncChoose.Checked = false;
			num = 4;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncYAbsChoose1.Checked = true;
				rbEncYAbsChoose0.Checked = false;
			}
			else
			{
				rbEncYAbsChoose1.Checked = false;
				rbEncYAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncYIncChoose.Checked = true;
			rbEncYAbsChoose0.Checked = false;
			rbEncYAbsChoose1.Checked = false;
		}
		num = 32;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncZIncChoose.Checked = false;
			num = 16;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncZAbsChoose1.Checked = true;
				rbEncZAbsChoose0.Checked = false;
			}
			else
			{
				rbEncZAbsChoose1.Checked = false;
				rbEncZAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncZIncChoose.Checked = true;
			rbEncZAbsChoose0.Checked = false;
			rbEncZAbsChoose1.Checked = false;
		}
		num = 128;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncAIncChoose.Checked = false;
			num = 64;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncAAbsChoose1.Checked = true;
				rbEncAAbsChoose0.Checked = false;
			}
			else
			{
				rbEncAAbsChoose1.Checked = false;
				rbEncAAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncAIncChoose.Checked = true;
			rbEncAAbsChoose0.Checked = false;
			rbEncAAbsChoose1.Checked = false;
		}
		num = 256;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncBIncChoose.Checked = false;
			num = 128;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncBAbsChoose1.Checked = true;
				rbEncBAbsChoose0.Checked = false;
			}
			else
			{
				rbEncBAbsChoose1.Checked = false;
				rbEncBAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncBIncChoose.Checked = true;
			rbEncBAbsChoose0.Checked = false;
			rbEncBAbsChoose1.Checked = false;
		}
		num = 128;
		if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
		{
			rbEncCIncChoose.Checked = false;
			num = 64;
			if (num == (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] & num))
			{
				rbEncCAbsChoose1.Checked = true;
				rbEncCAbsChoose0.Checked = false;
			}
			else
			{
				rbEncCAbsChoose1.Checked = false;
				rbEncCAbsChoose0.Checked = true;
			}
		}
		else
		{
			rbEncCIncChoose.Checked = true;
			rbEncCAbsChoose0.Checked = false;
			rbEncCAbsChoose1.Checked = false;
		}
		((Control)tbAbsEnc0DataBit).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit0].ToString();
		if (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat0] < 2)
		{
			((ListControl)AbsEnc0DataFormat).SelectedIndex = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat0];
		}
		else
		{
			((Control)AbsEnc0DataFormat).Text = GlobalData.GlobalLanguage.AbsEncFrm_EncoderFormatError;
		}
		((Control)tbAbsEnc0Scale).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale0].ToString();
		float num2 = 0f;
		num2 = 1f / ((float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale0] * 0.1f * 2f) * 1000000f;
		((Control)tbAbsEnc0FClk).Text = num2.ToString("f1");
		float num3 = (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale0] * 0.1f * 2f * (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm0];
		((Control)tbAbsEnc0FramePeriod).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm0].ToString("f0");
		float num4 = (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale0] * 0.1f * 2f * (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit0];
		((Control)tbAbsEnc0TmOri).Text = (num3 - num4).ToString();
		((Control)tbAbsEnc1DataBit).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit1].ToString();
		if (PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat1] < 2)
		{
			((ListControl)AbsEnc1DataFormat).SelectedIndex = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat1];
		}
		else
		{
			((Control)AbsEnc1DataFormat).Text = GlobalData.GlobalLanguage.AbsEncFrm_EncoderFormatError;
		}
		((Control)tbAbsEnc1Scale).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale1].ToString();
		num2 = 1f / ((float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale1] * 0.1f * 2f) * 1000000f;
		((Control)tbAbsEnc1FClk).Text = num2.ToString("f1");
		num3 = (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale1] * 0.1f * 2f * (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm1];
		((Control)tbAbsEnc1FramePeriod).Text = PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm1].ToString("f0");
		num4 = (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale1] * 0.1f * 2f * (float)(int)PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit1];
		((Control)tbAbsEnc1TmOri).Text = (num3 - num4).ToString();
	}

	private void bnCacEnc0Scale_Click(object sender, EventArgs e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float num = 1f / float.Parse(((Control)tbAbsEnc0FClk).Text) * 1000000f / 0.1f / 2f;
			if (num > 250f)
			{
				MessageBox.Show(GlobalData.GlobalLanguage.AbsEncFrm_CheckFrequencyParamereUnitIsHz);
			}
			else
			{
				((Control)tbAbsEnc0Scale).Text = num.ToString("f0");
			}
		}
		catch (Exception)
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
		}
	}

	private void bnCacEnc1Scale_Click(object sender, EventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float num = 1f / float.Parse(((Control)tbAbsEnc1FClk).Text) * 1000000f / 0.1f / 2f;
			if (num > 250f)
			{
				MessageBox.Show(GlobalData.GlobalLanguage.AbsEncFrm_CheckFrequencyParamereUnitIsHz);
			}
			((Control)tbAbsEnc1Scale).Text = num.ToString("f0");
		}
		catch (Exception)
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
		}
	}

	private void bnAbsEnc0CacTm_Click(object sender, EventArgs e)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float num = 1f / float.Parse(((Control)tbAbsEnc0FClk).Text) * 1000000f;
			float num2 = num * float.Parse(((Control)tbAbsEnc0DataBit).Text);
			num2 += float.Parse(((Control)tbAbsEnc0TmOri).Text);
			num2 /= num;
			if (num2 > 250f)
			{
				MessageBox.Show(GlobalData.GlobalLanguage.AbsEncFrm_ValueTooLargeCheckInput);
			}
			((Control)tbAbsEnc0FramePeriod).Text = num2.ToString("f0");
		}
		catch
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
		}
	}

	private void bnAbsEnc1CacTm_Click(object sender, EventArgs e)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float num = 1f / float.Parse(((Control)tbAbsEnc1FClk).Text) * 1000000f;
			float num2 = num * float.Parse(((Control)tbAbsEnc1DataBit).Text);
			num2 += float.Parse(((Control)tbAbsEnc1TmOri).Text);
			num2 /= num;
			if (num2 > 250f)
			{
				MessageBox.Show(GlobalData.GlobalLanguage.AbsEncFrm_ValueTooLargeCheckInput);
			}
			((Control)tbAbsEnc1FramePeriod).Text = num2.ToString("f0");
		}
		catch
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
		}
	}

	private void button5_Click(object sender, EventArgs e)
	{
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ushort num = 0;
			int num2 = 0;
			num2 = 0;
			num = (rbEncXIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncXAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			num2 = 2;
			num = (rbEncYIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncYAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			num2 = 4;
			num = (rbEncZIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncZAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			num2 = 6;
			num = (rbEncAIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncAAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			num2 = 8;
			num = (rbEncBIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncBAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			num2 = 10;
			num = (rbEncCIncChoose.Checked ? ((ushort)(num | (ushort)(EncTypeInc << num2))) : ((!rbEncCAbsChoose1.Checked) ? ((ushort)(num | (ushort)(EncTypeAbs0 << num2))) : ((ushort)(num | (ushort)(EncTypeAbs1 << num2)))));
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_EncChoose, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_EncChoose] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc0DataBit).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_DataBit0, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit0] = num;
			}
			num = (ushort)((ListControl)AbsEnc0DataFormat).SelectedIndex;
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_DataFormat0, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat0] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc0Scale).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_ClkScale0, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale0] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc0FramePeriod).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_ClkTm0, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm0] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc1DataBit).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_DataBit1, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataBit1] = num;
			}
			num = (ushort)((ListControl)AbsEnc1DataFormat).SelectedIndex;
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_DataFormat1, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_DataFormat1] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc1Scale).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_ClkScale1, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkScale1] = num;
			}
			num = ushort.Parse(((Control)tbAbsEnc1FramePeriod).Text);
			if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_SendPara(10, (byte)PMainFrm.Mccard.AbsEncParaIndex_ClkTm1, num))
			{
				PMainFrm.AbsEncPara[PMainFrm.Mccard.AbsEncParaIndex_ClkTm1] = num;
			}
		}
		catch
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CacPulseDLFrm_DataFormatError);
		}
	}

	private void button7_Click(object sender, EventArgs e)
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AbsEncParaFrm));
		groupBox1 = new GroupBox();
		rbEncXAbsChoose1 = new RadioButton();
		rbEncXAbsChoose0 = new RadioButton();
		rbEncXIncChoose = new RadioButton();
		groupBox2 = new GroupBox();
		rbEncYAbsChoose1 = new RadioButton();
		rbEncYAbsChoose0 = new RadioButton();
		rbEncYIncChoose = new RadioButton();
		groupBox3 = new GroupBox();
		rbEncZAbsChoose1 = new RadioButton();
		rbEncZAbsChoose0 = new RadioButton();
		rbEncZIncChoose = new RadioButton();
		groupBox4 = new GroupBox();
		rbEncAAbsChoose1 = new RadioButton();
		rbEncAAbsChoose0 = new RadioButton();
		rbEncAIncChoose = new RadioButton();
		groupBox5 = new GroupBox();
		rbEncBAbsChoose1 = new RadioButton();
		rbEncBAbsChoose0 = new RadioButton();
		rbEncBIncChoose = new RadioButton();
		groupBox6 = new GroupBox();
		rbEncCAbsChoose1 = new RadioButton();
		rbEncCAbsChoose0 = new RadioButton();
		rbEncCIncChoose = new RadioButton();
		groupBox7 = new GroupBox();
		AbsEnc0DataFormat = new ComboBox();
		label9 = new Label();
		tbAbsEnc0TmOri = new TextBox();
		label4 = new Label();
		tbAbsEnc0DataBit = new TextBox();
		label3 = new Label();
		bnAbsEnc0CacTm = new Button();
		tbAbsEnc0FramePeriod = new TextBox();
		label2 = new Label();
		bnCacEnc0Scale = new Button();
		tbAbsEnc0Scale = new TextBox();
		tbAbsEnc0FClk = new TextBox();
		label1 = new Label();
		groupBox8 = new GroupBox();
		AbsEnc1DataFormat = new ComboBox();
		label10 = new Label();
		tbAbsEnc1TmOri = new TextBox();
		label5 = new Label();
		tbAbsEnc1DataBit = new TextBox();
		label6 = new Label();
		bnAbsEnc1CacTm = new Button();
		tbAbsEnc1FramePeriod = new TextBox();
		label7 = new Label();
		bnCacEnc1Scale = new Button();
		tbAbsEnc1Scale = new TextBox();
		tbAbsEnc1FClk = new TextBox();
		label8 = new Label();
		button5 = new Button();
		button6 = new Button();
		button7 = new Button();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)groupBox3).SuspendLayout();
		((Control)groupBox4).SuspendLayout();
		((Control)groupBox5).SuspendLayout();
		((Control)groupBox6).SuspendLayout();
		((Control)groupBox7).SuspendLayout();
		((Control)groupBox8).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)rbEncXAbsChoose1);
		((Control)groupBox1).Controls.Add((Control)(object)rbEncXAbsChoose0);
		((Control)groupBox1).Controls.Add((Control)(object)rbEncXIncChoose);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(rbEncXAbsChoose1, "rbEncXAbsChoose1");
		((Control)rbEncXAbsChoose1).Name = "rbEncXAbsChoose1";
		rbEncXAbsChoose1.TabStop = true;
		((ButtonBase)rbEncXAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncXAbsChoose0, "rbEncXAbsChoose0");
		((Control)rbEncXAbsChoose0).Name = "rbEncXAbsChoose0";
		rbEncXAbsChoose0.TabStop = true;
		((ButtonBase)rbEncXAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncXIncChoose, "rbEncXIncChoose");
		((Control)rbEncXIncChoose).Name = "rbEncXIncChoose";
		rbEncXIncChoose.TabStop = true;
		((ButtonBase)rbEncXIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)rbEncYAbsChoose1);
		((Control)groupBox2).Controls.Add((Control)(object)rbEncYAbsChoose0);
		((Control)groupBox2).Controls.Add((Control)(object)rbEncYIncChoose);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(rbEncYAbsChoose1, "rbEncYAbsChoose1");
		((Control)rbEncYAbsChoose1).Name = "rbEncYAbsChoose1";
		rbEncYAbsChoose1.TabStop = true;
		((ButtonBase)rbEncYAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncYAbsChoose0, "rbEncYAbsChoose0");
		((Control)rbEncYAbsChoose0).Name = "rbEncYAbsChoose0";
		rbEncYAbsChoose0.TabStop = true;
		((ButtonBase)rbEncYAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncYIncChoose, "rbEncYIncChoose");
		((Control)rbEncYIncChoose).Name = "rbEncYIncChoose";
		rbEncYIncChoose.TabStop = true;
		((ButtonBase)rbEncYIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox3, "groupBox3");
		((Control)groupBox3).Controls.Add((Control)(object)rbEncZAbsChoose1);
		((Control)groupBox3).Controls.Add((Control)(object)rbEncZAbsChoose0);
		((Control)groupBox3).Controls.Add((Control)(object)rbEncZIncChoose);
		((Control)groupBox3).Name = "groupBox3";
		groupBox3.TabStop = false;
		componentResourceManager.ApplyResources(rbEncZAbsChoose1, "rbEncZAbsChoose1");
		((Control)rbEncZAbsChoose1).Name = "rbEncZAbsChoose1";
		rbEncZAbsChoose1.TabStop = true;
		((ButtonBase)rbEncZAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncZAbsChoose0, "rbEncZAbsChoose0");
		((Control)rbEncZAbsChoose0).Name = "rbEncZAbsChoose0";
		rbEncZAbsChoose0.TabStop = true;
		((ButtonBase)rbEncZAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncZIncChoose, "rbEncZIncChoose");
		((Control)rbEncZIncChoose).Name = "rbEncZIncChoose";
		rbEncZIncChoose.TabStop = true;
		((ButtonBase)rbEncZIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox4, "groupBox4");
		((Control)groupBox4).Controls.Add((Control)(object)rbEncAAbsChoose1);
		((Control)groupBox4).Controls.Add((Control)(object)rbEncAAbsChoose0);
		((Control)groupBox4).Controls.Add((Control)(object)rbEncAIncChoose);
		((Control)groupBox4).Name = "groupBox4";
		groupBox4.TabStop = false;
		componentResourceManager.ApplyResources(rbEncAAbsChoose1, "rbEncAAbsChoose1");
		((Control)rbEncAAbsChoose1).Name = "rbEncAAbsChoose1";
		rbEncAAbsChoose1.TabStop = true;
		((ButtonBase)rbEncAAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncAAbsChoose0, "rbEncAAbsChoose0");
		((Control)rbEncAAbsChoose0).Name = "rbEncAAbsChoose0";
		rbEncAAbsChoose0.TabStop = true;
		((ButtonBase)rbEncAAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncAIncChoose, "rbEncAIncChoose");
		((Control)rbEncAIncChoose).Name = "rbEncAIncChoose";
		rbEncAIncChoose.TabStop = true;
		((ButtonBase)rbEncAIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox5, "groupBox5");
		((Control)groupBox5).Controls.Add((Control)(object)rbEncBAbsChoose1);
		((Control)groupBox5).Controls.Add((Control)(object)rbEncBAbsChoose0);
		((Control)groupBox5).Controls.Add((Control)(object)rbEncBIncChoose);
		((Control)groupBox5).Name = "groupBox5";
		groupBox5.TabStop = false;
		componentResourceManager.ApplyResources(rbEncBAbsChoose1, "rbEncBAbsChoose1");
		((Control)rbEncBAbsChoose1).Name = "rbEncBAbsChoose1";
		rbEncBAbsChoose1.TabStop = true;
		((ButtonBase)rbEncBAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncBAbsChoose0, "rbEncBAbsChoose0");
		((Control)rbEncBAbsChoose0).Name = "rbEncBAbsChoose0";
		rbEncBAbsChoose0.TabStop = true;
		((ButtonBase)rbEncBAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncBIncChoose, "rbEncBIncChoose");
		((Control)rbEncBIncChoose).Name = "rbEncBIncChoose";
		rbEncBIncChoose.TabStop = true;
		((ButtonBase)rbEncBIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox6, "groupBox6");
		((Control)groupBox6).Controls.Add((Control)(object)rbEncCAbsChoose1);
		((Control)groupBox6).Controls.Add((Control)(object)rbEncCAbsChoose0);
		((Control)groupBox6).Controls.Add((Control)(object)rbEncCIncChoose);
		((Control)groupBox6).Name = "groupBox6";
		groupBox6.TabStop = false;
		componentResourceManager.ApplyResources(rbEncCAbsChoose1, "rbEncCAbsChoose1");
		((Control)rbEncCAbsChoose1).Name = "rbEncCAbsChoose1";
		rbEncCAbsChoose1.TabStop = true;
		((ButtonBase)rbEncCAbsChoose1).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncCAbsChoose0, "rbEncCAbsChoose0");
		((Control)rbEncCAbsChoose0).Name = "rbEncCAbsChoose0";
		rbEncCAbsChoose0.TabStop = true;
		((ButtonBase)rbEncCAbsChoose0).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbEncCIncChoose, "rbEncCIncChoose");
		((Control)rbEncCIncChoose).Name = "rbEncCIncChoose";
		rbEncCIncChoose.TabStop = true;
		((ButtonBase)rbEncCIncChoose).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(groupBox7, "groupBox7");
		((Control)groupBox7).Controls.Add((Control)(object)AbsEnc0DataFormat);
		((Control)groupBox7).Controls.Add((Control)(object)label9);
		((Control)groupBox7).Controls.Add((Control)(object)tbAbsEnc0TmOri);
		((Control)groupBox7).Controls.Add((Control)(object)label4);
		((Control)groupBox7).Controls.Add((Control)(object)tbAbsEnc0DataBit);
		((Control)groupBox7).Controls.Add((Control)(object)label3);
		((Control)groupBox7).Controls.Add((Control)(object)bnAbsEnc0CacTm);
		((Control)groupBox7).Controls.Add((Control)(object)tbAbsEnc0FramePeriod);
		((Control)groupBox7).Controls.Add((Control)(object)label2);
		((Control)groupBox7).Controls.Add((Control)(object)bnCacEnc0Scale);
		((Control)groupBox7).Controls.Add((Control)(object)tbAbsEnc0Scale);
		((Control)groupBox7).Controls.Add((Control)(object)tbAbsEnc0FClk);
		((Control)groupBox7).Controls.Add((Control)(object)label1);
		((Control)groupBox7).Name = "groupBox7";
		groupBox7.TabStop = false;
		componentResourceManager.ApplyResources(AbsEnc0DataFormat, "AbsEnc0DataFormat");
		((ListControl)AbsEnc0DataFormat).FormattingEnabled = true;
		AbsEnc0DataFormat.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("AbsEnc0DataFormat.Items"),
			componentResourceManager.GetString("AbsEnc0DataFormat.Items1")
		});
		((Control)AbsEnc0DataFormat).Name = "AbsEnc0DataFormat";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(tbAbsEnc0TmOri, "tbAbsEnc0TmOri");
		((Control)tbAbsEnc0TmOri).Name = "tbAbsEnc0TmOri";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(tbAbsEnc0DataBit, "tbAbsEnc0DataBit");
		((Control)tbAbsEnc0DataBit).Name = "tbAbsEnc0DataBit";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(bnAbsEnc0CacTm, "bnAbsEnc0CacTm");
		((Control)bnAbsEnc0CacTm).Name = "bnAbsEnc0CacTm";
		((ButtonBase)bnAbsEnc0CacTm).UseVisualStyleBackColor = true;
		((Control)bnAbsEnc0CacTm).Click += bnAbsEnc0CacTm_Click;
		componentResourceManager.ApplyResources(tbAbsEnc0FramePeriod, "tbAbsEnc0FramePeriod");
		((Control)tbAbsEnc0FramePeriod).Name = "tbAbsEnc0FramePeriod";
		((TextBoxBase)tbAbsEnc0FramePeriod).ReadOnly = true;
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(bnCacEnc0Scale, "bnCacEnc0Scale");
		((Control)bnCacEnc0Scale).Name = "bnCacEnc0Scale";
		((ButtonBase)bnCacEnc0Scale).UseVisualStyleBackColor = true;
		((Control)bnCacEnc0Scale).Click += bnCacEnc0Scale_Click;
		componentResourceManager.ApplyResources(tbAbsEnc0Scale, "tbAbsEnc0Scale");
		((Control)tbAbsEnc0Scale).Name = "tbAbsEnc0Scale";
		((TextBoxBase)tbAbsEnc0Scale).ReadOnly = true;
		componentResourceManager.ApplyResources(tbAbsEnc0FClk, "tbAbsEnc0FClk");
		((Control)tbAbsEnc0FClk).Name = "tbAbsEnc0FClk";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(groupBox8, "groupBox8");
		((Control)groupBox8).Controls.Add((Control)(object)AbsEnc1DataFormat);
		((Control)groupBox8).Controls.Add((Control)(object)label10);
		((Control)groupBox8).Controls.Add((Control)(object)tbAbsEnc1TmOri);
		((Control)groupBox8).Controls.Add((Control)(object)label5);
		((Control)groupBox8).Controls.Add((Control)(object)tbAbsEnc1DataBit);
		((Control)groupBox8).Controls.Add((Control)(object)label6);
		((Control)groupBox8).Controls.Add((Control)(object)bnAbsEnc1CacTm);
		((Control)groupBox8).Controls.Add((Control)(object)tbAbsEnc1FramePeriod);
		((Control)groupBox8).Controls.Add((Control)(object)label7);
		((Control)groupBox8).Controls.Add((Control)(object)bnCacEnc1Scale);
		((Control)groupBox8).Controls.Add((Control)(object)tbAbsEnc1Scale);
		((Control)groupBox8).Controls.Add((Control)(object)tbAbsEnc1FClk);
		((Control)groupBox8).Controls.Add((Control)(object)label8);
		((Control)groupBox8).Name = "groupBox8";
		groupBox8.TabStop = false;
		componentResourceManager.ApplyResources(AbsEnc1DataFormat, "AbsEnc1DataFormat");
		((ListControl)AbsEnc1DataFormat).FormattingEnabled = true;
		AbsEnc1DataFormat.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("AbsEnc1DataFormat.Items"),
			componentResourceManager.GetString("AbsEnc1DataFormat.Items1")
		});
		((Control)AbsEnc1DataFormat).Name = "AbsEnc1DataFormat";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(tbAbsEnc1TmOri, "tbAbsEnc1TmOri");
		((Control)tbAbsEnc1TmOri).Name = "tbAbsEnc1TmOri";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(tbAbsEnc1DataBit, "tbAbsEnc1DataBit");
		((Control)tbAbsEnc1DataBit).Name = "tbAbsEnc1DataBit";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(bnAbsEnc1CacTm, "bnAbsEnc1CacTm");
		((Control)bnAbsEnc1CacTm).Name = "bnAbsEnc1CacTm";
		((ButtonBase)bnAbsEnc1CacTm).UseVisualStyleBackColor = true;
		((Control)bnAbsEnc1CacTm).Click += bnAbsEnc1CacTm_Click;
		componentResourceManager.ApplyResources(tbAbsEnc1FramePeriod, "tbAbsEnc1FramePeriod");
		((Control)tbAbsEnc1FramePeriod).Name = "tbAbsEnc1FramePeriod";
		((TextBoxBase)tbAbsEnc1FramePeriod).ReadOnly = true;
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(bnCacEnc1Scale, "bnCacEnc1Scale");
		((Control)bnCacEnc1Scale).Name = "bnCacEnc1Scale";
		((ButtonBase)bnCacEnc1Scale).UseVisualStyleBackColor = true;
		((Control)bnCacEnc1Scale).Click += bnCacEnc1Scale_Click;
		componentResourceManager.ApplyResources(tbAbsEnc1Scale, "tbAbsEnc1Scale");
		((Control)tbAbsEnc1Scale).Name = "tbAbsEnc1Scale";
		((TextBoxBase)tbAbsEnc1Scale).ReadOnly = true;
		componentResourceManager.ApplyResources(tbAbsEnc1FClk, "tbAbsEnc1FClk");
		((Control)tbAbsEnc1FClk).Name = "tbAbsEnc1FClk";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(button5, "button5");
		((Control)button5).Name = "button5";
		((ButtonBase)button5).UseVisualStyleBackColor = true;
		((Control)button5).Click += button5_Click;
		componentResourceManager.ApplyResources(button6, "button6");
		((Control)button6).Name = "button6";
		((ButtonBase)button6).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(button7, "button7");
		((Control)button7).Name = "button7";
		((ButtonBase)button7).UseVisualStyleBackColor = true;
		((Control)button7).Click += button7_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)button7);
		((Control)this).Controls.Add((Control)(object)button6);
		((Control)this).Controls.Add((Control)(object)button5);
		((Control)this).Controls.Add((Control)(object)groupBox8);
		((Control)this).Controls.Add((Control)(object)groupBox7);
		((Control)this).Controls.Add((Control)(object)groupBox6);
		((Control)this).Controls.Add((Control)(object)groupBox5);
		((Control)this).Controls.Add((Control)(object)groupBox4);
		((Control)this).Controls.Add((Control)(object)groupBox3);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "AbsEncParaFrm";
		((Form)this).Load += AbsEncParaFrm_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)groupBox3).ResumeLayout(false);
		((Control)groupBox3).PerformLayout();
		((Control)groupBox4).ResumeLayout(false);
		((Control)groupBox4).PerformLayout();
		((Control)groupBox5).ResumeLayout(false);
		((Control)groupBox5).PerformLayout();
		((Control)groupBox6).ResumeLayout(false);
		((Control)groupBox6).PerformLayout();
		((Control)groupBox7).ResumeLayout(false);
		((Control)groupBox7).PerformLayout();
		((Control)groupBox8).ResumeLayout(false);
		((Control)groupBox8).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
