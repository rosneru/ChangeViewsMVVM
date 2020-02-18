using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Database;
using Database.Enums;

namespace Logic.ViewModels
{
  public class MethodViewModel : ViewModelBase
  {
    /// <summary>
    /// Create a new method.
    /// </summary>
    public ICommand CmdNew { get; }

    /// <summary>
    /// Rename current method.
    /// </summary>
    public ICommand CmdRename { get; }

    /// <summary>
    /// Create a new method and copy the content of the current method
    /// into it.
    /// </summary>
    public ICommand CmdDuplicate { get; }

    /// <summary>
    /// Delete the current method.
    /// </summary>
    public ICommand CmdDelete { get; }

    /// <summary>
    /// Apply changes of the current method.
    /// </summary>
    public ICommand CmdApply { get; }

    /// <summary>
    /// Rejetc changes of the current method.
    /// </summary>
    public ICommand CmdReject { get; }

    /// <summary>
    /// Ok command of the method management operations 'New', 'Rename',
    /// 'Duplicate' and 'Delete'
    /// </summary>
    public ICommand CmdOk { get; }

    /// Cancel command of the method management operations 'New',
    /// 'Rename', 'Duplicate' and 'Delete'
    public ICommand CmdCancel { get; }


    /// <summary>
    /// The currently selected method.
    /// </summary>
    public Method CurrentMethod
    {
      get => AllMethodsView?.CurrentItem as Method;
      set
      {
        AllMethodsView?.MoveCurrentTo(value);
      }
    }

    /// <summary>
    /// Working copy of the currently selected methid. All edits and
    /// changes are done here until <see cref="CmdApply"/> or <see
    /// cref="CmdReject"/> are invoked.
    /// </summary>
    public Method CurrentMethodWorkingCopy { get; set; }

    /// <summary>
    /// List of all methods.
    /// </summary>
    ObservableCollection<Method> AllMethods { get; set; }

    /// <summary>
    /// View of all methods.
    /// </summary>
    public ListCollectionView AllMethodsView { get; }

    /// <summary>
    /// User input of method name for operations 'New', 'Rename' and
    /// 'Duplicate'.
    /// </summary>
    public string UserInputMethodName { get; set; }

    /// <summary>
    /// Prompt to inform the user about the operations 'New', 'Rename',
    /// 'Duplicate' and 'Delete'.
    /// </summary>
    public string UserInputPrompt { get; private set; }

    /// <summary>
    /// Is set to true when one of the method operations 'New', 'Rename' 
    /// or 'Duplicate' is due to be done.
    /// </summary>
    public bool IsNamePanelOpen { get; private set; } = false;

    /// <summary>
    /// Is set to true when the method operation 'Delete' is due to be 
    /// done.
    /// </summary>
    public bool IsDeletePanelOpen { get; private set; } = false;

    /// <summary>
    /// Is set to true when the method settings are allowed to be
    /// changed.
    /// </summary>
    public bool IsEditingAllowed { get; private set; } = false;

    /// <summary>
    /// Is set to true when the 'Manage' menu is allowed to be used.
    /// </summary>
    public bool IsMenuAllowed { get; private set; } = true;

    /// <summary>
    /// Is set to true if the method contains not yet applied changes.
    /// </summary>
    public bool HasChangesPending 
    { 
      get
      {
        if (CurrentMethod == null || CurrentMethodWorkingCopy == null)
        {
          return false;
        }

        return !CurrentMethod.Equals(CurrentMethodWorkingCopy);
      }  
    }

    /// <summary>
    /// Array of all available modes of the pressure sensor.
    /// 
    /// Can be used as ItemSource.
    /// </summary>
    public PressureSensorMode[] PressureSensorModes
    {
      get
      {
        return BaseEnumeration.GetAll<PressureSensorMode>().ToArray();
      }
    }


    public MethodViewModel()
    {
      CmdNew = new RelayCommand(p =>
      {
        intendedAction = IntendedAction.New;
        UserInputMethodName = "";
        UserInputPrompt = "Create a new method with the following name:";
        IsNamePanelOpen = true;
        IsDeletePanelOpen = false;
        IsMenuAllowed = false;
        IsEditingAllowed = false;

      }, p => !HasChangesPending && !IsNamePanelOpen);

      CmdRename = new RelayCommand(p =>
      {
        intendedAction = IntendedAction.Rename;
        UserInputMethodName = CurrentMethod.Name;
        UserInputPrompt = "Rename current method to:";
        IsNamePanelOpen = true;
        IsDeletePanelOpen = false;
        IsMenuAllowed = false;
        IsEditingAllowed = false;
      }, p => (CurrentMethod != null) &&
              (!string.IsNullOrEmpty(CurrentMethod.Name)) &&
              !HasChangesPending);

      CmdDuplicate = new RelayCommand(p =>
      {
        intendedAction = IntendedAction.Duplicate;
        UserInputMethodName = $"{CurrentMethod.Name} (copy)";
        UserInputPrompt = "Create a duplicate method with the following name:";
        IsNamePanelOpen = true;
        IsDeletePanelOpen = false;
        IsMenuAllowed = false;
        IsEditingAllowed = false;
      }, p => (CurrentMethod != null) &&
              (!string.IsNullOrEmpty(CurrentMethod.Name)) &&
              !HasChangesPending);

      CmdDelete = new RelayCommand(p =>
      {
        intendedAction = IntendedAction.Delete;
        UserInputMethodName = "";
        UserInputPrompt = "Delete the current method - are you really sure?";
        IsNamePanelOpen = false;
        IsDeletePanelOpen = true;
        IsMenuAllowed = false;
        IsEditingAllowed = false;
      }, p => (CurrentMethod != null) &&
              (!string.IsNullOrEmpty(CurrentMethod.Name)) &&
              !HasChangesPending);

      CmdOk = new RelayCommand(p =>
      {
        if (performAction() == true)
        {
          intendedAction = IntendedAction.None;
          UserInputMethodName = "";
          UserInputPrompt = "";
          IsNamePanelOpen = false;
          IsDeletePanelOpen = false;
          IsMenuAllowed = true;
          IsEditingAllowed = true;
        }
      }, p => (intendedAction == IntendedAction.Delete) ||
              (!string.IsNullOrEmpty(UserInputMethodName)));

      CmdCancel = new RelayCommand(p =>
      {
        intendedAction = IntendedAction.None;
        UserInputMethodName = "";
        UserInputPrompt = "";
        IsNamePanelOpen = false;
        IsDeletePanelOpen = false;
        IsMenuAllowed = true;
        IsEditingAllowed = true;
      }, p => IsNamePanelOpen || IsDeletePanelOpen);


      CmdApply = new RelayCommand(p =>
      {
        applyChanges();
      }, p => HasChangesPending);


      CmdReject = new RelayCommand(p =>
      {
        rejectChanges();
      }, p => HasChangesPending);

      // Create method collection
      AllMethods = new ObservableCollection<Method>();
      AllMethods.CollectionChanged += (s, e) =>
      {
        if (e.NewItems != null)
        {
          foreach (INotifyPropertyChanged added in e.NewItems)
          {
            added.PropertyChanged += allMethodsItemAdded;
          }
        }

        if (e.OldItems != null)
        {
          foreach (INotifyPropertyChanged deleted in e.OldItems)
          {
            deleted.PropertyChanged -= allMethodsItemAdded;
          }
        }
      };

      // Create a default view for method collection
      var defaultView = CollectionViewSource.GetDefaultView(AllMethods);
      AllMethodsView = defaultView as ListCollectionView;
      AllMethodsView.CurrentChanged += allMethodsViewCurrentChanged;

      CurrentMethod = (Method)defaultView.CurrentItem;
    }

    private void allMethodsItemAdded(object sender, 
                                     PropertyChangedEventArgs e)
    {
      AllMethodsView.Refresh();
    }

    private void allMethodsViewCurrentChanged(object sender, EventArgs e)
    {
      if(CurrentMethod == null)
      {
        return;
      }

      CurrentMethodWorkingCopy = CurrentMethod.DeepCopy();
    }

    private enum IntendedAction
    {
      None,
      New,
      Rename,
      Duplicate,
      Delete
    }

    private IntendedAction intendedAction = IntendedAction.None;

    private bool performAction()
    {
      switch (intendedAction)
      {
        case IntendedAction.New:
          return methodNew();

        case IntendedAction.Rename:
          return methodRename();

        case IntendedAction.Duplicate:
          return methodDuplicate();

        case IntendedAction.Delete:
          return methodDelete();

        default:
          return false;
      }
    }

    private bool methodNew()
    {
      var newMethod = new Method();
      newMethod.Name = UserInputMethodName;
      AllMethods.Add(newMethod);

      CurrentMethod = newMethod;
      AllMethodsView.MoveCurrentTo(CurrentMethod);

      return true;
    }

    private bool methodRename()
    {
      CurrentMethod.Name = UserInputMethodName;
      CurrentMethodWorkingCopy.Name = UserInputMethodName;
      return true;
    }

    private bool methodDuplicate()
    {
      var duplicateMethod = CurrentMethod.DeepCopy();
      duplicateMethod.Name = UserInputMethodName;
      AllMethods.Add(duplicateMethod);

      CurrentMethod = duplicateMethod;
      AllMethodsView.MoveCurrentTo(CurrentMethod);

      return true;
    }

    private bool methodDelete()
    {
      return false;
    }

    private void applyChanges()
    {
      int currentIndex = AllMethods.IndexOf(CurrentMethod);
      AllMethods[currentIndex] = CurrentMethodWorkingCopy.DeepCopy();
    }

    private void rejectChanges()
    {
      // Reload current method into working copy
      CurrentMethodWorkingCopy = CurrentMethod.DeepCopy();
    }
  }
}
