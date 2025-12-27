namespace Movie_App_MinimalApi.Services
{
    public interface IFileStorage
    {
        /*
         purpose of creating this interface is to define the contract for file storage 
        operations such as storing, deleting, and editing files. This abstraction allows for different implementations of
        file storage like local storage, cloud storage, etc., without changing the code that uses these operations.

        Store method: This method is responsible for storing a file in a specified container (folder) and returns the URL of the stored file.
        Delete method: This method deletes a file from a specified container based on the provided URL (route).
        Edit method: This method combines the delete and store operations to replace an existing file with a new one. It first deletes the old file using its URL and then
        stores the new file in the specified container, returning the URL of the new file.
         */


        Task<string>Store(string container, IFormFile file);
        //returns the url of the stored file , container is the folder name and file is the file to be stored

        Task Delete(string? route, string container); //route is the url of the file to be deleted, container is the folder name
        async Task<string> Edit(string container, IFormFile file, string? route)
        {
            await Delete(route, container);
            //delete the old file, route is the url of the old file and container is the folder name
            
            return await Store(container, file);
            //store the new file and return the url of the new file.it ask the store method to store the new file in the container(folder)
        }
    }
}
/*
 in future we can use it to store files in cloud storage like azure blob storage,aws s3 etc
by creating a new class that implements this interface, use use to store pdf files,images,videos etc
 */