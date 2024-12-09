

using Microsoft.AspNetCore.Http;

namespace Core.Application.Helpers
{
    public static class FileHelped
    {
        public static string UploadFile(IFormFile file, object id, bool isEditMode = false, string entidad = "", string imagePath = "")
        {
            if (file == null)
            {
                if (isEditMode) 
                {
                    return imagePath;
                }
               
            }

            string basePath = $"/Images/{entidad}/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new FileInfo(file!.FileName);
            string fileName = guid + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Manejo de la imagen existente
            if (isEditMode && !string.IsNullOrEmpty(imagePath))
            {
                // Obtener el nombre del archivo de la ruta proporcionada
                string oldFileName = Path.GetFileName(imagePath);
                string completeImageOldPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{imagePath}");

                if (File.Exists(completeImageOldPath))
                {
                    File.Delete(completeImageOldPath);
                }
                else
                {
                    // Log para verificar que no se encontró el archivo
                    Console.WriteLine($"El archivo {completeImageOldPath} no fue encontrado para eliminar.");
                }

            }
                return $"{basePath}/{fileName}";
        }

        public static void FileDelete(string entidad, object id)
        {
            string basePath = $"/Images/{entidad}/{id}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new(path);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo folder in directory.GetDirectories())
                {
                    folder.Delete(true);
                }

                Directory.Delete(path);
            }

        }

    }
}
