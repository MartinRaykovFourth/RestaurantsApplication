using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;

namespace RestaurantsApplication.Repositories.Contracts
{
    public interface IRequestRepository
    {
        public bool PendingRequestsExist();
        public void Add(RequestCopyDTO requestDTO, List<RecordCopyDTO> recordDTOs);
        public Task<List<RequestCopyDTO>> GetRequestsAndSetStatus();
        public Task SaveChangesAsync();
        public Task ChangeRequestStatusAsync(int requestId, string status);
        public Task SetFailMessageAsync(int requestId, string failMessage);
    }
}
