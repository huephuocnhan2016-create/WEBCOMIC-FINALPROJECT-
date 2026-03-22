namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class MangaImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Order { get; set; } // Thứ tự trang 1, 2, 3...

        // Foreign Key
        public int ChapterId { get; set; }

        // --- Add this Navigation Property ---
        public Chapter Chapter { get; set; }
    }
}
