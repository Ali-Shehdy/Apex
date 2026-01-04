using Microsoft.EntityFrameworkCore;

namespace Apex.Events.Data
{
    public static class DbSchemaInitializer
    {
        public static void EnsureEventCancellationColumn(EventsDbContext dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();
            var shouldClose = connection.State == System.Data.ConnectionState.Closed;

            if (shouldClose)
            {
                connection.Open();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(Events);";
                using var reader = command.ExecuteReader();

                var hasColumn = false;
                while (reader.Read())
                {
                    var columnName = reader.GetString(1);
                    if (string.Equals(columnName, "IsCancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        hasColumn = true;
                        break;
                    }
                }

                if (!hasColumn)
                {
                    using var alterCommand = connection.CreateCommand();
                    alterCommand.CommandText = "ALTER TABLE Events ADD COLUMN IsCancelled INTEGER NOT NULL DEFAULT 0;";
                    alterCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                if (shouldClose)
                {
                    connection.Close();
                }
            }
        }
    }
}