using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CORE_TALAB.Models.Table
{
    public class auth 
    {
        [Key]
        [Column("auth_id")]
        public int auth_id { get; set; }
        [Column("key")]
        public string key { get; set; }
        [Column("name")]
        public string name { get; set; }
    }
}