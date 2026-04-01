using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Manga
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bộ truyện")]
        [StringLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả bộ truyện")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL ảnh bìa")]
        public string? ImageUrl { get; set; } // Cho phép null nếu chưa chọn ảnh

        [NotMapped] // Đánh dấu không tạo cột trong Database
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }

        // Lượt xem - Thêm dòng này để hết lỗi 'ViewCount'
        [Display(Name = "Lượt xem")]
        public int ViewCount { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public int GenreId { get; set; }

        [ValidateNever]
        [ForeignKey("GenreId")]
        public virtual Genre? Genre { get; set; }

        public bool IsApproved { get; set; } = false;

        [ValidateNever]
        public string? AuthorId { get; set; }

        public bool IsVipOnly { get; set; } = false;

        public string? AuthorName { get; set; }

        // Quan hệ 1-Nhiều với Chapter
        [ValidateNever]
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}