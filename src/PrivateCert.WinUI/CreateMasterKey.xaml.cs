using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PrivateCert.Lib.Interfaces;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI
{
    /// <summary>
    /// Interaction logic for MasterKey.xaml
    /// </summary>
    public partial class CreateMasterKey : Window
    {
        private readonly Lib.Features.CreateMasterKey.CommandHandler createMasterKeyCommandHandler;

        public CreateMasterKey(Lib.Features.CreateMasterKey.CommandHandler createMasterKeyCommandHandler)
        {
            this.createMasterKeyCommandHandler = createMasterKeyCommandHandler;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.CreateMasterKey.Command(pwdPassword.Password, pwdRetypePassword.Password);
            var result = createMasterKeyCommandHandler.Handle(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
