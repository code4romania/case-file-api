using Microsoft.EntityFrameworkCore;
using System;

namespace CaseFile.Entities
{
    public partial class CaseFileContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Beneficiary)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.BeneficiaryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Ngo)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.NgoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BeneficiaryId, e.OptionToQuestionId });
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.OptionAnswered)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.OptionToQuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Beneficiary)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.BeneficiaryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserForm>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BeneficiaryId, e.FormId });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserForms)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.UserForms)
                    .HasForeignKey(d => d.FormId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Beneficiary)
                    .WithMany(p => p.UserForms)
                    .HasForeignKey(d => d.BeneficiaryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(d => d.FormSection)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<OptionToQuestion>(entity =>
            {                
                entity.HasIndex(e => new { e.OptionId, e.QuestionId })
                    .HasName("IX_OptionToQuestion")
                    .IsUnique();

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.OptionsToQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Option)
                    .WithMany(p => p.OptionsToQuestions)
                    .HasForeignKey(d => d.OptionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });            
        }
        
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<County> Counties { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Ngo> Ngos { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<OptionToQuestion> OptionsToQuestions { get; set; }        
        public virtual DbSet<FormSection> FormSections { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Family> Families { get; set; }
        public virtual DbSet<UserForm> UserForms { get; set; }
        public virtual DbSet<NgoRequest> NgoRequests { get; set; }
        public virtual DbSet<Report> Reports { get; set; }

    }
}