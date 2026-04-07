using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCCDEMO;

public class LuaCtrlFrm : Form
{
	public enum eDebugLevelType
	{
		eDebugLevInfo,
		eDebugLevDebug,
		eDebugLevWarn,
		eDebugLevErr,
		eDebugLevException
	}

	private MainFrm PMainFrm;

	private string ProgFilePath;

	private int luaEditFrmPosLeft = 6;

	private int luaEditFrmPosTop = 32;

	private string luaLibHeadString = "mclib.";

	private IContainer components;

	private SplitContainer splitContainer1;

	private SplitContainer splitContainer2;

	private MenuStrip menuStrip1;

	private ToolStripMenuItem LuaFileDebug;

	private ListBox lbLuaFileList;

	private TextBox tbLuaEdit;

	private Button bnLuaDel;

	private Button bnLuaDoString;

	private Label label1;

	private ComboBox cbLuaFilePosIndex;

	private Button bnLuaRunFile;

	private Button bnSpiFileDownload;

	private Button bnSpiFileUpload;

	private ToolStripMenuItem LuaFileOp;

	private ToolStripMenuItem File_Open;

	private ToolStripMenuItem File_New;

	private ToolStripMenuItem File_Save;

	private ToolStripMenuItem File_SaveAs;

	private OpenFileDialog openFileDialog1;

	private SaveFileDialog saveFileDialog1;

	private Button bnCompile;

	private Button bnInsert;

	private Button bnAppend;

	private Button bnRun;

	private Button bnStep;

	private Button bnContinueFileRun;

	private Button bnQuiteFileRun;

	private Button bnPauseFileRun;

	private TabControl tabLuaCmndEdit;

	private TabPage tbAxisCmnd;

	private TabPage tbKeyWord;

	private RadioButton rbQuiteHome;

	private RadioButton rbHomeB;

	private RadioButton rbHome;

	private TextBox tbPTPAcc;

	private Button bnLuaInsertNewLine;

	private TextBox tbPTPVel;

	private TextBox tbPTPDist;

	private Label label5;

	private ComboBox cbAxisID;

	private RadioButton rbEStop;

	private RadioButton rbStopB;

	private RadioButton rbStop;

	private RadioButton rbPTPCmndMoveJBR;

	private RadioButton rbPTPCmndMoveJB;

	private RadioButton rbPTPCmndMoveJR;

	private RadioButton rbPTPCmndMoveJ;

	private RadioButton rbPTPSetPos;

	private CheckBox ckbPTPAcc;

	private CheckBox ckbPTPVel;

	private CheckBox ckbPTPDist;

	private SplitContainer splitContainer3;

	private SplitContainer splitContainer4;

	private SplitContainer splitContainer5;

	private TabPage tbGroupCmnd;

	private RichTextBox rtbLuaDebug;

	private CheckBox ckbPTPDelay;

	private TextBox tbPTPDelay;

	private Button bnDelTab;

	private Button bnAddTab;

	private TabPage tbAskCmnd;

	private TabPage tbIOCmnd;

	private Label label2;

	private ComboBox cbAskNCDevID;

	private RadioButton rbAskJRunStt;

	private RadioButton rbAskJPos;

	private RadioButton rbAskJSpd;

	private RadioButton rbPTPMovPre;

	private RadioButton rbAskJAlarm;

	private RadioButton rbAskGrpRunStt;

	private RadioButton rbAskGrpSpd;

	private RadioButton rbPTPSetSpd;

	private RadioButton rbIOCmndWDI;

	private RadioButton rbIOCmndDO;

	private Label label3;

	private ComboBox cbIOCmndIndex;

	private RadioButton rbIOCmndDI;

	private CheckBox ckbIOVal;

	private RadioButton rbAskGrpPos;

	private RadioButton rbGrpCmndMovPRB;

	private RadioButton rbGrpCmndMovPB;

	private RadioButton rbGrpCmndMovPR;

	private RadioButton rbGrpCmndMovP;

	private RadioButton rbGrpCmndMovLRB;

	private RadioButton rbGrpCmndMovLB;

	private RadioButton rbGrpCmndMovLR;

	private RadioButton rbGrpCmndMovL;

	private RadioButton rbGrpCmndSetGrpSpd;

	private CheckBox ckbGrpAxisEnY;

	private CheckBox ckbGrpAxisEnX;

	private CheckBox ckbGrpAxisEnC;

	private CheckBox ckbGrpAxisEnB;

	private CheckBox ckbGrpAxisEnA;

	private CheckBox ckbGrpAxisEnZ;

	private TextBox tbGrpMovPosC;

	private TextBox tbGrpMovPosB;

	private TextBox tbGrpMovPosA;

	private TextBox tbGrpMovPosZ;

	private TextBox tbGrpMovPosY;

	private TextBox tbGrpMovPosX;

	private TextBox tbGrpMovAcc;

	private TextBox tbGrpMovVel;

	private Button bnGrpAxisEnable;

	private Label label7;

	private TextBox tbGrpCmndAxisEnStt;

	private CheckBox ckbGrpMovAcc;

	private CheckBox ckbGrpMovSpd;

	private Button button7;

	private Button button6;

	private Button button5;

	private Button button4;

	private Button button3;

	private Button button2;

	private Button button1;

	private Button button8;

	private RichTextBox richTextBox1;

	private Button button10;

	private Button button9;

	public LuaCtrlFrm()
	{
		InitializeComponent();
	}

	private void LuaCtrlFrm_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		((Control)cbLuaFilePosIndex).Text = "选择文件位置";
		((ListControl)cbAxisID).SelectedIndex = 0;
	}

	private void bnLuaDoString_Click(object sender, EventArgs e)
	{
		if (((Control)tbLuaEdit).Text.Length > 0)
		{
			PMainFrm.Mccard.MoCtrCard_Lua_DoString(((Control)tbLuaEdit).Text);
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "编辑框中没有指令");
		}
	}

	private void lbLuaDoString_DoubleClick(object sender, EventArgs e)
	{
		if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
		{
			((Control)tbLuaEdit).Text = lbLuaFileList.SelectedItem.ToString();
		}
	}

	private void tbLuaEdit_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r')
		{
			if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
			{
				lbLuaFileList.Items.Insert(((ListControl)lbLuaFileList).SelectedIndex, (object)((Control)tbLuaEdit).Text);
				lbLuaFileList.Items.RemoveAt(((ListControl)lbLuaFileList).SelectedIndex);
			}
			else
			{
				lbLuaFileList.Items.Insert(lbLuaFileList.Items.Count, (object)((Control)tbLuaEdit).Text);
			}
		}
		else if (e.KeyChar == '\u001b')
		{
			((Control)tbLuaEdit).Text = "";
		}
	}

	private void bnLuaDel_Click(object sender, EventArgs e)
	{
		if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
		{
			lbLuaFileList.Items.RemoveAt(((ListControl)lbLuaFileList).SelectedIndex);
		}
	}

	private void File_Open_Click(object sender, EventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between I4 and Unknown
		((FileDialog)openFileDialog1).Filter = "*(*.ar)|*.ar";
		((FileDialog)openFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)openFileDialog1).ShowDialog())
		{
			lbLuaFileList.Items.Clear();
			ProgFilePath = ((FileDialog)openFileDialog1).FileName;
			StreamReader streamReader = new StreamReader(ProgFilePath);
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine() + Environment.NewLine;
				lbLuaFileList.Items.Add((object)text);
			}
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, $"成功打开<{ProgFilePath}>程序文件");
			streamReader.Close();
		}
	}

	private void File_New_Click(object sender, EventArgs e)
	{
		lbLuaFileList.Items.Clear();
		ProgFilePath = "";
	}

	private void File_Save_Click(object sender, EventArgs e)
	{
		ushort num = 0;
		if (lbLuaFileList.Items.Count > 0)
		{
			if ("" != ProgFilePath)
			{
				StreamWriter streamWriter = new StreamWriter(ProgFilePath, append: false, Encoding.ASCII);
				for (num = 0; num < lbLuaFileList.Items.Count; num++)
				{
					((ListControl)lbLuaFileList).SelectedIndex = num;
					streamWriter.WriteLine(((Control)lbLuaFileList).Text);
				}
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, "成功保存程序至" + ProgFilePath);
				streamWriter.Close();
			}
			else
			{
				File_SaveAs_Click(sender, e);
			}
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "程序区为空，不能保存！");
		}
	}

	private void File_SaveAs_Click(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)saveFileDialog1).Filter = "程序文件(*.ar)|*.ar";
		((FileDialog)saveFileDialog1).FileName = "ProgFile.ar";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 != (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			return;
		}
		ProgFilePath = ((FileDialog)saveFileDialog1).FileName;
		if (lbLuaFileList.Items.Count > 0)
		{
			StreamWriter streamWriter = new StreamWriter(ProgFilePath, append: false, Encoding.ASCII);
			for (num = 0; num < lbLuaFileList.Items.Count; num++)
			{
				((ListControl)lbLuaFileList).SelectedIndex = num;
				streamWriter.WriteLine(((Control)lbLuaFileList).Text);
			}
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, "成功保存程序至" + ProgFilePath);
			streamWriter.Close();
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "程序区为空，不能保存！");
		}
	}

	private void bnSpiFileDownload_Click(object sender, EventArgs e)
	{
		try
		{
			byte[] array = new byte[4096];
			int num = int.Parse(((Control)cbLuaFilePosIndex).Text);
			int num2 = 0;
			if (lbLuaFileList.Items.Count > 0)
			{
				for (int i = 0; i < lbLuaFileList.Items.Count; i++)
				{
					((ListControl)lbLuaFileList).SelectedIndex = i;
					byte[] bytes = Encoding.ASCII.GetBytes(((Control)lbLuaFileList).Text + Environment.NewLine);
					Array.Copy(bytes, 0, array, num2, bytes.Length);
					num2 += bytes.Length;
				}
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "程序区为空，不能下载！");
			}
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_DownloadFile(array, num2, (ushort)num))
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, $"将 AR 程序下载到[{num}]号文件成功！");
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, $"将 AR 程序下载到[{num}]号文件失败！");
			}
		}
		catch (Exception ex)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, "下载过程出现异常！EX = " + ex.ToString());
		}
	}

	private void bnSpiFileUpload_Click(object sender, EventArgs e)
	{
		try
		{
			byte[] array = new byte[4096];
			int num = int.Parse(((Control)cbLuaFilePosIndex).Text);
			ushort fileLen = 0;
			lbLuaFileList.Items.Clear();
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_ReadFile((ushort)num, array, out fileLen))
			{
				if (fileLen > 0)
				{
					byte[] array2 = new byte[fileLen];
					Array.Copy(array, 0, array2, 0, fileLen);
					StreamReader streamReader = new StreamReader(new MemoryStream(array2));
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine() + Environment.NewLine;
						lbLuaFileList.Items.Add((object)text);
					}
					PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, $"文件[{num}]上传成功！");
				}
				else
				{
					PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, $"文件[{num}]内容解析错误，可能已经被损坏！");
				}
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, $"文件[{num}]上传失败！");
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, $"文件[{((Control)cbLuaFilePosIndex).Text}]上传过程出现异常！请检查文件ID值！");
		}
	}

	private void bnLuaRunFile_Click(object sender, EventArgs e)
	{
		try
		{
			ushort num = ushort.Parse(((Control)cbLuaFilePosIndex).Text);
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_Run(num))
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, $"运行文件[{num}]成功！");
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, $"运行文件[{num}]成功！");
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, $"执行文件[{((Control)cbLuaFilePosIndex).Text}]出现异常！请检查文件ID值！");
		}
	}

	private void bnPauseFileRun_Click(object sender, EventArgs e)
	{
		try
		{
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_Pause())
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, "执行暂停运行指令成功！");
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "执行暂停运行指令失败！");
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, "执行暂停运行指令出现异常！");
		}
	}

	private void bnQuiteFileRun_Click(object sender, EventArgs e)
	{
		try
		{
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_Stop())
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, "执行退出运行指令成功！");
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "执行退出运行指令失败！");
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, "执行退出运行指令出现异常！");
		}
	}

	private void bnContinueFileRun_Click(object sender, EventArgs e)
	{
		try
		{
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_AR_Continue())
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevInfo, "执行继续运行指令成功！");
			}
			else
			{
				PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "执行继续运行指令失败！");
			}
		}
		catch (Exception)
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevException, "执行继续运行指令出现异常！");
		}
	}

	private void cbBLuaCmnd_SelectedIndexChanged(object sender, EventArgs e)
	{
		luaProgramePTP luaProgramePTP2 = new luaProgramePTP();
		((Form)luaProgramePTP2).TopLevel = false;
		((Control)tbAxisCmnd).Controls.Add((Control)(object)luaProgramePTP2);
		((Control)luaProgramePTP2).Left = luaEditFrmPosLeft;
		((Control)luaProgramePTP2).Top = luaEditFrmPosTop;
		((Control)luaProgramePTP2).Show();
	}

	private string fun_lua_GetCmndFromEditAera()
	{
		string text = luaLibHeadString;
		if (tabLuaCmndEdit.SelectedIndex == 0)
		{
			if (rbPTPCmndMoveJ.Checked || rbPTPCmndMoveJB.Checked || rbPTPCmndMoveJR.Checked || rbPTPCmndMoveJBR.Checked)
			{
				if (rbPTPCmndMoveJ.Checked)
				{
					text += "MovJ(";
				}
				else if (rbPTPCmndMoveJB.Checked)
				{
					text += "MovJB(";
				}
				else if (rbPTPCmndMoveJR.Checked)
				{
					text += "MovJR(";
				}
				else if (rbPTPCmndMoveJBR.Checked)
				{
					text += "MovJRB(";
				}
				if (((ListControl)cbAxisID).SelectedIndex > 5)
				{
					fun_PrintfInfo("轴号错误，运动指令的轴号不能大于5", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += ((Control)cbAxisID).Text;
				text += $", {((Control)tbPTPDist).Text}";
				if (ckbPTPVel.Checked || ckbPTPAcc.Checked || ckbPTPDelay.Checked)
				{
					text += ",\"";
					if (ckbPTPVel.Checked)
					{
						text += $"Spd={((Control)tbPTPVel).Text}";
					}
					if (ckbPTPAcc.Checked)
					{
						text += $" Acc={((Control)tbPTPAcc).Text}";
					}
					if (ckbPTPDelay.Checked)
					{
						text += $" Delay={((Control)tbPTPDelay).Text}";
					}
					text += "\"";
				}
				text += ")";
			}
			else if (rbEStop.Checked)
			{
				if (rbEStop.Checked)
				{
					text += "EStop";
				}
			}
			else if (rbStop.Checked || rbStopB.Checked)
			{
				if (rbStop.Checked)
				{
					text += "Stop(";
				}
				else if (rbStopB.Checked)
				{
					text += "StopB(";
				}
				text += ((Control)cbAxisID).Text;
				if (ckbPTPAcc.Checked)
				{
					text += $", \"Acc={((Control)tbPTPAcc).Text}\"";
				}
				text += ")";
			}
			else if (rbHome.Checked || rbHomeB.Checked)
			{
				if (rbHome.Checked)
				{
					text += "Hom(";
				}
				else if (rbHomeB.Checked)
				{
					text += "HomB(";
				}
				text += ((Control)cbAxisID).Text;
				if (ckbPTPVel.Checked || ckbPTPAcc.Checked || ckbPTPDelay.Checked)
				{
					text += ",\"";
					if (ckbPTPVel.Checked)
					{
						text += $"Spd={((Control)tbPTPVel).Text}";
					}
					if (ckbPTPAcc.Checked)
					{
						text += $" Acc={((Control)tbPTPAcc).Text}";
					}
					if (ckbPTPDelay.Checked)
					{
						text += $" Delay={((Control)tbPTPDelay).Text}";
					}
					text += "\"";
				}
				text += ")";
			}
			else if (rbQuiteHome.Checked)
			{
				text += $"QHom({((Control)cbAxisID).Text})";
			}
			else if (rbPTPSetPos.Checked)
			{
				if (!ckbPTPDist.Checked)
				{
					fun_PrintfInfo("请指定坐标位置", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += $"SetJPos({((Control)cbAxisID).Text}, {((Control)tbPTPDist).Text})";
			}
			else if (rbPTPSetSpd.Checked)
			{
				if (!ckbPTPVel.Checked)
				{
					fun_PrintfInfo("请指定速度值", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += $"SetJPos({((Control)cbAxisID).Text}, {((Control)tbPTPVel).Text})";
			}
			else
			{
				if (!rbPTPMovPre.Checked)
				{
					fun_PrintfInfo("请选择指令！", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += "MovPre()";
			}
			text = "FunRes=" + text;
		}
		else if (1 == tabLuaCmndEdit.SelectedIndex)
		{
			if (rbGrpCmndMovP.Checked || rbGrpCmndMovPB.Checked || rbGrpCmndMovPR.Checked || rbGrpCmndMovPRB.Checked || rbGrpCmndMovL.Checked || rbGrpCmndMovLB.Checked || rbGrpCmndMovLR.Checked || rbGrpCmndMovLRB.Checked)
			{
				if (rbGrpCmndMovP.Checked)
				{
					text += "MovP(";
				}
				else if (rbGrpCmndMovPB.Checked)
				{
					text += "MovPB(";
				}
				else if (rbGrpCmndMovPR.Checked)
				{
					text += "MovPR(";
				}
				else if (rbGrpCmndMovPRB.Checked)
				{
					text += "MovPRB(";
				}
				else if (rbGrpCmndMovL.Checked)
				{
					text += "MovL(";
				}
				else if (rbGrpCmndMovLB.Checked)
				{
					text += "MovLB(";
				}
				else if (rbGrpCmndMovLR.Checked)
				{
					text += "MovLR(";
				}
				else if (rbGrpCmndMovLRB.Checked)
				{
					text += "MovLRB(";
				}
				text += string.Format("{0}, [{1},{2},{3},{4},{5},{6}]", new object[7]
				{
					((Control)tbGrpCmndAxisEnStt).Text,
					((Control)tbGrpMovPosX).Text,
					((Control)tbGrpMovPosY).Text,
					((Control)tbGrpMovPosZ).Text,
					((Control)tbGrpMovPosA).Text,
					((Control)tbGrpMovPosB).Text,
					((Control)tbGrpMovPosC).Text
				});
				if (ckbGrpMovSpd.Checked || ckbGrpMovAcc.Checked)
				{
					text += ", \"";
					if (ckbGrpMovSpd.Checked)
					{
						text += $"Spd{((Control)tbGrpMovVel).Text}";
						if (ckbGrpMovAcc.Checked)
						{
							text += $" Acc{((Control)tbGrpMovAcc).Text}";
						}
					}
					else if (ckbGrpMovAcc.Checked)
					{
						text += $"Acc{((Control)tbGrpMovAcc).Text}";
					}
					text += "\"";
				}
				text = "FunRes=" + text + ")";
			}
			else if (rbGrpCmndSetGrpSpd.Checked)
			{
				if (!ckbGrpMovSpd.Checked)
				{
					fun_PrintfInfo("请选择并设置速度！", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text = "FunRes=" + text + $"SetGrpSpd(0,{((Control)tbGrpMovVel).Text})";
			}
		}
		else if (2 == tabLuaCmndEdit.SelectedIndex)
		{
			if (rbAskJRunStt.Checked)
			{
				text += $"GetJStt({((Control)cbAskNCDevID).Text})";
			}
			else if (rbAskJSpd.Checked)
			{
				text += $"GetJSpd({((Control)cbAskNCDevID).Text})";
			}
			else if (rbAskJPos.Checked)
			{
				text += $"GetJPos({((Control)cbAskNCDevID).Text})";
			}
			else if (rbAskJAlarm.Checked)
			{
				text += $"GetAlarm({((Control)cbAskNCDevID).Text})";
			}
			else if (rbAskGrpRunStt.Checked)
			{
				text += string.Format("GetGrpStt()", new object[0]);
			}
			else if (rbAskGrpSpd.Checked)
			{
				text += string.Format("GetGrpSpd()", new object[0]);
			}
			else
			{
				if (!rbAskGrpPos.Checked)
				{
					fun_PrintfInfo("请选择指令！", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += string.Format("GetGrpPos()", new object[0]);
			}
			text = "FunRes, RunStt=" + text;
		}
		else if (3 == tabLuaCmndEdit.SelectedIndex)
		{
			if (rbIOCmndDO.Checked)
			{
				text += $"DO({((Control)cbIOCmndIndex).Text},{(ckbIOVal.Checked ? 1 : 0)})";
			}
			else if (rbIOCmndDI.Checked)
			{
				text += $"DI({((Control)cbIOCmndIndex).Text})";
				text = "DIStt=" + text;
			}
			else
			{
				if (!rbIOCmndWDI.Checked)
				{
					fun_PrintfInfo("请选择指令！", eDebugLevelType.eDebugLevErr);
					return null;
				}
				text += $"WDI({((Control)cbIOCmndIndex).Text},{(ckbIOVal.Checked ? 1 : 0)})";
				text = "WDIStt=" + text;
			}
		}
		return text;
	}

	private void bnLuaEditConfirm_Click(object sender, EventArgs e)
	{
		if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
		{
			lbLuaFileList.Items.Insert(((ListControl)lbLuaFileList).SelectedIndex, (object)"");
		}
	}

	public void fun_PrintfInfo(string sInfo, eDebugLevelType eLev = eDebugLevelType.eDebugLevInfo)
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
		((Control)rtbLuaDebug).Text = DateTime.Now.ToString() + "\t" + text + sInfo + Environment.NewLine + ((Control)rtbLuaDebug).Text;
	}

	private void bnInsert_Click(object sender, EventArgs e)
	{
		int num = ((ListControl)lbLuaFileList).SelectedIndex;
		if (num < 0)
		{
			num = 0;
		}
		string text = fun_lua_GetCmndFromEditAera();
		if (text != null)
		{
			lbLuaFileList.Items.Insert(num, (object)text);
		}
		else
		{
			fun_PrintfInfo("获取AR指令失败！", eDebugLevelType.eDebugLevErr);
		}
	}

	private void bnAddTab_Click(object sender, EventArgs e)
	{
		if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
		{
			string text = "\t" + lbLuaFileList.SelectedItem.ToString();
			int selectedIndex = ((ListControl)lbLuaFileList).SelectedIndex;
			lbLuaFileList.Items.RemoveAt(selectedIndex);
			lbLuaFileList.Items.Insert(selectedIndex, (object)text);
			((ListControl)lbLuaFileList).SelectedIndex = selectedIndex;
		}
	}

	private void bnDelTab_Click(object sender, EventArgs e)
	{
		if (((ListControl)lbLuaFileList).SelectedIndex >= 0)
		{
			string text = lbLuaFileList.SelectedItem.ToString();
			int selectedIndex = ((ListControl)lbLuaFileList).SelectedIndex;
			lbLuaFileList.Items.RemoveAt(selectedIndex);
			if (text.First() == '\t')
			{
				text = text.Substring(1);
			}
			lbLuaFileList.Items.Insert(selectedIndex, (object)text);
			((ListControl)lbLuaFileList).SelectedIndex = selectedIndex;
		}
	}

	private void bnGrpAxisEnable_Click(object sender, EventArgs e)
	{
		if (((Control)tbGrpCmndAxisEnStt).Text == "0")
		{
			ckbGrpAxisEnX.Checked = true;
			ckbGrpAxisEnY.Checked = true;
			ckbGrpAxisEnZ.Checked = true;
			ckbGrpAxisEnA.Checked = true;
			ckbGrpAxisEnB.Checked = true;
			ckbGrpAxisEnC.Checked = true;
		}
		else
		{
			ckbGrpAxisEnX.Checked = false;
			ckbGrpAxisEnY.Checked = false;
			ckbGrpAxisEnZ.Checked = false;
			ckbGrpAxisEnA.Checked = false;
			ckbGrpAxisEnB.Checked = false;
			ckbGrpAxisEnC.Checked = false;
		}
	}

	private void ckbGrpAxisEnX_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		CheckBox val = (CheckBox)sender;
		int num = int.Parse(((Control)tbGrpCmndAxisEnStt).Text);
		if (((Control)val).Name == ((Control)ckbGrpAxisEnX).Name)
		{
			num = ((!ckbGrpAxisEnX.Checked) ? (num & -2) : (num | 1));
		}
		else if (((Control)val).Name == ((Control)ckbGrpAxisEnY).Name)
		{
			num = ((!ckbGrpAxisEnY.Checked) ? (num & -3) : (num | 2));
		}
		else if (((Control)val).Name == ((Control)ckbGrpAxisEnZ).Name)
		{
			num = ((!ckbGrpAxisEnZ.Checked) ? (num & -5) : (num | 4));
		}
		else if (((Control)val).Name == ((Control)ckbGrpAxisEnA).Name)
		{
			num = ((!ckbGrpAxisEnA.Checked) ? (num & -9) : (num | 8));
		}
		else if (((Control)val).Name == ((Control)ckbGrpAxisEnB).Name)
		{
			num = ((!ckbGrpAxisEnB.Checked) ? (num & -17) : (num | 0x10));
		}
		else if (((Control)val).Name == ((Control)ckbGrpAxisEnC).Name)
		{
			num = ((!ckbGrpAxisEnC.Checked) ? (num & -33) : (num | 0x20));
		}
		((Control)tbGrpCmndAxisEnStt).Text = num.ToString();
	}

	private void ckbIOVal_CheckedChanged(object sender, EventArgs e)
	{
		if (ckbIOVal.Checked)
		{
			((Control)ckbIOVal).Text = "有效";
		}
		else
		{
			((Control)ckbIOVal).Text = "无效";
		}
	}

	private void bnAppend_Click(object sender, EventArgs e)
	{
		string text = fun_lua_GetCmndFromEditAera();
		if (text != null)
		{
			lbLuaFileList.Items.Add((object)text);
		}
		else
		{
			fun_PrintfInfo("获取AR指令失败！", eDebugLevelType.eDebugLevErr);
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
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
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
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Expected O, but got Unknown
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Expected O, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Expected O, but got Unknown
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Expected O, but got Unknown
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Expected O, but got Unknown
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Expected O, but got Unknown
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Expected O, but got Unknown
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Expected O, but got Unknown
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Expected O, but got Unknown
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Expected O, but got Unknown
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Expected O, but got Unknown
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Expected O, but got Unknown
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Expected O, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Expected O, but got Unknown
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Expected O, but got Unknown
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Expected O, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Expected O, but got Unknown
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Expected O, but got Unknown
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Expected O, but got Unknown
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Expected O, but got Unknown
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Expected O, but got Unknown
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Expected O, but got Unknown
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Expected O, but got Unknown
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Expected O, but got Unknown
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Expected O, but got Unknown
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Expected O, but got Unknown
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Expected O, but got Unknown
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Expected O, but got Unknown
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Expected O, but got Unknown
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Expected O, but got Unknown
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Expected O, but got Unknown
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Expected O, but got Unknown
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Expected O, but got Unknown
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected O, but got Unknown
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Expected O, but got Unknown
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fc: Expected O, but got Unknown
		//IL_0fb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c66: Unknown result type (might be due to invalid IL or missing references)
		//IL_29c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2eeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3471: Unknown result type (might be due to invalid IL or missing references)
		splitContainer1 = new SplitContainer();
		splitContainer2 = new SplitContainer();
		splitContainer4 = new SplitContainer();
		lbLuaFileList = new ListBox();
		tbLuaEdit = new TextBox();
		splitContainer3 = new SplitContainer();
		splitContainer5 = new SplitContainer();
		tabLuaCmndEdit = new TabControl();
		tbAxisCmnd = new TabPage();
		rbPTPSetSpd = new RadioButton();
		rbPTPMovPre = new RadioButton();
		ckbPTPDelay = new CheckBox();
		tbPTPDelay = new TextBox();
		ckbPTPAcc = new CheckBox();
		ckbPTPVel = new CheckBox();
		ckbPTPDist = new CheckBox();
		rbPTPSetPos = new RadioButton();
		rbQuiteHome = new RadioButton();
		rbHomeB = new RadioButton();
		rbHome = new RadioButton();
		tbPTPAcc = new TextBox();
		tbPTPVel = new TextBox();
		tbPTPDist = new TextBox();
		label5 = new Label();
		cbAxisID = new ComboBox();
		rbEStop = new RadioButton();
		rbStopB = new RadioButton();
		rbStop = new RadioButton();
		rbPTPCmndMoveJBR = new RadioButton();
		rbPTPCmndMoveJB = new RadioButton();
		rbPTPCmndMoveJR = new RadioButton();
		rbPTPCmndMoveJ = new RadioButton();
		tbGroupCmnd = new TabPage();
		ckbGrpMovAcc = new CheckBox();
		ckbGrpMovSpd = new CheckBox();
		label7 = new Label();
		tbGrpCmndAxisEnStt = new TextBox();
		bnGrpAxisEnable = new Button();
		tbGrpMovAcc = new TextBox();
		tbGrpMovVel = new TextBox();
		tbGrpMovPosC = new TextBox();
		tbGrpMovPosB = new TextBox();
		tbGrpMovPosA = new TextBox();
		tbGrpMovPosZ = new TextBox();
		tbGrpMovPosY = new TextBox();
		tbGrpMovPosX = new TextBox();
		ckbGrpAxisEnC = new CheckBox();
		ckbGrpAxisEnB = new CheckBox();
		ckbGrpAxisEnA = new CheckBox();
		ckbGrpAxisEnZ = new CheckBox();
		ckbGrpAxisEnY = new CheckBox();
		ckbGrpAxisEnX = new CheckBox();
		rbGrpCmndSetGrpSpd = new RadioButton();
		rbGrpCmndMovLRB = new RadioButton();
		rbGrpCmndMovLB = new RadioButton();
		rbGrpCmndMovLR = new RadioButton();
		rbGrpCmndMovL = new RadioButton();
		rbGrpCmndMovPRB = new RadioButton();
		rbGrpCmndMovPB = new RadioButton();
		rbGrpCmndMovPR = new RadioButton();
		rbGrpCmndMovP = new RadioButton();
		tbAskCmnd = new TabPage();
		rbAskGrpPos = new RadioButton();
		rbAskGrpSpd = new RadioButton();
		rbAskGrpRunStt = new RadioButton();
		rbAskJAlarm = new RadioButton();
		rbAskJPos = new RadioButton();
		rbAskJSpd = new RadioButton();
		label2 = new Label();
		cbAskNCDevID = new ComboBox();
		rbAskJRunStt = new RadioButton();
		tbIOCmnd = new TabPage();
		ckbIOVal = new CheckBox();
		rbIOCmndWDI = new RadioButton();
		rbIOCmndDO = new RadioButton();
		label3 = new Label();
		cbIOCmndIndex = new ComboBox();
		rbIOCmndDI = new RadioButton();
		tbKeyWord = new TabPage();
		bnCompile = new Button();
		bnStep = new Button();
		bnRun = new Button();
		bnDelTab = new Button();
		bnAddTab = new Button();
		bnLuaDoString = new Button();
		bnLuaDel = new Button();
		bnAppend = new Button();
		bnInsert = new Button();
		bnLuaInsertNewLine = new Button();
		bnContinueFileRun = new Button();
		bnPauseFileRun = new Button();
		bnQuiteFileRun = new Button();
		cbLuaFilePosIndex = new ComboBox();
		bnSpiFileUpload = new Button();
		bnLuaRunFile = new Button();
		label1 = new Label();
		bnSpiFileDownload = new Button();
		menuStrip1 = new MenuStrip();
		LuaFileOp = new ToolStripMenuItem();
		File_Open = new ToolStripMenuItem();
		File_New = new ToolStripMenuItem();
		File_Save = new ToolStripMenuItem();
		File_SaveAs = new ToolStripMenuItem();
		LuaFileDebug = new ToolStripMenuItem();
		rtbLuaDebug = new RichTextBox();
		openFileDialog1 = new OpenFileDialog();
		saveFileDialog1 = new SaveFileDialog();
		button1 = new Button();
		button2 = new Button();
		button3 = new Button();
		button4 = new Button();
		button5 = new Button();
		button6 = new Button();
		button7 = new Button();
		button8 = new Button();
		richTextBox1 = new RichTextBox();
		button9 = new Button();
		button10 = new Button();
		((ISupportInitialize)splitContainer1).BeginInit();
		((Control)splitContainer1.Panel1).SuspendLayout();
		((Control)splitContainer1.Panel2).SuspendLayout();
		((Control)splitContainer1).SuspendLayout();
		((ISupportInitialize)splitContainer2).BeginInit();
		((Control)splitContainer2.Panel1).SuspendLayout();
		((Control)splitContainer2.Panel2).SuspendLayout();
		((Control)splitContainer2).SuspendLayout();
		((ISupportInitialize)splitContainer4).BeginInit();
		((Control)splitContainer4.Panel1).SuspendLayout();
		((Control)splitContainer4.Panel2).SuspendLayout();
		((Control)splitContainer4).SuspendLayout();
		((ISupportInitialize)splitContainer3).BeginInit();
		((Control)splitContainer3.Panel1).SuspendLayout();
		((Control)splitContainer3.Panel2).SuspendLayout();
		((Control)splitContainer3).SuspendLayout();
		((ISupportInitialize)splitContainer5).BeginInit();
		((Control)splitContainer5.Panel1).SuspendLayout();
		((Control)splitContainer5.Panel2).SuspendLayout();
		((Control)splitContainer5).SuspendLayout();
		((Control)tabLuaCmndEdit).SuspendLayout();
		((Control)tbAxisCmnd).SuspendLayout();
		((Control)tbGroupCmnd).SuspendLayout();
		((Control)tbAskCmnd).SuspendLayout();
		((Control)tbIOCmnd).SuspendLayout();
		((Control)tbKeyWord).SuspendLayout();
		((Control)menuStrip1).SuspendLayout();
		((Control)this).SuspendLayout();
		splitContainer1.BorderStyle = (BorderStyle)1;
		splitContainer1.Dock = (DockStyle)5;
		((Control)splitContainer1).Location = new Point(0, 0);
		((Control)splitContainer1).Name = "splitContainer1";
		splitContainer1.Orientation = (Orientation)0;
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)splitContainer2);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)menuStrip1);
		((Control)splitContainer1.Panel2).Controls.Add((Control)(object)rtbLuaDebug);
		((Control)splitContainer1).Size = new Size(1158, 736);
		splitContainer1.SplitterDistance = 611;
		((Control)splitContainer1).TabIndex = 0;
		splitContainer2.BorderStyle = (BorderStyle)1;
		splitContainer2.Dock = (DockStyle)5;
		((Control)splitContainer2).Location = new Point(0, 25);
		((Control)splitContainer2).Name = "splitContainer2";
		((Control)splitContainer2.Panel1).Controls.Add((Control)(object)splitContainer4);
		((Control)splitContainer2.Panel2).Controls.Add((Control)(object)splitContainer3);
		((Control)splitContainer2).Size = new Size(1158, 586);
		splitContainer2.SplitterDistance = 848;
		((Control)splitContainer2).TabIndex = 1;
		splitContainer4.BorderStyle = (BorderStyle)1;
		splitContainer4.Dock = (DockStyle)5;
		((Control)splitContainer4).Location = new Point(0, 0);
		((Control)splitContainer4).Name = "splitContainer4";
		splitContainer4.Orientation = (Orientation)0;
		((Control)splitContainer4.Panel1).Controls.Add((Control)(object)lbLuaFileList);
		((Control)splitContainer4.Panel2).Controls.Add((Control)(object)tbLuaEdit);
		((Control)splitContainer4).Size = new Size(848, 586);
		splitContainer4.SplitterDistance = 543;
		((Control)splitContainer4).TabIndex = 4;
		((Control)lbLuaFileList).Dock = (DockStyle)5;
		((ListControl)lbLuaFileList).FormattingEnabled = true;
		lbLuaFileList.ItemHeight = 12;
		lbLuaFileList.Items.AddRange(new object[6] { "A={10,20,30,40,50,60}", "Mov(A, 10)", "MovJ 0 100 \"spd20\"", "LuaLib1_0.MovJ 0 100 \"spd20\"", "LuaLib1_0.MovJ(0,100,\"spd20\")", "LuaLib1_0.MovJ(0,100)" });
		((Control)lbLuaFileList).Location = new Point(0, 0);
		((Control)lbLuaFileList).Name = "lbLuaFileList";
		((Control)lbLuaFileList).Size = new Size(846, 541);
		((Control)lbLuaFileList).TabIndex = 2;
		((Control)lbLuaFileList).DoubleClick += lbLuaDoString_DoubleClick;
		((Control)tbLuaEdit).BackColor = SystemColors.Info;
		((Control)tbLuaEdit).Dock = (DockStyle)5;
		((Control)tbLuaEdit).Font = new Font("宋体", 20f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)tbLuaEdit).Location = new Point(0, 0);
		((Control)tbLuaEdit).Name = "tbLuaEdit";
		((Control)tbLuaEdit).Size = new Size(846, 38);
		((Control)tbLuaEdit).TabIndex = 3;
		((Control)tbLuaEdit).KeyPress += new KeyPressEventHandler(tbLuaEdit_KeyPress);
		splitContainer3.BorderStyle = (BorderStyle)1;
		splitContainer3.Dock = (DockStyle)5;
		((Control)splitContainer3).Location = new Point(0, 0);
		((Control)splitContainer3).Name = "splitContainer3";
		splitContainer3.Orientation = (Orientation)0;
		((Control)splitContainer3.Panel1).Controls.Add((Control)(object)splitContainer5);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnContinueFileRun);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnPauseFileRun);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnQuiteFileRun);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)cbLuaFilePosIndex);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnSpiFileUpload);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnLuaRunFile);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)label1);
		((Control)splitContainer3.Panel2).Controls.Add((Control)(object)bnSpiFileDownload);
		((Control)splitContainer3).Size = new Size(306, 586);
		splitContainer3.SplitterDistance = 472;
		((Control)splitContainer3).TabIndex = 0;
		splitContainer5.BorderStyle = (BorderStyle)1;
		splitContainer5.Dock = (DockStyle)5;
		((Control)splitContainer5).Location = new Point(0, 0);
		((Control)splitContainer5).Name = "splitContainer5";
		splitContainer5.Orientation = (Orientation)0;
		((Control)splitContainer5.Panel1).Controls.Add((Control)(object)tabLuaCmndEdit);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnDelTab);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnAddTab);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnLuaDoString);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnLuaDel);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnAppend);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnInsert);
		((Control)splitContainer5.Panel2).Controls.Add((Control)(object)bnLuaInsertNewLine);
		((Control)splitContainer5).Size = new Size(306, 472);
		splitContainer5.SplitterDistance = 341;
		((Control)splitContainer5).TabIndex = 11;
		((Control)tabLuaCmndEdit).Controls.Add((Control)(object)tbAxisCmnd);
		((Control)tabLuaCmndEdit).Controls.Add((Control)(object)tbGroupCmnd);
		((Control)tabLuaCmndEdit).Controls.Add((Control)(object)tbAskCmnd);
		((Control)tabLuaCmndEdit).Controls.Add((Control)(object)tbIOCmnd);
		((Control)tabLuaCmndEdit).Controls.Add((Control)(object)tbKeyWord);
		((Control)tabLuaCmndEdit).Dock = (DockStyle)5;
		((Control)tabLuaCmndEdit).Location = new Point(0, 0);
		((Control)tabLuaCmndEdit).Name = "tabLuaCmndEdit";
		tabLuaCmndEdit.SelectedIndex = 0;
		((Control)tabLuaCmndEdit).Size = new Size(304, 339);
		((Control)tabLuaCmndEdit).TabIndex = 10;
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPSetSpd);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPMovPre);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)ckbPTPDelay);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)tbPTPDelay);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)ckbPTPAcc);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)ckbPTPVel);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)ckbPTPDist);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPSetPos);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbQuiteHome);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbHomeB);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbHome);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)tbPTPAcc);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)tbPTPVel);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)tbPTPDist);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)label5);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)cbAxisID);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbEStop);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbStopB);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbStop);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPCmndMoveJBR);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPCmndMoveJB);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPCmndMoveJR);
		((Control)tbAxisCmnd).Controls.Add((Control)(object)rbPTPCmndMoveJ);
		tbAxisCmnd.Location = new Point(4, 22);
		((Control)tbAxisCmnd).Name = "tbAxisCmnd";
		((Control)tbAxisCmnd).Padding = new Padding(3);
		((Control)tbAxisCmnd).Size = new Size(296, 313);
		tbAxisCmnd.TabIndex = 0;
		((Control)tbAxisCmnd).Text = "单轴指令";
		tbAxisCmnd.UseVisualStyleBackColor = true;
		((Control)rbPTPSetSpd).AutoSize = true;
		((Control)rbPTPSetSpd).Location = new Point(90, 72);
		((Control)rbPTPSetSpd).Name = "rbPTPSetSpd";
		((Control)rbPTPSetSpd).Size = new Size(65, 16);
		((Control)rbPTPSetSpd).TabIndex = 46;
		((Control)rbPTPSetSpd).Text = "SetJSpd";
		((ButtonBase)rbPTPSetSpd).UseVisualStyleBackColor = true;
		((Control)rbPTPMovPre).AutoSize = true;
		((Control)rbPTPMovPre).Location = new Point(16, 94);
		((Control)rbPTPMovPre).Name = "rbPTPMovPre";
		((Control)rbPTPMovPre).Size = new Size(65, 16);
		((Control)rbPTPMovPre).TabIndex = 45;
		((Control)rbPTPMovPre).Text = "MovJPre";
		((ButtonBase)rbPTPMovPre).UseVisualStyleBackColor = true;
		((Control)ckbPTPDelay).AutoSize = true;
		((Control)ckbPTPDelay).Location = new Point(11, 206);
		((Control)ckbPTPDelay).Name = "ckbPTPDelay";
		((Control)ckbPTPDelay).Size = new Size(48, 16);
		((Control)ckbPTPDelay).TabIndex = 44;
		((Control)ckbPTPDelay).Text = "延时";
		((ButtonBase)ckbPTPDelay).UseVisualStyleBackColor = true;
		((Control)tbPTPDelay).Location = new Point(63, 203);
		((Control)tbPTPDelay).Name = "tbPTPDelay";
		((Control)tbPTPDelay).Size = new Size(53, 21);
		((Control)tbPTPDelay).TabIndex = 43;
		((Control)tbPTPDelay).Text = "0";
		((Control)ckbPTPAcc).AutoSize = true;
		((Control)ckbPTPAcc).Location = new Point(124, 179);
		((Control)ckbPTPAcc).Name = "ckbPTPAcc";
		((Control)ckbPTPAcc).Size = new Size(60, 16);
		((Control)ckbPTPAcc).TabIndex = 42;
		((Control)ckbPTPAcc).Text = "加速度";
		((ButtonBase)ckbPTPAcc).UseVisualStyleBackColor = true;
		((Control)ckbPTPVel).AutoSize = true;
		((Control)ckbPTPVel).Location = new Point(11, 179);
		((Control)ckbPTPVel).Name = "ckbPTPVel";
		((Control)ckbPTPVel).Size = new Size(48, 16);
		((Control)ckbPTPVel).TabIndex = 41;
		((Control)ckbPTPVel).Text = "速度";
		((ButtonBase)ckbPTPVel).UseVisualStyleBackColor = true;
		((Control)ckbPTPDist).AutoSize = true;
		((Control)ckbPTPDist).Location = new Point(124, 152);
		((Control)ckbPTPDist).Name = "ckbPTPDist";
		((Control)ckbPTPDist).Size = new Size(48, 16);
		((Control)ckbPTPDist).TabIndex = 38;
		((Control)ckbPTPDist).Text = "距离";
		((ButtonBase)ckbPTPDist).UseVisualStyleBackColor = true;
		((Control)rbPTPSetPos).AutoSize = true;
		((Control)rbPTPSetPos).Location = new Point(16, 72);
		((Control)rbPTPSetPos).Name = "rbPTPSetPos";
		((Control)rbPTPSetPos).Size = new Size(65, 16);
		((Control)rbPTPSetPos).TabIndex = 37;
		((Control)rbPTPSetPos).Text = "SetJPos";
		((ButtonBase)rbPTPSetPos).UseVisualStyleBackColor = true;
		((Control)rbQuiteHome).AutoSize = true;
		((Control)rbQuiteHome).Location = new Point(162, 50);
		((Control)rbQuiteHome).Name = "rbQuiteHome";
		((Control)rbQuiteHome).Size = new Size(47, 16);
		((Control)rbQuiteHome).TabIndex = 36;
		((Control)rbQuiteHome).Text = "QHom";
		((ButtonBase)rbQuiteHome).UseVisualStyleBackColor = true;
		((Control)rbHomeB).AutoSize = true;
		((Control)rbHomeB).Location = new Point(90, 50);
		((Control)rbHomeB).Name = "rbHomeB";
		((Control)rbHomeB).Size = new Size(47, 16);
		((Control)rbHomeB).TabIndex = 35;
		((Control)rbHomeB).Text = "HomB";
		((ButtonBase)rbHomeB).UseVisualStyleBackColor = true;
		((Control)rbHome).AutoSize = true;
		((Control)rbHome).Location = new Point(16, 50);
		((Control)rbHome).Name = "rbHome";
		((Control)rbHome).Size = new Size(41, 16);
		((Control)rbHome).TabIndex = 34;
		((Control)rbHome).Text = "Hom";
		((ButtonBase)rbHome).UseVisualStyleBackColor = true;
		((Control)tbPTPAcc).Location = new Point(188, 177);
		((Control)tbPTPAcc).Name = "tbPTPAcc";
		((Control)tbPTPAcc).Size = new Size(53, 21);
		((Control)tbPTPAcc).TabIndex = 33;
		((Control)tbPTPAcc).Text = "0.5";
		((Control)tbPTPVel).Location = new Point(63, 176);
		((Control)tbPTPVel).Name = "tbPTPVel";
		((Control)tbPTPVel).Size = new Size(53, 21);
		((Control)tbPTPVel).TabIndex = 31;
		((Control)tbPTPVel).Text = "10.0";
		((Control)tbPTPDist).Location = new Point(188, 149);
		((Control)tbPTPDist).Name = "tbPTPDist";
		((Control)tbPTPDist).Size = new Size(53, 21);
		((Control)tbPTPDist).TabIndex = 29;
		((Control)tbPTPDist).Text = "10.0";
		((Control)label5).AutoSize = true;
		((Control)label5).Location = new Point(42, 154);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(17, 12);
		((Control)label5).TabIndex = 27;
		((Control)label5).Text = "ID";
		((ListControl)cbAxisID).FormattingEnabled = true;
		cbAxisID.Items.AddRange(new object[7] { "0", "1", "2", "3", "4", "5", "255" });
		((Control)cbAxisID).Location = new Point(63, 150);
		((Control)cbAxisID).Name = "cbAxisID";
		((Control)cbAxisID).Size = new Size(53, 20);
		((Control)cbAxisID).TabIndex = 26;
		((Control)rbEStop).AutoSize = true;
		((Control)rbEStop).Location = new Point(162, 28);
		((Control)rbEStop).Name = "rbEStop";
		((Control)rbEStop).Size = new Size(53, 16);
		((Control)rbEStop).TabIndex = 25;
		((Control)rbEStop).Text = "EStop";
		((ButtonBase)rbEStop).UseVisualStyleBackColor = true;
		((Control)rbStopB).AutoSize = true;
		((Control)rbStopB).Location = new Point(90, 28);
		((Control)rbStopB).Name = "rbStopB";
		((Control)rbStopB).Size = new Size(59, 16);
		((Control)rbStopB).TabIndex = 24;
		((Control)rbStopB).Text = "StopJB";
		((ButtonBase)rbStopB).UseVisualStyleBackColor = true;
		((Control)rbStop).AutoSize = true;
		((Control)rbStop).Location = new Point(16, 28);
		((Control)rbStop).Name = "rbStop";
		((Control)rbStop).Size = new Size(53, 16);
		((Control)rbStop).TabIndex = 23;
		((Control)rbStop).Text = "StopJ";
		((ButtonBase)rbStop).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJBR).AutoSize = true;
		((Control)rbPTPCmndMoveJBR).Location = new Point(232, 6);
		((Control)rbPTPCmndMoveJBR).Name = "rbPTPCmndMoveJBR";
		((Control)rbPTPCmndMoveJBR).Size = new Size(59, 16);
		((Control)rbPTPCmndMoveJBR).TabIndex = 22;
		((Control)rbPTPCmndMoveJBR).Text = "MovJRB";
		((ButtonBase)rbPTPCmndMoveJBR).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJB).AutoSize = true;
		((Control)rbPTPCmndMoveJB).Location = new Point(162, 6);
		((Control)rbPTPCmndMoveJB).Name = "rbPTPCmndMoveJB";
		((Control)rbPTPCmndMoveJB).Size = new Size(53, 16);
		((Control)rbPTPCmndMoveJB).TabIndex = 21;
		((Control)rbPTPCmndMoveJB).Text = "MovJB";
		((ButtonBase)rbPTPCmndMoveJB).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJR).AutoSize = true;
		((Control)rbPTPCmndMoveJR).Location = new Point(90, 6);
		((Control)rbPTPCmndMoveJR).Name = "rbPTPCmndMoveJR";
		((Control)rbPTPCmndMoveJR).Size = new Size(53, 16);
		((Control)rbPTPCmndMoveJR).TabIndex = 20;
		((Control)rbPTPCmndMoveJR).Text = "MovJR";
		((ButtonBase)rbPTPCmndMoveJR).UseVisualStyleBackColor = true;
		((Control)rbPTPCmndMoveJ).AutoSize = true;
		rbPTPCmndMoveJ.Checked = true;
		((Control)rbPTPCmndMoveJ).Location = new Point(16, 6);
		((Control)rbPTPCmndMoveJ).Name = "rbPTPCmndMoveJ";
		((Control)rbPTPCmndMoveJ).Size = new Size(47, 16);
		((Control)rbPTPCmndMoveJ).TabIndex = 18;
		rbPTPCmndMoveJ.TabStop = true;
		((Control)rbPTPCmndMoveJ).Text = "MovJ";
		((ButtonBase)rbPTPCmndMoveJ).UseVisualStyleBackColor = true;
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpMovAcc);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpMovSpd);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)label7);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpCmndAxisEnStt);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)bnGrpAxisEnable);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovAcc);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovVel);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosC);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosA);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosZ);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosY);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)tbGrpMovPosX);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnC);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnA);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnZ);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnY);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)ckbGrpAxisEnX);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndSetGrpSpd);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovLRB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovLB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovLR);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovL);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovPRB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovPB);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovPR);
		((Control)tbGroupCmnd).Controls.Add((Control)(object)rbGrpCmndMovP);
		tbGroupCmnd.Location = new Point(4, 22);
		((Control)tbGroupCmnd).Name = "tbGroupCmnd";
		((Control)tbGroupCmnd).Padding = new Padding(3);
		((Control)tbGroupCmnd).Size = new Size(296, 313);
		tbGroupCmnd.TabIndex = 2;
		((Control)tbGroupCmnd).Text = "多轴组令";
		tbGroupCmnd.UseVisualStyleBackColor = true;
		((Control)ckbGrpMovAcc).AutoSize = true;
		((Control)ckbGrpMovAcc).Location = new Point(133, 185);
		((Control)ckbGrpMovAcc).Name = "ckbGrpMovAcc";
		((Control)ckbGrpMovAcc).Size = new Size(60, 16);
		((Control)ckbGrpMovAcc).TabIndex = 64;
		((Control)ckbGrpMovAcc).Text = "加速度";
		((ButtonBase)ckbGrpMovAcc).UseVisualStyleBackColor = true;
		((Control)ckbGrpMovSpd).AutoSize = true;
		((Control)ckbGrpMovSpd).Location = new Point(133, 158);
		((Control)ckbGrpMovSpd).Name = "ckbGrpMovSpd";
		((Control)ckbGrpMovSpd).Size = new Size(48, 16);
		((Control)ckbGrpMovSpd).TabIndex = 63;
		((Control)ckbGrpMovSpd).Text = "速度";
		((ButtonBase)ckbGrpMovSpd).UseVisualStyleBackColor = true;
		((Control)label7).AutoSize = true;
		((Control)label7).Location = new Point(158, 134);
		((Control)label7).Name = "label7";
		((Control)label7).Size = new Size(29, 12);
		((Control)label7).TabIndex = 62;
		((Control)label7).Text = "轴选";
		((Control)tbGrpCmndAxisEnStt).Enabled = false;
		((Control)tbGrpCmndAxisEnStt).Location = new Point(197, 128);
		((Control)tbGrpCmndAxisEnStt).Name = "tbGrpCmndAxisEnStt";
		((Control)tbGrpCmndAxisEnStt).Size = new Size(80, 21);
		((Control)tbGrpCmndAxisEnStt).TabIndex = 61;
		((Control)tbGrpCmndAxisEnStt).Text = "0";
		((Control)bnGrpAxisEnable).Anchor = (AnchorStyles)10;
		((Control)bnGrpAxisEnable).Location = new Point(202, 250);
		((Control)bnGrpAxisEnable).Name = "bnGrpAxisEnable";
		((Control)bnGrpAxisEnable).Size = new Size(75, 38);
		((Control)bnGrpAxisEnable).TabIndex = 60;
		((Control)bnGrpAxisEnable).Text = "切换轴选";
		((ButtonBase)bnGrpAxisEnable).UseVisualStyleBackColor = true;
		((Control)bnGrpAxisEnable).Click += bnGrpAxisEnable_Click;
		((Control)tbGrpMovAcc).Location = new Point(197, 182);
		((Control)tbGrpMovAcc).Name = "tbGrpMovAcc";
		((Control)tbGrpMovAcc).Size = new Size(80, 21);
		((Control)tbGrpMovAcc).TabIndex = 58;
		((Control)tbGrpMovAcc).Text = "10.0";
		((Control)tbGrpMovVel).Location = new Point(197, 155);
		((Control)tbGrpMovVel).Name = "tbGrpMovVel";
		((Control)tbGrpMovVel).Size = new Size(80, 21);
		((Control)tbGrpMovVel).TabIndex = 57;
		((Control)tbGrpMovVel).Text = "1.0";
		((Control)tbGrpMovPosC).Location = new Point(44, 263);
		((Control)tbGrpMovPosC).Name = "tbGrpMovPosC";
		((Control)tbGrpMovPosC).Size = new Size(80, 21);
		((Control)tbGrpMovPosC).TabIndex = 54;
		((Control)tbGrpMovPosC).Text = "10.0";
		((Control)tbGrpMovPosB).Location = new Point(44, 236);
		((Control)tbGrpMovPosB).Name = "tbGrpMovPosB";
		((Control)tbGrpMovPosB).Size = new Size(80, 21);
		((Control)tbGrpMovPosB).TabIndex = 53;
		((Control)tbGrpMovPosB).Text = "10.0";
		((Control)tbGrpMovPosA).Location = new Point(44, 209);
		((Control)tbGrpMovPosA).Name = "tbGrpMovPosA";
		((Control)tbGrpMovPosA).Size = new Size(80, 21);
		((Control)tbGrpMovPosA).TabIndex = 52;
		((Control)tbGrpMovPosA).Text = "10.0";
		((Control)tbGrpMovPosZ).Location = new Point(44, 182);
		((Control)tbGrpMovPosZ).Name = "tbGrpMovPosZ";
		((Control)tbGrpMovPosZ).Size = new Size(80, 21);
		((Control)tbGrpMovPosZ).TabIndex = 51;
		((Control)tbGrpMovPosZ).Text = "10.0";
		((Control)tbGrpMovPosY).Location = new Point(44, 155);
		((Control)tbGrpMovPosY).Name = "tbGrpMovPosY";
		((Control)tbGrpMovPosY).Size = new Size(80, 21);
		((Control)tbGrpMovPosY).TabIndex = 50;
		((Control)tbGrpMovPosY).Text = "10.0";
		((Control)tbGrpMovPosX).Location = new Point(44, 128);
		((Control)tbGrpMovPosX).Name = "tbGrpMovPosX";
		((Control)tbGrpMovPosX).Size = new Size(80, 21);
		((Control)tbGrpMovPosX).TabIndex = 49;
		((Control)tbGrpMovPosX).Text = "10.0";
		((Control)ckbGrpAxisEnC).AutoSize = true;
		((Control)ckbGrpAxisEnC).Location = new Point(10, 265);
		((Control)ckbGrpAxisEnC).Name = "ckbGrpAxisEnC";
		((Control)ckbGrpAxisEnC).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnC).TabIndex = 48;
		((Control)ckbGrpAxisEnC).Text = "C";
		((ButtonBase)ckbGrpAxisEnC).UseVisualStyleBackColor = true;
		ckbGrpAxisEnC.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)ckbGrpAxisEnB).AutoSize = true;
		((Control)ckbGrpAxisEnB).Location = new Point(10, 238);
		((Control)ckbGrpAxisEnB).Name = "ckbGrpAxisEnB";
		((Control)ckbGrpAxisEnB).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnB).TabIndex = 48;
		((Control)ckbGrpAxisEnB).Text = "B";
		((ButtonBase)ckbGrpAxisEnB).UseVisualStyleBackColor = true;
		ckbGrpAxisEnB.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)ckbGrpAxisEnA).AutoSize = true;
		((Control)ckbGrpAxisEnA).Location = new Point(10, 211);
		((Control)ckbGrpAxisEnA).Name = "ckbGrpAxisEnA";
		((Control)ckbGrpAxisEnA).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnA).TabIndex = 48;
		((Control)ckbGrpAxisEnA).Text = "A";
		((ButtonBase)ckbGrpAxisEnA).UseVisualStyleBackColor = true;
		ckbGrpAxisEnA.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)ckbGrpAxisEnZ).AutoSize = true;
		((Control)ckbGrpAxisEnZ).Location = new Point(10, 184);
		((Control)ckbGrpAxisEnZ).Name = "ckbGrpAxisEnZ";
		((Control)ckbGrpAxisEnZ).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnZ).TabIndex = 48;
		((Control)ckbGrpAxisEnZ).Text = "Z";
		((ButtonBase)ckbGrpAxisEnZ).UseVisualStyleBackColor = true;
		ckbGrpAxisEnZ.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)ckbGrpAxisEnY).AutoSize = true;
		((Control)ckbGrpAxisEnY).Location = new Point(10, 157);
		((Control)ckbGrpAxisEnY).Name = "ckbGrpAxisEnY";
		((Control)ckbGrpAxisEnY).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnY).TabIndex = 48;
		((Control)ckbGrpAxisEnY).Text = "Y";
		((ButtonBase)ckbGrpAxisEnY).UseVisualStyleBackColor = true;
		ckbGrpAxisEnY.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)ckbGrpAxisEnX).AutoSize = true;
		((Control)ckbGrpAxisEnX).Location = new Point(10, 130);
		((Control)ckbGrpAxisEnX).Name = "ckbGrpAxisEnX";
		((Control)ckbGrpAxisEnX).Size = new Size(30, 16);
		((Control)ckbGrpAxisEnX).TabIndex = 48;
		((Control)ckbGrpAxisEnX).Text = "X";
		((ButtonBase)ckbGrpAxisEnX).UseVisualStyleBackColor = true;
		ckbGrpAxisEnX.CheckedChanged += ckbGrpAxisEnX_CheckedChanged;
		((Control)rbGrpCmndSetGrpSpd).AutoSize = true;
		((Control)rbGrpCmndSetGrpSpd).Location = new Point(10, 68);
		((Control)rbGrpCmndSetGrpSpd).Name = "rbGrpCmndSetGrpSpd";
		((Control)rbGrpCmndSetGrpSpd).Size = new Size(77, 16);
		((Control)rbGrpCmndSetGrpSpd).TabIndex = 47;
		((Control)rbGrpCmndSetGrpSpd).Text = "SetGrpSpd";
		((ButtonBase)rbGrpCmndSetGrpSpd).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovLRB).AutoSize = true;
		((Control)rbGrpCmndMovLRB).Location = new Point(226, 28);
		((Control)rbGrpCmndMovLRB).Name = "rbGrpCmndMovLRB";
		((Control)rbGrpCmndMovLRB).Size = new Size(59, 16);
		((Control)rbGrpCmndMovLRB).TabIndex = 30;
		((Control)rbGrpCmndMovLRB).Text = "MovLRB";
		((ButtonBase)rbGrpCmndMovLRB).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovLB).AutoSize = true;
		((Control)rbGrpCmndMovLB).Location = new Point(152, 28);
		((Control)rbGrpCmndMovLB).Name = "rbGrpCmndMovLB";
		((Control)rbGrpCmndMovLB).Size = new Size(53, 16);
		((Control)rbGrpCmndMovLB).TabIndex = 29;
		((Control)rbGrpCmndMovLB).Text = "MovLB";
		((ButtonBase)rbGrpCmndMovLB).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovLR).AutoSize = true;
		((Control)rbGrpCmndMovLR).Location = new Point(78, 28);
		((Control)rbGrpCmndMovLR).Name = "rbGrpCmndMovLR";
		((Control)rbGrpCmndMovLR).Size = new Size(53, 16);
		((Control)rbGrpCmndMovLR).TabIndex = 28;
		((Control)rbGrpCmndMovLR).Text = "MovLR";
		((ButtonBase)rbGrpCmndMovLR).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovL).AutoSize = true;
		((Control)rbGrpCmndMovL).Location = new Point(10, 28);
		((Control)rbGrpCmndMovL).Name = "rbGrpCmndMovL";
		((Control)rbGrpCmndMovL).Size = new Size(47, 16);
		((Control)rbGrpCmndMovL).TabIndex = 27;
		((Control)rbGrpCmndMovL).Text = "MovL";
		((ButtonBase)rbGrpCmndMovL).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovPRB).AutoSize = true;
		((Control)rbGrpCmndMovPRB).Location = new Point(226, 6);
		((Control)rbGrpCmndMovPRB).Name = "rbGrpCmndMovPRB";
		((Control)rbGrpCmndMovPRB).Size = new Size(59, 16);
		((Control)rbGrpCmndMovPRB).TabIndex = 26;
		((Control)rbGrpCmndMovPRB).Text = "MovPRB";
		((ButtonBase)rbGrpCmndMovPRB).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovPB).AutoSize = true;
		((Control)rbGrpCmndMovPB).Location = new Point(152, 6);
		((Control)rbGrpCmndMovPB).Name = "rbGrpCmndMovPB";
		((Control)rbGrpCmndMovPB).Size = new Size(53, 16);
		((Control)rbGrpCmndMovPB).TabIndex = 25;
		((Control)rbGrpCmndMovPB).Text = "MovPB";
		((ButtonBase)rbGrpCmndMovPB).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovPR).AutoSize = true;
		((Control)rbGrpCmndMovPR).Location = new Point(78, 6);
		((Control)rbGrpCmndMovPR).Name = "rbGrpCmndMovPR";
		((Control)rbGrpCmndMovPR).Size = new Size(53, 16);
		((Control)rbGrpCmndMovPR).TabIndex = 24;
		((Control)rbGrpCmndMovPR).Text = "MovPR";
		((ButtonBase)rbGrpCmndMovPR).UseVisualStyleBackColor = true;
		((Control)rbGrpCmndMovP).AutoSize = true;
		rbGrpCmndMovP.Checked = true;
		((Control)rbGrpCmndMovP).Location = new Point(10, 6);
		((Control)rbGrpCmndMovP).Name = "rbGrpCmndMovP";
		((Control)rbGrpCmndMovP).Size = new Size(47, 16);
		((Control)rbGrpCmndMovP).TabIndex = 23;
		rbGrpCmndMovP.TabStop = true;
		((Control)rbGrpCmndMovP).Text = "MovP";
		((ButtonBase)rbGrpCmndMovP).UseVisualStyleBackColor = true;
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskGrpPos);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskGrpSpd);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskGrpRunStt);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskJAlarm);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskJPos);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskJSpd);
		((Control)tbAskCmnd).Controls.Add((Control)(object)label2);
		((Control)tbAskCmnd).Controls.Add((Control)(object)cbAskNCDevID);
		((Control)tbAskCmnd).Controls.Add((Control)(object)rbAskJRunStt);
		tbAskCmnd.Location = new Point(4, 22);
		((Control)tbAskCmnd).Name = "tbAskCmnd";
		((Control)tbAskCmnd).Padding = new Padding(3);
		((Control)tbAskCmnd).Size = new Size(296, 313);
		tbAskCmnd.TabIndex = 3;
		((Control)tbAskCmnd).Text = "查询指令";
		tbAskCmnd.UseVisualStyleBackColor = true;
		((Control)rbAskGrpPos).AutoSize = true;
		((Control)rbAskGrpPos).Location = new Point(202, 69);
		((Control)rbAskGrpPos).Name = "rbAskGrpPos";
		((Control)rbAskGrpPos).Size = new Size(77, 16);
		((Control)rbAskGrpPos).TabIndex = 36;
		((Control)rbAskGrpPos).Text = "GetGrpPos";
		((ButtonBase)rbAskGrpPos).UseVisualStyleBackColor = true;
		((Control)rbAskGrpSpd).AutoSize = true;
		((Control)rbAskGrpSpd).Location = new Point(106, 69);
		((Control)rbAskGrpSpd).Name = "rbAskGrpSpd";
		((Control)rbAskGrpSpd).Size = new Size(77, 16);
		((Control)rbAskGrpSpd).TabIndex = 35;
		((Control)rbAskGrpSpd).Text = "GetGrpSpd";
		((ButtonBase)rbAskGrpSpd).UseVisualStyleBackColor = true;
		((Control)rbAskGrpRunStt).AutoSize = true;
		((Control)rbAskGrpRunStt).Location = new Point(10, 69);
		((Control)rbAskGrpRunStt).Name = "rbAskGrpRunStt";
		((Control)rbAskGrpRunStt).Size = new Size(77, 16);
		((Control)rbAskGrpRunStt).TabIndex = 34;
		((Control)rbAskGrpRunStt).Text = "GetGrpStt";
		((ButtonBase)rbAskGrpRunStt).UseVisualStyleBackColor = true;
		((Control)rbAskJAlarm).AutoSize = true;
		((Control)rbAskJAlarm).Location = new Point(10, 28);
		((Control)rbAskJAlarm).Name = "rbAskJAlarm";
		((Control)rbAskJAlarm).Size = new Size(77, 16);
		((Control)rbAskJAlarm).TabIndex = 33;
		((Control)rbAskJAlarm).Text = "GetJAlarm";
		((ButtonBase)rbAskJAlarm).UseVisualStyleBackColor = true;
		((Control)rbAskJPos).AutoSize = true;
		((Control)rbAskJPos).Location = new Point(202, 6);
		((Control)rbAskJPos).Name = "rbAskJPos";
		((Control)rbAskJPos).Size = new Size(65, 16);
		((Control)rbAskJPos).TabIndex = 32;
		((Control)rbAskJPos).Text = "GetJPos";
		((ButtonBase)rbAskJPos).UseVisualStyleBackColor = true;
		((Control)rbAskJSpd).AutoSize = true;
		((Control)rbAskJSpd).Location = new Point(106, 6);
		((Control)rbAskJSpd).Name = "rbAskJSpd";
		((Control)rbAskJSpd).Size = new Size(65, 16);
		((Control)rbAskJSpd).TabIndex = 31;
		((Control)rbAskJSpd).Text = "GetJSpd";
		((ButtonBase)rbAskJSpd).UseVisualStyleBackColor = true;
		((Control)label2).AutoSize = true;
		((Control)label2).Location = new Point(8, 178);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(17, 12);
		((Control)label2).TabIndex = 30;
		((Control)label2).Text = "ID";
		((ListControl)cbAskNCDevID).FormattingEnabled = true;
		cbAskNCDevID.Items.AddRange(new object[7] { "0", "1", "2", "3", "4", "5", "255" });
		((Control)cbAskNCDevID).Location = new Point(28, 175);
		((Control)cbAskNCDevID).Name = "cbAskNCDevID";
		((Control)cbAskNCDevID).Size = new Size(53, 20);
		((Control)cbAskNCDevID).TabIndex = 29;
		((Control)cbAskNCDevID).Text = "0";
		((Control)rbAskJRunStt).AutoSize = true;
		rbAskJRunStt.Checked = true;
		((Control)rbAskJRunStt).Location = new Point(10, 6);
		((Control)rbAskJRunStt).Name = "rbAskJRunStt";
		((Control)rbAskJRunStt).Size = new Size(65, 16);
		((Control)rbAskJRunStt).TabIndex = 27;
		rbAskJRunStt.TabStop = true;
		((Control)rbAskJRunStt).Text = "GetJStt";
		((ButtonBase)rbAskJRunStt).UseVisualStyleBackColor = true;
		((Control)tbIOCmnd).Controls.Add((Control)(object)ckbIOVal);
		((Control)tbIOCmnd).Controls.Add((Control)(object)rbIOCmndWDI);
		((Control)tbIOCmnd).Controls.Add((Control)(object)rbIOCmndDO);
		((Control)tbIOCmnd).Controls.Add((Control)(object)label3);
		((Control)tbIOCmnd).Controls.Add((Control)(object)cbIOCmndIndex);
		((Control)tbIOCmnd).Controls.Add((Control)(object)rbIOCmndDI);
		tbIOCmnd.Location = new Point(4, 22);
		((Control)tbIOCmnd).Name = "tbIOCmnd";
		((Control)tbIOCmnd).Padding = new Padding(3);
		((Control)tbIOCmnd).Size = new Size(296, 313);
		tbIOCmnd.TabIndex = 4;
		((Control)tbIOCmnd).Text = "IO指令";
		tbIOCmnd.UseVisualStyleBackColor = true;
		((Control)ckbIOVal).AutoSize = true;
		((Control)ckbIOVal).Location = new Point(198, 116);
		((Control)ckbIOVal).Name = "ckbIOVal";
		((Control)ckbIOVal).Size = new Size(48, 16);
		((Control)ckbIOVal).TabIndex = 42;
		((Control)ckbIOVal).Text = "无效";
		((ButtonBase)ckbIOVal).UseVisualStyleBackColor = true;
		ckbIOVal.CheckedChanged += ckbIOVal_CheckedChanged;
		((Control)rbIOCmndWDI).AutoSize = true;
		((Control)rbIOCmndWDI).Location = new Point(80, 6);
		((Control)rbIOCmndWDI).Name = "rbIOCmndWDI";
		((Control)rbIOCmndWDI).Size = new Size(41, 16);
		((Control)rbIOCmndWDI).TabIndex = 35;
		((Control)rbIOCmndWDI).Text = "WDI";
		((ButtonBase)rbIOCmndWDI).UseVisualStyleBackColor = true;
		((Control)rbIOCmndDO).AutoSize = true;
		((Control)rbIOCmndDO).Location = new Point(9, 28);
		((Control)rbIOCmndDO).Name = "rbIOCmndDO";
		((Control)rbIOCmndDO).Size = new Size(35, 16);
		((Control)rbIOCmndDO).TabIndex = 34;
		((Control)rbIOCmndDO).Text = "DO";
		((ButtonBase)rbIOCmndDO).UseVisualStyleBackColor = true;
		((Control)label3).AutoSize = true;
		((Control)label3).Location = new Point(7, 117);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(53, 12);
		((Control)label3).TabIndex = 33;
		((Control)label3).Text = "IO Index";
		((ListControl)cbIOCmndIndex).FormattingEnabled = true;
		cbIOCmndIndex.Items.AddRange(new object[33]
		{
			"255", "0", "1", "2", "3", "4", "5", "6", "7", "8",
			"9", "10", "11", "12", "13", "14", "15", "16", "17", "18",
			"19", "20", "21", "22", "23", "24", "25", "26", "27", "28",
			"29", "30", "31"
		});
		((Control)cbIOCmndIndex).Location = new Point(66, 114);
		((Control)cbIOCmndIndex).Name = "cbIOCmndIndex";
		((Control)cbIOCmndIndex).Size = new Size(115, 20);
		((Control)cbIOCmndIndex).TabIndex = 32;
		((Control)cbIOCmndIndex).Text = "0";
		((Control)rbIOCmndDI).AutoSize = true;
		rbIOCmndDI.Checked = true;
		((Control)rbIOCmndDI).Location = new Point(9, 6);
		((Control)rbIOCmndDI).Name = "rbIOCmndDI";
		((Control)rbIOCmndDI).Size = new Size(35, 16);
		((Control)rbIOCmndDI).TabIndex = 31;
		rbIOCmndDI.TabStop = true;
		((Control)rbIOCmndDI).Text = "DI";
		((ButtonBase)rbIOCmndDI).UseVisualStyleBackColor = true;
		((Control)tbKeyWord).Controls.Add((Control)(object)button10);
		((Control)tbKeyWord).Controls.Add((Control)(object)button9);
		((Control)tbKeyWord).Controls.Add((Control)(object)richTextBox1);
		((Control)tbKeyWord).Controls.Add((Control)(object)button8);
		((Control)tbKeyWord).Controls.Add((Control)(object)button7);
		((Control)tbKeyWord).Controls.Add((Control)(object)button6);
		((Control)tbKeyWord).Controls.Add((Control)(object)button5);
		((Control)tbKeyWord).Controls.Add((Control)(object)button4);
		((Control)tbKeyWord).Controls.Add((Control)(object)button3);
		((Control)tbKeyWord).Controls.Add((Control)(object)button2);
		((Control)tbKeyWord).Controls.Add((Control)(object)button1);
		((Control)tbKeyWord).Controls.Add((Control)(object)bnCompile);
		((Control)tbKeyWord).Controls.Add((Control)(object)bnStep);
		((Control)tbKeyWord).Controls.Add((Control)(object)bnRun);
		tbKeyWord.Location = new Point(4, 22);
		((Control)tbKeyWord).Name = "tbKeyWord";
		((Control)tbKeyWord).Padding = new Padding(3);
		((Control)tbKeyWord).Size = new Size(296, 313);
		tbKeyWord.TabIndex = 1;
		((Control)tbKeyWord).Text = "关键字";
		tbKeyWord.UseVisualStyleBackColor = true;
		((Control)bnCompile).Location = new Point(214, 344);
		((Control)bnCompile).Name = "bnCompile";
		((Control)bnCompile).Size = new Size(75, 41);
		((Control)bnCompile).TabIndex = 5;
		((Control)bnCompile).Text = "编译";
		((ButtonBase)bnCompile).UseVisualStyleBackColor = true;
		((Control)bnStep).Location = new Point(133, 344);
		((Control)bnStep).Name = "bnStep";
		((Control)bnStep).Size = new Size(75, 41);
		((Control)bnStep).TabIndex = 8;
		((Control)bnStep).Text = "单步";
		((ButtonBase)bnStep).UseVisualStyleBackColor = true;
		((Control)bnRun).Location = new Point(53, 344);
		((Control)bnRun).Name = "bnRun";
		((Control)bnRun).Size = new Size(75, 41);
		((Control)bnRun).TabIndex = 9;
		((Control)bnRun).Text = "运行";
		((ButtonBase)bnRun).UseVisualStyleBackColor = true;
		((Control)bnDelTab).Location = new Point(157, 47);
		((Control)bnDelTab).Name = "bnDelTab";
		((Control)bnDelTab).Size = new Size(45, 38);
		((Control)bnDelTab).TabIndex = 21;
		((Control)bnDelTab).Text = "<-";
		((ButtonBase)bnDelTab).UseVisualStyleBackColor = true;
		((Control)bnDelTab).Click += bnDelTab_Click;
		((Control)bnAddTab).Location = new Point(106, 47);
		((Control)bnAddTab).Name = "bnAddTab";
		((Control)bnAddTab).Size = new Size(45, 38);
		((Control)bnAddTab).TabIndex = 20;
		((Control)bnAddTab).Text = "->";
		((ButtonBase)bnAddTab).UseVisualStyleBackColor = true;
		((Control)bnAddTab).Click += bnAddTab_Click;
		((Control)bnLuaDoString).Anchor = (AnchorStyles)10;
		((Control)bnLuaDoString).Location = new Point(4, 47);
		((Control)bnLuaDoString).Name = "bnLuaDoString";
		((Control)bnLuaDoString).Size = new Size(96, 38);
		((Control)bnLuaDoString).TabIndex = 3;
		((Control)bnLuaDoString).Text = "执行";
		((ButtonBase)bnLuaDoString).UseVisualStyleBackColor = true;
		((Control)bnLuaDoString).Click += bnLuaDoString_Click;
		((Control)bnLuaDel).Location = new Point(106, 3);
		((Control)bnLuaDel).Name = "bnLuaDel";
		((Control)bnLuaDel).Size = new Size(45, 38);
		((Control)bnLuaDel).TabIndex = 4;
		((Control)bnLuaDel).Text = "删除";
		((ButtonBase)bnLuaDel).UseVisualStyleBackColor = true;
		((Control)bnLuaDel).Click += bnLuaDel_Click;
		((Control)bnAppend).Location = new Point(55, 3);
		((Control)bnAppend).Name = "bnAppend";
		((Control)bnAppend).Size = new Size(45, 38);
		((Control)bnAppend).TabIndex = 7;
		((Control)bnAppend).Text = "追加";
		((ButtonBase)bnAppend).UseVisualStyleBackColor = true;
		((Control)bnAppend).Click += bnAppend_Click;
		((Control)bnInsert).Location = new Point(4, 3);
		((Control)bnInsert).Name = "bnInsert";
		((Control)bnInsert).Size = new Size(45, 38);
		((Control)bnInsert).TabIndex = 6;
		((Control)bnInsert).Text = "插入";
		((ButtonBase)bnInsert).UseVisualStyleBackColor = true;
		((Control)bnInsert).Click += bnInsert_Click;
		((Control)bnLuaInsertNewLine).Location = new Point(157, 3);
		((Control)bnLuaInsertNewLine).Name = "bnLuaInsertNewLine";
		((Control)bnLuaInsertNewLine).Size = new Size(45, 38);
		((Control)bnLuaInsertNewLine).TabIndex = 19;
		((Control)bnLuaInsertNewLine).Text = "换行";
		((ButtonBase)bnLuaInsertNewLine).UseVisualStyleBackColor = true;
		((Control)bnLuaInsertNewLine).Click += bnLuaEditConfirm_Click;
		((Control)bnContinueFileRun).Location = new Point(225, 70);
		((Control)bnContinueFileRun).Name = "bnContinueFileRun";
		((Control)bnContinueFileRun).Size = new Size(75, 32);
		((Control)bnContinueFileRun).TabIndex = 9;
		((Control)bnContinueFileRun).Text = "继续";
		((ButtonBase)bnContinueFileRun).UseVisualStyleBackColor = true;
		((Control)bnContinueFileRun).Click += bnContinueFileRun_Click;
		((Control)bnPauseFileRun).Location = new Point(154, 36);
		((Control)bnPauseFileRun).Name = "bnPauseFileRun";
		((Control)bnPauseFileRun).Size = new Size(65, 65);
		((Control)bnPauseFileRun).TabIndex = 7;
		((Control)bnPauseFileRun).Text = "暂停";
		((ButtonBase)bnPauseFileRun).UseVisualStyleBackColor = true;
		((Control)bnPauseFileRun).Click += bnPauseFileRun_Click;
		((Control)bnQuiteFileRun).Location = new Point(225, 34);
		((Control)bnQuiteFileRun).Name = "bnQuiteFileRun";
		((Control)bnQuiteFileRun).Size = new Size(75, 32);
		((Control)bnQuiteFileRun).TabIndex = 8;
		((Control)bnQuiteFileRun).Text = "退出";
		((ButtonBase)bnQuiteFileRun).UseVisualStyleBackColor = true;
		((Control)bnQuiteFileRun).Click += bnQuiteFileRun_Click;
		((ListControl)cbLuaFilePosIndex).FormattingEnabled = true;
		cbLuaFilePosIndex.Items.AddRange(new object[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
		((Control)cbLuaFilePosIndex).Location = new Point(84, 11);
		((Control)cbLuaFilePosIndex).Name = "cbLuaFilePosIndex";
		((Control)cbLuaFilePosIndex).Size = new Size(216, 20);
		((Control)cbLuaFilePosIndex).TabIndex = 2;
		((Control)bnSpiFileUpload).Location = new Point(3, 70);
		((Control)bnSpiFileUpload).Name = "bnSpiFileUpload";
		((Control)bnSpiFileUpload).Size = new Size(75, 32);
		((Control)bnSpiFileUpload).TabIndex = 4;
		((Control)bnSpiFileUpload).Text = "上传";
		((ButtonBase)bnSpiFileUpload).UseVisualStyleBackColor = true;
		((Control)bnSpiFileUpload).Click += bnSpiFileUpload_Click;
		((Control)bnLuaRunFile).Location = new Point(84, 36);
		((Control)bnLuaRunFile).Name = "bnLuaRunFile";
		((Control)bnLuaRunFile).Size = new Size(65, 65);
		((Control)bnLuaRunFile).TabIndex = 6;
		((Control)bnLuaRunFile).Text = "执行";
		((ButtonBase)bnLuaRunFile).UseVisualStyleBackColor = true;
		((Control)bnLuaRunFile).Click += bnLuaRunFile_Click;
		((Control)label1).AutoSize = true;
		((Control)label1).Location = new Point(8, 14);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(53, 12);
		((Control)label1).TabIndex = 3;
		((Control)label1).Text = "文件位置";
		((Control)bnSpiFileDownload).Location = new Point(3, 34);
		((Control)bnSpiFileDownload).Name = "bnSpiFileDownload";
		((Control)bnSpiFileDownload).Size = new Size(75, 32);
		((Control)bnSpiFileDownload).TabIndex = 5;
		((Control)bnSpiFileDownload).Text = "下载";
		((ButtonBase)bnSpiFileDownload).UseVisualStyleBackColor = true;
		((Control)bnSpiFileDownload).Click += bnSpiFileDownload_Click;
		((ToolStrip)menuStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[2]
		{
			(ToolStripItem)LuaFileOp,
			(ToolStripItem)LuaFileDebug
		});
		((Control)menuStrip1).Location = new Point(0, 0);
		((Control)menuStrip1).Name = "menuStrip1";
		((Control)menuStrip1).Size = new Size(1158, 25);
		((Control)menuStrip1).TabIndex = 0;
		((Control)menuStrip1).Text = "menuStrip1";
		((ToolStripDropDownItem)LuaFileOp).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)File_Open,
			(ToolStripItem)File_New,
			(ToolStripItem)File_Save,
			(ToolStripItem)File_SaveAs
		});
		((ToolStripItem)LuaFileOp).Name = "LuaFileOp";
		((ToolStripItem)LuaFileOp).Size = new Size(44, 21);
		((ToolStripItem)LuaFileOp).Text = "文件";
		((ToolStripItem)File_Open).Name = "File_Open";
		((ToolStripItem)File_Open).Size = new Size(148, 22);
		((ToolStripItem)File_Open).Text = "打开程序文件";
		((ToolStripItem)File_Open).Click += File_Open_Click;
		((ToolStripItem)File_New).Name = "File_New";
		((ToolStripItem)File_New).Size = new Size(148, 22);
		((ToolStripItem)File_New).Text = "新建程序文件";
		((ToolStripItem)File_New).Click += File_New_Click;
		((ToolStripItem)File_Save).Name = "File_Save";
		((ToolStripItem)File_Save).Size = new Size(148, 22);
		((ToolStripItem)File_Save).Text = "保存程序文件";
		((ToolStripItem)File_Save).Click += File_Save_Click;
		((ToolStripItem)File_SaveAs).Name = "File_SaveAs";
		((ToolStripItem)File_SaveAs).Size = new Size(148, 22);
		((ToolStripItem)File_SaveAs).Text = "另存为";
		((ToolStripItem)File_SaveAs).Click += File_SaveAs_Click;
		((ToolStripItem)LuaFileDebug).Name = "LuaFileDebug";
		((ToolStripItem)LuaFileDebug).Size = new Size(44, 21);
		((ToolStripItem)LuaFileDebug).Text = "调试";
		((Control)rtbLuaDebug).Dock = (DockStyle)5;
		((Control)rtbLuaDebug).Location = new Point(0, 0);
		((Control)rtbLuaDebug).Name = "rtbLuaDebug";
		((Control)rtbLuaDebug).Size = new Size(1156, 119);
		((Control)rtbLuaDebug).TabIndex = 0;
		((Control)rtbLuaDebug).Text = "";
		((FileDialog)openFileDialog1).FileName = "openFileDialog1";
		((Control)button1).Location = new Point(6, 6);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(90, 38);
		((Control)button1).TabIndex = 10;
		((Control)button1).Text = "if then else";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button2).Location = new Point(6, 50);
		((Control)button2).Name = "button2";
		((Control)button2).Size = new Size(90, 38);
		((Control)button2).TabIndex = 11;
		((Control)button2).Text = "while";
		((ButtonBase)button2).UseVisualStyleBackColor = true;
		((Control)button3).Location = new Point(6, 94);
		((Control)button3).Name = "button3";
		((Control)button3).Size = new Size(90, 38);
		((Control)button3).TabIndex = 12;
		((Control)button3).Text = "repeat";
		((ButtonBase)button3).UseVisualStyleBackColor = true;
		((Control)button4).Location = new Point(6, 138);
		((Control)button4).Name = "button4";
		((Control)button4).Size = new Size(90, 38);
		((Control)button4).TabIndex = 13;
		((Control)button4).Text = "for";
		((ButtonBase)button4).UseVisualStyleBackColor = true;
		((Control)button5).Location = new Point(102, 50);
		((Control)button5).Name = "button5";
		((Control)button5).Size = new Size(90, 38);
		((Control)button5).TabIndex = 14;
		((Control)button5).Text = "return";
		((ButtonBase)button5).UseVisualStyleBackColor = true;
		((Control)button6).Location = new Point(102, 6);
		((Control)button6).Name = "button6";
		((Control)button6).Size = new Size(90, 38);
		((Control)button6).TabIndex = 15;
		((Control)button6).Text = "break";
		((ButtonBase)button6).UseVisualStyleBackColor = true;
		((Control)button7).Location = new Point(102, 94);
		((Control)button7).Name = "button7";
		((Control)button7).Size = new Size(90, 38);
		((Control)button7).TabIndex = 16;
		((Control)button7).Text = "goto";
		((ButtonBase)button7).UseVisualStyleBackColor = true;
		((Control)button8).Location = new Point(102, 138);
		((Control)button8).Name = "button8";
		((Control)button8).Size = new Size(90, 38);
		((Control)button8).TabIndex = 17;
		((Control)button8).Text = "function";
		((ButtonBase)button8).UseVisualStyleBackColor = true;
		((Control)richTextBox1).Dock = (DockStyle)2;
		((Control)richTextBox1).Location = new Point(3, 182);
		((Control)richTextBox1).Name = "richTextBox1";
		((Control)richTextBox1).Size = new Size(290, 128);
		((Control)richTextBox1).TabIndex = 18;
		((Control)richTextBox1).Text = "";
		((Control)button9).Location = new Point(198, 6);
		((Control)button9).Name = "button9";
		((Control)button9).Size = new Size(90, 38);
		((Control)button9).TabIndex = 19;
		((Control)button9).Text = "local";
		((ButtonBase)button9).UseVisualStyleBackColor = true;
		((Control)button10).Location = new Point(198, 138);
		((Control)button10).Name = "button10";
		((Control)button10).Size = new Size(90, 38);
		((Control)button10).TabIndex = 20;
		((Control)button10).Text = "添加自定义";
		((ButtonBase)button10).UseVisualStyleBackColor = true;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(1158, 736);
		((Control)this).Controls.Add((Control)(object)splitContainer1);
		((Form)this).MainMenuStrip = menuStrip1;
		((Control)this).Name = "LuaCtrlFrm";
		((Control)this).Text = "LuaCtrlFrm";
		((Form)this).Load += LuaCtrlFrm_Load;
		((Control)splitContainer1.Panel1).ResumeLayout(false);
		((Control)splitContainer1.Panel1).PerformLayout();
		((Control)splitContainer1.Panel2).ResumeLayout(false);
		((ISupportInitialize)splitContainer1).EndInit();
		((Control)splitContainer1).ResumeLayout(false);
		((Control)splitContainer2.Panel1).ResumeLayout(false);
		((Control)splitContainer2.Panel2).ResumeLayout(false);
		((ISupportInitialize)splitContainer2).EndInit();
		((Control)splitContainer2).ResumeLayout(false);
		((Control)splitContainer4.Panel1).ResumeLayout(false);
		((Control)splitContainer4.Panel2).ResumeLayout(false);
		((Control)splitContainer4.Panel2).PerformLayout();
		((ISupportInitialize)splitContainer4).EndInit();
		((Control)splitContainer4).ResumeLayout(false);
		((Control)splitContainer3.Panel1).ResumeLayout(false);
		((Control)splitContainer3.Panel2).ResumeLayout(false);
		((Control)splitContainer3.Panel2).PerformLayout();
		((ISupportInitialize)splitContainer3).EndInit();
		((Control)splitContainer3).ResumeLayout(false);
		((Control)splitContainer5.Panel1).ResumeLayout(false);
		((Control)splitContainer5.Panel2).ResumeLayout(false);
		((ISupportInitialize)splitContainer5).EndInit();
		((Control)splitContainer5).ResumeLayout(false);
		((Control)tabLuaCmndEdit).ResumeLayout(false);
		((Control)tbAxisCmnd).ResumeLayout(false);
		((Control)tbAxisCmnd).PerformLayout();
		((Control)tbGroupCmnd).ResumeLayout(false);
		((Control)tbGroupCmnd).PerformLayout();
		((Control)tbAskCmnd).ResumeLayout(false);
		((Control)tbAskCmnd).PerformLayout();
		((Control)tbIOCmnd).ResumeLayout(false);
		((Control)tbIOCmnd).PerformLayout();
		((Control)tbKeyWord).ResumeLayout(false);
		((Control)menuStrip1).ResumeLayout(false);
		((Control)menuStrip1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
