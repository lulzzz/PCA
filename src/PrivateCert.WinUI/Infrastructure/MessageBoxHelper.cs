using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FluentValidation.Results;
using PrivateCert.WinUI.Properties;

namespace PrivateCert.WinUI.Infrastructure
{
    public static class MessageBoxHelper
    {
        public static MessageBoxResult ShowErrorMessage(IList<ValidationFailure> failures)
        {
            var messages = string.Join(System.Environment.NewLine, failures.Select(c => " - " + c.ErrorMessage));
            return ShowMessage($"Error(s):{System.Environment.NewLine}{messages}", MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowErrorMessage(string message)
        {
            return ShowMessage(message, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowWarningMessage(string message)
        {
            return ShowMessage(message, MessageBoxImage.Warning);
        }

        public static MessageBoxResult ShowMessage(string message, MessageBoxImage icon)
        {
            return ShowMessage(message, MessageBoxButton.OK, icon);
        }

        public static MessageBoxResult ShowMessage(string message, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(message, "Private Certificate Manager", button, icon);
        }

        public static MessageBoxResult ShowInfoMessage(string message)
        {
            return ShowMessage(message, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowQuestionMessage(string message)
        {
            return ShowMessage(message, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
