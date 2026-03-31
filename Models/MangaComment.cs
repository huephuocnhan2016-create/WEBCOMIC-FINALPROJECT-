using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class MangaComment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Liên kết với Chương truyện tranh
        public int ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        // Liên kết với người dùng (IdentityUser)
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}