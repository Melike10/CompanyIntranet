using System.ComponentModel.DataAnnotations;

namespace CompanyIntranet.DTOs
{
    public class AnnouncementDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık boş bırakılamaz")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik boş bırakılamaz")]
        public string Content { get; set; }

    }
}
