using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class SpawnerWaveComponent : Node
{
    [Export] public int Margin = 8;
    [Export] public Node2D EnemyContainer;
    [Export] public Node2D DropContainer;
    [Export] public Node2D EffectContainer;

    private int _leftBorder;
    private int _rightBorder;

    public override void _Ready()
    {
        _leftBorder = Margin;
        _rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") - Margin;
    }

    public void SpawnWave(SpawnerComponent spawner, int count, int spacing)
    {
        PackedScene enemyScene = spawner.Scene;
        int spriteWidth = GetSpriteWidth(enemyScene);
        int screenCenter = (_leftBorder + _rightBorder) / 2;
        int totalSpacing = spriteWidth + spacing;

        for (int i = 0; i < count; i++)
        {
            int offset = (i - count / 2) * totalSpacing;
            Vector2 spawnPosition = new Vector2(screenCenter + offset, -120);
            Node2D enemy = enemyScene.Instantiate<Node2D>();
            enemy.GlobalPosition = spawnPosition;
            enemy.AddToGroup("spawn_wave");

            if (enemy.HasNode("DropComponent"))
            {
                var drop = enemy.GetNode<DropComponent>("DropComponent");
                drop.DropTarget = DropContainer;
                drop.EffectTarget = EffectContainer;
            }

            if (enemy.HasNode("DestroyedComponent"))
            {
                var effect = enemy.GetNode<DestroyedComponent>("DestroyedComponent");
                effect.EffectTarget = EffectContainer;
            }

            EnemyContainer.AddChild(enemy);
        }
    }

    public async Task SpawnWaveUntilCleared(SpawnerComponent spawner, int count, int spacing)
    {
        PackedScene enemyScene = spawner.Scene;
        int spriteWidth = GetSpriteWidth(enemyScene);
        int screenCenter = (_leftBorder + _rightBorder) / 2;
        int totalSpacing = spriteWidth + spacing;

        List<Node> spawnedEnemies = new();

        for (int i = 0; i < count; i++)
        {
            int offset = (i - count / 2) * totalSpacing;
            Vector2 spawnPosition = new Vector2(screenCenter + offset, -120);

            Node2D enemy = enemyScene.Instantiate<Node2D>();
            enemy.GlobalPosition = spawnPosition;
            enemy.AddToGroup("spawn_wave");

            if (enemy.HasNode("DropComponent"))
            {
                var drop = enemy.GetNode<DropComponent>("DropComponent");
                drop.DropTarget = DropContainer;
                drop.EffectTarget = EffectContainer;
            }

            if (enemy.HasNode("DestroyedComponent"))
            {
                var effect = enemy.GetNode<DestroyedComponent>("DestroyedComponent");
                effect.EffectTarget = EffectContainer;
            }

            EnemyContainer.AddChild(enemy);

            if (enemy.HasNode("PersistenceComponent"))
            {
                var persistence = enemy.GetNode<PersistenceComponent>("PersistenceComponent");
                persistence.ShouldStay = true;
            }

            spawnedEnemies.Add(enemy);
        }

        while (spawnedEnemies.Any(e => IsInstanceValid(e)))
            await Task.Delay(200);
    }

    private int GetSpriteWidth(PackedScene enemyScene)
    {
        int spriteWidth = 0;
        Node2D tempEnemy = enemyScene.Instantiate<Node2D>();
        AnimatedSprite2D sprite = tempEnemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        if (sprite?.SpriteFrames is SpriteFrames frames && frames.HasAnimation("move"))
            spriteWidth = (int)frames.GetFrameTexture("move", 0).GetWidth();

        tempEnemy.QueueFree();
        return spriteWidth;
    }
}
