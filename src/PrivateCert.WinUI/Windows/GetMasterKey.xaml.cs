using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    public partial class GetMasterKey : BaseWindow
    {
        private readonly Lib.Features.GetMasterKey.QueryHandler getMasterKeyQueryHandler;

        public GetMasterKey(Lib.Features.GetMasterKey.QueryHandler getMasterKeyQueryHandler)
        {
            this.getMasterKeyQueryHandler = getMasterKeyQueryHandler;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.GetMasterKey.Query(pwdPassword.Password);
            var result = getMasterKeyQueryHandler.Handle(query);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            App.MasterKeyDecrypted = pwdPassword.Password;

            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void GetMasterKey_OnLoaded(object sender, RoutedEventArgs e)
        {
            pwdPassword.Focus();
        }
    }
}
