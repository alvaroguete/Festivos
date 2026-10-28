using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Festivos.Desktop;
internal static class Program
{
 [STAThread]static int Main(){try{var app=new App();app.InitializeComponent();var window=new MainWindow();window.Show();Pump();
 if(window.Icon is null)throw new Exception("No se cargó el icono integrado de Colombia");
 Console.WriteLine("OK: icono WPF integrado desde el ensamblado de la aplicación");
 CheckDragging(window);
 foreach(var size in new[]{(320d,380d),(390d,560d),(390d,690d),(900d,900d),(320d,380d)}){window.Width=size.Item1;window.Height=size.Item2;Pump();window.UpdateLayout();var days=(System.Windows.Controls.Primitives.UniformGrid)window.FindName("Days");if(days.Children.Count!=42||days.ActualHeight<140)throw new Exception("Calendario inválido al redimensionar");if(((FrameworkElement)window.FindName("MonthPanel")).Visibility!=(size.Item2>=530?Visibility.Visible:Visibility.Collapsed))throw new Exception("Modo adaptable incorrecto");Console.WriteLine($"OK: redimensionamiento {size}");}
 window.GoToday();Pump();window.Close();app.Shutdown();Console.WriteLine("OK: apertura, calendario, modos adaptables y cierre WPF");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
 static void CheckDragging(MainWindow window)
 {
  var header=(FrameworkElement)window.FindName("DragArea");
  var pin=(FrameworkElement)window.FindName("PinButton");
  var handle=new System.Windows.Interop.WindowInteropHelper(window).Handle;
  int Hit(FrameworkElement element,Point point)
  {
   var screen=element.PointToScreen(point);
   var coordinates=new IntPtr(unchecked((int)(((uint)(ushort)(int)screen.Y<<16)|(ushort)(int)screen.X)));
   return (int)SendMessage(handle,0x0084,IntPtr.Zero,coordinates);
  }
  foreach(bool topmost in new[]{false,true})
  {
   window.Topmost=topmost;Pump();
   if(Hit(header,new Point(35,header.ActualHeight/2))!=2)
    throw new Exception("El encabezado no permite mover la ventana con el mecanismo nativo de Windows");
   if(Hit(pin,new Point(pin.ActualWidth/2,pin.ActualHeight/2))!=1)
    throw new Exception("El botón de fijar quedó atrapado en la zona de arrastre");
  }
  Console.WriteLine("OK: encabezado arrastrable y botones interactivos, normal y siempre encima");
 }
 [System.Runtime.InteropServices.DllImport("user32.dll")]
 static extern IntPtr SendMessage(IntPtr hwnd,uint message,IntPtr wParam,IntPtr lParam);
 static void Pump(){var frame=new DispatcherFrame();Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(()=>frame.Continue=false));Dispatcher.PushFrame(frame);}
}
