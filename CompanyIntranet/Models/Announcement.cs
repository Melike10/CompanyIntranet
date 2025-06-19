namespace CompanyIntranet.Models
{
    public class Announcement:BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
    }
}
