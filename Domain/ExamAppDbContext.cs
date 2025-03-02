using Domain.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
   public class ExamAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public ExamAppDbContext()
        {
                
        }
      
        public ExamAppDbContext(DbContextOptions<ExamAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-13L63PN;Initial Catalog=ExamApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
        }



        public virtual DbSet<ApplicationUser> users { get; set; }
        public virtual DbSet<TbExam> Exams { get; set; }
        public virtual DbSet<TbExamResult> ExamResults { get; set; }
        public virtual DbSet<TbQuestion> Questions { get; set; }
        public virtual DbSet<TbAnswer> Answers { get; set; }
        public virtual DbSet<TbUserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<TbExam>(entity =>
            {
                entity.HasKey(e => e.Id);


                entity.Property(e => e.Title).IsRequired(false);


                entity.Property(e => e.CreatedBy).HasColumnType("nvarchar(200)").IsRequired(true);


                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())").IsRequired(true);

                entity.Property(e => e.UpdatedDate)
                  .HasColumnType("datetime");


            });



            base.OnModelCreating(modelBuilder);
        }
        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
