using Skipperu.Data.Repositories;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Dtos.ErrorHandling;
using Xunit.Sdk;

namespace Skipperu.Models.Collections
{
    public class CollectionManagerModel
    {
        private readonly CollectionsRepo collectionsRepo;

        public CollectionManagerModel (ICollectionsRepo Repo)
        {
            collectionsRepo = (CollectionsRepo)Repo;
        }

        public async Task<ResultMessage> CreateRootFolder(GlobalUser User, string FolderName)
        {
            try
            {
                await collectionsRepo.AddAsync(new Data.Requests.Collection { FolderName = FolderName, UserNav = User, FolderPath = FolderName });
                await collectionsRepo.SaveAsync();
                return new ResultMessage { Message = "Folder Created", type = MessageTypes.SUCCESFUL };
            }
            catch (Exception ex)
            {
                return new ResultMessage { Message = ex.Message, type = MessageTypes.ERROR };
            }

        }
        public async Task<ResultMessage> CreateFolder(GlobalUser User, string ParentFolderPath, string FolderName, string FolderPath)
        {

            var Collection = (await collectionsRepo.
            GetAllByUserAsync(User.GlobalUserID)).Where(x => x.FolderPath == ParentFolderPath).FirstOrDefault();


            string ParentFolderID = Collection.FolderRootID;

       
            //TODO: HasParentFolder Property in DB
            var Folder = (await collectionsRepo.GetAllByParentFolderAsync(ParentFolderID))
                .Where(x => x.FolderName == FolderName ).FirstOrDefault();


            if (Folder == null)
            {
                var ParentCollection = await collectionsRepo.GetByIdAsync(ParentFolderID);

                if (ParentCollection != null)
                {
                    await collectionsRepo.AddAsync(
                        new Data.Requests.Collection { 
                            FolderName = FolderName, ParentFolderID = ParentFolderID, UserNav = ParentCollection.UserNav, FolderPath = FolderPath
                        }
                    );

                    await collectionsRepo.SaveAsync();

                    return new ResultMessage { Message = "Folder Created", type = MessageTypes.SUCCESFUL };
                }
                else
                {
                    return new ResultMessage { Message = "Parent Folder Does Not Exist Yet", type = MessageTypes.ERROR };
                }

            }
            else
            {
                return new ResultMessage { Message = "Cant Create Duplicate Folders", type = MessageTypes.WARNING };
            }



        }
        
        private async Task DeleteChildren(string ParentFolderID)
        {
            var ParentFolderItems = (await collectionsRepo.GetAllByParentFolderAsync(ParentFolderID));

            if(!ParentFolderItems.Any())
            {
                return;
            }
            else
            {
                foreach(Collection Item in ParentFolderItems)
                {
                    await collectionsRepo.DeleteAsync(Item.FolderRootID);
                    await DeleteChildren(Item.FolderRootID);
                }
            }
        }
        
        public async Task<ResultMessage> DeleteFolder(string UserID, string FolderName, string FolderPath)
        {
            var Folder =  (await collectionsRepo.GetAllByUserAsync(UserID))
                .Where(x => x.FolderPath == FolderPath).FirstOrDefault();
            
            if(Folder != null)
            {
                await collectionsRepo.DeleteAsync(Folder.FolderRootID);
                await DeleteChildren(Folder.FolderRootID);
                await collectionsRepo.SaveAsync();

                return new ResultMessage { Message = "Folder Deleted", type = MessageTypes.SUCCESFUL };
            }

            return new ResultMessage { Message = "Folder Does not Exist", type = MessageTypes.NOTFOUND };
        }

        public async Task<ResultMessage> ChangeFolderName(string UserID, string FolderName, string NewFolderName, string ParentID)
        {
            var Folder = (await collectionsRepo.GetAllByParentFolderAsync(ParentID))
                .Where(x => x.FolderName == FolderName && x.GlobalUserID == UserID).FirstOrDefault();

            if (Folder != null)
            {
                Folder.FolderName = NewFolderName;
                await collectionsRepo.UpdateAsync(Folder);
                await collectionsRepo.SaveAsync();

                return new ResultMessage { Message = "Folder Name Updated", type = MessageTypes.SUCCESFUL };
            }

            return new ResultMessage { Message = "Folder Does not Exist", type = MessageTypes.NOTFOUND };
        }

        //TODO: From return Messages
        public async Task<IEnumerable<Collection>> GetAllSubFolders(string FolderName, string GlobalUserID, string? ParentID )
        {
            //TODO: folderexists Check in Repo
            if(ParentID == null)
            {
                var CurrentFolder =  (await collectionsRepo.GetAllByUserAsync(GlobalUserID))
                    .Where(x => x.FolderName == FolderName && x.ParentFolderID == null);

                var subFolders = await collectionsRepo.GetAllByParentFolderAsync(CurrentFolder.FirstOrDefault().FolderRootID);
                return subFolders;
            }
            else
            {
                var CurrentFolder =  (await collectionsRepo.GetAllByParentFolderAsync(ParentID))
                    .Where(x => x.FolderName == FolderName);

                var subFolders = await collectionsRepo.GetAllByParentFolderAsync(CurrentFolder.FirstOrDefault().FolderRootID);
                return subFolders;
            }

        }

        public async Task<IEnumerable<Collection>> GetAllRootFoldersForUser(string GobalUserID)
        {
            return (await collectionsRepo.GetAllByUserAsync(GobalUserID)).Where(x => x.ParentFolderID == null);
        }

    }
}
