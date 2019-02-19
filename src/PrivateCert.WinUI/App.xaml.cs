using System;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PrivateCert.CompositionRoot;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;
using PrivateCert.WinUI.Infrastructure;
using PrivateCert.WinUI.Windows;
using StructureMap;

namespace PrivateCert.WinUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        public static string MasterKeyDecrypted { get; set; }

        private static readonly Mutex Mutex = new Mutex(true, "MutexPrivateCertBy" + Environment.UserName);

        private IPrivateCertRepository privateCertRepository;

        private IContainer container;

        public void Dispose()
        {
            container?.Dispose();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                decimal erroId = 0;

                // O erro pode ter acontecido antes da criação do container.
                if (privateCertRepository != null)
                {
                    erroId = privateCertRepository.InsertError(new Log(e.Exception));
                }

                MessageBoxHelper.ShowErrorMessage(
                    "Infelizmente ocorreu um erro na aplicação." + Environment.NewLine + Environment.NewLine +
                    "Por favor, entre em contato com a equipe de suporte informando o código para facilitar a identificação do problema: " +
                    erroId);
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBoxHelper.ShowErrorMessage(
                        "Ocorreu um erro no sistema e ele será finalizado." + Environment.NewLine + Environment.NewLine +
                        "Por favor, entre em contato com a equipe de suporte informando a mensagem abaixo." +
                        Environment.NewLine + ex.Message);
                }
                finally
                {
                    Current.Shutdown(1);
                }
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only 
        // log the event, and inform the user about it. 
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                decimal erroId = 0;

                // O erro pode ter acontecido antes da criação do container.
                if (privateCertRepository != null)
                {
                    erroId = privateCertRepository.InsertError(new Log(e.ExceptionObject as Exception));
                }

                MessageBoxHelper.ShowErrorMessage(
                    "Infelizmente ocorreu um erro na aplicação." + Environment.NewLine + Environment.NewLine +
                    "Por favor, entre em contato com a equipe de suporte informando o código para facilitar a identificação do problema: " +
                    erroId);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBoxHelper.ShowErrorMessage(
                        "Ocorreu um erro desconhecido no sistema e ele será finalizado." + Environment.NewLine +
                        Environment.NewLine +
                        "Por favor, entre em contato com a equipe de suporte informando a mensagem abaixo." +
                        Environment.NewLine + exc.Message);
                }
                finally
                {
                    Current.Shutdown(1);
                }
            }
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (!Mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBoxHelper.ShowInfoMessage("Application is already open.");
                return;
            }

            try
            {
                InitializeComponent();

                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

                AutoMapperInitializer.Initialize();

                SqlIoC.InitializeBaseContainer();
                container = SqlIoC.GetNestedContainer();

                privateCertRepository = container.GetInstance<IPrivateCertRepository>();

                var page = container.GetInstance<MainWindow>();
                page.ShowInTaskbar = true;
                page.InitializeComponent();
                page.Show();
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }
    }
}