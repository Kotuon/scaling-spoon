
namespace Game.Entity;

using Godot;

public partial class AreaKey : Entity.Key
{
    protected override void ResolveCollisionEnter(Node node)
    {
        base.ResolveCollisionEnter(node);

        completed = true;
    }

}
