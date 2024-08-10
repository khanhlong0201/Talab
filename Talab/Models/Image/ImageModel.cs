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

namespace Talab.Model.Image
{
    public class UploadImageModel
    {
        public List<IFormFile> Image { get; set; }
    }
    public class ImageModel
    {
        public int image_id { get; set; }
        public int? waranty_id { get; set; }
        public string link { get; set; }
        public string link_name { get; set; }
        public string type { get; set; }
        public DateTime created_at { get; set; }
    }

}
