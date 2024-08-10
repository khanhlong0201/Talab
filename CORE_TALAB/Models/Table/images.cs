using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CORE_TALAB.Models.Table
{
    public class images 
    {
        [Key]
        [Column("image_id")]
        public int image_id { get; set; }
        [Column("warranty_id")]
        public int warrantyId { get; set; }

        [Column("link")]
        public string link { get; set; }

        [Column("link_name")]
        public string link_name { get; set; }

        [Column("type")]
        public string type { get; set; }
        public DateTime created_at { get; set; }
        public short state { get; set; }
    }
}