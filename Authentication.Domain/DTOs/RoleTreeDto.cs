public class RoleTreeDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<RoleTreeDto> Children { get; set; } = new();
}
