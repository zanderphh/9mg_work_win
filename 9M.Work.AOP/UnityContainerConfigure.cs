using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.AOP
{
    public class UnityContainerConfigure
    {
        private UnityContainer container;
        public UnityContainerConfigure()
        {
            container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            container.LoadConfiguration(section, "FirstClass");
        }

        public UnityContainerConfigure(string Sectionnode, string Containernode)
        {
            container = new UnityContainer();
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(Sectionnode);
            container.LoadConfiguration(section, Containernode);
        }
        public T GetServer<T>()
        {
            return container.Resolve<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigName">配置文件中指定的文字</param>
        /// <returns></returns>
        public T GetServer<T>(string ConfigName)
        {
            return container.Resolve<T>(ConfigName);
        }

        /// <summary>
        /// 返回构结函数带参数
        /// </summary>
        /// <typeparam name="T">依赖对象</typeparam>
        /// <param name="parameterList">参数集合（参数名，参数值）</param>
        /// <returns></returns>
        public T GetServer<T>(Dictionary<string, object> parameterList)
        {
            var list = new ParameterOverrides();
            foreach (KeyValuePair<string, object> item in parameterList)
            {
                list.Add(item.Key, item.Value);
            }
            return container.Resolve<T>(list);
        }
        /// <summary>
        /// 返回构结函数带参数
        /// </summary>
        /// <typeparam name="T">依赖对象</typeparam>
        /// <param name="ConfigName">配置文件中指定的文字(没写会报异常)</param>
        /// <param name="parameterList">参数集合（参数名，参数值）</param>
        /// <returns></returns>
        public T GetServer<T>(string ConfigName, Dictionary<string, object> parameterList)
        {
            var list = new ParameterOverrides();
            foreach (KeyValuePair<string, object> item in parameterList)
            {
                list.Add(item.Key, item.Value);
            }
            return container.Resolve<T>(ConfigName, list);
        }
    }
}
