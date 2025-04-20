# VRSketch

## Requirements
* Unity 2022.3.50f1
* Meta Quest Link (set meta quest link as the active openxr runtime, settings->general->openxr runtime)
* Meta Quest 3
* Logitech MX Ink (optional for pen support)

## Development
* OculusController.cs
  * controller mappings for touch controllers and hand tracking
* MenuCanvas.cs
  * functions relating to the buttons on the menu
* KeyboardController.cs
  * for debugging and calling menu buttons and controller buttons
* DrawController.cs, DrawingCanvas.cs, StrokeCreationModule.cs
  * code related to drawing of a stroke
* Stroke.cs
  * stroke base class for the stroke prefab

