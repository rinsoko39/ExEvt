using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using m2d;
using UnityEngine;
using XX;

namespace ExEvt;

[BepInPlugin("rinsoko.exevt", "ExEvt", "0.1")]
public class ExEvtPlugin: BaseUnityPlugin {

    public static ManualLogSource LoggerInstance = null!;

    public void Awake() {
        LoggerInstance = Logger;
        Logger.LogInfo("Hello, world!");
        Harmony.CreateAndPatchAll(GetType());
    }

    // MARK: Merge Evt

    [HarmonyPatch(typeof(NKT), "readStreamingText")]
    [HarmonyPostfix]
    public static void MergeExText(string path, ref string __result) {
        LoggerInstance.LogInfo($"Processing {path}.");
        var merger = new TextMerger(__result);
        var dir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "exevt"));
        bool flag = false;
        foreach(var subdir in dir.GetDirectories()) {
            if(GetFile(subdir, path) is FileInfo file) {
                LoggerInstance.LogInfo($"Found {path} in {subdir.Name}.");
                flag = true;
                var content = Read(file);
                content = content.Replace("//!ORIGIN", __result);
                merger.Merge(content);
            }
        }
        if(flag)
            LoggerInstance.LogInfo($"Merged evt cotent:\n{merger}");
        __result = merger.ToString();
    }

    private static string Read(FileInfo file) {
        using var stream = new StreamReader(file.OpenRead());
        return stream.ReadToEnd();
    }

    private static FileInfo? GetFile(DirectoryInfo dir, string name) {
        var file = Path.Combine(dir.FullName, name);
        if(!File.Exists(file))
            return null;
        return new FileInfo(file);
    }
    
    // MARK: Custom Command

    public delegate void Command(StringHolder argv);

    public static SortedDictionary<string, Command> CustomCommands { get; } = [];

    [HarmonyPatch(typeof(M2DEventListener), "EvtRead")]
    [HarmonyPostfix]
    public static void ReadCustomEvt(StringHolder rER, ref bool __result) {
        if(__result)
            return;
        if(CustomCommands.TryGetValue(rER.cmd, out var command)) {
            LoggerInstance.LogInfo($"Custom command: {rER.cmd}");
            command.Invoke(rER);
            __result = true;
        }
    }
    
    public static void AssignCustomCommand(string name, Command action)
        => CustomCommands.Add(name, action);
    
    public static void AssignCustomCommand(string name, Action action)
        => CustomCommands.Add(name, (_) => action.Invoke());

}
