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
    public class SearchQueryModel
    {
        public SearchModel SearchModel { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } 
        public string Sort { get; set; }
        public string SortDirection { get; set; }
    }
    public class SearchModel
    {
        public string SearchString { get; set; }
        public DateTime? FromDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime? ToDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
    }

}
