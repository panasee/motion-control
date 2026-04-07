using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MCCDEMO;
using MccCustomLanguage;
using ZedGraph;

namespace ADTToolBox;

public class DataCurve : Form
{
	private enum eDebugLevelType
	{
		eDebugLevInfo,
		eDebugLevDebug,
		eDebugLevWarn,
		eDebugLevErr,
		eDebugLevException
	}

	private RichTextBox mInfoDebugBox;

	private MainFrm mMastrFrm;

	private CheckBox[] chbChanel = (CheckBox[])(object)new CheckBox[8];

	private ComboBox[] cbDev = (ComboBox[])(object)new ComboBox[8];

	private ComboBox[] cbDt = (ComboBox[])(object)new ComboBox[8];

	private byte[] SerialReadBuf = new byte[4196];

	private byte[] SerialReadNotProcessBuf = new byte[4196];

	private int DtCntNotProcess;

	private int ByteOffstProcess;

	private IContainer components;

	private GraphPane mCurvePane;

	private RollingPointPairList[] mCurvePointList;

	private LineItem[] mCurveItem;

	public ScopeDevice mScopeDev;

	private SaveFileDialog saveFileDialog1;

	private OpenFileDialog openFileDialog1;

	private SplitContainer splitScope;

	private Button bnStop;

	private Button DrawButton;

	private ZedGraphControl xyzPos12;

	private TabControl tbCfg;

	private TabPage tbScopeCfg;

	private Button bnDelete;

	private Button bnClose;

	private CheckBox checkBox8;

	private ComboBox comboBox23;

	private ComboBox comboBox24;

	private CheckBox checkBox7;

	private ComboBox comboBox20;

	private ComboBox comboBox21;

	private CheckBox checkBox6;

	private ComboBox comboBox17;

	private ComboBox comboBox18;

	private CheckBox checkBox5;

	private ComboBox comboBox14;

	private ComboBox comboBox15;

	private CheckBox checkBox4;

	private ComboBox comboBox11;

	private ComboBox comboBox12;

	private CheckBox checkBox3;

	private ComboBox comboBox8;

	private ComboBox comboBox9;

	private CheckBox checkBox2;

	private ComboBox comboBox5;

	private ComboBox comboBox6;

	private DomainUpDown SampleCounter;

	private DomainUpDown SampleTick;

	private Label CounterLable;

	private Label TickLable;

	private Label label6;

	private Label label7;

	private Label label8;

	private CheckBox checkBox1;

	private ComboBox comboBox2;

	private ComboBox comboBox3;

	private Button bnConfig;

	private TabPage tbScopeOp;

	private Button bnCfgExport;

	private Button bnCfgImport;

	private ListBox listBox1;

	private Button bnDtImport;

	private Button bnDtExport;

	private CheckBox XaxischeckBox;

	private GroupBox grp_SerialPortConfig;

	private Button bn_CloseSp;

	private Button bn_OpenSp;

	private Label lb_SrPort;

	private ComboBox cb_SpName;

	private SerialPort serlPrt;

	private Timer timer1;

	private Button bnSaveHistoryData;

	private Label lbSampledCnt;

	private Label lbFrameErrCntName;

	private Label label4;

	private Label label1;

	private Label label2;

	private Label lbFrameErrCnt;

	private Label lbFrameOkCnt;

	private Label lbUploadSampledCnt;

	private Button bnClearHistoryData;

	private void fun_PrintfInfo(string sInfo)
	{
		((Control)mInfoDebugBox).Text = DateTime.Now.ToString() + "\tINFO\t" + sInfo + Environment.NewLine + ((Control)mInfoDebugBox).Text;
	}

	private void fun_PrintfInfo(eDebugLevelType eLev, string sInfo)
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
		((Control)mInfoDebugBox).Text = DateTime.Now.ToString() + "\t" + text + sInfo + Environment.NewLine + ((Control)mInfoDebugBox).Text;
	}

	public DataCurve()
	{
		InitializeComponent();
	}

	public DataCurve(RichTextBox cInfoTextBox)
	{
		mInfoDebugBox = cInfoTextBox;
		InitializeComponent();
	}

	public DataCurve(RichTextBox cInfoTextBox, MainFrm MastrFrm)
	{
		mMastrFrm = MastrFrm;
		mInfoDebugBox = cInfoTextBox;
		InitializeComponent();
	}

	private void DataCurve_Load(object sender, EventArgs e)
	{
		int num = 0;
		mScopeDev = new ScopeDevice();
		for (num = 0; num < 8; num++)
		{
			switch (num)
			{
			case 0:
				mScopeDev.Channel[num].cLine = Color.Red;
				break;
			case 1:
				mScopeDev.Channel[num].cLine = Color.Blue;
				break;
			case 2:
				mScopeDev.Channel[num].cLine = Color.Violet;
				break;
			case 3:
				mScopeDev.Channel[num].cLine = Color.Green;
				break;
			case 4:
				mScopeDev.Channel[num].cLine = Color.Orange;
				break;
			case 5:
				mScopeDev.Channel[num].cLine = Color.Black;
				break;
			case 6:
				mScopeDev.Channel[num].cLine = Color.Violet;
				break;
			case 7:
				mScopeDev.Channel[num].cLine = Color.Brown;
				break;
			}
		}
		mCurvePointList = new RollingPointPairList[8];
		mCurveItem = new LineItem[8];
		mCurvePane = xyzPos12.GraphPane;
		mCurvePane.YAxis.MajorGrid.IsVisible = true;
		mCurvePane.XAxis.MajorGrid.IsVisible = true;
		xyzPos12.IsShowPointValues = true;
		xyzPos12.GraphPane.XAxis.Title.IsVisible = false;
		xyzPos12.GraphPane.YAxis.Title.IsVisible = false;
		xyzPos12.GraphPane.Title.IsVisible = false;
		for (num = 0; num < 8; num++)
		{
			mCurvePointList[num] = new RollingPointPairList(10000);
			mCurveItem[num] = mCurvePane.AddCurve("", mCurvePointList[num], mScopeDev.Channel[num].cLine, SymbolType.None);
		}
		xyzPos12.IsShowPointValues = true;
		xyzPos12.AxisChange();
		((Control)xyzPos12).Invalidate();
		cbDev[0] = comboBox3;
		cbDev[1] = comboBox6;
		cbDev[2] = comboBox9;
		cbDev[3] = comboBox12;
		cbDev[4] = comboBox15;
		cbDev[5] = comboBox18;
		cbDev[6] = comboBox21;
		cbDev[7] = comboBox24;
		cbDt[0] = comboBox2;
		cbDt[1] = comboBox5;
		cbDt[2] = comboBox8;
		cbDt[3] = comboBox11;
		cbDt[4] = comboBox14;
		cbDt[5] = comboBox17;
		cbDt[6] = comboBox20;
		cbDt[7] = comboBox23;
		chbChanel[0] = checkBox1;
		chbChanel[1] = checkBox2;
		chbChanel[2] = checkBox3;
		chbChanel[3] = checkBox4;
		chbChanel[4] = checkBox5;
		chbChanel[5] = checkBox6;
		chbChanel[6] = checkBox7;
		chbChanel[7] = checkBox8;
		for (num = 0; num < 8; num++)
		{
			chbChanel[num].Checked = false;
			((Control)chbChanel[num]).Enabled = true;
			((ListControl)cbDev[num]).SelectedIndex = 0;
			((Control)cbDev[num]).Enabled = false;
			((ListControl)cbDt[num]).SelectedIndex = 0;
			((Control)cbDt[num]).Enabled = false;
		}
		((Control)cb_SpName).Text = "";
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
		fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ScopeVersionInfo + 538379798u.ToString("x"));
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		CheckBox val = (CheckBox)sender;
		uint num = uint.Parse(((Control)val).Name.ToString().Substring(((Control)val).Name.ToString().Length - 1, 1)) - 1;
		if (num != 0 && !chbChanel[num - 1].Checked)
		{
			chbChanel[num].Checked = false;
		}
		else if (chbChanel[num].Checked)
		{
			((ListControl)cbDev[num]).SelectedIndex = 0;
			((Control)cbDev[num]).Enabled = true;
			((ListControl)cbDt[num]).SelectedIndex = 0;
			((Control)cbDt[num]).Enabled = true;
		}
		else
		{
			((Control)cbDev[num]).Enabled = false;
			((Control)cbDt[num]).Enabled = false;
		}
	}

	private void DrawButton_Click(object sender, EventArgs e)
	{
		if (mScopeDev.dwChannelSum == 0)
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_NoChannelPleaseConfigThemFirst);
			return;
		}
		DtCntNotProcess = 0;
		ByteOffstProcess = 0;
		mScopeDev.dwCntIndx = 0u;
		mScopeDev.dwCheckSumValidCnt = 0u;
		mScopeDev.dwCheckSumErrCnt = 0u;
		for (int i = 0; i < 8; i++)
		{
			mCurvePointList[i].Clear();
		}
		xyzPos12.AxisChange();
		((Control)xyzPos12).Invalidate();
		timer1.Enabled = true;
		if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_Start((byte)mScopeDev.dwChannelSum, mScopeDev.dwSampleCnt, (byte)mScopeDev.dwSampleTick))
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_SendStartSampleCommandFailed);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		if (mScopeDev.dwChannelSum != 0)
		{
			CfgChannelCmndInfo cfgChannelCmndInfo = new CfgChannelCmndInfo
			{
				Channel = new ChannelCfgInfoType[8]
			};
			int num = 0;
			for (num = 0; num < mScopeDev.dwChannelSum; num++)
			{
				cfgChannelCmndInfo.Channel[num].dwChannelIndex = mScopeDev.Channel[num].dwChannelIndx;
				cfgChannelCmndInfo.Channel[num].dwDevId = mScopeDev.Channel[num].dwDevId;
				cfgChannelCmndInfo.Channel[num].dwItemInfo = mScopeDev.Channel[num].dwItem;
				if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_SetChannelPara((byte)cfgChannelCmndInfo.Channel[num].dwChannelIndex, (byte)cfgChannelCmndInfo.Channel[num].dwDevId, (byte)cfgChannelCmndInfo.Channel[num].dwItemInfo))
				{
					fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_SendSomeChannelConfigCommandFailed, cfgChannelCmndInfo.Channel[num].dwChannelIndex));
					break;
				}
			}
			if (num == mScopeDev.dwChannelSum)
			{
				fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ConfigAllChannelSucceed);
			}
		}
		else
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_NotConfigChannelCanNotDownloadChannelParameters);
		}
	}

	private void bnDelete_Click(object sender, EventArgs e)
	{
		if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_Reset())
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_DeleteScopConfigFailed);
		}
	}

	private void bnClose_Click(object sender, EventArgs e)
	{
		if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_Stop())
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_CloseScopFailed);
		}
	}

	private void bnCfgExport_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).Filter = "cfg files(*.cfg)|*.cfg";
		((FileDialog)val).FileName = "ScopeChanelCfg.cfg";
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				string text = ((FileDialog)val).FileName.ToString();
				FileStream fileStream = new FileStream(text, FileMode.Create);
				stScopeSaveOpenType stScopeSaveOpenType2 = new stScopeSaveOpenType
				{
					Channel = new stScopeSaveOpenChannelType[8]
				};
				int num = 0;
				stScopeSaveOpenType2.dwSampleTick = mScopeDev.dwSampleTick;
				stScopeSaveOpenType2.dwSampleCnt = mScopeDev.dwSampleCnt;
				stScopeSaveOpenType2.dwChannelSum = mScopeDev.dwChannelSum;
				stScopeSaveOpenType2.dwChannelEnableBit = mScopeDev.dwChannelEnableBit;
				for (num = 0; num < 8; num++)
				{
					stScopeSaveOpenType2.Channel[num].dwChannelIndx = mScopeDev.Channel[num].dwChannelIndx;
					stScopeSaveOpenType2.Channel[num].dwDevId = mScopeDev.Channel[num].dwDevId;
					stScopeSaveOpenType2.Channel[num].dwItem = mScopeDev.Channel[num].dwItem;
				}
				int num2 = Marshal.SizeOf((object)stScopeSaveOpenType2);
				byte[] buffer = CustomMethord.StructToBytes(stScopeSaveOpenType2, num2);
				fileStream.Write(buffer, 0, num2);
				fileStream.Close();
				fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_SaveScopeConfigFileSucceed, text.ToString()));
			}
		}
		catch (Exception ex)
		{
			fun_PrintfInfo(ex.ToString());
		}
	}

	private void bnCfgImport_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog();
		((FileDialog)val).Filter = "cfg files(*.cfg)|*.cfg";
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() != 1)
			{
				return;
			}
			stScopeSaveOpenType stScopeSaveOpenType2 = new stScopeSaveOpenType
			{
				Channel = new stScopeSaveOpenChannelType[8]
			};
			string text = ((FileDialog)val).FileName.ToString();
			FileStream fileStream = new FileStream(text, FileMode.Open);
			int num = Marshal.SizeOf((object)stScopeSaveOpenType2);
			byte[] array = new byte[num];
			int num2 = 0;
			if (num == fileStream.Read(array, 0, num))
			{
				stScopeSaveOpenType2 = (stScopeSaveOpenType)CustomMethord.ByteToStruct(array, typeof(stScopeSaveOpenType));
				mScopeDev.dwSampleTick = stScopeSaveOpenType2.dwSampleTick;
				mScopeDev.dwSampleCnt = stScopeSaveOpenType2.dwSampleCnt;
				mScopeDev.dwChannelSum = stScopeSaveOpenType2.dwChannelSum;
				mScopeDev.dwChannelEnableBit = stScopeSaveOpenType2.dwChannelEnableBit;
				for (num2 = 0; num2 < 8; num2++)
				{
					mScopeDev.Channel[num2].dwChannelIndx = stScopeSaveOpenType2.Channel[num2].dwChannelIndx;
					mScopeDev.Channel[num2].dwDevId = stScopeSaveOpenType2.Channel[num2].dwDevId;
					mScopeDev.Channel[num2].dwItem = stScopeSaveOpenType2.Channel[num2].dwItem;
					mScopeDev.Channel[num2].sLineName = "";
				}
				fileStream.Close();
				for (num2 = 0; num2 < 8; num2++)
				{
					if (1 << num2 == ((1 << num2) & mScopeDev.dwChannelEnableBit))
					{
						chbChanel[num2].Checked = true;
						((ListControl)cbDev[num2]).SelectedIndex = (int)mScopeDev.Channel[num2].dwDevId;
						((ListControl)cbDt[num2]).SelectedIndex = (int)mScopeDev.Channel[num2].dwItem;
						mScopeDev.Channel[num2].sLineName = num2 + "通道" + ((Control)cbDev[num2]).Text + ((Control)cbDt[num2]).Text;
					}
				}
				((Control)SampleTick).Text = mScopeDev.dwSampleTick.ToString();
				((Control)SampleCounter).Text = mScopeDev.dwSampleCnt.ToString();
				fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_LoadScopeConfigFileSucceed, text.ToString()));
			}
			else
			{
				fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ConfigFileEmpty);
			}
		}
		catch (Exception ex)
		{
			fun_PrintfInfo(ex.ToString());
		}
	}

	private void bnConfig_Click(object sender, EventArgs e)
	{
		uint num = 0u;
		if (false)
		{
			return;
		}
		mScopeDev.dwChannelEnableBit = 0u;
		mScopeDev.dwChannelSum = 0u;
		mScopeDev.dwSampleTick = uint.Parse(((Control)SampleTick).Text);
		mScopeDev.dwSampleCnt = uint.Parse(((Control)SampleCounter).Text);
		for (num = 0u; num < 8; num++)
		{
			if (chbChanel[num].Checked)
			{
				uint num2 = 1u;
				mScopeDev.dwChannelSum++;
				mScopeDev.Channel[num].dwChannelIndx = num;
				mScopeDev.Channel[num].dwDevId = (uint)((ListControl)cbDev[num]).SelectedIndex;
				mScopeDev.Channel[num].dwItem = (uint)((ListControl)cbDt[num]).SelectedIndex;
				mScopeDev.Channel[num].sLineName = num + GlobalData.GlobalLanguage.ScopeFrm_WordChannel + ((Control)cbDev[num]).Text + ((Control)cbDt[num]).Text;
				mScopeDev.dwChannelEnableBit |= num2 << (int)num;
				fun_PrintfInfo(mScopeDev.Channel[num].sLineName);
			}
			else
			{
				mScopeDev.Channel[num].dwChannelIndx = 10u;
				mScopeDev.Channel[num].dwDevId = 0u;
				mScopeDev.Channel[num].dwItem = 0u;
				mScopeDev.Channel[num].sLineName = "";
			}
		}
		fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ChannelConfigDonDownloadConfigParameters);
		Thread.Sleep(200);
		fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_OpenAndDeleteAllConfigInfo);
		if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_Reset())
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_OpenScopeFailed);
			return;
		}
		fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_StartDownloadConfigParametes);
		Thread.Sleep(200);
		button1_Click(sender, e);
	}

	private void bnDtExport_Click(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between I4 and Unknown
		try
		{
			string text = "";
			((FileDialog)saveFileDialog1).Filter = "数据文件(*.dat)|*.dat";
			((FileDialog)saveFileDialog1).FileName = "CurveData.dat";
			((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
			if (1 != (int)((CommonDialog)saveFileDialog1).ShowDialog())
			{
				return;
			}
			text = ((FileDialog)saveFileDialog1).FileName;
			if (text != "")
			{
				ushort num = 0;
				ushort num2 = 0;
				StreamWriter streamWriter = new StreamWriter(text, append: false, Encoding.ASCII);
				for (num = 0; num < 10000; num++)
				{
					string text2 = "";
					for (num2 = 0; num2 < 8; num2++)
					{
						text2 = text2 + mScopeDev.Channel[num2].mValue[num].ToString("F3") + "\t";
					}
					streamWriter.WriteLine(text2);
				}
				streamWriter.Close();
				fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_SaveFileTo + text);
			}
			else
			{
				fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_CanNotSaveFileBecauseFileNameNull);
			}
		}
		catch (Exception ex)
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_SaveFileFailed);
			Console.WriteLine(ex.ToString());
		}
	}

	private void bnDtImport_Click(object sender, EventArgs e)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between I4 and Unknown
		try
		{
			string text = "";
			((FileDialog)openFileDialog1).Filter = "*(*.dat)|*.dat";
			((FileDialog)openFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
			if (1 != (int)((CommonDialog)openFileDialog1).ShowDialog())
			{
				return;
			}
			ushort num = 0;
			ushort num2 = 0;
			for (num = 0; num < 8; num++)
			{
				mCurvePointList[num].Clear();
			}
			xyzPos12.AxisChange();
			((Control)xyzPos12).Invalidate();
			text = ((FileDialog)openFileDialog1).FileName;
			StreamReader streamReader = new StreamReader(text);
			while (!streamReader.EndOfStream)
			{
				string[] array = streamReader.ReadLine().Split(new string[1] { "\t" }, StringSplitOptions.None);
				for (num = 0; num < 8; num++)
				{
					mScopeDev.Channel[num].mValue[num2] = double.Parse(array[num]);
				}
				num2++;
			}
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_OpenFileSucceed, text));
			streamReader.Close();
			for (num = 0; num < 8; num++)
			{
				double[] array2 = new double[num2];
				Color color = Color.Red;
				ushort num3 = 0;
				mCurvePointList[num].Clear();
				for (num3 = 0; num3 < num2; num3++)
				{
					array2[num3] = (int)num3;
				}
				mCurvePointList[num].Add(array2, mScopeDev.Channel[num].mValue);
				switch (num)
				{
				case 0:
					color = Color.Red;
					break;
				case 1:
					color = Color.Blue;
					break;
				case 2:
					color = Color.Violet;
					break;
				case 3:
					color = Color.Green;
					break;
				case 4:
					color = Color.Orange;
					break;
				case 5:
					color = Color.Black;
					break;
				case 6:
					color = Color.Violet;
					break;
				case 7:
					color = Color.Brown;
					break;
				}
				mCurveItem[num] = mCurvePane.AddCurve("", mCurvePointList[num], color, SymbolType.None);
				xyzPos12.IsShowPointValues = true;
				xyzPos12.AxisChange();
				((Control)xyzPos12).Invalidate();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	private void tbCfg_SelectedIndexChanged(object sender, EventArgs e)
	{
		if ("tbScopeOp" == ((Control)tbCfg.TabPages[tbCfg.SelectedIndex]).Name)
		{
			listBox1.Items.Clear();
			ushort num = 0;
			string text = "";
			for (num = 0; num < 8; num++)
			{
				text = ((10 == mScopeDev.Channel[num].dwChannelIndx) ? (num + GlobalData.GlobalLanguage.ScopeFrm_WordChannel + "\t" + GlobalData.GlobalLanguage.ScopeFrm_WordNotConfig) : (num + GlobalData.GlobalLanguage.ScopeFrm_WordChannel + ":\t" + ((Control)cbDev[num]).Text + ((Control)cbDt[num]).Text));
				listBox1.Items.Add((object)text);
			}
		}
	}

	private void bnStop_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		if (mMastrFrm.Mccard.FunResErr == mMastrFrm.Mccard.MoCtrCard_Scope_Stop())
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_SendStopSampleCommandFailed);
		}
	}

	private void XaxischeckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (XaxischeckBox.Checked)
		{
			((Control)checkBox1).Enabled = true;
			((Control)checkBox1).Text = "ScopeX";
			((Control)checkBox2).Enabled = true;
			((Control)checkBox2).Text = "ScopeY";
			((Control)checkBox3).Enabled = false;
			((Control)checkBox4).Enabled = false;
			((Control)checkBox5).Enabled = false;
			((Control)checkBox6).Enabled = false;
			((Control)checkBox7).Enabled = false;
			((Control)checkBox8).Enabled = false;
		}
		else
		{
			((Control)checkBox1).Enabled = true;
			((Control)checkBox1).Text = GlobalData.GlobalLanguage.ScopeFrm_WordChannel + "0";
			((Control)checkBox2).Enabled = true;
			((Control)checkBox2).Text = GlobalData.GlobalLanguage.ScopeFrm_WordChannel + "1";
			((Control)checkBox3).Enabled = true;
			((Control)checkBox4).Enabled = true;
			((Control)checkBox5).Enabled = true;
			((Control)checkBox6).Enabled = true;
			((Control)checkBox7).Enabled = true;
			((Control)checkBox8).Enabled = true;
		}
	}

	private void lb_SrPort_Click(object sender, EventArgs e)
	{
		((Control)cb_SpName).Text = "";
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
	}

	private int fun_InitialSp(string SerialPortName)
	{
		try
		{
			serlPrt.PortName = SerialPortName;
			if (serlPrt.IsOpen)
			{
				serlPrt.Close();
			}
			serlPrt.ReadTimeout = 500;
			serlPrt.ReceivedBytesThreshold = 1024;
			serlPrt.RtsEnable = false;
			serlPrt.WriteTimeout = 500;
			serlPrt.ReadBufferSize = 4096;
			serlPrt.WriteBufferSize = 4096;
			serlPrt.BaudRate = 115200;
			serlPrt.Parity = (Parity)0;
			serlPrt.DataBits = 8;
			serlPrt.StopBits = (StopBits)1;
			serlPrt.Encoding = Encoding.GetEncoding("Gb2312");
			serlPrt.Open();
			return 0;
		}
		catch (Exception)
		{
			return 1;
		}
	}

	private void bn_OpenSp_Click(object sender, EventArgs e)
	{
		if (1 == fun_InitialSp(((Control)cb_SpName).Text))
		{
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_OpenPortFailed, ((Control)cb_SpName).Text));
		}
		else
		{
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.ScopeFrm_OpenPortSucceed, ((Control)cb_SpName).Text));
		}
	}

	private void bn_CloseSp_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		try
		{
			serlPrt.Close();
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ClosePortSucceed);
		}
		catch (Exception)
		{
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_ClosePortFailed);
		}
	}

	private void DataCurve_FormClosed(object sender, FormClosedEventArgs e)
	{
		bn_CloseSp_Click(sender, (EventArgs)(object)e);
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!serlPrt.IsOpen || serlPrt.BytesToRead <= 0)
		{
			return;
		}
		int num = 0;
		int bytesToRead = serlPrt.BytesToRead;
		serlPrt.Read(SerialReadBuf, DtCntNotProcess, bytesToRead);
		ByteOffstProcess = 0;
		DtCntNotProcess += bytesToRead;
		num = DtCntNotProcess;
		int num2 = 0;
		while (num2 < num)
		{
			if (170 == SerialReadBuf[num2])
			{
				if (num2 + 3 >= num)
				{
					Array.Copy(SerialReadBuf, ByteOffstProcess, SerialReadNotProcessBuf, 0, DtCntNotProcess);
					break;
				}
				if (85 == SerialReadBuf[num2 + 3])
				{
					int num3 = SerialReadBuf[num2 + 1];
					if (DtCntNotProcess < num3)
					{
						Array.Copy(SerialReadBuf, ByteOffstProcess, SerialReadNotProcessBuf, 0, DtCntNotProcess);
						break;
					}
					byte[] array = new byte[40];
					Array.Copy(SerialReadBuf, num2, array, 0, num3);
					byte b = 0;
					byte b2 = array[2];
					array[2] = 0;
					for (int i = 0; i < num3; i++)
					{
						b += array[i];
					}
					if (b == b2)
					{
						stListElementType item = new stListElementType
						{
							mDtVal = new double[8]
						};
						ScopeUploadDtType scopeUploadDtType = default(ScopeUploadDtType);
						scopeUploadDtType = (ScopeUploadDtType)CustomMethord.ByteToStruct(array, typeof(ScopeUploadDtType));
						mScopeDev.dwSampledCnt = scopeUploadDtType.SampleCnt;
						mScopeDev.dwCntIndx++;
						for (int j = 0; j < mScopeDev.dwChannelSum; j++)
						{
							if (mScopeDev.Channel[j].dwItem == 0)
							{
								if (j == 0)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal0;
								}
								else if (1 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal1;
								}
								else if (2 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal2;
								}
								else if (3 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal3;
								}
								else if (4 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal4;
								}
								else if (5 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal5;
								}
								else if (6 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal6;
								}
								else if (7 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.FVal7;
								}
							}
							else if (1 == mScopeDev.Channel[j].dwItem)
							{
								if (j == 0)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal0;
								}
								else if (1 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal1;
								}
								else if (2 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal2;
								}
								else if (3 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal3;
								}
								else if (4 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal4;
								}
								else if (5 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal5;
								}
								else if (6 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal6;
								}
								else if (7 == j)
								{
									item.mDtVal[j] = scopeUploadDtType.IVal7;
								}
							}
							else
							{
								item.mDtVal[j] = 0.0;
							}
							mScopeDev.Channel[j].WriteDtToBuff(item.mDtVal[j]);
							mCurvePointList[j].Add(mScopeDev.dwCntIndx, item.mDtVal[j]);
						}
						mScopeDev.mDtList.Add(item);
						num2 += num3;
						ByteOffstProcess += num3;
						DtCntNotProcess -= num3;
						mScopeDev.dwCheckSumValidCnt++;
					}
					else
					{
						num2++;
						ByteOffstProcess++;
						DtCntNotProcess--;
						mScopeDev.dwCheckSumErrCnt++;
					}
				}
				else
				{
					num2++;
					ByteOffstProcess++;
					DtCntNotProcess--;
				}
			}
			else
			{
				num2++;
				ByteOffstProcess++;
				DtCntNotProcess--;
			}
		}
		Array.Clear(SerialReadBuf, 0, SerialReadBuf.Length);
		Array.Copy(SerialReadNotProcessBuf, 0, SerialReadBuf, 0, DtCntNotProcess);
		((Control)lbFrameErrCnt).Text = mScopeDev.dwCheckSumErrCnt.ToString();
		((Control)lbFrameOkCnt).Text = mScopeDev.dwCheckSumValidCnt.ToString();
		((Control)lbSampledCnt).Text = mScopeDev.dwCntIndx.ToString();
		((Control)lbUploadSampledCnt).Text = mScopeDev.dwSampledCnt.ToString();
		xyzPos12.AxisChange();
		((Control)xyzPos12).Invalidate();
	}

	private void bnSaveHistoryData_Click(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).Filter = "dat files(*.dat)|*.dat";
		((FileDialog)val).FileName = "dataHistory.dat";
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() != 1)
			{
				return;
			}
			StreamWriter streamWriter = new StreamWriter(((FileDialog)val).FileName.ToString(), append: false, Encoding.ASCII);
			foreach (stListElementType mDt in mScopeDev.mDtList)
			{
				string value = string.Format("{0:F6}\t{1:F6}\t{2:F6}\t{3:F6}\t{4:F6}\t{5:F6}\t{6:F6}\t{7:F6}", new object[8]
				{
					mDt.mDtVal[0],
					mDt.mDtVal[1],
					mDt.mDtVal[2],
					mDt.mDtVal[3],
					mDt.mDtVal[4],
					mDt.mDtVal[5],
					mDt.mDtVal[6],
					mDt.mDtVal[7]
				});
				streamWriter.WriteLine(value);
			}
			streamWriter.Close();
			fun_PrintfInfo(GlobalData.GlobalLanguage.ScopeFrm_SaveHistoryDataSucceed);
		}
		catch (Exception ex)
		{
			fun_PrintfInfo(ex.ToString());
		}
	}

	private void bnClearHistoryData_Click(object sender, EventArgs e)
	{
		mScopeDev.mDtList.Clear();
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
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Expected O, but got Unknown
		//IL_2065: Unknown result type (might be due to invalid IL or missing references)
		//IL_206f: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataCurve));
		saveFileDialog1 = new SaveFileDialog();
		openFileDialog1 = new OpenFileDialog();
		splitScope = new SplitContainer();
		tbCfg = new TabControl();
		tbScopeCfg = new TabPage();
		bnClearHistoryData = new Button();
		lbFrameErrCnt = new Label();
		lbFrameOkCnt = new Label();
		lbUploadSampledCnt = new Label();
		lbSampledCnt = new Label();
		lbFrameErrCntName = new Label();
		label4 = new Label();
		label1 = new Label();
		label2 = new Label();
		bnSaveHistoryData = new Button();
		grp_SerialPortConfig = new GroupBox();
		bn_CloseSp = new Button();
		bn_OpenSp = new Button();
		lb_SrPort = new Label();
		cb_SpName = new ComboBox();
		XaxischeckBox = new CheckBox();
		checkBox8 = new CheckBox();
		comboBox23 = new ComboBox();
		comboBox24 = new ComboBox();
		checkBox7 = new CheckBox();
		comboBox20 = new ComboBox();
		comboBox21 = new ComboBox();
		checkBox6 = new CheckBox();
		comboBox17 = new ComboBox();
		comboBox18 = new ComboBox();
		checkBox5 = new CheckBox();
		comboBox14 = new ComboBox();
		comboBox15 = new ComboBox();
		checkBox4 = new CheckBox();
		comboBox11 = new ComboBox();
		comboBox12 = new ComboBox();
		checkBox3 = new CheckBox();
		comboBox8 = new ComboBox();
		comboBox9 = new ComboBox();
		checkBox2 = new CheckBox();
		comboBox5 = new ComboBox();
		comboBox6 = new ComboBox();
		SampleCounter = new DomainUpDown();
		SampleTick = new DomainUpDown();
		CounterLable = new Label();
		TickLable = new Label();
		label6 = new Label();
		label7 = new Label();
		label8 = new Label();
		checkBox1 = new CheckBox();
		comboBox2 = new ComboBox();
		comboBox3 = new ComboBox();
		bnConfig = new Button();
		bnCfgExport = new Button();
		bnCfgImport = new Button();
		bnDelete = new Button();
		bnClose = new Button();
		tbScopeOp = new TabPage();
		bnDtImport = new Button();
		bnDtExport = new Button();
		listBox1 = new ListBox();
		bnStop = new Button();
		DrawButton = new Button();
		xyzPos12 = new ZedGraphControl();
		serlPrt = new SerialPort(components);
		timer1 = new Timer(components);
		((ISupportInitialize)splitScope).BeginInit();
		((Control)splitScope.Panel1).SuspendLayout();
		((Control)splitScope.Panel2).SuspendLayout();
		((Control)splitScope).SuspendLayout();
		((Control)tbCfg).SuspendLayout();
		((Control)tbScopeCfg).SuspendLayout();
		((Control)grp_SerialPortConfig).SuspendLayout();
		((Control)tbScopeOp).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(saveFileDialog1, "saveFileDialog1");
		((FileDialog)openFileDialog1).FileName = "openFileDialog1";
		componentResourceManager.ApplyResources(openFileDialog1, "openFileDialog1");
		componentResourceManager.ApplyResources(splitScope, "splitScope");
		splitScope.BorderStyle = (BorderStyle)1;
		((Control)splitScope).Name = "splitScope";
		componentResourceManager.ApplyResources(splitScope.Panel1, "splitScope.Panel1");
		((Control)splitScope.Panel1).Controls.Add((Control)(object)tbCfg);
		componentResourceManager.ApplyResources(splitScope.Panel2, "splitScope.Panel2");
		((Control)splitScope.Panel2).Controls.Add((Control)(object)xyzPos12);
		componentResourceManager.ApplyResources(tbCfg, "tbCfg");
		((Control)tbCfg).Controls.Add((Control)(object)tbScopeCfg);
		((Control)tbCfg).Controls.Add((Control)(object)tbScopeOp);
		((Control)tbCfg).Name = "tbCfg";
		tbCfg.SelectedIndex = 0;
		tbCfg.SelectedIndexChanged += tbCfg_SelectedIndexChanged;
		componentResourceManager.ApplyResources(tbScopeCfg, "tbScopeCfg");
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnClearHistoryData);
		((Control)tbScopeCfg).Controls.Add((Control)(object)lbFrameErrCnt);
		((Control)tbScopeCfg).Controls.Add((Control)(object)lbFrameOkCnt);
		((Control)tbScopeCfg).Controls.Add((Control)(object)lbUploadSampledCnt);
		((Control)tbScopeCfg).Controls.Add((Control)(object)lbSampledCnt);
		((Control)tbScopeCfg).Controls.Add((Control)(object)lbFrameErrCntName);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label4);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label1);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label2);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnSaveHistoryData);
		((Control)tbScopeCfg).Controls.Add((Control)(object)grp_SerialPortConfig);
		((Control)tbScopeCfg).Controls.Add((Control)(object)XaxischeckBox);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox8);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox23);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox24);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox7);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox20);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox21);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox6);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox17);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox18);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox5);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox14);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox15);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox4);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox11);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox12);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox3);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox8);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox9);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox2);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox5);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox6);
		((Control)tbScopeCfg).Controls.Add((Control)(object)SampleCounter);
		((Control)tbScopeCfg).Controls.Add((Control)(object)SampleTick);
		((Control)tbScopeCfg).Controls.Add((Control)(object)CounterLable);
		((Control)tbScopeCfg).Controls.Add((Control)(object)TickLable);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label6);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label7);
		((Control)tbScopeCfg).Controls.Add((Control)(object)label8);
		((Control)tbScopeCfg).Controls.Add((Control)(object)checkBox1);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox2);
		((Control)tbScopeCfg).Controls.Add((Control)(object)comboBox3);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnConfig);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnCfgExport);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnCfgImport);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnDelete);
		((Control)tbScopeCfg).Controls.Add((Control)(object)bnClose);
		((Control)tbScopeCfg).Name = "tbScopeCfg";
		tbScopeCfg.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnClearHistoryData, "bnClearHistoryData");
		((Control)bnClearHistoryData).Name = "bnClearHistoryData";
		((ButtonBase)bnClearHistoryData).UseVisualStyleBackColor = true;
		((Control)bnClearHistoryData).Click += bnClearHistoryData_Click;
		componentResourceManager.ApplyResources(lbFrameErrCnt, "lbFrameErrCnt");
		lbFrameErrCnt.BorderStyle = (BorderStyle)1;
		((Control)lbFrameErrCnt).Name = "lbFrameErrCnt";
		componentResourceManager.ApplyResources(lbFrameOkCnt, "lbFrameOkCnt");
		lbFrameOkCnt.BorderStyle = (BorderStyle)1;
		((Control)lbFrameOkCnt).Name = "lbFrameOkCnt";
		componentResourceManager.ApplyResources(lbUploadSampledCnt, "lbUploadSampledCnt");
		lbUploadSampledCnt.BorderStyle = (BorderStyle)1;
		((Control)lbUploadSampledCnt).Name = "lbUploadSampledCnt";
		componentResourceManager.ApplyResources(lbSampledCnt, "lbSampledCnt");
		lbSampledCnt.BorderStyle = (BorderStyle)1;
		((Control)lbSampledCnt).Name = "lbSampledCnt";
		componentResourceManager.ApplyResources(lbFrameErrCntName, "lbFrameErrCntName");
		((Control)lbFrameErrCntName).Name = "lbFrameErrCntName";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(bnSaveHistoryData, "bnSaveHistoryData");
		((Control)bnSaveHistoryData).Name = "bnSaveHistoryData";
		((ButtonBase)bnSaveHistoryData).UseVisualStyleBackColor = true;
		((Control)bnSaveHistoryData).Click += bnSaveHistoryData_Click;
		componentResourceManager.ApplyResources(grp_SerialPortConfig, "grp_SerialPortConfig");
		((Control)grp_SerialPortConfig).Controls.Add((Control)(object)bn_CloseSp);
		((Control)grp_SerialPortConfig).Controls.Add((Control)(object)bn_OpenSp);
		((Control)grp_SerialPortConfig).Controls.Add((Control)(object)lb_SrPort);
		((Control)grp_SerialPortConfig).Controls.Add((Control)(object)cb_SpName);
		((Control)grp_SerialPortConfig).Name = "grp_SerialPortConfig";
		grp_SerialPortConfig.TabStop = false;
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
		((Control)lb_SrPort).Click += lb_SrPort_Click;
		componentResourceManager.ApplyResources(cb_SpName, "cb_SpName");
		((ListControl)cb_SpName).FormattingEnabled = true;
		((Control)cb_SpName).Name = "cb_SpName";
		componentResourceManager.ApplyResources(XaxischeckBox, "XaxischeckBox");
		((Control)XaxischeckBox).Name = "XaxischeckBox";
		((ButtonBase)XaxischeckBox).UseVisualStyleBackColor = true;
		XaxischeckBox.CheckedChanged += XaxischeckBox_CheckedChanged;
		componentResourceManager.ApplyResources(checkBox8, "checkBox8");
		((Control)checkBox8).Name = "checkBox8";
		((ButtonBase)checkBox8).UseVisualStyleBackColor = true;
		checkBox8.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox23, "comboBox23");
		((ListControl)comboBox23).FormattingEnabled = true;
		comboBox23.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox23.Items"),
			componentResourceManager.GetString("comboBox23.Items1")
		});
		((Control)comboBox23).Name = "comboBox23";
		componentResourceManager.ApplyResources(comboBox24, "comboBox24");
		((ListControl)comboBox24).FormattingEnabled = true;
		comboBox24.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox24.Items"),
			componentResourceManager.GetString("comboBox24.Items1"),
			componentResourceManager.GetString("comboBox24.Items2"),
			componentResourceManager.GetString("comboBox24.Items3"),
			componentResourceManager.GetString("comboBox24.Items4"),
			componentResourceManager.GetString("comboBox24.Items5")
		});
		((Control)comboBox24).Name = "comboBox24";
		componentResourceManager.ApplyResources(checkBox7, "checkBox7");
		((Control)checkBox7).Name = "checkBox7";
		((ButtonBase)checkBox7).UseVisualStyleBackColor = true;
		checkBox7.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox20, "comboBox20");
		((ListControl)comboBox20).FormattingEnabled = true;
		comboBox20.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox20.Items"),
			componentResourceManager.GetString("comboBox20.Items1")
		});
		((Control)comboBox20).Name = "comboBox20";
		componentResourceManager.ApplyResources(comboBox21, "comboBox21");
		((ListControl)comboBox21).FormattingEnabled = true;
		comboBox21.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox21.Items"),
			componentResourceManager.GetString("comboBox21.Items1"),
			componentResourceManager.GetString("comboBox21.Items2"),
			componentResourceManager.GetString("comboBox21.Items3"),
			componentResourceManager.GetString("comboBox21.Items4"),
			componentResourceManager.GetString("comboBox21.Items5")
		});
		((Control)comboBox21).Name = "comboBox21";
		componentResourceManager.ApplyResources(checkBox6, "checkBox6");
		((Control)checkBox6).Name = "checkBox6";
		((ButtonBase)checkBox6).UseVisualStyleBackColor = true;
		checkBox6.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox17, "comboBox17");
		((ListControl)comboBox17).FormattingEnabled = true;
		comboBox17.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox17.Items"),
			componentResourceManager.GetString("comboBox17.Items1")
		});
		((Control)comboBox17).Name = "comboBox17";
		componentResourceManager.ApplyResources(comboBox18, "comboBox18");
		((ListControl)comboBox18).FormattingEnabled = true;
		comboBox18.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox18.Items"),
			componentResourceManager.GetString("comboBox18.Items1"),
			componentResourceManager.GetString("comboBox18.Items2"),
			componentResourceManager.GetString("comboBox18.Items3"),
			componentResourceManager.GetString("comboBox18.Items4"),
			componentResourceManager.GetString("comboBox18.Items5")
		});
		((Control)comboBox18).Name = "comboBox18";
		componentResourceManager.ApplyResources(checkBox5, "checkBox5");
		((Control)checkBox5).Name = "checkBox5";
		((ButtonBase)checkBox5).UseVisualStyleBackColor = true;
		checkBox5.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox14, "comboBox14");
		((ListControl)comboBox14).FormattingEnabled = true;
		comboBox14.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox14.Items"),
			componentResourceManager.GetString("comboBox14.Items1")
		});
		((Control)comboBox14).Name = "comboBox14";
		componentResourceManager.ApplyResources(comboBox15, "comboBox15");
		((ListControl)comboBox15).FormattingEnabled = true;
		comboBox15.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox15.Items"),
			componentResourceManager.GetString("comboBox15.Items1"),
			componentResourceManager.GetString("comboBox15.Items2"),
			componentResourceManager.GetString("comboBox15.Items3"),
			componentResourceManager.GetString("comboBox15.Items4"),
			componentResourceManager.GetString("comboBox15.Items5")
		});
		((Control)comboBox15).Name = "comboBox15";
		componentResourceManager.ApplyResources(checkBox4, "checkBox4");
		((Control)checkBox4).Name = "checkBox4";
		((ButtonBase)checkBox4).UseVisualStyleBackColor = true;
		checkBox4.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox11, "comboBox11");
		((ListControl)comboBox11).FormattingEnabled = true;
		comboBox11.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox11.Items"),
			componentResourceManager.GetString("comboBox11.Items1")
		});
		((Control)comboBox11).Name = "comboBox11";
		componentResourceManager.ApplyResources(comboBox12, "comboBox12");
		((ListControl)comboBox12).FormattingEnabled = true;
		comboBox12.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox12.Items"),
			componentResourceManager.GetString("comboBox12.Items1"),
			componentResourceManager.GetString("comboBox12.Items2"),
			componentResourceManager.GetString("comboBox12.Items3"),
			componentResourceManager.GetString("comboBox12.Items4"),
			componentResourceManager.GetString("comboBox12.Items5")
		});
		((Control)comboBox12).Name = "comboBox12";
		componentResourceManager.ApplyResources(checkBox3, "checkBox3");
		((Control)checkBox3).Name = "checkBox3";
		((ButtonBase)checkBox3).UseVisualStyleBackColor = true;
		checkBox3.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox8, "comboBox8");
		((ListControl)comboBox8).FormattingEnabled = true;
		comboBox8.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox8.Items"),
			componentResourceManager.GetString("comboBox8.Items1")
		});
		((Control)comboBox8).Name = "comboBox8";
		componentResourceManager.ApplyResources(comboBox9, "comboBox9");
		((ListControl)comboBox9).FormattingEnabled = true;
		comboBox9.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox9.Items"),
			componentResourceManager.GetString("comboBox9.Items1"),
			componentResourceManager.GetString("comboBox9.Items2"),
			componentResourceManager.GetString("comboBox9.Items3"),
			componentResourceManager.GetString("comboBox9.Items4"),
			componentResourceManager.GetString("comboBox9.Items5")
		});
		((Control)comboBox9).Name = "comboBox9";
		componentResourceManager.ApplyResources(checkBox2, "checkBox2");
		((Control)checkBox2).Name = "checkBox2";
		((ButtonBase)checkBox2).UseVisualStyleBackColor = true;
		checkBox2.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox5, "comboBox5");
		((ListControl)comboBox5).FormattingEnabled = true;
		comboBox5.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox5.Items"),
			componentResourceManager.GetString("comboBox5.Items1")
		});
		((Control)comboBox5).Name = "comboBox5";
		componentResourceManager.ApplyResources(comboBox6, "comboBox6");
		((ListControl)comboBox6).FormattingEnabled = true;
		comboBox6.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox6.Items"),
			componentResourceManager.GetString("comboBox6.Items1"),
			componentResourceManager.GetString("comboBox6.Items2"),
			componentResourceManager.GetString("comboBox6.Items3"),
			componentResourceManager.GetString("comboBox6.Items4"),
			componentResourceManager.GetString("comboBox6.Items5")
		});
		((Control)comboBox6).Name = "comboBox6";
		componentResourceManager.ApplyResources(SampleCounter, "SampleCounter");
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items1"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items2"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items3"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items4"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items5"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items6"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items7"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items8"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items9"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items10"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items11"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items12"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items13"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items14"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items15"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items16"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items17"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items18"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items19"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items20"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items21"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items22"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items23"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items24"));
		((ArrayList)(object)SampleCounter.Items).Add((object?)componentResourceManager.GetString("SampleCounter.Items25"));
		((Control)SampleCounter).Name = "SampleCounter";
		componentResourceManager.ApplyResources(SampleTick, "SampleTick");
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items1"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items2"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items3"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items4"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items5"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items6"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items7"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items8"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items9"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items10"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items11"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items12"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items13"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items14"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items15"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items16"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items17"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items18"));
		((ArrayList)(object)SampleTick.Items).Add((object?)componentResourceManager.GetString("SampleTick.Items19"));
		((Control)SampleTick).Name = "SampleTick";
		componentResourceManager.ApplyResources(CounterLable, "CounterLable");
		((Control)CounterLable).Name = "CounterLable";
		componentResourceManager.ApplyResources(TickLable, "TickLable");
		((Control)TickLable).Name = "TickLable";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(checkBox1, "checkBox1");
		((Control)checkBox1).Name = "checkBox1";
		((ButtonBase)checkBox1).UseVisualStyleBackColor = true;
		checkBox1.CheckedChanged += checkBox1_CheckedChanged;
		componentResourceManager.ApplyResources(comboBox2, "comboBox2");
		((ListControl)comboBox2).FormattingEnabled = true;
		comboBox2.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBox2.Items"),
			componentResourceManager.GetString("comboBox2.Items1")
		});
		((Control)comboBox2).Name = "comboBox2";
		componentResourceManager.ApplyResources(comboBox3, "comboBox3");
		((ListControl)comboBox3).FormattingEnabled = true;
		comboBox3.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("comboBox3.Items"),
			componentResourceManager.GetString("comboBox3.Items1"),
			componentResourceManager.GetString("comboBox3.Items2"),
			componentResourceManager.GetString("comboBox3.Items3"),
			componentResourceManager.GetString("comboBox3.Items4"),
			componentResourceManager.GetString("comboBox3.Items5")
		});
		((Control)comboBox3).Name = "comboBox3";
		componentResourceManager.ApplyResources(bnConfig, "bnConfig");
		((Control)bnConfig).Name = "bnConfig";
		((ButtonBase)bnConfig).UseVisualStyleBackColor = true;
		((Control)bnConfig).Click += bnConfig_Click;
		componentResourceManager.ApplyResources(bnCfgExport, "bnCfgExport");
		((Control)bnCfgExport).Name = "bnCfgExport";
		((ButtonBase)bnCfgExport).UseVisualStyleBackColor = true;
		((Control)bnCfgExport).Click += bnCfgExport_Click;
		componentResourceManager.ApplyResources(bnCfgImport, "bnCfgImport");
		((Control)bnCfgImport).Name = "bnCfgImport";
		((ButtonBase)bnCfgImport).UseVisualStyleBackColor = true;
		((Control)bnCfgImport).Click += bnCfgImport_Click;
		componentResourceManager.ApplyResources(bnDelete, "bnDelete");
		((Control)bnDelete).Name = "bnDelete";
		((ButtonBase)bnDelete).UseVisualStyleBackColor = true;
		((Control)bnDelete).Click += bnDelete_Click;
		componentResourceManager.ApplyResources(bnClose, "bnClose");
		((Control)bnClose).Name = "bnClose";
		((ButtonBase)bnClose).UseVisualStyleBackColor = true;
		((Control)bnClose).Click += bnClose_Click;
		componentResourceManager.ApplyResources(tbScopeOp, "tbScopeOp");
		((Control)tbScopeOp).Controls.Add((Control)(object)bnDtImport);
		((Control)tbScopeOp).Controls.Add((Control)(object)bnDtExport);
		((Control)tbScopeOp).Controls.Add((Control)(object)listBox1);
		((Control)tbScopeOp).Controls.Add((Control)(object)bnStop);
		((Control)tbScopeOp).Controls.Add((Control)(object)DrawButton);
		((Control)tbScopeOp).Name = "tbScopeOp";
		tbScopeOp.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnDtImport, "bnDtImport");
		((Control)bnDtImport).Name = "bnDtImport";
		((ButtonBase)bnDtImport).UseVisualStyleBackColor = true;
		((Control)bnDtImport).Click += bnDtImport_Click;
		componentResourceManager.ApplyResources(bnDtExport, "bnDtExport");
		((Control)bnDtExport).Name = "bnDtExport";
		((ButtonBase)bnDtExport).UseVisualStyleBackColor = true;
		((Control)bnDtExport).Click += bnDtExport_Click;
		componentResourceManager.ApplyResources(listBox1, "listBox1");
		((ListControl)listBox1).FormattingEnabled = true;
		((Control)listBox1).Name = "listBox1";
		componentResourceManager.ApplyResources(bnStop, "bnStop");
		((Control)bnStop).Name = "bnStop";
		((ButtonBase)bnStop).UseVisualStyleBackColor = true;
		((Control)bnStop).Click += bnStop_Click;
		componentResourceManager.ApplyResources(DrawButton, "DrawButton");
		((Control)DrawButton).Name = "DrawButton";
		((ButtonBase)DrawButton).UseVisualStyleBackColor = true;
		((Control)DrawButton).Click += DrawButton_Click;
		componentResourceManager.ApplyResources(xyzPos12, "xyzPos12");
		xyzPos12.IsSizeNSCursor = false;
		xyzPos12.IsSizeWECursor = false;
		((Control)xyzPos12).Name = "xyzPos12";
		xyzPos12.ScrollGrace = 0.0;
		xyzPos12.ScrollMaxX = 0.0;
		xyzPos12.ScrollMaxY = 0.0;
		xyzPos12.ScrollMaxY2 = 0.0;
		xyzPos12.ScrollMinX = 0.0;
		xyzPos12.ScrollMinY = 0.0;
		xyzPos12.ScrollMinY2 = 0.0;
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)splitScope);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Control)this).Name = "DataCurve";
		((Form)this).FormClosed += new FormClosedEventHandler(DataCurve_FormClosed);
		((Form)this).Load += DataCurve_Load;
		((Control)splitScope.Panel1).ResumeLayout(false);
		((Control)splitScope.Panel2).ResumeLayout(false);
		((ISupportInitialize)splitScope).EndInit();
		((Control)splitScope).ResumeLayout(false);
		((Control)tbCfg).ResumeLayout(false);
		((Control)tbScopeCfg).ResumeLayout(false);
		((Control)tbScopeCfg).PerformLayout();
		((Control)grp_SerialPortConfig).ResumeLayout(false);
		((Control)grp_SerialPortConfig).PerformLayout();
		((Control)tbScopeOp).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
