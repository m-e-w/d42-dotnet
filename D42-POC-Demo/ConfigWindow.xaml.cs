using D42_POC_Demo.Classes;
using System.Windows;
using System.Windows.Media;

namespace D42_POC_Demo
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private D42_Helper _d42_Helper;
        public ConfigWindow(D42_Helper d42_Helper)
        {
            InitializeComponent();
            saveButton.Click += SaveButton_Click;
            exitButton.Click += ExitButton_Click;
            _d42_Helper = d42_Helper;
            //Set UI controls to current config in d42_api
            sslComboBox.ItemsSource = D42_API.SSL_CHOICES;
            httpComboBox.ItemsSource = D42_API.HTTP_CHOICES;
            userTextbox.Text = _d42_Helper.Username;
            urlTextbox.Text = _d42_Helper.Url;
            httpComboBox.SelectedIndex = _d42_Helper.Https;
            sslComboBox.SelectedIndex = _d42_Helper.Ssl;
            passwordTextbox.Password = "";

            if(_d42_Helper.Password.Length > 0)
            {
                statusLabel.Foreground = new SolidColorBrush(Colors.Green);
                statusLabel.Content = "Password Saved";
            }
            else
            {
                statusLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Only save the PW if it has been changed and isn't null

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(passwordTextbox.SecurePassword.Length > 0)
            {
                _d42_Helper.UpdateConfig(userTextbox.Text, passwordTextbox.SecurePassword, urlTextbox.Text, httpComboBox.SelectedIndex, sslComboBox.SelectedIndex);
                this.Close();
            }

            else if(_d42_Helper.Password.Length > 0)
            {
                _d42_Helper.UpdateConfig(userTextbox.Text, _d42_Helper.Password, urlTextbox.Text, httpComboBox.SelectedIndex, sslComboBox.SelectedIndex);
                this.Close();
            }
            else
            {
                passwordTextbox.Focus();
                statusLabel.Content = "Error: No password saved.";
            }

        }
    }
}
