using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Model;

namespace StudentManagementSys.Data
{
    public class StudentManagementSysContext : IdentityDbContext
    {
        public StudentManagementSysContext (DbContextOptions<StudentManagementSysContext> options)
            : base(options)
        {
        }

        public DbSet<StudentManagementSys.Model.Classroom> Classroom { get; set; } = default!;

        public DbSet<StudentManagementSys.Model.Student> Student { get; set; } = default!;

        public DbSet<StudentManagementSys.Model.Item> Item { get; set; } = default!;

        public DbSet<StudentManagementSys.Model.Staff> Staff { get; set; } = default!;

        public DbSet<StudentManagementSys.Model.Store> Store { get; set; } = default!;

        public DbSet<StudentManagementSys.Model.ClassSubject> ClassSubject { get; set; } = default!;
    }
}
