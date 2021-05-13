using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppOfficina.Portable.Services
{
    public class Logger
    {
        private readonly SQLiteAsyncConnection _conn;
        public Logger(string dbPath)
        {
            _conn = new SQLiteAsyncConnection(dbPath);
            _conn.CreateTableAsync<Log>().Wait();
        }
        public async Task UpdateLogsSyncStatusAsync(bool status)
        {
            var logs = await _conn.Table<Log>().Where(w => !w.IsSynced).ToListAsync();
            logs.ForEach(l => l.IsSynced = true);
            await _conn.UpdateAllAsync(logs);
        }
        public async Task<List<Log>> GetLogsAsync()
        {
            return await _conn.Table<Log>().Where(w => !w.IsSynced).ToListAsync();
        }
        public async Task<int> LogInfo(string message, string traceLocation)
        {
            return await _conn.InsertAsync(new Log() { Message = message, LocationTrace = traceLocation, IsSynced = false, IsErrorLog = false, Date = DateTime.Now });
        }
        public async Task<int> LogError(string ex, string traceLocation)
        {
            return await _conn.InsertAsync(new Log() { Message = ex, LocationTrace = traceLocation, IsSynced = false, IsErrorLog = true, Date = DateTime.Now });
        }

        [Table("Log")]
        public class Log
        {
            [PrimaryKey]
            [AutoIncrement]
            public int Id { get; set; }
            public string Message { get; set; }
            public string LocationTrace { get; set; }
            public DateTime Date { get; set; }
            public bool IsErrorLog { get; set; }
            public bool IsSynced { get; set; }
        }
    }
}
