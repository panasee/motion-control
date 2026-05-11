#!/usr/bin/env python3
"""
MCC 运动控制卡 Python 驱动库
完整替代 MCC6DLL.dll

基于 MCC6DLL.dll 反编译代码实现
支持 TCP/IP 和 RS232 通信

作者: Coder Bot
日期: 2026-03-28
"""

import socket
import struct
import threading
import time
from typing import Optional, Tuple, List, Union, Callable
from dataclasses import dataclass, field
from enum import IntEnum
from functools import wraps

# 串口通信库
try:
    import serial
    import serial.tools.list_ports
    SERIAL_AVAILABLE = True
except ImportError:
    SERIAL_AVAILABLE = False


# ============================================================
# 常量定义
# ============================================================

FRAME_HEAD = 0xAA
FRAME_TAIL = 0x5A
MAX_AXIS_NUM = 6
PARA_NUM_PER_AXIS = 16
PARA_SUM = 96


class CMMND(IntEnum):
    """命令类型"""
    PROFILE_MOVE = 0x80           # 凸轮/曲线运动
    SCOPE = 0x81                  # 示波器
    OFFLINERUN_PARA = 0x88        # 离线运行参数
    OFFLINERUN_START = 0x89       # 启动离线运行
    OFFLINERUN_STOP = 0x8A        # 停止离线运行
    OFFLINERUN_CLEAR = 0x8B       # 清除离线运行
    MOVEPULSE = 0x8C              # 脉冲运动
    MOVESINE = 0x8D               # 正弦运动
    BOARD_MAINTENANCE = 0x8E      # 板卡维护 (原C#: CMMND_BOARD_MAINTIANCE)
    EXPOS_FOLLOW = 0x8F           # 外部位置跟随
    COMPENSATE = 0xD0             # 补偿操作 (原C#: CMMND_COMPENSATE=208)
    CLRPULSECNTINTERNAL = 0xD1    # 清除内部脉冲计数 (原C#: CMMND_CLRPULSECNTINTERNAL=209)
    TMC2209 = 0x40                # TMC2209驱动
    SETGEAR_PARA = 0xE0           # 设置齿轮参数
    GEAR_OP = 0xE1                # 齿轮操作
    SINPOS_MOVE = 0xE2            # 正弦位置运动
    LUA = 0xE3                    # Lua脚本
    SPIFLASH = 0xE4               # SPI Flash
    DS402HOME = 0xE8              # DS402回零
    PLATFORM_COM = 0xE9           # 平台补偿
    MDICMND = 0xF1                # MDI命令
    SETPARA = 0xF2                # 设置参数
    GCODECMND = 0xF3              # G代码命令
    MANUAL_OP = 0xF4              # 手动操作
    ASKPARA = 0xF5                # 请求参数
    ASKMOVEINFO = 0xF6            # 请求运动信息
    SYSTEM_OP = 0xF7              # 系统操作
    RESETPOS = 0xF8               # 复位位置
    SETOUTPUT = 0xF9              # 设置输出
    ANALOGOP = 0xFA               # 模拟量操作
    SETINPUTLEV = 0xFB            # 设置输入电平
    SETENCODER = 0xFC             # 设置编码器
    RESETZ = 0xFD                 # Z轴复位
    SAVESYSPARA = 0xFE            # 保存系统参数


class MANOP(IntEnum):
    """手动操作子命令"""
    MOVEINNERDIST = 0x00          # 内部距离运动
    MOVERELDISTNOPARA = 0x01      # 相对运动（无参数）
    MOVEABSDISTNOPARA = 0x02      # 绝对运动（无参数）
    MOVERELDIST = 0x03            # 相对运动
    MOVEABSDIST = 0x04            # 绝对运动
    SPDMOVENOPARA = 0x05          # 速度运动（无参数）
    SPDMOVE = 0x06                # 速度运动
    MOVECHAGEVEL = 0x07           # 变速
    SPDMOVEWITHINTIME = 0x08      # 定时速度运动
    MOVEABS_SYNCEXIN = 0x09       # 同步输入绝对运动
    MOVEREL_SYNCEXIN = 0x0A       # 同步输入相对运动
    UPDATECOMPENSATE = 0x20       # 更新补偿
    EMGERNCYSTOP = 0x80           # 急停
    PARASTOP = 0x81               # 参数停止
    PAUSE = 0x82                  # 暂停
    RESTARTMOVE = 0x90            # 重启运动
    QUITEMOVE = 0x91              # 退出运动
    HOMINGNOPARA = 0xA0           # 回零（无参数）
    HOMING = 0xA1                 # 回零
    HOMINGCANCLE = 0xA2           # 取消回零
    HOMINGWITHINDIST = 0xA3       # 限距回零
    HOMINGWITHSWIPT = 0xA4        # 开关回零


class AskInfo(IntEnum):
    """信息查询索引"""
    POS_EACH = 0x00               # 单轴位置
    SPD_EACH = 0x01               # 单轴速度
    ANA_EACH = 0x02               # 单轴模拟量
    ENCdr_EACH = 0x03             # 单轴编码器
    HOMSTT_EACH = 0x04            # 单轴回零状态
    INPUT_L = 0x05                # 低16位输入
    INPUT_H = 0x06                # 高16位输入
    OUTPUT_L = 0x07               # 低16位输出
    OUTPUT_H = 0x08               # 高16位输出
    INTRUPT_L = 0x09              # 低16位中断输入
    INTRUPT_H = 0x0A              # 高16位中断输入
    NCSTT = 0x0B                  # NC状态
    HOMESTT = 0x0C                # 回零状态
    COMDIFF_EACH = 0x0D           # 单轴补偿差
    LIMITSTT_EACH = 0x0E          # 单轴限位状态
    OFFLINEMOVE = 0x20            # 离线运动状态
    MOVEPULSECNT = 0x21           # 运动脉冲计数
    MOVEPULSEPOS = 0x22           # 运动脉冲位置
    MOVEPULSEINFO = 0x23          # 运动脉冲信息
    POS_GRP = 0x80                # 组位置
    SPD_GRP = 0x81                # 组速度
    ANA_GRP = 0x82                # 组模拟量
    ENCdr_GRP = 0x83              # 组编码器
    COMDIFF_GRP = 0x84            # 组补偿差
    LIMITSTT_GRP = 0x85           # 组限位状态
    CUSTOMINFO_WY1 = 0xB0         # 自定义信息
    MCU_ID = 0xE0                 # MCU ID
    ALLCFGINFO = 0xFE             # 所有配置信息
    VERSION = 0xFF                # 版本信息


class ParaIndex(IntEnum):
    """参数索引"""
    ManDist = 0
    HighSpd = 1
    LowSpd = 2
    HomeSpd = 3
    Acc = 4
    Pulse = 5
    HomeDist = 6
    MotortInputLev = 7
    AnaEn = 8
    AnaZeroScope = 9
    ComWdith = 10
    ComScope = 11
    EncPulse = 12
    CmndEncGear = 13
    SoftLimitNeg = 14
    SoftLimitPos = 15


class LimitBitMask(IntEnum):
    """限位状态位掩码"""
    # 硬限位
    HardLimitPos = 0x01    # 正硬限位触发
    HardLimitNeg = 0x02    # 负硬限位触发
    HardLimit = 0x03       # 硬限位触发 (任一方向)
    
    # 软限位
    SoftLimitPos = 0x04    # 正软限位触发
    SoftLimitNeg = 0x08    # 负软限位触发
    SoftLimit = 0x0C       # 软限位触发 (任一方向)
    
    # 任意限位
    AnyLimitPos = 0x05     # 正限位触发 (硬或软)
    AnyLimitNeg = 0x0A     # 负限位触发 (硬或软)
    AnyLimit = 0x0F        # 任意限位触发


class SubCmndScope(IntEnum):
    """示波器子命令"""
    SET = 0
    RESET = 1
    START = 2
    STOP = 3


class SubCmndProfile(IntEnum):
    """凸轮子命令"""
    SET_PROFILE = 0
    SET_PARA = 1
    START = 2
    STOP = 3


class SubCmndTMC2209(IntEnum):
    """TMC2209子命令"""
    REG_WRITE = 1
    REG_READ = 2
    REG_SAVE = 3
    ENABLE = 4
    DISABLE = 5
    RESET = 6
    SET_INPUT_MODE = 7


class SubCmndExPosFollow(IntEnum):
    """外部位置跟随子命令"""
    START = 1
    STOP = 2


class SubCmndLua(IntEnum):
    """Lua子命令"""
    CMD_SEND_RUN = 1
    RUN_FILE_RUN = 2
    STOP_FILE_RUN = 3
    PAUSE_FILE_RUN = 4
    CONTINUE_FILE_RUN = 5
    GET_RUNNING_INFO = 16


class FunRes(IntEnum):
    """函数返回值"""
    OK = 1
    ERR = 2


# ============================================================
# CRC16 Modbus 校验
# ============================================================

class CRC16Modbus:
    """CRC16 Modbus 校验表"""
    
    CRCHI = [
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
    ]
    
    CRCLO = [
        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
        0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
        0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
        0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
        0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
        0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
        0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
        0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
        0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
        0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
        0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
        0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
        0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
        0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
        0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
        0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
        0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
        0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
        0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
        0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
        0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
        0x43, 0x83, 0x41, 0x81, 0x80, 0x40
    ]
    
    @classmethod
    def calculate(cls, data: bytes) -> int:
        """计算CRC16 Modbus校验值"""
        crc_hi = 0xFF
        crc_lo = 0xFF
        for byte in data:
            idx = crc_hi ^ byte
            crc_hi = crc_lo ^ cls.CRCHI[idx]
            crc_lo = cls.CRCLO[idx]
        return (crc_hi << 8) | crc_lo


# ============================================================
# 帧协议
# ============================================================

@dataclass
class ResponseFrame:
    """响应帧"""
    cmd_type: int
    sub_cmd: int
    data_len: int
    status: int
    values: List[Union[float, int]] = field(default_factory=list)
    raw_values: List[bytes] = field(default_factory=list)


def build_frame(cmd_type: int, data: bytes = b'', card_index: int = 0) -> bytes:
    """
    构建命令帧
    
    Args:
        cmd_type: 命令类型
        data: 数据载荷
        card_index: 卡索引
    
    Returns:
        完整的命令帧 (8字节帧头 + 数据)
    
    帧结构 (基于 MCC6DLL.dll 反编译):
        [0] u8SpCom_Headr   = 0xAA (帧头)
        [1] u8SpCom_DtLen   = 数据长度 (不含帧头8字节，但含命令头4字节)
        [2] u8SpCom_CardIndx = 卡索引
        [3] u8SpCom_Check   = ~u8SpCom_DtLen (长度取反校验)
        [4] u8SpDt_CheckSum = 校验和 (从第5字节开始累加)
        [5] u8SpDt_CmndType = 命令类型
        [6] u8SpDt_Len      = 数据长度
        [7] u8SpDt_CmndNo   = 命令号 (通常为0)
        [8...] Data         = 数据载荷
    """
    # 数据长度 = payload + 4字节命令头 (CmndType, Len, CmndNo等已计入帧头)
    # 实际协议中 DtLen 是从命令头开始的数据长度
    payload_len = len(data)
    data_len = payload_len + 4  # 含命令头4字节
    
    # 构建帧头 (8字节)
    header = bytearray(8)
    header[0] = FRAME_HEAD                # u8SpCom_Headr = 0xAA
    header[1] = data_len                  # u8SpCom_DtLen
    header[2] = card_index                # u8SpCom_CardIndx
    header[3] = (~data_len) & 0xFF        # u8SpCom_Check (长度取反)
    header[4] = 0                         # u8SpDt_CheckSum (稍后计算)
    header[5] = cmd_type                  # u8SpDt_CmndType
    header[6] = data_len                  # u8SpDt_Len
    header[7] = 0                         # u8SpDt_CmndNo
    
    # 组合帧 (8字节帧头 + 数据载荷)
    frame = bytearray(header) + bytearray(data)
    
    # 校验和计算: 从第5字节 (索引4，u8SpDt_CheckSum位置) 开始，
    # 累加所有后续字节，但要排除校验和字节本身
    # 实际协议: sum(frame[5:]) & 0xFF
    checksum = sum(frame[5:]) & 0xFF
    frame[4] = checksum
    
    return bytes(frame)


def parse_response(data: bytes, expected_values: int = 0) -> Optional[ResponseFrame]:
    """
    解析响应帧
    
    Args:
        data: 响应数据 (原始字节)
        expected_values: 期望的返回值数量
    
    Returns:
        ResponseFrame 或 None
    
    响应帧结构 (stUploadNcSttType):
        [0] u8CmndtType  = 命令类型
        [1] u8SubCmnd    = 子命令
        [2] u8Len        = 数据长度 (返回值数量)
        [3] u8CmndStt    = 命令状态 (0=成功)
        [4...] Val[]     = 返回值数组 (每个 stReturnValType 4字节)
    
    stReturnValType 是 4字节 union，可解析为:
        - float fVal
        - int IVal / uint IUVal
        - short IWVal1, IWVal2 (两个16位值)
    """
    if len(data) < 4:
        return None
    
    # 解析响应帧头 (4字节)
    cmd_type = data[0]
    sub_cmd = data[1]
    data_len = data[2]
    status = data[3]
    
    # 实际返回值数量由 u8Len 指定
    actual_values = min(data_len, expected_values) if expected_values > 0 else data_len
    
    # 解析返回值 (从索引4开始，每个4字节)
    values = []
    raw_bytes = []
    for i in range(actual_values):
        offset = 4 + i * 4
        if offset + 4 <= len(data):
            # 4字节 raw 数据，可按需解析为不同类型
            raw = data[offset:offset+4]
            raw_bytes.append(raw)
            # 默认解析为 float (最常用)
            val = struct.unpack('<f', raw)[0]
            values.append(val)
    
    frame = ResponseFrame(cmd_type, sub_cmd, data_len, status, values, raw_bytes)
    return frame


# ============================================================
# 通信层
# ============================================================

class TCPCommunication:
    """TCP通信"""
    
    # 响应帧最小长度: 4字节头 (cmd_type, sub_cmd, len, status)
    RESPONSE_HEADER_SIZE = 4
    # 返回值单元大小: 4字节
    VALUE_UNIT_SIZE = 4
    
    def __init__(self, ip: str = "192.168.0.7", port: int = 8888, timeout: float = 0.2):
        self.ip = ip
        self.port = port
        self.timeout = timeout
        self.socket: Optional[socket.socket] = None
        self.lock = threading.Lock()
        self.connected = False
    
    def connect(self) -> bool:
        """建立连接"""
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.socket.settimeout(self.timeout)
            self.socket.connect((self.ip, self.port))
            self.connected = True
            return True
        except Exception as e:
            self.connected = False
            return False
    
    def disconnect(self):
        """断开连接"""
        if self.socket:
            try:
                self.socket.close()
            except:
                pass
            self.socket = None
        self.connected = False
    
    def send_and_recv(self, data: bytes, recv_len: int = 0, retry: int = 4) -> Optional[bytes]:
        """
        发送并接收
        
        Args:
            data: 发送数据 (完整命令帧)
            recv_len: 期望的返回值数量 (不是字节长度!)
            retry: 重试次数
        
        Returns:
            响应帧数据 或 None
        
        Note:
            recv_len 是期望的返回值数量，实际接收字节长度为:
            RESPONSE_HEADER_SIZE + recv_len * VALUE_UNIT_SIZE + 1 (校验和)
            C# 原始协议: SpIntrThrdHod = DtLenWanted + 5
        """
        with self.lock:
            for attempt in range(retry):
                try:
                    self.socket.send(data)

                    # 计算实际接收长度 (+1 为响应尾部校验和字节)
                    if recv_len > 0:
                        expected_bytes = self.RESPONSE_HEADER_SIZE + recv_len * self.VALUE_UNIT_SIZE + 1
                    else:
                        # 不确定长度时，先读头部判断
                        expected_bytes = 4096

                    resp = self.socket.recv(expected_bytes)

                    if resp and len(resp) >= self.RESPONSE_HEADER_SIZE:
                        # 截断到数据区（去掉末尾校验和字节），按帧头 u8Len 字段
                        actual_len = resp[2]  # u8Len 字段
                        data_end = self.RESPONSE_HEADER_SIZE + actual_len * self.VALUE_UNIT_SIZE
                        if len(resp) > data_end:
                            resp = resp[:data_end]
                        return resp
                    elif resp:
                        # 响应太短但非空，可能是不带返回值的确认帧
                        return resp
                        
                except socket.timeout:
                    # 超时重试
                    if attempt < retry - 1:
                        continue
                    return None
                except Exception as e:
                    # 其他异常，连接可能已断开
                    self.connected = False
                    return None
            return None


class SerialCommunication:
    """RS232串口通信"""
    
    # 响应帧最小长度: 4字节头 (cmd_type, sub_cmd, len, status)
    RESPONSE_HEADER_SIZE = 4
    # 返回值单元大小: 4字节
    VALUE_UNIT_SIZE = 4
    
    def __init__(self, port: str = "COM3", baudrate: int = 115200, timeout: float = 0.2,
                 log_callback: Optional[Callable[[str, str], None]] = None):
        """
        初始化串口通信
        
        Args:
            port: 串口设备名 (如 "COM3", "/dev/ttyUSB0")
            baudrate: 波特率 (默认 115200)
            timeout: 读超时时间 (秒)
            log_callback: 日志回调函数 (level, message)
        """
        if not SERIAL_AVAILABLE:
            raise ImportError("pyserial 未安装，无法使用串口通信。请运行: pip install pyserial")
        
        self.port = port
        self.baudrate = baudrate
        self.timeout = timeout
        self.serial_conn: Optional[serial.Serial] = None
        self.lock = threading.Lock()
        self.connected = False
        self._log_callback = log_callback
    
    def _log(self, level: str, msg: str):
        """内部日志"""
        if self._log_callback:
            self._log_callback(level, msg)
    
    def connect(self) -> bool:
        """建立串口连接"""
        self._log("INFO", f"尝试打开串口: {self.port} @ {self.baudrate} baud")
        try:
            self.serial_conn = serial.Serial(
                port=self.port,
                baudrate=self.baudrate,
                bytesize=serial.EIGHTBITS,
                parity=serial.PARITY_NONE,
                stopbits=serial.STOPBITS_ONE,
                timeout=self.timeout,
                write_timeout=self.timeout
            )
            self._log("INFO", f"串口已打开: {self.port}, 超时={self.timeout}s")
            
            # 等待串口稳定
            time.sleep(0.1)
            
            # 清空缓冲区
            in_waiting = self.serial_conn.in_waiting
            out_waiting = self.serial_conn.out_waiting
            if in_waiting > 0:
                self._log("WARN", f"清空输入缓冲区: {in_waiting} bytes")
                self.serial_conn.reset_input_buffer()
            if out_waiting > 0:
                self._log("WARN", f"清空输出缓冲区: {out_waiting} bytes")
                self.serial_conn.reset_output_buffer()
            
            self.connected = True
            self._log("INFO", "串口连接成功")
            return True
        except serial.SerialException as e:
            self._log("ERROR", f"串口打开失败: {e}")
            self.connected = False
            return False
        except Exception as e:
            self.connected = False
            return False
    
    def disconnect(self):
        """断开串口连接"""
        if self.serial_conn:
            try:
                if self.serial_conn.is_open:
                    self.serial_conn.close()
            except:
                pass
            self.serial_conn = None
        self.connected = False
    
    def clear_buffers(self):
        """清空输入和输出缓冲区"""
        if self.serial_conn and self.serial_conn.is_open:
            try:
                if self.serial_conn.in_waiting > 0:
                    self.serial_conn.reset_input_buffer()
                if self.serial_conn.out_waiting > 0:
                    self.serial_conn.reset_output_buffer()
            except:
                pass
    
    def send_and_recv(self, data: bytes, recv_len: int = 0, retry: int = 4) -> Optional[bytes]:
        """
        发送并接收
        
        Args:
            data: 发送数据 (完整命令帧)
            recv_len: 期望的返回值数量
            retry: 重试次数
        
        Returns:
            响应帧数据 或 None
        """
        with self.lock:
            for attempt in range(retry):
                try:
                    self._log("TX", f"尝试 #{attempt+1}: 发送 {len(data)} bytes")
                    self._log("TX", f"  数据: {data.hex()}")
                    
                    # 清空输入缓冲区
                    in_waiting = self.serial_conn.in_waiting
                    if in_waiting > 0:
                        self._log("WARN", f"  清空输入缓冲: {in_waiting} bytes")
                        self.serial_conn.reset_input_buffer()
                    
                    # 发送数据
                    self.serial_conn.write(data)
                    self.serial_conn.flush()
                    self._log("TX", f"  已发送，等待响应...")
                    
                    # 不再固定等待 MCU；直接进入按超时受控的读取路径，
                    # 让串口驱动/设备的真实响应时间决定等待时长，避免人为引入 50ms 固定延迟。
                    
                    # 计算期望接收长度 (+1 为响应尾部校验和字节，同C#: SpIntrThrdHod = DtLenWanted + 5)
                    if recv_len > 0:
                        expected_bytes = self.RESPONSE_HEADER_SIZE + recv_len * self.VALUE_UNIT_SIZE + 1
                        self._log("RX", f"  期望接收: {expected_bytes} bytes (header=4 + {recv_len}*4 + 1校验)")
                    else:
                        expected_bytes = 0
                        self._log("RX", f"  未知长度，先读响应头")

                    # 读取响应
                    resp = None
                    if expected_bytes > 0:
                        raw = self._read_exact(expected_bytes, timeout=0.3)
                        if raw:
                            self._log("RX", f"  收到: {len(raw)} bytes: {raw.hex()}")
                            # 去掉末尾校验和字节，只保留 header + data
                            resp = raw[:-1] if len(raw) == expected_bytes else raw
                        else:
                            self._log("RX", f"  超时，未收到数据")
                    else:
                        header = self._read_exact(self.RESPONSE_HEADER_SIZE, timeout=0.3)
                        if header and len(header) >= self.RESPONSE_HEADER_SIZE:
                            self._log("RX", f"  收到响应头: {header.hex()}")
                            actual_len = header[2]  # u8Len 字段
                            self._log("RX", f"  u8Len={actual_len} (返回值数量)")
                            if actual_len > 0:
                                # 数据 + 1字节校验和
                                body_with_checksum = self._read_exact(actual_len * self.VALUE_UNIT_SIZE + 1, timeout=0.2)
                                if body_with_checksum:
                                    # 去掉末尾校验和
                                    resp = header + body_with_checksum[:-1]
                                    self._log("RX", f"  完整响应: {resp.hex()}")
                                else:
                                    self._log("RX", f"  读取返回值超时")
                                    resp = header
                            else:
                                # 只有4字节头 + 1字节校验和，丢弃校验和
                                checksum_byte = self._read_exact(1, timeout=0.1)
                                resp = header
                        else:
                            self._log("RX", f"  响应头超时或长度不足: {header.hex() if header else 'None'}")
                    
                    if resp and len(resp) >= self.RESPONSE_HEADER_SIZE:
                        # 解析响应状态
                        status = resp[3]
                        if status == 0:
                            self._log("RX", f"  响应成功 (status=0)")
                        else:
                            self._log("WARN", f"  响应错误 (status={status})")
                        return resp
                        
                except serial.SerialTimeoutException as e:
                    self._log("ERROR", f"  发送超时: {e}")
                    if attempt < retry - 1:
                        self._log("INFO", f"  重试...")
                        continue
                    return None
                except serial.SerialException as e:
                    self._log("ERROR", f"  串口异常: {e}")
                    self.connected = False
                    return None
                except Exception as e:
                    self._log("ERROR", f"  其他异常: {e}")
                    if attempt < retry - 1:
                        continue
                    return None
            
            self._log("ERROR", f"所有重试失败")
            return None
    
    def _read_exact(self, length: int, timeout: float = 0.1) -> Optional[bytes]:
        """
        精确读取指定长度的数据
        
        Args:
            length: 要读取的字节数
            timeout: 总超时时间
        
        Returns:
            读取的数据 或 None
        """
        if length == 0:
            return b''
        
        data = b''
        start_time = time.time()
        
        while len(data) < length:
            remaining = length - len(data)
            timeout_remaining = timeout - (time.time() - start_time)
            
            if timeout_remaining <= 0:
                break
            
            # 读取可用数据
            try:
                chunk = self.serial_conn.read(min(remaining, self.serial_conn.in_waiting or remaining))
                if chunk:
                    data += chunk
                else:
                    # 无数据，短暂等待
                    time.sleep(0.005)
            except:
                break
        
        return data if len(data) >= length else None
    
    @staticmethod
    def list_available_ports() -> List[str]:
        """
        列出可用的串口
        
        Returns:
            串口设备名列表
        """
        if not SERIAL_AVAILABLE:
            return []
        
        ports = serial.tools.list_ports.comports()
        return [port.device for port in ports]


# ============================================================
# 主驱动类
# ============================================================

class MCCDriver:
    """
    MCC运动控制卡驱动
    
    完整替代 MCC6DLL.dll，支持所有核心功能
    
    使用示例:
        driver = MCCDriver()
        if driver.connect("192.168.0.7", 8888):
            pos = driver.get_axis_pos(0)
            driver.rel_move(0, 10.0, 5.0, 100.0)
            driver.disconnect()
    """
    
    def __init__(self, debug: bool = False, log_callback: Optional[Callable[[str, str], None]] = None,
                 timeout: float = 0.2):
        self._comm: Optional[TCPCommunication] = None
        self._debug = debug
        self._card_index = 0
        self._connected = False
        self._timeout = timeout

        # 日志回调 (level, message)
        self._log_callback = log_callback
        
        # 缓存状态
        self.axis_pos = [0.0] * MAX_AXIS_NUM
        self.axis_spd = [0.0] * MAX_AXIS_NUM
        self.axis_enc = [0] * MAX_AXIS_NUM
        self.input_state = 0
    
    def _log(self, level: str, msg: str):
        """内部日志方法"""
        if self._log_callback:
            self._log_callback(level, msg)
        if self._debug:
            print(f"[{level}] {msg}")
    
    @property
    def connected(self) -> bool:
        return self._connected
    
    # ========== 连接管理 ==========
    
    def connect(self, ip: str = "192.168.0.7", port: int = 8888) -> bool:
        """
        建立TCP连接
        
        Args:
            ip: 控制卡IP地址
            port: 端口号
        
        Returns:
            连接是否成功
        """
        self._comm = TCPCommunication(ip, port)
        if self._comm.connect():
            # 尝试读取位置验证连接
            pos = self.get_axis_pos(0xFF)
            if pos is not None:
                self._connected = True
                return True
            self._comm.disconnect()
        return False
    
    def connect_serial(self, port: str, baudrate: int = 115200) -> bool:
        """
        建立串口连接
        
        Args:
            port: 串口号 (如 "COM3" 或 "/dev/ttyUSB0")
            baudrate: 波特率 (默认 115200)
        
        Returns:
            连接是否成功
        """
        if not SERIAL_AVAILABLE:
            raise ImportError("pyserial 未安装。请运行: pip install pyserial")
        
        self._log("INFO", f"开始串口连接: {port} @ {baudrate} baud")
        
        try:
            # 串口需要更长的超时时间
            serial_timeout = max(self._timeout, 1.0)  # 至少 1 秒
            self._log("INFO", f"串口超时设置: {serial_timeout}s")
            
            # 创建串口通信对象，传递日志回调
            self._comm = SerialCommunication(port, baudrate, timeout=serial_timeout,
                                             log_callback=self._log_callback)
            
            if not self._comm.connect():
                self._log("ERROR", "串口打开失败")
                self._comm = None
                return False
            
            # 等待串口稳定
            self._log("INFO", "等待串口稳定...")
            time.sleep(0.3)
            
            # 清空缓冲区
            self._log("INFO", "清空缓冲区...")
            self._comm.clear_buffers()
            
            # 验证连接：尝试读取位置
            self._log("INFO", "验证连接: 读取轴位置 (axis=0xFF)")
            for retry in range(3):
                self._log("INFO", f"验证尝试 #{retry+1}")
                time.sleep(0.1)
                pos = self.get_axis_pos(0xFF)
                if pos is not None:
                    self._log("INFO", f"验证成功! 收到 {len(pos)} 个轴位置")
                    self._connected = True
                    return True
                else:
                    self._log("WARN", f"验证失败，等待重试...")
            
            # 验证失败
            self._log("ERROR", "所有验证尝试失败，断开串口")
            self._comm.disconnect()
            self._comm = None
            return False
            
        except Exception as e:
            self._log("ERROR", f"串口连接异常: {e}")
            self._comm = None
            return False
    
    @staticmethod
    def list_serial_ports() -> List[str]:
        """
        列出可用的串口
        
        Returns:
            串口设备名列表
        """
        return SerialCommunication.list_available_ports()
    
    def disconnect(self):
        """断开连接"""
        if self._comm:
            self._comm.disconnect()
            self._comm = None
        self._connected = False
    
    def _send_command(self, cmd_type: int, data: bytes = b'', 
                      expected_values: int = 0, retry: int = 4) -> Optional[ResponseFrame]:
        """发送命令并接收响应"""
        if not self._comm:
            return None
        
        frame = build_frame(cmd_type, data, self._card_index)

        resp_data = self._comm.send_and_recv(frame, expected_values, retry)
        
        if resp_data:
            return parse_response(resp_data, expected_values)
        return None
    
    # ========== 位置/速度读取 ==========
    
    def get_axis_pos(self, axis_id: int) -> Optional[Union[float, List[float]]]:
        """
        获取轴位置
        
        Args:
            axis_id: 轴ID (0-5)，0xFF 表示所有轴
        
        Returns:
            单轴位置或所有轴位置列表
        """
        if axis_id < MAX_AXIS_NUM:
            # 单轴
            data = bytes([axis_id, AskInfo.POS_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                self.axis_pos[axis_id] = resp.values[0]
                return resp.values[0]
        elif axis_id == 0xFF:
            # 所有轴
            data = bytes([axis_id, AskInfo.POS_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                for i in range(MAX_AXIS_NUM):
                    self.axis_pos[i] = resp.values[i]
                return resp.values
        return None
    
    def get_axis_spd(self, axis_id: int) -> Optional[Union[float, List[float]]]:
        """
        获取轴速度
        
        Args:
            axis_id: 轴ID (0-5)，0xFF 表示所有轴
        """
        if axis_id < MAX_AXIS_NUM:
            data = bytes([axis_id, AskInfo.SPD_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                self.axis_spd[axis_id] = resp.values[0]
                return resp.values[0]
        elif axis_id == 0xFF:
            data = bytes([axis_id, AskInfo.SPD_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                for i in range(MAX_AXIS_NUM):
                    self.axis_spd[i] = resp.values[i]
                return resp.values
        return None
    
    def get_encoder_val(self, axis_id: int) -> Optional[Union[int, List[int]]]:
        """
        获取编码器值
        
        Args:
            axis_id: 轴ID (0-5)，0xFF 表示所有轴
        """
        if axis_id < MAX_AXIS_NUM:
            data = bytes([axis_id, AskInfo.ENCdr_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                val = int(resp.values[0])
                self.axis_enc[axis_id] = val
                return val
        elif axis_id == 0xFF:
            data = bytes([axis_id, AskInfo.ENCdr_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                for i in range(MAX_AXIS_NUM):
                    self.axis_enc[i] = int(resp.values[i])
                return self.axis_enc[:]
        return None
    
    def get_run_state(self) -> Optional[int]:
        """获取运行状态"""
        data = bytes([0, AskInfo.NCSTT])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            self.run_state = int(resp.values[0])
            return self.run_state
        return None
    
    def get_home_state(self) -> Optional[List[int]]:
        """获取回零状态"""
        data = bytes([0, AskInfo.HOMESTT])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=2)
        if resp and resp.status == 0:
            return [int(v) for v in resp.values[:2]]
        return None
    
    def get_limit_state(self, axis_id: int) -> Optional[Union[int, List[int]]]:
        """
        获取限位状态
        
        Args:
            axis_id: 轴ID (0-5)，0xFF 表示所有轴
        """
        if axis_id < MAX_AXIS_NUM:
            data = bytes([axis_id, AskInfo.LIMITSTT_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                return int(resp.values[0])
        elif axis_id == 0xFF:
            data = bytes([axis_id, AskInfo.LIMITSTT_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                return [int(v) for v in resp.values]
        return None
    
    # ========== 单轴运动控制 ==========
    
    def rel_move(self, axis_id: int, distance: float, speed: float = 0, accel: float = 0) -> bool:
        """
        相对位置运动
        
        Args:
            axis_id: 轴ID
            distance: 相对距离
            speed: 速度 (0表示使用默认速度)
            accel: 加速度 (0表示使用默认加速度)
        """
        sub_cmd = MANOP.MOVERELDIST if speed > 0 else MANOP.MOVERELDISTNOPARA
        data = struct.pack('<BBbbfff', sub_cmd, axis_id, 0, 0, distance, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def abs_move(self, axis_id: int, position: float, speed: float = 0, accel: float = 0) -> bool:
        """
        绝对位置运动
        
        Args:
            axis_id: 轴ID
            position: 目标位置
            speed: 速度
            accel: 加速度
        """
        sub_cmd = MANOP.MOVEABSDIST if speed > 0 else MANOP.MOVEABSDISTNOPARA
        data = struct.pack('<BBbbfff', sub_cmd, axis_id, 0, 0, position, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def abs_move_sync_input(self, axis_id: int, position: float, speed: float, accel: float) -> bool:
        """同步输入绝对位置运动"""
        data = struct.pack('<BBbbfff', MANOP.MOVEABS_SYNCEXIN, axis_id, 0, 0, position, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def rel_move_sync_input(self, axis_id: int, distance: float, speed: float, accel: float) -> bool:
        """同步输入相对位置运动"""
        data = struct.pack('<BBbbfff', MANOP.MOVEREL_SYNCEXIN, axis_id, 0, 0, distance, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def move_at_speed(self, axis_id: int, speed: float = 0, accel: float = 0) -> bool:
        """
        速度运动（无目标位置）
        
        Args:
            axis_id: 轴ID
            speed: 速度
            accel: 加速度
        """
        sub_cmd = MANOP.SPDMOVE if speed != 0 else MANOP.SPDMOVENOPARA
        data = struct.pack('<BBbbfff', sub_cmd, axis_id, 0, 0, 0, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def move_at_speed_with_time(self, axis_id: int, speed: float, accel: float, time_limit: float) -> bool:
        """定时速度运动"""
        data = struct.pack('<BBbbfff', MANOP.SPDMOVEWITHINTIME, axis_id, 0, 0, time_limit, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def change_motion_para(self, axis_id: int, speed: float, accel: float = 0) -> bool:
        """变更运动参数（运行中）"""
        data = struct.pack('<BBbbfff', MANOP.MOVECHAGEVEL, axis_id, 0, 0, 0, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    # ========== 回零 ==========
    
    def home(self, axis_id: int, speed: float = 0, accel: float = 0) -> bool:
        """
        回零
        
        Args:
            axis_id: 轴ID (0-5)
            speed: 回零速度 (0表示使用默认速度)
            accel: 加速度
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        sub_cmd = MANOP.HOMING if speed > 0 else MANOP.HOMINGNOPARA
        data = struct.pack('<BBbbfff', sub_cmd, axis_id, 0, 0, 0, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def home_all(self, speed: float = 0, accel: float = 0) -> bool:
        """
        所有轴回零
        
        Args:
            speed: 回零速度
            accel: 加速度
        
        Returns:
            是否所有轴都成功启动回零
        """
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.home(i, speed, accel):
                success = False
        return success
    
    def home_with_dist(self, axis_id: int, max_dist: float, speed: float, accel: float) -> bool:
        """
        限距回零
        
        Args:
            axis_id: 轴ID
            max_dist: 最大搜索距离
            speed: 回零速度
            accel: 加速度
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.HOMINGWITHINDIST, axis_id, 0, 0, max_dist, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def home_with_switch(self, axis_id: int, speed: float, accel: float, dist: float) -> bool:
        """
        开关回零
        
        Args:
            axis_id: 轴ID
            speed: 回零速度
            accel: 加速度
            dist: 开关距离
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.HOMINGWITHSWIPT, axis_id, 0, 0, dist, speed, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def cancel_home(self, axis_id: int) -> bool:
        """
        取消回零
        
        Args:
            axis_id: 轴ID (0-5)
        
        Returns:
            命令是否成功
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.HOMINGCANCLE, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def cancel_home_all(self) -> bool:
        """
        取消所有轴回零
        
        Returns:
            是否所有轴都成功取消
        """
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.cancel_home(i):
                success = False
        return success
    
    def get_axis_home_state(self, axis_id: int) -> Optional[int]:
        """
        获取单轴回零状态
        
        Args:
            axis_id: 轴ID (0-5)
        
        Returns:
            回零状态码 或 None
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return None
        data = bytes([axis_id, AskInfo.HOMSTT_EACH])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    # ========== 停止控制 ==========
    
    def emergency_stop(self, axis_id: int) -> bool:
        """
        急停 (立即停止，不减速)
        
        Args:
            axis_id: 轴ID (0-5)，不支持广播，需逐轴调用
        
        Returns:
            命令是否成功
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.EMGERNCYSTOP, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def emergency_stop_all(self) -> bool:
        """
        所有轴急停
        
        逐轴发送急停命令，确保所有轴立即停止。
        
        Returns:
            是否所有轴都成功急停
        """
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.emergency_stop(i):
                success = False
        return success
    
    def para_stop(self, axis_id: int, accel: float = 0) -> bool:
        """
        参数停止（减速停止）
        
        Args:
            axis_id: 轴ID (0-5)
            accel: 减速度 (0表示使用默认参数)
        
        Returns:
            命令是否成功
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.PARASTOP, axis_id, 0, 0, 0, 0, accel)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def para_stop_all(self, accel: float = 0) -> bool:
        """
        所有轴减速停止
        
        Args:
            accel: 减速度
        
        Returns:
            是否所有轴都成功停止
        """
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.para_stop(i, accel):
                success = False
        return success
    
    def group_para_stop_uniform(self, accel: float = 0) -> bool:
        """
        组参数停止 (多轴同时减速停止，所有轴使用相同减速度)

        使用 GCODECMND 发送组停止命令。

        Args:
            accel: 减速度 (所有轴相同)

        Returns:
            命令是否成功
        """
        # 使用 group move 的停止模式 (gcode = 0x8B)
        positions = [0.0] * MAX_AXIS_NUM
        speeds = [0.0] * MAX_AXIS_NUM
        accels = [accel] * MAX_AXIS_NUM
        axis_en = [0] * MAX_AXIS_NUM  # 停止时轴使能无意义
        return self._group_move(0x8B, axis_en, positions, speeds, accels)
    
    def pause(self, axis_id: int) -> bool:
        """
        暂停运动
        
        Args:
            axis_id: 轴ID (0-5)
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.PAUSE, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def pause_all(self) -> bool:
        """暂停所有轴"""
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.pause(i):
                success = False
        return success
    
    def restart(self, axis_id: int) -> bool:
        """
        重启运动 (恢复暂停的运动)
        
        Args:
            axis_id: 轴ID (0-5)
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.RESTARTMOVE, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def restart_all(self) -> bool:
        """重启所有轴"""
        success = True
        for i in range(MAX_AXIS_NUM):
            if not self.restart(i):
                success = False
        return success
    
    def quit_motion(self, axis_id: int) -> bool:
        """
        退出运动控制
        
        Args:
            axis_id: 轴ID (0-5)
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        data = struct.pack('<BBbbfff', MANOP.QUITEMOVE, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    # ========== 位置复位 ==========
    
    def reset_coordinate(self, axis_id: int, pos: float, absolute: bool = True) -> bool:
        """
        复位坐标
        
        Args:
            axis_id: 轴ID
            pos: 新位置值
            absolute: True=绝对复位, False=相对复位
        """
        sub_cmd = 1 if absolute else 2
        data = struct.pack('<BBHf', sub_cmd, axis_id, 0, pos)
        resp = self._send_command(CMMND.RESETPOS, data)
        return resp is not None and resp.status == 0
    
    def reset_z(self) -> bool:
        """Z轴复位"""
        if not self._comm:
            return False
        frame = build_frame(CMMND.RESETZ, b'\x00\x00\x00\x00', self._card_index)
        resp_data = self._comm.send_and_recv(frame)
        return resp_data is not None
    
    def set_encoder_pos(self, axis_id: int, pos: int) -> bool:
        """设置编码器位置"""
        data = struct.pack('<BBHi', axis_id, 0, 0, pos)
        resp = self._send_command(CMMND.SETENCODER, data)
        return resp is not None and resp.status == 0
    
    def clear_internal_pulse_count(self, axis_id: int) -> bool:
        """清除内部脉冲计数"""
        data = bytes([axis_id, 0, 0, 0])
        resp = self._send_command(CMMND.CLRPULSECNTINTERNAL, data)
        return resp is not None and resp.status == 0
    
    # ========== 参数读写 ==========
    
    def read_para(self, axis_id: int, para_index: int) -> Optional[float]:
        """
        读取参数
        
        Args:
            axis_id: 轴ID
            para_index: 参数索引
        
        Returns:
            参数值
        """
        data = bytes([axis_id, para_index])
        resp = self._send_command(CMMND.ASKPARA, data, expected_values=1)
        if resp and resp.status == 0:
            return resp.values[0]
        return None
    
    def write_para(self, axis_id: int, para_index: int, value: Union[float, int]) -> bool:
        """
        写入参数
        
        Args:
            axis_id: 轴ID
            para_index: 参数索引
            value: 参数值
        """
        if isinstance(value, float):
            data = bytes([axis_id, para_index, 0, 0]) + struct.pack('<f', value)
        else:
            data = bytes([axis_id, para_index, 0, 0]) + struct.pack('<i', value)
        resp = self._send_command(CMMND.SETPARA, data)
        return resp is not None and resp.status == 0
    
    def save_para_to_rom(self) -> bool:
        """保存参数到ROM"""
        if not self._comm:
            return False
        frame = build_frame(CMMND.SAVESYSPARA, b'\x00\x00\x00\x00', self._card_index)
        resp_data = self._comm.send_and_recv(frame)
        return resp_data is not None
    
    # ========== IO 操作 ==========
    
    def get_input(self, group: int = 0) -> Optional[int]:
        """
        获取输入状态
        
        Args:
            group: 输入组 (0=低16位, 1=高16位)
        """
        info = AskInfo.INPUT_L if group == 0 else AskInfo.INPUT_H
        data = bytes([group, info, 0, 0])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            self.input_state = int(resp.values[0])
            return self.input_state
        return None
    
    def get_output(self, group: int = 0) -> Optional[int]:
        """
        获取输出状态
        
        Args:
            group: 输出组 (0=低16位, 1=高16位)
        """
        info = AskInfo.OUTPUT_L if group == 0 else AskInfo.OUTPUT_H
        data = bytes([group, info, 0, 0])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            self.output_state = int(resp.values[0])
            return self.output_state
        return None
    
    def set_output(self, output_index: int, value: bool) -> bool:
        """
        设置输出
        
        Args:
            output_index: 输出索引
            value: True=置位, False=复位
        """
        sub_cmd = 1 if value else 2
        data = bytes([sub_cmd, output_index, 0, 0])
        resp = self._send_command(CMMND.SETOUTPUT, data)
        return resp is not None and resp.status == 0
    
    def get_interrupt_input(self, group: int = 0) -> Optional[int]:
        """获取中断输入状态"""
        info = AskInfo.INTRUPT_L if group == 0 else AskInfo.INTRUPT_H
        data = bytes([group, info, 0, 0])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    # ========== 多轴运动 ==========
    
    def group_rel_move_ptp(self, axis_en: List[int], positions: List[float], 
                           speeds: List[float], accels: List[float]) -> bool:
        """
        多轴相对PTP运动
        
        Args:
            axis_en: 轴使能列表 [X, Y, Z, A, B, C]
            positions: 目标位置列表
            speeds: 速度列表
            accels: 加速度列表
        """
        return self._group_move(0x80, axis_en, positions, speeds, accels)
    
    def group_abs_move_ptp(self, axis_en: List[int], positions: List[float],
                           speeds: List[float], accels: List[float]) -> bool:
        """多轴绝对PTP运动"""
        return self._group_move(0x00, axis_en, positions, speeds, accels)
    
    def group_rel_move(self, axis_en: List[int], positions: List[float],
                       speeds: List[float], accels: List[float]) -> bool:
        """多轴相对插补运动"""
        return self._group_move(0x81, axis_en, positions, speeds, accels)
    
    def group_abs_move(self, axis_en: List[int], positions: List[float],
                       speeds: List[float], accels: List[float]) -> bool:
        """多轴绝对插补运动"""
        return self._group_move(0x01, axis_en, positions, speeds, accels)
    
    def group_rel_move_ptp_sync(self, axis_en: List[int], positions: List[float],
                                speeds: List[float], accels: List[float]) -> bool:
        """多轴相对PTP运动（同步输入）"""
        return self._group_move(0x80, axis_en, positions, speeds, accels, sync_input=True)
    
    def group_abs_move_ptp_sync(self, axis_en: List[int], positions: List[float],
                                speeds: List[float], accels: List[float]) -> bool:
        """多轴绝对PTP运动（同步输入）"""
        return self._group_move(0x00, axis_en, positions, speeds, accels, sync_input=True)
    
    def group_speed_move(self, axis_en: List[int], speeds: List[float],
                        accels: List[float]) -> bool:
        """多轴速度运动"""
        positions = [0.0] * 6
        return self._group_move(0x8A, axis_en, positions, speeds, accels)
    
    def group_para_stop(self, accels: List[float]) -> bool:
        """多轴参数停止"""
        positions = [0.0] * 6
        speeds = [0.0] * 6
        axis_en = [0] * 6
        return self._group_move(0x8B, axis_en, positions, speeds, accels)
    
    def _group_move(self, gcode: int, axis_en: List[int], positions: List[float],
                    speeds: List[float], accels: List[float], sync_input: bool = False) -> bool:
        """内部多轴运动实现"""
        axis_mask = 0
        for i, en in enumerate(axis_en[:6]):
            if en:
                axis_mask |= (1 << i)
        
        mode = 0x30 | (0x80 if sync_input else 0)
        
        data = bytearray(84)
        data[0] = gcode
        data[1] = axis_mask
        data[2] = 0  # CmndIndx
        data[3] = mode
        
        # 位置
        for i in range(min(6, len(positions))):
            struct.pack_into('<f', data, 4 + i * 4, positions[i])
        
        # 速度
        for i in range(min(6, len(speeds))):
            struct.pack_into('<f', data, 28 + i * 4, speeds[i])
        
        # 加速度
        for i in range(min(6, len(accels))):
            struct.pack_into('<f', data, 52 + i * 4, accels[i])
        
        # GCODECMND may acknowledge a started move without returning a value.
        # Let the response header's u8Len decide how much payload to read; forcing
        # one return value causes valid zero-value ACKs to be treated as timeouts.
        resp = self._send_command(CMMND.GCODECMND, bytes(data))
        return resp is not None and resp.status == 0
    
    # ========== 电子齿轮 ==========
    
    def set_gear_para(self, slave_id: int, master_id: int, gear_ratio: float) -> bool:
        """
        设置齿轮参数
        
        Args:
            slave_id: 从轴ID
            master_id: 主轴ID
            gear_ratio: 齿轮比
        """
        data = bytes([master_id, slave_id, 0, 0]) + struct.pack('<f', gear_ratio)
        resp = self._send_command(CMMND.SETGEAR_PARA, data)
        return resp is not None and resp.status == 0
    
    def gear_start(self, slave_id: int) -> bool:
        """启动齿轮耦合"""
        data = bytes([slave_id, 1, 0, 0]) + struct.pack('<f', 0)
        resp = self._send_command(CMMND.GEAR_OP, data)
        return resp is not None and resp.status == 0
    
    def gear_stop(self, slave_id: int) -> bool:
        """停止齿轮耦合"""
        data = bytes([slave_id, 0, 0, 0]) + struct.pack('<f', 0)
        resp = self._send_command(CMMND.GEAR_OP, data)
        return resp is not None and resp.status == 0
    
    # ========== 凸轮曲线 ==========
    
    def set_profile_curve_para(self, axis_id: int, profile_index: int, params: List[float]) -> bool:
        """
        设置凸轮曲线参数
        
        Args:
            axis_id: 轴ID
            profile_index: 曲线索引
            params: 8个参数值
        """
        if len(params) < 8:
            params = list(params) + [0.0] * (8 - len(params))
        
        data = bytearray(40)
        data[0] = axis_id
        data[1] = profile_index
        data[2] = SubCmndProfile.SET_PROFILE
        data[3] = 0
        
        for i in range(8):
            struct.pack_into('<f', data, 4 + i * 4, params[i])
        
        resp = self._send_command(CMMND.PROFILE_MOVE, bytes(data))
        return resp is not None and resp.status == 0
    
    def start_profile_move(self, axis_id: int, move_dir: int) -> bool:
        """启动凸轮运动"""
        data = struct.pack('<BBBBfHHH', axis_id, 0, SubCmndProfile.START, move_dir,
                          0.0, 0, 0, 0)
        resp = self._send_command(CMMND.PROFILE_MOVE, data)
        return resp is not None and resp.status == 0

    def stop_profile_move(self, axis_id: int) -> bool:
        """停止凸轮运动"""
        data = struct.pack('<BBBBfHHH', axis_id, 0, SubCmndProfile.STOP, 0,
                          0.0, 0, 0, 0)
        resp = self._send_command(CMMND.PROFILE_MOVE, data)
        return resp is not None and resp.status == 0
    
    # ========== 正弦谐波运动 ==========
    
    def start_simple_harmonic_move(self, axis_id: int, rel_abs: int, amplitude: float,
                                   period: float, phase: float, cycles: int) -> bool:
        """
        启动简谐运动
        
        Args:
            axis_id: 轴ID
            rel_abs: 0=相对, 1=绝对
            amplitude: 振幅
            period: 周期
            phase: 相位
            cycles: 循环次数
        """
        data = struct.pack('<BBBBIf', SubCmndProfile.START, axis_id, rel_abs, 0, cycles, amplitude)
        data += struct.pack('<ff', period, phase)
        resp = self._send_command(CMMND.MOVESINE, data)
        return resp is not None and resp.status == 0
    
    def stop_simple_harmonic_move(self, axis_id: int) -> bool:
        """停止简谐运动"""
        data = bytes([SubCmndProfile.STOP, axis_id, 0, 0]) + b'\x00' * 16
        resp = self._send_command(CMMND.MOVESINE, data)
        return resp is not None and resp.status == 0
    
    def change_harmonic_period(self, axis_id: int, period: float) -> bool:
        """变更简谐运动周期"""
        data = bytes([3, axis_id, 0, 0]) + struct.pack('<I', 0) + struct.pack('<ff', period, 0)
        resp = self._send_command(CMMND.MOVESINE, data)
        return resp is not None and resp.status == 0
    
    # ========== 脉冲运动 ==========
    
    def rel_move_and_pulse(self, axis_id: int, spd_dir: int, output_index: int,
                           distance: float, dist_head: float, dist_tail: float,
                           speed: float, dist_pulse: float, pulse_width: int, mode: int) -> bool:
        """相对运动并输出脉冲"""
        data = struct.pack('<BBBBBBBBffff', 0, axis_id, spd_dir, 1, mode, output_index,
                          pulse_width, 0, distance, speed, 0, dist_head)
        data += struct.pack('<ff', dist_tail, dist_pulse)
        resp = self._send_command(CMMND.MOVEPULSE, data)
        return resp is not None and resp.status == 0
    
    def stop_rel_move_and_pulse(self, axis_id: int) -> bool:
        """停止脉冲运动"""
        data = bytes([1, axis_id, 0, 1, 0, 0, 0, 0]) + b'\x00' * 24
        resp = self._send_command(CMMND.MOVEPULSE, data)
        return resp is not None and resp.status == 0
    
    def get_move_pulse_count(self, axis_id: int) -> Optional[int]:
        """获取运动脉冲计数"""
        data = bytes([axis_id, AskInfo.MOVEPULSECNT])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    def get_move_pulse_pos(self, axis_id: int) -> Optional[float]:
        """获取运动脉冲位置"""
        data = bytes([axis_id, AskInfo.MOVEPULSEPOS])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return resp.values[0]
        return None
    
    # ========== 补偿 ==========
    
    def update_compensate(self, axis_id: int) -> bool:
        """更新补偿参数"""
        data = struct.pack('<BBbbfff', MANOP.UPDATECOMPENSATE, axis_id, 0, 0, 0, 0, 0)
        resp = self._send_command(CMMND.MANUAL_OP, data)
        return resp is not None and resp.status == 0
    
    def get_compensate_diff(self, axis_id: int) -> Optional[Union[int, List[int]]]:
        """获取补偿差值"""
        if axis_id < MAX_AXIS_NUM:
            data = bytes([axis_id, AskInfo.COMDIFF_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                return int(resp.values[0])
        elif axis_id == 0xFF:
            data = bytes([axis_id, AskInfo.COMDIFF_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                return [int(v) for v in resp.values]
        return None
    
    def enable_compensate(self, axis_id: int, enable: bool) -> bool:
        """使能补偿功能"""
        return self._enable_function(axis_id, 3, enable)
    
    def enable_soft_limit(self, axis_id: int, enable: bool) -> bool:
        """使能软限位"""
        return self._enable_function(axis_id, 2, enable)
    
    def enable_hard_limit(self, axis_id: int, enable: bool) -> bool:
        """使能硬限位"""
        return self._enable_function(axis_id, 1, enable)
    
    def set_soft_limit(self, axis_id: int, neg_limit: float, pos_limit: float) -> bool:
        """
        设置软限位位置
        
        Args:
            axis_id: 轴ID (0-5)
            neg_limit: 负向软限位位置 (mm)
            pos_limit: 正向软限位位置 (mm)
        
        Returns:
            设置是否成功
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return False
        
        # 依次设置负限位和正限位
        success_neg = self.write_para(axis_id, ParaIndex.SoftLimitNeg, neg_limit)
        success_pos = self.write_para(axis_id, ParaIndex.SoftLimitPos, pos_limit)
        
        return success_neg and success_pos
    
    def get_soft_limit(self, axis_id: int) -> Optional[Tuple[float, float]]:
        """
        获取软限位位置
        
        Args:
            axis_id: 轴ID (0-5)
        
        Returns:
            (负限位, 正限位) 或 None
        """
        if axis_id < 0 or axis_id >= MAX_AXIS_NUM:
            return None
        
        neg = self.read_para(axis_id, ParaIndex.SoftLimitNeg)
        pos = self.read_para(axis_id, ParaIndex.SoftLimitPos)
        
        if neg is not None and pos is not None:
            return (neg, pos)
        return None
    
    def set_all_soft_limits(self, limits: List[Tuple[float, float]]) -> bool:
        """
        设置所有轴的软限位
        
        Args:
            limits: [(neg, pos), ...] 列表，最多6个
        
        Returns:
            是否全部成功
        """
        success = True
        for i, (neg, pos) in enumerate(limits[:MAX_AXIS_NUM]):
            if not self.set_soft_limit(i, neg, pos):
                success = False
        return success
    
    def parse_limit_state(self, state: int) -> dict:
        """
        解析限位状态
        
        Args:
            state: 限位状态值
        
        Returns:
            {
                'hard_pos': bool,  # 正硬限位触发
                'hard_neg': bool,  # 负硬限位触发
                'soft_pos': bool,  # 正软限位触发
                'soft_neg': bool,  # 负软限位触发
                'any_limit': bool, # 任意限位触发
            }
        """
        return {
            'hard_pos': bool(state & LimitBitMask.HardLimitPos),
            'hard_neg': bool(state & LimitBitMask.HardLimitNeg),
            'soft_pos': bool(state & LimitBitMask.SoftLimitPos),
            'soft_neg': bool(state & LimitBitMask.SoftLimitNeg),
            'any_limit': bool(state & LimitBitMask.AnyLimit),
        }
    
    def get_parsed_limit_state(self, axis_id: int) -> Optional[dict]:
        """
        获取并解析限位状态
        
        Args:
            axis_id: 轴ID (0-5)
        
        Returns:
            解析后的限位状态字典 或 None
        """
        state = self.get_limit_state(axis_id)
        if state is not None and isinstance(state, int):
            return self.parse_limit_state(state)
        return None
    
    def set_limit_trigger_level(self, axis_id: int, level: int) -> bool:
        """设置限位触发电平"""
        return self._enable_function(axis_id, 4, level)
    
    def set_home_trigger_level(self, axis_id: int, level: int) -> bool:
        """设置回零触发电平"""
        return self._enable_function(axis_id, 5, level)
    
    def _enable_function(self, axis_id: int, func_type: int, value: int) -> bool:
        """内部使能函数"""
        data = bytes([axis_id, func_type, value, 0])
        resp = self._send_command(CMMND.SETINPUTLEV, data)
        return resp is not None and resp.status == 0
    
    # ========== 平台调平 ==========
    
    def enable_platform_level(self, z_axis: int, x_axis: int, y_axis: int) -> bool:
        """使能平台调平"""
        data = bytes([1, x_axis, y_axis, z_axis])
        resp = self._send_command(CMMND.PLATFORM_COM, data)
        return resp is not None and resp.status == 0
    
    def disable_platform_level(self, axis_id: int) -> bool:
        """禁用平台调平"""
        data = bytes([0, 0, 0, axis_id])
        resp = self._send_command(CMMND.PLATFORM_COM, data)
        return resp is not None and resp.status == 0
    
    # ========== 外部位置跟随 ==========
    
    def start_ex_pos_follow(self, axis_en_bit: int) -> bool:
        """启动外部位置跟随"""
        data = bytes([SubCmndExPosFollow.START, axis_en_bit])
        resp = self._send_command(CMMND.EXPOS_FOLLOW, data)
        return resp is not None and resp.status == 0
    
    def stop_ex_pos_follow(self, axis_dis_bit: int) -> bool:
        """停止外部位置跟随"""
        data = bytes([SubCmndExPosFollow.STOP, axis_dis_bit])
        resp = self._send_command(CMMND.EXPOS_FOLLOW, data)
        return resp is not None and resp.status == 0
    
    # ========== 示波器 ==========
    
    def scope_set_channel(self, channel_id: int, dev_id: int, sample_target: int) -> bool:
        """
        设置示波器通道
        
        Args:
            channel_id: 通道ID
            dev_id: 设备ID
            sample_target: 采样目标
        """
        data = bytes([SubCmndScope.SET, dev_id, channel_id, sample_target, 0, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.SCOPE, data)
        return resp is not None and resp.status == 0
    
    def scope_reset(self) -> bool:
        """重置示波器"""
        data = bytes([SubCmndScope.RESET, 0, 0, 0, 0, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.SCOPE, data)
        return resp is not None and resp.status == 0
    
    def scope_start(self, channel_count: int, sample_count: int, sample_tick: int) -> bool:
        """
        启动示波器采样
        
        Args:
            channel_count: 通道数
            sample_count: 采样点数
            sample_tick: 采样周期
        """
        data = bytes([SubCmndScope.START, 0, 0, 0, channel_count, sample_tick, 0, 0])
        data += struct.pack('<I', sample_count)
        resp = self._send_command(CMMND.SCOPE, data)
        return resp is not None and resp.status == 0
    
    def scope_stop(self) -> bool:
        """停止示波器"""
        data = bytes([SubCmndScope.STOP, 0, 0, 0, 0, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.SCOPE, data)
        return resp is not None and resp.status == 0
    
    # ========== TMC2209 步进驱动 ==========
    
    def tmc2209_enable(self) -> bool:
        """使能TMC2209驱动"""
        data = bytes([SubCmndTMC2209.ENABLE, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    def tmc2209_disable(self) -> bool:
        """禁用TMC2209驱动"""
        data = bytes([SubCmndTMC2209.DISABLE, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    def tmc2209_reset(self) -> bool:
        """复位TMC2209驱动"""
        data = bytes([SubCmndTMC2209.RESET, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    def tmc2209_reg_read(self, reg_index: int) -> Optional[int]:
        """读取TMC2209寄存器"""
        data = bytes([SubCmndTMC2209.REG_READ, 0]) + struct.pack('<h', reg_index) + b'\x00' * 4
        resp = self._send_command(CMMND.TMC2209, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    def tmc2209_reg_write(self, reg_index: int, value: int) -> bool:
        """写入TMC2209寄存器"""
        data = bytes([SubCmndTMC2209.REG_WRITE, 0]) + struct.pack('<h', reg_index)
        data += struct.pack('<I', value)
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    def tmc2209_reg_save(self) -> bool:
        """保存TMC2209寄存器"""
        data = bytes([SubCmndTMC2209.REG_SAVE, 0, 0, 0]) + b'\x00' * 4
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    def tmc2209_set_input_mode(self, index: int, mode: int) -> bool:
        """设置TMC2209输入模式"""
        data = bytes([SubCmndTMC2209.SET_INPUT_MODE, 0]) + struct.pack('<h', index)
        data += struct.pack('<I', mode)
        resp = self._send_command(CMMND.TMC2209, data)
        return resp is not None and resp.status == 0
    
    # ========== Lua 脚本 ==========
    
    def lua_do_string(self, code: str) -> bool:
        """
        执行Lua字符串
        
        Args:
            code: Lua代码 (最大128字符)
        """
        if len(code) > 128:
            return False
        
        data = bytearray(132)
        data[0] = SubCmndLua.CMD_SEND_RUN
        data[1:4] = b'\x00\x00\x00'
        for i, c in enumerate(code[:128]):
            data[4 + i] = ord(c)
        
        resp = self._send_command(CMMND.LUA, bytes(data))
        return resp is not None and resp.status == 0
    
    def lua_run_file(self, file_id: int) -> bool:
        """运行Lua文件"""
        data = bytes([SubCmndLua.RUN_FILE_RUN, 0]) + struct.pack('<H', file_id)
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    def lua_stop_file(self) -> bool:
        """停止Lua文件运行"""
        data = bytes([SubCmndLua.STOP_FILE_RUN, 0, 0, 0])
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    def lua_pause_file(self) -> bool:
        """暂停Lua文件运行"""
        data = bytes([SubCmndLua.PAUSE_FILE_RUN, 0, 0, 0])
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    def lua_continue_file(self) -> bool:
        """继续Lua文件运行"""
        data = bytes([SubCmndLua.CONTINUE_FILE_RUN, 0, 0, 0])
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    # ========== 离线程序运行控制 ==========
    
    def offline_run_set_para(self, para_index: int, value: float) -> bool:
        """
        设置离线运行参数
        
        Args:
            para_index: 参数索引
            value: 参数值
        """
        data = bytes([para_index, 0, 0, 0]) + struct.pack('<f', value)
        resp = self._send_command(CMMND.OFFLINERUN_PARA, data)
        return resp is not None and resp.status == 0
    
    def offline_run_start(self, file_index: int = 0) -> bool:
        """
        启动离线程序运行
        
        Args:
            file_index: 文件索引 (指定运行的程序文件)
        """
        data = bytes([file_index, 0, 0, 0])
        resp = self._send_command(CMMND.OFFLINERUN_START, data)
        return resp is not None and resp.status == 0
    
    def offline_run_stop(self) -> bool:
        """
        停止离线程序运行
        """
        data = bytes([0, 0, 0, 0])
        resp = self._send_command(CMMND.OFFLINERUN_STOP, data)
        return resp is not None and resp.status == 0
    
    def offline_run_clear(self) -> bool:
        """
        清除离线运行状态
        """
        data = bytes([0, 0, 0, 0])
        resp = self._send_command(CMMND.OFFLINERUN_CLEAR, data)
        return resp is not None and resp.status == 0
    
    def offline_run_pause(self) -> bool:
        """
        暂停离线程序运行
        
        Note: 使用 SYSTEM_OP 或特定命令实现暂停
        """
        # 通过 Lua 停止文件运行实现暂停效果
        data = bytes([SubCmndLua.PAUSE_FILE_RUN, 0, 0, 0])
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    def offline_run_resume(self) -> bool:
        """
        继续离线程序运行
        """
        data = bytes([SubCmndLua.CONTINUE_FILE_RUN, 0, 0, 0])
        resp = self._send_command(CMMND.LUA, data)
        return resp is not None and resp.status == 0
    
    def get_offline_run_state(self) -> Optional[int]:
        """
        获取离线运行状态
        
        Returns:
            状态码 或 None
        """
        data = bytes([0, AskInfo.OFFLINEMOVE])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    # ========== G代码/MDI ==========
    
    def send_mdi_command(self, mdi_str: str, cmd_index: int = 0, grp_or_axis: int = 0x30) -> bool:
        """
        发送MDI命令 (简化的G代码解析)
        
        Args:
            mdi_str: MDI命令字符串 (如 "G01 X100 Y50 F500")
            cmd_index: 命令索引
            grp_or_axis: 组/轴模式 (默认 0x30 = 组模式)
        
        Returns:
            命令是否成功
        
        支持的命令:
            - G00/G01: 直线运动
            - G28: 回零
            - G92: 坐标设定
            - M代码: 辅助功能 (部分)
        
        Note:
            这是简化版解析，完整G代码需要专门的解析器
        """
        import re
        
        # 解析命令字符串
        parts = mdi_str.strip().upper().split()
        if not parts:
            return False
        
        # 提取 G代码 和参数
        g_code = None
        axis_pos = {}  # {'X': 100.0, 'Y': 50.0, ...}
        feed_rate = 0.0
        
        for part in parts:
            if part.startswith('G'):
                try:
                    g_code = int(part[1:])
                except ValueError:
                    pass
            elif part.startswith('F'):
                try:
                    feed_rate = float(part[1:])
                except ValueError:
                    pass
            elif part[0] in 'XYZABC':
                try:
                    axis_pos[part[0]] = float(part[1:])
                except ValueError:
                    pass
        
        if g_code is None:
            return False
        
        # 处理不同的 G代码
        axis_map = {'X': 0, 'Y': 1, 'Z': 2, 'A': 3, 'B': 4, 'C': 5}
        
        if g_code == 0 or g_code == 1:
            # G00/G01: 直线运动 (快速/切削)
            if not axis_pos:
                return False
            
            # 构建多轴运动命令
            positions = [0.0] * MAX_AXIS_NUM
            speeds = [feed_rate if feed_rate > 0 else 10.0] * MAX_AXIS_NUM
            accels = [100.0] * MAX_AXIS_NUM
            axis_en = [0] * MAX_AXIS_NUM
            
            for axis_name, pos in axis_pos.items():
                axis_id = axis_map.get(axis_name, -1)
                if axis_id >= 0:
                    positions[axis_id] = pos
                    axis_en[axis_id] = 1
            
            # 使用 GCODECMND 发送
            gcode_byte = 0x00 if g_code == 1 else 0x80  # 绝对/插补 vs PTP
            return self._group_move(gcode_byte, axis_en, positions, speeds, accels)
        
        elif g_code == 28:
            # G28: 回零
            if axis_pos:
                # 先移动到指定位置再回零
                positions = [0.0] * MAX_AXIS_NUM
                axis_en = [0] * MAX_AXIS_NUM
                for axis_name, pos in axis_pos.items():
                    axis_id = axis_map.get(axis_name, -1)
                    if axis_id >= 0:
                        positions[axis_id] = pos
                        axis_en[axis_id] = 1
                # 先移动到中间点
                speeds = [10.0] * MAX_AXIS_NUM
                accels = [100.0] * MAX_AXIS_NUM
                self._group_move(0x00, axis_en, positions, speeds, accels)
            
            # 回零
            return self.home_all()
        
        elif g_code == 92:
            # G92: 坐标设定
            for axis_name, pos in axis_pos.items():
                axis_id = axis_map.get(axis_name, -1)
                if axis_id >= 0:
                    self.reset_coordinate(axis_id, pos, absolute=True)
            return True
        
        else:
            # 未实现的 G代码
            return False
    
    def execute_gcode_line(self, line: str) -> bool:
        """
        执行单行G代码
        
        Args:
            line: G代码行
        
        Returns:
            是否成功执行
        """
        # 去除注释和前后空格
        line = line.strip()
        if not line or line.startswith(';') or line.startswith('#'):
            return True  # 注释行，忽略
        
        # 移除行内注释
        if ';' in line:
            line = line[:line.index(';')].strip()
        
        return self.send_mdi_command(line)
    
    # ========== 信息查询 ==========
    
    def get_version(self) -> Optional[int]:
        """获取控制卡版本"""
        data = bytes([0, AskInfo.VERSION])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
        if resp and resp.status == 0:
            return int(resp.values[0])
        return None
    
    def get_mcu_id(self) -> Optional[List[int]]:
        """获取MCU ID"""
        data = bytes([0, AskInfo.MCU_ID])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=3)
        if resp and resp.status == 0:
            return [int(v) for v in resp.values[:3]]
        return None
    
    def get_all_config_info(self) -> Optional[List[int]]:
        """获取所有配置信息"""
        data = bytes([0, AskInfo.ALLCFGINFO])
        resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=8)
        if resp and resp.status == 0:
            return [int(v) for v in resp.values[:8]]
        return None
    
    def get_ad_val(self, axis_id: int) -> Optional[Union[int, List[int]]]:
        """获取AD值"""
        if axis_id < MAX_AXIS_NUM:
            data = bytes([axis_id, AskInfo.ANA_EACH])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=1)
            if resp and resp.status == 0:
                return int(resp.values[0])
        elif axis_id == 0xFF:
            data = bytes([axis_id, AskInfo.ANA_GRP])
            resp = self._send_command(CMMND.ASKMOVEINFO, data, expected_values=MAX_AXIS_NUM)
            if resp and resp.status == 0:
                return [int(v) for v in resp.values]
        return None
    
    # ========== 手轮/摇杆 ==========
    
    def set_joystick_enable(self, axis_id: int, enable: bool) -> bool:
        """设置摇杆使能"""
        data = bytes([axis_id, 1 if enable else 0])
        resp = self._send_command(CMMND.ANALOGOP, data)
        return resp is not None and resp.status == 0
    
    # ========== 工具函数 ==========
    
    @staticmethod
    def delay_ms(ms: int):
        """毫秒延时"""
        time.sleep(ms / 1000.0)
    
    def set_card_index(self, index: int):
        """设置卡索引"""
        self._card_index = index
    
    # ========== 上下文管理器 ==========
    
    def __enter__(self):
        return self
    
    def __exit__(self, exc_type, exc_val, exc_tb):
        self.disconnect()
        return False


# ============================================================
# 便捷函数
# ============================================================

def create_driver(ip: str = "192.168.0.7", port: int = 8888) -> Optional[MCCDriver]:
    """
    创建并连接驱动
    
    Args:
        ip: 控制卡IP地址
        port: 端口号
    
    Returns:
        已连接的驱动实例，连接失败返回None
    """
    driver = MCCDriver()
    if driver.connect(ip, port):
        return driver
    return None


# ============================================================
# 测试代码
# ============================================================

if __name__ == "__main__":
    # 示例使用
    driver = MCCDriver()
    
    if driver.connect("192.168.0.7", 8888):
        print("连接成功")
        
        # 获取所有轴位置
        positions = driver.get_axis_pos(0xFF)
        if positions:
            for i, pos in enumerate(positions):
                print(f"轴{i} 位置: {pos:.3f}")
        
        # 单轴相对运动
        if driver.rel_move(0, 10.0, 5.0, 100.0):
            print("运动命令发送成功")
        
        # 读取参数
        high_spd = driver.read_para(0, ParaIndex.HighSpd)
        if high_spd is not None:
            print(f"高速: {high_spd}")
        
        # 断开连接
        driver.disconnect()
    else:
        print("连接失败")
