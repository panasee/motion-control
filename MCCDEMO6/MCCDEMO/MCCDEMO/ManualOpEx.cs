using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class ManualOpEx : Form
{
	private MainFrm pMainFrm = new MainFrm();

	private IContainer components;

	private GroupBox groupBox1;

	private Label label2;

	private Label lbDist;

	private RadioButton rbMotionJog;

	private RadioButton rbMotionSpd;

	private Button bnMoveAll;

	private RadioButton rbMotionAbs;

	private CheckBox cbAxisChooseC;

	private CheckBox cbAxisChooseB;

	private CheckBox cbAxisChooseA;

	private CheckBox cbAxisChooseZ;

	private CheckBox cbAxisChooseY;

	private CheckBox cbAxisChooseX;

	private TextBox tbSpdC;

	private TextBox tbDistC;

	private TextBox tbSpdB;

	private TextBox tbDistB;

	private TextBox tbSpdA;

	private TextBox tbDistA;

	private TextBox tbSpdZ;

	private TextBox tbDistZ;

	private TextBox tbSpdY;

	private TextBox tbDistY;

	private TextBox tbSpdX;

	private TextBox tbDistX;

	private Button bnParaStopC;

	private Button bnEmStopC;

	private Button bnParaStopB;

	private Button bnEmStopB;

	private Button bnParaStopA;

	private Button bnEmStopA;

	private Button bnParaStopZ;

	private Button bnEmStopZ;

	private Button bnParaStopY;

	private Button bnEmStopY;

	private Button bnParaStopX;

	private Button bnEmStopX;

	private Button bnParaStopAll;

	private Button bnEmStopAll;

	private Button CNeg;

	private Button CPos;

	private Button BNeg;

	private Button BPos;

	private Button TNeg;

	private Button TPos;

	private Button ZNeg;

	private Button ZPos;

	private Button XNeg;

	private Button YNeg;

	private Button YPos;

	private Button XPos;

	private RadioButton rbMotionRel;

	private Button bnMoveAllEach;

	private Button bnResetPosC;

	private Button bnResetPosB;

	private Button bnResetPosA;

	private Button bnResetPosZ;

	private Button bnResetPosY;

	private Button bnResetPosX;

	private Button button1;

	private CheckBox cbSyncExInput;

	private Label label1;

	private TextBox tbAccC;

	private TextBox tbAccB;

	private TextBox tbAccA;

	private TextBox tbAccZ;

	private TextBox tbAccY;

	private TextBox tbAccX;

	private Button bnGrpParaStop;

	private Button bnGrpEmStop;

	private Label label3;

	private TextBox tbHomSttC;

	private TextBox tbHomSttB;

	private TextBox tbHomSttA;

	private TextBox tbHomSttZ;

	private TextBox tbHomSttY;

	private TextBox tbHomSttX;

	private RadioButton rbMotionHome;

	private Timer timer1;

	public ManualOpEx()
	{
		InitializeComponent();
	}

	private void bnEmStop_Click(object sender, EventArgs e)
	{
		pMainFrm.Mccard.MoCtrCard_EmergencyStopAxisMov(byte.MaxValue);
	}

	private void rbMotionAbs_CheckedChanged(object sender, EventArgs e)
	{
		if (rbMotionAbs.Checked)
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModePos;
		}
		else
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModeDist;
		}
		((Control)cbAxisChooseX).Enabled = true;
		((Control)cbAxisChooseY).Enabled = true;
		((Control)cbAxisChooseZ).Enabled = true;
		((Control)cbAxisChooseA).Enabled = true;
		((Control)cbAxisChooseB).Enabled = true;
		((Control)cbAxisChooseC).Enabled = true;
		((Control)tbDistX).Enabled = true;
		((Control)tbDistY).Enabled = true;
		((Control)tbDistZ).Enabled = true;
		((Control)tbDistA).Enabled = true;
		((Control)tbDistB).Enabled = true;
		((Control)tbDistC).Enabled = true;
		((Control)XNeg).Enabled = false;
		((Control)YNeg).Enabled = false;
		((Control)ZNeg).Enabled = false;
		((Control)TNeg).Enabled = false;
		((Control)BNeg).Enabled = false;
		((Control)CNeg).Enabled = false;
		timer1.Enabled = false;
	}

	private void rbMotionRel_CheckedChanged(object sender, EventArgs e)
	{
		if (rbMotionRel.Checked)
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModeDist;
		}
		else
		{
			((Control)lbDist).Text = GlobalData.GlobalLanguage.ManualOpExFrm_CommandModePos;
		}
		((Control)tbDistX).Enabled = true;
		((Control)tbDistY).Enabled = true;
		((Control)tbDistZ).Enabled = true;
		((Control)tbDistA).Enabled = true;
		((Control)tbDistB).Enabled = true;
		((Control)tbDistC).Enabled = true;
		((Control)XNeg).Enabled = true;
		((Control)YNeg).Enabled = true;
		((Control)ZNeg).Enabled = true;
		((Control)TNeg).Enabled = true;
		((Control)BNeg).Enabled = true;
		((Control)CNeg).Enabled = true;
		timer1.Enabled = false;
	}

	private void rbMotionSpd_CheckedChanged(object sender, EventArgs e)
	{
		((Control)tbDistX).Enabled = true;
		((Control)tbDistY).Enabled = true;
		((Control)tbDistZ).Enabled = true;
		((Control)tbDistA).Enabled = true;
		((Control)tbDistB).Enabled = true;
		((Control)tbDistC).Enabled = true;
		((Control)XNeg).Enabled = true;
		((Control)YNeg).Enabled = true;
		((Control)ZNeg).Enabled = true;
		((Control)TNeg).Enabled = true;
		((Control)BNeg).Enabled = true;
		((Control)CNeg).Enabled = true;
		timer1.Enabled = false;
	}

	private void rbMotionJog_CheckedChanged(object sender, EventArgs e)
	{
		((Control)tbDistX).Enabled = false;
		((Control)tbDistY).Enabled = false;
		((Control)tbDistZ).Enabled = false;
		((Control)tbDistA).Enabled = false;
		((Control)tbDistB).Enabled = false;
		((Control)tbDistC).Enabled = false;
		((Control)XNeg).Enabled = true;
		((Control)YNeg).Enabled = true;
		((Control)ZNeg).Enabled = true;
		((Control)TNeg).Enabled = true;
		((Control)BNeg).Enabled = true;
		((Control)CNeg).Enabled = true;
		timer1.Enabled = false;
	}

	private void ManualOpEx_Load(object sender, EventArgs e)
	{
		pMainFrm = (MainFrm)(object)((Form)this).Owner;
		cbAxisChooseX.Checked = false;
		cbAxisChooseY.Checked = false;
		cbAxisChooseZ.Checked = false;
		cbAxisChooseA.Checked = false;
		cbAxisChooseB.Checked = false;
		cbAxisChooseC.Checked = false;
		rbMotionRel.Checked = true;
		((Control)tbDistX).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseXAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistY).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseYAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistZ).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseZAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistA).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseAAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistB).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseBAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbDistC).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseCAxis + pMainFrm.Mccard.ParaIndex_ManDist].ToString();
		((Control)tbSpdX).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseXAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdY).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseYAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdZ).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseZAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdA).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseAAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdB).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseBAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbSpdC).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseCAxis + pMainFrm.Mccard.ParaIndex_HighSpd].ToString();
		((Control)tbAccX).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseXAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
		((Control)tbAccY).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseYAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
		((Control)tbAccZ).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseZAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
		((Control)tbAccA).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseAAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
		((Control)tbAccB).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseBAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
		((Control)tbAccC).Text = pMainFrm.SysPara[pMainFrm.Mccard.ParaIndex_BaseCAxis + pMainFrm.Mccard.ParaIndex_Acc].ToString();
	}

	private void XPos_MouseDown(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		float fPos = 0f;
		float num = 0f;
		float fAcc = 0f;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)XPos).Name || ((Control)val).Name == ((Control)XNeg).Name)
			{
				if (((Control)val).Name == ((Control)XNeg).Name)
				{
					fPos = 0f - float.Parse(((Control)tbDistX).Text);
					num = 0f - float.Parse(((Control)tbSpdX).Text);
				}
				else
				{
					fPos = float.Parse(((Control)tbDistX).Text);
					num = float.Parse(((Control)tbSpdX).Text);
				}
				fAcc = float.Parse(((Control)tbAccX).Text);
				axisId = 0;
			}
			else if (((Control)val).Name == ((Control)YPos).Name || ((Control)val).Name == ((Control)YNeg).Name)
			{
				if (((Control)val).Name == ((Control)YPos).Name)
				{
					fPos = float.Parse(((Control)tbDistY).Text);
					num = float.Parse(((Control)tbSpdY).Text);
				}
				else
				{
					fPos = 0f - float.Parse(((Control)tbDistY).Text);
					num = 0f - float.Parse(((Control)tbSpdY).Text);
				}
				fAcc = float.Parse(((Control)tbAccY).Text);
				axisId = 1;
			}
			else if (((Control)val).Name == ((Control)ZPos).Name || ((Control)val).Name == ((Control)ZNeg).Name)
			{
				if (((Control)val).Name == ((Control)ZPos).Name)
				{
					fPos = float.Parse(((Control)tbDistZ).Text);
					num = float.Parse(((Control)tbSpdZ).Text);
				}
				else
				{
					fPos = 0f - float.Parse(((Control)tbDistZ).Text);
					num = 0f - float.Parse(((Control)tbSpdZ).Text);
				}
				fAcc = float.Parse(((Control)tbAccZ).Text);
				axisId = 2;
			}
			else if (((Control)val).Name == ((Control)TPos).Name || ((Control)val).Name == ((Control)TNeg).Name)
			{
				if (((Control)val).Name == ((Control)TPos).Name)
				{
					fPos = float.Parse(((Control)tbDistA).Text);
					num = float.Parse(((Control)tbSpdA).Text);
				}
				else
				{
					fPos = 0f - float.Parse(((Control)tbDistA).Text);
					num = 0f - float.Parse(((Control)tbSpdA).Text);
				}
				fAcc = float.Parse(((Control)tbAccA).Text);
				axisId = 3;
			}
			else if (((Control)val).Name == ((Control)BPos).Name || ((Control)val).Name == ((Control)BNeg).Name)
			{
				if (((Control)val).Name == ((Control)BPos).Name)
				{
					fPos = float.Parse(((Control)tbDistB).Text);
					num = float.Parse(((Control)tbSpdB).Text);
				}
				else
				{
					fPos = 0f - float.Parse(((Control)tbDistB).Text);
					num = 0f - float.Parse(((Control)tbSpdB).Text);
				}
				fAcc = float.Parse(((Control)tbAccB).Text);
				axisId = 4;
			}
			else if (((Control)val).Name == ((Control)CPos).Name || ((Control)val).Name == ((Control)CNeg).Name)
			{
				if (((Control)val).Name == ((Control)CPos).Name)
				{
					fPos = float.Parse(((Control)tbDistC).Text);
					num = float.Parse(((Control)tbSpdC).Text);
				}
				else
				{
					fPos = 0f - float.Parse(((Control)tbDistC).Text);
					num = 0f - float.Parse(((Control)tbSpdC).Text);
				}
				fAcc = float.Parse(((Control)tbAccC).Text);
				axisId = 5;
			}
			if (rbMotionAbs.Checked)
			{
				if (cbSyncExInput.Checked)
				{
					pMainFrm.Mccard.MoCtrCard_MCrlAxisAbsMoveSyncInput(axisId, fPos, Math.Abs(num), fAcc);
				}
				else
				{
					pMainFrm.Mccard.MoCtrCard_MCrlAxisAbsMove(axisId, fPos, Math.Abs(num), fAcc);
				}
			}
			else if (rbMotionRel.Checked)
			{
				if (cbSyncExInput.Checked)
				{
					pMainFrm.Mccard.MoCtrCard_MCrlAxisRelMoveSyncInput(axisId, fPos, Math.Abs(num), fAcc);
				}
				else
				{
					pMainFrm.Mccard.MoCtrCard_MCrlAxisRelMove(axisId, fPos, Math.Abs(num), fAcc);
				}
			}
			else if (rbMotionSpd.Checked || rbMotionJog.Checked)
			{
				pMainFrm.Mccard.MoCtrCard_MCrlAxisMoveAtSpd(axisId, num, fAcc);
			}
			else if (rbMotionHome.Checked)
			{
				pMainFrm.Mccard.MoCtrCard_SeekZero(axisId, num, fAcc);
			}
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void XPos_MouseUp(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)XPos).Name || ((Control)val).Name == ((Control)XNeg).Name)
			{
				axisId = 0;
			}
			else if (((Control)val).Name == ((Control)YPos).Name || ((Control)val).Name == ((Control)YNeg).Name)
			{
				axisId = 1;
			}
			else if (((Control)val).Name == ((Control)ZPos).Name || ((Control)val).Name == ((Control)ZNeg).Name)
			{
				axisId = 2;
			}
			else if (((Control)val).Name == ((Control)TPos).Name || ((Control)val).Name == ((Control)TNeg).Name)
			{
				axisId = 3;
			}
			else if (((Control)val).Name == ((Control)BPos).Name || ((Control)val).Name == ((Control)BNeg).Name)
			{
				axisId = 4;
			}
			else if (((Control)val).Name == ((Control)CPos).Name || ((Control)val).Name == ((Control)CNeg).Name)
			{
				axisId = 5;
			}
			if (rbMotionJog.Checked)
			{
				pMainFrm.Mccard.MoCtrCard_ParaStopAxisMov(axisId, 0f);
			}
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void bnEmStopX_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)bnEmStopX).Name)
			{
				axisId = 0;
			}
			else if (((Control)val).Name == ((Control)bnEmStopY).Name)
			{
				axisId = 1;
			}
			else if (((Control)val).Name == ((Control)bnEmStopZ).Name)
			{
				axisId = 2;
			}
			else if (((Control)val).Name == ((Control)bnEmStopA).Name)
			{
				axisId = 3;
			}
			else if (((Control)val).Name == ((Control)bnEmStopB).Name)
			{
				axisId = 4;
			}
			else if (((Control)val).Name == ((Control)bnEmStopC).Name)
			{
				axisId = 5;
			}
			pMainFrm.Mccard.MoCtrCard_EmergencyStopAxisMov(axisId);
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void bnParaStopC_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		float fAcc = 0f;
		byte axisId = 0;
		try
		{
			if (((Control)val).Name == ((Control)bnParaStopX).Name)
			{
				axisId = 0;
				fAcc = float.Parse(((Control)tbAccX).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopY).Name)
			{
				axisId = 1;
				fAcc = float.Parse(((Control)tbAccY).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopZ).Name)
			{
				axisId = 2;
				fAcc = float.Parse(((Control)tbAccZ).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopA).Name)
			{
				axisId = 3;
				fAcc = float.Parse(((Control)tbAccA).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopB).Name)
			{
				axisId = 4;
				fAcc = float.Parse(((Control)tbAccB).Text);
			}
			else if (((Control)val).Name == ((Control)bnParaStopC).Name)
			{
				axisId = 5;
				fAcc = float.Parse(((Control)tbAccC).Text);
			}
			pMainFrm.Mccard.MoCtrCard_ParaStopAxisMov(axisId, fAcc);
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void bnParaStopAll_Click(object sender, EventArgs e)
	{
		pMainFrm.Mccard.MoCtrCard_ParaStopAxisMov(byte.MaxValue, 0f);
	}

	private void bnMoveAll_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		if (rbMotionJog.Checked)
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_NotSupportGroupMoveInJogMode);
		}
		else if (rbMotionAbs.Checked || rbMotionRel.Checked)
		{
			if (cbAxisChooseX.Checked || cbAxisChooseY.Checked || cbAxisChooseZ.Checked || cbAxisChooseA.Checked || cbAxisChooseB.Checked || cbAxisChooseC.Checked)
			{
				try
				{
					byte[] array = new byte[6];
					float[] array2 = new float[6];
					float[] array3 = new float[6];
					float[] array4 = new float[6];
					if (cbAxisChooseX.Checked)
					{
						array[0] = 1;
					}
					array2[0] = float.Parse(((Control)tbDistX).Text);
					array3[0] = float.Parse(((Control)tbSpdX).Text);
					array4[0] = float.Parse(((Control)tbAccX).Text);
					if (cbAxisChooseY.Checked)
					{
						array[1] = 1;
					}
					array2[1] = float.Parse(((Control)tbDistY).Text);
					array3[1] = float.Parse(((Control)tbSpdY).Text);
					array4[1] = float.Parse(((Control)tbAccY).Text);
					if (cbAxisChooseZ.Checked)
					{
						array[2] = 1;
					}
					array2[2] = float.Parse(((Control)tbDistZ).Text);
					array3[2] = float.Parse(((Control)tbSpdZ).Text);
					array4[2] = float.Parse(((Control)tbAccZ).Text);
					if (cbAxisChooseA.Checked)
					{
						array[3] = 1;
					}
					array2[3] = float.Parse(((Control)tbDistA).Text);
					array3[3] = float.Parse(((Control)tbSpdA).Text);
					array4[3] = float.Parse(((Control)tbAccA).Text);
					if (cbAxisChooseB.Checked)
					{
						array[4] = 1;
					}
					array2[4] = float.Parse(((Control)tbDistB).Text);
					array3[4] = float.Parse(((Control)tbSpdB).Text);
					array4[4] = float.Parse(((Control)tbAccB).Text);
					if (cbAxisChooseC.Checked)
					{
						array[5] = 1;
					}
					array2[5] = float.Parse(((Control)tbDistC).Text);
					array3[5] = float.Parse(((Control)tbSpdC).Text);
					array4[5] = float.Parse(((Control)tbAccC).Text);
					if (rbMotionAbs.Checked)
					{
						if (((Control)val).Name == ((Control)bnMoveAll).Name)
						{
							if (cbSyncExInput.Checked)
							{
								pMainFrm.Mccard.MoCtrCard_MCrlGroupAbsMoveSyncInput(array, array2, array3, array4);
							}
							else
							{
								pMainFrm.Mccard.MoCtrCard_MCrlGroupAbsMove(array, array2, array3, array4);
							}
						}
						else if (cbSyncExInput.Checked)
						{
							pMainFrm.Mccard.MoCtrCard_MCrlGroupAbsMovePTPSyncInput(array, array2, array3, array4);
						}
						else
						{
							pMainFrm.Mccard.MoCtrCard_MCrlGroupAbsMovePTP(array, array2, array3, array4);
						}
					}
					else if (((Control)val).Name == ((Control)bnMoveAll).Name)
					{
						if (cbSyncExInput.Checked)
						{
							pMainFrm.Mccard.MoCtrCard_MCrlGroupRelMoveSyncInput(array, array2, array3, array4);
						}
						else
						{
							pMainFrm.Mccard.MoCtrCard_MCrlGroupRelMove(array, array2, array3, array4);
						}
					}
					else if (cbSyncExInput.Checked)
					{
						pMainFrm.Mccard.MoCtrCard_MCrlGroupRelMovePTPSyncInput(array, array2, array3, array4);
					}
					else
					{
						pMainFrm.Mccard.MoCtrCard_MCrlGroupRelMovePTP(array, array2, array3, array4);
					}
					return;
				}
				catch
				{
					pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
					return;
				}
			}
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_NotChooseAxisInGroupMode);
		}
		else
		{
			if (!rbMotionSpd.Checked)
			{
				return;
			}
			if (cbAxisChooseX.Checked || cbAxisChooseY.Checked || cbAxisChooseZ.Checked || cbAxisChooseA.Checked || cbAxisChooseB.Checked || cbAxisChooseC.Checked)
			{
				try
				{
					byte[] array5 = new byte[6];
					float[] array6 = new float[6];
					float[] array7 = new float[6];
					float[] array8 = new float[6];
					if (cbAxisChooseX.Checked)
					{
						array5[0] = 1;
						array6[0] = float.Parse(((Control)tbDistX).Text);
						array7[0] = float.Parse(((Control)tbSpdX).Text);
						array8[0] = float.Parse(((Control)tbAccX).Text);
					}
					if (cbAxisChooseY.Checked)
					{
						array5[1] = 1;
						array6[1] = float.Parse(((Control)tbDistY).Text);
						array7[1] = float.Parse(((Control)tbSpdY).Text);
						array8[1] = float.Parse(((Control)tbAccY).Text);
					}
					if (cbAxisChooseZ.Checked)
					{
						array5[2] = 1;
						array6[2] = float.Parse(((Control)tbDistZ).Text);
						array7[2] = float.Parse(((Control)tbSpdZ).Text);
						array8[2] = float.Parse(((Control)tbAccZ).Text);
					}
					if (cbAxisChooseA.Checked)
					{
						array5[3] = 1;
						array6[3] = float.Parse(((Control)tbDistA).Text);
						array7[3] = float.Parse(((Control)tbSpdA).Text);
						array8[3] = float.Parse(((Control)tbAccA).Text);
					}
					if (cbAxisChooseB.Checked)
					{
						array5[4] = 1;
						array6[4] = float.Parse(((Control)tbDistB).Text);
						array7[4] = float.Parse(((Control)tbSpdB).Text);
						array8[4] = float.Parse(((Control)tbAccB).Text);
					}
					if (cbAxisChooseC.Checked)
					{
						array5[5] = 1;
						array6[5] = float.Parse(((Control)tbDistC).Text);
						array7[5] = float.Parse(((Control)tbSpdC).Text);
						array8[5] = float.Parse(((Control)tbAccC).Text);
					}
					pMainFrm.Mccard.MoCtrCard_MCrlGroupSpdMove(array5, array7, array8);
					return;
				}
				catch
				{
					pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
					return;
				}
			}
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_NotChooseAxisInGroupMode);
		}
	}

	private void bnResetPosX_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Button val = (Button)sender;
		byte axisId = 0;
		float restPos = 0f;
		if (((Control)val).Name == ((Control)bnResetPosX).Name)
		{
			axisId = 0;
			restPos = float.Parse(((Control)tbDistX).Text);
		}
		else if (((Control)val).Name == ((Control)bnResetPosY).Name)
		{
			axisId = 1;
			restPos = float.Parse(((Control)tbDistY).Text);
		}
		else if (((Control)val).Name == ((Control)bnResetPosZ).Name)
		{
			axisId = 2;
			restPos = float.Parse(((Control)tbDistZ).Text);
		}
		else if (((Control)val).Name == ((Control)bnResetPosA).Name)
		{
			axisId = 3;
			restPos = float.Parse(((Control)tbDistA).Text);
		}
		else if (((Control)val).Name == ((Control)bnResetPosB).Name)
		{
			axisId = 4;
			restPos = float.Parse(((Control)tbDistB).Text);
		}
		else if (((Control)val).Name == ((Control)bnResetPosC).Name)
		{
			axisId = 5;
			restPos = float.Parse(((Control)tbDistC).Text);
		}
		pMainFrm.Mccard.MoCtrCard_ResetCoordinate(axisId, restPos);
	}

	private void button1_Click(object sender, EventArgs e)
	{
		pMainFrm.Mccard.MoCtrCard_ResetCoordinate(byte.MaxValue, 0f);
	}

	private void button3_Click(object sender, EventArgs e)
	{
		float[] array = new float[pMainFrm.Mccard.MaxAxisNum];
		try
		{
			array[0] = float.Parse(((Control)tbAccX).Text);
			array[1] = float.Parse(((Control)tbAccY).Text);
			array[2] = float.Parse(((Control)tbAccZ).Text);
			array[3] = float.Parse(((Control)tbAccA).Text);
			array[4] = float.Parse(((Control)tbAccB).Text);
			array[5] = float.Parse(((Control)tbAccC).Text);
			pMainFrm.Mccard.MoCtrCard_MCtrlGroupParaStop(array);
		}
		catch
		{
			pMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.ManualOpExFrm_MoveParameterError);
		}
	}

	private void button2_Click(object sender, EventArgs e)
	{
		pMainFrm.Mccard.MoCtrCard_QuiteMotionControl(byte.MaxValue);
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (rbMotionHome.Checked)
		{
			uint[] array = new uint[2];
			if (pMainFrm.Mccard.FunResOk == pMainFrm.Mccard.MoCtrCard_GetHomeState(array))
			{
				((Control)tbHomSttX).Text = (array[1] & 0xFF).ToString();
				((Control)tbHomSttY).Text = ((array[1] >> 8) & 0xFF).ToString();
				((Control)tbHomSttZ).Text = (array[0] & 0xFF).ToString();
				((Control)tbHomSttA).Text = ((array[0] >> 8) & 0xFF).ToString();
				((Control)tbHomSttB).Text = ((array[0] >> 16) & 0xFF).ToString();
				((Control)tbHomSttC).Text = ((array[0] >> 24) & 0xFF).ToString();
			}
		}
	}

	private void rbMotionHome_CheckedChanged(object sender, EventArgs e)
	{
		timer1.Interval = 200;
		timer1.Enabled = true;
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
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Expected O, but got Unknown
		//IL_17a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b0: Expected O, but got Unknown
		//IL_17bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c7: Expected O, but got Unknown
		//IL_1801: Unknown result type (might be due to invalid IL or missing references)
		//IL_180b: Expected O, but got Unknown
		//IL_1818: Unknown result type (might be due to invalid IL or missing references)
		//IL_1822: Expected O, but got Unknown
		//IL_185c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1866: Expected O, but got Unknown
		//IL_1873: Unknown result type (might be due to invalid IL or missing references)
		//IL_187d: Expected O, but got Unknown
		//IL_18b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c1: Expected O, but got Unknown
		//IL_18ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d8: Expected O, but got Unknown
		//IL_1912: Unknown result type (might be due to invalid IL or missing references)
		//IL_191c: Expected O, but got Unknown
		//IL_1929: Unknown result type (might be due to invalid IL or missing references)
		//IL_1933: Expected O, but got Unknown
		//IL_196d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1977: Expected O, but got Unknown
		//IL_1984: Unknown result type (might be due to invalid IL or missing references)
		//IL_198e: Expected O, but got Unknown
		//IL_19c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d2: Expected O, but got Unknown
		//IL_19df: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e9: Expected O, but got Unknown
		//IL_1a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2d: Expected O, but got Unknown
		//IL_1a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a44: Expected O, but got Unknown
		//IL_1a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a88: Expected O, but got Unknown
		//IL_1a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9f: Expected O, but got Unknown
		//IL_1ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae3: Expected O, but got Unknown
		//IL_1af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afa: Expected O, but got Unknown
		//IL_1b34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3e: Expected O, but got Unknown
		//IL_1b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b55: Expected O, but got Unknown
		//IL_1b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b99: Expected O, but got Unknown
		//IL_1ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb0: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ManualOpEx));
		groupBox1 = new GroupBox();
		label3 = new Label();
		tbHomSttC = new TextBox();
		tbHomSttB = new TextBox();
		tbHomSttA = new TextBox();
		tbHomSttZ = new TextBox();
		tbHomSttY = new TextBox();
		tbHomSttX = new TextBox();
		rbMotionHome = new RadioButton();
		bnGrpParaStop = new Button();
		bnGrpEmStop = new Button();
		label1 = new Label();
		tbAccC = new TextBox();
		tbAccB = new TextBox();
		tbAccA = new TextBox();
		tbAccZ = new TextBox();
		tbAccY = new TextBox();
		tbAccX = new TextBox();
		cbSyncExInput = new CheckBox();
		button1 = new Button();
		bnResetPosC = new Button();
		bnResetPosB = new Button();
		bnResetPosA = new Button();
		bnResetPosZ = new Button();
		bnResetPosY = new Button();
		bnResetPosX = new Button();
		bnMoveAllEach = new Button();
		label2 = new Label();
		lbDist = new Label();
		rbMotionJog = new RadioButton();
		rbMotionSpd = new RadioButton();
		bnMoveAll = new Button();
		rbMotionRel = new RadioButton();
		rbMotionAbs = new RadioButton();
		cbAxisChooseC = new CheckBox();
		cbAxisChooseB = new CheckBox();
		cbAxisChooseA = new CheckBox();
		cbAxisChooseZ = new CheckBox();
		cbAxisChooseY = new CheckBox();
		cbAxisChooseX = new CheckBox();
		tbSpdC = new TextBox();
		tbDistC = new TextBox();
		tbSpdB = new TextBox();
		tbDistB = new TextBox();
		tbSpdA = new TextBox();
		tbDistA = new TextBox();
		tbSpdZ = new TextBox();
		tbDistZ = new TextBox();
		tbSpdY = new TextBox();
		tbDistY = new TextBox();
		tbSpdX = new TextBox();
		tbDistX = new TextBox();
		bnParaStopC = new Button();
		bnEmStopC = new Button();
		bnParaStopB = new Button();
		bnEmStopB = new Button();
		bnParaStopA = new Button();
		bnEmStopA = new Button();
		bnParaStopZ = new Button();
		bnEmStopZ = new Button();
		bnParaStopY = new Button();
		bnEmStopY = new Button();
		bnParaStopX = new Button();
		bnEmStopX = new Button();
		bnParaStopAll = new Button();
		bnEmStopAll = new Button();
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
		timer1 = new Timer(components);
		((Control)groupBox1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)label3);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttC);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttB);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttA);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttY);
		((Control)groupBox1).Controls.Add((Control)(object)tbHomSttX);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionHome);
		((Control)groupBox1).Controls.Add((Control)(object)bnGrpParaStop);
		((Control)groupBox1).Controls.Add((Control)(object)bnGrpEmStop);
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccC);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccB);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccA);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccY);
		((Control)groupBox1).Controls.Add((Control)(object)tbAccX);
		((Control)groupBox1).Controls.Add((Control)(object)cbSyncExInput);
		((Control)groupBox1).Controls.Add((Control)(object)button1);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosC);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosB);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosA);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosY);
		((Control)groupBox1).Controls.Add((Control)(object)bnResetPosX);
		((Control)groupBox1).Controls.Add((Control)(object)bnMoveAllEach);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)lbDist);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionJog);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionSpd);
		((Control)groupBox1).Controls.Add((Control)(object)bnMoveAll);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionRel);
		((Control)groupBox1).Controls.Add((Control)(object)rbMotionAbs);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseC);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseB);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseA);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseZ);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseY);
		((Control)groupBox1).Controls.Add((Control)(object)cbAxisChooseX);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdC);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistC);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdB);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistB);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdA);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistA);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistZ);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdY);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistY);
		((Control)groupBox1).Controls.Add((Control)(object)tbSpdX);
		((Control)groupBox1).Controls.Add((Control)(object)tbDistX);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopC);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopC);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopB);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopB);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopA);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopA);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopZ);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopY);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopY);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopX);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopX);
		((Control)groupBox1).Controls.Add((Control)(object)bnParaStopAll);
		((Control)groupBox1).Controls.Add((Control)(object)bnEmStopAll);
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
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(tbHomSttC, "tbHomSttC");
		((Control)tbHomSttC).Name = "tbHomSttC";
		componentResourceManager.ApplyResources(tbHomSttB, "tbHomSttB");
		((Control)tbHomSttB).Name = "tbHomSttB";
		componentResourceManager.ApplyResources(tbHomSttA, "tbHomSttA");
		((Control)tbHomSttA).Name = "tbHomSttA";
		componentResourceManager.ApplyResources(tbHomSttZ, "tbHomSttZ");
		((Control)tbHomSttZ).Name = "tbHomSttZ";
		componentResourceManager.ApplyResources(tbHomSttY, "tbHomSttY");
		((Control)tbHomSttY).Name = "tbHomSttY";
		componentResourceManager.ApplyResources(tbHomSttX, "tbHomSttX");
		((Control)tbHomSttX).Name = "tbHomSttX";
		componentResourceManager.ApplyResources(rbMotionHome, "rbMotionHome");
		((Control)rbMotionHome).Name = "rbMotionHome";
		rbMotionHome.TabStop = true;
		((ButtonBase)rbMotionHome).UseVisualStyleBackColor = true;
		rbMotionHome.CheckedChanged += rbMotionHome_CheckedChanged;
		componentResourceManager.ApplyResources(bnGrpParaStop, "bnGrpParaStop");
		((Control)bnGrpParaStop).Name = "bnGrpParaStop";
		((ButtonBase)bnGrpParaStop).UseVisualStyleBackColor = true;
		((Control)bnGrpParaStop).Click += button3_Click;
		componentResourceManager.ApplyResources(bnGrpEmStop, "bnGrpEmStop");
		((Control)bnGrpEmStop).Name = "bnGrpEmStop";
		((ButtonBase)bnGrpEmStop).UseVisualStyleBackColor = true;
		((Control)bnGrpEmStop).Click += button2_Click;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbAccC, "tbAccC");
		((Control)tbAccC).Name = "tbAccC";
		componentResourceManager.ApplyResources(tbAccB, "tbAccB");
		((Control)tbAccB).Name = "tbAccB";
		componentResourceManager.ApplyResources(tbAccA, "tbAccA");
		((Control)tbAccA).Name = "tbAccA";
		componentResourceManager.ApplyResources(tbAccZ, "tbAccZ");
		((Control)tbAccZ).Name = "tbAccZ";
		componentResourceManager.ApplyResources(tbAccY, "tbAccY");
		((Control)tbAccY).Name = "tbAccY";
		componentResourceManager.ApplyResources(tbAccX, "tbAccX");
		((Control)tbAccX).Name = "tbAccX";
		componentResourceManager.ApplyResources(cbSyncExInput, "cbSyncExInput");
		((Control)cbSyncExInput).Name = "cbSyncExInput";
		((ButtonBase)cbSyncExInput).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(button1, "button1");
		((Control)button1).Name = "button1";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button1).Click += button1_Click;
		componentResourceManager.ApplyResources(bnResetPosC, "bnResetPosC");
		((Control)bnResetPosC).Name = "bnResetPosC";
		((ButtonBase)bnResetPosC).UseVisualStyleBackColor = true;
		((Control)bnResetPosC).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnResetPosB, "bnResetPosB");
		((Control)bnResetPosB).Name = "bnResetPosB";
		((ButtonBase)bnResetPosB).UseVisualStyleBackColor = true;
		((Control)bnResetPosB).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnResetPosA, "bnResetPosA");
		((Control)bnResetPosA).Name = "bnResetPosA";
		((ButtonBase)bnResetPosA).UseVisualStyleBackColor = true;
		((Control)bnResetPosA).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnResetPosZ, "bnResetPosZ");
		((Control)bnResetPosZ).Name = "bnResetPosZ";
		((ButtonBase)bnResetPosZ).UseVisualStyleBackColor = true;
		((Control)bnResetPosZ).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnResetPosY, "bnResetPosY");
		((Control)bnResetPosY).Name = "bnResetPosY";
		((ButtonBase)bnResetPosY).UseVisualStyleBackColor = true;
		((Control)bnResetPosY).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnResetPosX, "bnResetPosX");
		((Control)bnResetPosX).Name = "bnResetPosX";
		((ButtonBase)bnResetPosX).UseVisualStyleBackColor = true;
		((Control)bnResetPosX).Click += bnResetPosX_Click;
		componentResourceManager.ApplyResources(bnMoveAllEach, "bnMoveAllEach");
		((Control)bnMoveAllEach).Name = "bnMoveAllEach";
		((ButtonBase)bnMoveAllEach).UseVisualStyleBackColor = true;
		((Control)bnMoveAllEach).Click += bnMoveAll_Click;
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(lbDist, "lbDist");
		((Control)lbDist).Name = "lbDist";
		componentResourceManager.ApplyResources(rbMotionJog, "rbMotionJog");
		((Control)rbMotionJog).Name = "rbMotionJog";
		rbMotionJog.TabStop = true;
		((ButtonBase)rbMotionJog).UseVisualStyleBackColor = true;
		rbMotionJog.CheckedChanged += rbMotionJog_CheckedChanged;
		componentResourceManager.ApplyResources(rbMotionSpd, "rbMotionSpd");
		((Control)rbMotionSpd).Name = "rbMotionSpd";
		rbMotionSpd.TabStop = true;
		((ButtonBase)rbMotionSpd).UseVisualStyleBackColor = true;
		rbMotionSpd.CheckedChanged += rbMotionSpd_CheckedChanged;
		componentResourceManager.ApplyResources(bnMoveAll, "bnMoveAll");
		((Control)bnMoveAll).Name = "bnMoveAll";
		((ButtonBase)bnMoveAll).UseVisualStyleBackColor = true;
		((Control)bnMoveAll).Click += bnMoveAll_Click;
		componentResourceManager.ApplyResources(rbMotionRel, "rbMotionRel");
		((Control)rbMotionRel).Name = "rbMotionRel";
		rbMotionRel.TabStop = true;
		((ButtonBase)rbMotionRel).UseVisualStyleBackColor = true;
		rbMotionRel.CheckedChanged += rbMotionRel_CheckedChanged;
		componentResourceManager.ApplyResources(rbMotionAbs, "rbMotionAbs");
		((Control)rbMotionAbs).Name = "rbMotionAbs";
		rbMotionAbs.TabStop = true;
		((ButtonBase)rbMotionAbs).UseVisualStyleBackColor = true;
		rbMotionAbs.CheckedChanged += rbMotionAbs_CheckedChanged;
		componentResourceManager.ApplyResources(cbAxisChooseC, "cbAxisChooseC");
		((Control)cbAxisChooseC).Name = "cbAxisChooseC";
		((ButtonBase)cbAxisChooseC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbAxisChooseB, "cbAxisChooseB");
		((Control)cbAxisChooseB).Name = "cbAxisChooseB";
		((ButtonBase)cbAxisChooseB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbAxisChooseA, "cbAxisChooseA");
		((Control)cbAxisChooseA).Name = "cbAxisChooseA";
		((ButtonBase)cbAxisChooseA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbAxisChooseZ, "cbAxisChooseZ");
		((Control)cbAxisChooseZ).Name = "cbAxisChooseZ";
		((ButtonBase)cbAxisChooseZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbAxisChooseY, "cbAxisChooseY");
		((Control)cbAxisChooseY).Name = "cbAxisChooseY";
		((ButtonBase)cbAxisChooseY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(cbAxisChooseX, "cbAxisChooseX");
		((Control)cbAxisChooseX).Name = "cbAxisChooseX";
		((ButtonBase)cbAxisChooseX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tbSpdC, "tbSpdC");
		((Control)tbSpdC).Name = "tbSpdC";
		componentResourceManager.ApplyResources(tbDistC, "tbDistC");
		((Control)tbDistC).Name = "tbDistC";
		componentResourceManager.ApplyResources(tbSpdB, "tbSpdB");
		((Control)tbSpdB).Name = "tbSpdB";
		componentResourceManager.ApplyResources(tbDistB, "tbDistB");
		((Control)tbDistB).Name = "tbDistB";
		componentResourceManager.ApplyResources(tbSpdA, "tbSpdA");
		((Control)tbSpdA).Name = "tbSpdA";
		componentResourceManager.ApplyResources(tbDistA, "tbDistA");
		((Control)tbDistA).Name = "tbDistA";
		componentResourceManager.ApplyResources(tbSpdZ, "tbSpdZ");
		((Control)tbSpdZ).Name = "tbSpdZ";
		componentResourceManager.ApplyResources(tbDistZ, "tbDistZ");
		((Control)tbDistZ).Name = "tbDistZ";
		componentResourceManager.ApplyResources(tbSpdY, "tbSpdY");
		((Control)tbSpdY).Name = "tbSpdY";
		componentResourceManager.ApplyResources(tbDistY, "tbDistY");
		((Control)tbDistY).Name = "tbDistY";
		componentResourceManager.ApplyResources(tbSpdX, "tbSpdX");
		((Control)tbSpdX).Name = "tbSpdX";
		componentResourceManager.ApplyResources(tbDistX, "tbDistX");
		((Control)tbDistX).Name = "tbDistX";
		componentResourceManager.ApplyResources(bnParaStopC, "bnParaStopC");
		((Control)bnParaStopC).Name = "bnParaStopC";
		((ButtonBase)bnParaStopC).UseVisualStyleBackColor = true;
		((Control)bnParaStopC).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopC, "bnEmStopC");
		((Control)bnEmStopC).Name = "bnEmStopC";
		((ButtonBase)bnEmStopC).UseVisualStyleBackColor = true;
		((Control)bnEmStopC).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopB, "bnParaStopB");
		((Control)bnParaStopB).Name = "bnParaStopB";
		((ButtonBase)bnParaStopB).UseVisualStyleBackColor = true;
		((Control)bnParaStopB).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopB, "bnEmStopB");
		((Control)bnEmStopB).Name = "bnEmStopB";
		((ButtonBase)bnEmStopB).UseVisualStyleBackColor = true;
		((Control)bnEmStopB).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopA, "bnParaStopA");
		((Control)bnParaStopA).Name = "bnParaStopA";
		((ButtonBase)bnParaStopA).UseVisualStyleBackColor = true;
		((Control)bnParaStopA).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopA, "bnEmStopA");
		((Control)bnEmStopA).Name = "bnEmStopA";
		((ButtonBase)bnEmStopA).UseVisualStyleBackColor = true;
		((Control)bnEmStopA).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopZ, "bnParaStopZ");
		((Control)bnParaStopZ).Name = "bnParaStopZ";
		((ButtonBase)bnParaStopZ).UseVisualStyleBackColor = true;
		((Control)bnParaStopZ).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopZ, "bnEmStopZ");
		((Control)bnEmStopZ).Name = "bnEmStopZ";
		((ButtonBase)bnEmStopZ).UseVisualStyleBackColor = true;
		((Control)bnEmStopZ).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopY, "bnParaStopY");
		((Control)bnParaStopY).Name = "bnParaStopY";
		((ButtonBase)bnParaStopY).UseVisualStyleBackColor = true;
		((Control)bnParaStopY).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopY, "bnEmStopY");
		((Control)bnEmStopY).Name = "bnEmStopY";
		((ButtonBase)bnEmStopY).UseVisualStyleBackColor = true;
		((Control)bnEmStopY).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopX, "bnParaStopX");
		((Control)bnParaStopX).Name = "bnParaStopX";
		((ButtonBase)bnParaStopX).UseVisualStyleBackColor = true;
		((Control)bnParaStopX).Click += bnParaStopC_Click;
		componentResourceManager.ApplyResources(bnEmStopX, "bnEmStopX");
		((Control)bnEmStopX).Name = "bnEmStopX";
		((ButtonBase)bnEmStopX).UseVisualStyleBackColor = true;
		((Control)bnEmStopX).Click += bnEmStopX_Click;
		componentResourceManager.ApplyResources(bnParaStopAll, "bnParaStopAll");
		((Control)bnParaStopAll).Name = "bnParaStopAll";
		((ButtonBase)bnParaStopAll).UseVisualStyleBackColor = true;
		((Control)bnParaStopAll).Click += bnParaStopAll_Click;
		componentResourceManager.ApplyResources(bnEmStopAll, "bnEmStopAll");
		((Control)bnEmStopAll).Name = "bnEmStopAll";
		((ButtonBase)bnEmStopAll).UseVisualStyleBackColor = true;
		((Control)bnEmStopAll).Click += bnEmStop_Click;
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
		timer1.Tick += timer1_Tick;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Name = "ManualOpEx";
		((Form)this).Load += ManualOpEx_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
