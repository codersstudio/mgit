# mgit - Manage Multiple Git Repositories

`mgit` is a powerful command-line tool designed to streamline the management of multiple Git repositories simultaneously. It allows you to execute common Git commands across a configured set of repositories with a single command.

A key feature of `mgit` is its AI-powered commit message translation. When you commit your changes, `mgit` can automatically translate your commit message into English using a configured Large Language Model (LLM) like Gemini, ChatGPT, or Ollama.

## Overview

Developers often work on projects that are split across multiple repositories. `mgit` simplifies this workflow by providing a single interface to perform actions like checking status, adding files, committing, pushing, and pulling for all repositories at once. This saves time and reduces the chance of forgetting to update a repository.

## Features

- **Multi-Repo Management**: Execute Git commands across multiple repositories defined in a simple `mgit.yml` configuration file.
- **AI-Powered Commits**: Automatically translate commit messages into English before committing.
- **Supported LLMs**: Integrates with Gemini, ChatGPT, and Ollama.
- **Standard Git Commands**: Supports `init`, `status`, `log`, `pull`, `checkout`, `add`, `branch`, `commit`, and `push`.
- **Credential Management**: Uses the system's Git credential manager to securely authenticate with remotes.

## Setup

1.  **Initialize `mgit`**:
    Navigate to your root project directory containing all your Git repositories and run the `init` command.
    ```sh
    mgit init
    ```
    This will scan for all Git repositories in the subdirectories and create a `mgit.yml` file in your current directory.

2.  **Configure `mgit.yml`**:
    After initialization, you must edit the `mgit.yml` file to configure your repositories, author details, and preferred LLM provider. The file has three main sections: `llm`, `author`, and `repos`.

    - **`llm`**: Configuration for the AI service that translates commit messages.
        - `provider`: The LLM provider to use. Supported values are `Ollama`, `Gemini`, or `Chatgpt`.
        - `model`: The specific model name (e.g., `gpt-oss:20b`, `gemini-pro`).
        - `url`: The base URL for the LLM provider's API. This is primarily for self-hosted providers like Ollama (e.g., `http://localhost:11434`).
        - `apiKey`: Your API key for the selected service (if required).
    - **`author`**: Your default commit author information.
        - `name`: Your name.
        - `email`: Your email address.
    - **`repos`**: A list of absolute paths to the Git repositories you want to manage.

    **Example `mgit.yml`:**
    ```yaml
    llm:
      provider: "ollama"
      model: "gpt-oss:20b"
      url: "http://localhost:11434"
      apiKey: ""
    author:
      name: your-name
      email: your-email@example.com
    repos:
      - /path/to/your/repo1
      - /path/to/your/repo2
    ```

## Usage

All commands operate on the list of repositories defined in `mgit.yml`.

### `init`
Scans subdirectories for Git repositories and generates the `mgit.yml` file.
```sh
mgit init
```

### `status`
Shows the working tree status for all repositories.
```sh
mgit status
```

### `add`
Stages files for the next commit.
```sh
# Stage all changes
mgit add .

# Stage a specific file
mgit add path/to/file.txt
```

### `commit`
Commits the staged changes. The commit message will be automatically translated to English.
```sh
mgit commit -m "내 첫 커밋"
# The tool will translate "내 첫 커밋" to "My first commit" and use it.
```

### `push`
Pushes committed changes to the `origin` remote for all repositories.
```sh
mgit push
```

### `pull`
Pulls the latest changes from the `origin` remote for all repositories.
```sh
mgit pull
```

### `branch`
Manage branches across all repositories.
```sh
# List all branches
mgit branch

# Create a new branch in all repos
mgit branch --create <new-branch-name>

# Delete a branch in all repos
mgit branch --delete <branch-name>
```

### `checkout`
Checks out a specific branch in all repositories.
```sh
mgit checkout <branch-name>
```

### `log`
Shows the commit logs for all repositories.
```sh
# Show the most recent 10 logs
mgit log -n 10
```