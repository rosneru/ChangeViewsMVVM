# MVVM - Navigation between views

This is a demonstration solution to my [question on stackoverflow](https://stackoverflow.com/questions/60280730/combobox-binding-selecteditem-lost-after-viewmodel-change-wpf-mvvm "ComboBox binding - SelectedItem lost after ViewModel change")

The combobox in method view loses its SelectedItem after switching to 
settings view because its DataContext is set to null.

How can this be solved?

# Development Environment
- Visual Studio 2019
- .NET Framework 4.7.2

## Nuget dependencies
They should be automatically restored after project loading

- PropertyChanged.Fody
- HamburgerMenu

# How to use it
UI.Desktop has to be selected as startup project.

**For some reason unkonw to me, to successfully build for the first time, all propjects should be unloaded and then loaded and build one after another in the following order:**
1. Database
2. Logic
3. Desktop

After starting the method view is dislayed. With Manage->New you can 
create new methods and edit them.

After switching to the still empty settings menu and back to method menu
the combobox has lost its selected item.
