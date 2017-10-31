using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace _9M.Work.WSVariable
{
    /// <summary>  
    /// SOAP头  
    /// </summary>  
    public class WSSoapHeader : Variable
    {
        /// <summary>  
        /// 构造一个SOAP头  
        /// </summary>  
        public WSSoapHeader()
        {
            this.Properties = new SerializableDictionary<string, object>();
        }

        /// <summary>  
        /// 构造一个SOAP头  
        /// </summary>  
        /// <param name="className">SOAP头的类名</param>  
        public WSSoapHeader(string className)
        {
            this.ClassName = className;
            this.Properties = new SerializableDictionary<string, object>();
        }

        /// <summary>  
        /// 构造一个SOAP头  
        /// </summary>  
        /// <param name="className">SOAP头的类名</param>  
        /// <param name="properties">SOAP头的类属性名及属性值</param>  
        public WSSoapHeader(string className, SerializableDictionary<string, object> properties)
        {
            this.ClassName = className;
            this.Properties = properties;
        }

        /// <summary>  
        /// SOAP头的类名  
        /// </summary>  
        public string ClassName { get; set; }


        /// <summary>  
        /// SOAP头的类属性名及属性值  
        /// </summary>  
        public SerializableDictionary<string, object> Properties { get; set; }

        /// <summary>  
        /// 为SOAP头增加一个属性及值  
        /// </summary>  
        /// <param name="name">SOAP头的类属性名</param>  
        /// <param name="value">SOAP头的类属性值</param>  
        public void AddProperty(string name, object value)
        {
            if (this.Properties == null)
            {
                this.Properties = new SerializableDictionary<string, object>();
            }
            Properties.Add(name, value);
        }
    }


}
