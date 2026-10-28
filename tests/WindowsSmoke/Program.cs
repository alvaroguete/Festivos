using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Festivos.Desktop;
internal static class Program
{
 [STAThread]static int Main(){try{var app=new App();app.InitializeComponent();var window=new MainWindow();window.Show();Pump();
 foreach(var size in new[]{(320d,380d),(390d,560d),(390d,690d),(900d,900d),(320d,380d)}){window.Width=size.Item1;window.Height=size.Item2;Pump();window.UpdateLayout();var days=(System.Windows.Controls.Primitives.UniformGrid)window.FindName("Days");if(days.Children.Count!=42||days.ActualHeight<140)throw new Exception("Calendario inválido al redimensionar");if(((FrameworkElement)window.FindName("MonthPanel")).Visibility!=(size.Item2>=530?Visibility.Visible:Visibility.Collapsed))throw new Exception("Modo adaptable incorrecto");Console.WriteLine($"OK: redimensionamiento {size}");}
 window.GoToday();Pump();window.Close();app.Shutdown();Console.WriteLine("OK: apertura, calendario, modos adaptables y cierre WPF");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
}
