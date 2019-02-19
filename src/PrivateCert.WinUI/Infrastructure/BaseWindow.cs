using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StructureMap;

namespace PrivateCert.WinUI.Infrastructure
{
    public class BaseWindow : Window
    {
        public IContainer IoCContainer { get; set; }

        protected override void OnClosed(EventArgs e)
        {
            IoCContainer?.Dispose();
            base.OnClosed(e);
        }
    }
}
