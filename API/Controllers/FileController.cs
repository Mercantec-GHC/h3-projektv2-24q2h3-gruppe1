using Microsoft.AspNetCore.Mvc;
using System.IO;

[Route("File")]
public class FileController : Controller
{
    public IActionResult GetPdf(string fileName)
    {
        // Combine the base directory with the wwwroot/files directory and the file name
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", fileName);

        // Ensure the file exists
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/pdf");
    }
}
