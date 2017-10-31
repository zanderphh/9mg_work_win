using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    public interface BaseDialog : INavigationAware
    {

        DelegateCommand CancelCommand
        {
            get;
        }

        void CloseDialog();

        string Title { get; }
    }
}
