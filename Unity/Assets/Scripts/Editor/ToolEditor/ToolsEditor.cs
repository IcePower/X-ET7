using UnityEditor;

namespace ET
{
    public static class ToolsEditor
    {
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string genCode = "sh gen_code.sh";
#else
            const string genCode = "gen_code.bat";
#endif
            ShellHelper.Run($"{genCode}", "../Tools/Luban/");
        }
        
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
        }
    }
}