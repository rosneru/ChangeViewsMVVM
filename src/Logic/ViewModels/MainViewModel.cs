using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Logic.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    public ICommand CmdOpenSettingsView { get; }
    public ICommand CmdOpenMethodView { get; }
    
    public string WindowTitle { get; private set; } = "MVVM and change views";
    public ViewModelBase CurrentViewModel { get; set; }
    public MethodViewModel MethodVM { get; set; }
    public SettingsViewModel SettingsVM { get; set; }


    public MainViewModel()
    {
      CmdOpenMethodView = new RelayCommand(p =>
      {
        if(MethodVM == null)
        {
          MethodVM = new MethodViewModel();
        }

        CurrentViewModel = MethodVM;

      }, p => !(CurrentViewModel is MethodViewModel));

      CmdOpenSettingsView = new RelayCommand(p =>
      {
        if(SettingsVM == null)
        {
          SettingsVM = new SettingsViewModel();
        }

        CurrentViewModel = SettingsVM;

      }, p => !(CurrentViewModel is SettingsViewModel));

      CmdOpenMethodView.Execute(null);
    }
  }
}
