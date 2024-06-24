# Quick Start Guide for Contributing to Color Palette Builder

## 1. Install Visual Studio or Visual Studio Code

### Visual Studio Community 2022

- Download and install Visual Studio Community 2022 from the official website.
- Ensure the WinUI 3 Workflow is installed.

### Visual Studio Code (Optional)

- Download and install Visual Studio Code from the official website.
- Install the following extensions:
  - C# (by Microsoft)
  - C# Extensions
  - XML Tools
  
  > Note: This might not be all that is necessary, ensure it is everything necessary to build windows applications. There might be significant extensions needed. Test by creating a new project from scratch and running to make sure you can get a windows displayed. My personal experience for windows forms applications that it is significantly more straight forward to use Visual Studio instead.

## 2. Install Git

- Download and install Git from the official website. [Git - Downloads (git-scm.com)](https://git-scm.com/downloads)

## 3. Create a GitHub Account

- Sign up for a free account on GitHub. [GitHub](https://github.com/)

## 4. Fork and Clone the Repository

### Fork the Repository

- Go to the Color Palette Builder GitHub repository: https://github.com/Force5vey/ColorPaletteBuilder
- Click the "Fork" button in the top-right corner to create your own copy.

### Clone Your Fork

- Open a terminal or command prompt.

- Clone your forked repository:  

  ```sh  
  git clone https://github.com/your-username/ColorPaletteBuilder.git  
  cd ColorPaletteBuilder

- ## 5. Set Up the Development Environment

  ### Visual Studio

  - Open the solution file (`.sln`) in Visual Studio.
  - Ensure the required NuGet packages are installed:
    - Microsoft.Windows.SDK.BuildTools
    - Microsoft.WindowsAppSDK
    - System.Text.Json

  ### Visual Studio Code (Optional)

  - Open the cloned repository folder in Visual Studio Code.

## 6. Make Changes / Add Features

### Create a Feature Branch

- Ensure your local repository is up-to-date:

  ```sh
  Copy codegit checkout development
  git pull upstream development
  ```

- Create a new feature branch:

  ```sh
  Copy code
  git checkout -b feature/your-feature-name
  ```

### Make and Commit Changes

- Make your changes in the new feature branch.

- Stage and commit your changes:

  ```sh
  Copy codegit add .
  git commit -m "Describe your feature or fix"
  ```

### Push Changes

- Push your feature branch to GitHub:

  ```sh
  Copy code
  git push origin feature/your-feature-name
  ```

- ## 7. Create a Pull Request

  - Go to your forked repository on GitHub.
  - Click the "New pull request" button.
  - Select `development` as the base branch and your feature branch as the compare branch.
  - Provide a descriptive title and detailed description of your changes.
  - Click "Create pull request".

