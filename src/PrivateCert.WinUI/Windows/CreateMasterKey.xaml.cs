using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for MasterKey.xaml
    /// </summary>
    public partial class CreateMasterKey : BaseWindow
    {
        private readonly Lib.Features.CreateMasterKey.CommandHandler createMasterKeyCommandHandler;

        public CreateMasterKey(Lib.Features.CreateMasterKey.CommandHandler createMasterKeyCommandHandler)
        {
            this.createMasterKeyCommandHandler = createMasterKeyCommandHandler;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateMasterKey.Command(pwdPassword.Password, pwdRetypePassword.Password);
            var result = createMasterKeyCommandHandler.Handle(command);
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
            this.Close();
        }

        private void CreateMasterKey_OnLoaded(object sender, RoutedEventArgs e)
        {
            pwdPassword.Focus();
        }
    }
}
