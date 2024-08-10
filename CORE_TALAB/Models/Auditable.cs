using System;

namespace CORE_TALAB.Models
{
    public class Auditable
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public short state { get; set; }
    }
}
