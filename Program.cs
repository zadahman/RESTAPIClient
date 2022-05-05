using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using Client.Repositories;

namespace Client
{
    public class Program
    {
        private static readonly string _baseUrl = "http://localhost:9009/movies";
        private static readonly HttpClient client = new HttpClient();
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Hubber!");

            // await ProcessRepositories();
            IngestAndPostData();

            Console.ReadLine();
        }


        public static void IngestAndPostData()
        {

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            StreamReader streamReader = new StreamReader("movies.json");
            var json = streamReader.ReadToEnd();

            var movieList = JsonSerializer.Deserialize<List<Movies>>(json);

            foreach (var movie in movieList)
            {
                client.PostAsync(_baseUrl, CreateHttpContent(movie));
            }
        }

        /*
        private static async Task ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));



            //var streamTask = client.GetStreamAsync(_baseUrl);
            //var repositories = await JsonSerializer.DeserializeAsync<List<ContactRepository>>(await streamTask);
            
            try
            {
                Console.WriteLine("Processing Repositories...");

                ContactRepository contact1 = new ContactRepository
                {
                    Name = "Zillah",
                    PhoneNumber = "218999768",
                    Email = "zillahada@swe.com"
                };

                ContactRepository contact2 = new ContactRepository
                {
                    Name = "Zahra",
                    PhoneNumber = "412999768",
                    Email = "zahraada@dataeng.com"
                };

                Console.WriteLine();

                var url = await CreateContact(contact1);
                Console.WriteLine($"Created {nameof(contact1)} at {url}");

                var contactOneId = url.Segments.Last();
                var getContact = await GetContactAsync(contactOneId);
                DisplayContact(getContact);

                getContact.PhoneNumber = "2189797441";
                var updateContact = await UpdateContactAsync(getContact);

                var isUpdated = (updateContact.PhoneNumber == contact1.PhoneNumber) ? "No" : "Yes";
                Console.WriteLine($"Has Contact Phone Number changed? {isUpdated}");

                Console.WriteLine("Adding another contact");
                url = await CreateContact(contact2);
                Console.WriteLine($"Created {nameof(contact2)} at {url}");

                Console.WriteLine("Displaying All Contacts");
                var allContacts = await GetAllContactsAsync();

                foreach(var contact in allContacts)
                {
                    DisplayContact(contact);
                }

                Console.WriteLine("Deleting first contact");
                DeleteContactAsync(contactOneId);

                Console.WriteLine("Displaying All Contacts after delete");
                allContacts = await GetAllContactsAsync();

                foreach (var contact in allContacts)
                {
                    DisplayContact(contact);
                }

                Console.WriteLine("Completed Processing Repositories.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
        static async Task<Uri> CreateContact(ContactRepository contact)
        {
            HttpResponseMessage response = await client.PostAsync(
                   _baseUrl, CreateHttpContent(contact));
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<List<ContactRepository>> GetAllContactsAsync()
        {
            var streamTask = await client.GetAsync(_baseUrl);
            streamTask.EnsureSuccessStatusCode();

            var results = JsonSerializer.DeserializeAsync<List<ContactRepository>>(await streamTask.Content.ReadAsStreamAsync());

            return results.Result;
        }

        static async Task<ContactRepository> GetContactAsync(string id)
        {
            var url = $"{_baseUrl}/{id}";

            var streamTask = await client.GetAsync(url);
            streamTask.EnsureSuccessStatusCode();

            var results = JsonSerializer.Deserialize<ContactRepository>(streamTask.Content.ReadAsStringAsync().Result);

            if (results == null)
                return new ContactRepository();
            else return results;
        }

        static async Task<ContactRepository> UpdateContactAsync(ContactRepository contact)
        {
            var url = $"{_baseUrl}/{contact.Id}";

            var response = await client.PutAsync(url, CreateHttpContent(contact));
            response.EnsureSuccessStatusCode();

            return await GetContactAsync(contact.Id.ToString());
        }

        static async void DeleteContactAsync(string id)
        {
            var url = $"{_baseUrl}/{id}";
            var deleteTask = await client.DeleteAsync(url);
            deleteTask.EnsureSuccessStatusCode();
        }

        private static void DisplayContact(ContactRepository contact)
        {
            Console.WriteLine();
            Console.WriteLine($"Name: {contact.Name}\tPhone Number: " +
                $"{contact.PhoneNumber}\tEmail: {contact.Email}");
        }
        */
        private static StringContent CreateHttpContent(Movies contact)
        {
            var companySerialized = JsonSerializer.Serialize(contact);
            return new StringContent(companySerialized, Encoding.UTF8, "application/json");
        }

    }
}