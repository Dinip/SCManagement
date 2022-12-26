using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Models;
using System.Text;
using SCManagement.Services.AzureStorageService.Models;

namespace SCManagement.Data {
    public class ApplicationDbContext : IdentityDbContext<User> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //create country portugal
            builder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Portugal" }
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
                districts.Add(new District { Id = counter++, CountryId = 1, Name = s });
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
                int districtId = districts.Find(d => d.Name.Equals(Convert.ToString(c.distrito)))!.Id;
                counties.Add(new County { Id = counter++, DistrictId = districtId, Name = c.nome });
            }
            //add list of counties to db
            builder.Entity<County>().HasData(counties);
        }

        public DbSet<SCManagement.Services.AzureStorageService.Models.BlobDto> BlobDto { get; set; }
    }
}

