using System.Windows;

namespace Labb_3.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportTriviaDbDialog.xaml
    /// </summary>
    public partial class ImportTriviaDbDialog : Window
    {
        public ImportTriviaDbDialog()
        {
            InitializeComponent();

        }

        private void Button_Click_Trivia(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


    }
}
