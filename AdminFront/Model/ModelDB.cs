namespace MIA.Model
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public partial class ModelDB : DbContext
    {
        public ModelDB()
            : base(DecryptConecctionString())
        {
        }
        public virtual DbSet<UsersAssistants> UsersAssistants { get; set; }
        public virtual DbSet<Assistants> Assistants { get; set; }
        public virtual DbSet<AuditDB> AuditDB { get; set; }
        public virtual DbSet<UsersTypes> TypeUsers { get; set; }
        public virtual DbSet<EmailParameters> EmailParameters { get; set; }
        public virtual DbSet<AuditTicket> AuditTickets { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RpaParameter> RpaParameters { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersRole> UsersRoles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assistants>()
                .HasMany(e => e.UsersAssistants)
                .WithRequired(e => e.Assistants)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assistants>()
                .HasMany(e => e.RpaParameters)
                .WithRequired(e => e.Assistants)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assistants>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.Assistants)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.UsersRoles)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>()
                .HasMany(e => e.AuditTickets)
                .WithRequired(e => e.State)
                .HasForeignKey(e => e.StartStateAuditTicket)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>()
                .HasMany(e => e.AuditTickets1)
                .WithRequired(e => e.State1)
                .HasForeignKey(e => e.EndStateAuditTicket)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>()
                .HasMany(e => e.Tickets)
                .WithRequired(e => e.State)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>()
                .HasMany(e => e.AuditTickets)
                .WithRequired(e => e.Ticket)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AuditTickets)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersRoles)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersAssistants)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UsersTypes>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.TypeUser)
                .WillCascadeOnDelete(false);
        }
        public static string DecryptConecctionString()
        {
            HashAlgorithm hash = MD5.Create();
            string cipherText = ConfigurationManager.ConnectionStrings["ModelDB"].ToString();
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = hash.ComputeHash(Encoding.UTF8.GetBytes("CedexGenerico"));
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

}
