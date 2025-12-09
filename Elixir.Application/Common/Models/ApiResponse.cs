using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elixir.Application.Common.Models;
public class ApiResponse<T>
{
    //as per the frontend's expectations, the response should have these properties
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("data")]
    public T? Data { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new List<string>();
    [JsonPropertyName("pagination")]
    public PaginationMetadata? Pagination { get; set; }

    public ApiResponse() { }
    public ApiResponse(int statusCode, string message, bool success, T data, List<string>? errors = null, PaginationMetadata? pagination = null)
    {
        StatusCode = statusCode;
        Message = message;
        Success = success;
        Data = data;
        Errors = errors ?? new List<string>();
        Pagination = pagination;
    }
}