namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class NovelRating
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int NovelId { get; set; }
        public string UserId { get; set; }
        public virtual Novel? Novel { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }

    public class NovelChapterRating
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int NovelChapterId { get; set; }
        public string UserId { get; set; }
        public virtual NovelChapter? NovelChapter { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
