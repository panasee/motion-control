using System;
using System.ComponentModel;
using System.Windows.Forms;
using MccCustomLanguage;

namespace MCCDEMO;

public class InputFunCfgFrm : Form
{
	private MainFrm PMainFrm;

	private uint gbInputFunParaL;

	private uint gbInputFunParaH;

	private IContainer components;

	private Label lbIn00;

	private ComboBox cbInFun00;

	private ComboBox cbInFun01;

	private Label label1;

	private ComboBox cbInFun02;

	private Label label2;

	private ComboBox cbInFun03;

	private Label label3;

	private ComboBox cbInFun04;

	private Label label4;

	private ComboBox cbInFun05;

	private Label label5;

	private ComboBox cbInFun06;

	private Label label6;

	private ComboBox cbInFun07;

	private Label label7;

	private ComboBox cbInFun15;

	private Label label8;

	private ComboBox cbInFun14;

	private Label label9;

	private ComboBox cbInFun13;

	private Label label10;

	private ComboBox cbInFun12;

	private Label label11;

	private ComboBox cbInFun11;

	private Label label12;

	private ComboBox cbInFun10;

	private Label label13;

	private ComboBox cbInFun09;

	private Label label14;

	private ComboBox cbInFun08;

	private Label label15;

	private ComboBox cbInFun31;

	private Label label16;

	private ComboBox cbInFun30;

	private Label label17;

	private ComboBox cbInFun29;

	private Label label18;

	private ComboBox cbInFun28;

	private Label label19;

	private ComboBox cbInFun27;

	private Label label20;

	private ComboBox cbInFun26;

	private Label label21;

	private ComboBox cbInFun25;

	private Label label22;

	private ComboBox cbInFun24;

	private Label label23;

	private ComboBox cbInFun23;

	private Label label24;

	private ComboBox cbInFun22;

	private Label label25;

	private ComboBox cbInFun21;

	private Label label26;

	private ComboBox cbInFun20;

	private Label label27;

	private ComboBox cbInFun19;

	private Label label28;

	private ComboBox cbInFun18;

	private Label label29;

	private ComboBox cbInFun17;

	private Label label30;

	private ComboBox cbInFun16;

	private Label label31;

	private Button bnUpdate;

	private Button bnSet;

	private Button bnSaveToRom;

	public InputFunCfgFrm(object mOwner)
	{
		InitializeComponent();
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		PMainFrm = (MainFrm)mOwner;
	}

	private void fun_UpdateInputFunParaToFrm()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		uint num2 = 0u;
		uint[] array = new uint[1];
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_ReadPara(PMainFrm.Mccard.ParaIDBase_InputExFun, PMainFrm.Mccard.InputExFunParaIndex_MaskL, array))
		{
			gbInputFunParaL = array[0];
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.InputFunCfgFrm_ReadInputCfgParaError);
		}
		if (PMainFrm.Mccard.FunResOk == PMainFrm.Mccard.MoCtrCard_ReadPara(PMainFrm.Mccard.ParaIDBase_InputExFun, PMainFrm.Mccard.InputExFunParaIndex_MaskH, array))
		{
			gbInputFunParaH = array[0];
		}
		else
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.InputFunCfgFrm_ReadInputCfgParaError);
		}
		for (num = 0; num < 32; num++)
		{
			if (num < 16)
			{
				num2 = gbInputFunParaL;
				num2 >>= num << 1;
			}
			else
			{
				num2 = gbInputFunParaH;
				num2 >>= num - 16 << 1;
			}
			string text = $"cbInFun{num:D2}";
			Control[] array2 = ((Control)this).Controls.Find(text, false);
			if (array2.Length != 0)
			{
				int selectedIndex = (int)(num2 & 3);
				((ListControl)(ComboBox)array2[0]).SelectedIndex = selectedIndex;
			}
		}
	}

	private void InputFunCfgFrm_Load(object sender, EventArgs e)
	{
		fun_UpdateInputFunParaToFrm();
	}

	private void bnUpdate_Click(object sender, EventArgs e)
	{
		fun_UpdateInputFunParaToFrm();
	}

	private void bnSet_Click(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		uint num2 = 0u;
		gbInputFunParaL = 0u;
		gbInputFunParaH = 0u;
		for (num = 0; num < 32; num++)
		{
			string text = $"cbInFun{num:D2}";
			Control[] array = ((Control)this).Controls.Find(text, false);
			if (array.Length != 0)
			{
				num2 = (uint)((ListControl)(ComboBox)array[0]).SelectedIndex;
			}
			if (num < 16)
			{
				num2 <<= num << 1;
				gbInputFunParaL |= num2;
			}
			else
			{
				num2 <<= num - 16 << 1;
				gbInputFunParaH |= num2;
			}
		}
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(PMainFrm.Mccard.ParaIDBase_InputExFun, PMainFrm.Mccard.InputExFunParaIndex_MaskL, gbInputFunParaL))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.InputFunCfgFrm_SetLow16BitsInputFunFailed);
		}
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SendPara(PMainFrm.Mccard.ParaIDBase_InputExFun, PMainFrm.Mccard.InputExFunParaIndex_MaskH, gbInputFunParaH))
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.InputFunCfgFrm_SetHigh16BitsInputFunFailed);
		}
	}

	private void bnSaveToRom_Click(object sender, EventArgs e)
	{
		if (PMainFrm.Mccard.FunResErr == PMainFrm.Mccard.MoCtrCard_SaveSystemParaToROM())
		{
			PMainFrm.fun_PrintfInfo(MainFrm.eDebugLevelType.eDebugLevErr, GlobalData.GlobalLanguage.InputFunCfgFrm_SaveParameterToControllerFailed);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(InputFunCfgFrm));
		lbIn00 = new Label();
		cbInFun00 = new ComboBox();
		cbInFun01 = new ComboBox();
		label1 = new Label();
		cbInFun02 = new ComboBox();
		label2 = new Label();
		cbInFun03 = new ComboBox();
		label3 = new Label();
		cbInFun04 = new ComboBox();
		label4 = new Label();
		cbInFun05 = new ComboBox();
		label5 = new Label();
		cbInFun06 = new ComboBox();
		label6 = new Label();
		cbInFun07 = new ComboBox();
		label7 = new Label();
		cbInFun15 = new ComboBox();
		label8 = new Label();
		cbInFun14 = new ComboBox();
		label9 = new Label();
		cbInFun13 = new ComboBox();
		label10 = new Label();
		cbInFun12 = new ComboBox();
		label11 = new Label();
		cbInFun11 = new ComboBox();
		label12 = new Label();
		cbInFun10 = new ComboBox();
		label13 = new Label();
		cbInFun09 = new ComboBox();
		label14 = new Label();
		cbInFun08 = new ComboBox();
		label15 = new Label();
		cbInFun31 = new ComboBox();
		label16 = new Label();
		cbInFun30 = new ComboBox();
		label17 = new Label();
		cbInFun29 = new ComboBox();
		label18 = new Label();
		cbInFun28 = new ComboBox();
		label19 = new Label();
		cbInFun27 = new ComboBox();
		label20 = new Label();
		cbInFun26 = new ComboBox();
		label21 = new Label();
		cbInFun25 = new ComboBox();
		label22 = new Label();
		cbInFun24 = new ComboBox();
		label23 = new Label();
		cbInFun23 = new ComboBox();
		label24 = new Label();
		cbInFun22 = new ComboBox();
		label25 = new Label();
		cbInFun21 = new ComboBox();
		label26 = new Label();
		cbInFun20 = new ComboBox();
		label27 = new Label();
		cbInFun19 = new ComboBox();
		label28 = new Label();
		cbInFun18 = new ComboBox();
		label29 = new Label();
		cbInFun17 = new ComboBox();
		label30 = new Label();
		cbInFun16 = new ComboBox();
		label31 = new Label();
		bnUpdate = new Button();
		bnSet = new Button();
		bnSaveToRom = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(lbIn00, "lbIn00");
		((Control)lbIn00).Name = "lbIn00";
		((ListControl)cbInFun00).FormattingEnabled = true;
		cbInFun00.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun00.Items"),
			componentResourceManager.GetString("cbInFun00.Items1"),
			componentResourceManager.GetString("cbInFun00.Items2"),
			componentResourceManager.GetString("cbInFun00.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun00, "cbInFun00");
		((Control)cbInFun00).Name = "cbInFun00";
		((ListControl)cbInFun01).FormattingEnabled = true;
		cbInFun01.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun01.Items"),
			componentResourceManager.GetString("cbInFun01.Items1"),
			componentResourceManager.GetString("cbInFun01.Items2"),
			componentResourceManager.GetString("cbInFun01.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun01, "cbInFun01");
		((Control)cbInFun01).Name = "cbInFun01";
		componentResourceManager.ApplyResources(label1, "label1");
		((Control)label1).Name = "label1";
		((ListControl)cbInFun02).FormattingEnabled = true;
		cbInFun02.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun02.Items"),
			componentResourceManager.GetString("cbInFun02.Items1"),
			componentResourceManager.GetString("cbInFun02.Items2"),
			componentResourceManager.GetString("cbInFun02.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun02, "cbInFun02");
		((Control)cbInFun02).Name = "cbInFun02";
		componentResourceManager.ApplyResources(label2, "label2");
		((Control)label2).Name = "label2";
		((ListControl)cbInFun03).FormattingEnabled = true;
		cbInFun03.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun03.Items"),
			componentResourceManager.GetString("cbInFun03.Items1"),
			componentResourceManager.GetString("cbInFun03.Items2"),
			componentResourceManager.GetString("cbInFun03.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun03, "cbInFun03");
		((Control)cbInFun03).Name = "cbInFun03";
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)label3).Name = "label3";
		((ListControl)cbInFun04).FormattingEnabled = true;
		cbInFun04.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun04.Items"),
			componentResourceManager.GetString("cbInFun04.Items1"),
			componentResourceManager.GetString("cbInFun04.Items2"),
			componentResourceManager.GetString("cbInFun04.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun04, "cbInFun04");
		((Control)cbInFun04).Name = "cbInFun04";
		componentResourceManager.ApplyResources(label4, "label4");
		((Control)label4).Name = "label4";
		((ListControl)cbInFun05).FormattingEnabled = true;
		cbInFun05.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun05.Items"),
			componentResourceManager.GetString("cbInFun05.Items1"),
			componentResourceManager.GetString("cbInFun05.Items2"),
			componentResourceManager.GetString("cbInFun05.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun05, "cbInFun05");
		((Control)cbInFun05).Name = "cbInFun05";
		componentResourceManager.ApplyResources(label5, "label5");
		((Control)label5).Name = "label5";
		((ListControl)cbInFun06).FormattingEnabled = true;
		cbInFun06.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun06.Items"),
			componentResourceManager.GetString("cbInFun06.Items1"),
			componentResourceManager.GetString("cbInFun06.Items2"),
			componentResourceManager.GetString("cbInFun06.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun06, "cbInFun06");
		((Control)cbInFun06).Name = "cbInFun06";
		componentResourceManager.ApplyResources(label6, "label6");
		((Control)label6).Name = "label6";
		((ListControl)cbInFun07).FormattingEnabled = true;
		cbInFun07.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun07.Items"),
			componentResourceManager.GetString("cbInFun07.Items1"),
			componentResourceManager.GetString("cbInFun07.Items2"),
			componentResourceManager.GetString("cbInFun07.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun07, "cbInFun07");
		((Control)cbInFun07).Name = "cbInFun07";
		componentResourceManager.ApplyResources(label7, "label7");
		((Control)label7).Name = "label7";
		((ListControl)cbInFun15).FormattingEnabled = true;
		cbInFun15.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun15.Items"),
			componentResourceManager.GetString("cbInFun15.Items1"),
			componentResourceManager.GetString("cbInFun15.Items2"),
			componentResourceManager.GetString("cbInFun15.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun15, "cbInFun15");
		((Control)cbInFun15).Name = "cbInFun15";
		componentResourceManager.ApplyResources(label8, "label8");
		((Control)label8).Name = "label8";
		((ListControl)cbInFun14).FormattingEnabled = true;
		cbInFun14.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun14.Items"),
			componentResourceManager.GetString("cbInFun14.Items1"),
			componentResourceManager.GetString("cbInFun14.Items2"),
			componentResourceManager.GetString("cbInFun14.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun14, "cbInFun14");
		((Control)cbInFun14).Name = "cbInFun14";
		componentResourceManager.ApplyResources(label9, "label9");
		((Control)label9).Name = "label9";
		((ListControl)cbInFun13).FormattingEnabled = true;
		cbInFun13.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun13.Items"),
			componentResourceManager.GetString("cbInFun13.Items1"),
			componentResourceManager.GetString("cbInFun13.Items2"),
			componentResourceManager.GetString("cbInFun13.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun13, "cbInFun13");
		((Control)cbInFun13).Name = "cbInFun13";
		componentResourceManager.ApplyResources(label10, "label10");
		((Control)label10).Name = "label10";
		((ListControl)cbInFun12).FormattingEnabled = true;
		cbInFun12.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun12.Items"),
			componentResourceManager.GetString("cbInFun12.Items1"),
			componentResourceManager.GetString("cbInFun12.Items2"),
			componentResourceManager.GetString("cbInFun12.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun12, "cbInFun12");
		((Control)cbInFun12).Name = "cbInFun12";
		componentResourceManager.ApplyResources(label11, "label11");
		((Control)label11).Name = "label11";
		((ListControl)cbInFun11).FormattingEnabled = true;
		cbInFun11.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun11.Items"),
			componentResourceManager.GetString("cbInFun11.Items1"),
			componentResourceManager.GetString("cbInFun11.Items2"),
			componentResourceManager.GetString("cbInFun11.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun11, "cbInFun11");
		((Control)cbInFun11).Name = "cbInFun11";
		componentResourceManager.ApplyResources(label12, "label12");
		((Control)label12).Name = "label12";
		((ListControl)cbInFun10).FormattingEnabled = true;
		cbInFun10.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun10.Items"),
			componentResourceManager.GetString("cbInFun10.Items1"),
			componentResourceManager.GetString("cbInFun10.Items2"),
			componentResourceManager.GetString("cbInFun10.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun10, "cbInFun10");
		((Control)cbInFun10).Name = "cbInFun10";
		componentResourceManager.ApplyResources(label13, "label13");
		((Control)label13).Name = "label13";
		((ListControl)cbInFun09).FormattingEnabled = true;
		cbInFun09.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun09.Items"),
			componentResourceManager.GetString("cbInFun09.Items1"),
			componentResourceManager.GetString("cbInFun09.Items2"),
			componentResourceManager.GetString("cbInFun09.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun09, "cbInFun09");
		((Control)cbInFun09).Name = "cbInFun09";
		componentResourceManager.ApplyResources(label14, "label14");
		((Control)label14).Name = "label14";
		((ListControl)cbInFun08).FormattingEnabled = true;
		cbInFun08.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun08.Items"),
			componentResourceManager.GetString("cbInFun08.Items1"),
			componentResourceManager.GetString("cbInFun08.Items2"),
			componentResourceManager.GetString("cbInFun08.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun08, "cbInFun08");
		((Control)cbInFun08).Name = "cbInFun08";
		componentResourceManager.ApplyResources(label15, "label15");
		((Control)label15).Name = "label15";
		((ListControl)cbInFun31).FormattingEnabled = true;
		cbInFun31.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun31.Items"),
			componentResourceManager.GetString("cbInFun31.Items1"),
			componentResourceManager.GetString("cbInFun31.Items2"),
			componentResourceManager.GetString("cbInFun31.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun31, "cbInFun31");
		((Control)cbInFun31).Name = "cbInFun31";
		componentResourceManager.ApplyResources(label16, "label16");
		((Control)label16).Name = "label16";
		((ListControl)cbInFun30).FormattingEnabled = true;
		cbInFun30.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun30.Items"),
			componentResourceManager.GetString("cbInFun30.Items1"),
			componentResourceManager.GetString("cbInFun30.Items2"),
			componentResourceManager.GetString("cbInFun30.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun30, "cbInFun30");
		((Control)cbInFun30).Name = "cbInFun30";
		componentResourceManager.ApplyResources(label17, "label17");
		((Control)label17).Name = "label17";
		((ListControl)cbInFun29).FormattingEnabled = true;
		cbInFun29.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun29.Items"),
			componentResourceManager.GetString("cbInFun29.Items1"),
			componentResourceManager.GetString("cbInFun29.Items2"),
			componentResourceManager.GetString("cbInFun29.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun29, "cbInFun29");
		((Control)cbInFun29).Name = "cbInFun29";
		componentResourceManager.ApplyResources(label18, "label18");
		((Control)label18).Name = "label18";
		((ListControl)cbInFun28).FormattingEnabled = true;
		cbInFun28.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun28.Items"),
			componentResourceManager.GetString("cbInFun28.Items1"),
			componentResourceManager.GetString("cbInFun28.Items2"),
			componentResourceManager.GetString("cbInFun28.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun28, "cbInFun28");
		((Control)cbInFun28).Name = "cbInFun28";
		componentResourceManager.ApplyResources(label19, "label19");
		((Control)label19).Name = "label19";
		((ListControl)cbInFun27).FormattingEnabled = true;
		cbInFun27.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun27.Items"),
			componentResourceManager.GetString("cbInFun27.Items1"),
			componentResourceManager.GetString("cbInFun27.Items2"),
			componentResourceManager.GetString("cbInFun27.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun27, "cbInFun27");
		((Control)cbInFun27).Name = "cbInFun27";
		componentResourceManager.ApplyResources(label20, "label20");
		((Control)label20).Name = "label20";
		((ListControl)cbInFun26).FormattingEnabled = true;
		cbInFun26.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun26.Items"),
			componentResourceManager.GetString("cbInFun26.Items1"),
			componentResourceManager.GetString("cbInFun26.Items2"),
			componentResourceManager.GetString("cbInFun26.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun26, "cbInFun26");
		((Control)cbInFun26).Name = "cbInFun26";
		componentResourceManager.ApplyResources(label21, "label21");
		((Control)label21).Name = "label21";
		((ListControl)cbInFun25).FormattingEnabled = true;
		cbInFun25.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun25.Items"),
			componentResourceManager.GetString("cbInFun25.Items1"),
			componentResourceManager.GetString("cbInFun25.Items2"),
			componentResourceManager.GetString("cbInFun25.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun25, "cbInFun25");
		((Control)cbInFun25).Name = "cbInFun25";
		componentResourceManager.ApplyResources(label22, "label22");
		((Control)label22).Name = "label22";
		((ListControl)cbInFun24).FormattingEnabled = true;
		cbInFun24.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun24.Items"),
			componentResourceManager.GetString("cbInFun24.Items1"),
			componentResourceManager.GetString("cbInFun24.Items2"),
			componentResourceManager.GetString("cbInFun24.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun24, "cbInFun24");
		((Control)cbInFun24).Name = "cbInFun24";
		componentResourceManager.ApplyResources(label23, "label23");
		((Control)label23).Name = "label23";
		((ListControl)cbInFun23).FormattingEnabled = true;
		cbInFun23.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun23.Items"),
			componentResourceManager.GetString("cbInFun23.Items1"),
			componentResourceManager.GetString("cbInFun23.Items2"),
			componentResourceManager.GetString("cbInFun23.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun23, "cbInFun23");
		((Control)cbInFun23).Name = "cbInFun23";
		componentResourceManager.ApplyResources(label24, "label24");
		((Control)label24).Name = "label24";
		((ListControl)cbInFun22).FormattingEnabled = true;
		cbInFun22.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun22.Items"),
			componentResourceManager.GetString("cbInFun22.Items1"),
			componentResourceManager.GetString("cbInFun22.Items2"),
			componentResourceManager.GetString("cbInFun22.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun22, "cbInFun22");
		((Control)cbInFun22).Name = "cbInFun22";
		componentResourceManager.ApplyResources(label25, "label25");
		((Control)label25).Name = "label25";
		((ListControl)cbInFun21).FormattingEnabled = true;
		cbInFun21.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun21.Items"),
			componentResourceManager.GetString("cbInFun21.Items1"),
			componentResourceManager.GetString("cbInFun21.Items2"),
			componentResourceManager.GetString("cbInFun21.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun21, "cbInFun21");
		((Control)cbInFun21).Name = "cbInFun21";
		componentResourceManager.ApplyResources(label26, "label26");
		((Control)label26).Name = "label26";
		((ListControl)cbInFun20).FormattingEnabled = true;
		cbInFun20.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun20.Items"),
			componentResourceManager.GetString("cbInFun20.Items1"),
			componentResourceManager.GetString("cbInFun20.Items2"),
			componentResourceManager.GetString("cbInFun20.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun20, "cbInFun20");
		((Control)cbInFun20).Name = "cbInFun20";
		componentResourceManager.ApplyResources(label27, "label27");
		((Control)label27).Name = "label27";
		((ListControl)cbInFun19).FormattingEnabled = true;
		cbInFun19.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun19.Items"),
			componentResourceManager.GetString("cbInFun19.Items1"),
			componentResourceManager.GetString("cbInFun19.Items2"),
			componentResourceManager.GetString("cbInFun19.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun19, "cbInFun19");
		((Control)cbInFun19).Name = "cbInFun19";
		componentResourceManager.ApplyResources(label28, "label28");
		((Control)label28).Name = "label28";
		((ListControl)cbInFun18).FormattingEnabled = true;
		cbInFun18.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun18.Items"),
			componentResourceManager.GetString("cbInFun18.Items1"),
			componentResourceManager.GetString("cbInFun18.Items2"),
			componentResourceManager.GetString("cbInFun18.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun18, "cbInFun18");
		((Control)cbInFun18).Name = "cbInFun18";
		componentResourceManager.ApplyResources(label29, "label29");
		((Control)label29).Name = "label29";
		((ListControl)cbInFun17).FormattingEnabled = true;
		cbInFun17.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun17.Items"),
			componentResourceManager.GetString("cbInFun17.Items1"),
			componentResourceManager.GetString("cbInFun17.Items2"),
			componentResourceManager.GetString("cbInFun17.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun17, "cbInFun17");
		((Control)cbInFun17).Name = "cbInFun17";
		componentResourceManager.ApplyResources(label30, "label30");
		((Control)label30).Name = "label30";
		((ListControl)cbInFun16).FormattingEnabled = true;
		cbInFun16.Items.AddRange(new object[4]
		{
			componentResourceManager.GetString("cbInFun16.Items"),
			componentResourceManager.GetString("cbInFun16.Items1"),
			componentResourceManager.GetString("cbInFun16.Items2"),
			componentResourceManager.GetString("cbInFun16.Items3")
		});
		componentResourceManager.ApplyResources(cbInFun16, "cbInFun16");
		((Control)cbInFun16).Name = "cbInFun16";
		componentResourceManager.ApplyResources(label31, "label31");
		((Control)label31).Name = "label31";
		componentResourceManager.ApplyResources(bnUpdate, "bnUpdate");
		((Control)bnUpdate).Name = "bnUpdate";
		((ButtonBase)bnUpdate).UseVisualStyleBackColor = true;
		((Control)bnUpdate).Click += bnUpdate_Click;
		componentResourceManager.ApplyResources(bnSet, "bnSet");
		((Control)bnSet).Name = "bnSet";
		((ButtonBase)bnSet).UseVisualStyleBackColor = true;
		((Control)bnSet).Click += bnSet_Click;
		componentResourceManager.ApplyResources(bnSaveToRom, "bnSaveToRom");
		((Control)bnSaveToRom).Name = "bnSaveToRom";
		((ButtonBase)bnSaveToRom).UseVisualStyleBackColor = true;
		((Control)bnSaveToRom).Click += bnSaveToRom_Click;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)bnSaveToRom);
		((Control)this).Controls.Add((Control)(object)bnSet);
		((Control)this).Controls.Add((Control)(object)bnUpdate);
		((Control)this).Controls.Add((Control)(object)cbInFun31);
		((Control)this).Controls.Add((Control)(object)label16);
		((Control)this).Controls.Add((Control)(object)cbInFun30);
		((Control)this).Controls.Add((Control)(object)label17);
		((Control)this).Controls.Add((Control)(object)cbInFun29);
		((Control)this).Controls.Add((Control)(object)label18);
		((Control)this).Controls.Add((Control)(object)cbInFun28);
		((Control)this).Controls.Add((Control)(object)label19);
		((Control)this).Controls.Add((Control)(object)cbInFun27);
		((Control)this).Controls.Add((Control)(object)label20);
		((Control)this).Controls.Add((Control)(object)cbInFun26);
		((Control)this).Controls.Add((Control)(object)label21);
		((Control)this).Controls.Add((Control)(object)cbInFun25);
		((Control)this).Controls.Add((Control)(object)label22);
		((Control)this).Controls.Add((Control)(object)cbInFun24);
		((Control)this).Controls.Add((Control)(object)label23);
		((Control)this).Controls.Add((Control)(object)cbInFun23);
		((Control)this).Controls.Add((Control)(object)label24);
		((Control)this).Controls.Add((Control)(object)cbInFun22);
		((Control)this).Controls.Add((Control)(object)label25);
		((Control)this).Controls.Add((Control)(object)cbInFun21);
		((Control)this).Controls.Add((Control)(object)label26);
		((Control)this).Controls.Add((Control)(object)cbInFun20);
		((Control)this).Controls.Add((Control)(object)label27);
		((Control)this).Controls.Add((Control)(object)cbInFun19);
		((Control)this).Controls.Add((Control)(object)label28);
		((Control)this).Controls.Add((Control)(object)cbInFun18);
		((Control)this).Controls.Add((Control)(object)label29);
		((Control)this).Controls.Add((Control)(object)cbInFun17);
		((Control)this).Controls.Add((Control)(object)label30);
		((Control)this).Controls.Add((Control)(object)cbInFun16);
		((Control)this).Controls.Add((Control)(object)label31);
		((Control)this).Controls.Add((Control)(object)cbInFun15);
		((Control)this).Controls.Add((Control)(object)label8);
		((Control)this).Controls.Add((Control)(object)cbInFun14);
		((Control)this).Controls.Add((Control)(object)label9);
		((Control)this).Controls.Add((Control)(object)cbInFun13);
		((Control)this).Controls.Add((Control)(object)label10);
		((Control)this).Controls.Add((Control)(object)cbInFun12);
		((Control)this).Controls.Add((Control)(object)label11);
		((Control)this).Controls.Add((Control)(object)cbInFun11);
		((Control)this).Controls.Add((Control)(object)label12);
		((Control)this).Controls.Add((Control)(object)cbInFun10);
		((Control)this).Controls.Add((Control)(object)label13);
		((Control)this).Controls.Add((Control)(object)cbInFun09);
		((Control)this).Controls.Add((Control)(object)label14);
		((Control)this).Controls.Add((Control)(object)cbInFun08);
		((Control)this).Controls.Add((Control)(object)label15);
		((Control)this).Controls.Add((Control)(object)cbInFun07);
		((Control)this).Controls.Add((Control)(object)label7);
		((Control)this).Controls.Add((Control)(object)cbInFun06);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)cbInFun05);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)cbInFun04);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)cbInFun03);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)cbInFun02);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)cbInFun01);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)cbInFun00);
		((Control)this).Controls.Add((Control)(object)lbIn00);
		((Control)this).Name = "InputFunCfgFrm";
		((Form)this).Load += InputFunCfgFrm_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
