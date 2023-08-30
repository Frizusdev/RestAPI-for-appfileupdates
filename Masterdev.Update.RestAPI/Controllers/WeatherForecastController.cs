using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json.Serialization;
using Masterdev.Update.RestAPI.Entity;
using Masterdev.Update.RestAPI.Models;
using Masterdev.Update.RestAPI.Properties;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Masterdev.Update.RestAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class Masterdev_Updater : ControllerBase
{
    private readonly ILogger<Masterdev_Updater> _logger;
    private readonly ApplicationDbContext _context;

    public Masterdev_Updater(ILogger<Masterdev_Updater> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Zwraca listę aplikacji w bazie danych z ich ID potrzebnym do wgrania aktualizacji na serwer
    /// </summary>
    /// <returns></returns>

    [HttpGet(Name = "getapps")]
    public IActionResult GetApps()
    {
        try
        {
            return Ok(_context.apps.OrderBy(x => x.id).ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine("\n ERROR: " + e + "\n");
            return NotFound("ERROR: \n" + e);
        }
    }

    /// <summary>
    /// Funkcja get pozwalająca na pobranie aktualizacji z serwera po nazwie pliku (Nazwy sie nie powtorzą)
    /// Można zaktualizować ścieżki o podfolder z nazwą aplikacji dla czytelności
    /// </summary>
    /// <param name="file_name">Nazwa Pliku</param>
    /// <param name="version">Wersja aktualizacji</param>
    /// <returns></returns>

    [HttpGet(Name = "updatezip")]
    public IActionResult GetUpdate(string file_name, string version)
    {
        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Updatezips");
            //var data = System.IO.File.ReadAllBytes(Path.Combine(path, updatename));

            if (Path.Combine(path, file_name).Any() == true)
            {
                var data = System.IO.File.ReadAllBytes(Path.Combine(path, file_name));
                //AddToDBData(who, version);
                return File(data, "application/zip", file_name);
            }
            else
            {
                Console.WriteLine("\n ERROR: File doesn't exist !" + "\n");
                return NotFound("ERROR: File doesn't exist !");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("\n ERROR: " + e + "\n");
            return NotFound("ERROR: \n" + e);
        }
    }

    /// <summary>
    /// Funkcja do wgrywania aktualizacji na serwer (wersja aplikacji jest weryfikowana tutaj)
    /// </summary>
    /// <param name="postedFile">Zip z aktualizacją</param>
    /// <param name="version">Wersja Aktualizacji</param>
    /// <param name="who">Kto wgrywa</param>
    /// <param name="app_id">ID aplikacji</param>
    /// <returns></returns>

    [HttpPost(Name = "UploadUpdate")]
    public IActionResult UploadUpdate(IFormFile postedFile, string version, int who, int app_id)
    {
        try
        {
            var versions = _context.files.Select(s => s.version).ToList();
            var result = versions.OrderByDescending(x => int.Parse(x.Replace(".", "").ToString())).FirstOrDefault();
            if (versions.Contains(version))
            {
                return Ok("Wersja już istnieje. Najnowsza wgrana wersja to " + result);
            }

            if (int.Parse(result.Replace(".", "")) > int.Parse(version.Replace(".", "")))
            {
                return Ok("Wersja jest niższa od aktualnie wgranych. Najnowsza wgrana wersja to " + result);
            }
            // Odebranie i zaoisanie do Updatezips
            string fileName = Path.GetFileName(postedFile.FileName);
            if (postedFile != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Updatezips");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (Path.Exists(path + "/" + fileName))
                {
                    fileName = getNextFileName(path, fileName);
                }

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                AddToDBFile(fileName, who, version, app_id);
            }
            return Ok();


        }
        catch (Exception e)
        {
            Console.WriteLine("\n ERROR: " + e + "\n");
            return NotFound("ERROR: \n" + e);
        }
    }

    /// <summary>
    /// Wywoływana po prawidłowym wgraniu pliku na serwer ,dodaje rekord w bazie danych o dodanym pliku przez kogo oraz wersji
    /// </summary>
    /// <param name="filename">Nazwa Pliku Zip Aktualizacji</param>
    /// <param name="createdby">Przez Kogo Wgrane</param>
    /// <param name="version">Wersja Aktualizacji</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult AddToDBFile(string filename, int createdby, string version, int app_id)
    {
        var today = DateTime.Now;

        try
        {
            var file = new Files();

            file.createdby = createdby;
            file.file_name = filename;
            file.app_id = app_id;
            file.file_path = Path.Combine(Directory.GetCurrentDirectory(), "Updatezips");
            file.version = version;
            file.date = DateTime.Now;

            _context.files.Add(file);
            _context.SaveChanges();
            return Ok(file.file_path + file.file_name);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error while connecting to DB");
        }
    }

    /// <summary>
    /// Funkcja zwracająca nowszą wersję do konsoli gdy jest aktualna zwraca null
    /// </summary>
    /// <param name="appname">Nazwa aplikacji</param>
    /// <param name="version">Aktualnie wgrana wersja</param>
    /// <returns></returns>

    [HttpGet]
    public IActionResult GetNewerVersion(string appname, string version)
    {
        try
        {
            int appid = _context.apps.Where(x => x.App_Name == appname).FirstOrDefault().id;
            if (appid != null)
            {
                var list = _context.files.Where(x => x.app_id == appid).ToList();

                //List<int> array = list.Select(x => int.Parse(x.version.Replace(".", ""))).ToList();

                var versionnew = int.Parse(version.Replace(".", ""));

                var g = list.Where(x => int.Parse(x.version.Replace(".", "")) > versionnew).OrderBy(x => int.Parse(x.version.Replace(".", ""))).FirstOrDefault();

                if (g != null)
                {
                    return Ok(new { g.file_name, g.version });
                }
                else
                {
                    version = null;
                    string file_name = null;
                    return Ok(new { version, file_name });
                }
            }
            else return StatusCode(500);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }


    /// <summary>
    /// Dodaje applikacje do bazy danych oraz zwraca zip z aplikacją konsolową
    /// </summary>
    /// <param name="appname">Nazwa Aplikacji Do bazy danych i IIS</param>
    /// <param name="directoryname">Nazwa podfolderu z aplikacją do aktualizowania</param>
    /// <param name="address">Adres strony</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult AddNewApplication (string appname, string directoryname, string address = "http://10.0.1.172")
    {
        try
        {

            var app = new Apps();
            app.App_Name = appname;
            _context.apps.Add(app);

            config x = new config();

            x.applicationName = appname;
            x.directoryName = directoryname;
            x.updateFilePath = address + "/Masterdev_Updater/GetNewerVersion";
            x.updateZipFilePath = address + "/Masterdev_Updater/GetUpdate";
            x.serverFileName = "uploaded.txt";

            System.IO.File.WriteAllText(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ClientFiles", "ClientApp", "config.txt"), JsonConvert.SerializeObject(x));

            string startPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ClientFiles", "ClientApp");
            string zipPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ClientFiles" , "package.zip");
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(startPath, zipPath);

            //return Ok("Aplikacja dodana do Bazy Danych");
            var data = System.IO.File.ReadAllBytes(zipPath);

            _context.SaveChanges();

            return File(data, "application/zip", "package.zip");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Error Przy dodawaniu aplikacji do bazy danych. Kod błędu: \n" + e);
        }
    }

    private string getNextFileName(string path, string fileName)
    {
        string extension = Path.GetExtension(fileName);

        int i = 0;
        while (Path.Exists(path + "/" + fileName))
        {
            if (i == 0)
                fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
            else
                fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
        }

        return fileName;
    }
}

