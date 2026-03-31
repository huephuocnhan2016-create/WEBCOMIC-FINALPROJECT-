using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chương")]
        public string Title { get; set; }

        // --- THÊM DÒNG NÀY ĐỂ HẾT LỖI ---
        [Required(ErrorMessage = "Vui lòng nhập số chương")]
        public int ChapterNumber { get; set; }
        // --------------------------------

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int MangaId { get; set; }

        [ForeignKey("MangaId")]
        [ValidateNever]
        public virtual Manga? Manga { get; set; }

        [ValidateNever]
        public virtual ICollection<MangaImage> Images { get; set; } = new List<MangaImage>();

        public string? Content { get; set; }
    }
}