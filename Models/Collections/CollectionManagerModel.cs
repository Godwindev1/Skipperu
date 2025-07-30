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

        public async Task<ResultMessage> CreateRootFolder(GlobalUser User, string FolderNameNormalized)
        {
            try
            {
                await collectionsRepo.AddAsync(new Collection { FolderNameNormalized = FolderNameNormalized, GlobalUserID = User.GlobalUserID, FolderPathNormalized = FolderNameNormalized });
                await collectionsRepo.SaveAsync();
                return new ResultMessage { Message = "Folder Created", type = MessageTypes.SUCCESFUL };
            }
            catch (Exception ex)
            {
                return new ResultMessage { Message = ex.Message, type = MessageTypes.ERROR };
            }

        }
        public async Task<ResultMessage> CreateFolder(GlobalUser User, string ParentFolderPath, string FolderNameNormalized, string FolderPathNormalized)
        {

            var Collection = (await collectionsRepo
                .GetAllByUserAsync(User.GlobalUserID))
                .Where(x => x.FolderPathNormalized == ParentFolderPath)
                .FirstOrDefault();

            if(Collection == null)
            {
                return new ResultMessage { Message = "Parent FOlder Does Not Exist", type = MessageTypes.NOTFOUND };
            }

            string ParentFolderID = Collection.FolderRootID;

       
            //TODO: HasParentFolder Property in DB
            var Folder = (await collectionsRepo.GetAllByParentFolderAsync(ParentFolderID))
                .Where(x => x.FolderNameNormalized == FolderNameNormalized )
                .FirstOrDefault();


            if (Folder == null)
            {
                var ParentCollection = await collectionsRepo.GetByIdAsync(ParentFolderID);

                if (ParentCollection != null)
                {
                    await collectionsRepo.AddAsync(
                        new Data.Requests.Collection { 
                            FolderNameNormalized = FolderNameNormalized, ParentFolderID = ParentFolderID, UserNav = ParentCollection.UserNav, FolderPathNormalized = FolderPathNormalized
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
            var ParentFolderItems = await collectionsRepo.GetAllByParentFolderAsync(ParentFolderID);

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
        
        public async Task<ResultMessage> DeleteFolder(string UserID, string FolderPathNormalized)
        {
            var Folder =  (await collectionsRepo.GetAllByUserAsync(UserID))
                .Where(x => x.FolderPathNormalized == FolderPathNormalized)
                .FirstOrDefault();
            
            if(Folder != null)
            {
                await collectionsRepo.DeleteAsync(Folder.FolderRootID);
                await DeleteChildren(Folder.FolderRootID);
                await collectionsRepo.SaveAsync();

                return new ResultMessage { Message = "Folder Deleted", type = MessageTypes.SUCCESFUL };
            }

            return new ResultMessage { Message = "Folder Does not Exist", type = MessageTypes.NOTFOUND };
        }

        public async Task<ResultMessage> ChangeFolderName(string UserID, string NewFolderName, string FolderPathNormalized, string newFolderPath)
        {
            var Folder = (await collectionsRepo.GetAllByUserAsync(UserID))
                .Where(x => x.FolderPathNormalized == FolderPathNormalized).FirstOrDefault();

            if (Folder != null)
            {
                Folder.FolderPathNormalized = newFolderPath;
                Folder.FolderNameNormalized = NewFolderName;
                await collectionsRepo.UpdateAsync(Folder);
                await collectionsRepo.SaveAsync();

                return new ResultMessage { Message = "Folder Name Updated", type = MessageTypes.SUCCESFUL };
            }

            return new ResultMessage { Message = "Folder Does not Exist", type = MessageTypes.NOTFOUND };
        }


        //TODO: template Like Class That Has Return Type As Well As ReturnMessage
        public async Task<IEnumerable<Collection>> GetAllSubFolders(string FolderNameNormalized, string GlobalUserID, string FolderPathNormalized )
        {
            if(await collectionsRepo.DoesFolderExist(GlobalUserID, FolderPathNormalized)) {

                var Root = (await collectionsRepo
                    .GetAllByUserAsync(GlobalUserID))
                    .Where(x=> x.FolderPathNormalized == FolderPathNormalized)
                    .FirstOrDefault();

                var subFolders = await collectionsRepo.GetAllByParentFolderAsync(Root.FolderRootID);
                return subFolders;
            }

            return null;

        }

        public async Task<IEnumerable<Collection>> GetAllRootFoldersForUser(string GobalUserID)
        {
            return (await collectionsRepo.GetAllByUserAsync(GobalUserID)).Where(x => x.ParentFolderID == null);
        }

    }
}
