using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace _9M.Work.Utility
{
    public class EntityHelper
    {
        /// <summary>
        /// 克隆实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">需要克隆的实体</param>
        /// <returns></returns>
        public static T CloneEntity<T>(T entity) where T:new()
        {
            T ent = new T();
            PropertyInfo[] propertyInfoes = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propertyInfoes)
            {
                if (propertyInfo.PropertyType.IsArray || (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string)))
                {
                    object child = propertyInfo.GetValue(entity,null);
                    if (child == null)
                        continue;
                    Type childType = child.GetType();
                    if (childType.IsGenericType)
                    {
                        Type typeDefinition = childType.GetGenericArguments()[0];
                        IList items = childType.Assembly.CreateInstance(childType.FullName) as IList;

                        PropertyInfo[] childPropertyInfoes =
                            typeDefinition.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        IList lst = child as IList;
                        for (int i = 0; i < lst.Count; i++)
                        {
                            object itemEntity = null;
                            if (typeDefinition.IsClass && typeDefinition != typeof (string))
                            {
                                itemEntity = typeDefinition.Assembly.CreateInstance(typeDefinition.FullName);
                                foreach (PropertyInfo childProperty in childPropertyInfoes)
                                {
                                    childProperty.SetValue(itemEntity, childProperty.GetValue(lst[i], null), null);
                                }
                            }
                            else
                            {
                                itemEntity = lst[i];
                            }


                            items.Add(itemEntity);
                        }
                        propertyInfo.SetValue(ent, items, null);

                    }
                    else if(childType.IsClass)
                    {
                        propertyInfo.SetValue(ent,propertyInfo.GetValue(entity, null), null);
                    }
                    continue;
                }
                propertyInfo.SetValue(ent, propertyInfo.GetValue(entity, null), null);
            }

            FieldInfo[] fieldInfoes = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fieldInfo in fieldInfoes)
            {
                fieldInfo.SetValue(ent, fieldInfo.GetValue(entity));
            }

            return ent;
        }
    
    }
}
