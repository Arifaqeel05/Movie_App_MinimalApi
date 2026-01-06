
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;

namespace Movie_App_MinimalApi.Services
{
    /*Instead of writing a separate constructor method inside the class, we declare dependencies
     1. IWebHostEnvironment env: Gives us access to the server's file system, specifically the "wwwroot" folder.
    2. IHttpContextAccessor httpContextAccessor: Gives us access to the current web request 
    (e.g., is it HTTP or HTTPS? What is the domain name?).*/
    public class LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileStorage
    {
        // 'route' is the full URL of the image to be deleted and 'container' is the folder name (e.g., "movies").
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route)) 
                // 1. Validation: If the route string is null or empty, there is nothing to delete.
            {
                return Task.CompletedTask;
            }
            var fileName= Path.GetFileName(route);
            // Extract Filename: We have the full URL, but we only need the file name (e.g., "image.jpg").


            /*  Find Physical Path: We need to find where this file actually lives on the hard drive.
             env.WebRootPath -> The path to "wwwroot" on your computer (e.g., "C:\Projects\MyApp\wwwroot").
            Path.Combine -> Joins these strings safely into a valid Windows/Linux path.
             Result: "C:\Projects\MyApp\wwwroot\movies\image.jpg" */
            var fileDirectory = Path.Combine(env.WebRootPath, container, fileName);


            /*Check & Delete: Always check if a file exists before trying to delete it to avoid crashes.
             The 'File' class (System.IO) handles the actual deletion from the disk.*/
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);//get the file extension
            var fileName = $"{Guid.NewGuid()}{extension}";//create a unique file name using GUID
            var folder = Path.Combine(env.WebRootPath, container);
            //Target Folder: Determine where inside 'wwwroot' we want to save this.


            /*Create Folder if Missing: If the 'movies' folder doesn't exist yet, create it.
            'Directory' is a static class in System.IO for managing folders.*/
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            } 

            string route=Path.Combine(folder, fileName);
            //Full Save Path: The exact path including the new filename."C:\Projects\MyApp\wwwroot\movies\e8a9d1-23b1.jpg"


            /*
             Save the File:
           'using': This is a safety wrapper. It ensures that the 'MemoryStream' is destroyed 
            and cleared from RAM immediately after we are done with it (prevents memory leaks).
             */
            using (var ms = new MemoryStream()) 
            {
                await file.CopyToAsync(ms); // Copy the uploaded file (IFormFile) into the MemoryStream (RAM).
                var content = ms.ToArray();// Convert the stream into a byte array (raw data 0s and 1s).
                await File.WriteAllBytesAsync(route, content);// Write those bytes to the hard drive at the 'route' path.
            }

            //Build the Public URL: We need to give the frontend a link they can use to display the image.

           var scheme=httpContextAccessor.HttpContext!.Request.Scheme; //http or https
            var host=httpContextAccessor.HttpContext!.Request.Host;// Host: "localhost:5000" or "www.mywebsite.com"
            var url = $"{scheme}://{host}";

            //Final URL &Path Normalization:
            var urlFile = Path.Combine(url, container, fileName).Replace("\\", "/");
            return urlFile;


        }
    }
}

/*
 IWebHostEnvironment env: to get the web root path where we want to store the files OR it gives information about 
the environment the application is running in, such as development, staging, or production.

    HttpContextAccessor httpContextAccessor: to access the current HTTP context, which is useful for getting 
information about the current request, such as the URL scheme (http or https) and host name.
 */

