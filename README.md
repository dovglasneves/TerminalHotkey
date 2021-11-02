# TerminalHotkey

TerminalHotkey is a program that runs in the background to define the hotkey Ctrl+Alt+T to access the Windows Terminal, it also defines the combination Ctrl+Alt+Shift+T as a hosrtcut to close itself. The principle is basic, it defines a keyboard hook to capture the keys and use it to check if the combination of keys are pressed.
Starting the program will register it in the Windows StartUp, making it always active until the user press the shortcut to close it and remove from register.

<img src="https://github.com/dovglasneves/TerminalHotkey/blob/main/preview/Terminal_HTK.gif" alt="TerminalHotkey Preview" border="0" />
