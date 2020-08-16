using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Planner.Model;

namespace Planner.Data
{
    public class DataContext : DbContext
    {
        //Migrations:
        //Add-Migration "Version0001" -Project Planner -StartupProject Planner
        //Remove-Migration -Project Planner -StartupProject Planner

        //Common SQL commands:
        //Open SQL Server Object Explorer, right click the server, click on "New query".
        //Drop Database BbqPlannerDatabase;

        #region Sets

        public DbSet<User> Users { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<Event> Events { get; set; }

        #endregion

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Relational configurations

            //Users with participations in events: U -> P <- E
            builder.Entity<Participation>().HasKey(sc => new { sc.UserId, sc.EventId });

            builder.Entity<Participation>().HasOne(bc => bc.User).WithMany(b => b.UserEvents).HasForeignKey(bc => bc.UserId);
            builder.Entity<Participation>().HasOne(bc => bc.Event).WithMany(c => c.EventsUsers).HasForeignKey(bc => bc.EventId);

            #endregion

            #region Seed data

            var users = new List<User>
            {
                new User { Id = 1, Email = "alice@exemplo.com", Name = "Alice", Role = "Admin" },
                new User { Id = 2, Email = "beto@exemplo.com", Name = "Beto", Role = "User" },
                new User { Id = 3, Email = "diego.b@exemplo.com", Name = "Diego B.", Role = "User" },
                new User { Id = 4, Email = "diego.p@exemplo.com", Name = "Diego P.", Role = "User" },
                new User { Id = 5, Email = "fernando@exemplo.com", Name = "Fernando", Role = "User" },
                new User { Id = 6, Email = "gabriel@exemplo.com", Name = "Gabriel", Role = "User" },
                new User { Id = 7, Email = "leonardo@exemplo.com", Name = "Leonardo", Role = "User" },
                new User { Id = 8, Email = "marcus@exemplo.com", Name = "Marcus J.", Role = "User" },
                new User { Id = 9, Email = "michele@exemplo.com", Name = "Michele", Role = "Admin" },
                new User { Id = 10, Email = "paulo@exemplo.com", Name = "Paulo", Role = "User" },
                new User { Id = 11, Email = "rafael@exemplo.com", Name = "Rafael S.", Role = "User" },
                new User { Id = 12, Email = "ralf@exemplo.com", Name = "Ralf", Role = "User" },
                new User { Id = 13, Email = "ruan@exemplo.com", Name = "Ruan", Role = "User" },
                new User { Id = 14, Email = "thales@exemplo.com", Name = "Thales", Role = "User" },
                new User { Id = 15, Email = "wait@exemplo.com", Name = "Wait", Role = "User" },
            };

            //Aplica a mesma senha para todos os usuários, como teste.
            foreach (var user in users)
            {
                using var hmac = new System.Security.Cryptography.HMACSHA512();
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("abc@12345"));
                user.PasswordLastUpdatedUtc = DateTime.UtcNow;
            }

            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Aniversário", DueTo = new DateTime(2020, 12, 1), SuggestedValue = 10, SuggestedValueWithDrinks = 20 },
                new Event { Id = 2, Name = "Final de Ano", DueTo = new DateTime(2020, 12, 1), SuggestedValue = 50, SuggestedValueWithDrinks = 60 },
                new Event { Id = 3, DueTo = new DateTime(2020, 12, 1), SuggestedValue = 10, SuggestedValueWithDrinks = 15 }
            };

            var participations = new List<Participation>
            {
                //First event.
                new Participation { EventId = 1, UserId = 1, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 2, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 3, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 4, AddedIn = new DateTime(2020, 08, 03), Contribution = 10, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 5, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 6, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = true, Observation = null },
                new Participation { EventId = 1, UserId = 7, AddedIn = new DateTime(2020, 08, 03), Contribution = 10, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 8, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 9, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 10, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 11, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 12, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 13, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = true, Observation = null },
                new Participation { EventId = 1, UserId = 14, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 1, UserId = 15, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },

                //Second event.
                new Participation { EventId = 2, UserId = 1, AddedIn = new DateTime(2020, 08, 03), Contribution = 50, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 2, AddedIn = new DateTime(2020, 08, 03), Contribution = 60, HasPaid = false, Observation = "Vai organizar o sistema de som." },
                new Participation { EventId = 2, UserId = 4, AddedIn = new DateTime(2020, 08, 03), Contribution = 50, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 5, AddedIn = new DateTime(2020, 08, 03), Contribution = 50, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 7, AddedIn = new DateTime(2020, 08, 03), Contribution = 60, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 9, AddedIn = new DateTime(2020, 08, 03), Contribution = 60, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 10, AddedIn = new DateTime(2020, 08, 03), Contribution = 60, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 11, AddedIn = new DateTime(2020, 08, 03), Contribution = 50, HasPaid = false, Observation = null },
                new Participation { EventId = 2, UserId = 13, AddedIn = new DateTime(2020, 08, 03), Contribution = 50, HasPaid = true, Observation = null },
                new Participation { EventId = 2, UserId = 15, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = "Só vai ficar um pouco." },

                //Third event.
                new Participation { EventId = 3, UserId = 10, AddedIn = new DateTime(2020, 08, 03), Contribution = 10, HasPaid = false, Observation = null },
                new Participation { EventId = 3, UserId = 11, AddedIn = new DateTime(2020, 08, 03), Contribution = 10, HasPaid = false, Observation = null },
                new Participation { EventId = 3, UserId = 12, AddedIn = new DateTime(2020, 08, 03), Contribution = 20, HasPaid = false, Observation = null },
                new Participation { EventId = 3, UserId = 13, AddedIn = new DateTime(2020, 08, 03), Contribution = 15, HasPaid = true, Observation = null },
                new Participation { EventId = 3, UserId = 14, AddedIn = new DateTime(2020, 08, 03), Contribution = 15, HasPaid = false, Observation = null },
                new Participation { EventId = 3, UserId = 15, AddedIn = new DateTime(2020, 08, 03), Contribution = 30, HasPaid = false, Observation = "Vai trazer outra pessoa, vai pagar em dobro." },
            };

            builder.Entity<User>().HasData(users);
            builder.Entity<Event>().HasData(events);
            builder.Entity<Participation>().HasData(participations);
            
            #endregion

            base.OnModelCreating(builder);
        }
    }
}