namespace Instrux.Services.DTOs;

public class CreateTodoItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
}
