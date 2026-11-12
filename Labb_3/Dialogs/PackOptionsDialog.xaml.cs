using System.Windows;

namespace Labb_3.Dialogs
{
    /// <summary>
    /// Interaction logic for PackOptionsDialog.xaml
    /// </summary>
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {

                textBox.Focus();
                textBox.SelectAll();
            };
        }


    }
}
