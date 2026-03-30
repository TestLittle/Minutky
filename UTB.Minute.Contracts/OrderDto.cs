using UTB.Minute.Db;

namespace UTB.Minute.Contracts
{
    public record OrderDto(int OrderId, OrderStatus status, int MenuItemId);
    //K requestu dodat id foreign keys
}
