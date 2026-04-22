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
using System.Windows.Shapes;

namespace AsyncTaskManagerWpf
{
    /// <summary>
    /// CustomMessageBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public bool Result { get; private set; }
        //public CustomMessageBox( string message, string title, bool showCancel, CustomMessageBoxType type )
        //{
        //    InitializeComponent();

        //    MessageText.Text = message;
        //    TitleText.Text = title;

        //    if ( showCancel )
        //        CancelButton.Visibility = Visibility.Visible;

        //    SetIcon( type );
        //}
        //private void SetIcon( CustomMessageBoxType type )
        //{
        //    switch ( type ) {
        //        case CustomMessageBoxType.Success:
        //            IconText.Text = "✔";
        //            IconText.Foreground = Brushes.Green;
        //            break;

        //        case CustomMessageBoxType.Warning:
        //            IconText.Text = "⚠";
        //            IconText.Foreground = Brushes.Orange;
        //            break;

        //        case CustomMessageBoxType.Error:
        //            IconText.Text = "✖";
        //            IconText.Foreground = Brushes.Red;
        //            break;

        //        default:
        //            IconText.Text = "ℹ";
        //            IconText.Foreground = Brushes.Gray;
        //            break;
        //    }
        //}
        public CustomMessageBox( string message, string title = "알림", bool showCancel = false )
        {
            InitializeComponent();

            MessageText.Text = message;
            TitleText.Text = title;

            if ( showCancel ) {
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void Ok_Click( object sender, RoutedEventArgs e )
        {
            Result = true;
            Close();
            //Result = true;
            //DialogResult = true;
        }

        private void Cancel_Click( object sender, RoutedEventArgs e )
        {
            Result = false;
            Close();
            //Result = false;
            //DialogResult = false;
        }

        private void Close_Click( object sender, RoutedEventArgs e )
        {
            Result = false;
            Close();
            //Result = false;
            //DialogResult = false;
        }

        private void TitleBar_MouseDown( object sender, MouseButtonEventArgs e )
        {
            DragMove();
        }
        //public static bool Show( string message,
        //               string title = "알림",
        //               CustomMessageBoxType type = CustomMessageBoxType.Info )
        //{
        //    var msg = new CustomMessageBox( message, title, false, type );
        //    msg.Owner = Application.Current.MainWindow;

        //    msg.ShowDialog();
        //    return true;
        //}

        //public static bool ShowConfirm( string message,
        //                              string title = "확인",
        //                              CustomMessageBoxType type = CustomMessageBoxType.Warning )
        //{
        //    var msg = new CustomMessageBox( message, title, true, type );
        //    msg.Owner = Application.Current.MainWindow;

        //    return msg.ShowDialog() == true;
        //}
    }
}
