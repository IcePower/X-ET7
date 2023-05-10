# X-ET 是一个融合了 ET, FairyGUI, luban, YooAsset 的缝合怪。

# 运行指南
## 开发模式
有3种开发模式可以选择。先在菜单里选择 ET - BuildTool 打开 ET.BuildEditor 界面，然后选择对应的模式。
1. CodeMode 选择 Client Server, PlayMode 选择 Editor Simulate Mode。菜单里选择 ET - ChangeDefine - Add ENABLE_CODES。
   这种模式比较简单，不用 Build DLL, 不用构建资源，只能在编辑器里使用。
2. CodeMode 选择 Client, PlayMode 选择 Editor Simulate Mode。菜单里选择 ET - ChangeDefine - Remove ENABLE_CODES。
   这种模式下，改了代码后需要点击 BuildModelAndHotfix, 不用构建资源，只能在编辑器里使用。
3. CodeMode 选择 Client, PlayMode 选择 Host Play Mode, 菜单里选择 ET - ChangeDefine - Remove ENABLE_CODES。
   这种模式下，改了代码后需要点击 BuildModelAndHotfix，然后在菜单里选择 YooAsset - AssetBundle Builder 构建资源。改了资源后也需要构建资源。
   这种模式下可以在编辑器里和真机上运行。

## 出补丁包的方法
1. 首次构建资源包。
   菜单里选择 YooAsset - AssetBundle Builder 打开资源包构建工具。
   第一次构建资源时 Copy Buildin File Option 里选择 Clear And Copy All，构建完成后会生成一个带日期的目录，比如2023-04-10-1408，将这个目录改名为v1.0,
   然后复制到WEB服务器上的 CDN/{平台名称}/ 目录下。平台名称可选 PC, Android, IPhone, WebGL。可以在 MonoResComponent.GetHostServerURL() 修改服务器地址和这些参数。
2. 出补丁包。
   如果代码有改动，需要在 BuildEditor 里 BuildModelAndHotfix。
   然后打开资源包构建工具界面，Copy Buildin File Option 里选择 None，构建完成后将资源全选，然后复制覆盖到 CDN/{平台名称}/v1.0 目录里。

电脑上的缓存目录是 X-ET7/Unity/Sandbox，如果需要重新下载补丁可以删除这个目录。

---
# HybridCLR
1. 下载补丁的流程在热更层里，下载完DLL后会调用Game.Close()，然后重新走初始化流程载入新的DLL。旧的DLL无法卸载。
2. Build的DLL增加了编号，这是因为HybridCLR不能重新载入同名的DLL，而且Unity编辑器里重复载入同名的DLL也会有问题。编号记录在 GlobalConfig 里。
3. 增加了一个热更新界面。热更新界面也可以热更，在下次启动后生效。

---
# FairyGUI
### 代码生成
使用 C# 实现了 FairyGUI 代码生成功能，和原 FairyGUI 编辑器代码生成相比有以下优点：
1. 扩展方便，不用再去写lua插件了。
2. 可以获取到跨包引用组件的类型，原FairyGUI编辑器生成的是GComponent。
3. 根据控制器页面生成了枚举，方便调用。

### PanelInspector 插件
做了一个插件: PanelInspector，选中一个组件后，可以选择它的界面类型，目前有两种：主界面和公共界面。

一个包里可以有多个主界面，公共界面是指在多个界面会共用的界面，参考 TestBPanel。

### 使用方式
1. FairyGUI 编辑器里只生成配置和图片，不生成代码。
2. 在ET - BuildTool里，点击"FUI代码生成"按钮，来生成代码。

### UIPackage动态管理
基于[FairyGUI-Dynamic](https://github.com/SunHowe/FairyGUI-Dynamic.git)模块，为FairyGUI静态跨包依赖的情况提供了支持，该模块内对UIPackage提供了统一的加载与卸载功能。  
静态跨包依赖指的是在FairyGUI编辑器中，PackageA内的组件1直接引用了PackageB内的组件2或图片3的情况，该模块内部会在加载PackageA时，自动加载其依赖的PackageB，并在PackageA被卸载时，检测PackageB是否还有其他依赖，若没有的话将自动卸载PackageB。

---
# Luban
1. 已将 ET 自带的配置表全部修改为 Luban 的格式，配置表加载流程也做了修改。
2. 修改了默认的代码生成模板，命名和使用方式更接近 ET 的习惯。在 ET - BuildTool里，点击"ExcelExporter" 来使用。

---
# YooAsset
1. 在 YooAssetHelper 里用 ETTask 对 YooAsset 的异步操作做了扩展。
2. 在 MonoResComponent 里初始化。
3. 在 ResComponentSystem 里调用了 YooAsset 加载的接口。对 Handle 做了缓存。
4. 在 BuildEditor 里切换 PlayMode。

---
# Reference
1. ET: 更新至[597866c](https://github.com/egametang/ET/commit/597866cb53b487186d216f08fc8bf3e9f353c0c7)
2. YooAsset: 更新至[1.4.12](https://github.com/tuyoogame/YooAsset/commit/e2788839586876da483729377580aa5cb8d06408)
3. Luban: 更新至[8e68ab0](https://github.com/focus-creative-games/luban)
4. ET-EUI: https://github.com/zzjfengqing/ET-EUI
5. FairyGUI: https://www.fairygui.com/
6. FairyGUI-Dynamic: https://github.com/SunHowe/FairyGUI-Dynamic
