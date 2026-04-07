using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MCCDEMO;

public class frmPlatfromLevPara : Form
{
	private const int Max_Element_Cnt = 16;

	private const int Max_ElementCnt_InMCU = 32;

	private int[] gbPntCnt = new int[6];

	private float[] gbPosModeVal = new float[6];

	private ListBox[] gbLbTable = (ListBox[])(object)new ListBox[6];

	private int[] gbTableType = new int[6];

	private int gbTableIndex;

	private TextBox[,] ZtextBoxes = new TextBox[32, 32];

	private TextBox[] XtextBoxes = (TextBox[])(object)new TextBox[32];

	private TextBox[] YtextBoxes = (TextBox[])(object)new TextBox[32];

	private MainFrm pMainFrm;

	private IContainer components;

	private Button bnEnter;

	private TextBox tbYDirCnt;

	private TextBox tbXDirCnt;

	private Label label2;

	private Label label7;

	private CheckBox chbSaveToFile;

	private SaveFileDialog saveFileDialog1;

	public frmPlatfromLevPara(object mOwner)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		InitializeComponent();
		Size size = new Size(51, 23);
		for (short num = 0; num < 32; num++)
		{
			XtextBoxes[num] = new TextBox();
			YtextBoxes[num] = new TextBox();
			for (short num2 = 0; num2 < 32; num2++)
			{
				ZtextBoxes[num, num2] = new TextBox();
			}
		}
		for (short num3 = 0; num3 < 16; num3++)
		{
			Point location = new Point(76 + num3 * (size.Width + 1), 470);
			((Control)XtextBoxes[num3]).Size = size;
			((Control)XtextBoxes[num3]).Location = location;
			((Control)this).Controls.Add((Control)(object)XtextBoxes[num3]);
			((Control)XtextBoxes[num3]).Enabled = false;
			((Control)XtextBoxes[num3]).BackColor = Color.LightGray;
			XtextBoxes[num3].TextAlign = (HorizontalAlignment)2;
			((Control)XtextBoxes[num3]).Name = "tbXPos" + num3;
			((Control)XtextBoxes[num3]).Text = "NULL";
			Point location2 = new Point(6, 432 - num3 * (size.Height + 1));
			((Control)YtextBoxes[num3]).Size = size;
			((Control)YtextBoxes[num3]).Location = location2;
			((Control)this).Controls.Add((Control)(object)YtextBoxes[num3]);
			((Control)YtextBoxes[num3]).Enabled = false;
			((Control)YtextBoxes[num3]).BackColor = Color.LightGray;
			YtextBoxes[num3].TextAlign = (HorizontalAlignment)2;
			((Control)YtextBoxes[num3]).Name = "tbYPos" + num3;
			((Control)YtextBoxes[num3]).Text = "NULL";
			for (short num4 = 0; num4 < 16; num4++)
			{
				Point location3 = new Point(76 + num3 * (size.Width + 1), 432 - num4 * (size.Height + 1));
				((Control)ZtextBoxes[num3, num4]).Size = size;
				((Control)ZtextBoxes[num3, num4]).Location = location3;
				((Control)this).Controls.Add((Control)(object)ZtextBoxes[num3, num4]);
				((Control)ZtextBoxes[num3, num4]).Enabled = false;
				ZtextBoxes[num3, num4].TextAlign = (HorizontalAlignment)2;
				((Control)ZtextBoxes[num3, num4]).Name = "tbXPos" + num3 + "tbYPos" + num4;
				((Control)ZtextBoxes[num3, num4]).Text = "NULL";
			}
		}
		((Control)this).Height = 550;
		((Control)this).Width = 950;
		pMainFrm = (MainFrm)mOwner;
	}

	private void bnEnter_Click(object sender, EventArgs e)
	{
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Invalid comparison between I4 and Unknown
		int[] array = new int[1];
		int[] array2 = new int[1];
		if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MoCtrCard_GetPlatformLevTableInfo(array, array2))
		{
			((Control)tbXDirCnt).Text = array[0].ToString();
			((Control)tbYDirCnt).Text = array2[0].ToString();
			float[] array3 = new float[1];
			float[] array4 = new float[1];
			float[] array5 = new float[1];
			float[] array6 = new float[1];
			float[] array7 = new float[1];
			float[] array8 = new float[array[0]];
			float[] array9 = new float[array2[0]];
			float[,] array10 = new float[array[0], array2[0]];
			float[,] array11 = new float[array[0], array2[0]];
			float[,] array12 = new float[array[0], array2[0]];
			for (int i = 0; i < array[0]; i++)
			{
				for (int j = 0; j < array2[0]; j++)
				{
					if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MoCtrCard_GetPlatformLevTableData(i, j, array3, array4, array5, array6, array7))
					{
						((Control)XtextBoxes[i]).Text = array3[0].ToString();
						((Control)YtextBoxes[j]).Text = array4[0].ToString();
						((Control)ZtextBoxes[i, j]).Text = array5[0].ToString();
						array8[i] = array3[0];
						array9[j] = array4[0];
						array10[i, j] = array5[0];
						array11[i, j] = array6[0];
						array12[i, j] = array7[0];
						continue;
					}
					pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, $"读取平面补偿表[{i},{j}]点失败，退出读取！");
					return;
				}
			}
			if (!chbSaveToFile.Checked)
			{
				return;
			}
			((FileDialog)saveFileDialog1).Filter = "补偿表文件(*.txt)|*.txt";
			((FileDialog)saveFileDialog1).FileName = "comMergeTableReadBack.txt";
			((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
			if (1 != (int)((CommonDialog)saveFileDialog1).ShowDialog())
			{
				return;
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(((FileDialog)saveFileDialog1).FileName, append: false, Encoding.ASCII);
				streamWriter.WriteLine(array[0]);
				streamWriter.WriteLine(array2[0]);
				int num = array[0];
				int num2 = array2[0];
				for (int k = 0; k < num; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						streamWriter.WriteLine(string.Format("[x={0},y={1},z={2},A={3},B={4}]", new object[5]
						{
							array9[k],
							array9[l],
							array10[k, l],
							array11[k, l],
							array12[k, l]
						}));
					}
				}
				streamWriter.Close();
				return;
			}
			catch (Exception ex)
			{
				pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "将读回的参数写成文件失败！ex = " + ex.ToString());
				return;
			}
		}
		pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, "读取平面补偿表信息失败！");
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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected O, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		bnEnter = new Button();
		tbYDirCnt = new TextBox();
		tbXDirCnt = new TextBox();
		label2 = new Label();
		label7 = new Label();
		chbSaveToFile = new CheckBox();
		saveFileDialog1 = new SaveFileDialog();
		((Control)this).SuspendLayout();
		((Control)bnEnter).Font = new Font("楷体", 15f);
		((Control)bnEnter).Location = new Point(413, 7);
		((Control)bnEnter).Name = "bnEnter";
		((Control)bnEnter).Size = new Size(98, 36);
		((Control)bnEnter).TabIndex = 24;
		((Control)bnEnter).Text = "读取";
		((ButtonBase)bnEnter).UseVisualStyleBackColor = true;
		((Control)bnEnter).Click += bnEnter_Click;
		((Control)tbYDirCnt).Enabled = false;
		((Control)tbYDirCnt).Font = new Font("楷体", 15f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)tbYDirCnt).Location = new Point(293, 12);
		((Control)tbYDirCnt).Name = "tbYDirCnt";
		((Control)tbYDirCnt).Size = new Size(100, 30);
		((Control)tbYDirCnt).TabIndex = 23;
		((Control)tbYDirCnt).Text = "0";
		tbYDirCnt.TextAlign = (HorizontalAlignment)2;
		((Control)tbXDirCnt).Enabled = false;
		((Control)tbXDirCnt).Font = new Font("楷体", 15f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)tbXDirCnt).Location = new Point(105, 12);
		((Control)tbXDirCnt).Name = "tbXDirCnt";
		((Control)tbXDirCnt).Size = new Size(88, 30);
		((Control)tbXDirCnt).TabIndex = 22;
		((Control)tbXDirCnt).Text = "0";
		tbXDirCnt.TextAlign = (HorizontalAlignment)2;
		((Control)label2).AutoSize = true;
		((Control)label2).Font = new Font("楷体", 15f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label2).Location = new Point(20, 15);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(79, 20);
		((Control)label2).TabIndex = 20;
		((Control)label2).Text = "X点数：";
		label2.TextAlign = (ContentAlignment)16;
		((Control)label7).AutoSize = true;
		((Control)label7).Font = new Font("楷体", 15f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label7).Location = new Point(208, 15);
		((Control)label7).Name = "label7";
		((Control)label7).Size = new Size(79, 20);
		((Control)label7).TabIndex = 21;
		((Control)label7).Text = "Y点数：";
		label7.TextAlign = (ContentAlignment)64;
		((Control)chbSaveToFile).AutoSize = true;
		((Control)chbSaveToFile).Location = new Point(565, 19);
		((Control)chbSaveToFile).Name = "chbSaveToFile";
		((Control)chbSaveToFile).Size = new Size(72, 16);
		((Control)chbSaveToFile).TabIndex = 25;
		((Control)chbSaveToFile).Text = "保存文件";
		((ButtonBase)chbSaveToFile).UseVisualStyleBackColor = true;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(1188, 765);
		((Control)this).Controls.Add((Control)(object)chbSaveToFile);
		((Control)this).Controls.Add((Control)(object)bnEnter);
		((Control)this).Controls.Add((Control)(object)tbYDirCnt);
		((Control)this).Controls.Add((Control)(object)tbXDirCnt);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)label7);
		((Control)this).Name = "frmPlatfromLevPara";
		((Control)this).Text = "平面补偿表信息";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
