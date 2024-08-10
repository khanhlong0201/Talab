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

namespace Talab.Model.Search
{
    public class SearchModel
    {
        public string patientName { get; set; }
        public string patientPhoneNumber { get; set; }
        public string clinic { get; set; }
        public string labName { get; set; }
        public string doctor { get; set; }
        public string product { get; set; }
        public string codeNumber { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }

}
