using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using StructureMap;

namespace PrivateCert.WinUI.UserControls
{
    public class BaseUserControl : UserControl, IDisposable
    {
        public event EventHandler Closed;

        public event EventHandler ClosedAndExitApplication;

        public IContainer IoCContainer { get; set; }

        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler handler = Closed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnClosedAndExitApplication(EventArgs e)
        {
            EventHandler handler = ClosedAndExitApplication;
            handler?.Invoke(this, e);
        }

        protected void Close()
        {
            this.Dispose();
            OnClosed(EventArgs.Empty);
        }

        protected void CloseAndExit()
        {
            this.Dispose();
            OnClosed(EventArgs.Empty);
        }

        public virtual void Dispose()
        {
            IoCContainer?.Dispose();
        }
    }
}
