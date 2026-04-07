#!/usr/bin/env python3
"""
MCC 运动控制卡 - PyQt6 最简控制界面

功能:
- TCP/串口连接
- 6轴手动控制 (点动/回零)
- 位置/速度实时显示
- 急停/停止
- IO状态显示

生产环境版本 - 必须依赖 mcc_driver.py
"""

import sys
from typing import Optional, List
from PyQt6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QGridLayout, QGroupBox, QPushButton, QLabel, QLineEdit,
    QComboBox, QCheckBox, QTextEdit, QTabWidget,
    QStatusBar, QDoubleSpinBox, QFrame, QRadioButton, QButtonGroup,
    QMessageBox, QMenuBar
)
from PyQt6.QtCore import Qt, QTimer
from PyQt6.QtGui import QFont, QColor, QPalette, QPainter, QPen, QBrush

# 生产环境：必须导入真正的驱动
from mcc_driver import MCCDriver, SERIAL_AVAILABLE


class PositionPlot(QWidget):
    """位置绘图板 - 显示6轴当前位置柱状图"""
    
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setMinimumSize(200, 150)
        self.positions = [0.0] * 6  # X, Y, Z, A, B, C
        self.axis_names = ["X", "Y", "Z", "A", "B", "C"]
        self.colors = [
            QColor(42, 130, 218),  # X - 蓝
            QColor(50, 205, 50),   # Y - 绿
            QColor(255, 165, 0),   # Z - 橙
            QColor(255, 69, 0),    # A - 红
            QColor(148, 0, 211),   # B - 紫
            QColor(0, 206, 209),   # C - 青
        ]
        self.max_range = 100.0  # 默认范围，会动态调整
        
    def set_positions(self, positions: List[float]):
        """更新位置数据"""
        self.positions = positions
        # 动态调整范围
        max_pos = max(abs(p) for p in positions) if positions else 0
        self.max_range = max(max_pos * 1.2, 100.0)  # 至少100，或者比最大值大20%
        self.update()  # 触发重绘
        
    def paintEvent(self, event):
        """绘制柱状图"""
        painter = QPainter(self)
        painter.setRenderHint(QPainter.RenderHint.Antialiasing)
        
        width = self.width()
        height = self.height()
        
        # 背景
        painter.fillRect(0, 0, width, height, QColor(30, 30, 30))
        
        # 绘制区域边距
        margin_left = 30
        margin_right = 10
        margin_top = 20
        margin_bottom = 25
        
        plot_width = width - margin_left - margin_right
        plot_height = height - margin_top - margin_bottom
        
        if plot_width <= 0 or plot_height <= 0:
            return
        
        bar_width = plot_width / 6 - 8  # 每个柱子宽度，留间隙
        bar_gap = 8
        
        # 绘制零线
        painter.setPen(QPen(QColor(100, 100, 100), 1))
        zero_y = margin_top + plot_height / 2
        painter.drawLine(margin_left, int(zero_y), width - margin_right, int(zero_y))
        
        # 绘制刻度线
        painter.setPen(QPen(QColor(80, 80, 80), 1))
        for i in range(5):
            y = margin_top + plot_height * i / 4
            painter.drawLine(margin_left, int(y), width - margin_right, int(y))
        
        # 绘制刻度值
        painter.setPen(QPen(QColor(150, 150, 150), 1))
        painter.setFont(QFont("Arial", 8))
        for i, val in enumerate([self.max_range, self.max_range/2, 0, -self.max_range/2, -self.max_range]):
            y = margin_top + plot_height * i / 4
            painter.drawText(2, int(y + 4), f"{val:.0f}")
        
        # 绘制柱状图
        for i, pos in enumerate(self.positions):
            x = margin_left + i * (bar_width + bar_gap) + bar_gap / 2
            
            # 柱子高度（正值向上，负值向下）
            bar_height = abs(pos) / self.max_range * (plot_height / 2)
            bar_height = min(bar_height, plot_height / 2 - 2)  # 不超过半区
            
            color = self.colors[i]
            painter.setBrush(QBrush(color))
            painter.setPen(QPen(color.darker(120), 1))
            
            if pos >= 0:
                # 正值：从零线向上
                painter.drawRect(int(x), int(zero_y - bar_height), int(bar_width), int(bar_height))
            else:
                # 负值：从零线向下
                painter.drawRect(int(x), int(zero_y), int(bar_width), int(bar_height))
            
            # 绘制轴名
            painter.setPen(QPen(QColor(200, 200, 200), 1))
            painter.setFont(QFont("Arial", 10, QFont.Weight.Bold))
            painter.drawText(int(x + bar_width/2 - 5), int(height - 8), self.axis_names[i])
            
            # 绘制数值
            painter.setFont(QFont("Arial", 8))
            val_y = zero_y - bar_height - 12 if pos >= 0 else zero_y + bar_height + 12
            painter.drawText(int(x + bar_width/2 - 15), int(val_y), f"{pos:.1f}")


class MCCMainWindow(QMainWindow):
    """MCC 主窗口"""
    
    def __init__(self):
        super().__init__()
        
        # 日志回调函数 (在 info_text 创建后使用)
        def log_callback(level: str, msg: str):
            # Verbose 模式显示所有日志，非 verbose 只显示 INFO/WARN/ERROR
            verbose = self.verbose_check.isChecked() if hasattr(self, 'verbose_check') else True
            if verbose or level in ("INFO", "WARN", "ERROR"):
                self.log_info(f"[{level}] {msg}")
        
        self.driver = MCCDriver(log_callback=log_callback)
        self.jog_mode = False
        self.speed_override = 0.0
        
        # 位置缓存
        self.axis_pos = [0.0] * 6
        self.axis_spd = [0.0] * 6
        
        # 初始化 UI
        self.init_ui()
        
        # 定时器 - 状态更新
        self.update_timer = QTimer()
        self.update_timer.timeout.connect(self.update_status)
        self.update_timer.setInterval(200)  # 200ms
    
    def init_ui(self):
        """初始化界面"""
        self.setWindowTitle("MCC 运动控制 - Python版")
        self.setMinimumSize(1200, 800)
        
        # 创建中央部件
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        main_layout = QHBoxLayout(central_widget)
        
        # ==================== 左侧面板 ====================
        left_panel = QWidget()
        left_layout = QVBoxLayout(left_panel)
        
        # --- 通信配置 ---
        comm_group = self.create_comm_group()
        left_layout.addWidget(comm_group)
        
        # --- 位置显示 ---
        pos_group = self.create_position_group()
        left_layout.addWidget(pos_group)
        
        # --- 状态选项 ---
        status_group = self.create_status_options()
        left_layout.addWidget(status_group)
        
        # --- 信息输出 ---
        info_group = QGroupBox("信息输出")
        info_layout = QVBoxLayout(info_group)
        
        # Verbose 开关
        self.verbose_check = QCheckBox("详细日志 (Verbose)")
        self.verbose_check.setChecked(True)
        info_layout.addWidget(self.verbose_check)
        
        self.info_text = QTextEdit()
        self.info_text.setReadOnly(True)
        self.info_text.setMaximumHeight(200)  # 增加高度
        self.info_text.setStyleSheet("font-family: monospace; font-size: 10px;")
        info_layout.addWidget(self.info_text)
        
        # 清空日志按钮
        clear_btn = QPushButton("清空日志")
        clear_btn.clicked.connect(lambda: self.info_text.clear())
        info_layout.addWidget(clear_btn)
        
        left_layout.addWidget(info_group)
        
        main_layout.addWidget(left_panel, stretch=1)
        
        # ==================== 中间面板 ====================
        middle_panel = QWidget()
        middle_layout = QVBoxLayout(middle_panel)
        
        # --- 手动控制 ---
        manual_group = self.create_manual_control()
        middle_layout.addWidget(manual_group)
        
        # --- 回零控制 ---
        home_group = self.create_home_control()
        middle_layout.addWidget(home_group)
        
        main_layout.addWidget(middle_panel, stretch=1)
        
        # ==================== 右侧面板 ====================
        right_panel = QWidget()
        right_layout = QVBoxLayout(right_panel)
        
        # --- 位置绘图板 ---
        self.position_plot = PositionPlot()
        plot_group = QGroupBox("位置图示")
        plot_layout = QVBoxLayout(plot_group)
        plot_layout.addWidget(self.position_plot)
        right_layout.addWidget(plot_group, stretch=2)
        
        # --- IO 状态 ---
        io_group = self.create_io_status()
        right_layout.addWidget(io_group, stretch=1)
        
        main_layout.addWidget(right_panel, stretch=1)
        
        # ==================== 状态栏 ====================
        self.status_bar = QStatusBar()
        self.setStatusBar(self.status_bar)
        self.status_label = QLabel("未连接")
        self.run_label = QLabel("停止")
        self.status_bar.addPermanentWidget(QLabel("连接:"))
        self.status_bar.addPermanentWidget(self.status_label)
        self.status_bar.addPermanentWidget(QLabel(" | 运行:"))
        self.status_bar.addPermanentWidget(self.run_label)
        
        # ==================== 菜单栏 ====================
        self.create_menu_bar()
    
    def create_comm_group(self) -> QGroupBox:
        """创建通信配置组"""
        group = QGroupBox("通信配置")
        layout = QVBoxLayout(group)
        
        # 选项卡
        tab = QTabWidget()
        
        # TCP 页面
        tcp_page = QWidget()
        tcp_layout = QGridLayout(tcp_page)
        
        tcp_layout.addWidget(QLabel("IP地址:"), 0, 0)
        self.ip_edit = QLineEdit("192.168.0.7")
        tcp_layout.addWidget(self.ip_edit, 0, 1)
        
        tcp_layout.addWidget(QLabel("端口:"), 1, 0)
        self.port_edit = QLineEdit("8888")
        tcp_layout.addWidget(self.port_edit, 1, 1)
        
        self.btn_connect = QPushButton("连接")
        self.btn_connect.clicked.connect(self.on_connect)
        tcp_layout.addWidget(self.btn_connect, 2, 0)
        
        self.btn_disconnect = QPushButton("断开")
        self.btn_disconnect.clicked.connect(self.on_disconnect)
        self.btn_disconnect.setEnabled(False)
        tcp_layout.addWidget(self.btn_disconnect, 2, 1)
        
        tab.addTab(tcp_page, "TCP/IP")
        
        # 串口页面
        serial_page = QWidget()
        serial_layout = QGridLayout(serial_page)
        
        serial_layout.addWidget(QLabel("串口:"), 0, 0)
        self.serial_combo = QComboBox()
        self.serial_combo.addItems(["COM1", "COM2", "COM3", "/dev/ttyUSB0", "/dev/ttyUSB1"])
        serial_layout.addWidget(self.serial_combo, 0, 1)
        
        btn_refresh_ports = QPushButton("刷新")
        btn_refresh_ports.setMaximumWidth(60)
        btn_refresh_ports.clicked.connect(self.refresh_serial_ports)
        serial_layout.addWidget(btn_refresh_ports, 0, 2)
        
        serial_layout.addWidget(QLabel("波特率:"), 1, 0)
        self.baud_combo = QComboBox()
        self.baud_combo.addItems(["9600", "19200", "38400", "57600", "115200"])
        self.baud_combo.setCurrentText("115200")
        serial_layout.addWidget(self.baud_combo, 1, 1)
        
        btn_serial_open = QPushButton("打开串口")
        btn_serial_open.clicked.connect(self.on_serial_connect)
        serial_layout.addWidget(btn_serial_open, 2, 0)
        
        btn_serial_close = QPushButton("关闭串口")
        btn_serial_close.clicked.connect(self.on_serial_disconnect)
        serial_layout.addWidget(btn_serial_close, 2, 1)
        
        tab.addTab(serial_page, "RS232")
        
        layout.addWidget(tab)
        
        # 连接状态指示
        status_layout = QHBoxLayout()
        status_layout.addWidget(QLabel("连接状态:"))
        self.conn_indicator = QFrame()
        self.conn_indicator.setFixedSize(20, 20)
        self.conn_indicator.setStyleSheet("background-color: gray; border-radius: 10px;")
        status_layout.addWidget(self.conn_indicator)
        status_layout.addStretch()
        layout.addLayout(status_layout)
        
        return group
    
    def create_position_group(self) -> QGroupBox:
        """创建位置显示组"""
        group = QGroupBox("轴位置/速度")
        layout = QGridLayout(group)
        
        # 轴名
        axes = ["X", "Y", "Z", "A", "B", "C"]
        
        # 标题行
        layout.addWidget(QLabel("轴"), 0, 0)
        layout.addWidget(QLabel("位置"), 0, 1)
        layout.addWidget(QLabel("速度"), 0, 2)
        layout.addWidget(QLabel("单位"), 0, 3)
        
        # 创建位置和速度标签
        self.pos_labels = []
        self.spd_labels = []
        self.unit_labels = []
        self.axis_radios = []
        
        self.axis_button_group = QButtonGroup()
        
        for i, axis in enumerate(axes):
            row = i + 1
            
            # 单选按钮
            radio = QRadioButton(axis)
            if i == 0:
                radio.setChecked(True)
            self.axis_button_group.addButton(radio, i)
            layout.addWidget(radio, row, 0)
            self.axis_radios.append(radio)
            
            # 位置标签
            pos_label = QLabel("0.000")
            pos_label.setMinimumWidth(80)
            pos_label.setStyleSheet("background-color: #2d2d2d; color: #00ff00; padding: 5px; font-family: monospace;")
            layout.addWidget(pos_label, row, 1)
            self.pos_labels.append(pos_label)
            
            # 速度标签
            spd_label = QLabel("0.0")
            spd_label.setMinimumWidth(60)
            spd_label.setStyleSheet("background-color: #2d2d2d; color: #ffff00; padding: 5px; font-family: monospace;")
            layout.addWidget(spd_label, row, 2)
            self.spd_labels.append(spd_label)
            
            # 单位
            unit_label = QLabel("mm")
            layout.addWidget(unit_label, row, 3)
            self.unit_labels.append(unit_label)
        
        return group
    
    def create_status_options(self) -> QGroupBox:
        """创建状态选项组"""
        group = QGroupBox("显示选项")
        layout = QVBoxLayout(group)
        
        self.cb_update_pos = QCheckBox("更新位置")
        self.cb_update_pos.setChecked(True)
        layout.addWidget(self.cb_update_pos)
        
        self.cb_update_spd = QCheckBox("更新速度")
        self.cb_update_spd.setChecked(True)
        layout.addWidget(self.cb_update_spd)
        
        self.cb_update_io = QCheckBox("更新IO状态")
        self.cb_update_io.setChecked(True)
        layout.addWidget(self.cb_update_io)
        
        return group
    
    def create_manual_control(self) -> QGroupBox:
        """创建手动控制组"""
        group = QGroupBox("手动控制")
        layout = QGridLayout(group)
        
        axes = ["X", "Y", "Z", "A", "B", "C"]
        
        # 创建正负方向按钮
        self.btn_neg = []
        self.btn_pos = []
        
        for i, axis in enumerate(axes):
            # 负方向按钮
            btn_neg = QPushButton(f"{axis}-")
            btn_neg.setMinimumSize(50, 40)
            btn_neg.pressed.connect(lambda a=i: self.on_axis_neg(a))
            btn_neg.released.connect(lambda a=i: self.on_axis_stop(a))
            layout.addWidget(btn_neg, i, 0)
            self.btn_neg.append(btn_neg)

            # 正方向按钮
            btn_pos = QPushButton(f"{axis}+")
            btn_pos.setMinimumSize(50, 40)
            btn_pos.pressed.connect(lambda a=i: self.on_axis_pos(a))
            btn_pos.released.connect(lambda a=i: self.on_axis_stop(a))
            layout.addWidget(btn_pos, i, 1)
            self.btn_pos.append(btn_pos)
        
        # Jog 模式
        self.cb_jog = QCheckBox("Jog模式")
        self.cb_jog.stateChanged.connect(self.on_jog_mode_changed)
        layout.addWidget(self.cb_jog, 6, 0, 1, 2)
        
        # 速度控制
        layout.addWidget(QLabel("手动速度:"), 7, 0)
        self.manual_spd = QDoubleSpinBox()
        self.manual_spd.setRange(0, 1000)
        self.manual_spd.setValue(5.0)
        self.manual_spd.setDecimals(1)
        layout.addWidget(self.manual_spd, 7, 1)
        
        # 急停和停止
        btn_em_stop = QPushButton("急停")
        btn_em_stop.setStyleSheet("background-color: red; color: white; font-weight: bold;")
        btn_em_stop.setMinimumSize(80, 40)
        btn_em_stop.clicked.connect(self.on_emergency_stop)
        layout.addWidget(btn_em_stop, 8, 0)
        
        btn_para_stop = QPushButton("停止")
        btn_para_stop.setStyleSheet("background-color: orange; color: white;")
        btn_para_stop.setMinimumSize(80, 40)
        btn_para_stop.clicked.connect(self.on_para_stop)
        layout.addWidget(btn_para_stop, 8, 1)
        
        return group
    
    def create_home_control(self) -> QGroupBox:
        """创建回零控制组"""
        group = QGroupBox("回零控制")
        layout = QGridLayout(group)
        
        axes = ["X", "Y", "Z", "A", "B", "C"]
        
        self.btn_home = []
        for i, axis in enumerate(axes):
            btn = QPushButton(f"{axis}回零")
            btn.clicked.connect(lambda checked, a=i: self.on_home(a))
            layout.addWidget(btn, i // 3, i % 3)
            self.btn_home.append(btn)
        
        btn_all_home = QPushButton("全部回零")
        btn_all_home.clicked.connect(lambda: self.on_home(0xFF))
        layout.addWidget(btn_all_home, 2, 0, 1, 2)
        
        btn_cancel_home = QPushButton("取消回零")
        btn_cancel_home.clicked.connect(lambda: self.on_cancel_home(0xFF))
        layout.addWidget(btn_cancel_home, 2, 2)
        
        return group
    
    def create_io_status(self) -> QGroupBox:
        """创建IO状态组"""
        group = QGroupBox("IO状态")
        layout = QGridLayout(group)
        
        # 输入状态
        layout.addWidget(QLabel("输入:"), 0, 0)
        self.input_label = QLabel("0x0000")
        self.input_label.setStyleSheet("font-family: monospace;")
        layout.addWidget(self.input_label, 0, 1)
        
        # 输出状态
        layout.addWidget(QLabel("输出:"), 1, 0)
        self.output_label = QLabel("0x0000")
        self.output_label.setStyleSheet("font-family: monospace;")
        layout.addWidget(self.output_label, 1, 1)
        
        # IO 指示灯
        io_grid = QGridLayout()
        self.io_indicators = []
        for i in range(16):
            indicator = QFrame()
            indicator.setFixedSize(15, 15)
            indicator.setStyleSheet("background-color: #333; border: 1px solid #555;")
            io_grid.addWidget(indicator, i // 8, i % 8)
            self.io_indicators.append(indicator)
        
        layout.addLayout(io_grid, 2, 0, 1, 2)
        
        return group
    
    def create_menu_bar(self):
        """创建菜单栏"""
        menubar = self.menuBar()
        
        # 文件菜单
        file_menu = menubar.addMenu("文件(&F)")
        
        exit_action = file_menu.addAction("退出(&X)")
        exit_action.triggered.connect(self.close)
        
        # 参数菜单
        param_menu = menubar.addMenu("参数(&P)")
        
        read_action = param_menu.addAction("读取参数(&R)")
        read_action.triggered.connect(self.on_read_params)
        
        write_action = param_menu.addAction("写入参数(&W)")
        write_action.triggered.connect(self.on_write_params)
        
        param_menu.addSeparator()
        
        save_rom_action = param_menu.addAction("保存到板卡(&S)")
        save_rom_action.triggered.connect(self.on_save_to_rom)
        
        # 帮助菜单
        help_menu = menubar.addMenu("帮助(&H)")
        
        about_action = help_menu.addAction("关于(&A)")
        about_action.triggered.connect(self.on_about)
    
    # ==================== 事件处理 ====================
    
    def log_info(self, msg: str):
        """记录信息"""
        import datetime
        timestamp = datetime.datetime.now().strftime("%H:%M:%S")
        self.info_text.append(f"[{timestamp}] {msg}")
    
    def on_connect(self):
        """TCP连接"""
        ip = self.ip_edit.text()
        port = int(self.port_edit.text())
        
        if self.driver.connect(ip, port):
            self.log_info(f"TCP连接成功: {ip}:{port}")
            self.btn_connect.setEnabled(False)
            self.btn_disconnect.setEnabled(True)
            self.conn_indicator.setStyleSheet("background-color: lime; border-radius: 10px;")
            self.status_label.setText("已连接")
            self.update_timer.start()
        else:
            self.log_info(f"TCP连接失败: {ip}:{port}")
            QMessageBox.warning(self, "错误", f"无法连接到 {ip}:{port}")
    
    def on_disconnect(self):
        """断开连接"""
        self.update_timer.stop()
        self.driver.disconnect()
        self.btn_connect.setEnabled(True)
        self.btn_disconnect.setEnabled(False)
        self.conn_indicator.setStyleSheet("background-color: gray; border-radius: 10px;")
        self.status_label.setText("未连接")
        self.log_info("已断开连接")
    
    def on_serial_connect(self):
        """串口连接"""
        port = self.serial_combo.currentText()
        baudrate = int(self.baud_combo.currentText())
        
        try:
            if self.driver.connect_serial(port, baudrate):
                self.log_info(f"串口连接成功: {port} @ {baudrate}")
                self.conn_indicator.setStyleSheet("background-color: lime; border-radius: 10px;")
                self.status_label.setText(f"已连接 ({port})")
                self.update_timer.start()
            else:
                self.log_info(f"串口连接失败: {port}")
                QMessageBox.warning(self, "错误", f"无法打开串口 {port}")
        except ImportError as e:
            self.log_info("pyserial 未安装")
            QMessageBox.warning(self, "错误", "需要安装 pyserial: pip install pyserial")
        except Exception as e:
            self.log_info(f"串口连接异常: {e}")
            QMessageBox.warning(self, "错误", f"串口连接失败: {e}")
    
    def on_serial_disconnect(self):
        """串口断开"""
        self.update_timer.stop()
        self.driver.disconnect()
        self.conn_indicator.setStyleSheet("background-color: gray; border-radius: 10px;")
        self.status_label.setText("未连接")
        self.log_info("串口已断开")
    
    def refresh_serial_ports(self):
        """刷新串口列表"""
        ports = MCCDriver.list_serial_ports()
        if ports:
            self.serial_combo.clear()
            self.serial_combo.addItems(ports)
            self.log_info(f"发现串口: {', '.join(ports)}")
        else:
            self.log_info("未发现可用串口")
    
    def on_jog_mode_changed(self, state):
        """Jog模式改变"""
        self.jog_mode = state == Qt.CheckState.Checked.value
    
    def on_axis_neg(self, axis: int):
        """轴负方向"""
        if not self.driver.connected:
            return
        
        spd = -self.manual_spd.value()
        if self.jog_mode:
            # Jog模式: 速度运动
            if self.driver.move_at_speed(axis, spd, 50.0):
                self.log_info(f"轴{axis} 负向速度运动: {spd:.1f}")
        else:
            # 点动模式: 相对运动一小段距离
            dist = -1.0  # 1mm
            if self.driver.rel_move(axis, dist, abs(spd), 100.0):
                self.log_info(f"轴{axis} 负向点动: {dist}mm")
    
    def on_axis_pos(self, axis: int):
        """轴正方向"""
        if not self.driver.connected:
            return
        
        spd = self.manual_spd.value()
        if self.jog_mode:
            # Jog模式: 速度运动
            if self.driver.move_at_speed(axis, spd, 50.0):
                self.log_info(f"轴{axis} 正向速度运动: {spd:.1f}")
        else:
            # 点动模式: 相对运动一小段距离
            dist = 1.0  # 1mm
            if self.driver.rel_move(axis, dist, spd, 100.0):
                self.log_info(f"轴{axis} 正向点动: {dist}mm")
    
    def on_axis_stop(self, axis: int):
        """轴停止"""
        if not self.driver.connected:
            return
        
        if self.jog_mode:
            self.driver.para_stop(axis)
    
    def on_home(self, axis: int):
        """回零"""
        if not self.driver.connected:
            self.log_info("未连接，无法回零")
            return
        
        if axis == 0xFF:
            # 所有轴回零
            if self.driver.home_all():
                self.log_info("全部轴回零启动")
            else:
                self.log_info("部分轴回零启动失败")
        else:
            if self.driver.home(axis):
                self.log_info(f"轴{axis} 回零启动")
            else:
                self.log_info(f"轴{axis} 回零启动失败")
    
    def on_cancel_home(self, axis: int):
        """取消回零"""
        if not self.driver.connected:
            self.log_info("未连接，无法取消回零")
            return
        
        if axis == 0xFF:
            if self.driver.cancel_home_all():
                self.log_info("取消所有轴回零")
            else:
                self.log_info("部分轴取消回零失败")
        else:
            if self.driver.cancel_home(axis):
                self.log_info(f"取消轴{axis}回零")
            else:
                self.log_info(f"取消轴{axis}回零失败")
    
    def on_emergency_stop(self):
        """急停"""
        if self.driver.connected:
            if self.driver.emergency_stop_all():
                self.log_info("急停! 所有轴已停止")
            else:
                self.log_info("急停! 部分轴停止失败")
        else:
            self.log_info("未连接")
    
    def on_para_stop(self):
        """减速停止"""
        if self.driver.connected:
            if self.driver.para_stop_all():
                self.log_info("所有轴减速停止")
            else:
                self.log_info("部分轴停止失败")
        else:
            self.log_info("未连接")
    
    def on_read_params(self):
        """读取参数"""
        if self.driver.connected:
            self.log_info("读取参数...")
            # TODO: 实现参数读取
        else:
            self.log_info("未连接，无法读取参数")
    
    def on_write_params(self):
        """写入参数"""
        if self.driver.connected:
            self.log_info("写入参数...")
        else:
            self.log_info("未连接，无法写入参数")
    
    def on_save_to_rom(self):
        """保存到ROM"""
        if self.driver.connected:
            self.driver.save_para_to_rom()
            self.log_info("参数已保存到ROM")
        else:
            self.log_info("未连接，无法保存参数")
    
    def on_about(self):
        """关于"""
        QMessageBox.about(self, "关于", 
            "MCC 运动控制卡 Python 控制界面\n\n"
            "基于 MCC6DLL.dll 反编译代码实现\n"
            "支持 TCP/IP 和 RS232 通信\n\n"
            "版本: 1.0.0"
        )
    
    def update_status(self):
        """更新状态"""
        if not self.driver.connected:
            return
        
        # 更新位置
        if self.cb_update_pos.isChecked():
            pos = self.driver.get_axis_pos(0xFF)
            if pos:
                for i in range(6):
                    self.pos_labels[i].setText(f"{pos[i]:.3f}")
                # 更新绘图板
                self.position_plot.set_positions(pos)
        
        # 更新速度
        if self.cb_update_spd.isChecked():
            spd = self.driver.get_axis_spd(0xFF)
            if spd:
                for i in range(6):
                    self.spd_labels[i].setText(f"{spd[i]:.1f}")
        
        # 更新IO状态
        if self.cb_update_io.isChecked():
            inp = self.driver.get_input(0)
            if inp is not None:
                self.input_label.setText(f"0x{inp:04X}")
                for i in range(16):
                    bit = bool(inp & (1 << i))
                    self.io_indicators[i].setStyleSheet(
                        "background-color: lime; border: 1px solid #555;" if bit
                        else "background-color: #333; border: 1px solid #555;"
                    )
            out = self.driver.get_output(0)
            if out is not None:
                self.output_label.setText(f"0x{out:04X}")

        # 更新运行状态
        state = self.driver.get_run_state()
        if state is not None:
            if state == 0:
                self.run_label.setText("停止")
                self.run_label.setStyleSheet("")
            else:
                self.run_label.setText("运行中")
                self.run_label.setStyleSheet("color: green;")
    
    def closeEvent(self, event):
        """窗口关闭事件"""
        self.update_timer.stop()
        if self.driver.connected:
            self.driver.disconnect()
        event.accept()


def main():
    app = QApplication(sys.argv)
    
    # 设置样式
    app.setStyle("Fusion")
    
    # 深色主题
    palette = QPalette()
    palette.setColor(QPalette.ColorRole.Window, QColor(53, 53, 53))
    palette.setColor(QPalette.ColorRole.WindowText, QColor(255, 255, 255))
    palette.setColor(QPalette.ColorRole.Base, QColor(25, 25, 25))
    palette.setColor(QPalette.ColorRole.AlternateBase, QColor(53, 53, 53))
    palette.setColor(QPalette.ColorRole.ToolTipBase, QColor(255, 255, 255))
    palette.setColor(QPalette.ColorRole.ToolTipText, QColor(255, 255, 255))
    palette.setColor(QPalette.ColorRole.Text, QColor(255, 255, 255))
    palette.setColor(QPalette.ColorRole.Button, QColor(53, 53, 53))
    palette.setColor(QPalette.ColorRole.ButtonText, QColor(255, 255, 255))
    palette.setColor(QPalette.ColorRole.BrightText, QColor(255, 0, 0))
    palette.setColor(QPalette.ColorRole.Link, QColor(42, 130, 218))
    palette.setColor(QPalette.ColorRole.Highlight, QColor(42, 130, 218))
    palette.setColor(QPalette.ColorRole.HighlightedText, QColor(0, 0, 0))
    app.setPalette(palette)
    
    window = MCCMainWindow()
    window.show()
    
    sys.exit(app.exec())


if __name__ == "__main__":
    main()