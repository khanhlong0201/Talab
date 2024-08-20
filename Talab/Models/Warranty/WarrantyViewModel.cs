using Talab.Model.Image;

namespace Talab.Model.Warranty
{
    public class WarrantyModel 
    {
        public int? WarrantyId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string Clinic { get; set; }
        public string LabName { get; set; }
        public string Doctor { get; set; }
        public string Product { get; set; }
        public string CodeNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public short State { get; set; }
        public List<IFormFile> ImageSrcList { get; set; } = new List<IFormFile>();
    }
    public class WarrantyViewModel : WarrantyModel
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class BasicFile
    {
        public string Id { get; set; }  
        public string Title { get; set; }
        public string Src { get; set; }
    }
    public class WarrantyReponseModel : WarrantyViewModel
    {
        public List<BasicFile> FileWithSrcList { get; set; }
        public List<ImageModel> ListImages { get; set; }
        public List<BasicFile> ImageSrcPreviewList { get; set; } = new List<BasicFile>();
        public List<string> ImageLinkNameSrcPreviewList { get; set; } = new List<string>();

    }
}
