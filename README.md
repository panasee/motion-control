# MCC 运动控制器驱动程序

MCCDemo6 运动控制卡的 Python 驱动和 GUI 界面，支持 TCP/IP 和 RS232 通信。

---

## 目录

1. [环境准备](#1-环境准备)
2. [连接流程](#2-连接流程)
3. [参数设置 SOP](#3-参数设置-sop)
4. [回零流程 SOP](#4-回零流程-sop)
5. [运动控制 SOP](#5-运动控制-sop)
6. [限位设置 SOP](#6-限位设置-sop)
7. [IO 控制 SOP](#7-io-控制-sop)
8. [故障排查](#8-故障排查)

---

## 1. 环境准备

### 1.1 创建测试环境

```bash
conda create -n mcc-test python=3.11 -y
conda activate mcc-test
pip install pyserial PyQt6
```

### 1.2 运行 GUI

```bash
cd /home/dongkai-claw/od-work/claw-dir
python controller_gui.py
```

---

## 2. 连接流程

### 2.1 TCP/IP 连接

**默认参数**:
- IP: `192.168.1.100`
- Port: `5000`

**Python 代码**:
```python
from onway_driver import MCCDriver

driver = MCCDriver()
driver.connect_tcp("192.168.1.100", 5000)
```

**验证连接**:
```python
# 检查连接状态
if driver.is_connected():
    print("连接成功")
    
# 读取轴位置验证
pos = driver.get_axis_pos(0)  # 读取 X 轴位置
print(f"X 轴位置: {pos} mm")
```

### 2.2 RS232 连接

**默认参数**:
- Baudrate: `115200`

**Python 代码**:
```python
driver.connect_serial("/dev/ttyUSB0", 115200)
```

**注意事项**:
- Linux: `/dev/ttyUSB0`, `/dev/ttyACM0`
- Windows: `COM1`, `COM2`, ...

---

## 3. 参数设置 SOP

### 3.1 参数索引对照表

| 参数 | 索引 | 单位 | 说明 |
|-----|------|-----|-----|
| HighSpd | 1 | mm/s | 高速 |
| LowSpd | 2 | mm/s | 低速 |
| HomeSpd | 3 | mm/s | 回零速度 |
| Acc | 4 | mm/s² | 加速度 |
| Pulse | 5 | pulse/mm | 脉冲当量 |
| HomeDist | 6 | mm | 回零距离 |
| SoftLimitNeg | 14 | mm | 负软限位 |
| SoftLimitPos | 15 | mm | 正软限位 |

### 3.2 设置单轴参数

```python
# 设置 X 轴 (轴号 0) 参数
driver.write_para(0, ParaIndex.HighSpd, 100.0)     # 高速 100 mm/s
driver.write_para(0, ParaIndex.LowSpd, 10.0)       # 低速 10 mm/s
driver.write_para(0, ParaIndex.HomeSpd, 50.0)      # 回零速度 50 mm/s
driver.write_para(0, ParaIndex.Acc, 500.0)         # 加速度 500 mm/s²
driver.write_para(0, ParaIndex.Pulse, 1000.0)      # 脉冲当量 1000 pulse/mm
```

### 3.3 读取参数

```python
# 读取 X 轴高速
high_speed = driver.read_para(0, ParaIndex.HighSpd)
print(f"高速: {high_speed} mm/s")

# 读取加速度
accel = driver.read_para(0, ParaIndex.Acc)
print(f"加速度: {accel} mm/s²")
```

### 3.4 参数设置建议值

| 轴类型 | 高速 | 低速 | 回零速度 | 加速度 |
|-------|-----|-----|---------|--------|
| 丝杆传动 | 50-100 | 5-10 | 20-50 | 200-500 |
| 同步带 | 200-500 | 10-20 | 50-100 | 500-1000 |
| 旋转轴 | 100-200 | 10-30 | 50-80 | 300-800 |

---

## 4. 回零流程 SOP

### 4.1 回零模式

| 模式值 | 说明 |
|-------|-----|
| 0 | 单方向回零 (负向) |
| 1 | 双方向回零 |
| 2 | 正向回零 |

### 4.2 单轴回零

```python
# 1. 设置回零参数
driver.write_para(0, ParaIndex.HomeSpd, 50.0)      # 回零速度
driver.write_para(0, ParaIndex.HomeDist, 10.0)     # 回零距离

# 2. 开始回零
driver.home_axis(0, mode=1)  # 双方向回零

# 3. 等待回零完成
while driver.is_moving(0):
    time.sleep(0.1)

# 4. 验证回零结果
pos = driver.get_axis_pos(0)
print(f"回零后位置: {pos} mm (应接近 0)")
```

### 4.3 所有轴回零

```python
# 所有轴同时回零
driver.home_all_axes(mode=1)

# 等待所有轴完成
for axis in range(6):
    while driver.is_moving(axis):
        time.sleep(0.1)
```

---

## 5. 运动控制 SOP

### 5.1 点动控制

```python
# 正向点动 (持续移动)
driver.jog_forward(0)  # X 轴正向点动

# 停止点动
driver.stop_axis(0)

# 负向点动
driver.jog_backward(0)  # X 轴负向点动
```

### 5.2 相对运动

```python
# X 轴移动 +50 mm
driver.rel_move(0, 50.0)

# 指定速度和加速度
driver.rel_move(0, 50.0, speed=100.0, accel=500.0)
```

### 5.3 绝对定位

```python
# X 轴定位到 100 mm 位置
driver.abs_move(0, 100.0)

# 指定速度和加速度
driver.abs_move(0, 100.0, speed=80.0, accel=400.0)
```

### 5.4 多轴联动

```python
# 三轴同时移动到目标位置
driver.line_move(
    axis_list=[0, 1, 2],      # X, Y, Z 轴
    target_pos=[100.0, 50.0, 30.0],
    speed=100.0
)
```

### 5.5 状态查询

```python
# 查询轴位置
pos = driver.get_axis_pos(0)

# 查询运动状态
moving = driver.is_moving(0)

# 查询所有轴位置
all_pos = driver.get_all_axis_pos()
```

---

## 6. 限位设置 SOP

### 6.1 软限位设置

```python
# 设置 X 轴软限位范围
driver.set_soft_limit(0, -150.0, 200.0)  # -150mm ~ +200mm

# 启用软限位
driver.enable_soft_limit(0, True)

# 验证设置
limits = driver.get_soft_limit(0)
print(f"软限位: {limits[0]} ~ {limits[1]} mm")
```

### 6.2 硬限位设置

```python
# 启用硬限位
driver.enable_hard_limit(0, True)

# 设置限位触发电平 (高电平触发 = 1, 低电平触发 = 0)
driver.set_limit_trigger_level(0, 1)
```

### 6.3 限位状态查询

```python
# 获取限位状态 (原始值)
state = driver.get_limit_state(0)

# 获取解析后的限位状态
parsed = driver.get_parsed_limit_state(0)

if parsed:
    if parsed['soft_pos']:
        print("正软限位触发")
    if parsed['soft_neg']:
        print("负软限位触发")
    if parsed['hard_pos']:
        print("正硬限位触发")
    if parsed['hard_neg']:
        print("负硬限位触发")
```

### 6.4 限位设置建议

| 轴行程 | 负软限位 | 正软限位 | 硬限位间距 |
|-------|---------|---------|-----------|
| 300mm | -145mm | +145mm | 5mm |
| 500mm | -245mm | +245mm | 5mm |
| 1000mm | -495mm | +495mm | 5mm |

**建议**: 软限位应在硬限位内侧保留 5mm 安全距离。

---

## 7. IO 控制 SOP

### 7.1 输入信号查询

```python
# 查询所有输入状态
inputs = driver.get_all_input_state()
print(f"输入状态: {inputs}")

# 查询单个输入 (位掩码)
input_0 = (inputs >> 0) & 1  # 输入 0
input_1 = (inputs >> 1) & 1  # 输入 1
```

### 7.2 输出信号控制

```python
# 设置输出 0 为高
driver.set_output(0, True)

# 设置输出 1 为低
driver.set_output(1, False)

# 读取输出状态
outputs = driver.get_all_output_state()
```

---

## 8. 故障排查

### 8.1 连接失败

| 问题 | 解决方法 |
|-----|---------|
| TCP 连接超时 | 检查 IP 地址、网络连接、控制器电源 |
| 串口无法打开 | 检查端口权限 `chmod 666 /dev/ttyUSB0` |
| 无法读取位置 | 检查控制器是否正常启动 |

### 8.2 运动异常

| 问题 | 可能原因 | 解决方法 |
|-----|---------|---------|
| 轴不动 | 限位触发、使能未开 | 检查限位状态、使能轴 |
| 位置偏差 | 脉冲当量设置错误 | 重新设置 Pulse 参数 |
| 运动卡顿 | 加速度过大 | 降低加速度参数 |

### 8.3 限位报警

```python
# 查询限位状态
state = driver.get_parsed_limit_state(0)

if state['any_limit']:
    # 限位触发处理
    if state['hard_pos'] or state['hard_neg']:
        print("硬限位触发，需手动复位")
    
    if state['soft_pos']:
        driver.rel_move(0, -5.0)  # 负向移动脱离限位
    
    if state['soft_neg']:
        driver.rel_move(0, 5.0)   # 正向移动脱离限位
```

### 8.4 常用诊断命令

```python
# 检查连接
driver.is_connected()

# 读取所有轴位置
driver.get_all_axis_pos()

# 读取所有输入状态
driver.get_all_input_state()

# 读取轴运动状态
driver.get_axis_state(0)

# 读取限位状态
driver.get_parsed_limit_state(0)
```

---

## 附录：API 快速参考

### 连接相关

| 函数 | 说明 |
|-----|-----|
| `connect_tcp(ip, port)` | TCP 连接 |
| `connect_serial(port, baudrate)` | 串口连接 |
| `disconnect()` | 断开连接 |
| `is_connected()` | 检查连接状态 |

### 参数相关

| 函数 | 说明 |
|-----|-----|
| `read_para(axis, index)` | 读取参数 |
| `write_para(axis, index, value)` | 写入参数 |

### 运动相关

| 函数 | 说明 |
|-----|-----|
| `home_axis(axis, mode)` | 单轴回零 |
| `home_all_axes(mode)` | 所有轴回零 |
| `jog_forward(axis)` | 正向点动 |
| `jog_backward(axis)` | 负向点动 |
| `stop_axis(axis)` | 停止轴 |
| `stop_all_axes()` | 停止所有轴 |
| `rel_move(axis, dist)` | 相对运动 |
| `abs_move(axis, pos)` | 绝对定位 |
| `line_move(axes, poses, speed)` | 多轴联动 |
| `get_axis_pos(axis)` | 读取轴位置 |
| `is_moving(axis)` | 检查运动状态 |

### 限位相关

| 函数 | 说明 |
|-----|-----|
| `enable_soft_limit(axis, enable)` | 启用软限位 |
| `enable_hard_limit(axis, enable)` | 启用硬限位 |
| `set_soft_limit(axis, neg, pos)` | 设置软限位位置 |
| `get_soft_limit(axis)` | 获取软限位位置 |
| `get_limit_state(axis)` | 获取限位状态 |
| `get_parsed_limit_state(axis)` | 获取解析后限位状态 |

### IO 相关

| 函数 | 说明 |
|-----|-----|
| `set_output(bit, value)` | 设置输出 |
| `get_all_input_state()` | 读取所有输入 |
| `get_all_output_state()` | 读取所有输出 |
