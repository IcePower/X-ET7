using System;
using System.Collections.Generic;
using System.IO;
using Bright.Serialization;

namespace ET
{
    public class GetAllConfigBytes: AInvokeHandler<ConfigComponent.GetAllConfigBytes, Dictionary<string,ByteBuf>>
    {
        public override Dictionary<string, ByteBuf> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<string, ByteBuf> output = new Dictionary<string, ByteBuf>();
            List<string> startConfigs = new List<string>()
            {
                "StartMachineConfigCategory", 
                "StartProcessConfigCategory", 
                "StartSceneConfigCategory", 
                "StartZoneConfigCategory",
            };
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof (ConfigAttribute));
            foreach (Type configType in configTypes)
            {
                string configFilePath;
                if (startConfigs.Contains(configType.Name))
                {
                    configFilePath = $"../Config/Excel/s/{Options.Instance.StartConfig}/{configType.Name.ToLower()}.bytes";    
                }
                else
                {
                    configFilePath = $"../Config/Excel/s/{configType.Name}.bytes";
                }
                output[configType.Name] = new ByteBuf(File.ReadAllBytes(configFilePath));
            }

            return output;
        }
    }
    
    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigComponent.GetOneConfigBytes, ByteBuf>
    {
        public override ByteBuf Handle(ConfigComponent.GetOneConfigBytes args)
        {
            ByteBuf configBytes = new ByteBuf(File.ReadAllBytes($"../Config/{args.ConfigName}.bytes"));
            return configBytes;
        }
    }
}