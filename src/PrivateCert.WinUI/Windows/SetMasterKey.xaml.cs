using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for MasterKey.xaml
    /// </summary>
    public partial class SetMasterKey : BaseWindow
    {
        private readonly Lib.Features.SetMasterKey.CommandHandler setMasterKeyCommandHandler;

        public SetMasterKey(Lib.Features.SetMasterKey.CommandHandler setMasterKeyCommandHandler)
        {
            this.setMasterKeyCommandHandler = setMasterKeyCommandHandler;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.SetMasterKey.Command(pwdCurrentPassword.Password, pwdPassword.Password, pwdRetypePassword.Password);
            var result = setMasterKeyCommandHandler.Handle(command);
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

        private void SetMasterKey_OnLoaded(object sender, RoutedEventArgs e)
        {
            pwdCurrentPassword.Focus();
        }
    }
}
