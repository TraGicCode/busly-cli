---
sidebar_position: 1
title: FAQ
---

# Frequently Asked Questions

## Why are my progress indicators and success check marks not showing when I send a message?

If you run a `send`, `publish`, or `timeout send` command and the spinner or ✔ checkmark never appear, your terminal is likely not configured to output **UTF-8**.

Busly CLI uses [Spectre.Console](https://spectreconsole.net) to render rich terminal output. Spectre.Console requires the console to be set to UTF-8 encoding in order to render Unicode characters (spinners, checkmarks, etc.) correctly. Without it, the output may be silently dropped or garbled.

### Fix for PowerShell

Add the following line to your PowerShell profile (or run it in your current session):

```powershell
[console]::InputEncoding = [console]::OutputEncoding = New-Object System.Text.UTF8Encoding
```

To make this permanent, add it to your `$PROFILE`:

```powershell
notepad $PROFILE
```

Then paste the line above, save, and restart your terminal.

### Fix for Windows Command Prompt

Run the following before using Busly CLI:

```cmd
chcp 65001
```

### Emoji & Font Considerations

Even with UTF-8 correctly configured, the appearance of indicators like `✔` may vary:

- **Font Support** — Emoji rendering depends on your console's font. Not all fonts support color emoji; consider using a font like [Cascadia Code](https://github.com/microsoft/cascadia-code) or another Nerd Font.
- **Console Support** — Some consoles may display emoji as monochrome symbols instead of color icons.
- **Cross-Platform** — Emoji support varies across different operating systems and terminal applications. Linux and macOS terminals generally have better out-of-the-box support than Windows Console Host.

