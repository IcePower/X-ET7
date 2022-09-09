using System;
using System.Collections.Generic;
using Bright.Serialization;
using UnityEngine;

namespace ET.Client
{
    [Callback]
    public class GetAllConfigBytes: ACallbackHandler<ConfigComponent.GetAllConfigBytes, Dictionary<string, ByteBuf>>
    {
        public override Dictionary<string, ByteBuf> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<string, ByteBuf> output = new Dictionary<string, ByteBuf>();

            Root.Instance.Scene.AddComponent<ResComponent>();
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof(ConfigAttribute));
            foreach (Type configType in configTypes)
            {
                TextAsset v = ResComponent.Instance.LoadAsset<TextAsset>(configType.Name.ToLower()) as TextAsset;
                output[configType.Name] = new ByteBuf(v.bytes);
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