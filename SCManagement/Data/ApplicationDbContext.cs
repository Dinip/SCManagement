using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Models;
using System.Text;
using SCManagement.Services.AzureStorageService.Models;
using Unidecode.NET;

namespace SCManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Country { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<County> County { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<BlobDto> BlobDto { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //create country portugal
            builder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Portugal", NormalizedName = "portugal" }
                );

            //read districts json file to a string
            Encoding encoding = Encoding.UTF8;
            string districtsJsonString = File.ReadAllText("Data/dataset/distritos.json", encoding);

            //parse the string to get the values from json
            List<string> districtsJson = JsonConvert.DeserializeObject<List<string>>(districtsJsonString)!;
            List<District> districts = new List<District>();
            int counter = 1;
            foreach (string s in districtsJson)
            {
                string normalizedName = s.ToLower().Unidecode();
                districts.Add(new District { Id = counter++, CountryId = 1, Name = s, NormalizedName = normalizedName });
            }
            //add list of districts to db
            builder.Entity<District>().HasData(districts);


            //read counties json file to a string
            string countiesJsonString = File.ReadAllText("Data/dataset/concelhos.json", encoding);

            //parse the string to get the values from json
            var countiesJson = JsonConvert.DeserializeObject<List<dynamic>>(countiesJsonString)!;

            List<County> counties = new List<County>();
            counter = 1;
            foreach (dynamic c in countiesJson)
            {
                string nome = (string)c.nome;
                string normalizedName = nome.ToLower().Unidecode();
                int districtId = districts.Find(d => d.Name.Equals(Convert.ToString(c.distrito)))!.Id;
                counties.Add(new County { Id = counter++, DistrictId = districtId, Name = nome, NormalizedName = normalizedName });
            }
            //add list of counties to db
            builder.Entity<County>().HasData(counties);
        }
    }
}

