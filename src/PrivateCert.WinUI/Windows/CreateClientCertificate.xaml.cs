using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for CreateClientCertificate.xaml
    /// </summary>
    public partial class CreateClientCertificate : BaseWindow
    {
        private readonly Lib.Features.CreateClientCertificate.CommandHandler createClientCertificateCommandHandler;

        public CreateClientCertificate(Lib.Features.CreateClientCertificate.CommandHandler createClientCertificateCommandHandler)
        {
            this.createClientCertificateCommandHandler = createClientCertificateCommandHandler;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateClientCertificate.Command((Lib.Features.CreateClientCertificate.ViewModel)DataContext, App.MasterKeyDecrypted);
            var result = createClientCertificateCommandHandler.Handle(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            MessageBoxHelper.ShowInfoMessage("Client certificate successfully created.");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Lib.Features.CreateClientCertificate.ViewModel()
            {
                IssuerName = "signer",
                Domain = "@domain.com",
                ExpirationDateInDays = 360
            };
            txtCountry.Focus();
        }
    }
}
