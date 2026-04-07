using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class SportAssitant : Form
{
	private IContainer components;

	private GroupBox grp_SerialPortConfig;

	private Button bn_CloseSp;

	private Button bn_OpenSp;

	private Label lb_SrPort;

	private ComboBox cb_SpName;

	private SerialPort serlPrt;

	private RichTextBox SpText;

	private Timer timer1;

	public SportAssitant()
	{
		InitializeComponent();
	}

	private void fun_PrintfInfo(string sInfo)
	{
		try
		{
			((TextBoxBase)SpText).AppendText(sInfo + "\n");
			((TextBoxBase)SpText).ScrollToCaret();
		}
		catch (Exception ex)
		{
			RichTextBox spText = SpText;
			((Control)spText).Text = ((Control)spText).Text + ex.ToString();
		}
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
			serlPrt.ReceivedBytesThreshold = 1;
			serlPrt.RtsEnable = true;
			serlPrt.WriteTimeout = 500;
			serlPrt.ReadBufferSize = 1024;
			serlPrt.WriteBufferSize = 1024;
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
		timer1.Enabled = false;
		if (1 == fun_InitialSp(((Control)cb_SpName).Text))
		{
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.USARTAssitantFrm_OpenPort + GlobalData.GlobalLanguage.USARTAssitantFrm_OperateFailed, ((Control)cb_SpName).Text));
			return;
		}
		timer1.Enabled = true;
		fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.USARTAssitantFrm_OpenPort + GlobalData.GlobalLanguage.USARTAssitantFrm_OperateSucceed, ((Control)cb_SpName).Text));
	}

	private void bn_CloseSp_Click(object sender, EventArgs e)
	{
		timer1.Enabled = false;
		try
		{
			serlPrt.Close();
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.USARTAssitantFrm_ClosePort + GlobalData.GlobalLanguage.USARTAssitantFrm_OperateSucceed, ((Control)cb_SpName).Text));
		}
		catch (Exception)
		{
			fun_PrintfInfo(string.Format(GlobalData.GlobalLanguage.USARTAssitantFrm_ClosePort + GlobalData.GlobalLanguage.USARTAssitantFrm_OperateFailed, ((Control)cb_SpName).Text));
		}
	}

	private void SportAssitant_FormClosed(object sender, FormClosedEventArgs e)
	{
		bn_CloseSp_Click(sender, (EventArgs)(object)e);
	}

	private void SportAssitant_Load(object sender, EventArgs e)
	{
		string[] portNames = SerialPort.GetPortNames();
		cb_SpName.DataSource = portNames;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		byte[] array = new byte[256];
		int bytesToRead = serlPrt.BytesToRead;
		if (bytesToRead > 0)
		{
			serlPrt.Read(array, 0, bytesToRead);
			fun_PrintfInfo(Encoding.ASCII.GetString(array));
		}
	}

	private void SpText_DoubleClick_1(object sender, EventArgs e)
	{
		((Control)SpText).Text = "";
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
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SportAssitant));
		grp_SerialPortConfig = new GroupBox();
		bn_CloseSp = new Button();
		bn_OpenSp = new Button();
		lb_SrPort = new Label();
		cb_SpName = new ComboBox();
		serlPrt = new SerialPort(components);
		SpText = new RichTextBox();
		timer1 = new Timer(components);
		((Control)grp_SerialPortConfig).SuspendLayout();
		((Control)this).SuspendLayout();
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
		componentResourceManager.ApplyResources(cb_SpName, "cb_SpName");
		((ListControl)cb_SpName).FormattingEnabled = true;
		((Control)cb_SpName).Name = "cb_SpName";
		componentResourceManager.ApplyResources(SpText, "SpText");
		((Control)SpText).Name = "SpText";
		((Control)SpText).DoubleClick += SpText_DoubleClick_1;
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)SpText);
		((Control)this).Controls.Add((Control)(object)grp_SerialPortConfig);
		((Control)this).Name = "SportAssitant";
		((Form)this).FormClosed += new FormClosedEventHandler(SportAssitant_FormClosed);
		((Form)this).Load += SportAssitant_Load;
		((Control)grp_SerialPortConfig).ResumeLayout(false);
		((Control)grp_SerialPortConfig).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
