using System;
using System.Collections.Generic;
using System.IO;
using Bright.Serialization;
using UnityEngine;

namespace ET.Client
{
    [Callback]
    public class GetAllConfigBytes: ACallbackHandler<ConfigComponent.GetAllConfigBytes, Dictionary<string, ByteBuf>>
    {
        public override Dictionary<string, ByteBuf> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Root.Instance.Scene.AddComponent<ResComponent>();

            Dictionary<string, ByteBuf> output = new Dictionary<string, ByteBuf>();
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof(ConfigAttribute));

            if (Define.IsEditor)
            {
                string ct = "cs";
                CodeMode codeMode = Init.Instance.GlobalConfig.CodeMode;
                switch (codeMode)
                {
                    case CodeMode.Client:
                        ct = "c";
                        break;
                    case CodeMode.Server:
                        ct = "s";
                        break;
                    case CodeMode.ClientServer:
                        ct = "cs";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                List<string> startConfigs = new List<string>()
                {
                    "StartMachineConfigCategory", 
                    "StartProcessConfigCategory", 
                    "StartSceneConfigCategory", 
                    "StartZoneConfigCategory",
                };
                foreach (Type configType in configTypes)
                {
                    string configFilePath;
                    if (startConfigs.Contains(configType.Name))
                    {
                        configFilePath = $"../Config/Excel/{ct}/{Options.Instance.StartConfig}/{configType.Name.ToLower()}.bytes";    
                    }
                    else
                    {
                        configFilePath = $"../Config/Excel/{ct}/{configType.Name}.bytes";
                    }
                    output[configType.Name] = new ByteBuf(File.ReadAllBytes(configFilePath));
                }
            }
            else
            {
                foreach (Type configType in configTypes)
                {
                    TextAsset v = ResComponent.Instance.LoadAsset<TextAsset>(configType.Name.ToLower()) as TextAsset;
                    output[configType.Name] = new ByteBuf(v.bytes);
                }
            }
            
            return output;
        }
    }
    
    [Callback]
    public class GetOneConfigBytes: ACallbackHandler<ConfigComponent.GetOneConfigBytes, ByteBuf>
    {
        public override ByteBuf Handle(ConfigComponent.GetOneConfigBytes args)
        {
            throw new NotImplementedException("client cant use LoadOneConfig");
        }
    }
}