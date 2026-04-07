# MCCDEMO6 传动控制软件分析报告

**分析日期**: 2026-03-28  
**软件版本**: MCCDEMO6_ENG  
**分析对象**: 运动控制卡(MCC - Motion Control Card)上位机控制软件

---

## 1. 软件概述

MCCDEMO6 是一款 Windows 平台下的运动控制卡(Motion Control Card)上位机软件,用于控制多轴运动控制系统。该软件通过 TCP/IP 或 RS232 串口与硬件控制卡通信,实现:

- 多轴(最多6轴)运动控制
- 手动/自动运动模式
- 电子齿轮与凸轮曲线
- 位置补偿与平台调平
- 示波器数据采集
- 步进驱动器(TMC2209)配置
- Lua脚本扩展
- G代码/MDI指令执行

---

## 2. 软件架构

### 2.1 组件结构

```
MCCDEMO6_ENG/
├── MCCDEMO.exe          # 主程序 (C# WinForms)
├── MCC6DLL.dll          # 核心通信与控制库
├── LinearAlgebra.dll    # 线性代数运算库
├── ZedGraph.dll         # 图表绘制库
├── SystemPara.xml       # 系统参数配置
├── Languages/           # 多语言支持
│   ├── Chinese.xml
│   ├── English.xml
│   ├── Languages.cs     # 语言加载类
│   └── GloableData.cs   # 全局数据类
└── app.publish/         # ClickOnce部署
```

### 2.2 技术栈

- **开发框架**: .NET Framework 4.0 Client Profile
- **UI框架**: Windows Forms (WinForms)
- **图表库**: ZedGraph (开源.NET图表库)
- **部署方式**: ClickOnce 应用

---

## 3. 硬件通信接口

### 3.1 通信方式

软件支持两种通信方式:

| 通信方式 | 配置参数 | 默认值 |
|---------|---------|-------|
| **TCP/IP** | TCPIPAddress | 192.168.0.7 |
|           | TCPPort | 8888 |
| **RS232串口** | RS232Port | COM3 |

### 3.2 通信接口函数 (MCC6DLL.dll)

#### TCP通信
```
Fun_CreateLink_TCP()   - 创建TCP连接
Fun_CloseLink_TCP()    - 关闭TCP连接
Fun_SendBuffer_TCP()   - 发送数据缓冲区
Fun_GetBuffer_TCP()    - 接收数据缓冲区
```

#### RS232串口通信
```
Fun_CreateLink_RS232() - 创建串口连接
Fun_CloseLink_RS232()  - 关闭串口连接
Fun_SendBuffer_RS232() - 发送数据缓冲区
Fun_GetBuffer_RS232()  - 接收数据缓冲区
```

#### 通用接口(自动适配通信方式)
```
Fun_CreateLink()       - 创建连接(根据配置自动选择TCP或RS232)
Fun_CloseLink()        - 关闭连接
Fun_SendBuffer()       - 发送数据
Fun_GetBuffer()        - 接收数据
Fun_DeCodeCmmndState() - 解码命令状态
Fun_PrintInfo()        - 打印调试信息
```

### 3.3 通信端口类型枚举
```
eCommPortNull    = 0  // 无连接
eCommPortRS232   = 1  // RS232串口
eCommPortTCP     = 2  // TCP/IP网络
```

---

## 4. 命令帧结构（精确字节级定义）

### 4.1 基本帧头结构（8字节）

```c
struct stCmndHeadType {
    byte u8SpCom_Headr;     // 帧头 = 0xAA (170)
    byte u8SpCom_DtLen;     // 数据长度（不含帧头8字节）
    byte u8SpCom_CardIndx;  // 卡索引（默认0）
    byte u8SpCom_Check;     // 校验 = ~u8SpCom_DtLen（长度取反）
    byte u8SpDt_CheckSum;   // 校验和（从第5字节开始累加）
    byte u8SpDt_CmndType;   // 命令类型
    byte u8SpDt_Len;        // 数据长度（同u8SpCom_DtLen）
    byte u8SpDt_CmndNo;     // 命令号（通常为0）
}
```

### 4.2 帧格式图示

```
字节位置:  0    1    2    3    4    5    6    7    8...N
         ┌────┬────┬────┬────┬────┬────┬────┬────┬─────────┐
         │0xAA│Len │Card│~Len│CSum│Cmd │Len │CmdN│ Data... │
         └────┴────┴────┴────┴────┴────┴────┴────┴─────────┘
           ↑         ↑         ↑    ↑
         帧头      卡索引    校验和 命令类型
```

### 4.3 校验和计算方法

```csharp
// 校验和 = 从第5字节开始累加所有字节，取低8位
uint sum = 0;
for (int i = 4; i < totalLen; i++) {
    sum += frame[i];
}
frame[4] = (byte)sum;
```

### 4.4 命令类型字节码（精确值）

| 命令类型 | 十六进制 | 十进制 | 功能描述 |
|---------|---------|-------|---------|
| `CMMND_ProfileMove_Cmnd` | 0x80 | 128 | 凸轮/曲线运动 |
| `Cmnd_Scope_Cmnd` | 0x81 | 129 | 示波器 |
| `CMMND_OFFLINERUN_PARA` | 0x88 | 136 | 离线运行参数 |
| `CMMND_OFFLINERUN_START` | 0x89 | 137 | 启动离线运行 |
| `CMMND_OFFLINERUN_STOP` | 0x8A | 138 | 停止离线运行 |
| `CMMND_OFFLINERUN_CLEAR` | 0x8B | 139 | 清除离线运行 |
| `CMMND_MOVEPULSE` | 0x8C | 140 | 脉冲运动 |
| `CMMND_MOVESINE` | 0x8D | 141 | 正弦运动 |
| `CMMND_COMPENSATE` | 0x8E | 142 | 补偿操作 |
| `CMMND_EXPOS_FOLLOW` | 0x8F | 143 | 外部位置跟随 |
| `CMMND_TMC2209` | 0x40 | 64 | TMC2209驱动操作 |
| `CMMND_SETGEAR_PARA` | 0xE0 | 224 | 设置齿轮参数 |
| `CMMND_GEAR_OP` | 0xE1 | 225 | 齿轮操作 |
| `CMMND_SINPOS_MOVE` | 0xE2 | 226 | 正弦位置运动 |
| `CMMND_LUA` | 0xE3 | 227 | Lua脚本 |
| `CMMND_SPIFLASH` | 0xE4 | 228 | SPI Flash操作 |
| `CMMND_DS402HOME` | 0xE8 | 232 | DS402回零 |
| `CMMND_PLATFORM_COM` | 0xE9 | 233 | 平台补偿 |
| `CMMND_MDICMND` | 0xF1 | 241 | MDI命令 |
| `CMMND_SETPARA` | 0xF2 | 242 | 设置参数 |
| `CMMND_GCODECMND` | 0xF3 | 243 | G代码命令 |
| `CMMND_MANUAL_OP` | 0xF4 | 244 | 手动操作 |
| `CMMND_ASKPARA` | 0xF5 | 245 | 请求参数 |
| `CMMND_ASKMOVEINFO` | 0xF6 | 246 | 请求运动信息 |
| `CMMND_SYSTEM_OP` | 0xF7 | 247 | 系统操作 |
| `CMMND_RESETPOS` | 0xF8 | 248 | 复位位置 |
| `CMMND_SETOUTPUT` | 0xF9 | 249 | 设置输出 |
| `CMMND_ANALOGOP` | 0xFA | 250 | 模拟量操作 |
| `CMMND_SETINPUTLEV` | 0xFB | 251 | 设置输入电平 |
| `CMMND_SETENCODER` | 0xFC | 252 | 设置编码器 |
| `CMMND_RESETZ` | 0xFD | 253 | Z轴复位 |
| `CMMND_SAVESYSPARA` | 0xFE | 254 | 保存系统参数 |

### 4.5 手动操作子命令（MANOP_*）

| 子命令 | 十六进制 | 功能 |
|-------|---------|------|
| `MANOP_MOVEINNERDIST` | 0x00 | 内部距离运动 |
| `MANOP_MOVERELDISTNOPARA` | 0x01 | 相对运动（无参数） |
| `MANOP_MOVEABSDISTNOPARA` | 0x02 | 绝对运动（无参数） |
| `MANOP_MOVERELDIST` | 0x03 | 相对运动 |
| `MANOP_MOVEABSDIST` | 0x04 | 绝对运动 |
| `MANOP_SPDMOVENOPARA` | 0x05 | 速度运动（无参数） |
| `MANOP_SPDMOVE` | 0x06 | 速度运动 |
| `MANOP_MOVECHAGEVEL` | 0x07 | 变速 |
| `MANOP_SPDMOVEWITHINTIME` | 0x08 | 定时速度运动 |
| `MANOP_MOVEABS_SYNCEXIN` | 0x09 | 同步输入绝对运动 |
| `MANOP_MOVEREL_SYNCEXIN` | 0x0A | 同步输入相对运动 |
| `MANOP_UPDATECOMPENSATE` | 0x20 | 更新补偿 |
| `MANOP_EMGERNCYSTOP` | 0x80 | 急停 |
| `MANOP_PARASTOP` | 0x81 | 参数停止 |
| `MANOP_PAUSE` | 0x82 | 暂停 |
| `MANOP_RESTARTMOVE` | 0x90 | 重启运动 |
| `MANOP_QUITEMOVE` | 0x91 | 退出运动 |
| `MANOP_HOMINGNOPARA` | 0xA0 | 回零（无参数） |
| `MANOP_HOMING` | 0xA1 | 回零 |
| `MANOP_HOMINGCANCLE` | 0xA2 | 取消回零 |
| `MANOP_HOMINGWITHINDIST` | 0xA3 | 限距回零 |
| `MANOP_HOMINGWITHSWIPT` | 0xA4 | 开关回零 |

### 4.6 信息查询索引（AskInfo_*）

| 索引 | 十六进制 | 功能 |
|-----|---------|------|
| `AskInfo_Pos_Each` | 0x00 | 单轴位置 |
| `AskInfo_Spd_Each` | 0x01 | 单轴速度 |
| `AskInfo_Ana_Each` | 0x02 | 单轴模拟量 |
| `AskInfo_Encdr_Each` | 0x03 | 单轴编码器 |
| `AskInfo_HomStt_Each` | 0x04 | 单轴回零状态 |
| `AskInfo_Input_L` | 0x05 | 低16位输入 |
| `AskInfo_Input_H` | 0x06 | 高16位输入 |
| `AskInfo_Output_L` | 0x07 | 低16位输出 |
| `AskInfo_Output_H` | 0x08 | 高16位输出 |
| `AskInfo_Intrupt_L` | 0x09 | 低16位中断输入 |
| `AskInfo_Intrupt_H` | 0x0A | 高16位中断输入 |
| `AskInfo_NcStt` | 0x0B | NC状态 |
| `AskInfo_HomeStt` | 0x0C | 回零状态 |
| `AskInfo_ComDiff_Each` | 0x0D | 单轴补偿差 |
| `AskInfo_LimitStt_Each` | 0x0E | 单轴限位状态 |
| `AskInfo_OfflineMove` | 0x20 | 离线运动状态 |
| `AskInfo_MovePulseCnt` | 0x21 | 运动脉冲计数 |
| `AskInfo_MovePulsePos` | 0x22 | 运动脉冲位置 |
| `AskInfo_MovePulseInfo` | 0x23 | 运动脉冲信息 |
| `AskInfo_Pos_Grp` | 0x80 | 组位置 |
| `AskInfo_Spd_Grp` | 0x81 | 组速度 |
| `AskInfo_Ana_Grp` | 0x82 | 组模拟量 |
| `AskInfo_Encdr_Grp` | 0x83 | 组编码器 |
| `AskInfo_ComDiff_Grp` | 0x84 | 组补偿差 |
| `AskInfo_LimitStt_Grp` | 0x85 | 组限位状态 |
| `AskInfo_CustomInfo_wy1` | 0xB0 | 自定义信息 |
| `AskInfo_McuId` | 0xE0 | MCU ID |
| `AskInfo_AllCfgInfo` | 0xFE | 所有配置信息 |
| `AskInfo_Version` | 0xFF | 版本信息 |

### 4.7 返回帧结构（14字节头 + 数据）

```c
struct stUploadNcSttType {
    byte u8CmndtType;        // 命令类型
    byte u8SubCmnd;          // 子命令
    byte u8Len;              // 数据长度
    byte u8CmndStt;          // 命令状态（0=成功）
    stReturnValType Val[64]; // 返回值数组
}

struct stReturnValType {
    float fVal;    // 浮点值
    int IVal;      // 整数值
    uint IUVal;    // 无符号整数值
    short IWVal1;  // 第一个字
    short IWVal2;  // 第二个字
}
```

### 4.8 关键常量

| 常量 | 值 | 含义 |
|-----|---|------|
| `SPCOMFRAMEHEADR` | 0xAA (170) | 帧头 |
| `FRAME_TAIL` | 0x5A (90) | 帧尾（未使用） |
| `INPUT_CMMND_SIZE` | 12 | 最小输入帧大小 |
| `OUTPUT_CMMND_SIZE` | 14 | 最小输出帧大小 |
| `MAX_AXIS_NUM` | 6 | 最大轴数 |
| `PARA_NUM_PER_AXIS` | 16 | 每轴参数数 |
| `PARA_SUM` | 96 | 总参数数 |

#### 主要命令类型枚举

| 命令类型 | 功能描述 |
|---------|---------|
| `CMMND_ASKPARA` | 请求参数 |
| `CMMND_SETPARA` | 设置参数 |
| `CMMND_SAVESYSPARA` | 保存系统参数到ROM |
| `CMMND_RESETPOS` | 复位位置 |
| `CMMND_RESETZ` | Z轴复位 |
| `CMMND_MANUAL_OP` | 手动操作 |
| `CMMND_SYSTEM_OP` | 系统操作 |
| `CMMND_GEAR_OP` | 电子齿轮操作 |
| `CMMND_GCODECMND` | G代码命令 |
| `CMMND_MDICMND` | MDI命令 |
| `CMMND_LUA` | Lua脚本执行 |
| `CMMND_OFFLINERUN_START` | 启动离线运行 |
| `CMMND_OFFLINERUN_STOP` | 停止离线运行 |
| `CMMND_OFFLINERUN_PARA` | 离线运行参数 |
| `CMMND_OFFLINERUN_CLEAR` | 清除离线运行 |
| `CMMND_COMPENSATE` | 补偿操作 |
| `CMMND_PLATFORM_COM` | 平台补偿 |
| `CMMND_SETOUTPUT` | 设置输出 |
| `CMMND_SETINPUTLEV` | 设置输入电平 |
| `CMMND_SETENCODER` | 设置编码器 |
| `CMMND_SETGEAR_PARA` | 设置齿轮参数 |
| `CMMND_SINPOS_MOVE` | 正弦位置运动 |
| `CMMND_MOVESINE` | 正弦运动 |
| `CMMND_MOVEPULSE` | 脉冲运动 |
| `CMMND_EXPOS_FOLLOW` | 外部位置跟随 |
| `CMMND_ANALOGOP` | 模拟量操作 |
| `CMMND_SPIFLASH` | SPI Flash操作 |
| `CMMND_TMC2209` | TMC2209驱动操作 |
| `CMMND_ASKMOVEINFO` | 请求运动信息 |
| `CMMND_CLRPULSECNTINTERNAL` | 清除内部脉冲计数 |
| `CMMND_BOARD_MAINTIANCE` | 板卡维护 |
| `CMMND_DS402HOME` | DS402回零 |

---

## 5. 运动控制API

### 5.1 初始化与连接

```c
// 初始化控制卡
MoCtrCard_Initial()

// 卸载/断开连接
MoCtrCard_Unload()

// 设置控制卡索引
MoCtrCard_SetMCCardIndex()

// 获取DLL版本
MoCtrCard_GetDLLVersion()

// 获取控制卡版本
MoCtrCard_GetCardVersion()
```

### 5.2 基本运动控制

#### 单轴运动
```c
// 相对位置运动
MoCtrCard_MCrlAxisRelMove(axisId, distance, speed, accel)
MoCtrCard_MCrlAxisRelMoveSyncInput(axisId, distance, speed, accel, syncInput)

// 绝对位置运动
MoCtrCard_MCrlAxisAbsMove(axisId, position, speed, accel)
MoCtrCard_MCrlAxisAbsMoveSyncInput(axisId, position, speed, accel, syncInput)

// 速度运动(无目标位置)
MoCtrCard_MCrlAxisMoveAtSpd(axisId, speed, accel)
MoCtrCard_MCrlAxisMoveAtSpdWithInTime(axisId, speed, accel, timeLimit)

// 正弦曲线运动
MoCtrCard_MCrlAxisSinRelMove(axisId, distance, speed, accel, sineParams)
MoCtrCard_MCrlAxisSinAbsMove(axisId, position, speed, accel, sineParams)

// 基本运动(通用)
MoCtrCard_MCrlAxisMove(axisId, position, speed, accel)

// 脉冲+距离运动
MoCtrCard_MCrlAxisRelMoveAndPulse(axisId, distance, pulseCount)
MoCtrCard_StopAxisRelMoveAndPulse(axisId)
```

#### 多轴(组)运动
```c
// 相对位置PTP运动
MoCtrCard_MCrlGroupRelMovePTP(axisMask, distances[], speed[], accel[])
MoCtrCard_MCrlGroupRelMovePTPSyncInput(axisMask, distances[], speed[], accel[], syncInput)

// 绝对位置PTP运动
MoCtrCard_MCrlGroupAbsMovePTP(axisMask, positions[], speed[], accel[])
MoCtrCard_MCrlGroupAbsMovePTPSyncInput(axisMask, positions[], speed[], accel[], syncInput)

// 多轴联动(插补)
MoCtrCard_MCrlGroupRelMove(axisMask, distances[], speed[], accel[])
MoCtrCard_MCrlGroupAbsMove(axisMask, positions[], speed[], accel[])

// 多轴速度运动
MoCtrCard_MCrlGroupSpdMove(axisMask, speeds[], accels[])

// 组运动停止
MoCtrCard_MCtrlGroupParaStop(axisMask)
```

### 5.3 运动控制与停止

```c
// 急停
MoCtrCard_EmergencyStopAxisMov(axisId)

// 参数停止(减速停止)
MoCtrCard_ParaStopAxisMov(axisId)

// 暂停运动
MoCtrCard_PauseAxisMov(axisId)

// 重启运动
MoCtrCard_ReStartAxisMov(axisId)

// 退出运动控制
MoCtrCard_QuiteMotionControl()
```

### 5.4 回零操作

```c
// 基本回零
MoCtrCard_SeekZero(axisId)

// 限距回零
MoCtrCard_SeekZeroWithInDist(axisId, maxDistance)

// 取消回零
MoCtrCard_CancelSeekZero(axisId)

// 设置回零方式
MoctrCard_SetHomeMethord(axisId, homeMethod)

// 获取回零状态
MoCtrCard_GetHomeState(axisId)
```

### 5.5 位置与状态读取

```c
// 获取轴位置
MoCtrCard_GetAxisPos(axisId) → gbAxisPos

// 获取轴速度
MoCtrCard_GetAxisSpd(axisId) → gbAxisSpd

// 获取运行状态
MoCtrCard_GetRunState(axisId) → gbRunState

// 获取限位状态
MoCtrCard_GetLimitState(axisId) → gbLimitStt

// 获取输入状态
MoCtrCard_GetInputState() → gbInput

// 获取中断输入状态
MoCtrCard_GetIntInputState()

// 获取输出状态
MoCtrCard_GetOutState() → gbOutput

// 获取编码器值
MoCtrCard_GetEncoderVal(axisId) → gbAxisEnc

// 获取AD值(模拟量)
MoCtrCard_GetAdVal(channelId)

// 获取运动脉冲计数
MoCtrCard_GetMovePulseCnt() → gbMovePulseCnt

// 获取运动脉冲位置
MoCtrCard_GetMovePulsePos() → gbMovePulsePos

// 获取补偿差值
MoCtrCard_GetComDiff() → gbComDiff
```

### 5.6 位置复位

```c
// 坐标复位
MoCtrCard_ResetCoordinate(axisId, mode)
// mode: RESETPOS_REL (相对复位) 或 RESETPOS_ABS (绝对复位)

// Z轴复位
MoCtrCard_RstZ()

// 清除内部脉冲计数
MoCtrCard_ClearInternalPulseCount()

// 设置编码器位置
MoCtrCard_SetEncoderPos(axisId, position)
```

---

## 6. 参数系统

### 6.1 轴参数索引

| 参数索引 | 含义 | 单位/说明 |
|---------|------|----------|
| `ParaIndex_Acc` | 加速度 | mm/s² 或自定义 |
| `ParaIndex_HighSpd` | 高速 | mm/s |
| `ParaIndex_LowSpd` | 低速 | mm/s |
| `ParaIndex_HomeSpd` | 回零速度 | mm/s |
| `ParaIndex_HomeDist` | 回零距离 | mm |
| `ParaIndex_ManDist` | 手动距离 | mm |
| `ParaIndex_EncPulse` | 编码器脉冲当量 | pulse/mm |
| `ParaIndex_SoftLimitPos` | 正软限位 | mm |
| `ParaIndex_SoftLimitNeg` | 负软限位 | mm |
| `ParaIndex_ComWdith` | 补偿宽度 | - |
| `ParaIndex_ComScope` | 补偿范围 | - |
| `ParaIndex_AnaEn` | 模拟量使能 | - |
| `ParaIndex_AnaZeroScope` | 模拟量零范围 | - |
| `ParaIndex_MotortInputLev` | 电机输入电平 | - |
| `ParaIndex_CmndEncGear` | 命令编码器齿轮比 | - |
| `ParaIndex_BaseXAxis` | X轴基准 | - |
| `ParaIndex_BaseYAxis` | Y轴基准 | - |
| `ParaIndex_BaseZAxis` | Z轴基准 | - |
| `ParaIndex_BaseAAxis` | A轴基准 | - |
| `ParaIndex_BaseBAxis` | B轴基准 | - |
| `ParaIndex_BaseCAxis` | C轴基准 | - |

### 6.2 参数类定义

```
ParaClassAbsEnc        - 绝对编码器参数类
ParaClassAxisExReg     - 轴扩展参数类
ParaClassAxisExRegAbsOffset - 轴扩展绝对偏移参数类
ParaClassInputCfgReg   - 输入配置参数类
ParaClassSysCtrlReg    - 系统控制参数类
```

### 6.3 绝对编码器参数索引

| 参数索引 | 含义 |
|---------|------|
| `AbsEncParaIndex_EncChoose` | 编码器选择 |
| `AbsEncParaIndex_ClkScale0` | 时钟缩放0 |
| `AbsEncParaIndex_ClkTm0` | 时钟时间0 |
| `AbsEncParaIndex_DataFormat0` | 数据格式0 |
| `AbsEncParaIndex_DataBit0` | 数据位0 |
| `AbsEncParaIndex_ClkScale1` | 时钟缩放1 |
| `AbsEncParaIndex_ClkTm1` | 时钟时间1 |
| `AbsEncParaIndex_DataFormat1` | 数据格式1 |
| `AbsEncParaIndex_DataBit1` | 数据位1 |

### 6.4 输入扩展功能参数

```
InputExFunParaIndex_MaskL - 低16位输入掩码
InputExFunParaIndex_MaskH - 高16位输入掩码
```

### 6.5 参数读写API

```c
// 读取参数
MoCtrCard_ReadPara(paraClass, paraIndex, axisId)

// 发送/写入参数
MoCtrCard_SendPara(paraClass, paraIndex, axisId, value)

// 保存系统参数到ROM
MoCtrCard_SaveSystemParaToROM()

// 设置运动曲线参数
MoCtrCard_SetProfilePara(axisId, profileParams)
MoCtrCard_SetProfileCurvePara(axisId, curveParams)

// 设置离线运动参数
MoCtrCard_SetAxisOfflineMovePara(axisId, offlineParams)
MoCtrCard_SetGrpOfflineMovePara(axisMask, offlineParams)

// 变更运动参数(运行中)
MoCtrCard_ChangeMotionPara(axisId, newSpeed, newAccel)
```

---

## 7. 电子齿轮与凸轮曲线

### 7.1 电子齿轮API

```c
// 设置齿轮参数
MoCtrCard_SetGearPara(masterId, slaveId, gearRatio)

// 启动齿轮耦合
MoCtrCard_ManGearStart(masterId, slaveId)

// 停止齿轮耦合
MoCtrCard_ManGearStop(masterId, slaveId)

// 齿轮操作命令
MoCtrCard_ManGearOp(masterId, slaveId, operation)
```

### 7.2 凸轮曲线API

```c
// 设置凸轮曲线参数
stSetProfileCurvePara {
    nMasterId;    // 主轴ID
    nSlaveId;     // 从轴ID
    fPara;        // 曲线参数数组
}

// 启动凸轮运动
MoCtrCard_StartProfileMove(axisId)

// 停止凸轮运动
MoCtrCard_StopProfileMove(axisId)
```

### 7.3 正弦谐波运动

```c
// 简谐运动参数结构
stMoveSine {
    fDistHead;    // 起始距离
    fPeriod;      // 周期
    fPhase;       // 相位
}

// 启动简谐运动
MoCtrCard_StartSimpleHarmonicMove(axisId, sineParams)

// 停止简谐运动
MoCtrCard_StopSimpleHarmonicMove(axisId)

// 变更简谐运动周期
MoCtrCard_ChangeSimpleHarmonicT(axisId, newPeriod)
```

---

## 8. 补偿系统

### 8.1 补偿类型

| 类型 | 功能 |
|-----|------|
| **螺距补偿** | 单轴位置补偿表 |
| **平台调平** | 多轴平面补偿 |

### 8.2 补偿API

```c
// 获取补偿表数据
MoCtrCard_GetComTableData(tableId)
MoCtrCard_GetComTableInfo()

// 设置补偿表
subCmnd_BoardMtn_SetComTable(tableId, data[])

// 读取补偿表
subCmnd_BoardMtn_ReadComTable(tableId)

// 更新补偿参数
MoCtrCard_UpdateCompensateParameter(axisId)

// 平台调平
MoCtrCard_GetPlatformLevTableData()
MoCtrCard_GetPlatformLevTableInfo()

// 启用/禁用平台补偿
subCmnd_PlatformLevel_Start()
subCmnd_PlatformLevel_Stop()
```

### 8.3 信号电平配置

```
BitMask_ComTable   - 补偿表位掩码
BitMask_Com        - 补偿使能位掩码
BitMask_HardLimit  - 硬限位位掩码
BitMask_SoftLimit  - 软限位位掩码
BitMask_LimitPos   - 正限位位掩码
BitMask_LimitNeg   - 负限位位掩码
BitMask_HomeSig    - 回零信号位掩码
BitMask_HomeMode   - 回零模式位掩码

// 子命令
subCmnd_SetSigLev_ComEn()        - 补偿使能电平
subCmnd_SetSigLev_ComTable()     - 补偿表电平
subCmnd_SetSigLev_HardLimitEn()  - 硬限位使能电平
subCmnd_SetSigLev_SoftLimitEn()  - 软限位使能电平
SubCmnd_SetSigLev_HomeMode()     - 回零模式电平
subCmnd_setSigLev_LimitLev()     - 限位电平
subCmnd_SetSigLev_OriLev()       - 原点电平
```

---

## 9. 示波器功能

### 9.1 示波器API

```c
// 重置示波器
MoCtrCard_Scope_Reset()

// 设置通道参数
MoCtrCard_Scope_SetChannelPara(channelId, channelParams)

// 启动采样
MoCtrCard_Scope_Start()

// 停止采样
MoCtrCard_Scope_Stop()
```

### 9.2 示波器子命令

```
subCmnd_Scope_Reset  - 重置
subCmnd_Scope_Set    - 设置参数
subCmnd_Scope_Start  - 开始采样
subCmnd_Scope_Stop   - 停止采样
```

---

## 10. 步进驱动器支持 (TMC2209)

### 10.1 TMC2209 API

TMC2209 是 Trinamic 公司的 UART 接口步进驱动芯片,支持静音模式、微步细分等功能。

```c
// 驱动使能/禁用
MotorDrive_Enable(axisId)
MotorDrive_Disable(axisId)

// 寄存器读写
MotorDrive_RegRead(axisId, regId)
MotorDrive_RegWrite(axisId, regId, value)
MotorDrive_RegSave(axisId)  // 保存寄存器值

// 驱动复位
MotorDrive_Reset(axisId)

// 手动操作
MotorDrive_ManualOP(axisId, operation)

// 设置输入模式
MotorDrive_SetInputMode(axisId, inputMode)
```

### 10.2 TMC2209 子命令

```
subCmnd_TMC2209_Enable     - 使能驱动
subCmnd_TMC2209_Disable    - 禁用驱动
subCmnd_TMC2209_RegRead    - 读寄存器
subCmnd_TMC2209_RegWrite   - 写寄存器
subCmnd_TMC2209_RegSave    - 保存寄存器
subCmnd_TMC2209_Reset      - 复位驱动
subCmnd_TMC2209_SetInputMode - 设置输入模式
```

### 10.3 驱动参数索引

| 参数索引 | 含义 |
|---------|------|
| `MotorDrv_ParaIndex_Pulse` | 脉冲参数 |
| `MotorDrv_ParaIndex_IRun` | 运行电流 |
| `MotorDrv_ParaIndex_IHold` | 保持电流 |
| `MotorDrv_ParaIndex_InputFun` | 输入功能 |

### 10.4 驱动状态

```
MotorDrv_TMCState  - TMC驱动状态
MotorDrv_TMCAlarm  - TMC报警状态
```

---

## 11. Lua脚本支持

### 11.1 Lua API

```c
// 执行Lua字符串
MoCtrCard_Lua_DoString(luaCode)

// 获取Lua执行结果
MoCtrCard_Lua_GetResult() → gbLuaRes
```

### 11.2 Lua子命令

```
subCmnd_Lua_CmndSendRun    - 发送命令运行
subCmnd_Lua_ContinueFileRun - 继续文件运行
subCmnd_Lua_GetRunningInfo  - 获取运行信息
subCmnd_Lua_PauseFileRun    - 暂停文件运行
subCmnd_Lua_RunFileRun      - 运行文件
subCmnd_Lua_StopFileRun     - 停止文件运行
```

### 11.3 Lua结果类型

```
eLuaResNull          - 无结果
eLuaResDoStringLen   - 字符串长度
eLuaResFileEmpty     - 文件为空
eLuaResFileException - 文件异常
eLuaResFileMaxLen    - 文件超长
eLuaResFileName      - 文件名
eLuaResFileSendErr   - 文件发送错误
```

---

## 12. 离线运行与G代码

### 12.1 离线运行API

```c
// 自动运行控制
MoCtrCard_AR_Run()
MoCtrCard_AR_Stop()
MoCtrCard_AR_Pause()
MoCtrCard_AR_Continue()

// 文件操作
MoCtrCard_AR_ReadFile(filePath)
MoCtrCard_AR_DownloadFile(filePath)

// 手动操作命令
MoCtrCard_AR_ManualOperationCmnd(operation)

// 发送MDI命令
MoCtrCard_SendMDICommand(mdiCode)

// G代码命令填充
MoCtrCard_FillGCodeCmndByCmndInfo(gCodeInfo)
```

### 12.2 手动操作类型

| 操作类型 | 功能 |
|---------|------|
| `MANOP_HOMING` | 回零 |
| `MANOP_HOMINGWITHINDIST` | 限距回零 |
| `MANOP_HOMINGWITHSWIPT` | 开关回零 |
| `MANOP_HOMINGCANCLE` | 取消回零 |
| `MANOP_SPDMOVE` | 速度运动 |
| `MANOP_SPDMOVENOPARA` | 无参数速度运动 |
| `MANOP_SPDMOVEWITHINTIME` | 定时速度运动 |
| `MANOP_MOVERELDIST` | 相对距离运动 |
| `MANOP_MOVEABSDIST` | 绝对距离运动 |
| `MANOP_MOVEREL_SYNCEXIN` | 同步输入相对运动 |
| `MANOP_MOVEABS_SYNCEXIN` | 同步输入绝对运动 |
| `MANOP_MOVEINNERDIST` | 内部距离运动 |
| `MANOP_MOVECHAGEVEL` | 变速运动 |
| `MANOP_PAUSE` | 暂停 |
| `MANOP_RESTARTMOVE` | 重启运动 |
| `MANOP_QUITEMOVE` | 退出运动 |
| `MANOP_PARASTOP` | 参数停止 |
| `MANOP_EMGERNCYSTOP` | 急停 |
| `MANOP_UPDATECOMPENSATE` | 更新补偿 |

---

## 13. IO控制

### 13.1 输入输出API

```c
// 设置输出
MoCtrCard_SetOutput(outputId, value)

// 输出状态结构
stSetOutput {
    nOutMode;    // 输出模式
    nDevId;      // 设备ID
}
```

### 13.2 输入状态查询

```
AskInfo_Input_L   - 低16位输入状态
AskInfo_Input_H   - 高16位输入状态
AskInfo_Intrupt_L - 低16位中断输入
AskInfo_Intrupt_H - 高16位中断输入
AskInfo_Output_L  - 低16位输出状态
AskInfo_Output_H  - 高16位输出状态
```

---

## 14. 外部位置跟随

### 14.1 外部位置跟随API

```c
// 启动外部位置跟随
MoCtrCard_ExPosFollow_Start()

// 停止外部位置跟随
MoCtrCard_ExPosFollow_Stop()
```

### 14.2 子命令

```
subCmnd_ExPosFollow_Start  - 启动跟随
subCmnd_ExPosFollow_Stop   - 停止跟随
```

---

## 15. SPI Flash操作

### 15.1 SPI Flash API

```c
// 页读取
MoCtrCard_SPIFlash_PageRead(pageId, offset, length)

// 页写入
MoCtrCard_SPIFlash_PageWrite(pageId, offset, data[])
```

### 15.2 子命令与结构

```
subCmnd_SpiFlash_Read  - 读取
subCmnd_SpiFlash_Write - 写入

stSPIFlashRd {
    nPage;       // 页号
    fileId;      // 文件ID
}

stSPIFlashWr {
    nPage;       // 页号
    fileId;      // 文件ID
    data[];      // 数据
}
```

---

## 16. 状态查询结构

### 16.1 信息查询结构

```
stAskInfo {
    AskInfo_Pos_Each      // 单轴位置
    AskInfo_Pos_Grp       // 组位置
    AskInfo_Spd_Each      // 单轴速度
    AskInfo_Spd_Grp       // 组速度
    AskInfo_Encdr_Each    // 单轴编码器
    AskInfo_Encdr_Grp     // 组编码器
    AskInfo_LimitStt_Each // 单轴限位状态
    AskInfo_LimitStt_Grp  // 组限位状态
    AskInfo_HomStt_Each   // 单轴回零状态
    AskInfo_Ana_Each      // 单轴模拟量
    AskInfo_Ana_Grp       // 组模拟量
    AskInfo_ComDiff_Each  // 单轴补偿差
    AskInfo_ComDiff_Grp   // 组补偿差
    AskInfo_HomeStt       // 回零状态
    AskInfo_NcStt         // NC状态
    AskInfo_OfflineMove   // 离线运动状态
    AskInfo_MovePulseCnt  // 运动脉冲计数
    AskInfo_MovePulsePos  // 运动脉冲位置
    AskInfo_MovePulseInfo // 运动脉冲信息
    AskInfo_Version       // 版本信息
    AskInfo_McuId         // MCU ID
    AskInfo_AllCfgInfo    // 所有配置信息
    AskInfo_CustomInfo_wy1 // 自定义信息
}
```

### 16.2 命令状态返回

```
CMND_INFO_NOTHING        - 无状态
CMND_INFO_INVALIDCMND    - 无效命令
CMND_INFO_CMNDCODEERR    - 命令代码错误
CMND_INFO_INDEXERROR     - 索引错误
CMND_INFO_SYSRUNNING     - 系统运行中
CMND_INFO_OPCMNDINDEX    - 操作命令索引
CMND_INFO_OUTPUTINDEX    - 输出索引
CMND_INFO_JIAOYANHE      - 校验和错误
MCU_CMND_EXECUTE_OK      - MCU命令执行成功
```

---

## 17. 手轮与摇杆控制

### 17.1 API

```c
// 设置摇杆使能
MoCtrCard_SetJoyStickEnable(enable)
```

### 17.2 手轮模式参数

```
SysCtrlPara_HandWheelMode - 手轮模式配置
```

---

## 18. 其他辅助功能

### 18.1 延时

```c
// 毫秒延时
MoCtrCard_DelayMS(milliseconds)
```

### 18.2 调试功能

```c
// 打开调试窗口
MoCtrCard_OpenSpDebugForm()

// 关闭调试窗口
MoCtrCard_CloseSpDebugForm()

// 测试函数
MoCtrCard_Test()
```

### 18.3 硬件信息

```c
// 获取板卡硬件信息
MoCtrCard_GetBoardHardInfo()

// 获取所有配置信息
MoCtrCard_GetAllConfigInfo()

// 获取自定义信息
MoCtrCard_GetCustomInfo()
```

---

## 19. 轴数量与参数定义

| 常量 | 值 | 含义 |
|-----|---|------|
| `MAX_AXIS_NUM` | 6 | 最大轴数量(X, Y, Z, A, B, C) |
| `PARA_NUM_PER_AXIS` | - | 每轴参数数量 |
| `PARA_SUM` | - | 总参数数量 |

---

## 20. 总结

### 20.1 软件特点

1. **多通信方式**: 支持 TCP/IP 和 RS232 串口
2. **多轴控制**: 最多6轴联动
3. **丰富运动模式**: PTP、插补、正弦、脉冲、速度控制
4. **电子齿轮/凸轮**: 支持主从轴耦合
5. **补偿功能**: 螺距补偿、平台调平
6. **示波器**: 实时数据采集与分析
7. **Lua扩展**: 支持脚本编程
8. **G代码支持**: CNC标准指令
9. **步进驱动集成**: TMC2209 UART驱动配置

### 20.2 硬件交互接口总结

| 接口 | 协议 | 端口 | 用途 |
|-----|------|------|------|
| **TCP/IP** | 自定义帧协议 | 192.168.0.7:8888 | 主通信通道 |
| **RS232** | 自定义帧协议 | COM3 | 备用通信通道 |
| **TMC2209 UART** | 单线UART | 通过控制卡转发 | 步进驱动配置 |

### 20.3 通信帧特点

- 固定帧头/帧尾结构
- 命令类型+子命令类型双级分类
- 支持参数读写、运动控制、状态查询三类主要操作
- CRC校验(校验和)

---

## 附录A: Python 协议实现示例

以下是基于反编译代码的 Python 通信协议实现：

```python
#!/usr/bin/env python3
"""
MCC 运动控制卡 Python 通信库
基于 MCC6DLL.dll 反编译代码实现
"""

import socket
import struct
from typing import Tuple, List, Optional
from dataclasses import dataclass
from enum import IntEnum


# ============== 命令类型定义 ==============
class CMMND(IntEnum):
    """命令类型"""
    PROFILE_MOVE = 0x80       # 凸轮/曲线运动
    SCOPE = 0x81              # 示波器
    OFFLINERUN_PARA = 0x88    # 离线运行参数
    OFFLINERUN_START = 0x89   # 启动离线运行
    OFFLINERUN_STOP = 0x8A    # 停止离线运行
    MOVEPULSE = 0x8C          # 脉冲运动
    MOVESINE = 0x8D           # 正弦运动
    COMPENSATE = 0x8E         # 补偿操作
    EXPOS_FOLLOW = 0x8F       # 外部位置跟随
    TMC2209 = 0x40            # TMC2209驱动
    SETGEAR_PARA = 0xE0       # 设置齿轮参数
    GEAR_OP = 0xE1            # 齿轮操作
    SINPOS_MOVE = 0xE2        # 正弦位置运动
    LUA = 0xE3                # Lua脚本
    SPIFLASH = 0xE4           # SPI Flash
    DS402HOME = 0xE8          # DS402回零
    PLATFORM_COM = 0xE9       # 平台补偿
    MDICMND = 0xF1            # MDI命令
    SETPARA = 0xF2            # 设置参数
    GCODECMND = 0xF3          # G代码命令
    MANUAL_OP = 0xF4          # 手动操作
    ASKPARA = 0xF5            # 请求参数
    ASKMOVEINFO = 0xF6        # 请求运动信息
    SYSTEM_OP = 0xF7          # 系统操作
    RESETPOS = 0xF8           # 复位位置
    SETOUTPUT = 0xF9          # 设置输出
    ANALOGOP = 0xFA           # 模拟量操作
    SETINPUTLEV = 0xFB        # 设置输入电平
    SETENCODER = 0xFC         # 设置编码器
    RESETZ = 0xFD             # Z轴复位
    SAVESYSPARA = 0xFE        # 保存系统参数


class MANOP(IntEnum):
    """手动操作子命令"""
    MOVEINNERDIST = 0x00
    MOVERELDISTNOPARA = 0x01
    MOVEABSDISTNOPARA = 0x02
    MOVERELDIST = 0x03
    MOVEABSDIST = 0x04
    SPDMOVENOPARA = 0x05
    SPDMOVE = 0x06
    MOVECHAGEVEL = 0x07
    SPDMOVEWITHINTIME = 0x08
    MOVEABS_SYNCEXIN = 0x09
    MOVEREL_SYNCEXIN = 0x0A
    UPDATECOMPENSATE = 0x20
    EMGERNCYSTOP = 0x80
    PARASTOP = 0x81
    PAUSE = 0x82
    RESTARTMOVE = 0x90
    QUITEMOVE = 0x91
    HOMINGNOPARA = 0xA0
    HOMING = 0xA1
    HOMINGCANCLE = 0xA2
    HOMINGWITHINDIST = 0xA3
    HOMINGWITHSWIPT = 0xA4


class AskInfo(IntEnum):
    """信息查询索引"""
    POS_EACH = 0x00
    SPD_EACH = 0x01
    ANA_EACH = 0x02
    ENCdr_EACH = 0x03
    HOMSTT_EACH = 0x04
    INPUT_L = 0x05
    INPUT_H = 0x06
    OUTPUT_L = 0x07
    OUTPUT_H = 0x08
    INTRUPT_L = 0x09
    INTRUPT_H = 0x0A
    NCSTT = 0x0B
    HOMESTT = 0x0C
    COMDIFF_EACH = 0x0D
    LIMITSTT_EACH = 0x0E
    OFFLINEMOVE = 0x20
    MOVEPULSECNT = 0x21
    MOVEPULSEPOS = 0x22
    POS_GRP = 0x80
    SPD_GRP = 0x81
    ANA_GRP = 0x82
    ENCdr_GRP = 0x83
    COMDIFF_GRP = 0x84
    LIMITSTT_GRP = 0x85
    MCU_ID = 0xE0
    ALLCFGINFO = 0xFE
    VERSION = 0xFF


# ============== 帧结构 ==============
FRAME_HEAD = 0xAA

@dataclass
class CommandFrame:
    """命令帧结构"""
    cmd_type: int       # 命令类型
    data_len: int       # 数据长度
    card_index: int = 0 # 卡索引
    cmd_no: int = 0     # 命令号
    data: bytes = b''   # 数据载荷


def build_frame(cmd: CommandFrame) -> bytes:
    """构建命令帧"""
    # 帧头结构 (8字节)
    header = bytearray(8)
    header[0] = FRAME_HEAD                    # u8SpCom_Headr
    header[1] = cmd.data_len                  # u8SpCom_DtLen
    header[2] = cmd.card_index                # u8SpCom_CardIndx
    header[3] = (~cmd.data_len) & 0xFF        # u8SpCom_Check (取反)
    header[4] = 0                             # u8SpDt_CheckSum (稍后计算)
    header[5] = cmd.cmd_type                  # u8SpDt_CmndType
    header[6] = cmd.data_len                  # u8SpDt_Len
    header[7] = cmd.cmd_no                    # u8SpDt_CmndNo
    
    # 组合帧
    frame = bytes(header) + cmd.data
    
    # 计算校验和 (从第5字节开始累加)
    checksum = sum(frame[4:]) & 0xFF
    
    # 填入校验和
    frame = bytearray(frame)
    frame[4] = checksum
    
    return bytes(frame)


def parse_response(data: bytes) -> Tuple[int, int, int, List[float]]:
    """
    解析返回帧
    返回: (命令类型, 子命令, 状态, 值列表)
    """
    if len(data) < 4:
        return (0, 0, -1, [])
    
    cmd_type = data[0]
    sub_cmd = data[1]
    data_len = data[2]
    status = data[3]
    
    # 解析返回值 (每个值4字节, float或int)
    values = []
    for i in range(4, min(len(data), 4 + data_len * 4), 4):
        val = struct.unpack('<f', data[i:i+4])[0]
        values.append(val)
    
    return (cmd_type, sub_cmd, status, values)


# ============== 通信类 ==============
class MCCController:
    """MCC运动控制卡通信类"""
    
    def __init__(self, ip: str = "192.168.0.7", port: int = 8888):
        self.ip = ip
        self.port = port
        self.sock: Optional[socket.socket] = None
        self.timeout = 0.2
    
    def connect(self) -> bool:
        """建立TCP连接"""
        try:
            self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.sock.settimeout(self.timeout)
            self.sock.connect((self.ip, self.port))
            return True
        except Exception as e:
            print(f"连接失败: {e}")
            return False
    
    def disconnect(self):
        """断开连接"""
        if self.sock:
            self.sock.close()
            self.sock = None
    
    def _send_and_recv(self, frame: bytes, expect_len: int = 0) -> Optional[bytes]:
        """发送帧并接收响应"""
        if not self.sock:
            return None
        
        try:
            self.sock.send(frame)
            
            # 计算期望接收长度
            resp_len = expect_len + 5 if expect_len > 0 else 5
            data = self.sock.recv(4096)
            return data
        except socket.timeout:
            return None
        except Exception as e:
            print(f"通信错误: {e}")
            return None
    
    # ========== 运动控制 ==========
    def get_axis_pos(self, axis_id: int = 0xFF) -> Optional[List[float]]:
        """获取轴位置 (0xFF = 所有轴)"""
        cmd = CommandFrame(
            cmd_type=CMMND.ASKMOVEINFO,
            data_len=4,
            data=bytes([axis_id, AskInfo.POS_EACH if axis_id < 6 else AskInfo.POS_GRP, 0, 0])
        )
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4 if axis_id < 6 else 24)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0:
                return values
        return None
    
    def get_axis_spd(self, axis_id: int = 0xFF) -> Optional[List[float]]:
        """获取轴速度"""
        cmd = CommandFrame(
            cmd_type=CMMND.ASKMOVEINFO,
            data_len=4,
            data=bytes([axis_id, AskInfo.SPD_EACH if axis_id < 6 else AskInfo.SPD_GRP, 0, 0])
        )
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4 if axis_id < 6 else 24)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0:
                return values
        return None
    
    def rel_move(self, axis_id: int, distance: float, speed: float = 0, accel: float = 0) -> bool:
        """相对位置运动"""
        data = struct.pack('<BBbbffff', 
            MANOP.MOVERELDIST if speed > 0 else MANOP.MOVERELDISTNOPARA,
            axis_id, 0, 0,
            distance, speed, accel, 0
        )
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def abs_move(self, axis_id: int, position: float, speed: float = 0, accel: float = 0) -> bool:
        """绝对位置运动"""
        data = struct.pack('<BBbbffff',
            MANOP.MOVEABSDIST if speed > 0 else MANOP.MOVEABSDISTNOPARA,
            axis_id, 0, 0,
            position, speed, accel, 0
        )
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def home(self, axis_id: int, speed: float = 0, accel: float = 0) -> bool:
        """回零"""
        sub_cmd = MANOP.HOMING if speed > 0 else MANOP.HOMINGNOPARA
        data = struct.pack('<BBbbffff', sub_cmd, axis_id, 0, 0, 0, speed, accel, 0)
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def emergency_stop(self, axis_id: int) -> bool:
        """急停"""
        data = struct.pack('<BBbbffff', MANOP.EMGERNCYSTOP, axis_id, 0, 0, 0, 0, 0, 0)
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def stop(self, axis_id: int, accel: float = 0) -> bool:
        """减速停止"""
        data = struct.pack('<BBbbffff', MANOP.PARASTOP, axis_id, 0, 0, 0, 0, accel, 0)
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def pause(self, axis_id: int) -> bool:
        """暂停"""
        data = struct.pack('<BBbbffff', MANOP.PAUSE, axis_id, 0, 0, 0, 0, 0, 0)
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def restart(self, axis_id: int) -> bool:
        """重启运动"""
        data = struct.pack('<BBbbffff', MANOP.RESTARTMOVE, axis_id, 0, 0, 0, 0, 0, 0)
        cmd = CommandFrame(cmd_type=CMMND.MANUAL_OP, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    # ========== 参数操作 ==========
    def read_para(self, axis_id: int, para_index: int) -> Optional[float]:
        """读取参数"""
        data = bytes([axis_id, para_index])
        cmd = CommandFrame(cmd_type=CMMND.ASKPARA, data_len=4, data=data.ljust(4, b'\x00'))
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0 and values:
                return values[0]
        return None
    
    def write_para(self, axis_id: int, para_index: int, value: float) -> bool:
        """写入参数"""
        data = bytes([axis_id, para_index, 0, 0]) + struct.pack('<f', value)
        cmd = CommandFrame(cmd_type=CMMND.SETPARA, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    def save_para(self) -> bool:
        """保存参数到ROM"""
        cmd = CommandFrame(cmd_type=CMMND.SAVESYSPARA, data_len=4)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    # ========== IO操作 ==========
    def get_input(self, group: int = 0) -> Optional[int]:
        """获取输入状态"""
        info = AskInfo.INPUT_L if group == 0 else AskInfo.INPUT_H
        data = bytes([0, info, 0, 0])
        cmd = CommandFrame(cmd_type=CMMND.ASKMOVEINFO, data_len=4, data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0 and values:
                return int(values[0])
        return None
    
    def get_output(self, group: int = 0) -> Optional[int]:
        """获取输出状态"""
        info = AskInfo.OUTPUT_L if group == 0 else AskInfo.OUTPUT_H
        data = bytes([0, info, 0, 0])
        cmd = CommandFrame(cmd_type=CMMND.ASKMOVEINFO, data_len=4, data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0 and values:
                return int(values[0])
        return None
    
    def set_output(self, output_index: int, value: bool) -> bool:
        """设置输出"""
        sub_cmd = 1 if value else 2  # OutputSet=1, OutputReset=2
        data = bytes([sub_cmd, output_index, 0, 0])
        cmd = CommandFrame(cmd_type=CMMND.SETOUTPUT, data_len=len(data), data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 0)
        return resp is not None and resp[3] == 0
    
    # ========== 信息查询 ==========
    def get_version(self) -> Optional[int]:
        """获取控制卡版本"""
        data = bytes([0, AskInfo.VERSION, 0, 0])
        cmd = CommandFrame(cmd_type=CMMND.ASKMOVEINFO, data_len=4, data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0 and values:
                return int(values[0])
        return None
    
    def get_run_state(self) -> Optional[int]:
        """获取运行状态"""
        data = bytes([0, AskInfo.NCSTT, 0, 0])
        cmd = CommandFrame(cmd_type=CMMND.ASKMOVEINFO, data_len=4, data=data)
        frame = build_frame(cmd)
        resp = self._send_and_recv(frame, 4)
        
        if resp:
            _, _, status, values = parse_response(resp)
            if status == 0 and values:
                return int(values[0])
        return None


# ============== 使用示例 ==============
if __name__ == "__main__":
    # 创建控制器实例
    mc = MCCController("192.168.0.7", 8888)
    
    # 连接
    if mc.connect():
        print("连接成功")
        
        # 获取所有轴位置
        positions = mc.get_axis_pos(0xFF)  # 0xFF = 所有轴
        if positions:
            for i, pos in enumerate(positions):
                print(f"轴{i} 位置: {pos:.3f}")
        
        # 单轴相对运动
        if mc.rel_move(axis_id=0, distance=10.0, speed=5.0, accel=100.0):
            print("运动命令发送成功")
        
        # 读取参数
        high_spd = mc.read_para(axis_id=0, para_index=1)  # ParaIndex_HighSpd
        if high_spd is not None:
            print(f"高速: {high_spd}")
        
        # 断开连接
        mc.disconnect()
    else:
        print("连接失败")
```

---

## 附录B: 反编译源代码位置

反编译后的完整 C# 源代码位于：

```
~/od-work/claw-dir/decompiled/
├── MCC6DLL/
│   ├── DataExchange.cs    # TCP/RS232通信层
│   ├── crcCls.cs          # CRC16 Modbus校验
│   └── SpDataShow.cs      # 数据显示辅助
├── SerialPortLibrary/
│   └── SPLibClass.cs      # 完整协议实现（约2000行）
├── Properties/
│   └── AssemblyInfo.cs
└── MCC6DLL.csproj
```

**关键文件**: `SPLibClass.cs` 包含完整的帧协议定义和所有 API 函数实现。

---

## 附录C: UI窗体列表

| 窗体 | 功能 |
|-----|------|
| MainFrm | 主窗体 |
| SetSysParaFrm | 系统参数设置 |
| SysCtrlParaCfgFrm | 系统控制参数配置 |
| TestIOFrm | IO测试 |
| AbsEncParaFrm | 绝对编码器参数 |
| TMC2209Frm | TMC2209驱动配置 |
| CurveDefineFrm | 曲线定义 |
| CompensateParameterFrm | 补偿参数 |
| CacPulseEquivalent | 脉冲当量计算 |
| ManCtrlEx/ManualOpEx | 手动控制扩展 |
| ManCtrlSin | 手动正弦控制 |
| ExPosFollow | 外部位置跟随 |
| ScopeFrm | 示波器 |
| LuaCtrlFrm | Lua控制 |
| ProgInputFrm | 程序输入 |
| SeekZero | 回零设置 |
| SignleLevCac | 信号电平计算 |
| InputFunCfgFrm | 输入功能配置 |
| AxisExParaFrm | 轴扩展参数 |
| SportAssitant | 串口助手 |
| AboutBox | 关于对话框 |