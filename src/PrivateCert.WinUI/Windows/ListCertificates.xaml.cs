using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PrivateCert.Lib.Features;
using PrivateCert.Lib.Interfaces;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    ///     Interaction logic for ListCertificates.xaml
    /// </summary>
    public partial class ListCertificates : BaseWindow
    {
        private readonly Lib.Features.ListCertificates.QueryHandler listCertificateQueryHandler;

        private readonly DownloadCertificate.QueryHandler downloadCertificateQueryHandler;

        public ListCertificates(
            Lib.Features.ListCertificates.QueryHandler listCertificateQueryHandler, IPrivateCertRepository privateCertRepository,
            DownloadCertificate.QueryHandler downloadCertificateQueryHandler)
        {
            this.listCertificateQueryHandler = listCertificateQueryHandler;
            this.downloadCertificateQueryHandler = downloadCertificateQueryHandler;
            InitializeComponent();
        }

        private void ListCertificates_OnLoaded(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.ListCertificates.Query();
            var viewModel = listCertificateQueryHandler.Handle(query);
            if (!viewModel.ValidationResult.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(viewModel.ValidationResult.Errors);
                Close();
            }

            DataContext = viewModel;
        }

        private void DgCertificates_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var certificateId = ((Lib.Features.ListCertificates.CertificateVM) dgCertificates.SelectedItem).CertificateId;

            var query = new DownloadCertificate.Query(certificateId, App.MasterKeyDecrypted);
            var viewModel = downloadCertificateQueryHandler.Handle(query);
            if (!viewModel.ValidationResult.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(viewModel.ValidationResult.Errors);
                return;
            }

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = viewModel.FileNameSuggestion;
            saveFileDialog.Filter = viewModel.ExtensionFilter;
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, viewModel.CertificateData);
                MessageBoxHelper.ShowInfoMessage($"Certificate saved in {saveFileDialog.FileName}");
            }
        }
    }
}