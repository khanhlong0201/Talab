using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CORE_TALAB.Models.Table
{
    public class warrantys : Auditable
    {
        [Key]
        [Column("warranty_id")]
        public int warrantyId { get; set; }

        [Column("patientname")]
        public string patientName { get; set; }

        [Column("patientphonenumber")]
        public string patientPhoneNumber { get; set; }

        [Column("clinnic")]
        public string clinic { get; set; }

        [Column("labname")]
        public string labName { get; set; }

        [Column("doctor")]
        public string doctor { get; set; }

        [Column("product")]
        public string product { get; set; }

        [Column("codenumber")]
        public string codeNumber { get; set; }

        [Column("expirationdate")]
        public DateTime? expirationDate { get; set; }
    }
}