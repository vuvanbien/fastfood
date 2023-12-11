namespace fastfood.Models.PageViewModel
{
    public class PageInfo
    {
        public int TotalPage { get; set; }
        public int PagePerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalPage / PagePerPage);
    }
}
