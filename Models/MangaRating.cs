namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class MangaRating
    {
        public int Id { get; set; }
        public int Score { get; set; } // Điểm từ 1 đến 5 sao
        public int MangaId { get; set; }
        public string UserId { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }

    public class MangaChapterRating
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int ChapterId { get; set; }
        public string UserId { get; set; }
        public virtual Chapter? Chapter { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
