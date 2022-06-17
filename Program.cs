using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Client.Repositories;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;

namespace Client
{
    public class Program
    {
        private static readonly string _baseUrl = "https://localhost:7115/api/records";//"http://localhost:9009/movies";
        private static readonly HttpClient client = new HttpClient();
        private static List<Record> records = new List<Record>();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Checkr!");


            IngestAndPostData();
            await ProcessRepositories();

            Console.ReadLine();
        }


        public static void IngestAndPostData()
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                //Delimiter = ",",
                // Comment = '%'
            };

            using (var reader = new StreamReader("data/chkr.csv"))
            using (var csv = new CsvReader(reader, configuration))
            {
                records = csv.GetRecords<Record>().ToList();
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var rec in records)
            {
                Console.WriteLine(rec.actor);
                client.PostAsync(_baseUrl, CreateHttpContent(rec));
            }


            /*
                StreamReader streamReader = new StreamReader("movies.json");
                var json = streamReader.ReadToEnd();

                var movieList = JsonSerializer.Deserialize<List<Movies>>(json);

                foreach (var movie in movieList)
                {
                    client.PostAsync(_baseUrl, CreateHttpContent(movie));
                }
            */
        }

        
        private static async Task ProcessRepositories()
        {
            //var streamTask = client.GetStreamAsync(_baseUrl);
            //var repositories = await JsonSerializer.DeserializeAsync<List<ContactRepository>>(await streamTask);
            
            try
            {
                Console.WriteLine("\nProcessing Repositories...");

                Record record1 = new Record
                {
                    year= 1983,
                    length = 104,
                    title= "Dead Zone, The",
                    subject= "Horror",
                    actor= "Walken, Christopher",
                    actress= "Adams, Brooke",
                    director= "Cronenberg, David",
                    popularity = 79,
                    awards = false,
                    image = "NicholasCage.png"
                };

                Record record2 = new Record
                {
                    year = 1979,
                    length = 122,
                    title = "Cuba",
                    subject = "Action",
                    actor = "Connery, Sean",
                    actress = "Adams, Brooke",
                    director = "Lester, Richard",
                    popularity = 6,
                    awards = false,
                    image = "seanConnery.png"
                };

                Console.WriteLine();

                var url = await Create(record1);
                Console.WriteLine($"Created {nameof(record1)} at {url}");

                var recordOneId = url.Segments.Last();
                Console.WriteLine("rec " + recordOneId);
                var getRecord = await GetAsync(recordOneId);
                Display(getRecord);

                getRecord.year = 1904;
                var updateRecord = await UpdateAsync(getRecord);

                var isUpdated = (updateRecord.year == record1.year) ? "No" : "Yes";
                Console.WriteLine($"Has record changed? {isUpdated}");

                Console.WriteLine("Adding another record");
                url = await Create(record2);
                Console.WriteLine($"Created {nameof(record2)} at {url}");

                Console.WriteLine("\nDisplaying All Records");
                var allRecords = await GetAllAsync();

                foreach(var record in allRecords)
                {
                    Display(record);
                }

                Console.WriteLine("Deleting first record");
                var isDeleted = await DeleteAsync(recordOneId);
                Console.WriteLine($"Has record been deleted? {isDeleted}");

                Console.WriteLine("\nDisplaying All Records after delete");
                allRecords = await GetAllAsync();

                foreach (var record in allRecords)
                {
                    Display(record);
                }

                Console.WriteLine("Completed Processing Repositories.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
        static async Task<Uri> Create(Record record)
        {
            HttpResponseMessage response = await client.PostAsync(
                   _baseUrl, CreateHttpContent(record));
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<List<Record>> GetAllAsync()
        {
            var streamTask = await client.GetAsync(_baseUrl);
            streamTask.EnsureSuccessStatusCode();

            var results = JsonSerializer.DeserializeAsync<List<Record>>(await streamTask.Content.ReadAsStreamAsync());

            return results.Result;
        }

        static async Task<Record> GetAsync(string id)
        {
            var url = $"{_baseUrl}/{id}";

            var streamTask = await client.GetAsync(url);
            streamTask.EnsureSuccessStatusCode();

            var results = JsonSerializer.Deserialize<Record>(streamTask.Content.ReadAsStringAsync().Result);

            if (results == null)
                return new Record();
            else return results;
        }

        static async Task<Record> UpdateAsync(Record record)
        {
            var url = $"{_baseUrl}/{record.id}";

            var response = await client.PutAsync(url, CreateHttpContent(record));
            response.EnsureSuccessStatusCode();

            return await GetAsync(record.id.ToString());
        }

        static async Task<bool> DeleteAsync(string id)
        {
            var url = $"{_baseUrl}/{id}";
            var deleteTask = await client.DeleteAsync(url);
            deleteTask.EnsureSuccessStatusCode();
            return deleteTask.IsSuccessStatusCode ? true : false;
        }

        private static void Display(Record record)
        {
            Console.WriteLine();
            Console.WriteLine($"Year: {record.year}\tActor: " +
                $"{record.actor}\tTitle: {record.title}");
        }
        
        private static StringContent CreateHttpContent(Record record)
        {
            var recordSerialized = JsonSerializer.Serialize(record);
            return new StringContent(recordSerialized, Encoding.UTF8, "application/json");
        }

    }
}