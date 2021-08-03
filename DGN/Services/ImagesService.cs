using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DGN.Services
{
    public class ImagesService
    {
        private readonly string IMAGES_LOCATION = "wwwroot/images/";
        public readonly string CLIENT_IMAGES_LOCATION = "/images/";
        private readonly List<string> ALLOWD_IMAGE_EXTENSIONS = new List<string>(){ ".png", ".jpg", ".jpeg" };

        public async Task<bool> UploadImage(IFormFile img, string fileName)
        {
            if (!IsImageValid(img)) 
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
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public void DeleteImage(string fileName)
        {
            if (System.IO.File.Exists(IMAGES_LOCATION + fileName))
            {
                System.IO.File.Delete(IMAGES_LOCATION + fileName);
            }
        }

        public bool IsImageValid(IFormFile img)
        {
            bool isValid = false;
            if (img != null && img.Length > 0)
            {
                var imageExtension = System.IO.Path.GetExtension(img.FileName);
                isValid = ALLOWD_IMAGE_EXTENSIONS.Contains(imageExtension);
            }
            
            return isValid;
        }
    }
}
