namespace UTB.Minute.Contracts
{
    public record MinuteMealDto (int MinuteMealId, string Desc, double Price);
    public record MinuteMealRequestDto(string Desc, double Price);
    public record MinuteMealPatchDescDto(string Desc);
    public record MinuteMealPatchPriceDto(double Price);
}
