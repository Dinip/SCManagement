using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Models;
using System.Text;
using SCManagement.Services.AzureStorageService.Models;
using Unidecode.NET;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService.Models;
using SCManagement.Services.PlansService.Models;
using Newtonsoft.Json.Linq;

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
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventEnroll> EventEnroll { get; set; }
        public DbSet<EventResult> EventResult { get; set; }
        public DbSet<ClubPaymentSettings> ClubPaymentSettings { get; set; }
        public DbSet<EventTranslation> EventTranslations { get; set; }
        public DbSet<Bioimpedance> Bioimpedance { get; set; }
        public DbSet<ClubModalityStatistics> ClubModalityStatistics { get; set; }
        public DbSet<ClubPaymentStatistics> ClubPaymentStatistics { get; set; }
        public DbSet<ClubUserStatistics> ClubUserStatistics { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<SystemPaymentStatistics> SystemPaymentStatistics { get; set; }
        public DbSet<SystemPlansStatistics> SystemPlansStatistics { get; set; }
        public DbSet<Notification> Notifications { get; set; }

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
            new Modality { Id = 1 },
            new Modality { Id = 2 },
            new Modality { Id = 3 },
            new Modality { Id = 4 },
            new Modality { Id = 5 },
            new Modality { Id = 6 },
            new Modality { Id = 7 },
            new Modality { Id = 8 },
            new Modality { Id = 9 },
            new Modality { Id = 10 }
            );

            //Add translations to modalities
            builder.Entity<ModalityTranslation>().HasData(
            new ModalityTranslation { Id = 1, ModalityId = 1, Value = "Atletismo", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 2, ModalityId = 1, Value = "Athletics", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 3, ModalityId = 2, Value = "Basquetebol", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 4, ModalityId = 2, Value = "Basketball", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 5, ModalityId = 3, Value = "Futebol", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 6, ModalityId = 3, Value = "Football", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 7, ModalityId = 4, Value = "Futsal", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 8, ModalityId = 4, Value = "Futsal", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 9, ModalityId = 5, Value = "Hóquei em Patins", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 10, ModalityId = 5, Value = "Roller Hockey", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 11, ModalityId = 6, Value = "Natação", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 12, ModalityId = 6, Value = "Swimming", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 13, ModalityId = 7, Value = "Voleibol", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 14, ModalityId = 7, Value = "Volleyball", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 15, ModalityId = 8, Value = "BTT", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 16, ModalityId = 8, Value = "BTT", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 17, ModalityId = 9, Value = "Taekwondo", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 18, ModalityId = 9, Value = "Taekwondo", Atribute = "Name", Language = "en-US" },
            new ModalityTranslation { Id = 19, ModalityId = 10, Value = "Orientação", Atribute = "Name", Language = "pt-PT" },
            new ModalityTranslation { Id = 20, ModalityId = 10, Value = "Orientation", Atribute = "Name", Language = "en-US" }
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

            builder.Entity<TrainingPlan>()
            .HasOne(tp => tp.Athlete)
            .WithMany(u => u.TrainingPlans)
            .HasForeignKey(tp => tp.AthleteId);

            builder.Entity<TrainingPlan>()
            .Ignore(tp => tp.Athlete);

            builder.Entity<MealPlan>()
            .HasOne(tp => tp.Athlete)
            .WithMany(u => u.MealPlans)
            .HasForeignKey(tp => tp.AthleteId);

            builder.Entity<MealPlan>()
            .Ignore(tp => tp.Athlete);

            builder.Entity<Goal>()
            .HasOne(tp => tp.Athlete)
            .WithMany(u => u.Goals)
            .HasForeignKey(tp => tp.AthleteId);

            builder.Entity<Goal>()
            .Ignore(tp => tp.Trainer);
        }
    }
}

