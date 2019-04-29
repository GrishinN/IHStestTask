using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IHStask.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {            
            return new string[] {};
        }

        // GET api/values/lll
        [HttpGet("{request}")]
        public List<string> Get(string request)
        {
            string[] searchQueryWords = request.ToLower().Split(' ');
            List<string> response = new List<string>();

            using (indexFiledbContext db = new indexFiledbContext())
            {
                
                var foundWord = db.Words.FirstOrDefault(p => p.Content == searchQueryWords[0]);
                if (foundWord == null)
                {
                    return response;
                }

                List<int> docsId = db.Inverse.Where(p => p.Word.WordId == foundWord.WordId).Select(i => i.DocId).ToList();

                for (int i = 1; i < searchQueryWords.Length; i++)
                {
                    foundWord = db.Words.FirstOrDefault(p => p.Content == searchQueryWords[i]);
                    if (foundWord == null)
                    {
                        return new List<string>();
                    }
                    docsId = db.Inverse.Where(p => docsId.Contains(p.DocId)).Where(p => p.Word.WordId == foundWord.WordId).Select(p => p.DocId).ToList();
                }

                foreach(int id in docsId)
                {
                    Document doc = db.Documents.FirstOrDefault(item => item.DocId == id);
                    response.Add(doc.Name + doc.Type);
                }
                
            }
            return response;
        }

        [HttpPost]
        public ActionResult Upload(List<IFormFile> upload)
        {
            foreach (IFormFile uploadFile in upload)
            {               
                int id;
                string strFileExtension = Path.GetExtension(uploadFile.FileName);
                string name = Path.GetFileNameWithoutExtension(uploadFile.FileName);
               
                using (indexFiledbContext db = new indexFiledbContext())
                {
                    Document document = new Document();
                    document.Name = Path.GetFileNameWithoutExtension(uploadFile.FileName);
                    document.Type = Path.GetExtension(uploadFile.FileName);

                    if (db.Documents.Where(p => p.Name == document.Name && p.Type == document.Type).Count() == 0)
                    {
                        db.Documents.Add(document);
                        db.SaveChanges();

                        id = db.Documents.Where(p => p.Name == document.Name && p.Type == document.Type).Single().DocId;
                    }
                    else
                    {
                        ModelState.AddModelError(uploadFile.FileName, "file already downloaded");
                        return BadRequest(ModelState);
                    }

                }
                byte[] fileDoc = null;
                using (var binaryReader = new BinaryReader(uploadFile.OpenReadStream()))
                {
                    fileDoc = binaryReader.ReadBytes((int)uploadFile.Length);
                }
               
                List<string> words = TextSeparation(fileDoc);
                FileIndexing(words, id);
            }
            return Created("", "indexing successful");
        }


        public List<string> TextSeparation(byte[] file)
        {
            string utfStringFile = Encoding.UTF8.GetString(file, 0, file.Length);
            String pattern = @"\w+[^\s\W]*\w+|\w";

            var matches = Regex.Matches(utfStringFile, pattern);
            List<string> words = new List<string>();
            foreach (Match match in matches)
            {
                words.Add(match.Value.ToLower());
            }

            return words.Distinct().ToList();
        }

        public void FileIndexing(List<string> words, int docId)
        {
            using (indexFiledbContext db = new indexFiledbContext())
            {
                foreach (string word in words)
                {
                    if (db.Words.Where(item => item.Content == word.ToLower()).Count() == 0)
                    {
                        Word wordDB = new Word();
                        wordDB.Content = word;

                        db.Words.Add(wordDB);
                        db.SaveChanges();
                       
                    }
                    Inverse inverse = new Inverse();
                    inverse.DocId = docId;
                    inverse.WordId = db.Words.Where(p => p.Content == word).Single().WordId;

                    db.Inverse.Add(inverse);
                    db.SaveChanges();
                }
            }
        }

        
    }
}
