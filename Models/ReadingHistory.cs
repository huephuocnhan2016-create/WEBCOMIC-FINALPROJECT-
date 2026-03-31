using System.ComponentModel.DataAnnotations;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class ReadingHistory
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int MangaId { get; set; }
        public DateTime LastRead { get; set; } = DateTime.Now;

        // Quan hệ liên kết
        public virtual ApplicationUser? User { get; set; }
        public virtual Manga? Manga { get; set; }
    }
}