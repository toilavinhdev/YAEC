namespace YAEC.Shared.ValueObjects;

public interface IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
}

public class Pagination
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public int TotalItem { get; set; }
    
    public long TotalPage => (long)Math.Ceiling(TotalItem / (double)PageSize);
}

public class PagedList<T>
{
    public Pagination? Pagination { get; set; }
    
    public List<T> Items { get; set; } = [];
}