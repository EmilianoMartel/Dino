using Raylib_cs;
using System.Numerics;

public class PruebasRayLib
{
    static int screenWidth = 1240;
    static int screenHeight = 720;

    static float delta = 0;

    //Image
    static Image dino = Raylib.LoadImage("assets/dino.png");
    static Image cactus = Raylib.LoadImage("assets/cactus.png");
    static Image floor = Raylib.LoadImage("assets/floor.png");

    //const
    const float FLOOR_POSITION = 360;

    //Player Position
    static Vector2 position = new Vector2(-1000, FLOOR_POSITION);

    //Cactus
    static List<Vector2> cactusPosition = new List<Vector2>();
    const float Y_POSITION_CACTUS = 10f;
    static float treshold = cactus.width * 2;


    //Movement
    static float movement = 50f;

    //Jump
    static bool isJumping = false;
    static float playerVerticalSpeed = 0f;
    const float GRAVITY = 50f;
    const float JUMP_IMPULSE = 200f;
    static float playerFloorPosition = FLOOR_POSITION;

    public static void Main()
    {
        Console.WriteLine(screenHeight / 2);
        Random random = new Random();

        Raylib.InitWindow(screenWidth, screenHeight, "Dino");

        Texture2D dinoTexture = Raylib.LoadTextureFromImage(dino);
        Raylib.UnloadImage(dino);

        Texture2D cactusTexture = Raylib.LoadTextureFromImage(cactus);
        Raylib.UnloadImage(cactus);

        Texture2D floorTexture = Raylib.LoadTextureFromImage(floor);
        Raylib.UnloadImage(floor);

        treshold = cactusTexture.width;

        AddRandomCactus();

        Camera2D camera = new Camera2D();
        camera.target = new Vector2(0, 0);
        camera.offset = new Vector2(screenWidth / 2, screenHeight / 2);
        camera.rotation = 0;
        camera.zoom = .5f;

        while (!Raylib.WindowShouldClose())
        {
            UpdatePlayer();

            Raylib.BeginDrawing();

            delta = Raylib.GetFrameTime();

            Raylib.ClearBackground(Color.SKYBLUE);

            /*Draw Gameplay - start*/
            Raylib.BeginMode2D(camera);

            DrawSky();

            Rectangle bounds = new Rectangle(position.X, position.Y, dinoTexture.width, dinoTexture.height);
            Raylib.DrawRectangle(-screenWidth / 2, 500, screenWidth, screenHeight/3, Color.GREEN);
            Raylib.DrawRectangle((int)bounds.x - (int)bounds.width / 2, (int)bounds.y - (int)bounds.height / 2, dinoTexture.width, dinoTexture.height, Color.RED);
            Raylib.DrawTexture(dinoTexture, (int)position.X - dinoTexture.width / 2, (int)position.Y - dinoTexture.height / 2, Color.WHITE);
            UpdateCactus(cactusTexture);
            Raylib.EndMode2D();
            /*Draw Gameplay - end*/

            /*Draw UI - start*/

            /*Draw UI - end*/
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private static void UpdatePlayer()
    {
        Jump();
    }

    private static void DrawSky()
    {
        for (int i = -100; i < 100; i++)
        {
            Raylib.DrawRectangle(i * 100, -1000, 100, screenHeight*4, (i % 2 == 0 ? Color.SKYBLUE : Color.DARKBLUE));
        }
    }

    static void Jump()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && !isJumping)
        {
            playerVerticalSpeed = -JUMP_IMPULSE;
            playerFloorPosition = position.Y;
            isJumping = true;
        }

        if (isJumping)
        {
            position.Y += playerVerticalSpeed * delta;
            playerVerticalSpeed += GRAVITY * delta;

            if (position.Y >= playerFloorPosition)
            {
                playerVerticalSpeed = 0;
                isJumping = false;
            }
        }
    }

    static void UpdateCactus(Texture2D cactusTexture)
    {
        Raylib.DrawTexture(cactusTexture, 0, screenHeight / 2, Color.WHITE);
        Raylib.DrawTexture(cactusTexture, 0 + (int)treshold, screenHeight / 2, Color.WHITE);
        for (int i = 0; i < cactusPosition.Count; i++)
        {
            Vector2 positionCactus = cactusPosition[i];
            positionCactus.X -= movement * delta;
            cactusPosition[i] = positionCactus;
            Rectangle bounds = new Rectangle(positionCactus.X, positionCactus.Y, cactusTexture.width, cactusTexture.height);
            Raylib.DrawRectangle((int)bounds.x, (int)bounds.y - cactusTexture.height / 2, cactusTexture.width, cactusTexture.height, Color.RED);
            Raylib.DrawTexture(cactusTexture, (int)positionCactus.X, (int)positionCactus.Y - cactusTexture.height / 2, Color.WHITE);

        }
    }

    static void AddRandomCactus()
    {
        Vector2 position = new Vector2(GetRandomPosition(), (screenHeight / 2) + 20);
        cactusPosition.Add(position);
    }

    static void RelocateCactus()
    {

    }

    static int GetRandomPosition()
    {
        Random random = new Random();

        return random.Next(screenWidth,screenWidth+screenWidth/2);
    }
}