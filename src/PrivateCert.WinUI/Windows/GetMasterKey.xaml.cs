using System.Windows;
using MediatR;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    public partial class GetMasterKey : BaseWindow
    {
        private readonly IMediator mediator;

        public GetMasterKey(IMediator mediator)
        {
            this.mediator = mediator;
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.GetMasterKey.Query(pwdPassword.Password);
            var result = await mediator.Send(query);
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
