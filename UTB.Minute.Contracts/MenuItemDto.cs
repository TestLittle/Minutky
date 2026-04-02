namespace UTB.Minute.Contracts
{
    public record MenuItemDto(int MenuItemId, DateTime Date, int Portions, int MinuteMealId);
    public record MenuItemRequestDto(DateTime Date, int Portions, int MinuteMealId);
    public record MenuItemPatchDateDto(DateTime Date);
    public record MenuItemPatchPortionsDto(int Portions);
    public record MenuItemPatchMeal(int MinuteMealId);
}
