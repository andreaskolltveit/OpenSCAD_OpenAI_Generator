- Find the output in `bin\Release\net8.0-windows\win-x64\publish\`.

2. **Distribute:**
- Share the `.exe` (or the whole folder) from the publish directory.

3. **Installer (optional):**
- Use tools like MSIX Packaging Tool, WiX, or Inno Setup for a professional installer.

---

## Usage

1. **Launch the app.**
2. **Enter a description** of your 3D model (e.g., "A 20mm cube with a 5mm hole through the center").
3. Click **"Generate OpenSCAD"** to get the code.
4. Review or edit the code if needed.
5. Click **"Export STL"** to save the 3D model as an STL file (requires OpenSCAD).

---

## OpenSCAD Code Generation Rules

- Only 3D objects at the top level (cube, sphere, cylinder, etc.).
- 2D shapes (circle, square, polygon) are always extruded to 3D using `linear_extrude()`.
- All required parameters are always specified.
- Only `//` and `/* ... */` comments are used.
- No markdown, echo, assert, or debugging statements.
- Output is always a complete, valid OpenSCAD script.

---

## Troubleshooting

- **STL export fails:** Make sure OpenSCAD is installed and the path is correct. The model must be 3D.
- **OpenAI errors:** Check your API key and internet connection.
- **Permissions:** Run as administrator if you have file access issues.

---

## License

MIT License

---

## Credits

- [OpenAI](https://openai.com/)
- [OpenSCAD](https://openscad.org/)
- [HandyControl](https://github.com/HandyOrg/HandyControl)