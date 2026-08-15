# Batch Parameter Update

A Revit add-in that writes a text value into one instance parameter across every element the
user has selected, in a single transaction.

The command reads the current selection, asks for a parameter name and a new value, updates
every element that exposes a writable text instance parameter under that name, and reports how
many elements were updated and how many were skipped with the reason for each skip.

## Status

Work in progress. Build, installation and usage instructions are added as the project advances.

## Supported Revit versions

Revit 2023, 2024, 2025 and 2026.

## License

Proprietary. Provided for evaluation only, all rights reserved. See [LICENSE](LICENSE).
