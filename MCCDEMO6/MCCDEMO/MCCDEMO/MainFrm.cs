using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ADTToolBox;
using MccCustomLanguage;
using SerialPortLibrary;

namespace MCCDEMO;

public class MainFrm : Form
{
	public enum eDebugLevelType
	{
		eDebugLevInfo,
		eDebugLevDebug,
		eDebugLevWarn,
		eDebugLevErr,
		eDebugLevException
	}

	public ushort[] AbsEncPara = new ushort[9];

	private ManualOpEx ManualOpExFrm;

	private ManCtrlSin ManualSinPosFrm;

	private DataCurve ScopeFrm;

	private LuaCtrlFrm luaCtrFrm;

	private XmlDocument xmlDoc = new XmlDocument();

	private uint gbLimitInputLast;

	private string gbStrAppName = "MCCDEMO";

	private IContainer components;

	public ListBox ProgrameList;

	private MenuStrip menuStrip1;

	private GroupBox groupBox1;

	private Button TNeg;

	private Button TPos;

	private Button ZNeg;

	private Button ZPos;

	private Button XNeg;

	private Button YNeg;

	private Button YPos;

	private Button XPos;

	private GroupBox Homing;

	private Button SetHomePara;

	private Button CancleAllHome;

	private Button AllHome;

	private Button THome;

	private Button ZHome;

	private Button YHome;

	private Button XHome;

	private GroupBox Contrl;

	private Button ExitMove;

	private Button ClearAxisPos;

	private Button RestartProg;

	private Button PauseProg;

	private Button StartProg;

	private GroupBox groupBox2;

	private StatusStrip statusStrip1;

	private PictureBox PicRunStt;

	private PictureBox PicLinkStt;

	private Label lbRun;

	private Label lbLink;

	private Timer timer1;

	public SPLibClass Mccard = new SPLibClass();

	public ushort FunRes_OK;

	public ushort FunRes_Err;

	public float[] SysPara;

	public uint SysUint;

	private uint[] SysRunSttNow;

	private uint[] SysRunSttLast;

	private float[] rLastPos;

	private CompensateParameterFrm ComConfigFrm;

	private ParaShow ShowSysParaFrm;

	private SeekZero SeekZeroFrm;

	private TestIOFrm IOAndJoyStickFrm;

	private ProgInputFrm InputProgFrm;

	private CurveDefine mCurveDefFrm;

	private ExPosFollow mExPosFollowFrm;

	private ManCtrlEx mManCtrlFrm;

	private MovePulseCtrl mMovePulseFrm;

	private frmSimpleSine mMoveSine;

	private frmSetAllPara mfrmSetAllPara;

	private ToolStripMenuItem 文件ToolStripMenuItem;

	private ToolStripMenuItem 参数ToolStripMenuItem;

	private ToolStripMenuItem 参数ToolStripMenuItem1;

	private ToolStripMenuItem Tool_ParaDownload;

	private ToolStripMenuItem Tool_ParaUpload;

	private ToolStripMenuItem Tool_ParaShow;

	private ToolStripMenuItem Tool_SaveParaToFile;

	private ToolStripMenuItem Tool_ReadParaFromFile;

	private OpenFileDialog openFileDialog1;

	private SaveFileDialog saveFileDialog1;

	private ToolStripMenuItem Tool_ProgAppend;

	private ToolStripMenuItem Tool_ProgInsert;

	private ToolStripMenuItem Tool_ProgModify;

	private ToolStripMenuItem Tool_ProgDelete;

	private bool FlagProTextCtr;

	private uint ProgramIndex;

	private uint ProCircleTimes;

	private uint ProgCircleTimesSended;

	private bool FlagPause;

	private ToolStripStatusLabel AutoProgMsg;

	private ToolStripMenuItem File_Open;

	private ToolStripMenuItem File_New;

	private ToolStripMenuItem File_Save;

	private ToolStripMenuItem File_SaveAs;

	private string ProgFilePath;

	private Button bn_CloseSp;

	private Button bn_OpenSp;

	private ComboBox cb_SpName;

	private ToolStripMenuItem Tool_About;

	private Button CNeg;

	private Button CPos;

	private Button BNeg;

	private Button BPos;

	private PictureBox[] PICAxisIN;

	private Button CHome;

	private Button BHome;

	private ToolStripMenuItem 扩展ToolStripMenuItem;

	private ToolStripMenuItem 曲线与电子齿轮ToolStripMenuItem;

	private ToolStripMenuItem 单轴运动参数调节ToolStripMenuItem;

	private ToolStripMenuItem 保存至板卡ToolStripMenuItem;

	private ToolStripMenuItem 闭环补偿ToolStripMenuItem;

	private ToolStripMenuItem 补偿参数设置ToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem 串口数据窗口ToolStripMenuItem;

	private ToolStripMenuItem 打开ToolStripMenuItem;

	private ToolStripMenuItem 关闭ToolStripMenuItem;

	private ToolStripMenuItem 设置参数ToolStripMenuItem;

	private ToolStripMenuItem 设置摇杆参数ToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator2;

	private TextBox tbSpdCtrlVal;

	private CheckBox cbSpdCtrl;

	private Button bnParaStop;

	private Button bnEmStop;

	private Label lb_CirleTimes;

	private TextBox txt_CircleTimes;

	private ToolStripSeparator toolStripSeparator3;

	private Label lb_SrPort;

	private GroupBox cbUpdateOpPos;

	private CheckBox chUpdateOpPos;

	private CheckBox cbUpdateOpStt;

	private CheckBox cbUpdateOpEnc;

	private CheckBox cbUpdateOpSpd;

	private CheckBox cbUpdateInput;

	private CheckBox cbJog;

	private ToolStripMenuItem 串口助手ToolStripMenuItem;

	private CheckBox cbMovePulse;

	private ToolStripMenuItem tsmMovePulse;

	private ToolStripMenuItem tsmSimpleSine;

	private ToolStripMenuItem tsmManualOpEx;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem tsmScope;

	private ToolStripMenuItem tlsmReadComTablePara;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripMenuItem tsmExPosFollow;

	private ToolStripMenuItem tsmSinPosMove;

	private TabControl tabCommCfg;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private Button bn_OpenTCP;

	private Button bn_CloseTCP;

	private TextBox tbTcpIPPort;

	private TextBox tbTcpIPAddr;

	private Label label2;

	private Label label1;

	private RichTextBox rtbInfo;

	private SplitContainer splitContainer1;

	private CheckBox cbPosShowModC;

	private CheckBox cbPosShowModB;

	private CheckBox cbPosShowModA;

	private CheckBox cbPosShowModZ;

	private CheckBox cbPosShowModY;

	private CheckBox cbPosShowModX;

	private TextBox textEnPosC;

	private TextBox textEnPosB;

	private Label lbUintC;

	private Label lbUintB;

	private RadioButton rbAxisC;

	private Label PosC;

	private Label lbPosC;

	private RadioButton rbAxisB;

	private Label PosB;

	private Label lbPosB;

	private Button bnRstEncoderPos;

	private TextBox textEnPosT;

	private TextBox textEnPosZ;

	private TextBox textEnPosY;

	private TextBox textEnPosX;

	private RadioButton rbAxisT;

	private RadioButton rbAxisZ;

	private RadioButton rbAxisY;

	private RadioButton rbAxisX;

	private Label SpdAxis;

	private Label PosT;

	private Label PosZ;

	private Label PosY;

	private Label lbSpd;

	private Label PosX;

	private Label lbPosT;

	private Label lbPosZ;

	private Label lbPosY;

	private Label lbUintT;

	private Label lbUintZ;

	private Label lbUintY;

	private Label lbUintX;

	private Label lbPosX;

	private TabControl tbCtrlMCInfo;

	private TabPage tbPageLimitInfo;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private Label lbOrign;

	private Label lblLimitR;

	private Label lbLimitL;

	private TabPage tbPageOther;

	private TextBox tbMovePulseCnt;

	private TextBox tbMovePulsePos;

	private Label label10;

	private Label label9;

	private RadioButton rbMPB;

	private RadioButton rbMPA;

	private RadioButton rbMPY;

	private RadioButton rbMPZ;

	private RadioButton rbMPC;

	private RadioButton rbMPX;

	private ToolStripMenuItem SignleLevCac;

	private TabPage tabPage3;

	private ComboBox cbbCardIndex;

	private Label label11;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripMenuItem tsmTMC2209;

	private ToolStripMenuItem tlsmReadPlatformTablePara;

	private ToolStripMenuItem tsmLUA;

	public MainFrm()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		InitializeComponent();
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		SysPara = new float[Mccard.ParaSum];
		rLastPos = new float[6];
		SysRunSttNow = new uint[1];
		SysRunSttLast = new uint[1];
		try
		{
			((XmlNode)xmlDoc).RemoveAll();
			xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "SystemPara.xml");
		}
		catch (Exception)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSystemParaXMLFileInExecuteDir);
		}
		FunRes_Err = 2;
		FunRes_OK = 1;
		FlagProTextCtr = false;
		ProCircleTimes = 0u;
		ProgramIndex = 0u;
		ProgCircleTimesSended = 0u;
		FlagPause = false;
		((Control)textEnPosX).Enabled = false;
		((Control)textEnPosY).Enabled = false;
		((Control)textEnPosZ).Enabled = false;
		((Control)textEnPosT).Enabled = false;
		((Control)textEnPosB).Enabled = false;
		((Control)textEnPosC).Enabled = false;
		((Control)bnRstEncoderPos).Enabled = true;
		ProgFilePath = "";
		timer1.Enabled = false;
		timer1.Interval = 300;
		SysUint = 0u;
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
		((Control)tbSpdCtrlVal).Enabled = false;
		fun_UpdateAppAccordingToXml();
		CreatePictboxAxisInput();
		ChangeSysUnitDisplay();
		timer1.Enabled = true;
	}

	public void fun_PrintfInfo(string sInfo)
	{
		((Control)rtbInfo).Text = sInfo + Environment.NewLine + ((Control)rtbInfo).Text;
	}

	public void fun_PrintfInfo(eDebugLevelType eLev, string sInfo)
	{
		string text = "";
		switch (eLev)
		{
		case eDebugLevelType.eDebugLevInfo:
			text = "INFO\t";
			break;
		case eDebugLevelType.eDebugLevWarn:
			text = "WARN\t";
			break;
		case eDebugLevelType.eDebugLevErr:
			text = "ERROR\t";
			break;
		case eDebugLevelType.eDebugLevException:
			text = "EXCEPT\t";
			break;
		}
		((Control)rtbInfo).Text = DateTime.Now.ToString() + "\t" + text + sInfo + Environment.NewLine + ((Control)rtbInfo).Text;
	}

	private void fun_UpdateAppAccordingToXml()
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSystemParaXMLFileInExecuteDirUseDefaultPara);
			return;
		}
		try
		{
			XmlElement documentElement = xmlDoc.DocumentElement;
			XmlNode val = ((XmlNode)documentElement).SelectSingleNode("//LINK//TCPIPAddress");
			((Control)tbTcpIPAddr).Text = val.InnerText;
			val = ((XmlNode)documentElement).SelectSingleNode("//LINK//TCPPort");
			((Control)tbTcpIPPort).Text = val.InnerText;
			val = ((XmlNode)documentElement).SelectSingleNode("//LINK//RS232Port");
			((Control)cb_SpName).Text = val.InnerText;
			val = ((XmlNode)documentElement).SelectSingleNode("//ManualPara//CyclicCnt");
			((Control)txt_CircleTimes).Text = val.InnerText;
			val = ((XmlNode)documentElement).SelectSingleNode("//ManualPara//ManualSpd");
			((Control)tbSpdCtrlVal).Text = val.InnerText;
			val = ((XmlNode)documentElement).SelectSingleNode("//SysPara//Unit");
			SysUint = uint.Parse(val.InnerText);
			val = ((XmlNode)documentElement).SelectSingleNode("//SysPara//UpdateTick");
			timer1.Interval = int.Parse(val.InnerText);
			val = ((XmlNode)documentElement).SelectSingleNode("//SysPara//AppName");
			gbStrAppName = val.InnerText;
			((Control)this).Text = gbStrAppName;
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_DecodeXmlFileOK);
		}
		catch (Exception)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_DecodeXmlFileFailed);
		}
	}

	private void CreatePictboxAxisInput()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		ushort num = 0;
		PICAxisIN = (PictureBox[])(object)new PictureBox[18];
		for (num = 0; num < 18; num++)
		{
			PICAxisIN[num] = new PictureBox();
			((Control)tbPageLimitInfo).Controls.Add((Control)(object)PICAxisIN[num]);
		}
		ushort num2 = 0;
		ushort num3 = 0;
		for (num = 0; num < 18; num++)
		{
			((Control)PICAxisIN[num]).BackColor = Color.Black;
			((Control)PICAxisIN[num]).Location = new Point(40 + num2, 27 + num3);
			((Control)PICAxisIN[num]).Name = "PICAxisIN" + num;
			((Control)PICAxisIN[num]).Size = new Size(23, 23);
			PICAxisIN[num].TabIndex = 30;
			PICAxisIN[num].TabStop = false;
			num2 += 50;
			if (num2 > 120)
			{
				num2 = 0;
				num3 += 26;
			}
		}
	}

	private void ChangeSysUnitDisplay()
	{
		if ((SysUint & 0xF) == 0)
		{
			((Control)lbUintX).Text = "P";
		}
		else if (1 == (SysUint & 0xF))
		{
			((Control)lbUintX).Text = "um";
		}
		else if (2 == (SysUint & 0xF))
		{
			((Control)lbUintX).Text = "mm";
		}
		else if (3 == (SysUint & 0xF))
		{
			((Control)lbUintX).Text = "°";
		}
		else
		{
			((Control)lbUintX).Text = "";
		}
		if ((SysUint & 0xF0) == 0)
		{
			((Control)lbUintY).Text = "P";
		}
		else if (16 == (SysUint & 0xF0))
		{
			((Control)lbUintY).Text = "um";
		}
		else if (32 == (SysUint & 0xF0))
		{
			((Control)lbUintY).Text = "mm";
		}
		else if (48 == (SysUint & 0xF0))
		{
			((Control)lbUintY).Text = "°";
		}
		else
		{
			((Control)lbUintY).Text = "";
		}
		if ((SysUint & 0xF00) == 0)
		{
			((Control)lbUintZ).Text = "P";
		}
		else if (256 == (SysUint & 0xF00))
		{
			((Control)lbUintZ).Text = "um";
		}
		else if (512 == (SysUint & 0xF00))
		{
			((Control)lbUintZ).Text = "mm";
		}
		else if (768 == (SysUint & 0xF00))
		{
			((Control)lbUintZ).Text = "°";
		}
		else
		{
			((Control)lbUintZ).Text = "";
		}
		if ((SysUint & 0xF000) == 0)
		{
			((Control)lbUintT).Text = "P";
		}
		else if (4096 == (SysUint & 0xF000))
		{
			((Control)lbUintT).Text = "um";
		}
		else if (8192 == (SysUint & 0xF000))
		{
			((Control)lbUintT).Text = "mm";
		}
		else if (12288 == (SysUint & 0xF000))
		{
			((Control)lbUintT).Text = "°";
		}
		else
		{
			((Control)lbUintT).Text = "";
		}
		if ((SysUint & 0xF0000) == 0)
		{
			((Control)lbUintB).Text = "P";
		}
		else if (65536 == (SysUint & 0xF0000))
		{
			((Control)lbUintB).Text = "um";
		}
		else if (131072 == (SysUint & 0xF0000))
		{
			((Control)lbUintB).Text = "mm";
		}
		else if (196608 == (SysUint & 0xF0000))
		{
			((Control)lbUintB).Text = "°";
		}
		else
		{
			((Control)lbUintB).Text = "";
		}
		if ((SysUint & 0xF00000) == 0)
		{
			((Control)lbUintC).Text = "P";
		}
		else if (1048576 == (SysUint & 0xF00000))
		{
			((Control)lbUintC).Text = "um";
		}
		else if (2097152 == (SysUint & 0xF00000))
		{
			((Control)lbUintC).Text = "mm";
		}
		else if (3145728 == (SysUint & 0xF00000))
		{
			((Control)lbUintC).Text = "°";
		}
		else
		{
			((Control)lbUintC).Text = "";
		}
	}

	private void ConfigJoystick_Click(object sender, EventArgs e)
	{
		IOAndJoyStickFrm = new TestIOFrm();
		((Form)IOAndJoyStickFrm).Owner = (Form)(object)this;
		((Control)IOAndJoyStickFrm).Show();
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		bool flag = false;
		uint num = (uint)SysPara[Mccard.ParaIndex_BaseCAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		num <<= 3;
		num += (uint)SysPara[Mccard.ParaIndex_BaseBAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		num <<= 3;
		num += (uint)SysPara[Mccard.ParaIndex_BaseAAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		num <<= 3;
		num += (uint)SysPara[Mccard.ParaIndex_BaseZAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		num <<= 3;
		num += (uint)SysPara[Mccard.ParaIndex_BaseYAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		num <<= 3;
		num += (uint)SysPara[Mccard.ParaIndex_BaseXAxis + Mccard.ParaIndex_MotortInputLev] & 7;
		if (Mccard.CommLinkEnable)
		{
			((Control)PicLinkStt).BackColor = Color.Lime;
		}
		else
		{
			((Control)PicLinkStt).BackColor = Color.DarkSlateGray;
		}
		if (Mccard.CommLinkEnable && cbUpdateOpStt.Checked && FunRes_OK == Mccard.MoCtrCard_GetRunState(SysRunSttNow))
		{
			if ((SysRunSttNow[0] & Mccard.Axis1RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.Axis2RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.Axis3RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.Axis4RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.Axis5RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.Axis6RunSttBit) == 0 && (SysRunSttNow[0] & Mccard.GrpRunSttBit) == 0)
			{
				((Control)PicRunStt).BackColor = Color.DarkSlateGray;
			}
			else
			{
				((Control)PicRunStt).BackColor = Color.Lime;
			}
			if ((SysRunSttLast[0] & 0x40000000) != (SysRunSttNow[0] & 0x40000000))
			{
				flag = true;
				SysRunSttLast[0] = SysRunSttNow[0];
			}
		}
		if (!Mccard.CommLinkEnable)
		{
			return;
		}
		float[] array = new float[Mccard.MaxAxisNum];
		int[] array2 = new int[Mccard.MaxAxisNum];
		uint[] array3 = new uint[1];
		if (chUpdateOpPos.Checked && FunRes_OK == Mccard.MoCtrCard_GetAxisPos(byte.MaxValue, array))
		{
			((Control)PosX).Text = array[0].ToString("f3");
			((Control)PosY).Text = array[1].ToString("f3");
			((Control)PosZ).Text = array[2].ToString("f3");
			((Control)PosT).Text = array[3].ToString("f3");
			((Control)PosB).Text = array[4].ToString("f3");
			((Control)PosC).Text = array[5].ToString("f3");
			rLastPos[0] = array[0];
			rLastPos[1] = array[1];
			rLastPos[2] = array[2];
			rLastPos[3] = array[3];
			rLastPos[4] = array[4];
			rLastPos[5] = array[5];
		}
		if (cbUpdateOpEnc.Checked && FunRes_OK == Mccard.MoCtrCard_GetEncoderVal(byte.MaxValue, array2))
		{
			double[] array4 = new double[Mccard.MaxAxisNum];
			if (0f == SysPara[5] || 0f == SysPara[Mccard.ParaNumPerAxis + 5] || 0f == SysPara[Mccard.ParaNumPerAxis * 2 + 5] || 0f == SysPara[Mccard.ParaNumPerAxis * 3 + 5] || 0f == SysPara[Mccard.ParaNumPerAxis * 4 + 5] || 0f == SysPara[Mccard.ParaNumPerAxis * 5 + 5])
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_PulseDLIs0ResetParaAndRestart);
			}
			else
			{
				array4[0] = (double)array2[0] / (double)SysPara[Mccard.ParaIndex_EncPulse];
				array4[1] = (double)array2[1] / (double)SysPara[Mccard.ParaNumPerAxis + Mccard.ParaIndex_EncPulse];
				array4[2] = (double)array2[2] / (double)SysPara[Mccard.ParaNumPerAxis * 2 + Mccard.ParaIndex_EncPulse];
				array4[3] = (double)array2[3] / (double)SysPara[Mccard.ParaNumPerAxis * 3 + Mccard.ParaIndex_EncPulse];
				array4[4] = (double)array2[4] / (double)SysPara[Mccard.ParaNumPerAxis * 4 + Mccard.ParaIndex_EncPulse];
				array4[5] = (double)array2[5] / (double)SysPara[Mccard.ParaNumPerAxis * 5 + Mccard.ParaIndex_EncPulse];
				((Control)textEnPosX).Text = (cbPosShowModX.Checked ? array4[0].ToString() : array2[0].ToString());
				((Control)textEnPosY).Text = (cbPosShowModY.Checked ? array4[1].ToString() : array2[1].ToString());
				((Control)textEnPosZ).Text = (cbPosShowModZ.Checked ? array4[2].ToString() : array2[2].ToString());
				((Control)textEnPosT).Text = (cbPosShowModA.Checked ? array4[3].ToString() : array2[3].ToString());
				((Control)textEnPosB).Text = (cbPosShowModB.Checked ? array4[4].ToString() : array2[4].ToString());
				((Control)textEnPosC).Text = (cbPosShowModC.Checked ? array4[5].ToString() : array2[5].ToString());
			}
		}
		((Control)SpdAxis).Text = "0.000";
		if (cbUpdateOpSpd.Checked && FunRes_OK == Mccard.MoCtrCard_GetAxisSpd(byte.MaxValue, array))
		{
			if (rbAxisX.Checked)
			{
				((Control)SpdAxis).Text = array[0].ToString("f1");
			}
			else if (rbAxisY.Checked)
			{
				((Control)SpdAxis).Text = array[1].ToString("f1");
			}
			else if (rbAxisZ.Checked)
			{
				((Control)SpdAxis).Text = array[2].ToString("f1");
			}
			else if (rbAxisT.Checked)
			{
				((Control)SpdAxis).Text = array[3].ToString("f1");
			}
			else if (rbAxisB.Checked)
			{
				((Control)SpdAxis).Text = array[4].ToString("f1");
			}
			else if (rbAxisC.Checked)
			{
				((Control)SpdAxis).Text = array[5].ToString("f1");
			}
		}
		if (((Control)tbCtrlMCInfo.SelectedTab).Name == "tbPageOther")
		{
			byte axisId = 0;
			if (rbMPX.Checked)
			{
				axisId = 0;
			}
			else if (rbMPY.Checked)
			{
				axisId = 1;
			}
			else if (rbMPZ.Checked)
			{
				axisId = 2;
			}
			else if (rbMPA.Checked)
			{
				axisId = 3;
			}
			else if (rbMPB.Checked)
			{
				axisId = 4;
			}
			else if (rbMPC.Checked)
			{
				axisId = 5;
			}
			if (FunRes_OK == Mccard.MoCtrCard_GetMovePulseCnt(axisId, array3))
			{
				((Control)tbMovePulseCnt).Text = array3[0].ToString();
			}
			if (Mccard.FunResOk == Mccard.MoCtrCard_GetMovePulsePos(axisId, array))
			{
				((Control)tbMovePulsePos).Text = array[0].ToString("f3");
			}
		}
		if (cbUpdateInput.Checked && FunRes_OK == Mccard.MoCtrCard_GetInputState(0, array3))
		{
			bool flag2 = false;
			bool flag3 = false;
			Color backColor = Color.FromName("DarkSlateGray");
			Color backColor2 = Color.FromName("Red");
			ushort num2 = 0;
			uint num3 = 0u;
			uint num4 = 0u;
			array3[0] ^= num;
			for (num2 = 0; num2 < 6; num2++)
			{
				num3 = (array3[0] >> 3 * num2) & 7;
				num4 = (gbLimitInputLast >> 3 * num2) & 7;
				if (1 == (num3 & 1) && (num4 & 1) == 0)
				{
					flag2 = true;
				}
				if (1 == (num3 & 1))
				{
					((Control)PICAxisIN[3 * num2]).BackColor = backColor2;
				}
				else
				{
					((Control)PICAxisIN[3 * num2]).BackColor = backColor;
				}
				if (2 == (num3 & 2) && (num4 & 2) == 0)
				{
					flag3 = true;
				}
				if (2 == (num3 & 2))
				{
					((Control)PICAxisIN[3 * num2 + 1]).BackColor = backColor2;
				}
				else
				{
					((Control)PICAxisIN[3 * num2 + 1]).BackColor = backColor;
				}
				if (4 == (num3 & 4))
				{
					((Control)PICAxisIN[3 * num2 + 2]).BackColor = backColor2;
				}
				else
				{
					((Control)PICAxisIN[3 * num2 + 2]).BackColor = backColor;
				}
			}
			gbLimitInputLast = array3[0];
			if (flag2 || flag3)
			{
				if (flag3)
				{
					fun_PrintfInfo(eDebugLevelType.eDebugLevWarn, GlobalData.GlobalLanguage.MainFrm_PositiveLimitEnableStop);
				}
				else if (flag2)
				{
					fun_PrintfInfo(eDebugLevelType.eDebugLevWarn, GlobalData.GlobalLanguage.MainFrm_NegtiveLimitEnableStop);
				}
				if (FlagProTextCtr)
				{
					FlagProTextCtr = false;
					((Control)PauseProg).Enabled = true;
					((Control)StartProg).Enabled = true;
					((Control)RestartProg).Enabled = true;
					((Control)ClearAxisPos).Enabled = true;
					((Control)ExitMove).Enabled = true;
					((Control)ProgrameList).Enabled = true;
					((Control)txt_CircleTimes).Enabled = true;
				}
			}
		}
		if (!(FlagProTextCtr && FlagPause && flag))
		{
			return;
		}
		flag = false;
		ProgramIndex++;
		if (ProgramIndex >= ProgrameList.Items.Count)
		{
			ProgramIndex = 0u;
			FlagProTextCtr = false;
			if (++ProgCircleTimesSended < ProCircleTimes)
			{
				FlagProTextCtr = true;
			}
			else
			{
				((Control)PauseProg).Enabled = true;
				((Control)StartProg).Enabled = true;
				((Control)RestartProg).Enabled = true;
				((Control)ClearAxisPos).Enabled = true;
				((Control)ExitMove).Enabled = true;
				((Control)ProgrameList).Enabled = true;
				((Control)txt_CircleTimes).Enabled = true;
				((ToolStripItem)AutoProgMsg).Text = GlobalData.GlobalLanguage.MainFrm_AllProgrameHasSended;
			}
		}
		if (FlagProTextCtr)
		{
			((ListControl)ProgrameList).SelectedIndex = (int)ProgramIndex;
			if (FunRes_OK == Mccard.MoCtrCard_SendMDICommand(ProgrameList.SelectedItem.ToString()))
			{
				((ToolStripItem)AutoProgMsg).Text = string.Format(GlobalData.GlobalLanguage.MainFrm_SendSomeProgrameSucceed, ProgCircleTimesSended + 1, ProgramIndex + 1);
			}
			else
			{
				((ToolStripItem)AutoProgMsg).Text = string.Format(GlobalData.GlobalLanguage.MainFrm_SendSomeProgrameFailed, ProgCircleTimesSended + 1, ProgramIndex + 1);
			}
		}
	}

	private void ConfigPara_Click(object sender, EventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float[] resSpd = new float[1];
		timer1.Enabled = false;
		mfrmSetAllPara = new frmSetAllPara();
		((Form)mfrmSetAllPara).Owner = (Form)(object)this;
		((Form)mfrmSetAllPara).ShowDialog();
		ChangeSysUnitDisplay();
		Mccard.MoCtrCard_GetAxisSpd(0, resSpd);
		timer1.Enabled = true;
	}

	private void Tool_ParaShow_Click(object sender, EventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		ShowSysParaFrm = new ParaShow(0);
		((Form)ShowSysParaFrm).Owner = (Form)(object)this;
		((Form)ShowSysParaFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void Tool_ParaDownload_Click(object sender, EventArgs e)
	{
		ushort num = 0;
		num = 0;
		while (num < Mccard.ParaSum)
		{
			if (FunRes_OK == Mccard.MoCtrCard_SendPara((byte)(num / Mccard.ParaNumPerAxis), (byte)(num % Mccard.ParaNumPerAxis), SysPara[num]))
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_ParameterDownloading, (float)(int)num / (float)(Mccard.ParaSum - 1)));
				num++;
				continue;
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_ParameterDownloadFailed);
			break;
		}
		num = 0;
		while (num < Mccard.EncParaSum)
		{
			float paraVal = (int)AbsEncPara[num];
			if (FunRes_OK == Mccard.MoCtrCard_SendPara(10, (byte)num, paraVal))
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_EncoderParameterDownloading, (float)(int)num / (float)(Mccard.EncParaSum - 1)));
				num++;
				continue;
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_EncoderParameterDownloadFailed);
			break;
		}
	}

	private void Tool_ParaUpload_Click(object sender, EventArgs e)
	{
		ushort num = 0;
		float[] array = new float[1];
		num = 0;
		while (num < Mccard.ParaSum)
		{
			if (FunRes_OK == Mccard.MoCtrCard_ReadPara((byte)(num / Mccard.ParaNumPerAxis), (byte)(num % Mccard.ParaNumPerAxis), array))
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_SystemParameterUploading, (float)(int)num / (float)(Mccard.ParaSum - 1)));
				SysPara[num] = array[0];
				num++;
				continue;
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_SystemParameterUploadFailed);
			break;
		}
		num = 0;
		while (num < Mccard.EncParaSum)
		{
			if (FunRes_OK == Mccard.MoCtrCard_ReadPara(10, (byte)num, array))
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_EncoderParameterUploading, (float)(int)num / (float)(Mccard.EncParaSum - 1)));
				AbsEncPara[num] = (ushort)array[0];
				num++;
				continue;
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_EncoderarameterUploadFailed);
			break;
		}
	}

	private void Tool_ReadParaFromFile_Click(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)openFileDialog1).Filter = "参数文件(*.para)|*.para";
		((FileDialog)openFileDialog1).FileName = "syspara.para";
		((FileDialog)openFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)openFileDialog1).ShowDialog())
		{
			StreamReader streamReader = new StreamReader(((FileDialog)openFileDialog1).FileName);
			for (num = 0; num < Mccard.ParaSum; num++)
			{
				SysPara[num] = float.Parse(streamReader.ReadLine());
			}
			for (num = 0; num < Mccard.EncParaSum; num++)
			{
				AbsEncPara[num] = ushort.Parse(streamReader.ReadLine());
			}
			streamReader.Close();
		}
	}

	private void Tool_SaveParaToFile_Click(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)saveFileDialog1).Filter = "参数文件(*.para)|*.para";
		((FileDialog)saveFileDialog1).FileName = "syspara.para";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			StreamWriter streamWriter = new StreamWriter(((FileDialog)saveFileDialog1).FileName, append: false, Encoding.ASCII);
			for (num = 0; num < Mccard.ParaSum; num++)
			{
				streamWriter.WriteLine(SysPara[num].ToString("f3"));
			}
			for (num = 0; num < Mccard.EncParaSum; num++)
			{
				streamWriter.WriteLine(AbsEncPara[num].ToString());
			}
			streamWriter.Close();
		}
	}

	private void XHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(0);
	}

	private void YHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(1);
	}

	private void ZHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(2);
	}

	private void THome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(3);
	}

	private void AllHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(byte.MaxValue);
	}

	private void CancleAllHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_CancelSeekZero(byte.MaxValue);
	}

	private void SetHomePara_Click(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		SeekZeroFrm = new SeekZero();
		((Form)SeekZeroFrm).Owner = (Form)(object)this;
		((Form)SeekZeroFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void Tool_ProgAppend_Click(object sender, EventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		InputProgFrm = new ProgInputFrm(1);
		((Form)InputProgFrm).Owner = (Form)(object)this;
		((Form)InputProgFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void Tool_ProgModify_Click(object sender, EventArgs e)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (((ListControl)ProgrameList).SelectedIndex >= 0)
		{
			timer1.Enabled = false;
			InputProgFrm = new ProgInputFrm(ProgrameList.SelectedItem.ToString());
			((Form)InputProgFrm).Owner = (Form)(object)this;
			((Form)InputProgFrm).ShowDialog();
			timer1.Enabled = true;
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_ChooseProgrameLineToEdit);
		}
	}

	private void Tool_ProgDelete_Click(object sender, EventArgs e)
	{
		if (((ListControl)ProgrameList).SelectedIndex >= 0)
		{
			ProgrameList.Items.Remove(ProgrameList.SelectedItem);
		}
	}

	private void Tool_ProgInsert_Click(object sender, EventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		InputProgFrm = new ProgInputFrm(3);
		((Form)InputProgFrm).Owner = (Form)(object)this;
		((Form)InputProgFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void StartProg_Click(object sender, EventArgs e)
	{
		key_startfun();
	}

	private void ProgrameList_SelectedIndexChanged(object sender, EventArgs e)
	{
	}

	private void ClearAxisPos_Click(object sender, EventArgs e)
	{
		if (FunRes_OK == Mccard.MoCtrCard_ResetCoordinate(byte.MaxValue, 0f) && FlagProTextCtr)
		{
			((Control)ClearAxisPos).Enabled = false;
			((Control)StartProg).Enabled = false;
			((Control)ExitMove).Enabled = false;
			((Control)ClearAxisPos).Enabled = false;
			((Control)PauseProg).Enabled = true;
		}
	}

	private void ExitMove_Click(object sender, EventArgs e)
	{
		key_quitefun();
	}

	private void PauseProg_Click(object sender, EventArgs e)
	{
		if ("" == ((Control)ProgrameList).Text)
		{
			Mccard.MoCtrCard_EmergencyStopAxisMov(byte.MaxValue);
		}
		else
		{
			key_pausefun();
		}
	}

	private void RestartProg_Click(object sender, EventArgs e)
	{
		key_restartfun();
	}

	private void File_Open_Click(object sender, EventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between I4 and Unknown
		((FileDialog)openFileDialog1).Filter = "*(*.prog)|*.prog";
		((FileDialog)openFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)openFileDialog1).ShowDialog())
		{
			ProgrameList.Items.Clear();
			ProgFilePath = ((FileDialog)openFileDialog1).FileName;
			StreamReader streamReader = new StreamReader(ProgFilePath);
			while (!streamReader.EndOfStream)
			{
				ProgrameList.Items.Add((object)streamReader.ReadLine());
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_OpenProgrameFileSucceed, ProgFilePath));
			streamReader.Close();
		}
	}

	private void File_New_Click(object sender, EventArgs e)
	{
		ProgrameList.Items.Clear();
		ProgFilePath = "";
	}

	private void File_SaveAs_Click(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)saveFileDialog1).Filter = "程序文件(*.prog)|*.prog";
		((FileDialog)saveFileDialog1).FileName = "ProgFile.prog";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 != (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			return;
		}
		ProgFilePath = ((FileDialog)saveFileDialog1).FileName;
		if (ProgrameList.Items.Count > 0)
		{
			StreamWriter streamWriter = new StreamWriter(ProgFilePath, append: false, Encoding.ASCII);
			for (num = 0; num < ProgrameList.Items.Count; num++)
			{
				((ListControl)ProgrameList).SelectedIndex = num;
				streamWriter.WriteLine(((Control)ProgrameList).Text);
			}
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_SaveProgrameFileTo + ProgFilePath);
			streamWriter.Close();
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_SaveProgrameFileFailedBeacuseNoPrigrame);
		}
	}

	private void File_Save_Click(object sender, EventArgs e)
	{
		ushort num = 0;
		if (ProgrameList.Items.Count > 0)
		{
			if ("" != ProgFilePath)
			{
				StreamWriter streamWriter = new StreamWriter(ProgFilePath, append: false, Encoding.ASCII);
				for (num = 0; num < ProgrameList.Items.Count; num++)
				{
					((ListControl)ProgrameList).SelectedIndex = num;
					streamWriter.WriteLine(((Control)ProgrameList).Text);
				}
				fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_SaveProgrameFileTo + ProgFilePath);
				streamWriter.Close();
			}
			else
			{
				File_SaveAs_Click(sender, e);
			}
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_SaveProgrameFileFailedBeacuseNoPrigrame);
		}
	}

	private void bn_OpenSp_Click(object sender, EventArgs e)
	{
		_ = new float[1];
		((Control)bn_OpenSp).Enabled = false;
		((Control)bn_CloseSp).Enabled = true;
		if (FunRes_Err == Mccard.MoCtrCard_Initial(((Control)cb_SpName).Text))
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.MainFrm_ConnectMCCardFailed, ((Control)cb_SpName).Text));
			return;
		}
		fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_ConnectMCCardSucceed, ((Control)cb_SpName).Text));
		fun_updateSystemPara();
		timer1.Enabled = true;
	}

	private void bn_CloseSp_Click(object sender, EventArgs e)
	{
		((Control)bn_OpenSp).Enabled = true;
		((Control)bn_CloseSp).Enabled = false;
		timer1.Enabled = false;
		((Control)PicLinkStt).BackColor = Color.DarkSlateGray;
		((Control)PicRunStt).BackColor = Color.DarkSlateGray;
		if (Mccard.FunResOk == Mccard.MoCtrCard_Unload())
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_CloseMCCardSucceed);
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_CloseMCCardFailed);
		}
	}

	private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
	{
		try
		{
			Mccard.MoCtrCard_Unload();
			if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
			}
			else
			{
				fun_changeXmlElementVal("//SysPara//Unit", SysUint.ToString());
			}
			if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
			{
				fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
			}
			else
			{
				fun_changeXmlElementVal("//SysPara//UpdateTick", timer1.Interval.ToString());
			}
			xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "SystemPara.xml");
		}
		catch (Exception)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_UnloadFailed);
		}
	}

	private void Tool_About_Click(object sender, EventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		AboutBox aboutBox = new AboutBox();
		((Form)aboutBox).Owner = (Form)(object)this;
		((Form)aboutBox).ShowDialog();
		timer1.Enabled = true;
	}

	private void key_startfun()
	{
		if (0 >= ProgrameList.Items.Count)
		{
			return;
		}
		uint[] array = new uint[1];
		if (FunRes_OK == Mccard.MoCtrCard_GetRunState(array))
		{
			if ((array[0] & Mccard.Axis1RunSttBit) == 0 && (array[0] & Mccard.Axis2RunSttBit) == 0 && (array[0] & Mccard.Axis3RunSttBit) == 0 && (array[0] & Mccard.Axis4RunSttBit) == 0 && (array[0] & Mccard.Axis5RunSttBit) == 0 && (array[0] & Mccard.Axis6RunSttBit) == 0 && (array[0] & Mccard.GrpRunSttBit) == 0 && !FlagProTextCtr)
			{
				FlagProTextCtr = true;
				FlagPause = true;
				ProgramIndex = 0u;
				ProCircleTimes = uint.Parse(((Control)txt_CircleTimes).Text);
				((Control)txt_CircleTimes).Enabled = false;
				ProgCircleTimesSended = 0u;
				((ListControl)ProgrameList).SelectedIndex = (int)ProgramIndex;
				if (FunRes_OK == Mccard.MoCtrCard_SendMDICommand(ProgrameList.SelectedItem.ToString()))
				{
					SysRunSttLast[0] = array[0];
					((ToolStripItem)AutoProgMsg).Text = string.Format(GlobalData.GlobalLanguage.MainFrm_SendSomeProgrameSucceed, ProgCircleTimesSended + 1, ProgramIndex + 1);
					((Control)PauseProg).Enabled = true;
					((Control)StartProg).Enabled = false;
					((Control)RestartProg).Enabled = false;
					((Control)ClearAxisPos).Enabled = false;
					((Control)ExitMove).Enabled = false;
					((Control)ProgrameList).Enabled = false;
				}
				else
				{
					((ToolStripItem)AutoProgMsg).Text = string.Format(GlobalData.GlobalLanguage.MainFrm_SendSomeProgrameFailed, ProgCircleTimesSended + 1, ProgramIndex + 1);
				}
			}
			else
			{
				((ToolStripItem)AutoProgMsg).Text = GlobalData.GlobalLanguage.MainFrm_ProgrameRunningCanNotStartAutoRunAgain;
			}
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_SendProgrameCommandFailedCheckComunication);
		}
	}

	private void key_pausefun()
	{
		FlagPause = false;
		if (FunRes_OK == Mccard.MoCtrCard_PauseAxisMov(15) && FlagProTextCtr)
		{
			((Control)RestartProg).Enabled = true;
			((Control)ExitMove).Enabled = true;
			((Control)PauseProg).Enabled = false;
		}
	}

	private void key_quitefun()
	{
		if (FunRes_OK == Mccard.MoCtrCard_QuiteMotionControl(byte.MaxValue) && FlagProTextCtr)
		{
			((Control)ClearAxisPos).Enabled = true;
			((Control)StartProg).Enabled = true;
			((Control)ExitMove).Enabled = true;
			((Control)ClearAxisPos).Enabled = true;
			((Control)PauseProg).Enabled = true;
			((Control)ProgrameList).Enabled = true;
			FlagProTextCtr = false;
			ProgramIndex = 0u;
			((Control)txt_CircleTimes).Enabled = true;
		}
	}

	private void key_restartfun()
	{
		if (FunRes_OK == Mccard.MoCtrCard_ReStartAxisMov(15) && FlagProTextCtr)
		{
			FlagPause = true;
			((Control)RestartProg).Enabled = false;
			((Control)PauseProg).Enabled = true;
			((Control)ExitMove).Enabled = false;
		}
	}

	private void bnRstEncoderPos_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SetEncoderPos(0, int.Parse(((Control)textEnPosX).Text));
		Mccard.MoCtrCard_SetEncoderPos(1, int.Parse(((Control)textEnPosY).Text));
		Mccard.MoCtrCard_SetEncoderPos(2, int.Parse(((Control)textEnPosZ).Text));
		Mccard.MoCtrCard_SetEncoderPos(3, int.Parse(((Control)textEnPosT).Text));
		Mccard.MoCtrCard_SetEncoderPos(4, int.Parse(((Control)textEnPosB).Text));
		Mccard.MoCtrCard_SetEncoderPos(5, int.Parse(((Control)textEnPosC).Text));
		((Control)textEnPosX).Enabled = false;
		((Control)textEnPosY).Enabled = false;
		((Control)textEnPosZ).Enabled = false;
		((Control)textEnPosT).Enabled = false;
		((Control)textEnPosB).Enabled = false;
		((Control)textEnPosC).Enabled = false;
	}

	private void BHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(4);
	}

	private void CHome_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SeekZero(5);
	}

	private void 曲线与电子齿办ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		mCurveDefFrm = new CurveDefine();
		((Form)mCurveDefFrm).Owner = (Form)(object)this;
		((Control)mCurveDefFrm).Show();
	}

	private void 单轴运动参数调节ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		mManCtrlFrm = new ManCtrlEx();
		((Form)mManCtrlFrm).Owner = (Form)(object)this;
		((Control)mManCtrlFrm).Show();
	}

	private void 保存至板卡ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SaveSystemParaToROM();
	}

	private void 补偿参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		ComConfigFrm = new CompensateParameterFrm();
		((Form)ComConfigFrm).Owner = (Form)(object)this;
		((Form)ComConfigFrm).ShowDialog();
		float[] resSpd = new float[6];
		Mccard.MoCtrCard_GetAxisSpd(0, resSpd);
		timer1.Enabled = true;
	}

	private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_OpenSpDebugForm();
	}

	private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_CloseSpDebugForm();
	}

	private void cbSpdCtrl_CheckedChanged(object sender, EventArgs e)
	{
		if (cbSpdCtrl.Checked)
		{
			((Control)tbSpdCtrlVal).Enabled = true;
		}
		else
		{
			((Control)tbSpdCtrlVal).Enabled = false;
		}
	}

	private void bnEmStop_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_EmergencyStopAxisMov(byte.MaxValue);
	}

	private void bnParaStop_Click(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_ParaStopAxisMov(byte.MaxValue, 0f);
	}

	private void cbUpdateOpEnc_CheckedChanged(object sender, EventArgs e)
	{
		if (cbUpdateOpEnc.Checked)
		{
			((Control)bnRstEncoderPos).Enabled = false;
		}
		else
		{
			((Control)bnRstEncoderPos).Enabled = true;
		}
	}

	private void XPos_MouseDown(object sender, MouseEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (cbJog.Checked && cbSpdCtrl.Checked)
		{
			Button val = (Button)sender;
			if (((Control)val).Name == "XPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(0, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "XNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(0, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "YPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(1, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "YNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(1, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "ZPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(2, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "ZNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(2, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "TPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(3, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "TNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(3, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "BPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(4, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "BNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(4, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "CPos")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(5, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
			else if (((Control)val).Name == "CNeg")
			{
				Mccard.MoCtrCard_MCrlAxisMoveAtSpd(5, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
			}
		}
	}

	private void XPos_MouseUp(object sender, MouseEventArgs e)
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (cbJog.Checked && cbSpdCtrl.Checked)
		{
			Button val = (Button)sender;
			if (((Control)val).Name == "XPos" || ((Control)val).Name == "XNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(0, 0f);
			}
			else if (((Control)val).Name == "YPos" || ((Control)val).Name == "YNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(1, 0f);
			}
			else if (((Control)val).Name == "ZPos" || ((Control)val).Name == "ZNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(2, 0f);
			}
			else if (((Control)val).Name == "TPos" || ((Control)val).Name == "TNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(3, 0f);
			}
			else if (((Control)val).Name == "BPos" || ((Control)val).Name == "BNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(4, 0f);
			}
			else if (((Control)val).Name == "CPos" || ((Control)val).Name == "CNeg")
			{
				Mccard.MoCtrCard_ParaStopAxisMov(5, 0f);
			}
		}
		else
		{
			if (cbJog.Checked)
			{
				return;
			}
			Button val2 = (Button)sender;
			if (((Control)val2).Name == "XPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(0, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(0, 1);
				}
			}
			else if (((Control)val2).Name == "XNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(0, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(0, -1);
				}
			}
			else if (((Control)val2).Name == "YPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(1, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(1, 1);
				}
			}
			else if (((Control)val2).Name == "YNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(1, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(1, -1);
				}
			}
			else if (((Control)val2).Name == "ZPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(2, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(2, 1);
				}
			}
			else if (((Control)val2).Name == "ZNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(2, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(2, -1);
				}
			}
			else if (((Control)val2).Name == "TPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(3, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(3, 1);
				}
			}
			else if (((Control)val2).Name == "TNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(3, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(3, -1);
				}
			}
			else if (((Control)val2).Name == "BPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(4, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(4, 1);
				}
			}
			else if (((Control)val2).Name == "BNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(4, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(4, -1);
				}
			}
			else if (((Control)val2).Name == "CPos")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(5, float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(5, 1);
				}
			}
			else if (((Control)val2).Name == "CNeg")
			{
				if (cbSpdCtrl.Checked)
				{
					Mccard.MoCtrCard_MCrlAxisMoveAtSpd(5, 0f - float.Parse(((Control)tbSpdCtrlVal).Text), 0f);
				}
				else
				{
					Mccard.MoCtrCard_MCrlAxisMove(5, -1);
				}
			}
		}
	}

	private void cbPosShowModX_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		CheckBox val = (CheckBox)sender;
		if (val.Checked)
		{
			if (((Control)val).Name == ((Control)cbPosShowModX).Name)
			{
				((Control)textEnPosX).Enabled = true;
			}
			else if (((Control)val).Name == ((Control)cbPosShowModY).Name)
			{
				((Control)textEnPosY).Enabled = true;
			}
			else if (((Control)val).Name == ((Control)cbPosShowModZ).Name)
			{
				((Control)textEnPosZ).Enabled = true;
			}
			else if (((Control)val).Name == ((Control)cbPosShowModA).Name)
			{
				((Control)textEnPosT).Enabled = true;
			}
			else if (((Control)val).Name == ((Control)cbPosShowModB).Name)
			{
				((Control)textEnPosB).Enabled = true;
			}
			else if (((Control)val).Name == ((Control)cbPosShowModC).Name)
			{
				((Control)textEnPosC).Enabled = true;
			}
		}
		else if (((Control)val).Name == ((Control)cbPosShowModX).Name)
		{
			((Control)textEnPosX).Enabled = false;
		}
		else if (((Control)val).Name == ((Control)cbPosShowModY).Name)
		{
			((Control)textEnPosY).Enabled = false;
		}
		else if (((Control)val).Name == ((Control)cbPosShowModZ).Name)
		{
			((Control)textEnPosZ).Enabled = false;
		}
		else if (((Control)val).Name == ((Control)cbPosShowModA).Name)
		{
			((Control)textEnPosT).Enabled = false;
		}
		else if (((Control)val).Name == ((Control)cbPosShowModB).Name)
		{
			((Control)textEnPosB).Enabled = false;
		}
		else if (((Control)val).Name == ((Control)cbPosShowModC).Name)
		{
			((Control)textEnPosC).Enabled = false;
		}
	}

	private void 串口助手ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		((Control)new SportAssitant()).Show();
		timer1.Enabled = true;
	}

	private void tsmMovePulse_Click(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		mMovePulseFrm = new MovePulseCtrl();
		((Form)mMovePulseFrm).Owner = (Form)(object)this;
		((Form)mMovePulseFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void tsmSimpleSine_Click(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = true;
		mMoveSine = new frmSimpleSine();
		((Form)mMoveSine).Owner = (Form)(object)this;
		((Form)mMoveSine).ShowDialog();
		timer1.Enabled = true;
	}

	private void tsmManualOpEx_Click(object sender, EventArgs e)
	{
		ManualOpExFrm = new ManualOpEx();
		((Form)ManualOpExFrm).Owner = (Form)(object)this;
		((Control)ManualOpExFrm).Show();
	}

	private void tsmScope_Click(object sender, EventArgs e)
	{
		ScopeFrm = new DataCurve(rtbInfo, this);
		((Control)ScopeFrm).Show();
	}

	private void tlsmReadComTablePara_Click(object sender, EventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		timer1.Enabled = false;
		ShowSysParaFrm = new ParaShow(1);
		((Form)ShowSysParaFrm).Owner = (Form)(object)this;
		((Form)ShowSysParaFrm).ShowDialog();
		timer1.Enabled = true;
	}

	private void tsmExPosFollow_Click(object sender, EventArgs e)
	{
		mExPosFollowFrm = new ExPosFollow();
		((Form)mExPosFollowFrm).Owner = (Form)(object)this;
		((Control)mExPosFollowFrm).Show();
	}

	private void tsmSinPosMove_Click(object sender, EventArgs e)
	{
		ManualSinPosFrm = new ManCtrlSin();
		((Form)ManualSinPosFrm).Owner = (Form)(object)this;
		((Control)ManualSinPosFrm).Show();
	}

	private void lb_SrPort_DoubleClick(object sender, EventArgs e)
	{
		((Control)cb_SpName).Text = "";
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
	}

	private void fun_updateSystemPara()
	{
		ushort num = 0;
		float[] array = new float[1];
		num = 0;
		while (num < Mccard.ParaSum && FunRes_OK == Mccard.MoCtrCard_ReadPara((byte)(num / Mccard.ParaNumPerAxis), (byte)(num % Mccard.ParaNumPerAxis), array))
		{
			SysPara[num] = array[0];
			num++;
		}
		if (num < Mccard.ParaSum)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_UpdateSystemParameterFailed + num + "]!");
			return;
		}
		fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_UpdateSystemParameterSucceed);
		num = 0;
		while (num < 9 && FunRes_OK == Mccard.MoCtrCard_ReadPara(10, (byte)num, array))
		{
			AbsEncPara[num] = (ushort)array[0];
			num++;
		}
		if (num < 9)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_UpdateEncoderParameterFailed + num + "]!");
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_UpdateEncoderParameterSucceed);
		}
	}

	private void bn_OpenTCP_Click(object sender, EventArgs e)
	{
		((Control)bn_OpenTCP).Enabled = false;
		((Control)bn_CloseTCP).Enabled = true;
		if (FunRes_Err == Mccard.MoCtrCard_Initial(((Control)tbTcpIPAddr).Text, int.Parse(((Control)tbTcpIPPort).Text)))
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, string.Format(GlobalData.GlobalLanguage.MainFrm_TCPConnectFailedCheckIPAndPort, ((Control)tbTcpIPAddr).Text, ((Control)tbTcpIPPort).Text));
			return;
		}
		fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, string.Format(GlobalData.GlobalLanguage.MainFrm_TCPConnectSucceed, ((Control)tbTcpIPAddr).Text, ((Control)tbTcpIPPort).Text));
		fun_updateSystemPara();
	}

	private void txt_CircleTimes_TextChanged(object sender, EventArgs e)
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
		}
		else
		{
			fun_changeXmlElementVal("//ManualPara//CyclicCnt", ((Control)txt_CircleTimes).Text);
		}
	}

	private void fun_changeXmlElementVal(string xPath, string fValStr)
	{
		((XmlNode)xmlDoc.DocumentElement).SelectSingleNode(xPath).InnerText = fValStr;
	}

	private void tbTcpIPAddr_TextChanged(object sender, EventArgs e)
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
		}
		else
		{
			fun_changeXmlElementVal("//LINK//TCPIPAddress", ((Control)tbTcpIPAddr).Text);
		}
	}

	private void tbTcpIPPort_TextChanged(object sender, EventArgs e)
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
		}
		else
		{
			fun_changeXmlElementVal("//LINK//TCPPort", ((Control)tbTcpIPPort).Text);
		}
	}

	private void cb_SpName_TextChanged(object sender, EventArgs e)
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
		}
		else
		{
			fun_changeXmlElementVal("//LINK//RS232Port", ((Control)cb_SpName).Text);
		}
	}

	private void tbSpdCtrlVal_TextChanged(object sender, EventArgs e)
	{
		if (((XmlNode)xmlDoc).ChildNodes.Count == 0)
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_NoSysParaFileFristOpenItFirst);
		}
		else
		{
			fun_changeXmlElementVal("//ManualPara//ManualSpd", ((Control)tbSpdCtrlVal).Text);
		}
	}

	private void rtbInfo_DoubleClick(object sender, EventArgs e)
	{
		((Control)rtbInfo).Text = "";
	}

	private void bn_CloseTCP_Click(object sender, EventArgs e)
	{
		((Control)bn_OpenTCP).Enabled = true;
		((Control)bn_CloseTCP).Enabled = false;
		if (FunRes_Err == Mccard.MoCtrCard_Unload())
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.MainFrm_TCPCloseFailed);
		}
		else
		{
			fun_PrintfInfo(eDebugLevelType.eDebugLevInfo, GlobalData.GlobalLanguage.MainFrm_TCPCloseSucceed);
		}
	}

	private void SignleLevCac_Click(object sender, EventArgs e)
	{
		_ = new float[1];
		SignleLevCac signleLevCac = new SignleLevCac();
		((Form)signleLevCac).Owner = (Form)(object)this;
		((Control)signleLevCac).Show();
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		Mccard.MoCtrCard_SetMCCardIndex(byte.Parse(((Control)cbbCardIndex).Text));
	}

	private void tsmTMC2209_Click(object sender, EventArgs e)
	{
		TMC2209 tMC = new TMC2209();
		((Form)tMC).Owner = (Form)(object)this;
		((Control)tMC).Show();
	}

	private void chUpdateOpPos_CheckedChanged(object sender, EventArgs e)
	{
		if (chUpdateOpPos.Checked)
		{
			float[] resPos = new float[Mccard.MaxAxisNum];
			Mccard.MoCtrCard_GetAxisPos(byte.MaxValue, resPos);
		}
	}

	private void tlsmReadPlatformTablePara_Click(object sender, EventArgs e)
	{
		((Control)new frmPlatfromLevPara(this)).Show();
	}

	private void tsmLUA_Click(object sender, EventArgs e)
	{
		luaCtrFrm = new LuaCtrlFrm();
		((Form)luaCtrFrm).Owner = (Form)(object)this;
		((Control)luaCtrFrm).Show();
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
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Expected O, but got Unknown
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Expected O, but got Unknown
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Expected O, but got Unknown
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Expected O, but got Unknown
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Expected O, but got Unknown
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Expected O, but got Unknown
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Expected O, but got Unknown
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Expected O, but got Unknown
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Expected O, but got Unknown
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Expected O, but got Unknown
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Expected O, but got Unknown
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Expected O, but got Unknown
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Expected O, but got Unknown
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Expected O, but got Unknown
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Expected O, but got Unknown
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Expected O, but got Unknown
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Expected O, but got Unknown
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Expected O, but got Unknown
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Expected O, but got Unknown
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Expected O, but got Unknown
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Expected O, but got Unknown
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Expected O, but got Unknown
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Expected O, but got Unknown
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Expected O, but got Unknown
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Expected O, but got Unknown
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Expected O, but got Unknown
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Expected O, but got Unknown
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Expected O, but got Unknown
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Expected O, but got Unknown
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Expected O, but got Unknown
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Expected O, but got Unknown
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Expected O, but got Unknown
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Expected O, but got Unknown
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Expected O, but got Unknown
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Expected O, but got Unknown
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Expected O, but got Unknown
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Expected O, but got Unknown
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Expected O, but got Unknown
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Expected O, but got Unknown
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Expected O, but got Unknown
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Expected O, but got Unknown
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Expected O, but got Unknown
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Expected O, but got Unknown
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Expected O, but got Unknown
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Expected O, but got Unknown
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Expected O, but got Unknown
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Expected O, but got Unknown
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Expected O, but got Unknown
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Expected O, but got Unknown
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Expected O, but got Unknown
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Expected O, but got Unknown
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Expected O, but got Unknown
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Expected O, but got Unknown
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Expected O, but got Unknown
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Expected O, but got Unknown
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Expected O, but got Unknown
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Expected O, but got Unknown
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Expected O, but got Unknown
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Expected O, but got Unknown
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Expected O, but got Unknown
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Expected O, but got Unknown
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Expected O, but got Unknown
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Expected O, but got Unknown
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Expected O, but got Unknown
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Expected O, but got Unknown
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Expected O, but got Unknown
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Expected O, but got Unknown
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Expected O, but got Unknown
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Expected O, but got Unknown
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Expected O, but got Unknown
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Expected O, but got Unknown
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Expected O, but got Unknown
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Expected O, but got Unknown
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Expected O, but got Unknown
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Expected O, but got Unknown
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Expected O, but got Unknown
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Expected O, but got Unknown
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Expected O, but got Unknown
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Expected O, but got Unknown
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Expected O, but got Unknown
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Expected O, but got Unknown
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Expected O, but got Unknown
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Expected O, but got Unknown
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Expected O, but got Unknown
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Expected O, but got Unknown
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Expected O, but got Unknown
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Expected O, but got Unknown
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Expected O, but got Unknown
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Expected O, but got Unknown
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Expected O, but got Unknown
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Expected O, but got Unknown
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Expected O, but got Unknown
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Expected O, but got Unknown
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Expected O, but got Unknown
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Expected O, but got Unknown
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Expected O, but got Unknown
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Expected O, but got Unknown
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Expected O, but got Unknown
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Expected O, but got Unknown
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Expected O, but got Unknown
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Expected O, but got Unknown
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Expected O, but got Unknown
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Expected O, but got Unknown
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Expected O, but got Unknown
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Expected O, but got Unknown
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Expected O, but got Unknown
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Expected O, but got Unknown
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Expected O, but got Unknown
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Expected O, but got Unknown
		//IL_270d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2717: Expected O, but got Unknown
		//IL_2724: Unknown result type (might be due to invalid IL or missing references)
		//IL_272e: Expected O, but got Unknown
		//IL_2768: Unknown result type (might be due to invalid IL or missing references)
		//IL_2772: Expected O, but got Unknown
		//IL_277f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2789: Expected O, but got Unknown
		//IL_27c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_27cd: Expected O, but got Unknown
		//IL_27da: Unknown result type (might be due to invalid IL or missing references)
		//IL_27e4: Expected O, but got Unknown
		//IL_281e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2828: Expected O, but got Unknown
		//IL_2835: Unknown result type (might be due to invalid IL or missing references)
		//IL_283f: Expected O, but got Unknown
		//IL_2879: Unknown result type (might be due to invalid IL or missing references)
		//IL_2883: Expected O, but got Unknown
		//IL_2890: Unknown result type (might be due to invalid IL or missing references)
		//IL_289a: Expected O, but got Unknown
		//IL_28d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_28de: Expected O, but got Unknown
		//IL_28eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_28f5: Expected O, but got Unknown
		//IL_292f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2939: Expected O, but got Unknown
		//IL_2946: Unknown result type (might be due to invalid IL or missing references)
		//IL_2950: Expected O, but got Unknown
		//IL_298a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2994: Expected O, but got Unknown
		//IL_29a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ab: Expected O, but got Unknown
		//IL_29e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ef: Expected O, but got Unknown
		//IL_29fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a06: Expected O, but got Unknown
		//IL_2a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a4a: Expected O, but got Unknown
		//IL_2a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a61: Expected O, but got Unknown
		//IL_2a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2aa5: Expected O, but got Unknown
		//IL_2ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2abc: Expected O, but got Unknown
		//IL_2af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b00: Expected O, but got Unknown
		//IL_2b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b17: Expected O, but got Unknown
		//IL_3bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bb5: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainFrm));
		splitContainer1 = new SplitContainer();
		cbPosShowModC = new CheckBox();
		cbPosShowModB = new CheckBox();
		cbPosShowModA = new CheckBox();
		cbPosShowModZ = new CheckBox();
		cbPosShowModY = new CheckBox();
		cbPosShowModX = new CheckBox();
		textEnPosC = new TextBox();
		textEnPosB = new TextBox();
		lbUintC = new Label();
		lbUintB = new Label();
		rbAxisC = new RadioButton();
		PosC = new Label();
		lbPosC = new Label();
		rbAxisB = new RadioButton();
		PosB = new Label();
		lbPosB = new Label();
		bnRstEncoderPos = new Button();
		textEnPosT = new TextBox();
		textEnPosZ = new TextBox();
		textEnPosY = new TextBox();
		textEnPosX = new TextBox();
		rbAxisT = new RadioButton();
		rbAxisZ = new RadioButton();
		rbAxisY = new RadioButton();
		rbAxisX = new RadioButton();
		SpdAxis = new Label();
		PosT = new Label();
		PosZ = new Label();
		PosY = new Label();
		lbSpd = new Label();
		PosX = new Label();
		lbPosT = new Label();
		lbPosZ = new Label();
		lbPosY = new Label();
		lbUintT = new Label();
		lbUintZ = new Label();
		lbUintY = new Label();
		lbUintX = new Label();
		lbPosX = new Label();
		tbCtrlMCInfo = new TabControl();
		tbPageLimitInfo = new TabPage();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		label7 = new Label();
		label8 = new Label();
		lbOrign = new Label();
		lblLimitR = new Label();
		lbLimitL = new Label();
		tbPageOther = new TabPage();
		rbMPB = new RadioButton();
		rbMPA = new RadioButton();
		rbMPY = new RadioButton();
		rbMPZ = new RadioButton();
		rbMPC = new RadioButton();
		rbMPX = new RadioButton();
		tbMovePulseCnt = new TextBox();
		tbMovePulsePos = new TextBox();
		label10 = new Label();
		label9 = new Label();
		ProgrameList = new ListBox();
		menuStrip1 = new MenuStrip();
		文件ToolStripMenuItem = new ToolStripMenuItem();
		File_Open = new ToolStripMenuItem();
		File_New = new ToolStripMenuItem();
		File_Save = new ToolStripMenuItem();
		File_SaveAs = new ToolStripMenuItem();
		参数ToolStripMenuItem = new ToolStripMenuItem();
		Tool_ProgAppend = new ToolStripMenuItem();
		Tool_ProgInsert = new ToolStripMenuItem();
		Tool_ProgModify = new ToolStripMenuItem();
		Tool_ProgDelete = new ToolStripMenuItem();
		参数ToolStripMenuItem1 = new ToolStripMenuItem();
		Tool_ParaDownload = new ToolStripMenuItem();
		Tool_ParaUpload = new ToolStripMenuItem();
		Tool_ParaShow = new ToolStripMenuItem();
		Tool_SaveParaToFile = new ToolStripMenuItem();
		Tool_ReadParaFromFile = new ToolStripMenuItem();
		toolStripSeparator2 = new ToolStripSeparator();
		设置参数ToolStripMenuItem = new ToolStripMenuItem();
		设置摇杆参数ToolStripMenuItem = new ToolStripMenuItem();
		SignleLevCac = new ToolStripMenuItem();
		toolStripSeparator3 = new ToolStripSeparator();
		tlsmReadComTablePara = new ToolStripMenuItem();
		tlsmReadPlatformTablePara = new ToolStripMenuItem();
		toolStripSeparator5 = new ToolStripSeparator();
		保存至板卡ToolStripMenuItem = new ToolStripMenuItem();
		扩展ToolStripMenuItem = new ToolStripMenuItem();
		曲线与电子齿轮ToolStripMenuItem = new ToolStripMenuItem();
		单轴运动参数调节ToolStripMenuItem = new ToolStripMenuItem();
		tsmMovePulse = new ToolStripMenuItem();
		tsmSimpleSine = new ToolStripMenuItem();
		tsmManualOpEx = new ToolStripMenuItem();
		tsmExPosFollow = new ToolStripMenuItem();
		tsmSinPosMove = new ToolStripMenuItem();
		toolStripSeparator1 = new ToolStripSeparator();
		串口数据窗口ToolStripMenuItem = new ToolStripMenuItem();
		打开ToolStripMenuItem = new ToolStripMenuItem();
		关闭ToolStripMenuItem = new ToolStripMenuItem();
		串口助手ToolStripMenuItem = new ToolStripMenuItem();
		toolStripSeparator4 = new ToolStripSeparator();
		tsmScope = new ToolStripMenuItem();
		toolStripSeparator6 = new ToolStripSeparator();
		tsmTMC2209 = new ToolStripMenuItem();
		闭环补偿ToolStripMenuItem = new ToolStripMenuItem();
		补偿参数设置ToolStripMenuItem = new ToolStripMenuItem();
		Tool_About = new ToolStripMenuItem();
		groupBox1 = new GroupBox();
		cbJog = new CheckBox();
		bnParaStop = new Button();
		bnEmStop = new Button();
		tbSpdCtrlVal = new TextBox();
		cbSpdCtrl = new CheckBox();
		CNeg = new Button();
		CPos = new Button();
		BNeg = new Button();
		BPos = new Button();
		TNeg = new Button();
		TPos = new Button();
		ZNeg = new Button();
		ZPos = new Button();
		XNeg = new Button();
		YNeg = new Button();
		YPos = new Button();
		XPos = new Button();
		Homing = new GroupBox();
		CHome = new Button();
		BHome = new Button();
		SetHomePara = new Button();
		CancleAllHome = new Button();
		AllHome = new Button();
		THome = new Button();
		ZHome = new Button();
		YHome = new Button();
		XHome = new Button();
		Contrl = new GroupBox();
		lb_CirleTimes = new Label();
		txt_CircleTimes = new TextBox();
		ExitMove = new Button();
		ClearAxisPos = new Button();
		RestartProg = new Button();
		PauseProg = new Button();
		StartProg = new Button();
		groupBox2 = new GroupBox();
		statusStrip1 = new StatusStrip();
		AutoProgMsg = new ToolStripStatusLabel();
		PicRunStt = new PictureBox();
		PicLinkStt = new PictureBox();
		lbRun = new Label();
		lbLink = new Label();
		timer1 = new Timer(components);
		openFileDialog1 = new OpenFileDialog();
		saveFileDialog1 = new SaveFileDialog();
		bn_CloseSp = new Button();
		bn_OpenSp = new Button();
		lb_SrPort = new Label();
		cb_SpName = new ComboBox();
		cbUpdateOpPos = new GroupBox();
		cbMovePulse = new CheckBox();
		cbUpdateInput = new CheckBox();
		cbUpdateOpEnc = new CheckBox();
		cbUpdateOpSpd = new CheckBox();
		chUpdateOpPos = new CheckBox();
		cbUpdateOpStt = new CheckBox();
		tabCommCfg = new TabControl();
		tabPage1 = new TabPage();
		tabPage2 = new TabPage();
		tbTcpIPPort = new TextBox();
		tbTcpIPAddr = new TextBox();
		label2 = new Label();
		label1 = new Label();
		bn_OpenTCP = new Button();
		bn_CloseTCP = new Button();
		tabPage3 = new TabPage();
		cbbCardIndex = new ComboBox();
		label11 = new Label();
		rtbInfo = new RichTextBox();
		tsmLUA = new ToolStripMenuItem();
		((ISupportInitialize)splitContainer1).BeginInit();
		((Control)splitContainer1.Panel1).SuspendLayout();
		((Control)splitContainer1.Panel2).SuspendLayout();
		((Control)splitContainer1).SuspendLayout();
		((Control)tbCtrlMCInfo).SuspendLayout();
		((Control)tbPageLimitInfo).SuspendLayout();
		((Control)tbPageOther).SuspendLayout();
		((Control)menuStrip1).SuspendLayout();
		((Control)groupBox1).SuspendLayout();
		((Control)Homing).SuspendLayout();
		((Control)Contrl).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)statusStrip1).SuspendLayout();
		((ISupportInitialize)PicRunStt).BeginInit();
		((ISupportInitialize)PicLinkStt).BeginInit();
		((Control)cbUpdateOpPos).SuspendLayout();
		((Control)tabCommCfg).SuspendLayout();
		((Control)tabPage1).SuspendLayout();
		((Control)tabPage2).SuspendLayout();
		((Control)tabPage3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(splitContainer1, "splitContainer1");
		((Control)splitContainer1).Name = "splitContainer1";
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModA);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cbPosShowModX);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosC);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosB);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)bnRstEncoderPos);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosT);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)textEnPosX);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisT);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)rbAxisX);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)SpdAxis);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosT);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbSpd);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)PosX);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosT);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintT);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintZ);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintY);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbUintX);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)lbPosX);
		((Control)splitContainer1.Panel2).Controls.Add((Control)(object)tbCtrlMCInfo);
		componentResourceManager.ApplyResources(cbPosShowModC, "cbPosShowModC");
		((Control)cbPosShowModC).Name = "cbPosShowModC";
		((ButtonBase)cbPosShowModC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbPosShowModB, "cbPosShowModB");
		((Control)cbPosShowModB).Name = "cbPosShowModB";
		((ButtonBase)cbPosShowModB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbPosShowModA, "cbPosShowModA");
		((Control)cbPosShowModA).Name = "cbPosShowModA";
		((ButtonBase)cbPosShowModA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbPosShowModZ, "cbPosShowModZ");
		((Control)cbPosShowModZ).Name = "cbPosShowModZ";
		((ButtonBase)cbPosShowModZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbPosShowModY, "cbPosShowModY");
		((Control)cbPosShowModY).Name = "cbPosShowModY";
		((ButtonBase)cbPosShowModY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbPosShowModX, "cbPosShowModX");
		((Control)cbPosShowModX).Name = "cbPosShowModX";
		((ButtonBase)cbPosShowModX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(textEnPosC, "textEnPosC");
		((Control)textEnPosC).Name = "textEnPosC";
		componentResourceManager.ApplyResources(textEnPosB, "textEnPosB");
		((Control)textEnPosB).Name = "textEnPosB";
		componentResourceManager.ApplyResources(lbUintC, "lbUintC");
		((Control)lbUintC).Name = "lbUintC";
		componentResourceManager.ApplyResources(lbUintB, "lbUintB");
		((Control)lbUintB).Name = "lbUintB";
		componentResourceManager.ApplyResources(rbAxisC, "rbAxisC");
		((Control)rbAxisC).Name = "rbAxisC";
		rbAxisC.TabStop = true;
		((ButtonBase)rbAxisC).UseVisualStyleBackColor = true;
		PosC.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosC, "PosC");
		((Control)PosC).Name = "PosC";
		componentResourceManager.ApplyResources(lbPosC, "lbPosC");
		((Control)lbPosC).Name = "lbPosC";
		componentResourceManager.ApplyResources(rbAxisB, "rbAxisB");
		((Control)rbAxisB).Name = "rbAxisB";
		rbAxisB.TabStop = true;
		((ButtonBase)rbAxisB).UseVisualStyleBackColor = true;
		PosB.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosB, "PosB");
		((Control)PosB).Name = "PosB";
		componentResourceManager.ApplyResources(lbPosB, "lbPosB");
		((Control)lbPosB).Name = "lbPosB";
		componentResourceManager.ApplyResources(bnRstEncoderPos, "bnRstEncoderPos");
		((Control)bnRstEncoderPos).Name = "bnRstEncoderPos";
		((ButtonBase)bnRstEncoderPos).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(textEnPosT, "textEnPosT");
		((Control)textEnPosT).Name = "textEnPosT";
		componentResourceManager.ApplyResources(textEnPosZ, "textEnPosZ");
		((Control)textEnPosZ).Name = "textEnPosZ";
		componentResourceManager.ApplyResources(textEnPosY, "textEnPosY");
		((Control)textEnPosY).Name = "textEnPosY";
		componentResourceManager.ApplyResources(textEnPosX, "textEnPosX");
		((Control)textEnPosX).Name = "textEnPosX";
		componentResourceManager.ApplyResources(rbAxisT, "rbAxisT");
		((Control)rbAxisT).Name = "rbAxisT";
		rbAxisT.TabStop = true;
		((ButtonBase)rbAxisT).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbAxisZ, "rbAxisZ");
		((Control)rbAxisZ).Name = "rbAxisZ";
		rbAxisZ.TabStop = true;
		((ButtonBase)rbAxisZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbAxisY, "rbAxisY");
		((Control)rbAxisY).Name = "rbAxisY";
		rbAxisY.TabStop = true;
		((ButtonBase)rbAxisY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbAxisX, "rbAxisX");
		((Control)rbAxisX).Name = "rbAxisX";
		rbAxisX.TabStop = true;
		((ButtonBase)rbAxisX).UseVisualStyleBackColor = true;
		SpdAxis.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(SpdAxis, "SpdAxis");
		((Control)SpdAxis).Name = "SpdAxis";
		PosT.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosT, "PosT");
		((Control)PosT).Name = "PosT";
		PosZ.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosZ, "PosZ");
		((Control)PosZ).Name = "PosZ";
		PosY.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosY, "PosY");
		((Control)PosY).Name = "PosY";
		componentResourceManager.ApplyResources(lbSpd, "lbSpd");
		((Control)lbSpd).Name = "lbSpd";
		PosX.BorderStyle = (BorderStyle)1;
		componentResourceManager.ApplyResources(PosX, "PosX");
		((Control)PosX).Name = "PosX";
		componentResourceManager.ApplyResources(lbPosT, "lbPosT");
		((Control)lbPosT).Name = "lbPosT";
		componentResourceManager.ApplyResources(lbPosZ, "lbPosZ");
		((Control)lbPosZ).Name = "lbPosZ";
		componentResourceManager.ApplyResources(lbPosY, "lbPosY");
		((Control)lbPosY).Name = "lbPosY";
		componentResourceManager.ApplyResources(lbUintT, "lbUintT");
		((Control)lbUintT).Name = "lbUintT";
		componentResourceManager.ApplyResources(lbUintZ, "lbUintZ");
		((Control)lbUintZ).Name = "lbUintZ";
		componentResourceManager.ApplyResources(lbUintY, "lbUintY");
		((Control)lbUintY).Name = "lbUintY";
		componentResourceManager.ApplyResources(lbUintX, "lbUintX");
		((Control)lbUintX).Name = "lbUintX";
		componentResourceManager.ApplyResources(lbPosX, "lbPosX");
		((Control)lbPosX).Name = "lbPosX";
		((Control)tbCtrlMCInfo).Controls.Add((Control)(object)tbPageLimitInfo);
		((Control)tbCtrlMCInfo).Controls.Add((Control)(object)tbPageOther);
		componentResourceManager.ApplyResources(tbCtrlMCInfo, "tbCtrlMCInfo");
		((Control)tbCtrlMCInfo).Name = "tbCtrlMCInfo";
		tbCtrlMCInfo.SelectedIndex = 0;
		((Control)tbPageLimitInfo).BackColor = SystemColors.Control;
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label3);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label4);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label5);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label6);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label7);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)label8);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)lbOrign);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)lblLimitR);
		((Control)tbPageLimitInfo).Controls.Add((Control)(object)lbLimitL);
		componentResourceManager.ApplyResources(tbPageLimitInfo, "tbPageLimitInfo");
		((Control)tbPageLimitInfo).Name = "tbPageLimitInfo";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(lbOrign, "lbOrign");
		((Control)lbOrign).Name = "lbOrign";
		componentResourceManager.ApplyResources(lblLimitR, "lblLimitR");
		((Control)lblLimitR).Name = "lblLimitR";
		componentResourceManager.ApplyResources(lbLimitL, "lbLimitL");
		((Control)lbLimitL).Name = "lbLimitL";
		((Control)tbPageOther).BackColor = SystemColors.Control;
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPB);
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPA);
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPY);
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPZ);
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPC);
		((Control)tbPageOther).Controls.Add((Control)(object)rbMPX);
		((Control)tbPageOther).Controls.Add((Control)(object)tbMovePulseCnt);
		((Control)tbPageOther).Controls.Add((Control)(object)tbMovePulsePos);
		((Control)tbPageOther).Controls.Add((Control)(object)label10);
		((Control)tbPageOther).Controls.Add((Control)(object)label9);
		componentResourceManager.ApplyResources(tbPageOther, "tbPageOther");
		((Control)tbPageOther).Name = "tbPageOther";
		componentResourceManager.ApplyResources(rbMPB, "rbMPB");
		((Control)rbMPB).Name = "rbMPB";
		((ButtonBase)rbMPB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMPA, "rbMPA");
		((Control)rbMPA).Name = "rbMPA";
		((ButtonBase)rbMPA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMPY, "rbMPY");
		((Control)rbMPY).Name = "rbMPY";
		((ButtonBase)rbMPY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMPZ, "rbMPZ");
		((Control)rbMPZ).Name = "rbMPZ";
		((ButtonBase)rbMPZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMPC, "rbMPC");
		((Control)rbMPC).Name = "rbMPC";
		((ButtonBase)rbMPC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbMPX, "rbMPX");
		rbMPX.Checked = true;
		((Control)rbMPX).Name = "rbMPX";
		rbMPX.TabStop = true;
		((ButtonBase)rbMPX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbMovePulseCnt, "tbMovePulseCnt");
		((Control)tbMovePulseCnt).Name = "tbMovePulseCnt";
		componentResourceManager.ApplyResources(tbMovePulsePos, "tbMovePulsePos");
		((Control)tbMovePulsePos).Name = "tbMovePulsePos";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		((ListControl)ProgrameList).FormattingEnabled = true;
		componentResourceManager.ApplyResources(ProgrameList, "ProgrameList");
		((Control)ProgrameList).Name = "ProgrameList";
		ProgrameList.SelectedIndexChanged += ProgrameList_SelectedIndexChanged;
		((ToolStrip)menuStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[6]
		{
			(ToolStripItem)文件ToolStripMenuItem,
			(ToolStripItem)参数ToolStripMenuItem,
			(ToolStripItem)参数ToolStripMenuItem1,
			(ToolStripItem)扩展ToolStripMenuItem,
			(ToolStripItem)闭环补偿ToolStripMenuItem,
			(ToolStripItem)Tool_About
		});
		componentResourceManager.ApplyResources(menuStrip1, "menuStrip1");
		((Control)menuStrip1).Name = "menuStrip1";
		((ToolStripDropDownItem)文件ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)File_Open,
			(ToolStripItem)File_New,
			(ToolStripItem)File_Save,
			(ToolStripItem)File_SaveAs
		});
		((ToolStripItem)文件ToolStripMenuItem).Name = "文件ToolStripMenuItem";
		componentResourceManager.ApplyResources(文件ToolStripMenuItem, "文件ToolStripMenuItem");
		((ToolStripItem)File_Open).Name = "File_Open";
		componentResourceManager.ApplyResources(File_Open, "File_Open");
		((ToolStripItem)File_Open).Click += File_Open_Click;
		((ToolStripItem)File_New).Name = "File_New";
		componentResourceManager.ApplyResources(File_New, "File_New");
		((ToolStripItem)File_New).Click += File_New_Click;
		((ToolStripItem)File_Save).Name = "File_Save";
		componentResourceManager.ApplyResources(File_Save, "File_Save");
		((ToolStripItem)File_Save).Click += File_Save_Click;
		((ToolStripItem)File_SaveAs).Name = "File_SaveAs";
		componentResourceManager.ApplyResources(File_SaveAs, "File_SaveAs");
		((ToolStripItem)File_SaveAs).Click += File_SaveAs_Click;
		((ToolStripDropDownItem)参数ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)Tool_ProgAppend,
			(ToolStripItem)Tool_ProgInsert,
			(ToolStripItem)Tool_ProgModify,
			(ToolStripItem)Tool_ProgDelete
		});
		((ToolStripItem)参数ToolStripMenuItem).Name = "参数ToolStripMenuItem";
		componentResourceManager.ApplyResources(参数ToolStripMenuItem, "参数ToolStripMenuItem");
		((ToolStripItem)Tool_ProgAppend).Name = "Tool_ProgAppend";
		componentResourceManager.ApplyResources(Tool_ProgAppend, "Tool_ProgAppend");
		((ToolStripItem)Tool_ProgAppend).Click += Tool_ProgAppend_Click;
		((ToolStripItem)Tool_ProgInsert).Name = "Tool_ProgInsert";
		componentResourceManager.ApplyResources(Tool_ProgInsert, "Tool_ProgInsert");
		((ToolStripItem)Tool_ProgInsert).Click += Tool_ProgInsert_Click;
		((ToolStripItem)Tool_ProgModify).Name = "Tool_ProgModify";
		componentResourceManager.ApplyResources(Tool_ProgModify, "Tool_ProgModify");
		((ToolStripItem)Tool_ProgModify).Click += Tool_ProgModify_Click;
		((ToolStripItem)Tool_ProgDelete).Name = "Tool_ProgDelete";
		componentResourceManager.ApplyResources(Tool_ProgDelete, "Tool_ProgDelete");
		((ToolStripItem)Tool_ProgDelete).Click += Tool_ProgDelete_Click;
		((ToolStripDropDownItem)参数ToolStripMenuItem1).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[14]
		{
			(ToolStripItem)Tool_ParaDownload,
			(ToolStripItem)Tool_ParaUpload,
			(ToolStripItem)Tool_ParaShow,
			(ToolStripItem)Tool_SaveParaToFile,
			(ToolStripItem)Tool_ReadParaFromFile,
			(ToolStripItem)toolStripSeparator2,
			(ToolStripItem)设置参数ToolStripMenuItem,
			(ToolStripItem)设置摇杆参数ToolStripMenuItem,
			(ToolStripItem)SignleLevCac,
			(ToolStripItem)toolStripSeparator3,
			(ToolStripItem)tlsmReadComTablePara,
			(ToolStripItem)tlsmReadPlatformTablePara,
			(ToolStripItem)toolStripSeparator5,
			(ToolStripItem)保存至板卡ToolStripMenuItem
		});
		((ToolStripItem)参数ToolStripMenuItem1).Name = "参数ToolStripMenuItem1";
		componentResourceManager.ApplyResources(参数ToolStripMenuItem1, "参数ToolStripMenuItem1");
		((ToolStripItem)Tool_ParaDownload).Name = "Tool_ParaDownload";
		componentResourceManager.ApplyResources(Tool_ParaDownload, "Tool_ParaDownload");
		((ToolStripItem)Tool_ParaDownload).Click += Tool_ParaDownload_Click;
		((ToolStripItem)Tool_ParaUpload).Name = "Tool_ParaUpload";
		componentResourceManager.ApplyResources(Tool_ParaUpload, "Tool_ParaUpload");
		((ToolStripItem)Tool_ParaUpload).Click += Tool_ParaUpload_Click;
		((ToolStripItem)Tool_ParaShow).Name = "Tool_ParaShow";
		componentResourceManager.ApplyResources(Tool_ParaShow, "Tool_ParaShow");
		((ToolStripItem)Tool_ParaShow).Click += Tool_ParaShow_Click;
		((ToolStripItem)Tool_SaveParaToFile).Name = "Tool_SaveParaToFile";
		componentResourceManager.ApplyResources(Tool_SaveParaToFile, "Tool_SaveParaToFile");
		((ToolStripItem)Tool_SaveParaToFile).Click += Tool_SaveParaToFile_Click;
		((ToolStripItem)Tool_ReadParaFromFile).Name = "Tool_ReadParaFromFile";
		componentResourceManager.ApplyResources(Tool_ReadParaFromFile, "Tool_ReadParaFromFile");
		((ToolStripItem)Tool_ReadParaFromFile).Click += Tool_ReadParaFromFile_Click;
		((ToolStripItem)toolStripSeparator2).Name = "toolStripSeparator2";
		componentResourceManager.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
		((ToolStripItem)设置参数ToolStripMenuItem).Name = "设置参数ToolStripMenuItem";
		componentResourceManager.ApplyResources(设置参数ToolStripMenuItem, "设置参数ToolStripMenuItem");
		((ToolStripItem)设置参数ToolStripMenuItem).Click += ConfigPara_Click;
		((ToolStripItem)设置摇杆参数ToolStripMenuItem).Name = "设置摇杆参数ToolStripMenuItem";
		componentResourceManager.ApplyResources(设置摇杆参数ToolStripMenuItem, "设置摇杆参数ToolStripMenuItem");
		((ToolStripItem)设置摇杆参数ToolStripMenuItem).Click += ConfigJoystick_Click;
		((ToolStripItem)SignleLevCac).Name = "SignleLevCac";
		componentResourceManager.ApplyResources(SignleLevCac, "SignleLevCac");
		((ToolStripItem)SignleLevCac).Click += SignleLevCac_Click;
		((ToolStripItem)toolStripSeparator3).Name = "toolStripSeparator3";
		componentResourceManager.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
		((ToolStripItem)tlsmReadComTablePara).Name = "tlsmReadComTablePara";
		componentResourceManager.ApplyResources(tlsmReadComTablePara, "tlsmReadComTablePara");
		((ToolStripItem)tlsmReadComTablePara).Click += tlsmReadComTablePara_Click;
		((ToolStripItem)tlsmReadPlatformTablePara).Name = "tlsmReadPlatformTablePara";
		componentResourceManager.ApplyResources(tlsmReadPlatformTablePara, "tlsmReadPlatformTablePara");
		((ToolStripItem)tlsmReadPlatformTablePara).Click += tlsmReadPlatformTablePara_Click;
		((ToolStripItem)toolStripSeparator5).Name = "toolStripSeparator5";
		componentResourceManager.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
		((ToolStripItem)保存至板卡ToolStripMenuItem).Name = "保存至板卡ToolStripMenuItem";
		componentResourceManager.ApplyResources(保存至板卡ToolStripMenuItem, "保存至板卡ToolStripMenuItem");
		((ToolStripItem)保存至板卡ToolStripMenuItem).Click += 保存至板卡ToolStripMenuItem_Click;
		((ToolStripDropDownItem)扩展ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[15]
		{
			(ToolStripItem)曲线与电子齿轮ToolStripMenuItem,
			(ToolStripItem)单轴运动参数调节ToolStripMenuItem,
			(ToolStripItem)tsmMovePulse,
			(ToolStripItem)tsmSimpleSine,
			(ToolStripItem)tsmManualOpEx,
			(ToolStripItem)tsmExPosFollow,
			(ToolStripItem)tsmSinPosMove,
			(ToolStripItem)tsmLUA,
			(ToolStripItem)toolStripSeparator1,
			(ToolStripItem)串口数据窗口ToolStripMenuItem,
			(ToolStripItem)串口助手ToolStripMenuItem,
			(ToolStripItem)toolStripSeparator4,
			(ToolStripItem)tsmScope,
			(ToolStripItem)toolStripSeparator6,
			(ToolStripItem)tsmTMC2209
		});
		((ToolStripItem)扩展ToolStripMenuItem).Name = "扩展ToolStripMenuItem";
		componentResourceManager.ApplyResources(扩展ToolStripMenuItem, "扩展ToolStripMenuItem");
		((ToolStripItem)曲线与电子齿轮ToolStripMenuItem).Name = "曲线与电子齿轮ToolStripMenuItem";
		componentResourceManager.ApplyResources(曲线与电子齿轮ToolStripMenuItem, "曲线与电子齿轮ToolStripMenuItem");
		((ToolStripItem)曲线与电子齿轮ToolStripMenuItem).Click += 曲线与电子齿办ToolStripMenuItem_Click;
		((ToolStripItem)单轴运动参数调节ToolStripMenuItem).Name = "单轴运动参数调节ToolStripMenuItem";
		componentResourceManager.ApplyResources(单轴运动参数调节ToolStripMenuItem, "单轴运动参数调节ToolStripMenuItem");
		((ToolStripItem)单轴运动参数调节ToolStripMenuItem).Click += 单轴运动参数调节ToolStripMenuItem_Click;
		((ToolStripItem)tsmMovePulse).Name = "tsmMovePulse";
		componentResourceManager.ApplyResources(tsmMovePulse, "tsmMovePulse");
		((ToolStripItem)tsmMovePulse).Click += tsmMovePulse_Click;
		((ToolStripItem)tsmSimpleSine).Name = "tsmSimpleSine";
		componentResourceManager.ApplyResources(tsmSimpleSine, "tsmSimpleSine");
		((ToolStripItem)tsmSimpleSine).Click += tsmSimpleSine_Click;
		((ToolStripItem)tsmManualOpEx).Name = "tsmManualOpEx";
		componentResourceManager.ApplyResources(tsmManualOpEx, "tsmManualOpEx");
		((ToolStripItem)tsmManualOpEx).Click += tsmManualOpEx_Click;
		((ToolStripItem)tsmExPosFollow).Name = "tsmExPosFollow";
		componentResourceManager.ApplyResources(tsmExPosFollow, "tsmExPosFollow");
		((ToolStripItem)tsmExPosFollow).Click += tsmExPosFollow_Click;
		((ToolStripItem)tsmSinPosMove).Name = "tsmSinPosMove";
		componentResourceManager.ApplyResources(tsmSinPosMove, "tsmSinPosMove");
		((ToolStripItem)tsmSinPosMove).Click += tsmSinPosMove_Click;
		((ToolStripItem)toolStripSeparator1).Name = "toolStripSeparator1";
		componentResourceManager.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
		((ToolStripDropDownItem)串口数据窗口ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[2]
		{
			(ToolStripItem)打开ToolStripMenuItem,
			(ToolStripItem)关闭ToolStripMenuItem
		});
		((ToolStripItem)串口数据窗口ToolStripMenuItem).Name = "串口数据窗口ToolStripMenuItem";
		componentResourceManager.ApplyResources(串口数据窗口ToolStripMenuItem, "串口数据窗口ToolStripMenuItem");
		((ToolStripItem)打开ToolStripMenuItem).Name = "打开ToolStripMenuItem";
		componentResourceManager.ApplyResources(打开ToolStripMenuItem, "打开ToolStripMenuItem");
		((ToolStripItem)打开ToolStripMenuItem).Click += 打开ToolStripMenuItem_Click;
		((ToolStripItem)关闭ToolStripMenuItem).Name = "关闭ToolStripMenuItem";
		componentResourceManager.ApplyResources(关闭ToolStripMenuItem, "关闭ToolStripMenuItem");
		((ToolStripItem)关闭ToolStripMenuItem).Click += 关闭ToolStripMenuItem_Click;
		((ToolStripItem)串口助手ToolStripMenuItem).Name = "串口助手ToolStripMenuItem";
		componentResourceManager.ApplyResources(串口助手ToolStripMenuItem, "串口助手ToolStripMenuItem");
		((ToolStripItem)串口助手ToolStripMenuItem).Click += 串口助手ToolStripMenuItem_Click;
		((ToolStripItem)toolStripSeparator4).Name = "toolStripSeparator4";
		componentResourceManager.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
		((ToolStripItem)tsmScope).Name = "tsmScope";
		componentResourceManager.ApplyResources(tsmScope, "tsmScope");
		((ToolStripItem)tsmScope).Click += tsmScope_Click;
		((ToolStripItem)toolStripSeparator6).Name = "toolStripSeparator6";
		componentResourceManager.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
		((ToolStripItem)tsmTMC2209).Name = "tsmTMC2209";
		componentResourceManager.ApplyResources(tsmTMC2209, "tsmTMC2209");
		((ToolStripItem)tsmTMC2209).Click += tsmTMC2209_Click;
		((ToolStripDropDownItem)闭环补偿ToolStripMenuItem).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[1] { (ToolStripItem)补偿参数设置ToolStripMenuItem });
		((ToolStripItem)闭环补偿ToolStripMenuItem).Name = "闭环补偿ToolStripMenuItem";
		componentResourceManager.ApplyResources(闭环补偿ToolStripMenuItem, "闭环补偿ToolStripMenuItem");
		((ToolStripItem)补偿参数设置ToolStripMenuItem).Name = "补偿参数设置ToolStripMenuItem";
		componentResourceManager.ApplyResources(补偿参数设置ToolStripMenuItem, "补偿参数设置ToolStripMenuItem");
		((ToolStripItem)补偿参数设置ToolStripMenuItem).Click += 补偿参数设置ToolStripMenuItem_Click;
		((ToolStripItem)Tool_About).Name = "Tool_About";
		componentResourceManager.ApplyResources(Tool_About, "Tool_About");
		((ToolStripItem)Tool_About).Click += Tool_About_Click;
		((Control)groupBox1).Controls.Add((Control)(object)cbJog);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStop);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStop);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdCtrlVal);
		((Control)groupBox1).Controls.Add((Control)(object)cbSpdCtrl);
		((Control)groupBox1).Controls.Add((Control)(object)CNeg);
		((Control)groupBox1).Controls.Add((Control)(object)CPos);
		((Control)groupBox1).Controls.Add((Control)(object)BNeg);
		((Control)groupBox1).Controls.Add((Control)(object)BPos);
		((Control)groupBox1).Controls.Add((Control)(object)TNeg);
		((Control)groupBox1).Controls.Add((Control)(object)TPos);
		((Control)groupBox1).Controls.Add((Control)(object)ZNeg);
		((Control)groupBox1).Controls.Add((Control)(object)ZPos);
		((Control)groupBox1).Controls.Add((Control)(object)XNeg);
		((Control)groupBox1).Controls.Add((Control)(object)YNeg);
		((Control)groupBox1).Controls.Add((Control)(object)YPos);
		((Control)groupBox1).Controls.Add((Control)(object)XPos);
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(cbJog, "cbJog");
		((Control)cbJog).Name = "cbJog";
		((ButtonBase)cbJog).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnParaStop, "bnParaStop");
		((Control)bnParaStop).Name = "bnParaStop";
		((ButtonBase)bnParaStop).UseVisualStyleBackColor = true;
		((Control)bnParaStop).Click += bnParaStop_Click;
		componentResourceManager.ApplyResources(bnEmStop, "bnEmStop");
		((Control)bnEmStop).Name = "bnEmStop";
		((ButtonBase)bnEmStop).UseVisualStyleBackColor = true;
		((Control)bnEmStop).Click += bnEmStop_Click;
		componentResourceManager.ApplyResources(tbSpdCtrlVal, "tbSpdCtrlVal");
		((Control)tbSpdCtrlVal).Name = "tbSpdCtrlVal";
		((Control)tbSpdCtrlVal).TextChanged += tbSpdCtrlVal_TextChanged;
		componentResourceManager.ApplyResources(cbSpdCtrl, "cbSpdCtrl");
		((Control)cbSpdCtrl).Name = "cbSpdCtrl";
		((ButtonBase)cbSpdCtrl).UseVisualStyleBackColor = true;
		cbSpdCtrl.CheckedChanged += cbSpdCtrl_CheckedChanged;
		componentResourceManager.ApplyResources(CNeg, "CNeg");
		((Control)CNeg).Name = "CNeg";
		((ButtonBase)CNeg).UseVisualStyleBackColor = true;
		((Control)CNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)CNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(CPos, "CPos");
		((Control)CPos).Name = "CPos";
		((ButtonBase)CPos).UseVisualStyleBackColor = true;
		((Control)CPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)CPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(BNeg, "BNeg");
		((Control)BNeg).Name = "BNeg";
		((ButtonBase)BNeg).UseVisualStyleBackColor = true;
		((Control)BNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)BNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(BPos, "BPos");
		((Control)BPos).Name = "BPos";
		((ButtonBase)BPos).UseVisualStyleBackColor = true;
		((Control)BPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)BPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(TNeg, "TNeg");
		((Control)TNeg).Name = "TNeg";
		((ButtonBase)TNeg).UseVisualStyleBackColor = true;
		((Control)TNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)TNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(TPos, "TPos");
		((Control)TPos).Name = "TPos";
		((ButtonBase)TPos).UseVisualStyleBackColor = true;
		((Control)TPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)TPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(ZNeg, "ZNeg");
		((Control)ZNeg).Name = "ZNeg";
		((ButtonBase)ZNeg).UseVisualStyleBackColor = true;
		((Control)ZNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)ZNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(ZPos, "ZPos");
		((Control)ZPos).Name = "ZPos";
		((ButtonBase)ZPos).UseVisualStyleBackColor = true;
		((Control)ZPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)ZPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(XNeg, "XNeg");
		((Control)XNeg).Name = "XNeg";
		((ButtonBase)XNeg).UseVisualStyleBackColor = true;
		((Control)XNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)XNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(YNeg, "YNeg");
		((Control)YNeg).Name = "YNeg";
		((ButtonBase)YNeg).UseVisualStyleBackColor = true;
		((Control)YNeg).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)YNeg).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(YPos, "YPos");
		((Control)YPos).Name = "YPos";
		((ButtonBase)YPos).UseVisualStyleBackColor = true;
		((Control)YPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)YPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		componentResourceManager.ApplyResources(XPos, "XPos");
		((Control)XPos).Name = "XPos";
		((ButtonBase)XPos).UseVisualStyleBackColor = true;
		((Control)XPos).MouseDown += new MouseEventHandler(XPos_MouseDown);
		((Control)XPos).MouseUp += new MouseEventHandler(XPos_MouseUp);
		((Control)Homing).Controls.Add((Control)(object)CHome);
		((Control)Homing).Controls.Add((Control)(object)BHome);
		((Control)Homing).Controls.Add((Control)(object)SetHomePara);
		((Control)Homing).Controls.Add((Control)(object)CancleAllHome);
		((Control)Homing).Controls.Add((Control)(object)AllHome);
		((Control)Homing).Controls.Add((Control)(object)THome);
		((Control)Homing).Controls.Add((Control)(object)ZHome);
		((Control)Homing).Controls.Add((Control)(object)YHome);
		((Control)Homing).Controls.Add((Control)(object)XHome);
		componentResourceManager.ApplyResources(Homing, "Homing");
		((Control)Homing).Name = "Homing";
		Homing.TabStop = false;
		componentResourceManager.ApplyResources(CHome, "CHome");
		((Control)CHome).Name = "CHome";
		((ButtonBase)CHome).UseVisualStyleBackColor = true;
		((Control)CHome).Click += CHome_Click;
		componentResourceManager.ApplyResources(BHome, "BHome");
		((Control)BHome).Name = "BHome";
		((ButtonBase)BHome).UseVisualStyleBackColor = true;
		((Control)BHome).Click += BHome_Click;
		componentResourceManager.ApplyResources(SetHomePara, "SetHomePara");
		((Control)SetHomePara).Name = "SetHomePara";
		((ButtonBase)SetHomePara).UseVisualStyleBackColor = true;
		((Control)SetHomePara).Click += SetHomePara_Click;
		componentResourceManager.ApplyResources(CancleAllHome, "CancleAllHome");
		((Control)CancleAllHome).Name = "CancleAllHome";
		((ButtonBase)CancleAllHome).UseVisualStyleBackColor = true;
		((Control)CancleAllHome).Click += CancleAllHome_Click;
		componentResourceManager.ApplyResources(AllHome, "AllHome");
		((Control)AllHome).Name = "AllHome";
		((ButtonBase)AllHome).UseVisualStyleBackColor = true;
		((Control)AllHome).Click += AllHome_Click;
		componentResourceManager.ApplyResources(THome, "THome");
		((Control)THome).Name = "THome";
		((ButtonBase)THome).UseVisualStyleBackColor = true;
		((Control)THome).Click += THome_Click;
		componentResourceManager.ApplyResources(ZHome, "ZHome");
		((Control)ZHome).Name = "ZHome";
		((ButtonBase)ZHome).UseVisualStyleBackColor = true;
		((Control)ZHome).Click += ZHome_Click;
		componentResourceManager.ApplyResources(YHome, "YHome");
		((Control)YHome).Name = "YHome";
		((ButtonBase)YHome).UseVisualStyleBackColor = true;
		((Control)YHome).Click += YHome_Click;
		componentResourceManager.ApplyResources(XHome, "XHome");
		((Control)XHome).Name = "XHome";
		((ButtonBase)XHome).UseVisualStyleBackColor = true;
		((Control)XHome).Click += XHome_Click;
		((Control)Contrl).Controls.Add((Control)(object)lb_CirleTimes);
		((Control)Contrl).Controls.Add((Control)(object)txt_CircleTimes);
		((Control)Contrl).Controls.Add((Control)(object)ExitMove);
		((Control)Contrl).Controls.Add((Control)(object)ClearAxisPos);
		((Control)Contrl).Controls.Add((Control)(object)RestartProg);
		((Control)Contrl).Controls.Add((Control)(object)PauseProg);
		((Control)Contrl).Controls.Add((Control)(object)StartProg);
		componentResourceManager.ApplyResources(Contrl, "Contrl");
		((Control)Contrl).Name = "Contrl";
		Contrl.TabStop = false;
		componentResourceManager.ApplyResources(lb_CirleTimes, "lb_CirleTimes");
		((Control)lb_CirleTimes).Name = "lb_CirleTimes";
		componentResourceManager.ApplyResources(txt_CircleTimes, "txt_CircleTimes");
		((Control)txt_CircleTimes).Name = "txt_CircleTimes";
		((Control)txt_CircleTimes).TextChanged += txt_CircleTimes_TextChanged;
		componentResourceManager.ApplyResources(ExitMove, "ExitMove");
		((Control)ExitMove).Name = "ExitMove";
		((ButtonBase)ExitMove).UseVisualStyleBackColor = true;
		((Control)ExitMove).Click += ExitMove_Click;
		componentResourceManager.ApplyResources(ClearAxisPos, "ClearAxisPos");
		((Control)ClearAxisPos).Name = "ClearAxisPos";
		((ButtonBase)ClearAxisPos).UseVisualStyleBackColor = true;
		((Control)ClearAxisPos).Click += ClearAxisPos_Click;
		componentResourceManager.ApplyResources(RestartProg, "RestartProg");
		((Control)RestartProg).Name = "RestartProg";
		((ButtonBase)RestartProg).UseVisualStyleBackColor = true;
		((Control)RestartProg).Click += RestartProg_Click;
		componentResourceManager.ApplyResources(PauseProg, "PauseProg");
		((Control)PauseProg).Name = "PauseProg";
		((ButtonBase)PauseProg).UseVisualStyleBackColor = true;
		((Control)PauseProg).Click += PauseProg_Click;
		componentResourceManager.ApplyResources(StartProg, "StartProg");
		((Control)StartProg).Name = "StartProg";
		((ButtonBase)StartProg).UseVisualStyleBackColor = true;
		((Control)StartProg).Click += StartProg_Click;
		((Control)groupBox2).Controls.Add((Control)(object)splitContainer1);
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		((ToolStrip)statusStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[1] { (ToolStripItem)AutoProgMsg });
		componentResourceManager.ApplyResources(statusStrip1, "statusStrip1");
		((Control)statusStrip1).Name = "statusStrip1";
		((ToolStripItem)AutoProgMsg).Name = "AutoProgMsg";
		componentResourceManager.ApplyResources(AutoProgMsg, "AutoProgMsg");
		((Control)PicRunStt).BackColor = Color.Black;
		componentResourceManager.ApplyResources(PicRunStt, "PicRunStt");
		((Control)PicRunStt).Name = "PicRunStt";
		PicRunStt.TabStop = false;
		((Control)PicLinkStt).BackColor = Color.Black;
		componentResourceManager.ApplyResources(PicLinkStt, "PicLinkStt");
		((Control)PicLinkStt).Name = "PicLinkStt";
		PicLinkStt.TabStop = false;
		componentResourceManager.ApplyResources(lbRun, "lbRun");
		((Control)lbRun).Name = "lbRun";
		componentResourceManager.ApplyResources(lbLink, "lbLink");
		((Control)lbLink).Name = "lbLink";
		timer1.Interval = 200;
		timer1.Tick += timer1_Tick;
		((FileDialog)openFileDialog1).FileName = "openFileDialog1";
		componentResourceManager.ApplyResources(bn_CloseSp, "bn_CloseSp");
		((Control)bn_CloseSp).Name = "bn_CloseSp";
		((ButtonBase)bn_CloseSp).UseVisualStyleBackColor = true;
		((Control)bn_CloseSp).Click += bn_CloseSp_Click;
		componentResourceManager.ApplyResources(bn_OpenSp, "bn_OpenSp");
		((Control)bn_OpenSp).Name = "bn_OpenSp";
		((ButtonBase)bn_OpenSp).UseVisualStyleBackColor = true;
		((Control)bn_OpenSp).Click += bn_OpenSp_Click;
		componentResourceManager.ApplyResources(lb_SrPort, "lb_SrPort");
		((Control)lb_SrPort).Name = "lb_SrPort";
		((Control)lb_SrPort).DoubleClick += lb_SrPort_DoubleClick;
		((ListControl)cb_SpName).FormattingEnabled = true;
		componentResourceManager.ApplyResources(cb_SpName, "cb_SpName");
		((Control)cb_SpName).Name = "cb_SpName";
		((Control)cb_SpName).TextChanged += cb_SpName_TextChanged;
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)cbMovePulse);
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)cbUpdateInput);
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)cbUpdateOpEnc);
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)cbUpdateOpSpd);
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)chUpdateOpPos);
		((Control)cbUpdateOpPos).Controls.Add((Control)(object)cbUpdateOpStt);
		componentResourceManager.ApplyResources(cbUpdateOpPos, "cbUpdateOpPos");
		((Control)cbUpdateOpPos).Name = "cbUpdateOpPos";
		cbUpdateOpPos.TabStop = false;
		componentResourceManager.ApplyResources(cbMovePulse, "cbMovePulse");
		((Control)cbMovePulse).Name = "cbMovePulse";
		((ButtonBase)cbMovePulse).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbUpdateInput, "cbUpdateInput");
		cbUpdateInput.Checked = true;
		cbUpdateInput.CheckState = (CheckState)1;
		((Control)cbUpdateInput).Name = "cbUpdateInput";
		((ButtonBase)cbUpdateInput).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbUpdateOpEnc, "cbUpdateOpEnc");
		cbUpdateOpEnc.Checked = true;
		cbUpdateOpEnc.CheckState = (CheckState)1;
		((Control)cbUpdateOpEnc).Name = "cbUpdateOpEnc";
		((ButtonBase)cbUpdateOpEnc).UseVisualStyleBackColor = true;
		cbUpdateOpEnc.CheckedChanged += cbUpdateOpEnc_CheckedChanged;
		componentResourceManager.ApplyResources(cbUpdateOpSpd, "cbUpdateOpSpd");
		cbUpdateOpSpd.Checked = true;
		cbUpdateOpSpd.CheckState = (CheckState)1;
		((Control)cbUpdateOpSpd).Name = "cbUpdateOpSpd";
		((ButtonBase)cbUpdateOpSpd).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(chUpdateOpPos, "chUpdateOpPos");
		chUpdateOpPos.Checked = true;
		chUpdateOpPos.CheckState = (CheckState)1;
		((Control)chUpdateOpPos).Name = "chUpdateOpPos";
		((ButtonBase)chUpdateOpPos).UseVisualStyleBackColor = true;
		chUpdateOpPos.CheckedChanged += chUpdateOpPos_CheckedChanged;
		componentResourceManager.ApplyResources(cbUpdateOpStt, "cbUpdateOpStt");
		cbUpdateOpStt.Checked = true;
		cbUpdateOpStt.CheckState = (CheckState)1;
		((Control)cbUpdateOpStt).Name = "cbUpdateOpStt";
		((ButtonBase)cbUpdateOpStt).UseVisualStyleBackColor = true;
		((Control)tabCommCfg).Controls.Add((Control)(object)tabPage1);
		((Control)tabCommCfg).Controls.Add((Control)(object)tabPage2);
		((Control)tabCommCfg).Controls.Add((Control)(object)tabPage3);
		componentResourceManager.ApplyResources(tabCommCfg, "tabCommCfg");
		((Control)tabCommCfg).Name = "tabCommCfg";
		tabCommCfg.SelectedIndex = 0;
		((Control)tabPage1).BackColor = Color.Transparent;
		((Control)tabPage1).Controls.Add((Control)(object)bn_OpenSp);
		((Control)tabPage1).Controls.Add((Control)(object)bn_CloseSp);
		((Control)tabPage1).Controls.Add((Control)(object)cb_SpName);
		((Control)tabPage1).Controls.Add((Control)(object)lb_SrPort);
		componentResourceManager.ApplyResources(tabPage1, "tabPage1");
		((Control)tabPage1).Name = "tabPage1";
		((Control)tabPage2).BackColor = Color.Transparent;
		((Control)tabPage2).Controls.Add((Control)(object)tbTcpIPPort);
		((Control)tabPage2).Controls.Add((Control)(object)tbTcpIPAddr);
		((Control)tabPage2).Controls.Add((Control)(object)label2);
		((Control)tabPage2).Controls.Add((Control)(object)label1);
		((Control)tabPage2).Controls.Add((Control)(object)bn_OpenTCP);
		((Control)tabPage2).Controls.Add((Control)(object)bn_CloseTCP);
		componentResourceManager.ApplyResources(tabPage2, "tabPage2");
		((Control)tabPage2).Name = "tabPage2";
		componentResourceManager.ApplyResources(tbTcpIPPort, "tbTcpIPPort");
		((Control)tbTcpIPPort).Name = "tbTcpIPPort";
		((Control)tbTcpIPPort).TextChanged += tbTcpIPPort_TextChanged;
		componentResourceManager.ApplyResources(tbTcpIPAddr, "tbTcpIPAddr");
		((Control)tbTcpIPAddr).Name = "tbTcpIPAddr";
		((Control)tbTcpIPAddr).TextChanged += tbTcpIPAddr_TextChanged;
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(bn_OpenTCP, "bn_OpenTCP");
		((Control)bn_OpenTCP).Name = "bn_OpenTCP";
		((ButtonBase)bn_OpenTCP).UseVisualStyleBackColor = true;
		((Control)bn_OpenTCP).Click += bn_OpenTCP_Click;
		componentResourceManager.ApplyResources(bn_CloseTCP, "bn_CloseTCP");
		((Control)bn_CloseTCP).Name = "bn_CloseTCP";
		((ButtonBase)bn_CloseTCP).UseVisualStyleBackColor = true;
		((Control)bn_CloseTCP).Click += bn_CloseTCP_Click;
		((Control)tabPage3).Controls.Add((Control)(object)cbbCardIndex);
		((Control)tabPage3).Controls.Add((Control)(object)label11);
		componentResourceManager.ApplyResources(tabPage3, "tabPage3");
		((Control)tabPage3).Name = "tabPage3";
		tabPage3.UseVisualStyleBackColor = true;
		((ListControl)cbbCardIndex).FormattingEnabled = true;
		cbbCardIndex.Items.AddRange(new object[8]
		{
			componentResourceManager.GetString("cbbCardIndex.Items"),
			componentResourceManager.GetString("cbbCardIndex.Items1"),
			componentResourceManager.GetString("cbbCardIndex.Items2"),
			componentResourceManager.GetString("cbbCardIndex.Items3"),
			componentResourceManager.GetString("cbbCardIndex.Items4"),
			componentResourceManager.GetString("cbbCardIndex.Items5"),
			componentResourceManager.GetString("cbbCardIndex.Items6"),
			componentResourceManager.GetString("cbbCardIndex.Items7")
		});
		componentResourceManager.ApplyResources(cbbCardIndex, "cbbCardIndex");
		((Control)cbbCardIndex).Name = "cbbCardIndex";
		cbbCardIndex.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(rtbInfo, "rtbInfo");
		((Control)rtbInfo).Name = "rtbInfo";
		((Control)rtbInfo).DoubleClick += rtbInfo_DoubleClick;
		((ToolStripItem)tsmLUA).Name = "tsmLUA";
		componentResourceManager.ApplyResources(tsmLUA, "tsmLUA");
		((ToolStripItem)tsmLUA).Click += tsmLUA_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)rtbInfo);
		((Control)this).Controls.Add((Control)(object)tabCommCfg);
		((Control)this).Controls.Add((Control)(object)cbUpdateOpPos);
		((Control)this).Controls.Add((Control)(object)lbLink);
		((Control)this).Controls.Add((Control)(object)lbRun);
		((Control)this).Controls.Add((Control)(object)PicLinkStt);
		((Control)this).Controls.Add((Control)(object)PicRunStt);
		((Control)this).Controls.Add((Control)(object)statusStrip1);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)Contrl);
		((Control)this).Controls.Add((Control)(object)Homing);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Controls.Add((Control)(object)ProgrameList);
		((Control)this).Controls.Add((Control)(object)menuStrip1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MainMenuStrip = menuStrip1;
		((Form)this).MaximizeBox = false;
		((Control)this).Name = "MainFrm";
		((Form)this).FormClosed += new FormClosedEventHandler(MainFrm_FormClosed);
		((Form)this).Load += Form1_Load;
		((Control)splitContainer1.Panel1).ResumeLayout(false);
		((Control)splitContainer1.Panel1).PerformLayout();
		((Control)splitContainer1.Panel2).ResumeLayout(false);
		((ISupportInitialize)splitContainer1).EndInit();
		((Control)splitContainer1).ResumeLayout(false);
		((Control)tbCtrlMCInfo).ResumeLayout(false);
		((Control)tbPageLimitInfo).ResumeLayout(false);
		((Control)tbPageLimitInfo).PerformLayout();
		((Control)tbPageOther).ResumeLayout(false);
		((Control)tbPageOther).PerformLayout();
		((Control)menuStrip1).ResumeLayout(false);
		((Control)menuStrip1).PerformLayout();
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)Homing).ResumeLayout(false);
		((Control)Contrl).ResumeLayout(false);
		((Control)Contrl).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)statusStrip1).ResumeLayout(false);
		((Control)statusStrip1).PerformLayout();
		((ISupportInitialize)PicRunStt).EndInit();
		((ISupportInitialize)PicLinkStt).EndInit();
		((Control)cbUpdateOpPos).ResumeLayout(false);
		((Control)cbUpdateOpPos).PerformLayout();
		((Control)tabCommCfg).ResumeLayout(false);
		((Control)tabPage1).ResumeLayout(false);
		((Control)tabPage1).PerformLayout();
		((Control)tabPage2).ResumeLayout(false);
		((Control)tabPage2).PerformLayout();
		((Control)tabPage3).ResumeLayout(false);
		((Control)tabPage3).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
