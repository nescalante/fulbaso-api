namespace Fulbaso.Helpers
{
    public interface IEntity : IEntityWithId
    {
        string Description { get; set; }

        bool IsActive { get; set; }

        string PrimaryField { get; }
    }
}
