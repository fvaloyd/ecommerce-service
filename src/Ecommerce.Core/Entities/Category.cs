namespace Ecommerce.Core.Entities;

public class Category : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool State { get; set; }

    public void ChangeState(bool newState)
    {
        State = newState;
    }

    public void ModifyName(string newName)
    {
        Name = newName;
    }
}
