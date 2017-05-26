// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Построение трехмерного графика функции

module XnaDemo

open System
open System.Drawing
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Graphics

type FGame() as self =
    inherit Game()
    
    let mutable scale = 20
    let mutable graphics = new GraphicsDeviceManager(self)
    let mutable cameraPosition = new Vector3(30.f, 25.f, 30.f)
    let mutable oldMouseState = Mouse.GetState()
    let mutable effect = null

    let mutable functionVertices = []
    let mutable functionIndices = []
    let mutable gridVertices = []
    
    let mutable f = fun x y t -> 
        let r = sqrt(x*x+y*y)
        exp(r)*sin(10.*Math.PI*r+t)

    let func x y t = 
        let x,y,t = float(x/100.f), float(y/100.f), float(t/3000.f) 
        float32(f x y t)* 7.f

    override this.Initialize() =

        let rasterizerState = new RasterizerState()
        rasterizerState.CullMode <- CullMode.None
        self.GraphicsDevice.RasterizerState <- rasterizerState
        
        effect <- new BasicEffect(self.GraphicsDevice)
        effect.VertexColorEnabled <- true

        self.setUpMatrices(cameraPosition)
        self.createGridVertices()
        self.createFunctionIndices()

        base.Initialize()

    override this.Update(gameTime : GameTime) =
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            then self.Exit()

        let xRotation = (float32)(oldMouseState.X - Mouse.GetState().X)/100.f
        let yRotation = (float32)(oldMouseState.Y - Mouse.GetState().Y)/10000.f
        let yRotationAxis = Vector3.Cross(cameraPosition, Vector3.Up)
        let zoom = (float32)(oldMouseState.ScrollWheelValue - Mouse.GetState().ScrollWheelValue)/1000.f

        let xRotationMatrix = Matrix.CreateRotationY(xRotation)
        let yRotationMatrix = Matrix.CreateFromAxisAngle(yRotationAxis, -yRotation)
        let zoomMatrix = Matrix.CreateScale(1.f + zoom)

        cameraPosition <- Vector3.Transform(cameraPosition, xRotationMatrix * yRotationMatrix * zoomMatrix)
        self.setUpMatrices(cameraPosition)

        oldMouseState <- Mouse.GetState()
        self.updateFunctionVertices(gameTime) 
               
        base.Update gameTime

    override this.Draw(gameTime : GameTime) =
        self.GraphicsDevice.Clear(Color.CornflowerBlue)
        for pass in effect.CurrentTechnique.Passes do
            pass.Apply()
            self.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, Array.ofList(gridVertices), 
                0, gridVertices.Length / 2)
            self.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Array.ofList(functionVertices), 
                0, functionVertices.Length,Array.ofList(functionIndices:int16 list), 0, functionIndices.Length / 3)
        base.Draw gameTime

    member this.setUpMatrices(cameraPosition : Vector3) =
        effect.World <- Matrix.CreateScale(1.f)                         
        effect.View <- Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up)
        effect.Projection <- Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                                         self.GraphicsDevice.DisplayMode.AspectRatio, 1.f, 100.f)
        
    member this.createGridVertices() =
            let c = Color.Black
            gridVertices <-
               [ for i in -scale..scale do
                    let fi,fs = (float32)i, (float32)scale
                    yield new VertexPositionColor(new Vector3(fi, 0.f, fs), c)
                    yield new VertexPositionColor(new Vector3(fi, 0.f, -fs), c)
                    yield new VertexPositionColor(new Vector3(fs, 0.f, fi), c)
                    yield new VertexPositionColor(new Vector3(-fs, 0.f, fi), c)]

    member this.updateFunctionVertices(gameTime : GameTime) =
        let t = gameTime.TotalGameTime.TotalMilliseconds
        functionVertices <-
          [ for i in -scale..scale do
              for j in -scale..scale do
                let y = func ((float32)i) ((float32)j) ((float32)t)
                let color = new Microsoft.Xna.Framework.Color((-Math.Abs(y+5.f)*0.15f+1.f),(-Math.Abs(y)*0.15f+1.f),(-Math.Abs(y-5.f)*0.15f+1.f))
                yield new VertexPositionColor(new Vector3((float32)i, y, (float32)j), color)]

    member this.createFunctionIndices() = 
        functionIndices <- 
          [ for i in 0..2 * scale - 1 do
              for j in 0..2 * scale - 1 do
                let ulIndex = (int16)(i * (2 * scale + 1) + j)
                let urIndex = ulIndex + 1s
                let dlIndex = (int16)((i + 1) * (2 * scale + 1) + j)
                let drIndex = dlIndex + 1s
                yield! [dlIndex;drIndex;urIndex;dlIndex;urIndex;ulIndex]]

let fgame = new FGame()
fgame.Run()