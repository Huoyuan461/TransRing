# 转环 TransRing

**不用打开软件的本地文件格式转换器。**

按住 `Ctrl` 拖动文件，鼠标位置会弹出格式圆环，把文件拖到目标格式上就转完了。244 种格式：图片、PDF、Word、Excel、PPT、视频、音频、电子书、压缩包、CAD、字体。文件全在本地处理，不上传。

## 安装

从 [Releases](https://github.com/Huoyuan461/TransRing/releases) 下载：

- **Windows 10/11**：运行 `TransRing-<版本>-win-Setup.exe`，或解压 `win-Portable.zip` 直接运行 `TransRing.exe`。安装程序可添加右键菜单、开机启动、`transring` 命令行和可选转换引擎。
- **macOS 14+**（预览版）、**Linux**（预览版）：见原项目说明。

## 购买 Buy

**$29 一次性买断**（终身授权，含 v1.x 免费更新）— Pay with PayPal:

👉 **https://paypal.me/huoyuan453/29**

付款时请在备注里留下你的邮箱，下载与授权信息将发送到该邮箱。One-time payment via PayPal — please leave your email in the payment note for delivery.

## 特性

- **鼠标位置的圆环**：按住按键拖文件，只显示适合这些文件的格式
- **244 种格式**，无需安装 Office 等其他软件
- **全本地处理**：不上传文件，离线可用
- **批量转换**、监控文件夹自动转换、右键菜单、命令行
- 浅色/深色主题，简体中文默认

## 命令行

```bash
transring convert photo.heic --to jpg
transring convert *.png --to webp --quality 80 -o converted
```

## 构建

需要 .NET 10 SDK（Windows）：

```bash
git clone https://github.com/Huoyuan461/TransRing.git
cd TransRing
pwsh build/fetch-engines.ps1 -Only rhwp,pandoc
dotnet run --project src/TransRing.App
```

打包安装程序：`pwsh build/build-installer.ps1`（需 Inno Setup 7），再用代码签名证书签名。

## 许可证

MIT。转环基于开源项目 [Filee](https://github.com/KnifeLemon/Filee)（© 2026 Myeonggu Kim，MIT）的二次开发品牌版；随附的第三方组件许可证见 [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md)。转换引擎（FFmpeg、Ghostscript 等）为独立进程、由用户按需从官网下载，不随安装包分发。

<!-- sponsors: src/TransRing.App/Assets/Sponsors/sponsors.json -->
