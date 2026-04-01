using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Novel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên truyện")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả bộ truyện")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL ảnh bìa")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public int GenreId { get; set; }

        [ValidateNever]
        public virtual Genre? Genre { get; set; }

        public bool IsApproved { get; set; } = false;

        public string? AuthorId { get; set; }

        public bool IsVipOnly { get; set; } = false;

        [ValidateNever]
        public virtual ICollection<NovelChapter> Chapters { get; set; } = new List<NovelChapter>();
    }
}
