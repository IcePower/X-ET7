using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class LocalizeComponent : Entity, IAwake
    {
        public SystemLanguage CurrentLanguage { get; set; }

    }
}