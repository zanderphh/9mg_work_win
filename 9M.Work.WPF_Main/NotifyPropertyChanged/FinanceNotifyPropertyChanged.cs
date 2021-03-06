﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.WPF_Main.Views.FinanceRefund
{
    public partial class Finance : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            try
            {
                if (PropertyChanged != null && !string.IsNullOrEmpty(propName))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
