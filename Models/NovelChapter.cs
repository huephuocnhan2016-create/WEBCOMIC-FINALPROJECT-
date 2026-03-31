using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class NovelChapter
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chương")]
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int NovelId { get; set; }

        [ValidateNever]
        public virtual Novel? Novel { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung chương")]
        public string Content { get; set; }

        public bool IsVipOnly { get; set; } = false;
    }
}
