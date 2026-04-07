using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LinearAlgebra;
using MccCustomLanguage;
using ZedGraph;

namespace MCCDEMO;

public class CurveDefine : Form
{
	private struct stCurveData
	{
		public uint LinkType;

		public float PosStartMaster;

		public float PosStartSlave;

		public float PosEndMaster;

		public float PosEndSlave;

		public float VelStart;

		public float VelEnd;

		public float AccStart;

		public float AccEnd;
	}

	private struct stProfileMovePara
	{
		public uint nMaxCycleCnt;

		public uint nMaxSecCnt;

		public uint nPeriodType;

		public float Period;
	}

	public enum SectionLinkType
	{
		Polynomial1,
		Polynomial3,
		Polynomial5
	}

	private struct stProfileCoeType
	{
		public uint nLinkType;

		public float PosMaster;

		public float Constant;

		public float CoefficientOfLinear;

		public float CoefficientOfQuadric;

		public float CoefficientOfCube;

		public float CoefficientOfFourth;

		public float CoefficientOfFiveth;
	}

	private const ushort wMaxCurveRow = 10;

	private const ushort wMaxCurveCol = 8;

	private const ushort wMaxCurveSec = 10;

	private stCurveData[] stCurveDt = new stCurveData[10];

	private stProfileMovePara stProfilePara;

	private stProfileCoeType[] stProfileCoeDt = new stProfileCoeType[10];

	private IContainer components;

	private ZedGraphControl zedGraphControl1;

	private OpenFileDialog OpenCurveFileDialog;

	private float[,] fCurveData;

	private GraphPane mCurvePane;

	private PointPairList[] mCurvePointList;

	private LineItem[] mCurveItem;

	private MainFrm PMainFrm;

	private GroupBox groupBox1;

	private Label label1;

	private TextBox tbGearRatio;

	private GroupBox gbGearSlv;

	private RadioButton rbGearSlvC;

	private RadioButton rbGearSlvB;

	private RadioButton rbGearSlvA;

	private RadioButton rbGearSlvZ;

	private RadioButton rbGearSlvY;

	private RadioButton rbGearSlvX;

	private GroupBox gbGearMstr;

	private RadioButton rbGearMstrC;

	private RadioButton rbGearMstrB;

	private RadioButton rbGearMstrA;

	private RadioButton rbGearMstrZ;

	private RadioButton rbGearMstrY;

	private RadioButton rbGearMstrX;

	private Button bnStopGearMove;

	private Button bnStartGearMove;

	private Button bnSetGearPara;

	private GroupBox groupBox2;

	private Button bnStop;

	private Button bnStartNeg;

	private Button bnStartPos;

	private Button bnDownLoadCurve;

	private Button bnChooseFile;

	private Label lbInfo;

	private TabControl tabControl1;

	private TabPage tabPage1;

	private DataGridView dataGridViewCam;

	private TabPage tabPage2;

	private Button bnGenerateProfileDt;

	private TextBox tbWordCycle;

	private TextBox tbPeriod;

	private TextBox tbRowCnt;

	private Label label4;

	private Label label3;

	private Label label2;

	private Button bnSetProfilePara;

	private Label label5;

	private ComboBox cbProfileMoveAxis;

	private Label label6;

	private ComboBox cbProfilePeriodType;

	private Button bnSaveProfile;

	private SaveFileDialog saveFileDialog1;

	private DataGridViewComboBoxColumn FuncMode;

	private DataGridViewTextBoxColumn MasterStartPos;

	private DataGridViewTextBoxColumn SlaveStartPos;

	private DataGridViewTextBoxColumn SlaveStartVel;

	private DataGridViewTextBoxColumn SlaveStartAcc;

	private DataGridViewTextBoxColumn MasterEndPos;

	private DataGridViewTextBoxColumn SlaveEndPos;

	private DataGridViewTextBoxColumn SlaveEndVel;

	private DataGridViewTextBoxColumn SlaveEndAcc;

	public CurveDefine()
	{
		InitializeComponent();
	}

	private void zedGraphControl1_Load(object sender, EventArgs e)
	{
	}

	private float fun_cacCurveYVal(float[,] fCoef, float fXVal, ushort wRow)
	{
		float result = 0f;
		if (0.0 == (double)fCoef[wRow, 0])
		{
			result = fCoef[wRow, 2] + fCoef[wRow, 3] * fXVal;
		}
		else if (1.0 == (double)fCoef[wRow, 0])
		{
			float num = fXVal * fXVal;
			float num2 = num * fXVal;
			result = fCoef[wRow, 2] + fCoef[wRow, 3] * fXVal + fCoef[wRow, 4] * num + fCoef[wRow, 5] * num2;
		}
		else if (2.0 == (double)fCoef[wRow, 0])
		{
			float num3 = fXVal * fXVal;
			float num4 = num3 * fXVal;
			float num5 = num4 * fXVal;
			float num6 = num5 * fXVal;
			result = fCoef[wRow, 2] + fCoef[wRow, 3] * fXVal + fCoef[wRow, 4] * num3 + fCoef[wRow, 5] * num4 + fCoef[wRow, 6] * num5 + fCoef[wRow, 7] * num6;
		}
		return result;
	}

	private void bnChooseFile_Click(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between I4 and Unknown
		ushort num = 0;
		((FileDialog)OpenCurveFileDialog).Filter = "曲线数据文件(*.dat)|*.dat";
		((FileDialog)OpenCurveFileDialog).FileName = "CurveDefine.dat";
		((FileDialog)OpenCurveFileDialog).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 != (int)((CommonDialog)OpenCurveFileDialog).ShowDialog())
		{
			return;
		}
		StreamReader streamReader = new StreamReader(((FileDialog)OpenCurveFileDialog).FileName);
		stProfilePara.nMaxSecCnt = 0u;
		Array.Clear(fCurveData, 0, fCurveData.Length);
		for (num = 0; num < 10; num++)
		{
			try
			{
				string[] array = streamReader.ReadLine().Split(new char[1] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				for (ushort num2 = 0; num2 < 8; num2++)
				{
					fCurveData[num, num2] = float.Parse(array[num2]);
				}
				stProfilePara.nMaxSecCnt++;
			}
			catch (Exception)
			{
				break;
			}
		}
		streamReader.Close();
		if (stProfilePara.nMaxSecCnt <= 1)
		{
			return;
		}
		stProfilePara.nPeriodType = 0u;
		stProfilePara.nMaxCycleCnt = 0u;
		stProfilePara.Period = fCurveData[stProfilePara.nMaxSecCnt - 1, 1];
		for (num = 0; num < 10; num++)
		{
			mCurvePointList[num].Clear();
			mCurvePane.CurveList.Remove(mCurveItem[num]);
		}
		for (num = 0; num < stProfilePara.nMaxSecCnt - 1; num++)
		{
			float num3 = fCurveData[num, 1];
			float num4 = (fCurveData[num + 1, 1] - num3) / 10f;
			for (ushort num2 = 0; num2 < 11; num2++)
			{
				float num5 = fun_cacCurveYVal(fCurveData, num3, num);
				mCurvePointList[num].Add(num3, num5);
				num3 += num4;
			}
			mCurveItem[num] = mCurvePane.AddCurve("", mCurvePointList[num], Color.Red, SymbolType.Diamond);
		}
		zedGraphControl1.AxisChange();
		((Control)zedGraphControl1).Invalidate();
	}

	private void CurveDefine_Load(object sender, EventArgs e)
	{
		mCurvePointList = new PointPairList[10];
		mCurveItem = new LineItem[10];
		mCurvePane = zedGraphControl1.GraphPane;
		fCurveData = new float[10, 8];
		for (ushort num = 0; num < 10; num++)
		{
			mCurvePointList[num] = new PointPairList();
		}
		stProfilePara.nMaxSecCnt = 0u;
		PMainFrm = (MainFrm)(object)((Form)this).Owner;
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
	}

	private void bnDownLoadCurve_Click(object sender, EventArgs e)
	{
		ushort num = 0;
		if (0 >= stProfilePara.nMaxSecCnt)
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_ChooseCurveBeforeDownloadCurveParameters;
			return;
		}
		for (num = 0; num < stProfilePara.nMaxSecCnt; num++)
		{
			float[] array = new float[8];
			ushort num2 = 0;
			for (num2 = 0; num2 < 8; num2++)
			{
				array[num2] = fCurveData[num, num2];
			}
			if (PMainFrm.FunRes_Err == PMainFrm.Mccard.MoCtrCard_SetProfileCurvePara((byte)((ListControl)cbProfileMoveAxis).SelectedIndex, (byte)num, array))
			{
				((Control)lbInfo).Text = string.Format(GlobalData.GlobalLanguage.CurveDefineFrm_SendSomeParameterFailed, num + 1);
				return;
			}
		}
		((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendMovementParameterSucceed;
	}

	private void bnSetGearPara_Click(object sender, EventArgs e)
	{
		byte b = 0;
		byte b2 = 0;
		b = (byte)(rbGearMstrX.Checked ? 1 : 0);
		b = (byte)(rbGearMstrY.Checked ? 2 : b);
		b = (byte)(rbGearMstrZ.Checked ? 3 : b);
		b = (byte)(rbGearMstrA.Checked ? 4 : b);
		b = (byte)(rbGearMstrB.Checked ? 5 : b);
		b = (byte)(rbGearMstrC.Checked ? 6 : b);
		b--;
		b2 = (byte)(rbGearSlvX.Checked ? 1 : 0);
		b2 = (byte)(rbGearSlvY.Checked ? 2 : b2);
		b2 = (byte)(rbGearSlvZ.Checked ? 3 : b2);
		b2 = (byte)(rbGearSlvA.Checked ? 4 : b2);
		b2 = (byte)(rbGearSlvB.Checked ? 5 : b2);
		b2 = (byte)(rbGearSlvC.Checked ? 6 : b2);
		b2--;
		((Control)lbInfo).Text = "";
		if (byte.MaxValue == b || byte.MaxValue == b2)
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_PleaseDefineMasterAndSlave;
			return;
		}
		if (((Control)tbGearRatio).Text == "")
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_DefineGearRatio;
			return;
		}
		float fGear = float.Parse(((Control)tbGearRatio).Text);
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SetGearPara(b2, b, fGear))
		{
			((Control)lbInfo).Text = string.Format(GlobalData.GlobalLanguage.CurveDefineFrm_SendGearParameterSucceed, b, b2);
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendGearParameterFailed;
		}
	}

	private void bnStartGearMove_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbGearSlvX.Checked ? 1 : 0);
		b = (byte)(rbGearSlvY.Checked ? 2 : b);
		b = (byte)(rbGearSlvZ.Checked ? 3 : b);
		b = (byte)(rbGearSlvA.Checked ? 4 : b);
		b = (byte)(rbGearSlvB.Checked ? 5 : b);
		b = (byte)(rbGearSlvC.Checked ? 6 : b);
		b--;
		((Control)lbInfo).Text = "";
		if (byte.MaxValue == b)
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_ChooseSlaveAxis;
		}
		else if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ManGearStart(b))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendGearInCommandSucceed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendGearInCommandFailed;
		}
	}

	private void bnStopGearMove_Click(object sender, EventArgs e)
	{
		byte b = 0;
		b = (byte)(rbGearSlvX.Checked ? 1 : 0);
		b = (byte)(rbGearSlvY.Checked ? 2 : b);
		b = (byte)(rbGearSlvZ.Checked ? 3 : b);
		b = (byte)(rbGearSlvA.Checked ? 4 : b);
		b = (byte)(rbGearSlvB.Checked ? 5 : b);
		b = (byte)(rbGearSlvC.Checked ? 6 : b);
		b--;
		((Control)lbInfo).Text = "";
		if (byte.MaxValue == b)
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_ChooseSlaveAxis;
		}
		else if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ManGearStop(b))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendGearOutCommandSucceed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendGearOutCommandFailed;
		}
	}

	private void bnStart_Click(object sender, EventArgs e)
	{
		if (PMainFrm.FunRes_Err == PMainFrm.Mccard.MoCtrCard_StartProfileMove((byte)((ListControl)cbProfileMoveAxis).SelectedIndex, 1))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamInCommandFailed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamInCommandSucceed;
		}
	}

	private void bnStop_Click(object sender, EventArgs e)
	{
		if (PMainFrm.FunRes_Err == PMainFrm.Mccard.MoCtrCard_StopProfileMove((byte)((ListControl)cbProfileMoveAxis).SelectedIndex))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamOutCommandFailed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamOutCommandSucceed;
		}
	}

	private void bnStartNeg_Click(object sender, EventArgs e)
	{
		if (PMainFrm.FunRes_Err == PMainFrm.Mccard.MoCtrCard_StartProfileMove((byte)((ListControl)cbProfileMoveAxis).SelectedIndex, 0))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamInCommandFailed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SendCamInCommandSucceed;
		}
	}

	private void bnGenerateProfileDt_Click(object sender, EventArgs e)
	{
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_ = dataGridViewCam.NewRowIndex;
		int num = 0;
		int num2 = 0;
		bool flag = false;
		try
		{
			for (num = 0; num < dataGridViewCam.Rows.Count - 1; num++)
			{
				for (num2 = 0; num2 < 9; num2++)
				{
					if (dataGridViewCam.Rows[num].Cells[num2].Value == null)
					{
						flag = true;
						MessageBox.Show(GlobalData.GlobalLanguage.CurveDefineFrm_NotAllParameterDefined);
						return;
					}
				}
				if (flag)
				{
					break;
				}
				stCurveDt[num].LinkType = (uint)(SectionLinkType)Enum.Parse(typeof(SectionLinkType), dataGridViewCam.Rows[num].Cells[0].Value.ToString());
				stCurveDt[num].PosStartMaster = float.Parse(dataGridViewCam.Rows[num].Cells[1].Value.ToString());
				stCurveDt[num].PosStartSlave = float.Parse(dataGridViewCam.Rows[num].Cells[2].Value.ToString());
				stCurveDt[num].VelStart = float.Parse(dataGridViewCam.Rows[num].Cells[3].Value.ToString());
				stCurveDt[num].AccStart = float.Parse(dataGridViewCam.Rows[num].Cells[4].Value.ToString());
				stCurveDt[num].PosEndMaster = float.Parse(dataGridViewCam.Rows[num].Cells[5].Value.ToString());
				stCurveDt[num].PosEndSlave = float.Parse(dataGridViewCam.Rows[num].Cells[6].Value.ToString());
				stCurveDt[num].VelEnd = float.Parse(dataGridViewCam.Rows[num].Cells[7].Value.ToString());
				stCurveDt[num].AccEnd = float.Parse(dataGridViewCam.Rows[num].Cells[8].Value.ToString());
			}
			stCurveDt[num].LinkType = stCurveDt[num - 1].LinkType;
			stCurveDt[num].PosStartMaster = stCurveDt[num - 1].PosEndMaster;
			stCurveDt[num].PosStartSlave = stCurveDt[num - 1].PosEndSlave;
			stCurveDt[num].VelStart = 0f;
			stCurveDt[num].AccStart = 0f;
			stCurveDt[num].PosEndMaster = stCurveDt[num - 1].PosEndMaster;
			stCurveDt[num].PosEndSlave = stCurveDt[num - 1].PosEndSlave;
			stCurveDt[num].VelEnd = 0f;
			stCurveDt[num].AccEnd = 0f;
			num++;
			if (num == 0)
			{
				stProfilePara.nMaxSecCnt = 0u;
				stProfilePara.nMaxCycleCnt = 0u;
				stProfilePara.Period = 0f;
				stProfilePara.nPeriodType = 0u;
			}
			else
			{
				stProfilePara.Period = stCurveDt[num - 1].PosEndMaster;
				stProfilePara.nMaxSecCnt = (uint)num;
				stProfilePara.nMaxCycleCnt = 0u;
				stProfilePara.nPeriodType = 0u;
			}
			MessageBox.Show(string.Format(GlobalData.GlobalLanguage.CurveDefineFrm_DefineMovementPointCntAndT, stProfilePara.nMaxSecCnt, stProfilePara.Period));
			if (stProfilePara.nMaxSecCnt == 0)
			{
				return;
			}
			Array.Clear(fCurveData, 0, fCurveData.Length);
			for (num = 0; num < stProfilePara.nMaxSecCnt - 1; num++)
			{
				stProfileCoeDt[num] = CalculateCoefficient(stCurveDt[num], (SectionLinkType)stCurveDt[num].LinkType);
				fCurveData[num, 0] = stProfileCoeDt[num].nLinkType;
				fCurveData[num, 1] = stProfileCoeDt[num].PosMaster;
				fCurveData[num, 2] = stProfileCoeDt[num].Constant;
				fCurveData[num, 3] = stProfileCoeDt[num].CoefficientOfLinear;
				fCurveData[num, 4] = stProfileCoeDt[num].CoefficientOfQuadric;
				fCurveData[num, 5] = stProfileCoeDt[num].CoefficientOfCube;
				fCurveData[num, 6] = stProfileCoeDt[num].CoefficientOfFourth;
				fCurveData[num, 7] = stProfileCoeDt[num].CoefficientOfFiveth;
			}
			stProfileCoeDt[num].nLinkType = 0u;
			stProfileCoeDt[num].PosMaster = stCurveDt[num].PosStartMaster;
			stProfileCoeDt[num].Constant = 0f;
			stProfileCoeDt[num].CoefficientOfLinear = 0f;
			stProfileCoeDt[num].CoefficientOfQuadric = 0f;
			stProfileCoeDt[num].CoefficientOfCube = 0f;
			stProfileCoeDt[num].CoefficientOfFourth = 0f;
			stProfileCoeDt[num].CoefficientOfFiveth = 0f;
			fCurveData[num, 0] = stProfileCoeDt[num].nLinkType;
			fCurveData[num, 1] = stProfileCoeDt[num].PosMaster;
			fCurveData[num, 2] = stProfileCoeDt[num].Constant;
			fCurveData[num, 3] = stProfileCoeDt[num].CoefficientOfLinear;
			fCurveData[num, 4] = stProfileCoeDt[num].CoefficientOfQuadric;
			fCurveData[num, 5] = stProfileCoeDt[num].CoefficientOfCube;
			fCurveData[num, 6] = stProfileCoeDt[num].CoefficientOfFourth;
			fCurveData[num, 7] = stProfileCoeDt[num].CoefficientOfFiveth;
			for (ushort num3 = 0; num3 < 10; num3++)
			{
				mCurvePointList[num3].Clear();
				mCurvePane.CurveList.Remove(mCurveItem[num3]);
			}
			zedGraphControl1.AxisChange();
			((Control)zedGraphControl1).Invalidate();
			for (num = 0; num < stProfilePara.nMaxSecCnt - 1; num++)
			{
				float num4 = stProfileCoeDt[num].PosMaster;
				float num5 = (stProfileCoeDt[num + 1].PosMaster - num4) / 10f;
				for (int i = 0; i < 11; i++)
				{
					float num6 = fun_cacCurveYVal(fCurveData, num4, (ushort)num);
					mCurvePointList[num].Add(num4, num6);
					num4 += num5;
				}
				mCurveItem[num] = mCurvePane.AddCurve("", mCurvePointList[num], Color.Red, SymbolType.Diamond);
				zedGraphControl1.AxisChange();
				((Control)zedGraphControl1).Invalidate();
			}
		}
		catch
		{
			MessageBox.Show(GlobalData.GlobalLanguage.CurveDefineFrm_DataError, "ERROR", (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private void dataGridViewCam_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
	{
		((DataGridViewCell)e.Row.HeaderCell).Value = $"{((DataGridViewBand)e.Row).Index + 1}";
	}

	private stProfileCoeType CalculateCoefficient(stCurveData stCurveDt, SectionLinkType enumFMType)
	{
		stProfileCoeType result = new stProfileCoeType
		{
			PosMaster = stCurveDt.PosStartMaster,
			nLinkType = (uint)enumFMType
		};
		switch (enumFMType)
		{
		case SectionLinkType.Polynomial1:
			result.CoefficientOfLinear = (stCurveDt.PosEndSlave - stCurveDt.PosStartSlave) / (stCurveDt.PosEndMaster - stCurveDt.PosStartMaster);
			result.Constant = stCurveDt.PosStartSlave - result.CoefficientOfLinear * stCurveDt.PosStartMaster;
			result.CoefficientOfQuadric = 0f;
			result.CoefficientOfCube = 0f;
			result.CoefficientOfFourth = 0f;
			result.CoefficientOfFiveth = 0f;
			break;
		case SectionLinkType.Polynomial3:
		{
			float posStartMaster2 = stCurveDt.PosStartMaster;
			float num9 = posStartMaster2 * posStartMaster2;
			float num10 = posStartMaster2 * posStartMaster2 * posStartMaster2;
			float posEndMaster2 = stCurveDt.PosEndMaster;
			float num11 = posEndMaster2 * posEndMaster2;
			float num12 = posEndMaster2 * posEndMaster2 * posEndMaster2;
			double[] obj = new double[16]
			{
				1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0,
				0.0, 0.0, 0.0, 1.0, 0.0, 0.0
			};
			obj[1] = posStartMaster2;
			obj[2] = num9;
			obj[3] = num10;
			obj[6] = 2f * posStartMaster2;
			obj[7] = 3f * num9;
			obj[9] = posEndMaster2;
			obj[10] = num11;
			obj[11] = num12;
			obj[14] = 2f * posEndMaster2;
			obj[15] = 3f * num11;
			Matrix matrix4 = Matrix.Create(4, 4, obj);
			Matrix matrix5 = Matrix.Create(4, 1, new double[4] { stCurveDt.PosStartSlave, stCurveDt.VelStart, stCurveDt.PosEndSlave, stCurveDt.VelEnd });
			Matrix matrix6 = matrix4.Inverse();
			Matrix matrix7 = matrix6 * matrix5;
			_ = matrix6 * matrix4;
			result.Constant = (float)matrix7.Elements[0, 0];
			result.CoefficientOfLinear = (float)matrix7.Elements[1, 0];
			result.CoefficientOfQuadric = (float)matrix7.Elements[2, 0];
			result.CoefficientOfCube = (float)matrix7.Elements[3, 0];
			result.CoefficientOfFourth = 0f;
			result.CoefficientOfFiveth = 0f;
			break;
		}
		case SectionLinkType.Polynomial5:
		{
			float posStartMaster = stCurveDt.PosStartMaster;
			float num = posStartMaster * posStartMaster;
			float num2 = posStartMaster * posStartMaster * posStartMaster;
			float num3 = posStartMaster * posStartMaster * posStartMaster * posStartMaster;
			float num4 = posStartMaster * posStartMaster * posStartMaster * posStartMaster * posStartMaster;
			float posEndMaster = stCurveDt.PosEndMaster;
			float num5 = posEndMaster * posEndMaster;
			float num6 = posEndMaster * posEndMaster * posEndMaster;
			float num7 = posEndMaster * posEndMaster * posEndMaster * posEndMaster;
			float num8 = posEndMaster * posEndMaster * posEndMaster * posEndMaster * posEndMaster;
			Matrix matrix = Matrix.Create(6, 6, new double[36]
			{
				1.0,
				posStartMaster,
				num,
				num2,
				num3,
				num4,
				0.0,
				1.0,
				2f * posStartMaster,
				3f * num,
				4f * num2,
				5f * num3,
				0.0,
				0.0,
				2.0,
				6f * posStartMaster,
				12f * num,
				20f * num2,
				1.0,
				posEndMaster,
				num5,
				num6,
				num7,
				num8,
				0.0,
				1.0,
				2f * posEndMaster,
				3f * num5,
				4f * num6,
				5f * num7,
				0.0,
				0.0,
				2.0,
				6f * posEndMaster,
				12f * num5,
				20f * num6
			});
			Matrix matrix2 = Matrix.Create(6, 1, new double[6] { stCurveDt.PosStartSlave, stCurveDt.VelStart, stCurveDt.AccStart, stCurveDt.PosEndSlave, stCurveDt.VelEnd, stCurveDt.AccEnd });
			Matrix matrix3 = matrix.Inverse() * matrix2;
			result.Constant = (float)matrix3.Elements[0, 0];
			result.CoefficientOfLinear = (float)matrix3.Elements[1, 0];
			result.CoefficientOfQuadric = (float)matrix3.Elements[2, 0];
			result.CoefficientOfCube = (float)matrix3.Elements[3, 0];
			result.CoefficientOfFourth = (float)matrix3.Elements[4, 0];
			result.CoefficientOfFiveth = (float)matrix3.Elements[5, 0];
			break;
		}
		}
		return result;
	}

	private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (1 == tabControl1.SelectedIndex)
		{
			((Control)tbWordCycle).Text = stProfilePara.nMaxCycleCnt.ToString();
			((Control)tbRowCnt).Text = stProfilePara.nMaxSecCnt.ToString();
			((Control)tbPeriod).Text = stProfilePara.Period.ToString();
			((ListControl)cbProfileMoveAxis).SelectedIndex = 0;
			((ListControl)cbProfilePeriodType).SelectedIndex = 0;
		}
	}

	private void bnSetProfilePara_Click(object sender, EventArgs e)
	{
		stProfilePara.nMaxCycleCnt = uint.Parse(((Control)tbWordCycle).Text);
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SetProfilePara((byte)((ListControl)cbProfileMoveAxis).SelectedIndex, (ushort)stProfilePara.nMaxSecCnt, stProfilePara.Period, (ushort)stProfilePara.nMaxCycleCnt, (ushort)((ListControl)cbProfilePeriodType).SelectedIndex))
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SetCamParameterFailed;
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_SetCamParameterSucceed;
		}
	}

	private void bnSaveProfile_Click(object sender, EventArgs e)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between I4 and Unknown
		((FileDialog)saveFileDialog1).Filter = "轮廓文件(*.dat)|*.dat";
		((FileDialog)saveFileDialog1).FileName = "CurveDefine.dat";
		((FileDialog)saveFileDialog1).InitialDirectory = Directory.GetCurrentDirectory();
		if (1 != (int)((CommonDialog)saveFileDialog1).ShowDialog())
		{
			return;
		}
		string fileName = ((FileDialog)saveFileDialog1).FileName;
		if (stProfilePara.nMaxSecCnt != 0)
		{
			StreamWriter streamWriter = new StreamWriter(fileName, append: false, Encoding.ASCII);
			for (ushort num = 0; num < stProfilePara.nMaxSecCnt; num++)
			{
				string text = "";
				for (ushort num2 = 0; num2 < 8; num2++)
				{
					text += fCurveData[num, num2];
					text += "\t";
				}
				streamWriter.WriteLine(text);
			}
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.ScopeFrm_SaveFileTo + fileName;
			streamWriter.Close();
		}
		else
		{
			((Control)lbInfo).Text = GlobalData.GlobalLanguage.CurveDefineFrm_CamProfileEmpty;
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
		//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CurveDefine));
		zedGraphControl1 = new ZedGraphControl();
		OpenCurveFileDialog = new OpenFileDialog();
		groupBox1 = new GroupBox();
		label1 = new Label();
		tbGearRatio = new TextBox();
		gbGearSlv = new GroupBox();
		rbGearSlvC = new RadioButton();
		rbGearSlvB = new RadioButton();
		rbGearSlvA = new RadioButton();
		rbGearSlvZ = new RadioButton();
		rbGearSlvY = new RadioButton();
		rbGearSlvX = new RadioButton();
		gbGearMstr = new GroupBox();
		rbGearMstrC = new RadioButton();
		rbGearMstrB = new RadioButton();
		rbGearMstrA = new RadioButton();
		rbGearMstrZ = new RadioButton();
		rbGearMstrY = new RadioButton();
		rbGearMstrX = new RadioButton();
		bnStopGearMove = new Button();
		bnStartGearMove = new Button();
		bnSetGearPara = new Button();
		groupBox2 = new GroupBox();
		bnSaveProfile = new Button();
		bnChooseFile = new Button();
		bnGenerateProfileDt = new Button();
		bnStop = new Button();
		bnStartNeg = new Button();
		bnStartPos = new Button();
		bnDownLoadCurve = new Button();
		lbInfo = new Label();
		tabControl1 = new TabControl();
		tabPage1 = new TabPage();
		dataGridViewCam = new DataGridView();
		tabPage2 = new TabPage();
		label6 = new Label();
		cbProfilePeriodType = new ComboBox();
		label5 = new Label();
		cbProfileMoveAxis = new ComboBox();
		bnSetProfilePara = new Button();
		tbWordCycle = new TextBox();
		tbPeriod = new TextBox();
		tbRowCnt = new TextBox();
		label4 = new Label();
		label3 = new Label();
		label2 = new Label();
		saveFileDialog1 = new SaveFileDialog();
		FuncMode = new DataGridViewComboBoxColumn();
		MasterStartPos = new DataGridViewTextBoxColumn();
		SlaveStartPos = new DataGridViewTextBoxColumn();
		SlaveStartVel = new DataGridViewTextBoxColumn();
		SlaveStartAcc = new DataGridViewTextBoxColumn();
		MasterEndPos = new DataGridViewTextBoxColumn();
		SlaveEndPos = new DataGridViewTextBoxColumn();
		SlaveEndVel = new DataGridViewTextBoxColumn();
		SlaveEndAcc = new DataGridViewTextBoxColumn();
		((Control)groupBox1).SuspendLayout();
		((Control)gbGearSlv).SuspendLayout();
		((Control)gbGearMstr).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)tabControl1).SuspendLayout();
		((Control)tabPage1).SuspendLayout();
		((ISupportInitialize)dataGridViewCam).BeginInit();
		((Control)tabPage2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(zedGraphControl1, "zedGraphControl1");
		zedGraphControl1.IsSizeNSCursor = false;
		zedGraphControl1.IsSizeWECursor = false;
		((Control)zedGraphControl1).Name = "zedGraphControl1";
		zedGraphControl1.ScrollGrace = 0.0;
		zedGraphControl1.ScrollMaxX = 0.0;
		zedGraphControl1.ScrollMaxY = 0.0;
		zedGraphControl1.ScrollMaxY2 = 0.0;
		zedGraphControl1.ScrollMinX = 0.0;
		zedGraphControl1.ScrollMinY = 0.0;
		zedGraphControl1.ScrollMinY2 = 0.0;
		((UserControl)zedGraphControl1).Load += zedGraphControl1_Load;
		((FileDialog)OpenCurveFileDialog).FileName = "openFileDialog1";
		componentResourceManager.ApplyResources(OpenCurveFileDialog, "OpenCurveFileDialog");
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		((Control)groupBox1).Controls.Add((Control)(object)label1);
		((Control)groupBox1).Controls.Add((Control)(object)tbGearRatio);
		((Control)groupBox1).Controls.Add((Control)(object)gbGearSlv);
		((Control)groupBox1).Controls.Add((Control)(object)gbGearMstr);
		((Control)groupBox1).Controls.Add((Control)(object)bnStopGearMove);
		((Control)groupBox1).Controls.Add((Control)(object)bnStartGearMove);
		((Control)groupBox1).Controls.Add((Control)(object)bnSetGearPara);
		((Control)groupBox1).Name = "groupBox1";
		groupBox1.TabStop = false;
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(tbGearRatio, "tbGearRatio");
		((Control)tbGearRatio).Name = "tbGearRatio";
		componentResourceManager.ApplyResources(gbGearSlv, "gbGearSlv");
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvC);
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvB);
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvA);
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvZ);
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvY);
		((Control)gbGearSlv).Controls.Add((Control)(object)rbGearSlvX);
		((Control)gbGearSlv).Name = "gbGearSlv";
		gbGearSlv.TabStop = false;
		componentResourceManager.ApplyResources(rbGearSlvC, "rbGearSlvC");
		((Control)rbGearSlvC).Name = "rbGearSlvC";
		rbGearSlvC.TabStop = true;
		((ButtonBase)rbGearSlvC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearSlvB, "rbGearSlvB");
		((Control)rbGearSlvB).Name = "rbGearSlvB";
		rbGearSlvB.TabStop = true;
		((ButtonBase)rbGearSlvB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearSlvA, "rbGearSlvA");
		((Control)rbGearSlvA).Name = "rbGearSlvA";
		rbGearSlvA.TabStop = true;
		((ButtonBase)rbGearSlvA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearSlvZ, "rbGearSlvZ");
		((Control)rbGearSlvZ).Name = "rbGearSlvZ";
		rbGearSlvZ.TabStop = true;
		((ButtonBase)rbGearSlvZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearSlvY, "rbGearSlvY");
		rbGearSlvY.Checked = true;
		((Control)rbGearSlvY).Name = "rbGearSlvY";
		rbGearSlvY.TabStop = true;
		((ButtonBase)rbGearSlvY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearSlvX, "rbGearSlvX");
		((Control)rbGearSlvX).Name = "rbGearSlvX";
		rbGearSlvX.TabStop = true;
		((ButtonBase)rbGearSlvX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(gbGearMstr, "gbGearMstr");
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrC);
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrB);
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrA);
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrZ);
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrY);
		((Control)gbGearMstr).Controls.Add((Control)(object)rbGearMstrX);
		((Control)gbGearMstr).Name = "gbGearMstr";
		gbGearMstr.TabStop = false;
		componentResourceManager.ApplyResources(rbGearMstrC, "rbGearMstrC");
		((Control)rbGearMstrC).Name = "rbGearMstrC";
		rbGearMstrC.TabStop = true;
		((ButtonBase)rbGearMstrC).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearMstrB, "rbGearMstrB");
		((Control)rbGearMstrB).Name = "rbGearMstrB";
		rbGearMstrB.TabStop = true;
		((ButtonBase)rbGearMstrB).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearMstrA, "rbGearMstrA");
		((Control)rbGearMstrA).Name = "rbGearMstrA";
		rbGearMstrA.TabStop = true;
		((ButtonBase)rbGearMstrA).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearMstrZ, "rbGearMstrZ");
		((Control)rbGearMstrZ).Name = "rbGearMstrZ";
		rbGearMstrZ.TabStop = true;
		((ButtonBase)rbGearMstrZ).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearMstrY, "rbGearMstrY");
		((Control)rbGearMstrY).Name = "rbGearMstrY";
		rbGearMstrY.TabStop = true;
		((ButtonBase)rbGearMstrY).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(rbGearMstrX, "rbGearMstrX");
		rbGearMstrX.Checked = true;
		((Control)rbGearMstrX).Name = "rbGearMstrX";
		rbGearMstrX.TabStop = true;
		((ButtonBase)rbGearMstrX).UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(bnStopGearMove, "bnStopGearMove");
		((Control)bnStopGearMove).Name = "bnStopGearMove";
		((ButtonBase)bnStopGearMove).UseVisualStyleBackColor = true;
		((Control)bnStopGearMove).Click += bnStopGearMove_Click;
		componentResourceManager.ApplyResources(bnStartGearMove, "bnStartGearMove");
		((Control)bnStartGearMove).Name = "bnStartGearMove";
		((ButtonBase)bnStartGearMove).UseVisualStyleBackColor = true;
		((Control)bnStartGearMove).Click += bnStartGearMove_Click;
		componentResourceManager.ApplyResources(bnSetGearPara, "bnSetGearPara");
		((Control)bnSetGearPara).Name = "bnSetGearPara";
		((ButtonBase)bnSetGearPara).UseVisualStyleBackColor = true;
		((Control)bnSetGearPara).Click += bnSetGearPara_Click;
		componentResourceManager.ApplyResources(groupBox2, "groupBox2");
		((Control)groupBox2).Controls.Add((Control)(object)bnSaveProfile);
		((Control)groupBox2).Controls.Add((Control)(object)bnChooseFile);
		((Control)groupBox2).Name = "groupBox2";
		groupBox2.TabStop = false;
		componentResourceManager.ApplyResources(bnSaveProfile, "bnSaveProfile");
		((Control)bnSaveProfile).Name = "bnSaveProfile";
		((ButtonBase)bnSaveProfile).UseVisualStyleBackColor = true;
		((Control)bnSaveProfile).Click += bnSaveProfile_Click;
		componentResourceManager.ApplyResources(bnChooseFile, "bnChooseFile");
		((Control)bnChooseFile).Name = "bnChooseFile";
		((ButtonBase)bnChooseFile).UseVisualStyleBackColor = true;
		((Control)bnChooseFile).Click += bnChooseFile_Click;
		componentResourceManager.ApplyResources(bnGenerateProfileDt, "bnGenerateProfileDt");
		((Control)bnGenerateProfileDt).Name = "bnGenerateProfileDt";
		((ButtonBase)bnGenerateProfileDt).UseVisualStyleBackColor = true;
		((Control)bnGenerateProfileDt).Click += bnGenerateProfileDt_Click;
		componentResourceManager.ApplyResources(bnStop, "bnStop");
		((Control)bnStop).Name = "bnStop";
		((ButtonBase)bnStop).UseVisualStyleBackColor = true;
		((Control)bnStop).Click += bnStop_Click;
		componentResourceManager.ApplyResources(bnStartNeg, "bnStartNeg");
		((Control)bnStartNeg).Name = "bnStartNeg";
		((ButtonBase)bnStartNeg).UseVisualStyleBackColor = true;
		((Control)bnStartNeg).Click += bnStartNeg_Click;
		componentResourceManager.ApplyResources(bnStartPos, "bnStartPos");
		((Control)bnStartPos).Name = "bnStartPos";
		((ButtonBase)bnStartPos).UseVisualStyleBackColor = true;
		((Control)bnStartPos).Click += bnStart_Click;
		componentResourceManager.ApplyResources(bnDownLoadCurve, "bnDownLoadCurve");
		((Control)bnDownLoadCurve).Name = "bnDownLoadCurve";
		((ButtonBase)bnDownLoadCurve).UseVisualStyleBackColor = true;
		((Control)bnDownLoadCurve).Click += bnDownLoadCurve_Click;
		componentResourceManager.ApplyResources(lbInfo, "lbInfo");
		lbInfo.BorderStyle = (BorderStyle)1;
		((Control)lbInfo).ForeColor = Color.Red;
		((Control)lbInfo).Name = "lbInfo";
		componentResourceManager.ApplyResources(tabControl1, "tabControl1");
		((Control)tabControl1).Controls.Add((Control)(object)tabPage1);
		((Control)tabControl1).Controls.Add((Control)(object)tabPage2);
		((Control)tabControl1).Name = "tabControl1";
		tabControl1.SelectedIndex = 0;
		tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
		componentResourceManager.ApplyResources(tabPage1, "tabPage1");
		((Control)tabPage1).Controls.Add((Control)(object)dataGridViewCam);
		((Control)tabPage1).Name = "tabPage1";
		tabPage1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(dataGridViewCam, "dataGridViewCam");
		dataGridViewCam.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		dataGridViewCam.Columns.AddRange((DataGridViewColumn[])(object)new DataGridViewColumn[9]
		{
			(DataGridViewColumn)FuncMode,
			(DataGridViewColumn)MasterStartPos,
			(DataGridViewColumn)SlaveStartPos,
			(DataGridViewColumn)SlaveStartVel,
			(DataGridViewColumn)SlaveStartAcc,
			(DataGridViewColumn)MasterEndPos,
			(DataGridViewColumn)SlaveEndPos,
			(DataGridViewColumn)SlaveEndVel,
			(DataGridViewColumn)SlaveEndAcc
		});
		((Control)dataGridViewCam).Name = "dataGridViewCam";
		dataGridViewCam.RowTemplate.Height = 23;
		dataGridViewCam.RowStateChanged += new DataGridViewRowStateChangedEventHandler(dataGridViewCam_RowStateChanged);
		componentResourceManager.ApplyResources(tabPage2, "tabPage2");
		((Control)tabPage2).Controls.Add((Control)(object)bnGenerateProfileDt);
		((Control)tabPage2).Controls.Add((Control)(object)label6);
		((Control)tabPage2).Controls.Add((Control)(object)bnStop);
		((Control)tabPage2).Controls.Add((Control)(object)bnStartNeg);
		((Control)tabPage2).Controls.Add((Control)(object)cbProfilePeriodType);
		((Control)tabPage2).Controls.Add((Control)(object)bnStartPos);
		((Control)tabPage2).Controls.Add((Control)(object)bnDownLoadCurve);
		((Control)tabPage2).Controls.Add((Control)(object)label5);
		((Control)tabPage2).Controls.Add((Control)(object)cbProfileMoveAxis);
		((Control)tabPage2).Controls.Add((Control)(object)bnSetProfilePara);
		((Control)tabPage2).Controls.Add((Control)(object)tbWordCycle);
		((Control)tabPage2).Controls.Add((Control)(object)tbPeriod);
		((Control)tabPage2).Controls.Add((Control)(object)tbRowCnt);
		((Control)tabPage2).Controls.Add((Control)(object)label4);
		((Control)tabPage2).Controls.Add((Control)(object)label3);
		((Control)tabPage2).Controls.Add((Control)(object)label2);
		((Control)tabPage2).Name = "tabPage2";
		tabPage2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(cbProfilePeriodType, "cbProfilePeriodType");
		((ListControl)cbProfilePeriodType).FormattingEnabled = true;
		cbProfilePeriodType.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbProfilePeriodType.Items"),
			componentResourceManager.GetString("cbProfilePeriodType.Items1")
		});
		((Control)cbProfilePeriodType).Name = "cbProfilePeriodType";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(cbProfileMoveAxis, "cbProfileMoveAxis");
		((ListControl)cbProfileMoveAxis).FormattingEnabled = true;
		cbProfileMoveAxis.Items.AddRange(new object[6]
		{
			componentResourceManager.GetString("cbProfileMoveAxis.Items"),
			componentResourceManager.GetString("cbProfileMoveAxis.Items1"),
			componentResourceManager.GetString("cbProfileMoveAxis.Items2"),
			componentResourceManager.GetString("cbProfileMoveAxis.Items3"),
			componentResourceManager.GetString("cbProfileMoveAxis.Items4"),
			componentResourceManager.GetString("cbProfileMoveAxis.Items5")
		});
		((Control)cbProfileMoveAxis).Name = "cbProfileMoveAxis";
		componentResourceManager.ApplyResources(bnSetProfilePara, "bnSetProfilePara");
		((Control)bnSetProfilePara).Name = "bnSetProfilePara";
		((ButtonBase)bnSetProfilePara).UseVisualStyleBackColor = true;
		((Control)bnSetProfilePara).Click += bnSetProfilePara_Click;
		componentResourceManager.ApplyResources(tbWordCycle, "tbWordCycle");
		((Control)tbWordCycle).Name = "tbWordCycle";
		componentResourceManager.ApplyResources(tbPeriod, "tbPeriod");
		((Control)tbPeriod).Name = "tbPeriod";
		componentResourceManager.ApplyResources(tbRowCnt, "tbRowCnt");
		((Control)tbRowCnt).Name = "tbRowCnt";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(saveFileDialog1, "saveFileDialog1");
		componentResourceManager.ApplyResources(FuncMode, "FuncMode");
		FuncMode.Items.AddRange(new object[3] { "Polynomial1", "Polynomial3", "Polynomial5" });
		((DataGridViewColumn)FuncMode).Name = "FuncMode";
		((DataGridViewBand)FuncMode).Resizable = (DataGridViewTriState)1;
		((DataGridViewColumn)FuncMode).SortMode = (DataGridViewColumnSortMode)1;
		componentResourceManager.ApplyResources(MasterStartPos, "MasterStartPos");
		((DataGridViewColumn)MasterStartPos).Name = "MasterStartPos";
		componentResourceManager.ApplyResources(SlaveStartPos, "SlaveStartPos");
		((DataGridViewColumn)SlaveStartPos).Name = "SlaveStartPos";
		componentResourceManager.ApplyResources(SlaveStartVel, "SlaveStartVel");
		((DataGridViewColumn)SlaveStartVel).Name = "SlaveStartVel";
		componentResourceManager.ApplyResources(SlaveStartAcc, "SlaveStartAcc");
		((DataGridViewColumn)SlaveStartAcc).Name = "SlaveStartAcc";
		componentResourceManager.ApplyResources(MasterEndPos, "MasterEndPos");
		((DataGridViewColumn)MasterEndPos).Name = "MasterEndPos";
		componentResourceManager.ApplyResources(SlaveEndPos, "SlaveEndPos");
		((DataGridViewColumn)SlaveEndPos).Name = "SlaveEndPos";
		componentResourceManager.ApplyResources(SlaveEndVel, "SlaveEndVel");
		((DataGridViewColumn)SlaveEndVel).Name = "SlaveEndVel";
		componentResourceManager.ApplyResources(SlaveEndAcc, "SlaveEndAcc");
		((DataGridViewColumn)SlaveEndAcc).Name = "SlaveEndAcc";
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)tabControl1);
		((Control)this).Controls.Add((Control)(object)lbInfo);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Controls.Add((Control)(object)zedGraphControl1);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Name = "CurveDefine";
		((Form)this).Load += CurveDefine_Load;
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)gbGearSlv).ResumeLayout(false);
		((Control)gbGearSlv).PerformLayout();
		((Control)gbGearMstr).ResumeLayout(false);
		((Control)gbGearMstr).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)tabControl1).ResumeLayout(false);
		((Control)tabPage1).ResumeLayout(false);
		((ISupportInitialize)dataGridViewCam).EndInit();
		((Control)tabPage2).ResumeLayout(false);
		((Control)tabPage2).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
