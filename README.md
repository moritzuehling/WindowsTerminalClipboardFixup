# WindowsTerminalClipboardFixup

Workaround for: https://github.com/microsoft/terminal/issues/2944

## How does it work

Everytime you copy text, it checks if the copied text was copied out of the new Windows Terminal.
If that is indeed the case, it tries to estimate the length of a line, and tries to split up the text according to that.

The estimation of line-lengths is a bit
[primitive](https://github.com/moritzuehling/WindowsTerminalClipboardFixup/blob/13973fe18ce291ea8908efe5da78d4dcae00424c/WindowsTerminalClipboardFixup/HiddenForm.cs#L133),
if you have a better idea, please feel free to contribute it. Either way, it works better than whatever the terminal is doing right now.

## How do I get this?

You can download a build [here](https://github.com/moritzuehling/WindowsTerminalClipboardFixup/releases). 
