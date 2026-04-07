using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MCCDEMO;

public class SetSysParaFrm : Form
{
	private IContainer components;

	private Label label1;

	private TextBox ParaX_Pulse;

	private Label lbParaX;

	private Label label2;

	private TextBox ParaX_Dist;

	private Label label3;

	private TextBox ParaX_Spd;

	private Label label4;

	private TextBox ParaX_Acc;

	private ComboBox UnitX;

	private ComboBox LLX;

	private ComboBox ORX;

	private Label Unit;

	private Label label6;

	private Label label7;

	private Label lbParaY;

	private TextBox ParaY_Pulse;

	private TextBox ParaY_Dist;

	private TextBox ParaY_Spd;

	private TextBox ParaY_Acc;

	private ComboBox UnitY;

	private ComboBox LLY;

	private ComboBox ORY;

	private Label lbParaZ;

	private TextBox ParaZ_Pulse;

	private TextBox ParaZ_Dist;

	private TextBox ParaZ_Spd;

	private TextBox ParaZ_Acc;

	private ComboBox UnitZ;

	private ComboBox LLZ;

	private ComboBox ORZ;

	private Label lbParaA;

	private TextBox ParaT_Pulse;

	private TextBox ParaT_Dist;

	private TextBox ParaT_Spd;

	private TextBox ParaT_Acc;

	private ComboBox UnitT;

	private ComboBox LLT;

	private ComboBox ORT;

	private Button BnSetPara;

	private Button BnSaveToBord;

	private ComboBox ORB;

	private ComboBox LLB;

	private ComboBox UnitB;

	private TextBox ParaB_Acc;

	private TextBox ParaB_Spd;

	private TextBox ParaB_Dist;

	private TextBox ParaB_Pulse;

	private Label lbParaB;

	private ComboBox ORC;

	private ComboBox LLC;

	private ComboBox UnitC;

	private TextBox ParaC_Acc;

	private TextBox ParaC_Spd;

	private TextBox ParaC_Dist;

	private TextBox ParaC_Pulse;

	private Label lbParaC;

	private MainFrm PMainFrm;

	private int ParaIndex;

	private float ParaVal;

	private CacPulseEquivalent FrmCacPulseEqu;

	public float fsFVal;

	private Label label11;

	private Label lable5;

	private TextBox tbLimitPosT;

	private TextBox tbLimitNegT;

	private TextBox tbLimitPosZ;

	private TextBox tbLimitNegZ;

	private TextBox tbLimitPosY;

	private TextBox tbLimitNegY;

	private TextBox tbLimitPosX;

	private TextBox tbLimitNegX;

	private TextBox tbLimitPosB;

	private TextBox tbLimitNegB;

	private TextBox tbLimitPosC;

	private TextBox tbLimitNegC;

	private TextBox tbEncoderPulseC;

	private TextBox tbEncoderPulseB;

	private TextBox tbEncoderPulseT;

	private TextBox tbEncoderPulseZ;

	private TextBox tbEncoderPulseY;

	private TextBox tbEncoderPulseX;

	private Label label5;

	private ComboBox cbSoftLimitEnC;

	private ComboBox cbSoftLimitEnB;

	private ComboBox cbSoftLimitEnT;

	private ComboBox cbSoftLimitEnZ;

	private ComboBox cbSoftLimitEnY;

	private ComboBox cbSoftLimitEnX;

	private Label label8;

	private ComboBox cbHardLimitC;

	private ComboBox cbHardLimitB;

	private ComboBox cbHardLimitT;

	private ComboBox cbHardLimitZ;

	private ComboBox cbHardLimitY;

	private ComboBox cbHardLimitX;

	private Label label9;

	private Label label10;

	private ComboBox cbHomModeX;

	private ComboBox cbHomModeY;

	private ComboBox cbHomModeZ;

	private ComboBox cbHomModeA;

	private ComboBox cbHomModeB;

	private ComboBox cbHomModeC;

	public SetSysParaFrm(object mOwner)
	{
		InitializeComponent();
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		PMainFrm = (MainFrm)mOwner;
	}

	private void SetSysParaFrm_Load(object sender, EventArgs e)
	{
		((Control)ParaX_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseX).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaX_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaX_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaX_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegX).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosX).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		int num = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLX).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLX).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORX).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORX).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitX).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitX).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnX).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnX).SelectedIndex = 0;
		}
		((ListControl)cbHomModeX).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitX).SelectedIndex = (int)(PMainFrm.SysUint & 0xF);
		((Control)ParaY_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseY).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaY_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaY_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaY_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegY).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosY).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		num = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLY).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLY).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORY).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORY).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitY).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitY).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnY).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnY).SelectedIndex = 0;
		}
		((ListControl)cbHomModeY).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitY).SelectedIndex = (int)((PMainFrm.SysUint >> 4) & 0xF);
		((Control)ParaZ_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseZ).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaZ_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaZ_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaZ_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegZ).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosZ).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		num = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLZ).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLZ).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORZ).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORZ).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitZ).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitZ).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnZ).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnZ).SelectedIndex = 0;
		}
		((ListControl)cbHomModeZ).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitZ).SelectedIndex = (int)((PMainFrm.SysUint >> 8) & 0xF);
		((Control)ParaT_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseT).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaT_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaT_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaT_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegT).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosT).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		num = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLT).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLT).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORT).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORT).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitT).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitT).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnT).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnT).SelectedIndex = 0;
		}
		((ListControl)cbHomModeA).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitT).SelectedIndex = (int)((PMainFrm.SysUint >> 12) & 0xF);
		((Control)ParaB_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseB).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaB_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaB_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaB_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegB).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosB).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		num = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLB).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLB).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORB).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORB).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitB).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitB).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnB).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnB).SelectedIndex = 0;
		}
		((ListControl)cbHomModeB).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitB).SelectedIndex = (int)((PMainFrm.SysUint >> 12) & 0xF);
		((Control)ParaC_Pulse).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_Pulse].ToString();
		((Control)tbEncoderPulseC).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_EncPulse].ToString();
		((Control)ParaC_Dist).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_ManDist].ToString("f3");
		((Control)ParaC_Spd).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_HighSpd].ToString("f3");
		((Control)ParaC_Acc).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_Acc].ToString("f3");
		((Control)tbLimitNegC).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg].ToString("f3");
		((Control)tbLimitPosC).Text = PMainFrm.SysPara[PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos].ToString("f3");
		num = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_LimitSig) != 0)
		{
			((ListControl)LLC).SelectedIndex = 1;
		}
		else
		{
			((ListControl)LLC).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeSig) != 0)
		{
			((ListControl)ORC).SelectedIndex = 1;
		}
		else
		{
			((ListControl)ORC).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HardLimit) != 0)
		{
			((ListControl)cbHardLimitC).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbHardLimitC).SelectedIndex = 0;
		}
		if (((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_SoftLimit) != 0)
		{
			((ListControl)cbSoftLimitEnC).SelectedIndex = 1;
		}
		else
		{
			((ListControl)cbSoftLimitEnC).SelectedIndex = 0;
		}
		((ListControl)cbHomModeC).SelectedIndex = ((int)PMainFrm.SysPara[num] & PMainFrm.Mccard.BitMask_HomeMode) >> 8;
		((ListControl)UnitC).SelectedIndex = (int)((PMainFrm.SysUint >> 12) & 0xF);
	}

	private void BnSetPara_Click(object sender, EventArgs e)
	{
		if (ParaIndex < PMainFrm.Mccard.MaxAxisNum)
		{
			PMainFrm.Mccard.MoCtrCard_SendPara((byte)(ParaIndex / 10), (byte)(ParaIndex % 10), ParaVal);
		}
	}

	private void SendParaToMCU(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		TextBox val = (TextBox)sender;
		if (((Control)ParaX_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaX_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseX).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseX).Text);
		}
		else if (((Control)ParaX_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaX_Dist).Text);
		}
		else if (((Control)ParaX_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaX_Spd).Text);
		}
		else if (((Control)ParaX_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaX_Acc).Text);
		}
		else if (((Control)tbLimitNegX).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegX).Text);
		}
		else if (((Control)tbLimitPosX).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosX).Text);
		}
		else if (((Control)ParaY_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaY_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseY).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseY).Text);
		}
		else if (((Control)ParaY_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaY_Dist).Text);
		}
		else if (((Control)ParaY_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaY_Spd).Text);
		}
		else if (((Control)ParaY_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaY_Acc).Text);
		}
		else if (((Control)tbLimitNegY).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegY).Text);
		}
		else if (((Control)tbLimitPosY).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosY).Text);
		}
		else if (((Control)ParaZ_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaZ_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseZ).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseZ).Text);
		}
		else if (((Control)ParaZ_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaZ_Dist).Text);
		}
		else if (((Control)ParaZ_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaZ_Spd).Text);
		}
		else if (((Control)ParaZ_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaZ_Acc).Text);
		}
		else if (((Control)tbLimitNegZ).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegZ).Text);
		}
		else if (((Control)tbLimitPosZ).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosZ).Text);
		}
		else if (((Control)ParaT_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaT_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseT).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseT).Text);
		}
		else if (((Control)ParaT_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaT_Dist).Text);
		}
		else if (((Control)ParaT_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaT_Spd).Text);
		}
		else if (((Control)ParaT_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaT_Acc).Text);
		}
		else if (((Control)tbLimitNegT).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegT).Text);
		}
		else if (((Control)tbLimitPosT).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosT).Text);
		}
		else if (((Control)ParaB_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaB_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseB).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseB).Text);
		}
		else if (((Control)ParaB_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaB_Dist).Text);
		}
		else if (((Control)ParaB_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaB_Spd).Text);
		}
		else if (((Control)ParaB_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaB_Acc).Text);
		}
		else if (((Control)tbLimitNegB).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegB).Text);
		}
		else if (((Control)tbLimitPosB).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosB).Text);
		}
		else if (((Control)ParaC_Pulse).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_Pulse;
			ParaVal = float.Parse(((Control)ParaC_Pulse).Text);
		}
		else if (((Control)tbEncoderPulseC).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_EncPulse;
			ParaVal = float.Parse(((Control)tbEncoderPulseC).Text);
		}
		else if (((Control)ParaC_Dist).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_ManDist;
			ParaVal = float.Parse(((Control)ParaC_Dist).Text);
		}
		else if (((Control)ParaC_Spd).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_HighSpd;
			ParaVal = float.Parse(((Control)ParaC_Spd).Text);
		}
		else if (((Control)ParaC_Acc).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_Acc;
			ParaVal = float.Parse(((Control)ParaC_Acc).Text);
		}
		else if (((Control)tbLimitNegC).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_SoftLimitNeg;
			ParaVal = float.Parse(((Control)tbLimitNegC).Text);
		}
		else if (((Control)tbLimitPosC).Name == ((Control)val).Name)
		{
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseCAxis + PMainFrm.Mccard.ParaIndex_SoftLimitPos;
			ParaVal = float.Parse(((Control)tbLimitPosC).Text);
		}
		if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_SendPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), ParaVal))
		{
			PMainFrm.SysPara[ParaIndex] = ParaVal;
		}
	}

	private void SendLimitAndOrgToMCU(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		byte axisId = 0;
		ComboBox val = (ComboBox)sender;
		if (((Control)ORX).Name == ((Control)val).Name)
		{
			axisId = 0;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)ORY).Name == ((Control)val).Name)
		{
			axisId = 1;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)ORZ).Name == ((Control)val).Name)
		{
			axisId = 2;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)ORT).Name == ((Control)val).Name)
		{
			axisId = 3;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)ORB).Name == ((Control)val).Name)
		{
			axisId = 4;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)ORC).Name == ((Control)val).Name)
		{
			axisId = 5;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_SetHomeSigTrigLev(axisId, (((ListControl)val).SelectedIndex > 0) ? ((byte)1) : ((byte)0)))
		{
			float[] paraVal = new float[1];
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ReadPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), paraVal))
			{
				PMainFrm.SysPara[ParaIndex] = ParaVal;
			}
		}
	}

	private void ChangeUint(object sender, EventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		uint num = PMainFrm.SysUint;
		ComboBox val = (ComboBox)sender;
		if (((Control)UnitX).Name == ((Control)val).Name)
		{
			num &= 0xFFFFF0;
			num |= (uint)((ListControl)UnitX).SelectedIndex;
		}
		else if (((Control)UnitY).Name == ((Control)val).Name)
		{
			num &= 0xFFFF0F;
			num |= (uint)(((ListControl)UnitY).SelectedIndex << 4);
		}
		else if (((Control)UnitZ).Name == ((Control)val).Name)
		{
			num &= 0xFFF0FF;
			num |= (uint)(((ListControl)UnitZ).SelectedIndex << 8);
		}
		else if (((Control)UnitT).Name == ((Control)val).Name)
		{
			num &= 0xFF0FFF;
			num |= (uint)(((ListControl)UnitT).SelectedIndex << 12);
		}
		else if (((Control)UnitB).Name == ((Control)val).Name)
		{
			num &= 0xF0FFFF;
			num |= (uint)(((ListControl)UnitB).SelectedIndex << 16);
		}
		else if (((Control)UnitC).Name == ((Control)val).Name)
		{
			num &= 0xFFFFF;
			num |= (uint)(((ListControl)UnitC).SelectedIndex << 20);
		}
		PMainFrm.SysUint = num;
	}

	private void BnSaveToBord_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.CommLinkEnable)
		{
			PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM();
		}
	}

	private void ParaX_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaX_Pulse).Text = fsFVal.ToString();
	}

	private void ParaY_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaY_Pulse).Text = fsFVal.ToString();
	}

	private void ParaZ_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaZ_Pulse).Text = fsFVal.ToString();
	}

	private void ParaT_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaT_Pulse).Text = fsFVal.ToString();
	}

	private void ParaB_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaB_Pulse).Text = fsFVal.ToString();
	}

	private void ParaC_Pulse_DoubleClick(object sender, EventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		FrmCacPulseEqu = new CacPulseEquivalent();
		((Form)FrmCacPulseEqu).Owner = (Form)(object)this;
		((Form)FrmCacPulseEqu).ShowDialog();
		((Control)ParaC_Pulse).Text = fsFVal.ToString();
	}

	private void LLX_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		byte axisId = 0;
		ComboBox val = (ComboBox)sender;
		if (((Control)LLX).Name == ((Control)val).Name)
		{
			axisId = 0;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)LLY).Name == ((Control)val).Name)
		{
			axisId = 1;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)LLZ).Name == ((Control)val).Name)
		{
			axisId = 2;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)LLT).Name == ((Control)val).Name)
		{
			axisId = 3;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)LLB).Name == ((Control)val).Name)
		{
			axisId = 4;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)LLC).Name == ((Control)val).Name)
		{
			axisId = 5;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_SetHardLimitSigTrigLev(axisId, (((ListControl)val).SelectedIndex > 0) ? ((byte)1) : ((byte)0)))
		{
			float[] paraVal = new float[1];
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ReadPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), paraVal))
			{
				PMainFrm.SysPara[ParaIndex] = ParaVal;
			}
		}
	}

	private void cbSoftLimitEnX_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		byte axisId = 0;
		ComboBox val = (ComboBox)sender;
		if (((Control)cbSoftLimitEnX).Name == ((Control)val).Name)
		{
			axisId = 0;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbSoftLimitEnY).Name == ((Control)val).Name)
		{
			axisId = 1;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbSoftLimitEnZ).Name == ((Control)val).Name)
		{
			axisId = 2;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbSoftLimitEnT).Name == ((Control)val).Name)
		{
			axisId = 3;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbSoftLimitEnB).Name == ((Control)val).Name)
		{
			axisId = 4;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbSoftLimitEnC).Name == ((Control)val).Name)
		{
			axisId = 5;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_EnableSoftLimitFunction(axisId, (((ListControl)val).SelectedIndex > 0) ? ((byte)1) : ((byte)0)))
		{
			float[] paraVal = new float[1];
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ReadPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), paraVal))
			{
				PMainFrm.SysPara[ParaIndex] = ParaVal;
			}
		}
	}

	private void cbHardLimitX_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		byte axisId = 0;
		ComboBox val = (ComboBox)sender;
		if (((Control)cbHardLimitX).Name == ((Control)val).Name)
		{
			axisId = 0;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHardLimitY).Name == ((Control)val).Name)
		{
			axisId = 1;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHardLimitZ).Name == ((Control)val).Name)
		{
			axisId = 2;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHardLimitT).Name == ((Control)val).Name)
		{
			axisId = 3;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHardLimitB).Name == ((Control)val).Name)
		{
			axisId = 4;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHardLimitC).Name == ((Control)val).Name)
		{
			axisId = 5;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_EnableHardLimitFunction(axisId, (((ListControl)val).SelectedIndex > 0) ? ((byte)1) : ((byte)0)))
		{
			float[] array = new float[1];
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ReadPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), array))
			{
				PMainFrm.SysPara[ParaIndex] = array[0];
			}
		}
	}

	private void cbHomModeX_Leave(object sender, EventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		byte axisId = 0;
		ComboBox val = (ComboBox)sender;
		if (((Control)cbHomModeX).Name == ((Control)val).Name)
		{
			axisId = 0;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseXAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHomModeY).Name == ((Control)val).Name)
		{
			axisId = 1;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseYAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHomModeZ).Name == ((Control)val).Name)
		{
			axisId = 2;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHomModeA).Name == ((Control)val).Name)
		{
			axisId = 3;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseAAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHomModeB).Name == ((Control)val).Name)
		{
			axisId = 4;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseBAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		else if (((Control)cbHomModeC).Name == ((Control)val).Name)
		{
			axisId = 5;
			ParaIndex = PMainFrm.Mccard.ParaIndex_BaseZAxis + PMainFrm.Mccard.ParaIndex_MotortInputLev;
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoctrCard_SetHomeMethord(axisId, (byte)((ListControl)val).SelectedIndex))
		{
			float[] array = new float[1];
			if (PMainFrm.FunRes_OK == PMainFrm.Mccard.MoCtrCard_ReadPara((byte)(ParaIndex / PMainFrm.Mccard.ParaNumPerAxis), (byte)(ParaIndex % PMainFrm.Mccard.ParaNumPerAxis), array))
			{
				PMainFrm.SysPara[ParaIndex] = array[0];
			}
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SetSysParaFrm));
		label1 = new Label();
		ParaX_Pulse = new TextBox();
		lbParaX = new Label();
		label2 = new Label();
		ParaX_Dist = new TextBox();
		label3 = new Label();
		ParaX_Spd = new TextBox();
		label4 = new Label();
		ParaX_Acc = new TextBox();
		UnitX = new ComboBox();
		LLX = new ComboBox();
		ORX = new ComboBox();
		Unit = new Label();
		label6 = new Label();
		label7 = new Label();
		lbParaY = new Label();
		ParaY_Pulse = new TextBox();
		ParaY_Dist = new TextBox();
		ParaY_Spd = new TextBox();
		ParaY_Acc = new TextBox();
		UnitY = new ComboBox();
		LLY = new ComboBox();
		ORY = new ComboBox();
		lbParaZ = new Label();
		ParaZ_Pulse = new TextBox();
		ParaZ_Dist = new TextBox();
		ParaZ_Spd = new TextBox();
		ParaZ_Acc = new TextBox();
		UnitZ = new ComboBox();
		LLZ = new ComboBox();
		ORZ = new ComboBox();
		lbParaA = new Label();
		ParaT_Pulse = new TextBox();
		ParaT_Dist = new TextBox();
		ParaT_Spd = new TextBox();
		ParaT_Acc = new TextBox();
		UnitT = new ComboBox();
		LLT = new ComboBox();
		ORT = new ComboBox();
		BnSetPara = new Button();
		BnSaveToBord = new Button();
		ORB = new ComboBox();
		LLB = new ComboBox();
		UnitB = new ComboBox();
		ParaB_Acc = new TextBox();
		ParaB_Spd = new TextBox();
		ParaB_Dist = new TextBox();
		ParaB_Pulse = new TextBox();
		lbParaB = new Label();
		ORC = new ComboBox();
		LLC = new ComboBox();
		UnitC = new ComboBox();
		ParaC_Acc = new TextBox();
		ParaC_Spd = new TextBox();
		ParaC_Dist = new TextBox();
		ParaC_Pulse = new TextBox();
		lbParaC = new Label();
		label11 = new Label();
		lable5 = new Label();
		tbLimitPosT = new TextBox();
		tbLimitNegT = new TextBox();
		tbLimitPosZ = new TextBox();
		tbLimitNegZ = new TextBox();
		tbLimitPosY = new TextBox();
		tbLimitNegY = new TextBox();
		tbLimitPosX = new TextBox();
		tbLimitNegX = new TextBox();
		tbLimitPosB = new TextBox();
		tbLimitNegB = new TextBox();
		tbLimitPosC = new TextBox();
		tbLimitNegC = new TextBox();
		tbEncoderPulseC = new TextBox();
		tbEncoderPulseB = new TextBox();
		tbEncoderPulseT = new TextBox();
		tbEncoderPulseZ = new TextBox();
		tbEncoderPulseY = new TextBox();
		tbEncoderPulseX = new TextBox();
		label5 = new Label();
		cbSoftLimitEnC = new ComboBox();
		cbSoftLimitEnB = new ComboBox();
		cbSoftLimitEnT = new ComboBox();
		cbSoftLimitEnZ = new ComboBox();
		cbSoftLimitEnY = new ComboBox();
		cbSoftLimitEnX = new ComboBox();
		label8 = new Label();
		cbHardLimitC = new ComboBox();
		cbHardLimitB = new ComboBox();
		cbHardLimitT = new ComboBox();
		cbHardLimitZ = new ComboBox();
		cbHardLimitY = new ComboBox();
		cbHardLimitX = new ComboBox();
		label9 = new Label();
		label10 = new Label();
		cbHomModeX = new ComboBox();
		cbHomModeY = new ComboBox();
		cbHomModeZ = new ComboBox();
		cbHomModeA = new ComboBox();
		cbHomModeB = new ComboBox();
		cbHomModeC = new ComboBox();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		componentResourceManager.ApplyResources(ParaX_Pulse, "ParaX_Pulse");
		((Control)ParaX_Pulse).Name = "ParaX_Pulse";
		((Control)ParaX_Pulse).DoubleClick += ParaX_Pulse_DoubleClick;
		((Control)ParaX_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(lbParaX, "lbParaX");
		((Control)lbParaX).Name = "lbParaX";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		componentResourceManager.ApplyResources(ParaX_Dist, "ParaX_Dist");
		((Control)ParaX_Dist).Name = "ParaX_Dist";
		((Control)ParaX_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		componentResourceManager.ApplyResources(ParaX_Spd, "ParaX_Spd");
		((Control)ParaX_Spd).Name = "ParaX_Spd";
		((Control)ParaX_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		componentResourceManager.ApplyResources(ParaX_Acc, "ParaX_Acc");
		((Control)ParaX_Acc).Name = "ParaX_Acc";
		((Control)ParaX_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(UnitX, "UnitX");
		((ListControl)UnitX).FormattingEnabled = true;
		UnitX.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitX.Items"),
			componentResourceManager.GetString("UnitX.Items1"),
			componentResourceManager.GetString("UnitX.Items2"),
			componentResourceManager.GetString("UnitX.Items3")
		});
		((Control)UnitX).Name = "UnitX";
		((Control)UnitX).Leave += ChangeUint;
		componentResourceManager.ApplyResources(LLX, "LLX");
		((ListControl)LLX).DisplayMember = "1";
		((ListControl)LLX).FormattingEnabled = true;
		LLX.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLX.Items"),
			componentResourceManager.GetString("LLX.Items1")
		});
		((Control)LLX).Name = "LLX";
		((ListControl)LLX).ValueMember = "1";
		((Control)LLX).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(ORX, "ORX");
		((ListControl)ORX).FormattingEnabled = true;
		ORX.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORX.Items"),
			componentResourceManager.GetString("ORX.Items1")
		});
		((Control)ORX).Name = "ORX";
		((Control)ORX).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(Unit, "Unit");
		((Control)Unit).Name = "Unit";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		componentResourceManager.ApplyResources(lbParaY, "lbParaY");
		((Control)lbParaY).Name = "lbParaY";
		componentResourceManager.ApplyResources(ParaY_Pulse, "ParaY_Pulse");
		((Control)ParaY_Pulse).Name = "ParaY_Pulse";
		((Control)ParaY_Pulse).DoubleClick += ParaY_Pulse_DoubleClick;
		((Control)ParaY_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaY_Dist, "ParaY_Dist");
		((Control)ParaY_Dist).Name = "ParaY_Dist";
		((Control)ParaY_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaY_Spd, "ParaY_Spd");
		((Control)ParaY_Spd).Name = "ParaY_Spd";
		((Control)ParaY_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaY_Acc, "ParaY_Acc");
		((Control)ParaY_Acc).Name = "ParaY_Acc";
		((Control)ParaY_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(UnitY, "UnitY");
		((ListControl)UnitY).FormattingEnabled = true;
		UnitY.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitY.Items"),
			componentResourceManager.GetString("UnitY.Items1"),
			componentResourceManager.GetString("UnitY.Items2"),
			componentResourceManager.GetString("UnitY.Items3")
		});
		((Control)UnitY).Name = "UnitY";
		((Control)UnitY).Leave += ChangeUint;
		componentResourceManager.ApplyResources(LLY, "LLY");
		((ListControl)LLY).FormattingEnabled = true;
		LLY.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLY.Items"),
			componentResourceManager.GetString("LLY.Items1")
		});
		((Control)LLY).Name = "LLY";
		((Control)LLY).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(ORY, "ORY");
		((ListControl)ORY).FormattingEnabled = true;
		ORY.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORY.Items"),
			componentResourceManager.GetString("ORY.Items1")
		});
		((Control)ORY).Name = "ORY";
		((Control)ORY).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(lbParaZ, "lbParaZ");
		((Control)lbParaZ).Name = "lbParaZ";
		componentResourceManager.ApplyResources(ParaZ_Pulse, "ParaZ_Pulse");
		((Control)ParaZ_Pulse).Name = "ParaZ_Pulse";
		((Control)ParaZ_Pulse).DoubleClick += ParaZ_Pulse_DoubleClick;
		((Control)ParaZ_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_Dist, "ParaZ_Dist");
		((Control)ParaZ_Dist).Name = "ParaZ_Dist";
		((Control)ParaZ_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_Spd, "ParaZ_Spd");
		((Control)ParaZ_Spd).Name = "ParaZ_Spd";
		((Control)ParaZ_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaZ_Acc, "ParaZ_Acc");
		((Control)ParaZ_Acc).Name = "ParaZ_Acc";
		((Control)ParaZ_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(UnitZ, "UnitZ");
		((ListControl)UnitZ).FormattingEnabled = true;
		UnitZ.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitZ.Items"),
			componentResourceManager.GetString("UnitZ.Items1"),
			componentResourceManager.GetString("UnitZ.Items2"),
			componentResourceManager.GetString("UnitZ.Items3")
		});
		((Control)UnitZ).Name = "UnitZ";
		((Control)UnitZ).Leave += ChangeUint;
		componentResourceManager.ApplyResources(LLZ, "LLZ");
		((ListControl)LLZ).FormattingEnabled = true;
		LLZ.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLZ.Items"),
			componentResourceManager.GetString("LLZ.Items1")
		});
		((Control)LLZ).Name = "LLZ";
		((Control)LLZ).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(ORZ, "ORZ");
		((ListControl)ORZ).FormattingEnabled = true;
		ORZ.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORZ.Items"),
			componentResourceManager.GetString("ORZ.Items1")
		});
		((Control)ORZ).Name = "ORZ";
		((Control)ORZ).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(lbParaA, "lbParaA");
		((Control)lbParaA).Name = "lbParaA";
		componentResourceManager.ApplyResources(ParaT_Pulse, "ParaT_Pulse");
		((Control)ParaT_Pulse).Name = "ParaT_Pulse";
		((Control)ParaT_Pulse).DoubleClick += ParaT_Pulse_DoubleClick;
		((Control)ParaT_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaT_Dist, "ParaT_Dist");
		((Control)ParaT_Dist).Name = "ParaT_Dist";
		((Control)ParaT_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaT_Spd, "ParaT_Spd");
		((Control)ParaT_Spd).Name = "ParaT_Spd";
		((Control)ParaT_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaT_Acc, "ParaT_Acc");
		((Control)ParaT_Acc).Name = "ParaT_Acc";
		((Control)ParaT_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(UnitT, "UnitT");
		((ListControl)UnitT).FormattingEnabled = true;
		UnitT.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitT.Items"),
			componentResourceManager.GetString("UnitT.Items1"),
			componentResourceManager.GetString("UnitT.Items2"),
			componentResourceManager.GetString("UnitT.Items3")
		});
		((Control)UnitT).Name = "UnitT";
		((Control)UnitT).Leave += ChangeUint;
		componentResourceManager.ApplyResources(LLT, "LLT");
		((ListControl)LLT).FormattingEnabled = true;
		LLT.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLT.Items"),
			componentResourceManager.GetString("LLT.Items1")
		});
		((Control)LLT).Name = "LLT";
		((Control)LLT).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(ORT, "ORT");
		((ListControl)ORT).FormattingEnabled = true;
		ORT.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORT.Items"),
			componentResourceManager.GetString("ORT.Items1")
		});
		((Control)ORT).Name = "ORT";
		((Control)ORT).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(BnSetPara, "BnSetPara");
		((Control)BnSetPara).Name = "BnSetPara";
		((ButtonBase)BnSetPara).UseVisualStyleBackColor = true;
		((Control)BnSetPara).Click += BnSetPara_Click;
		componentResourceManager.ApplyResources(BnSaveToBord, "BnSaveToBord");
		((Control)BnSaveToBord).Name = "BnSaveToBord";
		((ButtonBase)BnSaveToBord).UseVisualStyleBackColor = true;
		((Control)BnSaveToBord).Click += BnSaveToBord_Click;
		componentResourceManager.ApplyResources(ORB, "ORB");
		((ListControl)ORB).FormattingEnabled = true;
		ORB.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORB.Items"),
			componentResourceManager.GetString("ORB.Items1")
		});
		((Control)ORB).Name = "ORB";
		((Control)ORB).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(LLB, "LLB");
		((ListControl)LLB).FormattingEnabled = true;
		LLB.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLB.Items"),
			componentResourceManager.GetString("LLB.Items1")
		});
		((Control)LLB).Name = "LLB";
		((Control)LLB).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(UnitB, "UnitB");
		((ListControl)UnitB).FormattingEnabled = true;
		UnitB.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitB.Items"),
			componentResourceManager.GetString("UnitB.Items1"),
			componentResourceManager.GetString("UnitB.Items2"),
			componentResourceManager.GetString("UnitB.Items3")
		});
		((Control)UnitB).Name = "UnitB";
		((Control)UnitB).Leave += ChangeUint;
		componentResourceManager.ApplyResources(ParaB_Acc, "ParaB_Acc");
		((Control)ParaB_Acc).Name = "ParaB_Acc";
		((Control)ParaB_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaB_Spd, "ParaB_Spd");
		((Control)ParaB_Spd).Name = "ParaB_Spd";
		((Control)ParaB_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaB_Dist, "ParaB_Dist");
		((Control)ParaB_Dist).Name = "ParaB_Dist";
		((Control)ParaB_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaB_Pulse, "ParaB_Pulse");
		((Control)ParaB_Pulse).Name = "ParaB_Pulse";
		((Control)ParaB_Pulse).DoubleClick += ParaB_Pulse_DoubleClick;
		((Control)ParaB_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(lbParaB, "lbParaB");
		((Control)lbParaB).Name = "lbParaB";
		componentResourceManager.ApplyResources(ORC, "ORC");
		((ListControl)ORC).FormattingEnabled = true;
		ORC.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("ORC.Items"),
			componentResourceManager.GetString("ORC.Items1")
		});
		((Control)ORC).Name = "ORC";
		((Control)ORC).Leave += SendLimitAndOrgToMCU;
		componentResourceManager.ApplyResources(LLC, "LLC");
		((ListControl)LLC).FormattingEnabled = true;
		LLC.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("LLC.Items"),
			componentResourceManager.GetString("LLC.Items1")
		});
		((Control)LLC).Name = "LLC";
		((Control)LLC).Leave += LLX_Leave;
		componentResourceManager.ApplyResources(UnitC, "UnitC");
		((ListControl)UnitC).FormattingEnabled = true;
		UnitC.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("UnitC.Items"),
			componentResourceManager.GetString("UnitC.Items1"),
			componentResourceManager.GetString("UnitC.Items2"),
			componentResourceManager.GetString("UnitC.Items3")
		});
		((Control)UnitC).Name = "UnitC";
		((Control)UnitC).Leave += ChangeUint;
		componentResourceManager.ApplyResources(ParaC_Acc, "ParaC_Acc");
		((Control)ParaC_Acc).Name = "ParaC_Acc";
		((Control)ParaC_Acc).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaC_Spd, "ParaC_Spd");
		((Control)ParaC_Spd).Name = "ParaC_Spd";
		((Control)ParaC_Spd).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaC_Dist, "ParaC_Dist");
		((Control)ParaC_Dist).Name = "ParaC_Dist";
		((Control)ParaC_Dist).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(ParaC_Pulse, "ParaC_Pulse");
		((Control)ParaC_Pulse).Name = "ParaC_Pulse";
		((Control)ParaC_Pulse).DoubleClick += ParaC_Pulse_DoubleClick;
		((Control)ParaC_Pulse).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(lbParaC, "lbParaC");
		((Control)lbParaC).Name = "lbParaC";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		componentResourceManager.ApplyResources(lable5, "lable5");
		((Control)lable5).Name = "lable5";
		componentResourceManager.ApplyResources(tbLimitPosT, "tbLimitPosT");
		((Control)tbLimitPosT).Name = "tbLimitPosT";
		((Control)tbLimitPosT).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegT, "tbLimitNegT");
		((Control)tbLimitNegT).Name = "tbLimitNegT";
		((Control)tbLimitNegT).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitPosZ, "tbLimitPosZ");
		((Control)tbLimitPosZ).Name = "tbLimitPosZ";
		((Control)tbLimitPosZ).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegZ, "tbLimitNegZ");
		((Control)tbLimitNegZ).Name = "tbLimitNegZ";
		((Control)tbLimitNegZ).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitPosY, "tbLimitPosY");
		((Control)tbLimitPosY).Name = "tbLimitPosY";
		((Control)tbLimitPosY).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegY, "tbLimitNegY");
		((Control)tbLimitNegY).Name = "tbLimitNegY";
		((Control)tbLimitNegY).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitPosX, "tbLimitPosX");
		((Control)tbLimitPosX).Name = "tbLimitPosX";
		((Control)tbLimitPosX).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegX, "tbLimitNegX");
		((Control)tbLimitNegX).Name = "tbLimitNegX";
		((Control)tbLimitNegX).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitPosB, "tbLimitPosB");
		((Control)tbLimitPosB).Name = "tbLimitPosB";
		((Control)tbLimitPosB).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegB, "tbLimitNegB");
		((Control)tbLimitNegB).Name = "tbLimitNegB";
		((Control)tbLimitNegB).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitPosC, "tbLimitPosC");
		((Control)tbLimitPosC).Name = "tbLimitPosC";
		((Control)tbLimitPosC).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbLimitNegC, "tbLimitNegC");
		((Control)tbLimitNegC).Name = "tbLimitNegC";
		((Control)tbLimitNegC).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseC, "tbEncoderPulseC");
		((Control)tbEncoderPulseC).Name = "tbEncoderPulseC";
		((Control)tbEncoderPulseC).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseB, "tbEncoderPulseB");
		((Control)tbEncoderPulseB).Name = "tbEncoderPulseB";
		((Control)tbEncoderPulseB).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseT, "tbEncoderPulseT");
		((Control)tbEncoderPulseT).Name = "tbEncoderPulseT";
		((Control)tbEncoderPulseT).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseZ, "tbEncoderPulseZ");
		((Control)tbEncoderPulseZ).Name = "tbEncoderPulseZ";
		((Control)tbEncoderPulseZ).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseY, "tbEncoderPulseY");
		((Control)tbEncoderPulseY).Name = "tbEncoderPulseY";
		((Control)tbEncoderPulseY).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(tbEncoderPulseX, "tbEncoderPulseX");
		((Control)tbEncoderPulseX).Name = "tbEncoderPulseX";
		((Control)tbEncoderPulseX).Leave += SendParaToMCU;
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		componentResourceManager.ApplyResources(cbSoftLimitEnC, "cbSoftLimitEnC");
		((ListControl)cbSoftLimitEnC).FormattingEnabled = true;
		cbSoftLimitEnC.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnC.Items"),
			componentResourceManager.GetString("cbSoftLimitEnC.Items1")
		});
		((Control)cbSoftLimitEnC).Name = "cbSoftLimitEnC";
		((Control)cbSoftLimitEnC).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(cbSoftLimitEnB, "cbSoftLimitEnB");
		((ListControl)cbSoftLimitEnB).FormattingEnabled = true;
		cbSoftLimitEnB.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnB.Items"),
			componentResourceManager.GetString("cbSoftLimitEnB.Items1")
		});
		((Control)cbSoftLimitEnB).Name = "cbSoftLimitEnB";
		((Control)cbSoftLimitEnB).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(cbSoftLimitEnT, "cbSoftLimitEnT");
		((ListControl)cbSoftLimitEnT).FormattingEnabled = true;
		cbSoftLimitEnT.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnT.Items"),
			componentResourceManager.GetString("cbSoftLimitEnT.Items1")
		});
		((Control)cbSoftLimitEnT).Name = "cbSoftLimitEnT";
		((Control)cbSoftLimitEnT).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(cbSoftLimitEnZ, "cbSoftLimitEnZ");
		((ListControl)cbSoftLimitEnZ).FormattingEnabled = true;
		cbSoftLimitEnZ.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnZ.Items"),
			componentResourceManager.GetString("cbSoftLimitEnZ.Items1")
		});
		((Control)cbSoftLimitEnZ).Name = "cbSoftLimitEnZ";
		((Control)cbSoftLimitEnZ).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(cbSoftLimitEnY, "cbSoftLimitEnY");
		((ListControl)cbSoftLimitEnY).FormattingEnabled = true;
		cbSoftLimitEnY.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnY.Items"),
			componentResourceManager.GetString("cbSoftLimitEnY.Items1")
		});
		((Control)cbSoftLimitEnY).Name = "cbSoftLimitEnY";
		((Control)cbSoftLimitEnY).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(cbSoftLimitEnX, "cbSoftLimitEnX");
		((ListControl)cbSoftLimitEnX).FormattingEnabled = true;
		cbSoftLimitEnX.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbSoftLimitEnX.Items"),
			componentResourceManager.GetString("cbSoftLimitEnX.Items1")
		});
		((Control)cbSoftLimitEnX).Name = "cbSoftLimitEnX";
		((Control)cbSoftLimitEnX).Leave += cbSoftLimitEnX_Leave;
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		componentResourceManager.ApplyResources(cbHardLimitC, "cbHardLimitC");
		((ListControl)cbHardLimitC).FormattingEnabled = true;
		cbHardLimitC.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitC.Items"),
			componentResourceManager.GetString("cbHardLimitC.Items1")
		});
		((Control)cbHardLimitC).Name = "cbHardLimitC";
		((Control)cbHardLimitC).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(cbHardLimitB, "cbHardLimitB");
		((ListControl)cbHardLimitB).FormattingEnabled = true;
		cbHardLimitB.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitB.Items"),
			componentResourceManager.GetString("cbHardLimitB.Items1")
		});
		((Control)cbHardLimitB).Name = "cbHardLimitB";
		((Control)cbHardLimitB).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(cbHardLimitT, "cbHardLimitT");
		((ListControl)cbHardLimitT).FormattingEnabled = true;
		cbHardLimitT.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitT.Items"),
			componentResourceManager.GetString("cbHardLimitT.Items1")
		});
		((Control)cbHardLimitT).Name = "cbHardLimitT";
		((Control)cbHardLimitT).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(cbHardLimitZ, "cbHardLimitZ");
		((ListControl)cbHardLimitZ).FormattingEnabled = true;
		cbHardLimitZ.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitZ.Items"),
			componentResourceManager.GetString("cbHardLimitZ.Items1")
		});
		((Control)cbHardLimitZ).Name = "cbHardLimitZ";
		((Control)cbHardLimitZ).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(cbHardLimitY, "cbHardLimitY");
		((ListControl)cbHardLimitY).FormattingEnabled = true;
		cbHardLimitY.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitY.Items"),
			componentResourceManager.GetString("cbHardLimitY.Items1")
		});
		((Control)cbHardLimitY).Name = "cbHardLimitY";
		((Control)cbHardLimitY).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(cbHardLimitX, "cbHardLimitX");
		((ListControl)cbHardLimitX).FormattingEnabled = true;
		cbHardLimitX.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("cbHardLimitX.Items"),
			componentResourceManager.GetString("cbHardLimitX.Items1")
		});
		((Control)cbHardLimitX).Name = "cbHardLimitX";
		((Control)cbHardLimitX).Leave += cbHardLimitX_Leave;
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		componentResourceManager.ApplyResources(cbHomModeX, "cbHomModeX");
		((ListControl)cbHomModeX).FormattingEnabled = true;
		cbHomModeX.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeX.Items"),
			componentResourceManager.GetString("cbHomModeX.Items1"),
			componentResourceManager.GetString("cbHomModeX.Items2")
		});
		((Control)cbHomModeX).Name = "cbHomModeX";
		((Control)cbHomModeX).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(cbHomModeY, "cbHomModeY");
		((ListControl)cbHomModeY).FormattingEnabled = true;
		cbHomModeY.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeY.Items"),
			componentResourceManager.GetString("cbHomModeY.Items1"),
			componentResourceManager.GetString("cbHomModeY.Items2")
		});
		((Control)cbHomModeY).Name = "cbHomModeY";
		((Control)cbHomModeY).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(cbHomModeZ, "cbHomModeZ");
		((ListControl)cbHomModeZ).FormattingEnabled = true;
		cbHomModeZ.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeZ.Items"),
			componentResourceManager.GetString("cbHomModeZ.Items1"),
			componentResourceManager.GetString("cbHomModeZ.Items2")
		});
		((Control)cbHomModeZ).Name = "cbHomModeZ";
		((Control)cbHomModeZ).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(cbHomModeA, "cbHomModeA");
		((ListControl)cbHomModeA).FormattingEnabled = true;
		cbHomModeA.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeA.Items"),
			componentResourceManager.GetString("cbHomModeA.Items1"),
			componentResourceManager.GetString("cbHomModeA.Items2")
		});
		((Control)cbHomModeA).Name = "cbHomModeA";
		((Control)cbHomModeA).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(cbHomModeB, "cbHomModeB");
		((ListControl)cbHomModeB).FormattingEnabled = true;
		cbHomModeB.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeB.Items"),
			componentResourceManager.GetString("cbHomModeB.Items1"),
			componentResourceManager.GetString("cbHomModeB.Items2")
		});
		((Control)cbHomModeB).Name = "cbHomModeB";
		((Control)cbHomModeB).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(cbHomModeC, "cbHomModeC");
		((ListControl)cbHomModeC).FormattingEnabled = true;
		cbHomModeC.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("cbHomModeC.Items"),
			componentResourceManager.GetString("cbHomModeC.Items1"),
			componentResourceManager.GetString("cbHomModeC.Items2")
		});
		((Control)cbHomModeC).Name = "cbHomModeC";
		((Control)cbHomModeC).Leave += cbHomModeX_Leave;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)cbHomModeC);
		((Control)this).Controls.Add((Control)(object)cbHomModeB);
		((Control)this).Controls.Add((Control)(object)cbHomModeA);
		((Control)this).Controls.Add((Control)(object)cbHomModeZ);
		((Control)this).Controls.Add((Control)(object)cbHomModeY);
		((Control)this).Controls.Add((Control)(object)cbHomModeX);
		((Control)this).Controls.Add((Control)(object)label10);
		((Control)this).Controls.Add((Control)(object)cbHardLimitC);
		((Control)this).Controls.Add((Control)(object)cbHardLimitB);
		((Control)this).Controls.Add((Control)(object)cbHardLimitT);
		((Control)this).Controls.Add((Control)(object)cbHardLimitZ);
		((Control)this).Controls.Add((Control)(object)cbHardLimitY);
		((Control)this).Controls.Add((Control)(object)cbHardLimitX);
		((Control)this).Controls.Add((Control)(object)label9);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnC);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnB);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnT);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnZ);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnY);
		((Control)this).Controls.Add((Control)(object)cbSoftLimitEnX);
		((Control)this).Controls.Add((Control)(object)label8);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseC);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseB);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseT);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseZ);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseY);
		((Control)this).Controls.Add((Control)(object)tbEncoderPulseX);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)tbLimitPosC);
		((Control)this).Controls.Add((Control)(object)tbLimitNegC);
		((Control)this).Controls.Add((Control)(object)tbLimitPosB);
		((Control)this).Controls.Add((Control)(object)tbLimitNegB);
		((Control)this).Controls.Add((Control)(object)label11);
		((Control)this).Controls.Add((Control)(object)lable5);
		((Control)this).Controls.Add((Control)(object)tbLimitPosT);
		((Control)this).Controls.Add((Control)(object)tbLimitNegT);
		((Control)this).Controls.Add((Control)(object)tbLimitPosZ);
		((Control)this).Controls.Add((Control)(object)tbLimitNegZ);
		((Control)this).Controls.Add((Control)(object)tbLimitPosY);
		((Control)this).Controls.Add((Control)(object)tbLimitNegY);
		((Control)this).Controls.Add((Control)(object)tbLimitPosX);
		((Control)this).Controls.Add((Control)(object)tbLimitNegX);
		((Control)this).Controls.Add((Control)(object)ORC);
		((Control)this).Controls.Add((Control)(object)LLC);
		((Control)this).Controls.Add((Control)(object)UnitC);
		((Control)this).Controls.Add((Control)(object)ParaC_Acc);
		((Control)this).Controls.Add((Control)(object)ParaC_Spd);
		((Control)this).Controls.Add((Control)(object)ParaC_Dist);
		((Control)this).Controls.Add((Control)(object)ParaC_Pulse);
		((Control)this).Controls.Add((Control)(object)lbParaC);
		((Control)this).Controls.Add((Control)(object)ORB);
		((Control)this).Controls.Add((Control)(object)LLB);
		((Control)this).Controls.Add((Control)(object)UnitB);
		((Control)this).Controls.Add((Control)(object)ParaB_Acc);
		((Control)this).Controls.Add((Control)(object)ParaB_Spd);
		((Control)this).Controls.Add((Control)(object)ParaB_Dist);
		((Control)this).Controls.Add((Control)(object)ParaB_Pulse);
		((Control)this).Controls.Add((Control)(object)lbParaB);
		((Control)this).Controls.Add((Control)(object)BnSaveToBord);
		((Control)this).Controls.Add((Control)(object)BnSetPara);
		((Control)this).Controls.Add((Control)(object)ORT);
		((Control)this).Controls.Add((Control)(object)ORZ);
		((Control)this).Controls.Add((Control)(object)ORY);
		((Control)this).Controls.Add((Control)(object)ORX);
		((Control)this).Controls.Add((Control)(object)LLT);
		((Control)this).Controls.Add((Control)(object)LLZ);
		((Control)this).Controls.Add((Control)(object)LLY);
		((Control)this).Controls.Add((Control)(object)LLX);
		((Control)this).Controls.Add((Control)(object)UnitT);
		((Control)this).Controls.Add((Control)(object)UnitZ);
		((Control)this).Controls.Add((Control)(object)UnitY);
		((Control)this).Controls.Add((Control)(object)UnitX);
		((Control)this).Controls.Add((Control)(object)ParaT_Acc);
		((Control)this).Controls.Add((Control)(object)ParaZ_Acc);
		((Control)this).Controls.Add((Control)(object)ParaY_Acc);
		((Control)this).Controls.Add((Control)(object)ParaX_Acc);
		((Control)this).Controls.Add((Control)(object)ParaT_Spd);
		((Control)this).Controls.Add((Control)(object)ParaZ_Spd);
		((Control)this).Controls.Add((Control)(object)ParaY_Spd);
		((Control)this).Controls.Add((Control)(object)ParaX_Spd);
		((Control)this).Controls.Add((Control)(object)label7);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)Unit);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)ParaT_Dist);
		((Control)this).Controls.Add((Control)(object)ParaZ_Dist);
		((Control)this).Controls.Add((Control)(object)ParaY_Dist);
		((Control)this).Controls.Add((Control)(object)ParaX_Dist);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)ParaT_Pulse);
		((Control)this).Controls.Add((Control)(object)ParaZ_Pulse);
		((Control)this).Controls.Add((Control)(object)lbParaA);
		((Control)this).Controls.Add((Control)(object)ParaY_Pulse);
		((Control)this).Controls.Add((Control)(object)lbParaZ);
		((Control)this).Controls.Add((Control)(object)ParaX_Pulse);
		((Control)this).Controls.Add((Control)(object)lbParaY);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)lbParaX);
		((Control)this).Controls.Add((Control)(object)label1);
		((Form)this).FormBorderStyle = (FormBorderStyle)3;
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).Name = "SetSysParaFrm";
		((Form)this).Load += SetSysParaFrm_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
