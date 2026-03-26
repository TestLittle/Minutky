namespace UTB.Minute.Contracts
{
    public record MenuItemDto(int MenuItemId, DateTime Date, int Portions);
    public record MenuItemRequestDto(DateTime Date, int Portions);
    public record MenuItemPatchDateDto(DateTime Date);
    public record MenuItemPatchPortionsDto(int Portions);
}
