using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using _9M.Work.Model;
using System.Reflection;

namespace _9M.Work.DbObject
{
    public class BaseDAL
    {
        public static string WorkPlatConntiong = "server=59.173.238.102,3433;database=WorkPlatform;uid=sa;pwd=www.9mg.cn";
        public static string O2OConntiong = "server=59.173.238.102,3433;database=Db_JunZhe_O2O_V2;uid=sa;pwd=www.9mg.cn";
        public static string TemplateConnectionString { get; set; }
        public static string DBConnectionString
        {
            get;
            set;
        }

        protected string GetSqlString
        {
            get
            {
                return DBConnectionString;
            }
        }

        string strConn = string.Empty;


        public void LookError<T>(Expression<Func<T, bool>> seleWhere) where T : class
        {
            try
            {
                using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
                {
                    var list = db.Set<T>().AsExpandable().Where(seleWhere).ToList();
                }
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
            }
        }

        public string BulkCopy(DataTable dataTable, string TableName)
        {
            string res = "success";
            try
            {
                using (SqlBulkCopy sbc = new SqlBulkCopy(strConn))
                {
                    sbc.BulkCopyTimeout = 300;
                    sbc.DestinationTableName = TableName;
                    sbc.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        #region 构造方法

        public BaseDAL()
        {
            strConn = this.GetSqlString;
        }

        public BaseDAL(string connString)
        {
            strConn = connString;
        }

        #endregion

        #region 通用增删改查

        #region 非原始sql语句方式
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool Add<T>(T entity) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                db.Entry<T>(entity).State = EntityState.Added;
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool Update<T>(T entity) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
        }



        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool UpdateByList<T>(List<T> list) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                foreach (T entity in list)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry<T>(entity).State = EntityState.Modified;
                }
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool Delete<T>(T entity) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Deleted;
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="deleWhere">删除条件</param>
        /// <returns>返回受影响行数</returns>
        public bool DeleteByConditon<T>(Expression<Func<T, bool>> deleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                List<T> entitys = db.Set<T>().Where(deleWhere).ToList();
                entitys.ForEach(m => db.Entry<T>(m).State = EntityState.Deleted);
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T GetSingleById<T>(int id) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Find(id);
            }
        }

        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T GetSingleByStrId<T>(Guid id) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Find(id);
            }
        }

        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll<T>() where T : class
        {
            try
            {
                using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
                {
                    return db.Set<T>().AsExpandable().ToList<T>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }


        /// <summary>
        /// 获取所有实体集合(单个排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll<T, Tkey>(Expression<Func<T, Tkey>> orderWhere, bool isDesc) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return CommonSort(db.Set<T>().AsExpandable(), orderWhere, isDesc).ToList<T>();
            }
        }

        /// <summary>
        /// 获取所有实体集合(多个排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll<T>(params OrderModelField[] orderByExpression) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return CommonSort(db.Set<T>().AsExpandable(), orderByExpression).ToList();
            }
        }

        /// <summary>
        /// 单个排序通用方法
        /// </summary>
        /// <typeparam name="Tkey">排序字段</typeparam>
        /// <param name="data">要排序的数据</param>
        /// <param name="orderWhere">排序条件</param>
        /// <param name="isDesc">是否倒序</param>
        /// <returns>排序后的集合</returns>
        public IQueryable<T> CommonSort<T, Tkey>(IQueryable<T> data, Expression<Func<T, Tkey>> orderWhere, bool isDesc) where T : class
        {
            if (isDesc)
            {
                return data.OrderByDescending(orderWhere);
            }
            else
            {
                return data.OrderBy(orderWhere);
            }
        }

        /// <summary>
        /// 多个排序通用方法
        /// </summary>
        /// <typeparam name="Tkey">排序字段</typeparam>
        /// <param name="data">要排序的数据</param>
        /// <param name="orderWhereAndIsDesc">字典集合(排序条件,是否倒序)</param>
        /// <returns>排序后的集合</returns>
        public IQueryable<T> CommonSort<T>(IQueryable<T> data, params OrderModelField[] orderByExpression) where T : class
        {
            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");

            if (orderByExpression != null && orderByExpression.Length > 0)
            {
                for (int i = 0; i < orderByExpression.Length; i++)
                {
                    //根据属性名获取属性
                    var property = typeof(T).GetProperty(orderByExpression[i].PropertyName);
                    //创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    string OrderName = "";
                    if (i > 0)
                    {
                        OrderName = orderByExpression[i].IsDesc ? "ThenByDescending" : "ThenBy";
                    }
                    else
                        OrderName = orderByExpression[i].IsDesc ? "OrderByDescending" : "OrderBy";

                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), property.PropertyType },
                        data.Expression, Expression.Quote(orderByExp));

                    data = data.Provider.CreateQuery<T>(resultExp);
                }
            }
            return data;
        }

        /// <summary>
        /// 根据条件查询实体集合
        /// </summary>
        /// <param name="seleWhere">查询条件 lambel表达式</param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> seleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().AsExpandable().Where(seleWhere).ToList();
            }
        }

        /// <summary>
        /// 根据条件查询实体集合
        /// </summary>
        /// <param name="seleWhere">查询条件 lambel表达式</param>
        /// <returns></returns>
        public List<T> GetList<T, TValue>(Expression<Func<T, TValue>> seleWhere, IEnumerable<TValue> conditions) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().AsExpandable().WhereIn<T, TValue>(seleWhere, conditions).ToList();
            }
        }

        /// <summary>
        /// 根据条件查询实体集合(单个字段排序)
        /// </summary>
        /// <param name="seleWhere">查询条件 lambel表达式</param>
        /// <returns></returns>
        public List<T> GetList<T, Tkey>(Expression<Func<T, bool>> seleWhere, Expression<Func<T, Tkey>> orderWhere, bool isDesc) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return CommonSort(db.Set<T>().AsExpandable().Where(seleWhere), orderWhere, isDesc).ToList();
            }
        }

        /// <summary>
        /// 根据条件查询实体集合(多个字段排序)
        /// </summary>
        /// <param name="seleWhere">查询条件 lambel表达式</param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> seleWhere, params OrderModelField[] orderByExpression) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return CommonSort(db.Set<T>().AsExpandable().Where(seleWhere), orderByExpression).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(无条件无排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T, Tkey>(int pageIndex, int pageSize, out int totalcount) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return db.Set<T>().AsExpandable().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(无条件单个排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, Tkey>> orderWhere, bool isDesc, out int totalcount) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return CommonSort(db.Set<T>().AsExpandable(), orderWhere, isDesc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(无条件多字段排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T>(int pageIndex, int pageSize, out int totalcount, params OrderModelField[] orderByExpression) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return CommonSort(db.Set<T>().AsExpandable(), orderByExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(有条件无排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> seleWhere, out int totalcount) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Where(seleWhere).Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return db.Set<T>().AsExpandable().Where(seleWhere).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(有条件单个排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> seleWhere,
            Expression<Func<T, Tkey>> orderWhere, bool isDesc, out int totalcount) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Where(seleWhere).Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return CommonSort(db.Set<T>().AsExpandable().Where(seleWhere), orderWhere, isDesc).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取分页集合(有条件多字段排序)
        /// </summary>
        /// <returns></returns>
        public List<T> GetListPaged<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> seleWhere,
            out int totalcount, params OrderModelField[] orderModelFiled) where T : class
        {

            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Where(seleWhere).Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return CommonSort(db.Set<T>().AsExpandable().Where(seleWhere), orderModelFiled).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }


        }
        #endregion

        #region 自定义方法


        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="conditionModelFields">条件</param>
        /// <returns></returns>
        public T GetSingle<T>(params ExpressionModelField[] conditionModelFields) where T : class
        {
            var predicateBuilder = GetExpression_Where<T>(conditionModelFields);
            return GetSingle(predicateBuilder);
        }


        /// <summary>
        /// 根据条件获取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="conditionModelFields">条件</param>
        /// <returns></returns>
        public List<T> GetList<T>(params ExpressionModelField[] conditionModelFields) where T : class
        {
            var seleWhere = GetExpression_Where<T>(conditionModelFields);
            return GetList(seleWhere);
        }


        /// <summary>
        /// 根据条件获取数据(待排序)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="conditionModelFields">条件</param>
        /// <returns></returns>
        public List<T> GetList<T>(ExpressionModelField[] conditionModelFields, OrderModelField[] orderModelFields) where T : class
        {
            var seleWhere = GetExpression_Where<T>(conditionModelFields);

            return GetList(seleWhere, orderModelFields);
        }


        /// <summary>
        /// 分页获取记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页多少记录</param>
        /// <param name="conditionModelFields">条件列</param>
        /// <param name="orderModelFields">排序列</param>
        /// <returns></returns>
        public Dictionary<string, object> GetListPaged<T>(int pageIndex, int pageSize, ExpressionModelField[] conditionModelFields,
            OrderModelField[] orderModelFields) where T : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            var predicateBuilder = GetExpression_Where<T>(conditionModelFields);
            if (orderModelFields == null || orderModelFields.Length == 0)
            {
                orderModelFields = new[] { new OrderModelField() { IsDesc = false, PropertyName = "Id" } };
            }
            int recordCount = 0;
            List<T> list = GetListPaged<T>(pageIndex, pageSize, predicateBuilder, out recordCount, orderModelFields);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("total", recordCount);
            dic.Add("rows", list);
            return dic;
        }

        /// <summary>
        /// 分页获取记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页多少记录</param>
        /// <param name="conditionModelFields">条件列</param>
        /// <param name="orderModelFields">排序列</param>
        /// <returns></returns>
        public Dictionary<string, object> GetListPaged<T>(int pageIndex, int pageSize, ExpressionModelField[] conditionModelFields_And,
            ExpressionModelField[] conditionModelFields_Or, OrderModelField[] orderModelFields) where T : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            var predicateBuilder_And = GetExpression_Where<T>(conditionModelFields_And);
            var predicateBuilder_Or = GetExpression_Where_Or<T>(conditionModelFields_Or);
            var predicateBuilder = predicateBuilder_And.And(predicateBuilder_Or);
            if (orderModelFields == null || orderModelFields.Length == 0)
            {
                orderModelFields = new[] { new OrderModelField() { IsDesc = false, PropertyName = "Id" } };
            }
            int recordCount = 0;
            List<T> list = GetListPaged<T>(pageIndex, pageSize, predicateBuilder, out recordCount, orderModelFields);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("total", recordCount);
            dic.Add("rows", list);
            return dic;
        }




        public Dictionary<string, object> GetListPaged<T>(int pageIndex, int pageSize, OrderModelField[] orderModelFields) where T : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (orderModelFields == null || orderModelFields.Length == 0)
            {
                orderModelFields = new[] { new OrderModelField() { IsDesc = false, PropertyName = "Id" } };
            }
            int recordCount = 0;
            List<T> list = GetListPaged<T>(pageIndex, pageSize, out recordCount, orderModelFields);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("total", recordCount);
            dic.Add("rows", list);
            return dic;
        }


        /// <summary>
        /// 分页获取记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页多少记录</param>
        /// <param name="conditionModelFields">条件列</param>
        /// <param name="orderModelFields">排序列</param>
        /// <returns></returns>
        public Dictionary<string, object> GetListPaged_Include<T>(int pageIndex, int pageSize, LeftModelField leftField, ExpressionModelField[] conditionModelFields,
            OrderModelField[] orderModelFields) where T : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            dynamic lambdaExpression = GetExpression_Include<T>(leftField);
            var predicateBuilder = GetExpression_Where<T>(conditionModelFields);
            if (orderModelFields == null || orderModelFields.Length == 0)
            {
                orderModelFields = new[] { new OrderModelField() { IsDesc = false, PropertyName = "Id" } };
            }
            int recordCount = 0;
            List<T> list = GetListPaged_Include(pageIndex, pageSize, lambdaExpression, predicateBuilder, out recordCount, orderModelFields);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("total", recordCount);
            dic.Add("rows", list);
            return dic;
        }


        /// <summary>
        /// 分页获取记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页多少记录</param>
        /// <param name="conditionModelFields">条件列</param>
        /// <param name="orderModelFields">排序列</param>
        /// <returns></returns>
        public Dictionary<string, object> GetListPaged_Include<T>(int pageIndex, int pageSize, LeftModelField leftField, ExpressionModelField[] conditionModelFields_And,
            ExpressionModelField[] conditionModelFields_Or, OrderModelField[] orderModelFields) where T : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            dynamic lambdaExpression = GetExpression_Include<T>(leftField);
            var predicateBuilder_And = GetExpression_Where<T>(conditionModelFields_And);
            var predicateBuilder_Or = GetExpression_Where_Or<T>(conditionModelFields_Or);

            var predicateBuilder = PredicateBuilder.True<T>();
            predicateBuilder = predicateBuilder.And(predicateBuilder_And);
            predicateBuilder = predicateBuilder.And(predicateBuilder_Or);

            if (orderModelFields == null || orderModelFields.Length == 0)
            {
                orderModelFields = new[] { new OrderModelField() { IsDesc = false, PropertyName = "Id" } };
            }
            int recordCount = 0;
            List<T> list = GetListPaged_Include(pageIndex, pageSize, lambdaExpression, predicateBuilder, out recordCount, orderModelFields);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("total", recordCount);
            dic.Add("rows", list);
            return dic;
        }


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <returns></returns>
        public LambdaExpression GetExpression_Include<T>(LeftModelField includeField) where T : class
        {
            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "model");
            //根据属性名获取属性
            var property = Expression.Property(parameter, includeField.PropertyName);
            //创建表达式
            var lambdaExpression = Expression.Lambda(property, parameter);

            return lambdaExpression;
        }


        /// <summary>
        /// 动态生成条件表达式
        /// </summary>
        /// <typeparam name="T">返回实体类型</typeparam>
        /// <param name="expressionModels">条件列</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetExpression_Where<T>(ExpressionModelField[] expressionModels)
        {
            var predicateBuilder = PredicateBuilder.True<T>();
            if (expressionModels != null && expressionModels.Length > 0)
            {
                foreach (var expressionModel in expressionModels)
                {
                    var parameter = Expression.Parameter(typeof(T), "model");
                    var left = Expression.Property(parameter, expressionModel.Name);
                    var right = Expression.Constant(expressionModel.Value);
                    PropertyInfo pi = typeof(T).GetProperty(left.Member.Name);
                    if (pi != null)
                    {
                        right = Expression.Constant(expressionModel.Value, pi.PropertyType);
                    }

                    Expression be = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
                    switch (expressionModel.Relation)
                    {
                        case EnumRelation.Equal:
                            be = Expression.Equal(left, right);
                            break;
                        case EnumRelation.GreaterThan:
                            be = Expression.GreaterThan(left, right);
                            break;
                        case EnumRelation.GreaterThanOrEqual:
                            be = Expression.GreaterThanOrEqual(left, right);
                            break;
                        case EnumRelation.LessThan:
                            be = Expression.LessThan(left, right);
                            break;
                        case EnumRelation.LessThanOrEqual:
                            be = Expression.LessThanOrEqual(left, right);
                            break;
                        case EnumRelation.NotEqual:
                            be = Expression.NotEqual(left, right);
                            break;
                        case EnumRelation.Contains:
                            MethodCallExpression memberExpression = Expression.Call(left,
                                typeof(string).GetMethod("Contains"), right);
                            //be = Expression.And(be, memberExpression);
                            be = memberExpression;
                            break;
                        default:
                            be = Expression.Equal(left, right);
                            break;
                    }
                    var lambda = Expression.Lambda<Func<T, bool>>(be, parameter);
                    predicateBuilder = predicateBuilder.And(lambda);
                }
            }
            return predicateBuilder;
        }


        /// <summary>
        /// 动态生成条件表达式
        /// </summary>
        /// <typeparam name="T">返回实体类型</typeparam>
        /// <param name="expressionModels">or条件列</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetExpression_Where_Or<T>(ExpressionModelField[] expressionModels)
        {
            var predicateBuilder = PredicateBuilder.True<T>();
            if (expressionModels != null && expressionModels.Length > 0)
            {
                int index = 0;
                foreach (var expressionModel in expressionModels)
                {
                    var parameter = Expression.Parameter(typeof(T), "model");
                    var left = Expression.Property(parameter, expressionModel.Name);
                    var right = Expression.Constant(expressionModel.Value);
                    Expression be = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
                    switch (expressionModel.Relation)
                    {
                        case EnumRelation.Equal:
                            be = Expression.Equal(left, right);
                            break;
                        case EnumRelation.GreaterThan:
                            be = Expression.GreaterThan(left, right);
                            break;
                        case EnumRelation.GreaterThanOrEqual:
                            be = Expression.GreaterThanOrEqual(left, right);
                            break;
                        case EnumRelation.LessThan:
                            be = Expression.LessThan(left, right);
                            break;
                        case EnumRelation.LessThanOrEqual:
                            be = Expression.LessThanOrEqual(left, right);
                            break;
                        case EnumRelation.NotEqual:
                            be = Expression.NotEqual(left, right);
                            break;
                        case EnumRelation.Contains:
                            MethodCallExpression memberExpression = Expression.Call(left,
                                typeof(string).GetMethod("Contains"), right);
                            be = memberExpression;
                            break;
                        default:
                            be = Expression.Equal(left, right);
                            break;
                    }
                    Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(be, parameter);
                    predicateBuilder = index == 0 ? predicateBuilder.And(lambda) : predicateBuilder.Or(lambda);

                    index++;
                }
            }
            return predicateBuilder;
        }

        #endregion

        #region 扩展方法


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool DeleteByEntity<T>(T entity) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                db.Set<T>().Attach(entity);
                db.Entities.Remove(entity);

                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 根据主键Id来删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public bool DeleteById<T>(int id) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                T t = db.Set<T>().Find(id);
                db.Entities.Remove(t);
                return db.SaveChanges() > 0;
            }
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool DeleteByList<T>(List<T> lists) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                foreach (T entity in lists)
                {
                    db.Set<T>().Attach(entity);
                    db.Entities.Remove(entity);
                }
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> seleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().SingleOrDefault(seleWhere);
            }
        }

        public bool AddList<T>(List<T> list) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                foreach (T t in list)
                {
                    db.Entities.Add(t);
                }
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public T AddEntity<T>(T entity) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                T t = db.Entities.Add(entity);
                return db.SaveChanges() > 0 ? t : null;
            }
        }


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <returns></returns>
        public T GetSingleById_Include<T, Tkey>(Expression<Func<T, Tkey>> expression, Expression<Func<T, bool>> seleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Include(expression).SingleOrDefault(seleWhere);
            }
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <returns></returns>
        public T GetSingle_Include<T, Tkey>(Expression<Func<T, Tkey>> expression, Expression<Func<T, bool>> seleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Include(expression).SingleOrDefault(seleWhere);
            }
        }

        /// <summary>
        /// 根据条件查询实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="seleWhere">查询条件</param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public List<T> GetList_Include<T, TValue>(Expression<Func<T, TValue>> expression, Expression<Func<T, TValue>> seleWhere, IEnumerable<TValue> conditions) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Include(expression).AsExpandable().WhereIn<T, TValue>(seleWhere, conditions).ToList();
            }
        }



        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll_Include<T, Tkey>(Expression<Func<T, Tkey>> expression) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Include(expression).AsExpandable().ToList();
            }
        }

        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll_Include<T, Tkey>(Expression<Func<T, Tkey>> expression, Expression<Func<T, bool>> seleWhere) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Set<T>().Include(expression).Where(seleWhere).AsExpandable().ToList();
            }
        }


        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll_Include<T, Tkey>(Expression<Func<T, Tkey>> expression, Expression<Func<T, bool>> seleWhere, params OrderModelField[] orderModelFiled) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return CommonSort(db.Set<T>().Include(expression).AsExpandable().Where(seleWhere), orderModelFiled).ToList();
            }
        }
        public List<T> GetListPaged_Include<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, Tkey>> expression, Expression<Func<T, bool>> seleWhere,
            out int totalcount, params OrderModelField[] orderModelFiled) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                totalcount = db.Set<T>().AsExpandable().Where(seleWhere).Count();//获取总数
                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
                return CommonSort(db.Set<T>().Include(expression).AsExpandable().Where(seleWhere), orderModelFiled).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        #endregion

        #region 原始sql操作

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        public int ExecuteSql(string sql)
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.ExecuteSqlCommand(sql);
            }
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        public int ExecuteSql(string sql, params object[] paras)
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.ExecuteSqlCommand(sql, paras);
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public List<T> QueryList<T>(string sql, params object[] paras) where T : class
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.SqlQuery<T>(sql, paras).ToList();
            }
        }


        public List<T> QueryDataTable<T>(string sql, params object[] paras) where T : class
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.SqlQuery<T>(sql, paras).ToList();
            }
        }
        /*
                public Li QueryToDataTable<T>(string sql, params object[] paras) where T :class
                {
                    using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
                    {
                        return db.Database.SqlQuery<T>(sql, paras).ToList();
                    }
                }*/


        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql, params object[] paras) where T : class
        {
            using (_9MWorkDataContext<T> db = new _9MWorkDataContext<T>(strConn))
            {
                return db.Database.SqlQuery<T>(sql, paras).FirstOrDefault();
            }
        }


        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="lsSql"></param>
        /// <param name="lsParas"></param>
        public bool ExecuteTransaction(List<String> lsSql, List<Object[]> lsParas)
        {
            bool isRes = false;
            try
            {
                using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < lsSql.Count; i++)
                            {
                                if (lsParas != null && lsParas.Count > 0)
                                {
                                    db.Database.ExecuteSqlCommand(lsSql[i], lsParas[i]);
                                }
                                else
                                {
                                    db.Database.ExecuteSqlCommand(lsSql[i]);
                                }
                            }
                            /*      foreach (String item in lsSql)
                                  {
                                      db.Database.ExecuteSqlCommand(item);
                                  }
                            */
                            tran.Commit();
                            isRes = true;
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            isRes = false;
                        }
                    }
                }

            }
            catch (Exception err)
            {
               
            }
            return isRes;
        }



        #region 自定义方法

        /// <summary>
        /// 查询单个列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public T QueryField<T>(string sql, params object[] paras) where T : struct
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.SqlQuery<T>(sql, paras).FirstOrDefault();
            }
        }

        /// <summary>
        /// 查询单个列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public T QueryField2<T>(string sql, params object[] paras) where T : class
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.SqlQuery<T>(sql, paras).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取列表(动态类型)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="paras">查询参数值</param>
        /// <returns></returns>
        public IEnumerable<dynamic> QueryList(string sql, params object[] paras)
        {
            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                return db.Database.SqlQuery<dynamic>(sql, paras);
            }
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="paras">查询参数值</param>
        /// <returns>返回DataTable</returns>
        public DataTable QueryDataTable(string sql, params object[] paras)
        {
            DataTable table = new DataTable();

            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                using (var conn = new SqlConnection(strConn))
                {
                    conn.Open();

                    using (SqlDataAdapter sqlAd = new SqlDataAdapter(sql, conn))
                    {
                        sqlAd.SelectCommand.Parameters.AddRange(paras);
                        sqlAd.Fill(table);
                    }
                }
            }

            return table;
        }

        #endregion

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="dataTable"></param>
        public bool BulkInsertAll(DataTable dataTable)
        {
            bool res = false;

            using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
            {
                string connStr = db.Database.Connection.ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = string.Format("[{0}]", dataTable.TableName);
                            try
                            {
                                bulkCopy.WriteToServer(dataTable);
                                transaction.Commit();
                                res = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                transaction.Rollback();
                                res = false;
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                }
            }

            return res;
        }


        #endregion

        #endregion

        #region 通用属性
        /// <summary>
        /// 获取数据库服务器当前时间。
        /// </summary>
        public DateTime ServerTime
        {
            get
            {
                using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
                {
                    String sql = "SELECT GETDATE()";
                    Object objServerTime = db.Database.SqlQuery<Object>(sql);
                    return Convert.ToDateTime(objServerTime);
                }
            }
        }

        /// <summary>
        /// 获取数据库版本。
        /// </summary>
        public String DatabaseVersion
        {
            get
            {
                using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
                {
                    try
                    {
                        String sql = "SELECT Version FROM Sys_Version";
                        Object objServerTime = db.Database.SqlQuery<Object>(sql);
                        return Convert.ToString(objServerTime);
                    }
                    catch
                    {
                    }
                    return String.Empty;
                }
            }
        }
        #endregion


    }

    #region 扩展方法  支持 in 操作

    public static class QueryableExtension
    {
        /// <summary>
        /// 扩展方法  支持 in 操作
        /// </summary>
        /// <typeparam name="TEntity">需要扩展的对象类型</typeparam>
        /// <typeparam name="TValue">in 的值类型</typeparam>
        /// <param name="source">需要扩展的对象</param>
        /// <param name="valueSelector">值选择器 例如c=>c.UserId</param>
        /// <param name="values">值集合</param>
        /// <returns></returns>
        public static IQueryable<TEntity> WhereIn<TEntity, TValue>(this IQueryable<TEntity> source, Expression<Func<TEntity, TValue>> valueSelector,
                IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
            {
                return source;
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return source.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
        }
    }

    #endregion
}
