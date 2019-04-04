using System.Windows;
using MediatR;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for MasterKey.xaml
    /// </summary>
    public partial class SetMasterKey : BaseWindow
    {
        private readonly IMediator mediator;

        public SetMasterKey(IMediator mediator)
        {
            this.mediator = mediator;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.SetMasterKey.Command(pwdCurrentPassword.Password, pwdPassword.Password, pwdRetypePassword.Password);
            var result = await mediator.Send(command);
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
