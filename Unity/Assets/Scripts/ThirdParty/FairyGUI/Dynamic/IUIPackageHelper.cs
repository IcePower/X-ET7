namespace FairyGUI.Dynamic
{
    /// <summary>
    /// UIPackage辅助工具接口
    /// </summary>
    public interface IUIPackageHelper
    {
        /// <summary>
        /// 通过包id获取包名
        /// </summary>
        string GetPackageNameById(string id);
    }
}