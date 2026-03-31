using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class NovelComment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Liên kết với Chương truyện chữ
        public int NovelChapterId { get; set; }
        [ForeignKey("NovelChapterId")]
        public virtual NovelChapter? NovelChapter { get; set; }

        // Liên kết với người dùng
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}