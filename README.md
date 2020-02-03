HonYomi - abandoned

#This project is abandoned and works as-is. Any future recreation will not target Elm


Simple audiobook server written in C# (dotnet core) and Elm, for Linux, Windows, and (probably but I have no way to check) MacOS.

Scans directory paths to recursively index books/files.
Tracks which file in a book you're on, and what your progression for that book is.

*requires ffmpeg on PATH*

todo:
- full multi-user support
- better book information, scraping details from web
- better file metadata (eg. duration, ID3 tags)
- book progress logging (view progression rate, see "checkpoints" in case of mistaken seeking ruining actual listening position)
- more audio controls
- better UI
