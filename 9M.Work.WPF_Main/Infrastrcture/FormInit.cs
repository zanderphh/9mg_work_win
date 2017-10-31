using _9M.Work.WPF_Main.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _9M.Work.WPF_Main.Infrastrcture
{
    public class FormInit : Window
    {
        public static RightContentView RightView { get; set; }

        //  public static readonly string CloseStr = "   X";
        //打开窗体
        public void OpenView(string Title, object obj, bool HasCloseBtn)
        {
            //Title += CloseStr;
            TabControl control = RightView.sampleSelector;
            ItemCollection items = control.Items;
            bool b = true;
            TabItem SelectTab = null;

            foreach (var item in items)
            {
                TabItem tab = item as TabItem;
                StackPanel st = tab.Header as StackPanel;
                if ((st.Children[0] as TextBlock).Text.Equals(Title))
                {
                    SelectTab = tab;
                    b = false;
                    break;
                }
            }

            if (b) //没有此页就打开页面
            {
                StackPanel st = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock tabtitle = new TextBlock() { Text = Title, FontSize = 13, FontWeight = FontWeights.SemiBold, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                st.Children.Add(tabtitle);
                if (HasCloseBtn)
                {
                    Button closebutton = new Button() { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                    Style style = this.FindResource("BaseButtonCloseMenuStyle") as Style;
                    closebutton.Style = style;
                    closebutton.Margin = new Thickness(8, 0, 0, 0);
                    closebutton.Click += closebutton_Click;
                    st.Children.Add(closebutton);
                }
                TabItem t = new TabItem();
                t.Header = st;

                Border bs = new Border() { BorderThickness = new Thickness(1.5), BorderBrush = new SolidColorBrush(Colors.LightGray) };
                UserControl user = obj as UserControl;
                user.Margin = new Thickness(0, 2, 0, 0);
                bs.Child = user;
                t.Content = bs;
                control.Items.Add(t);
                t.IsSelected = true;
            }
            else //如果有的话就激活
            {
                SelectTab.IsSelected = true;
            }
            //设置头布的功能导航
            TopView.CommonTopView.NativeText.Text = "[ " + Title + " ]";
        }

        //关闭窗体
        private void closebutton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TabItem tab = (btn.Parent as StackPanel).Parent as TabItem;
            RightView.sampleSelector.Items.Remove(tab);
        }



        //打开模态窗
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UiElement">当前元素</param>
        /// <param name="DiaLogView">弹出窗体</param>
        /// <param name="HasMeny">是否允许弹出多个</param>
        public static void OpenDialog(FrameworkElement UiElement, UserControl DiaLogView, bool HasMeny)
        {
            MainShell window = Window.GetWindow(UiElement) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;
            if (!HasMeny && rightview.DialogPanel.Children.Count > 0)
            {
                return;
            }
            rightview.TuChen.Visibility = Visibility.Visible;
            rightview.DialogPanel.Children.Add(DiaLogView);
        }

        public static void OpenDialog(FrameworkElement UiElement, UserControl DiaLogView, bool HasMeny, int level)
        {
            MainShell window = Window.GetWindow(UiElement) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;

            StackPanel sp = new StackPanel();
            if (level.Equals(2))
            {
                sp = rightview.DialogPanel2;
            }
            else if (level.Equals(3))
            {
                sp = rightview.DialogPanel3;
            }

            if (!HasMeny && sp.Children.Count > 0)
            {
                return;
            }
            rightview.TuChen.Visibility = Visibility.Visible;
            sp.Children.Add(DiaLogView);
        }



        //打开模态窗
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UiElement">当前元素</param>
        /// <param name="DiaLogView">弹出窗体</param>
        /// <param name="HasMeny">是否允许弹出多个</param>
        //public static void OpenDialog(FrameworkElement UiElement, UserControl DiaLogView, bool HasMeny)
        //{
        //    Window window = Window.GetWindow(UiElement);
        //   // window.IsEnabled = false;
        //    StackPanel sp = new StackPanel() { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F000000")), Opacity = 1.0 };
        //    window.Content = sp;
        //    // <StackPanel   Opacity="1.0" Background="#7F000000"   Visibility="Collapsed" Name="TuChen" Margin="0,-65,0,0"></StackPanel>
        //    //window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080")); 
        //   // window.Opacity = 1.0;

        //}

        //关闭模态窗
        public static void CloseDialog(UserControl DiaLogView)
        {
            MainShell window = Window.GetWindow(DiaLogView) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;
            rightview.TuChen.Visibility = Visibility.Collapsed;
            rightview.DialogPanel.Children.Remove(DiaLogView);
        }

        public static void CloseDialog(UserControl DiaLogView, int level)
        {
            MainShell window = Window.GetWindow(DiaLogView) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;
            //rightview.TuChen.Visibility = Visibility.Collapsed;

            StackPanel sp = new StackPanel();
            if (level.Equals(2))
            {
                sp = rightview.DialogPanel2;
            }
            else if (level.Equals(3))
            {
                sp = rightview.DialogPanel3;
            }
            sp.Children.Remove(DiaLogView);
        }

        public static object FindFather(UserControl DiaLogView)
        {
            MainShell window = Window.GetWindow(DiaLogView) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;
            object obj = ((rightview.sampleSelector.SelectedItem as TabItem).Content as Border).Child;
            return obj;
        }


        public static object FindFather(UserControl DiaLogView,Type t)
        {

            Object obj = null;
            MainShell window = Window.GetWindow(DiaLogView) as MainShell;
            RightContentView rightview = window.MainContent.Content as RightContentView;

            List<StackPanel> sps = new List<StackPanel>();

            sps.Add(rightview.DialogPanel);
            sps.Add(rightview.DialogPanel2);

            foreach (StackPanel sp in sps)
            {
                System.Collections.IEnumerator ator = sp.Children.GetEnumerator();

                while (ator.MoveNext())
                {
                    if (ator.Current.GetType() == t)
                    {
                        obj = ator.Current;
                        break;
                    }
                }
            }

            return obj;
        }

    }
}
