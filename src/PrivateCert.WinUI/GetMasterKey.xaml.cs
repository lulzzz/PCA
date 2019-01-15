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
    public partial class GetMasterKey : Window
    {
        private readonly Lib.Features.GetMasterKey.QueryHandler getMasterKeyQueryHandler;

        public GetMasterKey(Lib.Features.GetMasterKey.QueryHandler getMasterKeyQueryHandler)
        {
            this.getMasterKeyQueryHandler = getMasterKeyQueryHandler;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            var query = new Lib.Features.GetMasterKey.Query(pwdPassword.Password);
            var result = getMasterKeyQueryHandler.Handle(query);
            if (!result.IsValid)
            {
                MessageBoxHelper.ShowErrorMessage(result.Errors);
                return;
            }

            App.MasterKeyDecrypted = pwdPassword.Password;

            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
