using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore_2_webapi.Models;
using System.IO;
using DriveDotNet.Models;
using DriveDotNet.Service;
using Microsoft.AspNetCore.Authorization;

namespace aspnetcore_2_webapi.Controllers
{
 //   [Authorize]
    [Route("api/[controller]/[action]/{id=<your-default-folder-id>}")]
    public class FilesController : Controller
    {
        Driver service;
        string rootFolderId = "<your-folder-id>";

        public string GetApplicationRoot()
        {
          return "";
        }

        public FilesController()
        {
            var rootDir  = GetApplicationRoot();
            service = new Driver(rootDir);
        }

        [HttpGet]
        [ResponseCache(Duration = 60*60, Location = ResponseCacheLocation.Any)]
        public FileResultContainer Index(){
            return List(rootFolderId);
        }

        [HttpGet]
        [ResponseCache(Duration = 60*60, Location = ResponseCacheLocation.Any,VaryByQueryKeys = new string[]{"*"})]
        public FileResultContainer List(string id)
        {
            id = string.IsNullOrWhiteSpace(id)? rootFolderId : id ;
      
            var files = new FileResultContainer();
            
            try{
                files = service.GetFiles(id,"root");
            }catch(Exception){
                // return Error();
            }

            return files;
        }

        [HttpGet]
        [ResponseCache(Duration = 60*60, Location = ResponseCacheLocation.Any,VaryByQueryKeys = new string[]{"*"})]
        public FileResultContainer Search(string id,string searchString)
        {
            var files = new FileResultContainer();
            
            try{
                files = service.SearchFiles(id,searchString);
            }catch(Exception){
                // return Error();
            }

            return files;
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Authorize("DownloadPolicy")]
        public async Task<IActionResult> Download(string id,string fileType){
            Stream stream = await service.AsyncDownload(id);
            if(stream != null){ 
                return File(stream, $"{fileType}"); // returns a FileStreamResult
            }
            else{
                return Error();
            }
        }

        async Task PutTaskDelay(int milliseconds)
        {
            await Task.Delay(milliseconds);
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
