using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MCCDEMO;

public class ProgInputFrm : Form
{
	private IContainer components;

	private ComboBox cb_MotionType;

	private Label lb_MotionType;

	private ComboBox cb_ProType;

	private Label lb_ProType;

	private GroupBox grpNotIntrpltType;

	private GroupBox grpAxis1;

	private Label label4;

	private Label label3;

	private Label label2;

	private Label label1;

	private ComboBox cb_GrpAxis1_AxisIndex;

	private TextBox Txt_GrpAxis1_AxisAcc;

	private TextBox Txt_GrpAxis1_AxisSpd;

	private TextBox Txt_GrpAxis1_AxisPos;

	private GroupBox grpAxis4;

	private TextBox Txt_GrpAxis4_AxisAcc;

	private TextBox Txt_GrpAxis4_AxisSpd;

	private TextBox Txt_GrpAxis4_AxisPos;

	private Label label13;

	private Label label14;

	private Label label15;

	private Label label16;

	private ComboBox cb_GrpAxis4_AxisIndex;

	private GroupBox grpAxis3;

	private TextBox Txt_GrpAxis3_AxisAcc;

	private TextBox Txt_GrpAxis3_AxisSpd;

	private TextBox Txt_GrpAxis3_AxisPos;

	private Label label9;

	private Label label10;

	private Label label11;

	private Label label12;

	private ComboBox cb_GrpAxis3_AxisIndex;

	private GroupBox grpAxis2;

	private TextBox Txt_GrpAxis2_AxisAcc;

	private TextBox Txt_GrpAxis2_AxisSpd;

	private TextBox Txt_GrpAxis2_AxisPos;

	private Label label5;

	private Label label6;

	private Label label7;

	private Label label8;

	private ComboBox cb_GrpAxis2_AxisIndex;

	private GroupBox grpIntrpltType;

	private TextBox Txt_grpIntrAxis_Acc;

	private GroupBox grpIntrAxis4;

	private TextBox Txt_grpIntrAxis4_Dist;

	private Label label19;

	private Label label20;

	private ComboBox cb_grpIntrAxis4_Index;

	private TextBox Txt_grpIntrAxis_Spd;

	private GroupBox grpIntrAxis3;

	private TextBox Txt_grpIntrAxis3_Dist;

	private Label label23;

	private Label label24;

	private ComboBox cb_grpIntrAxis3_Index;

	private GroupBox grpIntrAxis2;

	private TextBox Txt_grpIntrAxis2_Dist;

	private Label label27;

	private Label label28;

	private ComboBox cb_grpIntrAxis2_Index;

	private Label label17;

	private Label label18;

	private GroupBox grpIntrAxis1;

	private TextBox Txt_grpIntrAxis1_Dist;

	private Label label31;

	private Label label32;

	private ComboBox cb_grpIntrAxis1_Index;

	private GroupBox grpOutputType;

	private Label label21;

	private Label label36;

	private Label label33;

	private ComboBox cb_Output_Value;

	private ComboBox cb_Output_Index;

	private GroupBox grpHomeType;

	private Label label39;

	private ComboBox cb_grphomeAxis1_Index;

	private Label label26;

	private Label label25;

	private Label label22;

	private ComboBox cb_grphomeAxis4_Index;

	private ComboBox cb_grphomeAxis3_Index;

	private ComboBox cb_grphomeAxis2_Index;

	private GroupBox grpCircleType;

	private TextBox Txt_CircleAxis_R;

	private TextBox Txt_CircleAxis_Spd;

	private TextBox Txt_CircleAxis_Y;

	private TextBox Txt_CircleAxis_X;

	private Label label46;

	private Label label29;

	private Label label47;

	private Label label48;

	private Label lb_CircleTimes;

	private Label lb_DelayTime;

	private TextBox Txt_CircleTimes;

	private TextBox Txt_DelayTime;

	private Button bnConfirm;

	private Button bnQuit;

	private ushort mEditType;

	private string mGCodeString;

	private MainFrm PMainFrm;

	private GroupBox grpAxis6;

	private TextBox Txt_GrpAxis6_AxisAcc;

	private TextBox Txt_GrpAxis6_AxisSpd;

	private TextBox Txt_GrpAxis6_AxisPos;

	private Label label38;

	private Label label40;

	private Label label41;

	private Label label42;

	private ComboBox cb_GrpAxis6_AxisIndex;

	private GroupBox grpAxis5;

	private TextBox Txt_GrpAxis5_AxisAcc;

	private TextBox Txt_GrpAxis5_AxisSpd;

	private TextBox Txt_GrpAxis5_AxisPos;

	private Label label30;

	private Label label34;

	private Label label35;

	private Label label37;

	private ComboBox cb_GrpAxis5_AxisIndex;

	private GroupBox grpIntrAxis6;

	private TextBox Txt_grpIntrAxis6_Dist;

	private Label label45;

	private Label label49;

	private ComboBox cb_grpIntrAxis6_Index;

	private GroupBox grpIntrAxis5;

	private TextBox Txt_grpIntrAxis5_Dist;

	private Label label43;

	private Label label44;

	private ComboBox cb_grpIntrAxis5_Index;

	private Label label51;

	private ComboBox cb_grphomeAxis6_Index;

	private Label label50;

	private ComboBox cb_grphomeAxis5_Index;

	private const byte CommandType_1AxisPtp = 0;

	private const byte CommandType_2AxisPtp = 1;

	private const byte CommandType_3AxisPtp = 2;

	private const byte CommandType_4AxisPtp = 3;

	private const byte CommandType_5AxisPtp = 4;

	private const byte CommandType_6AxisPtp = 5;

	private const byte CommandType_2AxisIntrplt = 6;

	private const byte CommandType_3AxisIntrplt = 7;

	private const byte CommandType_4AxisIntrplt = 8;

	private const byte CommandType_5AxisIntrplt = 9;

	private const byte CommandType_6AxisIntrplt = 10;

	private const byte CommandType_1AxisHome = 11;

	private const byte CommandType_2AxisHome = 12;

	private const byte CommandType_3AxisHome = 13;

	private const byte CommandType_4AxisHome = 14;

	private const byte CommandType_5AxisHome = 15;

	private const byte CommandType_6AxisHome = 16;

	private const byte CommandType_CCircle = 17;

	private const byte CommandType_CcCircle = 18;

	private const byte CommandType_Output = 19;

	private const byte CommandType_WaitInpus = 20;

	private GroupBox grpWaitInput;

	private ComboBox cbWaitInput4;

	private ComboBox cbWaitInput3;

	private ComboBox cbWaitInput2;

	private ComboBox cbWaitInput1;

	private Label label52;

	private Label label53;

	private Label label54;

	private Label label55;

	private TextBox tbWaitInputs;

	private CheckBox cbSyncExInput;

	public ProgInputFrm()
	{
		InitializeComponent();
		mEditType = 0;
	}

	public ProgInputFrm(ushort EditType)
	{
		InitializeComponent();
		mEditType = EditType;
	}

	public ProgInputFrm(string EditString)
	{
		InitializeComponent();
		mEditType = 2;
		mGCodeString = EditString;
	}

	private void ProgInputFrm_Load(object sender, EventArgs e)
	{
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		if (1 == mEditType || 3 == mEditType)
		{
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpIntrpltType).Enabled = false;
			((Control)grpHomeType).Enabled = false;
			((Control)grpOutputType).Enabled = false;
			((Control)grpCircleType).Enabled = false;
			((Control)Txt_DelayTime).Enabled = true;
			((Control)Txt_CircleTimes).Enabled = true;
			((Control)lb_CircleTimes).Enabled = true;
			((Control)lb_DelayTime).Enabled = true;
			((Control)bnConfirm).Enabled = true;
			((Control)bnQuit).Enabled = true;
			((Control)lb_ProType).Enabled = true;
			((Control)lb_MotionType).Enabled = true;
			((Control)cb_MotionType).Enabled = true;
			((Control)cb_ProType).Enabled = true;
			((Control)grpAxis2).Enabled = false;
			((Control)grpAxis3).Enabled = false;
			((Control)grpAxis4).Enabled = false;
			((Control)grpAxis5).Enabled = false;
			((Control)grpAxis6).Enabled = false;
			((ListControl)cb_MotionType).SelectedIndex = 0;
			((ListControl)cb_ProType).SelectedIndex = 1;
			((ListControl)cb_GrpAxis1_AxisIndex).SelectedIndex = 0;
			((ListControl)cb_GrpAxis2_AxisIndex).SelectedIndex = 1;
			((ListControl)cb_GrpAxis3_AxisIndex).SelectedIndex = 2;
			((ListControl)cb_GrpAxis4_AxisIndex).SelectedIndex = 3;
			((ListControl)cb_GrpAxis5_AxisIndex).SelectedIndex = 4;
			((ListControl)cb_GrpAxis6_AxisIndex).SelectedIndex = 5;
			((ListControl)cb_grpIntrAxis1_Index).SelectedIndex = 0;
			((ListControl)cb_grpIntrAxis2_Index).SelectedIndex = 1;
			((ListControl)cb_grpIntrAxis3_Index).SelectedIndex = 2;
			((ListControl)cb_grpIntrAxis4_Index).SelectedIndex = 3;
			((ListControl)cb_grpIntrAxis5_Index).SelectedIndex = 4;
			((ListControl)cb_grpIntrAxis6_Index).SelectedIndex = 5;
			((ListControl)cb_grphomeAxis1_Index).SelectedIndex = 0;
			((ListControl)cb_grphomeAxis2_Index).SelectedIndex = 1;
			((ListControl)cb_grphomeAxis3_Index).SelectedIndex = 2;
			((ListControl)cb_grphomeAxis4_Index).SelectedIndex = 3;
			((ListControl)cb_grphomeAxis5_Index).SelectedIndex = 4;
			((ListControl)cb_grphomeAxis6_Index).SelectedIndex = 5;
			((ListControl)cb_Output_Index).SelectedIndex = 0;
			((ListControl)cb_Output_Value).SelectedIndex = 0;
		}
		else if (2 == mEditType)
		{
			ushort[] array = new ushort[7] { 4, 4, 4, 4, 4, 4, 4 };
			float[] array2 = new float[7];
			float[] array3 = new float[7];
			float[] array4 = new float[7];
			ushort num = 0;
			float num2 = 0f;
			float num3 = 0f;
			ushort num4 = 0;
			ushort num5 = 0;
			ushort selectedIndex = 0;
			uint result = 0u;
			string text = "";
			((Control)grpNotIntrpltType).Enabled = false;
			((Control)grpIntrpltType).Enabled = false;
			((Control)grpHomeType).Enabled = false;
			((Control)grpOutputType).Enabled = false;
			((Control)grpCircleType).Enabled = false;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)Txt_CircleTimes).Enabled = false;
			((Control)lb_CircleTimes).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)bnConfirm).Enabled = false;
			((Control)bnQuit).Enabled = false;
			((Control)lb_ProType).Enabled = false;
			((Control)lb_MotionType).Enabled = false;
			((Control)cb_MotionType).Enabled = false;
			((Control)cb_ProType).Enabled = false;
			string[] array5 = new string[27]
			{
				"^G[ ]*[0-9]*", "[Q]?X[ ]*-?[0-9]+\\.?[0-9]*", "FX[ ]*-?[0-9]+\\.?[0-9]*", "ACCX[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?Y[ ]*-?[0-9]+\\.?[0-9]*", "FY[ ]*-?[0-9]+\\.?[0-9]*", "ACCY[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?Z[ ]*-?[0-9]+\\.?[0-9]*", "FZ[ ]*-?[0-9]+\\.?[0-9]*", "ACCZ[ ]*-?[0-9]+\\.?[0-9]*",
				"[Q]?A[ ]*-?[0-9]+\\.?[0-9]*", "FA[ ]*-?[0-9]+\\.?[0-9]*", "ACCA[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?B[ ]*-?[0-9]+\\.?[0-9]*", "FB[ ]*-?[0-9]+\\.?[0-9]*", "ACCB[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?C[ ]*-?[0-9]+\\.?[0-9]*", "FC[ ]*-?[0-9]+\\.?[0-9]*", "ACCC[ ]*-?[0-9]+\\.?[0-9]*", "[Q]?F[ ]*-?[0-9]+\\.?[0-9]*",
				"[Q]?ACC[ ]*-?[0-9]+\\.?[0-9]*", "R[ ]*-?[0-9]+\\.?[0-9]*", "D[ ]*-?[0-9]+\\.?[0-9]*", "CLEAR[ ]*[0-9]+", "SET[ ]*[0-9]+", "HOME[ ]*([X|Y|Z|T][0|1|2|3])+", "WAIT[ ]*-?[0-9]+\\.?[0-9]*"
			};
			string pattern = "[^a-z|A-Z][0-9|\\.]*";
			for (num5 = 0; num5 < array5.Length; num5++)
			{
				MatchCollection matchCollection = Regex.Matches(mGCodeString, array5[num5]);
				if (matchCollection.Count > 0)
				{
					text = Regex.Matches(matchCollection[0].Value.ToString(), pattern)[0].Value.ToString();
					if (num5 == 0)
					{
						num = ushort.Parse(text.Trim());
					}
					else if (1 == num5)
					{
						num4++;
						array[num4] = 0;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (2 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (3 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (4 == num5)
					{
						num4++;
						array[num4] = 1;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (5 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (6 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (7 == num5)
					{
						num4++;
						array[num4] = 2;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (8 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (9 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (10 == num5)
					{
						num4++;
						array[num4] = 3;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (11 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (12 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (13 == num5)
					{
						num4++;
						array[num4] = 4;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (14 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (15 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (16 == num5)
					{
						num4++;
						array[num4] = 5;
						array2[num4] = float.Parse(text.Trim());
					}
					else if (17 == num5)
					{
						array3[num4] = float.Parse(text.Trim());
					}
					else if (18 == num5)
					{
						array4[num4] = float.Parse(text.Trim());
					}
					else if (19 == num5)
					{
						array3[1] = float.Parse(text.Trim());
					}
					else if (20 == num5)
					{
						array4[1] = float.Parse(text.Trim());
					}
					else if (21 == num5)
					{
						num2 = float.Parse(text.Trim());
					}
					else if (22 == num5)
					{
						num3 = float.Parse(text.Trim());
					}
					else if (23 == num5)
					{
						num = 2048;
						selectedIndex = ushort.Parse(text.Trim());
					}
					else if (24 == num5)
					{
						num = 2049;
						selectedIndex = ushort.Parse(text.Trim());
					}
					else if (25 == num5)
					{
						num = 2050;
					}
					else if (26 == num5)
					{
						num = 7;
						uint.TryParse(text.Trim(), out result);
					}
				}
			}
			if (num == 0 || 80 == num)
			{
				((Control)cb_ProType).Enabled = true;
				((Control)cb_MotionType).Enabled = true;
				((Control)lb_ProType).Enabled = true;
				((Control)lb_MotionType).Enabled = true;
				((Control)lb_CircleTimes).Enabled = false;
				((Control)Txt_CircleTimes).Enabled = false;
				((ListControl)cb_MotionType).SelectedIndex = 2;
				((ListControl)cb_MotionType).SelectedIndex = num4 - 1;
				if (num == 0)
				{
					((ListControl)cb_ProType).SelectedIndex = 0;
				}
				else
				{
					((ListControl)cb_ProType).SelectedIndex = 1;
				}
				if (num4 > 0)
				{
					((ListControl)cb_GrpAxis1_AxisIndex).SelectedIndex = array[1];
					((Control)Txt_GrpAxis1_AxisPos).Text = array2[1].ToString();
					((Control)Txt_GrpAxis1_AxisSpd).Text = array3[1].ToString();
					((Control)Txt_GrpAxis1_AxisAcc).Text = array4[1].ToString();
				}
				if (num4 > 1)
				{
					((ListControl)cb_GrpAxis2_AxisIndex).SelectedIndex = array[2];
					((Control)Txt_GrpAxis2_AxisPos).Text = array2[2].ToString();
					((Control)Txt_GrpAxis2_AxisSpd).Text = array3[2].ToString();
					((Control)Txt_GrpAxis2_AxisAcc).Text = array4[2].ToString();
				}
				if (num4 > 2)
				{
					((ListControl)cb_GrpAxis3_AxisIndex).SelectedIndex = array[3];
					((Control)Txt_GrpAxis3_AxisPos).Text = array2[3].ToString();
					((Control)Txt_GrpAxis3_AxisSpd).Text = array3[3].ToString();
					((Control)Txt_GrpAxis3_AxisAcc).Text = array4[3].ToString();
				}
				if (num4 > 3)
				{
					((ListControl)cb_GrpAxis4_AxisIndex).SelectedIndex = array[4];
					((Control)Txt_GrpAxis4_AxisPos).Text = array2[4].ToString();
					((Control)Txt_GrpAxis4_AxisSpd).Text = array3[4].ToString();
					((Control)Txt_GrpAxis4_AxisAcc).Text = array4[4].ToString();
				}
				if (num4 > 4)
				{
					((ListControl)cb_GrpAxis5_AxisIndex).SelectedIndex = array[5];
					((Control)Txt_GrpAxis5_AxisPos).Text = array2[5].ToString();
					((Control)Txt_GrpAxis5_AxisSpd).Text = array3[5].ToString();
					((Control)Txt_GrpAxis5_AxisAcc).Text = array4[5].ToString();
				}
				if (num4 > 5)
				{
					((ListControl)cb_GrpAxis6_AxisIndex).SelectedIndex = array[6];
					((Control)Txt_GrpAxis6_AxisPos).Text = array2[6].ToString();
					((Control)Txt_GrpAxis6_AxisSpd).Text = array3[6].ToString();
					((Control)Txt_GrpAxis6_AxisAcc).Text = array4[6].ToString();
				}
				((Control)Txt_DelayTime).Text = num3.ToString();
			}
			else if (1 == num || 81 == num)
			{
				((Control)cb_ProType).Enabled = true;
				((Control)cb_MotionType).Enabled = true;
				((Control)lb_ProType).Enabled = true;
				((Control)lb_MotionType).Enabled = true;
				((Control)lb_CircleTimes).Enabled = false;
				((Control)Txt_CircleTimes).Enabled = false;
				((ListControl)cb_MotionType).SelectedIndex = 4 + num4 - 2;
				if (1 == num)
				{
					((ListControl)cb_ProType).SelectedIndex = 0;
				}
				else
				{
					((ListControl)cb_ProType).SelectedIndex = 1;
				}
				if (num4 > 0)
				{
					((ListControl)cb_grpIntrAxis1_Index).SelectedIndex = array[1];
					((Control)Txt_grpIntrAxis1_Dist).Text = array2[1].ToString();
				}
				if (num4 > 1)
				{
					((ListControl)cb_grpIntrAxis2_Index).SelectedIndex = array[2];
					((Control)Txt_grpIntrAxis2_Dist).Text = array2[2].ToString();
				}
				if (num4 > 2)
				{
					((ListControl)cb_grpIntrAxis3_Index).SelectedIndex = array[3];
					((Control)Txt_grpIntrAxis3_Dist).Text = array2[3].ToString();
				}
				if (num4 > 3)
				{
					((ListControl)cb_grpIntrAxis4_Index).SelectedIndex = array[4];
					((Control)Txt_grpIntrAxis4_Dist).Text = array2[4].ToString();
				}
				if (num4 > 4)
				{
					((ListControl)cb_grpIntrAxis5_Index).SelectedIndex = array[5];
					((Control)Txt_grpIntrAxis5_Dist).Text = array2[5].ToString();
				}
				if (num4 > 5)
				{
					((ListControl)cb_grpIntrAxis6_Index).SelectedIndex = array[6];
					((Control)Txt_grpIntrAxis6_Dist).Text = array2[6].ToString();
				}
				((Control)Txt_grpIntrAxis_Spd).Text = array3[1].ToString();
				((Control)Txt_grpIntrAxis_Acc).Text = array4[1].ToString();
				((Control)Txt_DelayTime).Text = num3.ToString();
			}
			else if (2 == num || 82 == num || 3 == num || 83 == num)
			{
				((Control)cb_ProType).Enabled = true;
				((Control)cb_MotionType).Enabled = true;
				((Control)lb_ProType).Enabled = true;
				((Control)lb_MotionType).Enabled = true;
				((Control)lb_CircleTimes).Enabled = false;
				((Control)Txt_CircleTimes).Enabled = false;
				if (2 == num || 82 == num)
				{
					((ListControl)cb_MotionType).SelectedIndex = 7;
				}
				else
				{
					((ListControl)cb_MotionType).SelectedIndex = 8;
				}
				if (num > 3)
				{
					((ListControl)cb_ProType).SelectedIndex = 1;
				}
				else
				{
					((ListControl)cb_ProType).SelectedIndex = 0;
				}
				((Control)Txt_CircleAxis_X).Text = array2[1].ToString();
				((Control)Txt_CircleAxis_Y).Text = array2[2].ToString();
				((Control)Txt_CircleAxis_R).Text = num2.ToString();
				((Control)Txt_CircleAxis_Spd).Text = array3[1].ToString();
				((Control)Txt_DelayTime).Text = num3.ToString();
			}
			else if (2048 == num || 2049 == num)
			{
				((ListControl)cb_MotionType).SelectedIndex = 9;
				((ListControl)cb_Output_Index).SelectedIndex = selectedIndex;
				if (2048 == num)
				{
					((ListControl)cb_Output_Value).SelectedIndex = 0;
				}
				else
				{
					((ListControl)cb_Output_Value).SelectedIndex = 1;
				}
			}
			else if (2050 == num)
			{
				((ListControl)cb_MotionType).SelectedIndex = 10;
				if (num4 > 0)
				{
					((ListControl)cb_grphomeAxis1_Index).SelectedIndex = array[1];
				}
				if (num4 > 1)
				{
					((ListControl)cb_grphomeAxis2_Index).SelectedIndex = array[2];
				}
				if (num4 > 2)
				{
					((ListControl)cb_grphomeAxis3_Index).SelectedIndex = array[3];
				}
				if (num4 > 3)
				{
					((ListControl)cb_grphomeAxis4_Index).SelectedIndex = array[4];
				}
				if (num4 > 4)
				{
					((ListControl)cb_grphomeAxis5_Index).SelectedIndex = array[5];
				}
				if (num4 > 5)
				{
					((ListControl)cb_grphomeAxis6_Index).SelectedIndex = array[6];
				}
				((Control)Txt_DelayTime).Text = num3.ToString();
			}
			else
			{
				if (7 != num)
				{
					return;
				}
				int num6 = 0;
				((ListControl)cb_MotionType).SelectedIndex = 20;
				((Control)grpWaitInput).Enabled = true;
				for (num5 = 0; num5 < 32; num5++)
				{
					if (1 << (int)num5 == ((1 << (int)num5) & result))
					{
						if (num6 == 0)
						{
							((Control)cbWaitInput1).Text = (num5 + 1).ToString();
						}
						else if (1 == num6)
						{
							((Control)cbWaitInput2).Text = (num5 + 1).ToString();
						}
						else if (2 == num6)
						{
							((Control)cbWaitInput3).Text = (num5 + 1).ToString();
						}
						else if (3 == num6)
						{
							((Control)cbWaitInput4).Text = (num5 + 1).ToString();
							break;
						}
						num6++;
					}
				}
			}
		}
		else
		{
			((Control)grpNotIntrpltType).Enabled = false;
			((Control)grpIntrpltType).Enabled = false;
			((Control)grpHomeType).Enabled = false;
			((Control)grpOutputType).Enabled = false;
			((Control)grpCircleType).Enabled = false;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)Txt_CircleTimes).Enabled = false;
			((Control)lb_CircleTimes).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)bnConfirm).Enabled = false;
			((Control)bnQuit).Enabled = false;
			((Control)lb_ProType).Enabled = false;
			((Control)lb_MotionType).Enabled = false;
			((Control)cb_MotionType).Enabled = false;
			((Control)cb_ProType).Enabled = false;
			((Control)grpWaitInput).Enabled = false;
		}
	}

	private void cb_MotionType_SelectedIndexChanged(object sender, EventArgs e)
	{
		((Control)grpNotIntrpltType).Enabled = false;
		((Control)grpIntrpltType).Enabled = false;
		((Control)grpHomeType).Enabled = false;
		((Control)grpOutputType).Enabled = false;
		((Control)grpCircleType).Enabled = false;
		((Control)Txt_DelayTime).Enabled = true;
		((Control)lb_DelayTime).Enabled = true;
		((Control)Txt_CircleTimes).Enabled = false;
		((Control)lb_CircleTimes).Enabled = false;
		((Control)bnConfirm).Enabled = true;
		((Control)bnQuit).Enabled = true;
		((Control)lb_ProType).Enabled = true;
		((Control)lb_MotionType).Enabled = true;
		((Control)cb_MotionType).Enabled = true;
		((Control)cb_ProType).Enabled = true;
		((ListControl)cb_ProType).SelectedIndex = 0;
		switch (((ListControl)cb_MotionType).SelectedIndex)
		{
		case 0:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = false;
			((Control)grpAxis3).Enabled = false;
			((Control)grpAxis4).Enabled = false;
			((Control)grpAxis5).Enabled = false;
			((Control)grpAxis6).Enabled = false;
			break;
		case 1:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = true;
			((Control)grpAxis3).Enabled = false;
			((Control)grpAxis4).Enabled = false;
			((Control)grpAxis5).Enabled = false;
			((Control)grpAxis6).Enabled = false;
			break;
		case 2:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = true;
			((Control)grpAxis3).Enabled = true;
			((Control)grpAxis4).Enabled = false;
			((Control)grpAxis5).Enabled = false;
			((Control)grpAxis6).Enabled = false;
			break;
		case 3:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = true;
			((Control)grpAxis3).Enabled = true;
			((Control)grpAxis4).Enabled = true;
			((Control)grpAxis5).Enabled = false;
			((Control)grpAxis6).Enabled = false;
			break;
		case 4:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = true;
			((Control)grpAxis3).Enabled = true;
			((Control)grpAxis4).Enabled = true;
			((Control)grpAxis5).Enabled = true;
			((Control)grpAxis6).Enabled = false;
			break;
		case 5:
			((Control)grpNotIntrpltType).Enabled = true;
			((Control)grpAxis1).Enabled = true;
			((Control)grpAxis2).Enabled = true;
			((Control)grpAxis3).Enabled = true;
			((Control)grpAxis4).Enabled = true;
			((Control)grpAxis5).Enabled = true;
			((Control)grpAxis6).Enabled = true;
			break;
		case 6:
			((Control)grpIntrpltType).Enabled = true;
			((Control)grpIntrAxis1).Enabled = true;
			((Control)grpIntrAxis2).Enabled = true;
			((Control)grpIntrAxis3).Enabled = false;
			((Control)grpIntrAxis4).Enabled = false;
			((Control)grpIntrAxis5).Enabled = false;
			((Control)grpIntrAxis6).Enabled = false;
			break;
		case 7:
			((Control)grpIntrpltType).Enabled = true;
			((Control)grpIntrAxis1).Enabled = true;
			((Control)grpIntrAxis2).Enabled = true;
			((Control)grpIntrAxis3).Enabled = true;
			((Control)grpIntrAxis4).Enabled = false;
			((Control)grpIntrAxis5).Enabled = false;
			((Control)grpIntrAxis6).Enabled = false;
			break;
		case 8:
			((Control)grpIntrpltType).Enabled = true;
			((Control)grpIntrAxis1).Enabled = true;
			((Control)grpIntrAxis2).Enabled = true;
			((Control)grpIntrAxis3).Enabled = true;
			((Control)grpIntrAxis4).Enabled = true;
			((Control)grpIntrAxis5).Enabled = false;
			((Control)grpIntrAxis6).Enabled = false;
			break;
		case 9:
			((Control)grpIntrpltType).Enabled = true;
			((Control)grpIntrAxis1).Enabled = true;
			((Control)grpIntrAxis2).Enabled = true;
			((Control)grpIntrAxis3).Enabled = true;
			((Control)grpIntrAxis4).Enabled = true;
			((Control)grpIntrAxis5).Enabled = true;
			((Control)grpIntrAxis6).Enabled = false;
			break;
		case 10:
			((Control)grpIntrpltType).Enabled = true;
			((Control)grpIntrAxis1).Enabled = true;
			((Control)grpIntrAxis2).Enabled = true;
			((Control)grpIntrAxis3).Enabled = true;
			((Control)grpIntrAxis4).Enabled = true;
			((Control)grpIntrAxis5).Enabled = true;
			((Control)grpIntrAxis6).Enabled = true;
			break;
		case 17:
		case 18:
			((Control)grpCircleType).Enabled = true;
			break;
		case 11:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = false;
			((Control)cb_grphomeAxis3_Index).Enabled = false;
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((Control)cb_grphomeAxis5_Index).Enabled = false;
			((Control)cb_grphomeAxis6_Index).Enabled = false;
			break;
		case 12:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = true;
			((Control)cb_grphomeAxis3_Index).Enabled = false;
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((Control)cb_grphomeAxis5_Index).Enabled = false;
			((Control)cb_grphomeAxis6_Index).Enabled = false;
			break;
		case 13:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = true;
			((Control)cb_grphomeAxis3_Index).Enabled = true;
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((Control)cb_grphomeAxis5_Index).Enabled = false;
			((Control)cb_grphomeAxis6_Index).Enabled = false;
			break;
		case 14:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = true;
			((Control)cb_grphomeAxis3_Index).Enabled = true;
			((Control)cb_grphomeAxis4_Index).Enabled = true;
			((Control)cb_grphomeAxis5_Index).Enabled = false;
			((Control)cb_grphomeAxis6_Index).Enabled = false;
			break;
		case 15:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = true;
			((Control)cb_grphomeAxis3_Index).Enabled = true;
			((Control)cb_grphomeAxis4_Index).Enabled = true;
			((Control)cb_grphomeAxis5_Index).Enabled = true;
			((Control)cb_grphomeAxis6_Index).Enabled = false;
			break;
		case 16:
			((Control)grpHomeType).Enabled = true;
			((Control)Txt_DelayTime).Enabled = false;
			((Control)lb_DelayTime).Enabled = false;
			((Control)cb_grphomeAxis1_Index).Enabled = true;
			((Control)cb_grphomeAxis2_Index).Enabled = true;
			((Control)cb_grphomeAxis3_Index).Enabled = true;
			((Control)cb_grphomeAxis4_Index).Enabled = true;
			((Control)cb_grphomeAxis5_Index).Enabled = true;
			((Control)cb_grphomeAxis6_Index).Enabled = true;
			break;
		case 19:
			((Control)grpOutputType).Enabled = true;
			((Control)lb_CircleTimes).Enabled = false;
			((Control)lb_ProType).Enabled = false;
			((Control)Txt_CircleTimes).Enabled = false;
			((Control)cb_ProType).Enabled = false;
			break;
		case 20:
			((Control)grpWaitInput).Enabled = true;
			((ListControl)cbWaitInput1).SelectedIndex = 0;
			((ListControl)cbWaitInput2).SelectedIndex = 0;
			((ListControl)cbWaitInput3).SelectedIndex = 0;
			((ListControl)cbWaitInput4).SelectedIndex = 0;
			break;
		}
	}

	private void cb_ProType_SelectedIndexChanged(object sender, EventArgs e)
	{
		((Control)lb_CircleTimes).Enabled = false;
		((Control)Txt_CircleTimes).Enabled = false;
		if (1 == ((ListControl)cb_ProType).SelectedIndex && 18 != ((ListControl)cb_MotionType).SelectedIndex && 17 != ((ListControl)cb_MotionType).SelectedIndex)
		{
			((Control)lb_CircleTimes).Enabled = true;
			((Control)Txt_CircleTimes).Enabled = true;
		}
	}

	private void cb_grphomeAxis1_Index_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (4 == ((ListControl)cb_grphomeAxis1_Index).SelectedIndex)
		{
			((Control)cb_grphomeAxis2_Index).Enabled = false;
			((Control)cb_grphomeAxis3_Index).Enabled = false;
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((ListControl)cb_grphomeAxis2_Index).SelectedIndex = 4;
			((ListControl)cb_grphomeAxis3_Index).SelectedIndex = 4;
			((ListControl)cb_grphomeAxis4_Index).SelectedIndex = 4;
		}
		else
		{
			((Control)cb_grphomeAxis2_Index).Enabled = true;
		}
	}

	private void cb_grphomeAxis2_Index_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (4 == ((ListControl)cb_grphomeAxis2_Index).SelectedIndex)
		{
			((Control)cb_grphomeAxis3_Index).Enabled = false;
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((ListControl)cb_grphomeAxis3_Index).SelectedIndex = 4;
			((ListControl)cb_grphomeAxis4_Index).SelectedIndex = 4;
		}
		else
		{
			((Control)cb_grphomeAxis3_Index).Enabled = true;
		}
	}

	private void cb_grphomeAxis3_Index_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (4 == ((ListControl)cb_grphomeAxis3_Index).SelectedIndex)
		{
			((Control)cb_grphomeAxis4_Index).Enabled = false;
			((ListControl)cb_grphomeAxis4_Index).SelectedIndex = 4;
		}
		else
		{
			((Control)cb_grphomeAxis4_Index).Enabled = true;
		}
	}

	private void bnConfirm_Click(object sender, EventArgs e)
	{
		string text = "";
		string text2 = "";
		if (((ListControl)cb_MotionType).SelectedIndex <= 5)
		{
			if (((ListControl)cb_ProType).SelectedIndex == 0)
			{
				text += "G00";
			}
			else
			{
				text += "G80";
				text2 += "G80";
			}
			if (((Control)grpAxis1).Enabled)
			{
				text = text + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisPos).Text + "F" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisAcc).Text;
				text2 = ((!(float.Parse(((Control)Txt_GrpAxis1_AxisPos).Text) < 0f)) ? (text2 + ((Control)cb_GrpAxis1_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis1_AxisPos).Text + "F" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisAcc).Text) : (text2 + ((Control)cb_GrpAxis1_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis1_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis1_AxisIndex).Text + ((Control)Txt_GrpAxis1_AxisAcc).Text));
			}
			if (((Control)grpAxis2).Enabled)
			{
				text = text + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisPos).Text + "F" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisAcc).Text;
				text2 = ((!(float.Parse(((Control)Txt_GrpAxis2_AxisPos).Text) < 0f)) ? (text2 + ((Control)cb_GrpAxis2_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis2_AxisPos).Text + "F" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisAcc).Text) : (text2 + ((Control)cb_GrpAxis2_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis2_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis2_AxisIndex).Text + ((Control)Txt_GrpAxis2_AxisAcc).Text));
			}
			if (((Control)grpAxis3).Enabled)
			{
				text = text + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisPos).Text + "F" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisAcc).Text;
				text2 = ((!(float.Parse(((Control)Txt_GrpAxis3_AxisPos).Text) < 0f)) ? (text2 + ((Control)cb_GrpAxis3_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis3_AxisPos).Text + "F" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisAcc).Text) : (text2 + ((Control)cb_GrpAxis3_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis3_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis3_AxisIndex).Text + ((Control)Txt_GrpAxis3_AxisAcc).Text));
			}
			if (((Control)grpAxis4).Enabled)
			{
				text = text + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisPos).Text + "F" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisAcc).Text;
				if (float.Parse(((Control)Txt_GrpAxis4_AxisPos).Text) < 0f)
				{
					text = text + ((Control)cb_GrpAxis4_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis4_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisAcc).Text;
				}
				else
				{
					text2 = text2 + ((Control)cb_GrpAxis4_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis4_AxisPos).Text + "F" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis4_AxisIndex).Text + ((Control)Txt_GrpAxis4_AxisAcc).Text;
				}
			}
			if (((Control)grpAxis5).Enabled)
			{
				text = text + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisPos).Text + "F" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisAcc).Text;
				if (float.Parse(((Control)Txt_GrpAxis5_AxisPos).Text) < 0f)
				{
					text = text + ((Control)cb_GrpAxis5_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis5_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisAcc).Text;
				}
				else
				{
					text2 = text2 + ((Control)cb_GrpAxis5_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis5_AxisPos).Text + "F" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis5_AxisIndex).Text + ((Control)Txt_GrpAxis5_AxisAcc).Text;
				}
			}
			if (((Control)grpAxis6).Enabled)
			{
				text = text + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisPos).Text + "F" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisAcc).Text;
				if (float.Parse(((Control)Txt_GrpAxis6_AxisPos).Text) < 0f)
				{
					text = text + ((Control)cb_GrpAxis6_AxisIndex).Text + Math.Abs(float.Parse(((Control)Txt_GrpAxis6_AxisPos).Text)) + "F" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisAcc).Text;
				}
				else
				{
					text2 = text2 + ((Control)cb_GrpAxis6_AxisIndex).Text + "-" + ((Control)Txt_GrpAxis6_AxisPos).Text + "F" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisSpd).Text + "ACC" + ((Control)cb_GrpAxis6_AxisIndex).Text + ((Control)Txt_GrpAxis6_AxisAcc).Text;
				}
			}
		}
		else if (((ListControl)cb_MotionType).SelectedIndex <= 10)
		{
			if (((ListControl)cb_ProType).SelectedIndex == 0)
			{
				text += "G01";
			}
			else
			{
				text += "G81";
				text2 += "G81";
			}
			if (((Control)grpIntrAxis1).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis1_Index).Text + ((Control)Txt_grpIntrAxis1_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis1_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis1_Index).Text + "-" + ((Control)Txt_grpIntrAxis1_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis1_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis1_Dist).Text))));
			}
			if (((Control)grpIntrAxis2).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis2_Index).Text + ((Control)Txt_grpIntrAxis2_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis2_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis2_Index).Text + "-" + ((Control)Txt_grpIntrAxis2_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis2_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis2_Dist).Text))));
			}
			if (((Control)grpIntrAxis3).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis3_Index).Text + ((Control)Txt_grpIntrAxis3_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis3_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis3_Index).Text + "-" + ((Control)Txt_grpIntrAxis3_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis3_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis3_Dist).Text))));
			}
			if (((Control)grpIntrAxis4).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis4_Index).Text + ((Control)Txt_grpIntrAxis4_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis4_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis4_Index).Text + "-" + ((Control)Txt_grpIntrAxis4_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis4_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis4_Dist).Text))));
			}
			if (((Control)grpIntrAxis5).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis5_Index).Text + ((Control)Txt_grpIntrAxis5_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis5_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis5_Index).Text + "-" + ((Control)Txt_grpIntrAxis5_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis5_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis5_Dist).Text))));
			}
			if (((Control)grpIntrAxis6).Enabled)
			{
				text = text + ((Control)cb_grpIntrAxis6_Index).Text + ((Control)Txt_grpIntrAxis6_Dist).Text;
				text2 = ((!(float.Parse(((Control)Txt_grpIntrAxis6_Dist).Text) < 0f)) ? (text2 + ((Control)cb_grpIntrAxis6_Index).Text + "-" + ((Control)Txt_grpIntrAxis6_Dist).Text) : (text2 + ((Control)cb_grpIntrAxis6_Index).Text + Math.Abs(float.Parse(((Control)Txt_grpIntrAxis6_Dist).Text))));
			}
			text = text + "F" + ((Control)Txt_grpIntrAxis_Spd).Text + "ACC" + ((Control)Txt_grpIntrAxis_Acc).Text;
			text2 = text2 + "F" + ((Control)Txt_grpIntrAxis_Spd).Text + "ACC" + ((Control)Txt_grpIntrAxis_Acc).Text;
		}
		else if (((ListControl)cb_MotionType).SelectedIndex <= 16)
		{
			text = "HOME";
			if (((Control)cb_grphomeAxis1_Index).Enabled)
			{
				text += cb_grpIntrAxis1_Index.Items[((ListControl)cb_grphomeAxis1_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis1_Index).SelectedIndex;
			}
			if (((Control)cb_grphomeAxis2_Index).Enabled)
			{
				text += cb_grpIntrAxis2_Index.Items[((ListControl)cb_grphomeAxis2_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis2_Index).SelectedIndex;
			}
			if (((Control)cb_grphomeAxis3_Index).Enabled)
			{
				text += cb_grpIntrAxis3_Index.Items[((ListControl)cb_grphomeAxis3_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis3_Index).SelectedIndex;
			}
			if (((Control)cb_grphomeAxis4_Index).Enabled)
			{
				text += cb_grpIntrAxis4_Index.Items[((ListControl)cb_grphomeAxis4_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis4_Index).SelectedIndex;
			}
			if (((Control)cb_grphomeAxis5_Index).Enabled)
			{
				text += cb_grpIntrAxis5_Index.Items[((ListControl)cb_grphomeAxis5_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis5_Index).SelectedIndex;
			}
			if (((Control)cb_grphomeAxis6_Index).Enabled)
			{
				text += cb_grpIntrAxis6_Index.Items[((ListControl)cb_grphomeAxis6_Index).SelectedIndex];
				text += ((ListControl)cb_grphomeAxis6_Index).SelectedIndex;
			}
		}
		else if (17 == ((ListControl)cb_MotionType).SelectedIndex || 18 == ((ListControl)cb_MotionType).SelectedIndex)
		{
			if (((ListControl)cb_ProType).SelectedIndex == 0)
			{
				text = ((17 != ((ListControl)cb_MotionType).SelectedIndex) ? (text + "G03") : (text + "G02"));
			}
			else if (17 == ((ListControl)cb_MotionType).SelectedIndex)
			{
				text += "G82";
				text2 += "G82";
			}
			else
			{
				text += "G83";
				text2 += "G83";
			}
			text = text + "X" + ((Control)Txt_CircleAxis_X).Text + "Y" + ((Control)Txt_CircleAxis_Y).Text + "R" + ((Control)Txt_CircleAxis_R).Text + "F" + ((Control)Txt_CircleAxis_Spd).Text;
			text2 = ((!(float.Parse(((Control)Txt_CircleAxis_X).Text) < 0f)) ? (text2 + "X-" + ((Control)Txt_CircleAxis_X).Text) : (text2 + "X" + Math.Abs(float.Parse(((Control)Txt_CircleAxis_X).Text))));
			text2 = ((!(float.Parse(((Control)Txt_CircleAxis_Y).Text) < 0f)) ? (text2 + "Y-" + ((Control)Txt_CircleAxis_Y).Text) : (text2 + "Y" + Math.Abs(float.Parse(((Control)Txt_CircleAxis_Y).Text))));
			text2 = text2 + "R" + ((Control)Txt_CircleAxis_R).Text + "F" + ((Control)Txt_CircleAxis_Spd).Text;
		}
		else if (19 == ((ListControl)cb_MotionType).SelectedIndex)
		{
			text = ((!("0" == ((Control)cb_Output_Value).Text)) ? (text + "SET " + ((Control)cb_Output_Index).Text) : (text + "CLEAR " + ((Control)cb_Output_Index).Text));
		}
		else if (20 == ((ListControl)cb_MotionType).SelectedIndex)
		{
			text = "WAIT " + ((Control)tbWaitInputs).Text;
		}
		if (((ListControl)cb_MotionType).SelectedIndex <= 10 || ((ListControl)cb_MotionType).SelectedIndex >= 17 || ((ListControl)cb_MotionType).SelectedIndex == 20)
		{
			text = text + "D" + ((Control)Txt_DelayTime).Text;
			text2 = text2 + "D" + ((Control)Txt_DelayTime).Text;
		}
		if (cbSyncExInput.Checked)
		{
			text += "SYNC27";
			text2 += "SYNC27";
		}
		string pattern = "\\s+";
		string replacement = "";
		text = Regex.Replace(text, pattern, replacement);
		text2 = Regex.Replace(text2, pattern, replacement);
		ushort num = 0;
		if (1 == mEditType || 3 == mEditType)
		{
			int num2 = ((ListControl)PMainFrm.ProgrameList).SelectedIndex;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (((Control)Txt_CircleTimes).Enabled && int.Parse(((Control)Txt_CircleTimes).Text) > 0)
			{
				for (num = 0; num < int.Parse(((Control)Txt_CircleTimes).Text); num++)
				{
					if (3 == mEditType)
					{
						PMainFrm.ProgrameList.Items.Insert(num2, (object)text);
						PMainFrm.ProgrameList.Items.Insert(num2 + 1, (object)text2);
					}
					else if (1 == mEditType)
					{
						PMainFrm.ProgrameList.Items.Add((object)text);
						PMainFrm.ProgrameList.Items.Add((object)text2);
					}
				}
			}
			else if (3 == mEditType)
			{
				PMainFrm.ProgrameList.Items.Insert(num2, (object)text);
			}
			else if (1 == mEditType)
			{
				PMainFrm.ProgrameList.Items.Add((object)text);
			}
		}
		else if (2 == mEditType)
		{
			int selectedIndex = ((ListControl)PMainFrm.ProgrameList).SelectedIndex;
			PMainFrm.ProgrameList.Items.RemoveAt(selectedIndex);
			if (selectedIndex >= 0)
			{
				PMainFrm.ProgrameList.Items.Insert(selectedIndex, (object)text);
				((ListControl)PMainFrm.ProgrameList).SelectedIndex = selectedIndex;
			}
		}
	}

	private void grpCircleType_Enter(object sender, EventArgs e)
	{
	}

	private void bnQuit_Click(object sender, EventArgs e)
	{
		((Form)this).Close();
	}

	private void cbWaitInput1_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_ = (ComboBox)sender;
		int num = 0;
		if (((ListControl)cbWaitInput1).SelectedIndex > 0)
		{
			num += 1 << ((ListControl)cbWaitInput1).SelectedIndex - 1;
		}
		if (((ListControl)cbWaitInput2).SelectedIndex > 0)
		{
			num += 1 << ((ListControl)cbWaitInput2).SelectedIndex - 1;
		}
		if (((ListControl)cbWaitInput3).SelectedIndex > 0)
		{
			num += 1 << ((ListControl)cbWaitInput3).SelectedIndex - 1;
		}
		if (((ListControl)cbWaitInput4).SelectedIndex > 0)
		{
			num += 1 << ((ListControl)cbWaitInput4).SelectedIndex - 1;
		}
		((Control)tbWaitInputs).Text = num.ToString("D");
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ProgInputFrm));
		cb_MotionType = new ComboBox();
		lb_MotionType = new Label();
		cb_ProType = new ComboBox();
		lb_ProType = new Label();
		grpNotIntrpltType = new GroupBox();
		grpAxis6 = new GroupBox();
		Txt_GrpAxis6_AxisAcc = new TextBox();
		Txt_GrpAxis6_AxisSpd = new TextBox();
		Txt_GrpAxis6_AxisPos = new TextBox();
		label38 = new Label();
		label40 = new Label();
		label41 = new Label();
		label42 = new Label();
		cb_GrpAxis6_AxisIndex = new ComboBox();
		grpAxis5 = new GroupBox();
		Txt_GrpAxis5_AxisAcc = new TextBox();
		Txt_GrpAxis5_AxisSpd = new TextBox();
		Txt_GrpAxis5_AxisPos = new TextBox();
		label30 = new Label();
		label34 = new Label();
		label35 = new Label();
		label37 = new Label();
		cb_GrpAxis5_AxisIndex = new ComboBox();
		grpAxis4 = new GroupBox();
		Txt_GrpAxis4_AxisAcc = new TextBox();
		Txt_GrpAxis4_AxisSpd = new TextBox();
		Txt_GrpAxis4_AxisPos = new TextBox();
		label13 = new Label();
		label14 = new Label();
		label15 = new Label();
		label16 = new Label();
		cb_GrpAxis4_AxisIndex = new ComboBox();
		grpAxis3 = new GroupBox();
		Txt_GrpAxis3_AxisAcc = new TextBox();
		Txt_GrpAxis3_AxisSpd = new TextBox();
		Txt_GrpAxis3_AxisPos = new TextBox();
		label9 = new Label();
		label10 = new Label();
		label11 = new Label();
		label12 = new Label();
		cb_GrpAxis3_AxisIndex = new ComboBox();
		grpAxis2 = new GroupBox();
		Txt_GrpAxis2_AxisAcc = new TextBox();
		Txt_GrpAxis2_AxisSpd = new TextBox();
		Txt_GrpAxis2_AxisPos = new TextBox();
		label5 = new Label();
		label6 = new Label();
		label7 = new Label();
		label8 = new Label();
		cb_GrpAxis2_AxisIndex = new ComboBox();
		grpAxis1 = new GroupBox();
		Txt_GrpAxis1_AxisAcc = new TextBox();
		Txt_GrpAxis1_AxisSpd = new TextBox();
		Txt_GrpAxis1_AxisPos = new TextBox();
		label4 = new Label();
		label3 = new Label();
		label2 = new Label();
		label1 = new Label();
		cb_GrpAxis1_AxisIndex = new ComboBox();
		grpIntrpltType = new GroupBox();
		grpIntrAxis6 = new GroupBox();
		Txt_grpIntrAxis6_Dist = new TextBox();
		label45 = new Label();
		label49 = new Label();
		cb_grpIntrAxis6_Index = new ComboBox();
		grpIntrAxis5 = new GroupBox();
		Txt_grpIntrAxis5_Dist = new TextBox();
		label43 = new Label();
		label44 = new Label();
		cb_grpIntrAxis5_Index = new ComboBox();
		Txt_grpIntrAxis_Acc = new TextBox();
		grpIntrAxis4 = new GroupBox();
		Txt_grpIntrAxis4_Dist = new TextBox();
		label19 = new Label();
		label20 = new Label();
		cb_grpIntrAxis4_Index = new ComboBox();
		Txt_grpIntrAxis_Spd = new TextBox();
		grpIntrAxis3 = new GroupBox();
		Txt_grpIntrAxis3_Dist = new TextBox();
		label23 = new Label();
		label24 = new Label();
		cb_grpIntrAxis3_Index = new ComboBox();
		grpIntrAxis2 = new GroupBox();
		Txt_grpIntrAxis2_Dist = new TextBox();
		label27 = new Label();
		label28 = new Label();
		cb_grpIntrAxis2_Index = new ComboBox();
		label17 = new Label();
		label18 = new Label();
		grpIntrAxis1 = new GroupBox();
		Txt_grpIntrAxis1_Dist = new TextBox();
		label31 = new Label();
		label32 = new Label();
		cb_grpIntrAxis1_Index = new ComboBox();
		grpOutputType = new GroupBox();
		label21 = new Label();
		label36 = new Label();
		label33 = new Label();
		cb_Output_Value = new ComboBox();
		cb_Output_Index = new ComboBox();
		grpHomeType = new GroupBox();
		label51 = new Label();
		cb_grphomeAxis6_Index = new ComboBox();
		label50 = new Label();
		cb_grphomeAxis5_Index = new ComboBox();
		label26 = new Label();
		label25 = new Label();
		label22 = new Label();
		label39 = new Label();
		cb_grphomeAxis4_Index = new ComboBox();
		cb_grphomeAxis3_Index = new ComboBox();
		cb_grphomeAxis2_Index = new ComboBox();
		cb_grphomeAxis1_Index = new ComboBox();
		grpCircleType = new GroupBox();
		Txt_CircleAxis_R = new TextBox();
		Txt_CircleAxis_Spd = new TextBox();
		Txt_CircleAxis_Y = new TextBox();
		Txt_CircleAxis_X = new TextBox();
		label46 = new Label();
		label29 = new Label();
		label47 = new Label();
		label48 = new Label();
		lb_CircleTimes = new Label();
		lb_DelayTime = new Label();
		Txt_CircleTimes = new TextBox();
		Txt_DelayTime = new TextBox();
		bnConfirm = new Button();
		bnQuit = new Button();
		grpWaitInput = new GroupBox();
		cbWaitInput4 = new ComboBox();
		cbWaitInput3 = new ComboBox();
		cbWaitInput2 = new ComboBox();
		cbWaitInput1 = new ComboBox();
		label52 = new Label();
		label53 = new Label();
		label54 = new Label();
		label55 = new Label();
		tbWaitInputs = new TextBox();
		cbSyncExInput = new CheckBox();
		((Control)grpNotIntrpltType).SuspendLayout();
		((Control)grpAxis6).SuspendLayout();
		((Control)grpAxis5).SuspendLayout();
		((Control)grpAxis4).SuspendLayout();
		((Control)grpAxis3).SuspendLayout();
		((Control)grpAxis2).SuspendLayout();
		((Control)grpAxis1).SuspendLayout();
		((Control)grpIntrpltType).SuspendLayout();
		((Control)grpIntrAxis6).SuspendLayout();
		((Control)grpIntrAxis5).SuspendLayout();
		((Control)grpIntrAxis4).SuspendLayout();
		((Control)grpIntrAxis3).SuspendLayout();
		((Control)grpIntrAxis2).SuspendLayout();
		((Control)grpIntrAxis1).SuspendLayout();
		((Control)grpOutputType).SuspendLayout();
		((Control)grpHomeType).SuspendLayout();
		((Control)grpCircleType).SuspendLayout();
		((Control)grpWaitInput).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(cb_MotionType, "cb_MotionType");
		((ListControl)cb_MotionType).FormattingEnabled = true;
		cb_MotionType.Items.AddRange(new object[21]
		{
			componentResourceManager.GetString("cb_MotionType.Items"),
			componentResourceManager.GetString("cb_MotionType.Items1"),
			componentResourceManager.GetString("cb_MotionType.Items2"),
			componentResourceManager.GetString("cb_MotionType.Items3"),
			componentResourceManager.GetString("cb_MotionType.Items4"),
			componentResourceManager.GetString("cb_MotionType.Items5"),
			componentResourceManager.GetString("cb_MotionType.Items6"),
			componentResourceManager.GetString("cb_MotionType.Items7"),
			componentResourceManager.GetString("cb_MotionType.Items8"),
			componentResourceManager.GetString("cb_MotionType.Items9"),
			componentResourceManager.GetString("cb_MotionType.Items10"),
			componentResourceManager.GetString("cb_MotionType.Items11"),
			componentResourceManager.GetString("cb_MotionType.Items12"),
			componentResourceManager.GetString("cb_MotionType.Items13"),
			componentResourceManager.GetString("cb_MotionType.Items14"),
			componentResourceManager.GetString("cb_MotionType.Items15"),
			componentResourceManager.GetString("cb_MotionType.Items16"),
			componentResourceManager.GetString("cb_MotionType.Items17"),
			componentResourceManager.GetString("cb_MotionType.Items18"),
			componentResourceManager.GetString("cb_MotionType.Items19"),
			componentResourceManager.GetString("cb_MotionType.Items20")
		});
		((Control)cb_MotionType).Name = "cb_MotionType";
		cb_MotionType.SelectedIndexChanged += cb_MotionType_SelectedIndexChanged;
		componentResourceManager.ApplyResources(lb_MotionType, "lb_MotionType");
		((Control)lb_MotionType).Name = "lb_MotionType";
		componentResourceManager.ApplyResources(cb_ProType, "cb_ProType");
		((ListControl)cb_ProType).FormattingEnabled = true;
		cb_ProType.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cb_ProType.Items"),
			componentResourceManager.GetString("cb_ProType.Items1")
		});
		((Control)cb_ProType).Name = "cb_ProType";
		cb_ProType.SelectedIndexChanged += cb_ProType_SelectedIndexChanged;
		componentResourceManager.ApplyResources(lb_ProType, "lb_ProType");
		((Control)lb_ProType).Name = "lb_ProType";
		componentResourceManager.ApplyResources(grpNotIntrpltType, "grpNotIntrpltType");
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis6);
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis5);
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis4);
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis3);
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis2);
		((Control)grpNotIntrpltType).Controls.Add((Control)(object)grpAxis1);
		((Control)grpNotIntrpltType).Name = "grpNotIntrpltType";
		grpNotIntrpltType.TabStop = false;
		componentResourceManager.ApplyResources(grpAxis6, "grpAxis6");
		((Control)grpAxis6).Controls.Add((Control)(object)Txt_GrpAxis6_AxisAcc);
		((Control)grpAxis6).Controls.Add((Control)(object)Txt_GrpAxis6_AxisSpd);
		((Control)grpAxis6).Controls.Add((Control)(object)Txt_GrpAxis6_AxisPos);
		((Control)grpAxis6).Controls.Add((Control)(object)label38);
		((Control)grpAxis6).Controls.Add((Control)(object)label40);
		((Control)grpAxis6).Controls.Add((Control)(object)label41);
		((Control)grpAxis6).Controls.Add((Control)(object)label42);
		((Control)grpAxis6).Controls.Add((Control)(object)cb_GrpAxis6_AxisIndex);
		((Control)grpAxis6).Name = "grpAxis6";
		grpAxis6.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis6_AxisAcc, "Txt_GrpAxis6_AxisAcc");
		((Control)Txt_GrpAxis6_AxisAcc).Name = "Txt_GrpAxis6_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis6_AxisSpd, "Txt_GrpAxis6_AxisSpd");
		((Control)Txt_GrpAxis6_AxisSpd).Name = "Txt_GrpAxis6_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis6_AxisPos, "Txt_GrpAxis6_AxisPos");
		((Control)Txt_GrpAxis6_AxisPos).Name = "Txt_GrpAxis6_AxisPos";
		componentResourceManager.ApplyResources(label38, "label38");
		((Control)label38).Name = "label38";
		componentResourceManager.ApplyResources(label40, "label40");
		((Control)label40).Name = "label40";
		componentResourceManager.ApplyResources(label41, "label41");
		((Control)label41).Name = "label41";
		componentResourceManager.ApplyResources(label42, "label42");
		((Control)label42).Name = "label42";
		componentResourceManager.ApplyResources(cb_GrpAxis6_AxisIndex, "cb_GrpAxis6_AxisIndex");
		((ListControl)cb_GrpAxis6_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis6_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis6_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis6_AxisIndex).Name = "cb_GrpAxis6_AxisIndex";
		componentResourceManager.ApplyResources(grpAxis5, "grpAxis5");
		((Control)grpAxis5).Controls.Add((Control)(object)Txt_GrpAxis5_AxisAcc);
		((Control)grpAxis5).Controls.Add((Control)(object)Txt_GrpAxis5_AxisSpd);
		((Control)grpAxis5).Controls.Add((Control)(object)Txt_GrpAxis5_AxisPos);
		((Control)grpAxis5).Controls.Add((Control)(object)label30);
		((Control)grpAxis5).Controls.Add((Control)(object)label34);
		((Control)grpAxis5).Controls.Add((Control)(object)label35);
		((Control)grpAxis5).Controls.Add((Control)(object)label37);
		((Control)grpAxis5).Controls.Add((Control)(object)cb_GrpAxis5_AxisIndex);
		((Control)grpAxis5).Name = "grpAxis5";
		grpAxis5.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis5_AxisAcc, "Txt_GrpAxis5_AxisAcc");
		((Control)Txt_GrpAxis5_AxisAcc).Name = "Txt_GrpAxis5_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis5_AxisSpd, "Txt_GrpAxis5_AxisSpd");
		((Control)Txt_GrpAxis5_AxisSpd).Name = "Txt_GrpAxis5_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis5_AxisPos, "Txt_GrpAxis5_AxisPos");
		((Control)Txt_GrpAxis5_AxisPos).Name = "Txt_GrpAxis5_AxisPos";
		componentResourceManager.ApplyResources(label30, "label30");
		((Control)label30).Name = "label30";
		componentResourceManager.ApplyResources(label34, "label34");
		((Control)label34).Name = "label34";
		componentResourceManager.ApplyResources(label35, "label35");
		((Control)label35).Name = "label35";
		componentResourceManager.ApplyResources(label37, "label37");
		((Control)label37).Name = "label37";
		componentResourceManager.ApplyResources(cb_GrpAxis5_AxisIndex, "cb_GrpAxis5_AxisIndex");
		((ListControl)cb_GrpAxis5_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis5_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis5_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis5_AxisIndex).Name = "cb_GrpAxis5_AxisIndex";
		componentResourceManager.ApplyResources(grpAxis4, "grpAxis4");
		((Control)grpAxis4).Controls.Add((Control)(object)Txt_GrpAxis4_AxisAcc);
		((Control)grpAxis4).Controls.Add((Control)(object)Txt_GrpAxis4_AxisSpd);
		((Control)grpAxis4).Controls.Add((Control)(object)Txt_GrpAxis4_AxisPos);
		((Control)grpAxis4).Controls.Add((Control)(object)label13);
		((Control)grpAxis4).Controls.Add((Control)(object)label14);
		((Control)grpAxis4).Controls.Add((Control)(object)label15);
		((Control)grpAxis4).Controls.Add((Control)(object)label16);
		((Control)grpAxis4).Controls.Add((Control)(object)cb_GrpAxis4_AxisIndex);
		((Control)grpAxis4).Name = "grpAxis4";
		grpAxis4.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis4_AxisAcc, "Txt_GrpAxis4_AxisAcc");
		((Control)Txt_GrpAxis4_AxisAcc).Name = "Txt_GrpAxis4_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis4_AxisSpd, "Txt_GrpAxis4_AxisSpd");
		((Control)Txt_GrpAxis4_AxisSpd).Name = "Txt_GrpAxis4_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis4_AxisPos, "Txt_GrpAxis4_AxisPos");
		((Control)Txt_GrpAxis4_AxisPos).Name = "Txt_GrpAxis4_AxisPos";
		componentResourceManager.ApplyResources(label13, "label13");
		((Control)label13).Name = "label13";
		componentResourceManager.ApplyResources(label14, "label14");
		((Control)label14).Name = "label14";
		componentResourceManager.ApplyResources(label15, "label15");
		((Control)label15).Name = "label15";
		componentResourceManager.ApplyResources(label16, "label16");
		((Control)label16).Name = "label16";
		componentResourceManager.ApplyResources(cb_GrpAxis4_AxisIndex, "cb_GrpAxis4_AxisIndex");
		((ListControl)cb_GrpAxis4_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis4_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis4_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis4_AxisIndex).Name = "cb_GrpAxis4_AxisIndex";
		componentResourceManager.ApplyResources(grpAxis3, "grpAxis3");
		((Control)grpAxis3).Controls.Add((Control)(object)Txt_GrpAxis3_AxisAcc);
		((Control)grpAxis3).Controls.Add((Control)(object)Txt_GrpAxis3_AxisSpd);
		((Control)grpAxis3).Controls.Add((Control)(object)Txt_GrpAxis3_AxisPos);
		((Control)grpAxis3).Controls.Add((Control)(object)label9);
		((Control)grpAxis3).Controls.Add((Control)(object)label10);
		((Control)grpAxis3).Controls.Add((Control)(object)label11);
		((Control)grpAxis3).Controls.Add((Control)(object)label12);
		((Control)grpAxis3).Controls.Add((Control)(object)cb_GrpAxis3_AxisIndex);
		((Control)grpAxis3).Name = "grpAxis3";
		grpAxis3.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis3_AxisAcc, "Txt_GrpAxis3_AxisAcc");
		((Control)Txt_GrpAxis3_AxisAcc).Name = "Txt_GrpAxis3_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis3_AxisSpd, "Txt_GrpAxis3_AxisSpd");
		((Control)Txt_GrpAxis3_AxisSpd).Name = "Txt_GrpAxis3_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis3_AxisPos, "Txt_GrpAxis3_AxisPos");
		((Control)Txt_GrpAxis3_AxisPos).Name = "Txt_GrpAxis3_AxisPos";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(label12, "label12");
		((Control)label12).Name = "label12";
		componentResourceManager.ApplyResources(cb_GrpAxis3_AxisIndex, "cb_GrpAxis3_AxisIndex");
		((ListControl)cb_GrpAxis3_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis3_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis3_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis3_AxisIndex).Name = "cb_GrpAxis3_AxisIndex";
		componentResourceManager.ApplyResources(grpAxis2, "grpAxis2");
		((Control)grpAxis2).Controls.Add((Control)(object)Txt_GrpAxis2_AxisAcc);
		((Control)grpAxis2).Controls.Add((Control)(object)Txt_GrpAxis2_AxisSpd);
		((Control)grpAxis2).Controls.Add((Control)(object)Txt_GrpAxis2_AxisPos);
		((Control)grpAxis2).Controls.Add((Control)(object)label5);
		((Control)grpAxis2).Controls.Add((Control)(object)label6);
		((Control)grpAxis2).Controls.Add((Control)(object)label7);
		((Control)grpAxis2).Controls.Add((Control)(object)label8);
		((Control)grpAxis2).Controls.Add((Control)(object)cb_GrpAxis2_AxisIndex);
		((Control)grpAxis2).Name = "grpAxis2";
		grpAxis2.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis2_AxisAcc, "Txt_GrpAxis2_AxisAcc");
		((Control)Txt_GrpAxis2_AxisAcc).Name = "Txt_GrpAxis2_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis2_AxisSpd, "Txt_GrpAxis2_AxisSpd");
		((Control)Txt_GrpAxis2_AxisSpd).Name = "Txt_GrpAxis2_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis2_AxisPos, "Txt_GrpAxis2_AxisPos");
		((Control)Txt_GrpAxis2_AxisPos).Name = "Txt_GrpAxis2_AxisPos";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(cb_GrpAxis2_AxisIndex, "cb_GrpAxis2_AxisIndex");
		((ListControl)cb_GrpAxis2_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis2_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis2_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis2_AxisIndex).Name = "cb_GrpAxis2_AxisIndex";
		componentResourceManager.ApplyResources(grpAxis1, "grpAxis1");
		((Control)grpAxis1).Controls.Add((Control)(object)Txt_GrpAxis1_AxisAcc);
		((Control)grpAxis1).Controls.Add((Control)(object)Txt_GrpAxis1_AxisSpd);
		((Control)grpAxis1).Controls.Add((Control)(object)Txt_GrpAxis1_AxisPos);
		((Control)grpAxis1).Controls.Add((Control)(object)label4);
		((Control)grpAxis1).Controls.Add((Control)(object)label3);
		((Control)grpAxis1).Controls.Add((Control)(object)label2);
		((Control)grpAxis1).Controls.Add((Control)(object)label1);
		((Control)grpAxis1).Controls.Add((Control)(object)cb_GrpAxis1_AxisIndex);
		((Control)grpAxis1).Name = "grpAxis1";
		grpAxis1.TabStop = false;
		componentResourceManager.ApplyResources(Txt_GrpAxis1_AxisAcc, "Txt_GrpAxis1_AxisAcc");
		((Control)Txt_GrpAxis1_AxisAcc).Name = "Txt_GrpAxis1_AxisAcc";
		componentResourceManager.ApplyResources(Txt_GrpAxis1_AxisSpd, "Txt_GrpAxis1_AxisSpd");
		((Control)Txt_GrpAxis1_AxisSpd).Name = "Txt_GrpAxis1_AxisSpd";
		componentResourceManager.ApplyResources(Txt_GrpAxis1_AxisPos, "Txt_GrpAxis1_AxisPos");
		((Control)Txt_GrpAxis1_AxisPos).Name = "Txt_GrpAxis1_AxisPos";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(cb_GrpAxis1_AxisIndex, "cb_GrpAxis1_AxisIndex");
		((ListControl)cb_GrpAxis1_AxisIndex).FormattingEnabled = true;
		cb_GrpAxis1_AxisIndex.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items1"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items2"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items3"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items4"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items5"),
			componentResourceManager.GetString("cb_GrpAxis1_AxisIndex.Items6")
		});
		((Control)cb_GrpAxis1_AxisIndex).Name = "cb_GrpAxis1_AxisIndex";
		componentResourceManager.ApplyResources(grpIntrpltType, "grpIntrpltType");
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis6);
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis5);
		((Control)grpIntrpltType).Controls.Add((Control)(object)Txt_grpIntrAxis_Acc);
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis4);
		((Control)grpIntrpltType).Controls.Add((Control)(object)Txt_grpIntrAxis_Spd);
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis3);
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis2);
		((Control)grpIntrpltType).Controls.Add((Control)(object)label17);
		((Control)grpIntrpltType).Controls.Add((Control)(object)label18);
		((Control)grpIntrpltType).Controls.Add((Control)(object)grpIntrAxis1);
		((Control)grpIntrpltType).Name = "grpIntrpltType";
		grpIntrpltType.TabStop = false;
		componentResourceManager.ApplyResources(grpIntrAxis6, "grpIntrAxis6");
		((Control)grpIntrAxis6).Controls.Add((Control)(object)Txt_grpIntrAxis6_Dist);
		((Control)grpIntrAxis6).Controls.Add((Control)(object)label45);
		((Control)grpIntrAxis6).Controls.Add((Control)(object)label49);
		((Control)grpIntrAxis6).Controls.Add((Control)(object)cb_grpIntrAxis6_Index);
		((Control)grpIntrAxis6).Name = "grpIntrAxis6";
		grpIntrAxis6.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis6_Dist, "Txt_grpIntrAxis6_Dist");
		((Control)Txt_grpIntrAxis6_Dist).Name = "Txt_grpIntrAxis6_Dist";
		componentResourceManager.ApplyResources(label45, "label45");
		((Control)label45).Name = "label45";
		componentResourceManager.ApplyResources(label49, "label49");
		((Control)label49).Name = "label49";
		componentResourceManager.ApplyResources(cb_grpIntrAxis6_Index, "cb_grpIntrAxis6_Index");
		((ListControl)cb_grpIntrAxis6_Index).FormattingEnabled = true;
		cb_grpIntrAxis6_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis6_Index.Items6")
		});
		((Control)cb_grpIntrAxis6_Index).Name = "cb_grpIntrAxis6_Index";
		componentResourceManager.ApplyResources(grpIntrAxis5, "grpIntrAxis5");
		((Control)grpIntrAxis5).Controls.Add((Control)(object)Txt_grpIntrAxis5_Dist);
		((Control)grpIntrAxis5).Controls.Add((Control)(object)label43);
		((Control)grpIntrAxis5).Controls.Add((Control)(object)label44);
		((Control)grpIntrAxis5).Controls.Add((Control)(object)cb_grpIntrAxis5_Index);
		((Control)grpIntrAxis5).Name = "grpIntrAxis5";
		grpIntrAxis5.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis5_Dist, "Txt_grpIntrAxis5_Dist");
		((Control)Txt_grpIntrAxis5_Dist).Name = "Txt_grpIntrAxis5_Dist";
		componentResourceManager.ApplyResources(label43, "label43");
		((Control)label43).Name = "label43";
		componentResourceManager.ApplyResources(label44, "label44");
		((Control)label44).Name = "label44";
		componentResourceManager.ApplyResources(cb_grpIntrAxis5_Index, "cb_grpIntrAxis5_Index");
		((ListControl)cb_grpIntrAxis5_Index).FormattingEnabled = true;
		cb_grpIntrAxis5_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis5_Index.Items6")
		});
		((Control)cb_grpIntrAxis5_Index).Name = "cb_grpIntrAxis5_Index";
		componentResourceManager.ApplyResources(Txt_grpIntrAxis_Acc, "Txt_grpIntrAxis_Acc");
		((Control)Txt_grpIntrAxis_Acc).Name = "Txt_grpIntrAxis_Acc";
		componentResourceManager.ApplyResources(grpIntrAxis4, "grpIntrAxis4");
		((Control)grpIntrAxis4).Controls.Add((Control)(object)Txt_grpIntrAxis4_Dist);
		((Control)grpIntrAxis4).Controls.Add((Control)(object)label19);
		((Control)grpIntrAxis4).Controls.Add((Control)(object)label20);
		((Control)grpIntrAxis4).Controls.Add((Control)(object)cb_grpIntrAxis4_Index);
		((Control)grpIntrAxis4).Name = "grpIntrAxis4";
		grpIntrAxis4.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis4_Dist, "Txt_grpIntrAxis4_Dist");
		((Control)Txt_grpIntrAxis4_Dist).Name = "Txt_grpIntrAxis4_Dist";
		componentResourceManager.ApplyResources(label19, "label19");
		((Control)label19).Name = "label19";
		componentResourceManager.ApplyResources(label20, "label20");
		((Control)label20).Name = "label20";
		componentResourceManager.ApplyResources(cb_grpIntrAxis4_Index, "cb_grpIntrAxis4_Index");
		((ListControl)cb_grpIntrAxis4_Index).FormattingEnabled = true;
		cb_grpIntrAxis4_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis4_Index.Items6")
		});
		((Control)cb_grpIntrAxis4_Index).Name = "cb_grpIntrAxis4_Index";
		componentResourceManager.ApplyResources(Txt_grpIntrAxis_Spd, "Txt_grpIntrAxis_Spd");
		((Control)Txt_grpIntrAxis_Spd).Name = "Txt_grpIntrAxis_Spd";
		componentResourceManager.ApplyResources(grpIntrAxis3, "grpIntrAxis3");
		((Control)grpIntrAxis3).Controls.Add((Control)(object)Txt_grpIntrAxis3_Dist);
		((Control)grpIntrAxis3).Controls.Add((Control)(object)label23);
		((Control)grpIntrAxis3).Controls.Add((Control)(object)label24);
		((Control)grpIntrAxis3).Controls.Add((Control)(object)cb_grpIntrAxis3_Index);
		((Control)grpIntrAxis3).Name = "grpIntrAxis3";
		grpIntrAxis3.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis3_Dist, "Txt_grpIntrAxis3_Dist");
		((Control)Txt_grpIntrAxis3_Dist).Name = "Txt_grpIntrAxis3_Dist";
		componentResourceManager.ApplyResources(label23, "label23");
		((Control)label23).Name = "label23";
		componentResourceManager.ApplyResources(label24, "label24");
		((Control)label24).Name = "label24";
		componentResourceManager.ApplyResources(cb_grpIntrAxis3_Index, "cb_grpIntrAxis3_Index");
		((ListControl)cb_grpIntrAxis3_Index).FormattingEnabled = true;
		cb_grpIntrAxis3_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis3_Index.Items6")
		});
		((Control)cb_grpIntrAxis3_Index).Name = "cb_grpIntrAxis3_Index";
		componentResourceManager.ApplyResources(grpIntrAxis2, "grpIntrAxis2");
		((Control)grpIntrAxis2).Controls.Add((Control)(object)Txt_grpIntrAxis2_Dist);
		((Control)grpIntrAxis2).Controls.Add((Control)(object)label27);
		((Control)grpIntrAxis2).Controls.Add((Control)(object)label28);
		((Control)grpIntrAxis2).Controls.Add((Control)(object)cb_grpIntrAxis2_Index);
		((Control)grpIntrAxis2).Name = "grpIntrAxis2";
		grpIntrAxis2.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis2_Dist, "Txt_grpIntrAxis2_Dist");
		((Control)Txt_grpIntrAxis2_Dist).Name = "Txt_grpIntrAxis2_Dist";
		componentResourceManager.ApplyResources(label27, "label27");
		((Control)label27).Name = "label27";
		componentResourceManager.ApplyResources(label28, "label28");
		((Control)label28).Name = "label28";
		componentResourceManager.ApplyResources(cb_grpIntrAxis2_Index, "cb_grpIntrAxis2_Index");
		((ListControl)cb_grpIntrAxis2_Index).FormattingEnabled = true;
		cb_grpIntrAxis2_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis2_Index.Items6")
		});
		((Control)cb_grpIntrAxis2_Index).Name = "cb_grpIntrAxis2_Index";
		componentResourceManager.ApplyResources(label17, "label17");
		((Control)label17).Name = "label17";
		componentResourceManager.ApplyResources(label18, "label18");
		((Control)label18).Name = "label18";
		componentResourceManager.ApplyResources(grpIntrAxis1, "grpIntrAxis1");
		((Control)grpIntrAxis1).Controls.Add((Control)(object)Txt_grpIntrAxis1_Dist);
		((Control)grpIntrAxis1).Controls.Add((Control)(object)label31);
		((Control)grpIntrAxis1).Controls.Add((Control)(object)label32);
		((Control)grpIntrAxis1).Controls.Add((Control)(object)cb_grpIntrAxis1_Index);
		((Control)grpIntrAxis1).Name = "grpIntrAxis1";
		grpIntrAxis1.TabStop = false;
		componentResourceManager.ApplyResources(Txt_grpIntrAxis1_Dist, "Txt_grpIntrAxis1_Dist");
		((Control)Txt_grpIntrAxis1_Dist).Name = "Txt_grpIntrAxis1_Dist";
		componentResourceManager.ApplyResources(label31, "label31");
		((Control)label31).Name = "label31";
		componentResourceManager.ApplyResources(label32, "label32");
		((Control)label32).Name = "label32";
		componentResourceManager.ApplyResources(cb_grpIntrAxis1_Index, "cb_grpIntrAxis1_Index");
		((ListControl)cb_grpIntrAxis1_Index).FormattingEnabled = true;
		cb_grpIntrAxis1_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items1"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items2"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items3"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items4"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items5"),
			componentResourceManager.GetString("cb_grpIntrAxis1_Index.Items6")
		});
		((Control)cb_grpIntrAxis1_Index).Name = "cb_grpIntrAxis1_Index";
		componentResourceManager.ApplyResources(grpOutputType, "grpOutputType");
		((Control)grpOutputType).Controls.Add((Control)(object)label21);
		((Control)grpOutputType).Controls.Add((Control)(object)label36);
		((Control)grpOutputType).Controls.Add((Control)(object)label33);
		((Control)grpOutputType).Controls.Add((Control)(object)cb_Output_Value);
		((Control)grpOutputType).Controls.Add((Control)(object)cb_Output_Index);
		((Control)grpOutputType).Name = "grpOutputType";
		grpOutputType.TabStop = false;
		componentResourceManager.ApplyResources(label21, "label21");
		((Control)label21).Name = "label21";
		componentResourceManager.ApplyResources(label36, "label36");
		((Control)label36).Name = "label36";
		componentResourceManager.ApplyResources(label33, "label33");
		((Control)label33).Name = "label33";
		componentResourceManager.ApplyResources(cb_Output_Value, "cb_Output_Value");
		((ListControl)cb_Output_Value).FormattingEnabled = true;
		cb_Output_Value.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cb_Output_Value.Items"),
			componentResourceManager.GetString("cb_Output_Value.Items1")
		});
		((Control)cb_Output_Value).Name = "cb_Output_Value";
		componentResourceManager.ApplyResources(cb_Output_Index, "cb_Output_Index");
		((ListControl)cb_Output_Index).FormattingEnabled = true;
		cb_Output_Index.Items.AddRange(new object[16]
		{
			componentResourceManager.GetString("cb_Output_Index.Items"),
			componentResourceManager.GetString("cb_Output_Index.Items1"),
			componentResourceManager.GetString("cb_Output_Index.Items2"),
			componentResourceManager.GetString("cb_Output_Index.Items3"),
			componentResourceManager.GetString("cb_Output_Index.Items4"),
			componentResourceManager.GetString("cb_Output_Index.Items5"),
			componentResourceManager.GetString("cb_Output_Index.Items6"),
			componentResourceManager.GetString("cb_Output_Index.Items7"),
			componentResourceManager.GetString("cb_Output_Index.Items8"),
			componentResourceManager.GetString("cb_Output_Index.Items9"),
			componentResourceManager.GetString("cb_Output_Index.Items10"),
			componentResourceManager.GetString("cb_Output_Index.Items11"),
			componentResourceManager.GetString("cb_Output_Index.Items12"),
			componentResourceManager.GetString("cb_Output_Index.Items13"),
			componentResourceManager.GetString("cb_Output_Index.Items14"),
			componentResourceManager.GetString("cb_Output_Index.Items15")
		});
		((Control)cb_Output_Index).Name = "cb_Output_Index";
		componentResourceManager.ApplyResources(grpHomeType, "grpHomeType");
		((Control)grpHomeType).Controls.Add((Control)(object)label51);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis6_Index);
		((Control)grpHomeType).Controls.Add((Control)(object)label50);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis5_Index);
		((Control)grpHomeType).Controls.Add((Control)(object)label26);
		((Control)grpHomeType).Controls.Add((Control)(object)label25);
		((Control)grpHomeType).Controls.Add((Control)(object)label22);
		((Control)grpHomeType).Controls.Add((Control)(object)label39);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis4_Index);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis3_Index);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis2_Index);
		((Control)grpHomeType).Controls.Add((Control)(object)cb_grphomeAxis1_Index);
		((Control)grpHomeType).Name = "grpHomeType";
		grpHomeType.TabStop = false;
		componentResourceManager.ApplyResources(label51, "label51");
		((Control)label51).Name = "label51";
		componentResourceManager.ApplyResources(cb_grphomeAxis6_Index, "cb_grphomeAxis6_Index");
		((ListControl)cb_grphomeAxis6_Index).FormattingEnabled = true;
		cb_grphomeAxis6_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis6_Index.Items6")
		});
		((Control)cb_grphomeAxis6_Index).Name = "cb_grphomeAxis6_Index";
		componentResourceManager.ApplyResources(label50, "label50");
		((Control)label50).Name = "label50";
		componentResourceManager.ApplyResources(cb_grphomeAxis5_Index, "cb_grphomeAxis5_Index");
		((ListControl)cb_grphomeAxis5_Index).FormattingEnabled = true;
		cb_grphomeAxis5_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis5_Index.Items6")
		});
		((Control)cb_grphomeAxis5_Index).Name = "cb_grphomeAxis5_Index";
		componentResourceManager.ApplyResources(label26, "label26");
		((Control)label26).Name = "label26";
		componentResourceManager.ApplyResources(label25, "label25");
		((Control)label25).Name = "label25";
		componentResourceManager.ApplyResources(label22, "label22");
		((Control)label22).Name = "label22";
		componentResourceManager.ApplyResources(label39, "label39");
		((Control)label39).Name = "label39";
		componentResourceManager.ApplyResources(cb_grphomeAxis4_Index, "cb_grphomeAxis4_Index");
		((ListControl)cb_grphomeAxis4_Index).FormattingEnabled = true;
		cb_grphomeAxis4_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis4_Index.Items6")
		});
		((Control)cb_grphomeAxis4_Index).Name = "cb_grphomeAxis4_Index";
		componentResourceManager.ApplyResources(cb_grphomeAxis3_Index, "cb_grphomeAxis3_Index");
		((ListControl)cb_grphomeAxis3_Index).FormattingEnabled = true;
		cb_grphomeAxis3_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis3_Index.Items6")
		});
		((Control)cb_grphomeAxis3_Index).Name = "cb_grphomeAxis3_Index";
		componentResourceManager.ApplyResources(cb_grphomeAxis2_Index, "cb_grphomeAxis2_Index");
		((ListControl)cb_grphomeAxis2_Index).FormattingEnabled = true;
		cb_grphomeAxis2_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis2_Index.Items6")
		});
		((Control)cb_grphomeAxis2_Index).Name = "cb_grphomeAxis2_Index";
		componentResourceManager.ApplyResources(cb_grphomeAxis1_Index, "cb_grphomeAxis1_Index");
		((ListControl)cb_grphomeAxis1_Index).FormattingEnabled = true;
		cb_grphomeAxis1_Index.Items.AddRange(new object[7]
		{
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items1"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items2"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items3"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items4"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items5"),
			componentResourceManager.GetString("cb_grphomeAxis1_Index.Items6")
		});
		((Control)cb_grphomeAxis1_Index).Name = "cb_grphomeAxis1_Index";
		componentResourceManager.ApplyResources(grpCircleType, "grpCircleType");
		((Control)grpCircleType).Controls.Add((Control)(object)Txt_CircleAxis_R);
		((Control)grpCircleType).Controls.Add((Control)(object)Txt_CircleAxis_Spd);
		((Control)grpCircleType).Controls.Add((Control)(object)Txt_CircleAxis_Y);
		((Control)grpCircleType).Controls.Add((Control)(object)Txt_CircleAxis_X);
		((Control)grpCircleType).Controls.Add((Control)(object)label46);
		((Control)grpCircleType).Controls.Add((Control)(object)label29);
		((Control)grpCircleType).Controls.Add((Control)(object)label47);
		((Control)grpCircleType).Controls.Add((Control)(object)label48);
		((Control)grpCircleType).Name = "grpCircleType";
		grpCircleType.TabStop = false;
		((Control)grpCircleType).Enter += grpCircleType_Enter;
		componentResourceManager.ApplyResources(Txt_CircleAxis_R, "Txt_CircleAxis_R");
		((Control)Txt_CircleAxis_R).Name = "Txt_CircleAxis_R";
		componentResourceManager.ApplyResources(Txt_CircleAxis_Spd, "Txt_CircleAxis_Spd");
		((Control)Txt_CircleAxis_Spd).Name = "Txt_CircleAxis_Spd";
		componentResourceManager.ApplyResources(Txt_CircleAxis_Y, "Txt_CircleAxis_Y");
		((Control)Txt_CircleAxis_Y).Name = "Txt_CircleAxis_Y";
		componentResourceManager.ApplyResources(Txt_CircleAxis_X, "Txt_CircleAxis_X");
		((Control)Txt_CircleAxis_X).Name = "Txt_CircleAxis_X";
		componentResourceManager.ApplyResources(label46, "label46");
		((Control)label46).Name = "label46";
		componentResourceManager.ApplyResources(label29, "label29");
		((Control)label29).Name = "label29";
		componentResourceManager.ApplyResources(label47, "label47");
		((Control)label47).Name = "label47";
		componentResourceManager.ApplyResources(label48, "label48");
		((Control)label48).Name = "label48";
		componentResourceManager.ApplyResources(lb_CircleTimes, "lb_CircleTimes");
		((Control)lb_CircleTimes).Name = "lb_CircleTimes";
		componentResourceManager.ApplyResources(lb_DelayTime, "lb_DelayTime");
		((Control)lb_DelayTime).Name = "lb_DelayTime";
		componentResourceManager.ApplyResources(Txt_CircleTimes, "Txt_CircleTimes");
		((Control)Txt_CircleTimes).Name = "Txt_CircleTimes";
		componentResourceManager.ApplyResources(Txt_DelayTime, "Txt_DelayTime");
		((Control)Txt_DelayTime).Name = "Txt_DelayTime";
		componentResourceManager.ApplyResources(bnConfirm, "bnConfirm");
		((Control)bnConfirm).Name = "bnConfirm";
		((ButtonBase)bnConfirm).UseVisualStyleBackColor = true;
		((Control)bnConfirm).Click += bnConfirm_Click;
		componentResourceManager.ApplyResources(bnQuit, "bnQuit");
		((Control)bnQuit).Name = "bnQuit";
		((ButtonBase)bnQuit).UseVisualStyleBackColor = true;
		((Control)bnQuit).Click += bnQuit_Click;
		componentResourceManager.ApplyResources(grpWaitInput, "grpWaitInput");
		((Control)grpWaitInput).Controls.Add((Control)(object)cbWaitInput4);
		((Control)grpWaitInput).Controls.Add((Control)(object)cbWaitInput3);
		((Control)grpWaitInput).Controls.Add((Control)(object)cbWaitInput2);
		((Control)grpWaitInput).Controls.Add((Control)(object)cbWaitInput1);
		((Control)grpWaitInput).Controls.Add((Control)(object)label52);
		((Control)grpWaitInput).Controls.Add((Control)(object)label53);
		((Control)grpWaitInput).Controls.Add((Control)(object)label54);
		((Control)grpWaitInput).Controls.Add((Control)(object)label55);
		((Control)grpWaitInput).Controls.Add((Control)(object)tbWaitInputs);
		((Control)grpWaitInput).Name = "grpWaitInput";
		grpWaitInput.TabStop = false;
		componentResourceManager.ApplyResources(cbWaitInput4, "cbWaitInput4");
		((ListControl)cbWaitInput4).FormattingEnabled = true;
		cbWaitInput4.Items.AddRange(new object[17]
		{
			componentResourceManager.GetString("cbWaitInput4.Items"),
			componentResourceManager.GetString("cbWaitInput4.Items1"),
			componentResourceManager.GetString("cbWaitInput4.Items2"),
			componentResourceManager.GetString("cbWaitInput4.Items3"),
			componentResourceManager.GetString("cbWaitInput4.Items4"),
			componentResourceManager.GetString("cbWaitInput4.Items5"),
			componentResourceManager.GetString("cbWaitInput4.Items6"),
			componentResourceManager.GetString("cbWaitInput4.Items7"),
			componentResourceManager.GetString("cbWaitInput4.Items8"),
			componentResourceManager.GetString("cbWaitInput4.Items9"),
			componentResourceManager.GetString("cbWaitInput4.Items10"),
			componentResourceManager.GetString("cbWaitInput4.Items11"),
			componentResourceManager.GetString("cbWaitInput4.Items12"),
			componentResourceManager.GetString("cbWaitInput4.Items13"),
			componentResourceManager.GetString("cbWaitInput4.Items14"),
			componentResourceManager.GetString("cbWaitInput4.Items15"),
			componentResourceManager.GetString("cbWaitInput4.Items16")
		});
		((Control)cbWaitInput4).Name = "cbWaitInput4";
		cbWaitInput4.SelectedIndexChanged += cbWaitInput1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(cbWaitInput3, "cbWaitInput3");
		((ListControl)cbWaitInput3).FormattingEnabled = true;
		cbWaitInput3.Items.AddRange(new object[17]
		{
			componentResourceManager.GetString("cbWaitInput3.Items"),
			componentResourceManager.GetString("cbWaitInput3.Items1"),
			componentResourceManager.GetString("cbWaitInput3.Items2"),
			componentResourceManager.GetString("cbWaitInput3.Items3"),
			componentResourceManager.GetString("cbWaitInput3.Items4"),
			componentResourceManager.GetString("cbWaitInput3.Items5"),
			componentResourceManager.GetString("cbWaitInput3.Items6"),
			componentResourceManager.GetString("cbWaitInput3.Items7"),
			componentResourceManager.GetString("cbWaitInput3.Items8"),
			componentResourceManager.GetString("cbWaitInput3.Items9"),
			componentResourceManager.GetString("cbWaitInput3.Items10"),
			componentResourceManager.GetString("cbWaitInput3.Items11"),
			componentResourceManager.GetString("cbWaitInput3.Items12"),
			componentResourceManager.GetString("cbWaitInput3.Items13"),
			componentResourceManager.GetString("cbWaitInput3.Items14"),
			componentResourceManager.GetString("cbWaitInput3.Items15"),
			componentResourceManager.GetString("cbWaitInput3.Items16")
		});
		((Control)cbWaitInput3).Name = "cbWaitInput3";
		cbWaitInput3.SelectedIndexChanged += cbWaitInput1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(cbWaitInput2, "cbWaitInput2");
		((ListControl)cbWaitInput2).FormattingEnabled = true;
		cbWaitInput2.Items.AddRange(new object[17]
		{
			componentResourceManager.GetString("cbWaitInput2.Items"),
			componentResourceManager.GetString("cbWaitInput2.Items1"),
			componentResourceManager.GetString("cbWaitInput2.Items2"),
			componentResourceManager.GetString("cbWaitInput2.Items3"),
			componentResourceManager.GetString("cbWaitInput2.Items4"),
			componentResourceManager.GetString("cbWaitInput2.Items5"),
			componentResourceManager.GetString("cbWaitInput2.Items6"),
			componentResourceManager.GetString("cbWaitInput2.Items7"),
			componentResourceManager.GetString("cbWaitInput2.Items8"),
			componentResourceManager.GetString("cbWaitInput2.Items9"),
			componentResourceManager.GetString("cbWaitInput2.Items10"),
			componentResourceManager.GetString("cbWaitInput2.Items11"),
			componentResourceManager.GetString("cbWaitInput2.Items12"),
			componentResourceManager.GetString("cbWaitInput2.Items13"),
			componentResourceManager.GetString("cbWaitInput2.Items14"),
			componentResourceManager.GetString("cbWaitInput2.Items15"),
			componentResourceManager.GetString("cbWaitInput2.Items16")
		});
		((Control)cbWaitInput2).Name = "cbWaitInput2";
		cbWaitInput2.SelectedIndexChanged += cbWaitInput1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(cbWaitInput1, "cbWaitInput1");
		((ListControl)cbWaitInput1).FormattingEnabled = true;
		cbWaitInput1.Items.AddRange(new object[17]
		{
			componentResourceManager.GetString("cbWaitInput1.Items"),
			componentResourceManager.GetString("cbWaitInput1.Items1"),
			componentResourceManager.GetString("cbWaitInput1.Items2"),
			componentResourceManager.GetString("cbWaitInput1.Items3"),
			componentResourceManager.GetString("cbWaitInput1.Items4"),
			componentResourceManager.GetString("cbWaitInput1.Items5"),
			componentResourceManager.GetString("cbWaitInput1.Items6"),
			componentResourceManager.GetString("cbWaitInput1.Items7"),
			componentResourceManager.GetString("cbWaitInput1.Items8"),
			componentResourceManager.GetString("cbWaitInput1.Items9"),
			componentResourceManager.GetString("cbWaitInput1.Items10"),
			componentResourceManager.GetString("cbWaitInput1.Items11"),
			componentResourceManager.GetString("cbWaitInput1.Items12"),
			componentResourceManager.GetString("cbWaitInput1.Items13"),
			componentResourceManager.GetString("cbWaitInput1.Items14"),
			componentResourceManager.GetString("cbWaitInput1.Items15"),
			componentResourceManager.GetString("cbWaitInput1.Items16")
		});
		((Control)cbWaitInput1).Name = "cbWaitInput1";
		cbWaitInput1.SelectedIndexChanged += cbWaitInput1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(label52, "label52");
		((Control)label52).Name = "label52";
		componentResourceManager.ApplyResources(label53, "label53");
		((Control)label53).Name = "label53";
		componentResourceManager.ApplyResources(label54, "label54");
		((Control)label54).Name = "label54";
		componentResourceManager.ApplyResources(label55, "label55");
		((Control)label55).Name = "label55";
		componentResourceManager.ApplyResources(tbWaitInputs, "tbWaitInputs");
		((Control)tbWaitInputs).Name = "tbWaitInputs";
		componentResourceManager.ApplyResources(cbSyncExInput, "cbSyncExInput");
		((Control)cbSyncExInput).Name = "cbSyncExInput";
		((ButtonBase)cbSyncExInput).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)cbSyncExInput);
		((Control)this).Controls.Add((Control)(object)grpWaitInput);
		((Control)this).Controls.Add((Control)(object)bnQuit);
		((Control)this).Controls.Add((Control)(object)bnConfirm);
		((Control)this).Controls.Add((Control)(object)grpOutputType);
		((Control)this).Controls.Add((Control)(object)grpHomeType);
		((Control)this).Controls.Add((Control)(object)Txt_DelayTime);
		((Control)this).Controls.Add((Control)(object)Txt_CircleTimes);
		((Control)this).Controls.Add((Control)(object)grpIntrpltType);
		((Control)this).Controls.Add((Control)(object)grpCircleType);
		((Control)this).Controls.Add((Control)(object)grpNotIntrpltType);
		((Control)this).Controls.Add((Control)(object)lb_ProType);
		((Control)this).Controls.Add((Control)(object)lb_DelayTime);
		((Control)this).Controls.Add((Control)(object)lb_MotionType);
		((Control)this).Controls.Add((Control)(object)cb_ProType);
		((Control)this).Controls.Add((Control)(object)lb_CircleTimes);
		((Control)this).Controls.Add((Control)(object)cb_MotionType);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "ProgInputFrm";
		((Form)this).Load += ProgInputFrm_Load;
		((Control)grpNotIntrpltType).ResumeLayout(false);
		((Control)grpAxis6).ResumeLayout(false);
		((Control)grpAxis6).PerformLayout();
		((Control)grpAxis5).ResumeLayout(false);
		((Control)grpAxis5).PerformLayout();
		((Control)grpAxis4).ResumeLayout(false);
		((Control)grpAxis4).PerformLayout();
		((Control)grpAxis3).ResumeLayout(false);
		((Control)grpAxis3).PerformLayout();
		((Control)grpAxis2).ResumeLayout(false);
		((Control)grpAxis2).PerformLayout();
		((Control)grpAxis1).ResumeLayout(false);
		((Control)grpAxis1).PerformLayout();
		((Control)grpIntrpltType).ResumeLayout(false);
		((Control)grpIntrpltType).PerformLayout();
		((Control)grpIntrAxis6).ResumeLayout(false);
		((Control)grpIntrAxis6).PerformLayout();
		((Control)grpIntrAxis5).ResumeLayout(false);
		((Control)grpIntrAxis5).PerformLayout();
		((Control)grpIntrAxis4).ResumeLayout(false);
		((Control)grpIntrAxis4).PerformLayout();
		((Control)grpIntrAxis3).ResumeLayout(false);
		((Control)grpIntrAxis3).PerformLayout();
		((Control)grpIntrAxis2).ResumeLayout(false);
		((Control)grpIntrAxis2).PerformLayout();
		((Control)grpIntrAxis1).ResumeLayout(false);
		((Control)grpIntrAxis1).PerformLayout();
		((Control)grpOutputType).ResumeLayout(false);
		((Control)grpHomeType).ResumeLayout(false);
		((Control)grpCircleType).ResumeLayout(false);
		((Control)grpCircleType).PerformLayout();
		((Control)grpWaitInput).ResumeLayout(false);
		((Control)grpWaitInput).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
