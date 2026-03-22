namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Khóa ngoại trỏ tới Manga
        public int MangaId { get; set; }
        public virtual Manga Manga { get; set; }

        // Danh sách ảnh của chương này
        public virtual ICollection<MangaImage> Images { get; set; } = new List<MangaImage>();
    }
}