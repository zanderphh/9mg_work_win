using _9M.Work.Utility;
using _9M.Work.WPF_Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    /// <summary>
    /// 快递公司
    /// </summary>
    public class ExpressCompanyVal : ObservableCollection<EnumEntity>
    {
        public ExpressCompanyVal()
        {
            EnumHelper.GetEnumList(typeof(ExpressCompanyEnum)).ForEach(a => this.Add(a));
        }
    }

    /// <summary>
    /// 退货原因
    /// </summary>
    public class RefundReasonVal : ObservableCollection<EnumEntity>
    {
        public RefundReasonVal()
        {
            EnumHelper.GetEnumList(typeof(RefundReason)).ForEach(a => this.Add(a));
        }
    }


    public class ColorFlag : ObservableCollection<EnumEntity> 
    {
        public ColorFlag()
        {
            EnumHelper.GetEnumList(typeof(FlagColorEnume)).ForEach(a => this.Add(a));
        }
    }


    public class JSDZCheckStatusVal : ObservableCollection<EnumEntity>
    {
        public JSDZCheckStatusVal()
        {
            EnumHelper.GetEnumList(typeof(JSDZ_CheckStatusEnum)).ForEach(a => this.Add(a));
        }
    }


    
    public class JSDZRegisterTypeVal : ObservableCollection<EnumEntity>
    {
        public JSDZRegisterTypeVal()
        {
            EnumHelper.GetEnumList(typeof(JSDZ_RegisterTypeEnum)).ForEach(a => this.Add(a));
        }
    }
    
}
