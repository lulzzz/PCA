using System;

namespace PrivateCert.AzureDal.Model
{
    public class Log
    {
        public Exception Exception { get; set; }

        private string BuildErrorMessage(Exception excecao)
        {
            var message = string.Empty;

            message += "----- EXCEPTION MESSAGE ----- " + Environment.NewLine + BuildExceptionMessage(excecao) +
                        Environment.NewLine;
            message += "----- STACK TRACE ----- " + Environment.NewLine + BuildStackTraceMessage(excecao) + Environment.NewLine;

            return message;
        }

        private string BuildExceptionMessage(Exception exception)
        {
            var message = string.Empty;
            if (!string.IsNullOrEmpty(exception.Message))
            {
                message = exception.GetType() + ": " + exception.Message;
            }

            //if (exception.GetType() == typeof(DbEntityValidationException))
            //{
            //    var excecaoDeValidacao = (DbEntityValidationException) exception;
            //    foreach (var entidadeComErro in excecaoDeValidacao.EntityValidationErrors)
            //    {
            //        message += " | Object type: " + entidadeComErro.Entry.Entity;
            //        foreach (var validacao in entidadeComErro.ValidationErrors)
            //        {
            //            message += " | Error Property: " + validacao.PropertyName;
            //            message += " | Property Error Message: " + validacao.ErrorMessage;
            //        }
            //    }
            //}

            //if (exception.GetType() == typeof(SqlException))
            //{
            //    var excecaoDeValidacao = (SqlException) exception;
            //    if (!string.IsNullOrEmpty(excecaoDeValidacao.Procedure))
            //    {
            //        message += " | Procedure: " + excecaoDeValidacao.Procedure;
            //    }
            //}

            //if (exception.GetType() == typeof(UpdateException))
            //{
            //    var excecaoDeValidacao = (UpdateException) exception;
            //    foreach (var stateEntry in excecaoDeValidacao.StateEntries)
            //    {
            //        message += " | Container: " + stateEntry.EntityKey.EntityContainerName;
            //        message += " | Class: " + stateEntry.EntityKey.EntitySetName;
            //    }
            //}

            if (exception.InnerException != null)
            {
                message += Environment.NewLine + " ----- INNER MESSAGE ----- " + Environment.NewLine +
                            BuildExceptionMessage(exception.InnerException);
            }

            return message;
        }

        private string BuildStackTraceMessage(Exception exception)
        {
            var message = string.Empty;
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                message = exception.StackTrace;
            }

            if (exception.InnerException != null)
            {
                message += Environment.NewLine + " ----- INNER STACK TRACE ----- " + Environment.NewLine +
                            BuildStackTraceMessage(exception.InnerException);
            }

            return message;
        }

        public string GetMessage()
        {
            return BuildErrorMessage(Exception);
        }
    }
}