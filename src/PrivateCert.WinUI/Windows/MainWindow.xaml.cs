﻿using System;
using System.Windows;
using System.Windows.Markup;
using MediatR;
using PrivateCert.CompositionRoot;
using PrivateCert.Lib.Interfaces;
using PrivateCert.WinUI.Controls;
using PrivateCert.WinUI.Infrastructure;

namespace PrivateCert.WinUI.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMediator mediator;

        public MainWindow(IMediator mediator)
        {
            this.mediator = mediator;
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
            foreach (IDisposable item in mainTab.Items)
            {
                item.Dispose();
            }

            Application.Current.Shutdown(0);
        }

        private void MenuNewRoot_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<CreateRootCertificate>();
        }

        private void MenuNewIntermediate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var masterKeyQueryResult = mediator.Send(new Lib.Features.CreateMasterKey.Query());

            if (masterKeyQueryResult.Result.IsValid)
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
            ShowPage<SetMasterKey>();
        }

        private void MenuList_Click(object sender, RoutedEventArgs e)
        {
            ShowTab<Controls.ListCertificates>("Certificates", true);
        }

        private void ShowTab<T>(string title, bool singleTab) where T : IComponentConnector
        {
            if (singleTab)
            {
                foreach (var tabItem in mainTab.Items)
                {
                    if (!(tabItem is MainTabItem<T> item))
                    {
                        continue;
                    }

                    item.Focus();
                    return;
                }
            }

            var newTabItem = new MainTabItem<T> {Title = title};
            mainTab.Items.Add(newTabItem);
            newTabItem.Focus();
        }

        private void MenuNewServer_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<CreateServerCertificate>();
        }

        private void MenuNewClient_Click(object sender, RoutedEventArgs e)
        {
            ShowPage<CreateClientCertificate>();
        }
    }
}