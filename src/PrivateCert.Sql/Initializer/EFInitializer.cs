using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PrivateCert.Sql.Initializer
{
    public class EFInitializer : IDatabaseInitializer<PrivateCertContext>
    {
        public void InitializeDatabase(PrivateCertContext context)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            var assembly = Assembly.GetExecutingAssembly();

            if (!context.Database.Exists())
            {
                context.Database.Create();
                ExecuteScript(context, assembly, "PrivateCert.Sql.Initializer.Create Database Structure.sql");
            }

            // Check DB version
            var maxVersion = GetMaxVersion(context);

            var allScriptNames = assembly.GetManifestResourceNames()
                .Where(str => str.StartsWith("PrivateCert.Sql.Initializer.Scripts"))
                .OrderBy(c => c);
            foreach (var scriptName in allScriptNames)
            {
                var m = Regex.Match(scriptName, @"\._(?<version>\d\d\d)\.", RegexOptions.IgnoreCase);
                if (!m.Success)
                {
                    continue;
                }

                var scriptVersion = int.Parse(m.Groups["version"].Value);

                if (scriptVersion < maxVersion)
                {
                    continue;
                }

                if (ScriptAlreadyExecuted(context, scriptVersion, scriptName))
                {
                    continue;
                }

                try
                {
                    context.BeginTransaction();

                    CreateVersionIfNotExists(context, scriptVersion);
                    ExecuteScript(context, assembly, scriptName);
                    InsertScriptLog(context, scriptVersion, scriptName);

                    context.CommitTransaction();
                }
                catch
                {
                    context.RollbackTransaction();
                }
            }
        }

        private static int? GetMaxVersion(PrivateCertContext context)
        {
            return context.Database.SqlQuery<int?>("SELECT MAX(VersionID) FROM _DBVersions").SingleOrDefault();
        }

        private static void ExecuteScript(PrivateCertContext context, Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var sql = reader.ReadToEnd();
                    context.Database.ExecuteSqlCommand(sql);
                }
            }
        }

        private void InsertScriptLog(PrivateCertContext context, int scriptVersion, string scriptName)
        {
            context.Database.ExecuteSqlCommand(
                "INSERT INTO _DBScripts (VersionID, ScriptName, ExecutionDate) VALUES (@p0, @p1, GETDATE())", scriptVersion,
                scriptName);
        }

        private bool ScriptAlreadyExecuted(PrivateCertContext context, int scriptVersion, string scriptName)
        {
            return context.Database.SqlQuery<int?>(
                    "SELECT ScriptID FROM _DBScripts WHERE VersionID = @p0 AND ScriptName = @p1", scriptVersion, scriptName)
                .SingleOrDefault()
                .HasValue;
        }

        private void CreateVersionIfNotExists(PrivateCertContext context, int scriptVersion)
        {
            if (!context.Database.SqlQuery<int?>("SELECT VersionID FROM _DBVersions WHERE VersionID = @p0", scriptVersion)
                .SingleOrDefault()
                .HasValue)
            {
                context.Database.ExecuteSqlCommand("INSERT INTO _DBVersions VALUES (@p0, GETDATE())", scriptVersion);
            }
        }
    }
}