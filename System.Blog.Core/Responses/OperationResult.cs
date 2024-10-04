using System.Text.Json.Serialization;

namespace System.Blog.Core.Responses;

public class OperationResult<T>
{
    public T? Result { get; set; }
    public bool ReqSuccess { get; set; } = true;
    public string? Message { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; } = 200;
}
