# Color Palette Builder

A comprehensive tool designed for developers and designers to create, manage, and organize color palettes for projects. This application allows users to build a color palette by selecting colors from a color wheel or using an eye dropper to pick colors directly from the screen. Users can label colors with specific names, assign them to groups, and organize them according to their project's needs.

## Features

- **Save and Open Palettes:** Save your color palettes in a proprietary .cpb format using industry-standard JSON for easy access and sharing.
- **Color Picker:** Choose colors seamlessly using an intuitive color wheel.
- **Eye Dropper Tool:** Extract colors from any part of your screen with precision.
- **Color Representation:** View and copy color codes in various formats, including Hex, RGBA, and C# code snippets.
- **Palette Management:** Efficiently manage your colors with a grid view that supports sorting and grouping by custom labels and categories.
- **User Customization:** Label colors based on their use, group them into categories, and sort them to keep your project organized.
- **Import/Export Compatibility:** Supports import/export functionality for cross-tool compatibility with formats like Adobe Swatch Exchange (.ase) and GIMP Palette (.gpl). (UPCOMING)

## Installation

### Prerequisites

- Windows 10/11
- .NET 5.0 or later
- WinUI 3 SDK

### Steps

1. **Clone the Repository:**

   - Open a command prompt or terminal window.

   - Navigate to the directory where you want to clone the repository.

   - Run the following command:

     ```
     sh
     Copy code
     git clone https://github.com/Force5vey/ColorPaletteBuilder
     ```

   **Navigate to the Project Directory:**

   - Change to the project directory by running:

     ```
     sh
     Copy code
     cd ColorPaletteBuilder
     ```

   **Open the Solution in Visual Studio:**

   - Open Visual Studio.
   - Select `File` > `Open` > `Project/Solution`.
   - Navigate to the `color-palette-builder` directory and open `color-palette-builder.sln`.

   **Build and Run the Application:**

   - In Visual Studio, select `Build` > `Build Solution` to compile the project.
   - Once the build is successful, run the application by pressing `F5` or selecting `Debug` > `Start Debugging`.

## Usage

### Creating a New Palette

1. Open the application.
2. Select `File` > `New Palette` from the menu.
3. Use the color picker or eye dropper tool to add colors to your palette.
4. Label and organize your colors as needed.

### Saving a Palette

1. Select `File` > `Save Palette`.
2. Choose a location and save your palette with a `.cpb` extension.

### Opening an Existing Palette

1. Select `File` > `Open Palette`.
2. Navigate to the `.cpb` file and open it.

### (UPCOMING) Exporting a Palette

1. Select `File` > `Export Palette`.
2. Choose the desired format (e.g., .ase, .gpl) and save the file.

## Development

### Project Structure

- `src/`: Contains the main source code for the application.
- `assets/`: Contains images, icons, and other static assets.
- `docs/`: Contains documentation and user guides.
- `tests/`: Contains unit and integration tests.

### Building the Project

1. Open the solution in Visual Studio.
2. Build the project by selecting `Build` > `Build Solution`.

### Contributing

- Fork the repository.
- Create a new branch for your feature or bugfix.
- Commit your changes and push the branch to your fork.
- Create a pull request with a detailed description of your changes.

## License

See LICENSE for more information.

## Contact

For inquiries, please contact:

- **Email:** apps@force5dev.com
- **Website:** [Force5Dev](https://www.force5dev.com)

## Roadmap

### Planned Features

- **Undo/Redo Functionality:** Track changes and allow users to undo/redo actions.
- **Preferences:** Allow customization of app settings, such as default save location and theme.
- **Additional Export Formats:** Add support for more color palette formats.
- **Integration with Design Tools:** Enable direct export/import with popular design tools.

### Known Issues

-  Eye dropper tool may not work correctly on multi-monitor setups.
-  Some color formats might display slight inaccuracies.

## Changelog

### v1.0.0

- Initial release with core features including color picker, eye dropper, and palette management.

### Future Versions

-  Add support for batch color importing.
-  Enhance UI/UX with additional customization options.

## Frequently Asked Questions (FAQ)

### How do I report a bug?

Please report bugs by creating an issue in the DevOps repository with detailed information about the problem and steps to reproduce it. or Send email to apps@force5dev.com

### Can I use this tool commercially?

Yes, using the tool commercially or in any manner is allowed. Utilizing the code to release your own version under your name is NOT allowed.

### How do I request a new feature?

Feature requests can be submitted by creating an issue in the DevOps repository. Please provide as much detail as possible to help us understand and prioritize your request. Or email apps@force5dev.com