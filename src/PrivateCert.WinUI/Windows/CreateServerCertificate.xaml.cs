using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    public partial class CreateServerCertificate : BaseWindow
    {
        private readonly Lib.Features.CreateServerCertificate.CommandHandler createServerCertificateCommandHandler;

        public CreateServerCertificate(Lib.Features.CreateServerCertificate.CommandHandler createServerCertificateCommandHandler)
        {
            this.createServerCertificateCommandHandler = createServerCertificateCommandHandler;
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
            DataContext = new Lib.Features.CreateServerCertificate.ViewModel()
            {
                IssuerName = "serverhost.domain.com ou *.domain.com",
                ExpirationDateInDays = 360
            };
            txtCountry.Focus();
        }
    }
}
