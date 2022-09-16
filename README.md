#### X-ET 是一个融合了 ET, FairyGUI, luban, YooAsset 的缝合怪。

# FairyGUI
### 代码生成
使用 C# 实现了 FairyGUI 代码生成功能，和原 FairyGUI 编辑器代码生成相比有以下优点：
1. 扩展方便，不用再去写lua插件了。
2. 可以获取到跨包引用组件的类型，原FairyGUI编辑器生成的是GComponent。
3. 根据控制器页面生成了枚举，方便调用。  
### 使用方式
FairyGUI 编辑器里只生成配置，不生成代码。在 BuildEditor 界面里，点击"FUI代码生成"按钮，来生成代码。

# Luban
1. 已将 ET 自带的配置表全部修改为 Luban 的格式，配置表加载流程也做了修改。
2. 修改了默认的代码生成模板，命名和使用方式更接近 ET 的习惯。在 BuildEditor 界面里，点击"ExcelExporter" 来使用。

# YooAsset
通过 Define.IsAsync 来切换 EditorSimulateMode 和 HostPlayMode。  
未来接入 wolong 后，初始化流程会有一些变动。
