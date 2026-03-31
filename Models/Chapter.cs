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
        public string Title { get; set; } // Dùng Title để khớp với View

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int MangaId { get; set; }

        [ForeignKey("MangaId")]
        [ValidateNever]
        public virtual Manga? Manga { get; set; }

        public int ChapterNumber { get; set; }

        // Danh sách ảnh phục vụ đọc truyện và Daily Quiz
        [ValidateNever]
        public virtual ICollection<MangaImage> Images { get; set; } = new List<MangaImage>();

        public string? Content { get; set; } // Thêm để khớp với cột trong DB của bạn
    }
}