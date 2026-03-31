using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class ReadingHistory
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int MangaId { get; set; }

        // Đổi ReadDate thành LastRead để khớp với code đang bị lỗi
        public DateTime LastRead { get; set; } = DateTime.Now;

        [ForeignKey("MangaId")]
        public virtual Manga? Manga { get; set; }
    }
}