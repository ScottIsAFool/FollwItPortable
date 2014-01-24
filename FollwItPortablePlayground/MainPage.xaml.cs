using System.Windows;
using FollwItPortable;
using FollwItPortable.Model;

namespace FollwItPortablePlayground
{
    public partial class MainPage
    {
        private readonly FollwItClient _client;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _client = new FollwItClient("kQcST6PXNLSfE");
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Username.Text) || string.IsNullOrEmpty(Password.Password))
            {
                return;
            }

            var isAuthenticated = await _client.AuthenticateAsync(Username.Text, Password.Password);
            MessageBox.Show(isAuthenticated.ToString());
        }

        private async void OtherButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var response = await _client.GetUserStreamAsync();
            var response = await _client.MarkEpisodeAsWatchedAsync(4325893, 1, 1);
        }
    }
}