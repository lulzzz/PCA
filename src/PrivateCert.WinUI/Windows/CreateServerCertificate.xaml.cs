using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    public partial class CreateServerCertificate : BaseWindow
    {
        private readonly Lib.Features.CreateServerCertificate.CommandHandler createServerCertificateCommandHandler;
        private readonly Lib.Features.CreateServerCertificate.QueryHandler createServerCertificateQueryHandler;

        public CreateServerCertificate(Lib.Features.CreateServerCertificate.CommandHandler createServerCertificateCommandHandler, Lib.Features.CreateServerCertificate.QueryHandler createServerCertificateQueryHandler)
        {
            this.createServerCertificateCommandHandler = createServerCertificateCommandHandler;
            this.createServerCertificateQueryHandler = createServerCertificateQueryHandler;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateServerCertificate.Command((Lib.Features.CreateServerCertificate.ViewModel)DataContext, App.MasterKeyDecrypted);
            var result = createServerCertificateCommandHandler.Handle(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            MessageBoxHelper.ShowInfoMessage("Server certificate successfully created.");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.CreateServerCertificate.Query();
            var viewModel = createServerCertificateQueryHandler.Handle(query);
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
