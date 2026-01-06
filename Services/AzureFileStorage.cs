
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Movie_App_MinimalApi.Services
{
    public class AzureFileStorage : IFileStorage
    {
        private string? az_connectionString;

        //purpose of this constructor is to get the azure storage connection string from appsettings.json using dependency injection
        public AzureFileStorage(IConfiguration config)
        {
           az_connectionString= config.GetConnectionString("azureconnectionstring")!;

        }
        public async Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route)) //check wheather route is null/empty
            {
                return;
            }
            var client = new BlobContainerClient(az_connectionString, container);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(route);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();



        }

        public async Task<string> Store(string container, IFormFile file)
        {
            //this blob container client is used to interact with the blob container in azure storage
            var client = new BlobContainerClient(az_connectionString, container);
            await client.CreateIfNotExistsAsync();

            client.SetAccessPolicy(PublicAccessType.Blob);//to set the access policy of the container to public so that the files can be accessed publicly


            //to get file extension:
            var file_extension = Path.GetExtension(file.FileName);
            var fileName =$"{Guid.NewGuid()}{file_extension}";//to create unique file name using guid.Guid stands for globally unique identifier
            var blob=client.GetBlobClient(fileName);//to get the blob client for the file to be uploaded.GetBlobClient method creates a reference to a blob in the container using the specified file name.

            //to find content type , we will use headers
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = file.ContentType;//simply we can say it is the content type of the file to be uploaded

            //now we will upload the file to azure blob storage
            await blob.UploadAsync(file.OpenReadStream(),blobHttpHeaders);
            //UploadAsync method uploads the file to the blob storage using the file stream and blob http headers.blob is simply the reference to the blob in the container

            //now we will return the url of the uploaded file
            return blob.Uri.ToString();

        }
    }
}
