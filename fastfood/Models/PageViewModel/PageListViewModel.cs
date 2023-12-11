
namespace fastfood.Models.PageViewModel
{
    public class PageListViewModel
    {
        public IEnumerable<Page> Pages { get; set; } = Enumerable.Empty<Page>();
        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}
