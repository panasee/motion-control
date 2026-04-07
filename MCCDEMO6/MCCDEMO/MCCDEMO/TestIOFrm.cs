using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class TestIOFrm : Form
{
	private int gbAnalogZeroValue = 2048;

	private int gbAnalogScopeValue = 4096;

	private IContainer components;

	private GroupBox groupBox1;

	private CheckBox JoystickT;

	private CheckBox JoystickZ;

	private CheckBox JoystickY;

	private Button bnSaveParaToMCU;

	private HScrollBar JoystickZeroScroll;

	private Label VValT;

	private Label VValZ;

	private Label VValY;

	private Label VValX;

	private GroupBox groupBox2;

	private Label lbOutputStt;

	private Label lbInputStt;

	private PictureBox OUT7;

	private PictureBox IN7;

	private PictureBox OUT6;

	private PictureBox IN6;

	private PictureBox OUT5;

	private PictureBox IN5;

	private PictureBox OUT4;

	private PictureBox IN4;

	private PictureBox OUT3;

	private PictureBox IN3;

	private PictureBox OUT2;

	private PictureBox IN2;

	private PictureBox OUT1;

	private PictureBox IN1;

	private PictureBox OUT0;

	private PictureBox IN0;

	private Timer timer1;

	private MainFrm PMainFrm;

	private Label lbZeroAera;

	private Color slateBlack;

	private Color slateRed;

	private Color slateGreen;

	private CheckBox JoystickX;

	private PictureBox OUT15;

	private PictureBox OUT14;

	private PictureBox OUT13;

	private PictureBox OUT12;

	private PictureBox OUT11;

	private PictureBox OUT10;

	private PictureBox OUT9;

	private PictureBox OUT8;

	private PictureBox IN15;

	private PictureBox IN14;

	private PictureBox IN13;

	private PictureBox IN12;

	private PictureBox IN11;

	private PictureBox IN10;

	private PictureBox IN9;

	private PictureBox IN8;

	private PictureBox IN23;

	private PictureBox IN22;

	private PictureBox IN21;

	private PictureBox IN20;

	private PictureBox IN19;

	private PictureBox IN18;

	private PictureBox IN17;

	private PictureBox IN16;

	private PictureBox IN31;

	private PictureBox IN30;

	private PictureBox IN29;

	private PictureBox IN28;

	private PictureBox IN27;

	private PictureBox IN26;

	private PictureBox IN25;

	private PictureBox IN24;

	private Label label4;

	private Label label3;

	private Label label6;

	private Label label2;

	private Label label5;

	private Label label1;

	private Label label7;

	private PictureBox Z_Axis5;

	private PictureBox Z_Axis4;

	private PictureBox Z_Axis3;

	private PictureBox Z_Axis2;

	private PictureBox Z_Axis1;

	private PictureBox Z_Axis0;

	private Label label8;

	private Label VValC;

	private Label VValB;

	private CheckBox JoystickC;

	private CheckBox JoystickB;

	private byte ConstPara_AnaEn;

	private byte ConstPara_AnaZeroScope;

	private byte ParaIndxAdjustZeroScop;

	private byte ConstPara_AxisA;

	private byte ConstPara_AxisB;

	private byte ConstPara_AxisC;

	private byte ConstPara_AxisX;

	private byte ConstPara_AxisY;

	private byte ConstPara_AxisZ;

	private RadioButton rbAanScopAdjEnC;

	private RadioButton rbAanScopAdjEnB;

	private RadioButton rbAanScopAdjEnA;

	private RadioButton rbAanScopAdjEnZ;

	private RadioButton rbAanScopAdjEnY;

	private RadioButton rbAanScopAdjEnX;

	private Label label9;

	private Label label10;

	private PictureBox Z_Axis15;

	private PictureBox Z_Axis14;

	private PictureBox Z_Axis13;

	private PictureBox Z_Axis12;

	private PictureBox Z_Axis11;

	private PictureBox Z_Axis10;

	private PictureBox Z_Axis9;

	private PictureBox Z_Axis8;

	private PictureBox Z_Axis7;

	private PictureBox Z_Axis6;

	private GroupBox groupBox3;

	private RadioButton rbPS2Pad;

	private RadioButton rbJoyStick;

	public TestIOFrm()
	{
		InitializeComponent();
	}

	private void TestIOFrm_Load(object sender, EventArgs e)
	{
		slateBlack = Color.FromName("DarkSlateGray");
		slateRed = Color.FromName("Red");
		slateGreen = Color.FromName("Lime");
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		ParaIndxAdjustZeroScop = 0;
		ConstPara_AnaEn = 8;
		ConstPara_AnaZeroScope = 9;
		ConstPara_AxisX = 0;
		ConstPara_AxisY = (byte)(ConstPara_AxisX + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisZ = (byte)(ConstPara_AxisY + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisA = (byte)(ConstPara_AxisZ + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisB = (byte)(ConstPara_AxisA + PMainFrm.Mccard.ParaNumPerAxis);
		ConstPara_AxisC = (byte)(ConstPara_AxisB + PMainFrm.Mccard.ParaNumPerAxis);
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisX]))
		{
			JoystickX.Checked = true;
			((Control)JoystickX).BackColor = slateGreen;
			((Control)VValX).BackColor = slateGreen;
		}
		else
		{
			JoystickX.Checked = false;
			((Control)JoystickX).BackColor = slateRed;
			((Control)VValX).BackColor = slateRed;
		}
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisY]))
		{
			JoystickY.Checked = true;
			((Control)JoystickY).BackColor = slateGreen;
			((Control)VValY).BackColor = slateGreen;
		}
		else
		{
			JoystickY.Checked = false;
			((Control)JoystickY).BackColor = slateRed;
			((Control)VValY).BackColor = slateRed;
		}
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisZ]))
		{
			JoystickZ.Checked = true;
			((Control)JoystickZ).BackColor = slateGreen;
			((Control)VValZ).BackColor = slateGreen;
		}
		else
		{
			JoystickZ.Checked = false;
			((Control)JoystickZ).BackColor = slateRed;
			((Control)VValZ).BackColor = slateRed;
		}
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisA]))
		{
			JoystickT.Checked = true;
			((Control)JoystickT).BackColor = slateGreen;
			((Control)VValT).BackColor = slateGreen;
		}
		else
		{
			JoystickT.Checked = false;
			((Control)JoystickT).BackColor = slateRed;
			((Control)VValT).BackColor = slateRed;
		}
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisB]))
		{
			JoystickB.Checked = true;
			((Control)JoystickB).BackColor = slateGreen;
			((Control)VValB).BackColor = slateGreen;
		}
		else
		{
			JoystickB.Checked = false;
			((Control)JoystickB).BackColor = slateRed;
			((Control)VValB).BackColor = slateRed;
		}
		if (1 == (1 & (int)PMainFrm.SysPara[ConstPara_AnaEn + ConstPara_AxisC]))
		{
			JoystickC.Checked = true;
			((Control)JoystickC).BackColor = slateGreen;
			((Control)VValC).BackColor = slateGreen;
		}
		else
		{
			JoystickC.Checked = false;
			((Control)JoystickC).BackColor = slateRed;
			((Control)VValC).BackColor = slateRed;
		}
		((ScrollBar)JoystickZeroScroll).Maximum = 2048;
		((ScrollBar)JoystickZeroScroll).Minimum = 0;
		((ScrollBar)JoystickZeroScroll).Value = 0;
		((Control)lbZeroAera).Text = "";
		timer1.Enabled = false;
		timer1.Interval = 400;
		timer1.Enabled = true;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		int[] array = new int[PMainFrm.Mccard.MaxAxisNum];
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetAdVal(byte.MaxValue, array))
		{
			((Control)VValX).Text = ((float)array[0] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
			((Control)VValY).Text = ((float)array[1] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
			((Control)VValZ).Text = ((float)array[2] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
			((Control)VValT).Text = ((float)array[3] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
			((Control)VValB).Text = ((float)array[4] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
			((Control)VValC).Text = ((float)array[5] / (float)gbAnalogScopeValue * 3.3f).ToString("f1") + "V";
		}
		uint[] array2 = new uint[1];
		if (!PMainFrm.Mccard.CommLinkEnable)
		{
			return;
		}
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetInputState(0, array2))
		{
			if (1 == (array2[0] & 1))
			{
				((Control)IN0).BackColor = slateBlack;
			}
			else
			{
				((Control)IN0).BackColor = slateRed;
			}
			if (2 == (array2[0] & 2))
			{
				((Control)IN1).BackColor = slateBlack;
			}
			else
			{
				((Control)IN1).BackColor = slateRed;
			}
			if (4 == (array2[0] & 4))
			{
				((Control)IN2).BackColor = slateBlack;
			}
			else
			{
				((Control)IN2).BackColor = slateRed;
			}
			if (8 == (array2[0] & 8))
			{
				((Control)IN3).BackColor = slateBlack;
			}
			else
			{
				((Control)IN3).BackColor = slateRed;
			}
			if (16 == (array2[0] & 0x10))
			{
				((Control)IN4).BackColor = slateBlack;
			}
			else
			{
				((Control)IN4).BackColor = slateRed;
			}
			if (32 == (array2[0] & 0x20))
			{
				((Control)IN5).BackColor = slateBlack;
			}
			else
			{
				((Control)IN5).BackColor = slateRed;
			}
			if (64 == (array2[0] & 0x40))
			{
				((Control)IN6).BackColor = slateBlack;
			}
			else
			{
				((Control)IN6).BackColor = slateRed;
			}
			if (128 == (array2[0] & 0x80))
			{
				((Control)IN7).BackColor = slateBlack;
			}
			else
			{
				((Control)IN7).BackColor = slateRed;
			}
			if (256 == (array2[0] & 0x100))
			{
				((Control)IN8).BackColor = slateBlack;
			}
			else
			{
				((Control)IN8).BackColor = slateRed;
			}
			if (512 == (array2[0] & 0x200))
			{
				((Control)IN9).BackColor = slateBlack;
			}
			else
			{
				((Control)IN9).BackColor = slateRed;
			}
			if (1024 == (array2[0] & 0x400))
			{
				((Control)IN10).BackColor = slateBlack;
			}
			else
			{
				((Control)IN10).BackColor = slateRed;
			}
			if (2048 == (array2[0] & 0x800))
			{
				((Control)IN11).BackColor = slateBlack;
			}
			else
			{
				((Control)IN11).BackColor = slateRed;
			}
			if (4096 == (array2[0] & 0x1000))
			{
				((Control)IN12).BackColor = slateBlack;
			}
			else
			{
				((Control)IN12).BackColor = slateRed;
			}
			if (8192 == (array2[0] & 0x2000))
			{
				((Control)IN13).BackColor = slateBlack;
			}
			else
			{
				((Control)IN13).BackColor = slateRed;
			}
			if (16384 == (array2[0] & 0x4000))
			{
				((Control)IN14).BackColor = slateBlack;
			}
			else
			{
				((Control)IN14).BackColor = slateRed;
			}
			if (32768 == (array2[0] & 0x8000))
			{
				((Control)IN15).BackColor = slateBlack;
			}
			else
			{
				((Control)IN15).BackColor = slateRed;
			}
			if (65536 == (array2[0] & 0x10000))
			{
				((Control)IN16).BackColor = slateBlack;
			}
			else
			{
				((Control)IN16).BackColor = slateRed;
			}
			if (131072 == (array2[0] & 0x20000))
			{
				((Control)IN17).BackColor = slateBlack;
			}
			else
			{
				((Control)IN17).BackColor = slateRed;
			}
			if (262144 == (array2[0] & 0x40000))
			{
				((Control)IN18).BackColor = slateBlack;
			}
			else
			{
				((Control)IN18).BackColor = slateRed;
			}
			if (524288 == (array2[0] & 0x80000))
			{
				((Control)IN19).BackColor = slateBlack;
			}
			else
			{
				((Control)IN19).BackColor = slateRed;
			}
			if (1048576 == (array2[0] & 0x100000))
			{
				((Control)IN20).BackColor = slateBlack;
			}
			else
			{
				((Control)IN20).BackColor = slateRed;
			}
			if (2097152 == (array2[0] & 0x200000))
			{
				((Control)IN21).BackColor = slateBlack;
			}
			else
			{
				((Control)IN21).BackColor = slateRed;
			}
			if (4194304 == (array2[0] & 0x400000))
			{
				((Control)IN22).BackColor = slateBlack;
			}
			else
			{
				((Control)IN22).BackColor = slateRed;
			}
			if (8388608 == (array2[0] & 0x800000))
			{
				((Control)IN23).BackColor = slateBlack;
			}
			else
			{
				((Control)IN23).BackColor = slateRed;
			}
			if (16777216 == (array2[0] & 0x1000000))
			{
				((Control)IN24).BackColor = slateBlack;
			}
			else
			{
				((Control)IN24).BackColor = slateRed;
			}
			if (33554432 == (array2[0] & 0x2000000))
			{
				((Control)IN25).BackColor = slateBlack;
			}
			else
			{
				((Control)IN25).BackColor = slateRed;
			}
			if (67108864 == (array2[0] & 0x4000000))
			{
				((Control)IN26).BackColor = slateBlack;
			}
			else
			{
				((Control)IN26).BackColor = slateRed;
			}
			if (134217728 == (array2[0] & 0x8000000))
			{
				((Control)IN27).BackColor = slateBlack;
			}
			else
			{
				((Control)IN27).BackColor = slateRed;
			}
			if (268435456 == (array2[0] & 0x10000000))
			{
				((Control)IN28).BackColor = slateBlack;
			}
			else
			{
				((Control)IN28).BackColor = slateRed;
			}
			if (536870912 == (array2[0] & 0x20000000))
			{
				((Control)IN29).BackColor = slateBlack;
			}
			else
			{
				((Control)IN29).BackColor = slateRed;
			}
			if (1073741824 == (array2[0] & 0x40000000))
			{
				((Control)IN30).BackColor = slateBlack;
			}
			else
			{
				((Control)IN30).BackColor = slateRed;
			}
			if (int.MinValue == (int)(array2[0] & 0x80000000u))
			{
				((Control)IN31).BackColor = slateBlack;
			}
			else
			{
				((Control)IN31).BackColor = slateRed;
			}
		}
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetOutState(0, array2))
		{
			if (1 == (array2[0] & 1))
			{
				((Control)OUT0).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT0).BackColor = slateRed;
			}
			if (2 == (array2[0] & 2))
			{
				((Control)OUT1).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT1).BackColor = slateRed;
			}
			if (4 == (array2[0] & 4))
			{
				((Control)OUT2).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT2).BackColor = slateRed;
			}
			if (8 == (array2[0] & 8))
			{
				((Control)OUT3).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT3).BackColor = slateRed;
			}
			if (16 == (array2[0] & 0x10))
			{
				((Control)OUT4).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT4).BackColor = slateRed;
			}
			if (32 == (array2[0] & 0x20))
			{
				((Control)OUT5).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT5).BackColor = slateRed;
			}
			if (64 == (array2[0] & 0x40))
			{
				((Control)OUT6).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT6).BackColor = slateRed;
			}
			if (128 == (array2[0] & 0x80))
			{
				((Control)OUT7).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT7).BackColor = slateRed;
			}
			if (256 == (array2[0] & 0x100))
			{
				((Control)OUT8).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT8).BackColor = slateRed;
			}
			if (512 == (array2[0] & 0x200))
			{
				((Control)OUT9).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT9).BackColor = slateRed;
			}
			if (1024 == (array2[0] & 0x400))
			{
				((Control)OUT10).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT10).BackColor = slateRed;
			}
			if (2048 == (array2[0] & 0x800))
			{
				((Control)OUT11).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT11).BackColor = slateRed;
			}
			if (4096 == (array2[0] & 0x1000))
			{
				((Control)OUT12).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT12).BackColor = slateRed;
			}
			if (8192 == (array2[0] & 0x2000))
			{
				((Control)OUT13).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT13).BackColor = slateRed;
			}
			if (16384 == (array2[0] & 0x4000))
			{
				((Control)OUT14).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT14).BackColor = slateRed;
			}
			if (32768 == (array2[0] & 0x8000))
			{
				((Control)OUT15).BackColor = slateBlack;
			}
			else
			{
				((Control)OUT15).BackColor = slateRed;
			}
		}
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetIntInputState(0, array2))
		{
			if (1 == (array2[0] & 1))
			{
				((Control)Z_Axis0).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis0).BackColor = slateRed;
			}
			if (2 == (array2[0] & 2))
			{
				((Control)Z_Axis1).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis1).BackColor = slateRed;
			}
			if (4 == (array2[0] & 4))
			{
				((Control)Z_Axis2).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis2).BackColor = slateRed;
			}
			if (8 == (array2[0] & 8))
			{
				((Control)Z_Axis3).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis3).BackColor = slateRed;
			}
			if (16 == (array2[0] & 0x10))
			{
				((Control)Z_Axis4).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis4).BackColor = slateRed;
			}
			if (32 == (array2[0] & 0x20))
			{
				((Control)Z_Axis5).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis5).BackColor = slateRed;
			}
			if (256 == (array2[0] & 0x100))
			{
				((Control)Z_Axis8).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis8).BackColor = slateRed;
			}
			if (512 == (array2[0] & 0x200))
			{
				((Control)Z_Axis9).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis9).BackColor = slateRed;
			}
			if (1024 == (array2[0] & 0x400))
			{
				((Control)Z_Axis10).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis10).BackColor = slateRed;
			}
			if (2048 == (array2[0] & 0x800))
			{
				((Control)Z_Axis11).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis11).BackColor = slateRed;
			}
			if (4096 == (array2[0] & 0x1000))
			{
				((Control)Z_Axis12).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis12).BackColor = slateRed;
			}
			if (8192 == (array2[0] & 0x2000))
			{
				((Control)Z_Axis13).BackColor = slateBlack;
			}
			else
			{
				((Control)Z_Axis13).BackColor = slateRed;
			}
		}
	}

	private void SetJoyStickEnable(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		CheckBox val = (CheckBox)sender;
		byte b = byte.MaxValue;
		byte b2 = 0;
		if (((Control)val).Name == ((Control)JoystickX).Name)
		{
			b = 0;
		}
		else if (((Control)val).Name == ((Control)JoystickY).Name)
		{
			b = 1;
		}
		else if (((Control)val).Name == ((Control)JoystickZ).Name)
		{
			b = 2;
		}
		else if (((Control)val).Name == ((Control)JoystickT).Name)
		{
			b = 3;
		}
		else if (((Control)val).Name == ((Control)JoystickB).Name)
		{
			b = 4;
		}
		else if (((Control)val).Name == ((Control)JoystickC).Name)
		{
			b = 5;
		}
		b2 = (byte)(val.Checked ? 1 : 0);
		if (PMainFrm.FunRes_OK != PMainFrm.Mccard.MoCtrCard_SetJoyStickEnable(b, b2))
		{
			return;
		}
		if (1 == b2)
		{
			if (b == 0)
			{
				((Control)VValX).BackColor = slateGreen;
			}
			else if (1 == b)
			{
				((Control)VValY).BackColor = slateGreen;
			}
			else if (2 == b)
			{
				((Control)VValZ).BackColor = slateGreen;
			}
			else if (3 == b)
			{
				((Control)VValT).BackColor = slateGreen;
			}
			else if (4 == b)
			{
				((Control)VValB).BackColor = slateGreen;
			}
			else if (5 == b)
			{
				((Control)VValC).BackColor = slateGreen;
			}
			((Control)val).BackColor = slateGreen;
		}
		else if (b2 == 0)
		{
			if (b == 0)
			{
				((Control)VValX).BackColor = slateRed;
			}
			else if (1 == b)
			{
				((Control)VValY).BackColor = slateRed;
			}
			else if (2 == b)
			{
				((Control)VValZ).BackColor = slateRed;
			}
			else if (3 == b)
			{
				((Control)VValT).BackColor = slateRed;
			}
			else if (4 == b)
			{
				((Control)VValB).BackColor = slateRed;
			}
			else if (5 == b)
			{
				((Control)VValC).BackColor = slateRed;
			}
			((Control)val).BackColor = slateRed;
		}
	}

	private void JoystickZeroScroll_ValueChanged(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.CommLinkEnable && ParaIndxAdjustZeroScop > 0 && PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(ParaIndxAdjustZeroScop / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndxAdjustZeroScop % PMainFrm.Mccard.ParaNumPerAxis), (float)((ScrollBar)JoystickZeroScroll).Value))
		{
			PMainFrm.SysPara[ParaIndxAdjustZeroScop] = ((ScrollBar)JoystickZeroScroll).Value;
			((Control)lbZeroAera).Text = string.Format(GlobalData.GlobalLanguage.TestIOFrm_TCPCloseSucceed, 1.65 - (double)(PMainFrm.SysPara[ParaIndxAdjustZeroScop] / (float)gbAnalogScopeValue) * 3.3, 1.65 + (double)(PMainFrm.SysPara[ParaIndxAdjustZeroScop] / (float)gbAnalogScopeValue) * 3.3);
		}
	}

	private void bnSaveParaToMCU_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.CommLinkEnable)
		{
			PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM();
		}
	}

	private void PicBoxOut_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between I4 and Unknown
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Invalid comparison between I4 and Unknown
		PictureBox val = (PictureBox)sender;
		if (1048576 == (int)e.Button)
		{
			byte b = byte.MaxValue;
			if (((Control)OUT0).Name == ((Control)val).Name)
			{
				b = 0;
			}
			else if (((Control)OUT1).Name == ((Control)val).Name)
			{
				b = 1;
			}
			else if (((Control)OUT2).Name == ((Control)val).Name)
			{
				b = 2;
			}
			else if (((Control)OUT3).Name == ((Control)val).Name)
			{
				b = 3;
			}
			else if (((Control)OUT4).Name == ((Control)val).Name)
			{
				b = 4;
			}
			else if (((Control)OUT5).Name == ((Control)val).Name)
			{
				b = 5;
			}
			else if (((Control)OUT6).Name == ((Control)val).Name)
			{
				b = 6;
			}
			else if (((Control)OUT7).Name == ((Control)val).Name)
			{
				b = 7;
			}
			else if (((Control)OUT8).Name == ((Control)val).Name)
			{
				b = 8;
			}
			else if (((Control)OUT9).Name == ((Control)val).Name)
			{
				b = 9;
			}
			else if (((Control)OUT10).Name == ((Control)val).Name)
			{
				b = 10;
			}
			else if (((Control)OUT11).Name == ((Control)val).Name)
			{
				b = 11;
			}
			else if (((Control)OUT12).Name == ((Control)val).Name)
			{
				b = 12;
			}
			else if (((Control)OUT13).Name == ((Control)val).Name)
			{
				b = 13;
			}
			else if (((Control)OUT14).Name == ((Control)val).Name)
			{
				b = 14;
			}
			else if (((Control)OUT15).Name == ((Control)val).Name)
			{
				b = 15;
			}
			if (byte.MaxValue != b && PMainFrm.Mccard.CommLinkEnable)
			{
				PMainFrm.Mccard.MoCtrCard_SetOutput(b, OutputVal: false);
			}
			ushort num = ushort.MaxValue;
			if (((Control)Z_Axis0).Name == ((Control)val).Name)
			{
				num = 0;
			}
			else if (((Control)Z_Axis1).Name == ((Control)val).Name)
			{
				num = 1;
			}
			else if (((Control)Z_Axis2).Name == ((Control)val).Name)
			{
				num = 2;
			}
			else if (((Control)Z_Axis3).Name == ((Control)val).Name)
			{
				num = 3;
			}
			else if (((Control)Z_Axis4).Name == ((Control)val).Name)
			{
				num = 4;
			}
			else if (((Control)Z_Axis5).Name == ((Control)val).Name)
			{
				num = 5;
			}
			if (ushort.MaxValue != num && PMainFrm.Mccard.CommLinkEnable)
			{
				PMainFrm.Mccard.MoCtrCard_RstZ(b);
			}
		}
		else if (2097152 == (int)e.Button)
		{
			byte outputIndex = 0;
			if (((Control)OUT0).Name == ((Control)val).Name)
			{
				outputIndex = 0;
			}
			else if (((Control)OUT1).Name == ((Control)val).Name)
			{
				outputIndex = 1;
			}
			else if (((Control)OUT2).Name == ((Control)val).Name)
			{
				outputIndex = 2;
			}
			else if (((Control)OUT3).Name == ((Control)val).Name)
			{
				outputIndex = 3;
			}
			else if (((Control)OUT4).Name == ((Control)val).Name)
			{
				outputIndex = 4;
			}
			else if (((Control)OUT5).Name == ((Control)val).Name)
			{
				outputIndex = 5;
			}
			else if (((Control)OUT6).Name == ((Control)val).Name)
			{
				outputIndex = 6;
			}
			else if (((Control)OUT7).Name == ((Control)val).Name)
			{
				outputIndex = 7;
			}
			else if (((Control)OUT8).Name == ((Control)val).Name)
			{
				outputIndex = 8;
			}
			else if (((Control)OUT9).Name == ((Control)val).Name)
			{
				outputIndex = 9;
			}
			else if (((Control)OUT10).Name == ((Control)val).Name)
			{
				outputIndex = 10;
			}
			else if (((Control)OUT11).Name == ((Control)val).Name)
			{
				outputIndex = 11;
			}
			else if (((Control)OUT12).Name == ((Control)val).Name)
			{
				outputIndex = 12;
			}
			else if (((Control)OUT13).Name == ((Control)val).Name)
			{
				outputIndex = 13;
			}
			else if (((Control)OUT14).Name == ((Control)val).Name)
			{
				outputIndex = 14;
			}
			else if (((Control)OUT15).Name == ((Control)val).Name)
			{
				outputIndex = 15;
			}
			if (PMainFrm.Mccard.CommLinkEnable)
			{
				PMainFrm.Mccard.MoCtrCard_SetOutput(outputIndex, OutputVal: true);
			}
		}
	}

	private void SelectAxisAdjustAnaZeroScop(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		RadioButton val = (RadioButton)sender;
		byte constPara_AnaZeroScope = ConstPara_AnaZeroScope;
		if (((Control)rbAanScopAdjEnX).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisX;
		}
		else if (((Control)rbAanScopAdjEnY).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisY;
		}
		else if (((Control)rbAanScopAdjEnZ).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisZ;
		}
		else if (((Control)rbAanScopAdjEnA).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisA;
		}
		else if (((Control)rbAanScopAdjEnB).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisB;
		}
		else if (((Control)rbAanScopAdjEnC).Name == ((Control)val).Name)
		{
			constPara_AnaZeroScope += ConstPara_AxisC;
		}
		else
		{
			constPara_AnaZeroScope = 0;
			ParaIndxAdjustZeroScop = 0;
		}
		if (constPara_AnaZeroScope > 0 && val.Checked)
		{
			ParaIndxAdjustZeroScop = constPara_AnaZeroScope;
			PMainFrm.SysPara[ParaIndxAdjustZeroScop] = ((PMainFrm.SysPara[ParaIndxAdjustZeroScop] > (float)gbAnalogZeroValue) ? ((float)gbAnalogZeroValue) : PMainFrm.SysPara[ParaIndxAdjustZeroScop]);
			((ScrollBar)JoystickZeroScroll).Value = (int)PMainFrm.SysPara[ParaIndxAdjustZeroScop];
			((Control)lbZeroAera).Text = string.Format(GlobalData.GlobalLanguage.TestIOFrm_TCPCloseSucceed, 1.65 - (double)(PMainFrm.SysPara[ParaIndxAdjustZeroScop] / (float)gbAnalogScopeValue) * 3.3, 1.65 + (double)(PMainFrm.SysPara[ParaIndxAdjustZeroScop] / (float)gbAnalogScopeValue) * 3.3);
		}
	}

	private void rbJoyStick_CheckedChanged(object sender, EventArgs e)
	{
		if (rbJoyStick.Checked)
		{
			gbAnalogScopeValue = 4096;
			gbAnalogZeroValue = 2048;
			((ScrollBar)JoystickZeroScroll).Maximum = gbAnalogZeroValue;
		}
	}

	private void rbPS2Pad_CheckedChanged(object sender, EventArgs e)
	{
		if (rbPS2Pad.Checked)
		{
			gbAnalogScopeValue = 256;
			gbAnalogZeroValue = 128;
			((ScrollBar)JoystickZeroScroll).Maximum = gbAnalogZeroValue;
		}
	}

	private void TestIOFrm_FormClosed(object sender, FormClosedEventArgs e)
	{
		timer1.Enabled = false;
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
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Expected O, but got Unknown
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Expected O, but got Unknown
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Expected O, but got Unknown
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected O, but got Unknown
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Expected O, but got Unknown
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Expected O, but got Unknown
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Expected O, but got Unknown
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Expected O, but got Unknown
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Expected O, but got Unknown
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Expected O, but got Unknown
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Expected O, but got Unknown
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Expected O, but got Unknown
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Expected O, but got Unknown
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Expected O, but got Unknown
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Expected O, but got Unknown
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Expected O, but got Unknown
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Expected O, but got Unknown
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Expected O, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Expected O, but got Unknown
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Expected O, but got Unknown
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Expected O, but got Unknown
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Expected O, but got Unknown
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Expected O, but got Unknown
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected O, but got Unknown
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Expected O, but got Unknown
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Expected O, but got Unknown
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Expected O, but got Unknown
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Expected O, but got Unknown
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Expected O, but got Unknown
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Expected O, but got Unknown
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Expected O, but got Unknown
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Expected O, but got Unknown
		//IL_1899: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a3: Expected O, but got Unknown
		//IL_18dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e7: Expected O, but got Unknown
		//IL_1921: Unknown result type (might be due to invalid IL or missing references)
		//IL_192b: Expected O, but got Unknown
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_196f: Expected O, but got Unknown
		//IL_19a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b3: Expected O, but got Unknown
		//IL_19ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f7: Expected O, but got Unknown
		//IL_1a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3b: Expected O, but got Unknown
		//IL_1a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7f: Expected O, but got Unknown
		//IL_1f33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3d: Expected O, but got Unknown
		//IL_1fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fae: Expected O, but got Unknown
		//IL_2015: Unknown result type (might be due to invalid IL or missing references)
		//IL_201f: Expected O, but got Unknown
		//IL_2059: Unknown result type (might be due to invalid IL or missing references)
		//IL_2063: Expected O, but got Unknown
		//IL_20ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d4: Expected O, but got Unknown
		//IL_210e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2118: Expected O, but got Unknown
		//IL_217f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2189: Expected O, but got Unknown
		//IL_21c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cd: Expected O, but got Unknown
		//IL_2234: Unknown result type (might be due to invalid IL or missing references)
		//IL_223e: Expected O, but got Unknown
		//IL_2278: Unknown result type (might be due to invalid IL or missing references)
		//IL_2282: Expected O, but got Unknown
		//IL_22e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_22f3: Expected O, but got Unknown
		//IL_232d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2337: Expected O, but got Unknown
		//IL_2371: Unknown result type (might be due to invalid IL or missing references)
		//IL_237b: Expected O, but got Unknown
		//IL_23e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ec: Expected O, but got Unknown
		//IL_25a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_25ac: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TestIOFrm));
		groupBox1 = new GroupBox();
		rbAanScopAdjEnC = new RadioButton();
		rbAanScopAdjEnB = new RadioButton();
		rbAanScopAdjEnA = new RadioButton();
		rbAanScopAdjEnZ = new RadioButton();
		rbAanScopAdjEnY = new RadioButton();
		rbAanScopAdjEnX = new RadioButton();
		label8 = new Label();
		VValC = new Label();
		VValB = new Label();
		JoystickC = new CheckBox();
		JoystickB = new CheckBox();
		bnSaveParaToMCU = new Button();
		JoystickZeroScroll = new HScrollBar();
		VValT = new Label();
		VValZ = new Label();
		lbZeroAera = new Label();
		VValY = new Label();
		VValX = new Label();
		JoystickT = new CheckBox();
		JoystickZ = new CheckBox();
		JoystickY = new CheckBox();
		JoystickX = new CheckBox();
		groupBox2 = new GroupBox();
		label9 = new Label();
		label10 = new Label();
		Z_Axis15 = new PictureBox();
		Z_Axis14 = new PictureBox();
		Z_Axis13 = new PictureBox();
		Z_Axis12 = new PictureBox();
		Z_Axis11 = new PictureBox();
		Z_Axis10 = new PictureBox();
		Z_Axis9 = new PictureBox();
		Z_Axis8 = new PictureBox();
		Z_Axis7 = new PictureBox();
		Z_Axis6 = new PictureBox();
		label7 = new Label();
		label4 = new Label();
		label3 = new Label();
		label6 = new Label();
		label2 = new Label();
		label5 = new Label();
		label1 = new Label();
		OUT15 = new PictureBox();
		OUT14 = new PictureBox();
		OUT13 = new PictureBox();
		OUT12 = new PictureBox();
		OUT11 = new PictureBox();
		OUT10 = new PictureBox();
		OUT9 = new PictureBox();
		OUT8 = new PictureBox();
		IN15 = new PictureBox();
		IN14 = new PictureBox();
		IN13 = new PictureBox();
		IN12 = new PictureBox();
		IN11 = new PictureBox();
		IN10 = new PictureBox();
		IN9 = new PictureBox();
		IN8 = new PictureBox();
		IN23 = new PictureBox();
		IN22 = new PictureBox();
		IN21 = new PictureBox();
		IN20 = new PictureBox();
		IN19 = new PictureBox();
		IN18 = new PictureBox();
		IN17 = new PictureBox();
		IN16 = new PictureBox();
		IN31 = new PictureBox();
		IN30 = new PictureBox();
		IN29 = new PictureBox();
		IN28 = new PictureBox();
		IN27 = new PictureBox();
		IN26 = new PictureBox();
		IN25 = new PictureBox();
		IN24 = new PictureBox();
		lbOutputStt = new Label();
		lbInputStt = new Label();
		OUT7 = new PictureBox();
		IN7 = new PictureBox();
		OUT6 = new PictureBox();
		IN6 = new PictureBox();
		Z_Axis5 = new PictureBox();
		OUT5 = new PictureBox();
		IN5 = new PictureBox();
		Z_Axis4 = new PictureBox();
		OUT4 = new PictureBox();
		IN4 = new PictureBox();
		Z_Axis3 = new PictureBox();
		OUT3 = new PictureBox();
		IN3 = new PictureBox();
		Z_Axis2 = new PictureBox();
		OUT2 = new PictureBox();
		IN2 = new PictureBox();
		Z_Axis1 = new PictureBox();
		OUT1 = new PictureBox();
		Z_Axis0 = new PictureBox();
		IN1 = new PictureBox();
		OUT0 = new PictureBox();
		IN0 = new PictureBox();
		timer1 = new Timer(components);
		groupBox3 = new GroupBox();
		rbPS2Pad = new RadioButton();
		rbJoyStick = new RadioButton();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((ISupportInitialize)Z_Axis15).BeginInit();
		((ISupportInitialize)Z_Axis14).BeginInit();
		((ISupportInitialize)Z_Axis13).BeginInit();
		((ISupportInitialize)Z_Axis12).BeginInit();
		((ISupportInitialize)Z_Axis11).BeginInit();
		((ISupportInitialize)Z_Axis10).BeginInit();
		((ISupportInitialize)Z_Axis9).BeginInit();
		((ISupportInitialize)Z_Axis8).BeginInit();
		((ISupportInitialize)Z_Axis7).BeginInit();
		((ISupportInitialize)Z_Axis6).BeginInit();
		((ISupportInitialize)OUT15).BeginInit();
		((ISupportInitialize)OUT14).BeginInit();
		((ISupportInitialize)OUT13).BeginInit();
		((ISupportInitialize)OUT12).BeginInit();
		((ISupportInitialize)OUT11).BeginInit();
		((ISupportInitialize)OUT10).BeginInit();
		((ISupportInitialize)OUT9).BeginInit();
		((ISupportInitialize)OUT8).BeginInit();
		((ISupportInitialize)IN15).BeginInit();
		((ISupportInitialize)IN14).BeginInit();
		((ISupportInitialize)IN13).BeginInit();
		((ISupportInitialize)IN12).BeginInit();
		((ISupportInitialize)IN11).BeginInit();
		((ISupportInitialize)IN10).BeginInit();
		((ISupportInitialize)IN9).BeginInit();
		((ISupportInitialize)IN8).BeginInit();
		((ISupportInitialize)IN23).BeginInit();
		((ISupportInitialize)IN22).BeginInit();
		((ISupportInitialize)IN21).BeginInit();
		((ISupportInitialize)IN20).BeginInit();
		((ISupportInitialize)IN19).BeginInit();
		((ISupportInitialize)IN18).BeginInit();
		((ISupportInitialize)IN17).BeginInit();
		((ISupportInitialize)IN16).BeginInit();
		((ISupportInitialize)IN31).BeginInit();
		((ISupportInitialize)IN30).BeginInit();
		((ISupportInitialize)IN29).BeginInit();
		((ISupportInitialize)IN28).BeginInit();
		((ISupportInitialize)IN27).BeginInit();
		((ISupportInitialize)IN26).BeginInit();
		((ISupportInitialize)IN25).BeginInit();
		((ISupportInitialize)IN24).BeginInit();
		((ISupportInitialize)OUT7).BeginInit();
		((ISupportInitialize)IN7).BeginInit();
		((ISupportInitialize)OUT6).BeginInit();
		((ISupportInitialize)IN6).BeginInit();
		((ISupportInitialize)Z_Axis5).BeginInit();
		((ISupportInitialize)OUT5).BeginInit();
		((ISupportInitialize)IN5).BeginInit();
		((ISupportInitialize)Z_Axis4).BeginInit();
		((ISupportInitialize)OUT4).BeginInit();
		((ISupportInitialize)IN4).BeginInit();
		((ISupportInitialize)Z_Axis3).BeginInit();
		((ISupportInitialize)OUT3).BeginInit();
		((ISupportInitialize)IN3).BeginInit();
		((ISupportInitialize)Z_Axis2).BeginInit();
		((ISupportInitialize)OUT2).BeginInit();
		((ISupportInitialize)IN2).BeginInit();
		((ISupportInitialize)Z_Axis1).BeginInit();
		((ISupportInitialize)OUT1).BeginInit();
		((ISupportInitialize)Z_Axis0).BeginInit();
		((ISupportInitialize)IN1).BeginInit();
		((ISupportInitialize)OUT0).BeginInit();
		((ISupportInitialize)IN0).BeginInit();
		((Control)groupBox3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnC);
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnB);
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnA);
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnZ);
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnY);
		((Control)groupBox1).Controls.Add((Control)(object)rbAanScopAdjEnX);
		((Control)groupBox1).Controls.Add((Control)(object)label8);
		((Control)groupBox1).Controls.Add((Control)(object)VValC);
		((Control)groupBox1).Controls.Add((Control)(object)VValB);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickC);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickB);
		((Control)groupBox1).Controls.Add((Control)(object)bnSaveParaToMCU);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickZeroScroll);
		((Control)groupBox1).Controls.Add((Control)(object)VValT);
		((Control)groupBox1).Controls.Add((Control)(object)VValZ);
		((Control)groupBox1).Controls.Add((Control)(object)lbZeroAera);
		((Control)groupBox1).Controls.Add((Control)(object)VValY);
		((Control)groupBox1).Controls.Add((Control)(object)VValX);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickT);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickZ);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickY);
		((Control)groupBox1).Controls.Add((Control)(object)JoystickX);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(rbAanScopAdjEnC, "rbAanScopAdjEnC");
		((Control)rbAanScopAdjEnC).Name = "rbAanScopAdjEnC";
		rbAanScopAdjEnC.TabStop = true;
		((ButtonBase)rbAanScopAdjEnC).UseVisualStyleBackColor = true;
		rbAanScopAdjEnC.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(rbAanScopAdjEnB, "rbAanScopAdjEnB");
		((Control)rbAanScopAdjEnB).Name = "rbAanScopAdjEnB";
		rbAanScopAdjEnB.TabStop = true;
		((ButtonBase)rbAanScopAdjEnB).UseVisualStyleBackColor = true;
		rbAanScopAdjEnB.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(rbAanScopAdjEnA, "rbAanScopAdjEnA");
		((Control)rbAanScopAdjEnA).Name = "rbAanScopAdjEnA";
		rbAanScopAdjEnA.TabStop = true;
		((ButtonBase)rbAanScopAdjEnA).UseVisualStyleBackColor = true;
		rbAanScopAdjEnA.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(rbAanScopAdjEnZ, "rbAanScopAdjEnZ");
		((Control)rbAanScopAdjEnZ).Name = "rbAanScopAdjEnZ";
		rbAanScopAdjEnZ.TabStop = true;
		((ButtonBase)rbAanScopAdjEnZ).UseVisualStyleBackColor = true;
		rbAanScopAdjEnZ.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(rbAanScopAdjEnY, "rbAanScopAdjEnY");
		((Control)rbAanScopAdjEnY).Name = "rbAanScopAdjEnY";
		rbAanScopAdjEnY.TabStop = true;
		((ButtonBase)rbAanScopAdjEnY).UseVisualStyleBackColor = true;
		rbAanScopAdjEnY.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(rbAanScopAdjEnX, "rbAanScopAdjEnX");
		((Control)rbAanScopAdjEnX).Name = "rbAanScopAdjEnX";
		rbAanScopAdjEnX.TabStop = true;
		((ButtonBase)rbAanScopAdjEnX).UseVisualStyleBackColor = true;
		rbAanScopAdjEnX.CheckedChanged += SelectAxisAdjustAnaZeroScop;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(VValC, "VValC");
		VValC.BorderStyle = (BorderStyle)1;
		((Control)VValC).Name = "VValC";
		componentResourceManager.ApplyResources(VValB, "VValB");
		VValB.BorderStyle = (BorderStyle)1;
		((Control)VValB).Name = "VValB";
		componentResourceManager.ApplyResources(JoystickC, "JoystickC");
		((Control)JoystickC).Name = "JoystickC";
		((ButtonBase)JoystickC).UseVisualStyleBackColor = true;
		JoystickC.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(JoystickB, "JoystickB");
		((Control)JoystickB).Name = "JoystickB";
		((ButtonBase)JoystickB).UseVisualStyleBackColor = true;
		JoystickB.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(bnSaveParaToMCU, "bnSaveParaToMCU");
		((Control)bnSaveParaToMCU).Name = "bnSaveParaToMCU";
		((ButtonBase)bnSaveParaToMCU).UseVisualStyleBackColor = true;
		((Control)bnSaveParaToMCU).Click += bnSaveParaToMCU_Click;
		componentResourceManager.ApplyResources(JoystickZeroScroll, "JoystickZeroScroll");
		((Control)JoystickZeroScroll).Name = "JoystickZeroScroll";
		((ScrollBar)JoystickZeroScroll).ValueChanged += JoystickZeroScroll_ValueChanged;
		componentResourceManager.ApplyResources(VValT, "VValT");
		VValT.BorderStyle = (BorderStyle)1;
		((Control)VValT).Name = "VValT";
		componentResourceManager.ApplyResources(VValZ, "VValZ");
		VValZ.BorderStyle = (BorderStyle)1;
		((Control)VValZ).Name = "VValZ";
		componentResourceManager.ApplyResources(lbZeroAera, "lbZeroAera");
		lbZeroAera.BorderStyle = (BorderStyle)2;
		((Control)lbZeroAera).Name = "lbZeroAera";
		componentResourceManager.ApplyResources(VValY, "VValY");
		VValY.BorderStyle = (BorderStyle)1;
		((Control)VValY).Name = "VValY";
		componentResourceManager.ApplyResources(VValX, "VValX");
		VValX.BorderStyle = (BorderStyle)1;
		((Control)VValX).Name = "VValX";
		componentResourceManager.ApplyResources(JoystickT, "JoystickT");
		((Control)JoystickT).Name = "JoystickT";
		((ButtonBase)JoystickT).UseVisualStyleBackColor = true;
		JoystickT.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(JoystickZ, "JoystickZ");
		((Control)JoystickZ).Name = "JoystickZ";
		((ButtonBase)JoystickZ).UseVisualStyleBackColor = true;
		JoystickZ.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(JoystickY, "JoystickY");
		((Control)JoystickY).Name = "JoystickY";
		((ButtonBase)JoystickY).UseVisualStyleBackColor = true;
		JoystickY.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(JoystickX, "JoystickX");
		((Control)JoystickX).Name = "JoystickX";
		((ButtonBase)JoystickX).UseVisualStyleBackColor = true;
		JoystickX.CheckedChanged += SetJoyStickEnable;
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)label9);
		((Control)groupBox2).Controls.Add((Control)(object)label10);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis15);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis14);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis13);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis12);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis11);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis10);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis9);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis8);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis7);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis6);
		((Control)groupBox2).Controls.Add((Control)(object)label7);
		((Control)groupBox2).Controls.Add((Control)(object)label4);
		((Control)groupBox2).Controls.Add((Control)(object)label3);
		((Control)groupBox2).Controls.Add((Control)(object)label6);
		((Control)groupBox2).Controls.Add((Control)(object)label2);
		((Control)groupBox2).Controls.Add((Control)(object)label5);
		((Control)groupBox2).Controls.Add((Control)(object)label1);
		((Control)groupBox2).Controls.Add((Control)(object)OUT15);
		((Control)groupBox2).Controls.Add((Control)(object)OUT14);
		((Control)groupBox2).Controls.Add((Control)(object)OUT13);
		((Control)groupBox2).Controls.Add((Control)(object)OUT12);
		((Control)groupBox2).Controls.Add((Control)(object)OUT11);
		((Control)groupBox2).Controls.Add((Control)(object)OUT10);
		((Control)groupBox2).Controls.Add((Control)(object)OUT9);
		((Control)groupBox2).Controls.Add((Control)(object)OUT8);
		((Control)groupBox2).Controls.Add((Control)(object)IN15);
		((Control)groupBox2).Controls.Add((Control)(object)IN14);
		((Control)groupBox2).Controls.Add((Control)(object)IN13);
		((Control)groupBox2).Controls.Add((Control)(object)IN12);
		((Control)groupBox2).Controls.Add((Control)(object)IN11);
		((Control)groupBox2).Controls.Add((Control)(object)IN10);
		((Control)groupBox2).Controls.Add((Control)(object)IN9);
		((Control)groupBox2).Controls.Add((Control)(object)IN8);
		((Control)groupBox2).Controls.Add((Control)(object)IN23);
		((Control)groupBox2).Controls.Add((Control)(object)IN22);
		((Control)groupBox2).Controls.Add((Control)(object)IN21);
		((Control)groupBox2).Controls.Add((Control)(object)IN20);
		((Control)groupBox2).Controls.Add((Control)(object)IN19);
		((Control)groupBox2).Controls.Add((Control)(object)IN18);
		((Control)groupBox2).Controls.Add((Control)(object)IN17);
		((Control)groupBox2).Controls.Add((Control)(object)IN16);
		((Control)groupBox2).Controls.Add((Control)(object)IN31);
		((Control)groupBox2).Controls.Add((Control)(object)IN30);
		((Control)groupBox2).Controls.Add((Control)(object)IN29);
		((Control)groupBox2).Controls.Add((Control)(object)IN28);
		((Control)groupBox2).Controls.Add((Control)(object)IN27);
		((Control)groupBox2).Controls.Add((Control)(object)IN26);
		((Control)groupBox2).Controls.Add((Control)(object)IN25);
		((Control)groupBox2).Controls.Add((Control)(object)IN24);
		((Control)groupBox2).Controls.Add((Control)(object)lbOutputStt);
		((Control)groupBox2).Controls.Add((Control)(object)lbInputStt);
		((Control)groupBox2).Controls.Add((Control)(object)OUT7);
		((Control)groupBox2).Controls.Add((Control)(object)IN7);
		((Control)groupBox2).Controls.Add((Control)(object)OUT6);
		((Control)groupBox2).Controls.Add((Control)(object)IN6);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis5);
		((Control)groupBox2).Controls.Add((Control)(object)OUT5);
		((Control)groupBox2).Controls.Add((Control)(object)IN5);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis4);
		((Control)groupBox2).Controls.Add((Control)(object)OUT4);
		((Control)groupBox2).Controls.Add((Control)(object)IN4);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis3);
		((Control)groupBox2).Controls.Add((Control)(object)OUT3);
		((Control)groupBox2).Controls.Add((Control)(object)IN3);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis2);
		((Control)groupBox2).Controls.Add((Control)(object)OUT2);
		((Control)groupBox2).Controls.Add((Control)(object)IN2);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis1);
		((Control)groupBox2).Controls.Add((Control)(object)OUT1);
		((Control)groupBox2).Controls.Add((Control)(object)Z_Axis0);
		((Control)groupBox2).Controls.Add((Control)(object)IN1);
		((Control)groupBox2).Controls.Add((Control)(object)OUT0);
		((Control)groupBox2).Controls.Add((Control)(object)IN0);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(Z_Axis15, "Z_Axis15");
		((Control)Z_Axis15).Name = "Z_Axis15";
		Z_Axis15.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis14, "Z_Axis14");
		((Control)Z_Axis14).Name = "Z_Axis14";
		Z_Axis14.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis13, "Z_Axis13");
		((Control)Z_Axis13).Name = "Z_Axis13";
		Z_Axis13.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis12, "Z_Axis12");
		((Control)Z_Axis12).Name = "Z_Axis12";
		Z_Axis12.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis11, "Z_Axis11");
		((Control)Z_Axis11).Name = "Z_Axis11";
		Z_Axis11.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis10, "Z_Axis10");
		((Control)Z_Axis10).Name = "Z_Axis10";
		Z_Axis10.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis9, "Z_Axis9");
		((Control)Z_Axis9).Name = "Z_Axis9";
		Z_Axis9.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis8, "Z_Axis8");
		((Control)Z_Axis8).Name = "Z_Axis8";
		Z_Axis8.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis7, "Z_Axis7");
		((Control)Z_Axis7).Name = "Z_Axis7";
		Z_Axis7.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis6, "Z_Axis6");
		((Control)Z_Axis6).Name = "Z_Axis6";
		Z_Axis6.TabStop = false;
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(OUT15, "OUT15");
		((Control)OUT15).Name = "OUT15";
		OUT15.TabStop = false;
		((Control)OUT15).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT14, "OUT14");
		((Control)OUT14).Name = "OUT14";
		OUT14.TabStop = false;
		((Control)OUT14).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT13, "OUT13");
		((Control)OUT13).Name = "OUT13";
		OUT13.TabStop = false;
		((Control)OUT13).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT12, "OUT12");
		((Control)OUT12).Name = "OUT12";
		OUT12.TabStop = false;
		((Control)OUT12).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT11, "OUT11");
		((Control)OUT11).Name = "OUT11";
		OUT11.TabStop = false;
		((Control)OUT11).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT10, "OUT10");
		((Control)OUT10).Name = "OUT10";
		OUT10.TabStop = false;
		((Control)OUT10).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT9, "OUT9");
		((Control)OUT9).Name = "OUT9";
		OUT9.TabStop = false;
		((Control)OUT9).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT8, "OUT8");
		((Control)OUT8).Name = "OUT8";
		OUT8.TabStop = false;
		((Control)OUT8).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN15, "IN15");
		((Control)IN15).Name = "IN15";
		IN15.TabStop = false;
		componentResourceManager.ApplyResources(IN14, "IN14");
		((Control)IN14).Name = "IN14";
		IN14.TabStop = false;
		componentResourceManager.ApplyResources(IN13, "IN13");
		((Control)IN13).Name = "IN13";
		IN13.TabStop = false;
		componentResourceManager.ApplyResources(IN12, "IN12");
		((Control)IN12).Name = "IN12";
		IN12.TabStop = false;
		componentResourceManager.ApplyResources(IN11, "IN11");
		((Control)IN11).Name = "IN11";
		IN11.TabStop = false;
		componentResourceManager.ApplyResources(IN10, "IN10");
		((Control)IN10).Name = "IN10";
		IN10.TabStop = false;
		componentResourceManager.ApplyResources(IN9, "IN9");
		((Control)IN9).Name = "IN9";
		IN9.TabStop = false;
		componentResourceManager.ApplyResources(IN8, "IN8");
		((Control)IN8).Name = "IN8";
		IN8.TabStop = false;
		componentResourceManager.ApplyResources(IN23, "IN23");
		((Control)IN23).Name = "IN23";
		IN23.TabStop = false;
		componentResourceManager.ApplyResources(IN22, "IN22");
		((Control)IN22).Name = "IN22";
		IN22.TabStop = false;
		componentResourceManager.ApplyResources(IN21, "IN21");
		((Control)IN21).Name = "IN21";
		IN21.TabStop = false;
		componentResourceManager.ApplyResources(IN20, "IN20");
		((Control)IN20).Name = "IN20";
		IN20.TabStop = false;
		componentResourceManager.ApplyResources(IN19, "IN19");
		((Control)IN19).Name = "IN19";
		IN19.TabStop = false;
		componentResourceManager.ApplyResources(IN18, "IN18");
		((Control)IN18).Name = "IN18";
		IN18.TabStop = false;
		componentResourceManager.ApplyResources(IN17, "IN17");
		((Control)IN17).Name = "IN17";
		IN17.TabStop = false;
		componentResourceManager.ApplyResources(IN16, "IN16");
		((Control)IN16).Name = "IN16";
		IN16.TabStop = false;
		componentResourceManager.ApplyResources(IN31, "IN31");
		((Control)IN31).Name = "IN31";
		IN31.TabStop = false;
		componentResourceManager.ApplyResources(IN30, "IN30");
		((Control)IN30).Name = "IN30";
		IN30.TabStop = false;
		componentResourceManager.ApplyResources(IN29, "IN29");
		((Control)IN29).Name = "IN29";
		IN29.TabStop = false;
		componentResourceManager.ApplyResources(IN28, "IN28");
		((Control)IN28).Name = "IN28";
		IN28.TabStop = false;
		componentResourceManager.ApplyResources(IN27, "IN27");
		((Control)IN27).Name = "IN27";
		IN27.TabStop = false;
		componentResourceManager.ApplyResources(IN26, "IN26");
		((Control)IN26).Name = "IN26";
		IN26.TabStop = false;
		componentResourceManager.ApplyResources(IN25, "IN25");
		((Control)IN25).Name = "IN25";
		IN25.TabStop = false;
		componentResourceManager.ApplyResources(IN24, "IN24");
		((Control)IN24).Name = "IN24";
		IN24.TabStop = false;
		componentResourceManager.ApplyResources(lbOutputStt, "lbOutputStt");
		((Control)lbOutputStt).Name = "lbOutputStt";
		componentResourceManager.ApplyResources(lbInputStt, "lbInputStt");
		((Control)lbInputStt).Name = "lbInputStt";
		componentResourceManager.ApplyResources(OUT7, "OUT7");
		((Control)OUT7).Name = "OUT7";
		OUT7.TabStop = false;
		((Control)OUT7).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN7, "IN7");
		((Control)IN7).Name = "IN7";
		IN7.TabStop = false;
		componentResourceManager.ApplyResources(OUT6, "OUT6");
		((Control)OUT6).Name = "OUT6";
		OUT6.TabStop = false;
		((Control)OUT6).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN6, "IN6");
		((Control)IN6).Name = "IN6";
		IN6.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis5, "Z_Axis5");
		((Control)Z_Axis5).Name = "Z_Axis5";
		Z_Axis5.TabStop = false;
		((Control)Z_Axis5).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT5, "OUT5");
		((Control)OUT5).Name = "OUT5";
		OUT5.TabStop = false;
		((Control)OUT5).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN5, "IN5");
		((Control)IN5).Name = "IN5";
		IN5.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis4, "Z_Axis4");
		((Control)Z_Axis4).Name = "Z_Axis4";
		Z_Axis4.TabStop = false;
		((Control)Z_Axis4).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT4, "OUT4");
		((Control)OUT4).Name = "OUT4";
		OUT4.TabStop = false;
		((Control)OUT4).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN4, "IN4");
		((Control)IN4).Name = "IN4";
		IN4.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis3, "Z_Axis3");
		((Control)Z_Axis3).Name = "Z_Axis3";
		Z_Axis3.TabStop = false;
		((Control)Z_Axis3).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT3, "OUT3");
		((Control)OUT3).Name = "OUT3";
		OUT3.TabStop = false;
		((Control)OUT3).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN3, "IN3");
		((Control)IN3).Name = "IN3";
		IN3.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis2, "Z_Axis2");
		((Control)Z_Axis2).Name = "Z_Axis2";
		Z_Axis2.TabStop = false;
		((Control)Z_Axis2).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT2, "OUT2");
		((Control)OUT2).Name = "OUT2";
		OUT2.TabStop = false;
		((Control)OUT2).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN2, "IN2");
		((Control)IN2).Name = "IN2";
		IN2.TabStop = false;
		componentResourceManager.ApplyResources(Z_Axis1, "Z_Axis1");
		((Control)Z_Axis1).Name = "Z_Axis1";
		Z_Axis1.TabStop = false;
		((Control)Z_Axis1).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(OUT1, "OUT1");
		((Control)OUT1).Name = "OUT1";
		OUT1.TabStop = false;
		((Control)OUT1).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(Z_Axis0, "Z_Axis0");
		((Control)Z_Axis0).Name = "Z_Axis0";
		Z_Axis0.TabStop = false;
		((Control)Z_Axis0).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN1, "IN1");
		((Control)IN1).Name = "IN1";
		IN1.TabStop = false;
		componentResourceManager.ApplyResources(OUT0, "OUT0");
		((Control)OUT0).Name = "OUT0";
		OUT0.TabStop = false;
		((Control)OUT0).MouseClick += new MouseEventHandler(PicBoxOut_MouseClick);
		componentResourceManager.ApplyResources(IN0, "IN0");
		((Control)IN0).Name = "IN0";
		IN0.TabStop = false;
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(groupBox3, "groupBox3");
		((Control)groupBox3).Controls.Add((Control)(object)rbPS2Pad);
		((Control)groupBox3).Controls.Add((Control)(object)rbJoyStick);
		((Control)groupBox3).Name = "groupBox3";
		groupBox3.TabStop = false;
		componentResourceManager.ApplyResources(rbPS2Pad, "rbPS2Pad");
		((Control)rbPS2Pad).Name = "rbPS2Pad";
		rbPS2Pad.TabStop = true;
		((ButtonBase)rbPS2Pad).UseVisualStyleBackColor = true;
		rbPS2Pad.CheckedChanged += rbPS2Pad_CheckedChanged;
		componentResourceManager.ApplyResources(rbJoyStick, "rbJoyStick");
		rbJoyStick.Checked = true;
		((Control)rbJoyStick).Name = "rbJoyStick";
		rbJoyStick.TabStop = true;
		((ButtonBase)rbJoyStick).UseVisualStyleBackColor = true;
		rbJoyStick.CheckedChanged += rbJoyStick_CheckedChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox3);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "TestIOFrm";
		((Form)this).FormClosed += new FormClosedEventHandler(TestIOFrm_FormClosed);
		((Form)this).Load += TestIOFrm_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((ISupportInitialize)Z_Axis15).EndInit();
		((ISupportInitialize)Z_Axis14).EndInit();
		((ISupportInitialize)Z_Axis13).EndInit();
		((ISupportInitialize)Z_Axis12).EndInit();
		((ISupportInitialize)Z_Axis11).EndInit();
		((ISupportInitialize)Z_Axis10).EndInit();
		((ISupportInitialize)Z_Axis9).EndInit();
		((ISupportInitialize)Z_Axis8).EndInit();
		((ISupportInitialize)Z_Axis7).EndInit();
		((ISupportInitialize)Z_Axis6).EndInit();
		((ISupportInitialize)OUT15).EndInit();
		((ISupportInitialize)OUT14).EndInit();
		((ISupportInitialize)OUT13).EndInit();
		((ISupportInitialize)OUT12).EndInit();
		((ISupportInitialize)OUT11).EndInit();
		((ISupportInitialize)OUT10).EndInit();
		((ISupportInitialize)OUT9).EndInit();
		((ISupportInitialize)OUT8).EndInit();
		((ISupportInitialize)IN15).EndInit();
		((ISupportInitialize)IN14).EndInit();
		((ISupportInitialize)IN13).EndInit();
		((ISupportInitialize)IN12).EndInit();
		((ISupportInitialize)IN11).EndInit();
		((ISupportInitialize)IN10).EndInit();
		((ISupportInitialize)IN9).EndInit();
		((ISupportInitialize)IN8).EndInit();
		((ISupportInitialize)IN23).EndInit();
		((ISupportInitialize)IN22).EndInit();
		((ISupportInitialize)IN21).EndInit();
		((ISupportInitialize)IN20).EndInit();
		((ISupportInitialize)IN19).EndInit();
		((ISupportInitialize)IN18).EndInit();
		((ISupportInitialize)IN17).EndInit();
		((ISupportInitialize)IN16).EndInit();
		((ISupportInitialize)IN31).EndInit();
		((ISupportInitialize)IN30).EndInit();
		((ISupportInitialize)IN29).EndInit();
		((ISupportInitialize)IN28).EndInit();
		((ISupportInitialize)IN27).EndInit();
		((ISupportInitialize)IN26).EndInit();
		((ISupportInitialize)IN25).EndInit();
		((ISupportInitialize)IN24).EndInit();
		((ISupportInitialize)OUT7).EndInit();
		((ISupportInitialize)IN7).EndInit();
		((ISupportInitialize)OUT6).EndInit();
		((ISupportInitialize)IN6).EndInit();
		((ISupportInitialize)Z_Axis5).EndInit();
		((ISupportInitialize)OUT5).EndInit();
		((ISupportInitialize)IN5).EndInit();
		((ISupportInitialize)Z_Axis4).EndInit();
		((ISupportInitialize)OUT4).EndInit();
		((ISupportInitialize)IN4).EndInit();
		((ISupportInitialize)Z_Axis3).EndInit();
		((ISupportInitialize)OUT3).EndInit();
		((ISupportInitialize)IN3).EndInit();
		((ISupportInitialize)Z_Axis2).EndInit();
		((ISupportInitialize)OUT2).EndInit();
		((ISupportInitialize)IN2).EndInit();
		((ISupportInitialize)Z_Axis1).EndInit();
		((ISupportInitialize)OUT1).EndInit();
		((ISupportInitialize)Z_Axis0).EndInit();
		((ISupportInitialize)IN1).EndInit();
		((ISupportInitialize)OUT0).EndInit();
		((ISupportInitialize)IN0).EndInit();
		((Control)groupBox3).ResumeLayout(false);
		((Control)groupBox3).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
