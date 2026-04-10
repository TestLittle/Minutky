using UTB.Minute.Db;

namespace UTB.Minute.Contracts
{
    public record OrderDto(int OrderId, OrderStatus status, int MenuItemId);
    public record OrderRequestDto(OrderStatus status, int MenuItemId);
    public record OrderPatchStatusDto(OrderStatus status);
    public record OrderPatchMenuItemDto(int MenuItemId);
}
