# Contributing to Color Palette Builder

Welcome! We're excited that you're interested in contributing to our project. This guide will help you get started, even if you're new to Git and GitHub.

## What is Git and GitHub?

- **Git:** A version control system that lets you track changes to your files and collaborate with others.
- **GitHub:** A platform that hosts Git repositories online and provides tools for collaboration.

## Getting Started

### Step 1: Set Up Git and GitHub

1. **Sign Up for GitHub:**
   - Go to [GitHub](https://github.com/) and sign up for an account.

2. **Install Git:**
   - Download and install Git from [git-scm.com](https://git-scm.com/).

3. **Configure Git:**
   - Open Git Bash or your terminal.
   - Set your username and email:
     ```sh
     git config --global user.name "Your Name"
     git config --global user.email "your-email@example.com"
     ```

### Step 2: Fork the Repository

1. **Fork the Repository:**
   - Go to the project repository on GitHub.
   - Click the "Fork" button at the top right to create your own copy.

2. **Clone Your Fork:**
   - Clone your forked repository to your local machine:
     ```sh
     git clone https://github.com/your-username/your-repo-name.git
     cd your-repo-name
     ```

3. **Add Upstream Remote:**
   - This links your local repository to the original repository:
     ```sh
     git remote add upstream https://github.com/original-username/original-repo-name.git
     ```

### Step 3: Create a Branch

1. **Create a Feature Branch:**
   - Always create a new branch for your changes:
     ```sh
     git checkout -b feature/your-feature-name
     ```

### Step 4: Make Changes and Commit

1. **Make Your Changes:**
   - Edit the files you need to change using your code editor.

2. **Stage and Commit Your Changes:**
   - Stage your changes:
     ```sh
     git add .
     ```
   - Commit your changes with a descriptive message:
     ```sh
     git commit -m "Add feature X"
     ```

### Step 5: Push Your Changes

1. **Push to Your Fork:**
   - Push your feature branch to your GitHub fork:
     ```sh
     git push origin feature/your-feature-name
     ```

### Step 6: Create a Pull Request

1. **Open a Pull Request:**
   - Go to your forked repository on GitHub.
   - Click "Compare & pull request."
   - Select `original-repo:development` as the base and your branch as the compare.
   - Provide a descriptive title and comment about your changes.
   - Click "Create pull request."

### Step 7: Review Process

1. **Respond to Feedback:**
   - Your pull request will be reviewed. Make changes if requested and push them to your branch.
   - Update your pull request with the new changes.

2. **Merge:**
   - Once approved, your pull request will be merged into the `development` branch.

### Step 8: Syncing Your Fork

1. **Sync Your Fork with Upstream:**
   - Fetch upstream changes:
     ```sh
     git fetch upstream
     ```
   - Merge changes into your local main branch:
     ```sh
     git checkout main
     git merge upstream/main
     ```
   - Push the updated main branch to your fork:
     ```sh
     git push origin main
     ```

## Additional Resources

- [GitHub Guides](https://guides.github.com/)
- [Pro Git Book](https://git-scm.com/book/en/v2)

Thank you for contributing! If you have any questions, feel free to ask.
