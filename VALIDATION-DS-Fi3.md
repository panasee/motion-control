# Nikon DS-Fi3 第一阶段实机验证

> 本文件保留首次独立验证的历史记录。后续已按用户要求完成 KsCam 工作台整合，当前使用说明与整合实测见 [CAMERA-INTEGRATION.md](CAMERA-INTEGRATION.md)；下文“暂不接入界面”描述仅适用于首次验证阶段。

日期：2026-11-11，Asia/Singapore。第一阶段已完成离线回归、独立枚举、单帧和连续取帧、曝光读回及通道顺序验证。全程未连接 Onway / Nexcope、未打开串口、未发送运动指令，也未启动 NIS-Elements 或主控制 GUI。

**接入结论：下一阶段优先直接调用本机 Nikon `KsCam.dll` 的 `CAM_*` 接口。** 该路径已在实机通过验证。暂不改动工作台界面或启用硬件按钮；本次交付是独立验证脚本和证据。正式接入应保留运行库版本检查、线程隔离、参数回读和确定的关闭流程。

## 离线回归

- 工作台回归：**85 passed, 41 subtests passed in 6.96s**。
- 新增探针离线测试：**8 passed**，覆盖拒绝模拟相机、拒绝多台实机歧义、空设备指针、关闭出错后仍释放设备列表，以及 dropless 恢复失败后仍尝试恢复其他参数。这些测试不加载相机 DLL。

使用项目内 `.venv-camera`，复用本机现有 Python 包；仅在该虚拟环境补装 OpenCV 及静态二进制检查工具 capstone。未修改全局 Python 环境。

| 组件 | 本机验证版本 |
|---|---|
| Python | 3.12.0，64 位 |
| PyQt6 | 6.7.1 |
| NumPy | 1.26.4 |
| OpenCV | 4.11.0.86 |
| pytest | 9.0.3 |
| pyserial | 3.5 |

复现离线回归：

```powershell
$env:QT_QPA_PLATFORM = 'offscreen'
.\.venv-camera\Scripts\python.exe -m pytest tests/test_microscope_imaging.py tests/test_microscope_workspace.py tests/test_stack_designer.py tests/test_controller_shell.py -q
.\.venv-camera\Scripts\python.exe -m pytest tests/test_nikon_ks_probe.py -q
```

## 本机设备、驱动和 SDK 盘点

| 项目 | 实测 / 本机文件信息 |
|---|---|
| Windows | Windows 11 Home，10.0.26200，64 位 |
| PnP 设备名 | Nikon DS-Fi3 |
| PnP 状态 | OK，ConfigManagerErrorCode=0 |
| 驱动 | Nikon Corporation，1.4.0.0，2022-04-19，`oem112.inf` |
| SDK 枚举的实机 | index=0，type=5，DS-Fi3 |
| 相机固件 / FPGA / FX3 | 110.04 / 3707 / E9 |
| SDK 报告的 USB | 3.0；未另用 USB 拓扑工具复核链路速度 |
| 其余枚举项 | DS-Ri2(Sim)、DS-Qi2(Sim)、DS-Fi3(Sim)，均未打开 |
| 实际使用的运行库 | `C:\Program Files\NIS-Elements D\KsCam.dll` |
| 运行库版本 / 位数 | 2.1.2.3，AMD64 |
| 运行库 SHA-256 | `4b44e3d45b148c9c9580525f0e51ed39743dc8d837bbe06e36343f9a6061335e` |

已安装 NIS-Elements D 6.02.00 LO、D 5.30.00 LO、F 5.21.00。Fi3 驱动文件位于 `C:\Program Files\Nikon\Shared\Drivers\Fi3`，NIS-Elements D 安装目录也保留了对应驱动副本。

在 Nikon / NIS 安装目录、Program Files、当前用户目录、D 盘应用 / 下载目录和用户项目目录中，未找到完整的 **DS-10-Fi3-Qi2-Ri2 Digital Sight SDK**、`DSCam.dll`、本机 SDK 头文件或 Fi3 采集示例。发现的 `U3L3-SDK` 是旧 DS-U3/L3 组件，`Examples/NkWindowDemo.mac` 是 NIS 宏示例，均不是本次所需的 Fi3 独立采集示例。搜索范围和访问错误已保存在原始记录中；这不是对所有磁盘目录不存在 SDK 的绝对断言。

因此本次没有运行“本机 SDK 自带示例”。替代验证方法是：读取 NikonKS 原作者仓库中的公开 `KsCam*.h` 和 Micro-Manager 调用流程，静态检查本机 DLL 导出及结构偏移，再编写独立 Python 探针。未下载或替换 Nikon 驱动 / DLL。

本机运行库和旧公开头文件存在差异：格式描述类型编号为 6；`CAM_Image` 含前导虚表槽和额外 `uiEndTime64`，x64 大小为 56 字节。本次核对了 `CAM_GetImage` 的缓冲区与字段偏移。探针将调用限制在上述 SHA-256 对应运行库，遇到另一版本会拒绝加载，不能把它直接视为所有 Digital Sight SDK 版本通用的封装。

## 单帧、尺寸与数据布局

`CAM_OpenDevices`、`CAM_Open`、参数读取、`CAM_GetImage`、停止传输、`CAM_Close`、`CAM_CloseDevices` 均已成功返回 0。单帧测试采用启动自由运行、等待一帧、复制数据、停止；**外触发和硬件同步未测试**。

| 项目 | 全幅 mode=1 | 2×2 平均 mode=3 |
|---|---:|---:|
| 实拍尺寸 | 2880 × 2048 | 1440 × 1024 |
| SDK 格式名称 | RGB 24 | RGB 24 |
| 内存字节顺序 | **B、G、R** | **B、G、R** |
| 数组类型 | uint8，H×W×3 | uint8，H×W×3 |
| 输出位深 | 每通道 8 位，合计 24 bit/px | 每通道 8 位，合计 24 bit/px |
| 图像行步长 | 8640 字节 | 4320 字节 |
| 像素数据长度 | 17,694,720 字节 | 4,423,680 字节 |
| SDK 要求的总缓冲区 | 17,703,360 字节 | 4,428,000 字节 |
| 像素后额外空间 | 8640 字节 | 4320 字节 |

这两种输出的像素行无额外填充；总缓冲区比像素多一行空间，前 256 字节可读取帧信息。不能把整块缓冲区当作像素。帧信息中的尺寸、格式和曝光与实际设置相符。该固件的旧版 footer serial 字段返回 0，设备身份以枚举并打开的实机 handle 为准，不能据此误判为模拟图像。

还枚举到了 mode=2 的 1440×1024 **ROI** 及对应 YUV444 模式；本次没有对 ROI / YUV444 进行取帧验证。mode=2 和 mode=3 尺寸相同但视野不同，工作台必须区分并分别标定。输出为 RGB24 不代表传感器原始 ADC 位深，也不代表已完成颜色或光强标定。

原始单帧数组：`docs/camera-validation/kscam-validation-2.raw.npy`。
已按 BGR→RGB 转换的 PNG：`docs/camera-validation/single-frame-rgb.png`。本次视野为近乎均匀的黄绿色背景；未为取得结构化样品图像移动或调焦。

### 通道顺序的实测依据

在 mode=3、20 ms、固定增益下，分别将相机红增益 89→119、蓝增益 77→107，每次取 5 帧，使用最后一帧按 16 px 网格采样平均值，随后恢复。帧内白平衡字段也确认了这些设置。

| 设置 | byte 0 平均值 | byte 1 平均值 | byte 2 平均值 |
|---|---:|---:|---:|
| 原设置 R=89，B=77 | 12.876 | 66.330 | 47.726 |
| 只提高红增益 | 13.495 | 61.808 | 66.089 |
| 只提高蓝增益 | 28.229 | 65.155 | 47.151 |
| 恢复原设置 | 12.422 | 66.446 | 47.299 |

红控制主要提高 byte 2，蓝控制主要提高 byte 0，确认 **BGR**，与 NikonKS 源码中的 BGR 转换路径一致。SDK 的颜色处理会影响其他通道，这个测试确认的是字节顺序，不是增益线性度或白平衡准确度。

## 曝光和连续采集

曝光属性从硬件重新读取使用 `CAM_GetAllFeatures`；旧头文件说明 `CAM_GetFeatures` 可能只返回 SDK 缓存。本次同时核对实际帧 footer，避免只记录 UI 设定值。

| 请求曝光 | 硬件读回 | 连续三帧 footer |
|---|---|---|
| 10 ms | 10,000 µs | 全部 10,000 µs |
| 20 ms | 20,000 µs | 全部 20,000 µs |
| 50 ms | 50,000 µs | 全部 50,000 µs |
| 100 ms，全幅补测 | 100,000 µs | 30 帧均为 100,000 µs |

曝光描述：最小 100 µs，最大 30,000,000 µs，步长 20 µs，默认 10,000 µs。未测试极限曝光和非步长对齐输入的舍入行为。

| 连续取帧条件 | 帧数 | 主机实际取帧率 | 帧号 / 数据检查 |
|---|---:|---:|---|
| 全幅，20 ms，默认传输 | 60 | 16.339 fps | 60 个不同图像；帧号间共缺 49 个编号，无重复 |
| 2×2 平均，20 ms，默认传输 | 60 | 29.949 fps | 帧号连续，60 个不同图像 |
| 全幅，20 ms，dropless=true | 60 | 16.347 fps | 仍缺 49 个编号，无重复 |
| 2×2 平均，20 ms，dropless=true | 60 | 29.997 fps | 帧号连续，60 个不同图像 |
| 全幅，100 ms，默认传输 | 30 | 9.993 fps | 帧号连续，事件与图像帧号一致 |

帧率计算为 `(帧数−1)/(末帧收到时间−首帧收到时间)`，是短时主机交付率，包含本探针的数据复制和校验开销，不是厂商性能保证。全幅 20 ms 的帧号跳跃在 dropless 开关两种状态均出现；不能宣称该条件下无丢帧，跳号具体发生于传感器、传输还是运行库尚未定位。100 ms 全幅及 20 ms 缩小模式通过帧号连续性检查。

## 恢复与路线选择

所有测试结束均停止传输、恢复并读回被修改的格式、ROI 位置、测光区域、曝光、自动曝光模式、增益、触发模式和红蓝白平衡，随后关闭相机并释放列表。dropless 从 false 改为 true 后也恢复为 false。最终保存的原设置为全幅 RGB24、曝光值 74.7 ms、连续自动曝光模式、增益值 200、红增益 89、蓝增益 77、触发关闭。曝光值是该次恢复读回，重新启用自动曝光后后续成像时仍可能变化。

**选择直接调用运行库的依据：** 本机路径已完成真实枚举、单帧、连续采集和读回；无需运行 NIS 主程序。Micro-Manager / pymmcore-plus / NikonKS 适配器在本机未安装，`DSCam.dll` 也未找到，因此没有把“未测试 NikonKS”写成“适配器实测失败”。此外，审阅的 NikonKS 源码 `UpdateImageSettings()` 仍按 Ri2/Qi2 硬编码模式尺寸；本机 Fi3 的 mode=1 实测为 2880×2048，而该分支按 4908×3264 处理。直接采用该逻辑存在尺寸不匹配风险。

后续工作台接入建议：相机专用工作线程负责取帧和释放，将 BGR 转换为连续 RGB8 数组交给现有分析；实时预览更新已有图像项，保留视野和标记，不循环调用 `load_image()`。初始预览可选择已验证帧号连续的 mode=3；全幅定量序列需明确曝光和连续性要求。当前参数记忆保留为用户预设，连接后以驱动范围和实际回读为准。运动台整合仍留在下一阶段。

## 复现和证据

独立探针：[tools/nikon_ks_probe.py](tools/nikon_ks_probe.py)。默认只枚举、打开、读参数和关闭；添加参数才会拍摄。

```powershell
.\.venv-camera\Scripts\python.exe tools\nikon_ks_probe.py
.\.venv-camera\Scripts\python.exe tools\nikon_ks_probe.py --capture --output docs/camera-validation/new-single.json
.\.venv-camera\Scripts\python.exe tools\nikon_ks_probe.py --validate --output docs/camera-validation/new-validation.json
.\.venv-camera\Scripts\python.exe tools\nikon_ks_probe.py --validate --dropless --output docs/camera-validation/new-dropless.json
```

原始记录保存在现有 Git 忽略的 `docs/camera-validation/` 中，随本机目录保留，不会自动随 Git 提交：

- `handoff-verification.json`：六文件哈希、分支、bundle 检查。
- `offline-regression.txt` / `.xml`、`probe-offline-tests.txt` / `.xml`：回归结果。
- `windows-inventory.json`、`python-environment.json`：系统和环境清单。
- `sdk-search-*.txt`、`sdk-user-search-*.txt`、`nis-interface-files.txt`：搜索范围与结果。
- `nis-runtime-pe.json`、`getimage-disassembly.txt`：本机运行库静态核对。
- `kscam-readonly-3.json`：枚举和参数读取成功记录。
- `kscam-validation-2.json`：默认传输、曝光与 BGR 实测。
- `kscam-dropless.json`：dropless 对照、参数恢复和 API 返回值。
- `full-frame-100ms.json`：全幅慢曝光补测。
- `kscam-final-single.json`：最终脚本再次完成实机单帧，参数恢复和释放错误列表均为空。
- 早期失败记录保留：旧格式枚举编号差异、footer serial 为零的误判、首次全幅跳号检查，均未隐去；每次都正常关闭相机。

参考接口来源：[NikonKS 安装说明](https://micro-manager.org/NikonKS)、[NikonKS 作者公开头文件](https://github.com/andrewgomella/NikonKsAdapter/tree/38c8cd81942fcaa2b7a91087f132e3feaee18e0f/SDK)、[审阅的 Micro-Manager NikonKsCam.cpp](https://github.com/micro-manager/mmCoreAndDevices/blob/af52af3d137189ff1e4e558737572597578b17b3/DeviceAdapters/NikonKs/NikonKsCam.cpp)、[Nikon DS-Fi3 厂商规格](https://www.microscope.healthcare.nikon.com/products/cameras/ds-fi3/specifications)。SDK 完整开发包的正规获取入口由 NikonKS 文档指向 Nikon 开发者注册与授权；本次已安装运行库的测试成功不等于已取得可再分发 SDK。
