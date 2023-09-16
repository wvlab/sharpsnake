using Raylib_CsLo;
using System.Numerics;

namespace SharpSnake;

public sealed class Game
{
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 450;
    const int SquareSize = 31;
    const int Pad = ScreenWidth % SquareSize;
    const int SnakeLength = 256;

    Food Fruit;
    Snake[] Player;
    Vector2[] SnakePosition;

    int FramesCounter;
    int CounterTail;

    bool GameOver;
    bool Pause;
    bool AllowMove;

    Vector2 Offset;

    void HandleMovementKeys()
    {
        if (!AllowMove)
        {
            return;
        }

        AllowMove = false;

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT) && (Player[0].speed.X == 0))
        {
            Player[0].speed.X = SquareSize;
            Player[0].speed.Y = 0;
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT) && (Player[0].speed.X == 0))
        {
            Player[0].speed.X = -SquareSize;
            Player[0].speed.Y = 0;
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP) && (Player[0].speed.Y == 0))
        {
            Player[0].speed.X = 0;
            Player[0].speed.Y = -SquareSize;
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN) && (Player[0].speed.Y == 0))
        {
            Player[0].speed.X = 0;
            Player[0].speed.Y = SquareSize;
        }
        else
        {
            AllowMove = true;
        }
    }

    void HandleMovement()
    {
        HandleMovementKeys();

        if ((FramesCounter % 10) == 0)
        {
            for (int i = 0; i < CounterTail; i++)
            {
                SnakePosition[i] = Player[i].position;
            }

            Player[0].position += Player[0].speed;
            AllowMove = true;

            for (int i = 1; i < CounterTail; i++)
            {
                Player[i].position = SnakePosition[i - 1];
            }
        }
    }

    void CheckWallCollision()
    {
        if (
            ((Player[0].position.X) > (ScreenWidth - Offset.X))
            || ((Player[0].position.Y) > (ScreenHeight - Offset.Y))
            || (Player[0].position.X < 0)
            || (Player[0].position.Y < 0)
        )
        {
            GameOver = true;
        }

    }

    void ActivateFruit()
    {
        Fruit.active = true;

        Fruit.Generate(ScreenWidth, ScreenHeight, SquareSize, Offset);

        for (int i = 0; i < CounterTail; i++)
        {
            while (Fruit.position == Player[i].position)
            {
                Fruit.Generate(ScreenWidth, ScreenHeight, SquareSize, Offset);
                i = 0;
            }
        }
    }

    void CheckFruitCollision()
    {
        if (
            (Player[0].position.X < (Fruit.position.X + Fruit.size.X)
            && (Player[0].position.X + Player[0].size.X) > Fruit.position.X)
            && (Player[0].position.Y < (Fruit.position.Y + Fruit.size.Y)
            && (Player[0].position.Y + Player[0].size.Y) > Fruit.position.Y)
        )
        {
            Player[CounterTail].position = SnakePosition[CounterTail - 1];
            CounterTail += 1;
            Fruit.active = false;
        }
    }

    public void Update()
    {
        if (GameOver)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                Reset();
            }
            return;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
        {
            this.Pause = !Pause;
        }

        if (Pause) return;

        HandleMovement();

        for (int i = 1; i < CounterTail; i++)
        {
            if (Player[0].position == Player[i].position)
            {
                GameOver = true;
            }
        }

        CheckWallCollision();

        if (!Fruit.active)
        {
            ActivateFruit();
        }

        CheckFruitCollision();

        FramesCounter++;
    }

    void DrawGrid() {
        for (int i = 0; i < ScreenWidth / SquareSize + 1; i++)
        {
            Raylib.DrawLineV(
                new Vector2(SquareSize * i + Offset.X / 2, Offset.Y / 2),
                new Vector2(SquareSize * i + Offset.X / 2, ScreenHeight),
                Raylib.LIGHTGRAY
            );
        }

        for (int i = 0; i < ScreenHeight / SquareSize + 1; i++)
        {
            Raylib.DrawLineV(
                new Vector2(Offset.X / 2, SquareSize * i + Offset.Y / 2),
                new Vector2(ScreenWidth - Offset.X / 2, SquareSize * i + Offset.Y / 2),
                Raylib.LIGHTGRAY
            );
        }
    }

    void DrawSnake()
    {
        for (int i = 0; i < CounterTail; i++)
        {
            Raylib.DrawRectangleV(Player[i].position, Player[i].size, Player[i].color);
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Raylib.RAYWHITE);

        DrawGrid();

        DrawSnake();

        Raylib.DrawRectangleV(Fruit.position, Fruit.size, Fruit.color);

        if (Pause)
        {
            Raylib.DrawText(
                "GAME PAUSED",
                ScreenWidth / 2 - Raylib.MeasureText("GAME PAUSED", 40) / 2,
                ScreenHeight / 2 - 40, 40, Raylib.GRAY
            );
        }

        if (GameOver)
        {

            Raylib.DrawText(
                "YOUR SCORE IS: " + CounterTail.ToString(),
                ScreenWidth / 2 - Raylib.MeasureText("PRESS [ENTER] TO PLAY AGAIN", 20) / 2,
                ScreenHeight / 2 - 20, 16,
                Raylib.GRAY
            );

            Raylib.DrawText(
                "PRESS [ENTER] TO PLAY AGAIN",
                ScreenWidth / 2 - Raylib.MeasureText("PRESS [ENTER] TO PLAY AGAIN", 20) / 2,
                ScreenHeight / 2 - 50, 20,
                Raylib.GRAY
            );
        }

        Raylib.EndDrawing();
    }

    public void Unload()
    {
        return;
    }

    private void Reset()
    {
        FramesCounter = 0;
        CounterTail = 1;
        GameOver = false;
        Pause = false;
        AllowMove = false;

        Offset.X = Pad;
        Offset.Y = Pad;

        for (int i = 0; i < SnakeLength; i++)
        {
            Player[i].position = Offset / 2;

            Player[i].size.X = SquareSize;
            Player[i].size.Y = SquareSize;

            Player[i].speed.X = SquareSize;
            Player[i].speed.Y = 0;

            Player[i].color = Raylib.BLUE;

            SnakePosition[i].X = 0.0f;
            SnakePosition[i].Y = 0.0f;
        }

        Player[0].color = Raylib.BLACK;

        Fruit.size.X = SquareSize;
        Fruit.size.Y = SquareSize;
        Fruit.color = Raylib.RED;
        Fruit.active = false;
    }

    public Game()
    {
        Player = new Snake[SnakeLength];
        SnakePosition = new Vector2[SnakeLength];

        Reset();
    }
}

public struct Snake
{
    public Vector2 position;
    public Vector2 size;
    public Vector2 speed;
    public Color color;
}

public struct Food
{
    public Vector2 position;
    public Vector2 size;
    public bool active;
    public Color color;

    public void Generate(
        int ScreenWidth,
        int ScreenHeight,
        int SquareSize,
        Vector2 Offset
    )
    {
        position.X = Raylib.GetRandomValue(0,
            ((ScreenWidth / SquareSize) - 1)
        ) * SquareSize + Offset.X / 2;
        position.Y = Raylib.GetRandomValue(0, (
            (ScreenHeight / SquareSize) - 1)
        ) * SquareSize + Offset.Y / 2;
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        Raylib.InitWindow(Game.ScreenWidth, Game.ScreenHeight, "Snake");
        Raylib.SetTargetFPS(60);

        Game game = new Game();
        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsWindowResized() || Raylib.IsWindowMaximized())
            {
                Raylib.SetWindowSize(Game.ScreenWidth, Game.ScreenHeight);
            }
            game.Update();
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}
