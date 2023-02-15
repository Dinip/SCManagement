using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Models;
using System.Text;
using SCManagement.Services.AzureStorageService.Models;
using Unidecode.NET;
using System.Reflection.Emit;
using Microsoft.Extensions.Hosting;

namespace SCManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClubTranslations> ClubTranslations { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<County> County { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<BlobDto> BlobDto { get; set; }
        public DbSet<Club> Club { get; set; }
        public DbSet<Modality> Modality { get; set; }
        public DbSet<RoleClub> RoleClub { get; set; }
        public DbSet<UsersRoleClub> UsersRoleClub { get; set; }
        public DbSet<CodeClub> CodeClub { get; set; }
        public DbSet<Team> Team { get; set; }

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

            //Create Modalities
            builder.Entity<Modality>().HasData(
                new Modality { Id = 1, Name = "Atletismo" },
                new Modality { Id = 2, Name = "Basquetebol" },
                new Modality { Id = 3, Name = "Futebol" },
                new Modality { Id = 4, Name = "Futsal" },
                new Modality { Id = 5, Name = "Hóquei em Patins" },
                new Modality { Id = 6, Name = "Natação" },
                new Modality { Id = 7, Name = "Voleibol" },
                new Modality { Id = 8, Name = "BTT" },
                new Modality { Id = 9, Name = "Taekwondo" },
                new Modality { Id = 10, Name = "Orientação" }
                );

            //Create Roles for the Club
            builder.Entity<RoleClub>().HasData(
                new RoleClub { Id = 10, RoleName = "Sócio" },
                new RoleClub { Id = 20, RoleName = "Atleta" },
                new RoleClub { Id = 30, RoleName = "Treinador" },
                new RoleClub { Id = 40, RoleName = "Secretaria" },
                new RoleClub { Id = 50, RoleName = "Administrador de Clube" }
                );

            builder.Entity<User>().Navigation(e => e.ProfilePicture).AutoInclude();
            builder.Entity<Team>().HasMany(x => x.Athletes).WithMany("Teams");
        }
    }
}

