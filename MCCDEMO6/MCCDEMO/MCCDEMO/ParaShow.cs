using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MCCDEMO;

public class ParaShow : Form
{
	private enum eShowParaType
	{
		eShowParaNcPara,
		eShowParaComTable
	}

	private eShowParaType eShowPara;

	private IContainer components;

	private ListBox listBox1;

	private SaveFileDialog saveFileDialog1;

	public ParaShow()
	{
		InitializeComponent();
	}

	public ParaShow(short nParaType)
	{
		eShowPara = (eShowParaType)nParaType;
		InitializeComponent();
	}

	private void ParaShow_Load(object sender, EventArgs e)
	{
		ushort num = 0;
		MainFrm mainFrm = (MainFrm)(object)((Form)this).Owner;
		if (eShowPara == eShowParaType.eShowParaNcPara)
		{
			for (num = 0; num < mainFrm.Mccard.ParaSum; num++)
			{
				string text = $"{num:d2}:{mainFrm.SysPara[num]:f3}";
				listBox1.Items.Add((object)text);
				if ((num + 1) % mainFrm.Mccard.ParaNumPerAxis == 0)
				{
					listBox1.Items.Add((object)"");
				}
			}
		}
		else
		{
			if (eShowParaType.eShowParaComTable != eShowPara)
			{
				return;
			}
			short[] array = new short[1];
			short[] array2 = new short[1];
			float[] array3 = new float[1];
			float[] array4 = new float[1];
			float[] array5 = new float[1];
			num = 0;
			while (num < mainFrm.Mccard.MaxAxisNum)
			{
				string text;
				if (mainFrm.Mccard.FunResOk == mainFrm.Mccard.MoCtrCard_GetComTableInfo((byte)num, array, array2, array3))
				{
					text = string.Format("AxisId={0:d2}, 定义段数={1:d3}，周期性={2:d3}, 距离模值={3:f3}", new object[4]
					{
						num,
						array[0],
						array2[0],
						array3[0]
					});
					listBox1.Items.Add((object)text);
					short num2 = 0;
					if (array[0] > 360)
					{
						listBox1.Items.Add((object)"参数有误，有效的段数大于360");
					}
					else
					{
						num2 = 0;
						while (num2 < array[0])
						{
							if (mainFrm.Mccard.FunResOk == mainFrm.Mccard.MoCtrCard_GetComTableData((byte)num, num2, array4, array5))
							{
								text = $"{num2:d3}:X={array4[0]:f3},Y={array5[0]:f3}";
								listBox1.Items.Add((object)text);
								num2++;
								continue;
							}
							text = $"读轴{num:d2}补偿表参数失败！";
							listBox1.Items.Add((object)text);
							break;
						}
						listBox1.Items.Add((object)"");
					}
					num++;
					continue;
				}
				text = $"读轴{num:d2}补偿表信息失败！";
				listBox1.Items.Add((object)text);
				break;
			}
		}
	}

	private void listBox1_DoubleClick(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)saveFileDialog1).Filter = "补偿表(*.tbl)|*.tbl";
		((FileDialog)saveFileDialog1).FileName = "comTablePara.tbl";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 == (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			StreamWriter streamWriter = new StreamWriter(((FileDialog)saveFileDialog1).FileName, append: false, Encoding.BigEndianUnicode);
			for (num = 0; num < listBox1.Items.Count; num++)
			{
				((ListControl)listBox1).SelectedIndex = num;
				streamWriter.WriteLine(listBox1.SelectedItem.ToString());
			}
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		listBox1 = new ListBox();
		saveFileDialog1 = new SaveFileDialog();
		((Control)this).SuspendLayout();
		((Control)listBox1).Anchor = (AnchorStyles)15;
		listBox1.ColumnWidth = 100;
		((ListControl)listBox1).FormattingEnabled = true;
		listBox1.ItemHeight = 12;
		((Control)listBox1).Location = new Point(3, 3);
		((Control)listBox1).Name = "listBox1";
		((Control)listBox1).Size = new Size(406, 664);
		((Control)listBox1).TabIndex = 0;
		((Control)listBox1).DoubleClick += listBox1_DoubleClick;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(421, 682);
		((Control)this).Controls.Add((Control)(object)listBox1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "ParaShow";
		((Control)this).Text = "ParaShow";
		((Form)this).Load += ParaShow_Load;
		((Control)this).ResumeLayout(false);
	}
}
