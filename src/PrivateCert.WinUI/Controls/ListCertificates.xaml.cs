using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediatR;
using Microsoft.Win32;
using PrivateCert.Lib.Features;
using PrivateCert.Lib.Interfaces;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Controls
{
    /// <summary>
    ///     Interaction logic for ListCertificates.xaml
    /// </summary>
    public partial class ListCertificates : UserControl
    {
        private readonly IMediator mediator;

        public ListCertificates(IMediator mediator)
        {
            this.mediator = mediator;
            InitializeComponent();
        }

        private async void ListCertificates_OnLoaded(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.ListCertificates.Query();
            var viewModel = await mediator.Send(query);
            if (!viewModel.ValidationResult.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(viewModel.ValidationResult.Errors);
                ((MainTabItem<ListCertificates>)Parent).Close();
            }

            DataContext = viewModel;
        }

        private async void DgCertificates_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var certificateId = ((Lib.Features.ListCertificates.CertificateVM) dgCertificates.SelectedItem).CertificateId;

            var query = new DownloadCertificate.Query(certificateId, App.MasterKeyDecrypted);
            var viewModel = await mediator.Send(query);
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