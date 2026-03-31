namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class QuizViewModel
    {
        public int CorrectMangaId { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
