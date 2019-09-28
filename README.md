# WindowsTerminalClipboardFixup

Workaround for: https://github.com/microsoft/terminal/issues/2944

In short, when this isn't running, the Windows Terminal thinks that this:
```
FreeBSD 12.0-RELEASE-p7 GENERIC                                                                                                                                                                                                               Welcome to FreeBSD!                                                                                                                                                                                                                           Release Notes, Errata: https://www.FreeBSD.org/releases/                                                               Security Advisories:   https://www.FreeBSD.org/security/                                                               FreeBSD Handbook:      https://www.FreeBSD.org/handbook/                                                               FreeBSD FAQ:           https://www.FreeBSD.org/faq/                                                                    Questions List: https://lists.FreeBSD.org/mailman/listinfo/freebsd-questions/                                          FreeBSD Forums:        https://forums.FreeBSD.org/                                                                                                                                                                                            Documents installed with the system are in the /usr/local/share/doc/freebsd/                                           directory, or can be installed later with:  pkg install en-freebsd-doc                                                 For other languages, replace "en" with a language code like de or fr.                                                                                                                                                                         Show the version of FreeBSD installed:  freebsd-version ; uname -a                                                     Please include that output and any error messages when posting questions.                                              Introduction to manual pages:  man man                                                                                 FreeBSD directory layout:      man hier                                                                                                                                                                                                       Edit /etc/motd to change this login announcement.                                                                                                                                                                                             [root@dolores ~]#                                                                                                                                        
```

is a valid way to copy this: 
![](https://i.imgur.com/aWTl1b5.png)

This software makes sure this comes out as
```
FreeBSD 12.0-RELEASE-p7 GENERIC

Welcome to FreeBSD!

Release Notes, Errata: https://www.FreeBSD.org/releases/
Security Advisories:   https://www.FreeBSD.org/security/
FreeBSD Handbook:      https://www.FreeBSD.org/handbook/
FreeBSD FAQ:           https://www.FreeBSD.org/faq/
Questions List: https://lists.FreeBSD.org/mailman/listinfo/freebsd-questions/
FreeBSD Forums:        https://forums.FreeBSD.org/

Documents installed with the system are in the /usr/local/share/doc/freebsd/
directory, or can be installed later with:  pkg install en-freebsd-doc
For other languages, replace "en" with a language code like de or fr.

Show the version of FreeBSD installed:  freebsd-version ; uname -a
Please include that output and any error messages when posting questions.
Introduction to manual pages:  man man
FreeBSD directory layout:      man hier

Edit /etc/motd to change this login announcement.

[root@dolores ~]#
```

## How do I get this?

You can download a build [here](https://github.com/moritzuehling/WindowsTerminalClipboardFixup/releases). 

## How do I run this?

Double-Click `WindowsTerminalClipboardFixup.exe`. It's running. If you want to exit, double-click it again, it will ask if you want to exit: 

![](https://i.imgur.com/lnm1DSq.png)

## How does it work?

Everytime you copy text, it checks if the copied text was copied out of the new Windows Terminal.
If that is indeed the case, it tries to estimate the length of a line, and tries to split up the text according to that.

The estimation of line-lengths is a bit
[primitive](https://github.com/moritzuehling/WindowsTerminalClipboardFixup/blob/13973fe18ce291ea8908efe5da78d4dcae00424c/WindowsTerminalClipboardFixup/HiddenForm.cs#L133),
if you have a better idea, please feel free to contribute it. Either way, it works better than whatever the terminal is doing right now.
