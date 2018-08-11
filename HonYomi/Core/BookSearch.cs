using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using static System.Text.RegularExpressions.Regex;

namespace HonYomi.Core
{
    internal class BookSearch
    {
        private static readonly HttpClient client = new HttpClient();
   

        private static string FormatSearch(string str)
        {
            str = str.ToLower();
            str = Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars      
            str = Replace(str, @"\s0+", " "); // Replace leading zeros
            str = Replace(str, @"\s+", " ").Trim();  // convert multiple spaces into one space
            str = str.Replace(" -", "");
            str = Replace(str, @"pt\s+[0-9]*","");
            str = Replace(str, @"\s", "+");          // //Replace spaces by dashes
            return str;
        }

        internal static BookInfo Search(string filename)
        {
            string query = FormatSearch(filename);
            HttpResponseMessage response = client.GetAsync($"http://openlibrary.org/search.json?q={query}").Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            } 

            OpenLibraryResult olr = JsonConvert.DeserializeObject<OpenLibraryResult>(response.Content.ReadAsStringAsync().Result);
            //the only successful state
            if (olr.NumFound >= 1)
            {
                return new BookInfo {Title = olr.Docs[0].Title, Author = string.Join(", ", olr.Docs[0].AuthorName), ISBN = olr.Docs[0].Isbn.FirstOrDefault()};
            }

            return null;
        }
    }

    internal class BookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
    }
    public  class OpenLibraryResult
    {
        [JsonProperty("start", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Start { get; set; }

        [JsonProperty("num_found", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? OpenLibraryResultNumFound { get; set; }

        [JsonProperty("numFound", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? NumFound { get; set; }

        [JsonProperty("docs", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Doc[] Docs { get; set; }
    }

    public  class Doc
    {
        [JsonProperty("title_suggest", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TitleSuggest { get; set; }

        [JsonProperty("edition_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] EditionKey { get; set; }

        [JsonProperty("cover_i", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? CoverI { get; set; }

        [JsonProperty("isbn", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Isbn { get; set; }

        [JsonProperty("has_fulltext", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasFulltext { get; set; }

        [JsonProperty("text", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Text { get; set; }

        [JsonProperty("author_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] AuthorName { get; set; }

        [JsonProperty("seed", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Seed { get; set; }

        [JsonProperty("oclc", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long[] Oclc { get; set; }

        [JsonProperty("author_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] AuthorKey { get; set; }

        [JsonProperty("subject", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Subject { get; set; }

        [JsonProperty("title", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("publish_date", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] PublishDate { get; set; }

        [JsonProperty("type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("ebook_count_i", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? EbookCountI { get; set; }

        [JsonProperty("edition_count", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? EditionCount { get; set; }

        [JsonProperty("key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("id_goodreads", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
  
        public long[] IdGoodreads { get; set; }

        [JsonProperty("publisher", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Publisher { get; set; }

        [JsonProperty("language", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Language { get; set; }

        [JsonProperty("last_modified_i", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? LastModifiedI { get; set; }

        [JsonProperty("id_librarything", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
  
        public long[] IdLibrarything { get; set; }

        [JsonProperty("cover_edition_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CoverEditionKey { get; set; }

        [JsonProperty("publish_year", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long[] PublishYear { get; set; }

        [JsonProperty("first_publish_year", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? FirstPublishYear { get; set; }

        [JsonProperty("ia", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Ia { get; set; }

        [JsonProperty("lending_identifier_s", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LendingIdentifierS { get; set; }

        [JsonProperty("ia_collection_s", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string IaCollectionS { get; set; }

        [JsonProperty("printdisabled_s", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string PrintdisabledS { get; set; }

        [JsonProperty("ia_box_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string[] IaBoxId { get; set; }

        [JsonProperty("public_scan_b", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? PublicScanB { get; set; }

        [JsonProperty("lending_edition_s", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LendingEditionS { get; set; }
    }

}