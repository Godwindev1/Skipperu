using AutoMapper;
using Skipperu.Data.Repositories;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Dtos.ErrorHandling;
using Skipperu.Dtos.RequestsInfo;
using Xunit.Sdk;

namespace Skipperu.Models.Requests
{
    public class RequestsMangerModel
    {
        private readonly RequestsRepo _repo;
        private readonly CollectionsRepo _collectionsRepo;

        public RequestsMangerModel(IRequestsRepo requestsRepository, ICollectionsRepo collectionsRepo)
        {
            _repo = (RequestsRepo)requestsRepository;
            _collectionsRepo = (CollectionsRepo)collectionsRepo;
        }

        public async Task<List<RequestDBstore>> GetAllRequestFromFolder(string FolderPath, string GlobalUserID)
        {
            var NormalizedPath = FolderPath.ToUpperInvariant();
            var Folder = await _collectionsRepo.GetAllByUserAsync(GlobalUserID);

            if(Folder == null || Folder.Count() == 0)
            {
                return null;
            }

            var FolderList = Folder.Where(prop => prop.FolderPathNormalized == NormalizedPath);

            if (FolderList == null)
            {
                return null;
            }

            var FolderID = FolderList .ToList().FirstOrDefault().FolderRootID;

       
            return (List<RequestDBstore>)await _repo.GetAllByFolderAsync(FolderID);
        }

        public async  Task<ResultMessage>  SaveRequestToFolder(RequestDBstore Request, string FolderPath, string GlobalUserID)
        {
            var NormalizedPath = FolderPath.ToUpperInvariant();
            var Folder = await _collectionsRepo.GetAllByUserAsync(GlobalUserID);

            if (Folder == null)
            {
                return new ResultMessage { Message = "User Does not Have these Folder", type = MessageTypes.NOTFOUND };
            }

            var FolderList = Folder.Where(prop => prop.FolderPathNormalized == NormalizedPath);

            if (FolderList == null)
            {
                return new ResultMessage { Message = "Folder Does not Exist", type = MessageTypes.NOTFOUND };
            }

            var FolderID = FolderList .ToList().FirstOrDefault().FolderRootID;

            RequestDBstore NewRequest = Request;
            NewRequest.ParentFolderID = FolderID;

            await _repo.AddAsync(NewRequest);
            await _repo.SaveAsync();

            return new ResultMessage { Message = "successful", type = MessageTypes.SUCCESFUL };
        }

        public async Task<ResultMessage> DeleteRequestFromFolder(string ID)
        {
            await _repo.DeleteAsync(ID);
            await _repo.SaveAsync();

            return new ResultMessage { Message = "successful", type = MessageTypes.SUCCESFUL };
        }

        public async Task<ResultMessage> ChangeRequestName(RequestDBstore Request)
        {
            if(Request == null)
            {
                return new ResultMessage { Message = "Request Input Is Null Application Error", type = MessageTypes.ERROR };
            }

            await _repo.UpdateAsync(Request);
            await _repo.SaveAsync();

            return new ResultMessage { Message = "Successful" };
        }
    }
}
