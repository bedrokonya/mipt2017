// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Реализация простейших 3D-примитивов на XNA

module XnaDemo

open System
open System.Drawing
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type FGame() as self =
    inherit Game()

    let n = 3
    let CreateVertices() =
          let phi = MathHelper.TwoPi / float32(n)
          let R = 10.f
          new VertexPositionColor(new Vector3(0.f,0.f,R),Color.Blue) ::
           [ for i in 0..(n-1) -> new VertexPositionColor(new Vector3(R*sin(phi*float32(i)), R*cos(phi*float32(i)), 0.0f), Color.Yellow)]
    
    let CreateIndices() = 
          [ for i in 1..n do
               let nx = if i<n then i+1 else 1
               yield (int16)0
               yield (int16)i
               yield (int16)nx ] @ [ (int16)1;(int16)2;(int16)3 ]

    let graphics = new GraphicsDeviceManager(self)
    let cameraPosition = new Vector3(0.f, -20.f, 20.f)
    let mutable effect = null

    let mutable Vertices = CreateVertices()
    let mutable Indices = CreateIndices()
    let mutable rot = 0.f
        
    override this.Initialize() =
        let rasterizerState = new RasterizerState()
        rasterizerState.CullMode <- CullMode.None
        self.GraphicsDevice.RasterizerState <- rasterizerState        
        effect <- new BasicEffect(self.GraphicsDevice)
        effect.VertexColorEnabled <- true
        self.setUpMatrices()
        base.Initialize()

    override this.Update(gameTime : GameTime) =
        rot <- rot + 0.01f
        self.setUpMatrices()
        base.Update gameTime

    override this.Draw(gameTime : GameTime) =
        self.GraphicsDevice.Clear(Color.Gray)
        for pass in effect.CurrentTechnique.Passes do
            pass.Apply()
            this.GraphicsDevice.DrawUserIndexedPrimitives(
              PrimitiveType.TriangleList, 
              Array.ofList(Vertices), 
              0, Vertices.Length,Array.ofList(Indices), 
              0, Indices.Length / 3)
        base.Draw gameTime

    member this.setUpMatrices() =
        effect.World <- Matrix.CreateFromAxisAngle(new Vector3(0.f,0.f,1.0f),rot)
        effect.View <- Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up)
        effect.Projection <- Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                                         self.GraphicsDevice.DisplayMode.AspectRatio, 1.f, 100.f)

    
let fgame = new FGame()
fgame.Run()