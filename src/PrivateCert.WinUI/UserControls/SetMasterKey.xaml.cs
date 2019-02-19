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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.UserControls
{
    /// <summary>
    /// Interaction logic for SetMasterKey.xaml
    /// </summary>
    public partial class SetMasterKey : BaseUserControl
    {
        private readonly Lib.Features.SetMasterKey.CommandHandler setMasterKeyCommandHandler;

        public SetMasterKey(Lib.Features.SetMasterKey.CommandHandler setMasterKeyCommandHandler)
        {
            this.setMasterKeyCommandHandler = setMasterKeyCommandHandler;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var command = new Lib.Features.SetMasterKey.Command(pwdCurrentPassword.Password, pwdPassword.Password, pwdRetypePassword.Password);
            var result = setMasterKeyCommandHandler.Handle(command);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetMasterKey_OnLoaded(object sender, RoutedEventArgs e)
        {
            pwdCurrentPassword.Focus();
        }

    }
}
