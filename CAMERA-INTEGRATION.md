# DS-Fi3 工作台整合记录

> 应用边界已按“显微镜与 motion-control 并列”原则调整，当前架构见 [APPLICATION-ARCHITECTURE.md](APPLICATION-ARCHITECTURE.md)。下方实机测试保留当时记录；本次架构调整使用离线验证，没有重新连接硬件。

2026-11-11，在 Windows x64 上使用 `KsCam.dll` 完成 Nikon DS-Fi3 工作台整合验证。首次独立验证见 [VALIDATION-DS-Fi3.md](VALIDATION-DS-Fi3.md)。

## 使用

```powershell
.\.venv-camera\Scripts\python.exe microscope_app.py
# 或在运动程序中通过“工具 → 显微镜工作台”启动独立进程
.\.venv-camera\Scripts\python.exe controller_gui.py
```

先释放 NIS-Elements 对相机的占用。窗口启动不连接相机；点击“连接相机”读取设备实际参数，再编辑曝光、增益和采样并点击“应用参数”。预览、单帧使用已应用参数；未应用的编辑不影响采集。采集使用手动曝光、自由运行。白平衡依旧是软件校色，保留收到的原始 RGB8。

停止预览冻结最后一帧，单帧取得一次新采集的图像。“保存相机原图与参数”输出 PNG 与同名 JSON，PNG 不包含软件白平衡或标记，也不是传感器 RAW。显微镜自身关闭、断开会停止采集、恢复连接前配置并释放设备；关闭运动程序不影响显微镜。异常在相机面板显示，清理失败不会被当作成功。

首次切换到相机、切换采样模式或帧尺寸改变会清除旧标记并取消标定；已有标记时先确认。相同来源与采样的帧保留标记、视图和 RGB 路径。物镜等光学变化需用户重新标定。

## 软件边界与 SDK 迁移

| 文件 | 责任 |
|---|---|
| `microscope_camera.py` | `CameraBackend` 接口、`CameraFrame`、单线程采集会话、最新帧邮箱、停止和断开 |
| `nikon_ks_camera.py` | 实机选择、参数范围与单位、全幅/平均采样映射、BGR→RGB、帧校验、参数恢复 |
| `nikon_ks_bindings.py` | 经本机验证的 ctypes 结构体、函数签名及 SHA-256 保护；诊断探针共用此处 |
| `microscope_workspace.py` | UI、软件白平衡、标记、测量、RGB 曲线、保存；不依赖 Nikon ABI |
| `microscope_app.py` | 独立应用入口、单实例唤回、自有图像服务和进程生命周期 |
| `controller_gui.py` / `peer_apps.py` | 运动程序中的外部应用启动入口；不导入或持有显微镜窗口 |

后端只在工作线程中创建和使用。接口以毫秒、增益倍率和拥有独立内存的连续 RGB8 数组交换数据；UI 不持有 SDK 缓冲区。归一化模式 ID 1 为全幅、3 为平均采样；新 SDK 后端自行映射到其枚举。界面约 15 Hz 读取最新帧，邮箱容量为一帧，控制队列有上限，SDK 调用日志最多保留 128 条。采集帧率、帧号缺口与主动跳过的显示帧分开计数。这是预览及单帧工作台，没有无损录像承诺。

拿到 DSCam.dll 后应新增后端并替换工厂，界面、会话、标记、测量和保存逻辑预计可复用。仍须对照正式头文件确认枚举、结构体布局、调用约定、曝光单位、像素布局、回调/轮询、缓冲区寿命和关闭流程，再运行同一套验收。当前不能承诺只换文件名；不同版本 ABI 不通用。

默认运行库：`C:\Program Files\NIS-Elements D\KsCam.dll`，2.1.2.3，SHA-256 `4b44e3d45b148c9c9580525f0e51ed39743dc8d837bbe06e36343f9a6061335e`。环境变量 `NIKON_KSCAM_DLL` 可指定另一个路径，但仍必须匹配该散列。未安装 SDK、未替换驱动、未复制厂商 DLL 到仓库。

## 整合后的实机验收

完整数据：[result.json](docs/camera-validation/integration/result.json)。图像：[single-original.png](docs/camera-validation/integration/single-original.png)，参数：[single-original.json](docs/camera-validation/integration/single-original.json)，界面：[workspace.png](docs/camera-validation/integration/workspace.png)。这些文件位于忽略的本地证据目录。

| 模式 | 曝光请求/硬件读回/帧读回 | 本轮帧数 | 实测采集速率 | 缺失帧号/重复 |
|---|---|---:|---:|---:|
| 全幅 2880×2048 | 100 / 100 / 100 ms | 25 | 9.98 fps | 0 / 0 |
| 平均采样 1440×1024 | 20 / 20 / 20 ms | 66 | 29.94 fps | 0 / 0 |
| 平均采样 1440×1024 | 50 / 50 / 50 ms | 21 | 15.06 fps | 0 / 0 |

三组增益请求和帧读回均为 2.00×。设备曝光范围 0.1–30000 ms、步长 0.02 ms；增益 1–64×、步长 0.01×。SDK 为 BGR24，工作台存储与显示为 RGB8。像素区不含帧尾：全幅行宽 8640 bytes、SDK 缓冲区 17703360 bytes；平均采样行宽 4320 bytes、SDK 缓冲区 4428000 bytes。

连续预览测试中 UI 20 ms 心跳最大间隔约 47 ms；单帧、原图及参数保存、RGB 路径、视图与标记保留通过。关闭活动预览、重开窗口后显式重连通过。30 秒曝光期间断开约 453 ms，线程退出成功。导出和截图期间包含同步文件处理，整个验收的 UI 最大心跳间隔约 906 ms；这不代表取帧 SDK 在 UI 线程执行。

最终关闭无恢复错误；重开时读回连接前配置：全幅、自动曝光模式、曝光 74.7 ms、增益 2×。验收进程 `imported_motion_modules=[]`、`stage_operations=[]`。所有实机操作仅经过独立工作台，没有导入主控制台、Onway、Nexcope 或 serial。

此前全幅 20 ms 的独立测试存在帧号缺口，本轮没有重新宣称该模式无损；平均采样 50 ms 的实测速率约 15 fps，不能仅以 1/曝光推算速率。没有做 USB 拔插、数小时稳定性或 NIS 争用的破坏性测试；SDK 错误路径由离线替身覆盖。

开发中修复了停止后残留图像事件导致重新拍摄 `CAM_GetImage: -10` 的问题：新采集前在停止状态清空图像事件队列，其他错误仍抛出。旧公开头文件将 -10 定义为 ACCESSDENIED；本机固定版本的非阻塞 EventPolling 在队列空时返回该码，因此该解释只用于这个固定版本及调用点，不能泛化到所有 API。另修复了异步关闭后连接按钮未恢复的问题。失败验收报告保留在同目录。长曝光验收脚本曾停在切换来源确认框，已修复脚本确认逻辑；中断后显式恢复了设备原配置，恢复记录为 `acceptance-harness-recovery.json`，最终完整重跑通过。

## 离线回归

**106 passed, 41 subtests passed in 6.75s**；报告为 [offline-regression.xml](docs/camera-validation/integration/offline-regression.xml)。所有自动测试使用离线替身，不加载 DLL。

```powershell
$env:QT_QPA_PLATFORM='offscreen'
.\.venv-camera\Scripts\python.exe -m pytest tests/test_microscope_camera.py tests/test_nikon_ks_camera.py tests/test_nikon_ks_probe.py tests/test_microscope_imaging.py tests/test_microscope_workspace.py tests/test_stack_designer.py tests/test_controller_shell.py -q
```

新增覆盖单线程所有权、有界帧缓存、新单帧、长曝光取消、失败后关闭、父窗口退出协调、重开后显式连接、实时图像保留标记/视图、分辨率改变失效标定、原图与参数配对、BGR 转换及缓冲区独立性、帧尾校验、帧号回绕、残留事件清理和恢复失败后的释放。

实机验收命令（会连接相机，结束恢复原参数；无运动驱动）：

```powershell
$env:QT_QPA_PLATFORM='offscreen'
.\.venv-camera\Scripts\python.exe tools/validate_camera_integration.py --hardware
```

独立诊断仍可用 `python -m tools.nikon_ks_probe`；显式增加 `--capture` 或 `--validate` 才拍摄。
