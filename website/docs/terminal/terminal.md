# Terminal Customizations

If you use **Busly CLI** often, you may want to customize your terminal to always display the currently configured transport. This is especially helpful to quickly know whether you’re pointing to your local or a non-production environment.

Below is an example configuration using **Oh My Posh** to display the current transport in your prompt:

![Busly Oh My Posh Custom Segment](../../static/gifs/terminal-customization-oh-my-posh.gif)

---

## Oh My Posh Custom Segment

Assuming **Oh My Posh** is installed and configured for your shell, you can extend your theme by adding a **command segment** that invokes `busly transport current`:

```json
{
  "$schema": "https://raw.githubusercontent.com/JanDeDobbeleer/oh-my-posh/main/themes/schema.json",
  "blocks": [
    {
      "type": "prompt",
      "alignment": "left",
      "segments": [
        // ... your existing segments ...
        {
          "type": "command",
          "style": "powerline",
          "powerline_symbol": "\ue0b0",
          "background": "#4e9a06",
          "foreground": "#000000",
          "properties": {
            "shell": "cmd",
            "command": "/c busly transport current"
          },
          "template": "🚌 {{ .Output }}"
        }
      ]
    }
  ],
  "final_space": true
}
```
