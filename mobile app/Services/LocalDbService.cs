using SQLite;
using mobile_app.Models;

namespace mobile_app.Services;

public class LocalDbService
{
    private const string DB_NAME = "demo_local_db.db3";
    private readonly SQLiteAsyncConnection _connection;

    public LocalDbService()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        // Creates the table if it doesn't exist
        _connection.CreateTableAsync<ReportModel>();
    }

    public async Task<List<ReportModel>> GetReportsAsync()
    {
        return await _connection.Table<ReportModel>().ToListAsync();
    }

    public async Task CreateReportAsync(ReportModel report)
    {
        await _connection.InsertAsync(report);
    }
}