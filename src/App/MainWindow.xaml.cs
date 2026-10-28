using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Cal=Festivos.Calendar;
namespace Festivos.Desktop;
public partial class MainWindow:Window,INotifyPropertyChanged
{
 readonly string path=System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"FestivosColombia","preferences.json");
 readonly CultureInfo spanish=CultureInfo.GetCultureInfo("es-CO");
 readonly DispatcherTimer timer=new(){Interval=TimeSpan.FromMinutes(1)};
 readonly DispatcherTimer saveTimer=new(){Interval=TimeSpan.FromSeconds(1)};
 Preferences prefs;DateOnly month;DateOnly today=DateOnly.FromDateTime(DateTime.Today);bool rendering;
 public event PropertyChangedEventHandler? PropertyChanged;
 public Brush BackgroundBrush=>(Brush)new BrushConverter().ConvertFromString(prefs.Color)!;
 public string MonthName=>spanish.TextInfo.ToTitleCase(month.ToDateTime(TimeOnly.MinValue).ToString("MMMM",spanish));
 public MainWindow(){prefs=PreferenceStore.Load(path);month=new(prefs.Year,prefs.Month,1);InitializeComponent();DataContext=this;
 Width=prefs.Width;Height=prefs.Height;Left=prefs.Left;Top=prefs.Top;Topmost=prefs.Topmost;Opacity=prefs.Opacity;
 YearPicker.ItemsSource=Enumerable.Range(Cal.MinYear,Cal.MaxYear-Cal.MinYear+1);
 Loaded+=(_,_)=>{KeepVisible();Render();};SizeChanged+=(_,_)=>{Adapt();QueueSave();};LocationChanged+=(_,_)=>QueueSave();Closing+=(_,_)=>Save(true);
 SystemEvents.DisplaySettingsChanged+=DisplayChanged;Closed+=(_,_)=>{timer.Stop();saveTimer.Stop();SystemEvents.DisplaySettingsChanged-=DisplayChanged;};
 saveTimer.Tick+=(_,_)=>{saveTimer.Stop();Save();};timer.Tick+=(_,_)=>{var now=DateOnly.FromDateTime(DateTime.Today);if(now!=today){today=now;Render();}};timer.Start();}
 void QueueSave(){if(!IsLoaded)return;saveTimer.Stop();saveTimer.Start();}
 void DisplayChanged(object? s,EventArgs e)=>Dispatcher.BeginInvoke(new Action(KeepVisible));
 void KeepVisible(){var hwnd=new System.Windows.Interop.WindowInteropHelper(this).Handle;var monitor=MonitorFromWindow(hwnd,2);var info=new MonitorInfo{Size=System.Runtime.InteropServices.Marshal.SizeOf<MonitorInfo>()};if(!GetMonitorInfo(monitor,ref info))return;
 var transform=PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice??Matrix.Identity;var a=transform.Transform(new Point(info.Work.Left,info.Work.Top));var b=transform.Transform(new Point(info.Work.Right,info.Work.Bottom));Width=Math.Min(Width,Math.Max(MinWidth,b.X-a.X));Height=Math.Min(Height,Math.Max(MinHeight,b.Y-a.Y));Left=Math.Clamp(Left,a.X,Math.Max(a.X,b.X-Width));Top=Math.Clamp(Top,a.Y,Math.Max(a.Y,b.Y-Height));}
 [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]struct NativeRect{public int Left,Top,Right,Bottom;}
 [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]struct MonitorInfo{public int Size;public NativeRect Monitor,Work;public uint Flags;}
 [System.Runtime.InteropServices.DllImport("user32.dll")]static extern IntPtr MonitorFromWindow(IntPtr window,uint flags);
 [System.Runtime.InteropServices.DllImport("user32.dll",CharSet=System.Runtime.InteropServices.CharSet.Auto)]static extern bool GetMonitorInfo(IntPtr monitor,ref MonitorInfo info);
 void Save(bool notify=false){if(WindowState==WindowState.Normal){prefs.Left=Left;prefs.Top=Top;prefs.Width=Width;prefs.Height=Height;}prefs.Year=month.Year;prefs.Month=month.Month;prefs.Topmost=Topmost;
 try{PreferenceStore.Save(path,prefs);}catch(Exception e)when(e is System.IO.IOException or UnauthorizedAccessException){if(notify)MessageBox.Show(this,"No se pudieron guardar las preferencias. Comprueba los permisos del perfil local.","Preferencias");}}
 void Adapt(){MonthPanel.Visibility=ActualHeight>=530?Visibility.Visible:Visibility.Collapsed;UpcomingPanel.Visibility=ActualHeight>=690?Visibility.Visible:Visibility.Collapsed;}
 void Render(){rendering=true;YearPicker.SelectedItem=month.Year;YearPicker.Text=month.Year.ToString();rendering=false;PropertyChanged?.Invoke(this,new(nameof(MonthName)));Days.Children.Clear();MonthHolidays.Children.Clear();Upcoming.Children.Clear();Description.Text="";
 PrevMonth.IsEnabled=PrevYear.IsEnabled=month>new DateOnly(Cal.MinYear,1,1);NextMonthButton.IsEnabled=NextYearButton.IsEnabled=month<new DateOnly(Cal.MaxYear,12,1);
 PinButton.Background=Topmost?new SolidColorBrush(Color.FromArgb(100,39,105,115)):Brushes.Transparent;
 var holidays=Cal.Holidays(month.Year);int offset=((int)month.DayOfWeek+6)%7;
 for(int i=0;i<42;i++){int day=i-offset+1;if(day<1||day>DateTime.DaysInMonth(month.Year,month.Month)){Days.Children.Add(new Border());continue;}
 var date=new DateOnly(month.Year,month.Month,day);var matches=holidays.Where(h=>h.Date==date).ToArray();
 var button=new Button{Content=day.ToString(),Padding=new Thickness(0),FontSize=15,Foreground=date.DayOfWeek==DayOfWeek.Sunday?Brushes.Firebrick:new SolidColorBrush(Color.FromRgb(65,60,41)),Background=matches.Length>0?new SolidColorBrush(Color.FromRgb(249,194,180)):Brushes.Transparent,ToolTip=matches.Length>0?string.Join("\n",matches.Select(h=>h.Name)):date.ToString("D",spanish)};
 System.Windows.Automation.AutomationProperties.SetName(button,date.ToString("D",spanish)+(matches.Length>0?", "+string.Join(", ",matches.Select(h=>h.Name)):""));
 button.Click+=(_,_)=>{var message=matches.Length>0?string.Join("\n",matches.Select(h=>h.Description)):date.ToString("D",spanish);Description.Text=message;Description.ToolTip=message;if(MonthPanel.Visibility!=Visibility.Visible||matches.Length>0)MessageBox.Show(this,message,"Detalle del día");};
 Days.Children.Add(new Border{BorderThickness=new Thickness(date==today?2:0),BorderBrush=new SolidColorBrush(Color.FromRgb(39,105,115)),CornerRadius=new CornerRadius(9),Margin=new Thickness(1),Child=button});}
 var current=holidays.Where(h=>h.Date.Month==month.Month).ToArray();foreach(var h in current)MonthHolidays.Children.Add(Line(h));if(current.Length==0)MonthHolidays.Children.Add(new TextBlock{Text="Este mes no tiene festivos nacionales.",FontSize=12});
 foreach(var h in Cal.Holidays(today.Year).Concat(Cal.Holidays(today.Year+1)).Where(h=>h.Date>=today).Take(3))Upcoming.Children.Add(Line(h));Adapt();QueueSave();}
 TextBlock Line(Holiday h)=>new(){Text=$"{h.Date:dd/MM}  {h.Name}",TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,4,0,0),FontSize=12,ToolTip=h.Description};
 void Move(int count){month=Cal.Move(month,count);Render();}
 void PreviousYear(object s,RoutedEventArgs e)=>Move(-12);void NextYear(object s,RoutedEventArgs e)=>Move(12);void PreviousMonth(object s,RoutedEventArgs e)=>Move(-1);void NextMonth(object s,RoutedEventArgs e)=>Move(1);
 public void GoToday(){month=new(today.Year,today.Month,1);Render();}
 void Today(object s,RoutedEventArgs e){month=new(today.Year,today.Month,1);Render();}
 void YearSelected(object s,SelectionChangedEventArgs e){if(!rendering&&YearPicker.SelectedItem is int y){month=new(y,month.Month,1);Render();}}
 void YearKey(object s,KeyEventArgs e){if(e.Key!=Key.Enter)return;if(int.TryParse(YearPicker.Text,out int y)&&y>=Cal.MinYear&&y<=Cal.MaxYear){month=new(y,month.Month,1);Render();}else MessageBox.Show(this,$"Escribe un año entre {Cal.MinYear} y {Cal.MaxYear}.","Año no válido");}
 void Pin(object s,RoutedEventArgs e){Topmost=!Topmost;Render();Save();}void Minimize(object s,RoutedEventArgs e)=>WindowState=WindowState.Minimized;void CloseWindow(object s,RoutedEventArgs e)=>Close();
 void Settings(object s,RoutedEventArgs e){var dialog=new SettingsWindow(prefs){Owner=this};if(dialog.ShowDialog()!=true)return;var p=dialog.Result;
 try{using var key=Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");if(p.AutoStart)key.SetValue("FestivosColombia",$"\"{Environment.ProcessPath}\"");else key.DeleteValue("FestivosColombia",false);}
 catch(Exception ex)when(ex is UnauthorizedAccessException or System.Security.SecurityException or System.IO.IOException){MessageBox.Show(this,"Windows no permitió cambiar el inicio automático. Las demás preferencias se guardarán.","Inicio automático");p.AutoStart=prefs.AutoStart;}
 prefs.Color=p.Color;prefs.Opacity=p.Opacity;prefs.AutoStart=p.AutoStart;Topmost=p.Topmost;Opacity=p.Opacity;PropertyChanged?.Invoke(this,new(nameof(BackgroundBrush)));Render();Save();}
}
