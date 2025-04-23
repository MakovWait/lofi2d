using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Components;

namespace Tmp.Project;

public class CFood : Component
{
    protected override Components Init(INodeInit self)
    {
        var snake = self.UseContext<Snake>();
        var bounds = self.UseContext<Bounds>();
        var items = new ReactiveList<FoodItem>();

        self.OnMount(SpawnNextFood);

        self.On<Update>(_ =>
        {
            var spawnNextFood = false;
            foreach (var foodItem in items)
            {
                if (foodItem.Position.DistanceSquaredTo(snake.Head.Transform.Origin) < 8 * 8)
                {
                    items.QueueRemove(foodItem);
                    spawnNextFood = true;
                    snake.AddBodyPart();
                }
            }
            items.FlushRemoveQueue();
            if (spawnNextFood)
            {
                SpawnNextFood();
            }
        });

        return new For<FoodItem>()
        {
            In = items,
            ItemKey = item => item.Key,
            Render = (item, _) => new CFoodItem(item)
        };

        void SpawnNextFood()
        {
            items.Add(new FoodItem(Guid.NewGuid().ToString())
            {
                Position = bounds.GetRandomFoodPosition()
            });
        }
    }
}

public class FoodItem(string key)
{
    public string Key { get; } = key;

    public required Vector2 Position { get; init; }
}

public class CFoodItem(FoodItem data) : Component
{
    protected override Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D(new Transform2D(0, data.Position));
        var canvasItem = self.UseCanvasItem(transform);

        canvasItem.OnDraw(ctx =>
        {
            ctx.DrawRect(new Rect2I(-8, -8, 16, 16), Color.Red);
        });

        return base.Init(self);
    }
}