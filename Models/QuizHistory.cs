using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class QuizHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime DatePlayed { get; set; }

        [Required]
        public string QuizType { get; set; }

        // Thiết lập mối quan hệ với bảng User (Nếu cần)
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}