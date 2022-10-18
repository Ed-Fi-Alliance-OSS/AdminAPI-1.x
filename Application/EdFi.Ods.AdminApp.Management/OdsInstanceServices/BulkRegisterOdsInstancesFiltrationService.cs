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
        private readonly IInferInstanceService _inferInstanceService;

        public BulkRegisterOdsInstancesFiltrationService(AdminAppDbContext database
            , IInferInstanceService inferInstanceService)
        {
            _database = database;
            _inferInstanceService = inferInstanceService;
        }

        public IEnumerable<IRegisterOdsInstanceModel> FilteredRecords(
            IEnumerable<IRegisterOdsInstanceModel> dataRecords, ApiMode mode)
        {
            var newRows = dataRecords.Where(
                dataRecord => !_database.OdsInstanceRegistrations.Any(
                    previousRegister =>
                        previousRegister.Name == _inferInstanceService.DatabaseName(dataRecord.NumericSuffix.Value, mode)));

            return newRows;
        }
    }
}
