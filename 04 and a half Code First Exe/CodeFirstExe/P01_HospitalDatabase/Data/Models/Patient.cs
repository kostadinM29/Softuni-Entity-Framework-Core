using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
   public class Patient
    {
        public Patient()
        {
            Diagnoses = new HashSet<Diagnose>();
            Visitations = new HashSet<Visitation>();
            Prescriptions = new HashSet<PatientMedicament>();
        }

        [Key]
        public int PatientId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "varchar(80)")]
        public string Email { get; set; }

        [Required]
        public bool HasInsurance { get; set; }


        public virtual ICollection<Diagnose> Diagnoses { get; set; }

        public virtual ICollection<Visitation> Visitations { get; set; }

        public virtual ICollection<PatientMedicament> Prescriptions { get; set; }

    }
}
