using Ecommerce.Core.Common;

namespace Ecommerce.Core.Entities;

public class Store : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool State { get; set; } = true;

    public void ChangeState()
    {
        State = !State;
    }

    public void SetName(string name)
    {
        if (name is null) throw new ArgumentNullException();
        if (name.Count() < 1) throw new ArgumentException("Name length could not be less than 1");
        Name = name;
    }

    public Store(){}

    public Store(string name)
    {
        SetName(name);
    }
}
