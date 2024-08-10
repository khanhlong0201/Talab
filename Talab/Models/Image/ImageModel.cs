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
        public int imageId { get; set; }
        public int? warantyId { get; set; }
        public string link { get; set; }
        public string linkName { get; set; }
        public string type { get; set; }
        public DateTime createdAt { get; set; }
    }

}
