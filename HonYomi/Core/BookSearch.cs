using System.Net.Http;
using Newtonsoft.Json;
using static System.Text.RegularExpressions.Regex;

namespace HonYomi.Core
{
    public class BookSearch
    {

        public BookSearch()
        {
        }

        private string CleanTitle(string str)
        {
            return str;
        }
        private string FormatSearch(string str)
        {
            str = str.ToLower();
            str = Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
            str = Replace(str, @"\s+", " ").Trim();  // convert multiple spaces into one space  
            str = Replace(str, @"\s", "+");          // //Replace spaces by dashes
            return str;
        }

        private BookInfo Search(string filename, HttpClient client)
        {
            string query = FormatSearch(filename);
            HttpResponseMessage response = client.GetAsync($"http://openlibrary.org/search.json?{query}").Result;
            if (!response.IsSuccessStatusCode)
            {
                return "";
            } //TODO WARNING

            OpenLibraryResult olr = JsonConvert.DeserializeObject<OpenLibraryResult>(response.Content.ReadAsStringAsync().Result);
            if (olr.NumFound == 1)
            {
                 BookInfo book = new BookInfo();
                return BookInfo{Title = olr.Docs.[0]}
            }
        }
    }

    internal class BookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
    }
    internal  class OpenLibraryResult
    {
        public long? Start                     { get; set; }
        public long? OpenLibraryResultNumFound { get; set; }
        public long? NumFound                  { get; set; }
        public Doc[] Docs                      { get; set; }
    }

    internal  class Doc
    {
        public string   TitleSuggest       { get; set; }
        public string[] EditionKey         { get; set; }
        public long?    CoverI             { get; set; }
        public string[] Isbn               { get; set; }
        public bool?    HasFulltext        { get; set; }
        public string[] Text               { get; set; }
        public string[] AuthorName         { get; set; }
        public string[] Seed               { get; set; }
        public long[]   Oclc               { get; set; }
        public string[] AuthorKey          { get; set; }
        public string[] Subject            { get; set; }
        public string   Title              { get; set; }
        public string[] PublishDate        { get; set; }
        public string   Type               { get; set; }
        public long?    EbookCountI        { get; set; }
        public long?    EditionCount       { get; set; }
        public string   Key                { get; set; }
        public long[]   IdGoodreads        { get; set; }
        public string[] Publisher          { get; set; }
        public string[] Language           { get; set; }
        public long?    LastModifiedI      { get; set; }
        public long[]   IdLibrarything     { get; set; }
        public string   CoverEditionKey    { get; set; }
        public long[]   PublishYear        { get; set; }
        public long?    FirstPublishYear   { get; set; }
        public string[] Ia                 { get; set; }
        public string   LendingIdentifierS { get; set; }
        public string   IaCollectionS      { get; set; }
        public string   PrintdisabledS     { get; set; }
        public string[] IaBoxId            { get; set; }
        public bool?    PublicScanB        { get; set; }
        public string   LendingEditionS    { get; set; }
    }

}