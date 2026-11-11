# MCC 运动控制器驱动程序

MCCDemo6 运动控制卡的 Python 驱动和 GUI 界面，支持 TCP/IP 和 RS232 通信。

## Current Entrypoints

- Motion application entrypoint: `controller_gui.py`
- Independent microscope application entrypoint: `microscope_app.py` (legacy command `microscope_workspace.py` delegates to it)
- Onway driver: `onway_driver.py`
- Nexcope driver: `nexcope_driver.py`
- `controller_gui.py` now hosts both `Onway` and `Nexcope` tabs
- `Onway` keeps the existing six-axis logic; `Nexcope` uses a separate three-axis page with XY-only position records

## 显微镜工作台与 Nikon DS-Fi3

显微镜是独立应用，与 motion-control 并列。运行 `.\.venv-camera\Scripts\python.exe microscope_app.py` 即可使用；运动程序中的 **工具 → 显微镜工作台…** 仅启动独立进程，重复启动会唤回已有窗口。关闭任一程序不会关闭另一程序。架构边界与后续运动接入原则见 [APPLICATION-ARCHITECTURE.md](APPLICATION-ARCHITECTURE.md)。
显微镜不依赖运动程序。相机与测量核心依赖 `PyQt6`、`numpy`；“工具”菜单中的叠层设计器按需加载，使用 OpenCV，完整安装包已包含该工具及其依赖声明。相机后端要求 Windows x64 与已验证的本机 `KsCam.dll` 2.1.2.3；启动窗口不会加载 DLL 或连接硬件。仓库目录名不代表应用从属关系；`pyproject.toml` 可独立构建显微镜安装包。

- 布局参考 NIS-Elements 的图像工作区：上方图像工具栏、左侧标记工具、中央图像、右侧设备与标记面板、底部测量表。
- **控制台 → Onway**：选择 TCP/IP（默认 `192.168.0.7:8888`），或选择串口、刷新/输入 COM 号和波特率，然后点击“连接 Onway”。读回 X、Y、Z、A、B、C 六轴位置后开放操作，无需运行运动控制程序或连接相机。两个 2.0 应用在同一 Windows 用户下互斥使用 Onway，换程序前先断开；旧版程序不参与互斥。
- **运动操作**：按住轴旁的“＋/−”按设定速度点动，松开停止该轴。“六轴回零”启动全部六轴回零，“停止运动”发送全轴停止。“记录位置”重新读取六轴并带时间追加到设置目录的 `onway-positions.csv`。显微镜不提供 Nexcope 控制，也不向运动程序的 `/position` 发布坐标。
- **连接相机 → 应用参数 → 实时预览 / 拍摄单帧**：先停止 NIS-Elements 对相机的占用，再点击连接。连接时只读取硬件参数，覆盖界面中的旧预设；编辑曝光、增益、采样后点击应用。采集使用手动曝光、自由运行。曝光按设备范围及步长量化，并显示硬件 / 帧读回值。支持全幅 2880×2048 和平均采样 1440×1024，内部将已实测的 BGR 转为 RGB8。平均采样 20 ms 实测约 30 fps；全幅 100 ms 约 10 fps。全幅短曝光此前出现帧号缺口，不保证无损连续录制。
- **停止与保存**：停止预览会冻结最后一帧；单帧拍摄会重新启动采集并取得新帧。空闲时“保存相机原图与参数”输出不带软件白平衡和标记的 RGB8 PNG，以及同名 JSON（尺寸、曝光、增益、帧号、时间、通道顺序等）。它不是传感器高位深 RAW。采集线程与界面隔离，预览只保留最新帧；“显示跳过”与设备帧号缺口分别统计。显微镜自身退出时停止采集、恢复连接前参数并释放设备；运动程序退出不影响相机。
- **更换采样 / 图像来源**：首次切换到相机或改变采样模式后，新图像清除标记并取消旧标定；已有标记时会先确认。同一来源及采样模式的实时帧保留缩放、标记和 RGB 路径。更换物镜等软件无法检测的光学变化仍需手动重新标定。
- **后端迁移**：界面与采集会话依赖 `microscope_camera.py` 的接口，当前实现为 `nikon_ks_camera.py`，ABI 封装在 `nikon_ks_bindings.py`。拿到官方 DSCam SDK 后增加对应后端即可复用界面、标记和保存流程；仍须重新验证结构体、参数单位、通道顺序、缓冲区寿命及关闭流程，不能仅改 DLL 文件名。默认路径为 `C:\Program Files\NIS-Elements D\KsCam.dll`，可用 `NIKON_KSCAM_DLL` 指向相同 SHA-256 的已验证运行库；其他版本会拒绝加载。实现和实测记录见 [CAMERA-INTEGRATION.md](CAMERA-INTEGRATION.md)。
- **打开图像**：支持 PNG、JPEG、BMP、TIFF；滚轮缩放、中键平移，也可使用平移工具、适合窗口及 1:1。当前只显示读入的单幅图像，不支持 ND2 或多页序列浏览。
- **长度**：拖出测量线；**轮廓**：依次点击顶点，Enter / 双击完成，Backspace 撤回当前顶点，Esc 取消；**矩形**：拖出四节点轮廓。
- 切换 **选择** 后可拖动标记、Shift＋拖动旋转。在右侧开启 **编辑节点** 后拖动节点，双击轮廓加点、右键删点（轮廓至少三个节点；测量线保留两个端点）。支持名称、颜色、线宽、线形、填充和角度修改。
- 测量始终使用原图像素坐标，与显示缩放无关；未标定显示 `px` / `px²`，设置实测的 `µm/px` 后显示物理长度 / 面积。轮廓结果为周长和面积；自交轮廓明确标为无法计算面积。更换图像会清除标记，有标记时先确认；标定比例作为显微镜配置保留，更换物镜、倍率或像素合并设置后请重新标定。
- **RGB 路径**：依次点击两个或更多节点，Enter / 双击完成开放折线。底部“RGB 路径强度”显示三通道曲线，也可选中已有长度测量线查看。沿原图路径每 1 px 采样并包含终点，使用双线性插值；横轴为累计路径距离，有标定时显示 µm。鼠标移到曲线上可读出距离及 RGB 值，各通道可单独显隐；节点编辑、移动和旋转后自动重新计算。路径超出图像时明确报错，不沿边界伪造采样。
- 曲线默认使用 **原始图像 · RGB8**，可切换 **白平衡后 · RGB8**；不采样叠加的标记。当前数值是图像转换到 8-bit RGB 后的 0–255 像素值，不是原生高位深传感器数据或绝对光功率。曲线 CSV 包含原图坐标、像素 / 实际距离、RGB、图像来源及应用的增益。
- **白平衡**位于“相机”选项卡，作用于本地图像和实时相机帧，不改变相机硬件白平衡。支持灰世界自动校正、点击中性灰 / 白区域（周围 9×9 像素，边缘处缩小范围）、手动 R/G/B 增益（0.01–16 倍）及恢复原图。每次从保留的原图计算，不累积校正；显示值四舍五入并截取到 0–255，导出的标记 PNG 使用校正后的图像。参考区域有零值通道或所需增益超限时明确报错并保留原设置。白平衡增益会记忆并沿用到新图像。
- **参数记忆**：显微镜自身正常退出时保存到 `%APPDATA%/microscope-workbench/settings.json`（无 APPDATA 时为 `~/.config/microscope-workbench/`）。新配置不存在时可读取旧 `%APPDATA%/motion-control/microscope-settings.json`，校验通过后仅写新目录，不修改旧文件。下次启动恢复曝光、相机增益、点动速度预设、完整精度的白平衡增益、标定比例、选项卡、RGB 数据来源与通道显隐、标记样式、节点编辑及窗口布局。恢复设置不会连接设备或启动采集 / 运动。保存失败会提示并阻止退出；损坏的设置不会被默认值覆盖。
- 可导出测量 CSV，以及原图尺寸的标记 PNG。PNG 是带标记的显示图，不是原始科学数据或可编辑工程。重复点击运动程序的入口会唤回尚在运行的显微镜，保留其当前会话；关闭显微镜则结束会话，再次启动不会恢复未保存的图像和标记，请先导出所需结果。

布局参考：[NIS-Elements D Advanced Layout](https://www.nisoftware.net/NikonSaleApplication/Help/Docs-D/eng_d/layout.advanced.html)。

## 异质结堆叠设计

从显微镜工作台的 **工具 → 异质结叠层设计…** 打开设计窗口，无需连接相机或运动设备。该入口已从 motion-control 移除；设计器归显微镜应用管理。
也可以通过 `python stack_designer.py` 单独启动。

1. **导入 flake 照片**：支持 PNG、JPEG、BMP、TIFF。也可点击“截图导入…”：程序窗口暂时隐藏，在任意显示器内拖动框选区域，松开鼠标自动导入，Esc 取消并恢复原窗口。截图按实际像素裁切，支持高 DPI 显示器；每次框选限于一个显示器。“粘贴图片”可直接导入系统截图工具等放入剪贴板的图片。导入后自动识别候选轮廓；一张照片中有多片时，用下拉框选择目标。
2. **修整轮廓**：照片预览和白板都支持拖动图形内部整体平移、按住 Shift 拖动图形内部整体旋转，也可在右侧输入精确旋转角度。勾选“编辑照片轮廓节点”或“编辑白板节点”后，可拖动节点、双击图形添加节点、右键节点删除（至少保留三个）；取消勾选可隐藏节点，方便整体操作。右侧还可修改名称、线形、线宽、轮廓颜色、填充颜色和不透明度。整体平移和旋转不会改写各节点的局部坐标。
3. **加入白板**：将当前轮廓复制到右侧白板当前视图中央，保留已调整的旋转角度和形状。继续导入其他照片不会清空白板；已有图形独立于照片中的后续编辑。
4. **组合堆叠**：拖动图形，调整旋转角度和尺寸倍率；在图层列表选择被遮挡的图形，通过上移、下移、置顶、置底调整顺序。列表最上方对应最前层。勾选“编辑白板节点”可进一步修整。
5. 两个视图均支持滚轮缩放、中键拖动平移和适合内容显示。
6. **导出透明 PNG**：点击白板上方的“导出透明 PNG…”。仅导出所有图层的轮廓线，保留颜色、线形、线宽、位置、旋转、尺寸和叠层顺序；背景及图形内部透明，不包含照片、节点或选中框。图片按所有轮廓范围自动裁剪并留出线宽边距，一个白板单位对应一个像素，不受视图缩放影响。导出不会改变白板中的填充或编辑状态。
7. **白板背景照片**：右侧“白板背景照片”可导入文件，或点击“当前照片”把左侧导入／截取／粘贴的图片放到白板底层。支持一个背景，始终位于所有多边形下方；默认锁定，勾选“移动背景”后可拖动，并可调整尺寸倍率、角度和不透明度。“移除”只移除背景，不影响多边形。透明 PNG 导出仍不包含背景。
8. **保存／打开工程**：点击白板上方的“保存工程…”保存为 `.hstack.json`，用“打开工程…”恢复后继续编辑。工程保存白板图层的节点、名称、位置、变换中心、旋转、缩放、线形、线宽、颜色和透明度；数组顺序表示从底层到顶层。如果白板有背景照片，会以内嵌 PNG 保存照片及其位置、旋转、缩放和透明度，原文件移动或删除后仍可恢复。不保存左侧用于识别的原图、照片路径或识别参数（除非主动将该照片设置为白板背景）。打开后自动显示全部图形，视图缩放、平移及选中状态不属于工程内容。v1.5.1 使用工程格式版本 2，并可打开旧版的版本 1 工程。
   打开工程会替换当前白板（有图层时先确认），不改动左侧照片。文件格式版本不支持、内容损坏或读取失败时，保留原白板；保存使用完整写入后替换，写入失败不会破坏旧工程文件。

**v1.6 手动选区**：自动识别效果不佳时，切换照片面板的“手动选区”。该标签下导入、截图或粘贴照片不会自动运行识别。

- **魔棒**：点击目标区域，根据点击位置的 RGB 颜色和容差选择像素。容差为每个颜色通道允许的差值（0–255），默认 20；“仅连续区域”默认开启，只选择与点击位置连通的区域，关闭则选择整张照片中符合颜色范围的像素。渐变颜色不会通过逐点累积容差扩散。
- **多边形套索**：沿目标边缘逐个点击顶点，Enter 或双击完成；Backspace 撤回最后一个顶点，Esc 取消未完成的套索。
- **画笔修补 / 矩形选区**：拖动涂抹或框选，配合减去模式可去掉误选区域；松开鼠标提交一次操作。画笔大小以原图像素为单位。
- 工具右侧支持“新选区、增加、减去、交集”；按住 Shift 临时增加、Alt 减去、Shift＋Alt 交集。蓝色半透明区域表示当前选区，边线标明边界。撤销／重做保存最近 20 步；照片视图内可用 Ctrl＋Z、Ctrl＋Y（或 Ctrl＋Shift＋Z）、Ctrl＋D 清除选区。“反选”交换选中和未选中区域。
- 点击 **从选区生成可编辑轮廓**，各个不相连的区域分别生成一个候选多边形，追加到照片轮廓列表，保留先前修整过的轮廓。随后自动切换到“查看 / 编辑轮廓”，可拖动节点、整体平移和旋转，再加入白板。需要继续选区时重新选择魔棒等工具；选区操作期间照片中的候选轮廓暂时隐藏。
- 手动选区使用**原图分辨率**，不经过自动识别的缩小或平滑处理。“轮廓误差”是多边形简化允许的原图像素偏差，默认 1 像素，越小保留的边角和节点越多；0 不做额外近似，直线上的冗余共线点仍会合并。节点数量由形状和误差决定，没有固定数量上限。
- 白板使用无孔多边形。选区含孔洞时会明确提示，不会默默填洞；可主动点击“填补孔洞”（可撤销），或用减去选区将孔洞连到外边缘。孤立单像素或零面积细线会提示清理或扩大；轮廓误差过大导致区域退化时会提示降低误差。
- 选区及其撤销历史在换照片时清空，不存入工程；生成并加入白板的多边形按原有工程格式完整保存。滚轮缩放、中键平移在选区工具中仍可使用。

**自动识别**使用照片边缘的基底颜色作为参考，适用于背景较均匀、边缘可见基底的照片。
支持调整色差阈值（0 为自动）、最小面积和多边形简化程度；重新识别会替换照片中的轮廓编辑，但不影响白板。
输出为外边界多边形，不包含内部孔洞；弱对比、阴影、污染或接触的 flakes 需要检查及手动修整，必要时先裁剪照片。
大照片会缩小到最长边 1600 像素进行识别，再还原到原图坐标。

初始图形大小按照片像素保持，不自动推断实际长度。不同显微倍率的照片需要统一比例后组合。
关闭设计窗口后，在同一显微镜会话中重新打开会保留本次设计；退出显微镜会关闭设计器，请先手动保存工程，下次打开 `.hstack.json` 即可恢复。PNG 只用于图像导出，不可替代可编辑的工程文件。
使用该功能需在运行和打包环境安装 `numpy`、`opencv-python`（已有 PyQt6）：

```bash
pip install numpy opencv-python
```

---

## Windows 独立 EXE 构建

在已安装 PyInstaller、PyQt6、pyserial、NumPy 和 OpenCV 的 Windows Python 环境下，从仓库根目录执行：

```powershell
python tools/build_windows.py motion       # release/v2.0/MotionController.exe
python tools/build_windows.py microscope   # release/v2.0/MicroscopeWorkbench.exe
```

也可用 `python tools/build_windows.py all` 依次构建两者。运动程序沿用原来的 PyInstaller 单文件、无控制台、应用图标及串口打包方式。两个 EXE 都可单独复制运行；如需从运动程序菜单打开显微镜，将二者放在同一目录。Nikon 相机仍需目标机上受支持的运行库。窗口、Windows 文件版本和 wheel 版本均来自 `app_version.py` 的 `APP_VERSION`，旧版本文件不会被覆盖。

## Current microscope image API

Run `microscope_app.py` (the legacy `microscope_workspace.py` command also works),
explicitly connect the camera, then start preview or capture a single frame.
The microscope owns its image service independently of the motion application:

```text
GET http://127.0.0.1:51236/api/v1/camera/image
```

- **200**: `Content-Type: image/png`, original RGB8 pixels at the current camera
  acquisition resolution, without software white balance, annotations or UI.
  `X-Image-Id` identifies this exact image; `Cache-Control: no-store` prevents
  HTTP caching. A repeated request can return the same image ID.
- **503**: `{"error":{"code":"image_unavailable"}}` when the workspace has no
  current camera image: before acquisition, while disconnected/reconnecting,
  or after opening a local image. Exiting the microscope stops its HTTP server.
- **500**: `{"error":{"code":"image_encoding_failed"}}` if PNG encoding fails.
- **POST** is rejected with **405**. GET does not connect hardware, change camera
  settings, or trigger a new exposure. Stopping preview retains its final frame
  while the camera remains connected. Check the embedded acquisition timestamp
  if freshness matters; this endpoint is a snapshot, not a video stream.

The PNG text field `microscope_frame` contains JSON belonging to the **same image**:

```json
{
  "schema_version": 1,
  "image_id": "unique-image-id",
  "source": "camera",
  "pixels": "original_rgb8_without_software_white_balance_or_annotations",
  "camera": {
    "device_serial": 123,
    "frame_no": 42,
    "captured_at": "2026-11-11T16:38:44.193394+08:00",
    "width": 2880,
    "height": 2048,
    "pixel_format": "RGB8",
    "exposure_ms": 20,
    "gain": 2
  }
}
```

This is an illustrative subset of the camera metadata; additional fields may
be present. `captured_at` is the backend's host-side frame retrieval time, not a
hardware exposure timestamp. There is currently **no synchronized stage position
or pixel-to-stage calibration** in this response. Reading `/position` separately
does not establish the position at exposure time.

Python download (no import from motion-control needed):

```python
from pathlib import Path
from urllib.request import urlopen

with urlopen("http://127.0.0.1:51236/api/v1/camera/image", timeout=10) as response:
    png = response.read()
    image_id = response.headers["X-Image-Id"]
Path("microscope.png").write_bytes(png)
```

PyDataVault can decode both pixels and metadata using its existing PySide6:

```python
import json
from PySide6.QtGui import QImage

image = QImage.fromData(png, "PNG")
metadata = json.loads(image.text("microscope_frame"))
assert metadata["image_id"] == image_id
```

Both microscope entry commands start this service; `--no-api` disables it and
`--api-port` selects another port. The old motion-owned address
`http://127.0.0.1:51235/api/v1/camera/image` remains a compatibility proxy to the
default port 51236, with the same PNG, metadata and image ID. It returns 503 when
the microscope is absent; `/position` still operates independently. New clients
should use the microscope directly. Requests already encoding an image can finish after a disconnect or a
new frame arrives; their pixels, metadata and image ID remain one consistent
snapshot. Subsequent requests use the new state.

### Extension boundaries

The two applications run in separate processes and exchange snapshots over HTTP.
They share the Onway driver and device lock without importing each other's GUI. The existing
`/position` response stays compatible. New routes use `/api/v1/`; clients should
ignore additional metadata fields, and incompatible changes should use a new
API version.

The HTTP layer receives an injected image snapshot provider. `CameraImageState`
owns the current image and caches its PNG on demand. Publication only replaces
the latest snapshot; encoding happens outside the GUI and publication lock,
with a separate lock limiting concurrent encoders. This is suitable for
on-demand image reads, not sustained full-frame video delivery.

Future movement/capture/scan commands need a separate command dispatcher into
the hardware-owning workers, plus task IDs, completion/error/cancellation status
and explicit coordinate units. Adding an HTTP write handler that directly calls
the drivers would not provide those guarantees. Future synchronized acquisition
should bind image, position and calibration to one acquisition ID rather than
combine unrelated latest-value requests.

## Third-party read-only XY endpoint

When `controller_gui.py` is running, it starts a localhost-only HTTP endpoint for
third-party programs that only need to read the current external XY coordinate.
The endpoint is read-only and does not expose any command path back into the
motion-control GUI or drivers.

### Endpoint

```text
GET http://127.0.0.1:51235/position
```

The position route supports only `GET /position`. Write attempts such as `POST /position`
return `405 Method Not Allowed`.

### Successful response

The response body is compact JSON with exactly `X` and `Y` fields, formatted to
four decimal places:

```http
200 OK
Content-Type: application/json; charset=utf-8
```

```json
{"X":12.3457, "Y":98.7654}
```

Axis mapping depends on the active connected controller:

| Connected controller | Returned `X` | Returned `Y` |
| --- | --- | --- |
| Onway | internal `A` axis, GUI index `3` | internal `B` axis, GUI index `4` |
| Nexcope | internal `X` axis | internal `Y` axis |

### Disconnected response

If no controller is connected, or the controller has been disconnected, the
endpoint returns an HTTP error instead of zero coordinates:

```http
503 Service Unavailable
Content-Type: application/json; charset=utf-8
```

```json
{"error":"not connected"}
```

Third-party programs should treat any non-`200` response as "position not
available" and must not substitute `0, 0` unless their own workflow explicitly
requires that fallback.

### Minimal client examples

Python:

```python
import json
from urllib.error import HTTPError
from urllib.request import urlopen

try:
    with urlopen("http://127.0.0.1:51235/position", timeout=1) as response:
        xy = json.loads(response.read().decode("utf-8"))
except HTTPError as exc:
    if exc.code == 503:
        xy = None
    else:
        raise

print(xy)
```

PowerShell:

```powershell
Invoke-RestMethod http://127.0.0.1:51235/position
```

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

在仓库根目录执行：

```bash
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
