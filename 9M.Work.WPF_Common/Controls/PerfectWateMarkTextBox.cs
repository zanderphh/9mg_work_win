using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace _9M.Work.WPF_Common.Controls
{
    public class PerfectWateMarkTextBox : TextBox
    {
        private Label wateMarkLable;

        private ScrollViewer wateMarkScrollViewer;

        static PerfectWateMarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PerfectWateMarkTextBox), new FrameworkPropertyMetadata(typeof(PerfectWateMarkTextBox)));
        }

        public PerfectWateMarkTextBox()
        {
            this.Loaded += new RoutedEventHandler(PerfectWateMarkTextBox_Loaded);
        }

        void PerfectWateMarkTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        void PerfectWateMarkTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.wateMarkLable.Content = WateMark;
            }
            catch
            { }
        }

        void PerfectWateMarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.wateMarkLable.Visibility = Visibility.Hidden;
        }

        public string WateMark
        {
            get { return (string)GetValue(WateMarkProperty); }

            set { SetValue(WateMarkProperty, value); }
        }

        public static DependencyProperty WateMarkProperty =
            DependencyProperty.Register("WateMark", typeof(string), typeof(PerfectWateMarkTextBox), new UIPropertyMetadata("水印"));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.wateMarkLable = this.GetTemplateChild("TextPrompt") as Label;

            this.wateMarkScrollViewer = this.GetTemplateChild("PART_ContentHost") as ScrollViewer;
        }

        
    }
}
