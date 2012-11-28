
namespace Federation.Core
{
    public static class SearchService
    {
        private static ISearchService _current = new SearchServiceImpl();
    }
}
