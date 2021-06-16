using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DGN.Services
{
    public class ImagesService
    {
        private readonly string IMAGES_LOCATION = "wwwroot/images/";
        public readonly string CLIENT_IMAGES_LOCATION = "/images/";

        public async Task<bool> UploadImage(IFormFile img, string fileName)
        {
            if (img.Length <= 0) 
            {
                return false;
            }
            try
            {
                using (var stream = System.IO.File.Create(IMAGES_LOCATION + fileName))
                {
                    await img.CopyToAsync(stream);
                }
            }
            catch (Exception) 
            {
                return false;
            }
            return true;
        }

        public async Task DeleteImage(string fileName)
        {
            System.IO.File.Delete(IMAGES_LOCATION + fileName);
        }
    }
}
