using CORE_TALAB.Common;
using CORE_TALAB.Enum;
using CORE_TALAB.EventEnums;
using CORE_TALAB.Models.Table;
using CORE_TALAB.Services;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Talab.Model.Warranty
{
    public class WarrantyModel
    {
        public int WarrantyId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string Clinic { get; set; }
        public string LabName { get; set; }
        public string Doctor { get; set; }
        public string Product { get; set; }
        public string CodeNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public short State { get; set; }
        public List<string> ListImageUrl { get; set; }
    }
}
