using System;
using System.Collections.Generic;

namespace ET
{
    public interface IConfigCategory
    {
        void Resolve(Dictionary<string, IConfigSingleton> _tables);
        
        void TranslateText(System.Func<string, string, string> translator);
    }

    public interface IConfigSingleton: IConfigCategory, ISingleton
    {
        
    }
    
    public abstract class ConfigSingleton<T>: IConfigSingleton where T: ConfigSingleton<T>, new()
    {
        [StaticField]
        private static T instance;

        public static T Instance
        {
            get
            {
                return instance ??= ConfigComponent.Instance.LoadOneConfig(typeof (T)) as T;
            }
        }

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof (T).Name}");
            }
            instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            T t = instance;
            instance = null;
            t.Dispose();
        }

        bool ISingleton.IsDisposed()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
        }

        public abstract void Resolve(Dictionary<string, IConfigSingleton> _tables);

        public abstract void TranslateText(Func<string, string, string> translator);
    }
}