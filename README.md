# ExEvt

ExEvt 是 ACT 游戏 Alice In Cradle 的插件。它用于向其他插件提供更便利的事件功能。

## 功能预览

### 合并多方事件修改

有时，可能遇到两个插件需要修改同一个事件文件的情况。即使修改的内容没有冲突，也无法将它们合并。ExEvt 支持不同插件对同一事件进行修改，ExEvt 会自动合并两次修改。

例如，对于与南丁格尔小姐的对话事件 `evt/___Nightingale/_portal.cmd` 中的一段：

```
SELECTARRAY_CLEAR
SELECTARRAY &&Select_buying BUYING
SELECTARRAY &&Select_talk LETSTALK $_qflag
SELECTARRAY &&Select_nothanks SKIP
SELECT_FOCUS $_foc_default
SELECT -1
```

若有两个插件分别添加了一个选项：

```
SELECTARRAY_CLEAR
SELECTARRAY &&Select_buying BUYING
SELECTARRAY &&Select_talk LETSTALK $_qflag
SELECTARRAY &&Select_nothanks SKIP
SELECTARRAY 亲亲 KISS
SELECT_FOCUS $_foc_default
SELECT -1
```

```
SELECTARRAY_CLEAR
SELECTARRAY &&Select_buying BUYING
SELECTARRAY &&Select_talk LETSTALK $_qflag
SELECTARRAY &&Select_nothanks SKIP
SELECTARRAY 抱抱 HUG
SELECT_FOCUS $_foc_default
SELECT -1
```

则经 ExEvt 合并后将变为：

```
SELECTARRAY_CLEAR
SELECTARRAY &&Select_buying BUYING
SELECTARRAY &&Select_talk LETSTALK $_qflag
SELECTARRAY &&Select_nothanks SKIP
SELECTARRAY 亲亲 KISS
SELECTARRAY 抱抱 HUG
SELECT_FOCUS $_foc_default
SELECT -1
```

从而添加的两个选项都将出现。

### 自定义指令

ExEvt 允许其他插件自定义指令。

自定义指令时，插件应传递一个指令名和一个委托，当遇到该指令时，将执行该委托。

自定义指令与事件修改一同使用，可以为插件制作提供诸多便利。

## 安装方法

1. [下载 BepInEx](https://github.com/BepInEx/BepInEx/releases)，将其解压至 Alice In Cradle 的根目录。

2. [下载 ExEvt.dll](https://github.com/rinsoko39/ExEvt/releases)，将其放入 `/BepInEx/plugins`。

## 使用指南

### 合并多方事件修改

1. 在 `StreamingAssets` 目录中创建 `exevt` 文件夹，在其中创建以你的插件名称命名的子文件夹。

2. 将要修改的文件以相对 `StreamingAssets` 的路径复制到创建的文件夹中。

3. 修改复制后的文件。

例如，你的插件名为 `Foo`，那么创建目录 `/AliceInCradle_Data/StreamingAssets/exevt/Foo/`。

你希望修改 `/AliceInCradle_Data/StreamingAssets/evt/___Nightingale/_portal.cmd`，它的相对路径是 `evt/___Nightingale/_portal.cmd`，那么将其复制到 `/AliceInCradle_Data/StreamingAssets/exevt/Foo/evt/___Nightingale/_portal.cmd`。

---

如果要保留原文件的内容，仅仅在其开头或结尾添加内容，可以在文件中以 `//!ORIGIN` 代替原始内容。

例如，如果想在事件进行时暂停音乐，可以这么写：

```
STOP_BGM
//!ORIGIN
START_BGM
```

### 自定义指令

请先创建一个 BepInEx 插件，并将 `ExEvt.dll` 加入项目引用。

通过调用 `ExEvt.ExEvtPlugin.AssignCustomCommand` 自定义指令。以下是 API：

---

#### `ExEvtPlugin.AssignCustomCommand(string, Action)`

```csharp
public static void AssignCustomCommand(string name, Action action);
```

定义一条自定义指令。

##### 参数

**`name`** `string`

指令名称。

**`action`** `Action`

运行指令时调用的委托。

---

#### `ExEvtPlugin.AssignCustomCommand(string, Command)`

```csharp
public static void AssignCustomCommand(string name, Command action);
```

定义一条自定义指令。

##### 参数

**`name`** `string`

指令名称。

**`action`** `Command`

运行指令时调用的委托。

---

#### `ExEvtPlugin.Command`

```csharp
public delegate void Command(StringHolder argv);
```

封装一个可以读取指令参数的方法。

##### 参数

**`argv`** `StringHolder`

参数列表。

`StringHolder` 是 `unsafeAssem.dll` 中的类 `XX.StringHolder`，它存储了指令的参数列表，并可以方便地读取参数。例如，`StringHolder._xU` 表示第 x 个参数转换成大写后的值；`StringHolder._Bx` 表示第 x 个参数转换成布尔类型后的值。

---

例如，你需要定义一个名为 `ADD` 的指令，在调用该指令时，将输出后两个参数的和。你可以使用如下代码：

```csharp
ExEvtPlugin.AssignCustomCommand("ADD", (argv) => {
    var res = argv._N1 + argv._N2;
    Debug.Log(res);
});
```

当调用指令 `ADD 114 514` 时，将输出 `628`。

## 编译该库

1. 确保当前 .NET SDK 版本至少为 8.0。

2. 将 Alice In Cradle 游戏目录中的 `/AliceInCradle_Data/Managed/*.dll` 和 `/BepInEx/core/*.dll` 复制到该目录中的 `/lib/`。

3. 在命令行中运行 `dotnet build`，或使用相关工具进行编译。
