# Technical Test - ASP.NET API for File Processing with OCR

This repository contains a technical test project where an ASP.NET API was developed to process files uploaded via HTTP requests, extract text using Tesseract OCR, and store the extracted information in a MySQL database. The processing flow and file registration are automated through HangFire, which handles asynchronous background tasks.

## Project Workflow

When a file is uploaded, it is temporarily stored in a designated folder on the server. Every minute, HangFire checks for new files to process. The files are then processed by Tesseract OCR for text extraction, and the extracted content, along with other file metadata, is saved in the database.

## Data Structure

The MySQL entity used to store the extracted data is structured as follows:

```csharp
public Guid Id { get; set; }
public long ArchiveId { get; set; }
public DateTime CreateAt { get; set; }
public DateTime ProcessAt { get; set; }
public string? Error { get; set; }
public string? ArchivePath { get; set; }
public string? Content { get; set; }

Technologies Used
ASP.NET Core: Framework for building the API.
MySQL: Relational database for storing processed data.
HangFire: Library for managing background tasks asynchronously.
Tesseract OCR: OCR tool for extracting text from files.
