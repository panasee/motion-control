using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MCCDEMO;

internal class AboutBox : Form
{
	private uint[] gbAllCgfInfo = new uint[8];

	private IContainer components;

	private TableLayoutPanel tableLayoutPanel;

	private PictureBox logoPictureBox;

	private Label labelProductName;

	private Label labelVersion;

	private Label labelCopyright;

	private Label labelCompanyName;

	private TextBox textBoxDescription;

	private Button okButton;

	private MainFrm PMainFrm;

	private uint MccardVersion;

	private uint DllVersion;

	private uint MccDemoVer;

	private uint[] McuInfo;

	private SaveFileDialog saveFileDialog1;

	public string AssemblyTitle
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
				if (assemblyTitleAttribute.Title != "")
				{
					return assemblyTitleAttribute.Title;
				}
			}
			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
		}
	}

	public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	public string AssemblyDescription
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
		}
	}

	public string AssemblyProduct
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyProductAttribute)customAttributes[0]).Product;
		}
	}

	public string AssemblyCopyright
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
		}
	}

	public string AssemblyCompany
	{
		get
		{
			object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);
			if (customAttributes.Length == 0)
			{
				return "";
			}
			return ((AssemblyCompanyAttribute)customAttributes[0]).Company;
		}
	}

	public AboutBox()
	{
		InitializeComponent();
		((Control)this).Text = $"关于 {AssemblyTitle}";
		((Control)labelProductName).Text = AssemblyProduct;
		((Control)labelCopyright).Text = AssemblyCopyright;
		((Control)labelCompanyName).Text = AssemblyCompany;
		((Control)textBoxDescription).Text = AssemblyDescription;
	}

	private void AboutBox_Load(object sender, EventArgs e)
	{
		uint[] array = new uint[1];
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetCardVersion(array))
		{
			MccardVersion = array[0];
		}
		else
		{
			MccardVersion = uint.MaxValue;
		}
		DllVersion = PMainFrm.Mccard.MoCtrCard_GetDLLVersion();
		((Control)labelVersion).Text = "DLL文件版本号:" + DllVersion.ToString("x") + "\t控制卡版本号：" + MccardVersion.ToString("x");
		McuInfo = new uint[3];
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetBoardHardInfo(McuInfo))
		{
			((Control)textBoxDescription).Text = "高速六轴摇杆控制器上位机控制程序 \r\n注册信息：" + McuInfo[0].ToString("x8") + " " + McuInfo[1].ToString("x8") + " " + McuInfo[2].ToString("x8");
		}
		else
		{
			((Control)textBoxDescription).Text = "高速六轴摇杆控制器上位机控制程序";
		}
		MccDemoVer = 539035184u;
		((Control)labelProductName).Text = "MCCDEMO 版本号：" + MccDemoVer.ToString("x");
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_GetAllConfigInfo(gbAllCgfInfo))
		{
			string text = string.Format("FPGA：{0:x}, MCVer：{1}，编码器：{2:x}，补偿：{3:x}，刹车：{4:x}，脉冲触发：{5:x}，ModbusPoll：{6:x}，MB编码器：{7:x}，手轮：[{8:x},{9:x}]，底板：{10:x}，核心：{11:x}", new object[12]
			{
				gbAllCgfInfo[0],
				gbAllCgfInfo[1],
				(gbAllCgfInfo[2] >> 24) & 0xFF,
				(gbAllCgfInfo[2] >> 16) & 0xFF,
				(gbAllCgfInfo[2] >> 8) & 0xFF,
				gbAllCgfInfo[2] & 0xFF,
				(gbAllCgfInfo[3] >> 24) & 0xFF,
				(gbAllCgfInfo[3] >> 16) & 0xFF,
				(gbAllCgfInfo[3] >> 8) & 0xFF,
				gbAllCgfInfo[3] & 0xFF,
				(gbAllCgfInfo[4] >> 24) & 0xFF,
				(gbAllCgfInfo[4] >> 16) & 0xFF
			});
			TextBox obj = textBoxDescription;
			((Control)obj).Text = ((Control)obj).Text + "\r\n" + text;
		}
		else
		{
			TextBox obj2 = textBoxDescription;
			((Control)obj2).Text = ((Control)obj2).Text + "\r\n 读配置失败！";
		}
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between I4 and Unknown
		((FileDialog)saveFileDialog1).Filter = "版本信息(*.ver)|*.ver";
		((FileDialog)saveFileDialog1).FileName = "verinfo" + DateTime.Today.ToString() + ".ver";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			StreamWriter streamWriter = new StreamWriter(((FileDialog)saveFileDialog1).FileName, append: false, Encoding.ASCII);
			streamWriter.WriteLine("*********************************************************");
			streamWriter.WriteLine(DateTime.Now.ToString());
			streamWriter.WriteLine($"FrameWork Ver = {MccardVersion:X}");
			streamWriter.WriteLine($"Dll Ver = {DllVersion:X}");
			streamWriter.WriteLine($"MccDemo Ver = {MccDemoVer:X}");
			streamWriter.WriteLine($"Hard ID = {{0x{McuInfo[0]:X8}, 0x{McuInfo[1]:X8}, 0x{McuInfo[2]:X8}}}");
			streamWriter.WriteLine(string.Format("{{0x{0:X8}, {1}, 0x{2:X8}, 0x{3:X8}, 0x{4:X8} }}", new object[5]
			{
				gbAllCgfInfo[0],
				gbAllCgfInfo[1],
				gbAllCgfInfo[2],
				gbAllCgfInfo[3],
				gbAllCgfInfo[4]
			}));
			streamWriter.WriteLine("*********************************************************");
			streamWriter.Close();
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AboutBox));
		tableLayoutPanel = new TableLayoutPanel();
		logoPictureBox = new PictureBox();
		labelProductName = new Label();
		labelVersion = new Label();
		labelCopyright = new Label();
		labelCompanyName = new Label();
		textBoxDescription = new TextBox();
		okButton = new Button();
		saveFileDialog1 = new SaveFileDialog();
		((Control)tableLayoutPanel).SuspendLayout();
		((ISupportInitialize)logoPictureBox).BeginInit();
		((Control)this).SuspendLayout();
		tableLayoutPanel.ColumnCount = 2;
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle((SizeType)2, 33f));
		tableLayoutPanel.ColumnStyles.Add(new ColumnStyle((SizeType)2, 67f));
		tableLayoutPanel.Controls.Add((Control)(object)logoPictureBox, 0, 0);
		tableLayoutPanel.Controls.Add((Control)(object)labelProductName, 1, 0);
		tableLayoutPanel.Controls.Add((Control)(object)labelVersion, 1, 1);
		tableLayoutPanel.Controls.Add((Control)(object)labelCopyright, 1, 2);
		tableLayoutPanel.Controls.Add((Control)(object)labelCompanyName, 1, 3);
		tableLayoutPanel.Controls.Add((Control)(object)textBoxDescription, 1, 4);
		tableLayoutPanel.Controls.Add((Control)(object)okButton, 1, 5);
		((Control)tableLayoutPanel).Dock = (DockStyle)5;
		((Control)tableLayoutPanel).Location = new Point(9, 8);
		((Control)tableLayoutPanel).Name = "tableLayoutPanel";
		tableLayoutPanel.RowCount = 6;
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 50f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)2, 10f));
		tableLayoutPanel.RowStyles.Add(new RowStyle((SizeType)1, 20f));
		((Control)tableLayoutPanel).Size = new Size(492, 323);
		((Control)tableLayoutPanel).TabIndex = 0;
		((Control)logoPictureBox).Dock = (DockStyle)5;
		logoPictureBox.Image = (Image)componentResourceManager.GetObject("logoPictureBox.Image");
		((Control)logoPictureBox).Location = new Point(3, 3);
		((Control)logoPictureBox).Name = "logoPictureBox";
		tableLayoutPanel.SetRowSpan((Control)(object)logoPictureBox, 6);
		((Control)logoPictureBox).Size = new Size(156, 317);
		logoPictureBox.SizeMode = (PictureBoxSizeMode)1;
		logoPictureBox.TabIndex = 12;
		logoPictureBox.TabStop = false;
		((Control)labelProductName).Dock = (DockStyle)5;
		((Control)labelProductName).Location = new Point(168, 0);
		((Control)labelProductName).Margin = new Padding(6, 0, 3, 0);
		((Control)labelProductName).MaximumSize = new Size(0, 16);
		((Control)labelProductName).Name = "labelProductName";
		((Control)labelProductName).Size = new Size(321, 16);
		((Control)labelProductName).TabIndex = 19;
		((Control)labelProductName).Text = "产品名称";
		labelProductName.TextAlign = (ContentAlignment)16;
		((Control)labelVersion).Dock = (DockStyle)5;
		((Control)labelVersion).Location = new Point(168, 32);
		((Control)labelVersion).Margin = new Padding(6, 0, 3, 0);
		((Control)labelVersion).MaximumSize = new Size(0, 16);
		((Control)labelVersion).Name = "labelVersion";
		((Control)labelVersion).Size = new Size(321, 16);
		((Control)labelVersion).TabIndex = 0;
		((Control)labelVersion).Text = "版本";
		labelVersion.TextAlign = (ContentAlignment)16;
		((Control)labelCopyright).Dock = (DockStyle)5;
		((Control)labelCopyright).Location = new Point(168, 64);
		((Control)labelCopyright).Margin = new Padding(6, 0, 3, 0);
		((Control)labelCopyright).MaximumSize = new Size(0, 16);
		((Control)labelCopyright).Name = "labelCopyright";
		((Control)labelCopyright).Size = new Size(321, 16);
		((Control)labelCopyright).TabIndex = 21;
		((Control)labelCopyright).Text = "版权";
		labelCopyright.TextAlign = (ContentAlignment)16;
		((Control)labelCompanyName).Dock = (DockStyle)5;
		((Control)labelCompanyName).Location = new Point(168, 96);
		((Control)labelCompanyName).Margin = new Padding(6, 0, 3, 0);
		((Control)labelCompanyName).MaximumSize = new Size(0, 16);
		((Control)labelCompanyName).Name = "labelCompanyName";
		((Control)labelCompanyName).Size = new Size(321, 16);
		((Control)labelCompanyName).TabIndex = 22;
		((Control)labelCompanyName).Text = "公司名称";
		labelCompanyName.TextAlign = (ContentAlignment)16;
		((Control)textBoxDescription).Dock = (DockStyle)5;
		((Control)textBoxDescription).Location = new Point(168, 131);
		((Control)textBoxDescription).Margin = new Padding(6, 3, 3, 3);
		((TextBoxBase)textBoxDescription).Multiline = true;
		((Control)textBoxDescription).Name = "textBoxDescription";
		((TextBoxBase)textBoxDescription).ReadOnly = true;
		textBoxDescription.ScrollBars = (ScrollBars)3;
		((Control)textBoxDescription).Size = new Size(321, 155);
		((Control)textBoxDescription).TabIndex = 23;
		((Control)textBoxDescription).TabStop = false;
		((Control)textBoxDescription).Text = "说明";
		((Control)okButton).Anchor = (AnchorStyles)10;
		okButton.DialogResult = (DialogResult)2;
		((Control)okButton).Location = new Point(414, 292);
		((Control)okButton).Name = "okButton";
		((Control)okButton).Size = new Size(75, 28);
		((Control)okButton).TabIndex = 24;
		((Control)okButton).Text = "确定(&O)";
		((Control)okButton).Click += okButton_Click;
		((Form)this).AcceptButton = (IButtonControl)(object)okButton;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(510, 339);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "AboutBox";
		((Control)this).Padding = new Padding(9, 8, 9, 8);
		((Form)this).ShowIcon = false;
		((Form)this).ShowInTaskbar = false;
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "AboutBox";
		((Form)this).Load += AboutBox_Load;
		((Control)tableLayoutPanel).ResumeLayout(false);
		((Control)tableLayoutPanel).PerformLayout();
		((ISupportInitialize)logoPictureBox).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
