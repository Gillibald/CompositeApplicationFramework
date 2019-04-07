#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CompositeApplicationFramework.Utility;

#endregion

namespace CompositeApplicationFramework.DataAccess
{
    public static class MongoDbHelper
    {
        public static MongoDbConnector GetServer()
        {
            var lMongoConnector = new MongoDbConnector();

            var processId = SpawnNewProcess(lMongoConnector.DbName, lMongoConnector.Port);

            lMongoConnector.ProcessId = processId;

            if (processId == -1) return lMongoConnector;

            var job = new JobObject();

            job.AddProcess(processId);

            return lMongoConnector;
        }

        public static MongoDbConnector GetServer(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentNullException(dbName);
            }

            var lMongoConnector = new MongoDbConnector(dbName);

            var processId = SpawnNewProcess(lMongoConnector.DbName, lMongoConnector.Port);

            lMongoConnector.ProcessId = processId;

            if (processId == -1) return null;

            var job = new JobObject();

            job.AddProcess(processId);

            return lMongoConnector;
        }

        public static MongoDbConnector GetServer(string dbName, string hostname, int port)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentNullException(dbName);
            }

            var lMongoConnector = new MongoDbConnector(dbName, hostname, port);

            var processId = SpawnNewProcess(lMongoConnector.DbName, lMongoConnector.Port);

            lMongoConnector.ProcessId = processId;

            if (processId == -1) return null;

            var job = new JobObject();

            job.AddProcess(processId);

            return lMongoConnector;
        }

        private static int SpawnNewProcess(string dbName, int port)
        {
            var lProjectsFolder = GetProjectsFolder();

            var lDbFolder = Path.Combine(lProjectsFolder, $"{dbName}\\db\\");

            var lLogFolder = Path.Combine(lProjectsFolder, $"{dbName}\\log\\");

            if (!Directory.Exists(lDbFolder))
            {
                Directory.CreateDirectory(lDbFolder);
            }

            if (!Directory.Exists(lLogFolder))
            {
                Directory.CreateDirectory(lLogFolder);
            }

            var lExecutionFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (lExecutionFolder == null) return -1;

            var lMongoDPath = Path.Combine(lExecutionFolder, "Mongo-Bin\\mongod.exe");

            var lockFile = Path.Combine(lDbFolder, "mongod.lock");

            if (File.Exists(lockFile))
            {
                File.Delete(lockFile);
            }

            var args = $"--dbpath {lDbFolder} --port {port} --logpath {lLogFolder}\\logfile";

            var startInfo = new ProcessStartInfo
            {
                UserName = string.Empty,
                Password = null,
                Domain = null,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = lMongoDPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args
            };

            var exeProcess = Process.Start(startInfo);

            if (exeProcess != null) return exeProcess.Id;

            return -1;
        }

        private static string GetProjectsFolder()
        {
            var lAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var assemblyName = Assembly.GetEntryAssembly();

            var lDbFolder = Path.Combine(lAppDataFolder, $"{assemblyName.GetName().Name}\\");

            if (!Directory.Exists(lDbFolder))
            {
                Directory.CreateDirectory(lDbFolder);
            }

            return lDbFolder;
        }
    }
}