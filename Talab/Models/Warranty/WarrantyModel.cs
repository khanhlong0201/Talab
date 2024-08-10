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
        public int warrantyId { get; set; }
        public string patientName { get; set; }
        public string patientPhoneNumber { get; set; }
        public string clinic { get; set; }
        public string labName { get; set; }
        public string doctor { get; set; }
        public string product { get; set; }
        public string codeNumber { get; set; }
        public DateTime? expirationDate { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public short state { get; set; }
        public List<string> listImageUrl { get; set; }
    }
}
