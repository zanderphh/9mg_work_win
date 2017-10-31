using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using _9M.Work.WPF_Main;

namespace _9M.Work.WPF_Main
{
    /// <summary>
    /// CCMessageBox显示的按钮类型
    /// </summary>
    public enum CCMessageBoxButton
    {
        OK = 0,
        OKCancel = 1,
        YesNo = 2,
        YesNoCancel = 3
    }

    /// <summary>
    /// CCMessageBox显示的图标类型
    /// </summary>
    public enum CCMessageBoxImage
    {
        None = 0,
        Error = 1,
        Question = 2,
        Warning = 3
    }

    /// <summary>
    /// 消息的重点显示按钮
    /// </summary>
    public enum CCMessageBoxDefaultButton
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 3,
        No = 4
    }

    /// <summary>
    /// 消息框的返回值
    /// </summary>
    public enum CCMessageBoxResult
    {
        //用户直接关闭了消息窗口
        None = 0,
        //用户点击确定按钮
        OK = 1,
        //用户点击取消按钮
        Cancel = 2,
        //用户点击是按钮
        Yes = 3,
        //用户点击否按钮
        No = 4
    }

    public class CCMessageBox
    {
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        public static CCMessageBoxResult Show(string cmessageBoxText)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            window.CMessageBoxText = cmessageBoxText;
            window.OKButtonVisibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    window.ShowDialog();
                }));
            return window.Result;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        /// <param name="caption">消息标题</param>
        public static CCMessageBoxResult Show(string cmessageBoxText, string caption)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            window.CMessageBoxText = cmessageBoxText;
            window.CMessageBoxTitle = caption;
            window.OKButtonVisibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        /// <param name="CCMessageBoxButton">消息框按钮</param>
        public static CCMessageBoxResult Show(string cmessageBoxText, CCMessageBoxButton CCMessageBoxButton)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            window.CMessageBoxText = cmessageBoxText;
            switch(CCMessageBoxButton)
            {
                case CCMessageBoxButton.OK:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.OKCancel:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNo:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNoCancel:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        /// <param name="caption">消息标题</param>
        /// <param name="CCMessageBoxButton">消息框按钮</param>
        public static CCMessageBoxResult Show(string cmessageBoxText, string caption, CCMessageBoxButton CCMessageBoxButton)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            window.CMessageBoxText = cmessageBoxText;
            window.CMessageBoxTitle = caption;
            switch (CCMessageBoxButton)
            {
                case CCMessageBoxButton.OK:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.OKCancel:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNo:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNoCancel:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        /// <param name="caption">消息标题</param>
        /// <param name="CCMessageBoxButton">消息框按钮</param>
        /// <param name="CCMessageBoxImage">消息框图标</param>
        /// <returns></returns>
        public static CCMessageBoxResult Show(string cmessageBoxText, string caption, CCMessageBoxButton CCMessageBoxButton, CCMessageBoxImage CCMessageBoxImage)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            
            window.CMessageBoxText = cmessageBoxText;
            window.CMessageBoxTitle = caption;
            switch (CCMessageBoxButton)
            {
                case CCMessageBoxButton.OK:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.OKCancel:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNo:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNoCancel:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="cmessageBoxText">消息内容</param>
        /// <param name="caption">消息标题</param>
        /// <param name="CCMessageBoxButton">消息框按钮</param>
        /// <param name="CCMessageBoxImage">消息框图标</param>
        /// <param name="CCMessageBoxDefaultButton">消息框默认按钮</param>
        /// <returns></returns>
        public static CCMessageBoxResult Show(string cmessageBoxText, string caption, CCMessageBoxButton CCMessageBoxButton, CCMessageBoxImage CCMessageBoxImage, CCMessageBoxDefaultButton CCMessageBoxDefaultButton)
        {
            CCMessageBoxWindow window = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window = new CCMessageBoxWindow();
            }));
            window.CMessageBoxText = cmessageBoxText;
            window.CMessageBoxTitle = caption;

            #region 按钮
            switch (CCMessageBoxButton)
            {
                case CCMessageBoxButton.OK:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.OKCancel:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNo:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        break;
                    }
                case CCMessageBoxButton.YesNoCancel:
                    {
                        window.YesButtonVisibility = Visibility.Visible;
                        window.NoButtonVisibility = Visibility.Visible;
                        window.CancelButtonVisibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        window.OKButtonVisibility = Visibility.Visible;
                        break;
                    }
            }
            #endregion

            #region 默认按钮
            switch(CCMessageBoxDefaultButton)
            {
                case CCMessageBoxDefaultButton.OK:
                    {
                        window.OKButtonStyle = ButtonStyle.NormalButtonStyle;
                        window.CancelButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.YesButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.NoButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        break;
                    }
                case CCMessageBoxDefaultButton.Cancel:
                    {
                        window.OKButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.CancelButtonStyle = ButtonStyle.NormalButtonStyle;
                        window.YesButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.NoButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        break;
                    }
                case CCMessageBoxDefaultButton.Yes:
                    {
                        window.OKButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.CancelButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.YesButtonStyle = ButtonStyle.NormalButtonStyle;
                        window.NoButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        break;
                    }
                case CCMessageBoxDefaultButton.No:
                    {
                        window.OKButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.CancelButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.YesButtonStyle = ButtonStyle.NotNormalButtonStyle;
                        window.NoButtonStyle = ButtonStyle.NormalButtonStyle;
                        break;
                    }
                case CCMessageBoxDefaultButton.None:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            #endregion

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                window.ShowDialog();
            }));
            return window.Result;
        }
    }
}
