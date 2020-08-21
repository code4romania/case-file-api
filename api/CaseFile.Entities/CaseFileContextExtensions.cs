using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CaseFile.Entities
{
    public static class CaseFileContextExtensions
    {
        private static readonly Dictionary<string, bool> FormsArray = new Dictionary<string, bool>()
        {
            { "A",false},
            { "B",true},
            { "C",true},
            { "D",false},
            { "E",false }
    };

        public static void EnsureSeedData(this CaseFileContext context)
        {
            if (!context.AllMigrationsApplied())
            {
                return;
            }

            //means we have data
            //if (context.Counties.Count() > 0)
            //    return;

            using (var tran = context.Database.BeginTransaction())
            {
                //context.DataCleanUp(); // why cleanup if we return when we have data? y tho.

                //context.SeedNGOs();
                //context.SeedCounties();
                //context.SeedObservers();

                //context.SeedOptions();
                //foreach (var form in FormsArray)
                //{
                //    context.SeedForms(form.Key, form.Value);
                //    context.SeedFormSections(form.Key);
                //    context.SeedQuestions(form.Key, form.Value);
                //}
                //context.SeedCities();
                //context.SeedBeneficiaries();

                //tran.Commit();
            }
        }

        private static void SeedCities(this CaseFileContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }

            context.Cities.Add(
                new City { Name = "Test City", CountyId = context.Counties.First().CountyId }
                );

            context.SaveChanges();
        }

        private static void SeedBeneficiaries(this CaseFileContext context)
        {
            if (context.Beneficiaries.Any())
            {
                return;
            }

            context.Beneficiaries.Add(
                    new Beneficiary() { BirthDate = new DateTime(1987, 6, 15), CityId = 1, CountyId = 1, Name = "Test Beneficiary", UserId = 1}
                );

            context.SaveChanges();
        }

        private static void SeedObservers(this CaseFileContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            context.Users.Add(
                    // pwd: ACgti/NZu42h6KR5js3dO9RQvBPCKhnJ67yyXfR69zWQZDjBVKz1xQPRthfPZZNJEQ==
                    //new User() { NgoId = 0, FromTeam = false, NgoId = 1, Phone = "0722222222", Name = "Test", Password = "1234", MobileDeviceId = Guid.NewGuid().ToString(), DeviceRegisterDate = DateTime.Now }
                    new User() { NgoId = 1, Phone = "0722222222", Name = "CaseFile Test", Email = "testing@casefile.com", Password = "Testing@1" }
                );

            context.SaveChanges();
        }

        private static void SeedCounties(this CaseFileContext context)
        {
            if (context.Counties.Any())
            {
                return;
            }

            context.Counties.AddRange(
                new County { CountyId = 1, Code = "AB", Name = "ALBA" },
                new County { CountyId = 2, Code = "AR", Name = "ARAD" },
                new County { CountyId = 3, Code = "AG", Name = "ARGES" },
                new County { CountyId = 4, Code = "BC", Name = "BACAU" },
                new County { CountyId = 5, Code = "BH", Name = "BIHOR" },
                new County { CountyId = 6, Code = "BN", Name = "BISTRITA-NASAUD" },
                new County { CountyId = 7, Code = "BT", Name = "BOTOSANI" },
                new County { CountyId = 8, Code = "BV", Name = "BRASOV" },
                new County { CountyId = 9, Code = "BR", Name = "BRAILA" },
                new County { CountyId = 10, Code = "BZ", Name = "BUZAU" },
                new County { CountyId = 11, Code = "CS", Name = "CARAS-SEVERIN" },
                new County { CountyId = 51, Code = "CL", Name = "CALARASI" },
                new County { CountyId = 12, Code = "CJ", Name = "CLUJ" },
                new County { CountyId = 13, Code = "CT", Name = "CONSTANTA" },
                new County { CountyId = 14, Code = "CV", Name = "COVASNA" },
                new County { CountyId = 15, Code = "DB", Name = "DÂMBOVITA" },
                new County { CountyId = 16, Code = "DJ", Name = "DOLJ" },
                new County { CountyId = 17, Code = "GL", Name = "GALATI" },
                new County { CountyId = 52, Code = "GR", Name = "GIURGIU" },
                new County { CountyId = 18, Code = "GJ", Name = "GORJ" },
                new County { CountyId = 19, Code = "HR", Name = "HARGHITA" },
                new County { CountyId = 20, Code = "HD", Name = "HUNEDOARA" },
                new County { CountyId = 21, Code = "IL", Name = "IALOMITA" },
                new County { CountyId = 22, Code = "IS", Name = "IASI" },
                new County { CountyId = 23, Code = "IF", Name = "ILFOV" },
                new County { CountyId = 24, Code = "MM", Name = "MARAMURES" },
                new County { CountyId = 25, Code = "MH", Name = "MEHEDINTI" },
                new County { CountyId = 40, Code = "B", Name = "BUCURESTI" },
                new County { CountyId = 26, Code = "MS", Name = "MURES" },
                new County { CountyId = 27, Code = "NT", Name = "NEAMT" },
                new County { CountyId = 28, Code = "OT", Name = "OLT" },
                new County { CountyId = 29, Code = "PH", Name = "PRAHOVA" },
                new County { CountyId = 30, Code = "SM", Name = "SATU MARE" },
                new County { CountyId = 31, Code = "SJ", Name = "SALAJ" },
                new County { CountyId = 32, Code = "SB", Name = "SIBIU" },
                new County { CountyId = 33, Code = "SV", Name = "SUCEAVA" },
                new County { CountyId = 34, Code = "TR", Name = "TELEORMAN" },
                new County { CountyId = 35, Code = "TM", Name = "TIMIS" },
                new County { CountyId = 36, Code = "TL", Name = "TULCEA" },
                new County { CountyId = 37, Code = "VS", Name = "VASLUI" },
                new County { CountyId = 38, Code = "VL", Name = "VÂLCEA" },
                new County { CountyId = 39, Code = "VN", Name = "VRANCEA" }
                );
        }

        private static void DataCleanUp(this CaseFileContext context)
        {
            context.Database.ExecuteSqlRaw("delete from OptionsToQuestions");
            context.Database.ExecuteSqlRaw("delete from Questions");
            context.Database.ExecuteSqlRaw("delete from FormSections");
            context.Database.ExecuteSqlRaw("delete from Forms");
            context.Database.ExecuteSqlRaw("delete from Counties");
            context.Database.ExecuteSqlRaw("delete from Users");
        }

        private static void SeedOptions(this CaseFileContext context)
        {
            if (context.Options.Any())
            {
                return;
            }

            context.Options.AddRange(
                new Option { Text = "Da", },
                new Option { Text = "Nu", },
                new Option { Text = "Nu stiu", },
                new Option { Text = "Dark Island", },
                new Option { Text = "London Pride", },
                new Option { Text = "Zaganu", },
                new Option { Text = "Transmisia manualã", },
                new Option { Text = "Transmisia automatã", },
                new Option { Text = "Altele (specificaţi)", IsFreeText = true },
                new Option { Text = "Metrou" },
                new Option { Text = "Tramvai" },
                new Option { Text = "Autobuz" }
            );

            context.SaveChanges();
        }
        private static void SeedFormSections(this CaseFileContext context, string formCode)
        {
            var form = context.Forms.SingleOrDefault(f => f.Code == formCode);
            if (form == null)
            {
                return;
            }

            context.FormSections.AddRange(
                new FormSection { Code = formCode + "B", Description = "Despre Bere", FormId = form.FormId },
                new FormSection { Code = formCode + "C", Description = "Description masini", FormId = form.FormId }
                );

            context.SaveChanges();
        }

        private static void SeedQuestions(this CaseFileContext context, string formCode, bool diaspora)
        {
            var f = context.Forms.FirstOrDefault(ff => ff.Code == formCode);
            if (f == null)
            {
                f = new Form { Code = formCode, Draft = false }; //, Diaspora = diaspora
                context.Forms.Add(f);
                context.SaveChanges();
            }

            var fsB = context.FormSections
                .FirstOrDefault(ff => ff.FormId == f.FormId && ff.Code == $"{formCode}B");
            var fsC = context.FormSections
                .FirstOrDefault(ff => ff.FormId == f.FormId && ff.Code == $"{formCode}C");

            if (fsB == null)
            {
                fsB = new FormSection { FormId = f.FormId, Code = $"{formCode}B", Description = $"section B of Form {f.FormId}" };
                context.FormSections.Add(fsB);
                context.SaveChanges();
            }
            if (fsC == null)
            {
                fsC = new FormSection { FormId = f.FormId, Code = $"{formCode}C", Description = $"section C of Form {f.FormId}" };
                context.FormSections.Add(fsC);
                context.SaveChanges();
            }
            context.Questions.AddRange(
                // primul formular
                new Question
                {
                    SectionId = fsB.FormSectionId, //B
                    QuestionType = QuestionType.SingleOption,
                    Text = $"{f.FormId}: Iti place berea? (se alege o singura optiune selectabila)",
                    OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion {OptionId = 1},
                        new OptionToQuestion {OptionId = 2, Flagged = true},
                        new OptionToQuestion {OptionId = 3}
                    }
                },
                 new Question
                 {
                     SectionId = fsB.FormSectionId, //B
                     QuestionType = QuestionType.MultipleOption,
                     Text = $"{f.FormId}: Ce tipuri de bere iti plac? (se pot alege optiuni multiple)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { OptionId = 4, Flagged = true},
                        new OptionToQuestion { OptionId = 5},
                        new OptionToQuestion { OptionId = 6}
                    }
                 },
                 new Question
                 {
                     SectionId = fsC.FormSectionId, //C
                     QuestionType = QuestionType.SingleOptionWithText,
                     Text = $"{f.FormId}: Ce tip de transmisie are masina ta? (se poate alege O singura optiune selectabila + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { OptionId = 7, Flagged = true},
                        new OptionToQuestion {OptionId = 8},
                        new OptionToQuestion { OptionId = 9}
                    }
                 },
                 new Question
                 {
                     SectionId = fsC.FormSectionId, //C
                     QuestionType = QuestionType.MultipleOptionWithText,
                     Text = $"{f.FormId}: Ce mijloace de transport folosesti sa ajungi la birou? (se pot alege mai multe optiuni + text pe O singura optiune)",
                     OptionsToQuestions = new List<OptionToQuestion>
                    {
                        new OptionToQuestion { OptionId = 10, Flagged = true},
                        new OptionToQuestion { OptionId = 11},
                        new OptionToQuestion { OptionId = 12},
                        new OptionToQuestion { OptionId = 9}
                    }
                 }
                );

            context.SaveChanges();

        }

        private static void SeedForms(this CaseFileContext context, string formCode, bool diaspora)
        {
            context.Forms.Add(
                 new Form { Code = formCode, Description = "Description " + formCode, CurrentVersion = 1, Draft = false, CreatedByUserId = 1, Type = FormType.Public } //, Diaspora = diaspora
             );

            context.SaveChanges();
        }

        private static void SeedNGOs(this CaseFileContext context)
        {
            if (context.Ngos.Any())
            {
                return;
            }

            context.Ngos.Add(new Ngo
            {
                Name = "FDP Cluj",
                //Organizer = true,
                ShortName = "FDP",
                IsActive = true
            });
            context.Ngos.Add(new Ngo
            {
                Name = "Guest NGO",
                //Organizer = false,
                ShortName = "GUE",
                IsActive = true
            });
            context.SaveChanges();

        }


        private static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}
