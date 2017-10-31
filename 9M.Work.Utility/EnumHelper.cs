using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using _9M.Work.Utility;

namespace _9M.Work.Utility
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        #region 取枚举数据

        /// <summary>
        /// 获取枚举的列表
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举的键值集合</returns>
        public static List<EnumEntity> GetEnumList(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            List<EnumEntity> entitys = new List<EnumEntity>();

            Type typeDescription = typeof(TextAttribute);

            FieldInfo[] fields = enumType.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    EnumEntity entity = new EnumEntity();

                    entity.Value = Convert.ToInt32(enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null));

                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        TextAttribute aa = (TextAttribute)arr[0];
                        entity.Text = aa.Text;
                    }
                    else
                    {
                        entity.Text = field.Name;
                    }
                    entitys.Add(entity); ;
                }
            }
            return entitys;
        }


        /// <summary>
        /// 获取枚举的列表
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举的键值集合</returns>
        public static List<EnumElementEntity> GetEnumEnumerable(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            List<EnumElementEntity> entitys = new List<EnumElementEntity>();

            Type typeDescription = typeof(TextAttribute);

            FieldInfo[] fields = enumType.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    EnumElementEntity entity = new EnumElementEntity();

                    entity.Value = Convert.ToInt32(enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null));

                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        TextAttribute aa = (TextAttribute)arr[0];
                        entity.Text = aa.Text;
                    }
                    else
                    {
                        entity.Text = field.Name;
                    }
                    entity.IsSelected = false;
                    entitys.Add(entity); ;
                }
            }
            return entitys;
        }



        /// <summary>
        /// 过去枚举属性TEXT
        /// </summary>
        /// <param name="enumConst">枚举值</param>
        /// <param name="enumType">枚举类型 typeOf()</param>
        /// <returns></returns>
        public static string GetEnumTextVal(int enumConst, Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            string textVal = "";

            Type typeDescription = typeof(TextAttribute);
            FieldInfo fieldInfo = enumType.GetField(System.Enum.GetName(enumType, enumConst).ToString());

            if (fieldInfo != null)
            {
                object[] arr = fieldInfo.GetCustomAttributes(typeDescription, true);
                if (arr.Length > 0)
                {
                    TextAttribute textAttribute = (TextAttribute)arr[0];
                    textVal = textAttribute.Text;
                }
            }

            return textVal;
        }

        /// <summary>
        /// 枚举返回成数据表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable GetEnumTable(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Text", typeof(System.String));
            dt.Columns.Add("Value", typeof(System.String));

            Type typeDescription = typeof(TextAttribute);

            FieldInfo[] fields = enumType.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    DataRow dr = dt.NewRow();

                    dr["Value"] = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();

                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        TextAttribute aa = (TextAttribute)arr[0];
                        dr["Text"] = aa.Text;
                    }
                    else
                    {
                        dr["Text"] = field.Name;
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// Get a table by Enum,the table has Text and Value columns
        /// Added by zzm 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable GetEnumTableForText(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Text", typeof(System.String));
            dt.Columns.Add("Value", typeof(System.String));

            FieldInfo[] fields = enumType.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    DataRow dr = dt.NewRow();

                    dr["Value"] = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    dr["Text"] = enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null).ToString();
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
        #endregion
    }


    #region 枚举实体类

    /// <summary>
    /// 键值集合
    /// </summary>
    public class EnumEntity
    {
        /// <summary>
        /// 枚举名
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; set; }
    }


    /// <summary>
    /// 选择框实体
    /// </summary>
    public class EnumElementEntity
    {
        /// <summary>
        /// 选项名
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 选项值
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }


        public string Background { get; set; }

    }






    /// <summary>
    /// 枚举实体扩展类
    /// </summary>
    public class TextAttribute : Attribute
    {
        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text"></param>
        public TextAttribute(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        public TextAttribute(string text, string url)
        {
            this.Text = text;
            this.Url = url;
        }

    #endregion

    }
}
