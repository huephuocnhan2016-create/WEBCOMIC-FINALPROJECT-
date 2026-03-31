namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class UserMangaUnlock
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MangaId { get; set; }
        public DateTime UnlockDate { get; set; } = DateTime.Now;
    }
}
