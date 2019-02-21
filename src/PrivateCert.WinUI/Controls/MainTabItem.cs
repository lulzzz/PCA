using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using PrivateCert.CompositionRoot;
using StructureMap;

namespace PrivateCert.WinUI.Controls
{
    public class MainTabItem<T> : TabItem, IDisposable where T : IComponentConnector
    {
        private readonly CloseableHeader tabHeader;

        private readonly IContainer ioCContainer;

        public MainTabItem()
        {
            tabHeader = new CloseableHeader();
            Header = tabHeader;

            tabHeader.btnClose.MouseEnter += BtnClose_MouseEnter;
            tabHeader.btnClose.MouseLeave += BtnClose_MouseLeave;
            tabHeader.btnClose.Click += BtnClose_Click;
            tabHeader.lblTitle.SizeChanged += LblTitle_SizeChanged;

            ioCContainer = SqlIoC.GetNestedContainer();
            var baseUserControl = ioCContainer.GetInstance<T>();
            baseUserControl.InitializeComponent();

            Content = baseUserControl;
        }

        public string Title
        {
            get => ((CloseableHeader) Header).lblTitle.Content.ToString();
            set => ((CloseableHeader) Header).lblTitle.Content = value;
        }

        public void Close()
        {
            Dispose();
            ((TabControl) Parent).Items.Remove(this);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader) Header).btnClose.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeader) Header).btnClose.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader) Header).btnClose.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsSelected)
            {
                ((CloseableHeader) Header).btnClose.Visibility = Visibility.Hidden;
            }
        }

        private void BtnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader) Header).btnClose.Foreground = Brushes.Red;
        }

        private void BtnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader) Header).btnClose.Foreground = Brushes.Black;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LblTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader) Header).btnClose.Margin = new Thickness(
                ((CloseableHeader) Header).lblTitle.ActualWidth + 5, 3, 4, 0);
        }

        public void Dispose()
        {
            ioCContainer?.Dispose();
            tabHeader.btnClose.MouseEnter -= BtnClose_MouseEnter;
            tabHeader.btnClose.MouseLeave -= BtnClose_MouseLeave;
            tabHeader.btnClose.Click -= BtnClose_Click;
            tabHeader.lblTitle.SizeChanged -= LblTitle_SizeChanged;
        }
    }
}