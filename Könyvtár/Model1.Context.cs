﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Könyvtár
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class book_vs19Entities1 : DbContext
    {
        public book_vs19Entities1()
            : base("name=book_vs19Entities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<InformationBundle> InformationBundle { get; set; }
        public virtual DbSet<konyv> konyv { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Reader_Card> Reader_Card { get; set; }
        public virtual DbSet<Relation_AuthorWriterBook> Relation_AuthorWriterBook { get; set; }
        public virtual DbSet<Relation_BookCategorie> Relation_BookCategorie { get; set; }
        public virtual DbSet<Relation_UserBook> Relation_UserBook { get; set; }
        public virtual DbSet<Relation_UserCategorie> Relation_UserCategorie { get; set; }
        public virtual DbSet<Rent> Rent { get; set; }
        public virtual DbSet<staff_role> staff_role { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<Writer> Writer { get; set; }
    }
}
