namespace TodoApi;

public record TodoGetDto
(
    int id,
    string Title,
    bool IsComplete
);