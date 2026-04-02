namespace UTB.Minute.Contracts
{
    public record MinuteMealDto (int MinuteMealId, string Desc, double Price, bool IsActive);
    public record MinuteMealRequestDto(string Desc, double Price, bool IsActive);
    public record MinuteMealPatchDescDto(string Desc);
    public record MinuteMealPatchPriceDto(double Price);
    public record MinuteMealPatchIsActiveDto(bool IsActive);
}
