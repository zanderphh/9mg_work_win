using _9M.Work.DbObject;
using _9M.Work.Model;
using _9M.Work.WPF_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _9M.Work.WPF_Main.ControlTemplate
{
    /// <summary>
    /// PerformancePanel.xaml 的交互逻辑
    /// </summary>
    public partial class PerformancePanel : UserControl
    {
        BaseDAL dal = new BaseDAL();
        public PerformancePanel()
        {
            InitializeComponent();
        }

        public int Performance { get; set; }
        /// <summary>
        /// 绑定绩校
        /// </summary>
        public void BindPerformance(int UserId)
        {
            string Sql = string.Format(@"
 Declare @Week smallint,@Date datetime
Set datefirst 1 /* 设置周的第一天 */
Set @Week= DATEPART(wk,GETDATE())/* 第几周 */
Set @Date=Dateadd(week,@Week-1,rtrim(year(getdate()))+'0101')-datepart(dw,rtrim(year(getdate()))+'0101')+1 /* 算出第16周的第一个日期 */
select CONVERT(varchar(100), a.dates, 111) as Now,ISNULL(c.deptname,'') as Dept,b.username,
a.finishcount as PerformanceDay , d.PerformanceMonth,IsNull( e.PerformanceWeek,0) as PerformanceWeek
 from T_Performance a left join T_userinfo b on a.userId = b.id left join T_department c on b.DeptId = c.id
left join
(
 select userId, sum(finishcount) as PerformanceMonth from T_Performance where dates>= CONVERT( varchar(100),dateadd(dd,-day(getdate())+1,getdate()) ,111) 
  and dates < CONVERT( varchar(100),dateadd(dd,-day(dateadd(month,1,getdate()))+1,dateadd(month,1,getdate())) ,111)
 group by userId) d on a.userId = d.userId
 
 left join
(
 select userId, sum(finishcount) as PerformanceWeek from T_Performance where dates>=  @Date+Case When 1>=@@datefirst Then 1-@@datefirst Else 7+(1-@@datefirst) End 
  and dates < @Date+Case When 7>=@@datefirst Then 7-@@datefirst Else 7+(7-@@datefirst) End
 group by userId) e on a.userId = e.userId
 
 where b.id = {0} and CONVERT(varchar(100), a.dates, 111) = CONVERT(varchar(100), GETDATE(), 111) and a.Performance={1}", UserId, Performance);
            List<PerformancePanelModel> list = dal.QueryList<PerformancePanelModel>(Sql, new object[] { });
            if (list.Count > 0)
            {
                PerformancePanelModel model = list[0];
                if (model != null)
                {
                    bl_Date.Text = model.Now;
                    bl_Dept.Text = model.Dept;
                    bl_User.Text = model.UserName;
                    bl_Month.Text = model.PerformanceMonth.ToString();
                    bl_Week.Text = model.PerformanceWeek.ToString();
                    bl_Day.Text = model.PerformanceDay.ToString();
                }
            }
        }

        /// <summary>
        /// 初始化绩效
        /// </summary>
        /// <param name="perfor"></param>
        public void InitPerformance(Model.Performance perfor)
        {
            //绩效初始化
            int UserId = CommonLogin.CommonUser.Id;
            int Performanceid = Convert.ToInt32(perfor);
            string checkSql = string.Format(@"  if not exists
  (select * from T_Performance
  where  CONVERT(varchar(100), dates, 111) = CONVERT(varchar(100), GETDATE(), 111)
  and userId = {0} and Performance = {1})
  insert into T_Performance values ({2},CONVERT(varchar(100), GETDATE(), 111)  ,0,{3})", UserId, Performanceid, UserId, Performanceid);
            List<string> sqllist = new List<string>();
            sqllist.Add(checkSql);
            bool bs = dal.ExecuteTransaction(sqllist, null);
            if (!bs)
            {
                CCMessageBox.Show("绩效初始化失败,请及时与管理员联系");
                return;
            }
        }

        public StatisticsModel StatisticsByTime(string UserName, Model.Performance perfor,DateTime datestart,DateTime dateend,String deptid="8")
        {
            StatisticsModel ms = null;
            string sql = string.Format("select '1' as Item,  ISNULL(SUM(finishcount),0) as count from T_Performance where userId = (select id from T_Userinfo where username='{0}' and DeptId='{4}') and Performance ={1} and dates>='{2}' and dates<='{3}'"
                , UserName, Convert.ToInt32(perfor), datestart, dateend,deptid);
            List<StatisticsModel> list = dal.QueryList<StatisticsModel>(sql, new object[] { });
            ms = list.Count > 0 ? list[0] : new StatisticsModel();
            return ms;
        }
    }
}
