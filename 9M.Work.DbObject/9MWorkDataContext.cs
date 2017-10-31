using _9M.Work.DbObject.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.DbObject
{
    public class _9MWorkDataContext : DbContext
    {
        private bool isNew = true; //是否是新的sql执行  
        private string strMsg = ""; //sql执行的相关信息  
        private string strConn = ""; //数据库连接字符串  
        private string UserName = ""; //日志用户名称  
        private string AdditionalInfo = ""; //日志额外信息  



        public _9MWorkDataContext(string connString) : // 数据库链接字符串  
            base(connString)
        {
            strConn = connString;
            //DbConfiguration.SetConfiguration(new Configuration(connString, true));
            Database.SetInitializer<_9MWorkDataContext>(null); //设置为空，防止自动检查和生成
            base.Database.Log = (info) => Debug.WriteLine(info);
            this.Configuration.LazyLoadingEnabled = true;
        }

        public _9MWorkDataContext(string connString, string logUserName, string logAdditionalInfo) : // 数据库链接字符串  
            base(connString)
        {
            strConn = connString;
            Database.SetInitializer<_9MWorkDataContext>(null); //设置为空，防止自动检查和生成  
            UserName = logUserName;
            AdditionalInfo = logAdditionalInfo;
            base.Database.Log = AddLogger;
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //去掉复数映射  
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            #region 这里映射数据表关系
            //测试
            modelBuilder.Configurations.Add(new TestMap());
            //品牌
            modelBuilder.Configurations.Add(new BrandMap());
            //商品
            modelBuilder.Configurations.Add(new WareMap());
            // modelBuilder.Configurations.Add(new WareSpecMap());
            //分类
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CategoryPropertyMap());
            //颜色
            modelBuilder.Configurations.Add(new ColorMap());
            //权限
            modelBuilder.Configurations.Add(new PermissionMap());
            modelBuilder.Configurations.Add(new DeptMap());
            modelBuilder.Configurations.Add(new UserInfoMap());
            //质检部
            modelBuilder.Configurations.Add(new QualityBatchMap());
            //商品日志
            modelBuilder.Configurations.Add(new WareLogMap());
            //店铺表
            modelBuilder.Configurations.Add(new ShopMap());
            //退货表
            modelBuilder.Configurations.Add(new RefundMap());
            //退货明细表
            modelBuilder.Configurations.Add(new RefundDetailMap());
            //未知包裹表
            modelBuilder.Configurations.Add(new UnknownGoodsMap());
            //未知款
            modelBuilder.Configurations.Add(new UnknownlistMap());
            //及时到帐
            modelBuilder.Configurations.Add(new RegisterJSDZMap());
            //最后更新时间
            modelBuilder.Configurations.Add(new LastSyncExpressTimeMap());
            //操作日志
            modelBuilder.Configurations.Add(new RefundLogMap());
            //商品日志
            modelBuilder.Configurations.Add(new GoodsLogMap());
            //活动
            modelBuilder.Configurations.Add(new ActivityMap());
            modelBuilder.Configurations.Add(new ActivityGoodsMap());
            modelBuilder.Configurations.Add(new ActivityLogMap());

            //福袋
            modelBuilder.Configurations.Add(new FuDaiBatchMap());
            modelBuilder.Configurations.Add(new FuDaiGoodsMap());
            //仓库盘点
            modelBuilder.Configurations.Add(new AndroidPDMap());

            //仓库日志
            modelBuilder.Configurations.Add(new AndroidLogMap());

            //扫描签收
            modelBuilder.Configurations.Add(new AndroidScanReceiptMap());

            //报表打印信息
            modelBuilder.Configurations.Add(new ReportInfoMap());

            modelBuilder.Configurations.Add(new FinanceRefundMap());

            //摄影绩效
            modelBuilder.Configurations.Add(new PhotographyMap());

            modelBuilder.Configurations.Add(new PhotographyDetailMap());

            //直播管理
            modelBuilder.Configurations.Add(new LiveMap());

            modelBuilder.Configurations.Add(new LiveGoodsMap());

            modelBuilder.Configurations.Add(new LiveCheckMap());

            modelBuilder.Configurations.Add(new NewMsgNoticeMap());
            #endregion
        }

        /// <summary>  
        /// 添加日志  
        /// </summary>  
        /// <param name="info"></param>  
        public void AddLogger(string info)
        {
            if (info != "\r\n" && (!info.Contains("Sys_EventLog")))
            {
                string strTemp = info.ToUpper().Trim();
                if (isNew)
                {
                    //记录增删改  
                    if (strTemp.StartsWith("INSERT") || strTemp.StartsWith("UPDATE") || strTemp.StartsWith("DELETE"))
                    {
                        strMsg = info;
                        isNew = false;
                    }
                }
                else
                {
                    if (strTemp.StartsWith("CLOSED CONNECTION"))
                    {
                        //增加新日志  
                        using (_9MWorkDataContext db = new _9MWorkDataContext(strConn))
                        {
                            try
                            {
                                //保存日志到数据库或其他地方  

                            }
                            catch (Exception ex)
                            {
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//logError.txt"))
                                {
                                    sw.Write(ex.Message);
                                    sw.Flush();
                                }
                            }
                        }
                        //清空  
                        strMsg = "";
                        isNew = true;
                    }
                    else
                    {
                        strMsg += info;
                    }
                }

            }
        }
    }







    public class _9MWorkDataContext<T> : _9MWorkDataContext where T : class
    {

        public _9MWorkDataContext(string connString) : // 数据库链接字符串  
            base(connString)
        {
            Database.SetInitializer<_9MWorkDataContext<T>>(null);//设置为空，防止自动检查和生成 
            this.Configuration.LazyLoadingEnabled = true;
        }


        public _9MWorkDataContext(string connString, string logUserName, string logAdditionalInfo) : // 数据库链接字符串  
            base(connString, logUserName, logAdditionalInfo)
        {
            Database.SetInitializer<_9MWorkDataContext<T>>(null);//设置为空，防止自动检查和生成  
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<T> Entities { get; set; }
    }
}
