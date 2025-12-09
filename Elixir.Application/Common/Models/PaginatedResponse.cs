namespace Elixir.Application.Common.Models;
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public PaginationMetadata Metadata { get; set; }

    public PaginatedResponse(List<T> data, PaginationMetadata metadata)
    {
        Data = data;
        Metadata = metadata;
    }
}
