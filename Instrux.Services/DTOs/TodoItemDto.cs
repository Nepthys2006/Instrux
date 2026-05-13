namespace Instrux.Services.DTOs;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
}
