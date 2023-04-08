# X-ET 是一个融合了 ET, FairyGUI, luban, YooAsset 的缝合怪。

# HybridCLR
1. 下载补丁的流程在热更层里，下载完DLL后会调用Game.Close()，然后重新走初始化流程载入新的DLL。旧的DLL无法卸载。
2. Build的DLL增加了编号，这是因为HybridCLR不能重新载入同名的DLL，而且Unity编辑器里重复载入同名的DLL也会有问题。编号记录在 GlobalConfig 里。
3. 增加了一个热更新界面。热更新界面也可以热更，在下次启动后生效。

# FairyGUI
### 代码生成
使用 C# 实现了 FairyGUI 代码生成功能，和原 FairyGUI 编辑器代码生成相比有以下优点：
1. 扩展方便，不用再去写lua插件了。
2. 可以获取到跨包引用组件的类型，原FairyGUI编辑器生成的是GComponent。
3. 根据控制器页面生成了枚举，方便调用。  

### 命名规则  
一个界面对应一个包, 界面的主组件名必须是 包名+Panel。比如 Login 界面在 Login 包里，主组件为 LoginPanel。  
不按照此规则的话不会报错，只是无法生成相关界面的代码。

### 使用方式
1. FairyGUI 编辑器里只生成配置和图片，不生成代码。
2. 在 ET - BuildTool里，点击"FUI代码生成"按钮，来生成代码。

### UIPackage动态管理
基于 [FairyGUI-Dynamic](https://github.com/SunHowe/FairyGUI-Dynamic.git)模块，为FairyGUI静态跨包依赖的情况提供了支持，该模块内对UIPackage提供了统一的加载与卸载功能
静态跨包依赖指的是在FairyGUI编辑器中，PackageA内的组件1直接引用了PackageB内的组件2或图片3的情况，该模块内部会在加载PackageA时，自动加载其依赖的PackageB，并在PackageA被卸载时，检测PackageB是否还有其他依赖，若没有的话将自动卸载PackageB

# Luban
1. 已将 ET 自带的配置表全部修改为 Luban 的格式，配置表加载流程也做了修改。
2. 修改了默认的代码生成模板，命名和使用方式更接近 ET 的习惯。在 ET - BuildTool里，点击"ExcelExporter" 来使用。

# YooAsset
1. 在 YooAssetHelper 里用 ETTask 对 YooAsset 的异步操作做了扩展。
2. 在 MonoResComponent 里初始化。
3. 在 ResComponentSystem 里调用了 YooAsset 加载的接口。对 Handle 做了缓存。
4. 在 BuildEditor 里切换 PlayMode。

# uTools(额外工具，非必须)

借助uTools工具快捷生成Proto和配置表文件，详细说明请参考[文档](uTools/uTools_README.md)

# Reference
1. ET: 更新至[dad13ae](https://github.com/egametang/ET/commit/dad13aea3675a1e87a7c33d1e513e249feceff56)
2. YooAsset: 更新至[1.4.9](https://github.com/tuyoogame/YooAsset/commit/e2788839586876da483729377580aa5cb8d06408)
3. Luban: 更新至[8e68ab0](https://github.com/focus-creative-games/luban)
4. ET-EUI: https://github.com/zzjfengqing/ET-EUI
5. FairyGUI: https://www.fairygui.com/