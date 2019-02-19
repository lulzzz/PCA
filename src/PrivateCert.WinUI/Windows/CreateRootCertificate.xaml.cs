using System.Windows;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    /// Interaction logic for CreateRootCertificate.xaml
    /// </summary>
    public partial class CreateRootCertificate : BaseWindow
    {
        private readonly Lib.Features.CreateRootCertificate.CommandHandler createRootCertificateCommandHandler;

        public CreateRootCertificate(Lib.Features.CreateRootCertificate.CommandHandler createRootCertificateCommandHandler)
        {
            this.createRootCertificateCommandHandler = createRootCertificateCommandHandler;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateRootCertificate.Command((Lib.Features.CreateRootCertificate.ViewModel)DataContext, App.MasterKeyDecrypted);
            var result = createRootCertificateCommandHandler.Handle(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            MessageBoxHelper.ShowInfoMessage("Root certificate successfully created.");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Lib.Features.CreateRootCertificate.ViewModel()
            {
                Country = "BR",
                Organization = "Organization",
                OrganizationUnit = "Technology",
                SubjectName =  "My Enterprise Root Certificate Authority vX",
                ExpirationDateInYears = 10
            };
            txtCountry.Focus();
        }
    }
}
