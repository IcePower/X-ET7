# X-ET 是一个融合了 ET, FairyGUI, luban, YooAsset 的缝合怪。

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

# Luban
1. 已将 ET 自带的配置表全部修改为 Luban 的格式，配置表加载流程也做了修改。
2. 修改了默认的代码生成模板，命名和使用方式更接近 ET 的习惯。在 ET - BuildTool里，点击"ExcelExporter" 来使用。

# YooAsset
1. 在 YooAssetHelper 里用 ETTask 对 YooAsset 的异步操作做了扩展。
2. 在 MonoResComponent 里初始化。这是因为 YooAsset 的初始化是异步的，在热更层没有找到合适的地方。
3. 在 ResComponentSystem 里调用了 YooAsset 加载的接口。对 Handle 做了缓存。
4. 通过 Define.IsAsync 来切换 EditorSimulateMode 和 HostPlayMode。

# Reference
1. ET: 更新至[ec578e6](https://github.com/egametang/ET/commit/ec578e6c2fe1bc6af9ce1e7acf6b9ae79f093ebf)
2. YooAsset: 更新至[cc75594](https://github.com/tuyoogame/YooAsset/commit/cc75594747e4af229ff5e32312bf714786e53b51)
3. Luban: https://github.com/focus-creative-games/luban
4. ET-EUI: https://github.com/zzjfengqing/ET-EUI
5. FairyGUI: https://www.fairygui.com/