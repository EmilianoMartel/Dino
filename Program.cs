using Raylib_cs;
using System.Numerics;

public class PruebasRayLib
{
    static int screenWidth = 1240;
    static int screenHeight = 720;
    public static void Main()
    {
        
        Raylib.InitWindow(screenWidth, screenHeight, "Dino");

        Image dino = Raylib.LoadImage("assets/dino.png"); 
        Texture2D dinoTexture = Raylib.LoadTextureFromImage(dino); 
        Raylib.UnloadImage(dino);

        Image cactus = Raylib.LoadImage("assets/cactus.png");
        Texture2D cactusTexture = Raylib.LoadTextureFromImage(cactus);
        Raylib.UnloadImage(cactus);

        Image floor = Raylib.LoadImage("assets/floor.jpeg");
        Texture2D floorTexture = Raylib.LoadTextureFromImage(floor);
        Raylib.UnloadImage(floor);

        Vector2 posicion = new Vector2(screenWidth / 2, screenHeight / 2);
        float velocidad = 0.1f;

        Rectangle player = new((int)posicion.X, (int)posicion.Y, dinoTexture.width, dinoTexture.height);

        Camera2D camera = new Camera2D();
        camera.target = new Vector2(player.x - 20, player.y - 20);
        camera.offset = new Vector2(screenWidth / 2, screenHeight / 2);
        camera.rotation = 0;
        camera.zoom = 1;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) posicion.X += velocidad; 
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) posicion.X -= velocidad; 
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) posicion.Y -= velocidad;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) posicion.Y += velocidad; 
            
            Raylib.ClearBackground(Color.SKYBLUE);

            /*Draw Gameplay - start*/
            Raylib.BeginMode2D(camera);

            Raylib.DrawTexture(floorTexture, 0, 0, Color.WHITE);

            Raylib.DrawRectangleRec(player, Color.RED);

            Raylib.DrawTexture(dinoTexture, (int)posicion.X - dinoTexture.width / 2, (int)posicion.Y - dinoTexture.height / 2, Color.WHITE);
            Raylib.EndMode2D();
            /*Draw Gameplay - end*/

            /*Draw UI - start*/
            
            /*Draw UI - end*/
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}