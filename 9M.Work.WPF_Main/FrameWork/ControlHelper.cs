using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _9M.Work.WPF_Main.FrameWork
{
    public class ControlHelper
    {
        /// <summary>
        /// 得到DATAGRID的一行
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static DataGridRow GetRow(DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>
        /// 得到DATAGRID的 TEMPLATEGRID的控件
        /// </summary>
        /// <param name="myDataGrid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public static object FindGridTemplateControl(System.Windows.Controls.DataGrid myDataGrid, int columnIndex, int rowIndex, string controlName)
        {
            FrameworkElement item = myDataGrid.Columns[columnIndex].GetCellContent(myDataGrid.Items[rowIndex]);
            DataGridTemplateColumn temp = (myDataGrid.Columns[columnIndex] as DataGridTemplateColumn);
            return temp.CellTemplate.FindName(controlName, item);
        }

        /// <summary>
        /// 得到DATAGRID的一个单元格组件
        /// </summary>
        /// <param name="myDataGrid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static object FindGridControl(System.Windows.Controls.DataGrid myDataGrid, int columnIndex, int rowIndex)
        {
            return (myDataGrid.Columns[columnIndex].GetCellContent(myDataGrid.Items[rowIndex]));
        }
    }
}
