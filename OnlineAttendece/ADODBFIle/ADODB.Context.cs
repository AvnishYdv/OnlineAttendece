﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineAttendece.ADODBFIle
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Office_AttendanceEntities : DbContext
    {
        public Office_AttendanceEntities()
            : base("name=Office_AttendanceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Holiday_Master> Holiday_Master { get; set; }
        public virtual DbSet<Working_day_Master> Working_day_Master { get; set; }
        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<AdminLogin> AdminLogins { get; set; }
        public virtual DbSet<Employee_Master> Employee_Master { get; set; }
        public virtual DbSet<Attendence_Master> Attendence_Master { get; set; }
        public virtual DbSet<Office_Master> Office_Master { get; set; }
    }
}
