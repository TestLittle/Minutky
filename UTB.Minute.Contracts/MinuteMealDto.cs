namespace UTB.Minute.Contracts
{
    public record MinuteMealDto (int MinuteMealId, string Desc, double Price, bool isActive);
    public record MinuteMealRequestDto(string Desc, double Price, bool isActive);
    public record MinuteMealPatchDescDto(string Desc);
    public record MinuteMealPatchPriceDto(double Price);
    public record MinuteMealPatchisActiveDto(bool isActive);
}
