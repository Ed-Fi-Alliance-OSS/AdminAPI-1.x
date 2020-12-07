using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.OdsInstanceServices
{
    public interface IBulkRegisterOdsInstancesFiltrationService
    {
        IEnumerable<IRegisterOdsInstanceModel> FilteredRecords(
            IEnumerable<IRegisterOdsInstanceModel> dataRecords, ApiMode mode);
    }

    public class BulkRegisterOdsInstancesFiltrationService : IBulkRegisterOdsInstancesFiltrationService
    {
        private readonly AdminAppDbContext _database;
        private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

        public BulkRegisterOdsInstancesFiltrationService(AdminAppDbContext database,
            IDatabaseConnectionProvider databaseConnectionProvider)
        {
            _database = database;
            _databaseConnectionProvider = databaseConnectionProvider;
        }

        public IEnumerable<IRegisterOdsInstanceModel> FilteredRecords(
            IEnumerable<IRegisterOdsInstanceModel> dataRecords, ApiMode mode)
        {
            var newRows = dataRecords.Where(
                dataRecord => !_database.OdsInstanceRegistrations.Any(
                    previousRegister =>
                        previousRegister.Name == InferInstanceDatabaseName(dataRecord.NumericSuffix, mode)));

            return newRows;
        }

        private string InferInstanceDatabaseName(int? newInstanceNumericSuffix, ApiMode mode)
        {
            using (var connection = _databaseConnectionProvider.CreateNewConnection(newInstanceNumericSuffix.Value, mode))
                return connection.Database;
        }
    }
}
