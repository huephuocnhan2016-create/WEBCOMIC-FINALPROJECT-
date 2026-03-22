namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Manga
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; } // Dùng cho Quiz
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public bool IsApproved { get; set; } // Quản trị viên duyệt
        public string AuthorId { get; set; } // Link tới User là Tác giả
        public bool IsVipOnly { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
