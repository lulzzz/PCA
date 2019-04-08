using System.Windows;
using MediatR;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for CreateClientCertificate.xaml
    /// </summary>
    public partial class CreateClientCertificate : BaseWindow
    {
        private readonly IMediator mediator;

        public CreateClientCertificate(IMediator mediator)
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
            var command = new Lib.Features.CreateClientCertificate.Command((Lib.Features.CreateClientCertificate.ViewModel)DataContext, App.MasterKeyDecrypted);
            var result = await mediator.Send(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            MessageBoxHelper.ShowInfoMessage("Client certificate successfully created.");
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.CreateClientCertificate.Query();
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
