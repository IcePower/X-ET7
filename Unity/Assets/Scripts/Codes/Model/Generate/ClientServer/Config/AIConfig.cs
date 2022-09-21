//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace ET
{

public sealed partial class AIConfig: Bright.Config.BeanBase
{
    public AIConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        AIConfigId = _buf.ReadInt();
        Order = _buf.ReadInt();
        Name = _buf.ReadString();
        Desc = _buf.ReadString();
        {int __n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);NodeParams = new int[__n0];for(var __index0 = 0 ; __index0 < __n0 ; __index0++) { int __e0;__e0 = _buf.ReadInt(); NodeParams[__index0] = __e0;}}
        PostInit();
    }

    public static AIConfig DeserializeAIConfig(ByteBuf _buf)
    {
        return new AIConfig(_buf);
    }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 所属ai
    /// </summary>
    public int AIConfigId { get; private set; }
    /// <summary>
    /// 此ai中的顺序
    /// </summary>
    public int Order { get; private set; }
    /// <summary>
    /// 节点名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 节点参数
    /// </summary>
    public int[] NodeParams { get; private set; }

    public const int __ID__ = -294143606;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, IConfigSingleton> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "AIConfigId:" + AIConfigId + ","
        + "Order:" + Order + ","
        + "Name:" + Name + ","
        + "Desc:" + Desc + ","
        + "NodeParams:" + Bright.Common.StringUtil.CollectionToString(NodeParams) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}