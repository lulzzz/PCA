using System.Windows;
using System.Windows.Markup;
using PrivateCert.CompositionRoot;
using PrivateCert.Lib.Interfaces;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IPrivateCertRepository privateCertRepository;

        private readonly Lib.Features.CreateMasterKey.QueryHandler createMasterKeyQueryHandler;

        public MainWindow(
            IPrivateCertRepository privateCertRepository, Lib.Features.CreateMasterKey.QueryHandler createMasterKeyQueryHandler)
        {
            this.privateCertRepository = privateCertRepository;
            this.createMasterKeyQueryHandler = createMasterKeyQueryHandler;
            InitializeComponent();
        }

        private static void ShowPage<T>(bool dialog = false, string failureMessage = null, bool exitIfFailure = false)
            where T : BaseWindow, IComponentConnector
        {
            var container = SqlIoC.GetNestedContainer();
            var page = container.GetInstance<T>();
            page.IoCContainer = container;
            page.InitializeComponent();
            page.ShowInTaskbar = false;
            page.Owner = Application.Current.MainWindow;

            if (dialog)
            {
                var sucess = page.ShowDialog();
                if (exitIfFailure && (!sucess.HasValue || !sucess.Value))
                {
                    MessageBoxHelper.ShowInfoMessage(failureMessage);
                    Application.Current.Shutdown(0);
                }
            }
            else
            {
                page.Show();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void menuNewRoot_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<CreateRootCertificate>();
        }

        private void MenuNewIntermediate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuNewEndUser_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var masterKeyQueryResult = createMasterKeyQueryHandler.Handle(new Lib.Features.CreateMasterKey.Query());

            if (masterKeyQueryResult.IsValid)
            {
                ShowPage<CreateMasterKey>(true, "Application will be closed since master key was not defined.", true);
            }
            else
            {
                ShowPage<GetMasterKey>(true, "Application will be closed since master key was not informed.", true);
            }
        }

        private void MenuSetMasterKey_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<SetMasterKey>(true);
        }

        private void MenuList_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<ListCertificates>(true);
        }
    }
}