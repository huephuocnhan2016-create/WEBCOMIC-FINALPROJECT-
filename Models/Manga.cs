using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Manga
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bộ truyện")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả bộ truyện")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL ảnh bìa")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public int GenreId { get; set; }

        [ValidateNever]
        public virtual Genre? Genre { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsVipOnly { get; set; } = false;
        public bool IsNovel { get; set; }
        public string? AuthorName { get; set; }

        // Thêm dòng này để hết lỗi 'LuotXem' (đổi thành ViewCount)
        public int ViewCount { get; set; } = 0;

        [ValidateNever]
        public string? AuthorId { get; set; }

        [ValidateNever]
        public virtual ApplicationUser? Author { get; set; }

        [ValidateNever]
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    } 
} 