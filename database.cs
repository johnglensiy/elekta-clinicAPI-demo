using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Models;

public class ClinicDb : DbContext
{
    public ClinicDb(DbContextOptions<ClinicDb> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
}
public class Patient
    {
        [Key]
        public int MedicalRecordNumber { get; set; }
        public string? Name { get; set; }
        public string? AdmittingDiagnosis { get; set; }
        public string? Department { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? Contacts { get; set; }
    }
