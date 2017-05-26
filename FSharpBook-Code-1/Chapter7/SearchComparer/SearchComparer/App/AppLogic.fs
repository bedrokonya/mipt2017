// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Приложение для Windows Phone 7
// Сравнение популярности двух слов в интернет

namespace WindowsPhonePanoramaApp

open System
open System.Net
open System.Windows
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Ink
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Animation
open System.Windows.Shapes
open System.Windows.Navigation
open Microsoft.Phone.Controls
open Microsoft.Phone.Shell
open Caliburn.Micro


/// This type implements the main page of the application
type MainPage() as this =
    inherit PhoneApplicationPage()
    // Load the Xaml for the page.
    do Application.LoadComponent(this, new System.Uri("/WindowsPhonePanoramaApp;component/MainPage.xaml", System.UriKind.Relative))
    let root = new PhoneApplicationFrame()
    let term1box : TextBox = this?Term1
    let term2box : TextBox = this?Term2
    let textitem1 : TextBlock = this?TextItem1
    let textitem2 : TextBlock = this?TextItem2
    let baritem1 : Rectangle = this?BarItem1
    let baritem2 : Rectangle = this?BarItem2
    let mainBtn : Button = this?MainBtn
    
    let mutable p1,p2,max = 100,50,100

    let redraw n x =
       if x>max then max <- x
       match n with
         | 1 -> p1 <- x; textitem1.Text <- x.ToString()
         | 2 -> p2 <- x; textitem2.Text <- x.ToString()
       baritem1.Width <- 400.*float(p1)/float(max)
       baritem2.Width <- 400.*float(p2)/float(max)

    do mainBtn.Click.Add(fun e ->
        let res1 = new Shwarsico.BingSearcher(term1box.Text)
        let res2 = new Shwarsico.BingSearcher(term2box.Text)
        res1.ResultAvailable.Add(fun res -> redraw 1 res.Count)
        res2.ResultAvailable.Add(fun res -> redraw 2 res.Count)
        textitem1.Text <- "Querying..." ; res1.Pull()
        textitem2.Text <- "Querying..." ; res2.Pull()
        )


/// One instance of this type is created in the application host project.
type App(app:Application) = 
    // Global handler for uncaught exceptions. 
    // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
    do app.UnhandledException.Add(fun e -> 
            if (System.Diagnostics.Debugger.IsAttached) then
                // An unhandled exception has occurred, break into the debugger
                System.Diagnostics.Debugger.Break();
     )
    let navigationService = IoC.Get<INavigationService>()
    do navigationService.Navigate(new Uri("/WindowsPhonePanoramaApp;component/MainPage.xaml", UriKind.Relative)) |> ignore
