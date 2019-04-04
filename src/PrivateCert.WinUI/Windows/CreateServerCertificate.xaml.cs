using System.Windows;
using MediatR;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    public partial class CreateServerCertificate : BaseWindow
    {
        private readonly IMediator mediator;

        public CreateServerCertificate(IMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateServerCertificate.Command((Lib.Features.CreateServerCertificate.ViewModel)DataContext, App.MasterKeyDecrypted);
            var result = await mediator.Send(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            MessageBoxHelper.ShowInfoMessage("Server certificate successfully created.");
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.CreateServerCertificate.Query();
            var viewModel = await mediator.Send(query);
            if (!viewModel.ValidationResult.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(viewModel.ValidationResult.Errors);
                this.Close();
            }

            DataContext = viewModel;
            txtCountry.Focus();
        }
    }
}
