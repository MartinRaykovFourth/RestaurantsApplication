using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;

namespace RestaurantsApplication.Services.RecordValidator
{
    public interface IRecordValidator
    {
        public void ValidateRecords(RequestCopyDTO req, ref bool isFailed, Dictionary<string, int> errorsDictionary);
    }
}
