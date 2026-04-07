using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class CompensateParameterFrm : Form
{
	private ushort ConstBitComTableEn;

	private IContainer components;

	private Label lbAxisNameX;

	private TextBox tbComHzX;

	private TextBox tbComZeroX;

	private CheckBox chbComEnX;

	private Button bnSetParaX;

	private Button bnUpdateParaX;

	private Label label1;

	private Label label2;

	private Button bnUpdateParaY;

	private Button bnSetParaY;

	private CheckBox chbComEnY;

	private TextBox tbComZeroY;

	private TextBox tbComHzY;

	private Label label3;

	private Button bnUpdateParaZ;

	private Button bnSetParaZ;

	private CheckBox chbComEnZ;

	private TextBox tbComZeroZ;

	private TextBox tbComHzZ;

	private Label label4;

	private Button bnUpdateParaA;

	private Button bnSetParaA;

	private CheckBox chbComEnA;

	private TextBox tbComZeroA;

	private TextBox tbComHzA;

	private Label label5;

	private Button bnUpdateParaB;

	private Button bnSetParaB;

	private CheckBox chbComEnB;

	private TextBox tbComZeroB;

	private TextBox tbComHzB;

	private Label label6;

	private Button bnUpdateParaC;

	private Button bnSetParaC;

	private CheckBox chbComEnC;

	private TextBox tbComZeroC;

	private TextBox tbComHzC;

	private Label label7;

	private MainFrm PMainFrm;

	private byte ConstPara_ComHz;

	private byte ConstPara_ComZero;

	private byte ConstPara_InputLev;

	private ushort ConstBitComEn;

	private Label label8;

	private TextBox tbComDiffC;

	private TextBox tbComDiffB;

	private TextBox tbComDiffA;

	private TextBox tbComDiffZ;

	private TextBox tbComDiffY;

	private TextBox tbComDiffX;

	private Timer timer1;

	private Button bnClrInPulC;

	private Button bnClrInPulB;

	private Button bnClrInPulA;

	private Button bnClrInPulZ;

	private Button bnClrInPulY;

	private Button bnClrInPulX;

	private CheckBox chbComTableEnC;

	private CheckBox chbComTableEnB;

	private CheckBox chbComTableEnA;

	private CheckBox chbComTableEnZ;

	private CheckBox chbComTableEnY;

	private CheckBox chbComTableEnX;

	private Button bnSaveParaToRom;

	private GroupBox groupBox1;

	private GroupBox groupBox2;

	private Label label9;

	private Label label10;

	private Label label11;

	private Button bnPlarformLevStop;

	private Button bnPlarformLevStart;

	private ComboBox cmbPlatformZID;

	private ComboBox cmbPlatformYID;

	private ComboBox cmbPlatformXID;

	public CompensateParameterFrm()
	{
		InitializeComponent();
	}

	private void CompensateParameterFrm_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		ConstPara_ComHz = 10;
		ConstPara_ComZero = 11;
		ConstPara_InputLev = 7;
		ConstBitComEn = 64;
		ConstBitComTableEn = 16;
		timer1.Enabled = false;
		timer1.Interval = 300;
		timer1.Enabled = true;
		((Control)tbComHzX).Text = PMainFrm.SysPara[ConstPara_ComHz].ToString();
		((Control)tbComZeroX).Text = PMainFrm.SysPara[ConstPara_ComZero].ToString();
		((Control)tbComHzY).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis + ConstPara_ComHz].ToString();
		((Control)tbComZeroY).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis + ConstPara_ComZero].ToString();
		((Control)tbComHzZ).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_ComHz].ToString();
		((Control)tbComZeroZ).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_ComZero].ToString();
		((Control)tbComHzA).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_ComHz].ToString();
		((Control)tbComZeroA).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_ComZero].ToString();
		((Control)tbComHzB).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_ComHz].ToString();
		((Control)tbComZeroB).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_ComZero].ToString();
		((Control)tbComHzC).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_ComHz].ToString();
		((Control)tbComZeroC).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_ComZero].ToString();
		ushort num = 0;
		float[] sysPara = PMainFrm.SysPara;
		_ = PMainFrm.Mccard.ParaNumPerAxis;
		num = (ushort)sysPara[0 + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnX.Checked = true;
			((Control)chbComEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnX.Checked = false;
			((Control)chbComEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnX.Checked = true;
			((Control)chbComTableEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnX.Checked = false;
			((Control)chbComTableEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnY.Checked = true;
			((Control)chbComEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnY.Checked = false;
			((Control)chbComEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnY.Checked = true;
			((Control)chbComTableEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnY.Checked = false;
			((Control)chbComTableEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnZ.Checked = true;
			((Control)chbComEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnZ.Checked = false;
			((Control)chbComEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnZ.Checked = true;
			((Control)chbComTableEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnZ.Checked = false;
			((Control)chbComTableEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnA.Checked = true;
			((Control)chbComEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnA.Checked = false;
			((Control)chbComEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnA.Checked = true;
			((Control)chbComTableEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnA.Checked = false;
			((Control)chbComTableEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnB.Checked = true;
			((Control)chbComEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnB.Checked = false;
			((Control)chbComEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnB.Checked = true;
			((Control)chbComTableEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnB.Checked = false;
			((Control)chbComTableEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_InputLev];
		if (ConstBitComEn == (ConstBitComEn & num))
		{
			chbComEnC.Checked = true;
			((Control)chbComEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComEnC.Checked = false;
			((Control)chbComEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		if (ConstBitComTableEn == (ConstBitComTableEn & num))
		{
			chbComTableEnC.Checked = true;
			((Control)chbComTableEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
		}
		else
		{
			chbComTableEnC.Checked = false;
			((Control)chbComTableEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
		}
		((ListControl)cmbPlatformXID).SelectedIndex = 0;
		((ListControl)cmbPlatformYID).SelectedIndex = 1;
		((ListControl)cmbPlatformZID).SelectedIndex = 2;
	}

	private void chbComEnX_Leave(object sender, EventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		int num = 0;
		byte b = PMainFrm.Mccard.ParaSum;
		CheckBox val = (CheckBox)sender;
		if (((Control)chbComEnX).Name == ((Control)val).Name)
		{
			_ = PMainFrm.Mccard.ParaNumPerAxis;
			b = (byte)(0 + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		else if (((Control)chbComEnY).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		else if (((Control)chbComEnZ).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		else if (((Control)chbComEnA).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		else if (((Control)chbComEnB).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		else if (((Control)chbComEnC).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_InputLev);
			num = (int)PMainFrm.SysPara[b];
			num = ((!val.Checked) ? (num & ~ConstBitComEn) : (num | ConstBitComEn));
		}
		if (b < PMainFrm.Mccard.ParaSum)
		{
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(b / PMainFrm.Mccard.ParaNumPerAxis), (byte)(b % PMainFrm.Mccard.ParaNumPerAxis), num))
			{
				PMainFrm.SysPara[b] = num;
			}
			ushort num2 = 0;
			float[] sysPara = PMainFrm.SysPara;
			_ = PMainFrm.Mccard.ParaNumPerAxis;
			num2 = (ushort)sysPara[0 + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnX).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
			num2 = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnY).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
			num2 = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnZ).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
			num2 = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnA).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
			num2 = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnB).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
			num2 = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_InputLev];
			if (ConstBitComEn == (ConstBitComEn & num2))
			{
				((Control)chbComEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)chbComEnC).Text = GlobalData.GlobalLanguage.ComParaFrm_WordCom + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
		}
	}

	private void bnUpdateParaX_Leave(object sender, EventArgs e)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		byte b = 85;
		Button val = (Button)sender;
		if (((Control)bnUpdateParaX).Name == ((Control)val).Name)
		{
			b = 0;
		}
		else if (((Control)bnUpdateParaY).Name == ((Control)val).Name)
		{
			b = 1;
		}
		else if (((Control)bnUpdateParaZ).Name == ((Control)val).Name)
		{
			b = 2;
		}
		else if (((Control)bnUpdateParaA).Name == ((Control)val).Name)
		{
			b = 3;
		}
		else if (((Control)bnUpdateParaB).Name == ((Control)val).Name)
		{
			b = 4;
		}
		else if (((Control)bnUpdateParaC).Name == ((Control)val).Name)
		{
			b = 5;
		}
		if (b < PMainFrm.Mccard.ParaSum)
		{
			PMainFrm.Mccard.MoCtrCard_UpdateCompensateParameter(b);
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		int[] array = new int[PMainFrm.Mccard.MaxAxisNum];
		if (1 == PMainFrm.Mccard.MoCtrCard_GetComDiff(byte.MaxValue, array))
		{
			((Control)tbComDiffX).Text = array[0].ToString();
			((Control)tbComDiffY).Text = array[1].ToString();
			((Control)tbComDiffZ).Text = array[2].ToString();
			((Control)tbComDiffA).Text = array[3].ToString();
			((Control)tbComDiffB).Text = array[4].ToString();
			((Control)tbComDiffC).Text = array[5].ToString();
		}
	}

	private void bnSetParaY_Click(object sender, EventArgs e)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		byte b = PMainFrm.Mccard.ParaSum;
		float num = 0f;
		byte b2 = PMainFrm.Mccard.ParaSum;
		float num2 = 0f;
		Button val = (Button)sender;
		if (((Control)bnSetParaX).Name == ((Control)val).Name)
		{
			_ = PMainFrm.Mccard.ParaNumPerAxis;
			b = (byte)(0 + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzX).Text);
			_ = PMainFrm.Mccard.ParaNumPerAxis;
			b2 = (byte)(0 + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroX).Text);
		}
		else if (((Control)bnSetParaY).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzY).Text);
			b2 = (byte)(PMainFrm.Mccard.ParaNumPerAxis + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroY).Text);
		}
		else if (((Control)bnSetParaZ).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzZ).Text);
			b2 = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 2 + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroZ).Text);
		}
		else if (((Control)bnSetParaA).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzA).Text);
			b2 = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 3 + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroA).Text);
		}
		else if (((Control)bnSetParaB).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzB).Text);
			b2 = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 4 + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroB).Text);
		}
		else if (((Control)bnSetParaC).Name == ((Control)val).Name)
		{
			b = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_ComHz);
			num = float.Parse(((Control)tbComHzC).Text);
			b2 = (byte)(PMainFrm.Mccard.ParaNumPerAxis * 5 + ConstPara_ComZero);
			num2 = float.Parse(((Control)tbComZeroC).Text);
		}
		if (b < PMainFrm.Mccard.ParaSum && b2 < PMainFrm.Mccard.ParaSum)
		{
			if (PMainFrm.Mccard.CommLinkEnable)
			{
				if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(b / PMainFrm.Mccard.ParaNumPerAxis), (byte)(b % PMainFrm.Mccard.ParaNumPerAxis), num))
				{
					PMainFrm.SysPara[b] = num;
				}
				else
				{
					PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_SetParameterFailed);
				}
				if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(b2 / PMainFrm.Mccard.ParaNumPerAxis), (byte)(b2 % PMainFrm.Mccard.ParaNumPerAxis), num2))
				{
					PMainFrm.SysPara[b2] = num2;
				}
				else
				{
					PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_SetParameterFailed);
				}
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_ConnectMCCard);
			}
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_ParameterIndxeError);
		}
	}

	private void bnClrInPulX_Click(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		byte b = byte.MaxValue;
		Button val = (Button)sender;
		if (((Control)bnClrInPulX).Name == ((Control)val).Name)
		{
			b = 0;
		}
		else if (((Control)bnClrInPulY).Name == ((Control)val).Name)
		{
			b = 1;
		}
		else if (((Control)bnClrInPulZ).Name == ((Control)val).Name)
		{
			b = 2;
		}
		else if (((Control)bnClrInPulA).Name == ((Control)val).Name)
		{
			b = 3;
		}
		else if (((Control)bnClrInPulB).Name == ((Control)val).Name)
		{
			b = 4;
		}
		else if (((Control)bnClrInPulC).Name == ((Control)val).Name)
		{
			b = 5;
		}
		if (b < 6)
		{
			PMainFrm.Mccard.MoCtrCard_ClearInternalPulseCount(b);
		}
	}

	private void bnSaveParaToRom_Click(object sender, EventArgs e)
	{
		PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM();
	}

	private void chbComTableEnX_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		byte b = 0;
		_ = PMainFrm.Mccard.ParaSum;
		CheckBox val = (CheckBox)sender;
		if (((Control)chbComTableEnX).Name == ((Control)val).Name)
		{
			b = 0;
		}
		else if (((Control)chbComTableEnY).Name == ((Control)val).Name)
		{
			b = 1;
		}
		else if (((Control)chbComTableEnZ).Name == ((Control)val).Name)
		{
			b = 2;
		}
		else if (((Control)chbComTableEnA).Name == ((Control)val).Name)
		{
			b = 3;
		}
		else if (((Control)chbComTableEnB).Name == ((Control)val).Name)
		{
			b = 4;
		}
		else if (((Control)chbComTableEnC).Name == ((Control)val).Name)
		{
			b = 5;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_EnableComTableFunction(b, (byte)(val.Checked ? 1 : 0)))
		{
			ushort num = 0;
			num = (ushort)PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * b + ConstPara_InputLev];
			num = (val.Checked ? ((ushort)(num | ConstBitComTableEn)) : ((ushort)(num & ~ConstBitComTableEn)));
			PMainFrm.SysPara[PMainFrm.Mccard.ParaNumPerAxis * b + ConstPara_InputLev] = (int)num;
			if (val.Checked)
			{
				((Control)val).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordEnable;
			}
			else
			{
				((Control)val).Text = GlobalData.GlobalLanguage.ComParaFrm_WordComTable + GlobalData.GlobalLanguage.ComParaFrm_WordDisable;
			}
		}
		else
		{
			((Control)val).Text = GlobalData.GlobalLanguage.ComParaFrm_SetParameterFailed;
		}
	}

	private void bnPlarformLevStart_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_EnablePlarformCom((byte)((ListControl)cmbPlatformZID).SelectedIndex, (byte)((ListControl)cmbPlatformXID).SelectedIndex, (byte)((ListControl)cmbPlatformYID).SelectedIndex))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.ComParaFrm_EnablePlatformCompensate);
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_EnablePlatformCompensateFailed);
		}
	}

	private void bnPlarformLevStop_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_DisablePlarformCom((byte)((ListControl)cmbPlatformZID).SelectedIndex))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.ComParaFrm_DisablePlatformCompensate);
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ComParaFrm_DisablePlatformCompensateFailed);
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
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Expected O, but got Unknown
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CompensateParameterFrm));
		lbAxisNameX = new Label();
		tbComHzX = new TextBox();
		tbComZeroX = new TextBox();
		chbComEnX = new CheckBox();
		bnSetParaX = new Button();
		bnUpdateParaX = new Button();
		label1 = new Label();
		label2 = new Label();
		bnUpdateParaY = new Button();
		bnSetParaY = new Button();
		chbComEnY = new CheckBox();
		tbComZeroY = new TextBox();
		tbComHzY = new TextBox();
		label3 = new Label();
		bnUpdateParaZ = new Button();
		bnSetParaZ = new Button();
		chbComEnZ = new CheckBox();
		tbComZeroZ = new TextBox();
		tbComHzZ = new TextBox();
		label4 = new Label();
		bnUpdateParaA = new Button();
		bnSetParaA = new Button();
		chbComEnA = new CheckBox();
		tbComZeroA = new TextBox();
		tbComHzA = new TextBox();
		label5 = new Label();
		bnUpdateParaB = new Button();
		bnSetParaB = new Button();
		chbComEnB = new CheckBox();
		tbComZeroB = new TextBox();
		tbComHzB = new TextBox();
		label6 = new Label();
		bnUpdateParaC = new Button();
		bnSetParaC = new Button();
		chbComEnC = new CheckBox();
		tbComZeroC = new TextBox();
		tbComHzC = new TextBox();
		label7 = new Label();
		label8 = new Label();
		tbComDiffC = new TextBox();
		tbComDiffB = new TextBox();
		tbComDiffA = new TextBox();
		tbComDiffZ = new TextBox();
		tbComDiffY = new TextBox();
		tbComDiffX = new TextBox();
		timer1 = new Timer(components);
		bnClrInPulC = new Button();
		bnClrInPulB = new Button();
		bnClrInPulA = new Button();
		bnClrInPulZ = new Button();
		bnClrInPulY = new Button();
		bnClrInPulX = new Button();
		chbComTableEnC = new CheckBox();
		chbComTableEnB = new CheckBox();
		chbComTableEnA = new CheckBox();
		chbComTableEnZ = new CheckBox();
		chbComTableEnY = new CheckBox();
		chbComTableEnX = new CheckBox();
		bnSaveParaToRom = new Button();
		groupBox1 = new GroupBox();
		groupBox2 = new GroupBox();
		bnPlarformLevStop = new Button();
		bnPlarformLevStart = new Button();
		cmbPlatformZID = new ComboBox();
		cmbPlatformYID = new ComboBox();
		cmbPlatformXID = new ComboBox();
		label9 = new Label();
		label10 = new Label();
		label11 = new Label();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(lbAxisNameX, "lbAxisNameX");
		((Control)lbAxisNameX).Name = "lbAxisNameX";
		componentResourceManager.ApplyResources(tbComHzX, "tbComHzX");
		((Control)tbComHzX).Name = "tbComHzX";
		componentResourceManager.ApplyResources(tbComZeroX, "tbComZeroX");
		((Control)tbComZeroX).Name = "tbComZeroX";
		componentResourceManager.ApplyResources(chbComEnX, "chbComEnX");
		((Control)chbComEnX).Name = "chbComEnX";
		((ButtonBase)chbComEnX).UseVisualStyleBackColor = true;
		chbComEnX.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(bnSetParaX, "bnSetParaX");
		((Control)bnSetParaX).Name = "bnSetParaX";
		((ButtonBase)bnSetParaX).UseVisualStyleBackColor = true;
		((Control)bnSetParaX).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(bnUpdateParaX, "bnUpdateParaX");
		((Control)bnUpdateParaX).Name = "bnUpdateParaX";
		((ButtonBase)bnUpdateParaX).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaX).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(bnUpdateParaY, "bnUpdateParaY");
		((Control)bnUpdateParaY).Name = "bnUpdateParaY";
		((ButtonBase)bnUpdateParaY).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaY).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(bnSetParaY, "bnSetParaY");
		((Control)bnSetParaY).Name = "bnSetParaY";
		((ButtonBase)bnSetParaY).UseVisualStyleBackColor = true;
		((Control)bnSetParaY).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(chbComEnY, "chbComEnY");
		((Control)chbComEnY).Name = "chbComEnY";
		((ButtonBase)chbComEnY).UseVisualStyleBackColor = true;
		chbComEnY.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(tbComZeroY, "tbComZeroY");
		((Control)tbComZeroY).Name = "tbComZeroY";
		componentResourceManager.ApplyResources(tbComHzY, "tbComHzY");
		((Control)tbComHzY).Name = "tbComHzY";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(bnUpdateParaZ, "bnUpdateParaZ");
		((Control)bnUpdateParaZ).Name = "bnUpdateParaZ";
		((ButtonBase)bnUpdateParaZ).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaZ).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(bnSetParaZ, "bnSetParaZ");
		((Control)bnSetParaZ).Name = "bnSetParaZ";
		((ButtonBase)bnSetParaZ).UseVisualStyleBackColor = true;
		((Control)bnSetParaZ).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(chbComEnZ, "chbComEnZ");
		((Control)chbComEnZ).Name = "chbComEnZ";
		((ButtonBase)chbComEnZ).UseVisualStyleBackColor = true;
		chbComEnZ.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(tbComZeroZ, "tbComZeroZ");
		((Control)tbComZeroZ).Name = "tbComZeroZ";
		componentResourceManager.ApplyResources(tbComHzZ, "tbComHzZ");
		((Control)tbComHzZ).Name = "tbComHzZ";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(bnUpdateParaA, "bnUpdateParaA");
		((Control)bnUpdateParaA).Name = "bnUpdateParaA";
		((ButtonBase)bnUpdateParaA).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaA).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(bnSetParaA, "bnSetParaA");
		((Control)bnSetParaA).Name = "bnSetParaA";
		((ButtonBase)bnSetParaA).UseVisualStyleBackColor = true;
		((Control)bnSetParaA).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(chbComEnA, "chbComEnA");
		((Control)chbComEnA).Name = "chbComEnA";
		((ButtonBase)chbComEnA).UseVisualStyleBackColor = true;
		chbComEnA.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(tbComZeroA, "tbComZeroA");
		((Control)tbComZeroA).Name = "tbComZeroA";
		componentResourceManager.ApplyResources(tbComHzA, "tbComHzA");
		((Control)tbComHzA).Name = "tbComHzA";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(bnUpdateParaB, "bnUpdateParaB");
		((Control)bnUpdateParaB).Name = "bnUpdateParaB";
		((ButtonBase)bnUpdateParaB).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaB).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(bnSetParaB, "bnSetParaB");
		((Control)bnSetParaB).Name = "bnSetParaB";
		((ButtonBase)bnSetParaB).UseVisualStyleBackColor = true;
		((Control)bnSetParaB).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(chbComEnB, "chbComEnB");
		((Control)chbComEnB).Name = "chbComEnB";
		((ButtonBase)chbComEnB).UseVisualStyleBackColor = true;
		chbComEnB.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(tbComZeroB, "tbComZeroB");
		((Control)tbComZeroB).Name = "tbComZeroB";
		componentResourceManager.ApplyResources(tbComHzB, "tbComHzB");
		((Control)tbComHzB).Name = "tbComHzB";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(bnUpdateParaC, "bnUpdateParaC");
		((Control)bnUpdateParaC).Name = "bnUpdateParaC";
		((ButtonBase)bnUpdateParaC).UseVisualStyleBackColor = true;
		((Control)bnUpdateParaC).Click += bnUpdateParaX_Leave;
		componentResourceManager.ApplyResources(bnSetParaC, "bnSetParaC");
		((Control)bnSetParaC).Name = "bnSetParaC";
		((ButtonBase)bnSetParaC).UseVisualStyleBackColor = true;
		((Control)bnSetParaC).Click += bnSetParaY_Click;
		componentResourceManager.ApplyResources(chbComEnC, "chbComEnC");
		((Control)chbComEnC).Name = "chbComEnC";
		((ButtonBase)chbComEnC).UseVisualStyleBackColor = true;
		chbComEnC.CheckedChanged += chbComEnX_Leave;
		componentResourceManager.ApplyResources(tbComZeroC, "tbComZeroC");
		((Control)tbComZeroC).Name = "tbComZeroC";
		componentResourceManager.ApplyResources(tbComHzC, "tbComHzC");
		((Control)tbComHzC).Name = "tbComHzC";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(tbComDiffC, "tbComDiffC");
		((Control)tbComDiffC).Name = "tbComDiffC";
		componentResourceManager.ApplyResources(tbComDiffB, "tbComDiffB");
		((Control)tbComDiffB).Name = "tbComDiffB";
		componentResourceManager.ApplyResources(tbComDiffA, "tbComDiffA");
		((Control)tbComDiffA).Name = "tbComDiffA";
		componentResourceManager.ApplyResources(tbComDiffZ, "tbComDiffZ");
		((Control)tbComDiffZ).Name = "tbComDiffZ";
		componentResourceManager.ApplyResources(tbComDiffY, "tbComDiffY");
		((Control)tbComDiffY).Name = "tbComDiffY";
		componentResourceManager.ApplyResources(tbComDiffX, "tbComDiffX");
		((Control)tbComDiffX).Name = "tbComDiffX";
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(bnClrInPulC, "bnClrInPulC");
		((Control)bnClrInPulC).Name = "bnClrInPulC";
		((ButtonBase)bnClrInPulC).UseVisualStyleBackColor = true;
		((Control)bnClrInPulC).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(bnClrInPulB, "bnClrInPulB");
		((Control)bnClrInPulB).Name = "bnClrInPulB";
		((ButtonBase)bnClrInPulB).UseVisualStyleBackColor = true;
		((Control)bnClrInPulB).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(bnClrInPulA, "bnClrInPulA");
		((Control)bnClrInPulA).Name = "bnClrInPulA";
		((ButtonBase)bnClrInPulA).UseVisualStyleBackColor = true;
		((Control)bnClrInPulA).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(bnClrInPulZ, "bnClrInPulZ");
		((Control)bnClrInPulZ).Name = "bnClrInPulZ";
		((ButtonBase)bnClrInPulZ).UseVisualStyleBackColor = true;
		((Control)bnClrInPulZ).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(bnClrInPulY, "bnClrInPulY");
		((Control)bnClrInPulY).Name = "bnClrInPulY";
		((ButtonBase)bnClrInPulY).UseVisualStyleBackColor = true;
		((Control)bnClrInPulY).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(bnClrInPulX, "bnClrInPulX");
		((Control)bnClrInPulX).Name = "bnClrInPulX";
		((ButtonBase)bnClrInPulX).UseVisualStyleBackColor = true;
		((Control)bnClrInPulX).Click += bnClrInPulX_Click;
		componentResourceManager.ApplyResources(chbComTableEnC, "chbComTableEnC");
		((Control)chbComTableEnC).Name = "chbComTableEnC";
		((ButtonBase)chbComTableEnC).UseVisualStyleBackColor = true;
		chbComTableEnC.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(chbComTableEnB, "chbComTableEnB");
		((Control)chbComTableEnB).Name = "chbComTableEnB";
		((ButtonBase)chbComTableEnB).UseVisualStyleBackColor = true;
		chbComTableEnB.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(chbComTableEnA, "chbComTableEnA");
		((Control)chbComTableEnA).Name = "chbComTableEnA";
		((ButtonBase)chbComTableEnA).UseVisualStyleBackColor = true;
		chbComTableEnA.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(chbComTableEnZ, "chbComTableEnZ");
		((Control)chbComTableEnZ).Name = "chbComTableEnZ";
		((ButtonBase)chbComTableEnZ).UseVisualStyleBackColor = true;
		chbComTableEnZ.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(chbComTableEnY, "chbComTableEnY");
		((Control)chbComTableEnY).Name = "chbComTableEnY";
		((ButtonBase)chbComTableEnY).UseVisualStyleBackColor = true;
		chbComTableEnY.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(chbComTableEnX, "chbComTableEnX");
		((Control)chbComTableEnX).Name = "chbComTableEnX";
		((ButtonBase)chbComTableEnX).UseVisualStyleBackColor = true;
		chbComTableEnX.CheckedChanged += chbComTableEnX_CheckedChanged;
		componentResourceManager.ApplyResources(bnSaveParaToRom, "bnSaveParaToRom");
		((Control)bnSaveParaToRom).Name = "bnSaveParaToRom";
		((ButtonBase)bnSaveParaToRom).UseVisualStyleBackColor = true;
		((Control)bnSaveParaToRom).Click += bnSaveParaToRom_Click;
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)bnSaveParaToRom);
		((Control)groupBox1).Controls.Add((Control)(object)lbAxisNameX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnC);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnB);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnA);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnY);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaX);
		((Control)groupBox1).Controls.Add((Control)(object)chbComTableEnX);
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulC);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulB);
		((Control)groupBox1).Controls.Add((Control)(object)label3);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulA);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzY);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroY);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulY);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnY);
		((Control)groupBox1).Controls.Add((Control)(object)bnClrInPulX);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaY);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffC);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaY);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffB);
		((Control)groupBox1).Controls.Add((Control)(object)label4);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffA);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffY);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbComDiffX);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaZ);
		((Control)groupBox1).Controls.Add((Control)(object)label8);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaC);
		((Control)groupBox1).Controls.Add((Control)(object)label5);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaC);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzA);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnC);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroA);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroC);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnA);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzC);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaA);
		((Control)groupBox1).Controls.Add((Control)(object)label7);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaA);
		((Control)groupBox1).Controls.Add((Control)(object)bnUpdateParaB);
		((Control)groupBox1).Controls.Add((Control)(object)label6);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetParaB);
		((Control)groupBox1).Controls.Add((Control)(object)tbComHzB);
		((Control)groupBox1).Controls.Add((Control)(object)chbComEnB);
		((Control)groupBox1).Controls.Add((Control)(object)tbComZeroB);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)bnPlarformLevStop);
		((Control)groupBox2).Controls.Add((Control)(object)bnPlarformLevStart);
		((Control)groupBox2).Controls.Add((Control)(object)cmbPlatformZID);
		((Control)groupBox2).Controls.Add((Control)(object)cmbPlatformYID);
		((Control)groupBox2).Controls.Add((Control)(object)cmbPlatformXID);
		((Control)groupBox2).Controls.Add((Control)(object)label9);
		((Control)groupBox2).Controls.Add((Control)(object)label10);
		((Control)groupBox2).Controls.Add((Control)(object)label11);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(bnPlarformLevStop, "bnPlarformLevStop");
		((Control)bnPlarformLevStop).Name = "bnPlarformLevStop";
		((ButtonBase)bnPlarformLevStop).UseVisualStyleBackColor = true;
		((Control)bnPlarformLevStop).Click += bnPlarformLevStop_Click;
		componentResourceManager.ApplyResources(bnPlarformLevStart, "bnPlarformLevStart");
		((Control)bnPlarformLevStart).Name = "bnPlarformLevStart";
		((ButtonBase)bnPlarformLevStart).UseVisualStyleBackColor = true;
		((Control)bnPlarformLevStart).Click += bnPlarformLevStart_Click;
		componentResourceManager.ApplyResources(cmbPlatformZID, "cmbPlatformZID");
		((ListControl)cmbPlatformZID).FormattingEnabled = true;
		cmbPlatformZID.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("cmbPlatformZID.Items"),
			componentResourceManager.GetString("cmbPlatformZID.Items1"),
			componentResourceManager.GetString("cmbPlatformZID.Items2"),
			componentResourceManager.GetString("cmbPlatformZID.Items3"),
			componentResourceManager.GetString("cmbPlatformZID.Items4"),
			componentResourceManager.GetString("cmbPlatformZID.Items5")
		});
		((Control)cmbPlatformZID).Name = "cmbPlatformZID";
		componentResourceManager.ApplyResources(cmbPlatformYID, "cmbPlatformYID");
		((ListControl)cmbPlatformYID).FormattingEnabled = true;
		cmbPlatformYID.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("cmbPlatformYID.Items"),
			componentResourceManager.GetString("cmbPlatformYID.Items1"),
			componentResourceManager.GetString("cmbPlatformYID.Items2"),
			componentResourceManager.GetString("cmbPlatformYID.Items3"),
			componentResourceManager.GetString("cmbPlatformYID.Items4"),
			componentResourceManager.GetString("cmbPlatformYID.Items5")
		});
		((Control)cmbPlatformYID).Name = "cmbPlatformYID";
		componentResourceManager.ApplyResources(cmbPlatformXID, "cmbPlatformXID");
		((ListControl)cmbPlatformXID).FormattingEnabled = true;
		cmbPlatformXID.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("cmbPlatformXID.Items"),
			componentResourceManager.GetString("cmbPlatformXID.Items1"),
			componentResourceManager.GetString("cmbPlatformXID.Items2"),
			componentResourceManager.GetString("cmbPlatformXID.Items3"),
			componentResourceManager.GetString("cmbPlatformXID.Items4"),
			componentResourceManager.GetString("cmbPlatformXID.Items5")
		});
		((Control)cmbPlatformXID).Name = "cmbPlatformXID";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "CompensateParameterFrm";
		((Form)this).Load += CompensateParameterFrm_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
